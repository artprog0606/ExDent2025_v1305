using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    public class GetBonusesResponse : List<BonusModel>, IApiResponse
    {
        public GetBonusesResponse(List<BonusModel> items)
        {
            this.AddRange(items);
        }
    }
}