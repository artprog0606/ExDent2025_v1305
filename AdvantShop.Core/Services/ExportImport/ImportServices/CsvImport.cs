using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Statistic;
using AdvantShop.FullSearch;
using AdvantShop.Taxes;
using CsvHelper;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Customers;
using AdvantShop.SEO;
using Debug = AdvantShop.Diagnostics.Debug;
using MissingFieldException=CsvHelper.MissingFieldException;

namespace AdvantShop.ExportImport
{
    public class CsvImport
    {
        private readonly string _fullPath;
        private readonly EImportProductActionType _actionWithProducts;
        private readonly bool _hasHeaders;
        private readonly bool _skipOriginalPhoto;
        private Dictionary<string, int> _fieldMapping;
        private readonly string _separators;
        private readonly string _encodings;
        private readonly string _columnSeparator;
        private readonly string _propertySeparator;
        private readonly bool _importRemains;
        private readonly bool _updatePhotos;
        private readonly bool _updateNameAndDescription;
        private readonly bool _downloadRemotePhoto;
        private readonly bool _onlyUpdateProducts;
        private readonly int _defaultCurrencyId;
        private readonly bool _trackChanges;
        private readonly string _modifiedBy;
        private readonly ChangedBy _changedBy;
        private bool _useCommonStatistic;
        private bool _useMassPrecalc;
        private readonly bool _enabled301Redirects;
        private readonly bool _addProductInParentCategory;
        private readonly int? _stocksToWarehouseId;
        private List<int> _addedColorIds;

        private readonly Dictionary<ICSVExportImport, List<CSVField>> _modulesAndFields;

        private CsvImport(
            string filePath, 
            bool hasHeaders, 
            EImportProductActionType actionWithProducts, 
            string separators, 
            string encodings, 
            Dictionary<string, int> fieldMapping, 
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
            bool addProductInParentCategory,
            int? stocksToWarehouseId)
        {
            _fullPath = filePath;
            _hasHeaders = hasHeaders;
            _actionWithProducts = actionWithProducts;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _separators = separators;
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

            _defaultCurrencyId = SettingsCatalog.DefaultCurrency.CurrencyId;
            _enabled301Redirects = enabled301Redirects;
            _addProductInParentCategory = addProductInParentCategory;
            _stocksToWarehouseId =
                stocksToWarehouseId != null && WarehouseService.Exists(stocksToWarehouseId.Value)
                    ? stocksToWarehouseId
                    : null;
            _addedColorIds = new List<int>();
        }

        public static CsvImport Factory(
            string filePath,
            bool hasHeaders,
            EImportProductActionType actionWithProducts,
            string separators,
            string encodings,
            Dictionary<string, int> fieldMapping,
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
            bool addProductInParentCategory = false,
            int? stocksToWarehouseId = null)
        {
            return new CsvImport(filePath, hasHeaders, actionWithProducts, separators, encodings, fieldMapping, columSeparator, propertySeparator,
                                 skipOriginalPhoto, remains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, useCommonStatistic, 
                                 onlyUpdateProducts, trackChanges, modifiedBy, enabled301Redirects, addProductInParentCategory, stocksToWarehouseId);
        }

