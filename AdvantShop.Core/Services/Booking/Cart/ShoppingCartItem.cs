using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Booking.Cart
{
    public class ShoppingCartItem
    {
        public ShoppingCartItem()
        {
            Services = new List<ServiceItem>();
        }

        public int ShoppingCartItemId { get; set; }
        public Guid CustomerId { get; set; }
        public int AffiliateId { get; set; }
        public int? ReservationResourceId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<ServiceItem> Services { get; set; }
        public float Sum
        {
            get { return Services.Sum(x => x.Price); }
        }
    }

    public class ServiceItem
    {
        public int ServiceId { get; set; }
        public float Amount { get; set; }


        private float? _price;
        public float Price
        {
            get
            {
                return _price ?? (_price = PriceService.RoundPrice(Service.RoundedPrice, null, CurrencyService.CurrentCurrency.Rate)).Value;
            }
        }

        private Service _service;
        public Service Service
        {
            get
            {
                return _service ?? (_service = ServiceService.Get(ServiceId));
            }
        }

    }
}
