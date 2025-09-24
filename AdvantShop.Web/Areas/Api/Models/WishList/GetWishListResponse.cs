using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Core.Services.Api;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.WishList
{
    public class GetWishListResponse : List<WishListItem>, IApiResponse
    {
        public GetWishListResponse(List<WishListItem> items)
        {
            this.AddRange(items);
        }
    }

    public class WishListItem : ProductItemShort
    {
        [JsonProperty(Order = 10)]
        public SizeApi Size { get; }
        
        [JsonProperty(Order = 11)]
        public ColorApi Color { get; }
    
        public WishListItem(ShoppingCartItem item) : base(item.Offer.Product, item.Offer)
        {
            Size = item.Offer.Size != null ? new SizeApi(item.Offer.Size) : null;
            Color = item.Offer.Color != null ? new ColorApi(item.Offer.Color) : null;
        }
    }
}