using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Orders.CustomOptions;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders.CustomOptions
{
    public class GetOrderItemCustomOptions : ICommandHandler<List<OrderItemCustomOption>>
    {
        private readonly int _orderItemId;

        public GetOrderItemCustomOptions(int orderItemId)
        {
            _orderItemId = orderItemId;
        }

        public List<OrderItemCustomOption> Execute()
        {
            var orderItem = OrderService.GetOrderItem(_orderItemId);
            if (orderItem == null)
                throw new BlException("OrderItem not found");

            if (orderItem.ProductID == null)
                return null;

            var currency = OrderService.GetOrderCurrency(orderItem.OrderID)?? 
                           CurrencyService.CurrentCurrency;
            
            var productOptions = CustomOptionsService.GetCustomOptionsByProductId(orderItem.ProductID.Value);
            
            if (orderItem.SelectedOptions == null || orderItem.SelectedOptions.Count == 0)
                return productOptions.Select(x => new OrderItemCustomOption(x, currency)).ToList();

            foreach (var productOption in productOptions)
            {
                var orderItemOptionItems = orderItem.SelectedOptions.Where(x => x.CustomOptionId == productOption.CustomOptionsId && x != null).ToList();
                productOption.SelectedOptions = new List<OptionItem>();
                foreach (var orderItemOption in orderItemOptionItems)
                {
                    var item = productOption.Options.Find(x => x.OptionId == orderItemOption.OptionId);
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(orderItemOption.OptionTitle) &&
                            (productOption.InputType == CustomOptionInputType.TextBoxMultiLine ||
                             productOption.InputType == CustomOptionInputType.TextBoxSingleLine))
                        {
                            item.Title = orderItemOption.OptionTitle;
                        }

                        item.DefaultQuantity = orderItemOption.OptionAmount;
                        productOption.SelectedOptions.Add(item);
                    }
                }
            }

            return productOptions.Select(x => new OrderItemCustomOption(x, currency)).ToList();
        }
    }
}