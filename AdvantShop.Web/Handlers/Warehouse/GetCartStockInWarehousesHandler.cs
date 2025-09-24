using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Models.Warehouse;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Warehouse
{
    public class GetCartStockInWarehousesHandler : ICommandHandler<int[], List<CartStockInWarehousesModel>>
    {
        public List<CartStockInWarehousesModel> Execute(int[] warehousesId)
        {
            var result = new List<CartStockInWarehousesModel>();
            if (warehousesId is null
                || warehousesId.Length is 0)
                return result;
            
            var cacheOfferStocks = new Dictionary<int, float>();

            foreach (var cartItem in ShoppingCartService.CurrentShoppingCart)
            {
                var model = new CartStockInWarehousesModel()
                {
                    Id = cartItem.ShoppingCartItemId,
                    Name = cartItem.Offer.Product.Name,
                };

                if (!cacheOfferStocks.ContainsKey(cartItem.OfferId))
                    cacheOfferStocks.Add(
                        cartItem.OfferId, 
                        WarehouseStocksService.GetOfferStocks(cartItem.OfferId)
                                              .Where(stock => warehousesId.Contains(stock.WarehouseId))
                                              .Sum(stock => stock.Quantity));

                if (cartItem.Amount > cacheOfferStocks[cartItem.OfferId])
                {
                    model.OutStock = true;
                    model.AvailableMessage =
                        cacheOfferStocks[cartItem.OfferId] <= 0f
                        ? "Нет в наличии"
                        : $"Доступно {cacheOfferStocks[cartItem.OfferId]} из {cartItem.Amount}";
                }

                cacheOfferStocks[cartItem.OfferId] -= cartItem.Amount;
                result.Add(model);
            }

            return result;
        }
    }
}