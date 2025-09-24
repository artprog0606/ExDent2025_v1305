using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Areas.Api.Models.Products
{
    public sealed class GetPriceRuleAmountListApiResponse : IApiResponse
    {
        public GetPriceRuleAmountListApiResponse(List<PriceRuleAmountListItem> items)
        {
            Items = items?.Select(x => new PriceRuleAmountListItemApi(x)).ToList();
        }
        
        public List<PriceRuleAmountListItemApi> Items { get; }
    }

    public sealed class PriceRuleAmountListItemApi
    {
        public PriceRuleAmountListItemApi(PriceRuleAmountListItem item)
        {
            Amount = item.Amount;
            Price = item.Price;
        }
        
        public string Amount { get; }
        public string Price { get; }
    }
}