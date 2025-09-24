using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Models.Booking.Cart
{
    public class AddToCartResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("cartId")]
        public int CartId { get; set; }

        public float TotalItems { get; internal set; }

        public AddToCartResult(string status)
        {
            Status = status;
        }
    }
}
