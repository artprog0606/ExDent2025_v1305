using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Repository;
using System;
using AdvantShop.Core.Common;
using AdvantShop.Payment;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetOrderItemsSummary
    {
        private readonly int _orderId;
        private readonly UrlHelper _urlHelper;

        public GetOrderItemsSummary(int orderId)
        {
            _orderId = orderId;

            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public OrderItemsSummaryModel Execute()
        {
            var order = _orderId != 0 ? OrderService.GetOrder(_orderId) : null;

            var orderCurrency =
                order != null && order.OrderCurrency != null
                    ? (Currency)order.OrderCurrency
                    : CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);

            if (order == null)
                return new OrderItemsSummaryModel() { OrderCurrency = orderCurrency, IsDraft = true };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            bool shippingUseWeight = false,
                shippingUseDemensions = false;
            float[] totalDimensionsFromShipping = null;
            float totalWeightFromShipping = 0;
            if (shippingMethod != null)
            {
                var shippingType = ReflectionExt.GetTypeByAttributeValue<Core.Common.Attributes.ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, shippingMethod.ShippingType);
                if (shippingType != null)
                {
                    var derivedTypeWeight = typeof(BaseShippingWithWeight);
                    var derivedTypeCargo = typeof(BaseShippingWithCargo);
                    shippingUseWeight = derivedTypeWeight.IsAssignableFrom(shippingType);
                    shippingUseDemensions = derivedTypeCargo.IsAssignableFrom(shippingType);

                    if (shippingUseWeight || shippingUseDemensions)
                    {
                        var calculationParameters = ShippingCalculationConfigurator.Configure()
                           .ByOrder(order)
                           .FromAdminArea()
                           .Build();

                        var shipping = (BaseShipping)Activator.CreateInstance(shippingType, shippingMethod, calculationParameters);
                        if (shippingUseWeight)
                            totalWeightFromShipping = ((BaseShippingWithWeight)shipping).GetTotalWeight();
                        if (shippingUseDemensions)
                            totalDimensionsFromShipping = ((BaseShippingWithCargo)shipping).GetDimensions();
                    }
                }
            }

            var payment = order.PaymentMethod;

            var totalDimensions = MeasureHelper.GetDimensions(order);
            var totalWeight = MeasureHelper.GetTotalWeight(order, order.OrderItems);

            var model = new OrderItemsSummaryModel
            {
                IsDraft = order.IsDraft,
                OrderCurrency = orderCurrency,

                ProductsCost = order.OrderCertificates != null && order.OrderCertificates.Count > 0
                    ? order.OrderCertificates.Sum(x => x.Sum)
                    : order.OrderItems.Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, orderCurrency)),

                OrderDiscount = order.OrderDiscount,
                OrderDiscountValue = order.OrderDiscountValue,

                Certificate = order.Certificate,
                Coupon = order.Coupon != null ? order.Coupon.ToString() : null,
                BonusCost = order.BonusCost,

                ShippingName = order.ArchivedShippingName,
                ShippingMethodId = order.ShippingMethodId,
                ShippingCost = order.ShippingCost,
                ShippingType = shippingMethod != null ? shippingMethod.ShippingType.ToLower() : "",
                OrderPickPoint = order.OrderPickPoint,

                DeliveryDate = order.DeliveryDate != null ? order.DeliveryDate.Value.ToShortDateTime() : null,
                DeliveryTime = order.DeliveryTime,
                DeliveryInterval = order.DeliveryInterval,

                PaymentName = order.PaymentMethodName,
                PaymentCost = order.PaymentCost,
                PaymentDetails = order.PaymentDetails,
                PaymentKey = payment != null ? payment.PaymentKey.ToLower() : null,
                PaymentMethodId = order.PaymentMethodId,

                CustomerComment = order.CustomerComment,

                Taxes =
                    order.Taxes.Select(
                        tax =>
                            new KeyValuePair<string, string>(
                                (tax.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") : "") + " " + tax.Name,
                                tax.Sum.HasValue ? tax.Sum.Value.FormatPrice(orderCurrency) : tax.Name)).ToList(),

                Sum = order.Sum,
                Paid = order.Payed,
                ShippingUseWeight = shippingUseWeight,
                TotalWeight = Convert.ToSingle(Math.Round(totalWeight, 3)),
                TotalWeightFromShipping = shippingUseWeight ? Convert.ToSingle(Math.Round(totalWeightFromShipping, 3)) : 0f,

                ShippingUseDemensions = shippingUseDemensions,
                TotalDemensions = totalDimensions[0] + " x " + totalDimensions[1] + " x " + totalDimensions[2] + " мм",
                TotalDemensionsFromShipping = shippingUseDemensions ? totalDimensionsFromShipping[0] + " x " + totalDimensionsFromShipping[1] + " x " + totalDimensionsFromShipping[2] + " мм" : null,
                TotalLength = totalDimensions[0],
                TotalWidth = totalDimensions[1],
                TotalHeight = totalDimensions[2],
                TotalLengthFromShipping = shippingUseDemensions ? totalDimensionsFromShipping[0] : 0f,
                TotalWidthFromShipping = shippingUseDemensions ? totalDimensionsFromShipping[1] : 0f,
                TotalHeightFromShipping = shippingUseDemensions ? totalDimensionsFromShipping[2] : 0f,
                IsNotEditedDimensions = order.TotalWidth == null && order.TotalHeight == null && order.TotalLength == null,
                IsNotEditedWeight = order.TotalWeight == null,
                ProductsDiscountPrice = order.GetOrderDiscountPrice(),
                CouponPrice = order.GetOrderCouponPrice(),
                DontCallBack = order.DontCallBack,
                FilesHelpText = FileHelpers.GetFilesHelpText(AttachmentService.FileTypes[AttachmentType.CustomerOrderFile], "15MB"),
                ReceivingMethod = order.ReceivingMethod,
                CountDevices = order.CountDevices,
                ShowCountDevices = SettingsCheckout.ShowCountDevices || order.CountDevices.HasValue
            };

            model.CustomerOrderAttachments = AttachmentService.GetAttachments<CustomerOrderAttachment>(_orderId).Select(x => new Models.Attachments.AttachmentModel
            {
                Id = x.Id,
                FilePathAdmin = x.PathAdmin,
                FileName = x.FileName,
                OriginFileName = x.OriginFileName,
                FilePath = x.Path,
                FileSize = x.FileSizeFormatted,
                ObjId = x.ObjId,
            }).ToList();

            if (model.Certificate != null)
                model.CertificatePrice = order.Certificate.Price;

            model.ShowSendBillingLink = order.ShowBillingLink();

            if (payment != null && model.PaymentKey != null)
            {
                model.ShowPrintPaymentDetails = (new List<string>() { "sberbank", "bill", "billby", "billkz", "billua", "check", "qiwi" }).Contains(model.PaymentKey);
                if (model.ShowPrintPaymentDetails)
                {
                    model.PrintPaymentDetailsText = payment.ButtonText;
                    model.PrintPaymentDetailsLink =
                        _urlHelper.AbsoluteActionUrl(payment.PaymentKey, "PaymentReceipt",
                            new {area = "", ordernumber = order.Number, ch = OrderService.GetPaymentReceiptHash(order)});
                }
                if (model.PaymentKey == "bill")
                {
                    var getCustomerDataMethod = payment.Parameters[BillTemplate.GetCustomerDataMethod];
                    if (getCustomerDataMethod.TryParseEnum<EGetCustomerDataMethod>() == EGetCustomerDataMethod.InPayment && payment.Parameters[BillTemplate.ShowPaymentDetails].TryParseBool())
                    {
                        model.ShowCompanyNamePaymentDetails = true;
                        model.ShowInnPaymentDetails = true;
                    }
                }
            }

            model.BonusCard = order.GetOrderBonusCard();
            model.BonusCardPurchase = PurchaseService.GetByOrderId(order.OrderID);
            model.BonusesAvailable = model.BonusCard != null
                ? (float)model.BonusCard.BonusesTotalAmount + 
                  (model.BonusCardPurchase != null
                      ? (float)model.BonusCardPurchase.BonusAmount
                      : 0)
                : 0;
            model.CanChangeBonusAmount = BonusSystemService.CanChangeBonusAmount(order, model.BonusCard, model.BonusCardPurchase);

            return model;
        }
    }
}
