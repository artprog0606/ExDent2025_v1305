using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.ViewModel.Feedback;
using AdvantShop.Web.Infrastructure.Controllers;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Saas;
using BotDetect.Web.Mvc;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Repository;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Models.User;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    public class FeedbackController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index(FeedbackType messageType = FeedbackType.Question)
        {
            var model = new FeedbackViewModel() { MessageType = messageType, Secret = "secret"};

            var customer = CustomerContext.CurrentCustomer;
            if (customer.RegistredUser)
            {
                model.Email = customer.EMail;
                model.Phone = customer.Phone;
                model.Name = customer.GetShortName();
                model.Agree = customer.IsAgreeForPromotionalNewsletter;
            }

            SetNgController(NgControllers.NgControllersTypes.FeedbackCtrl);
            SetMetaInformation(T("Feedback.Index.FeedbackHeader"));

            return View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult FeedbackForm(FeedbackViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Message) ||
                string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                (!string.IsNullOrWhiteSpace(model.Email) && !ValidationHelper.IsValidEmail(model.Email)) ||
                string.IsNullOrWhiteSpace(model.Phone))
            {
                return Json(new LoginResult("error", T("Feedback.Index.WrongData")));
            }

            if (!string.IsNullOrEmpty(model.Secret))
            {
                return Json(new LoginResult("error", T("Feedback.Index.WrongData")));
            }

            if (SettingsMain.EnableCaptchaInFeedback &&
                !MvcCaptcha.Validate("CaptchaSource", model.CaptchaCode, model.CaptchaSource))
            {
                return Json(new LoginResult("error", T("Js.Captcha.Wrong")));
            }

            if (SettingsCheckout.IsShowUserAgreementText && !model.Agree)
            {
                return Json(new LoginResult("error", T("User.Registration.ErrorAgreement")));
            }

            model.Name = HttpUtility.HtmlEncode(model.Name);
            model.Email = HttpUtility.HtmlEncode(model.Email);
            model.Phone = HttpUtility.HtmlEncode(model.Phone);
            model.Message = HttpUtility.HtmlEncode(model.Message);
            model.OrderNumber = HttpUtility.HtmlEncode(model.OrderNumber);

            var allowMessage = ModulesExecuter.CheckInfo(System.Web.HttpContext.Current, Core.Modules.Interfaces.ECheckType.Feedback, model.Email, model.Name, message: model.Message);
            if (!allowMessage)
            {
                return Json(new LoginResult("error", T("Common.SpamCheckFailed")));
            }

            var crmEnabled = !SaasDataService.IsSaasEnabled ||
                             (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.HaveCrm);
            if (SettingsFeedback.FeedbackAction == EnFeedbackAction.SendEmail
                || (SettingsFeedback.FeedbackAction == EnFeedbackAction.CreateLead && !crmEnabled))
            {
                var mail = new FeedbackMailTemplate(SettingsMain.SiteUrl, SettingsMain.ShopName,
                    model.Name, model.Email, model.Phone,
                    T("Feedback.Index.FeedbackForm") + ": " + model.MessageType.Localize(),
                    model.Message, model.OrderNumber);

                MailService.SendMailNow(CustomerContext.CustomerId, SettingsMail.EmailForFeedback, mail,
                    replyTo: model.Email);
            }
            else if (SettingsFeedback.FeedbackAction == EnFeedbackAction.CreateLead && crmEnabled)
            {
                CreateLead(model);
            }

            return Json(new LoginResult("succes"));
        }

        private void CreateLead(FeedbackViewModel model)
        {
            var message =
                string.Format("{0} со страницы \"{1}\"{2}: \n{3}",
                model.MessageType.Localize(),
                T("Feedback.Index.FeedbackHeader"),
                !string.IsNullOrWhiteSpace(model.OrderNumber) ? " к заказу " + model.OrderNumber : "",
                model.Message.Replace("\n", "<br>"));

            var source = OrderSourceService.GetOrderSource(OrderType.Feedback);

            var lead = new Lead()
            {
                Email = model.Email,
                FirstName = model.Name,
                Phone = model.Phone,
                CustomerId = CustomerContext.CurrentCustomer.RegistredUser ? CustomerContext.CustomerId : default,

                Customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    FirstName = model.Name,
                    EMail = model.Email,
                    Phone = model.Phone,
                    StandardPhone = model.Phone != null ? StringHelper.ConvertToStandardPhone(model.Phone) : default,
                    CustomerRole = Role.User
                },

                Comment = message,
                OrderSourceId = source?.Id ?? 0,
                
                Country = IpZoneContext.CurrentZone.CountryName,
                Region = IpZoneContext.CurrentZone.Region,
                District = IpZoneContext.CurrentZone.District,
                City = IpZoneContext.CurrentZone.City,
                Zip = IpZoneContext.CurrentZone.Zip
            };

            LeadService.AddLead(lead);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_Desktop);
        }
    }
}