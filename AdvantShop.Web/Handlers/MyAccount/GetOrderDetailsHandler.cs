using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Handlers.Common;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Models.Attachments;
using AdvantShop.Models.MyAccount;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Edost;
using AdvantShop.Shipping.Sdek;
using AdvantShop.Taxes;

namespace AdvantShop.Handlers.MyAccount
{
    public class GetOrderDetailsHandler
    {
        private readonly Order _order;
        private readonly UrlHelper _urlHelper;

        public GetOrderDetailsHandler(Order order)
        {
            _order = order;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public OrderDetailsModel Get()
        {
            var shippingInfo = new CustomerContact
            {
                Name = _order.OrderCustomer.GetFullName(),
                Street = _order.OrderCustomer.GetCustomerAddress(),
                City = _order.OrderCustomer.City,
                District = _order.OrderCustomer.District,
                Country = _order.OrderCustomer.Country,
                Region = _order.OrderCustomer.Region.IsNullOrEmpty() ? "-" : _order.OrderCustomer.Region,
                Zip = _order.OrderCustomer.Zip.IsNullOrEmpty() ? "-" : _order.OrderCustomer.Zip
            };

            var deliveryDate = _order.DeliveryDate.HasValue
                                    ? $"{Culture.ConvertShortDate(_order.DeliveryDate.Value)} {_order.DeliveryInterval.ReadableString}"
                                    : null;

            var items = new List<OrderDetailsItemModel>();
            foreach (var orderItem in _order.OrderItems)
            {
                var product = orderItem.ProductID.HasValue
                    ? ProductService.GetProduct(orderItem.ProductID.Value)
                    : null;

                items.Add(new OrderDetailsItemModel
                {
                    Name = orderItem.Name,
                    Price = PriceFormatService.FormatPrice(orderItem.Price, _order.OrderCurrency),
                    Amount = orderItem.Amount,
                    ArtNo = orderItem.ArtNo,
                    Id = orderItem.ProductID,
                    TotalPrice = PriceFormatService.FormatPrice(PriceService.SimpleRoundPrice(orderItem.Amount * orderItem.Price, _order.OrderCurrency), _order.OrderCurrency),
                    Photo = orderItem != null && orderItem.Photo != null
                        ? orderItem.Photo.ImageSrcXSmall()
                        : "images/nophoto_xsmall.png",
                    Url = product != null ? _urlHelper.RouteUrl("Product", new { url = product.UrlPath }) : string.Empty,
                    ColorHeader = SettingsCatalog.ColorsHeader,
                    SizeHeader = SettingsCatalog.SizesHeader,
                    ColorName = orderItem.Color,
                    SizeName = orderItem.Size,
                    UnitName = orderItem.Unit,
                    CustomOptions = orderItem.SelectedOptions,
                    Width = orderItem.Width,
                    Length = orderItem.Length,
                    Height = orderItem.Height
                });
            }

            var payments = new List<OrderDetailsPaymentModel>();
            if (_order.OrderStatusId != OrderStatusService.CanceledOrderStatus && (!SettingsCheckout.ManagerConfirmed || _order.ManagerConfirmed))
            {
                var paymentOptions = new GetPaymentsOfOrderForCustomer(_order).Get();

                foreach (var paymentOption in paymentOptions)
                    payments.Add(new OrderDetailsPaymentModel
                    {
                        Id = paymentOption.Id,
                        Name = paymentOption.Name
                    });
            }
            else
            {
                payments.Add(new OrderDetailsPaymentModel
                {
                    Id = _order.PaymentMethodId,
                    Name = _order.PaymentMethodName
                });
            }

            var taxes = _order.Taxes;

            string coupon = "";
            if (_order.Coupon != null)
            {
                switch (_order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        coupon = string.Format("- {0} ({1})",
                                        _order.Coupon.Value.FormatPrice(_order.OrderCurrency),
                                        _order.Coupon.Code);
                        break;
                    case CouponType.Percent:
                        var productsWithCoupon = _order.OrderItems.Where(item => item.IsCouponApplied).Sum(item => item.Price * item.Amount);

                        coupon = string.Format("- {0} ({1}%) ({2})",
                                        PriceService.SimpleRoundPrice(productsWithCoupon * _order.Coupon.Value / 100, _order.OrderCurrency).FormatPrice(_order.OrderCurrency),
                                        _order.Coupon.Value.FormatPriceInvariant(),
                                        _order.Coupon.Code);
                        break;
                }
            }

            var orderDetails = new OrderDetailsModel
            {
                Email = CustomerContext.CurrentCustomer.EMail,
                OrderID = _order.OrderID,
                Number = _order.Number,
                Code = _order.Code,
                StatusName = _order.OrderStatus.Hidden ? _order.PreviousStatus : _order.OrderStatus.StatusName,
                OrderItems = items,
                OrderCertificates = _order.OrderCertificates != null && _order.OrderCertificates.Any()
                    ? _order.OrderCertificates.Select(x => new OrderDetailsCertificateModel { Code = x.CertificateCode, Price = x.Sum.FormatPrice(_order.OrderCurrency) }).ToList()
                    : null,
                BillingAddress = shippingInfo,
                ShippingName = _order.OrderCustomer.FirstName + " " + _order.OrderCustomer.LastName,
                ShippingInfo = shippingInfo,
                ArchivedShippingName = _order.ArchivedShippingName,
                ShippingAddress = _order.OrderPickPoint != null ? _order.OrderPickPoint.PickPointAddress : null,
                PaymentMethodId = _order.PaymentMethodId,
                PaymentMethodName = _order.PaymentMethodName,
                TotalDiscountPrice = _order.GetOrderDiscountPrice(),
                TotalDiscountPriceFormatted = _order.GetOrderDiscountPrice().FormatPrice(_order.OrderCurrency),
                ProductsPrice = _order.OrderItems.Sum(item => PriceService.SimpleRoundPrice(item.Amount * item.Price, _order.OrderCurrency)).FormatPrice(_order.OrderCurrency),
                TotalDiscount = _order.OrderDiscount,
                CertificatePrice = _order.Certificate != null
                                        ? _order.Certificate.Price.FormatPrice(_order.OrderCurrency)
                                        : string.Empty,
                TotalPrice = _order.Sum.FormatPrice(_order.OrderCurrency),
                ShippingPrice = _order.ShippingCost.FormatPrice(_order.OrderCurrency),
                PaymentPrice = _order.PaymentCost != 0 ?
                                  (_order.PaymentCost > 0 ? "+ " : string.Empty) + _order.PaymentCost.FormatPrice(_order.OrderCurrency)
                                : string.Empty,
                PaymentPriceText = _order.PaymentCost != 0
                                        ? LocalizationService.GetResource("Checkout.PaymentCost")
                                        : LocalizationService.GetResource("Checkout.PaymentDiscount"),
                CustomerComment = _order.CustomerComment,
                Payments = payments,
                PaymentDetails = _order.PaymentDetails,
                Canceled = _order.OrderStatus.IsCanceled,
                Payed = _order.Payed,
                Bonus = _order.BonusCost != 0 ? _order.BonusCost.FormatPrice(_order.OrderCurrency) : "",
                TaxesNames = taxes.Select(tax => tax.Name).AggregateString(','),
                TaxesPrice = taxes.Any(x => x.Sum.HasValue) ? taxes.Sum(x => x.Sum).Value.FormatPrice(_order.OrderCurrency) : "-",
                TrackNumber = _order.TrackNumber,
                Coupon = coupon,
                StatusCancelForbidden = _order.OrderStatus.CancelForbidden,
                ChangePaymentForbidden = _order.OrderStatus.ChangePaymentForbidden,
                OrderDate = _order.OrderDate.ToString("g"),
                BonusCardPurchase = PurchaseService.GetByOrderId(_order.OrderID),
                OrderRecipient = OrderService.GetOrderRecipient(_order.OrderID),
                OrderReview = _order.OrderReview,
                DeliveryDateTime = deliveryDate,
                AllowAddOrderReview = SettingsCheckout.AllowAddOrderReview && (_order.Payed || !SettingsCheckout.ReviewOnlyPaidOrder),
                AllowUploadFiles = SettingsCheckout.AllowUploadFiles,
                FilesHelpText = FileHelpers.GetFilesHelpText(AttachmentService.FileTypes[AttachmentType.CustomerOrderFile], "15MB"),
                AllowedFileExtensions = FileHelpers.GetAllowedFileExtensions(AttachmentService.FileTypes[AttachmentType.AdminOrderFile]).AggregateString(",")
            };

            orderDetails.CustomerOrderAttachments = AttachmentService.GetAttachments<CustomerOrderAttachment>(_order.OrderID).Select(x => new AttachmentModel
            {
                Id = x.Id,
                ObjId = x.ObjId,
                FileName = x.OriginFileName.IsNullOrEmpty() ? x.FileName : x.OriginFileName
            }).ToList();

            orderDetails.ShippingHistory = new GetShippingHistoryAndPointInfoHandler(_order).Get();
            
            return orderDetails;
        }
    }
}