using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Handlers.Catalog;
using AdvantShop.Models.Catalog;
using CategoryModel = AdvantShop.Models.Catalog.CategoryModel;

namespace AdvantShop.Areas.Api.Handlers.Catalogs
{
    public class GetCatalogFilterApi : AbstractCommandHandler<GetCatalogFilterResponse>
    {
        private readonly CatalogFilter _filter;
        private Category _category;
    
        public GetCatalogFilterApi(CatalogFilter filter)
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

        protected override GetCatalogFilterResponse Handle()
        {
            if (_category.CategoryId == 0)
                return null;

            var categoryModel = (CategoryModel) _filter;

            var paging = new CategoryProductPagingHandler(_category, false, categoryModel, false).GetForFilter();

            var filter = paging.Filter;

            var tasks = new List<Task<List<FilterItemModel>>>();

            if (SettingsCatalog.ShowPriceFilter && !SettingsCatalog.HidePrice)
            {
                tasks.Add(new FilterPriceHandler(filter.CategoryId, filter.Indepth, filter.PriceFrom, filter.PriceTo)
                    .GetAsync());
            }

            if (SettingsCatalog.ShowProducerFilter)
            {
                tasks.Add(new FilterBrandHandler(filter.CategoryId, filter.Indepth, filter.BrandIds,
                    filter.AvailableBrandIds).GetAsync());
            }

            if (SettingsCatalog.ShowColorFilter)
            {
                tasks.Add(new FilterColorHandler(filter.CategoryId, filter.Indepth, filter.ColorIds,
                    filter.AvailableColorIds, SettingsCatalog.ShowOnlyAvalible || filter.Available, filter.ColorsViewMode).GetAsync());
            }

            if (SettingsCatalog.ShowSizeFilter)
            {
                tasks.Add(new FilterSizeHandler(filter.CategoryId, filter.Indepth, filter.SizeIds,
                    filter.AvailableSizeIds, SettingsCatalog.ShowOnlyAvalible || filter.Available).GetAsync());
            }

            if (SettingsCatalog.ShowWarehouseFilter)
            {
                tasks.Add(new FilterWarehouseHandler(filter.CategoryId, filter.Indepth, filter.WarehouseIds,
                    filter.AvailableWarehouseIds).GetAsync());
            }
            
            if (SettingsCatalog.ShowPropertiesFilterInParentCategories)
            {
                tasks.Add(new FilterPropertyHandler(filter.CategoryId, filter.Indepth, filter.PropertyIds, filter.AvailablePropertyIds, filter.RangePropertyIds).GetAsync());
            }
            else
            {
                var hasNoSubCategories =
                    CategoryService.GetChildCategoriesByCategoryId(filter.CategoryId, warehouseIds: WarehouseContext.GetAvailableWarehouseIds())
                        .Where(cat => cat.Enabled && !cat.Hidden)
                        .ToList()
                        .Count == 0;

                if (hasNoSubCategories)
                    tasks.Add(new FilterPropertyHandler(filter.CategoryId, filter.Indepth, filter.PropertyIds,
                        filter.AvailablePropertyIds, filter.RangePropertyIds).GetAsync());
            }

            if (SettingsCatalog.AvaliableFilterEnabled)
            {
                tasks.Add(new FilterAvailabilityHandler(filter.Available).GetAsync());
            }

            var result = tasks.Select(x => x.Result).SelectMany(x => x).Where(x => x != null).ToList();

            return new GetCatalogFilterResponse(result);
        }
    }
}