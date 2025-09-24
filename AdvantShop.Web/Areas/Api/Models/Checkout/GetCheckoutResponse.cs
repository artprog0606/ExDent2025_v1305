using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Checkout
{
    public class GetCheckoutResponse : IApiResponse
    {
        public string Url { get; set; }
    }
}