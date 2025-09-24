//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Booking
{
    public class BookingCurrency
    {
        public static implicit operator BookingCurrency(Currency cur)
        {
            return new BookingCurrency
            {
                CurrencyCode = cur.Iso3,
                CurrencyNumCode = cur.NumIso3,
                CurrencyValue = cur.Rate,
                CurrencySymbol = cur.Symbol,
                IsCodeBefore = cur.IsCodeBefore,
                EnablePriceRounding = cur.EnablePriceRounding,
                RoundNumbers = cur.RoundNumbers
            };
        }

        public static implicit operator Currency(BookingCurrency cur)
        {
            return new Currency
            {
                Iso3 = cur.CurrencyCode,
                NumIso3 = cur.CurrencyNumCode,
                Rate = cur.CurrencyValue,
                Symbol = cur.CurrencySymbol,
                IsCodeBefore = cur.IsCodeBefore,
                EnablePriceRounding = cur.EnablePriceRounding,
                RoundNumbers = cur.RoundNumbers
            };
        }

        public int BookingId { get; set; }
        public string CurrencyCode { get; set; }
        public int CurrencyNumCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        public float RoundNumbers { get; set; }
        public bool EnablePriceRounding { get; set; }
    }
}
