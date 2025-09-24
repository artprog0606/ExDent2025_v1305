using AdvantShop.Areas.Api.Handlers.Products;
using AdvantShop.Areas.Api.Models.Barcodes;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Barcodes
{
    public sealed class GetProductByBarcodeApi : AbstractCommandHandler<GetProductByBarcodeResponse>
    {
        private readonly SearchBarcode _model;
        private readonly ProductApiService _productApiService;

        public GetProductByBarcodeApi(SearchBarcode model)
        {
            _model = model;
            _productApiService = new ProductApiService();
        }
        
        protected override void Validate()
        {
            if (_model == null || string.IsNullOrWhiteSpace(_model.Barcode))
                throw new BlException("Укажите штрихкод");
        }

        protected override GetProductByBarcodeResponse Handle()
        {
            var offer = _productApiService.GetOfferByBarcode(_model.Barcode);
            if (offer == null)
                return new GetProductByBarcodeResponse(null);
            
            var productResponse = new GetProductApi(offer.ProductId, offer.ColorID, offer.SizeID).Execute();

            return new GetProductByBarcodeResponse(productResponse);
        }
    }
}