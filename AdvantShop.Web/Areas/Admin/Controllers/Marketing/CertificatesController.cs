using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Taxes;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Marketing.Certificates;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Marketing.Certificates;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Marketing
{
    [Auth(RoleAction.CouponsAndDiscounts)]
    public partial class CertificatesController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Certificates.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.CertificatesCtrl);

            return View();
        }

        public JsonResult GetCertificates(CertificatesFilterModel model)
        {
            return Json(new GetCertificates(model).Execute());
        }

        #region Commands

        private void Command(CertificatesFilterModel command, Func<int, CertificatesFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetCertificates(command);
                var ids = handler.GetItemsIds();

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCertificates(CertificatesFilterModel model)
        {
            Command(model, (id, c) =>
            {
                GiftCertificateService.DeleteCertificateById(id);
                return true;
            });

            return JsonOk();
        }

        #endregion

        #region Get/Add/Edit

        public JsonResult GetCertificatesItem(int? id)
        {
            var allowedPaymentsIds = GiftCertificateService.GetCertificatePaymentMethodsID();

            var payment = PaymentService.GetAllPaymentMethods(true).Where(x => !(x is IPaymentGiftCertificate) && allowedPaymentsIds.Contains(x.PaymentMethodId)).ToList();

            var listsPayment = new List<SelectListItem>();

            if (id == null)
            {
                listsPayment = payment.Select(x =>
                        new SelectListItem()
                        {
                            Text = x.Name,
                            Value = x.PaymentMethodId.ToString(),
                            Selected = x.PaymentMethodId == payment.FirstOrDefault().PaymentMethodId
                        })
                    .ToList();

                return Json(new { listsPayment });
            }

            var result = GiftCertificateService.GetCertificateById((int)id);
            
            listsPayment = payment.Select(x =>
                    new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.PaymentMethodId.ToString(),
                        Selected = result.CertificateOrder != null && result.CertificateOrder.PaymentMethodId != 0
                            ? x.PaymentMethodId == result.CertificateOrder.PaymentMethodId
                            : false
                    })
                .ToList();

            if (result == null)
                return JsonError();

            var model = new AdminCertificatesModel(result);
            
            return Json(new { model, listsPayment, FromEmail = result.CertificateOrder.OrderCustomer.Email });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCertificates(CertificatesFilterModel model, int paymentid)
        {
            
            try
            {
                ProcessJsonResult(new AddCertificateHandler(model, paymentid));

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Discounts_CertificateCreated);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return JsonError();
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditCertificates(CertificatesFilterModel model)
        {
            try
            {
                var giftCertificate = GiftCertificateService.GetCertificateById(model.CertificateId);

                if (giftCertificate == null)
                    return JsonError();

                if (!giftCertificate.OrderId.HasValue && model.Paid.HasValue && model.Paid.Value)
                    return JsonError("Нельзя сменить статус оплаты, нет привязанного заказа");


                giftCertificate.CertificateId = model.CertificateId;
                giftCertificate.FromName = model.FromName ?? string.Empty;
                giftCertificate.ToName = model.ToName ?? string.Empty;
                giftCertificate.Sum = model.Sum != null ? model.Sum.TryParseFloat() : 0.0f;
                giftCertificate.ToEmail = model.ToEmail ?? string.Empty;
                giftCertificate.Enable = model.Enable != null ? (bool)model.Enable : false;
                giftCertificate.Used = model.Used != null ? (bool)model.Used : false;
                giftCertificate.CertificateMessage = model.CertificateMessage ?? string.Empty;

                GiftCertificateService.UpdateCertificateById(giftCertificate);

                if (model.Paid != null && (bool)model.Paid != giftCertificate.Paid && giftCertificate.OrderId.HasValue)
                {
                    OrderService.PayOrder(giftCertificate.OrderId.Value, (bool)model.Paid);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return JsonError();
            }

            return JsonOk();
        }

        #endregion


        #region Settings

        public JsonResult GetSettings()
        {
            var paymentMethodIds = GiftCertificateService.GetCertificatePaymentMethodsID();
            var tax = TaxService.GetCertificateTax();

            return Json(new CertificatesSettings
            {
                PaymentMethods = PaymentService.GetAllPaymentMethods(true).Where(x => !(x is IPaymentGiftCertificate))
                    .Select(x => new SelectListItem()
                    {
                        Value = x.PaymentMethodId.ToString(),
                        Text = x.Name,
                        Selected = paymentMethodIds.Any(id => id == x.PaymentMethodId)
                    }).ToList(),

                Taxes = TaxService.GetTaxes()
                    .Select(x => new SelectItemModel(x.Name, x.TaxId)).ToList(),
                Tax = tax != null ? tax.TaxId : (int?)null,

                PaymentMethodType = (int)SettingsCertificates.PaymentMethodType,
                PaymentMethodTypes = Enum.GetValues(typeof(ePaymentMethodType)).Cast<ePaymentMethodType>().Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList(),

                PaymentSubjectType = (int)SettingsCertificates.PaymentSubjectType,
                PaymentSubjectTypes = Enum.GetValues(typeof(ePaymentSubjectType)).Cast<ePaymentSubjectType>().Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList(),

                ShowCertificatePaymentMetodOnlyCoversSum = SettingsCertificates.ShowCertificatePaymentMetodOnlyCoversSum
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveSettings(CertificatesSettings model)
        {
            var paymentsIds =
                model != null && model.PaymentMethods != null
                    ? model.PaymentMethods.Where(x => x.Selected).Select(x => x.Value.TryParseInt()).ToList()
                    : new List<int>();

            GiftCertificateService.SaveCertificatePaymentMethods(paymentsIds);

            if (model != null)
            {
                if (model.Tax.HasValue)
                    TaxService.SaveCertificateTax(model.Tax.Value);

                SettingsCertificates.PaymentMethodType = (ePaymentMethodType)model.PaymentMethodType;
                SettingsCertificates.PaymentSubjectType = (ePaymentSubjectType)model.PaymentSubjectType;

                SettingsCertificates.ShowCertificatePaymentMetodOnlyCoversSum = model.ShowCertificatePaymentMetodOnlyCoversSum;
            }

            return JsonOk();
        }

        #endregion

    }
}
