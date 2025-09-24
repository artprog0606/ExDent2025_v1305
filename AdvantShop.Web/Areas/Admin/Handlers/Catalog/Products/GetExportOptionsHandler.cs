using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetExportOptionsHandler : AbstractCommandHandler<ProductExportOptionsModel>
    {
        private readonly int _productId;
        private Product _product;

        public GetExportOptionsHandler(int productId)
        {
            _productId = productId;
        }

        protected override void Load()
        {
            _product = ProductService.GetProductWithExportOptions(_productId);
        }

        protected override void Validate()
        {
            if (_product == null)
                throw new BlException(T("Admin.Product.ExportOptions.Error.NotFound"));
        }

        protected override ProductExportOptionsModel Handle()
        {
            return new ProductExportOptionsModel()
            {
                ProductId = _product.ProductId,
                ExportOptions = _product.ExportOptions,
                IsMarkingRequired = _product.IsMarkingRequired
            };
        }
    }
}
