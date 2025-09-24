using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Booking.ReservationResources;
using AdvantShop.Web.Admin.Handlers.Booking.Services;
using AdvantShop.Web.Admin.Models.Booking.Services;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Booking.ReservationResources;
using AdvantShop.Web.Admin.ViewModels.Booking.ReservationResources;
using AdvantShop.Configuration;

namespace AdvantShop.Web.Admin.Controllers.Booking
{
    public partial class BookingResourcesController : BaseBookingController
    {
        public ActionResult Index()
        {
            if (SelectedAffiliate == null)
                return RedirectToAction("Index", "Booking");

            SetMetaInformation(T("Admin.BookingCategory.View.Employees"));
            SetNgController(NgControllers.NgControllersTypes.BookingReservationResourcesCtrl);

            var model = new ReservationResourcesModel()
            {
                SelectedAffiliate = SelectedAffiliate,
                AccessToEditing = SelectedAffiliate != null && AffiliateService.CheckAccessToEditing(SelectedAffiliate)
            };
            
            return View(model);
        }

        #region Get Add Update Delete

        public JsonResult GetResourcesList(int? affiliateId)
        {
            var customer = CustomerContext.CurrentCustomer;
            var manager = customer.IsManager ? ManagerService.GetManager(customer.Id) : null;

            IEnumerable<ReservationResource> reservationResources;
            if (affiliateId.HasValue)
            {
                var affiliate = AffiliateService.Get(affiliateId.Value);
                reservationResources =
                    ReservationResourceService.GetByAffiliate(affiliateId.Value)
                        .Where(x => ReservationResourceService.CheckAccess(x, affiliate, manager));

            }
            else
            {
                var affiliates =
                    AffiliateService.GetList().Where(x => AffiliateService.CheckAccess(x, manager, true)).ToList();

                reservationResources = ReservationResourceService.GetList().Where(rr => affiliates.Any(a => ReservationResourceService.CheckAccess(rr, a, manager)));
            }
            return Json(reservationResources.Select(x => new SelectItemModel(x.Name, x.Id.ToString())));
        }


        public JsonResult GetReservationResource(int? id, int? affiliateId)
        {
            if (id.HasValue)// && affiliateId.HasValue
            {
                var resource = ReservationResourceService.Get(id.Value);
                if (resource != null)
                {
                    var model = (AddUpdateReservationResourceModel) resource;
                    model.CanBeEditing = true;

                    if (affiliateId.HasValue)
                    {
                        if (ReservationResourceService.CheckAccess(resource, affiliateId.Value))
                        {
                            model.AffiliateId = affiliateId;
                            model.BookingIntervalMinutes =
                                ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliateId.Value,
                                    id.Value);
                            model.CanBeEditing = ReservationResourceService.CheckAccessToEditing(resource, affiliateId.Value);
                        }
                        else
                            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                    }

                    return JsonOk(model);
                }
                return JsonError("Указанный ресурс не найден");
            }

