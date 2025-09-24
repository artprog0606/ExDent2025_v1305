using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Helpers;
using AdvantShop.Models.ProductDetails;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetOfferStocks : ICommandHandler<OfferStocksModel>
    {
        private readonly int _offerId;

        public bool ShowOnlyAvalible { get; set; }

        public GetOfferStocks(int offerId)
        {
            _offerId = offerId;
        }

        public OfferStocksModel Execute()
        {
            var offer = OfferService.GetOffer(_offerId);
            if (offer is null) 
                return null;

            var model = new OfferStocksModel()
            {
                Stocks = new List<WarehouseStockModel>()
            };
            
            var warehouses = WarehouseService.GetList(enabled: true);
            var offerStocks = WarehouseStocksService.GetOfferStocks(offer.OfferId);
            var stockLabels = StockLabelService.GetAll();

            foreach (var warehouse in warehouses)
            {
                var offerStock = offerStocks.FirstOrDefault(offerStockItem => offerStockItem.WarehouseId == warehouse.Id);
                
                if (ShowOnlyAvalible
                    && (offerStock?.Quantity ?? 0f) <= 0f)
                    continue;

                var stockLabel = StockLabelService.GetStockLabel(offerStock?.Quantity ?? 0f, stockLabels);

                var unit = offer.Product.Unit?.DisplayName ?? "";

                model.Stocks.Add(new WarehouseStockModel()
                {
                    Name = warehouse.Name,
                    Type = warehouse.TypeId.HasValue
                        ? TypeWarehouseService.Get(warehouse.TypeId.Value)?.Name
                        : null,
                    Address = StringHelper.AggregateStrings(", ", 
                        warehouse.CityId.HasValue 
                            ? CityService.GetCity(warehouse.CityId.Value)?.Name
                            : null, 
                        warehouse.Address),
                    AddressComment = warehouse.AddressComment,
                    Latitude = warehouse.Latitude,
                    Longitude = warehouse.Longitude,
                    TimeOfWorkList =  
                        TimeOfWorkService.GetWarehouseTimeOfWork(warehouse.Id)
                                         .Select(TimeOfWorkService.FormatTimeOfWork)
                                         .ToArray(),
                    Stock = stockLabel is null
                        ? (offerStock?.Quantity ?? 0f).ToInvariantString()
                        : stockLabel.ClientName,
                    StockColor = stockLabel?.Color,
                    Unit = unit
                });
            }

            return model;
        }
    }
}