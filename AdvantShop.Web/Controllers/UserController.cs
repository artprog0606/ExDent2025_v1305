using AdvantShop.Configuration;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Handlers.User;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Models.User;
using AdvantShop.Security;
using AdvantShop.Security.OAuth;
using AdvantShop.ViewModel.User;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using BotDetect.Web.Mvc;
using System;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Diagnostics;

namespace AdvantShop.Controllers
{
    public class UserController : BaseClientController
    {
        private const string LoginJsonCaptchaCount = "login_json_count";
        private const string ForgotPasswordCaptchaCount = "forgotPassword_login_count";

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult LoginJson(string email, string password, string captchaCode, string captchaSource)
        {
            if (CustomerContext.CurrentCustomer.RegistredUser)
                return Json(new LoginResult("error", "User authorized"));

            var count = Convert.ToInt32(Session[LoginJsonCaptchaCount]);
            var requestCaptcha = count >= 2;

            if (count > 2 && !MvcCaptcha.Validate("CaptchaSource", captchaCode, captchaSource))
            {
                return Json(new LoginResult("error", T("Js.Captcha.Wrong"), requestCaptcha));
            }

            if (!string.IsNullOrEmpty(SettingsManager.ActiveTwoFactorAuthModule) &&
                SettingsManager.ActiveTwoFactorAuthModule != "-1" &&
                AuthorizeService.IsTwoFactorAuth(email, password))
            {
                Session["login_two_factor"] = email;
                Session["password_two_factor"] = password;
                Session[LoginJsonCaptchaCount] = 0;
                return Json(new LoginResult("success")
                {
                    RedirectTo = UrlService.GetUrl("adminv2/codeauth")
                });
            }
            
            Customer customer;

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && AuthorizeService.SignIn(email, password, false, true, out customer))
            {
                Session[LoginJsonCaptchaCount] = 0;
                return Json(new LoginResult("success")
                {
                    RedirectTo = !customer.IsAdmin && !customer.IsModerator ? Url.AbsoluteRouteUrl("home") : null
                });
            }

            Session[LoginJsonCaptchaCount] = count + 1;

            return Json(new LoginResult("error", T("User.Login.WrongPassword"), requestCaptcha));
        }

        [HttpGet]
        public ActionResult Login(string redirectTo, string state, string code)
        {
            if (!string.IsNullOrEmpty(state) && state.Contains("googleanalytics"))
            {
                GoogleOAuth.LoginAnalytics(code, "login");

                return RedirectToRoute("Home", new { state = "googleanalytics" });
            }

            if (CustomerContext.CurrentCustomer.RegistredUser)
            {
                if (!string.IsNullOrEmpty(redirectTo) && redirectTo != "/")
                    return Redirect(redirectTo);

                return RedirectToRoute(CustomerContext.CurrentCustomer.EMail.Contains("@temp") ? "MyAccount" : "Home");
            }

            EAuthMethod authMethod;

            switch (state)
            {
                case "smsAuth":
                    authMethod = EAuthMethod.Email;
                    break;
                case "emailAuth":
                    authMethod = EAuthMethod.Code;
                    break;
                default:
                    authMethod = SettingsOAuth.AuthMethod == EAuthMethod.Code && !SettingsOAuth.AuthByCodeActive 
                        ? EAuthMethod.Email
                        : SettingsOAuth.AuthMethod;
                    break;
            }
            
            var model = new LoginViewModel
            {
                AuthMethod = authMethod,
            };

            SetMetaInformation(T("User.Login.Header"));
            SetNoFollowNoIndex();
            SetNgController(NgControllers.NgControllersTypes.LoginCtrl);
            return View(model);
        }

