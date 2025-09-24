using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.SEO;
using CsvHelper.Configuration;
using MissingFieldException=CsvHelper.MissingFieldException;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AdvantShop.ExportImport
{
    public class CsvImportCategories
    {
        private readonly string _fullPath;
        private readonly bool _hasHeaders;

        private CsvCategoriesFieldsMapping _fieldMapping;
        private readonly string _separator;
        private readonly string _encodings;
        private readonly bool _updateNameAndDescription;
        private readonly bool _downloadRemotePhoto;
        private readonly bool _updatePhotos;
        private readonly string _modifiedBy;
        private readonly ChangedBy _changedBy;
        private bool _useCommonStatistic;
        private readonly string _propertySeparator;
        private readonly string _nameSameProductProperty;
        private readonly string _nameNotSameProductProperty;

        private readonly List<CategoryImportMapping> _categoryMapping = new List<CategoryImportMapping>();

        public CsvImportCategories(
            string filePath,
            bool hasHeaders,
            string separator,
            string encodings,
            CsvCategoriesFieldsMapping fieldMapping,
            string propertySeparator,
            string nameSameProductProperty,
            string nameNotSameProductProperty,
            bool updateNameAndDescription = true,
            bool downloadRemotePhoto = true,
            bool useCommonStatistic = true,
            bool updatePhotos = true,
            string modifiedBy = null)
        {
            _fullPath = filePath;
            _hasHeaders = hasHeaders;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _separator = separator;
            _updateNameAndDescription = updateNameAndDescription;
            _downloadRemotePhoto = downloadRemotePhoto;
            _useCommonStatistic = useCommonStatistic;
            _updatePhotos = updatePhotos;

            _modifiedBy = !string.IsNullOrEmpty(modifiedBy) ? modifiedBy : "CSV Import";
            _changedBy =
                CustomerContext.CurrentCustomer != null
                    ? new ChangedBy(_modifiedBy + " " + CustomerContext.CurrentCustomer.GetShortName()) {CustomerId = CustomerContext.CustomerId}
                    : new ChangedBy(_modifiedBy);
            _propertySeparator = propertySeparator;
            _nameSameProductProperty = nameSameProductProperty;
            _nameNotSameProductProperty = nameNotSameProductProperty;
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _separator ?? SeparatorsEnum.SemicolonSeparated.StrName();
            csvConfiguration.HasHeaderRecord = hasHeaderRecord ?? _hasHeaders;
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())), csvConfiguration);

            return reader;
        }

        public List<string[]> ReadFirstRecord()
        {
            var list = new List<string[]>();
            using (var csv = InitReader())
            {
                int count = 0;
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

        // private - чтобы нельзя было запускать импорт при уже запущеном,
        // через данный метот это не контролируется
        private void Process(Func<Category, Category> func = null)
        {
            try
            {
                _process(func);
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

        public Task<bool> ProcessThroughACommonStatistic(
            string currentProcess,
            string currentProcessName,
            Action onBeforeImportAction = null,
            Func<Category, Category> func = null,
            Action onAfterImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    if (onBeforeImportAction != null)
                        onBeforeImportAction();

                    _useCommonStatistic = true;
                    Process(func);

                    if (onAfterImportAction != null)
                        onAfterImportAction();
                },
                currentProcess,
                currentProcessName);
        }

        private void _process(Func<Category, Category> func = null)
        {
            Log("Начало импорта");

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can mapping colums");

            if (_fieldMapping.RelatedPropertiesMap.Count > 0 || _fieldMapping.SimilarPropertiesMap.Count > 0)
                GetPropertyNames();

            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());

            var postProcessing = _fieldMapping.ContainsKey(CategoryFields.ParentCategory.StrName());

            if (postProcessing)
                DoForCommonStatistic(() => CommonStatistic.TotalRow *= 2);

            ProcessRows(false, func);
            if (postProcessing)
                ProcessRows(true);

            ProductService.PreCalcProductParamsMass();
            CategoryService.RecalculateProductsCountManual();
            CategoryService.SetCategoryHierarchicallyEnabled(0);
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
            LuceneSearch.CreateAllIndexInBackground();

            CacheManager.Clean();
            FileHelpers.DeleteFilesFromImageTempInBackground();
            FileHelpers.DeleteFile(_fullPath);

            Log("Окончание импорта");
        }

        private void MapFields()
        {
            _fieldMapping = new CsvCategoriesFieldsMapping();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.Parser.Record.Length; i++)
                {
                    if (csv.Parser.Record[i] == ProductFields.None.StrName()) continue;
                    if (!_fieldMapping.ContainsKey(csv.Parser.Record[i]))
                        _fieldMapping.Add(csv.Parser.Record[i], i);
                }
            }
        }

        private void GetPropertyNames()
        {
            using (var csv = InitReader(false))
            {
                csv.Read();
                var invalidRelatedPropertiesIndexes = new List<int>();
                var invalidSimilarPropertiesIndexes = new List<int>();
                foreach (var index in _fieldMapping.RelatedPropertiesMap.Keys.ToList()) // ToList() needed, modifying dictionary values while iterating
                {
                    if (index > csv.Parser.Record.Length || csv.Parser.Record[index].IsNullOrEmpty())
                    {
                        invalidRelatedPropertiesIndexes.Add(index);
                        continue;
                    }
                    var colonIndex = csv.Parser.Record[index].IndexOf(':');
                    // if contains ":" - take last part, else - whole string
                    var name = csv.Parser.Record[index].Substring(colonIndex + 1, csv.Parser.Record[index].Length - colonIndex - 1);
                    if (name.IsNullOrEmpty())
                    {
                        invalidRelatedPropertiesIndexes.Add(index);
                        continue;
                    }
                    _fieldMapping.RelatedPropertiesMap[index] = name.SupperTrim();
                }
                foreach (var index in _fieldMapping.SimilarPropertiesMap.Keys.ToList()) // ToList() needed, modifying dictionary values while iterating
                {
                    if (index > csv.Parser.Record.Length || csv.Parser.Record[index].IsNullOrEmpty())
                    {
                        invalidSimilarPropertiesIndexes.Add(index);
                        continue;
                    }
                    var colonIndex = csv.Parser.Record[index].IndexOf(':');
                    // if contains ":" - take last part, else - whole string
                    var name = csv.Parser.Record[index].Substring(colonIndex + 1, csv.Parser.Record[index].Length - colonIndex - 1);
                    if (name.IsNullOrEmpty())
                    {
                        invalidSimilarPropertiesIndexes.Add(index);
                        continue;
                    }
                    _fieldMapping.SimilarPropertiesMap[index] = name.SupperTrim();
                }
                foreach (var index in invalidRelatedPropertiesIndexes)
                {
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsvV2.WrongPropertyHeader", (index + 1)));
                    _fieldMapping.RelatedPropertiesMap.Remove(index);
                }
                foreach (var index in invalidSimilarPropertiesIndexes)
                {
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsvV2.WrongPropertyHeader", (index + 1)));
                    _fieldMapping.SimilarPropertiesMap.Remove(index);
                }
            }
        }

        private long GetRowCount()
        {
            long count = 0;
            using (var csv = InitReader())
            {
                if (_hasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }

                while (csv.Read())
                    count++;
            }
            return count;
        }

        private void ProcessRows(bool postProcess, Func<Category, Category> func = null)
        {
            if (!File.Exists(_fullPath)) return;
            using (var csv = InitReader())
            {
                if (_hasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }

                while (csv.Read())
                {
                    if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking) && !_useCommonStatistic)
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var categoryInStrings = PrepareRow(csv);
                        if (categoryInStrings == null) continue;

                        var currentRowIndex = csv.CurrentIndex;

                        if (!postProcess)
                            UpdateInsertCategory(categoryInStrings, currentRowIndex, func);
                        else
                            PostProcess(categoryInStrings, currentRowIndex);
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

        private CsvCategoriesCategoryFields PrepareRow(IReader csv)
        {
            var categoryInStrings = new CsvCategoriesCategoryFields();

            foreach (CategoryFields field in Enum.GetValues(typeof(CategoryFields)))
            {
                try
                {
                    if (field == CategoryFields.RelatedProperties)
                    {
                        foreach (var index in _fieldMapping.RelatedPropertiesMap.Keys)
                        {
                            if (!categoryInStrings.RelatedProperties.ContainsKey(_fieldMapping.RelatedPropertiesMap[index]))
                                categoryInStrings.RelatedProperties.Add(_fieldMapping.RelatedPropertiesMap[index], GetCsvValue(csv, index));
                        }
                        continue;
                    }
                    if (field == CategoryFields.SimilarProperties)
                    {
                        foreach (var index in _fieldMapping.SimilarPropertiesMap.Keys)
                        {
                            if (!categoryInStrings.SimilarProperties.ContainsKey(_fieldMapping.SimilarPropertiesMap[index]))
                                categoryInStrings.SimilarProperties.Add(_fieldMapping.SimilarPropertiesMap[index], GetCsvValue(csv, index));
                        }
                        continue;
                    }
                    switch (field.Status())
                    {
                        case CsvFieldStatus.String:
                            GetString(field, csv, categoryInStrings);
                            break;
                        case CsvFieldStatus.StringRequired:
                            GetStringRequired(field, csv, categoryInStrings);
                            break;
                        case CsvFieldStatus.NotEmptyString:
                            GetStringNotNull(field, csv, categoryInStrings);
                            break;
                        case CsvFieldStatus.Float:
                            if (!GetDecimal(field, csv, categoryInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableFloat:
                            if (!GetNullableDecimal(field, csv, categoryInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.Int:
                            if (!GetInt(field, csv, categoryInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableInt:
                            if (!GetNullableInt(field, csv, categoryInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.DateTime:
                            if (!GetDateTime(field, csv, categoryInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableDateTime:
                            if (!GetNullableDateTime(field, csv, categoryInStrings))
                                return null;
                            break;
                    }
                }
                catch (MissingFieldException exception)
                {
                    DoForCommonStatistic(() => {
                        CommonStatistic.WriteLog($"Строка №{CommonStatistic.RowPosition}: Не валидный формат строки - пропущено поле {field.Localize()}");
                        CommonStatistic.TotalErrorRow++;
                    });
                    return null;
                }
            }
            return categoryInStrings;
        }

        private string GetCsvValue(IReaderRow row, int index)
        {
            return row[index].DefaultOrEmpty().SupperTrim();
        }

        private void UpdateInsertCategory(CsvCategoriesCategoryFields categoryInStrings, int currentRowIndex, Func<Category, Category> func = null)
        {
            try
            {
                Category category = null;

                var csvCategoryId = categoryInStrings.ContainsKey(CategoryFields.CategoryId)
                                    ? Convert.ToString(categoryInStrings[CategoryFields.CategoryId])
                                    : string.Empty;
                var externalId = categoryInStrings.ContainsKey(CategoryFields.ExternalId)
                                ? Convert.ToString(categoryInStrings[CategoryFields.ExternalId])
                                : string.Empty;

                if (!string.IsNullOrWhiteSpace(externalId))
                    category = CategoryService.GetCategoryFromDbByExternalId(externalId);

                if (category == null && !string.IsNullOrWhiteSpace(csvCategoryId))
                {
                    var categoryId = csvCategoryId.TryParseInt(true);
                    if (categoryId.HasValue)
                        category = CategoryService.GetCategoryFromDbByCategoryId(categoryId.Value);
                }

                var addingNew = category == null;

                if (addingNew)
                {
                    category = new Category
                    {
                        ExternalId = externalId,
                        DisplayStyle = ECategoryDisplayStyle.Tile,
                        Tags = null,
                        SizeChart = null
                    };
                }
                if (_updateNameAndDescription || addingNew)
                {
                    if (categoryInStrings.ContainsKey(CategoryFields.Name))
                        category.Name = categoryInStrings[CategoryFields.Name].AsString();
                    else
                        category.Name = category.Name ?? string.Empty;
                }

                category.ModifiedBy = _modifiedBy;

                if (categoryInStrings.ContainsKey(CategoryFields.Slug))
                {
                    var url = categoryInStrings[CategoryFields.Slug].AsString().IsNotEmpty()
                                      ? categoryInStrings[CategoryFields.Slug].AsString()
                                      : (category.Name != "" ? category.Name.Reduce(50) : category.CategoryId.ToString());
                    category.UrlPath = UrlService.GetAvailableValidUrl(category.CategoryId, ParamType.Category, url);
                }
                else
                {
                    var url = category.Name != "" ? category.Name.Reduce(50) : category.CategoryId.ToString();

                    category.UrlPath = category.UrlPath ?? UrlService.GetAvailableValidUrl(category.CategoryId, ParamType.Category, url);
                }

                if (categoryInStrings.ContainsKey(CategoryFields.SortOrder))
                    category.SortOrder = Convert.ToString(categoryInStrings[CategoryFields.SortOrder]).TryParseInt();

                if (categoryInStrings.ContainsKey(CategoryFields.Enabled))
                    category.Enabled = categoryInStrings[CategoryFields.Enabled].AsString().Trim().Equals("+");

                if (categoryInStrings.ContainsKey(CategoryFields.Hidden))
                    category.Hidden = categoryInStrings[CategoryFields.Hidden].AsString().Trim().Equals("+");

                if (_updateNameAndDescription || addingNew)
                {
                    if (categoryInStrings.ContainsKey(CategoryFields.BriefDescription))
                        category.BriefDescription = categoryInStrings[CategoryFields.BriefDescription].AsString();

                    if (categoryInStrings.ContainsKey(CategoryFields.Description))
                        category.Description = categoryInStrings[CategoryFields.Description].AsString();
                }

                if (categoryInStrings.ContainsKey(CategoryFields.DisplayStyle))
                {
                    Enum.TryParse(categoryInStrings[CategoryFields.DisplayStyle].AsString(), true, out ECategoryDisplayStyle style);

                    category.DisplayStyle = style;
                }

                if (categoryInStrings.ContainsKey(CategoryFields.Sorting))
                {
                    Enum.TryParse(categoryInStrings[CategoryFields.Sorting].AsString(), true, out ESortOrder sorting);

                    category.Sorting = sorting;
                }

                if (categoryInStrings.ContainsKey(CategoryFields.DisplayBrandsInMenu))
                    category.DisplayBrandsInMenu = categoryInStrings[CategoryFields.DisplayBrandsInMenu].AsString().Trim().Equals("+");

                if (categoryInStrings.ContainsKey(CategoryFields.DisplaySubCategoriesInMenu))
                    category.DisplaySubCategoriesInMenu = categoryInStrings[CategoryFields.DisplaySubCategoriesInMenu].AsString().Trim().Equals("+");

                if (categoryInStrings.ContainsKey(CategoryFields.ShowOnMainPage))
                    category.ShowOnMainPage = categoryInStrings[CategoryFields.ShowOnMainPage].AsString().Trim().Equals("+");

                if (categoryInStrings.ContainsKey(CategoryFields.SizeChart))
                {
                    var name = categoryInStrings[CategoryFields.SizeChart].AsString();
                    if (name.IsNotEmpty())
                        category.SizeChart = SizeChartService.GetByName(name);
                }

                var meta = (!addingNew ? MetaInfoService.GetMetaInfo(category.CategoryId, MetaType.Category) : null) 
                           ?? new MetaInfo() { ObjId = category.CategoryId, Type = MetaType.Category };

                if (categoryInStrings.ContainsKey(CategoryFields.Title) 
                    && string.IsNullOrEmpty(categoryInStrings[CategoryFields.Title].AsString())
                    || categoryInStrings.ContainsKey(CategoryFields.H1) 
                    && string.IsNullOrEmpty(categoryInStrings[CategoryFields.H1].AsString()))
                {
                    category.Meta = meta;
                }
                else
                {
                    category.Meta = new MetaInfo(0, category.CategoryId, MetaType.Category,
                        (categoryInStrings.ContainsKey(CategoryFields.Title) && categoryInStrings[CategoryFields.Title].AsString().IsNotEmpty()
                            ? categoryInStrings[CategoryFields.Title].AsString()
                            : meta.Title).DefaultOrEmpty(),
                        (categoryInStrings.ContainsKey(CategoryFields.MetaKeywords)
                            ? categoryInStrings[CategoryFields.MetaKeywords].AsString()
                            : meta.MetaKeywords).DefaultOrEmpty(),
                        (categoryInStrings.ContainsKey(CategoryFields.MetaDescription)
                            ? categoryInStrings[CategoryFields.MetaDescription].AsString()
                            : meta.MetaDescription).DefaultOrEmpty(),
                        (categoryInStrings.ContainsKey(CategoryFields.H1) && categoryInStrings[CategoryFields.H1].AsString().IsNotEmpty()
                            ? categoryInStrings[CategoryFields.H1].AsString()
                            : meta.H1).DefaultOrEmpty());
                }

                if (func != null)
                    category = func(category);

                if (!addingNew)
                {
                    CategoryService.UpdateCategory(category, false, true, _changedBy);
                    DoForCommonStatistic(() => CommonStatistic.TotalUpdateRow++);
                    Log("категория обновлена " + category.Name);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_EditCategory_Csv);
                }
                else
                {
                    category.CategoryId = CategoryService.AddCategory(category, false, false, true, _changedBy);
                    DoForCommonStatistic(() => CommonStatistic.TotalAddRow++);
                    Log("категория добавлена " + category.Name);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_CategoryCreated_Csv);
                }

                if (category.CategoryId >= 0)
                {
                    var idToUse = externalId.IsNullOrEmpty() ? csvCategoryId : externalId;
                    if (idToUse.IsNotEmpty() && !_categoryMapping.Any(x => x.CsvCategoryId == idToUse))
                    {
                        _categoryMapping.Add(new CategoryImportMapping(idToUse, category.CategoryId, currentRowIndex));
                    }
                    else if (idToUse.IsNullOrEmpty())
                    {
                        _categoryMapping.Add(new CategoryImportMapping(idToUse, category.CategoryId, currentRowIndex));
                    }

                    OtherFields(categoryInStrings, category.CategoryId, category.Name, addingNew);
                }
                else
                {
                    Log("Не удалось добавить категорию: " + category.Name);
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                }
            }
            catch (Exception e)
            {
                DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                Log(CommonStatistic.RowPosition + ": " + e.Message);
            }

            categoryInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }


        private void OtherFields(CsvCategoriesCategoryFields fields, int categoryId, string categoryName, bool newCategory)
        {
            if (fields.ContainsKey(CategoryFields.Tags))
            {
                TagService.DeleteMap(categoryId, ETagType.Category);

                var i = 0;

                foreach (var tagName in fields[CategoryFields.Tags].AsString().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var tag = TagService.Get(tagName);
                    if (tag == null)
                    {
                        var tagId = TagService.Add(new Tag
                        {
                            Name = tagName,
                            Enabled = true,
                            UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Tag, tagName)
                        });
                        TagService.AddMap(categoryId, tagId, ETagType.Category, i * 10);
                    }
                    else
                    {
                        TagService.AddMap(categoryId, tag.Id, ETagType.Category, i * 10);
                    }

                    i++;
                }
            }

            if (newCategory || _updatePhotos)
            {
                if (fields.ContainsKey(CategoryFields.Picture))
                {
                    var photo = fields[CategoryFields.Picture].AsString();
                    if (!string.IsNullOrEmpty(photo))
                    {
                        PhotoFromString(photo, categoryId, categoryName, PhotoType.CategoryBig, CategoryImageType.Big);
                    }
                }

                if (fields.ContainsKey(CategoryFields.MiniPicture))
                {
                    var photo = fields[CategoryFields.MiniPicture].AsString();
                    if (!string.IsNullOrEmpty(photo))
                    {
                        PhotoFromString(photo, categoryId, categoryName, PhotoType.CategorySmall, CategoryImageType.Small);
                    }
                }

                if (fields.ContainsKey(CategoryFields.Icon))
                {
                    var photo = fields[CategoryFields.Icon].AsString();
                    if (!string.IsNullOrEmpty(photo))
                    {
                        PhotoFromString(photo, categoryId, categoryName, PhotoType.CategoryIcon, CategoryImageType.Icon);
                    }
                }
            }

            if (fields.ContainsKey(CategoryFields.PropertyGroups))
            {
                PropertyGroupService.DeleteGroupCategoriesByCategoryId(categoryId);

                foreach (var groupName in fields[CategoryFields.PropertyGroups].AsString().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var group = PropertyGroupService.Get(groupName);
                    if (group == null)
                    {
                        var groupId = PropertyGroupService.Add(new PropertyGroup() { Name = groupName });

                        PropertyGroupService.AddGroupToCategory(groupId, categoryId);
                    }
                    else
                    {
                        PropertyGroupService.AddGroupToCategory(group.PropertyGroupId, categoryId);
                    }
                }
            }

            if (fields.ContainsKey(CategoryFields.RelatedCategories))
            {
                CategoryService.DeleteRelatedCategory(categoryId, RelatedType.Related);
                foreach (var relatedCategoryId in fields[CategoryFields.RelatedCategories].AsString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    CategoryService.AddRelatedCategory(categoryId, relatedCategoryId.TryParseInt(), RelatedType.Related);
            }

            if (fields.ContainsKey(CategoryFields.SimilarCategories))
            {
                CategoryService.DeleteRelatedCategory(categoryId, RelatedType.Alternative);
                foreach (var relatedCategoryId in fields[CategoryFields.SimilarCategories].AsString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    CategoryService.AddRelatedCategory(categoryId, relatedCategoryId.TryParseInt(), RelatedType.Alternative);
            }

            if (fields.RelatedProperties != null)
            {
                CategoryService.DeleteRelatedProperties(categoryId, RelatedType.Related);
                CategoryService.DeleteRelatedPropertyValues(categoryId, RelatedType.Related);
                foreach (var name in fields.RelatedProperties.Keys)
                {
                    var property = PropertyService.GetPropertyByName(name);
                    if (property == null)
                        continue;
                    foreach (var value in fields.RelatedProperties[name].Split(_propertySeparator).Distinct())
                    {
                        if (value == _nameSameProductProperty)
                        {
                            CategoryService.AddRelatedProperty(categoryId, property.PropertyId, RelatedType.Related, true);
                            continue;
                        }
                        else if (value == _nameNotSameProductProperty)
                        {
                            CategoryService.AddRelatedProperty(categoryId, property.PropertyId, RelatedType.Related, false);
                            continue;
                        }

                        var propertyValue = PropertyService.GetPropertyValueByName(property.PropertyId, value);
                        if (propertyValue == null)
                            continue;
                        CategoryService.AddRelatedPropertyValue(categoryId, propertyValue.PropertyValueId, RelatedType.Related);
                    }
                }
            }

            if (fields.SimilarProperties != null)
            {
                CategoryService.DeleteRelatedProperties(categoryId, RelatedType.Alternative);
                CategoryService.DeleteRelatedPropertyValues(categoryId, RelatedType.Alternative);
                foreach (var name in fields.SimilarProperties.Keys)
                {
                    var property = PropertyService.GetPropertyByName(name);
                    if (property == null)
                        continue;
                    foreach (var value in fields.SimilarProperties[name].Split(_propertySeparator).Distinct())
                    {
                        if (value == _nameSameProductProperty)
                        {
                            CategoryService.AddRelatedProperty(categoryId, property.PropertyId, RelatedType.Alternative, true);
                            continue;
                        }
                        else if (value == _nameNotSameProductProperty)
                        {
                            CategoryService.AddRelatedProperty(categoryId, property.PropertyId, RelatedType.Alternative, false);
                            continue;
                        }

                        var propertyValue = PropertyService.GetPropertyValueByName(property.PropertyId, value);
                        if (propertyValue == null)
                            continue;
                        CategoryService.AddRelatedPropertyValue(categoryId, propertyValue.PropertyValueId, RelatedType.Alternative);
                    }
                }
            }
        }

        private void PostProcess(IDictionary<CategoryFields, object> fields, int currentRowIndex)
        {
            var rowPosition = CommonStatistic.RowPosition;
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);

            var csvCategoryId = fields.ContainsKey(CategoryFields.CategoryId) ? Convert.ToString(fields[CategoryFields.CategoryId]) : string.Empty;
            var externalId = fields.ContainsKey(CategoryFields.ExternalId) ? Convert.ToString(fields[CategoryFields.ExternalId]) : string.Empty;

            var idToUse = externalId.IsNullOrEmpty() ? csvCategoryId : externalId;
            if (idToUse.IsNotEmpty() && !_categoryMapping.Any(x => x.CsvCategoryId == idToUse))
                return;

            var categoryMapping = idToUse.IsNotEmpty()
                ? _categoryMapping.Find(x => x.CsvCategoryId == idToUse)
                : _categoryMapping.Find(x => x.CsvRowIndex == currentRowIndex);

            var categoryId = categoryMapping != null ? categoryMapping.CategoryId : - 1;
            
            if (!CategoryService.IsExistCategory(categoryId))
            {
                Log(rowPosition + ": " + "Category Id '" + csvCategoryId + "' not found");
                DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                return;
            }

            if (fields.ContainsKey(CategoryFields.ParentCategory))
            {
                var csvParentCategoryId = Convert.ToString(fields[CategoryFields.ParentCategory]);

                var parentCategoryMapping = _categoryMapping.Find(x => x.CsvCategoryId == csvParentCategoryId);

                var parentCategoryId = parentCategoryMapping?.CategoryId ?? csvParentCategoryId.TryParseInt(true);

                if (parentCategoryId.HasValue && CategoryService.IsExistCategory(parentCategoryId.Value) && 
                    categoryId != 0 && parentCategoryId.Value != categoryId)
                {
                    SQLDataAccess.ExecuteNonQuery(
                        "Update Catalog.Category Set ParentCategory=@ParentCategory where CategoryID = @CategoryID",
                        CommandType.Text,
                        new SqlParameter("@ParentCategory", parentCategoryId.Value),
                        new SqlParameter("@CategoryID", categoryId));
                }
                else if (categoryId != 0)
                {
                    Log(rowPosition + ": " + "Родительская категория '" + csvParentCategoryId + "' не найдена");
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                }
            }
        }

        private void PhotoFromString(string photo, int categoryId, string categoryName, PhotoType photoType, CategoryImageType imageType)
        {
            // if remote picture we must download it
            if (photo.Contains("http://") || photo.Contains("https://"))
            {
                var uri = new Uri(photo);

                if (!_downloadRemotePhoto)
                {
                    PhotoService.DeletePhotos(categoryId, photoType);
                    PhotoService.AddPhotoWithOrignName(new Photo(0, categoryId, photoType)
                    {
                        PhotoName = uri.AbsoluteUri,
                        OriginName = uri.AbsoluteUri,
                        PhotoSortOrder = 0,
                    });
                    return;
                }

                var photoname = uri.PathAndQuery.Trim('/').Replace("/", "-");
                photoname = Path.GetInvalidFileNameChars().Aggregate(photoname, (current, c) => current.Replace(c.ToString(), ""));

                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));

                if (string.IsNullOrWhiteSpace(photoname) ||
                    IsCategoryHasThisPhotoByName(categoryId, photoname, photoType) ||
                    !FileHelpers.DownloadRemoteImageFile(photo, FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoname)))
                {
                    //if error in download proccess
                    Log("не найдено изображение для категории: " + categoryName + " , путь к файлу: " + photo);
                    DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
                    return;
                }

                photo = photoname;
            }

            photo = string.IsNullOrEmpty(photo) ? photo : photo.Trim();

            // temp picture folder
            var fullfilename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photo);

            if (!File.Exists(fullfilename))
                return;

            PhotoService.DeletePhotos(categoryId, photoType);

            var tempName = PhotoService.AddPhoto(new Photo(0, categoryId, photoType) { OriginName = photo });
            if (!string.IsNullOrWhiteSpace(tempName))
            {
                using (var image = Image.FromFile(fullfilename))
                {
                    FileHelpers.SaveCategoryImage(tempName, image, imageType);
                }
            }
        }

        private static bool IsCategoryHasThisPhotoByName(int categoryId, string originName, PhotoType photoType)
        {
            var name = SQLDataAccess.ExecuteScalar<string>(
                    "select top 1 PhotoName from Catalog.Photo where ObjID=@categoryId and OriginName=@originName and type=@type",
                    CommandType.Text,
                    new SqlParameter("@categoryId", categoryId),
                    new SqlParameter("@originName", originName),
                    new SqlParameter("@type", photoType.ToString()));

            return name.IsNotEmpty();
        }


        #region Help methods

        private void GetString(CategoryFields rEnum, IReaderRow csv, CsvCategoriesCategoryFields categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (_fieldMapping.ContainsKey(nameField))
                categoryInStrings.Add(rEnum, TrimAnyWay(csv[_fieldMapping[nameField]]));
        }

        private void GetStringNotNull(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) 
                return;
            
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                categoryInStrings.Add(rEnum, tempValue);
        }

        private void GetStringRequired(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) 
                return;
            
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                categoryInStrings.Add(rEnum, tempValue);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
            }
        }

        private bool GetDecimal(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, 0f);
                return true;
            }

            if (float.TryParse(value, out var decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, default(float?));
                return true;
            }

            if (float.TryParse(value, out var decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                categoryInStrings.Add(rEnum, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            
            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, 0);
                return true;
            }

            if (int.TryParse(value, out var intValue))
            {
                categoryInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }
        
        private bool GetNullableInt(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, default(int?));
                return true;
            }

            if (int.TryParse(value, out var intValue))
            {
                categoryInStrings.Add(rEnum, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, default(DateTime)); //value = default(DateTime).ToString(CultureInfo.InvariantCulture);
                return true;
            }
                
            if (DateTime.TryParse(value, out var dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(CategoryFields rEnum, IReaderRow csv, IDictionary<CategoryFields, object> categoryInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var value = TrimAnyWay(csv[_fieldMapping[nameField]]);

            if (string.IsNullOrEmpty(value))
            {
                categoryInStrings.Add(rEnum, default(DateTime?));
                return true;
            }

            if (DateTime.TryParse(value, out var dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                categoryInStrings.Add(rEnum, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), rEnum.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
        }

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
                CommonStatistic.RowPosition++;
            });
        }

        private void Log(string message)
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog(message));
        }

        private void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        #endregion
    }

    public class CategoryImportMapping
    {
        /// <summary>
        /// categoryId in csv
        /// </summary>
        public string CsvCategoryId { get; set; }

        /// <summary>
        /// categoryId in db
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// position in csv file
        /// </summary>
        public long CsvRowIndex { get; set; }

        public CategoryImportMapping(string csvCategoryId, int categoryId, long csvRowIndex)
        {
            CsvCategoryId = csvCategoryId;
            CategoryId = categoryId;
            CsvRowIndex = csvRowIndex;
        }
    }

    public class CsvCategoriesFieldsMapping : Dictionary<string, int>
    {
        public CsvCategoriesFieldsMapping() : base()
        {
            RelatedPropertiesMap = new Dictionary<int, string>();
            SimilarPropertiesMap = new Dictionary<int, string>();
        }

        /// <summary>
        /// key: index of column in csv; value: property name
        /// </summary>
        public Dictionary<int, string> RelatedPropertiesMap { get; set; }

        /// <summary>
        /// key: index of column in csv; value: property name
        /// </summary>
        public Dictionary<int, string> SimilarPropertiesMap { get; set; }

        public void AddField(string field, int index)
        {
            if (field.IsNullOrEmpty())
                return;
            if (field == CategoryFields.RelatedProperties.StrName() && !RelatedPropertiesMap.ContainsKey(index))
                RelatedPropertiesMap.Add(index, string.Empty);
            else if (field == CategoryFields.SimilarProperties.StrName() && !SimilarPropertiesMap.ContainsKey(index))
                SimilarPropertiesMap.Add(index, string.Empty);
            else if (!ContainsKey(field))
                Add(field, index);
        }
    }

    public class CsvCategoriesCategoryFields : Dictionary<CategoryFields, object>
    {
        public CsvCategoriesCategoryFields() : base()
        {
            RelatedProperties = new Dictionary<string, string>();
            SimilarProperties= new Dictionary<string, string>();
        }

        /// <summary>
        /// key: property name; value: property values in string
        /// </summary>
        public Dictionary<string, string> RelatedProperties { get; set; }

        /// <summary>
        /// key: property name; value: property values in string
        /// </summary>
        public Dictionary<string, string> SimilarProperties { get; set; }
    }
}