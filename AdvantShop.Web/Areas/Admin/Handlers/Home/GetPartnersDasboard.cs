using System.Collections.Generic;
using AdvantShop.Core.Services.DownloadableContent;
using AdvantShop.Web.Admin.ViewModels.Home;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetPartnersDasboard
    {
        public PartnersDashboardViewModel Execute()
        {
            var news = ShopNewsService.GetLastNews() ?? new AdminShopNews();

            return new PartnersDashboardViewModel()
            {
                News = news.NewsPartners ?? new List<AdminShopNewsItem>()
            };
        }
    }
}
