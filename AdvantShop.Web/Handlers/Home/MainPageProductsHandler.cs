using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ViewModel.Home;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Home
{
    public sealed class MainPageProductsHandler : ICommandHandler<MainPageProductsViewModel>
    {
        private readonly MainPageProductsViewModel _model = new MainPageProductsViewModel();
        private int _countNew;
        private readonly int _itemsCount = SettingsDesign.CountMainPageProductInSection;

        public MainPageProductsViewModel Execute()
        {
            GetCountNew();
            AddBaseProductLists();
            AddAdditionalProductLists();
            SortProductLists();
            return _model;
        }

        private void GetCountNew()
        {
            _countNew = SettingsCatalog.NewEnabled
                ? ProductOnMain.GetProductCountByType(EProductOnMain.New)
                : 0;

            if (_countNew == 0 
                && SettingsCatalog.NewEnabled 
                && SettingsCatalog.DisplayLatestProductsInNewOnMainPage)
            {
                _model.HideNewProductsLink = true;
                _model.NewArrivals = true;
            }
        }

        private void AddBaseProductLists()
        {
            _model.ProductLists.Add(PrepareProductModel(EProductOnMain.Best, _itemsCount));
            _model.ProductLists.Add(_countNew > 0
                ? PrepareProductModel(EProductOnMain.New, _itemsCount)
                : PrepareProductModel(EProductOnMain.NewArrivals, _itemsCount));
            _model.ProductLists.Add(PrepareProductModel(EProductOnMain.Sale, _itemsCount));
        }

        private void AddAdditionalProductLists()
        {
            var productLists = ProductListService.GetMainPageList();
            
            foreach (var productList in productLists)
            {
                var products = 
                    ProductListService.GetProducts(
                        productList.Id, 
                        _itemsCount,
                        WarehouseContext.GetAvailableWarehouseIds());
                
                if (products.Count > 0)
                {
                    var productListModel = new ProductViewModel(products)
                    {
                        Type = EProductOnMain.List,
                        Id = productList.Id,
                        Title = productList.Name,
                        CountProductsInLine = SettingsDesign.CountMainPageProductInLine,
                        ShowBriefDescription = SettingsMain.MainPageVisibleBriefDescription,
                        SortOrder = productList.SortOrder,
                        LinkText = LocalizationService.GetResource("Home.MainPageProducts.AllProducts"),
                        UrlPath = productList.UrlPath,
                    };
                    
                    _model.ProductLists.Add(productListModel);
                }
            }
        }
        
        private void SortProductLists() =>
            _model.ProductLists = _model.ProductLists
                .OrderBy(list => list.SortOrder)
                .ToList();
        
        private static ProductViewModel PrepareProductModel(EProductOnMain type, int itemsCount)
        {
            if (!CheckEnableBaseProductList(type))
                return new ProductViewModel() 
                {
                    ShowBriefDescription = SettingsMain.MainPageVisibleBriefDescription
                };

            var products = 
                ProductOnMain.GetProductsByType(
                    type, 
                    itemsCount,
                    WarehouseContext.GetAvailableWarehouseIds());
            
            var model = new ProductViewModel(products)
            {
                Type = type,
                CountProductsInLine = SettingsDesign.CountMainPageProductInLine,
                ShowBriefDescription = SettingsMain.MainPageVisibleBriefDescription,
                DisplayQuickView = false,
            };

            switch (type)
            {
                case EProductOnMain.Best:
                    model.Title = LocalizationService.GetResource("Home.MainPageProducts.BestSellersTitle");
                    model.SortOrder = SettingsCatalog.BestSorting;
                    model.LinkText = LocalizationService.GetResource("Home.MainPageProducts.BestSellersAllLink");
                    break;
                
                case EProductOnMain.New:
                case EProductOnMain.NewArrivals:
                    model.Title = LocalizationService.GetResource("Home.MainPageProducts.NewProductsTitle");
                    model.SortOrder = SettingsCatalog.NewSorting;
                    model.LinkText = LocalizationService.GetResource("Home.MainPageProducts.NewProductsAllLink");
                    break;
                
                case EProductOnMain.Sale:
                    model.Title = LocalizationService.GetResource("Home.MainPageProducts.SalesTitle");
                    model.SortOrder = SettingsCatalog.SalesSorting;
                    model.LinkText = LocalizationService.GetResource("Home.MainPageProducts.SalesAllLink");
                    break;
            }
            
            return model;
        }

        private static bool CheckEnableBaseProductList(EProductOnMain type)
        {
            switch (type)
            {
                case EProductOnMain.Best:
                    return SettingsCatalog.BestEnabled && SettingsCatalog.ShowBestOnMainPage;
                case EProductOnMain.New:
                    return SettingsCatalog.NewEnabled && SettingsCatalog.ShowNewOnMainPage;
                case EProductOnMain.NewArrivals:
                    return SettingsCatalog.NewEnabled && SettingsCatalog.DisplayLatestProductsInNewOnMainPage && SettingsCatalog.ShowNewOnMainPage;
                case EProductOnMain.Sale:
                    return SettingsCatalog.SalesEnabled && SettingsCatalog.ShowSalesOnMainPage;
                default:
                    return false;
            } 
        }
    }
}
