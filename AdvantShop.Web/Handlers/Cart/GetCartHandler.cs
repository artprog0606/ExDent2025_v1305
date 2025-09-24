using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shop;
using AdvantShop.Customers;
using AdvantShop.Models.Cart;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Handlers.Cart
{
    public class GetCartHandler
    {
        private readonly UrlHelper _urlHelper;
        private readonly ShoppingCart _cart;

        public GetCartHandler() : this(ShoppingCartService.CurrentShoppingCart)
        {
        }
        
        public GetCartHandler(ShoppingCart cart)
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            _cart = cart;
        }

        public CartModel Get()
        {
            var cartProducts =
                (from item in _cart
                    let product = item.Offer.Product
                    select new CartItemModel()
                    {
                        OfferId = item.OfferId,
                        ProductId = product.ProductId,
                        Sku = item.Offer.ArtNo,
                        Name = product.Name,
                        Link = _urlHelper.AbsoluteRouteUrl("Product", new {url = product.UrlPath}),
                        Amount = item.Amount,
                        Price = item.Price.FormatPrice(),
                        PriceWithDiscount = item.PriceWithDiscount.FormatPrice(),
                        Discount = item.Discount,
                        DiscountText = item.Discount.GetText(),
                        Cost = PriceService.SimpleRoundPrice(item.PriceWithDiscount * item.Amount).FormatPrice(),
                        PhotoPath = item.Offer.Photo.ImageSrcXSmall(),
                        PhotoSmallPath = item.Offer.Photo.ImageSrcSmall(),
                        PhotoMiddlePath = item.Offer.Photo.ImageSrcMiddle(),
                        PhotoAlt = product.Name,
                        ShoppingCartItemId = item.ShoppingCartItemId,
                        SelectedOptions =
                            CustomOptionsService.DeserializeFromXml(item.AttributesXml, product.Currency.Rate),
                        AttributesXml = item.AttributesXml,
                        ColorName = item.Offer.Color?.ColorName,
                        SizeName = item.Offer.SizeForCategory?.GetFullName(),
                        Avalible = ShoppingCartService.GetAvailableState(item, _cart),
                        AvailableAmount = item.Offer.Amount,
                        MinAmount = product.GetMinAmount(),
                        MaxAmount = item.Offer.GetMaxAvailableAmount(),
                        Multiplicity = product.Multiplicity > 0 ? product.Multiplicity : 1,
                        FrozenAmount = item.FrozenAmount,
                        IsGift = item.IsGift,
                        Unit = product.Unit?.DisplayName,
                        PriceRuleName = item.Offer.PriceRule?.Name,
                        BriefDescription = item.Offer.Product.BriefDescription,
                        InWishlist = ShoppingCartService.CurrentWishlist.Any(x => x.OfferId == item.OfferId)
                    }).ToList();

            var totalPrice = _cart.TotalPrice;
            var totalDiscount = _cart.TotalDiscount;
            var priceWithDiscount = totalPrice - totalDiscount;
            var totalItems = _cart.TotalItems;
            var discountOnTotalPrice = _cart.DiscountPercentOnTotalPrice;

            var count = string.Format("{0} {1}",
                totalItems == 0 ? "" : totalItems.ToString(CultureInfo.InvariantCulture),
                Strings.Numerals(totalItems,
                    LocalizationService.GetResource("Cart.Product0"),
                    LocalizationService.GetResource("Cart.Product1"),
                    LocalizationService.GetResource("Cart.Product2"),
                    LocalizationService.GetResource("Cart.Product5")));

            float bonusPlus = 0;

            if (totalPrice > 0 && BonusSystem.IsActive)
            {
                bonusPlus = BonusSystemService.GetBonusCost(_cart).BonusPlus;
            }

            var showConfirmButtons = true;
            foreach (var module in AttachedModules.GetModules<IShoppingCart>())
            {
                var moduleObject = (IShoppingCart)Activator.CreateInstance(module);
                showConfirmButtons &= moduleObject.ShowConfirmButtons;
                if (module.FullName.Contains("OrderConfirmationInShoppingCart"))
                {
                    showConfirmButtons = false;
                }
            }

            string isValidCart = ShoppingCartService.IsValidCart(_cart, totalItems, totalPrice);
            var isDefaultCustomerGroup = CustomerContext.CurrentCustomer.CustomerGroup.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup;

            var model = new CartModel
            {
                CartProducts = cartProducts,
                ColorHeader = SettingsCatalog.ColorsHeader,
                SizeHeader = SettingsCatalog.SizesHeader,
                Count = count,
                TotalItems = totalItems,
                BonusPlus = bonusPlus > 0 ? bonusPlus.FormatBonuses() : null,
                Valid = isValidCart,

                CouponInputVisible = _cart.HasItems && _cart.Coupon == null && _cart.Certificate == null && SettingsCheckout.DisplayPromoTextbox &&
                    (isDefaultCustomerGroup || SettingsCheckout.EnableGiftCertificateService), // не выводить поле, если покупатель не в группе по умолчанию и сертификаты запрещены
                IsDefaultCustomerGroup = isDefaultCustomerGroup,

                ShowConfirmButtons = showConfirmButtons,
                ShowBuyInOneClick = showConfirmButtons && SettingsCheckout.BuyInOneClick,
                BuyInOneClickText = SettingsCheckout.BuyInOneClickLinkText,
                EnablePhoneMask = SettingsMain.EnablePhoneMask,

                TotalProductPrice = totalPrice.FormatPrice(),
                TotalPrice = priceWithDiscount > 0 ? priceWithDiscount.FormatPrice() : 0F.FormatPrice(),

                DiscountPrice =
                    discountOnTotalPrice > 0 && totalPrice - _cart.TotalPriceIgnoreDiscount > 0
                        ? PriceFormatService.FormatDiscountPercent(totalPrice - _cart.TotalPriceIgnoreDiscount, discountOnTotalPrice, 0, true)
                        : null,

                Certificate = _cart.Certificate?.Sum.FormatPrice(),
                CertificateCode = _cart.Certificate?.CertificateCode,
                MobileIsFullCheckout = SettingsMobile.IsFullCheckout && showConfirmButtons,
                IsShowUnits = SettingsCatalog.ShowUnitsInCatalog,
                IsWishListVisibility = SettingsDesign.WishListVisibility
            };

            if (_cart.Coupon != null)
            {
                switch (_cart.Coupon.Type)
                {
                    case CouponType.Fixed:
                    case CouponType.Percent:
                        model.Coupon = totalDiscount != 0
                        ? new CartCoupon()
                        {
                            Code = _cart.Coupon.Code,
                            Price = (_cart.Coupon.Type == CouponType.Percent ? totalDiscount : _cart.Coupon.GetRate()).FormatPrice(),
                            Percent =
                                _cart.Coupon.Type == CouponType.Percent
                                    ? _cart.Coupon.Value.FormatPriceInvariant()
                                    : null
                        }
                        : new CartCoupon()
                        {
                            Code = _cart.Coupon.Code,
                            Price = 0f.FormatPrice(),
                            NotApplied = true,
                        };
                        break;
                    case CouponType.FixedOnGiftOffer:
                        if (_cart.Coupon.GiftOfferId.HasValue && _cart.Coupon.GiftOfferId != 0)
                            model.Coupon = new CartCoupon
                            {
                                Code = _cart.Coupon.Code,
                                Price = $"{ProductService.GetProductByOfferId(_cart.Coupon.GiftOfferId.Value)?.Name} ({_cart.Coupon.GetRate().FormatPrice()})"
                            };
                        break;
                }
            }

            return model;
        }
    }
}