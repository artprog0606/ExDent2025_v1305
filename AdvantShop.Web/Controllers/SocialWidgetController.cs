using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Models.SocialWidgets;
using AdvantShop.Saas;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class SocialWidgetController : Controller
    {
        [ChildActionOnly]
        public ActionResult Script()
        {
            if (!SettingsSocialWidget.IsActive)
                return new EmptyResult();
            
            if (Request.IsMobileApp())
                return new EmptyResult();

            var model = new SocialWidgetModel();

            if (SettingsSocialWidget.IsShowVkInDesktop && VkApiService.IsVkActive())
            {
                model.IsShowVk = true;
                model.VkGroupId = SettingsVk.Group.Id;
            }

            if (SettingsSocialWidget.IsShowFbInDesktop && new FacebookApiService().IsActive())
            {
                model.IsShowFb = true;
                model.FacebookMsgId = SettingsFacebook.GroupId;
            }

            if (SettingsSocialWidget.IsShowJivositeInDesktop)
            {
                var module = AttachedModules.GetModuleById("JivoSite", true);
                if (module != null)
                    model.IsShowJivosite = true;
            }

            if (SettingsSocialWidget.IsShowCallbackInDesktop)
            {
                var callback = IPTelephonyOperator.Current.CallBack;
                model.IsShowCallbackIpTelephony = callback != null && callback.Enabled &&
                                                  (!SaasDataService.IsSaasEnabled ||
                                                   SaasDataService.CurrentSaasData.HaveTelephony);

                if (!model.IsShowCallbackIpTelephony)
                {
                    var module = AttachedModules.GetModuleById("Callback", true);
                    if (module != null)
                        model.IsShowCallback = true;
                }
            }

            if (SettingsSocialWidget.IsShowWhatsAppInDesktop && SettingsSocialWidget.WhatsAppPhone.IsNotEmpty())
            {
                model.IsShowWhatsApp = true;
                model.LinkWhatsApp = "https://wa.me/" + SettingsSocialWidget.WhatsAppPhone;
            }

            model.IsShowViber = SettingsSocialWidget.IsShowViberInDesktop;
            model.IsShowOdnoklassniki = SettingsSocialWidget.IsShowOdnoklassnikiInDesktop
                && !string.IsNullOrWhiteSpace(SettingsSocialWidget.LinkOdnoklassniki);
            model.IsShowTelegram = SettingsSocialWidget.IsShowTelegramInDesktop
                && !string.IsNullOrWhiteSpace(SettingsSocialWidget.LinkTelegram);

            model.IsShowCustomWidget1 = !string.IsNullOrEmpty(SettingsSocialWidget.CustomLink1) &&
                                        !string.IsNullOrEmpty(SettingsSocialWidget.CustomLinkText1);

            model.IsShowCustomWidget2 = !string.IsNullOrEmpty(SettingsSocialWidget.CustomLink2) &&
                                        !string.IsNullOrEmpty(SettingsSocialWidget.CustomLinkText2);

            model.IsShowCustomWidget3 = !string.IsNullOrEmpty(SettingsSocialWidget.CustomLink3) &&
                                        !string.IsNullOrEmpty(SettingsSocialWidget.CustomLinkText3);

            model.ActiveCount =
                new[]
                {
                    model.IsShowVk, model.IsShowFb,
                    model.IsShowCallback, model.IsShowCallbackIpTelephony,
                    model.IsShowJivosite, model.IsShowWhatsApp, model.IsShowViber,
                    model.IsShowOdnoklassniki, model.IsShowTelegram,
                    model.IsShowCustomWidget1, model.IsShowCustomWidget2, model.IsShowCustomWidget3
                }.Count(x => x);

            if (model.ActiveCount == 0)
                return new EmptyResult();
            
            model.ShowPoppover = SettingsSocialWidget.ShowPopover;
            model.PoppoverText = SettingsSocialWidget.PopoverText;
            
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult MobileScript()
        {
            if (!SettingsSocialWidget.IsActive)
                return new EmptyResult();
            
            if (Request.IsMobileApp())
                return new EmptyResult();

            var model = new SocialWidgetModel();

            if (SettingsSocialWidget.IsShowVkInMobile && VkApiService.IsVkActive())
            {
                model.IsShowVk = true;
                model.VkGroupId = SettingsVk.Group.Id;
            }

            if (SettingsSocialWidget.IsShowFbInMobile && new FacebookApiService().IsActive())
            {
                model.IsShowFb = true;
                model.FacebookMsgId = SettingsFacebook.GroupId;
            }

            if (SettingsSocialWidget.IsShowJivositeInMobile)
            {
                var module = AttachedModules.GetModuleById("JivoSite", true);
                if (module != null)
                    model.IsShowJivosite = true;
            }

            if (SettingsSocialWidget.IsShowCallbackInMobile)
            {
                var callback = IPTelephonyOperator.Current.CallBack;
                model.IsShowCallbackIpTelephony = callback != null && callback.Enabled &&
                                                  (!SaasDataService.IsSaasEnabled ||
                                                   SaasDataService.CurrentSaasData.HaveTelephony);

                if (!model.IsShowCallbackIpTelephony)
                {
                    var module = AttachedModules.GetModuleById("Callback", true);
                    if (module != null)
                        model.IsShowCallback = true;
                }
            }

            if (SettingsSocialWidget.IsShowWhatsAppInMobile && SettingsSocialWidget.WhatsAppPhone.IsNotEmpty())
            {
                model.IsShowWhatsApp = true;
                model.LinkWhatsApp = "https://wa.me/" + SettingsSocialWidget.WhatsAppPhone;
            }
            model.IsShowViber = SettingsSocialWidget.IsShowViberInMobile;
            model.IsShowOdnoklassniki = SettingsSocialWidget.IsShowOdnoklassnikiInMobile
                && !string.IsNullOrWhiteSpace(SettingsSocialWidget.LinkOdnoklassniki);
            model.IsShowTelegram = SettingsSocialWidget.IsShowTelegramInMobile
                && !string.IsNullOrWhiteSpace(SettingsSocialWidget.LinkTelegram);

            model.IsShowCustomWidget1 = !string.IsNullOrEmpty(SettingsSocialWidget.CustomLink1) &&
                                        !string.IsNullOrEmpty(SettingsSocialWidget.CustomLinkText1);

            model.IsShowCustomWidget2 = !string.IsNullOrEmpty(SettingsSocialWidget.CustomLink2) &&
                                        !string.IsNullOrEmpty(SettingsSocialWidget.CustomLinkText2);

            model.IsShowCustomWidget3 = !string.IsNullOrEmpty(SettingsSocialWidget.CustomLink3) &&
                                        !string.IsNullOrEmpty(SettingsSocialWidget.CustomLinkText3);

            model.ActiveCount =
                new[]
                {
                    model.IsShowVk, model.IsShowFb,
                    model.IsShowCallback, model.IsShowCallbackIpTelephony,
                    model.IsShowJivosite, model.IsShowWhatsApp, model.IsShowViber,
                    model.IsShowOdnoklassniki, model.IsShowTelegram,
                    model.IsShowCustomWidget1, model.IsShowCustomWidget2, model.IsShowCustomWidget3
                }.Count(x => x);

            if (model.ActiveCount == 0)
                return new EmptyResult();

            model.ShowPoppover = SettingsSocialWidget.ShowPopover;
            model.PoppoverText = SettingsSocialWidget.PopoverText;
            
            return PartialView("Script", model);
        }
    }
}