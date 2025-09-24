using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Marketing.Emailings;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;
using AdvantShop.Web.Admin.ViewModels.Emailings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Marketing
{
    [Auth(RoleAction.Customers)]
    public partial class EmailingsController : BaseAdminController
    {
        #region Triggers

        public ActionResult TriggerEmailings(int id)
        {
            SetMetaInformation(T("Admin.Analytics.Trigger"));
            SetNgController(NgControllers.NgControllersTypes.TriggerAnalyticsCtrl);

            var model = new GetTriggerAnalyticsViewHandler(id).Execute();
            Track.TrackService.TrackEvent(Track.ETrackEvent.Triggers_ViewTriggerEmailingAnalitycs);
            return model != null ? View(model) : Error404();
        }

        public JsonResult GetTriggerEmailingsAnalytics(int triggerId, DateTime dateFrom, DateTime dateTo, Guid? emailingId)
        {
            return ProcessJsonResult(new GetTriggerAnalyticsHandler(triggerId, dateFrom, dateTo, emailingId));
        }

        #endregion

        #region ManualEmailings

        public ActionResult ManualEmailings()
        {
            SetMetaInformation(T("Admin.ManualEmailings.Title"));
            SetNgController(NgControllers.NgControllersTypes.ManualEmailingsCtrl);

            return View();
        }

        public ActionResult ManualEmailing(Guid id)
        {
            SetMetaInformation(T("Admin.ManualEmailings.Title"));
            SetNgController(NgControllers.NgControllersTypes.ManualEmailingCtrl);

            var emailing = ManualEmailingService.GetManualEmailing(id);
            if (emailing == null)
                return Error404();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Triggers_ViewManualEmailingAnalitycs);

            return View(new ManualEmailingViewModel
            {
                Id = id,
                Name = emailing.Subject,
                SendTime = emailing.DateCreated
            });
        }

        public JsonResult GetManualEmailingAnalytics(Guid id, string dateFrom, string dateTo)
        {
            var startDate = dateFrom.TryParseDateTime(isNullable: true);
            var endDate = dateTo.TryParseDateTime(isNullable: true);
            if (endDate.HasValue)
                endDate = TimeZoneInfo.ConvertTimeToUtc(endDate.Value);
            return ProcessJsonResult(new GetManualEmailingAnalyticsHandler(id, startDate, endDate));
        }

        public JsonResult GetManualEmailings(ManualEmailingsFilterModel model)
        {
            return Json(new GetManualEmailingsHandler(model).Execute());
        }

        public ActionResult ManualWithoutEmailing(int? id, string dateFrom, string dateTo)
        {
            var types = MailFormatService.GetMailFormatTypes();
            //SetMetaInformation(T("Admin.ManualEmailings.Title"));
            //SetNgController(NgControllers.NgControllersTypes.ManualWithoutEmailingCtrl);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Triggers_ViewManualEmailingAnalitycs);
            ViewBag.Id = id;
            ViewBag.DateFrom = dateFrom ?? DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo ?? DateTime.Now.ToString("yyyy-MM-dd");
            if (id.HasValue)
            {
                var t = types.FirstOrDefault(x => x.MailFormatTypeId == id.Value);
                ViewBag.Name = t != null ? t.TypeName : "не указан тип";
            }
            else
            {
                ViewBag.Name = "не указан тип";
            }
            return PartialView();
        }

        public JsonResult GetSubjectWithoutEmailings(string search)
        {
            var temp = AdvantShopMailService.GetSubjectWithoutEmailing();
            var types = MailFormatService.GetMailFormatTypes();

            foreach (var item in temp)
            {
                if (item.FormatId.HasValue)
                {
                    var t = types.FirstOrDefault(x => x.MailFormatTypeId == item.FormatId.Value);
                    item.FormatName = t != null ? t.TypeName : "не указан тип";
                }
                else
                {
                    item.FormatName = "не указан тип";
                }
            }
            var res = new FilterResult<SubjectWithoutEmailingResultDto>();
            res.PageIndex = 0;
            res.TotalPageCount = 1;


            if(search != null)
            {
                res.DataItems = temp.Where(x => x.FormatName.ToLower().Contains(search.ToLower())).ToList();
            }
            else
            {
                res.DataItems = temp;
            }

            res.TotalItemsCount = res.DataItems.Count;
            res.TotalString = T("Admin.Grid.FildTotal", res.DataItems.Count);

            return Json(res);
        }

        public JsonResult GetWithoutEmailings(int? id, GetWithoutEmailLogsDto model)
        {
            model.FormatId = id ?? model.FormatId;
            if (model.FormatId == 0)
                model.FormatId = null;

            var temp = AdvantShopMailService.GetWithoutEmailing(model);
            return Json(temp);
        }

        public JsonResult getManualWithoutEmailingAnalytics(int? id, string dateFrom, string dateTo)
        {
            var startDate = dateFrom.TryParseDateTime(isNullable: true);
            var endDate = dateTo.TryParseDateTime(isNullable: true);
            return ProcessJsonResult(new GetManualWithoutEmailingAnalyticsHandler(id, startDate, endDate));
        }



        #region Command

        private void Command(ManualEmailingsFilterModel command, Action<Guid, ManualEmailingsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetManualEmailingsHandler(command).GetItemsIds("Id");
                foreach (var id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [Auth, HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteManualEmailings(ManualEmailingsFilterModel command)
        {
            Command(command, (id, c) => ManualEmailingService.DeleteManualEmailing(id));
            return JsonOk();
        }

        #endregion

        #endregion

        #region Log

        public ActionResult Log(Guid id, string statuses)
        {
            SetMetaInformation(T("Admin.EmailingLog.Title"));
            SetNgController(NgControllers.NgControllersTypes.EmailingLogCtrl);

            var model = new GetEmailingLogViewHandler(id, statuses, this.Url).Execute();
            return model != null ? View(model) : Error404();
        }

        public JsonResult GetEmailingLog(EmailingLogFilterModel model)
        {
            return Json(new GetEmailingLogHandler(model).Execute());
        }

        public JsonResult GetEmail(int id)
        {
            var email = AdvantShopMailService.GetEmailLogs(id);
            return Json(email);
        }

        public JsonResult GetEmailStatuses()
        {
            return Json(Enum.GetValues(typeof(EmailStatus)).Cast<EmailStatus>()
                .Where(x => x != EmailStatus.None)
                .Select(x => new SelectItemModel(x.Localize(), x.ToString())));
        }

        public ActionResult LogWithout(int? id, string datefrom, string dateto, string statuses)
        {
            SetMetaInformation(T("Admin.EmailingLog.Title"));
            SetNgController(NgControllers.NgControllersTypes.EmailingLogCtrl);

            var model = new GetWithoutEmailingLogViewHandler(id, datefrom, dateto, statuses, this.Url).Execute();
            return View(model);
        }

        public ActionResult Subscribe(string email)
        {
            return ProcessJsonResult(() => AdvantShopMailService.Subscribe(email));
        }

        public ActionResult SupportSubscribe()
        {
            SetMetaInformation(T("Admin.ManualEmailings.Title"));
            SetNgController(NgControllers.NgControllersTypes.EmailingLogCtrl);
            return View();
        }
        #endregion
    }
}
