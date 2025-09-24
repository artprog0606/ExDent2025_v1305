using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public sealed class UserMeStatisticsResponse : IApiResponse
    {
        public string OrdersSum { get; set; }
        
        public int OrdersCount { get; set; }
        
        public string AverageCheck { get; set; }
        
        public string DurationOfWorkWithClient { get; set; }
        public int AddressesCount { get; set; }
        public int WishListCount { get; set; }
        public int ActiveOrdersCount { get; set; }
    }
}