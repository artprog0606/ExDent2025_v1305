using AdvantShop.Repository.Currencies;

namespace AdvantShop.Areas.Api.Models.Shared
{
    public class CurrencyApi
    {
        public string Name { get; set; }

        public string Symbol { get; set; }

        public float Rate { get; set; }

        public string Iso3 { get; set; }

        public bool IsCodeBefore { get; set; }

        public float RoundNumbers { get; set; }

        public bool EnablePriceRounding { get; set; }
        
        public CurrencyApi(Currency currency)
        {
            Name = currency.Name;
            Symbol = currency.Symbol;
            Rate = currency.Rate;
            Iso3 = currency.Iso3;
            IsCodeBefore = currency.IsCodeBefore;
            RoundNumbers = currency.RoundNumbers;
            EnablePriceRounding = currency.EnablePriceRounding;
        }
    }
}