using System;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Areas.Partners.Attributes;
using AdvantShop.Areas.Partners.Handlers.Account;
using AdvantShop.Areas.Partners.Models.Account;
using AdvantShop.Areas.Partners.ViewModels.Account;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using BotDetect.Web.Mvc;

namespace AdvantShop.Areas.Partners.Controllers
{
    [ExcludeFilter(typeof(PartnerAuthAttribute))]
    [SessionState(SessionStateBehavior.Default)]
    public class AccountController : BasePartnerController
    {
        #region Show Captcha

        private const string LoginAttemptsCountKey = "Partners_LoginAttemptsCount";

        private int LoginCount
        {
            get { return Convert.ToInt32(Session[LoginAttemptsCountKey]); }
            set { Session[LoginAttemptsCountKey] = LoginCount + 1; }
        }

        protected bool NeedShowCaptcha()
        {
            return LoginCount >= 2;
        }

        protected void LoginFail()
        {
            LoginCount++;
        }

        protected void LoginSuccess()
        {
            Session.Remove(LoginAttemptsCountKey);
        }

        public JsonResult JsonLoginFailResult(string error, object data = null)
        {
            LoginFail();
            return Json(new CommandResult
            {
                Result = false,
                Error = error,
                Obj = data
            });
        }

        #endregion

        #region Login / Logout

        public ActionResult Login()
        {
            if (PartnerContext.CurrentPartner != null)
                return RedirectToAction("Index", "Home");

            SetMetaInformation("Партнерская программа - Войти");

            var model = new LoginViewModel
            {
                ShowCaptcha = NeedShowCaptcha()
            };

            var from = Request["from"];
            if (from != null && from.StartsWith("/"))
                model.From = from;

            return View(model);
        }

        [HttpPost]
        [CaptchaValidation("CaptchaCode", "CaptchaSource")]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, string from)
        {
            if (PartnerContext.CurrentPartner != null)
                return RedirectToAction("Index", "Home");

            if (!string.IsNullOrEmpty(email) &&
                !string.IsNullOrEmpty(password) &&
                (!NeedShowCaptcha() || ModelState.IsValidField("CaptchaCode")))
            {
                if (PartnerAuthService.SignIn(email, password, false))
                {
                    LoginSuccess();
                    MvcCaptcha.ResetCaptcha("CaptchaSource");

                    if (!string.IsNullOrWhiteSpace(from) && from.StartsWith("/"))
                        return Redirect(UrlService.GenerateBaseUrl() + "/" + from.TrimStart('/'));

                    return RedirectToAction("Index", "Home");
                }
            }

            LoginFail();
            ShowMessage(NotifyType.Error, "Неверный логин или пароль");

            var model = new LoginViewModel
            {
                From = from,
                ShowCaptcha = NeedShowCaptcha(),
                Email = email
            };

            return View(model);
        }

        public ActionResult Logout()
        {
            PartnerAuthService.SignOut();

            return RedirectToAction("Login");
        }

        #endregion

        #region ForgotPassword

        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            SetMetaInformation("Партнерская программа - Восстановление пароля");
            SetNgController(NgControllers.NgControllersTypes.PartnerForgotPasswordCtrl);

            var viewModel = new ForgotPasswordViewModel
            {
                ShowCaptcha = NeedShowCaptcha()
            };

