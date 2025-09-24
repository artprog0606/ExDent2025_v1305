using AdvantShop.Core.Services.Api;
using AdvantShop.Handlers.Coupons;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Coupons
{
    public sealed class RemoveCouponApi : AbstractCommandHandler<ApiResponse>
    {
        protected override ApiResponse Handle()
        {
            new RemoveCoupon().Execute();
            return new ApiResponse(ApiStatus.Ok, null);
        }
    }
}