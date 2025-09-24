using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders.Coupons
{
    public class RemoveCoupon : AbstractCommandHandler
    {
        private readonly int _orderId;
        private Order _order;

        public RemoveCoupon(int orderId)
        {
            _orderId = orderId;
        }

        protected override void Validate()
        {
            _order = OrderService.GetOrder(_orderId);
            if (_order == null)
                throw new BlException("Заказ не найден");
            
            if (_order.Coupon == null)
                throw new BlException($"У заказа нет купона");
        }

        protected override void Handle()
        {
            var coupon = CouponService.GetCouponByCode(_order.Coupon.Code);
            if (coupon != null)
            {
                coupon.ActualUses -= 1;
                CouponService.UpdateCoupon(coupon);
            }

            _order.Coupon = null;
            OrderService.DeleteOrderCoupon(_order.OrderID);
            if (coupon != null && coupon.Type == CouponType.FixedOnGiftOffer && coupon.GiftOfferId.HasValue)
            {
                Offer offer = null;
                var item = _order.OrderItems.FirstOrDefault(x => x.IsByCoupon
                                                                && x.IsCustomPrice
                                                                && (offer = OfferService.GetOffer(x.ArtNo)) != null
                                                                && offer.OfferId == coupon.GiftOfferId);
                _order.OrderItems.Remove(item);
            }
            else
            {
                foreach (var orderItem in _order.OrderItems.Where(x => x.IsCouponApplied))
                {
                    orderItem.IsCouponApplied = false;
                    orderItem.Price =
                        OrderItemPriceService.CalculateFinalPrice(orderItem, _order, out _, out _, true);
                }
            }

            var oldOrderItems = OrderService.GetOrderItems(_order.OrderID);
            OrderService.AddUpdateOrderItems(_order.OrderItems, oldOrderItems, _order, trackChanges: !_order.IsDraft);

            new UpdateOrderTotal(_order).Execute();
        }
    }
}