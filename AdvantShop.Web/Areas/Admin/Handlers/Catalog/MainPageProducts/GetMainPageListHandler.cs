using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog.MainPageProducts;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.MainPageProducts
{
    public class GetMainPageListHandler : ICommandHandler<MainPageProductsModel>
    {
        private MainPageProductsModel _model;
        
        private readonly EProductOnMain _type;
        
        public GetMainPageListHandler(EProductOnMain type) =>
            _type = type;

        public MainPageProductsModel Execute()
        {
            _model = new MainPageProductsModel
            {
                Type = _type
            };

            switch (_type)
            {
                case EProductOnMain.Best:
                    _model.Description = SettingsCatalog.BestDescription;
                    _model.Enabled = SettingsCatalog.BestEnabled;
                    _model.ShowOnMainPage = SettingsCatalog.ShowBestOnMainPage;
                    _model.ShuffleList = SettingsCatalog.ShuffleBestOnMainPage;
                    _model.Sorting = SettingsCatalog.BestSorting;
                    break;

                case EProductOnMain.New:
                    _model.Description = SettingsCatalog.NewDescription;
                    _model.Enabled = SettingsCatalog.NewEnabled;
                    _model.ShowOnMainPage = SettingsCatalog.ShowNewOnMainPage;
                    _model.DisplayLatestProductsInNewOnMainPage = SettingsCatalog.DisplayLatestProductsInNewOnMainPage;
                    _model.ShuffleList = SettingsCatalog.ShuffleNewOnMainPage;
                    _model.Sorting = SettingsCatalog.NewSorting;
                    break;

                case EProductOnMain.Sale:
                    _model.Description = SettingsCatalog.DiscountDescription;
                    _model.Enabled = SettingsCatalog.SalesEnabled;
                    _model.ShowOnMainPage = SettingsCatalog.ShowSalesOnMainPage;
                    _model.ShuffleList = SettingsCatalog.ShuffleSalesOnMainPage;
                    _model.Sorting = SettingsCatalog.SalesSorting;
                    break;
            }

            SetMeta();

            return _model;
        }

        private void SetMeta()
        {
            var meta = MetaInfoService.GetMetaInfo(-1 * (int) _type, MetaType.MainPageProducts) ?? new MetaInfo();
            
            _model.H1 = meta.H1;
            _model.Title = meta.Title;
            _model.MetaKeywords = meta.MetaKeywords;
            _model.MetaDescription = meta.MetaDescription;
            _model.UseDefaultMeta = meta.ObjId == 0;
        }
    }
}