using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public class CurrencyTemplate
    {
        [TemplateDocxProperty("Symbol", LocalizeDescription = "Символ")]
        public string Symbol { get; set; }

        [TemplateDocxProperty("Rate", LocalizeDescription = "Курс")]
        public float Rate { get; set; }

        [TemplateDocxProperty("Iso3", LocalizeDescription = "Код ISO3")]
        public string Iso3 { get; set; }

        [TemplateDocxProperty("NumIso3", LocalizeDescription = "Числовой код ISO3")]
        public int NumIso3 { get; set; }

        public static explicit operator CurrencyTemplate(Currency currency)
        {
            var template = new CurrencyTemplate();

            template.Symbol = currency.Symbol;
            template.Rate = currency.Rate;
            template.Iso3 = currency.Iso3;
            template.NumIso3 = currency.NumIso3;

            return template;
        }
    }
}
