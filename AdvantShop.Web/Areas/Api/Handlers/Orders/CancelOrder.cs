using AdvantShop.Core.Services.Api;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public sealed class CancelOrder : AbstractCommandHandler<ApiResponse>
    {
        private readonly int _orderId;

        public CancelOrder(int orderId)
        {
            _orderId = orderId;
        }

        protected override ApiResponse Handle()
        {
            var order = OrderService.GetOrder(_orderId);
            
            new AdvantShop.Handlers.MyAccount.CancelOrder(order).Execute();

            return new ApiResponse();
        }
    }
}