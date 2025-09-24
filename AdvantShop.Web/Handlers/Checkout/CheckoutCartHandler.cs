using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Models.Cart;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;

namespace AdvantShop.Handlers.Checkout
{
    public class CheckoutCartHandler
    {
        #region Constructor

        private readonly UrlHelper _urlHelper;

        public CheckoutCartHandler()
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        #endregion

        public CheckoutCartModel Get(CheckoutData checkoutData, ShoppingCart cart, float shippingPrice, float paymentCost, Currency currency)
        {
            var productsPrice = cart.TotalPrice;
            var discountOnTotalPrice = cart.DiscountPercentOnTotalPrice;
            var totalDiscount = cart.TotalDiscount;
            
            var bonusPrice = 0f;
            var bonusPlus = 0f;
            var couponPrice = totalDiscount;

            if (BonusSystem.IsActive)
            {
                var bonusCost = BonusSystemService.GetBonusCost(cart, shippingPrice, checkoutData.Bonus.AppliedBonuses,
                                                            (checkoutData.User.WantRegist || checkoutData.User.WantBonusCard));

                bonusPrice = bonusCost.BonusPrice;
                if (checkoutData.Bonus.AppliedBonuses == 0 || !BonusSystem.ProhibitAccrualAndSubstractBonuses)
                    bonusPlus = bonusCost.BonusPlus + (checkoutData.User.WantRegist ? BonusSystem.BonusesForNewCard : 0);
                
                totalDiscount += bonusPrice;
            }

            TaxElement shippingTax = null;
            var shippingTaxType = checkoutData.SelectShipping != null 
                ? (checkoutData.SelectShipping.TaxId.HasValue && (shippingTax = TaxService.GetTax(checkoutData.SelectShipping.TaxId.Value)) != null ? shippingTax.TaxType : TaxType.None) 
                : TaxType.VatWithout;

            var taxesItems = TaxService.CalculateTaxes(cart, productsPrice - totalDiscount, shippingPrice, shippingTaxType);
            var taxesTotal = taxesItems.Where(tax => !tax.Key.ShowInPrice).Sum(item => item.Value);

            var totalTemp = (productsPrice + shippingPrice + taxesTotal - totalDiscount + paymentCost).RoundPrice(CurrencyService.CurrentCurrency.Rate);
            totalTemp = totalTemp > 0 ? totalTemp : 0;
            var totalPrice = totalTemp.FormatPrice();

            var model = new CheckoutCartModel
            {
                Items = cart.Select(item => new CheckoutCartItem()
                {
                    Name = item.Offer.Product.Name,
                    Amount = item.Amount,
                    Price = item.PriceWithDiscount.FormatPrice(),
                    Link = _urlHelper.RouteUrl("Product", new { url = item.Offer.Product.UrlPath }),
                    Cost = (item.PriceWithDiscount * item.Amount).FormatPrice(),
                    SelectedOptions = CustomOptionsService.DeserializeFromXml(item.AttributesXml, item.Offer.Product.Currency.Rate),
                    ColorName = item.Offer.Color?.ColorName,
                    SizeName = item.Offer.SizeForCategory?.GetFullName(),
                    PriceRuleName = item.Offer.PriceRule?.Name,
                    PhotoPath = item.Offer.Photo.ImageSrcXSmall(),
                    PhotoSmallPath = item.Offer.Photo.ImageSrcSmall(),
                    PhotoMiddlePath = item.Offer.Photo.ImageSrcMiddle(),
                    PhotoAlt = item.Offer.Photo.Alt,
                    MinAmount = item.Offer.Product.GetMinAmount(),
                    MaxAmount = item.Offer.GetMaxAvailableAmount(),
                    Avalible = ShoppingCartService.GetAvailableState(item, cart),
                    AvailableAmount = item.Offer.Amount,
                    Multiplicity = item.Offer.Product.Multiplicity > 0 ? item.Offer.Product.Multiplicity : 1,
                    FrozenAmount = item.FrozenAmount,
                    IsGift = item.IsGift,
                    ShoppingCartItemId = item.ShoppingCartItemId,
                    Unit = item.Offer.Product.Unit?.DisplayName,
                    BriefDescription = item.Offer.Product.BriefDescription
                }).ToList(),

                ColorHeader = SettingsCatalog.ColorsHeader,
                SizeHeader = SettingsCatalog.SizesHeader,

                Cost = productsPrice.FormatPrice(),
                Result = totalPrice,
                BuyOneClickEnabled = SettingsCheckout.BuyInOneClick && !SettingsCheckout.BuyInOneClickDisableInCheckout,
                ShowInCart = true,
                Delivery =
                    shippingPrice > 0
                        ? shippingPrice.FormatPrice()
                        : checkoutData.SelectShipping != null && !string.IsNullOrEmpty(checkoutData.SelectShipping.ZeroPriceMessage) ? checkoutData.SelectShipping.ZeroPriceMessage : null,
                
                Valid  = ShoppingCartService.IsValidCart(cart, cart.TotalItems, cart.TotalPrice)
            };

            if (paymentCost != 0)
                model.Payment = new CheckoutCartParam()
                {
                    Key =
                        paymentCost > 0
                            ? LocalizationService.GetResource("Checkout.PaymentCost")
                            : LocalizationService.GetResource("Checkout.PaymentDiscount"),
                    Value = paymentCost.FormatPrice()
                };

            if (discountOnTotalPrice > 0)
                model.Discount = new CheckoutCartParam()
                {
                    Key = discountOnTotalPrice.ToString(),
                    Value = ((cart.TotalPrice - cart.TotalPriceIgnoreDiscount) * discountOnTotalPrice / 100)
                                .RoundPrice(CurrencyService.CurrentCurrency.Rate)
                                .FormatPrice()
                };

            if (cart.Certificate != null)
                model.Certificate = cart.Certificate.Sum.FormatPrice();

            if (cart.Coupon != null)
            {
                if (cart.Coupon.Type != CouponType.FixedOnGiftOffer)
                    model.Coupon = couponPrice != 0
                        ? new CartCoupon()
                        {
                            Code = cart.Coupon.Code,
                            Price = couponPrice.FormatPrice(),
                            Percent =
                                cart.Coupon.Type == CouponType.Percent
                                    ? cart.Coupon.Value.FormatPriceInvariant()
                                    : null
                        }
                        : new CartCoupon()
                        {
                            Code = cart.Coupon.Code,
                            Price = 0f.FormatPrice(),
                            NotApplied = true,
                        };
                else
                    model.Coupon = new CartCoupon
                    {
                        Code = cart.Coupon.Code,
                        Price = $"{ProductService.GetProductByOfferId(cart.Coupon.GiftOfferId.Value).Name} ({cart.Coupon.Value.FormatPrice()})"
                    };

            }

            model.Taxes =
                taxesItems.Select(
                    tax =>
                        new CheckoutCartParam()
                        {
                            Key = $"{(tax.Key.ShowInPrice ? LocalizationService.GetResource("Core.Tax.IncludeTax") : "")} {tax.Key.Name}",
                            Value = tax.Value.FormatPrice()
                        }).ToList();

            if (bonusPrice != 0)
                model.Bonuses = bonusPrice.FormatBonuses();
                
            if (bonusPlus != 0)
                model.BonusPlus = bonusPlus.FormatBonuses();

            return model;
        }
    }
}