using System;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Handlers.Booking.Analytics.Reports;
using AdvantShop.Web.Admin.ViewModels.Booking.Analytics;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Booking
{
    public partial class BookingAnalyticsController : BaseBookingController
    {
        public ActionResult Index()
        {
            if (SelectedAffiliate == null || !AffiliateService.CheckAccessToAnalytic(SelectedAffiliate))
                return RedirectToAction("Index", "Booking");

            SetMetaInformation("Аналитика");
            SetNgController(NgControllers.NgControllersTypes.BookingAnalyticsCtrl);

            var model = new AnalyticsModel()
            {
                SelectedAffiliate = SelectedAffiliate
            };

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_ViewReports);
            
            return View(model);
        }

        public JsonResult GetTurnover(string type, string datefrom, string dateto, bool? isPaid, BookingStatus? status, string groupFormatString, int? affiliateId)
        {
            var from = datefrom.TryParseDateTime();
            var to = dateto.TryParseDateTime().Date.AddDays(1).AddMilliseconds(-1);
            //to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

            object result = null;

            var handler = new BookingTurnoverStatictsHandler(from, to, isPaid, status, groupFormatString, affiliateId);
            switch (type)
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

        public JsonResult GetCommon(string datefrom, string dateto, bool? isPaid, BookingStatus? status, int? affiliateId)
        {
            var from = datefrom.TryParseDateTime();
            var to = dateto.TryParseDateTime().Date.AddDays(1).AddMilliseconds(-1);
            //to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

            var result = new BookingCommonStatisticHandler(from, to, isPaid, status, affiliateId).Execute();
            return Json(result);
        }

        public JsonResult GetReservationResources(string datefrom, string dateto, bool? isPaid, BookingStatus? status, int? affiliateId)
        {
            var from = datefrom.TryParseDateTime();
            var to = dateto.TryParseDateTime().Date.AddDays(1).AddMilliseconds(-1);
            //to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

            var result = new BookingReservationResourcesStatictsHandler(from, to, isPaid, status, affiliateId).Execute();
            return Json(result);
        }

        public JsonResult GetBookingSources(string datefrom, string dateto, bool? isPaid, BookingStatus? status, int? affiliateId)
        {
            var from = datefrom.TryParseDateTime();
            var to = dateto.TryParseDateTime().Date.AddDays(1).AddMilliseconds(-1);
            //to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

            var result = new BookingSourcesStatictsHandler(from, to, isPaid, status, affiliateId).Execute();
            return Json(result);
        }

        public JsonResult GetPaymentMethods(string datefrom, string dateto, bool? isPaid, BookingStatus? status, int? affiliateId)
        {
            var from = datefrom.TryParseDateTime();
            var to = dateto.TryParseDateTime().Date.AddDays(1).AddMilliseconds(-1);
            //to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

            var result = new BookingPaymentMethodsStatisticHandler(from, to, isPaid, status, affiliateId).Execute();
            return Json(result);
        }

        public JsonResult GetServices(string datefrom, string dateto, bool? isPaid, BookingStatus? status, BookingStatus? noStatus, int? affiliateId, int? reservationResourceId, bool? groupByReservationResource)
        {
            var from = datefrom.TryParseDateTime(true);
            var to = dateto.TryParseDateTime(true);
            //to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

            var result = new BookingServicesStatisticHandler(from, to, isPaid, status, noStatus, affiliateId, reservationResourceId, groupByReservationResource).Execute();
            return Json(result);
        }
    }
}
