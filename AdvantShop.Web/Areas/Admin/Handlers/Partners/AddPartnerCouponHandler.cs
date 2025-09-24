using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class AddPartnerCouponHandler : AbstractCommandHandler
    {
        private readonly int _id;
        private readonly int _couponId;
        private Partner _partner;
        private Coupon _coupon;

        public AddPartnerCouponHandler(int partnerId, int couponId)
        {
            _id = partnerId;
            _couponId = couponId;
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
            _coupon = CouponService.GetCoupon(_couponId);
        }

        protected override void Validate()
        {
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
            if (_partner.CouponId.HasValue)
                throw new BlException(T("Admin.Partner.Errors.AlreadyHasCoupon"));
            if (_coupon == null)
                throw new BlException(T("Admin.Partner.Errors.CouponNotFound"));
        }

        protected override void Handle()
        {
            _partner.CouponId = _couponId;
            PartnerService.UpdatePartner(_partner);
        }
    }
}
