using System.Web;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Categories;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Catalog;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKey]
    public class CategoriesController : BaseApiController
    {
        // GET api/categories/{id}
        [HttpGet]
        public JsonResult Get(int id) => JsonApi(new GetCategory(id));

        // GET api/categories
        [HttpGet]
        public JsonResult Filter(CategoriesFilterModel filter) => JsonApi(new GetCategories(filter));

        // POST api/categories/add
        [HttpPost]
        public JsonResult Add(AddUpdateCategoryModel model) => JsonApi(new AddUpdateCategory(model));

        // POST api/categories/{id}
        [HttpPost]
        public JsonResult Update(int id, AddUpdateCategoryModel model) => JsonApi(new AddUpdateCategory(id, model));

        // POST api/categories/{id}/delete
        [HttpPost]
        public JsonResult Delete(int id) => JsonApi(new DeleteCategory(id));

        
        #region Pictures

        // POST api/categories/{id}/picture/add
        [HttpPost]
        public JsonResult PictureAdd(int id) => JsonApi(new AddPicture(id, PhotoType.CategoryBig));

        // POST api/categories/{id}/picture/addbylink
        [HttpPost]
        public JsonResult PictureAddByLink(int id, string link) =>
            JsonApi(new AddPictureByLink(id, PhotoType.CategoryBig, link));
        
        // POST api/categories/{id}/picture/delete
        [HttpPost]
        public JsonResult PictureDelete(int id) => JsonApi(new DeletePicture(id, PhotoType.CategoryBig));


        // POST api/categories/{id}/mini-picture/add 
        [HttpPost]
        public JsonResult MiniPictureAdd(int id) => JsonApi(new AddPicture(id, PhotoType.CategorySmall));

        // POST api/categories/{id}/mini-picture/addbylink
        [HttpPost]
        public JsonResult MiniPictureAddByLink(int id, string link) =>
            JsonApi(new AddPictureByLink(id, PhotoType.CategorySmall, link));

        // POST api/categories/{id}/picture/delete 
        [HttpPost]
        public JsonResult MiniPictureDelete(int id) => JsonApi(new DeletePicture(id, PhotoType.CategorySmall));

        
        // POST api/categories/{id}/menu-icon-picture/add 
        [HttpPost]
        public JsonResult MenuIconPictureAdd(int id) => JsonApi(new AddPicture(id, PhotoType.CategoryIcon));

        // POST api/categories/{id}/menu-icon-picture/addbylink
        [HttpPost]
        public JsonResult MenuIconPictureAddByLink(int id, string link) =>
            JsonApi(new AddPictureByLink(id, PhotoType.CategoryIcon, link));
        
        // POST api/categories/{id}/menu-icon-picture/delete 
        [HttpPost]
        public JsonResult MenuIconPictureDelete(int id) => JsonApi(new DeletePicture(id, PhotoType.CategoryIcon));

        #endregion
    }
}