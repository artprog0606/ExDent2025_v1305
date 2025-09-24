using AdvantShop.Configuration;
using AdvantShop.Repository.Currencies;
using System;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Services.Catalog
{
    public static class PriceExtentions
    {
        public static float RoundPrice(this float price, float baseCurrencyValue = 1, Currency renderCurrency = null)
        {
            return PriceService.RoundPrice(price, renderCurrency, baseCurrencyValue);
        }

        public static float RoundPrice(this float price, Currency renderCurrency)
        {
            return PriceService.RoundPrice(price, renderCurrency, 1);
        }

        public static string RoundAndFormatPrice(this float price, Currency renderCurrency, float baseCurrencyValue = 1)
        {
            return PriceService.RoundPrice(price, renderCurrency, baseCurrencyValue).FormatPrice(renderCurrency);
        }

        public static string FormatPrice(this float price)
        {
            return PriceFormatService.FormatPrice(price);
        }

        public static string FormatPrice(this float price, bool isWrap, bool isMainPrice)
        {
            return PriceFormatService.FormatPrice(price, isWrap, isMainPrice);
        }

        public static string FormatPrice(this float price, Currency currency)
        {
            return PriceFormatService.FormatPrice(price, currency);
        }

        public static string FormatPriceInvariant(this float price)
        {
            return PriceFormatService.FormatPriceInvariant(price);
        }
        public static float BaseRound(this float incoming)
        {
            return incoming.BaseRound(0);
        }

        public static float SimpleRoundPrice(this float price, Currency renderCurrency = null)
        {
            return PriceService.SimpleRoundPrice(price, renderCurrency);
        }

        public static float BaseRound(this float incoming, int digits)
        {
            return (float)(Math.Round(incoming, digits));
        }

        public static float ConvertCurrency(this float price, Currency sourceCurrency, Currency newCurrency)
        {
            if (sourceCurrency.Rate == newCurrency.Rate)
                return price;

            return PriceService.RoundPrice(CurrencyService.ConvertCurrency(price, newCurrency.Rate, sourceCurrency.Rate), newCurrency, newCurrency.Rate);
            //return PriceService.RoundPrice(price, newCurrency, sourceCurrency.Rate);
        }

        #region Default Currency / Decimal
        
        public static decimal RoundPrice(this decimal price, decimal baseCurrencyValue = 1, Currency renderCurrency = null)
        {
            return PriceService.RoundPrice(price, renderCurrency, baseCurrencyValue);
        }
        
        public static decimal SimpleRoundPrice(this decimal price, Currency renderCurrency = null)
        {
            return PriceService.SimpleRoundPrice(price, renderCurrency);
        }

        /// <summary>
        /// from currency with rate <paramref name="baseCurrencyValue"/> to default currency
        /// </summary>
        public static decimal RoundConvertToDefault(this decimal price, float baseCurrencyValue = 1)
        {
            return (decimal)PriceService.RoundPrice((float)price, SettingsCatalog.DefaultCurrency, baseCurrencyValue);
        }

        /// <summary>
        /// from default currency to base
        /// </summary>
        public static decimal RoundConvertToBase(this decimal price)
        {
            return price.RoundConvertToBase(SettingsCatalog.DefaultCurrency);
        }

        /// <summary>
        /// from <paramref name="currency"/> to base currency
        /// </summary>
        public static decimal RoundConvertToBase(this decimal price, Currency currency)
        {
            return (decimal)PriceService.RoundPrice(CurrencyService.ConvertCurrency((float)price, 1, currency.Rate), currency, currency.Rate);
        }

        public static string FormatRoundPriceDefault(this decimal price, float baseCurrencyValue = 1)
        {
            return PriceService.RoundPrice((float)price, SettingsCatalog.DefaultCurrency, baseCurrencyValue).FormatPrice(SettingsCatalog.DefaultCurrency);
        }

        #endregion
        
        public static string FormatBonuses(this float bonuses)
        {
            var bonus0 = LocalizationService.GetResource("Bonuses.Bonuses0");
            var bonus1 = LocalizationService.GetResource("Bonuses.Bonuses1");
            var bonus2 = LocalizationService.GetResource("Bonuses.Bonuses2");
            var bonus5 = LocalizationService.GetResource("Bonuses.Bonuses5");
            
            return $"{bonuses.ToString(bonuses%1 == 0 ? "### ### ##0.##" : "### ### ##0.00").TrimStart()} {Strings.Numerals(bonuses, bonus0, bonus1, bonus2, bonus5)}";
        }

        public static Discount RoundPrice(this Discount discount, Currency renderingCurrency = null, float currencyValue = 1)
        {
            var discountAmount = discount.Amount;
            if (discountAmount != 0)
                discountAmount = PriceService.RoundPrice(discountAmount, renderingCurrency, currencyValue);

            return new Discount(discount.Percent, discountAmount);
        }
    }
}
