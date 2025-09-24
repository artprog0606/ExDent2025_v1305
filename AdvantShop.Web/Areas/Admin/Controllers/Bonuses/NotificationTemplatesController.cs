using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Bonuses.Notification;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Bonuses.NotificationTemplates;
using AdvantShop.Web.Admin.Handlers.NotificationsTemplates;
using AdvantShop.Web.Admin.Models.Bonuses.NotificationTemplates;

namespace AdvantShop.Web.Admin.Controllers.Bonuses
{
    [Auth(RoleAction.BonusSystem)]
    [SaasFeature(Saas.ESaasProperty.BonusSystem)]
    [SalesChannel(ESalesChannelType.Bonus)]
    public class NotificationTemplatesController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.SmsTemplates.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.NotificationTemplatesCtrl);
            return View();
        }

        public JsonResult GetNotificationTemplates(NotificationTemplateFilterModel model)
        {
            return Json(new GetNotificationsTemplatesHandler(model).Execute());
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteNotificationTemplateMass(List<int> ids, string selectMode)
        {
            if (selectMode == "all")
            {
                var t = NotificationTemplateService.GetAll();
                foreach (var id in t)
                {
                    NotificationTemplateService.Delete(id.NotificationTemplateId);
                }
            }
            if (ids == null) return Json(false);
            foreach (var id in ids)
            {
                NotificationTemplateService.Delete(id);
            }
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteNotificationTemplate(int id)
        {
            NotificationTemplateService.Delete(id);
            return Json(true);
        }

        [HttpGet]
        public JsonResult AddEditNotificationTemplate(ENotifcationType notificationTypeId, EBonusNotificationMethod? bonusNotificationMethod)
        {
            if (bonusNotificationMethod != null)
            {
                var model = new NotificationTemplateModel(notificationTypeId, bonusNotificationMethod.Value);
                
                var t = NotificationTemplateService.Get(notificationTypeId, bonusNotificationMethod.Value);
                model.NotificationBody = t != null ? t.NotificationBody : "";
                
                return JsonOk(model);
            }
            else
            {
                var model = new NotificationTemplateModel(notificationTypeId);
                
                return JsonOk(model);
            }
        }

       [HttpPost, ValidateJsonAntiForgeryToken, ValidateAjax]
        public JsonResult AddEditNotificationTemplate(NotificationTemplateModel model)
        {
            return ProcessJsonResult(new AddEditNotificationTemplateHandler(model));
        }

        public ActionResult NotificationLog()
        {
            SetMetaInformation(T("Admin.SmsTemplates.SmsLog.Title"));
            SetNgController(NgControllers.NgControllersTypes.NotificationTemplatesCtrl);
            return View();
        }

        public JsonResult GetNotificationLogs(NotificationLogFilterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                    return Json(new GetNotificationLogHandler(model).Execute());
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return Json(new NotificationLogFilterModel());
        }
    }
}
