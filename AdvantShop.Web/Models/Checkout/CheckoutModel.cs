﻿using AdvantShop.Configuration;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;

namespace AdvantShop.Models.Checkout
{
    public enum CheckoutShowMode
    {
        None = 0,
        Lp = 1,
        EmptyLayout = 2
    }
    
    
    public class CheckoutModel
    {
        public bool ShowProductsPhotoInCheckoutCart { get; set; }
        public CheckoutData CheckoutData { get; set; }
        public bool IsLanding { get; set; }
        public bool IsApi { get; set; }
        public CheckoutShowMode ShowMode { get; set; }
        public OrderType? OrderType { get; set; }
        
        public EShoppingCartMode ShoppingCartMode { get; set; }
        public bool AllowEditAmount { get; set; }
        public bool ShowBriefDescriptionProductInCheckout { get; set; }
    }
}