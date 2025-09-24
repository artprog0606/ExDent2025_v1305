namespace AdvantShop.Web.Admin.Models.Settings.ShippingMethods
{
    public class ShippingCalculateModel
    {
        public int Id { get; set; }
        public string Iso3 { get; set; }
        public PreOrderItemModel Item { get; set; }
        public PreOrderModel PreOrder { get; set; }
        public bool ExtrachargeCargoEnabled { get; set; }
        public bool MarginEnabled { get; set; }
        public bool GeoEnabled { get; set; }
    }

    public class PreOrderModel
    {
        public string CityDest { get; set; }
        public string CountryDest { get; set; }
        public string RegionDest { get; set; }
        public string Zip { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Structure { get; set; }
        public string Apartment { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
    }

    public class PreOrderItemModel
    {
        public float Amount { get; set; }
        public float Weight { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }
        public float Price { get; set; }
    }
}
