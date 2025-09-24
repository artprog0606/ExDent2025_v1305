using AdvantShop.Web.Admin.Models.Catalog;

namespace AdvantShop.Web.Admin.Models.Settings.ShippingMethods
{
    public class ShippingProductsFilterModel : CatalogFilterModel
    {
        public int ShippingId { get; set; }
        
        public EnTypeRef TypeRef { get; set; }

        public bool? LinkShipping { get; set; }
    }

    public enum EnTypeRef
    {
        Exclude
    }
}