using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetProductPropertiesInBriefResponse : List<ProductPropertyApi>, IApiResponse
    {
        public GetProductPropertiesInBriefResponse(List<ProductPropertyApi> propertyValues)
        {
            this.AddRange(propertyValues);
        }
    }
}