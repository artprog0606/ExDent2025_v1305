using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Taxes;
using AdvantShop.ViewModel.Checkout;

namespace AdvantShop.Handlers.Checkout
{
    public class PrintOrderHandler
    {
        private readonly Order _order;
        private readonly PrintOrderModel _printOrder;

        public PrintOrderHandler(Order order, PrintOrderModel printOrder)
        {
            _order = order;
            _printOrder = printOrder;
        }

        public PrintOrderViewModel Execute()
        {
            var currency = _order.OrderCurrency;

            var model = new PrintOrderViewModel()
            {
                Order = _order,
                OrderItems = _order.OrderItems,
                OrderCurrency = currency,
                ShowStatusInfo = SettingsCheckout.PrintOrder_ShowStatusInfo,
                ShowMap = SettingsCheckout.PrintOrder_ShowMap && _printOrder.ShowMap,
                MapType = SettingsCheckout.PrintOrder_MapType,
                MapAdress =
                    StringHelper.AggregateStrings(", ", _order.OrderCustomer.Country, _order.OrderCustomer.Region,
                        _order.OrderCustomer.District, _order.OrderCustomer.City, _order.OrderCustomer.Street + " " + _order.OrderCustomer.House),
                ShowContacts = _order.OrderCertificates == null || _order.OrderCertificates.Count == 0
            };

            var productPrice = _order.OrderCertificates != null && _order.OrderCertificates.Count > 0
                                    ? _order.OrderCertificates.Sum(item => item.Sum)
                                    : _order.OrderItems.Sum(item => PriceService.SimpleRoundPrice(item.Amount * item.Price, currency));
            var productsIgnoreDiscountPrice = _order.OrderItems.Where(item => item.IgnoreOrderDiscount).Sum(item => item.Price * item.Amount);

            model.ProductsPrice = productPrice.FormatPrice(currency);

            if ((_order.OrderDiscount != 0 || _order.OrderDiscountValue != 0) && productPrice - productsIgnoreDiscountPrice > 0)
            {
                model.OrderDiscount = PriceFormatService.FormatDiscountPercent(productPrice - productsIgnoreDiscountPrice, _order.OrderDiscount,
                    _order.OrderDiscountValue, currency.CurrencySymbol, currency.IsCodeBefore, false);
            }

            if (_order.BonusCost != 0)
            {
                model.OrderBonus = _order.BonusCost.FormatPrice(currency);
            }

            if (_order.Certificate != null)
            {
                model.Certificate = _order.Certificate.Price.FormatPrice(currency);
            }

            if (_order.Coupon != null)
            {
                switch (_order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        model.Coupon = String.Format("-{0} ({1})",
                                        _order.Coupon.Value.FormatPrice(currency),
                                        _order.Coupon.Code);
                        break;

                    case CouponType.Percent:
                        var productsWithCoupon = _order.OrderItems.Where(item => item.IsCouponApplied).Sum(item => item.Price * item.Amount);

                        model.Coupon = String.Format("-{0} ({1}%) ({2})",
                                        PriceService.SimpleRoundPrice(productsWithCoupon * _order.Coupon.Value / 100, currency).FormatPrice(currency),
                                        _order.Coupon.Value.FormatPriceInvariant(),
                                        _order.Coupon.Code);
                        break;
                }
            }

            model.ShippingPrice = _order.ShippingCost.FormatPrice(currency);
            model.ShippingMethodName = _order.ArchivedShippingName +
                                       (_order.OrderPickPoint != null && !string.IsNullOrEmpty(_order.OrderPickPoint.PickPointAddress)
                                           ? " (" + _order.OrderPickPoint.PickPointAddress + ")"
                                           : "");

            if (_order.DeliveryDate != null || !string.IsNullOrEmpty(_order.DeliveryTime))
            {
                model.ShippingDeliveryTime =
                    (_order.DeliveryDate != null ? Culture.ConvertShortDate(_order.DeliveryDate.Value) : "") + " " +
                    _order.DeliveryInterval.ReadableString;
            }

            model.PaymentPriceTitle =
                _order.PaymentCost == 0
                    ? LocalizationService.GetResource("Checkout.Payment.Title")
                    : (_order.PaymentCost > 0
                        ? LocalizationService.GetResource("Checkout.PaymentCost")
                        : LocalizationService.GetResource("Checkout.PaymentDiscount"));
            model.PaymentPrice = _order.PaymentCost.FormatPrice(currency);
            model.PaymentMethodName = _order.ArchivedPaymentName;

            model.Taxes = _order.Taxes;
            model.TotalPrice = _order.Sum.FormatPrice(currency);

            if (!string.IsNullOrEmpty(_printOrder.Sorting) && !string.IsNullOrEmpty(_printOrder.SortingType))
            {
                var sorting = _printOrder.Sorting.ToLower();
                var sortingType = _printOrder.SortingType.ToLower();

                switch (sorting)
                {
                    case "name":
                        if (sortingType == "asc")
                            model.OrderItems = model.OrderItems.OrderBy(x => x.Name).ToList();
                        else if (sortingType == "desc")
                            model.OrderItems = model.OrderItems.OrderByDescending(x => x.Name).ToList();
                        break;

                    case "pricestring":
                        if (sortingType == "asc")
                            model.OrderItems = model.OrderItems.OrderBy(x => x.Price).ToList();
                        else if (sortingType == "desc")
                            model.OrderItems = model.OrderItems.OrderByDescending(x => x.Price).ToList();
                        break;

                    case "cost":
                        if (sortingType == "asc")
                            model.OrderItems = model.OrderItems.OrderBy(x => x.Amount*x.Price).ToList();
                        else if (sortingType == "desc")
                            model.OrderItems = model.OrderItems.OrderByDescending(x => x.Amount*x.Price).ToList();
                        break;
                    default:
                        break;
                }
            }

            model.TotalDimensions = MeasureHelper.GetDimensions(_order);
            model.TotalWeight = MeasureHelper.GetTotalWeight(_order, _order.OrderItems);

            AddChange(model);

            return model;
        }

        private void AddChange(PrintOrderViewModel model)
        {
            if (!string.IsNullOrEmpty(_order.PaymentDetails?.Change))
            {
                model.ChangeTitle = LocalizationService.GetResource("PrintOrder.ChangeTitle");
                model.Change = $"{_order.PaymentDetails.Change} {_order.OrderCurrency.CurrencySymbol}";
            }
        }
    }
}