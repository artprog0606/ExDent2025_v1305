using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Models.ProductDetails;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class StocksResponse : List<WarehouseStockModel>, IApiResponse
    {
        public StocksResponse(List<WarehouseStockModel> stocks)
        {
            if (stocks != null)
                this.AddRange(stocks);
        }
    }
    
    public class ProductStocksResponse : List<OfferStocksResponse>, IApiResponse
    {
        public ProductStocksResponse( List<OfferStocksResponse> offerStocks)
        {
            this.AddRange(offerStocks);
        }
    }

    public class OfferStocksResponse
    {
        public int OfferId { get; }
        public List<WarehouseStockModel> Stocks { get; }

        public OfferStocksResponse(int offerId, List<WarehouseStockModel> stocks)
        {
            OfferId = offerId;
            Stocks = stocks;
        }
    }
}