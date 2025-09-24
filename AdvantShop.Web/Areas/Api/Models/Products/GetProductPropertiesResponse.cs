using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetProductPropertiesResponse : List<ProductPropertyGroupApi>, IApiResponse
    {
        public GetProductPropertiesResponse(List<ProductPropertyGroupApi> propertyGroups)
        {
            this.AddRange(propertyGroups);
        }
    }
}