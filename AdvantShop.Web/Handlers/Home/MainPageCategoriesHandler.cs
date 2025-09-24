using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.ViewModel.Home;

namespace AdvantShop.Handlers.Home
{
    public class MainPageCategoriesHandler
    {
        public MainPageCategoriesViewModel Get()
        {
            var model = new MainPageCategoriesViewModel();

            var itemsCount = SettingsDesign.CountMainPageCategoriesInSection;

            model.Categories = 
                CategoryService.GetCategoriesOnMainPage(itemsCount, WarehouseContext.GetAvailableWarehouseIds());

            return model;
        }
    }
}