using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Statistic;
using AdvantShop.Taxes;
using CsvHelper;
using MissingFieldException=CsvHelper.MissingFieldException;

namespace AdvantShop.ExportImport
{
    /// <summary>
    /// Key: field type (EProductField.ToSrting() or moduleField.StrName; 
    /// value: index of column in csv
    /// </summary>
    public class CsvV2FieldsMapping : Dictionary<string, int>
    {
        public CsvV2FieldsMapping() : base()
        {
            CategoriesMap = new List<int>();
            CategorySortingMap = new List<int>();
            PropertiesMap = new Dictionary<int, string>();
            PriceRulesMap = new Dictionary<int, string>();
            WarehouseStocksMap = new Dictionary<int, string>();
        }

        /// <summary>
        /// index of column in csv
        /// </summary>
        public List<int> CategoriesMap { get; set; }
        
        /// <summary>
        /// index of column in csv
        /// </summary>
        public List<int> CategorySortingMap { get; set; }
        
        /// <summary>
        /// key: index of column in csv; value: property name
        /// </summary>
        public Dictionary<int, string> PropertiesMap { get; set; }

        /// <summary>
        /// key: index of column in csv; value: price rule name
        /// </summary>
        public Dictionary<int, string> PriceRulesMap { get; set; }

        /// <summary>
        /// key: index of column in csv; value: warehouse name
        /// </summary>
        public Dictionary<int, string> WarehouseStocksMap { get; set; }

        public void AddField(string field, int index)
        {
            if (field.IsNullOrEmpty())
                return;
            if (field == EProductField.Category.ToString())
                CategoriesMap.Add(index);
            else if (field == EProductField.Sorting.ToString())
                CategorySortingMap.Add(index);
            else if (field == EProductField.Property.ToString())
            {
                if (!PropertiesMap.ContainsKey(index))
                    PropertiesMap.Add(index, string.Empty);
            }
            else if (field == EProductField.PriceRule.ToString())
            {
                if (!PriceRulesMap.ContainsKey(index))
                    PriceRulesMap.Add(index, string.Empty);
            }
            else if (field == EProductField.WarehouseStock.ToString())
            {
                if (!WarehouseStocksMap.ContainsKey(index))
                    WarehouseStocksMap.Add(index, string.Empty);
            }
            else if (!ContainsKey(field))
                Add(field, index);
        }
    }

    public class CsvV2ProductFields : Dictionary<EProductField, object>
    {
        public CsvV2ProductFields() : base()
        {
            Categories = new List<string>();
            CategoriesSorting = new List<string>();
            Properties = new Dictionary<string, string>();
            PriceRulePrices = new List<CsvV2ProductFieldPriceRulePrice>();
            WarehouseStocks = new List<CsvV2ProductFieldWarehouseStocks>();
        }

        public List<string> Categories { get; set; }
        public List<string> CategoriesSorting { get; set; }
        
        /// <summary>
        /// key: property name; value: product property value
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
        
        public List<CsvV2ProductFieldPriceRulePrice> PriceRulePrices { get; set; }
        public List<CsvV2ProductFieldWarehouseStocks> WarehouseStocks { get; set; }
    }

    public class CsvV2ProductFieldPriceRulePrice
    {
        public string Artno { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }
    }

    public class CsvV2ProductFieldWarehouseStocks
    {
        public string Artno { get; set; }
        public string Name { get; set; }
        public float? Quantity { get; set; }
    }

    public class CsvImportV2
    {
        private readonly string _fullPath;
        private readonly EImportProductActionType _actionWithProducts;
        private readonly bool _hasHeader;
        private readonly bool _skipOriginalPhoto;
        private readonly string _separator;
        private readonly string _encoding;
        private readonly string _columnSeparator;
        private readonly string _propertySeparator;
        private readonly bool _importRemains;
        private readonly bool _updatePhotos;
        private readonly bool _updateNameAndDescription;
        private readonly bool _downloadRemotePhoto;
        private readonly bool _onlyUpdateProducts;
        private readonly bool _trackChanges;
        private readonly bool _enabled301Redirects;
        private readonly bool _addProductInParentCategory;
        private readonly string _modifiedBy;
        private readonly ChangedBy _changedBy;
        private readonly List<int> _addedColorIds;

        private CsvV2FieldsMapping _fieldMapping;
        private readonly Dictionary<ICSVExportImport, List<CSVField>> _modulesAndFields;

        private bool _useCommonStatistic;
        private bool _useMassPrecalc;

        private List<Warehouse> _warehouses;

        private long _rowsCount;
        private long _currentRow;

        private CsvImportV2(
            string filePath, 
            bool hasHeader, 
            EImportProductActionType actionWithProducts, 
            string separator, 
            string encoding,
            CsvV2FieldsMapping fieldMapping, 
            string columSeparator, 
            string propertySeparator, 
            bool skipOriginalPhoto, 
            bool importRemains, 
            bool updatePhotos, 
            bool updateNameAndDescription, 
            bool downloadRemotePhoto,
            bool useCommonStatistic,
            bool onlyUpdateProducts,
            bool trackChanges,
            string modifiedBy,
            bool enabled301Redirects,
            bool addProductInParentCategory)
        {
            _fullPath = filePath;
            _hasHeader = hasHeader;
            _actionWithProducts = actionWithProducts;
            _fieldMapping = fieldMapping;
            _encoding = encoding;
            _separator = separator;
            _columnSeparator = columSeparator;
            _propertySeparator = propertySeparator;
            _skipOriginalPhoto = skipOriginalPhoto;
            _importRemains = importRemains;
            _updatePhotos = updatePhotos;
            _updateNameAndDescription = updateNameAndDescription;
            _downloadRemotePhoto = downloadRemotePhoto;
            _useCommonStatistic = useCommonStatistic;
            _onlyUpdateProducts = onlyUpdateProducts;
            _trackChanges = trackChanges;
            _modifiedBy = !string.IsNullOrEmpty(modifiedBy) ? modifiedBy : "CSV Import" ;
            _changedBy =
                CustomerContext.CurrentCustomer != null
                    ? new ChangedBy(_modifiedBy + " " + CustomerContext.CurrentCustomer.GetShortName()) {CustomerId = CustomerContext.CustomerId}
                    : new ChangedBy(_modifiedBy);

            _modulesAndFields = new Dictionary<ICSVExportImport, List<CSVField>>();
            foreach (var csvExportImportModule in AttachedModules.GetModules<ICSVExportImport>())
            {
                var classInstance = (ICSVExportImport)Activator.CreateInstance(csvExportImportModule, null);
                if (ModulesRepository.IsActiveModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                {
                    var moduleFields = classInstance.GetCSVFields().ToList();
                    if (moduleFields.Any() && !_modulesAndFields.ContainsKey(classInstance))
                        _modulesAndFields.Add(classInstance, moduleFields);
                }
            }
            _enabled301Redirects = enabled301Redirects;
            _addProductInParentCategory = addProductInParentCategory;
            _addedColorIds = new List<int>();
        }

        public static CsvImportV2 Factory(
            string filePath,
            bool hasHeader,
            EImportProductActionType actionWithProducts,
            string separator,
            string encoding,
            CsvV2FieldsMapping fieldMapping,
            string columSeparator,
            string propertySeparator,
            bool skipOriginalPhoto = false,
            bool remains = false,
            bool updatePhotos = false,
            bool updateNameAndDescription = true,
            bool downloadRemotePhoto = true,
            bool useCommonStatistic = true,
            bool onlyUpdateProducts = false,
            bool trackChanges = false,
            string modifiedBy = null,
            bool enabled301Redirects = false,
            bool addProductInParentCategory = false)
        {
            return new CsvImportV2(filePath, hasHeader, actionWithProducts, separator, encoding, fieldMapping, columSeparator, propertySeparator,
                                   skipOriginalPhoto, remains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, useCommonStatistic, 
                                   onlyUpdateProducts, trackChanges, modifiedBy, enabled301Redirects, addProductInParentCategory);
        }

        public static CsvImportV2 Factory(
            string filePath, 
            bool hasHeader, 
            EImportProductActionType actionWithProducts,
            bool skipOriginalPhoto = false, 
            bool remains = false, 
            bool updatePhotos = false, 
            bool updateNameAndDescription = true, 
            bool downloadRemotePhoto = true,
            bool useCommonStatistic = true,
            bool onlyUpdateProducts = false)
        {
            return new CsvImportV2(filePath, hasHeader, actionWithProducts, null, null, null, null, null, 
                                   skipOriginalPhoto, remains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, useCommonStatistic, 
                                   onlyUpdateProducts, false, null, false, false);
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _separator ?? SeparatorsEnum.SemicolonSeparated.StrName();
            csvConfiguration.HasHeaderRecord = hasHeaderRecord ?? _hasHeader;
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encoding ?? EncodingsEnum.Utf8.StrName())), csvConfiguration);

