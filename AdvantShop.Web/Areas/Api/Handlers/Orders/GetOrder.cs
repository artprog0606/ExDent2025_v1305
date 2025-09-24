using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class GetOrder : ICommandHandler<OrderModel>
    {
        private readonly int _id;
        private readonly Guid? _customerId;
        private readonly bool _preparePrices;
        private readonly bool _extended;
        
        public GetOrder(int id)
        {
            _id = id;
        }
        
        public GetOrder(int id, Guid customerId) : this(id)
        {
            _customerId = customerId;
            _preparePrices = true;
            _extended = true;
        }
        
        public OrderModel Execute()
        {
            var order = OrderService.GetOrder(_id);

            if (order == null || order.IsDraft)
                throw new BlException("Заказ не найден");
            
            if (_customerId.HasValue && (order.OrderCustomer == null || order.OrderCustomer.CustomerID != _customerId.Value))
                throw new BlException("Заказ не найден");

            var result = OrderModel.FromOrder(order, _preparePrices, _extended);

            if (_extended)
            {
                result.Review = order.OrderReview != null ? new OrderReviewModel(order.OrderReview) : null;
                
                if (BonusSystem.IsActive)
                {
                    var bonusCardPurchase = PurchaseService.GetByOrderId(order.OrderID);
                    result.PreparedNewBonusesForOrder = bonusCardPurchase != null
                        ? ((float) bonusCardPurchase.NewBonusAmount).FormatBonuses()
                        : "";
                }

                foreach (var item in result.Items)
                {
                    if (item.ProductId == null) 
                        continue;

                    item.CanAddToCard = false;
                    item.Unit = "";
                    item.Multiplicity = 1;
                    
                    var product = ProductService.GetProduct(item.ProductId.Value);
                    if (product != null)
                    {
                        item.Multiplicity = product.Multiplicity;

                        var offer = product.Offers.FirstOrDefault(x => x.ArtNo.Equals(item.ArtNo, StringComparison.OrdinalIgnoreCase));

                        item.CanAddToCard = 
                            product.Enabled && product.CategoryEnabled &&
                            offer != null && 
                            offer.Amount >= item.Amount &&
                            offer.IsAvailableForPurchase(offer.Amount, product.GetMinAmount(), offer.RoundedPrice, product.Discount, product.AllowBuyOutOfStockProducts());

                        if (offer != null)
                            item.OfferId = offer.OfferId;
                    }

                    var customOptions = OrderService.GetOrderCustomOptionsByOrderItemId(item.OrderItemId);
                    if (customOptions != null && customOptions.Count > 0)
                    {
                        var productCustomOptions = product != null
                            ? CustomOptionsService.GetCustomOptionsByProductId(product.ProductId)
                            : null;

                        item.SelectedCustomOptions =
                            customOptions.Select(x =>
                                    new SelectedOrderCustomOptionItem(x, productCustomOptions, order.OrderCurrency))
                                .ToList();

                        if (item.CanAddToCard.Value)
                            item.CanAddToCard = IsValidCustomOptions(productCustomOptions, customOptions);
                    }
                }
            }

            return result;
        }

        private bool IsValidCustomOptions(List<CustomOption> productCustomOptions, List<EvaluatedCustomOptions> orderItemCustomOptions)
        {
            if (productCustomOptions == null || productCustomOptions.Count == 0)
                return false;
            
            foreach (var orderItemCustomOption in orderItemCustomOptions)
            {
                var item =
                    productCustomOptions.Find(x =>
                        x.CustomOptionsId == orderItemCustomOption.CustomOptionId &&
                        x.Options.Any(o => o.OptionId == orderItemCustomOption.OptionId));

                if (item == null)
                    return false;
            }

            foreach (var productCustomOption in productCustomOptions.Where(x => x.IsRequired))
            {
                var item =
                    orderItemCustomOptions.Find(x =>
                        x.CustomOptionId == productCustomOption.CustomOptionsId &&
                        productCustomOption.Options.Any(o => o.OptionId == x.OptionId));

                if (item == null)
                    return false;
            }

            return true;
        }
    }
}