using System;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Catalog
{
    public class PriceFormatService
    {
        public static string FormatPrice(float price, bool isWrap = false, bool isMainPrice = false, string unit = null)
        {
            var currency = CurrencyService.CurrentCurrency;
            return FormatPrice(price, currency.Rate, currency.Symbol, currency.Iso3, currency.IsCodeBefore, null, isWrap, isMainPrice, unit:unit);
        }

        public static string FormatPrice(float price, Currency currency, bool isWrap = false, bool isMainPrice = false, string unit = null)
        {
            return FormatPrice(price, currency.Rate, currency.Symbol, currency.Iso3, currency.IsCodeBefore, null, isWrap, isMainPrice, unit: unit);
        }

        public static string FormatPrice(float price, float currencyRate, string currencyCode, string currencyIso3, bool isCodeBefore, string zeroPriceMsg = null, bool isWrap = false, bool isMainPrice = false, string unit = null)
        {
            if (price == 0 && !String.IsNullOrEmpty(zeroPriceMsg))
                return zeroPriceMsg;

            if (unit == null)
            {
               unit = string.Empty;
            } else
            {
                unit = $"/{unit}";
            }

            var priceResult = String.Format(price % 1 == 0 ? "{0:### ### ##0.##}" : "{0:### ### ##0.00##}", currencyRate == 1 ? price : (float)Math.Round(price, 4)).Trim();

            var strCurrencyFormat = isWrap
                ? (isCodeBefore
                    ? $"<div class=\"price-currency\">{{1}}</div><div class=\"price-number\">{{0}}{unit}</div>"
                    : $"<div class=\"price-number\">{{0}}</div><div class=\"price-currency\">{{1}}{unit}</div>")
                : isMainPrice
                    ? (isCodeBefore 
                        ? $"<div class=\"price-currency\" itemprop=\"priceCurrency\" content=\"{{2}}\">{{1}}</div><div class=\"price-number\" itemprop=\"price\" content=\"{{3}}\">{{0}}{unit}</div>"
                        : $"<div class=\"price-number\" itemprop=\"price\" content=\"{{3}}\">{{0}}</div><div class=\"price-currency\" itemprop=\"priceCurrency\" content=\"{{2}}\">{{1}}{unit}</div>")
                    : (isCodeBefore ? "{1}{0}" : "{0}{1}");

            return String.Format(strCurrencyFormat, priceResult, currencyCode, currencyIso3, price.ToString("F4"));
        }
        
        public static string FormatPrice(float oldPrice, float finalPrice, Discount discount, bool showDiscount = false, bool isWrap = true, bool multiPrices = false, string unit = null)
        {
            if (oldPrice == 0)
                return
                    $"<div class=\"price-unknown\">{LocalizationService.GetResource("Core.Catalog.PriceFormat.ContactWithUs")}</div>";

            if (!showDiscount || !discount.HasValue)
                return String.Format("<div class=\"price-current cs-t-1\">{0}{1}</div>",
                    multiPrices ? LocalizationService.GetResource("Core.Catalog.PriceFormat.From") : "",
                    FormatPrice(finalPrice, isWrap, true, unit: unit));
             
            return
                String.Format(
                    "<div class=\"price-old cs-t-3\">{0}</div>" +
                    "<div class=\"price-new cs-t-1\">{1}{2}</div>" +
                    "<div class=\"price-discount\">{3} <div class=\"price-new-discount\">{4}</div> {5}</div>",
                    FormatPrice(oldPrice, isWrap, unit: unit),
                    multiPrices ? LocalizationService.GetResource("Core.Catalog.PriceFormat.From") : "",
                    FormatPrice(finalPrice, isWrap, true, unit: unit),
                    LocalizationService.GetResource("Core.Catalog.PriceFormat.DiscountBenefit"),
                    FormatPrice(PriceService.SimpleRoundPrice(oldPrice - finalPrice), unit: unit),
                    discount.Type == DiscountType.Percent
                        ? String.Format("{0} <div class=\"price-discount-percent-wrap\"><div class=\"price-discount-percent\">{1}%</div></div>",
                                        LocalizationService.GetResource("Core.Catalog.PriceFormat.DiscountOr"),
                                        FormatPriceInvariant(discount.Percent))
                        : ""
                );
        }
        
        public static string FormatPriceWithoutHtml(float price, float priceWithDiscount, Discount discount, bool showDiscount = false, bool multiPrices = false)
        {
            if (price == 0)
                return LocalizationService.GetResource("Core.Catalog.PriceFormat.ContactWithUs");

            if (!showDiscount || !discount.HasValue)
                return String.Format("{0}{1}",
                    multiPrices ? LocalizationService.GetResource("Core.Catalog.PriceFormat.From") : "",
                    FormatPrice(price, false, false));

            return String.Format("{0}{1}",
                multiPrices ? LocalizationService.GetResource("Core.Catalog.PriceFormat.From") : "",
                FormatPrice(priceWithDiscount, false, false));
        }

        public static string FormatPriceInvariant(float price)
        {
            // "### ### ##0.00##"
            return price.ToString(price % 1 == 0 ? "### ### ##0.##" : "### ### ##0.00").Trim();
            //return String.Format(price % 1 == 0 ? "{0:### ### ##0.##}" : "{0:### ### ##0.00}", price);
        }

        public static string FormatPricePlain(float price, Currency currency)
        {
            // "{0:### ### ##0.00##}"
            var priceResult = string.Format(price % 1 == 0 ? "{0:### ### ##0.##}" : "{0:### ### ##0.00}", currency.Rate == 1 ? price : Math.Round(price, 4)).Trim();
            return string.Format(currency.IsCodeBefore ? "{1}{0}" : "{0}{1}", priceResult, currency.Symbol);
        }

        public static string FormatDiscountPercent(float price, float discount, float discountValue, bool boolAddMinus)
        {
            var currency = CurrencyService.CurrentCurrency;
            return FormatDiscountPercent(price, discount, discountValue, currency.Symbol, currency.IsCodeBefore, boolAddMinus);
        }

        public static string FormatDiscountPercent(float price, float discountPercent, float discountValue, string currencyCode, bool isCodeBefore, bool boolAddMinus)
        {
            var strFormat = String.Empty;
            var priceDiscount = (price / 100 * discountPercent + discountValue).SimpleRoundPrice();

            // "{0:### ### ##0.00##}"
            var strFormatedPrice = String.Format(priceDiscount % 1 == 0 ? "{0:### ### ##0.##}" : "{0:### ### ##0.00}", priceDiscount).Trim();

            if (boolAddMinus)
                strFormat = "-";

            strFormat += discountValue == 0
                            ? (isCodeBefore ? "{1}{0} ({2}%)" : "{0}{1} ({2}%)")
                            : (isCodeBefore ? "{1}{0}" : "{0}{1}");

            return String.Format(strFormat, strFormatedPrice, currencyCode, discountPercent);
        }

        public static string RenderBonusPrice(float bonusPercent, float price, Discount discount, bool withoutHtml = false)
        {
            if (price <= 0 || bonusPercent <= 0)
                return string.Empty;

            var bonuses = (price * bonusPercent / 100).SimpleRoundPrice();
            
            var format = !withoutHtml
                ? "<div class=\"bonus-price\">" + LocalizationService.GetResource("Product.ProductInfo.BonusesOnCard") + "</div>"
                : LocalizationService.GetResource("Product.ProductInfo.BonusesOnCard");
            
            return String.Format(format, bonuses.FormatBonuses());
        }
    }
}
