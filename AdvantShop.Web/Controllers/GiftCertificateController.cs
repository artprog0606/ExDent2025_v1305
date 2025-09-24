using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Customers;
using AdvantShop.Handlers.GiftCertificate;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.SEO;
using AdvantShop.ViewModel.GiftCertificate;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using BotDetect.Web.Mvc;

namespace AdvantShop.Controllers
{
    public class GiftCertificateController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index()
        {
            if (!SettingsCheckout.EnableGiftCertificateService)
                return RedirectToRoute("Home");

            var customer = CustomerContext.CurrentCustomer;
            var payments = PaymentService.GetCertificatePaymentMethods();

            var model = new GiftCertificateViewModel()
            {
                PaymentMethods = payments,
                PaymentMethod = payments != null && payments.Count > 0 ? payments[0].PaymentMethodId : 0,
                PaymentKey = payments != null && payments.Count > 0 ? payments[0].PaymentKey.ToLower() : "",
                Sum = SettingsCheckout.MinimalPriceCertificate,
                EmailFrom = customer.RegistredUser ? customer.EMail : "",
                Phone = customer.RegistredUser ? customer.Phone : "",
                MinimumOrderPrice = CustomerGroupService.GetMinimumOrderPrice().FormatPrice(),
                MinimalPriceCertificate = SettingsCheckout.MinimalPriceCertificate.FormatPrice(),
                MaximalPriceCertificate = SettingsCheckout.MaximalPriceCertificate.FormatPrice()
            };

            SetMetaInformation(T("GiftCertificate.Index.Header"));
            SetNgController(NgControllers.NgControllersTypes.GiftCertificateCtrl);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCode", "CaptchaSource")]
        public ActionResult Index(GiftCertificateViewModel model)
        {
            var isValid = true;

            if (model.NameFrom.IsNullOrEmpty())
            {
                isValid = false;
                ShowMessage(NotifyType.Error, T("GiftCertificate.Index.WrongNameFrom"));
            }

            if (model.NameTo.IsNullOrEmpty())
            {
                isValid = false;
                ShowMessage(NotifyType.Error, T("GiftCertificate.Index.WrongNameTo"));
            }


            if (!ValidationHelper.IsValidEmail(model.EmailFrom))
            {
                isValid = false;
                ShowMessage(NotifyType.Error, T("GiftCertificate.Index.WrongEmailFrom"));
            }

            if (!ValidationHelper.IsValidEmail(model.EmailTo))
            {
                isValid = false;
                ShowMessage(NotifyType.Error, T("GiftCertificate.Index.WrongEmailTo"));
            }

            if (model.Sum < SettingsCheckout.MinimalPriceCertificate ||
                model.Sum > SettingsCheckout.MaximalPriceCertificate ||
                model.Sum < CustomerGroupService.GetMinimumOrderPrice())
            {
                isValid = false;
                ShowMessage(NotifyType.Error, T("GiftCertificate.Index.WrongSum"));
            }

            var payments = PaymentService.GetCertificatePaymentMethods();

            if (payments.Find(x => x.PaymentMethodId == model.PaymentMethod) == null)
            {
                isValid = false;
                ShowMessage(NotifyType.Error, T("GiftCertificate.Index.WrongPaymentMethod"));
            }

            if (SettingsMain.EnableCaptchaInGiftCerticate)
            {
                if (!ModelState.IsValidField("CaptchaCode"))
                {
                    isValid = false;
                    ShowMessage(NotifyType.Error, T("Captcha.Wrong"));
                }
                MvcCaptcha.ResetCaptcha("CaptchaSource");
            }

            if (SettingsCheckout.IsShowUserAgreementText && !model.Agreement)
            {
                isValid = false;
                ShowMessage(NotifyType.Error, T("User.Registration.ErrorAgreement"));
            }

            if (!isValid)
            {
                model.PaymentMethods = payments;
                model.PaymentMethod = payments.Count > 0 ? payments[0].PaymentMethodId : 0;
                model.PaymentKey = payments.Count > 0 ? payments[0].PaymentKey.ToLower() : "";

                SetMetaInformation(
                    new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, T("GiftCertificate.Index.Header"))),
                    string.Empty);

                SetNgController(NgControllers.NgControllersTypes.GiftCertificateCtrl);

                return View("Index", model);
            }

            var order = new CreateCertificateOrderHandler(model).Execute();
            if (order != null && order.OrderID != 0)
            {
                TempData["orderid"] = order.OrderID.ToString();
                return RedirectToRoute("CheckoutSuccess", new {code = order.Code});
            }

            SetNgController(NgControllers.NgControllersTypes.GiftCertificateCtrl);

            return View("Index", model);
        }

        [ChildActionOnly]
        public ActionResult GiftCertificateModal(bool isModal, string code)
        {
            var model = new GiftCertificateModalViewModel()
            {
                IsModal = isModal,
                GiftCertificate =
                    !string.IsNullOrWhiteSpace(code)
                        ? GiftCertificateService.GetCertificateByCode(code)
                        : null
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult GiftCertificateBlock()
        {
            if (!SettingsDesign.GiftSertificateVisibility || !SettingsCheckout.EnableGiftCertificateService)
                return new EmptyResult();

            return PartialView();
        }

        public ActionResult Print(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || GiftCertificateService.GetCertificateByCode(code) == null)
                return Error404();
            
            return View(new PrintGiftCertificateViewModel(){Code = code});
        }
    }
}