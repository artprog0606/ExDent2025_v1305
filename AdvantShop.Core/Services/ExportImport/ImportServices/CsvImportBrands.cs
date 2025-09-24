using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Statistic;
using CsvHelper;
using MissingFieldException=CsvHelper.MissingFieldException;

namespace AdvantShop.Core.Services.ExportImport.ImportServices
{
    public class CsvImportBrands
    {
        private readonly string _fullPath;
        private readonly ImportBrandsSettings _settings;

        public CsvImportBrands(string fullPath, ImportBrandsSettings settings)
        {
            _fullPath = fullPath;
            _settings = settings;
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _settings.ColumnSeparator ?? SeparatorsEnum.SemicolonSeparated.StrName();
            csvConfiguration.HasHeaderRecord = hasHeaderRecord ?? _settings.HasHeaders;
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_settings.Encodings ?? EncodingsEnum.Utf8.StrName())), csvConfiguration);

            return reader;
        }

        public List<string[]> ReadFirstRecord()
        {
            var list = new List<string[]>();
            using (var csv = InitReader())
            {
                var count = 0;
                while (csv.Read())
                {
                    if (count == 2)
                        break;

                    if (csv.Parser.Record != null)
                        list.Add(csv.Parser.Record);

                    count++;
                }
            }

            return list;
        }

        public Task<bool> ProcessThroughACommonStatistic(string currentProcess, string currentProcessName,
            Action onBeforeImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    onBeforeImportAction?.Invoke();

                    _settings.UseCommonStatistic = true;
                    Process();
                },
                currentProcess,
                currentProcessName);
        }

        public void Process()
        {
            try
            {
                _process();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.WriteLog(ex.Message);
                    CommonStatistic.TotalErrorRow++;
                });
            }
        }

        private void _process()
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog("Start of brand import"));

            if (_settings.FieldMapping == null)
                MapFields();

            if (_settings.FieldMapping == null)
                throw new Exception("can't map colums");

            DoForCommonStatistic(() => CommonStatistic.TotalRow += GetRowCount());

            ProcessRows();

            CacheManager.Clean();
            FileHelpers.DeleteFile(_fullPath);
            if (_settings.FieldMapping.ContainsKey(EBrandFields.Photo.StrName().ToLower()))
                FileHelpers.DeleteFilesFromImageTemp();

            DoForCommonStatistic(() => CommonStatistic.WriteLog("End of brand import"));
        }

        private void MapFields()
        {
            _settings.FieldMapping = new Dictionary<string, int>();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.Parser.Record.Length; i++)
                {
                    if (csv.Parser.Record[i] == ELeadFields.None.StrName()) continue;
                    if (!_settings.FieldMapping.ContainsKey(csv.Parser.Record[i]))
                        _settings.FieldMapping.Add(csv.Parser.Record[i], i);
                }
            }
        }

        private long GetRowCount()
        {
            long count = 0;
            using (var csv = InitReader())
            {
                if (_settings.HasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                while (csv.Read())
                    count++;
            }

            return count;
        }

        private void ProcessRows()
        {
            if (!File.Exists(_fullPath)) return;

            using (var csv = InitReader())
            {
                if (_settings.HasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                while (csv.Read())
                {
                    if (_settings.UseCommonStatistic && (!CommonStatistic.IsRun || CommonStatistic.IsBreaking))
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }

                    try
                    {
                        var brandInString = PrepareRow(csv);
                        if (brandInString == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        ProcessBrand(brandInString);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog($"{CommonStatistic.RowPosition}: {ex.Message}");
                            CommonStatistic.TotalErrorRow++;
                        });
                    }
                }
            }
        }

        private Dictionary<EBrandFields, object> PrepareRow(IReaderRow csv)
        {
            var brandInStrings = new Dictionary<EBrandFields, object>();

            foreach (EBrandFields field in Enum.GetValues(typeof(EBrandFields)))
            {
                try
                {
                    switch (field.Status())
                    {
                        case CsvFieldStatus.String:
                            GetString(field, csv, brandInStrings);
                            break;
                        case CsvFieldStatus.Int:
                            GetInt(field, csv, brandInStrings);
                            break;
                    }
                }
                catch (MissingFieldException)
                {
                    DoForCommonStatistic(() => {
                        CommonStatistic.WriteLog($"Строка №{CommonStatistic.RowPosition}: Не валидный формат строки - пропущено поле {field.Localize()}");
                        CommonStatistic.TotalErrorRow++;
                    });
                    return null;
                }
            }

            return brandInStrings;
        }

        private void ProcessBrand(IDictionary brandInStrings)
        {
            try
            {
                var name = brandInStrings.Contains(EBrandFields.Name)
                    ? Convert.ToString(brandInStrings[EBrandFields.Name])
                    : string.Empty;
                var urlPath = brandInStrings.Contains(EBrandFields.UrlPath)
                    ? Convert.ToString(brandInStrings[EBrandFields.UrlPath])
                    : string.Empty;

                if (string.IsNullOrWhiteSpace(name))
                {
                    DoForCommonStatistic(() => {
                        CommonStatistic.WriteLog($"Строка {CommonStatistic.RowPosition+1}: Производитель не может быть добавлен/обновлен - пустое поле {EBrandFields.Name.Localize()}");
                        CommonStatistic.TotalErrorRow++;
                        CommonStatistic.RowPosition++;
                    });
                    return;
                }

                Brand brand = null;
                if (!string.IsNullOrEmpty(name))
                    brand = BrandService.GetBrandByName(name);
                
                if (brand == null)
                    brand = new Brand
                    {
                        Name = name,
                        UrlPath = urlPath
                    };

                if (string.IsNullOrEmpty(brand.UrlPath))
                    brand.UrlPath = UrlService.GetAvailableValidUrl(brand.BrandId, ParamType.Brand, brand.Name.Reduce(50));

                var startDescriptionIsEmpty = brand.Description.IsNullOrEmpty();
                var startBriefDescriptionIsEmpty = brand.BriefDescription.IsNullOrEmpty();

                if (brandInStrings.Contains(EBrandFields.Description))
                    brand.Description = Convert.ToString(brandInStrings[EBrandFields.Description]);
                
                if (brandInStrings.Contains(EBrandFields.BriefDescription))
                    brand.BriefDescription = Convert.ToString(brandInStrings[EBrandFields.BriefDescription]);
               
                if (brandInStrings.Contains(EBrandFields.Enabled))
                {
                    var enabledAsString = brandInStrings[EBrandFields.Enabled].AsString();
                    if (!string.IsNullOrEmpty(enabledAsString))
                    {
                        enabledAsString = enabledAsString.Trim().ToLower();

                        brand.Enabled = enabledAsString.Equals("+") || enabledAsString.Equals("true");
                    }
                }

                if (brandInStrings.Contains(EBrandFields.BrandSiteUrl))
                    brand.BrandSiteUrl = Convert.ToString(brandInStrings[EBrandFields.BrandSiteUrl]);

                if (brandInStrings.Contains(EBrandFields.CountryId))
                {
                    var countryId = Convert.ToInt32(brandInStrings[EBrandFields.CountryId]);
                    var country = CountryService.GetCountry(countryId);
                    if (country != null)
                        brand.CountryId = country.CountryId;
                }

                if (brand.CountryId.IsDefault() && brandInStrings.Contains(EBrandFields.CountryName))
                {
                    var countryName = Convert.ToString(brandInStrings[EBrandFields.CountryName]);
                    var country = CountryService.GetCountryByName(countryName);
                    if (country != null)
                        brand.CountryId = country.CountryId;
                }

                if (brandInStrings.Contains(EBrandFields.CountryOfManufactureId))
                {
                    var countryOfManufactureId = Convert.ToInt32(brandInStrings[EBrandFields.CountryOfManufactureId]);
                    var countryOfManufacture = CountryService.GetCountry(countryOfManufactureId);
                    if (countryOfManufacture != null)
                        brand.CountryOfManufactureId = countryOfManufacture.CountryId;
                }

                if (brand.CountryOfManufactureId.IsDefault() && brandInStrings.Contains(EBrandFields.CountryOfManufactureName))
                {
                    var countryOfManufactureName = brandInStrings[EBrandFields.CountryOfManufactureName].AsString();
                    if (!string.IsNullOrEmpty(countryOfManufactureName))
                    {
                        var countryOfManufacture = CountryService.GetCountryByName(countryOfManufactureName);
                        if (countryOfManufacture != null)
                            brand.CountryOfManufactureId = countryOfManufacture.CountryId;
                    }
                }

                var isAdded = false;
                var isUpdated = false;
                if (brand.ID.IsDefault())
                {
                    if (_settings.ImportMode.HasFlag(ImportMode.Adding))
                    {
                        BrandService.AddBrand(brand);
                        isAdded = true;
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.TotalAddRow++;
                            CommonStatistic.WriteLog($"Производитель добавлен: {brand.ID} - {brand.Name}");
                        });
                    }
                }
                else if (_settings.ImportMode.HasFlag(ImportMode.Updating))
                {
                    if (_settings.UpdateOnlyByEmptyDescription is false || (startDescriptionIsEmpty && startBriefDescriptionIsEmpty))
                    {
                        BrandService.UpdateBrand(brand);
                        isUpdated = true;
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.TotalUpdateRow++;
                            CommonStatistic.WriteLog($"Производитель обновлен: {brand.ID} - {brand.Name}");
                        });
                    }
                }

                if (isAdded || isUpdated)
                {
                    if (brand.ID != 0 && brandInStrings.Contains(EBrandFields.Photo))
                    {
                        PhotoFromString(brand.ID, Convert.ToString(brandInStrings[EBrandFields.Photo]));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.TotalErrorRow++;
                    CommonStatistic.WriteLog(CommonStatistic.RowPosition + ": " + ex.Message);
                });
            }

            brandInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        private void PhotoFromString(int brandId, string photo)
        {
            if (photo.Contains("http://") || photo.Contains("https://"))
            {
                var uri = new Uri(photo);

                var photoName = uri.PathAndQuery.Split('?')[0].Trim('/').Replace("/", "-");
                photoName = Path.GetInvalidFileNameChars()
                    .Aggregate(photoName, (current, c) => current.Replace(c.ToString(), ""));

                if (photoName.Contains("."))
                {
                    var fileExtension = photoName.Substring(photoName.LastIndexOf('.'));
                    if (!FileHelpers.CheckFileExtensionByType(fileExtension, EFileType.Image))
                        return;
                }

                if (photoName.Length > 100)
                {
                    photoName = photoName.Length - 245 > 0
                        ? photoName.Substring(photoName.Length - 245)
                        : photoName;
                }

                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));

                var filename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                if (!FileHelpers.DownloadRemoteImageFile(photo, filename))
                    return;

                AddBrandPhoto(brandId, filename);
            }
            else
            {
                var filename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photo);

                if (!File.Exists(filename))
                    return;

                AddBrandPhoto(brandId, filename);
            }
        }

        private void AddBrandPhoto(int brandId, string filename)
        {
            try
            {
                PhotoService.DeletePhotos(brandId, PhotoType.Brand);

                var photo = new Photo(0, brandId, PhotoType.Brand) {OriginName = filename};
                var tempName = PhotoService.AddPhoto(photo);

                if (string.IsNullOrWhiteSpace(tempName)) return;

                using (var image = Image.FromFile(filename))
                {
                    FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, tempName),
                        SettingsPictureSize.BrandLogoWidth, SettingsPictureSize.BrandLogoHeight, image);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        #region Help methods

        private void GetString(EBrandFields rEnum, IReaderRow csv, IDictionary<EBrandFields, object> brandInStrings)
        {
            var nameField = rEnum.StrName().ToLower();
            if (_settings.FieldMapping.ContainsKey(nameField))
                brandInStrings.Add(rEnum, TrimAnyWay(csv[_settings.FieldMapping[nameField]]));
        }

        private void GetInt(EBrandFields rEnum, IReaderRow csv, IDictionary<EBrandFields, object> brandInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_settings.FieldMapping.ContainsKey(nameField)) return;
            var value = TrimAnyWay(csv[_settings.FieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
                value = "0";
            if (int.TryParse(value, out var intValue))
            {
                brandInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"),
                    rEnum.Localize(), CommonStatistic.RowPosition + 2));
            }
        }

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
            });
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
        }

        private void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_settings.UseCommonStatistic)
                commonStatisticAction();
        }

        #endregion Help methods
    }
    
    public class ImportBrandsSettings
    {
        public bool HasHeaders { get; set; }
        public string ColumnSeparator { get; set; }
        public string Encodings { get; set; }
        public Dictionary<string, int> FieldMapping { get; set; }
        public bool UseCommonStatistic { get; set; } = true;
        public ImportMode ImportMode { get; set; }
        public bool UpdateOnlyByEmptyDescription { get; set; }
    }

    [Flags]
    public enum ImportMode : byte
    {
        None = 0b0,
        Adding = 0b1 << 0,
        Updating = 0b1 << 1,
        Full = Adding | Updating
    }
}
