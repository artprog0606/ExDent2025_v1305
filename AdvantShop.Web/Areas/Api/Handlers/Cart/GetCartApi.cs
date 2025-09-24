using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Cart;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Catalog;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Handlers.Cart;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Cart
{
    public sealed class GetCartApi : AbstractCommandHandler<GetCartResponse>
    {
        private readonly CartApiModel _cart;
        private readonly bool _useCurrentCart;
        private readonly List<CartItemMap> _cartMap = new List<CartItemMap>();

        public GetCartApi(CartApiModel cart)
        {
            _cart = cart;
        }
        
        public GetCartApi()
        {
            _useCurrentCart = true;
        }

        protected override GetCartResponse Handle()
        {
            var items = _cart?.Items ?? new List<CartItemApiModel>();
            var clearCart = !_useCurrentCart && _cart != null && !_cart.AddItemsToCurrentCart;
            
            if (clearCart)
                ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

            foreach (var item in items)
            {
                var offer = OfferService.GetOffer(item.OfferId);
                if (offer == null)
                    continue;

                var cartItem = new ShoppingCartItem()
                {
                    CustomerId = CustomerContext.CustomerId,
                    OfferId = item.OfferId,
                    Amount = item.Amount,
                    AttributesXml = CustomOptionsHelper.GetAttributesXml(offer, item.CustomOptions)
                };

                ShoppingCartService.AddShoppingCartItem(cartItem, false);
                
                ProductGiftService.AddGiftByOfferToCart(offer);
                
                _cartMap.Add(new CartItemMap() {Index = item.Index, CartItem = cartItem});
            }
            
            var cart = new GetCartHandler(ShoppingCartService.CurrentShoppingCart).Get();
            var cartItemsApi = cart.CartProducts.Select(x => new CartItemApi(x)).ToList();

            if (cartItemsApi.Count > 0)
            {
                foreach (var item in cartItemsApi)
                {
                    var mapItem = _cartMap.Find(map => map.CartItem.OfferId == item.OfferId && map.CartItem.AttributesXml == item.AttributesXml);
                    if (mapItem != null)
                        item.Index = mapItem.Index;
                }

                var max = cartItemsApi.Max(x => x.Index) + 1;

                foreach (var item in cartItemsApi)
                {
                    if (item.Index == 0)
                        item.Index = max++;
                }
            }

            return new GetCartResponse()
            {
                ValidationError = !string.IsNullOrEmpty(cart.Valid) ? cart.Valid : null,

                Items = cartItemsApi.OrderBy(x => x.Index).ToList(),
                Count = cart.Count,
                PreparedTotalProductsPrice = cart.TotalProductPrice,
                PreparedTotalPrice = cart.TotalPrice,
                PreparedDiscountPrice = cart.DiscountPrice,
                Bonuses = cart.BonusPlus,
                Coupon = cart.Coupon != null ? new CartCouponApi(cart.Coupon) : null,
                ShowCoupon = cart.CouponInputVisible,
                ColorHeader = cart.ColorHeader,
                SizeHeader = cart.SizeHeader,
                
                Blocks = new CartBlocks()
                {
                    BeforeBlock = ModulesExecuter.GetModuleKeyValues("api_cart_before"),
                    AfterBlock = ModulesExecuter.GetModuleKeyValues("api_cart_after"),
                    RightBlock = ModulesExecuter.GetModuleKeyValues("api_cart_right_block")
                }
            };
        }
    }
}