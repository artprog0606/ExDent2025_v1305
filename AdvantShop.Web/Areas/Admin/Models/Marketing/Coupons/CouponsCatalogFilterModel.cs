using AdvantShop.Web.Admin.Models.Catalog;

namespace AdvantShop.Web.Admin.Models.Marketing.Coupons
{
    public class CouponsCatalogFilterModel : CatalogFilterModel
    {
        public int CouponId { get; set; }

        public bool? ApllyCoupon { get; set; }
    }
}
