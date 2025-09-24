using AdvantShop.Areas.Api.Models.Coupons;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Handlers.Coupons;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Coupons
{
    public sealed class AddCouponApi  : AbstractCommandHandler<AddCouponResponse>
    {
        private readonly CouponModel _coupon;

        public AddCouponApi(CouponModel coupon)
        {
            _coupon = coupon;
        }

        protected override void Validate()
        {
            if (_coupon == null || string.IsNullOrWhiteSpace(_coupon.Code))
                throw new BlException("Укажите код купона");
        }

        protected override AddCouponResponse Handle()
        {
            var result = new AddCoupon(_coupon.Code).Execute();
            return new AddCouponResponse()
            {
                Status = result.Result ? ApiStatus.Ok : ApiStatus.Error,
                Errors = result.Message
            };
        }
    }
}