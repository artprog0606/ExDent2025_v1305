using AdvantShop.Customers;

namespace AdvantShop.Models.Location
{
    public sealed class GeoModeModel
    {
        public CustomerContact CurrentContact { get; set; }
        public string ShippingType { get; set; }
        public bool IsUserRegistered { get; set; }
        public bool IsPointSelected { get; set; }
        public bool ShowSelfDelivery { get; set; }
        public bool ShowPickPoint { get; set; }
    }
}