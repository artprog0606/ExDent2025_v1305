using System;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Security;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Handlers.Settings.Users;
using AdvantShop.Web.Admin.Handlers.Shared.Account;
using AdvantShop.Web.Admin.Models.Settings.Users;
using AdvantShop.Web.Admin.ViewModels.Shared.Account;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using BotDetect.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public class AccountController : BaseController
    {
        private const string ForgotPasswordAdminCaptchaCount = "forgotPassword_admin_login_count";

        public ActionResult Login()
        {
            var customer = CustomerContext.CurrentCustomer;
            if (customer.Enabled && (customer.IsAdmin || customer.IsVirtual || customer.IsModerator))
                return RedirectToAction("Index", "Home");

            var model = new AccountLoginViewModel();

            var from = Request["from"];
            if (from != null && from.StartsWith("/"))
                model.From = from;

            var count = Convert.ToInt32(Session["admin_login_count"]);
            if (count >= 3)
                model.ShowCaptcha = true;

            if (Request["frompage"] == "closed")
                Track.TrackService.TrackEvent(Track.ETrackEvent.ClientBlocker_Authorized);

            var site = new LpSiteService().GetByDomainUrl(SettingsMain.SiteUrl);
            model.MainSiteName = site != null ? site.Name : SettingsMain.ShopName;
            model.Login = Request["login"];
            
            return View(model);
        }

        [HttpPost]
        [CaptchaValidation("CaptchaCode", "CaptchaSource")]
        public ActionResult Login(string txtLogin, string txtPassword, string from)
        {
            var email = txtLogin;
            var password = txtPassword;
            var count = Convert.ToInt32(Session["admin_login_count"]);

            if (!string.IsNullOrEmpty(email) && 
                !string.IsNullOrEmpty(password) && 
                (count < 3 || ModelState.IsValidField("CaptchaCode")))
            {
                if (!string.IsNullOrEmpty(SettingsManager.ActiveTwoFactorAuthModule) &&
                    SettingsManager.ActiveTwoFactorAuthModule != "-1" &&
                    AuthorizeService.IsTwoFactorAuth(email, password))
                {
                    Session["login_two_factor"] = email;
                    Session["password_two_factor"] = password;
                    return RedirectToAction("CodeAuth");
                }

                var adminFcmToken = HttpContext.Request.Cookies["afcm"]?.Value;
                if (AuthorizeService.SignIn(email, password, false, true, HttpUtility.UrlDecode(adminFcmToken)))
                {
                    Session.Remove("admin_login_count");
                    MvcCaptcha.ResetCaptcha("CaptchaSource");

                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Common_Login_AdminArea);

                    if (!string.IsNullOrWhiteSpace(from) && from.StartsWith("/"))
                        return Redirect(UrlService.GenerateBaseUrl(appendApplicationPath: false) + "/" + from.TrimStart('/'));

                    if (!string.IsNullOrEmpty(SettingsMain.AdminHomeForceRedirectUrl))
                        return Redirect(SettingsMain.AdminHomeForceRedirectUrl);

                    return RedirectToAction("Index", "Home");
                }
            }
            ShowMessage(NotifyType.Error, T("User.Login.WrongPassword"));
            Session["admin_login_count"] = Convert.ToInt32(Session["admin_login_count"]) + 1;

            return RedirectToAction("Login", new {from});
        }

        public ActionResult CodeAuth()
        {
            if(Session["login_two_factor"] == null)
                return RedirectToAction("Login");
            var model = new AccountLoginViewModel();

            var site = new LpSiteService().GetByDomainUrl(SettingsMain.SiteUrl);
            model.MainSiteName = site != null ? site.Name : SettingsMain.ShopName;
            model.Login = Request["login"];
            
            return View(model);
        }
                
        [HttpPost]
        public ActionResult CodeAuth(string txtCode)
        {
            var code = txtCode;
            var email = Convert.ToString(Session["login_two_factor"]);
            var password = Convert.ToString(Session["password_two_factor"]);
            if (AuthorizeService.IsTwoFactorAuthCodeValid(email, password, code))
            {
                if (AuthorizeService.SignIn(email, password, false, true))
                {
                    Session.Remove("admin_login_count");
                    Session.Remove("login_two_factor");
                    Session.Remove("password_two_factor");
                    MvcCaptcha.ResetCaptcha("CaptchaSource");

                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Common_Login_AdminArea);

                    if (!string.IsNullOrEmpty(SettingsMain.AdminHomeForceRedirectUrl))
                        return Redirect(SettingsMain.AdminHomeForceRedirectUrl);

                    return RedirectToAction("Index", "Home");
                }
            }

            ShowMessage(NotifyType.Error, T("User.Login.WrongCode"));

            return RedirectToAction("CodeAuth");
        }

        public ActionResult SetPassword(string email, string hash)
        {
            var model = new GetForgotPasswordViewModel(email, hash).Execute();
            model.FirstVisit = true;

            return View("ForgotPassword", model);
        }

        public ActionResult ForgotPassword(string email, string hash)
        {
            var model = new GetForgotPasswordViewModel(email, hash).Execute();

            var count = Convert.ToInt32(Session[ForgotPasswordAdminCaptchaCount]);
            if (count >= 2)
                model.ShowCaptcha = true;

            SetMetaInformation(T("Admin.Shared.ForgotPassword"));
            var site = new LpSiteService().GetByDomainUrl(SettingsMain.SiteUrl);
            model.MainSiteName = site != null ? site.Name : SettingsMain.ShopName;

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "CaptchaSource")]
        public ActionResult ForgotPassword(string email)
        {
            var model = new ForgotPasswordViewModel() { View = EForgotPasswordView.EmailSent };
            if (email.IsNullOrEmpty())
            {
                model.View = EForgotPasswordView.ForgotPassword;
                ModelState.AddModelError(string.Empty, T("Admin.Shared.EnterAnEmail"));
                return View(model);
            }

            var count = Convert.ToInt32(Session[ForgotPasswordAdminCaptchaCount]);
            if (count >= 2)
            {
                model.ShowCaptcha = true;

                if (!ModelState.IsValidField("CaptchaCode"))
                {
                    model.View = EForgotPasswordView.ForgotPassword;
                    model.Email = email;
                    ModelState.AddModelError(string.Empty, "Неправильно введен код капчи");

                    return View(model);
                }
            }

            var customer = CustomerService.GetCustomerByEmail(email);
            if (customer == null || !customer.Enabled || !(customer.IsAdmin || customer.IsModerator))
            {
                model.View = EForgotPasswordView.ForgotPassword;
                ModelState.AddModelError(string.Empty, T("Admin.Shared.EmployeeWithEmailNotFound"));

                Session[ForgotPasswordAdminCaptchaCount] = Convert.ToInt32(Session[ForgotPasswordAdminCaptchaCount]) + 1;
            }
            else
            {
                var mailTpl = new UserPasswordRepairMailTemplate(customer.EMail, 
                    ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(customer.Password)).ToLower());

                MailService.SendMailNow(customer.Id, customer.EMail, mailTpl);

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Common_ForgotPassword_AdminArea);

                Session[ForgotPasswordAdminCaptchaCount] = 0;
            }

            var site = new LpSiteService().GetByDomainUrl(SettingsMain.SiteUrl);
            model.MainSiteName = site != null ? site.Name : SettingsMain.ShopName;

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string newPassword, string newPasswordConfirm, string email, string hash, bool firstVisit = false)
        {
            var model = new ForgotPasswordViewModel()
            {
                View = EForgotPasswordView.PasswordRecovery,
                Email = email,
                Hash = hash,
                FirstVisit = firstVisit
            };

            if (newPassword.IsNullOrEmpty() || newPasswordConfirm.IsNullOrEmpty())
            {
                ModelState.AddModelError(string.Empty, T("Admin.Shared.EnterThePassword"));
                return View("ForgotPassword", model);
            }
            if (newPassword.Length < 6)
            {
                ModelState.AddModelError(string.Empty, T("Admin.Shared.PasswordMust6Characters"));
                return View("ForgotPassword", model);
            }
            if (newPassword != newPasswordConfirm)
            {
                ModelState.AddModelError(string.Empty, T("Admin.Shared.PasswordsNotMatch"));
                return View("ForgotPassword", model);
            }

            var customer = CustomerService.GetCustomerByEmail(model.Email);
            if (customer != null && customer.Enabled && (customer.IsAdmin || customer.IsModerator))
            {
                if (ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(customer.Password)).ToLower() == model.Hash.ToLower())
                {
                    CustomerService.ChangePassword(customer.Id, newPassword, false);
                    if (!string.IsNullOrEmpty(SettingsManager.ActiveTwoFactorAuthModule) &&
                        SettingsManager.ActiveTwoFactorAuthModule != "-1" &&
                        AuthorizeService.IsTwoFactorAuth(email, newPassword))
                    {
                        Session["login_two_factor"] = email;
                        Session["password_two_factor"] = newPassword;
                        return RedirectToAction("CodeAuth");
                    }
                    AuthorizeService.SignIn(model.Email, newPasswordConfirm, false, true);
                    if (firstVisit)
                    {
                        return Redirect(Url.AbsoluteRouteUrl(new { controller = "Home", action = "Index" }) + "?user=me");
                    }

                    model.View = EForgotPasswordView.PasswordChanged;
                }
                else
                {
                    model.View = EForgotPasswordView.RecoveryError;
                }
            }

            var site = new LpSiteService().GetByDomainUrl(SettingsMain.SiteUrl);
            model.MainSiteName = site != null ? site.Name : SettingsMain.ShopName;
            
            return View("ForgotPassword", model);
        }

        public JsonResult GetUserInfo(Guid customerId)
        {
            var dbModel = CustomerService.GetCustomer(customerId);
            if (dbModel == null)
                return JsonError(T("Admin.Users.Validate.NotFound"));
            return JsonOk(new GetUserModel(dbModel).Execute());
        }

        public JsonResult GetUserFormData()
        {
            return ProcessJsonResult(new GetUserFormDataHandler(CustomerContext.CustomerId));
        }

        public JsonResult GetCurrentUser()
        {
            return JsonOk(new GetUserModel(CustomerContext.CurrentCustomer).Execute());
        }

        public ActionResult TopPanelUser()
        {
            return PartialView("~/Areas/Admin/Templates/Mobile/Views/Common/TopPanelUser.cshtml",new GetUserModel(CustomerContext.CurrentCustomer).Execute());
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCurrentUser(AdminUserModel model)
        {
            if (model.CustomerId != CustomerContext.CustomerId)
                return JsonError();
            return ProcessJsonResult(new AddEditUserHandler(model, true));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SendChangePasswordMail()
        {
            return ProcessJsonResult(new SendChangePasswordEmailHandler(CustomerContext.CustomerId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePasswordJson(string password, string passwordConfirm)
        {
            return ProcessJsonResult(new ChangePasswordHandler(CustomerContext.CustomerId, password, passwordConfirm));
        }


        [HttpPost]
        [CaptchaValidation("CaptchaCode", "CaptchaSource")]
        public ActionResult SignInGuest(string from)
        {
            if (ModelState.IsValidField("CaptchaCode"))
            {
                Track.TrackService.TrackEvent(Track.ETrackEvent.ClientBlocker_Authorized);
                CustomerService.SetTechDomainGuest();
            }

            var url = !string.IsNullOrEmpty(from) ? from : UrlService.GetUrl();

            return Redirect(url);
        }

        public ActionResult SignInByToken(string token, string to)
        {
            var valueExists = CacheManager.TryGetValue<Customer>(token, out var customer);
            if (valueExists)
            {
                CacheManager.Remove(token);
                AuthorizeService.SignIn(customer.EMail, customer.Password, true, true);
            }

            var url = !string.IsNullOrEmpty(to) ? to : UrlService.GetUrl();

            return Redirect(url);
        }

        public ActionResult RedirectWithAuth(string domain, string path = null)
        {
            var token = Guid.NewGuid();

            CacheManager.Insert(token.ToString(), CustomerContext.CurrentCustomer);

            var s = domain + $"/adminv2/account/SignInByToken?token={token}&to={domain + path}";
            return Redirect(s);
        }

        public ActionResult Logout()
        {
            AuthorizeService.SignOut();

            return Redirect(SettingsMain.SiteUrl + "/logout");
        }

        [HttpGet]
        public ActionResult IsNotifyEnabled()
        {
            var enabled = !TrialService.IsTrialEnabled;
            return Json(new
            {
                enabled
            });
        }
    }
}