            return JsonError("Указаны не все параметры для определения ресурса");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(AddUpdateReservationResourceModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateReservationResourceHandler(model);
                if (handler.Execute())
                    return JsonOk();
                
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(AddUpdateReservationResourceModel model)
        {
            if (ModelState.IsValid)
            {
                if (ReservationResourceService.Get(model.Id) == null)
                    return JsonError("Указанный ресурс не найден");

                var handler = new AddUpdateReservationResourceHandler(model);
                if (handler.Execute())
                    return JsonOk();
                
                return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int? reservationResourceId)
        {
            if (reservationResourceId.HasValue)
            {
                ReservationResourceService.Delete(reservationResourceId.Value);
                return JsonOk();
            }

            return JsonError("Не указан идентификатор ресурса бронирования");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteReservationResourceImage(int id)
        {
            var reservationResource = ReservationResourceService.Get(id);
            if (reservationResource == null)
                return JsonError("Ресурс отсутствует");

            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingReservationResource, reservationResource.Image));

            reservationResource.Image = null;
            ReservationResourceService.Update(reservationResource);

            return JsonOk();
        }

        #endregion

        #region Form Modal

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetReservationResourceFormData(int? id, int? affiliateId, int? bookingIntervalMinutes, bool fistLoad)
        {
            Affiliate affiliate = null;
            if (affiliateId.HasValue)
            {
                affiliate = AffiliateService.Get(affiliateId.Value);
                if (affiliate == null)
                    return JsonError("Указанный филиал не найден");
                if (affiliate != null)
                {
                    if (id.HasValue && !ReservationResourceService.CheckAccess(ReservationResourceService.Get(id.Value), affiliate, null))
                        return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

                    if (!id.HasValue && !AffiliateService.CheckAccess(affiliate))
                        return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                }
            }

            var model =
                    new GetReservationResourceFormDataHandler(id, affiliate, bookingIntervalMinutes, fistLoad)
                        .Execute();

            return JsonOk(new
            {
                Managers = model.Managers,
                AffiliateBookingIntervalMinutes = model.AffiliateBookingIntervalMinutes,
                BookingIntervals = model.BookingIntervals,
                Times = model.Times,

                MondayWorkTimes = model.MondayWorkTimes,
                TuesdayWorkTimes = model.TuesdayWorkTimes,
                WednesdayWorkTimes = model.WednesdayWorkTimes,
                ThursdayWorkTimes = model.ThursdayWorkTimes,
                FridayWorkTimes = model.FridayWorkTimes,
                SaturdayWorkTimes = model.SaturdayWorkTimes,
                SundayWorkTimes = model.SundayWorkTimes,

                MondayTimes = model.MondayTimes,
                TuesdayTimes = model.TuesdayTimes,
                WednesdayTimes = model.WednesdayTimes,
                ThursdayTimes = model.ThursdayTimes,
                FridayTimes = model.FridayTimes,
                SaturdayTimes = model.SaturdayTimes,
                SundayTimes = model.SundayTimes,

                Tags = model.Tags
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetReservationResourceShedulerFormData(int? affiliateId, int? reservationResourceId)
        {
            if (affiliateId.HasValue && reservationResourceId.HasValue)
            {
                var affiliate = AffiliateService.Get(affiliateId.Value);
                if (affiliate == null)
                    return JsonError("Указанный филиал не найден");

                var reservationResource = ReservationResourceService.Get(reservationResourceId.Value);
                if (reservationResource == null)
                    return JsonError("Ресурс не найден");

                var customer = CustomerContext.CurrentCustomer;
                var manager = customer.IsManager ? ManagerService.GetManager(customer.Id) : null;

                if (!ReservationResourceService.CheckAccess(reservationResource, affiliate, manager))
                    return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

                return JsonOk(new
                {
                    Name = reservationResource.Name,
                    BookingDuration = new TimeSpan(0, ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id, reservationResource.Id) ?? affiliate.BookingIntervalMinutes, 0),
                    AccessToEditing = ReservationResourceService.CheckAccessToEditing(reservationResource, affiliate, manager),
                    AccessToViewBooking = AffiliateService.CheckAccess(affiliate, manager, false) || affiliate.AccessToViewBookingForResourceManagers
                });
            }

            return JsonError("Не все параметры указаны");
        }

        #endregion

        #region Actions for grid

        public JsonResult GetResources(ReservationResourcesFilterModel model)
        {
            return Json(new GetReservationResourcesHandler(model).Execute());
        }

        private void Command(ReservationResourcesFilterModel model, Action<int, ReservationResourcesFilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    func(id, model);
            }
            else
            {
                var ids = new GetReservationResourcesHandler(model).GetItemsIds();
                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteReservationResources(ReservationResourcesFilterModel model)
        {
            Command(model, (id, m) => ReservationResourceService.Delete(id));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceReservationResource(ReservationResourceGridModel model)
        {
            if (model.Id != 0)
            {
                var reservationResource = ReservationResourceService.Get(model.Id);

                if (model.AffiliateId.HasValue && !ReservationResourceService.CheckAccessToEditing(reservationResource, model.AffiliateId.Value))
                    return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

                reservationResource.Enabled = model.Enabled;
                reservationResource.SortOrder = model.SortOrder;
                ReservationResourceService.Update(reservationResource);

                if (model.AffiliateId.HasValue)
                {
                    if (model.BindAffiliate)
                        ReservationResourceService.AddUpdateRefAffiliate(model.AffiliateId.Value, reservationResource.Id, model.BookingIntervalMinutes);
                    else
                        ReservationResourceService.DeleteRefAffiliate(model.AffiliateId.Value, reservationResource.Id);
                }

                return JsonOk();
            }

            return JsonError();
        }

        #endregion

        #region Services

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddServices(int? affiliateId, int? reservationResourceId, List<int> serviceIds)
        {
            if (serviceIds == null || serviceIds.Count == 0)
                return JsonError("Не указан список услуг для добавления");

            if (affiliateId.HasValue && reservationResourceId.HasValue)
            {
                if (ReservationResourceService.CheckAccessToEditing(reservationResourceId.Value, affiliateId.Value))
                {
                    foreach (var serviceId in serviceIds)
                        ServiceService.AddReservationResourceService(affiliateId.Value, reservationResourceId.Value,
                            serviceId);

                    return JsonOk();
                }
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
            }

            return JsonError("Указаны не все параметры для определения ресурса");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteService(int? affiliateId, int? reservationResourceId, int? serviceId)
        {
            if (affiliateId.HasValue && reservationResourceId.HasValue && serviceId.HasValue)
            {
                if (ReservationResourceService.CheckAccessToEditing(reservationResourceId.Value, affiliateId.Value))
                {
                    ServiceService.DeleteReservationResourceService(affiliateId.Value, reservationResourceId.Value, serviceId.Value);

                    return JsonOk();
                }
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
            }

            return JsonError("Указаны не все параметры для определения услуги");
        }

        private void Command(ServicesFilterModel model, Action<int, ServicesFilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    func(id, model);
            }
            else
            {
                var ids = new GetServicesHandler(model).GetItemsIds();
                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteServices(ServicesFilterModel model)
        {
            if (ReservationResourceService.CheckAccessToEditing(model.ReservationResourceId.Value, model.AffiliateId.Value))
            { 
                Command(model, (id, m) => ServiceService.DeleteReservationResourceService(model.AffiliateId.Value, model.ReservationResourceId.Value, id));
                return JsonOk();
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        #endregion

        #region AdditionalTime

        #region Get Add Update Delete

        public JsonResult GetAdditionalTime(int? affiliateId, int? reservationResourceId, DateTime? date)
        {
            if (!date.HasValue)
                return JsonError("Не указана дата");

            if (!affiliateId.HasValue || !reservationResourceId.HasValue)
                return JsonError("Указаны не все параметры для определения ресурса");

            var reservationResource = ReservationResourceService.Get(reservationResourceId.Value);
            if (reservationResource == null)
                return JsonError("Указанный ресурс не найден");

            if (ReservationResourceService.CheckAccess(reservationResource, affiliateId.Value))
            {
                var model = new GetAdditionalTimeHandler(affiliateId.Value, reservationResource, date.Value)
                    .Execute();

                return JsonOk(new
                {
                    Times = model.Times,
                });
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddAdditionalTime(AddUpdateAdditionalTimeModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateAdditionalTimeHandler(model);
                if (handler.Execute())
                    return JsonOk();

                if (handler.UserConfirmIsRequired)
                    return JsonOk(new
                    {
                        UserConfirmIsRequired = true,
                        ConfirmButtomText = handler.ConfirmButtomText,
                        ConfirmMessage = handler.ConfirmMessage
                    });

                return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAdditionalTime(AddUpdateAdditionalTimeModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateAdditionalTimeHandler(model);
                if (handler.Execute())
                    return JsonOk();

                if (handler.UserConfirmIsRequired)
                    return JsonOk(new
                    {
                        UserConfirmIsRequired = true,
                        ConfirmButtomText = handler.ConfirmButtomText,
                        ConfirmMessage = handler.ConfirmMessage
                    });

                return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAdditionalTime(int? affiliateId, int? reservationResourceId, DateTime date)
        {
            if (!affiliateId.HasValue || !reservationResourceId.HasValue)
                return JsonError("Указаны не все параметры для определения ресурса");

            var reservationResource = ReservationResourceService.Get(reservationResourceId.Value);
            if (reservationResource == null)
                return JsonError("Указанный ресурс не найден");

            if (ReservationResourceService.CheckAccessToEditing(reservationResource, affiliateId.Value))
            {
                ReservationResourceAdditionalTimeService.GetByDate(affiliateId.Value, reservationResource.Id, date)
                    .ForEach(item => ReservationResourceAdditionalTimeService.Delete(item.Id));

                return JsonOk();
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        #endregion

        #region Form Modal

        public JsonResult GetAdditionalTimeFrom(int? affiliateId, int? reservationResourceId, DateTime? date)
        {
            if (!date.HasValue)
                return JsonError("Не указана дата");

            if (!affiliateId.HasValue || !reservationResourceId.HasValue)
                return JsonError("Указаны не все параметры для определения ресурса");

            var reservationResource = ReservationResourceService.Get(reservationResourceId.Value);
            if (reservationResource == null)
                return JsonError("Указанный ресурс не найден");

            if (ReservationResourceService.CheckAccess(reservationResource, affiliateId.Value))
            {
                var model = new GetAdditionalTimeFromDataHandler(affiliateId.Value, reservationResource, date.Value)
                    .Execute();

                return JsonOk(new
                {
                    Times = model.Times,
                    WorkTimes = model.WorkTimes,
                    ExistAdditionalTimes = model.ExistAdditionalTimes,
                    CanBeEditing = ReservationResourceService.CheckAccessToEditing(reservationResource, affiliateId.Value)
                });
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        #endregion

        public JsonResult GetYearAdditionalDate(int? affiliateId, int? reservationResourceId, int year)
        {
            if (!affiliateId.HasValue || !reservationResourceId.HasValue)
                return JsonError("Указаны не все параметры для определения ресурса");

            var reservationResource = ReservationResourceService.Get(reservationResourceId.Value);
            if (reservationResource == null)
                return JsonError("Указанный ресурс не найден");

            if (ReservationResourceService.CheckAccess(reservationResource, affiliateId.Value))
            {
                var dateYearFrom = new DateTime(year, 1, 1).Date;
                var yearTimes =
                    ReservationResourceAdditionalTimeService.GetByDateFromTo(affiliateId.Value, reservationResource.Id,
                        dateYearFrom, dateYearFrom.AddYears(1)).Select(x => x.StartTime.Date).Distinct().ToList();

                return JsonOk(yearTimes);
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        #endregion

    }
}
