using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Models.Cart;

namespace AdvantShop.Areas.Api.Models.Cart
{
    public class GetCartResponse : IApiResponse
    {
        public string ValidationError { get; set; }
        public List<CartItemApi> Items { get; set; }
        public string Count { get; set; }
        public string PreparedTotalProductsPrice { get; set; }
        public string PreparedTotalPrice { get; set; }
        public string PreparedDiscountPrice { get; set; }
        public string Bonuses { get; set; }
        public CartCouponApi Coupon { get; set; }
        public bool ShowCoupon { get; set; }
        public string ColorHeader { get; set; }
        public string SizeHeader { get; set; }
        
        public CartBlocks Blocks { get; set; }
    }

    public class CartBlocks
    {
        public string BeforeBlock { get; set; }
        public string AfterBlock { get; set; }
        public string RightBlock { get; set; }
    }

    public class CartCouponApi
    {
        public string Code { get; set; }
        public string Price { get; set; }
        public string Percent { get; set; }
        public bool Applied { get; set; }
        
        public CartCouponApi(CartCoupon coupon)
        {
            Code = coupon.Code;
            Price = coupon.Price;
            Percent = coupon.Percent;
            Applied = !coupon.NotApplied;
        }
    }
}