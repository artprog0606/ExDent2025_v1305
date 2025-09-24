using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Stories;
using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Inits
{
    public class GetStoriesRespone : List<StoryData>, IApiResponse
    {
        public GetStoriesRespone(List<StoryData> items)
        {
            this.AddRange(items);
        }
    }
}