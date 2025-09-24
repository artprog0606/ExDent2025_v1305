using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Models.Catalog;

namespace AdvantShop.Handlers.Catalog
{
    public class FilterSizeHandler
    {
        #region Fields

        private readonly int _categoryId;
        private readonly bool _indepth;

        private readonly bool _onlyAvailable;

        private readonly List<int> _selectedSizeIds;
        private readonly List<int> _availableSizeIds;
        private readonly List<int> _warehouseIds;
        private readonly EProductOnMain? _productOnMainType;
        private readonly int? _productListId;
        private readonly List<int> _productIds;

        #endregion

        #region Constructor

        public FilterSizeHandler(int categoryId, bool indepth, 
                                    List<int> selectedSizeIds, List<int> availableSizeIds, bool onlyAvailable) 
                                    : this(selectedSizeIds, availableSizeIds, onlyAvailable)
        {
            _categoryId = categoryId;
            _indepth = indepth;
        }
        
        public FilterSizeHandler(EProductOnMain type, int? productListId,
                                    List<int> selectedSizeIds, List<int> availableSizeIds, bool onlyAvailable) 
                                    : this(selectedSizeIds, availableSizeIds, onlyAvailable)
        {
            _productOnMainType = type;
            _productListId = productListId;
        }
        
        public FilterSizeHandler(List<int> productIds, 
                                    List<int> selectedSizeIds, List<int> availableSizeIds, bool onlyAvailable) 
                                    : this(selectedSizeIds, availableSizeIds, onlyAvailable)
        {
            _productIds = productIds;
        }
        
        public FilterSizeHandler(List<int> selectedSizeIds, List<int> availableSizeIds, bool onlyAvailable)
        {
            _selectedSizeIds = selectedSizeIds ?? new List<int>();
            _availableSizeIds = availableSizeIds;
            _onlyAvailable = onlyAvailable;
            
            _warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
        }

        #endregion

        public FilterItemModel Get()
        {
            var sizeItems =
                _productOnMainType is null && _productListId is null && _productIds is null
                    ? SizeService.GetSizesByCategoryId(_categoryId, _indepth, _onlyAvailable, _warehouseIds)
                    : SizeService.GetSizesByFilter(_productOnMainType, _productListId, _productIds, _onlyAvailable, _warehouseIds);
            
            if (sizeItems.Count == 0)
                return null;
            
            var sizes = 
                sizeItems.Select(x => new FilterSize()
                    {
                        SizeId = x.SizeId,
                        SizeName = x.GetFullName(),
                        SortOrder = x.SortOrder,
                        Checked = _selectedSizeIds.Contains(x.SizeId)
                    }).ToList();

            var model = new FilterItemModel()
            {
                Expanded = true,
                Type = "size",
                Title = SettingsCatalog.SizesHeader,
                Subtitle = "",
                Control = "checkbox"
            };

            foreach (var size in sizes)
            {
                model.Values.Add(new FilterListItemModel()
                {
                    Id = size.SizeId.ToString(),
                    Text = size.SizeName,
                    Selected = size.Checked,
                    Available = _availableSizeIds == null || _availableSizeIds.Contains(size.SizeId) || _selectedSizeIds.Count > 0
                });
            }

            return model;
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