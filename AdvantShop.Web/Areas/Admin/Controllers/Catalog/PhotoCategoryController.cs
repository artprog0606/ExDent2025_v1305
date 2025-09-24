using System;
using System.Web.Mvc;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.PhotoCategory;
using AdvantShop.Web.Admin.Models.Catalog.PhotoCategory;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class PhotoCategoryController : BaseAdminController
    {
        public JsonResult GetList(PhotoCategoryFilterModel model)
        {
            return Json(new GetPhotoCategoriesHandler(model).Execute());
        }

        #region Commands

        private void Command(PhotoCategoryFilterModel command, Action<int, PhotoCategoryFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetPhotoCategoriesHandler(command).GetItemsIds("Id");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePhotoCategories(PhotoCategoryFilterModel command)
        {
            Command(command, (id, c) => PhotoCategoryService.Delete(id));
            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            PhotoCategoryService.Delete(id);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(PhotoCategoryModel model)
        {
            var photoCategory = PhotoCategoryService.Get(model.Id);
            if (photoCategory == null)
                return Json(new {result = false});

            photoCategory.Name = model.Name;
            photoCategory.SortOrder = model.SortOrder;
            photoCategory.Enabled = model.Enabled;
            PhotoCategoryService.Update(photoCategory);

            return JsonOk();
        }

        #region Add | Update

        [HttpGet]
        public JsonResult Get(int id)
        {
            var photoCategory = PhotoCategoryService.Get(id);
            return JsonOk(new PhotoCategoryModel
            {
                Enabled = photoCategory.Enabled,
                Id = id,
                Name = photoCategory.Name,
                SortOrder = photoCategory.SortOrder
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(PhotoCategoryModel model)
        {
            PhotoCategoryService.Add(new PhotoCategory
            {
                Enabled = model.Enabled,
                Name = model.Name,
                SortOrder = model.SortOrder
            });
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(PhotoCategoryModel model)
        {
            PhotoCategoryService.Update(new PhotoCategory
            {
                Id = model.Id,
                Enabled = model.Enabled,
                Name = model.Name,
                SortOrder = model.SortOrder
            });
            return JsonOk();
        }
        
        #endregion
    }
}
