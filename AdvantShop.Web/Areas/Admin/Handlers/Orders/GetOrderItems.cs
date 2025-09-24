using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL2;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetOrderItems
    {
        private OrderItemsFilterModel _filterModel;
        private readonly UrlHelper _urlHelper;
        private SqlPaging _paging;

        public GetOrderItems(OrderItemsFilterModel model)
        {
            _filterModel = model;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public FilterResult<OrderItemModel> Execute()
        {
            var model = new FilterResult<OrderItemModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено товаров: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
                return model;
            
            model.DataItems = new List<OrderItemModel>();

            var order = OrderService.GetOrder(_filterModel.OrderId);
            var currency = order?.OrderCurrency?? CurrencyService.CurrentCurrency;
            var pageItems = _paging.PageItemsList<OrderItem>();
            
            var cacheOffers = new Dictionary<string, Offer>();
            var cacheOfferStocks = new Dictionary<int, List<WarehouseStock>>();
            var currentDistributeOrder =
                order != null
                    ? DistributionOfOrderItemService.GetDistributionOfOrderItems(order.OrderID)
                                  .GroupBy(item => item.OrderItemId)
                                  .ToDictionary(group => group.Key, group => group.ToList())
                    : null;
            var warehouses = WarehouseService.GetList();
            var cacheOfferStocksNoChanged = new Dictionary<int, List<WarehouseStock>>();

            foreach(var pageItem in pageItems)
            {
                var p = pageItem.ProductID != null ? ProductService.GetProduct(pageItem.ProductID.Value) : null;
                var selectedOptions = OrderService.GetOrderCustomOptionsByOrderItemId(pageItem.OrderItemID);
                var showEditCustomOptions = p != null && (selectedOptions != null && selectedOptions.Count > 0 ||
                                                          CustomOptionsService.GetCustomOptionsByProductId(p.ProductId).Any());
                var modelItem = new OrderItemModel()
                {
                    OrderItemId = pageItem.OrderItemID,
                    OrderId = pageItem.OrderID,
                    ImageSrc = pageItem.Photo.ImageSrcSmall(),
                    ArtNo = pageItem.ArtNo,
                    Name = pageItem.Name,
                    ProductId = p != null ? p.ProductId : default(int?),
                    ProductLink = p != null ? _urlHelper.AbsoluteActionUrl("Edit", "Product", new { id = p.ProductId }) : null,

                    Color = !string.IsNullOrEmpty(pageItem.Color) ? SettingsCatalog.ColorsHeader + ": " + pageItem.Color : "",
                    Size = !string.IsNullOrEmpty(pageItem.Size) ? SettingsCatalog.SizesHeader + ": " + pageItem.Size : "",
                    CustomOptions = selectedOptions != null ? RenderCustomOptions(selectedOptions, currency) : null,
                    ShowEditCustomOptions = showEditCustomOptions,

                    Price = pageItem.Price,
                    Amount = pageItem.Amount,
                    Cost = PriceService.SimpleRoundPrice(pageItem.Price * pageItem.Amount, currency).FormatPrice(currency),
                    Width = pageItem.Width,
                    Height = pageItem.Height,
                    Length = pageItem.Length,
                    Enabled = p != null ? p.Enabled : false,
                    Weight = pageItem.Weight,
                    BarCode = pageItem.BarCode,
                    
                    IsCustomPrice = pageItem.IsCustomPrice,
                    BasePrice = pageItem.BasePrice,
                    DiscountPercent = pageItem.DiscountPercent,
                    DiscountAmount = pageItem.DiscountAmount,
                    PriceWhenOrdering = (pageItem.BasePrice ?? 0).FormatPrice(currency),
                    DiscountWhenOrdering = pageItem.DiscountPercent != 0 
                        ? pageItem.DiscountPercent + "%"
                        : pageItem.DiscountAmount.FormatPrice(currency),
                    
                    UnitName = pageItem.Unit.IsNullOrEmpty() ? "-" : pageItem.Unit,
                    MeasureName = pageItem.MeasureType?.Localize() ?? "-",
                    PaymentSubjectName = pageItem.PaymentSubjectType.Localize() ?? "-",
                    PaymentMethodName = pageItem.PaymentMethodType.Localize() ?? "-",
                    DownloadLink = pageItem.DownloadLink.IsNullOrEmpty() ? string.Empty : pageItem.DownloadLink,
                    CountWarehouses = warehouses.Count
                };

                if (pageItem.TypeItem == TypeOrderItem.Product)
                    SetProductAvailabilityStatus(modelItem, pageItem, order, cacheOffers, cacheOfferStocks,
                        currentDistributeOrder, warehouses);
                else if (pageItem.TypeItem == TypeOrderItem.BookingService) 
                    modelItem.Available = true;

                if (FeaturesService.IsEnabled(EFeature.MarkingRequired) && pageItem.IsMarkingRequired)
                {
                    var items = MarkingOrderItemService.GetMarkingItems(pageItem.OrderItemID);
                    modelItem.MarkingRequiredValidation =
                        items.Count(x => !string.IsNullOrWhiteSpace(x.Code)) == (int) Math.Round(pageItem.Amount, MidpointRounding.AwayFromZero) && 
                        pageItem.Amount > 0;
                }

                if (pageItem.TypeItem == TypeOrderItem.Product
                    && _filterModel.ShowColumns?.Stocks is true)
                {
                    SetItemStocks(modelItem, pageItem, cacheOffers, cacheOfferStocksNoChanged, warehouses);
                }

                model.DataItems.Add(modelItem);
            };
            
            return model;
        }

        private static void SetProductAvailabilityStatus(OrderItemModel modelItem, OrderItem pageItem, Order order,
            Dictionary<string, Offer> cacheOffers, Dictionary<int, List<WarehouseStock>> cacheOfferStocks, Dictionary<int, List<DistributionOfOrderItem>> currentDistributeOrder, List<Warehouse> warehouses)
        {
            Offer offer;
            if (cacheOffers.ContainsKey(pageItem.ArtNo))
                offer = cacheOffers[pageItem.ArtNo];
            else
            {
                offer = OfferService.GetOffer(pageItem.ArtNo);
                cacheOffers.Add(pageItem.ArtNo, offer);
            }

            if (offer == null)
            {
                modelItem.Available = false;
                modelItem.AvailableText.Add(
                    string.Format(
                        LocalizationService.GetResource("Admin.Orders.GetOrderItems.AvailableLimit"),
                        0f));
            }
            else
            {
                if (order.Decremented
                    || currentDistributeOrder.Count > 0)
                {
                    // если списывали позиции то выводим доступное кол-во по складам
                    SetAvailabilityStatusByWarehouse(modelItem, pageItem, cacheOfferStocks, currentDistributeOrder,
                        warehouses, offer);
                }
                else
                    // если не списывали позиции то выводим доступное кол-во в общем
                    SetCommonAvailabilityStatus(modelItem, pageItem, offer);
            }
        }

        private static void SetAvailabilityStatusByWarehouse(OrderItemModel modelItem, OrderItem pageItem,
            Dictionary<int, List<WarehouseStock>> cacheOfferStocks, Dictionary<int, List<DistributionOfOrderItem>> currentDistributeOrder, List<Warehouse> warehouses, Offer offer)
        {
            currentDistributeOrder.TryGetValue(pageItem.OrderItemID, out var currentDistributionOfOrderItem);

            if (currentDistributionOfOrderItem is null)
            {
                SetCommonAvailabilityStatus(modelItem, pageItem, offer);
                return;
            }

            if (!cacheOfferStocks.ContainsKey(offer.OfferId))
                cacheOfferStocks.Add(offer.OfferId, WarehouseStocksService.GetOfferStocks(offer.OfferId));

            var offerStocks = cacheOfferStocks[offer.OfferId];

            foreach (var warehouseDistributed in currentDistributionOfOrderItem)
            {
                if (warehouseDistributed.Amount <= 0f)
                    continue;
                
                var warehouseStock =
                    offerStocks.Find(offerStock => offerStock.WarehouseId == warehouseDistributed.WarehouseId);

                var startAmount = warehouseStock?.Quantity ?? 0f;
                var warehouseStockQuantity = warehouseStock?.Quantity ?? 0f;
                if (warehouseStock != null)
                {
                    // виртуально резервируем на позицию остатки.
                    // если есть еще позиции по этой модификации,
                    // то проверка будет идти уже с учетом предыдущих позиций
                    warehouseStock.Quantity = warehouseStock.Quantity - warehouseDistributed.Amount
                                                                       + warehouseDistributed.DecrementedAmount;
                    warehouseStockQuantity = warehouseStock.Quantity;
                }
                else
                {
                    warehouseStockQuantity = /*warehouseStock.Quantity*/ 0f - warehouseDistributed.Amount
                                                                         + warehouseDistributed.DecrementedAmount;
                }

                if (warehouseStockQuantity < 0f)
                {
                    modelItem.Available = false;
                    modelItem.AvailableText.Add(
                        string.Format(
                            LocalizationService.GetResource("Admin.Orders.GetOrderItems.WarehouseAvailableLimit"),
                            warehouses.Find(warehouse => warehouse.Id == warehouseDistributed.WarehouseId)?.Name,
                            Math.Abs(warehouseStockQuantity) < warehouseDistributed.Amount ? Math.Abs(warehouseStockQuantity) : warehouseDistributed.Amount /*startAmount + warehouseDistributed.DecrementedAmount*/,
                            warehouseDistributed.Amount));
                }
                else if (modelItem.AvailableText.Count == 0)
                {
                    modelItem.Available = true;
                    // modelItem.AvailableText =
                    //     LocalizationService.GetResource("Admin.Orders.GetOrderItems.Available");
                }
            }
        }

        private static void SetCommonAvailabilityStatus(OrderItemModel modelItem, OrderItem pageItem, Offer offer)
        {
            if (pageItem.Amount <= 0f)
            {
                modelItem.Available = true;
                return;
            }

            var startAmount = offer.Amount;
            // виртуально резервируем на позицию остатки.
            // если есть еще позиции по этой модификации,
            // то проверка будет идти уже с учетом предыдущих позиций
#pragma warning disable CS0618 // Type or member is obsolete
            offer.SetAmount(offer.Amount - pageItem.Amount + pageItem.DecrementedAmount);
#pragma warning restore CS0618 // Type or member is obsolete

            if (offer.Amount < 0f && pageItem.Amount > 0f)
            {
                modelItem.Available = false;
                modelItem.AvailableText.Add(
                    string.Format(
                        LocalizationService.GetResource("Admin.Orders.GetOrderItems.AvailableLimit"),
                        Math.Abs(offer.Amount) < pageItem.Amount ? Math.Abs(offer.Amount) : pageItem.Amount /*startAmount + pageItem.DecrementedAmount*/,
                        pageItem.Amount));
            }
            else if (modelItem.AvailableText.Count == 0)
            {
                modelItem.Available = true;
                // modelItem.AvailableText =
                //     LocalizationService.GetResource("Admin.Orders.GetOrderItems.Available");
            }
        }

        private static void SetItemStocks(OrderItemModel modelItem, OrderItem pageItem,
            Dictionary<string, Offer> cacheOffers, Dictionary<int, List<WarehouseStock>> cacheOfferStocks, List<Warehouse> warehouses)
        {
            modelItem.StocksText = new List<string>();
            
            Offer offer;
            if (!cacheOffers.TryGetValue(pageItem.ArtNo, out offer))
            {
                offer = OfferService.GetOffer(pageItem.ArtNo);
                cacheOffers.Add(pageItem.ArtNo, offer);
            }

            if (offer == null)
                modelItem.StocksText.Add(
                    LocalizationService.GetResource("Admin.Orders.GetOrderItems.StocksNo"));
            else
                SetItemStocks(modelItem, pageItem, cacheOfferStocks, warehouses, offer);
        }

        private static void SetItemStocks(OrderItemModel modelItem, OrderItem pageItem,
            Dictionary<int, List<WarehouseStock>> cacheOfferStocks, List<Warehouse> warehouses, Offer offer)
        {
            if (!cacheOfferStocks.ContainsKey(offer.OfferId))
                cacheOfferStocks.Add(offer.OfferId, WarehouseStocksService.GetOfferStocks(offer.OfferId));

            var offerStocks = cacheOfferStocks[offer.OfferId];

            foreach (var warehouseStock in offerStocks)
            {
                modelItem.StocksText.Add(
                    string.Format(
                        LocalizationService.GetResource("Admin.Orders.GetOrderItems.StocksInWarehouse"),
                        warehouses.Find(warehouse => warehouse.Id == warehouseStock.WarehouseId)?.Name,
                        warehouseStock.Quantity));
            }
            
            if (modelItem.StocksText.Count == 0)
                modelItem.StocksText.Add(
                    LocalizationService.GetResource("Admin.Orders.GetOrderItems.StocksNo"));
        }

        public void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                    "OrderItemID",
                    "OrderID",
                    "PhotoID",
                    "ArtNo",
                    "Name",
                    "ProductID",
                    "Color",
                    "Size",
                    "Price",
                    "Amount",
                    "Width",
                    "Height",
                    "Length",
                    "Weight",
                    "Price * Amount".AsSqlField("Cost"),
                    "BookingServiceId",
                    "TypeItem",
                    "BarCode",
                    "IsMarkingRequired",
                    "IsCustomPrice",
                    "BasePrice",
                    "DiscountPercent",
                    "DiscountAmount",
                    "Unit",
                    "MeasureType",
                    "PaymentSubjectType",
                    "PaymentMethodType",
                    "DecrementedAmount",
                    "DownloadLink"
                )
                .From("[Order].[OrderItems]")
                .Where("OrderID = {0}", _filterModel.OrderId);

            Sorting();
        }

        public void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
                return;

            var sorting = _filterModel.Sorting.ToLower();
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
        
        public static string RenderCustomOptions(List<EvaluatedCustomOptions> evlist, Currency currency)
        {
            if (evlist == null || evlist.Count == 0)
                return "";

            var html = new StringBuilder("");
            foreach (var ev in evlist)
            {
                if (ev.OptionAmount != null || (!string.IsNullOrEmpty(ev.OptionTitle) && !string.IsNullOrEmpty(ev.CustomOptionTitle)))
                {
                    html.AppendFormat(
                        "<div class=\"orderitem-option\"><span class=\"orderitem-option-name\">{0}</span> <span class=\"orderitem-option-value\">{1} {3}</span></div>",
                        ev.CustomOptionTitle + (!string.IsNullOrEmpty(ev.OptionTitle) ? ":" : ""), ev.OptionTitle, ev.GetFormatPrice(currency), ev.OptionAmount > 1 ? "x " + ev.OptionAmount.ToString() : "");
                }
            }
            html.Append("");

            return html.ToString();
        }
    }
}
