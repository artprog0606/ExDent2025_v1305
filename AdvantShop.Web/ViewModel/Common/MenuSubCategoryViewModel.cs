using System.Collections.Generic;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Configuration;

namespace AdvantShop.ViewModel.Common
{
    public class MenuSubCategoryViewModel
    {
        public MenuSubCategoryViewModel(MenuViewModel parentModel, MenuItemModel item)
        {
            MenuItem = item;
            ViewMode = parentModel.ViewMode;
            CountColsProductsInRow = parentModel.CountColsProductsInRow;
            DisplayProductsCount = parentModel.DisplayProductsCount;
        }
        
        public MenuItemModel MenuItem { get; set; }

        public SettingsDesign.eMenuStyle ViewMode { get; set; }

        public int CountColsProductsInRow { get; set; }
        public bool DisplayProductsCount { get; set; }
    }
}
