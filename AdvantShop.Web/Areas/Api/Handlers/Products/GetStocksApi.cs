using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Handlers.ProductDetails;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public class GetStocksApi : AbstractCommandHandler<ProductStocksResponse>
    {
        private readonly int _id;
        private readonly int? _offerId;

        public GetStocksApi(int id, int? offerId)
        {
            _id = id;
            _offerId = offerId;
        }

        protected override void Validate()
        {
            if (!ProductService.IsExists(_id))
                throw new BlException("Товар не найден");
            
            if (_offerId != null && OfferService.GetOffer(_offerId.Value) == null)
                throw new BlException("Модификация не найдена");
        }

        protected override ProductStocksResponse Handle()
        {
            var offerStocks = new List<OfferStocksResponse>();

            var offerIds = _offerId != null
                ? new List<int>() {_offerId.Value}
                : OfferService.GetProductOffers(_id).Select(x => x.OfferId).ToList();

            foreach (var offerId in offerIds)
            {
                var stocksResult = new GetOfferStocks(offerId){ShowOnlyAvalible = SettingsCatalog.ShowOnlyAvailableWarehousesInProduct}.Execute();

                offerStocks.Add(new OfferStocksResponse(offerId, stocksResult?.Stocks));
            }

            return new ProductStocksResponse(offerStocks);
        }
    }
}