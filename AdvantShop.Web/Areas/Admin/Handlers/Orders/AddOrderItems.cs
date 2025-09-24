using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Taxes;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class AddOrderItems
    {
        private readonly Order _order;
        private readonly List<int> _offerIds;

        public AddOrderItems(Order order, List<int> offerIds)
        {
            _order = order;
            _offerIds = offerIds;
        }

        public bool Execute()
        {
            if (CurrencyService.CurrentCurrency.Iso3 != _order.OrderCurrency.CurrencyCode)
                CurrencyService.CurrentCurrency = _order.OrderCurrency;

            var saveChanges = false;
            var couponCode = _order.Coupon != null ? _order.Coupon.Code : null;

            foreach (var offerId in _offerIds)
            {
                var offer = OfferService.GetOffer(offerId);
                if (offer == null)
                    continue;

                var product = offer.Product;

                var prodMinAmount = product.GetMinAmount();

                var price = PriceService.GetFinalPrice(offer, _order.GetCustomerGroup());

                if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes)
                {
                    var customer =
                        _order.OrderCustomer != null && _order.OrderCustomer.CustomerID != Guid.Empty
                            ? CustomerService.GetCustomer(_order.OrderCustomer.CustomerID)
                            : null;
                    
                    var customerGroup = customer?.CustomerGroup ?? CustomerGroupService.GetDefaultCustomerGroup();
                    
                    offer.SetPriceRule(prodMinAmount, customerGroup.CustomerGroupId);

                    var discount =
                        offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                            ? PriceService.GetFinalDiscount(offer.RoundedPrice, product.Discount, product.Currency.Rate, customerGroup, product.ProductId,
                                                            doNotApplyOtherDiscounts: product.DoNotApplyOtherDiscounts,
                                                            productMainCategoryId: product.CategoryId)
                            : new Discount();

                    price = PriceService.GetFinalPrice(offer.RoundedPrice, discount);
                }

                var item = GetOrderItem(product, offer, price, prodMinAmount, couponCode, false);

                var oItem = _order.OrderItems.Find(x => x == item);
                if (oItem != null)
                {
                    oItem.Amount += 1;
                    saveChanges = true;
                }
                else
                {
                    _order.OrderItems.Add(item);
                    saveChanges = true;
                }

                ProductGiftService.AddGiftByOfferToOrder(_order, offer, GetOrderItem);
            }

            return saveChanges;
        }

        private OrderItem GetOrderItem(Product product, Offer offer, float price, float prodMinAmount, string couponCode, bool isGift)
        {
            var item = new OrderItem
            {
                ProductID = product.ProductId,
                Name = product.Name,
                ArtNo = offer.ArtNo,
                BarCode = offer.BarCode,
                Price = price,
                Amount = prodMinAmount,
                SupplyPrice = offer.SupplyPrice,
                SelectedOptions = new List<EvaluatedCustomOptions>(),
                Weight = offer.GetWeight(),
                IsCouponApplied = IsCouponApplied(couponCode, product.ProductId, offer),
                Color = offer.Color != null ? offer.Color.ColorName : null,
                Size = offer.SizeForCategory?.GetFullName(),
                PhotoID = offer.Photo != null ? offer.Photo.PhotoId : default(int),
                AccrueBonuses = offer.Product.AccrueBonuses,
                Width = offer.GetWidth(),
                Length = offer.GetLength(),
                Height = offer.GetHeight(),
                PaymentMethodType = offer.Product.PaymentMethodType,
                PaymentSubjectType = offer.Product.PaymentSubjectType,
                MeasureType = offer.Product.Unit?.MeasureType,
                Unit = offer.Product.Unit?.DisplayName,
                IsGift = isGift,
                IsMarkingRequired = product.IsMarkingRequired,
                
                BasePrice = isGift ? 0 : offer.RoundedPrice,
                DiscountPercent = product.Discount.Percent,
                DiscountAmount = product.Discount.Amount.RoundPrice(product.Currency.Rate, _order.OrderCurrency),
                DoNotApplyOtherDiscounts = product.DoNotApplyOtherDiscounts,
                DownloadLink = product.DownloadLink
            };

            item.IgnoreOrderDiscount |= item.DoNotApplyOtherDiscounts;

            var tax = product.TaxId != null ? TaxService.GetTax(product.TaxId.Value) : null;
            if (tax != null)
            {
                item.TaxId = tax.TaxId;
                item.TaxName = tax.Name;
                item.TaxType = tax.TaxType;
                item.TaxRate = tax.Rate;
                item.TaxShowInPrice = tax.ShowInPrice;
            }

            return item;
        }

        private bool IsCouponApplied(string couponCode, int productId, Offer offer)
        {
            if (couponCode.IsNullOrEmpty())
                return false;

            var coupon = CouponService.GetCouponByCode(couponCode);

            if (coupon == null)
                return false;

            var price = offer.RoundedPrice;
            var product = offer.Product;
            var discount = PriceService.GetFinalDiscount(price, product.Discount, product.Currency.Rate, _order.GetCustomerGroup(), offer.ProductId, productMainCategoryId: product.CategoryId);

            return coupon.IsAppliedToProduct(productId, price, discount, product.DoNotApplyOtherDiscounts);
        }
    }
}
