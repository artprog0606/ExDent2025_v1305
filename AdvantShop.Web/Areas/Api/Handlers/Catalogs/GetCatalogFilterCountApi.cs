using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Handlers.Catalog;
using CategoryModel = AdvantShop.Models.Catalog.CategoryModel;

namespace AdvantShop.Areas.Api.Handlers.Catalogs
{
    public class GetCatalogFilterCountApi : AbstractCommandHandler<GetCatalogFilterCountResponse>
    {
        private readonly CatalogFilter _filter;
        private Category _category;
    
        public GetCatalogFilterCountApi(CatalogFilter filter)
        {
            _filter = filter;
        }

        protected override void Load()
        {
            _category = _filter.CategoryId != null && _filter.CategoryId.Value >= 0
                ? CategoryService.GetCategory(_filter.CategoryId.Value)
                : !string.IsNullOrWhiteSpace(_filter.Url) ? CategoryService.GetCategory(_filter.Url) : null;
        }

        protected override void Validate()
        {
            if (_category == null)
                throw new BlException("Категория не найдена");
        }

        protected override GetCatalogFilterCountResponse Handle()
        {
            if (_category.CategoryId == 0)
                return null;

            var categoryModel = (CategoryModel) _filter;

            var paging = new CategoryProductPagingHandler(_category, false, categoryModel, false).GetForFilterProductCount();

            if (paging == null || paging.Pager == null)
                throw new BlException("Ошибка при фильтрации");


            return new GetCatalogFilterCountResponse(paging.Pager.TotalItemsCount);
        }
    }
}