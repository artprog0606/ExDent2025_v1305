using AdvantShop.Web.Admin.Models.Catalog;

namespace AdvantShop.Web.Admin.Models.Settings.ShippingMethods
{
    public class ShippingLinkProductModel : CatalogProductModel
    {
        public int ShippingId { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool LinkShipping { get; set; }
    }
}