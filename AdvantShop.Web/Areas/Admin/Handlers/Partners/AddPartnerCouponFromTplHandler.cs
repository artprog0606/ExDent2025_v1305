using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class AddPartnerCouponFromTplHandler : AbstractCommandHandler
    {
        private readonly int _id;
        private readonly string _couponCode;
        private Partner _partner;
        private Coupon _couponTpl;

        public AddPartnerCouponFromTplHandler(int partnerId, string couponCode)
        {
            _id = partnerId;
            _couponCode = couponCode;
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
            _couponTpl = CouponService.GetPartnersCouponTemplate();
        }

        protected override void Validate()
        {
            if (_couponCode == null)
                throw new BlException("Укажите код купона");
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
            if (_couponTpl == null)
                throw new BlException("Шаблон партнерского купона не настроен");
            if (_partner.CouponId.HasValue)
                throw new BlException(T("Admin.Partner.Errors.AlreadyHasCoupon"));
            if (CouponService.IsExistCouponCode(_couponCode) || GiftCertificateService.IsExistCertificateCode(_couponCode))
                throw new BlException("Код купона уже занят");
        }

        protected override void Handle()
        {
            var coupon = CouponService.GeneratePartnerCoupon(_couponTpl, _couponCode);
            _partner.CouponId = coupon.CouponID;
            PartnerService.UpdatePartner(_partner);
        }
    }
}
