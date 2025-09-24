using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetProductReviewsResponse : List<ProductReviewApi>, IApiResponse
    {
        public GetProductReviewsResponse(List<ProductReviewApi> reviews)
        {
            this.AddRange(reviews);
        }
    }
}