//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Catalog
{
    public enum CouponType
    {
        [Localize("Core.Coupon.CouponType.Fixed")]
        Fixed = 1,

        [Localize("Core.Coupon.CouponType.Percent")]
        Percent = 2,

        [Localize("Core.Coupon.CouponType.FixedOnGiftProduct")]
        FixedOnGiftOffer = 3
    }

    public enum CouponMode
    {
        None = 0,
        Template = 1,
        Generated = 2,
        TriggerTemplate = 3,
        PartnersTemplate = 4,
        Partner = 5
    }

    [Serializable]
    public class Coupon
    {
        public int CouponID { get; set; }
        public string Code { get; set; }
        public CouponType Type { get; set; }
        public float Value { get; set; }
        public string CurrencyIso3 { get; set; }
        public DateTime AddingDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int PossibleUses { get; set; }
        public int ActualUses { get; set; }
        public bool Enabled { get; set; }
        public float MinimalOrderPrice { get; set; }
        public bool IsMinimalOrderPriceFromAllCart { get; set; }
        public bool ForFirstOrder { get; set; }
        public string Comment { get; set; }

        public int? EntityId { get; set; }

        private List<int> _categoryIds;
        public List<int> CategoryIds => _categoryIds ?? (_categoryIds = CouponService.GetCategoriesIDsByCoupon(CouponID));

        private List<int> _productsIds;
        public List<int> ProductsIds => _productsIds ?? (_productsIds = CouponService.GetProductsIDsByCoupon(CouponID));

        private Currency _currency;
        public Currency Currency => _currency ?? (_currency = CurrencyService.Currency(CurrencyIso3));

        private List<int> _customerGroupsIds;
        public List<int> CustomerGroupIds => _customerGroupsIds ?? (_customerGroupsIds = CouponService.GetCustomerGroupIdsByCoupon(CouponID));

        public int? TriggerActionId { get; set; }

        public int? TriggerId { get; set; }

        public CouponMode Mode { get; set; }

        public int? Days { get; set; }

        public Guid? CustomerId { get; set; }

        private int? _giftOfferId;
        public int? GiftOfferId => _giftOfferId ?? (_giftOfferId = CouponService.GetOfferIdByCoupon(CouponID));
        
        public bool OnlyInMobileApp { get; set; }
        
        public bool ForFirstOrderInMobileApp { get; set; }
        public string ModuleId { get; set; }
        public bool OnlyOnCustomerBirthday { get; set; }
        public int? DaysBeforeBirthday { get; set; }
        public int? DaysAfterBirthday { get; set; }

        /// <summary>
        /// Применять купон к цене со скидкой?
        /// false - если скидка товара > купона, то берется скидка
        /// true - купон применяется к цене со скидкой
        /// </summary>
        public bool IsAppliedToPriceWithDiscount { get; set; }

        public float GetRate()
        {
            return Type == CouponType.Percent
                ? Value
                : PriceService.SimpleRoundPrice(Value * Currency.Rate / CurrencyService.CurrentCurrency.Rate);
        }

        public override int GetHashCode()
        {
            return CouponID.GetHashCode() ^
                   Code.GetHashCode() ^
                   PossibleUses.GetHashCode() ^
                   ActualUses.GetHashCode() ^
                   MinimalOrderPrice.GetHashCode() ^
                   IsMinimalOrderPriceFromAllCart.GetHashCode() ^
                   StartDate.GetHashCode() ^
                   ExpirationDate.GetHashCode() ^
                   Type.GetHashCode() ^
                   Value.GetHashCode() ^
                   Mode.GetHashCode() ^
                   (TriggerId ?? 0).GetHashCode() ^
                   (TriggerActionId ?? 0).GetHashCode() ^
                   (Days ?? 0).GetHashCode() ^
                   (CustomerId ?? Guid.Empty).GetHashCode() ^
                   ForFirstOrder.GetHashCode() ^
                   OnlyInMobileApp.GetHashCode() ^
                   ForFirstOrderInMobileApp.GetHashCode() ^
                   ModuleId.GetHashCode() ^
                   OnlyOnCustomerBirthday.GetHashCode() ^
                   (DaysBeforeBirthday ?? 0).GetHashCode() ^
                   (DaysAfterBirthday ?? 0).GetHashCode() ^
                   IsAppliedToPriceWithDiscount.GetHashCode();
        }
    }


    public static class CouponModeExtensions
    {
        public static bool IsAppliedToProduct(this Coupon coupon, int productId, float price, Discount productsDiscount, 
                                                bool doNotApplyOtherDiscounts)
        {
            if (doNotApplyOtherDiscounts)
                return false;
            
            if (!CouponService.IsCouponCanBeAppliedToProduct(coupon.CouponID, productId))
                return false;

            if (coupon.IsAppliedToPriceWithDiscount)
                return true;

            if (coupon.Type == CouponType.Percent)
            {
                if ((productsDiscount.Type == DiscountType.Percent && coupon.Value > productsDiscount.Percent) ||
                    (productsDiscount.Type == DiscountType.Amount && price * coupon.Value / 100 > productsDiscount.Amount))
                {
                    return true;
                }
            }
            else if (coupon.Type == CouponType.Fixed)
            {
                if ((productsDiscount.Type == DiscountType.Percent && coupon.Value > price * productsDiscount.Percent / 100) ||
                    (productsDiscount.Type == DiscountType.Amount && coupon.Value > productsDiscount.Amount))
                {
                    return true;
                }
            }
            else if (coupon.Type == CouponType.FixedOnGiftOffer)
            {
                return true;
            }
            return false;
        }
        
        public static bool IsAppliedToCard(this Coupon coupon, ShoppingCart cart)
        {
            bool isApplied;
            var price = coupon.IsMinimalOrderPriceFromAllCart
                ? cart.Sum(x => x.PriceWithDiscount * x.Amount)
                : cart.Where(x => x.IsCouponApplied).Sum(x => x.PriceWithDiscount*x.Amount);

            isApplied = price >= coupon.MinimalOrderPrice;
            if (isApplied && coupon.Type == CouponType.FixedOnGiftOffer)
            {
                foreach (var item in cart)
                {
                    var product = ProductService.GetProductByOfferId(item.OfferId);
                    if (product.DoNotApplyOtherDiscounts)
                        continue;
                    if (CouponService.IsCouponCanBeAppliedToProduct(coupon.CouponID, product.ProductId))
                        return true;
                }
                return false;
            }
            return isApplied;
        }

        public static bool IsAppliedToOrder(this Coupon coupon, Order order)
        {
            bool isApplied;
            var price = coupon.IsMinimalOrderPriceFromAllCart
                ? order.OrderItems.Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, order.OrderCurrency))
                : order.OrderItems.Where(x => x.IsCouponApplied)
                                  .Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, order.OrderCurrency));

            isApplied = price >= coupon.MinimalOrderPrice;
            if (isApplied && coupon.Type == CouponType.FixedOnGiftOffer)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem.DoNotApplyOtherDiscounts)
                        continue;
                    if (orderItem.ProductID.HasValue && CouponService.IsCouponCanBeAppliedToProduct(coupon.CouponID, orderItem.ProductID.Value))
                        return true;
                }
                return false;
            }
            return isApplied;
        }

        public static bool IsAppliedToOrder(this OrderCoupon coupon, Order order)
        {
            var price = coupon.IsMinimalOrderPriceFromAllCart
                ? order.OrderItems.Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, order.OrderCurrency))
                : order.OrderItems.Where(x => x.IsCouponApplied)
                                  .Sum(x => PriceService.SimpleRoundPrice(x.Price * x.Amount, order.OrderCurrency));

            return price >= coupon.MinimalOrderPrice;
        }
    }
}