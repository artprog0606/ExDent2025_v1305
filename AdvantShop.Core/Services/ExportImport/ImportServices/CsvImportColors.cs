using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using CsvHelper;
using CsvHelper.Configuration;
using Color = AdvantShop.Catalog.Color;

namespace AdvantShop.Core.Services.ExportImport
{
    public class CsvImportColors
    {
        private readonly string _fullFileName;
        private readonly ImportColorSettings _settings;

        public CsvImportColors(string fullFileName, ImportColorSettings settings)
        {
            _fullFileName = fullFileName;
            _settings = settings;
        }

        public void Process()
        {
            var hasErrors = false;
            if (_settings.UseCommonStatistic)
            {
                CommonStatistic.TotalRow += GetRowsCount(_fullFileName);
                CommonStatistic.CurrentProcessName = "Обновление иконок цветов";
                CommonStatistic.WriteLog("Запуск обновления иконок цветов");
            }
                
            using (var csvReader = InitCsvReader(_fullFileName))
            {
                if(string.IsNullOrEmpty(_settings.NameUpdatedColor))
                    while (csvReader.Read())
                    {
                        try
                        {
                            AddUpdateColor(csvReader);
                            if (_settings.UseCommonStatistic)
                                CommonStatistic.RowPosition++;
                        }
                        catch (Exception ex)
                        {
                            if (_settings.UseCommonStatistic)
                                CommonStatistic.WriteLog($"{csvReader.Parser.Row}: {ex.Message}");
                            Debug.Log.Warn(ex);
                            hasErrors = true;
                        }
                    }
                else
                    while (csvReader.Read())
                    {
                        try
                        {
                            if (UpdateColorByName(csvReader))
                                break;
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Warn(ex);
                            hasErrors = true;
                        }
                    }
            }
            if (_settings.UseCommonStatistic)
            {
                CommonStatistic.WriteLog("Конец обновления иконок цветов");
                CommonStatistic.CurrentProcessName = "Импорт завершен";
            }
            if (hasErrors)
                throw new BlException("Ошибка при импорте");
        }

        private void AddUpdateColor(CsvReader csvReader)
        {
            var colorName = (csvReader.GetField<string>("Name") ?? "").Trim();

            var color = ColorService.GetColor(colorName) ?? new Color() { ColorName = colorName };

            var isNew = color.ColorId == 0;

            if (_settings.UpdateOnlyColorWithoutCodeOrIcon && !isNew &&
                (((string.IsNullOrWhiteSpace(color.ColorCode) || color.ColorCode == "#000000") && !string.IsNullOrEmpty(color.IconFileName.PhotoName)) ||
                 (!string.IsNullOrWhiteSpace(color.ColorCode) && color.ColorCode != "#000000" && string.IsNullOrEmpty(color.IconFileName.PhotoName)))
                )
            {
                return;
            }

            color.ColorCode = csvReader.GetField<string>("Code");

            if (csvReader.HeaderRecord != null && csvReader.HeaderRecord.Contains("SortOrder"))
            {
                color.SortOrder = csvReader.GetField<int>("SortOrder");
            }

            if (isNew)
            {
                if (_settings.CreateNewColor)
                {
                    ColorService.AddColor(color);
                    if (_settings.UseCommonStatistic)
                        CommonStatistic.WriteLog($"{colorName} цвет добавлен");
                }
            }
            else
            {
                ColorService.UpdateColor(color);
                if (_settings.UseCommonStatistic)
                    CommonStatistic.WriteLog($"{colorName} цвет обновлен");
            }

            if (color.ColorId != 0)
            {
                var photo = csvReader.GetField<string>("Photo");
                if (!string.IsNullOrEmpty(photo))
                {
                    PhotoFromString(color.ColorId, photo);
                }
            }
        }


        private void PhotoFromString(int colorId, string photo)
        {
            if (photo.Contains("http://") || photo.Contains("https://"))
            {
                if (!_settings.DownloadIconByLink)
                {
                    AddColorPhotoByLink(colorId, photo);
                    return;
                }

                var uri = new Uri(photo);

                var photoName = uri.PathAndQuery.Split('?')[0].Trim('/').Replace("/", "-");
                photoName = Path.GetInvalidFileNameChars().Aggregate(photoName, (current, c) => current.Replace(c.ToString(), ""));

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

                AddColorPhoto(colorId, filename);
            }
            else
            {
                var filename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photo);

                if (!File.Exists(filename))
                    return;

                AddColorPhoto(colorId, filename);
            }
        }

        private void AddColorPhoto(int colorId, string fileName)
        {
            try
            {
                PhotoService.DeletePhotos(colorId, PhotoType.Color);

                var tempName = PhotoService.AddPhoto(new Photo(0, colorId, PhotoType.Color) { OriginName = fileName });

                if (!string.IsNullOrWhiteSpace(tempName))
                {
                    using (Image image = Image.FromFile(fileName))
                    {
                        var isRotated = FileHelpers.RotateImageIfNeed(image);

                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Catalog, tempName),
                            SettingsPictureSize.ColorIconWidthCatalog, SettingsPictureSize.ColorIconHeightCatalog, image, isRotated: isRotated);

                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Details, tempName),
                            SettingsPictureSize.ColorIconWidthDetails, SettingsPictureSize.ColorIconHeightDetails, image, isRotated: isRotated);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void AddColorPhotoByLink(int colorId, string photo)
        {
            PhotoService.DeletePhotos(colorId, PhotoType.Color);
            PhotoService.AddPhotoWithOrignName(new Photo(0, colorId, PhotoType.Color) { OriginName = photo, PhotoName = photo });
        }

        private bool UpdateColorByName(CsvReader csvReader)
        {
            var colorName = (csvReader.GetField<string>("Name") ?? "").Trim();
            if (!string.Equals(_settings.NameUpdatedColor, colorName, StringComparison.OrdinalIgnoreCase ))
                return false;
            var color = ColorService.GetColor(colorName);
            
            color.ColorCode = csvReader.GetField<string>("Code");

            ColorService.UpdateColor(color);

            var photo = csvReader.GetField<string>("Photo");
            if (!string.IsNullOrEmpty(photo))
                PhotoFromString(color.ColorId, photo);

            return true;
        }

        private static int GetRowsCount(string fullPath)
        {
            int count = 0;
            using (var csv = InitCsvReader(fullPath))
                while (csv.Read())
                    count++;
            return count;
        }

        private static CsvReader InitCsvReader(string fullPath)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.HasHeaderRecord = true;
            var reader = new CsvReader(new StreamReader(fullPath), csvConfiguration);
            reader.Read();
            reader.ReadHeader();
            return reader;
        }
    }


    public class ImportColorSettings
    {
        public bool CreateNewColor { get; set; }
        public bool UpdateOnlyColorWithoutCodeOrIcon { get; set; }
        public bool DownloadIconByLink { get; set; }
        public string NameUpdatedColor { get; set; }
        public bool UseCommonStatistic { get; set; }
        public ImportColorSettings()
        {
            UseCommonStatistic = UseCommonStatistic && CommonStatistic.IsRun;
        }
    }
}