            Partner partner;
            if (model.Email.IsNotEmpty() && model.Hash.IsNotEmpty() && 
                (partner = PartnerService.GetPartner(model.Email)) != null)
            {
                var hash = ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(partner.Password.IsNotEmpty() ? partner.Password : partner.Email));
                if (hash.ToLower() == model.Hash.ToLower())
                    viewModel.ShowRecovery = true;
            }

            return View(viewModel);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ForgotPasswordJson(ForgotPasswordModel model)
        {
            if (NeedShowCaptcha() && !MvcCaptcha.Validate("CaptchaSource", model.CaptchaCode, model.CaptchaSource))
                return JsonLoginFailResult("Введенный код не совпадает", new { ShowCaptcha = NeedShowCaptcha() });

            var partner = PartnerService.GetPartner(model.Email);
            if (partner != null)
            {
                var hash = ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(partner.Password.IsNotEmpty() ? partner.Password : partner.Email));
                var link = string.Format("{0}?email={1}&hash={2}", UrlService.GetUrl("partners/account/forgotpassword"), partner.Email, hash);

                var mail = new PwdRepairMailTemplate(hash.ToLower(), partner.Email, link);
                MailService.SendMailNow(partner.Email, mail);

                LoginSuccess();
                return JsonOk();
            }

            return JsonLoginFailResult("Партнер с указанным e-mail не зарегистрирован", new { ShowCaptcha = NeedShowCaptcha() });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangePasswordJson(ForgotPasswordModel model)
        {
            if (!(model.NewPassword.IsNotEmpty() && model.NewPasswordConfirm.IsNotEmpty()))
                return JsonError("Заполните обязательные поля");
            if (model.NewPassword != model.NewPasswordConfirm)
                return JsonError("Указанные пароли не совпадают");
            if (model.NewPassword.Length < 6)
                return JsonError("Длина пароля должна быть не менее 6 символов");

            var partner = PartnerService.GetPartner(model.Email);
            if (partner != null)
            {
                var hash = ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(partner.Password.IsNotEmpty() ? partner.Password : partner.Email));
                if (hash.ToLower() == model.Hash.ToLower())
                {
                    PartnerService.ChangePassword(partner.Id, model.NewPassword, false);
                    PartnerAuthService.SignIn(model.Email, model.NewPassword, false);

                    return JsonOk();
                }
            }

            return JsonError("При восстановлении пароля возникла ошибка");
        }

        #endregion

        #region Registration

        public ActionResult Registration()
        {
            if (PartnerContext.CurrentPartner != null)
                return RedirectToAction("Index", "Home");

            SetMetaInformation("Партнерская программа - Регистрация");

            var model = new RegistrationViewModel
            {
                PartnerType = EPartnerType.LegalEntity
            };

            return View(model);
        }

        [HttpPost]
        [CaptchaValidation("CaptchaCode", "CaptchaSource")]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(RegistrationViewModel model)
        {
            if (PartnerContext.CurrentPartner != null)
                return RedirectToAction("Index", "Home");

            if (SettingsPartners.EnableCaptchaInRegistrationPartners && !ModelState.IsValidField("CaptchaCode"))
            {
                ShowMessage(NotifyType.Error, "Введенный код не совпадает.");
                return View(model);
            }

            MvcCaptcha.ResetCaptcha("CaptchaSource");

            try
            {
                new RegistrationHandler(model).Execute();
                return RedirectToAction("FinishRegistration");
            }
            catch (BlException ex)
            {
                ShowMessage(NotifyType.Error, ex.Message);
                return View(model);
            }
        }

        public ActionResult FinishRegistration()
        {
            var partner = PartnerContext.CurrentPartner;
            if (partner == null)
                return RedirectToAction("Login");
            
            if ((partner.Type == EPartnerType.LegalEntity && partner.LegalEntity != null) || 
                (partner.Type == EPartnerType.NaturalPerson && partner.NaturalPerson != null))
                return RedirectToAction("Index", "Home");

            SetMetaInformation("Завершение регистрации - Личный кабинет партнера");
            
            var canSkip = partner.Type == EPartnerType.NaturalPerson &&
                          SettingsPartners.SkipRegPasportStepForIndividualEntity;

            var model = new FinishRegistrationViewModel
            {
                PartnerType = partner.Type,
                CanSkip = canSkip
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishRegistration(FinishRegistrationViewModel model)
        {
            var partner = PartnerContext.CurrentPartner;
            if (partner == null)
                return RedirectToAction("Login");
            
            if ((partner.Type == EPartnerType.LegalEntity && partner.LegalEntity != null) || 
                (partner.Type == EPartnerType.NaturalPerson && partner.NaturalPerson != null))
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                new FinishRegistrationHandler(model).Execute();
                return RedirectToAction("Index", "Home");
            }

            ShowErrorMessages();

            return View(model);
        }

        #endregion

        public ActionResult Blocked()
        {
            return View();
        }
    }
}