using System;
using AdvantShop.Core.Services.Api;
using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    [Obsolete]
    public class GetAdditionBonusesResponse : List<AdditionalBonusModel>, IApiResponse
    {
        public GetAdditionBonusesResponse(List<AdditionalBonusModel> items)
        {
            this.AddRange(items);
        }
    }
}