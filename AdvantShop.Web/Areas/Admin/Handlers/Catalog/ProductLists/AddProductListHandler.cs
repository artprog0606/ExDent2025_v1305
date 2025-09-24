using System;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ProductLists
{
    public class AddProductListHandler : ICommandHandler<int>
    {
        private ProductList _list;
        
        public AddProductListHandler(ProductList list) =>
            _list = list;

        public int Execute()
        {
            try
            {
                Validate();
                CheckUrlPath();
                AddMeta();
                return ProductListService.Add(_list);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException(ex.Message);
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(_list.Name))
                throw new BlException("Отсутствует название списка товаров");
            
            if (string.IsNullOrEmpty(_list.UrlPath))
                throw new BlException("Отсутствует синоним для URL запроса");
        }

        private void CheckUrlPath()
        {
            if (!UrlService.IsValidUrl(_list.UrlPath, ParamType.ProductList))
            {
                _list.UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.ProductList, _list.UrlPath);
            }
        }
        
        private void AddMeta() =>
            _list.Meta = new MetaInfo(0, _list.Id, MetaType.MainPageProducts,
                _list.Meta.Title.DefaultOrEmpty(), _list.Meta.MetaKeywords.DefaultOrEmpty(),
                _list.Meta.MetaDescription.DefaultOrEmpty(),
                _list.Meta.H1.DefaultOrEmpty());
    }
}