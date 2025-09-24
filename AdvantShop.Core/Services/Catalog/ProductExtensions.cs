using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Catalog
{
    public static class ProductExtensions
    {
        public static float GetWeight(this Offer offer)
        {
            return offer.Weight ?? 0;
        }
        
        public static (float, string) GetWeightAndUnits(this Offer offer)
        {
            var weight = offer?.GetWeight() ?? 0;
            string units;
            
            if (weight > 0 && weight < 1)
            {
                weight *= 1000;
                units = LocalizationService.GetResource("Product.Weight.Grams");
            }
            else
            {
                units = LocalizationService.GetResource("Product.Weight.Kg");
            }

            return (weight, units);
        }

        public static float GetLength(this Offer offer)
        {
            return offer.Length ?? 0;
        }


        public static float GetWidth(this Offer offer)
        {
            return offer.Width ?? 0;
        }

        public static float GetHeight(this Offer offer)
        {
            return offer.Height ?? 0;
        }

        public static string GetDimensions(this Offer offer, string separator = "x")
        {
            return offer.GetLength() + separator + offer.GetWidth() + separator + offer.GetHeight();
        }

        /// <summary>
        /// Минимальное кол-во товара для покупки
        /// </summary>
        public static float GetMinAmount(this Product product)
        {
            return GetMinAmount(product.Multiplicity, product.MinAmount);
        }
        
        public static float GetMinAmount(this ProductItem product)
        {
            return GetMinAmount(product.Multiplicity, product.MinAmount);
        }
        
        public static float GetMinAmount(float multiplicity, float? productMinAmount)
        {
            multiplicity = multiplicity > 0 ? multiplicity : 1;

            var minAmount = 0f;
            
            if (productMinAmount == null || multiplicity > productMinAmount)
            {
                minAmount = multiplicity;
            }
            else
            {
                var dMinAmount = (decimal)productMinAmount.Value;
                var dMultiplicity = (decimal)multiplicity;

                minAmount = (float)(Math.Ceiling(dMinAmount / dMultiplicity) * dMultiplicity);
            }

            return minAmount > 0 ? minAmount : 1;
        }
        
        public static float GetMaxAvailableAmount(this Offer offer)
        {
            return SettingsCheckout.AmountLimitation && !offer.Product.AllowBuyOutOfStockProducts()
                ? Math.Min(offer.Product.MaxAmount ?? int.MaxValue, offer.Amount)
                : offer.Product.MaxAmount ?? int.MaxValue;
        }

        public static bool AllowBuyOutOfStockProducts(this Product product, bool? allowPreOrderOverride = null)
        {
            return SettingsCheckout.OutOfStockAction == eOutOfStockAction.Cart || 
                   SettingsCheckout.OutOfStockAction == eOutOfStockAction.Preorder && (allowPreOrderOverride ?? product.AllowPreOrder);
        }

        /// <summary>
        /// Доступна ли модификация для покупки?
        /// </summary>
        /// <param name="offer">Модификация</param>
        /// <param name="amount">Кол-во с учетом кратности товара</param>
        /// <param name="amountMinToBuy">Минимальное кол-во для покупки</param>
        /// <param name="finalPrice">Финальная цена</param>
        /// <param name="finalDiscount">Финальная скидка</param>
        /// <param name="allowBuyOutOfStockProducts">Разрешить покупать товары не в наличии</param>
        /// <returns>true | false</returns>
        public static bool IsAvailableForPurchase(this Offer offer, 
                                                    float amount, float amountMinToBuy, 
                                                    float finalPrice, Discount finalDiscount,
                                                    bool allowBuyOutOfStockProducts)
        {
            return offer != null && 
                   IsAvailableForPurchase(amount, amountMinToBuy, finalPrice, finalDiscount, allowBuyOutOfStockProducts);
        }
        
        /// <summary>
        /// Доступна ли модификация для покупки?
        /// </summary>
        /// <param name="amount">Кол-во с учетом кратности товара</param>
        /// <param name="amountMinToBuy">Минимальное кол-во для покупки</param>
        /// <param name="finalPrice">Финальная цена</param>
        /// <param name="finalDiscount">Финальная скидка</param>
        /// <param name="allowBuyOutOfStockProducts">Разрешить покупать товары не в наличии</param>
        /// <returns>true | false</returns>
        public static bool IsAvailableForPurchase(float amount, float amountMinToBuy, 
                                                  float finalPrice, Discount finalDiscount, bool allowBuyOutOfStockProducts)
        {
            return (amount > 0 || allowBuyOutOfStockProducts) &&
                   (finalPrice > 0 ||
                    (finalPrice == 0 && finalDiscount != null && finalDiscount.HasValue) || // под модуль Бесплатно+Доставка
                    allowBuyOutOfStockProducts) &&
                   (amount >= amountMinToBuy || allowBuyOutOfStockProducts || !SettingsCheckout.AmountLimitation);
        }

        /// <summary>
        /// Доступна ли модификация для покупки в кредит?
        /// </summary>
        public static bool IsAvailableForPurchaseOnCredit(this Offer offer, 
                                                            bool isAvailableForPurchase, float finalPrice, 
                                                            float firstPaymentMinPrice, float? firstPaymentMaxPrice)
        {
            return isAvailableForPurchase &&
                   firstPaymentMinPrice <= finalPrice &&
                   (firstPaymentMaxPrice.HasValue == false || firstPaymentMaxPrice >= finalPrice);
        }

        /// <summary>
        /// Доступна ли модификация для покупки в один клик?
        /// </summary>
        public static bool IsAvailableForPurchaseOnBuyOneClick(this Offer offer, 
                                                                bool isAvailableForPurchase, float finalPrice, 
                                                                float minimumOrderPrice)
        {
            return isAvailableForPurchase && 
                   minimumOrderPrice <= finalPrice;
        }
        
        /// <summary>
        /// Доступна ли модификация под заказ
        /// </summary>
        public static bool IsAvailableForPreOrder(this Offer offer, float amountToPreOrder)
        {
            return offer.Product != null 
                   && offer.Product.AllowPreOrder 
                   && (offer.RoundedPrice == 0 
                       || offer.Amount <= 0 
                       || offer.Amount < amountToPreOrder
                       || offer.GetAmountByMultiplicity(offer.Product.Multiplicity) <= 0);
        }

        /// <summary>
        /// Кол-во с учетом кратности товара
        /// </summary>
        public static float GetAmountByMultiplicity(this Offer offer, float multiplicity)
        {
            if (offer == null)
                return 0;
            
            return GetAmountByMultiplicity(offer.Amount, multiplicity);
        }

        /// <summary>
        /// Кол-во с учетом кратности товара
        /// </summary>
        public static float GetAmountByMultiplicity(float amount, float multiplicity)
        {
            if (amount < multiplicity)
                return 0;

            if (multiplicity == 1)
                return amount;

            var amountDecimal = Convert.ToDecimal(amount);
            var multiplicityDecimal = Convert.ToDecimal(multiplicity);
            if (multiplicityDecimal == 0)
                multiplicityDecimal = 1;

            var mod = amountDecimal % multiplicityDecimal;
            if (mod == 0)
                return amount;

            return (float) (amountDecimal - mod);
        }

        public static Offer SetPriceRule(this Offer offer, int customerGroupId, 
            int? paymentMethodId = null, int? shippingMethodId = null)
        {
            if (offer == null)
                return null;

            var amount = offer.Product.GetMinAmount();

            offer.SetOfferPriceRule(amount, customerGroupId, paymentMethodId, shippingMethodId);
            return offer;
        }

        public static Offer SetPriceRule(this Offer offer, float amount, int customerGroupId, 
            int? paymentMethodId = null, int? shippingMethodId = null)
        {
            if (offer == null)
                return null;

            offer.SetOfferPriceRule(amount, customerGroupId, paymentMethodId, shippingMethodId);
            return offer;
        }

        public static (float, float, Discount, string) GetOfferPricesWithPriceRule(this Offer offer, float amount, string customOptionsXml, Customer customer, Discount customDiscount)
        {
            var product = offer.Product;
            var offerRoundedPrice = offer.RoundedPrice;
            
            offer.SetPriceRule(amount, customer.CustomerGroupId);

            var customOptionsPrice =
                !string.IsNullOrEmpty(customOptionsXml)
                    ? CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, HttpUtility.UrlDecode(customOptionsXml), product.Currency.Rate)
                    : 0;

            var price = (offer.RoundedPrice + customOptionsPrice).RoundPrice(CurrencyService.CurrentCurrency.Rate);
            var offerPrice = (offerRoundedPrice + customOptionsPrice).RoundPrice(CurrencyService.CurrentCurrency.Rate);
            
            var finalDiscount = 
                offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                    ? PriceService.GetFinalDiscount(price, customDiscount ?? product.Discount, 
                                                    product.Currency.Rate, customer.CustomerGroup, product.ProductId,
                                                    doNotApplyOtherDiscounts: product.DoNotApplyOtherDiscounts,
                                                    productMainCategoryId: product.CategoryId)
                    : new Discount(0, offerPrice - price);

            var oldPrice =
                offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                    ? price
                    : offerPrice;
            
            var finalPrice = 
                offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                    ? PriceService.GetFinalPrice(price, finalDiscount)
                    : price;
            
            var preparedPrice = 
                PriceFormatService.FormatPrice(oldPrice, finalPrice, finalDiscount, true, unit: SettingsCatalog.ShowUnitsInCatalog ? product.Unit?.Name : null);

            return (oldPrice, finalPrice, finalDiscount, preparedPrice);
        }
        
        public static (float, float, Discount, string) GetOfferPricesWithPriceRule(this Offer offer, OfferPriceRule rule, string customOptionsXml, Customer customer, Discount customDiscount)
        {
            var product = offer.Product;
            var offerRoundedPrice = offer.RoundedPrice;
            var priceByRule = (rule.PriceByRule ?? 0).RoundPrice(product.Currency.Rate);

            var customOptionsPrice =
                !string.IsNullOrEmpty(customOptionsXml)
                    ? CustomOptionsService.GetCustomOptionPrice(priceByRule, HttpUtility.UrlDecode(customOptionsXml), product.Currency.Rate)
                    : 0;

            var price = (priceByRule + customOptionsPrice).RoundPrice(CurrencyService.CurrentCurrency.Rate);
            var offerPrice = (offerRoundedPrice + customOptionsPrice).RoundPrice(CurrencyService.CurrentCurrency.Rate);
            
            var finalDiscount = 
                rule.ApplyDiscounts
                    ? PriceService.GetFinalDiscount(price, customDiscount ?? product.Discount, 
                        product.Currency.Rate, customer.CustomerGroup, product.ProductId,
                        doNotApplyOtherDiscounts: product.DoNotApplyOtherDiscounts,
                        productMainCategoryId: product.CategoryId)
                    : new Discount(0, offerPrice - price);

            var oldPrice =
                rule.ApplyDiscounts
                    ? price
                    : offerPrice;
            
            var finalPrice = 
                rule.ApplyDiscounts
                    ? PriceService.GetFinalPrice(price, finalDiscount)
                    : price;
            
            var preparedPrice = 
                PriceFormatService.FormatPrice(oldPrice, finalPrice, finalDiscount, true);

            return (oldPrice, finalPrice, finalDiscount, preparedPrice);
        }

        public static void SetAmountByStocksAndWarehouses(this List<Offer> offers, List<int> warehouseIds)
        {
            foreach (var offer in offers)
            {
                var stocks =
                    WarehouseStocksService.GetOfferStocks(offer.OfferId).Where(stock => warehouseIds.Contains(stock.WarehouseId));
                
                offer.SetAmountByStocks(stocks);
            }
        }
        
        public static void SetAmountByStocksAndWarehouses(this Offer offer, List<int> warehouseIds)
        {
            var stocks =
                WarehouseStocksService.GetOfferStocks(offer.OfferId).Where(stock => warehouseIds.Contains(stock.WarehouseId));
            
            offer.SetAmountByStocks(stocks);
        }
    }
}
