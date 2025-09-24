//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Core;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Shipping
{  
    public class ShippingManager
    {
        protected readonly ShippingCalculationParameters CalculationParameters;
        protected bool ShippingOptionFromParameters;
        public int? TimeLimitMilliseconds { get; set; }

        public ShippingManager(Func<IConfiguratorShippingCalculation, ShippingCalculationParameters> shippingCalculationConfiguration) 
            : this(shippingCalculationConfiguration(ShippingCalculationConfigurator.Configure()))
        {
        }

        public ShippingManager(ShippingCalculationParameters calculationParameters)
        {
            CalculationParameters = calculationParameters;
#if !DEBUG
            TimeLimitMilliseconds = 10_000; // 10 seconds
#endif
        }

        public ShippingManager PreferShippingOptionFromParameters()
        {
            ShippingOptionFromParameters = true;
            return this;
        }

        public ShippingManager WithoutPreferShippingOptionFromParameters()
        {
            ShippingOptionFromParameters = false;
            return this;
        }

        private CancellationTokenSource GetCancellationTokenSource()
        {
            var cts = new CancellationTokenSource();
            if (TimeLimitMilliseconds.HasValue)
                cts.CancelAfter(TimeLimitMilliseconds.Value);

            return cts;
        }
        
        public List<BaseShippingOption> GetOptions(CalculationVariants calculationVariants = CalculationVariants.All)
        {
            using (var cts = GetCancellationTokenSource())
            {
                var (result, availableMethods) =
                    CallShippingMethodsAsync((shippingMethod, shipping) =>
                        {
                            var options = shipping.CalculateOptions(calculationVariants)?.ToList();
                            if (options is null)
                                return null;
            
                            if (CalculationParameters.Currency != null)
                                options.ForEach(x => x.CurrentCurrency = CalculationParameters.Currency);

                            ProcessOptionsByModules(options);

                            if (SettingsCheckout.HideShippingNotAvailableForWarehouse)
                                return FilterByAvailableStocks(options);

                            CheckAvailableStocks(options);
                            return options;
                            
                        }, HttpContext.Current, cts.Token)
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult();
                
                var items = result
                           .Where(x => x != null)
                           .SelectMany(x => x)
                           .Where(x => x != null)
                           .ToList();
                
                return ProcessOptions(items, availableMethods);
            }
        }

        public List<BaseShippingOption> GetOptionsToPoint(string pointId)
        {
            using (var cts = GetCancellationTokenSource())
            {
                var (result, availableMethods) =
                    CallShippingMethodsAsync((shippingMethod, shipping) =>
                        {
                            var options = shipping.CalculateOptionsToPoint(pointId)?.ToList();
                            if (options is null)
                                return null;
         
                            if (CalculationParameters.Currency != null)
                                options.ForEach(x => x.CurrentCurrency = CalculationParameters.Currency);

                            ProcessOptionsByModules(options);

                            if (SettingsCheckout.HideShippingNotAvailableForWarehouse)
                                return FilterByAvailableStocks(options);

                            CheckAvailableStocks(options);
                            return options;
                            
                        }, HttpContext.Current, cts.Token)
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult();
                
                var items = result
                           .Where(x => x != null)
                           .SelectMany(x => x)
                           .Where(x => x != null)
                           .ToList();
                
                return ProcessOptions(items, availableMethods);
            }
        }

        public List<ShippingPointsResult> GetShippingPoints(float[,] coordinatesOfBounds, TypeBonds typeBonds, bool filterByAvailableStocks)
        {
            if (coordinatesOfBounds.GetLength(0) != 2
                || coordinatesOfBounds.GetLength(1) != 2)
                throw new ArgumentException("Возможен только массив float[2,2].", nameof(coordinatesOfBounds));

            using (var cts = GetCancellationTokenSource())
            {
                var (result, availableMethods) =
                    CallShippingMethodsAsync((shippingMethod, shipping) =>
                            new ShippingPointsResult
                            {
                                MethodId = shippingMethod.ShippingMethodId,
                                Points = shipping.CalculateShippingPoints(coordinatesOfBounds, typeBonds)
                            }, 
                            HttpContext.Current, 
                            cts.Token)
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult();

                var items = result
                           .Where(x => x?.Points?.Any() is true)
                           .ToList();

                ShippingMethod method;

                // в списке присутствуют доставки без флага "Показывать только при отсутствии других доставок"
                if (!items.All(o =>
                        (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == o.MethodId)) != null
                        && method.ShowIfNoOtherShippings))
                {
                    items = items
                        .Where(o =>
                             (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == o.MethodId)) == null
                             || method.ShowIfNoOtherShippings == false)
                        .ToList();
                }

                return filterByAvailableStocks
                    ? FilterAvailableStocks(items)
                    : items;
            }
        }
        
        public List<ShippingPointResult> GetShippingPointInfo(string pointId)
        {
            using (var cts = GetCancellationTokenSource())
            {
                var (result, _) =
                    CallShippingMethodsAsync((shippingMethod, shipping) =>
                                new ShippingPointResult
                                {
                                    MethodId = shippingMethod.ShippingMethodId,
                                    Point = shipping.GetShippingPointInfo(pointId)
                                }, 
                            HttpContext.Current,
                            cts.Token)
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult();

                return result
                      .Where(x => x?.Point != null)
                      .ToList();
            }
        }
        
        protected async Task<(List<T> result, List<ShippingMethod> outAvailableMethods)> CallShippingMethodsAsync<T>(Func<ShippingMethod, BaseShipping, T> callFunc, HttpContext httpContext, CancellationToken cancellationToken)
        {
            var availableMethods = GetAvailableMethods();

            var context = httpContext;

            var tasks = availableMethods
               .Select(shippingMethod => cancellationToken.CancelIfRequestedAsync<T>() ?? Task.Run(() =>
                    {
                        try
                        {
                            using (cancellationToken.Register(Thread.CurrentThread.Abort))
                            {
                                HttpContext.Current = context;

                                if (!CheckShippingOnGeoFilter(shippingMethod)
                                    || !CheckShippingOnCatalogFilter(shippingMethod))
                                    return default;

                                var type = ReflectionExt.GetTypeByAttributeValue<ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, shippingMethod.ShippingType);
                                //todo Пересмотреть передачу параметров, чтобы не требовалось DeepCloneJson
                        
                                var shipping = (BaseShipping)Activator.CreateInstance(type, shippingMethod, CalculationParameters.DeepCloneJson(Newtonsoft.Json.TypeNameHandling.All));

                                return callFunc(shippingMethod, shipping);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error(ex);
                            return default(T);
                        }
                    }, 
                    cancellationToken))
                .ToList();

            Task.WhenAll(tasks)
                 // чтобы не получить ошибку из-за прерванных/отмененных задач (без try catch)
                .ContinueWith(task => { /* mute */ })
                .Wait();

            var items = tasks
                       .Where(x => x.IsCompleted)
                       .Where(x => !x.IsFaulted)
                       .Where(x => !x.IsCanceled)
                       .Select(x => x.Result)
                       .ToList();
            
            return (items, availableMethods);
        }

        protected virtual List<BaseShippingOption> ProcessOptions(List<BaseShippingOption> items, List<ShippingMethod> availableMethods)
        {
            foreach (var baseShippingOption in items)
            {
                if (baseShippingOption.TemplateName.IsNotEmpty())
                {
                    baseShippingOption.Template = new AssetsTool.PathData(CalculationParameters.IsFromAdminArea ? "Admin" : null).GetPathByOriginalFileName(baseShippingOption
                       .TemplateName);  
                }
            }
            
            if (CalculationParameters.Currency != null)
                items.ForEach(x => x.CurrentCurrency = CalculationParameters.Currency);

            ShippingMethod method;
            var orderedItems = items.OrderBy(option =>
                (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == option.MethodId)) != null && method.MoveToEnd ? 1 : 0);

            if (SettingsCheckout.TypeSortOrderShippings == TypeSortOrderShippings.AscByRate)
                orderedItems = orderedItems.ThenBy(option => option.FinalRate);
            else if (SettingsCheckout.TypeSortOrderShippings == TypeSortOrderShippings.DescByRate)
                orderedItems = orderedItems.ThenByDescending(option => option.FinalRate);

            items = orderedItems.ToList();

            // в списке только одни "Показывать только при отсутствии других доставок"
            if (items.All(o =>
                    (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == o.MethodId)) != null
                    && method.ShowIfNoOtherShippings))
            {
                return items;
            }

            // в списке присутствуют доставки без флага "Показывать только при отсутствии других доставок"
            items = items
                   .Where(o =>
                        (method = availableMethods.FirstOrDefault(ship => ship.ShippingMethodId == o.MethodId)) == null
                        || method.ShowIfNoOtherShippings == false)
                   .ToList();
            
            return items;
        }

        private void ProcessOptionsByModules(List<BaseShippingOption> items)
        {
            var modules = AttachedModules.GetModuleInstances<IShippingCalculator>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                    module.ProcessOptions(items, CalculationParameters.PreOrderItems, CalculationParameters.ItemsTotalPriceWithDiscounts);
            }
        }

        protected virtual List<ShippingMethod> GetAvailableMethods()
        {
            var listMethods = ShippingMethodService.GetAllShippingMethods(true);
            listMethods = GetAvailableSaasMethods(listMethods);
            return GetAvailableMethods(listMethods);
        }

        protected virtual List<ShippingMethod> GetAvailableMethods(List<ShippingMethod> listMethods)
        {
            var availableOnlyForDigitalProduct = CalculationParameters.PreOrderItems.All(x => x.IsDigital);
            availableOnlyForDigitalProduct = availableOnlyForDigitalProduct && listMethods.Any(x => x.OnlyForDigitalProduct);
            var availableMethods =
                listMethods
                   .Where(x => CalculationParameters.ShowOnlyInDetails is false || x.ShowInDetails)
                   .Where(x => ShippingOptionFromParameters is false || CalculationParameters.ShippingOption == null
                                      || x.ShippingMethodId == CalculationParameters.ShippingOption.MethodId)
                   .Where(x => CalculationParameters.PreOrderItems.Count == 0 
                            || !availableOnlyForDigitalProduct && !x.OnlyForDigitalProduct 
                            || availableOnlyForDigitalProduct && x.OnlyForDigitalProduct)
                   .ToList();

            if (availableMethods.Any(x => x.ModuleStringId.IsNotEmpty()))
            {
                var activeShippingModules =
                    AttachedModules.GetModules<IShippingMethod>()
                                   .Where(module => module != null)
                                   .Select(module => (IShippingMethod) Activator.CreateInstance(module))
                                   .Select(module => module.ModuleStringId)
                                   .ToList();

                availableMethods =
                    availableMethods
                       .Where(x => x.ModuleStringId.IsNullOrEmpty() || activeShippingModules.Contains(x.ModuleStringId))
                       .ToList();
            }

            return availableMethods;
        }

        protected List<ShippingMethod> GetAvailableSaasMethods(List<ShippingMethod> listMethods)
        {
            if (Saas.SaasDataService.IsSaasEnabled
                && Saas.SaasDataService.CurrentSaasData.AvailableShippingTypesList?.Count > 0)
            {
                listMethods = listMethods
                             .Where(method =>
                                  method.ModuleStringId.IsNullOrEmpty()
                                  && Saas.SaasDataService.CurrentSaasData.AvailableShippingTypesList
                                         .Contains(method.ShippingType, StringComparer.OrdinalIgnoreCase))
                             .ToList();
            }

            return listMethods;
        }

        protected virtual bool CheckShippingOnGeoFilter(ShippingMethod shippingMethod)
        {
            var validGeo = false;
            if (ShippingPaymentGeoMaping.IsExistGeoShipping(shippingMethod.ShippingMethodId))
            {
                if (ShippingPaymentGeoMaping.CheckShippingEnabledGeo(
                        shippingMethod.ShippingMethodId,
                        CalculationParameters.Country,
                        CalculationParameters.Region,
                        CalculationParameters.City,
                        CalculationParameters.District))
                    validGeo = true;
            }
            else
                validGeo = true;

            return validGeo;
        }

        protected virtual bool CheckShippingOnCatalogFilter(ShippingMethod shippingMethod)
        {
            var validCatalog = false;
            if (ShippingCatalogMaping.IsExistLinksToShipping(shippingMethod.ShippingMethodId))
            {
                var productIds =
                    CalculationParameters.PreOrderItems
                                         .Where(x => x.ProductId.HasValue)
                                         .Select(x => x.ProductId.Value)
                                         .Distinct()
                                         .ToArray();

                if (ShippingCatalogMaping.CheckShippingEnabled(shippingMethod.ShippingMethodId, productIds))
                    validCatalog = true;
            }
            else
                validCatalog = true;

            return validCatalog;
        }

        private void CheckAvailableStocks(IEnumerable<BaseShippingOption> items)
        {
            var availableStocksByMethodId = new Dictionary<int, bool>();
            var groupItems = GetPreOrderItemsGroupByArtNo();

            var errorMessage = LocalizationService.GetResource("Core.Shippings.ShippingOption.Error.HaveNotAvailableItems");
            items
                .Where(shippingOption => !CheckShippingOptionForItems(groupItems, shippingOption, availableStocksByMethodId))
                .ForEach(x => x.ErrorMessage = errorMessage);
        }

        private IEnumerable<BaseShippingOption> FilterByAvailableStocks(IEnumerable<BaseShippingOption> items)
        {
            var availableStocksByMethodId = new Dictionary<int, bool>();
            var groupItems = GetPreOrderItemsGroupByArtNo();

            return items.Where(shippingOption => CheckShippingOptionForItems(groupItems, shippingOption, availableStocksByMethodId));
        }

        private List<(string ArtNo, int? ProductId, float Amount)> _preOrderItemsGroupByArtNo;
        private List<(string ArtNo, int? ProductId, float Amount)> GetPreOrderItemsGroupByArtNo() => 
            _preOrderItemsGroupByArtNo ?? (_preOrderItemsGroupByArtNo = 
                CalculationParameters.PreOrderItems
                    .GroupBy(item => (item.ArtNo, item.ProductId))// группируем позиции, на случай если в корзине товар с доп. опциями и без, или подарки (идут по отдельности)
                    .Select(group => (group.Key.ArtNo, group.Key.ProductId,
                            Amount: group.Sum(x => x.Amount)))
                    .ToList());

        private bool CheckShippingOptionForItems(List<(string ArtNo, int? ProductId, float Amount)> groupItems, BaseShippingOption shippingOption, Dictionary<int, bool> availableStocksByMethodId)
        {
            var checkWarehouses = shippingOption.GetOrderPickPoint()?.WarehouseIds;

            if (checkWarehouses?.Count > 0)
            {
                var available = groupItems.All(
                    preOrderItem =>
                        WarehouseStocksService.GetStocksInWarehouses(
                            preOrderItem.ProductId,
                            preOrderItem.ArtNo,
                            checkWarehouses) >= preOrderItem.Amount);
                if (!available)
                    return false;
            }
            else
            {
                if (!availableStocksByMethodId.ContainsKey(shippingOption.MethodId))
                {
                    checkWarehouses = shippingOption.Warehouses;
                    if (checkWarehouses is null
                        || checkWarehouses.Count == 0)
                        availableStocksByMethodId.Add(shippingOption.MethodId, true);
                    else
                        availableStocksByMethodId.Add(
                        shippingOption.MethodId,
                            groupItems.All(
                                preOrderItem =>
                                    WarehouseStocksService.GetStocksInWarehouses(
                                        preOrderItem.ProductId,
                                        preOrderItem.ArtNo,
                                        checkWarehouses) >= preOrderItem.Amount));
                }

                if (availableStocksByMethodId[shippingOption.MethodId] is false)
                    return false;
            }
            return true;
        }

        private List<ShippingPointsResult> FilterAvailableStocks(List<ShippingPointsResult> items)
        {
            var availableStocksByMethodId = new Dictionary<int, bool>();
            var availableStocksByWarehouseId = new Dictionary<int, bool>();

            var groupItems = GetPreOrderItemsGroupByArtNo();

            foreach (var pointsResult in items)
            {
                List<BaseShippingPoint> removePoints = null;
                var pointsResultPoints = pointsResult.Points.ToList();
                foreach (var point in pointsResultPoints.Where(p => p.WarehouseId.HasValue))
                {
                    if (!availableStocksByWarehouseId.ContainsKey(point.WarehouseId.Value))
                        availableStocksByWarehouseId.Add(
                            point.WarehouseId.Value,
                            groupItems.All(
                                preOrderItem =>
                                    WarehouseStocksService.GetStocksInWarehouses(
                                        preOrderItem.ProductId,
                                        preOrderItem.ArtNo,
                                        new List<int> {point.WarehouseId.Value}) >= preOrderItem.Amount));

                    if (availableStocksByWarehouseId[pointsResult.MethodId] is false)
                        (removePoints ?? (removePoints = new List<BaseShippingPoint>())).Add(point);
                }
                
                if (removePoints?.Count > 0)
                    foreach (var point in removePoints)
                        pointsResultPoints.Remove(point);

                pointsResult.Points = pointsResultPoints;
            }

            List<int> checkWarehouses;
            foreach (var pointsResult in items)
            {
                checkWarehouses = ShippingWarehouseMappingService.GetByMethod(pointsResult.MethodId);
                if (checkWarehouses is null
                    || checkWarehouses.Count == 0)
                    availableStocksByMethodId.Add(pointsResult.MethodId, true);
                else
                    availableStocksByMethodId.Add(
                        pointsResult.MethodId,
                        groupItems.All(
                            preOrderItem =>
                                WarehouseStocksService.GetStocksInWarehouses(
                                    preOrderItem.ProductId,
                                    preOrderItem.ArtNo,
                                    checkWarehouses) >= preOrderItem.Amount));

                if (availableStocksByMethodId[pointsResult.MethodId] is false)
                    pointsResult.Points = Enumerable.Empty<BaseShippingPoint>();
            }

            return items
               .Where(x => x.Points?.Any() is true)
                .ToList();
        }

        public override int GetHashCode()
        {
            return CalculationParameters.GetHashCode();
        }
    }
}
