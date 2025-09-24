using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.FivePost;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.FivePost
{
    public class FivePostOrderActions
    {
        private readonly int _orderId;

        public FivePostOrderActions(int orderId)
        {
            _orderId = orderId;
        }

        public OrderActionsModel Execute()
        {
            var trackNumber = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.FivePost.FivePost.TrackNumberOrderAdditionalDataName);

            return new OrderActionsModel
            {
                OrderId = _orderId,
                ShowCreateOrder = trackNumber.IsNullOrEmpty(),
                ShowDeleteOrder = trackNumber.IsNotEmpty()
            };
        }
    }
}
