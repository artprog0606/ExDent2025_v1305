using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Models.ProductDetails;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetRelatedProductsHandler
    {
        #region Fields

        private readonly Product _product;
        private readonly RelatedType _type;
        private readonly bool _isMobile;

        #endregion

        public GetRelatedProductsHandler(Product product, RelatedType type, bool isMobile)
        {
            _product = product;
            _type = type;
            _isMobile = isMobile;
        }

        public RelatedProductsViewModel Get()
        {
            if (_product == null)
                return null;

            var (products, html) = 
                ProductService.GetRelatedProductsByRelatedTypeSource(_product, _type, WarehouseContext.GetAvailableWarehouseIds());

            var productModels = new List<ProductModel>();

            if (products != null && products.Count > 0)
            {
                productModels = products.Where(p => p.ProductId != _product.ProductId).ToList();
            }

            var model = new RelatedProductsViewModel()
            {
                Products = new ProductViewModel(productModels, _isMobile)
                {
                    ShowBriefDescription = _isMobile
                        ? SettingsMobile.ShowBriefDescription
                        : SettingsCatalog.CatalogVisibleBriefDescription
                },
                Html = html,
                RelatedType = _type.ToString().ToLower()
            };

            model.Products.DisplayPhotoPreviews = false;

            return model;
        }
    }
}