            return reader;
        }

        public List<string[]> ReadFirst2()
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

        private void Process(Func<Product, Product> func = null, Action onBeforeMassActions = null)
        {
            try
            {
                _process(func, onBeforeMassActions);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
            DoForCommonStatistic(() => CommonStatistic.CurrentProcessName = LocalizationService.GetResource("Admin.ImportCsvV2.EndProcessName"));
        }

        public Task<bool> ProcessThroughACommonStatistic(
            string currentProcess, 
            string currentProcessName,
            Action onBeforeImportAction = null,
            Func<Product, Product> func = null,
            Action onBeforeMassActions = null,
            Action onAfterImportAction = null)
        {
            return CommonStatistic.StartNew(() =>
                {
                    if (onBeforeImportAction != null)
                        onBeforeImportAction();

                    _useCommonStatistic = true;
                    Process(func, onBeforeMassActions);

                    if (onAfterImportAction != null)
                        onAfterImportAction();
                },
                currentProcess,
                currentProcessName);
        }

        private void _process(Func<Product, Product> func = null, Action onBeforeMassActions = null)
        {
            Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.StartImport"));

            if (_fieldMapping == null)
                MapFields();

            if (_fieldMapping == null)
                throw new Exception("can't map colums");

            if (_fieldMapping.PropertiesMap.Any())
                GetPropertyNames();

            if (_fieldMapping.PriceRulesMap.Count > 0)
                GetPriceRuleNames();

            if (_fieldMapping.WarehouseStocksMap.Count > 0)
                GetWarehouseNames();

            var startAt = DateTime.Now;

            var somePostProcessing = _fieldMapping.ContainsKey(EProductField.Related.ToString()) ||
                                     _fieldMapping.ContainsKey(EProductField.Alternative.ToString()) ||
                                     _fieldMapping.ContainsKey(EProductField.Gifts.ToString()) ||
                                     _modulesAndFields.Values.Any(moduleFields => moduleFields.Any(moduleField => _fieldMapping.ContainsKey(moduleField.StrName)));

            _rowsCount = GetRowCount();
            DoForCommonStatistic(() => CommonStatistic.TotalRow = _rowsCount * (somePostProcessing ? 2 : 1));

            ProcessRows(false, func);
            if (somePostProcessing)
                ProcessRows(true);

            if (onBeforeMassActions != null)
                onBeforeMassActions();

            if (_actionWithProducts == EImportProductActionType.Disable)
            {
                Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.DisablingProducts"));
                ProductService.DisableAllProducts(startAt);
            }
            
            if (_actionWithProducts == EImportProductActionType.Delete)
            {
                Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.DisablingProducts"));
                ProductService.DeleteAllProducts(startAt);
            }
            
            if (_actionWithProducts == EImportProductActionType.ResetToZero)
            {
                Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.ResetToZero"));
                ProductService.ResetToZeroAllProducts(startAt);
            }

            CategoryService.RecalculateProductsCountManual();
            CategoryService.SetCategoryHierarchicallyEnabled(0);
            CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
            LuceneSearch.CreateAllIndexInBackground();

            ColorService.UpdateUnSetColorsFromAdvantshopCsv(true);
            
            // PreCalcProductParamsMass was moved to adding/updating each product
            // but for gifts and other updates, we use it at the end
            if (_useMassPrecalc)
                ProductService.PreCalcProductParamsMassInBackground();

            if (!_useMassPrecalc && _addedColorIds.Count > 0)
            {
                foreach (var colorId in _addedColorIds)
                    ProductService.PreCalcProductColors(colorId);
            }
            
            CacheManager.Clean();
            FileHelpers.DeleteFilesFromImageTempInBackground();
            FileHelpers.DeleteFile(_fullPath);

            Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.ImportCompleted"));
        }

        private void MapFields()
        {
            var fieldNames = new Dictionary<string, string>();
            foreach (EProductField item in Enum.GetValues(typeof(EProductField)))
                fieldNames.TryAddValue(item.Localize(), item.ToString());
            foreach (var moduleField in _modulesAndFields.Values.SelectMany(moduleFields => moduleFields))
            {
                if (fieldNames.ContainsKey(moduleField.DisplayName))
                    continue;
                fieldNames.Add(moduleField.DisplayName, moduleField.StrName);
            }

            _fieldMapping = new CsvV2FieldsMapping();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.Parser.Record.Length; i++)
                {
                    var header = csv.Parser.Record[i];
                    if (header.IsNullOrEmpty() || header == EProductField.None.ToString())
                        continue;
                    var identPart = header.Split(':').FirstOrDefault();
                    var field = fieldNames.ContainsKey(header)
                        ? fieldNames[header]
                        : fieldNames.ElementOrDefault(identPart, null);
                    _fieldMapping.AddField(field, i);
                }
            }
        }

        private void GetPropertyNames()
        {
            using (var csv = InitReader(false))
            {
                csv.Read();
                var invalidIndexes = new List<int>();
                foreach (var index in _fieldMapping.PropertiesMap.Keys.ToList()) // ToList() needed, modifying dictionary values while iterating
                {
                    if (index > csv.Parser.Record.Length || csv.Parser.Record[index].IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    var colonIndex = csv.Parser.Record[index].IndexOf(':');
                    // if contains ":" - take last part, else - whole string
                    var name = csv.Parser.Record[index].Substring(colonIndex + 1, csv.Parser.Record[index].Length - colonIndex - 1);
                    if (name.IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    _fieldMapping.PropertiesMap[index] = name.SupperTrim();
                }
                foreach (var index in invalidIndexes)
                {
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsvV2.WrongPropertyHeader", (index + 1)));
                    _fieldMapping.PropertiesMap.Remove(index);
                }
            }
        }

        private void GetPriceRuleNames()
        {
            var priceRules = PriceRuleService.GetList(false);
            
            using (var csv = InitReader(false))
            {
                csv.Read();
                var invalidIndexes = new List<int>();
                foreach (var index in _fieldMapping.PriceRulesMap.Keys.ToList()) // ToList() needed, modifying dictionary values while iterating
                {
                    if (index > csv.Parser.Record.Length || csv.Parser.Record[index].IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    var colonIndex = csv.Parser.Record[index].IndexOf(':');
                    // if contains ":" - take last part, else - whole string
                    var name = csv.Parser.Record[index].Substring(colonIndex + 1, csv.Parser.Record[index].Length - colonIndex - 1);
                    if (name.IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    
                    name = name.SupperTrim();

                    var rule = priceRules.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (rule == null)
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    
                    _fieldMapping.PriceRulesMap[index] = name;
                }
                foreach (var index in invalidIndexes)
                {
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsvV2.WrongPriceRuleHeader", (index + 1)));
                    _fieldMapping.PriceRulesMap.Remove(index);
                }
            }
        }

        private void GetWarehouseNames()
        {
            _warehouses = WarehouseService.GetList();

            using (var csv = InitReader(false))
            {
                csv.Read();
                var invalidIndexes = new List<int>();
                foreach (var index in _fieldMapping.WarehouseStocksMap.Keys.ToList()) // ToList() needed, modifying dictionary values while iterating
                {
                    if (index > csv.Parser.Record.Length || csv.Parser.Record[index].IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    var colonIndex = csv.Parser.Record[index].IndexOf(':');
                    // if contains ":" - take last part, else - whole string
                    var name = csv.Parser.Record[index].Substring(colonIndex + 1);
                    if (name.IsNullOrEmpty())
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    
                    name = name.SupperTrim();

                    var warehouse = _warehouses.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (warehouse == null)
                    {
                        invalidIndexes.Add(index);
                        continue;
                    }
                    
                    _fieldMapping.WarehouseStocksMap[index] = name;
                }
                foreach (var index in invalidIndexes)
                {
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsvV2.WrongWarehouseStocksHeader", (index + 1)));
                    _fieldMapping.WarehouseStocksMap.Remove(index);
                }
            }
        }

        private long GetRowCount()
        {
            long count = 0;
            using (var csv = InitReader())
            {
                if (_hasHeader)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                while (csv.Read())
                    count++;
            }
            return count;
        }

        private void ProcessRows(bool onlyPostProcess, Func<Product, Product> func = null)
        {
            if (!File.Exists(_fullPath)) return;
            using (var csv = InitReader())
            {
                if (_hasHeader)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                // product row and offers rows
                var rows = new List<CsvV2ProductFields>();
                Dictionary<CSVField, object> modulesFields = null;
                // group rows by product artno
                string artNo = null;
                for (int i = 0; i < _rowsCount + 1; i++)
                {
                    if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking) && _useCommonStatistic)
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }

                    _currentRow = i + (_hasHeader ? 2 : 1);

                    var isEnd = !csv.Read();
                    if (!isEnd)
                        DoForCommonStatistic(() => CommonStatistic.RowPosition++);

                    var currentArtNo = !isEnd && _fieldMapping.ContainsKey(EProductField.Code.ToString())
                        ? GetCsvValue(csv, _fieldMapping[EProductField.Code.ToString()])
                        : null;

                    // mapping contains ArtNo && product row is valid && current row is offer row
                    if (currentArtNo.IsNotEmpty() && rows.Any() && artNo == currentArtNo)
                    {
                        var rowFields = PrepareRow(csv, onlyOffer: true, onlyPostProcess: onlyPostProcess);
                        if (rowFields == null)
                            continue;
                        rows.Add(rowFields);
                    }

                    // first product row or next product row
                    if (currentArtNo.IsNullOrEmpty() || currentArtNo != artNo)
                    {
                        if (rows.Any())
                        {
                            try
                            {
                                if (!onlyPostProcess)
                                    UpdateInsertProduct(rows, func);
                                else
                                    PostProcess(rows, modulesFields, _modulesAndFields);
                            }
                            catch (Exception ex)
                            {
                                DoForCommonStatistic(() =>
                                {
                                    CommonStatistic.WriteLog(string.Format("{0}: {1}", _currentRow, ex.Message));
                                    CommonStatistic.TotalErrorRow++;
                                });
                            }
                        }

                        artNo = currentArtNo;
                        rows = new List<CsvV2ProductFields>();
                        if (isEnd)
                            break;
                        // get first or next product fields
                        var rowFields = PrepareRow(csv, onlyPostProcess: onlyPostProcess);
                        if (rowFields != null)
                            rows.Add(rowFields);
                        if (onlyPostProcess)
                            modulesFields = PrepareModuleRow(csv);
                    }
                }
            }
        }

        #region PrepareRow

        private CsvV2ProductFields PrepareRow(IReader csv, bool onlyOffer = false, bool onlyPostProcess = false)
        {
            var fields = new CsvV2ProductFields();

            if (onlyPostProcess && onlyOffer)
                return fields;// return empty row, no postprocess data

            foreach (EProductField productField in Enum.GetValues(typeof(EProductField)))
            {
                try
                {
                    if (productField == EProductField.None)
                        continue;

                    if (onlyOffer && !productField.IsOfferField())
                        continue;
                    if (onlyPostProcess && productField != EProductField.Code && !productField.IsPostProcessField())
                        continue;

                    if (productField == EProductField.Category)
                    {
                        foreach (var index in _fieldMapping.CategoriesMap)
                            fields.Categories.Add(GetCsvValue(csv, index));
                        continue;
                    }
                    if (productField == EProductField.Sorting)
                    {
                        foreach (var index in _fieldMapping.CategorySortingMap)
                            fields.CategoriesSorting.Add(GetCsvValue(csv, index));
                        continue;
                    }
                    if (productField == EProductField.Property)
                    {
                        foreach (var index in _fieldMapping.PropertiesMap.Keys)
                        {
                            if (!fields.Properties.ContainsKey(_fieldMapping.PropertiesMap[index]))
                                fields.Properties.Add(_fieldMapping.PropertiesMap[index], GetCsvValue(csv, index));
                        }
                        continue;
                    }

                    if (productField == EProductField.PriceRule)
                    {
                        foreach (var index in _fieldMapping.PriceRulesMap.Keys)
                        {
                            fields.PriceRulePrices.Add(new CsvV2ProductFieldPriceRulePrice()
                            {
                                Name = _fieldMapping.PriceRulesMap[index],
                                Price = GetCsvValue(csv, index).TryParseFloat(true)
                            });
                        }
                        continue;
                    }

                    if (productField == EProductField.WarehouseStock)
                    {
                        foreach (var index in _fieldMapping.WarehouseStocksMap.Keys)
                        {
                            fields.WarehouseStocks.Add(new CsvV2ProductFieldWarehouseStocks
                            {
                                Name = _fieldMapping.WarehouseStocksMap[index],
                                Quantity = GetCsvValue(csv, index).TryParseFloat(true)
                            });
                        }

                        continue;
                    }
                    
                    if (!_fieldMapping.ContainsKey(productField.ToString()))
                        continue;

                    object value;
                    switch (productField.Status())
                    {
                        case CsvFieldStatus.String:
                            GetString(productField, csv, out value);
                            break;
                        case CsvFieldStatus.StringRequired:
                            if (!GetStringRequired(productField, csv, out value))
                                return null;
                            break;
                        case CsvFieldStatus.NotEmptyString:
                            if (!GetStringNotNull(productField, csv, out value))
                                continue;
                            break;
                        case CsvFieldStatus.Float:
                            if (!GetFloat(productField, csv, out value, 0))
                                return null;
                            break;
                        case CsvFieldStatus.NullableFloat:
                            if (!GetFloat(productField, csv, out value, default(float?)))
                                return null;
                            break;
                        case CsvFieldStatus.Int:
                            if (!GetInt(productField, csv, out value, 0))
                                return null;
                            break;
                        case CsvFieldStatus.NullableInt:
                            if (!GetInt(productField, csv, out value, default(int?)))
                                return null;
                            break;
                        case CsvFieldStatus.Long:
                            if (!GetLong(productField, csv, out value, 0))
                                return null;
                            break;
                        case CsvFieldStatus.NullableLong:
                            if (!GetLong(productField, csv, out value, default(long?)))
                                return null;
                            break;
                        case CsvFieldStatus.DateTime:
                            if (!GetDateTime(productField, csv, out value, default(DateTime)))
                                return null;
                            break;
                        case CsvFieldStatus.NullableDateTime:
                            if (!GetDateTime(productField, csv, out value, default(DateTime?)))
                                return null;
                            break;
                        case CsvFieldStatus.None:
                            value = null;
                            break;
                        default:
                            throw new NotImplementedException("No implementation for CsvFieldStatus " + productField.ToString());
                    }

                    fields.Add(productField, value);
                }
                catch (MissingFieldException exception)
                {
                    DoForCommonStatistic(() => {
                        CommonStatistic.WriteLog($"Строка №{_currentRow}: Не валидный формат строки - пропущено поле {productField.Localize()}");
                        CommonStatistic.TotalErrorRow++;
                    });
                    return null;
                }
            }
            return fields;
        }

        private Dictionary<CSVField, object> PrepareModuleRow(IReader row)
        {
            var fields = new Dictionary<CSVField, object>();
            foreach (var moduleField in _modulesAndFields.Values.SelectMany(moduleFields => moduleFields))
            {
                if (_fieldMapping.ContainsKey(moduleField.StrName))
                    fields.Add(moduleField, GetCsvValue(row, moduleField.StrName));
            }
            return fields;
        }

        private bool GetString(EProductField fieldType, IReaderRow row, out object value)
        {
            value = GetCsvValue(row, fieldType);
            return true;
        }

        private bool GetStringNotNull(EProductField fieldType, IReaderRow row, out object value)
        {
            value = GetCsvValue(row, fieldType);
            return value.AsString().IsNotEmpty();
        }

        private bool GetStringRequired(EProductField fieldType, IReaderRow row, out object value)
        {
            value = GetCsvValue(row, fieldType);
            if (value.AsString().IsNullOrEmpty())
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), fieldType.Localize(), _currentRow));
                return false;
            }
            return true;
        }

        private bool GetFloat(EProductField fieldType, IReaderRow row, out object value, float? defaultValue)
        {
            value = defaultValue;
            var strValue = GetCsvValue(row, fieldType);
            if (strValue.IsNullOrEmpty())
                return true;

            float decValue;
            if (!float.TryParse(strValue, out decValue) &&
                !float.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), fieldType.Localize(), _currentRow));
                return false;
            }
            value = decValue;
            return true;
        }

        private bool GetInt(EProductField fieldType, IReaderRow row, out object value, int? defaultValue)
        {
            value = defaultValue;
            var strValue = GetCsvValue(row, fieldType);
            if (strValue.IsNullOrEmpty())
                return true;

            int intValue;
            if (!int.TryParse(strValue, out intValue))
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), fieldType.Localize(), _currentRow));
                return false;
            }
            value = intValue;
            return true;
        }
        
        private bool GetLong(EProductField fieldType, IReaderRow row, out object value, long? defaultValue)
        {
            value = defaultValue;
            var strValue = GetCsvValue(row, fieldType);
            if (strValue.IsNullOrEmpty())
                return true;

            if (!long.TryParse(strValue, out var longValue))
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), fieldType.Localize(), _currentRow));
                return false;
            }
            value = longValue;
            return true;
        }

        private bool GetDateTime(EProductField fieldType, IReaderRow row, out object value, DateTime? defaultValue)
        {
            value = defaultValue;
            var strValue = GetCsvValue(row, fieldType);
            if (strValue.IsNullOrEmpty())
                return true;

            DateTime dateValue;
            if (!DateTime.TryParse(strValue, out dateValue) &&
                !DateTime.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), fieldType.Localize(), _currentRow));
                return false;
            }
            value = dateValue;
            return true;
        }

        private string GetCsvValue(IReaderRow row, EProductField productField)
        {
            return GetCsvValue(row, productField.ToString());
        }

        private string GetCsvValue(IReaderRow row, string mappingKey)
        {
            return row[_fieldMapping[mappingKey]].DefaultOrEmpty().SupperTrim();
        }

        private string GetCsvValue(IReaderRow row, int index)
        {
            return row[index].DefaultOrEmpty().SupperTrim();
        }

        #endregion

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
            });
        }

        private static bool useMultiThreadImport = false;

        private void UpdateInsertProduct(List<CsvV2ProductFields> rows, Func<Product, Product> func = null)
        {
            if (useMultiThreadImport)
            {
                var added = false;
                while (!added)
                {
                    ThreadPool.GetAvailableThreads(out var workerThreads, out _);
                    if (workerThreads != 0)
                    {
                        Task.Factory.StartNew(() => UpdateInsertProductWorker(rows, func), TaskCreationOptions.LongRunning);
                        added = true;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            else
            {
                UpdateInsertProductWorker(rows, func);
            }
        }

        private void UpdateInsertProductWorker(List<CsvV2ProductFields> rows, Func<Product, Product> func = null)
        {
            try
            {
                var fields = rows[0];
                Product product = null;
                if (fields.ContainsKey(EProductField.Code) && fields[EProductField.Code].AsString().IsNullOrEmpty())
                    throw new Exception(LocalizationService.GetResource("Core.ExportImport.ImportCsvV2.NoProductCode"));

                var artNo = fields.ContainsKey(EProductField.Code) ? fields[EProductField.Code].AsString() : string.Empty;
                if (artNo.IsNotEmpty())
                    product = ProductService.GetProduct(artNo, withExportParams: true);

                bool addingNew = product == null;

                if (addingNew)
                {
                    if (_onlyUpdateProducts)
                        return;

                    product = new Product
                    {
                        ArtNo = artNo,
                        Multiplicity = 1,
                        CurrencyID = SettingsCatalog.DefaultCurrency.CurrencyId,
                        Enabled = true
                    };
                }

                if (_updateNameAndDescription || addingNew)
                {
                    if (fields.ContainsKey(EProductField.Name))
                        product.Name = fields[EProductField.Name].AsString();
                    else
                        product.Name = product.Name ?? string.Empty;
                }

                if (fields.ContainsKey(EProductField.Enabled))
                    product.Enabled = fields[EProductField.Enabled].AsString().Equals("+");

                if (fields.ContainsKey(EProductField.Currency))
                {
                    var currency = CurrencyService.Currency(fields[EProductField.Currency].AsString());
                    if (currency != null)
                        product.CurrencyID = currency.CurrencyId;
                    else
                        throw new Exception(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", 
                            EProductField.Currency.Localize(), string.Empty, _currentRow));
                }
                else if (addingNew)
                { 
                    var currencies = CurrencyService.GetAllCurrencies(true) ?? new List<Currency>();
                    var currency = currencies.FirstOrDefault(i => i.Rate == 1)
                        ?? currencies.FirstOrDefault(i => i.Iso3 == SettingsCatalog.DefaultCurrencyIso3);

                    if (currency != null)
                        product.CurrencyID = currency.CurrencyId;
                    else
                        throw new Exception("Currency not found");
                }

                if (fields.ContainsKey(EProductField.OrderByRequest))
                    product.AllowPreOrder = fields[EProductField.OrderByRequest].AsString().Equals("+");
                
                if (fields.ContainsKey(EProductField.AccrueBonuses))
                    product.AccrueBonuses = fields[EProductField.AccrueBonuses].AsString().Equals("+");

                if (fields.ContainsKey(EProductField.Unit))
                    product.UnitId = UnitService.UnitFromString(fields[EProductField.Unit].AsString());

                if (fields.ContainsKey(EProductField.Discount) ||
                    fields.ContainsKey(EProductField.DiscountAmount))
                {
                    var percent = !fields.ContainsKey(EProductField.Discount) || fields[EProductField.Discount] == null
                        ? product.Discount.Percent
                        : fields[EProductField.Discount].AsFloat();

                    var amount = !fields.ContainsKey(EProductField.DiscountAmount) || fields[EProductField.DiscountAmount] == null
                        ? product.Discount.Amount
                        : fields[EProductField.DiscountAmount].AsFloat();

                    product.Discount = new Discount(percent, amount);
                }

                if (fields.Keys.Any(field => field.IsOfferField()))
                {
                    product.HasMultiOffer = rows.Count > 1;
                    var oldOffers = new List<Offer>(product.Offers);
                    product.Offers.Clear();

                    for (var i  = 0; i < rows.Count; i++)
                    {
                        var offerFields = rows[i];
                        var sku = "";

                        if (offerFields.ContainsKey(EProductField.Sku))
                        {
                            sku = offerFields[EProductField.Sku].AsString();
                        }
                        else if (oldOffers.Count == 0 && !string.IsNullOrEmpty(product.ArtNo))
                        {
                            sku = product.ArtNo + (i == 0 ? "" : "-" + i);
                        }

                        var offer = (sku.IsNotEmpty()
                            ? oldOffers.FirstOrDefault(o => o.ArtNo.ToLower() == sku.ToLower())
                            : null) ?? new Offer();
                        
                        offer.ProductId = product.ProductId;
                        offer.Main = !product.Offers.Any();
                        offer.ArtNo = sku;

                        if (offerFields.ContainsKey(EProductField.Size))
                        {
                            var sizeName = offerFields[EProductField.Size].AsString();
                            if (sizeName.IsNotEmpty())
                            {
                                var size = SizeService.GetSize(sizeName) ?? new Size { SizeName = sizeName };
                                offer.SizeID = size.SizeId != 0 ? size.SizeId : SizeService.AddSize(size);
                            }
                            else
                                offer.SizeID = null;
                        }

                        if (offerFields.ContainsKey(EProductField.Color))
                        {
                            var colorName = offerFields[EProductField.Color].AsString();
                            if (colorName.IsNotEmpty())
                            {
                                var color = ColorService.GetColor(colorName) ?? new Color { ColorName = colorName, ColorCode = "#000000" };
                                if (color.ColorId != 0)
                                    offer.ColorID = color.ColorId;
                                else
                                {
                                    offer.ColorID = ColorService.AddColor(color);
                                    if (!_addedColorIds.Contains(color.ColorId))
                                        _addedColorIds.Add(color.ColorId);
                                }
                            }
                            else
                                offer.ColorID = null;
                        }

                        if (offerFields.ContainsKey(EProductField.Price))
                            offer.BasePrice = offerFields[EProductField.Price].AsFloat();
                        if (offerFields.ContainsKey(EProductField.PurchasePrice))
                            offer.SupplyPrice = offerFields[EProductField.PurchasePrice].AsFloat();
                        if (offerFields.ContainsKey(EProductField.Weight))
                            offer.Weight = offerFields[EProductField.Weight].AsFloat();
                        if (offerFields.ContainsKey(EProductField.Dimensions))
                        {
                            var dimensions = offerFields[EProductField.Dimensions].AsString().Trim().ToLower()
                                .Split(new[] { '|', 'x', 'х' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TryParseFloat()).ToList();
                            offer.Length = dimensions.Count > 0 ? dimensions[0] : 0;
                            offer.Width = dimensions.Count > 1 ? dimensions[1] : 0;
                            offer.Height = dimensions.Count > 2 ? dimensions[2] : 0;
                        }
                        if (offerFields.ContainsKey(EProductField.BarCode))
                            offer.BarCode = offerFields[EProductField.BarCode].AsString();

                        if (offerFields.PriceRulePrices != null)
                        {
                            foreach (var item in offerFields.PriceRulePrices)
                                item.Artno = offer.ArtNo;
                        }
                        
                        if(offerFields.WarehouseStocks != null)
                            foreach (var item in offerFields.WarehouseStocks)
                                item.Artno = offer.ArtNo;

                        product.Offers.Add(offer);
                    }

                    if (!product.Offers.Any())
                        product.Offers = oldOffers;
                }

                if (_updateNameAndDescription || addingNew)
                {
                    if (fields.ContainsKey(EProductField.BriefDescription))
                    {
                        var descr = fields[EProductField.BriefDescription].AsString();
                        if (descr.IsLongerThan(ProductService.MaxDescLength))
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.TextLengthLimit", EProductField.BriefDescription.Localize(), _currentRow, ProductService.MaxDescLength));
                        else
                            product.BriefDescription = descr;
                    }

                    if (fields.ContainsKey(EProductField.Description))
                    {
                        var descr = fields[EProductField.Description].AsString();
                        if (descr.IsLongerThan(ProductService.MaxDescLength))
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.TextLengthLimit", EProductField.Description.Localize(), _currentRow, ProductService.MaxDescLength));
                        else
                            product.Description = descr;
                    }
                }

                if (fields.ContainsKey(EProductField.YandexSalesNotes))
                    product.ExportOptions.YandexSalesNote = fields[EProductField.YandexSalesNotes].AsString();

                if (fields.ContainsKey(EProductField.GoogleGtin))
                    product.ExportOptions.Gtin = fields[EProductField.GoogleGtin].AsString();
                
                if (fields.ContainsKey(EProductField.GoogleMpn))
                    product.ExportOptions.Mpn = fields[EProductField.GoogleMpn].AsString();

                if (fields.ContainsKey(EProductField.GoogleProductCategory))
                    product.ExportOptions.GoogleProductCategory = fields[EProductField.GoogleProductCategory].AsString();

                if (fields.ContainsKey(EProductField.GoogleAvailabilityDate))
                {
                    var availabilityDate = fields[EProductField.GoogleAvailabilityDate].AsString();
                    
                    product.ExportOptions.GoogleAvailabilityDate = !string.IsNullOrEmpty(availabilityDate)
                        ? availabilityDate.TryParseDateTime()
                        : default(DateTime?);
                }

                if (fields.ContainsKey(EProductField.YandexTypePrefix))
                    product.ExportOptions.YandexTypePrefix = fields[EProductField.YandexTypePrefix].AsString();

                if (fields.ContainsKey(EProductField.YandexModel))
                    product.ExportOptions.YandexModel = fields[EProductField.YandexModel].AsString();

                if (fields.ContainsKey(EProductField.YandexName))
                    product.ExportOptions.YandexName = fields[EProductField.YandexName].AsString();

                if (fields.ContainsKey(EProductField.YandexSizeUnit))
                    product.ExportOptions.YandexSizeUnit = fields[EProductField.YandexSizeUnit].AsString();

                if (fields.ContainsKey(EProductField.YandexDeliveryDays))
                    product.ExportOptions.YandexDeliveryDays = fields[EProductField.YandexDeliveryDays].AsString().Reduce(5);

                if (fields.ContainsKey(EProductField.YandexBid))
                    product.ExportOptions.Bid = fields[EProductField.YandexBid].AsFloat() != 0 ? fields[EProductField.YandexBid].AsFloat() : 0;

                if (fields.ContainsKey(EProductField.YandexDiscounted))
                    product.ExportOptions.YandexProductDiscounted = fields[EProductField.YandexDiscounted].AsString().Equals("+");

                if (fields.ContainsKey(EProductField.YandexDiscountCondition))
                {
                    var yandexDiscountCondition = fields[EProductField.YandexDiscountCondition].AsString();
                    if (!string.IsNullOrEmpty(yandexDiscountCondition))
                    {
                        var condition = Enum.GetValues(typeof(EYandexDiscountCondition)).Cast<EYandexDiscountCondition>().FirstOrDefault(x => yandexDiscountCondition.Equals(x.StrName(), StringComparison.OrdinalIgnoreCase));
                        if (condition != EYandexDiscountCondition.None)
                            product.ExportOptions.YandexProductDiscountCondition = condition;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", EProductField.YandexDiscountCondition.Localize(), yandexDiscountCondition, _currentRow));
                    }
                }
                
                if (fields.ContainsKey(EProductField.YandexProductQuality))
                {
                    var yandexDiscountCondition = fields[EProductField.YandexProductQuality].AsString();
                    if (!string.IsNullOrEmpty(yandexDiscountCondition))
                    {
                        var quality = Enum.GetValues(typeof(EYandexProductQuality)).Cast<EYandexProductQuality>().FirstOrDefault(x => yandexDiscountCondition.Equals(x.StrName(), StringComparison.OrdinalIgnoreCase));
                        if (quality != EYandexProductQuality.None)
                            product.ExportOptions.YandexProductQuality = quality;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", EProductField.YandexProductQuality.Localize(), yandexDiscountCondition, _currentRow));
                    }
                }

                if (fields.ContainsKey(EProductField.YandexDiscountReason))
                    product.ExportOptions.YandexProductDiscountReason = fields[EProductField.YandexDiscountReason].AsString();

                if (fields.ContainsKey(EProductField.YandexMarketExpiry))
                    product.ExportOptions.YandexMarketExpiry = fields[EProductField.YandexMarketExpiry].AsString();
                
                if (fields.ContainsKey(EProductField.YandexMarketWarrantyDays))
                    product.ExportOptions.YandexMarketWarrantyDays = fields[EProductField.YandexMarketWarrantyDays].AsString();

                if (fields.ContainsKey(EProductField.YandexMarketCommentWarranty))
                    product.ExportOptions.YandexMarketCommentWarranty = fields[EProductField.YandexMarketCommentWarranty].AsString();

                if (fields.ContainsKey(EProductField.YandexMarketPeriodOfValidityDays))
                    product.ExportOptions.YandexMarketPeriodOfValidityDays = fields[EProductField.YandexMarketPeriodOfValidityDays].AsString();

                if (fields.ContainsKey(EProductField.YandexMarketServiceLifeDays))
                    product.ExportOptions.YandexMarketServiceLifeDays = fields[EProductField.YandexMarketServiceLifeDays].AsString();

                if (fields.ContainsKey(EProductField.YandexMarketTnVedCode))
                    product.ExportOptions.YandexMarketTnVedCode = fields[EProductField.YandexMarketTnVedCode].AsString();
                
                if (fields.ContainsKey(EProductField.YandexMarketStepQuantity))
                    product.ExportOptions.YandexMarketStepQuantity = fields[EProductField.YandexMarketStepQuantity].AsNullableInt();
                
                if (fields.ContainsKey(EProductField.YandexMarketMinQuantity))
                    product.ExportOptions.YandexMarketMinQuantity = fields[EProductField.YandexMarketMinQuantity].AsNullableInt();
                
                if (fields.ContainsKey(EProductField.YandexMarketCategoryId))
                    product.ExportOptions.YandexMarketCategoryId = fields[EProductField.YandexMarketCategoryId].AsNullableLong();
                
                if (fields.ContainsKey(EProductField.Adult))
                    product.ExportOptions.Adult = fields[EProductField.Adult].AsString().Equals("+");

                if (fields.ContainsKey(EProductField.ManufacturerWarranty))
                    product.ExportOptions.ManufacturerWarranty = fields[EProductField.ManufacturerWarranty].AsString().Equals("+");

                if (fields.ContainsKey(EProductField.ShippingPrice))
                    product.ShippingPrice = fields[EProductField.ShippingPrice].AsNullableFloat();

                if(fields.ContainsKey(EProductField.Marking))
                    product.IsMarkingRequired = fields[EProductField.Marking].AsString().Equals("+");

                if(fields.ContainsKey(EProductField.DoNotApplyOtherDiscounts))
                    product.DoNotApplyOtherDiscounts = fields[EProductField.DoNotApplyOtherDiscounts].AsString().Equals("+");
                
                if(fields.ContainsKey(EProductField.IsDigital))
                    product.IsDigital = fields[EProductField.IsDigital].AsString().Equals("+");
                
                if(fields.ContainsKey(EProductField.DownloadLink))
                    product.DownloadLink = fields[EProductField.DownloadLink].AsString();

                if (fields.ContainsKey(EProductField.ParamSynonym))
                {
                    var prodUrl = fields[EProductField.ParamSynonym].AsString().IsNotEmpty()
                                      ? fields[EProductField.ParamSynonym].AsString()
                                      : !string.IsNullOrEmpty(product.Name) ? product.Name : product.ArtNo;

                    product.UrlPath = UrlService.GetAvailableValidUrl(product.ProductId, ParamType.Product, prodUrl);
                }
                else
                {
                    product.UrlPath = product.UrlPath ??
                                      UrlService.GetAvailableValidUrl(product.ProductId, ParamType.Product,
                                          !string.IsNullOrEmpty(product.Name) ? product.Name : product.ArtNo);

                }

                if (_enabled301Redirects && fields.ContainsKey(EProductField.Url) && fields[EProductField.Url].AsString().IsNotEmpty())
                {
                    var redirectFrom = fields[EProductField.Url].AsString();
                    var redirectTo = $"{SettingsMain.SiteUrl}/products/{product.UrlPath}"; 
                    var redirect = RedirectSeoService.GetRedirectsSeoByRedirectFrom(redirectFrom);
                    if (redirect != null)
                    {
                        redirect.RedirectTo = redirectTo;
                        RedirectSeoService.UpdateRedirectSeo(redirect);
                    }
                    else if(!redirectTo.Equals(redirectFrom, StringComparison.OrdinalIgnoreCase))
                    {
                        redirect = new RedirectSeo
                        {
                            RedirectTo = redirectTo,
                            RedirectFrom = redirectFrom,
                            ProductArtNo = product.ArtNo
                        };
                        RedirectSeoService.AddRedirectSeo(redirect);
                    }
                }

                var meta = (!addingNew ? MetaInfoService.GetMetaInfo(product.ProductId, MetaType.Product) : null) ?? new MetaInfo();
                if (fields.ContainsKey(EProductField.SeoTitle) && string.IsNullOrEmpty(fields[EProductField.SeoTitle].AsString())
                    || fields.ContainsKey(EProductField.SeoH1) && string.IsNullOrEmpty(fields[EProductField.SeoH1].AsString()))
                    product.Meta = meta;
                else
                    product.Meta = new MetaInfo(0, product.ProductId, MetaType.Product,
                        (fields.ContainsKey(EProductField.SeoTitle)
                            ? fields[EProductField.SeoTitle].AsString()
                            : meta.Title).DefaultOrEmpty(),
                        (fields.ContainsKey(EProductField.SeoMetaKeywords)
                            ? fields[EProductField.SeoMetaKeywords].AsString()
                            : meta.MetaKeywords).DefaultOrEmpty(),
                        (fields.ContainsKey(EProductField.SeoMetaDescription)
                            ? fields[EProductField.SeoMetaDescription].AsString()
                            : meta.MetaDescription).DefaultOrEmpty(),
                        (fields.ContainsKey(EProductField.SeoH1)
                            ? fields[EProductField.SeoH1].AsString()
                            : meta.H1).DefaultOrEmpty());

                if (fields.ContainsKey(EProductField.MarkerBestseller))
                    product.BestSeller = fields[EProductField.MarkerBestseller].AsString().Equals("+");
                if (fields.ContainsKey(EProductField.MarkerNew))
                    product.New = fields[EProductField.MarkerNew].AsString().Equals("+");
                if (fields.ContainsKey(EProductField.MarkerOnSale))
                    product.OnSale = fields[EProductField.MarkerOnSale].AsString().Equals("+");
                if (fields.ContainsKey(EProductField.MarkerRecomended))
                    product.Recomended = fields[EProductField.MarkerRecomended].AsString().Equals("+");

                if (fields.ContainsKey(EProductField.Producer))
                    product.BrandId = BrandService.BrandFromString(fields[EProductField.Producer].AsString());

                if (fields.ContainsKey(EProductField.MinAmount))
                    product.MinAmount = fields[EProductField.MinAmount].AsFloat() != 0 ? fields[EProductField.MinAmount].AsFloat() : (float?)null;

                if (fields.ContainsKey(EProductField.MaxAmount))
                    product.MaxAmount = fields[EProductField.MaxAmount].AsFloat() != 0 ? fields[EProductField.MaxAmount].AsFloat() : (float?)null;

                if (fields.ContainsKey(EProductField.Multiplicity))
                    product.Multiplicity = fields[EProductField.Multiplicity].AsFloat() != 0 ? fields[EProductField.Multiplicity].AsFloat() : 1;

                if (fields.ContainsKey(EProductField.Tax))
                {
                    var taxName = fields[EProductField.Tax].AsString();
                    if (!string.IsNullOrEmpty(taxName))
                    {
                        var tax = TaxService.GetTaxes().FirstOrDefault(x => x.Name.ToLower() == taxName.ToLower());
                        if (tax != null)
                            product.TaxId = tax.TaxId;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", 
                                EProductField.Tax.Localize(), taxName, _currentRow));
                    }
                    else
                    {
                        product.TaxId = null;
                    }
                }

                if (fields.ContainsKey(EProductField.PaymentMethodType))
                {
                    var paymentMethodType = fields[EProductField.PaymentMethodType].AsString();
                    if (!string.IsNullOrEmpty(paymentMethodType))
                    {
                        var type = Enum.GetValues(typeof(ePaymentMethodType)).Cast<ePaymentMethodType>().FirstOrDefault(x => paymentMethodType.Equals(x.Localize(), StringComparison.OrdinalIgnoreCase));
                        if (type != 0)
                            product.PaymentMethodType = type;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", 
                                EProductField.PaymentMethodType.Localize(), paymentMethodType, _currentRow));
                    }
                }

                if (fields.ContainsKey(EProductField.PaymentSubjectType))
                {
                    var paymentSubjectType = fields[EProductField.PaymentSubjectType].AsString();
                    if (!string.IsNullOrEmpty(paymentSubjectType))
                    {
                        var type = Enum.GetValues(typeof(ePaymentSubjectType)).Cast<ePaymentSubjectType>().FirstOrDefault(x => paymentSubjectType.Equals(x.Localize(), StringComparison.OrdinalIgnoreCase));
                        if (type != 0)
                            product.PaymentSubjectType = type;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound",
                                EProductField.PaymentSubjectType.Localize(), paymentSubjectType, _currentRow));
                    }
                }

                if (fields.ContainsKey(EProductField.ManualRatio))
                {
                    var ratio = fields[EProductField.ManualRatio].AsFloat();
                    if(ratio > 0 && ratio <= 5)
                        product.ManualRatio = ratio;
                }

                if (fields.ContainsKey(EProductField.Comment))
                    product.Comment = fields[EProductField.Comment].AsString();

                if (fields.ContainsKey(EProductField.SizeChart))
                {
                    var name = fields[EProductField.SizeChart].AsString();
                    if (name.IsNotEmpty())
                        product.SizeChart = SizeChartService.GetByName(name);
                }

                product.ModifiedBy = _modifiedBy;

                if (func != null)
                    product = func(product);

                if (product.Offers != null && product.Offers.Count != 0)
                {
                    var artNoExist = false;
                    foreach (var offer in product.Offers)
                        if (OfferService.IsArtNoExist(offer.ArtNo, offer.OfferId))
                        {
                            artNoExist = true;
                            Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.OfferSkuIsBusy", offer.ArtNo));
                            break;
                        }
 
                    if (artNoExist)
                        product.Offers = OfferService.GetProductOffers(product.ProductId);
                }

                if (!addingNew)
                {
                    ProductService.UpdateProduct(product, false, _trackChanges, _changedBy);

                    if (!_trackChanges)
                        ProductHistoryService.ProductChanged(product, _changedBy);

                    DoForCommonStatistic(() => CommonStatistic.TotalUpdateRow++);
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductUpdated",
                        product.ArtNo));
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_EditProduct_Csv);
                }
                else
                {
                    if (!(SaasDataService.IsSaasEnabled && ProductService.GetProductsCount("[Enabled] = 1") >= SaasDataService.CurrentSaasData.ProductsCount))
                    {
                        ProductService.AddProduct(product, false, true, _changedBy);
                        DoForCommonStatistic(() => CommonStatistic.TotalAddRow++);
                        Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductAdded",
                            product.ArtNo));
                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ProductCreated_Csv);
                    }
                    else
                    {
                        Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.ProductsLimitRiched"));
                    }
                }

                if (product.ProductId > 0)
                {
                    OtherFields(rows, product.ProductId, product.ArtNo, addingNew);
                    ProductService.PreCalcProductParams(product.ProductId);
                }
                else
                {
                    LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductNotAdded", product.ArtNo));
                }
            }
            catch (Exception e)
            {
                //Debug.Log.Error(e);
                LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProcessRowError", _currentRow, e.Message));
            }
        }


        private void OtherFields(List<CsvV2ProductFields> rows, int productId, string productArtNo, bool isNewProduct)
        {
            var fields = rows[0];
            //Category
            if (fields.Categories.Any())
            {
                CategoryService.SubParseAndCreateCategories(productId, fields.Categories, fields.CategoriesSorting, _changedBy, _trackChanges, _addProductInParentCategory, isNewProduct);
            }

            if (fields.ContainsKey(EProductField.ExternalCategoryId))
            {
                var sorting = fields.ContainsKey(EProductField.Sorting)
                    ? fields[EProductField.Sorting].AsString()
                    : string.Empty;

                var externalCategories = fields[EProductField.ExternalCategoryId].AsString();
                CategoryService.AddProductToExternalCategories(externalCategories, productId, _columnSeparator, sorting);
            }

            //photos
            var distinctPhotos = new Dictionary<string, string>(); // photoName, color
            if (fields.ContainsKey(EProductField.Photos))
            {
                var photos = fields[EProductField.Photos].AsString().Split(new[] { _columnSeparator, "\n" }, StringSplitOptions.RemoveEmptyEntries).Distinct();
                distinctPhotos.AddRange(photos.Select(photo => new KeyValuePair<string, string>(photo, null)));
            }
            if (fields.ContainsKey(EProductField.OfferPhotos))
            {
                foreach (var row in rows)
                {
                    var color = row.ContainsKey(EProductField.Color) ? row[EProductField.Color].AsString() : null;
                    var photos = row[EProductField.OfferPhotos].AsString().Split(new[] { _columnSeparator, "\n" }, StringSplitOptions.RemoveEmptyEntries).Distinct();
                    foreach (var photo in photos)
                    {
                        // rewrite value by value with color if key exists
                        distinctPhotos.TryAddValue(photo, color);
                    }
                }
            }
            if (distinctPhotos.Any())
            {
                if (_updatePhotos && !isNewProduct)
                    PhotoService.DeleteProductPhotos(productId);

                var allSuccess = true;
                var isMain = true;
                foreach (var photo in distinctPhotos.Keys)
                {
                    if (PhotoService.PhotoFromString(productId, photo, isMain, distinctPhotos[photo], _skipOriginalPhoto, _downloadRemotePhoto, out var errorProcessPhoto))
                        isMain = false;
                    else
                    {
                        Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.NotPhotoProcessed", photo, errorProcessPhoto));
                        allSuccess = false;
                    }
                }
                if (!allSuccess)
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.NotAllPhotosProcessed", productArtNo));
            }

            //video
            if (fields.ContainsKey(EProductField.Videos))
            {
                string videos = fields[EProductField.Videos].AsString();
                ProductVideoService.VideoFromString(productId, videos);
            }

            //Properties
            if (fields.Properties.Count > 0)
            {
                // свойства, которых нет в файле, не обрабатываются. Удаляются только свойства с пустым значением. TASK-18399
                //PropertyService.DeleteProductProperties(productId);
                PropertyService.ProcessProductProperties(productId, fields.Properties, _columnSeparator);
            }

            //Price rules
            if (fields.PriceRulePrices.Count > 0)
            {
                var rules = PriceRuleService.GetList(false);

                foreach (var row in rows)
                {
                    foreach (var item in row.PriceRulePrices)
                    {
                        var offer = OfferService.GetOffer(item.Artno);
                        if (offer == null)
                            continue;

                        var rule = rules.Find(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                        if (rule == null)
                            continue;

                        PriceRuleService.AddUpdateOfferPriceRule(offer.OfferId, rule.Id, item.Price);
                    }
                }
            }
                            
            // warehouse stocks
            if (rows.Any(productFields => productFields.WarehouseStocks.Count > 0))
            {
                foreach (var row in rows)
                {
                    foreach (var stock in row.WarehouseStocks)
                    {
                        if (stock.Quantity is null)
                            continue;
                        
                        var offer = OfferService.GetOffer(stock.Artno);
                        if (offer == null
                            || offer.ProductId != productId)
                            continue;

                        var warehouse = _warehouses.Find(x => x.Name.Equals(stock.Name, StringComparison.OrdinalIgnoreCase));
                        if (warehouse == null)
                            continue;
                        
                        WarehouseStock warehouseStock;
                        if (_importRemains)
                        {
                            warehouseStock =
                                WarehouseStocksService.GetOfferStocks(offer.OfferId)
                                                      .FirstOrDefault(_ => _.WarehouseId == warehouse.Id)
                                ?? new WarehouseStock
                                {
                                    OfferId = offer.OfferId,
                                    WarehouseId = warehouse.Id,
                                    Quantity = 0f
                                };
                            warehouseStock.Quantity += stock.Quantity.Value;
                        }
                        else
                            warehouseStock = new WarehouseStock
                            {
                                OfferId = offer.OfferId,
                                WarehouseId = warehouse.Id,
                                Quantity = stock.Quantity.Value
                            };
                        
                        WarehouseStocksService.AddUpdateStocks(warehouseStock, trackChanges: _trackChanges, calcHasProductsForWarehouse: false);
                    }
                }
            } else if (_fieldMapping.WarehouseStocksMap.Count == 0
                       && fields.ContainsKey(EProductField.Amount))
            {
                _warehouses = _warehouses ?? WarehouseService.GetList();
                
                // поддержание старого поведения, когда не было складов
                if (_warehouses.Count == 1)
                {
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var offerFields = rows[i];
                        var sku = offerFields.ContainsKey(EProductField.Sku) ? offerFields[EProductField.Sku].AsString() : null;
                        if (sku.IsNullOrEmpty())
                            continue;
                                            
                        var offer = OfferService.GetOffer(sku);
                        if (offer == null
                            || offer.ProductId != productId)
                            continue;
                        
                        if (offerFields.ContainsKey(EProductField.Amount))
                        {
                            var amount = offerFields[EProductField.Amount].AsFloat();
                            
                            WarehouseStock stock;
                            if (_importRemains)
                            {
                                stock = WarehouseStocksService.GetOfferStocks(offer.OfferId).Single();
                                stock.Quantity += amount;
                            }
                            else
                                stock = new WarehouseStock
                                {
                                    OfferId = offer.OfferId,
                                    WarehouseId = _warehouses[0].Id,
                                    Quantity = amount
                                };
                            
                            WarehouseStocksService.AddUpdateStocks(stock, trackChanges: _trackChanges, calcHasProductsForWarehouse: false);
                        }
                    }
                }
            }

            if (fields.ContainsKey(EProductField.AvitoProductProperties))
            {
                string avitoProductProperties = fields[EProductField.AvitoProductProperties].AsString();
                ExportFeedAvitoService.ImportProductProperties(productId, avitoProductProperties, _columnSeparator, _propertySeparator);
            }

            if (fields.ContainsKey(EProductField.CustomOptions))
            {
                string customOption = fields[EProductField.CustomOptions].AsString();
                CustomOptionsService.CustomOptionsFromString(productId, customOption, isNewProduct);
            }

            if (fields.ContainsKey(EProductField.Tags) &&
                   (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags))
            {
                if (!isNewProduct)
                    TagService.DeleteMap(productId, ETagType.Product);

                var i = 0;

                foreach (var tagName in fields[EProductField.Tags].AsString().Split(new[] { _columnSeparator }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimTagName = tagName.SupperTrim();
                    var tag = TagService.Get(trimTagName);
                    if (tag == null)
                    {
                        var tagId = TagService.Add(new Tag
                        {
                            Name = trimTagName,
                            Enabled = true,
                            UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Tag, trimTagName)
                        });
                        TagService.AddMap(productId, tagId, ETagType.Product, i * 10);
                    }
                    else
                    {
                        TagService.AddMap(productId, tag.Id, ETagType.Product, i * 10);
                    }
                    i++;
                }
            }
        }

        private void Log(string message)
        {
            DoForCommonStatistic(() => CommonStatistic.WriteLog(message));
        }

        private void PostProcess(List<CsvV2ProductFields> rows, Dictionary<CSVField, object> moduleFieldValues, Dictionary<ICSVExportImport, List<CSVField>> modulesAndFields)
        {
            var fields = rows[0];
            if (!fields.ContainsKey(EProductField.Code))
                return;

            var artNo = fields[EProductField.Code].AsString();
            int productId = ProductService.GetProductId(artNo);
            if (productId == 0)
                return;

            //relations
            if (fields.ContainsKey(EProductField.Related))
            {
                var linkproducts = fields[EProductField.Related].AsString();
                ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Related, _columnSeparator);
            }

            //relations
            if (fields.ContainsKey(EProductField.Alternative))
            {
                var linkproducts = fields[EProductField.Alternative].AsString();
                ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Alternative, _columnSeparator);
            }

            //gifts
            if (fields.ContainsKey(EProductField.Gifts))
            {
                _useMassPrecalc = true;
                var linkproducts = fields[EProductField.Gifts].AsString();
                ProductGiftService.GiftsFromString(productId, linkproducts, _columnSeparator);
            }

            // modules
            foreach (var moduleInstance in modulesAndFields.Keys)
            {
                foreach (var moduleField in modulesAndFields[moduleInstance].Where(moduleFieldValues.ContainsKey))
                {
                    try
                    {
                        if (!moduleInstance.ProcessField(moduleField, productId, moduleFieldValues[moduleField].AsString(), _columnSeparator, _propertySeparator))
                            Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ModuleError", artNo, moduleInstance.ModuleStringId));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error("Csv import V2 error during post-processing of module fields", ex);
                        Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ModuleError", artNo, moduleInstance.ModuleStringId));
                    }
                }
            }
        }

        private void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }
    }
}