using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Models.Catalog;

namespace AdvantShop.Handlers.Catalog
{
    public class FilterBrandHandler
    {
        #region Fields

        private readonly int _categoryId;
        private readonly bool _indepth;
        private readonly EProductOnMain _flag;
        private readonly int? _listId;
        private readonly bool _onlyAvailable;
        private readonly List<int> _productIds;

        private readonly List<int> _selectedBrandIds;
        private readonly List<int> _availableBrandIds;
        private readonly List<int> _warehouseIds;

        #endregion

        #region Constructor

        public FilterBrandHandler(int categoryId, bool indepth, List<int> selectedBrandIds, List<int> availableBrandIds,
                                    EProductOnMain flag = EProductOnMain.None, int? listId = null, bool showOnlyAvailable = false, 
                                    List<int> productIds = null)
        {
            _categoryId = categoryId;
            _indepth = indepth;
            _flag = flag;
            _selectedBrandIds = selectedBrandIds ?? new List<int>();
            _availableBrandIds = availableBrandIds;
            _listId = listId;
            _onlyAvailable = showOnlyAvailable;
            _productIds = productIds;
            _warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
        }

        #endregion

        public FilterItemModel Get()
        {
            var brands = _flag == EProductOnMain.None && _productIds == null

                ? CacheManager.Get(CacheNames.BrandsInCategoryCacheName(_categoryId, _indepth, SettingsCatalog.ShowOnlyAvalible, _warehouseIds),
                    () => BrandService.GetBrandsByCategoryId(_categoryId, _indepth, SettingsCatalog.ShowOnlyAvalible, _warehouseIds))

                : CacheManager.Get(CacheNames.BrandsByFilterCacheName((int) _flag, _listId ?? 0, _productIds, _onlyAvailable, _warehouseIds),
                    () => BrandService.GetBrandsByFilter(_flag, _listId, _productIds, _onlyAvailable, _warehouseIds));

            if (brands == null || brands.Count == 0)
                return null;

            var filterBrand = new FilterItemModel()
            {
                Expanded = true,
                Type = "brand",
                Title = LocalizationService.GetResource("Catalog.FilterBrand.Brands"),
                Subtitle = "",
                Control = "checkbox"
            };

            foreach (var brand in brands)
            {
                if (_onlyAvailable && !(_availableBrandIds == null || _availableBrandIds.Contains(brand.BrandId)))
                    continue;

                var selected = _selectedBrandIds.Contains(brand.BrandId);

                filterBrand.Values.Add(new FilterListItemModel()
                {
                    Id = brand.BrandId.ToString(),
                    Text = brand.Name,
                    Selected = selected,
                    Available = selected || _availableBrandIds == null || _availableBrandIds.Contains(brand.BrandId) || _selectedBrandIds.Count > 0
                });
            }

            if (filterBrand.Values == null || filterBrand.Values.Count == 0)
                return null;

            return filterBrand;
        }

        public Task<List<FilterItemModel>> GetAsync()
        {
            return Task.Run(() =>
            {
                Localization.Culture.InitializeCulture();

                return new List<FilterItemModel> {Get()};
            });
        }
    }
}