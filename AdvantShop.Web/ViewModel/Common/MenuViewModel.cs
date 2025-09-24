using System.Collections.Generic;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Configuration;

namespace AdvantShop.ViewModel.Common
{
    public class MenuViewModel
    {
        public List<MenuItemModel> MenuItems { get; set; } = new List<MenuItemModel>();
        public string RootUrlPath { get; set; }
        public bool MenuItemsLimited { get; set; }

        public int SelectedItemId { get; set; }

        public bool IsExpanded { get; set; }

        public bool InLayout { get; set; }

        public SettingsDesign.eMenuStyle ViewMode { get; set; }

        public bool DisplayProductsCount { get; set; }

        public bool IsСlickability => 
            ViewMode == SettingsDesign.eMenuStyle.Accordion || ViewMode == SettingsDesign.eMenuStyle.Treeview;

        public int CountColsProductsInRow { get; set; }
        
        public bool LimitedCategoryMenu { get; set; }

        public bool isMainMenu { get; set; }
    }
}