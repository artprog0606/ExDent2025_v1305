using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders.Coupons
{
    public class ChangeCoupon : AbstractCommandHandler
    {
        private readonly int _orderId;
        private readonly string _couponCode;

        private Order _order;
        private Coupon _coupon;

        public ChangeCoupon(int orderId, string couponCode)
        {
            _orderId = orderId;
            _couponCode = couponCode;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_couponCode))
                throw new BlException("Укажите код купона");

            _coupon = CouponService.GetCouponByCode(_couponCode);
            if (_coupon == null)
                throw new BlException("Купон не найден");

            _order = OrderService.GetOrder(_orderId);
            if (_order == null)
                throw new BlException("Заказ не найден");
            
            if (_order.Coupon != null)
                throw new BlException($"К заказу уже применен купон {_order.Coupon.Code}");

            if (_coupon != null && _coupon.OnlyInMobileApp &&
                _order.OrderSource != null && _order.OrderSource.Type != OrderType.MobileApp)
            {
                throw new BlException($"Купон может быть применен только в мобильном приложении. Источник заказа не мобильное приложение.");
            }
            
            if (_coupon != null && _coupon.ForFirstOrderInMobileApp)
            {
                if (_order.OrderSource != null && _order.OrderSource.Type != OrderType.MobileApp)
                    throw new BlException($"Купон может быть применен только в мобильном приложении. Источник заказа не мобильное приложение.");
                
                if (_order.OrderCustomer == null)
                    throw new BlException("Купон может быть применен из-за отсутсвия покупателя.");
                
                if (OrderService.IsCustomerHasConfirmedOrdersFromMobileApp(_order.OrderCustomer.CustomerID))
                    throw new BlException(T("Coupon.CouponPost.CouponOnlyForFirstOrderInMobileApp"));
            }

            if (_coupon != null && _coupon.OnlyOnCustomerBirthday )
            {
                var error = CouponService.CheckCustomerCouponByBirthday(_coupon, CustomerService.GetCustomer(_order.OrderCustomer.CustomerID));
                if (error.IsNotEmpty())
                    throw new BlException(error);
            }
        }

        protected override void Handle()
        {
            _order.Coupon = new OrderCoupon(_coupon);

            if (_coupon.Type == CouponType.FixedOnGiftOffer && _coupon.GiftOfferId.HasValue && _coupon.IsAppliedToOrder(_order))
            {
                var offer = OfferService.GetOffer(_coupon.GiftOfferId.Value);
                var orderItem = (OrderItem)offer;

                orderItem.Price = _coupon.GetRate();
                orderItem.IsByCoupon = true;
                orderItem.IsCustomPrice = true;
                
                _order.OrderItems.Add(orderItem);
            }
            else
            {
                foreach (var item in _order.OrderItems)
                {
                    if (!item.ProductID.HasValue)
                        continue;

                    OrderItemPriceService.CalculateFinalPrice(item, _order, out float price, out Discount discount);
                    
                    if (_coupon.IsAppliedToProduct(item.ProductID.Value, price, discount, item.DoNotApplyOtherDiscounts))
                    {
                        item.IsCouponApplied = true;

                        if (item.IsCustomPrice)
                        {
                            item.BasePrice = item.Price;
                            item.DiscountAmount = 0;
                            item.DiscountPercent = 0;
                        }
                        
                        if (!_coupon.IsAppliedToPriceWithDiscount)
                            item.Price = price;
                    }
                }
            }

            if (_coupon.MinimalOrderPrice > 0 && !_coupon.IsAppliedToOrder(_order))
                throw new BlException($"Купон не может быть применен, так как стоимость товаров, участвующих в скидке по купону, меньше {_coupon.MinimalOrderPrice} {_order.OrderCurrency.CurrencySymbol}");

            OrderService.AddOrderCoupon(_order.OrderID, _order.Coupon);

            var oldOrderItems = OrderService.GetOrderItems(_order.OrderID);
            OrderService.AddUpdateOrderItems(_order.OrderItems, oldOrderItems, _order, trackChanges: !_order.IsDraft);

            _coupon.ActualUses += 1;
            CouponService.UpdateCoupon(_coupon);

            new UpdateOrderTotal(_order).Execute();
        }
    }
}