using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Barcodes
{
    public sealed class GetProductByBarcodeResponse : IApiResponse
    {
        public GetProductResponse Product { get; }

        public GetProductByBarcodeResponse(GetProductResponse product)
        {
            Product = product;
        }
    }
}