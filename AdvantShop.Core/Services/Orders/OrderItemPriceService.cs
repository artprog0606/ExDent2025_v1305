using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Orders
{
    public class OrderItemPriceService
    {
        public static float CalculateFinalPrice(OrderItem orderItem, Order order, out float productPrice, out Discount discount, bool useBasePrice = false)
        {
            var currency = order.OrderCurrency;
            var customerGroup = order.GetCustomerGroup();
            
            var priceByPriceRule =
                CalculateFinalPriceByPriceRule(orderItem, order, currency, customerGroup, out productPrice, out discount);
            
            if (priceByPriceRule != null)
                return priceByPriceRule.Value;

            var basePrice =
                !useBasePrice 
                    ? orderItem.GetPrice() 
                    : orderItem.BasePrice ?? orderItem.Price;

            return CalculateFinalPriceByBasePrice(basePrice, orderItem, currency, customerGroup, out productPrice, out discount);
        }

        private static float CalculateFinalPriceByBasePrice(float basePrice, OrderItem orderItem, OrderCurrency currency, 
                                                            CustomerGroup customerGroup, 
                                                            out float productPrice, out Discount discount)
        {
            var customOptionsPrice = CustomOptionsService.GetCustomOptionPrice(basePrice, orderItem.SelectedOptions);
            
            productPrice = PriceService.RoundPrice(basePrice + customOptionsPrice, currency, currency.CurrencyValue);
            
            var offer = OfferService.GetOffer(orderItem.ArtNo);

            discount =
                PriceService.GetFinalDiscount(productPrice,
                    orderItem.IsCustomPrice
                        ? new Discount()
                        : GetDiscount(orderItem, productPrice, currency), //new Discount(orderItem.DiscountPercent, orderItem.DiscountAmount),
                    currency.CurrencyValue,
                    customerGroup,
                    productId: offer?.ProductId ?? 0,
                    doNotApplyOtherDiscounts: orderItem.DoNotApplyOtherDiscounts,
                    productMainCategoryId: offer?.Product.CategoryId);
            
            var finalPrice = PriceService.GetFinalPrice(productPrice, discount, currency.CurrencyValue, currency);

            return finalPrice;
        }

        private static float? CalculateFinalPriceByPriceRule(OrderItem orderItem, Order order, OrderCurrency currency, 
                                                                CustomerGroup customerGroup, 
                                                                out float productPrice, out Discount discount)
        {
            productPrice = 0;
            discount = null;
            
            if (orderItem.IsCustomPrice || orderItem.ProductID == null)
                return null;
            
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.PriceTypes)
                return null;
            
            var offer = OfferService.GetOffer(orderItem.ArtNo);
            if (offer == null)
                return null;

            if (customerGroup.CustomerGroupId == 0 && order.OrderCustomer != null && order.OrderCustomer.CustomerID != Guid.Empty)
            {
                var customer = CustomerService.GetCustomer(customerGroup.CustomerGroupId);
                if (customer != null)
                    customerGroup.CustomerGroupId = customer.CustomerGroupId;
            }

            if (customerGroup.CustomerGroupId == 0)
                customerGroup.CustomerGroupId = CustomerGroupService.DefaultCustomerGroup;

            offer.SetPriceRule(orderItem.Amount, customerGroup.CustomerGroupId);
                    
            var customOptionsPrice = CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, orderItem.SelectedOptions);
                    
            productPrice = PriceService.RoundPrice(offer.RoundedPrice + customOptionsPrice, null, currency.CurrencyValue);

            discount = 
                offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                    ? PriceService.GetFinalDiscount(productPrice, 
                        GetDiscount(offer, orderItem, productPrice, currency), //new Discount(orderItem.DiscountPercent, orderItem.DiscountAmount), 
                        currency.CurrencyValue,
                        customerGroup,
                        productId: offer.ProductId,
                        doNotApplyOtherDiscounts: orderItem.DoNotApplyOtherDiscounts,
                        productMainCategoryId: offer.Product.CategoryId)
                    : new Discount();
                
            var finalPrice = PriceService.GetFinalPrice(productPrice, discount);
                    
            return finalPrice;
        }

        /// <summary>
        /// Вернет большую из скидок: скидка товара или скидка модуля
        /// </summary>
        private static Discount GetDiscount(Offer offer, OrderItem orderItem, float productPrice, OrderCurrency orderCurrency)
        {
            if (offer == null)
                return new Discount(orderItem.DiscountPercent, orderItem.DiscountAmount);
            
            if (offer.PriceRule != null && !offer.PriceRule.ApplyDiscounts)
                return new Discount();
            
            var product = offer.Product;
            
            if (product.DoNotApplyOtherDiscounts)
                return offer.Product.Discount;
            
            var discountByProduct = offer.Product.Discount;
            var discountByModule = GetModuleDiscount(orderItem);
            
            if (discountByModule == null)
                return discountByProduct;

            if (discountByModule.Type == discountByProduct.Type)
            {
                return discountByModule.Value > discountByProduct.Value
                        ? discountByModule
                        : discountByProduct;
            }

            var priceByModule = PriceService.GetFinalPrice(productPrice, discountByModule, product.Currency.Rate, orderCurrency);
            var priceByProduct = PriceService.GetFinalPrice(productPrice, discountByProduct, product.Currency.Rate, orderCurrency);
            
            return priceByModule > priceByProduct 
                ? discountByProduct 
                : discountByModule;
        }
        
        /// <summary>
        /// Вернет большую из скидок: скидка orderItem или скидка модуля
        /// </summary>
        private static Discount GetDiscount(OrderItem orderItem, float productPrice, OrderCurrency orderCurrency)
        {
            var discountByOrderItem = new Discount(orderItem.DiscountPercent, orderItem.DiscountAmount);
            var discountByModule = GetModuleDiscount(orderItem);
            
            if (discountByModule == null)
                return discountByOrderItem;

            if (discountByModule.Type == discountByOrderItem.Type)
            {
                return discountByModule.Value > discountByOrderItem.Value
                    ? discountByModule
                    : discountByOrderItem;
            }

            var priceByModule = PriceService.GetFinalPrice(productPrice, discountByModule, orderCurrency.CurrencyValue, orderCurrency);
            var priceByProduct = PriceService.GetFinalPrice(productPrice, discountByOrderItem, orderCurrency.CurrencyValue, orderCurrency);
            
            return priceByModule > priceByProduct 
                ? discountByOrderItem 
                : discountByModule;
        }

        // TODO: возможно нужно возвращать список скидок модулей и искать большую скидку
        private static Discount GetModuleDiscount(OrderItem orderItem)
        {
            var modules = AttachedModules.GetModuleInstances<IOrderItemDiscount>();
            if (modules == null || modules.Count == 0)
                return null;
            
            foreach (var moduleDiscount in modules)
            {
                var discount = moduleDiscount.GetOrderItemDiscount(orderItem);
                if (discount != null && discount.HasValue)
                    return discount;
            }
            return null;
        }
    }
}