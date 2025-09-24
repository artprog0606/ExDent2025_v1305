using AdvantShop.Web.Admin.Models.Catalog;

namespace AdvantShop.Web.Admin.Models.Marketing.Coupons
{
    public class CouponsCatalogProductModel : CatalogProductModelBySelector
    {        
        public int BrandId { get; set; }
        public string BrandName { get; set; }

        public int CouponId { get; set; }
        public bool ApllyCoupon { get; set; }
    }
}
