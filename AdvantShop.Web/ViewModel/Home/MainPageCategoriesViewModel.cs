using AdvantShop.Catalog;
using AdvantShop.Configuration;
using System.Collections.Generic;

namespace AdvantShop.ViewModel.Home
{
    public class MainPageCategoriesViewModel
    {
        public List<Category> Categories { get; set; }
        public readonly int CountMainPageCategoriesInLine;

        public MainPageCategoriesViewModel()
        {
            Categories = new List<Category>();
            CountMainPageCategoriesInLine = SettingsDesign.CountMainPageCategoriesInLine;
        }
    }
}