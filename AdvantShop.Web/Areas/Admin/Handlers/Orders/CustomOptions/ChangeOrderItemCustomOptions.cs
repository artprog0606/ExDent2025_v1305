using System;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class ChangeOrderItemCustomOptions
    {
        private readonly OrderItem _orderItem;
        private readonly Order _order;
        private readonly Currency _currency;
        private readonly string _customOptionsXml;
        private readonly string _artno;

        public ChangeOrderItemCustomOptions(OrderItem orderItem, Order order, string customOptionsXml, string artno)
        {
            _orderItem = orderItem;
            _order = order;
            _currency = order.OrderCurrency;
            _customOptionsXml = customOptionsXml;
            _artno = artno;
        }

        public bool Execute()
        {
            if (_orderItem.ProductID == null)
                throw new BlException("Товар не найден");

            var product = ProductService.GetProduct(_orderItem.ProductID.Value);
            if (product == null)
                throw new BlException("Товар не найден");

            var xml = !String.IsNullOrWhiteSpace(_customOptionsXml) && _customOptionsXml != "null"
                        ? HttpUtility.UrlDecode(_customOptionsXml)
                        : null;

            _orderItem.SelectedOptions = CustomOptionsService.DeserializeFromXml(xml, product.Currency.Rate, _order.OrderCurrency);
            
            _orderItem.Price = OrderItemPriceService.CalculateFinalPrice(_orderItem, _order, out _, out _);

            OrderService.DeleteOrderItemCustomOptions(_orderItem.OrderItemID);

            if (_orderItem.SelectedOptions != null && _orderItem.SelectedOptions.Count > 0)
            {
                foreach (var option in _orderItem.SelectedOptions)
                    OrderService.AddOrderItemCustomOption(option, _orderItem.OrderItemID);
            }

            return true;
        }
    }
}
