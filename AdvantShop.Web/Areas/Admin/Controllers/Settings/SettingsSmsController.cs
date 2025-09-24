using System;
using System.Web.Mvc;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Mails;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsSmsController : BaseAdminController
    {
        public JsonResult GetList(SmsTemplateFilterModel model)
        {
            return Json(new GetSmsTemplates(model).Execute());
        }
        
        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(SmsTemplateModel model)
        {
            var template = SmsOnOrderChangingService.Get(model.Id);
            if (template == null)
                return Json(new {result = false});

            template.Enabled = model.Enabled;
            template.SmsText = model.SmsText ?? "";

            SmsOnOrderChangingService.Update(template);

            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(SmsTemplateFilterModel command, Func<int, SmsTemplateFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var handler = new GetSmsTemplates(command);
                var ids = handler.GetItemsIds();

                foreach (var id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTemplates(SmsTemplateFilterModel command)
        {
            Command(command, (id, c) =>
            {
                SmsOnOrderChangingService.Delete(id);
                return true;
            });
            return JsonOk();
        }

        #endregion

        #region Add/Update sms template

        public JsonResult GetTemplate(int id)
        {
            return Json(SmsOnOrderChangingService.Get(id));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTemplate(SmsTemplateOnOrderChanging template)
        {
            SmsOnOrderChangingService.Add(template);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditTemplate(SmsTemplateOnOrderChanging template)
        {
            SmsOnOrderChangingService.Update(template);
            return JsonOk();
        }

        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTemplate(int id)
        {
            SmsOnOrderChangingService.Delete(id);
            return JsonOk();
        }

        public JsonResult GetOrderStatuses()
        {
            var result = OrderStatusService.GetOrderStatuses();
            return Json(result);
        }

        #endregion


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendTestSms(string phone, string text)
        {
            return ProcessJsonResult(() => new SendTestSms(phone, text).Execute());
        }
    }
}
