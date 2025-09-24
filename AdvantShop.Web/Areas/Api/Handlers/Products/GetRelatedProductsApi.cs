using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Handlers.ProductDetails;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public enum ERelatedProductsApiType
    {
        CrossSell = 0,
        UpSell = 1
    }
    
    public class GetRelatedProductsApi : AbstractCommandHandler<GetRelatedProductsResponse>
    {
        private readonly int _id;
        private readonly ERelatedProductsApiType _type;

        private Product _product;

        public GetRelatedProductsApi(int id, ERelatedProductsApiType type)
        {
            _id = id;
            _type = type;
        }

        protected override void Validate()
        {
            _product = ProductService.GetProduct(_id);
            if (_product == null)
                throw new BlException("Товар не найден");
        }

        protected override GetRelatedProductsResponse Handle()
        {
            var model = new GetRelatedProductsHandler(_product, (RelatedType) (int) _type, SettingsDesign.IsMobileTemplate).Get();

            var products =
                model?.Products?.Products != null
                    ? model.Products.Products
                        .Select(x => new CatalogProductItem(x, false, ShoppingCartService.CurrentWishlist))
                        .ToList()
                    : new List<CatalogProductItem>();

            return new GetRelatedProductsResponse(products);
        }
    }
}