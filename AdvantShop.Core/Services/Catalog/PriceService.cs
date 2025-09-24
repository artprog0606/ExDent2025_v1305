using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Catalog
{
    public class PriceService
    {
        public static float GetFinalPrice(float price)
        {
            return RoundPrice(price, null, CurrencyService.CurrentCurrency.Rate);
        }

        public static float GetFinalPrice(float price, Discount discount)
        {
            return GetFinalPrice(price, discount, CurrencyService.CurrentCurrency.Rate, null);
        }

        public static float GetFinalPrice(float price, Discount discount, float baseCurrencyValue, Currency renderCurrency = null)
        {
            var discountPrice = discount.Type == DiscountType.Percent ? price * discount.Percent / 100 : discount.Amount;

            var resultPrice = RoundPrice(price, renderCurrency, baseCurrencyValue) - RoundPrice(discountPrice, renderCurrency, baseCurrencyValue);
            
            return SimpleRoundPrice(resultPrice, renderCurrency);
        }


        public static float GetFinalPrice(Offer offer, Customer customer, string selectedOptions = null)
        {
            var customOptionsPrice =
                selectedOptions != null
                    ? CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, selectedOptions, offer.Product.Currency.Rate)
                    : 0;
            return GetFinalPrice(offer, customer.CustomerGroup, customOptionsPrice);
        }

        public static float GetFinalPrice(Offer offer, CustomerGroup customerGroup, float customOptionsPrice = 0)
        {
            var price = offer.RoundedPrice + customOptionsPrice;

            var discount = GetFinalDiscount(price, offer.Product.Discount.Percent, offer.Product.Discount.Amount, offer.Product.Currency.Rate, 
                                            customerGroup, offer.ProductId, 
                                            doNotApplyOtherDiscounts: offer.Product.DoNotApplyOtherDiscounts,
                                            productMainCategoryId: offer.Product.CategoryId);

            return GetFinalPrice(price, discount);
        }


        public static float GetFinalDiscountPercentage(float productDiscountPercent, CustomerGroup group,
            float discountByTime, int productId, List<ProductDiscount> productDiscounts, int? productMainCategoryId)
        {
            var finalDiscount = productDiscountPercent > discountByTime ? productDiscountPercent : discountByTime;

            if (group != null)
            {
                var customerGroupDiscountByCategory =
                    productMainCategoryId != null && productMainCategoryId != 0 && productMainCategoryId != -1
                        ? CustomerGroupService.GetCustomerGroupDiscountByCategory(group.CustomerGroupId, productMainCategoryId.Value)
                        : null;
                var customerGroupDiscount = customerGroupDiscountByCategory ?? group.GroupDiscount;

                finalDiscount = finalDiscount > customerGroupDiscount ? finalDiscount : customerGroupDiscount;
            }

            if (productId != 0 && productDiscounts != null && productDiscounts.Count > 0)
            {
                var prodDiscount = productDiscounts.Find(x => x.ProductId == productId);
                if (prodDiscount != null)
                {
                    finalDiscount = finalDiscount > prodDiscount.Discount ? finalDiscount : prodDiscount.Discount;
                }
            }

            return finalDiscount;
        }

        public static Discount GetFinalDiscount(float price, Discount discount, float currencyValue, 
                                                CustomerGroup customerGroup = null, int productId = 0, float discountByTime = -1,
                                                List<ProductDiscount> productDiscounts = null, bool doNotApplyOtherDiscounts = false, 
                                                int? productMainCategoryId = null)
        {
            return GetFinalDiscount(price, discount.Percent, discount.Amount, currencyValue, customerGroup, productId, 
                                    discountByTime, productDiscounts, doNotApplyOtherDiscounts, 
                                    null, productMainCategoryId);
        }

        public static Discount GetFinalDiscount(float price, float discountPercent, float discountAmount, float currencyValue, 
                                                CustomerGroup customerGroup = null, int productId = 0, float discountByTime = -1, 
                                                List<ProductDiscount> productDiscounts = null, bool doNotApplyOtherDiscounts = false,
                                                Currency renderingCurrency = null, int? productMainCategoryId = null)
        {
            if (discountAmount != 0)
                discountAmount = RoundPrice(discountAmount, renderingCurrency, currencyValue);

            if (doNotApplyOtherDiscounts && !ProductService.IgnoreProductDoNotApplyOtherDiscounts(productId))
                return new Discount(discountPercent, discountAmount);

            if (customerGroup == null)
                customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

            if (discountByTime == -1)
                discountByTime = DiscountByTimeService.GetCurrentDiscount(productId);

            if (productDiscounts == null)
                productDiscounts = productId != 0 ? ProductService.GetDiscountList() : null;

            var percent = GetFinalDiscountPercentage(discountPercent, customerGroup, discountByTime, productId, productDiscounts, productMainCategoryId);

            var type = price * percent / 100 > discountAmount ? DiscountType.Percent : DiscountType.Amount;

            return new Discount(percent, discountAmount, type);
        }

        public static float RoundPrice(float value, Currency renderingCurrency = null, float baseCurrencyValue = 1)
        {
            var currency = renderingCurrency ?? CurrencyService.CurrentCurrency;

            if (!currency.EnablePriceRounding)
                return (float)Math.Round(value / (double)currency.Rate * baseCurrencyValue, 4, MidpointRounding.AwayFromZero);

            return (float)(Math.Round(value / (double)currency.RoundNumbers / (double)currency.Rate * baseCurrencyValue, 0, MidpointRounding.AwayFromZero) * Math.Round(currency.RoundNumbers, 2));
        }
        
        public static decimal RoundPrice(decimal value, Currency renderingCurrency = null, decimal baseCurrencyValue = 1)
        {
            var currency = renderingCurrency ?? CurrencyService.CurrentCurrency;

            if (!currency.EnablePriceRounding)
                return Math.Round(value / (decimal)currency.Rate * baseCurrencyValue, 4, MidpointRounding.AwayFromZero);

            return (Math.Round(value / (decimal)currency.RoundNumbers / (decimal)currency.Rate * baseCurrencyValue, 0, MidpointRounding.AwayFromZero) * Math.Round((decimal)currency.RoundNumbers, 2));
        }

        public static float SimpleRoundPrice(float value, Currency renderingCurrency = null)
        {
            var currency = renderingCurrency ?? CurrencyService.CurrentCurrency;

            if (!currency.EnablePriceRounding)
                return (float)Math.Round(value, 4, MidpointRounding.AwayFromZero);

            return (float)(Math.Round(value / (double)currency.RoundNumbers, 0, MidpointRounding.AwayFromZero) * Math.Round(currency.RoundNumbers, 2));
        }
        
        public static decimal SimpleRoundPrice(decimal value, Currency renderingCurrency = null)
        {
            var currency = renderingCurrency ?? CurrencyService.CurrentCurrency;

            if (!currency.EnablePriceRounding)
                return Math.Round(value, 4, MidpointRounding.AwayFromZero);

            var roundNumbers = (decimal) currency.RoundNumbers;

            return Math.Round(value / roundNumbers, 0, MidpointRounding.AwayFromZero) * Math.Round(roundNumbers, 2);
        }
    }
}
