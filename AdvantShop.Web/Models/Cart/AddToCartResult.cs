using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Models.Cart
{
    public class AddToCartResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("cartId")]
        public int CartId { get; set; }

        public float TotalItems { get; internal set; }

        public ShoppingCartItem CartItem { get; set; }

        public AddToCartResult(string status)
        {
            Status = status;
        }
    }
}