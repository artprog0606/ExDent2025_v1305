using System;
using AdvantShop.Areas.Api.Models.Cart;

namespace AdvantShop.Areas.Api.Models.Checkout
{
    public sealed class CheckoutApiModel : CartApiModel
    {
        public CheckoutAddressApi Address { get; set; }
        
        public string ShippingId { get; set; }

        public string ShippingPointId { get; set; }
        
        public Guid? ContactId { get; set; }
    }

    public sealed class CheckoutAddressApi
    {
        public string ContactId { get; set; }
        
        public string Country { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
    }
}