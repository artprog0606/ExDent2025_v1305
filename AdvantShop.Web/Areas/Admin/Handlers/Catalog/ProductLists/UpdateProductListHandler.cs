using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ProductLists
{
    public class UpdateProductListHandler : ICommandHandler<int>
    {
        private ProductList _list;
        
        public UpdateProductListHandler(ProductList list) =>
            _list = list;

        public int Execute()
        {
            Validate();
            CheckUrlPath();
            AddMeta();
            Update();
            return _list.Id;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(_list.Name))
                throw new BlException("Отсутствует название списка товаров");
            
            if (_list.Id == 0)
                throw new BlException("Не удалось получить идентификатор списка");

            var oldList = ProductListService.Get(_list.Id);
            
            if (oldList == null)
                throw new BlException("Не удалось найти список товаров");
            
            if (string.IsNullOrEmpty(_list.UrlPath))
                throw new BlException("Отсутствует синоним для URL запроса");
        }

        private void CheckUrlPath()
        {
            if (UrlService.GetObjUrlFromDb(ParamType.ProductList, _list.Id) != _list.UrlPath 
                && !UrlService.IsValidUrl(_list.UrlPath, ParamType.ProductList))
            {
                _list.UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.ProductList, _list.UrlPath);
            }
        }

        private void AddMeta() =>
            _list.Meta = new MetaInfo(0, _list.Id, MetaType.MainPageProducts,
                _list.Meta.Title.DefaultOrEmpty(), _list.Meta.MetaKeywords.DefaultOrEmpty(),
                _list.Meta.MetaDescription.DefaultOrEmpty(),
                _list.Meta.H1.DefaultOrEmpty());
        
        private void Update() =>
            ProductListService.Update(_list);
    }
}