using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.FilePath;
using AdvantShop.Models.Catalog;

namespace AdvantShop.Handlers.Catalog
{
    public class FilterColorHandler
    {
        #region Fields

        private readonly int _categoryId;
        private readonly bool _indepth;
        private readonly bool _onlyAvailable;

        private readonly List<int> _selectedColorIds;
        private readonly List<int> _availableColorIds;

        private readonly string _colorsViewMode;
        private readonly List<int> _warehouseIds;
        
        private readonly EProductOnMain? _productOnMainType;
        private readonly int? _productListId;
        private readonly List<int> _productIds;

        #endregion

        #region Constructor
        
        public FilterColorHandler(int categoryId, bool indepth, 
                                    List<int> selectedColorIds, List<int> availableColorIds, bool onlyAvailable, 
                                    ColorsViewMode colorsViewMode = ColorsViewMode.Icon) : this(selectedColorIds, availableColorIds, onlyAvailable, colorsViewMode)
        {
            _categoryId = categoryId;
            _indepth = indepth;
        }

        public FilterColorHandler(EProductOnMain type, int? productListId,
                                    List<int> selectedColorIds, List<int> availableColorIds, bool onlyAvailable, 
                                    ColorsViewMode colorsViewMode) : this(selectedColorIds, availableColorIds, onlyAvailable, colorsViewMode)
        {
            _productOnMainType = type;
            _productListId = productListId;
        }
        
        public FilterColorHandler(List<int> productIds, 
                                    List<int> selectedColorIds, List<int> availableColorIds, bool onlyAvailable, 
                                    ColorsViewMode colorsViewMode) : this(selectedColorIds, availableColorIds, onlyAvailable, colorsViewMode)
        {
            _productIds = productIds;
        }
        
        public FilterColorHandler(List<int> selectedColorIds, List<int> availableColorIds, bool onlyAvailable, ColorsViewMode colorsViewMode)
        {
            _selectedColorIds = selectedColorIds ?? new List<int>();
            _availableColorIds = availableColorIds;
            _onlyAvailable = onlyAvailable;
            _colorsViewMode = colorsViewMode.ToString().ToLower();
            
            _warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
        }

        #endregion

        public FilterItemModel Get()
        {
            var colorItems =
                _productOnMainType is null && _productListId is null && _productIds is null
                    ? ColorService.GetColorsByCategoryId(_categoryId, _indepth, _onlyAvailable, _warehouseIds)
                    : ColorService.GetColorsByFilter(_productOnMainType, _productListId, _productIds, _onlyAvailable, _warehouseIds);

            if (colorItems.Count == 0)
                return null;

            var colors =
                colorItems.Select(x => new FilterColor()
                {
                    ColorId = x.ColorId,
                    ColorName = x.ColorName,
                    ColorCode = x.ColorCode,
                    IconFileName = x.IconFileName,
                    SortOrder = x.SortOrder,
                    Checked = _selectedColorIds.Contains(x.ColorId)
                }).ToList();
            
            var model = new FilterItemModel()
            {
                Expanded = true,
                Type = "color",
                Title = SettingsCatalog.ColorsHeader,
                Subtitle = "",
                Control = "color"
            };

            foreach (var color in colors)
            {
                model.Values.Add(new
                {
                    color.ColorName,
                    color.ColorCode,
                    Selected = color.Checked,
                    Text = color.ColorName,
                    Id = color.ColorId,
                    Available = _availableColorIds == null || _availableColorIds.Contains(color.ColorId) || _selectedColorIds.Count > 0,
                    ImageHeight = color.IconFileName.ImageHeightCatalog,
                    ImageWidth = color.IconFileName.ImageWidthCatalog,
                    color.IconFileName.PhotoName,
                    ImageSrc = color.IconFileName.ImageSrc(ColorImageType.Catalog),
                    ColorsViewMode = _colorsViewMode
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