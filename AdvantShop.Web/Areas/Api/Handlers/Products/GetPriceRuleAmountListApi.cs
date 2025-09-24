using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public sealed class GetPriceRuleAmountListApi : AbstractCommandHandler<GetPriceRuleAmountListApiResponse>
    {
        private readonly int _productId;
        private readonly int? _offerId;
        private Offer _offer;

        public GetPriceRuleAmountListApi(int id, int? offerId)
        {
            _productId = id;
            _offerId = offerId;
        }

        protected override void Load()
        {
            if (_offerId != null)
            {
                _offer = OfferService.GetOffer(_offerId.Value);
                
                if (_offer == null || _offer.ProductId != _productId)
                    throw new BlException("Товар не найден");
            }
            else
            {
                var p = ProductService.GetProduct(_productId);
                if (p == null)
                    throw new BlException("Товар не найден");

                _offer = OfferService.GetMainOffer(p.Offers, p.AllowPreOrder,
                                            warehouseId: WarehouseContext.GetAvailableWarehouseIds()?.FirstOrDefault());
            }
        }

        protected override GetPriceRuleAmountListApiResponse Handle()
        {
            var items =
                _offer != null
                    ? PriceRuleService.GetPriceRuleAmountListItems(
                        _offer.OfferId,
                        _offer.Product,
                        CustomerContext.CurrentCustomer.CustomerGroup)
                    : null;

            return new GetPriceRuleAmountListApiResponse(items);
        }
    }
}