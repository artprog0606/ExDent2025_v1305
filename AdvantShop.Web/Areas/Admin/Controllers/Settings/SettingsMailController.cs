using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Letters;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Mails;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.SettingsMail;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsMailController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.NotifyEMails.Title"));
            SetNgController(NgControllers.NgControllersTypes.MailSettingsCtrl);

            var model = new GetNotifyEmailsSettings().Execute();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(MailSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    new SaveNotifyEmailsSettings(model).Execute();
                    ShowMessage(NotifyType.Success, T("Admin.Settings.ChangesSaved"));
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_EditMailSettings);
                }
                catch (BlException e)
                {
                    ModelState.AddModelError("", e);
                }
            }

            ShowErrorMessages();

            var isMobile = SettingsDesign.IsMobileTemplate;

            var referrerParams = new System.Web.Routing.RouteValueDictionary();

            if (HttpContext.Request.UrlReferrer != null)
            {
                var nv = HttpUtility.ParseQueryString(HttpContext.Request.UrlReferrer.Query); //Преобразует урлхвост в NameValueCollection

                foreach (var nvKey in nv.AllKeys)
                {
                    if (referrerParams.ContainsKey(nvKey) is false)
                        referrerParams.Add(nvKey, nv.Get(nvKey));
                }
            }

            return isMobile ? RedirectToAction("Index", referrerParams) : RedirectToAction("Index");
        }


        public JsonResult GetMailFormats(MailFormatsFilterModel model)
        {
            return Json(new GetMailFormatsHandler(model).Execute());
        }

        public JsonResult GetMailFormatTypes()
        {
            return Json(MailFormatService.GetMailFormatTypes());
        }

        public JsonResult GetMailFormatTypesSelectOptions()
        {
            return Json(MailFormatService.GetMailFormatTypes().Select(x => new SelectItemModel(x.TypeName, x.MailFormatTypeId)));
        }

        public JsonResult GetMailFormat(int id)
        {
            return Json(MailFormatService.Get(id));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(MailFormatModel model)
        {
            if (ModelState.IsValid)
            {
                var mailFormatId = MailFormatService.Add(new MailFormat()
                {
                    MailFormatTypeId = model.MailFormatTypeId,
                    FormatName = model.FormatName,
                    FormatSubject = model.FormatSubject,
                    FormatText = model.FormatText,
                    Enable = model.Enable,
                    SortOrder = model.SortOrder,
                });

                if (mailFormatId > 0)
                    return Json(new { result = true });
            }

            var errors = new List<string>();

            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    errors.Add(error.ErrorMessage);

            return Json(new { result = false, errors = String.Join(", ", errors) });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Edit(MailFormat model)
        {
            var format = MailFormatService.Get(model.MailFormatId);

            format.FormatName = model.FormatName;
            format.FormatSubject = model.FormatSubject;
            format.FormatText = model.FormatText;
            format.Enable = model.Enable;
            format.SortOrder = model.SortOrder;

            MailFormatService.Update(format);
            
            var mailFormatTypes = MailFormatService.GetMailFormatTypes();
            
            if (mailFormatTypes.Any(type => 
                    type.MailFormatTypeId == model.MailFormatTypeId 
                    && type.MailType == "OnChangeOrderStatus"))
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_EditMailFormatWithTypeOnChangeOrderStatus);
            
            if (mailFormatTypes.Any(type => 
                    type.MailFormatTypeId == model.MailFormatTypeId 
                    && type.MailType == "OnNewOrderReview"))
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_EditMailFormatWithTypeOnNewOrderReview);

            return JsonOk();
        }

        private Type GetTypeByMailTemplateAttribute(string mailType)
        {
            return CacheManager.Get("types_by_mailTemplateType_" + mailType, () =>
            {
                foreach (var type in AppDomain.CurrentDomain
                                                 .GetAssemblies()
                                                 .Where(x => x.FullName.Contains("AdvantShop."))
                                                 .SelectMany(x => x.GetTypes()))
                {
                    var attributes = type.GetCustomAttributes(typeof(MailTemplateTypeAttribute), false);
                    if (attributes.Length == 0)
                        continue;

                    var attribute = attributes[0] as IAttribute<MailType>;
                    if (attribute == null || attribute.Value.ToString() != mailType)
                        continue;

                    return type;
                }

                return null;
            });
        }

        [HttpGet]
        public JsonResult GetTypeDescription(int mailFormatTypeId)
        {
            var mailType = MailFormatService.GetMailFormatType(mailFormatTypeId);
            if (mailType == null)
                return Json(new { result = false, error = T("Admin.Settings.NoTypeFoundWithId") + mailFormatTypeId });
            
            try
            {
                var classType = GetTypeByMailTemplateAttribute(mailType.MailType);
                if (classType != null)
                {
                    var mailTemplate = (MailTemplate)Activator.CreateInstance(classType, true);
                    if (mailTemplate != null)
                    {
                        var keysHtml = LetterBuilderHelper.GetLetterFormatKeysHtml(mailTemplate.GetKeyDescriptions());
                        
                        var commonKeysHtml = 
                            "<div class=\"letter-format-block\">" + 
                            LetterBuilderHelper.GetLetterFormatKeysHtml<CommonLetterTemplateKey>() + 
                            "</div>";
                        
                        return Json(new {result = true, message = keysHtml + commonKeysHtml});
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            
            return Json(new {result = true, message = mailType.Comment});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteMailFormat(int mailFormatId)
        {
            MailFormatService.Delete(mailFormatId);
            return Json(new { result = true, message = T("Admin.Settings.MailFormat.Deleted") });
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateStatus()
        {
            MailService.Reset();
            MailService.SendValidate();
            var confirm = CapShopSettings.ConfirmDate;
            return JsonOk(confirm);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendValidate()
        {
            try
            {
                if (MailService.SendValidate())
                    return JsonOk(T("Admin.Settings.MailFormat.EmailWasConfirmed"));

                return JsonOk(T("Admin.Settings.MailFormat.EmailWasSend"));
            }
            catch (BlException e)
            {
                ModelState.AddModelError("", e);
            }
            return JsonError();
        }

        #region Command

        private void Command(MailFormatsFilterModel model, Func<int, MailFormatsFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new GetMailFormatsHandler(model);
                var mailFormatIds = handler.GetItemsIds();

                foreach (int id in mailFormatIds)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteMailFormats(MailFormatsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                MailFormatService.Delete(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        #region Inplace

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceMailFormat(MailFormat model)
        {
            var dbModel = MailFormatService.Get(model.MailFormatId);
            if (dbModel == null)
                return Json(new { result = false });

            dbModel.Enable = model.Enable;
            dbModel.SortOrder = model.SortOrder;
            MailFormatService.Update(dbModel);

            return Json(new { result = true });
        }

        #endregion

        #region Templates

        #region Command Templates

        private void Command(MailAnswerTemplateFilterModel model, Func<int, MailAnswerTemplateFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new GetMailAnswerTemplatesHandler(model);
                var mailFormatIds = handler.GetItemsIds();

                foreach (int id in mailFormatIds)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteMailAnswerTemplates(MailAnswerTemplateFilterModel command)
        {
            Command(command, (id, c) =>
            {
                new MailAnswerTemplateService().Delete(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceMailAnswerTemplate(MailAnswerTemplate model)
        {
            var dbModel = new MailAnswerTemplateService().Get(model.TemplateId, false);
            if (dbModel == null)
                return Json(new { result = false });

            dbModel.Active = model.Active;
            dbModel.SortOrder = model.SortOrder;
            new MailAnswerTemplateService().Update(dbModel);

            return JsonOk();
        }

        public JsonResult GetMailAnswerTemplates(MailAnswerTemplateFilterModel model)
        {
            var result = new GetMailAnswerTemplatesHandler(model).Execute();
            return Json(result);
        }

        public JsonResult GetMailAnswerTemplate(int id)
        {
            return Json(new MailAnswerTemplateService().Get(id, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddMailTemplate(MailAnswerTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                var templateId = new MailAnswerTemplateService().Add(new MailAnswerTemplate
                {
                    Name = model.Name,
                    Subject = model.Subject,
                    Body = model.Body,
                    Active = model.Active,
                    SortOrder = model.SortOrder
                });

                if (templateId > 0)
                {
                    return JsonOk();
                }
            }

            var errors = new List<string>();

            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    errors.Add(error.ErrorMessage);

            return JsonError(errors.ToArray());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditMailTemplate(MailAnswerTemplateModel model)
        {
            var format = new MailAnswerTemplateService().Get(model.TemplateId, false);

            format.Name = model.Name;
            format.Subject = model.Subject;
            format.Body = model.Body;
            format.Active = model.Active;
            format.SortOrder = model.SortOrder;

            new MailAnswerTemplateService().Update(format);

            return JsonOk();
        }

        public JsonResult DeleteMailAnswerTemplate(int templateId)
        {
            new MailAnswerTemplateService().Delete(templateId);
            return JsonOk("Admin.Settings.MailFormat.Deleted");// (new { result = true, message = T("Admin.Settings.MailFormat.Deleted") });
        }
        #endregion

        #region Sms Templates

        #region Command Sms Templates

        private void SmsCommand(SmsAnswerTemplateFilterModel model, Func<int, SmsAnswerTemplateFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new GetSmsAnswerTemplatesHandler(model);
                var smsFormatIds = handler.GetItemsIds();

                foreach (int id in smsFormatIds)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSmsAnswerTemplates(SmsAnswerTemplateFilterModel command)
        {
            SmsCommand(command, (id, c) =>
            {
                new SmsAnswerTemplateService().Delete(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceSmsAnswerTemplate(SmsAnswerTemplate model)
        {
            var dbModel = new SmsAnswerTemplateService().Get(model.TemplateId, false);
            if (dbModel == null)
                return Json(new { result = false });

            dbModel.Active = model.Active;
            dbModel.SortOrder = model.SortOrder;
            new SmsAnswerTemplateService().Update(dbModel);

            return JsonOk();
        }

        public JsonResult GetSmsAnswerTemplates(SmsAnswerTemplateFilterModel model)
        {
            var result = new GetSmsAnswerTemplatesHandler(model).Execute();
            return Json(result);
        }
        
        [HttpGet]
        public JsonResult GetLetterFormatKeys()
        {
            return Json(new
            {
                storeKeys = LetterBuilderHelper.GetLetterFormatKeysHtml<CommonLetterTemplateKey>(),
                customerKeys = BonusSystem.IsActive
                    ? LetterBuilderHelper.GetLetterFormatKeysHtml<CustomerLetterTemplateKey>()
                    : LetterBuilderHelper.GetLetterFormatKeysHtml(LetterBuilderHelper.GetLetterFormatKeys(Enum.GetValues(typeof(CustomerLetterTemplateKey))
                    .Cast<CustomerLetterTemplateKey>()
                    .Where(x => x != CustomerLetterTemplateKey.BonusBalance)
                    .ToList())),
                orderKeys = LetterBuilderHelper.GetLetterFormatKeysHtml<OrderLetterTemplateKey>(),
                leadKeys = LetterBuilderHelper.GetLetterFormatKeysHtml<LeadLetterTemplateKey>(),
            });
        }

        public JsonResult GetSmsAnswerTemplate(int id)
        {
            return Json(new SmsAnswerTemplateService().Get(id, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddSmsTemplate(SmsAnswerTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                var templateId = new SmsAnswerTemplateService().Add(new SmsAnswerTemplate
                {
                    Name = model.Name,
                    Text = model.Text,
                    Active = model.Active,
                    SortOrder = model.SortOrder
                });

                if (templateId > 0)
                {
                    return JsonOk();
                }
            }

            var errors = new List<string>();

            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    errors.Add(error.ErrorMessage);

            return JsonError(errors.ToArray());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditSmsTemplate(SmsAnswerTemplateModel model)
        {
            var format = new SmsAnswerTemplateService().Get(model.TemplateId, false);

            format.Name = model.Name;
            format.Text = model.Text;
            format.Active = model.Active;
            format.SortOrder = model.SortOrder;

            new SmsAnswerTemplateService().Update(format);

            return JsonOk();
        }

        public JsonResult DeleteSmsAnswerTemplate(int templateId)
        {
            new SmsAnswerTemplateService().Delete(templateId);
            return JsonOk("Admin.Settings.MailFormat.Deleted");// (new { result = true, message = T("Admin.Settings.MailFormat.Deleted") });
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateAddress(string email)
        {

            try
            {
                if (!string.IsNullOrEmpty(email) && email.Contains("@"))
                {
                    var emailParts = email.Split('@');
                    var emailName = emailParts.FirstOrDefault();
                    MailService.Save(email, emailName);
                    return JsonOk(new
                    {
                        email,
                        name = emailName
                    });
                }
                else
                {
                    return JsonError(T("Admin.Customers.SpecifyTheValidEmail"));
                }
            }
            catch (BlException e)
            {
                Debug.Log.Error(e);
                return JsonError(e.Message);
            }
        }
    }


    [Auth(RoleAction.Settings)]
    public partial class SettingsMailTestController : BaseAdminController
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendTestMessage(SendTestMessageModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = new SendTestMessageHandler().Execute(model);
                    if (result)
                        return JsonOk();
                }
                catch (BlException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                }
            }
            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendTestMessageTemplate(string emailTo, string nameTemplate)
        {
            if (string.IsNullOrEmpty(emailTo) || string.IsNullOrEmpty(nameTemplate))
                return JsonError();

            try
            {
                MailTemplate mail = null;

                switch (nameTemplate.ToLower())
                {
                    case "emailfororders":
                        mail = NewOrderMailTemplate.Create(new Order()
                        {
                            OrderID = 1,
                            Number = "1",
                            OrderCurrency = CurrencyService.CurrentCurrency,
                            OrderCustomer = (OrderCustomer)CustomerContext.CurrentCustomer,
                            OrderItems = new List<OrderItem>() { new OrderItem() { ArtNo = "sku-123", Name = "Новый товар", Amount = 1, Price = 500 } },
                            ArchivedShippingName = "Доставка",
                            ShippingCost = 100,
                            ArchivedPaymentName = "Оплата картой",
                        });
                        break;

                    case "emailforleads":
                        mail = new LeadMailTemplate(new Lead()
                        {
                            Id = 1,
                            Email = "test@test.test",
                            Description = "Test",
                            Customer = CustomerContext.CurrentCustomer,
                        });
                        break;

                    case "emailforbookings":
                        mail = new BookingCreatedMailTemplate(new Core.Services.Booking.Booking()
                        {
                            Id = 1,
                            Email = "test@test.test",
                            FirstName = "Test",
                            Phone = "Test",
                            BeginDate = DateTime.Now,
                            EndDate = DateTime.Now,
                        });
                        break;

                    case "emailforproductdiscuss":
                        mail = new ProductDiscussMailTemplate("sku-123", "Новый товар",
                            Url.AbsoluteRouteUrl("Product", new {url = "test"}), "Test", DateTime.Now.ToString(),
                            "Test text", Url.AbsoluteRouteUrl("Product", new {url = "test"}), "test@test.test");
                        break;

                    case "emailforregreport":
                        mail = new RegistrationMailTemplate(CustomerContext.CurrentCustomer);
                        break;

                    case "emailforfeedback":
                        mail = new FeedbackMailTemplate(SettingsMain.SiteUrl, SettingsMain.ShopName, "Test",
                            "test@test.test", "",
                            T("Feedback.Index.FeedbackForm") + ": " + T("Feedback.FeedbackType.Question"), "Test", "");
                        break;

                    case "emailforpartners":
                        mail = new EmptyMailTemplate() {Subject = "Test", Body = "Test"};
                        break;

                    case "emailformissedcall":
                        mail = new MissedCallMailTemplate("+79000000000");
                        break;

                    default:
                        return JsonError("Формат письма не найден");
                }
                    
                mail.BuildMail();

                if (string.IsNullOrEmpty(mail.Subject) && string.IsNullOrEmpty(mail.Body))
                    return JsonError("Формат письма не найден или не активен");

                var result =
                    new SendTestMessageHandler().Execute(new SendTestMessageModel()
                    {
                        To = emailTo,
                        Subject = mail.Subject,
                        Body = mail.Body
                    });

                if (result)
                    return JsonOk();
            }
            catch (BlException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
            }
            return JsonError();
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult TestImap()
        {
            try
            {
                var result = new ImapMailService().TestImap();
                if (result)
                    return JsonOk();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (msg != null && msg.ToLower().Contains("login invalid credentials or imap is disabled"))
                    return
                        JsonError("Неправильный логин/пароль или протокол IMAP не поддерживается данным сервером.<br/><br/> " + msg +
                                  " <br/><br/><a href=\"https://www.advantshop.net/help/pages/email-google-yandex#110\" target=\"_blank\">Подробнее смотрите инструкцию.</a>");

                if (msg != null && msg.Contains("Этот хост неизвестен"))
                    return JsonError("Неправильный IMAP сервер");

                return JsonError(ex.Message);
            }
            return JsonError();
        }
    }
}
