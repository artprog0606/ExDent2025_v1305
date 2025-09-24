using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class UpdateExportOptionsHandler : AbstractCommandHandler
    {
        private readonly ProductExportOptionsModel _model;
        private Product _product;

        public UpdateExportOptionsHandler(ProductExportOptionsModel model)
        {
            _model = model;
        }

        protected override void Load()
        {
            _product = ProductService.GetProductWithExportOptions(_model.ProductId);
        }

        protected override void Validate()
        {
            if (_product == null)
                throw new BlException(T("Admin.Product.ExportOptions.Error.NotFound"));
            
            if (_model.ExportOptions.YandexProductDiscounted)
            {
                if (_model.ExportOptions.YandexProductDiscounted && _model.ExportOptions.YandexProductDiscountCondition == EYandexDiscountCondition.None)
                    throw new BlException(T("Admin.Product.ExportOptions.Error.DiscountConditionRequired"));
                
                if (_model.ExportOptions.YandexProductDiscounted && _model.ExportOptions.YandexProductDiscountReason.IsNullOrEmpty())
                    throw new BlException(T("Admin.Product.ExportOptions.Error.DiscountReasonRequired"));
                
                if (_model.ExportOptions.YandexProductDiscounted && _model.ExportOptions.YandexProductDiscountReason.Length > 3000)
                    throw new BlException(T("Admin.Product.ExportOptions.Error.DiscountReasonLength"));
            }
        }

        protected override void Handle()
        {
            _product.ExportOptions = _model.ExportOptions;
            _product.ExportOptions.IsChanged = true;
            _product.IsMarkingRequired = _model.IsMarkingRequired;
            
            ProductService.UpdateProduct(_product, true, true);
        }
    }
}