        public static CsvImport Factory(
            string filePath, 
            bool hasHeaders, 
            EImportProductActionType importProductActionType,
            bool skipOriginalPhoto = false, 
            bool remains = false, 
            bool updatePhotos = false, 
            bool updateNameAndDescription = true, 
            bool downloadRemotePhoto = true,
            bool useCommonStatistic = true,
            bool onlyUpdateProducts = false)
        {
            return new CsvImport(filePath, hasHeaders, importProductActionType, null, null, null, null, null, 
                                 skipOriginalPhoto, remains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, useCommonStatistic, 
                                 onlyUpdateProducts, false, null, false, false, null);
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _separators ?? SeparatorsEnum.SemicolonSeparated.StrName();
            csvConfiguration.HasHeaderRecord = hasHeaderRecord ?? _hasHeaders;
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())), csvConfiguration);

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

        // private - чтобы нельзя было запускать импорт при уже запущеном,
        // через данный метот это не контролируется
        private void Process(Func<Product, Product> func = null, Action onBeforeMassActions = null)
        {
            try
            {
                _process(func, onBeforeMassActions);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                DoForCommonStatistic(() =>
                {
                    CommonStatistic.WriteLog(ex.Message);
                    CommonStatistic.TotalErrorRow++;
                });
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
                throw new Exception("can mapping columns");
            
            var startAt = DateTime.Now;

            DoForCommonStatistic(() => CommonStatistic.TotalRow = GetRowCount());

            var somePostProcessing =
                _fieldMapping.ContainsKey(ProductFields.Related.StrName()) ||
                _fieldMapping.ContainsKey(ProductFields.Alternative.StrName()) ||
                _fieldMapping.ContainsKey(ProductFields.Gifts.StrName()) ||
                _fieldMapping.ContainsKey(ProductFields.Weight.StrName()) ||
                _fieldMapping.ContainsKey(ProductFields.Size.StrName()) ||
                _fieldMapping.ContainsKey(ProductFields.BarCode.StrName());

            foreach (var moduleFields in _modulesAndFields.Values)
            {
                somePostProcessing |= moduleFields.Any(moduleField => _fieldMapping.ContainsKey(moduleField.StrName));
            }

            if (somePostProcessing)
                DoForCommonStatistic(() => CommonStatistic.TotalRow *= 2);

            ProcessRows(false, _columnSeparator, _propertySeparator, func);
            if (somePostProcessing)
                ProcessRows(true, _columnSeparator, _propertySeparator);

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
            _fieldMapping = new Dictionary<string, int>();
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

        private void ProcessRows(bool onlyPostProcess, string columSeparator, string propertySeparator, Func<Product, Product> func = null)
        {
            if (!File.Exists(_fullPath))
                return;

            var productFields = PrepareProductFields();

            using (var csv = InitReader())
            {
                if (_hasHeaders)
                {
                    csv.Read();
                    csv.ReadHeader();
                }
                while (csv.Read())
                {
                    if ((!CommonStatistic.IsRun || CommonStatistic.IsBreaking) && _useCommonStatistic)
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var productInStrings = PrepareRow(productFields, csv);
                        if (productInStrings == null)
                        {
                            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                            continue;
                        }

                        if (!onlyPostProcess)
                            UpdateInsertProduct(productInStrings, columSeparator, propertySeparator, _skipOriginalPhoto, _importRemains, _updatePhotos, _updateNameAndDescription, _downloadRemotePhoto, _onlyUpdateProducts, func);
                        else
                            PostProcess(productInStrings, PrepareModuleRow(csv), _modulesAndFields, _columnSeparator, _propertySeparator);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog($"{CommonStatistic.RowPosition}: {ex.Message}");
                            CommonStatistic.TotalErrorRow++;
                        });
                    }
                }
            }
        }

        private List<CsvProductFieldMapping> PrepareProductFields()
        {
            var productFields = new List<CsvProductFieldMapping>();

            foreach (ProductFields productField in Enum.GetValues(typeof(ProductFields)))
            {
                var name = productField.StrName();
                if (!_fieldMapping.TryGetValue(name, out var index))
                    continue;

                productFields.Add(new CsvProductFieldMapping()
                {
                    ProductFields = productField,
                    Name = name,
                    Index = index,
                    Status = productField.Status()
                });
            }

            return productFields;
        }

        private Dictionary<ProductFields, object> PrepareRow(List<CsvProductFieldMapping> productFields, IReader csv)
        {
            // Step by rows
            var productInStrings = new Dictionary<ProductFields, object>();

            foreach (var productField in productFields)
            {
                try
                {
                    switch (productField.Status)
                    {
                        case CsvFieldStatus.String:
                            GetString(productField, csv, productInStrings);
                            break;
                        case CsvFieldStatus.StringRequired:
                            GetStringRequired(productField, csv, productInStrings);
                            break;
                        case CsvFieldStatus.NotEmptyString:
                            GetStringNotNull(productField, csv, productInStrings);
                            break;
                        case CsvFieldStatus.Float:
                            if (!GetDecimal(productField, csv, productInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableFloat:
                            if (!GetNullableDecimal(productField, csv, productInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.Int:
                            if (!GetInt(productField, csv, productInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableInt:
                            if (!GetNullableInt(productField, csv, productInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.Long:
                            if (!GetLong(productField, csv, productInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableLong:
                            if (!GetNullableLong(productField, csv, productInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.DateTime:
                            if (!GetDateTime(productField, csv, productInStrings))
                                return null;
                            break;
                        case CsvFieldStatus.NullableDateTime:
                            if (!GetNullableDateTime(productField, csv, productInStrings))
                                return null;
                            break;
                    }
                }
                catch (MissingFieldException exception)
                {
                    DoForCommonStatistic(() => {
                        CommonStatistic.WriteLog($"Строка №{CommonStatistic.RowPosition}: Не валидный формат строки - пропущено поле {productField.ProductFields.Localize()}");
                        CommonStatistic.TotalErrorRow++;
                    });
                    return null;
                }
            }
            return productInStrings;
        }

        private Dictionary<CSVField, object> PrepareModuleRow(IReader csv)
        {
            var productInStrings = new Dictionary<CSVField, object>();
            foreach (var moduleFields in _modulesAndFields.Values)
            {
                foreach (var moduleField in moduleFields)
                {
                    var nameField = moduleField.StrName;
                    if (_fieldMapping.TryGetValue(nameField, out var value))
                        productInStrings.Add(moduleField, TrimAnyWay(csv[value]));
                }
            }
            return productInStrings;
        }

        private bool GetString(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            productInStrings.Add(field.ProductFields, TrimAnyWay(csv[field.Index]));
            return true;
        }

        private bool GetStringNotNull(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var tempValue = TrimAnyWay(csv[field.Index]);
            if (!string.IsNullOrEmpty(tempValue))
                productInStrings.Add(field.ProductFields, tempValue);

            return true;
        }

        private bool GetStringRequired(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (!string.IsNullOrEmpty(value))
                productInStrings.Add(field.ProductFields, value);
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.CanNotEmpty"), ProductFields.Name.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDecimal(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, 0f);
                return true;
            }

            if (float.TryParse(value, out var decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDecimal(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, default(float?));
                return true;
            }

            if (float.TryParse(value, out var decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
            {
                productInStrings.Add(field.ProductFields, decValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);
            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, 0);
                return true;
            }

            if (int.TryParse(value, out var intValue))
            {
                productInStrings.Add(field.ProductFields, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }
        
        private bool GetNullableInt(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);
            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, default(int?));
                return true;
            }

            if (int.TryParse(value, out var intValue))
            {
                productInStrings.Add(field.ProductFields, intValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }
        
        private bool GetLong(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);
            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, 0L);
                return true;
            }

            if (long.TryParse(value, out var longValue))
            {
                productInStrings.Add(field.ProductFields, longValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }
        
        private bool GetNullableLong(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);
            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, default(long?));
                return true;
            }

            if (long.TryParse(value, out var longValue))
            {
                productInStrings.Add(field.ProductFields, longValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeNumber"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDateTime(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);
            if (string.IsNullOrEmpty(value))
                value = default(DateTime).ToString(CultureInfo.InvariantCulture);

            if (DateTime.TryParse(value, out var dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetNullableDateTime(CsvProductFieldMapping field, IReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var value = TrimAnyWay(csv[field.Index]);

            if (string.IsNullOrEmpty(value))
            {
                productInStrings.Add(field.ProductFields, default(DateTime?));
                return true;
            }

            if (DateTime.TryParse(value, out var dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateValue))
            {
                productInStrings.Add(field.ProductFields, dateValue);
            }
            else
            {
                LogError(string.Format(LocalizationService.GetResource("Core.ExportImport.ImportCsv.MustBeDateTime"), field.ProductFields.Localize(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.SupperTrim();
        }

        private void LogError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
                //CommonStatistic.RowPosition++;
            });
        }

        private static bool useMultiThreadImport = false;

        public void UpdateInsertProduct(Dictionary<ProductFields, object> productInStrings, string columSeparator, string propertySeparator, bool skipOriginalPhoto, bool importRemains, bool updatePhotos, bool updateNameAndDescription, bool downloadRemotePhoto, bool onlyUpdateProducts, Func<Product, Product> func = null)
        {
            if (useMultiThreadImport)
            {
                var added = false;
                while (!added)
                {
                    ThreadPool.GetAvailableThreads(out var workerThreads, out var asyncIoThreads);
                    if (workerThreads != 0)
                    {
                        //ThreadPool.QueueUserWorkItem(UpdateInsertProductWorker, productInStrings);
                        Task.Factory.StartNew(() => UpdateInsertProductWorker(columSeparator, propertySeparator, productInStrings, skipOriginalPhoto, importRemains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, onlyUpdateProducts, func), TaskCreationOptions.LongRunning);
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
                UpdateInsertProductWorker(columSeparator, propertySeparator, productInStrings, skipOriginalPhoto, importRemains, updatePhotos, updateNameAndDescription, downloadRemotePhoto, onlyUpdateProducts, func);
            }
        }

        private void UpdateInsertProductWorker(string columSeparator, string propertySeparator, Dictionary<ProductFields, object> productInStrings, 
                                               bool skipOriginalPhoto, bool importRemains, bool updatePhotos, bool updateNameAndDescription, bool downloadRemotePhoto, 
                                               bool onlyUpdateProducts, Func<Product, Product> func = null)
        {
            try
            {
                bool addingNew;
                Product product = null;
                
                if (productInStrings.ContainsKey(ProductFields.Sku) && productInStrings[ProductFields.Sku].AsString().IsNullOrEmpty())
                    throw new Exception("SKU can not be empty");

                var artNo = productInStrings.ContainsKey(ProductFields.Sku) ? productInStrings[ProductFields.Sku].AsString().SupperTrim() : string.Empty;
                if (string.IsNullOrEmpty(artNo))
                {
                    addingNew = true;
                }
                else
                {
                    product = ProductService.GetProduct(artNo, withExportParams: true);
                    addingNew = product == null;
                }

                if (addingNew)
                {
                    if (onlyUpdateProducts)
                    {
                        productInStrings.Clear();
                        DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                        return;
                    }

                    product = new Product
                    {
                        ArtNo = string.IsNullOrEmpty(artNo) ? null : artNo,
                        Multiplicity = 1,
                        CurrencyID = _defaultCurrencyId,
                        Enabled = true
                    };
                }

                if (updateNameAndDescription || addingNew)
                {
                    if (productInStrings.ContainsKey(ProductFields.Name))
                        product.Name = productInStrings[ProductFields.Name].AsString();
                    else
                        product.Name = product.Name ?? string.Empty;
                }

                if (productInStrings.ContainsKey(ProductFields.Enabled))
                {
                    product.Enabled = productInStrings[ProductFields.Enabled].AsString().SupperTrim().Equals("+");
                }

                if (productInStrings.ContainsKey(ProductFields.Currency))
                {
                    var currency = CurrencyService.GetCurrencyByIso3(productInStrings[ProductFields.Currency].AsString().SupperTrim());
                    if (currency != null)
                        product.CurrencyID = currency.CurrencyId;
                    else
                        throw new Exception("Currency not found");
                }
                else if (addingNew)
                { 
                    var currencies = CurrencyService.GetAllCurrencies(true);
                    if (currencies == null || currencies.Count == 0)
                        throw new Exception("Currency not found");

                    var currency = currencies.FirstOrDefault(x => x.Rate == 1) ??
                                   currencies.FirstOrDefault(x => x.Iso3 == SettingsCatalog.DefaultCurrencyIso3);

                    if (currency != null)
                        product.CurrencyID = currency.CurrencyId;
                    else
                        throw new Exception("Currency not found");
                }


                if (productInStrings.ContainsKey(ProductFields.OrderByRequest))
                    product.AllowPreOrder = productInStrings[ProductFields.OrderByRequest].AsString().SupperTrim().Equals("+");
                
                if (productInStrings.ContainsKey(ProductFields.AccrueBonuses))
                    product.AccrueBonuses = productInStrings[ProductFields.AccrueBonuses].AsString().SupperTrim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.Discount) ||
                    productInStrings.ContainsKey(ProductFields.DiscountAmount))
                {
                    var percent = !productInStrings.ContainsKey(ProductFields.Discount) || productInStrings[ProductFields.Discount] == null
                        ? product.Discount.Percent
                        : productInStrings[ProductFields.Discount].AsFloat();

                    var amount = !productInStrings.ContainsKey(ProductFields.DiscountAmount) || productInStrings[ProductFields.DiscountAmount] == null
                        ? product.Discount.Amount
                        : productInStrings[ProductFields.DiscountAmount].AsFloat();

                    product.Discount = new Discount(percent, amount);
                }

                if (updateNameAndDescription || addingNew)
                {
                    if (productInStrings.ContainsKey(ProductFields.BriefDescription))
                    {
                        var descr = productInStrings[ProductFields.BriefDescription].AsString();
                        if (descr.IsLongerThan(ProductService.MaxDescLength))
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.TextLengthLimit", ProductFields.BriefDescription.Localize(), CommonStatistic.RowPosition + 2, ProductService.MaxDescLength));
                        else
                            product.BriefDescription = descr;
                    }

                    if (productInStrings.ContainsKey(ProductFields.Description))
                    {
                        var descr = productInStrings[ProductFields.Description].AsString();
                        if (descr.IsLongerThan(ProductService.MaxDescLength))
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.TextLengthLimit", ProductFields.Description.Localize(), CommonStatistic.RowPosition + 2, ProductService.MaxDescLength));
                        else
                            product.Description = descr;
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.SalesNotes))
                    product.ExportOptions.YandexSalesNote = productInStrings[ProductFields.SalesNotes].AsString();

                if (productInStrings.ContainsKey(ProductFields.Gtin))
                    product.ExportOptions.Gtin = productInStrings[ProductFields.Gtin].AsString();
                
                if (productInStrings.ContainsKey(ProductFields.GoogleMpn))
                    product.ExportOptions.Mpn = productInStrings[ProductFields.GoogleMpn].AsString();

                if (productInStrings.ContainsKey(ProductFields.GoogleProductCategory))
                    product.ExportOptions.GoogleProductCategory = productInStrings[ProductFields.GoogleProductCategory].AsString();

                if (productInStrings.ContainsKey(ProductFields.GoogleAvailabilityDate))
                {
                    var availabilityDate = productInStrings[ProductFields.GoogleAvailabilityDate].AsString();
                    product.ExportOptions.GoogleAvailabilityDate = !string.IsNullOrEmpty(availabilityDate)
                        ? availabilityDate.TryParseDateTime()
                        : default(DateTime?);
                }

                if (productInStrings.ContainsKey(ProductFields.YandexTypePrefix))
                    product.ExportOptions.YandexTypePrefix = productInStrings[ProductFields.YandexTypePrefix].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexModel))
                    product.ExportOptions.YandexModel = productInStrings[ProductFields.YandexModel].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexName))
                    product.ExportOptions.YandexName = productInStrings[ProductFields.YandexName].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexSizeUnit))
                    product.ExportOptions.YandexSizeUnit = productInStrings[ProductFields.YandexSizeUnit].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexDiscounted))
                    product.ExportOptions.YandexProductDiscounted = productInStrings[ProductFields.YandexDiscounted].AsString().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.YandexDiscountCondition))
                {
                    var yandexDiscountCondition = productInStrings[ProductFields.YandexDiscountCondition].AsString();
                    if (!string.IsNullOrEmpty(yandexDiscountCondition))
                    {
                        var condition = Enum.GetValues(typeof(EYandexDiscountCondition)).Cast<EYandexDiscountCondition>().FirstOrDefault(x => yandexDiscountCondition.Equals(x.StrName(), StringComparison.OrdinalIgnoreCase));
                        if (condition != EYandexDiscountCondition.None)
                            product.ExportOptions.YandexProductDiscountCondition = condition;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", ProductFields.YandexDiscountCondition.Localize(), yandexDiscountCondition, CommonStatistic.RowPosition + 2));
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.YandexDiscountReason))
                    product.ExportOptions.YandexProductDiscountReason = productInStrings[ProductFields.YandexDiscountReason].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexProductQuality))
                {
                    var yandexProductQuality = productInStrings[ProductFields.YandexProductQuality].AsString();
                    if (!string.IsNullOrEmpty(yandexProductQuality))
                    {
                        var quality = Enum.GetValues(typeof(EYandexProductQuality)).Cast<EYandexProductQuality>().FirstOrDefault(x => yandexProductQuality.Equals(x.StrName(), StringComparison.OrdinalIgnoreCase));
                        if (quality != EYandexProductQuality.None)
                            product.ExportOptions.YandexProductQuality = quality;
                        else
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", ProductFields.YandexProductQuality.Localize(), yandexProductQuality, CommonStatistic.RowPosition + 2));
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.YandexMarketExpiry))
                    product.ExportOptions.YandexMarketExpiry = productInStrings[ProductFields.YandexMarketExpiry].AsString();
                
                if (productInStrings.ContainsKey(ProductFields.YandexMarketWarrantyDays))
                    product.ExportOptions.YandexMarketWarrantyDays = productInStrings[ProductFields.YandexMarketWarrantyDays].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexMarketCommentWarranty))
                    product.ExportOptions.YandexMarketCommentWarranty = productInStrings[ProductFields.YandexMarketCommentWarranty].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexMarketPeriodOfValidityDays))
                    product.ExportOptions.YandexMarketPeriodOfValidityDays = productInStrings[ProductFields.YandexMarketPeriodOfValidityDays].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexMarketServiceLifeDays))
                    product.ExportOptions.YandexMarketServiceLifeDays = productInStrings[ProductFields.YandexMarketServiceLifeDays].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexMarketTnVedCode))
                    product.ExportOptions.YandexMarketTnVedCode = productInStrings[ProductFields.YandexMarketTnVedCode].AsString();

                if (productInStrings.ContainsKey(ProductFields.YandexMarketStepQuantity))
                    product.ExportOptions.YandexMarketStepQuantity = productInStrings[ProductFields.YandexMarketStepQuantity].AsNullableInt();
                
                if (productInStrings.ContainsKey(ProductFields.YandexMarketMinQuantity))
                    product.ExportOptions.YandexMarketMinQuantity = productInStrings[ProductFields.YandexMarketMinQuantity].AsNullableInt();
                
                if (productInStrings.ContainsKey(ProductFields.YandexMarketCategoryId))
                    product.ExportOptions.YandexMarketCategoryId = productInStrings[ProductFields.YandexMarketCategoryId].AsNullableLong();
                
                if (productInStrings.ContainsKey(ProductFields.Adult))
                    product.ExportOptions.Adult = productInStrings[ProductFields.Adult].AsString().SupperTrim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.ManufacturerWarranty))
                    product.ExportOptions.ManufacturerWarranty = productInStrings[ProductFields.ManufacturerWarranty].AsString().SupperTrim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.ShippingPrice))
                    product.ShippingPrice = productInStrings[ProductFields.ShippingPrice].AsNullableFloat();

                if (productInStrings.ContainsKey(ProductFields.YandexDeliveryDays))
                {
                    var yandexDeliveryDays = productInStrings[ProductFields.YandexDeliveryDays].AsString();
                    product.ExportOptions.YandexDeliveryDays = yandexDeliveryDays.Length > 5
                        ? yandexDeliveryDays.Substring(0, 5)
                        : yandexDeliveryDays;
                }

                if (productInStrings.ContainsKey(ProductFields.Unit))
                    product.UnitId = UnitService.UnitFromString(productInStrings[ProductFields.Unit].AsString());

                List<(Offer offer, float amount)> loadedAmounts = null;
                
                if (productInStrings.ContainsKey(ProductFields.MultiOffer))
                {
                    OfferService.OffersFromString(product, productInStrings[ProductFields.MultiOffer].AsString(), columSeparator, propertySeparator, importRemains, out loadedAmounts, out var newColorIds);
                    if (newColorIds != null)
                        _addedColorIds = _addedColorIds.Union(newColorIds).ToList();
                }
                else
                {
                    OfferService.OfferFromFields(product, productInStrings.ContainsKey(ProductFields.Price) ? productInStrings[ProductFields.Price].AsFloat() : (float?)null,
                                                 productInStrings.ContainsKey(ProductFields.PurchasePrice) ? productInStrings[ProductFields.PurchasePrice].AsFloat() : (float?)null,
                                                 productInStrings.ContainsKey(ProductFields.Amount) ? productInStrings[ProductFields.Amount].AsFloat() : (float?)null, importRemains, out var loadedAmount);
                    if (loadedAmount.IsNotDefault())
                        loadedAmounts = new List<(Offer offer, float amount)> {loadedAmount};
                }

                if (productInStrings.ContainsKey(ProductFields.ParamSynonym))
                {
                    var prodUrl = productInStrings[ProductFields.ParamSynonym].AsString().IsNotEmpty()
                                      ? productInStrings[ProductFields.ParamSynonym].AsString()
                                      : !string.IsNullOrEmpty(product.Name) ? product.Name : product.ArtNo;

                    product.UrlPath = UrlService.GetAvailableValidUrl(product.ProductId, ParamType.Product, prodUrl);
                }
                else
                {
                    product.UrlPath = product.UrlPath ??
                                      UrlService.GetAvailableValidUrl(product.ProductId, ParamType.Product,
                                          !string.IsNullOrEmpty(product.Name) ? product.Name : product.ArtNo);

                }

                if (_enabled301Redirects && productInStrings.ContainsKey(ProductFields.Url) && productInStrings[ProductFields.Url].AsString().IsNotEmpty())
                {
                    var redirectFrom = productInStrings[ProductFields.Url].AsString();
                    var redirectTo = $"{SettingsMain.SiteUrl}/products/{product.UrlPath}";
                    var redirect = RedirectSeoService.GetRedirectsSeoByRedirectFrom(redirectFrom);
                    if (redirect != null)
                    {
                        redirect.RedirectTo = redirectTo;
                        RedirectSeoService.UpdateRedirectSeo(redirect);
                    }
                    else if (!redirectTo.Equals(redirectFrom, StringComparison.OrdinalIgnoreCase))
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
                
                if (productInStrings.ContainsKey(ProductFields.Title) && string.IsNullOrEmpty(productInStrings[ProductFields.Title].AsString())
                    || productInStrings.ContainsKey(ProductFields.H1) && string.IsNullOrEmpty(productInStrings[ProductFields.H1].AsString()))
                    product.Meta = meta;
                else
                    product.Meta = new MetaInfo(0, product.ProductId, MetaType.Product,
                        (productInStrings.ContainsKey(ProductFields.Title) && productInStrings[ProductFields.Title].AsString().IsNotEmpty()
                            ? productInStrings[ProductFields.Title].AsString()
                            : meta.Title).DefaultOrEmpty(),
                        (productInStrings.ContainsKey(ProductFields.MetaKeywords)
                            ? productInStrings[ProductFields.MetaKeywords].AsString()
                            : meta.MetaKeywords).DefaultOrEmpty(),
                        (productInStrings.ContainsKey(ProductFields.MetaDescription)
                            ? productInStrings[ProductFields.MetaDescription].AsString()
                            : meta.MetaDescription).DefaultOrEmpty(),
                        (productInStrings.ContainsKey(ProductFields.H1) && productInStrings[ProductFields.H1].AsString().IsNotEmpty()
                            ? productInStrings[ProductFields.H1].AsString()
                            : meta.H1).DefaultOrEmpty());

                if (productInStrings.ContainsKey(ProductFields.Markers))
                    ProductService.MarkersFromString(product, productInStrings[ProductFields.Markers].AsString(), columSeparator);

                if (productInStrings.ContainsKey(ProductFields.Producer))
                    product.BrandId = BrandService.BrandFromString(productInStrings[ProductFields.Producer].AsString());

                if (productInStrings.ContainsKey(ProductFields.MinAmount))
                    product.MinAmount = productInStrings[ProductFields.MinAmount].AsFloat() != 0 ? productInStrings[ProductFields.MinAmount].AsFloat() : (float?)null;

                if (productInStrings.ContainsKey(ProductFields.MaxAmount))
                    product.MaxAmount = productInStrings[ProductFields.MaxAmount].AsFloat() != 0 ? productInStrings[ProductFields.MaxAmount].AsFloat() : (float?)null;

                if (productInStrings.ContainsKey(ProductFields.Multiplicity))
                    product.Multiplicity = productInStrings[ProductFields.Multiplicity].AsFloat() != 0 ? productInStrings[ProductFields.Multiplicity].AsFloat() : 1;

                if (productInStrings.ContainsKey(ProductFields.Bid))
                    product.ExportOptions.Bid = productInStrings[ProductFields.Bid].AsFloat() != 0 ? productInStrings[ProductFields.Bid].AsFloat() : 0;

                if (productInStrings.ContainsKey(ProductFields.IsMarkingRequired))
                    product.IsMarkingRequired = productInStrings[ProductFields.IsMarkingRequired].AsString().SupperTrim().Equals("+");
                
                if (productInStrings.ContainsKey(ProductFields.IsDigital))
                    product.IsDigital = productInStrings[ProductFields.IsDigital].AsString().SupperTrim().Equals("+");
                
                if (productInStrings.ContainsKey(ProductFields.DownloadLink))
                    product.DownloadLink = productInStrings[ProductFields.DownloadLink].AsString();

                if (productInStrings.ContainsKey(ProductFields.Tax))
                {
                    var taxName = productInStrings[ProductFields.Tax].AsString();
                    if (!string.IsNullOrEmpty(taxName))
                    {
                        var tax = TaxService.GetTaxes().FirstOrDefault(x => x.Name.ToLower() == taxName.ToLower());
                        if (tax != null)
                            product.TaxId = tax.TaxId;
                        else
                        {
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", 
                                ProductFields.Tax.Localize(), taxName, CommonStatistic.RowPosition + 2));
                        }
                    }
                    else
                    {
                        product.TaxId = null;
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.PaymentMethodType))
                {
                    var paymentMethodType = productInStrings[ProductFields.PaymentMethodType].AsString();
                    if (!string.IsNullOrEmpty(paymentMethodType))
                    {
                        if (Enum.TryParse<ePaymentMethodType>(paymentMethodType, out var type))
                            product.PaymentMethodType = type;
                        else
                        {
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound", 
                                ProductFields.PaymentMethodType.Localize(), paymentMethodType, CommonStatistic.RowPosition + 2));
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.PaymentSubjectType))
                {
                    var paymentSubjectType = productInStrings[ProductFields.PaymentSubjectType].AsString();
                    if (!string.IsNullOrEmpty(paymentSubjectType))
                    {
                        if (Enum.TryParse<ePaymentSubjectType>(paymentSubjectType, out var type))
                            product.PaymentSubjectType = type;
                        else
                        {
                            LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.FieldNotFound",
                                ProductFields.PaymentSubjectType.Localize(), paymentSubjectType, CommonStatistic.RowPosition + 2));
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.ManualRatio))
                {
                    var ratio = productInStrings[ProductFields.ManualRatio].AsFloat();
                    if(ratio > 0 && ratio <= 5)
                    {
                        product.ManualRatio = ratio;
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.Comment))
                    product.Comment = productInStrings[ProductFields.Comment].AsString();

                if (productInStrings.ContainsKey(ProductFields.DoNotApplyOtherDiscounts))
                {
                    product.DoNotApplyOtherDiscounts = productInStrings[ProductFields.DoNotApplyOtherDiscounts].AsString().Equals("+");
                }

                if (productInStrings.ContainsKey(ProductFields.SizeChart))
                {
                    var name = productInStrings[ProductFields.SizeChart].AsString();
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
                    {
                        product.Offers = OfferService.GetProductOffers(product.ProductId);
                        loadedAmounts = null;
                    }
                }

                if (!addingNew)
                {
                    ProductService.UpdateProduct(product, false, _trackChanges, _changedBy);

                    if (!_trackChanges)
                        ProductHistoryService.ProductChanged(product, _changedBy);

                    DoForCommonStatistic(() => CommonStatistic.TotalUpdateRow++);
                    Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductUpdated", product.ArtNo));
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_EditProduct_Csv);
                }
                else
                {
                    if (!(SaasDataService.IsSaasEnabled && ProductService.GetProductsCount("[Enabled] = 1") >= SaasDataService.CurrentSaasData.ProductsCount))
                    {
                        ProductService.AddProduct(product, false, true, _changedBy);
                        DoForCommonStatistic(() => CommonStatistic.TotalAddRow++);
                        Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductAdded", product.ArtNo));
                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ProductCreated_Csv);
                    }
                    else
                    {
                        Log(LocalizationService.GetResource("Core.ExportImport.ImportCsv.ProductsLimitRiched"));
                    }
                }

                if (product.ProductId > 0)
                {
                    if (loadedAmounts != null && _stocksToWarehouseId != null)
                    {
                        foreach (var (offer, amount) in loadedAmounts)
                        {
                            if (offer.OfferId <= 0)
                                continue;

                            WarehouseStocksService.AddUpdateStocks(
                                new WarehouseStock
                                {
                                    OfferId = offer.OfferId,
                                    WarehouseId = _stocksToWarehouseId.Value,
                                    Quantity = amount
                                },
                                trackChanges: _trackChanges && !addingNew,
                                _changedBy,
                                calcHasProductsForWarehouse: false);
                        }
                    }
                    
                    OtherFields(productInStrings, product.ProductId, columSeparator, propertySeparator,
                                skipOriginalPhoto, updatePhotos, downloadRemotePhoto, product.ArtNo, addingNew);
                    
                    ProductService.PreCalcProductParams(product.ProductId);
                }
                else
                {
                    LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProductNotAdded", product.ArtNo));
                }
            }
            catch (Exception e)
            {
                Debug.Log.Warn(e);
                LogError(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ProcessRowError", CommonStatistic.RowPosition + 2, e.Message));
            }

            productInStrings.Clear();
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }


        private void OtherFields(IDictionary<ProductFields, object> fields, int productId, string columSeparator, string propertySeparator, bool skipOriginalPhoto, bool updatePhotos, bool downloadRemotePhoto, string productArtNo, bool isNewProduct)
        {
            if (fields.ContainsKey(ProductFields.Category))
            {
                var sorting = string.Empty;
                if (fields.ContainsKey(ProductFields.Sorting))
                {
                    sorting = fields[ProductFields.Sorting].AsString();
                }
                var parentCategory = fields[ProductFields.Category].AsString();
                
                CategoryService.SubParseAndCreateCategory(parentCategory, productId, columSeparator, sorting, _changedBy, _trackChanges, _addProductInParentCategory, isNewProduct: isNewProduct);
            }

            if (fields.ContainsKey(ProductFields.ExternalCategoryId))
            {
                var sorting = fields.ContainsKey(ProductFields.Sorting)
                    ? fields[ProductFields.Sorting].AsString()
                    : string.Empty;

                var externalCategories = fields[ProductFields.ExternalCategoryId].AsString();
                CategoryService.AddProductToExternalCategories(externalCategories, productId, columSeparator, sorting);
            }

            if (fields.ContainsKey(ProductFields.Photos))
            {
                string photos = fields[ProductFields.Photos].AsString();
                if (!string.IsNullOrEmpty(photos))
                {
                    if (updatePhotos && !isNewProduct)
                    {
                        PhotoService.DeleteProductPhotos(productId);
                    }

                    if (!PhotoService.PhotosFromString(productId, photos, columSeparator, propertySeparator, out var errorsProcessPhoto, skipOriginalPhoto, downloadRemotePhoto))
                    {
                        foreach (var error in errorsProcessPhoto)
                            Log(error);
                        Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.NotAllPhotosProcessed", productArtNo));
                    }
                }
            }

            if (fields.ContainsKey(ProductFields.Videos))
            {
                var videos = fields[ProductFields.Videos].AsString();
                ProductVideoService.VideoFromString(productId, videos);
            }
            
            if (fields.ContainsKey(ProductFields.Properties))
            {
                var properties = fields[ProductFields.Properties].AsString();
                PropertyService.PropertiesFromString(productId, properties, columSeparator, propertySeparator, isNewProduct);
            }
            
            if (fields.ContainsKey(ProductFields.AvitoProductProperties))
            {
                var avitoProductProperties = fields[ProductFields.AvitoProductProperties].AsString();
                ExportFeedAvitoService.ImportProductProperties(productId, avitoProductProperties, columSeparator, propertySeparator);
            }

            if (fields.ContainsKey(ProductFields.CustomOption))
            {
                var customOption = fields[ProductFields.CustomOption].AsString();
                CustomOptionsService.CustomOptionsFromString(productId, customOption, isNewProduct);
            }

            if (fields.ContainsKey(ProductFields.Tags) 
                && (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags))
            {
                if (!isNewProduct)
                    TagService.DeleteMap(productId, ETagType.Product);

                var i = 0;

                foreach (var tagName in fields[ProductFields.Tags].AsString().Split(new[] { columSeparator }, StringSplitOptions.RemoveEmptyEntries))
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

        public void PostProcess(Dictionary<ProductFields, object> productInStrings, Dictionary<CSVField, object> moduleFieldValues, Dictionary<ICSVExportImport, List<CSVField>> modulesAndFields, string columnSeparator, string propertySeparator)
        {
            if (productInStrings.ContainsKey(ProductFields.Sku))
            {
                var artNo = productInStrings[ProductFields.Sku].AsString();
                var productId = ProductService.GetProductId(artNo);
                if (productId == 0)
                    return;

                //relations
                if (productInStrings.ContainsKey(ProductFields.Related))
                {
                    var linkproducts = productInStrings[ProductFields.Related].AsString();
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Related, columnSeparator);
                }

                //relations
                if (productInStrings.ContainsKey(ProductFields.Alternative))
                {
                    var linkproducts = productInStrings[ProductFields.Alternative].AsString();
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Alternative, columnSeparator);
                }

                //gifts
                if (productInStrings.ContainsKey(ProductFields.Gifts))
                {
                    _useMassPrecalc = true;
                    var linkproducts = productInStrings[ProductFields.Gifts].AsString();
                    ProductGiftService.GiftsFromString(productId, linkproducts, columnSeparator);
                }

                var productOffers = ((productInStrings.ContainsKey(ProductFields.Weight) || productInStrings.ContainsKey(ProductFields.Size)) && !SettingsCatalog.EnableOfferWeightAndDimensions) ||
                                    (productInStrings.ContainsKey(ProductFields.BarCode) && !SettingsCatalog.EnableOfferBarCode)
                        ? OfferService.GetProductOffers(productId)
                        : null;
                bool changedProductOffers = false;


                if (productInStrings.ContainsKey(ProductFields.Weight))
                {
                    if (!SettingsCatalog.EnableOfferWeightAndDimensions)
                    {
                        var weight = productInStrings[ProductFields.Weight].AsFloat();

                        foreach (var offer in productOffers)
                        {
                            offer.Weight = weight;
                            changedProductOffers = true;
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.Size))
                {
                    if (!SettingsCatalog.EnableOfferWeightAndDimensions)
                    {
                        var dimensions = productInStrings[ProductFields.Size].AsString().Split(new[] { '|', 'x', 'х' }).Select(x => x.TryParseFloat()).ToList();

                        var length = dimensions.Count > 0 ? dimensions[0] : 0;
                        var width = dimensions.Count > 1 ? dimensions[1] : 0;
                        var height = dimensions.Count > 2 ? dimensions[2] : 0;

                        foreach (var offer in productOffers)
                        {
                            offer.Length = length;
                            offer.Width = width;
                            offer.Height = height;

                            changedProductOffers = true;
                        }
                    }
                }

                if (productInStrings.ContainsKey(ProductFields.BarCode))
                {
                    if (!SettingsCatalog.EnableOfferBarCode)
                    {
                        var barCode = productInStrings[ProductFields.BarCode].AsString();

                        foreach (var offer in productOffers)
                        {
                            offer.BarCode = barCode;
                            changedProductOffers = true;
                        }
                    }
                }

                if (changedProductOffers)
                    foreach (var offer in productOffers)
                        OfferService.UpdateOffer(offer);

                // modules
                foreach (var moduleInstance in modulesAndFields.Keys)
                {
                    foreach (var moduleField in modulesAndFields[moduleInstance].Where(moduleFieldValues.ContainsKey))
                    {
                        try
                        {
                            if (!moduleInstance.ProcessField(moduleField, productId, moduleFieldValues[moduleField].AsString(), columnSeparator, propertySeparator))
                                Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ModuleError", artNo, moduleInstance.ModuleStringId));
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error("Csv import error during post-processing of module fields", ex);
                            Log(LocalizationService.GetResourceFormat("Core.ExportImport.ImportCsv.ModuleError", artNo, moduleInstance.ModuleStringId));
                        }
                    }
                }
            }
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }

        public class CsvProductFieldMapping
        {
            public ProductFields ProductFields { get; set; }

            public string Name { get; set; }

            public CsvFieldStatus Status { get; set; }
            public int Index { get; set; }
        }
    }
}