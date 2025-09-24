//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Booking
{
    public class Service
    {
        public int Id { get; set; }
        public string ArtNo { get; set; }
        public int CategoryId { get; set; }
        public int CurrencyId { get; set; }
        private Currency _currency;
        public Currency Currency
        {
            get { return _currency ?? (_currency = CurrencyService.GetCurrency(CurrencyId, true)); }
        }

        public string Name { get; set; }
        public float BasePrice { get; set; }

        private float? _roundedPrice;

        public float RoundedPrice
        {
            get
            {
                return _roundedPrice ??
                       (float)(_roundedPrice = PriceService.RoundPrice(BasePrice, null, Currency.Rate));
            }
        }
        public string Image { get; set; }
        public string Description { get; set; }
        public TimeSpan? Duration { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
    }
}
