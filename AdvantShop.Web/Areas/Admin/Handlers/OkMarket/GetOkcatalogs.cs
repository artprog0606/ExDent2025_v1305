using AdvantShop.Core.Services.Crm.Ok.OkMarket;
using AdvantShop.Web.Admin.Models.OkMarket;
using AdvantShop.Web.Infrastructure.Admin;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.OkMarket
{
    public class GetOkCatalogs
    {
        public FilterResult<OkMarketCatalogModel> Execute()
        {
            var catalogs = OkMarketService.GetCatalogList().Select(x => new OkMarketCatalogModel(x)).ToList();

            var model = new FilterResult<OkMarketCatalogModel>
            {
                TotalItemsCount = catalogs.Count,
                TotalPageCount = 1,
                DataItems = catalogs,
                TotalString = "Всего: " + catalogs.Count
            };

            return model;
        }
    }
}