        [TechDomainGuard(Disable = true)]
        public ActionResult LoginToken(string email, string hash, string redirectTo, bool? showhelp)
        {
            SettingsLic.ShowAdvantshopJivoSiteForm = showhelp ?? false;
            var customer = CustomerService.GetCustomerByEmail(email);
            if (customer != null)
            {
                var hashComputed = SecurityHelper.EncodeWithHmac(customer.EMail, customer.Password);
                if (hash == hashComputed && AuthorizeService.SignIn(customer.EMail, customer.Password, true, true))
                {
                    var adminDomain = UrlService.GetAbsoluteBaseLink();
                    if (SettingsMain.IsTechDomainsReady) adminDomain = SettingsMain.TechDomainAdminPanel;

                    if (!string.IsNullOrEmpty(redirectTo) && redirectTo != "/" && redirectTo != "~/")
                    {
                        var trimmedRedirectTo = redirectTo.TrimStart('~');
                        if (trimmedRedirectTo.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
                            return Redirect(Url.AbsoluteActionUrl("RedirectWithAuth", "Account", new
                                {
                                    area = "AdminV2",
                                    domain = adminDomain,
                                    path = trimmedRedirectTo,
                                })
                            );

                        return Redirect(redirectTo);
                    }

                    if (!string.IsNullOrEmpty(SettingsMain.AdminHomeForceRedirectUrl))
                        return Redirect(
                            Uri.IsWellFormedUriString(SettingsMain.AdminHomeForceRedirectUrl, UriKind.Absolute)
                                ? SettingsMain.AdminHomeForceRedirectUrl
                                : Url.AbsoluteActionUrl("RedirectWithAuth", "Acc", new
                                {
                                    area = "AdminV2",
                                    domain = adminDomain,
                                    path = SettingsMain.AdminHomeForceRedirectUrl,
                                })
                        );

                    if (string.IsNullOrEmpty(redirectTo))
                        return Redirect(Url.AbsoluteActionUrl("RedirectWithAuth", "Account", new
                        {
                            area = "AdminV2",
                            domain = adminDomain,
                            path = "/adminv2",
                        }));
                }
            }

            return Redirect("~/");
        }

        public ActionResult Authorization(AuthorizationViewModel model)
        {
            if (model == null)
                model = new AuthorizationViewModel();

            var referrer = Request.GetUrlReferrer();
            if (model.RedirectTo == null && referrer != null)
            {
                if (!string.IsNullOrEmpty(referrer.ToString()) && referrer.Host.Contains(CommonHelper.GetParentDomain()))
                    model.RedirectTo = referrer.ToString();
            }

            return PartialView(model);
        }

        #region Code Auth

        public ActionResult CodeAuthorization(AuthorizationViewModel model)
        {
            if (model == null)
                model = new AuthorizationViewModel();

            var referrer = Request.GetUrlReferrer();
            if (model.RedirectTo == null && referrer != null)
            {
                if (!string.IsNullOrEmpty(referrer.ToString()) && referrer.Host.Contains(CommonHelper.GetParentDomain()))
                    model.RedirectTo = referrer.ToString();
            }
            
            return PartialView(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendCode(SendCodeModel model)
        {
            return ProcessJsonResult(new SendCode(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ConfirmCode(string phone, string code, bool signUp)
        {
            return ProcessJsonResult(new ConfirmCode(phone, code, signUp));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult IsPhoneConfirmed(string phone)
        {
            var phoneParsed = StringHelper.ConvertToStandardPhone(phone) ?? 0;
            if (phoneParsed == 0)
                return JsonOk(new {isConfirmed = false});

            var isPhoneConfirmed = new PhoneConfirmationService().IsPhoneConfirmed(phoneParsed, CustomerContext.CustomerId);
            
            return JsonOk(new {isConfirmed = isPhoneConfirmed});
        }

        [HttpGet]
        public string GetCodeDescription() => new GetCodeDescriptionHandler().Execute();

        #endregion

        [AdminMobileAppGuard(Disable = true)]
        public ActionResult Logout()
        {
            AuthorizeService.SignOut();

            if (MobileHelper.IsMobileAdminApp())
            {
                CustomerAdminPushNotificationService.UpdateFcmToken(CustomerContext.CustomerId, null);
                RedirectToAction("Login", "Account", new { area = "AdminV2" });
            }

            if (Request.GetUrlReferrer() != null)
            {
                var referrer = Request.GetUrlReferrer().ToString();

                if (!string.IsNullOrEmpty(referrer) && !(referrer.Contains("admin") 
                                                         || referrer.Contains("checkout") 
                                                         || referrer.Contains("advantshop.net")))
                    return Redirect(referrer);
            }

            return RedirectToRoute("Home");
        }

        #region OAuth
        public ActionResult LoginOpenId(string pageToRedirect, string code, string state)
        {
            pageToRedirect = (pageToRedirect ?? "").TrimStart('/').ToLower();

            var model = new LoginOpenIdViewModel()
            {
                DisplayFacebook = SettingsOAuth.FacebookActive,
                DisplayGoogle = SettingsOAuth.GoogleActive,
                DisplayMailRu = SettingsOAuth.MailActive,
                DisplayOdnoklassniki = SettingsOAuth.OdnoklassnikiActive,
                DisplayVk = SettingsOAuth.VkontakteActive,
                DisplayVkId = SettingsOAuth.VkIdActive, 
                DisplayYandex = SettingsOAuth.YandexActive,
                PageToRedirect = pageToRedirect
            };

            if (!model.AnyActive)
                return new EmptyResult();

            if (code.IsNullOrEmpty() || state.IsNotEmpty())
                return PartialView(model);
            
            var stateParam = state.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (stateParam.Length != 3 || 
                (stateParam.Length == 3 && !string.Equals(stateParam[2], CustomerContext.CurrentCustomer.Id.ToString())))
            {
                return PartialView(model);
            }

            if (model.DisplayVk && string.Equals(stateParam[0], "vk"))
            {
                VkOAuth.Login(code, pageToRedirect);
            }

            if (model.DisplayOdnoklassniki && string.Equals(stateParam[0], "ok"))
            {
                OkOAuth.Login(code, pageToRedirect);
            }

            if (model.DisplayGoogle && string.Equals(stateParam[0], "google"))
            {
                GoogleOAuth.Login(code, pageToRedirect);
            }

            if (model.DisplayMailRu && string.Equals(stateParam[0], "mail"))
            {
                MailOAuth.Login(code, pageToRedirect);
            }

            if (model.DisplayYandex && string.Equals(stateParam[0], "yandex"))
            {
                YandexOAuth.Login(code, pageToRedirect);
            }

            if (model.DisplayFacebook && string.Equals(stateParam[0], "fb"))
            {
                FacebookOAuth.Login(code, pageToRedirect);
            }

            Response.Redirect(stateParam[1], false);

            return PartialView(model);
        }

        public ActionResult LoginVk(string pageToRedirect)
        {
            return Redirect(VkOAuth.OpenDialog(pageToRedirect));
        }

        public ActionResult LoginFacebook(string pageToRedirect)
        {
            return Redirect(FacebookOAuth.OpenDialog(pageToRedirect));
        }

        public ActionResult LoginGoogle(string pageToRedirect)
        {
            return Redirect(GoogleOAuth.OpenDialog(pageToRedirect));
        }

        public ActionResult LoginGoogleAnalytics(string pageToRedirect)
        {
            return Redirect(GoogleOAuth.OpenAnalyticsDialog(pageToRedirect));
        }

        public ActionResult LoginOk(string pageToRedirect)
        {
            return Redirect(OkOAuth.OpenDialog(pageToRedirect));
        }

        public ActionResult LoginMailRu(string pageToRedirect)
        {
            return Redirect(MailOAuth.OpenDialog(pageToRedirect));
        }

        public ActionResult LoginYandex(string pageToRedirect)
        {
            return Redirect(YandexOAuth.OpenDialog(pageToRedirect));
        }

        public ActionResult LoginOAuth(string provider, string pageToRedirect)
        {
            var returUrl = string.IsNullOrEmpty(pageToRedirect) ? this.Url.Action("LoginExternal", "User", null, this.Request.Url.Scheme) : pageToRedirect;
            return string.IsNullOrWhiteSpace(provider)
                ? Redirect(PassportAdvantService.GetLoginPage(returUrl, SettingsLic.LicKey))
                : Redirect(PassportAdvantService.GetLoginRequest(returUrl, provider, SettingsLic.LicKey));
        }

        public async Task<ActionResult> LoginExternal(string code, string state)
        {
            if (string.IsNullOrEmpty(state)) throw new Exception("state missing");
            var data = state.Split('|');
            if (data.Length != 3) throw new Exception("state missing");
            var provider = data[0];
            var pageToRedirect = data[1];
            var advClientId = data[2];

            pageToRedirect = pageToRedirect.TrimStart('/').ToLower();

            await PassportAdvantService.Login(code, state);

            return Redirect(pageToRedirect);
        }
        #endregion

        #region Registration

        public ActionResult Registration(int? lpId)
        {
            if (CustomerContext.CurrentCustomer.RegistredUser || SettingsMain.RegistrationIsProhibited)
            {
                return RedirectToRoute("Home");
            }

            SetMetaInformation(T("User.Registration.Registration"));
            SetNoFollowNoIndex();
            SetNgController(NgControllers.NgControllersTypes.RegistrationPageCtrl);

            var model = new RegistrationViewModel();

            if (Demo.IsDemoEnabled)
            {
                model.IsDemo = true;
                model.Email = Demo.GetRandomEmail();
                model.FirstName = Demo.GetRandomName();
                model.LastName = Demo.GetRandomLastName();
                model.Phone = Demo.GetRandomPhone();
            }

            if (BonusSystem.IsActive)
            {
                model.IsBonusSystemActive = true;
                model.WantBonusCard = true;

                var bonuses = BonusSystem.BonusesForNewCard;
                if (bonuses != 0)
                    model.BonusesForNewCard = bonuses.FormatPrice();
            }

            if (lpId != null)
            {
                var lp = new LpService().Get(lpId.Value);
                if (lp != null)
                {
                    model.LpId = lpId;
                    LpService.CurrentLanding = lp;
                    SettingsDesign.IsMobileTemplate = false;
                }
            }

            return View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RegistrationJson(RegistrationViewModel model)
        {
            if (CustomerContext.CurrentCustomer.RegistredUser)
                return JsonOk();

            var handler = new RegistrationHandler();
            var error = handler.IsValid(model);

            if (!string.IsNullOrEmpty(error))
                return JsonError(error);
            
            var allowReg = ModulesExecuter.CheckInfo(System.Web.HttpContext.Current, Core.Modules.Interfaces.ECheckType.Registration, model.Email, model.FirstName, phone: model.Phone);
            if (!allowReg)
                return JsonError(T("Common.SpamCheckFailed"));

            handler.Register(model);

            TempData["IsRegisteredNow"] = "true";

            if (model.LpId != null)
            {
                var service = new LpService();
                var lp = service.Get(model.LpId.Value);
                if (lp != null)
                    return JsonOk(service.GetLpLink(lp.Id));
            }

            if (BonusSystem.IsActive)
            {
                model.IsBonusSystemActive = true;

                var bonuses = BonusSystem.BonusesForNewCard;
                if (bonuses != 0)
                    model.BonusesForNewCard = bonuses.FormatPrice();
            }

            // return JsonOk(Url.RouteUrl("MyAccount") + "?tab=orderhistory");
            var isShowSelfDelivery = TemplateSettingsProvider.Items["ShowSelfDeliveryInDeliveryWidgetOnMain"].TryParseBool();
            var isShowPickPoint = TemplateSettingsProvider.Items["ShowDeliveryInDeliveryWidgetOnMain"].TryParseBool();
            
            
            return JsonOk(Url.RouteUrl("Home") + (isShowPickPoint || isShowSelfDelivery  ? "?geoMode=address" : ""));
        }

        #endregion

        #region Forgot password

        public ActionResult ForgotPassword(string email, string recoverycode, int? lpId)
        {
            var model = new ForgotPasswordModel() { View = "forgotpass", Email = email, RecoveryCode = recoverycode };

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(recoverycode))
            {
                var customer = CustomerService.GetCustomerByEmail(email);
                if (customer != null)
                {
                    var hash = ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(!string.IsNullOrWhiteSpace(customer.Password) ? customer.Password : customer.EMail));

                    if (hash.ToLower() == model.RecoveryCode.ToLower())
                        model.View = "recovery";
                }
            }

            if (lpId != null)
            {
                var lp = new LpService().Get(lpId.Value);
                if (lp != null)
                {
                    model.LpId = lpId;
                    LpService.CurrentLanding = lp;
                    SettingsDesign.IsMobileTemplate = false;
                }
            }

            var count = Convert.ToInt32(Session[ForgotPasswordCaptchaCount]);
            if (count >= 2)
                model.ShowCaptcha = true;

            SetMetaInformation(T("User.ForgotPassword.PasswordRecovery"));
            SetNoFollowNoIndex();
            SetNgController(NgControllers.NgControllersTypes.ForgotPasswordCtrl);

            return lpId != null ? View("~/Views/User/ForgotPassword.cshtml", model) : View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ForgotPasswordJson(string email, string captchaCode, string captchaSource, int? lpId)
        {
            var count = Convert.ToInt32(Session[ForgotPasswordCaptchaCount]);
            var requestCaptcha = count >= 2;

            if (count > 2 && !MvcCaptcha.Validate("CaptchaSource", captchaCode, captchaSource))
            {
                return Json(new ForgotPasswordResultModel("error", T("Js.Captcha.Wrong"), requestCaptcha));
            }

            var customer = CustomerService.GetCustomerByEmail(email);
            if (customer != null)
            {
                var lp = lpId != null ? new LpService().Get(lpId.Value) : null;
                var lpSite = lp != null ? new LpSiteService().Get(lp.LandingSiteId) : null;

                var recoveryCode =
                    ValidationHelper.DeleteSigns(
                        SecurityHelper.GetPasswordHash(!string.IsNullOrWhiteSpace(customer.Password)
                            ? customer.Password
                            : customer.EMail));

                var link =
                    string.Format("{0}/forgotpassword?email={1}&recoverycode={2}{3}",
                        lpSite != null && !string.IsNullOrEmpty(lpSite.DomainUrl) ? lpSite.DomainUrl : SettingsMain.SiteUrl, // todo: lp domain
                        customer.EMail,
                        recoveryCode,
                        lpId != null ? "&lpId=" + lpId : null);

                var mail = new PwdRepairMailTemplate(recoveryCode.ToLower(), customer.EMail, link);

                MailService.SendMailNow(customer.Id, customer.EMail, mail);

                Session[ForgotPasswordCaptchaCount] = 0;
                return Json(new ForgotPasswordResultModel("success"));
            }

            Session[ForgotPasswordCaptchaCount] = count + 1;

            return Json(new ForgotPasswordResultModel("error", T("User.ForgotPassword.EmailNotFound"), requestCaptcha));
        }


        [HttpPost]
        public ActionResult ChangePassword(string newPassword, string newPasswordConfirm, string email, string recoveryCode)
        {
            var model = new ForgotPasswordModel() { View = "recovery", Email = email, RecoveryCode = recoveryCode };

            SetMetaInformation(T("User.ForgotPassword.PasswordRecovery"));

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(newPasswordConfirm) ||
                newPassword != newPasswordConfirm)
            {
                ShowMessage(NotifyType.Error, T("User.ForgotPassword.PasswordDiffrent"));
                return View("ForgotPassword", model);
            }

            var customer = CustomerService.GetCustomerByEmail(model.Email);
            if (customer != null)
            {
                var hash = ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(!string.IsNullOrWhiteSpace(customer.Password) ? customer.Password : customer.EMail));
                if (hash.ToLower() == model.RecoveryCode.ToLower())
                {
                    CustomerService.ChangePassword(customer.Id, newPassword, false);
                    AuthorizeService.SignIn(model.Email, newPasswordConfirm, false, true);

                    model.View = "passwordChanged";
                }
                else
                {
                    model.View = "recoveryError";
                }
            }

            return View("ForgotPassword", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePasswordJson(string newPassword, string newPasswordConfirm, string email, string recoveryCode, int? lpId)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(newPasswordConfirm) || newPassword != newPasswordConfirm)
            {
                return Json(new LoginResult("error", T("User.ForgotPassword.PasswordDiffrent")));
            }

            var customer = CustomerService.GetCustomerByEmail(email);
            if (customer != null)
            {
                var hash = ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(!string.IsNullOrWhiteSpace(customer.Password) ? customer.Password : customer.EMail));

                if (hash.ToLower() == recoveryCode.ToLower())
                {
                    CustomerService.ChangePassword(customer.Id, newPassword, false);
                    if (!string.IsNullOrEmpty(SettingsManager.ActiveTwoFactorAuthModule) &&
                        SettingsManager.ActiveTwoFactorAuthModule != "-1" &&
                        AuthorizeService.IsTwoFactorAuth(email, newPassword))
                    {
                        Session["login_two_factor"] = email;
                        Session["password_two_factor"] = newPassword;
                        Session[LoginJsonCaptchaCount] = 0;
                        return Json(new ForgotPasswordResultModel("success")
                        {
                            RedirectTo = UrlService.GetUrl("adminv2/codeauth")
                        });
                    }
                    AuthorizeService.SignIn(email, newPasswordConfirm, false, true);

                    return Json(new ForgotPasswordResultModel("success"));
                }
            }

            return Json(new ForgotPasswordResultModel("error", T("User.ForgotPassword.Error")));
        }

        #endregion

        #region ClientCode

        [ChildActionOnly]
        public ActionResult ClientCode()
        {
            if (BrowsersHelper.IsBot())
                return new EmptyResult();
            
            if (!SettingsDesign.ShowClientId)
                // стили по ClientCode необходимо включить в CriticalCss
                if (!DebugMode.IsDebugMode(eDebugMode.CriticalCss)) 
                    return new EmptyResult();

            return PartialView(new ClientCodeViewModel());
        }

        public JsonResult GetClientCode()
        {
            if (BrowsersHelper.IsBot())
                return JsonOk();
            
            if (!SettingsDesign.ShowClientId)
                // стили по ClientCode необходимо включить в CriticalCss
                if (!DebugMode.IsDebugMode(eDebugMode.CriticalCss)) 
                    return JsonOk();
            
            var code = ClientCodeService.GetClientCode(CustomerContext.CustomerId);

            return JsonOk(new ClientCodeViewModel()
            {
                Code = code.ToString("##,##0").Replace(",", "-").Replace("\u00A0", "-")
            });
        }

        #endregion

    }
}