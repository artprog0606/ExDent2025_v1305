using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics;
using AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports;
using AdvantShop.Web.Admin.Models.Marketing.Analytics;
using AdvantShop.Web.Admin.ViewModels.Analytics;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Admin.Handlers.Analytics;
using AdvantShop.Web.Admin.Models.Analytics;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Controllers.Marketing
{
    [Auth(RoleAction.Analytics)]
    [SaasFeature(ESaasProperty.DeepAnalytics)]
    public class AnalyticsController : BaseAdminController
    {
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public ActionResult Page()
        {
            return RedirectToAction(SaasDataService.IsEnabledFeature(ESaasProperty.DeepAnalytics) ? "Index" : "ExportProducts");
        }

        #region Analytics report

        public ActionResult Index()
        {
            var model = new AnalyticsReportModel();

            SetMetaInformation(T("Admin.Marketing.ConsolidatedError"));
            SetNgController(NgControllers.NgControllersTypes.AnalyticsReportCtrl);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Reports_ViewReports);

            return View(model);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetVortex(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var result = new VortexHandler(from, to).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetProfit(AnalyticsModel model)
        {
            model.ValidateDates();

            object result = null;

            var handler = new OrderStatictsHandler(model.DateFrom, model.DateTo, model.OrderStatus, model.Paid, model.UseShippingCost, model.GroupFormatString);
            switch (model.Type)
            {
                case "sum":
                    result = handler.GetOrdersSum();
                    break;
                case "count":
                    result = handler.GetOrdersCount();
                    break;

            }
            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetAvgCheck(AnalyticsModel model)
        {
            model.ValidateDates();

            object result = null;

            var handler = new AvgCheckHandler(model.DateFrom, model.DateTo, model.OrderStatus, model.Paid, model.GroupFormatString);
            switch (model.Type)
            {
                case "avg":
                    result = handler.GetAvgCheck();
                    break;
                case "city":
                    result = handler.GetAvgCheckByCity();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetOrders(AnalyticsModel model)
        {
            model.ValidateDates();

            object result = null;

            var handler = new OrdersByHandler(model.DateFrom, model.DateTo, model.OrderStatus, model.Paid, model.GroupFormatString);
            switch (model.Type)
            {
                case "payments":
                    result = handler.GetPayments();
                    break;
                case "shippings":
                    result = handler.GetShippings(minDeliveryDate: model.DeliveryDateFrom, maxDeliveryDate: model.DeliveryDateTo);
                    break;
                case "shippingsGroupedByName":
                    result = handler.GetShippings(true, model.DeliveryDateFrom, model.DeliveryDateTo);
                    break;
                case "statuses":
                    result = handler.GetStatuses();
                    break;
                case "sources":
                    result = handler.GetOrderTypes();
                    break;
                case "repeatorders":
                    result = handler.GetRepeatOrders();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetTelephony(AnalyticsModel model)
        {
            model.ValidateDates();

            object result = null;

            var handler = new TelephonyHandler(model.DateFrom, model.DateTo, model.GroupFormatString);
            switch (model.Type)
            {
                case "in":
                    result = handler.GetCallsCount(ECallType.In);
                    break;
                case "missed":
                    result = handler.GetCallsCount(ECallType.Missed);
                    break;
                case "out":
                    result = handler.GetCallsCount(ECallType.Out);
                    break;
                case "avgtime":
                    result = handler.GetAvgDuration();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetRfm(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var handler = new RfmAnalysisHandler(from, to);
            return Json(handler.GetData());
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetRfmCommonData(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var handler = new RfmCommonDataHandler(from, to);
            return Json(handler.GetData());
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetManagers(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var result = new ManagersHandler(from, to).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetAbcxyzAnalysis(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var result = new AbcxyzAnalysisHandler(from, to).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetProductStatisticsName(int productId)
        {
            var p = ProductService.GetProduct(productId);
            return Json(new { name = p != null ? p.Name : "" });
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetOfferStatisticsName(int offerId)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null || offer.Product == null)
                return Json(new { name = string.Empty });
            return Json(new { name = string.Join(" ", new []{ offer.Product.Name, offer.Size?.SizeName, offer.Color?.ColorName }.Where(x => x.IsNotEmpty())) });
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetProductStatistics(string type, string dateFrom, string dateTo, int productId, bool? paid, string groupFormatString)
        {
            ChartDataJsonModel result = null;
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var handler = new ProductStatisticsHandler(from, to, productId, paid, groupFormatString);
            switch (type)
            {
                case "sum":
                    result = handler.GetSum();
                    break;
                case "count":
                    result = handler.GetCount();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetOfferStatistics(string type, string dateFrom, string dateTo, int offerId, bool? paid, string groupFormatString)
        {
            ChartDataJsonModel result = null;
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var handler = new OfferStatisticsHandler(from, to, offerId, paid, groupFormatString);
            switch (type)
            {
                case "sum":
                    result = handler.GetSum();
                    break;
                case "count":
                    result = handler.GetCount();
                    break;
            }

            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public ActionResult GetProductStatisticsList(ProductOrdersStatisticsFilterModel filter)
        {
            var productStatisticsList = new ProductOrdersStatistics(filter).Execute();

            if (filter.OutputDataType == FilterOutputDataType.Csv)
            {
                string fileName = "productOrdersStatistics.csv";
                var fullFilePath = new ExportProductOrdersStatistics(productStatisticsList, fileName).Execute();
                return FileDeleteOnUpload(fullFilePath, "application/octet-stream", fileName);
            }

            return Json(productStatisticsList);
        }

        [Auth(RoleAction.Orders)]
        public ActionResult GetOfferStatisticsList(OfferOrdersStatisticsFilterModel filter)
        {
            var offerStatisticsList = new OfferOrdersStatistics(filter).Execute();

            if (filter.OutputDataType == FilterOutputDataType.Csv)
            {
                string fileName = "offerOrdersStatistics.csv";
                var fullFilePath = new ExportOfferOrdersStatistics(offerStatisticsList, fileName).Execute();
                return FileDeleteOnUpload(fullFilePath, "application/octet-stream", fileName);
            }

            return Json(offerStatisticsList);
        }

        [Auth(RoleAction.BonusSystem)]
        [HttpGet]
        public JsonResult GetBonusParticipantsStatistics(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);
            
            var result = new BonusParticipantsStatisticsHandler(from, to).Execute();
            
            return Json(result);
        }

        [Auth(RoleAction.BonusSystem)]
        [HttpGet]
        public JsonResult GetBonusCardGradesStatistics()
        {
            var result = new BonusGradesStatisticsHandler().Execute();

            return Json(result);
        }

        [Auth(RoleAction.BonusSystem)]
        [HttpGet]
        public JsonResult GetBonusMovementStatistics(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var result = new BonusMovementStatisticsHandler(from, to).Execute();

            return Json(result);
        }

        [Auth(RoleAction.BonusSystem)]
        [HttpGet]
        public JsonResult GetBonusTopUsersAccrued(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var model = new BonusTopUsersAccruedHandler(from, to).Execute();
            
            return Json(new { DataItems = model });
        }

        [Auth(RoleAction.BonusSystem)]
        [HttpGet]
        public JsonResult GetBonusTopUsersUsed(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var model = new BonusTopUsersUsedHandler(from, to).Execute();

            return Json(new { DataItems = model });
        }

        [Auth(RoleAction.BonusSystem)]
        [HttpGet]
        public JsonResult GetBonusRules(string dateFrom, string dateTo)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var model = new BonusRulesHandler(from, to).Execute();

            return Json(new { DataItems = model });
        }

        #endregion

        #region Analytics Filter

        [Auth(RoleAction.Catalog, RoleAction.Customers)]
        public ActionResult AnalyticsFilter(AnalyticsFilterModel data)
        {
            data.From = data.From.TryParseDateTime().ToString("dd-MM-yyyy");
            data.To = data.To.TryParseDateTime().ToString("dd-MM-yyyy");

            SetMetaInformation(T("Admin.Marketing.ConsolidatedError"));
            SetNgController(NgControllers.NgControllersTypes.AnalyticsFilterCtrl);

            return View(data);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetAnalyticsFilterAbcxyz(AnalyticsFilterModel data, BaseFilterModel filter)
        {
            var (from, to) = GetFromToDateTimes(data.From, data.To);
            
            return Json(new GetAnalyticsFilterAbcxyz(filter, from, to, data.Group).Execute());
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetAnalyticsFilterRfm(AnalyticsFilterModel data, BaseFilterModel filter)
        {
            var (from, to) = GetFromToDateTimes(data.From, data.To);
            
            return Json(new GetAnalyticsFilterRfm(filter, data.Group, from, to).Execute());
        }

        #endregion

        #region ExportOrders

        [Auth(RoleAction.Orders)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public ActionResult ExportOrders()
        {
            SetNgController(NgControllers.NgControllersTypes.AnalyticsCtrl);
            SetMetaInformation(T("Admin.Marketing.Analytics.ExportOrders"));

            var model = new GetExportOrdersModel().Execute();
            return View(model);
        }

        [Auth(RoleAction.Orders)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        public JsonResult GetExportOrdersSettings()
        {
            var model = new ExportOrdersModel();

            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var orderStatuses = new Dictionary<int, string>();
            foreach (var status in OrderStatusService.GetOrderStatuses())
            {
                orderStatuses.Add(status.StatusID, status.StatusName);
            }

            var orderSources = new Dictionary<int, string>();
            foreach (var source in OrderSourceService.GetOrderSources())
            {
                orderSources.Add(source.Id, source.Name);
            }

            model.PaidStatuses = new Dictionary<bool, string>
            {
                {true, T("Admin.Marketing.Paid") },
                {false, T("Admin.Marketing.NotPaid") }
            };

            model.Shippings = new Dictionary<int, string>();
            foreach (var shipping in ShippingMethodService.GetAllShippingMethods())
            {
                model.Shippings.Add(shipping.ShippingMethodId, shipping.Name);
            }

            model.Shipping = model.Shippings.Any() ? model.Shippings.ElementAt(0).Key : 0;

            model.Payments = new Dictionary<int, string>();
            foreach (var payment in PaymentService.GetAllPaymentMethods(false)){
                model.Payments.Add(payment.PaymentMethodId, payment.Name);
            }

            model.Payment = model.Payments.Any() ? model.Payments.ElementAt(0).Key : 0;
            
            model.Paid = true;

            model.Encoding = EncodingsEnum.Windows1251.StrName();
            model.Encodings = encodings;
            model.OrderStatuses = orderStatuses;
            model.Status = orderStatuses.Count > 0 ? orderStatuses.FirstOrDefault().Key : 0;
            model.OrderSources = orderSources;
            model.Source = orderSources.Count > 0 ? orderStatuses.FirstOrDefault().Key : 0;
            model.DateFrom = DateTime.Now.AddMonths(-3);
            model.DateTo = DateTime.Now;
            model.ShippingDateFrom = DateTime.Now.AddMonths(-3);
            model.ShippingDateTo = DateTime.Now;

            return JsonOk(model);
        }

        [Auth(RoleAction.Orders)]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [HttpPost]
        public JsonResult ExportOrders(ExportOrdersModel settings)
        {
            new ExportOrdersHandler(settings).Execute();
            return Json(new { result = true });
        }

        #endregion

        #region ExportOrder to xlsx
        [SaasFeature(ESaasProperty.HaveExcel)]
        [Auth(RoleAction.Orders)]
        public ActionResult ExportOrder(int orderId)
        {
            var order = OrderService.GetOrder(orderId);
            if (order == null)
                return Content(T("Admin.Marketing.OrderNotFound"));

            try
            {
                order.OrderItems = order.OrderItems.OrderByDescending(x => x.Name).ToList();
                if (order.OrderItems.Count > 1)
                {
                    var temp = order.OrderItems[order.OrderItems.Count - 1];
                    var orderItems = order.OrderItems.Where(x => x.OrderItemID != temp.OrderItemID).OrderByDescending(x => x.Name).ToList();
                    order.OrderItems.Clear();
                    order.OrderItems.Add(temp);
                    order.OrderItems.AddRange(orderItems.ToArray());
                }

                var strPath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                FileHelpers.CreateDirectory(strPath);

                var filename = $"Order{order.Number.RemoveSymbols()}.xlsx";
                var templatePath = Server.MapPath(OrderExcelExport.TemplateSingleOrder);

                new OrderExcelExport().SingleOrder(templatePath, strPath + filename, order);

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_ExportExcel);

                return File(strPath + filename, "application/octet-stream", filename);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return Content("error");
        }

        #endregion

        #region ExportProducts

        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [Auth(RoleAction.Orders)]
        public ActionResult ExportProducts()
        {
            SetNgController(NgControllers.NgControllersTypes.AnalyticsCtrl);
            SetMetaInformation(T("Admin.Marketing.Analytics.ExportProducts"));

            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var separators = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                if (enumItem == SeparatorsEnum.Custom) 
                    continue;
                separators.Add(enumItem.StrName(), enumItem.Localize());
            }

            return View(new ExportProductsModel
            {
                Encoding = EncodingsEnum.Windows1251.StrName(),
                Encodings = encodings,
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                Separators = separators
            });
        }

        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [Auth(RoleAction.Orders)]
        [HttpPost]
        public JsonResult GetExportProductsSettings()
        {
            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var separators = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                if (enumItem == SeparatorsEnum.Custom) 
                    continue;
                separators.Add(enumItem.StrName(), enumItem.Localize());
            }

            var model = new ExportProductsModel
            {
                Encoding = EncodingsEnum.Windows1251.StrName(),
                Encodings = encodings,
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                Separators = separators,
                DateFrom = DateTime.Now.AddMonths(-3),
                DateTo = DateTime.Now
            };
            return JsonOk(model);
        }

        [HttpPost]
        [ExcludeFilter(typeof(SaasFeatureAttribute))]
        [Auth(RoleAction.Orders)]
        public ActionResult ExportProducts(ExportProductsModel settings)
        {
            new ExportProductsHandler(settings).Execute();
            return Json(new { result = true });
        }

        #endregion

        [Auth(RoleAction.Customers)]
        public ActionResult SearchQueries()
        {
            SetMetaInformation(T("Admin.Analytics.SearchQueries"));
            SetNgController(NgControllers.NgControllersTypes.SearchQueriesCtrl);

            return View();
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetSearchQueriesStatistic(SearchQueriesFilterModel model)
        {
            return Json(new GetSearchQueriesStatistic(model).Execute());
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetResultCountRangeForPaging(SearchQueriesFilterModel command)
        {
            var handler = new GetSearchQueriesStatistic(command);
            var resultCount =
                handler.GetItemsIds<SearchQueriesResultCountRangeModel>("Max(ResultCount) as Max, Min(ResultCount) as Min")
                    .FirstOrDefault();

            return Json(new { from = resultCount?.Min ?? 0, to = resultCount?.Max ?? 10000000 });
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetTopCustomers(string from, string to)
        {
            var (fromDate, toDate) = GetFromToDateTimes(from, to);

            var model = new GetTopCustomersHandler(fromDate, toDate).Execute();
            return Json(new { DataItems = model });
        }

        [Auth(RoleAction.Customers)]
        public JsonResult GetCustomerGroups(string dateFrom, string dateTo, string groupFormatString)
        {
            var (from, to) = GetFromToDateTimes(dateFrom, dateTo);

            var result = new GetCustomersGroupHandler(from, to, groupFormatString).GetGroups();
            return Json(result);
        }

        [Auth(RoleAction.Orders)]
        public JsonResult GetOrdersData(string from, string to, bool? paid, int? orderStatus)
        {
            var (fromDate, toDate) = GetFromToDateTimes(from, to);

            var model = new GetOrdersDataHandler(fromDate, toDate, paid, orderStatus).Execute();
            return Json(model);
        }

        [Auth(RoleAction.Catalog)]
        public JsonResult GetCatalogData()
        {
            var model = new GetProductsDataHandler().Execute();
            return Json(model);
        }

        private (DateTime, DateTime) GetFromToDateTimes(string dateFrom, string dateTo)
        {
            var now = DateTime.Now.Date;
            
            var from = dateFrom.TryParseDateTime(now.AddMonths(-1));
            var parsedTo = dateTo.TryParseDateTime(true);
            var to =
                parsedTo != null 
                    ? new DateTime(parsedTo.Value.Year, parsedTo.Value.Month, parsedTo.Value.Day, 23, 59, 59)
                    : new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            return (from, to);
        }
    }
}
