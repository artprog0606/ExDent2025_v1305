using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Booking.Sms;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Booking.Affiliate;
using AdvantShop.Web.Admin.Handlers.Booking.Affiliate.Settings;
using AdvantShop.Web.Admin.Handlers.Booking.Affiliate.SmsTemplate;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Booking.Affiliate;
using AdvantShop.Web.Admin.Models.Booking.Affiliate.SmsTemplate;
using AdvantShop.Web.Admin.ViewModels.Booking.Settings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Booking
{
    public partial class BookingAffiliateController : BaseBookingController
    {
        #region For Menu Json

        //public bool CheckAccess
        //{
        //    get { return SelectedAffiliate != null && AffiliateService.CheckAccess(SelectedAffiliate); }
        //}

        //public bool CheckAccessToEditing
        //{
        //    get { return SelectedAffiliate != null && AffiliateService.CheckAccessToEditing(SelectedAffiliate); }
        //}

        #endregion

        public JsonResult GetAffiliates()
        {
            var currentCustomer = CustomerContext.CurrentCustomer;
            var currentManager = currentCustomer.IsManager ? ManagerService.GetManager(currentCustomer.Id) : null;

            return JsonOk(new
            {
                Affiliates =
                    AffiliateService.GetList().Where(x => AffiliateService.CheckAccess(x, currentManager, true)).Select(x => new SelectItemModel(x.Name, x.Id.ToString())).ToList()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddAffiliate(AffiliateModel model)
        {
            if (ModelState.IsValid)
            {
                var affiliate = new Affiliate
                {
                    Name = model.Name.DefaultOrEmpty(),
                    Description = model.Description.DefaultOrEmpty(),
                    Address = model.Address.DefaultOrEmpty(),
                    Phone = model.Phone.DefaultOrEmpty(),
                    SortOrder = model.SortOrder,
                    Enabled = model.Enabled,
                    BookingIntervalMinutes = model.BookingIntervalMinutes,
                    AccessForAll = true,
                    AnalyticsAccessForAll = true,
                    IsActiveSmsNotification = model.IsActiveSmsNotification,
                    ForHowManyMinutesToSendSms = model.ForHowManyMinutesToSendSms,
                    SmsTemplateBeforeStartBooiking = model.SmsTemplateBeforeStartBooiking,
                    CancelUnpaidViaMinutes = model.CancelUnpaidViaMinutes,
                };

                if (!affiliate.AccessForAll && !CustomerContext.CurrentCustomer.IsAdmin && CustomerContext.CurrentCustomer.IsManager)
                {
                    var manager = ManagerService.GetManager(CustomerContext.CurrentCustomer.Id);
                    affiliate.ManagerIds.Add(manager.ManagerId);
                }

                var id = AffiliateService.Add(affiliate);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_AffiliateCreated);

                return JsonOk(id);
            }
            return JsonError();

        }

        #region Settings Affiliate

        public ActionResult Settings(int? id = null)
        {
            if (id.HasValue)
            {
                if (!SelectAffiliate(id.Value) || SelectedAffiliate == null || SelectedAffiliate.Id != id.Value)
                    return RedirectToAction("Index", "Booking");
            }

            if (SelectedAffiliate == null || !AffiliateService.CheckAccessToEditing(SelectedAffiliate))
                return RedirectToAction("Index", "Booking");

            var model = new SettingsModel
            {
                Affiliate = new AffiliateModel()
                {
                    Id = SelectedAffiliate.Id,
                    Name = SelectedAffiliate.Name,
                    Address = SelectedAffiliate.Address,
                    StandardPhone = StringHelper.ConvertToStandardPhone(SelectedAffiliate.Phone),
                    Description = SelectedAffiliate.Description,
                    SortOrder = SelectedAffiliate.SortOrder,
                    Enabled = SelectedAffiliate.Enabled,
                    BookingIntervalMinutes = SelectedAffiliate.BookingIntervalMinutes,
                    AccessForAll = SelectedAffiliate.AccessForAll,
                    ManagerIds = SelectedAffiliate.ManagerIds,
                    AnalyticsAccessForAll = SelectedAffiliate.AnalyticsAccessForAll,
                    AnalyticManagerIds = SelectedAffiliate.AnalyticManagerIds,
                    AccessToViewBookingForResourceManagers = SelectedAffiliate.AccessToViewBookingForResourceManagers,
                    IsActiveSmsNotification = SelectedAffiliate.IsActiveSmsNotification,
                    ForHowManyMinutesToSendSms = SelectedAffiliate.ForHowManyMinutesToSendSms,
                    SmsTemplateBeforeStartBooiking = SelectedAffiliate.SmsTemplateBeforeStartBooiking,
                    CancelUnpaidViaMinutes = SelectedAffiliate.CancelUnpaidViaMinutes,
                },
                CanBeDeleting = CustomerContext.CurrentCustomer.IsAdmin,
                CanBeEditAccess = CustomerContext.CurrentCustomer.IsAdmin,
                IsActiveSmsModule = AttachedModules.GetModules<ISmsService>().Count > 0
            };

            SetMetaInformation("Настройки филиала");
            SetNgController(NgControllers.NgControllersTypes.BookingAffiliateSettingsCtrl);
            
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Settings(SettingsModel model)
        {
            var handler = new SettingsSaveHandler(model);
            if (handler.Execute())
            {
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            else
            {
                foreach (var error in handler.Errors)
                    ModelState.AddModelError("", error);

                ShowErrorMessages();
            }

            return Settings();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var affiliate = AffiliateService.Get(id);
            if (affiliate == null)
                return JsonError("Филиал не найден");

            if (!CustomerContext.CurrentCustomer.IsAdmin)
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            AffiliateService.Delete(id);
            return JsonOk();
        }

        public JsonResult GetSettingsAdditionalData(int affiliateId)
        {
            var affiliate = AffiliateService.Get(affiliateId);
            if (affiliate == null)
                return JsonError("Указанный филиал не найден");

            var managersForAccess = ManagerService.GetManagers(RoleAction.Booking);
            var affiliateManagerIdsWhoDoNotRole = affiliate.ManagerIds.Where(id => !managersForAccess.Any(m => m.ManagerId == id));
            foreach (var id in affiliateManagerIdsWhoDoNotRole)
            {
                var m = ManagerService.GetManager(id);
                if (m != null)
                    managersForAccess.Add(m);
            }

            var managersForAnalytic = affiliate.AccessForAll ? managersForAccess : affiliate.Managers;
            var analyticManagerIdsNotInAffiliateManagers = affiliate.AnalyticManagerIds.Where(id => !managersForAnalytic.Any(m => m.ManagerId == id));
            foreach (var id in analyticManagerIdsNotInAffiliateManagers)
            {
                var m = ManagerService.GetManager(id);
                if (m != null)
                    managersForAnalytic.Add(m);
            }

            return JsonOk(new
            {
                ManagersForAccess = managersForAccess.Select(x => new SelectItemModel(x.FullName, x.ManagerId.ToString())).ToList(),
                ManagersForAnalytic = managersForAnalytic.Select(x => new SelectItemModel(x.FullName, x.ManagerId.ToString())).ToList()
            });
        }

        public JsonResult GetSettingsTimesOfBooking(int affiliateId, int? intervalMinutes)
        {
            var affiliate = AffiliateService.Get(affiliateId);
            if (affiliate == null)
                return JsonError("Указанный филиал не найден");
            if (!AffiliateService.CheckAccess(affiliate))
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            var model =
                new GetSettingsTimesOfBookingHandler(affiliate, intervalMinutes ?? affiliate.BookingIntervalMinutes)
                    .Execute();

            return JsonOk(new
            {
                Times = model.Times,
                MondayTimes = model.MondayTimes,
                TuesdayTimes = model.TuesdayTimes,
                WednesdayTimes = model.WednesdayTimes,
                ThursdayTimes = model.ThursdayTimes,
                FridayTimes = model.FridayTimes,
                SaturdayTimes = model.SaturdayTimes,
                SundayTimes = model.SundayTimes,
            });
        }

        #endregion

        #region AdditionalTime

        #region Get Add Update Delete

        public JsonResult GetAdditionalTime(int affiliateId, DateTime? date)
        {
            if (!date.HasValue)
                return JsonError("Не указана дата");

            var affiliate = AffiliateService.Get(affiliateId);
            if (affiliate == null)
                return JsonError("Указанный филиал не найден");
            if (!AffiliateService.CheckAccess(affiliate))
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            var model = new GetAdditionalTimeHandler(affiliate, date.Value)
                        .Execute();

            return JsonOk(new
            {
                Times = model.Times
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddAdditionalTime(AddUpdateAdditionalTimeModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateAdditionalTimeHandler(model);
                if (handler.Execute())
                    return JsonOk();

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

                return JsonError(handler.Errors.ToArray());
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAdditionalTime(DeleteAdditionalTimeModel model)
        {
            if (ModelState.IsValid)
            {
                var affiliate = AffiliateService.Get(model.AffiliateId.Value);
                if (affiliate == null)
                    return JsonError("Указанный филиал не найден");
                if (!AffiliateService.CheckAccessToEditing(affiliate))
                    return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

                if (model.Date.HasValue)
                {
                    AffiliateAdditionalTimeService.DeleteByAffiliateAndDate(affiliate.Id, model.Date.Value);

                    new Handlers.Booking.ReservationResources.CorrectingReservationResourcesAdditionalTimeHandler(
                        affiliate.Id, model.Date.Value,
                        Handlers.Booking.ReservationResources.TypeCorrectingReservationResourcesTimes.All)
                        .Execute();
                }
                else //if (_model.StartDate.HasValue && _model.EndDate.HasValue)
                {
                    AffiliateAdditionalTimeService.DeleteByAffiliateAndDateFromTo(affiliate.Id, model.StartDate.Value.Date, model.EndDate.Value.Date.AddDays(1).AddMilliseconds(-1));

                    new Handlers.Booking.ReservationResources.CorrectingReservationResourcesAdditionalTimeHandler(
                        affiliate.Id, model.StartDate.Value, model.EndDate.Value,
                        Handlers.Booking.ReservationResources.TypeCorrectingReservationResourcesTimes.All).Execute();
                }

                return JsonOk();
            }
            return JsonError();
        }

        #endregion

        #region Form Modal

        public JsonResult GetAdditionalTimeFrom(int affiliateId, DateTime? date, DateTime? startDate, DateTime? endDate)
        {
            //if (!date.HasValue)
            //    return JsonError("Не указана дата");

            var affiliate = AffiliateService.Get(affiliateId);
            if (affiliate == null)
                return JsonError("Указанный филиал не найден");
            if (!AffiliateService.CheckAccess(affiliate))
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            var handler = date.HasValue || startDate.HasValue
                ? date.HasValue
                    ? new GetAdditionalTimeFromDataHandler(affiliate, date.Value)
                    : new GetAdditionalTimeFromDataHandler(affiliate, startDate.Value, endDate.Value)
                : new GetAdditionalTimeFromDataHandler(affiliate);

            var model = handler.Execute();

            return JsonOk(new
            {
                Times = model.Times,
                WorkTimes = model.WorkTimes,
                ExistAdditionalTimes = model.ExistAdditionalTimes
            });
        }

        #endregion

        public JsonResult GetYearAdditionalDate(int affiliateId, int year)
        {
            var affiliate = AffiliateService.Get(affiliateId);
            if (affiliate == null)
                return JsonError("Указанный филиал не найден");
            if (!AffiliateService.CheckAccess(affiliate))
                return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            var dateYearFrom = new DateTime(year, 1, 1).Date;
            var yearTimes =
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(affiliateId, dateYearFrom, dateYearFrom.AddYears(1)).Select(x => x.StartTime.Date).Distinct().ToList();

            return JsonOk(yearTimes);
        }

        #endregion

        #region SmsTemplates

        public JsonResult GetSmsTemplates(SmsTemplatesFilterModel model)
        {
            var handler = new GetSmsTemplatesHandler(model);
            var result = handler.Execute();

            return Json(result);
        }

        #region Commands

        private void Command(SmsTemplatesFilterModel command, Func<int, SmsTemplatesFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetSmsTemplatesHandler(command);
                var ids = handler.GetItemsIds("Id");

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSmsTemplates(SmsTemplatesFilterModel command)
        {
            Command(command, (id, c) =>
            {
                SmsTemplateService.Delete(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        #region CRUD

        public JsonResult GetSmsTemplate(int id)
        {
            var dbModel = SmsTemplateService.Get(id);
            if (dbModel == null)
                return JsonError("Шаблон не найден");

            var result = new SmsTemplateModel
            {
                Id = dbModel.Id,
                AffiliateId = dbModel.AffiliateId,
                Status = dbModel.Status,
                Text = dbModel.Text,
                Enabled = dbModel.Enabled,
            };

            return JsonOk(result);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceSmsTemplate(SmsTemplateModel model)
        {

            var template = SmsTemplateService.Get(model.Id);

            template.Enabled = model.Enabled;

            SmsTemplateService.Update(template);

            return JsonOk(model);
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddSmsTemplate(SmsTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                SmsTemplateService.Add(new SmsTemplate
                {
                    AffiliateId = model.AffiliateId,
                    Status = model.Status,
                    Text = model.Text.DefaultOrEmpty().Trim(),
                    Enabled = model.Enabled,
                });

                return JsonOk();
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateSmsTemplate(SmsTemplateModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var template = SmsTemplateService.Get(model.Id);
            if (template == null)
                return JsonError("Шаблон не найден");

            template.Text = model.Text.DefaultOrEmpty().Trim();
            template.Status = model.Status;
            template.Enabled = model.Enabled;

            SmsTemplateService.Update(template);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSmsTemplate(int id)
        {
            SmsTemplateService.Delete(id);
            return Json(new { result = true });
        }

        #endregion

        #region Modal

        public JsonResult GetSmsTemplateFormData()
        {
            return JsonOk(SmsTemplateService.Variables.ToList());
        }

        #endregion

        #endregion
    }
}
