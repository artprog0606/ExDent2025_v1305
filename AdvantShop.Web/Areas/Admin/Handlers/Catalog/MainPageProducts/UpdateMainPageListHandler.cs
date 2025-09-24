using System;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog.MainPageProducts;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.MainPageProducts
{
    public class UpdateMainPageListHandler : ICommandHandler
    {
        private readonly MainPageProductsModel _model;
        
        public UpdateMainPageListHandler(MainPageProductsModel model) =>
            _model = model;
        
        public void Execute()
        {
            try
            {
                UpdateMainPageProduct();
                UpdateMeta();
            }
            catch (Exception)
            {
                throw new BlException(LocalizationService.GetResource("Admin.Catalog.MainPageProducts.Update.Error"));
            }
        }

        private void UpdateMainPageProduct()
        {
            switch (_model.Type)
            {
                case EProductOnMain.Best: 
                    SettingsCatalog.BestDescription = _model.Description ?? "";
                    SettingsCatalog.BestEnabled = _model.Enabled;
                    SettingsCatalog.ShowBestOnMainPage = _model.ShowOnMainPage;
                    SettingsCatalog.ShuffleBestOnMainPage = _model.ShuffleList;
                    SettingsCatalog.BestSorting = _model.Sorting;
                    break;
                
                case EProductOnMain.New:
                    SettingsCatalog.NewDescription = _model.Description ?? "";
                    SettingsCatalog.NewEnabled = _model.Enabled;
                    SettingsCatalog.ShowNewOnMainPage = _model.ShowOnMainPage;
                    SettingsCatalog.DisplayLatestProductsInNewOnMainPage = _model.DisplayLatestProductsInNewOnMainPage ?? false;
                    SettingsCatalog.ShuffleNewOnMainPage = _model.ShuffleList;
                    SettingsCatalog.NewSorting = _model.Sorting;
                    break;
                
                case EProductOnMain.Sale:
                    SettingsCatalog.DiscountDescription = _model.Description ?? "";
                    SettingsCatalog.SalesEnabled = _model.Enabled;
                    SettingsCatalog.ShowSalesOnMainPage = _model.ShowOnMainPage;
                    SettingsCatalog.ShuffleSalesOnMainPage = _model.ShuffleList;
                    SettingsCatalog.SalesSorting = _model.Sorting;
                    break;
            }
        }

        private void UpdateMeta()
        {
            if (_model.UseDefaultMeta 
                || (_model.Title.IsNullOrEmpty()
                    && _model.MetaKeywords.IsNullOrEmpty()
                    && _model.MetaDescription.IsNullOrEmpty()
                    && _model.H1.IsNullOrEmpty()))
            {
                if (MetaInfoService.IsMetaExist(-1*(int) _model.Type, MetaType.MainPageProducts))
                    MetaInfoService.DeleteMetaInfo(-1*(int) _model.Type, MetaType.MainPageProducts);
            }
            else
                MetaInfoService.SetMeta(new MetaInfo(
                    0, 
                    -1*(int) _model.Type, 
                    MetaType.MainPageProducts, 
                    _model.Title, 
                    _model.MetaKeywords, 
                    _model.MetaDescription, 
                    _model.H1));
        }
    }
}