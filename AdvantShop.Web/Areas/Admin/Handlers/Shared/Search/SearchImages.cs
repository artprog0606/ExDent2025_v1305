using System;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Shared.Search;

namespace AdvantShop.Web.Admin.Handlers.Shared.Search
{
    public class SearchImages
    {
        #region Ctor

        private readonly int _objId;
        private readonly PhotoType _type;
        private readonly int _page;

        public SearchImages(int objId, PhotoType type, int page)
        {
            _objId = objId;
            _type = type;
            _page = page;
        }

        #endregion

        public ImageFindResponse Execute()
        {
            var term = "";

            switch (_type)
            {
                case PhotoType.Product:
                    var product = ProductService.GetProduct(_objId);
                    term = product != null ? product.Name : null;
                    break;
                case PhotoType.CategoryIcon:
                case PhotoType.CategorySmall:
                case PhotoType.CategoryBig:
                    var category = CategoryService.GetCategory(_objId);
                    term = category != null ? category.Name : null;
                    break;

                case PhotoType.Brand:
                    var brand = BrandService.GetBrandById(_objId);
                    term = brand != null ? brand.Name : null;
                    break;
                case PhotoType.News:
                    var news = AdvantShop.News.NewsService.GetNewsById(_objId);
                    term = news != null ? news.Title : null;
                    break;
                default:
                    throw new BlException("Wrong type for module BingImagesSearch");
            }

            if (string.IsNullOrWhiteSpace(term))
                throw new BlException("Укажите название для поиска");

            if (!CustomerContext.CurrentCustomer.IsAdmin && !(CustomerContext.CurrentCustomer.IsModerator
                && CustomerContext.CurrentCustomer.HasRoleAction(RoleAction.Catalog)))
            {
                throw new BlException("Доступ запрещен");
            }

            return Search(term, _page);
        }

        private ImageFindResponse Search(string term, int page)
        {
            try
            {
                var url = string.Format("{0}v1/image/find", SettingsLic.ImageServiceUrl);
                var data = new
                {
                    term = term,
                    licKey = SettingsLic.LicKey,
                    page = page
                };

                var response = RequestHelper.MakeRequest<ImageFindResponse>(url, data, method: ERequestMethod.POST, contentType: ERequestContentType.TextJson);

                return response;
            }
            catch (BlException ex)
            {
                throw new BlException(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }
    }
}
