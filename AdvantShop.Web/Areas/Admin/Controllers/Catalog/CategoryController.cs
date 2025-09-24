using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.Categories;
using AdvantShop.Web.Admin.Models.Catalog.Categories;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public partial class CategoryController : BaseAdminController
    {
        public ActionResult Add(int? parentId)
        {
            var model = new AdminCategoryModel()
            {
                IsEditMode = false,
                ParentCategoryId = parentId != null ? parentId.Value : 0,
                DisplayStyle = ECategoryDisplayStyle.Tile,
                Sorting = ESortOrder.NoSorting,
                DefaultMeta = true,
                Enabled = true
            };

            var parentCategory = CategoryService.GetCategory(model.ParentCategoryId);
            if (parentCategory != null)
                model.ParentCategoryName = parentCategory.Name;

            if (parentId.HasValue)
            {
                model.BreadCrumbs =
                    CategoryService.GetParentCategories((int)parentId)
                        .Select(x => new BreadCrumbs(x.Name, new UrlHelper(Request.RequestContext).AbsoluteActionUrl("Index", "Catalog", new { categoryId = x.CategoryId })))
                        .Reverse()
                        .ToList();
            }

            SetMetaInformation(T("Admin.Catalog.Index.CategoryTitle"));
            SetNgController(NgControllers.NgControllersTypes.CategoryCtrl);

            return View("Index", model);
        }

        public ActionResult Edit(int id)
        {
            var category = CategoryService.GetCategory(id);
            if (category == null)
                return Error404();

            var model = new GetCategoryModel(category).Execute();

            SetMetaInformation(T("Admin.Catalog.Index.CategoryTitle") + " - " + category.Name);
            SetNgController(NgControllers.NgControllersTypes.CategoryCtrl);

            return View("Index", model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(AdminCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var handler = new AddUpdateCategoryHandler(model);
                var categoryId = handler.Execute();

                if (categoryId != 0)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Index", "Catalog", new { categoryId });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Catalog.Index.CategoryTitle"));
            SetNgController(NgControllers.NgControllersTypes.CategoryCtrl);

            return View("Index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(AdminCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new AddUpdateCategoryHandler(model).Execute();
                if (result != -1)
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new { id = model.CategoryId });
                }
            }

            ShowErrorMessages();

            SetMetaInformation(T("Admin.Catalog.Index.CategoryTitle") + " - " + model.Name);
            SetNgController(NgControllers.NgControllersTypes.CategoryCtrl);

            return View("Index", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var needRedirect = false;
            var deleteCategory = CategoryService.GetCategory(id);
            try
            {

                if (id == -1)
                {
                    return new JsonResult { Data = new { result = false, needRedirect = false, id = id } };
                }
                CategoryService.DeleteCategoryAndPhotos(id);
                CategoryService.DeleteCategoryLink(id);
                CategoryService.RecalculateProductsCountManual();
                CategoryService.CalculateHasProductsForAllWarehouseInAllCategories();
                needRedirect = true;

            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return new JsonResult { Data = new { result = true, needRedirect = needRedirect, id = deleteCategory.ParentCategoryId } };
        }

        #region Picture

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPicture(HttpPostedFileBase file, PhotoType type, int? objId)
        {
            var result = new UploadCategoryPictures(file, type, objId).Execute();
            return result.Result
                ? JsonOk(new {picture = result.Picture, pictureId = result.PictureId})
                : JsonError(result.Error);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePicture(int pictureId)
        {
            var result = new DeleteCategoryPictureHandler(pictureId).Execute();
            return result.Result
                ? JsonOk(new {picture = result.Picture})
                : JsonError(result.Error ?? T("Admin.Catalog.ErrorDeletingImage"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadPictureByLink(PhotoType type, int? objId, string fileLink)
        {
            var result = new UploadCategoryPicturesByLink(type, objId, fileLink).Execute();
            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        #endregion

        public JsonResult GetTags(int categoryId)
        {
            return Json(new
            {
                tags = TagService.GetAutocompleteTags().Select(x => new {value = x.Name, id = x.Id}),
                selectedTags = TagService.Gets(categoryId, ETagType.Category, onlyEnabled: false).Select(x => new {value = x.Name, id = x.Id})
            });
        }

        public JsonResult GetSelectedBrands(int categoryId)
        {
            return Json(CategoryService.GetCategoryBrands(categoryId).Select(x => new { Id = x.ID, x.Name }));
        }

        public JsonResult GetCategoryBrands(int categoryId)
        {
            return Json(BrandService.GetBrandsByCategoryId(categoryId, true).Select(x => new { Id = x.ID, x.Name }));
        }

        public JsonResult GetBrandList(string q = null, int page = 1, int count = 100)
        {
            return Json(BrandService.GetBrandsBySearch(count, page, q, false).Select(x => new { Id = x.ID, x.Name }));
        }

        #region Property groups

        [HttpGet]
        public JsonResult GetAllPropertyGroups()
        {
            return Json(PropertyGroupService.GetList().Select(x => new
            {
                label = x.Name,
                value = x.PropertyGroupId
            }));
        }

        [HttpGet]
        public JsonResult GetPropertyGroups(int categoryId)
        {
            return Json(new
            {
                DataItems = PropertyGroupService.GetListByCategory(categoryId).Select(x => new
                {
                    CategoryId = categoryId,
                    x.PropertyGroupId,
                    x.Name
                }).ToList()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddGroupToCategory(int groupId, int categoryId)
        {
            if (groupId == 0 || PropertyGroupService.Get(groupId) == null)
                return Json(false);
            try
            {
                PropertyGroupService.AddGroupToCategory(groupId, categoryId);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_AddPropertyGroupToCategory);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Json(false);
            }
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteGroupFromCategory(int groupId, int categoryId)
        {
            PropertyGroupService.DeleteGroupFromCategory(groupId, categoryId);
            return Json(new { result = true });
        }

        #endregion

        #region Related / Alternative products

        // category
        public JsonResult GetRecomCategories(int categoryId, RelatedType type)
        {
            return Json(CategoryService.GetRelatedCategories(categoryId, type).Select(x => new { x.CategoryId, x.Name, Type = type }));
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteRecomCategory(int categoryId, int relCategoryId, RelatedType type)
        {
            CategoryService.DeleteRelatedCategory(categoryId, relCategoryId, type);
            return Json(new { result = true });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddRecomCategory(int categoryId, int relCategoryId, RelatedType type)
        {
            var categoryIds = CategoryService.GetRelatedCategoryIds(categoryId, type);

            if (categoryIds == null || !categoryIds.Contains(relCategoryId))
            {
                CategoryService.AddRelatedCategory(categoryId, relCategoryId, type);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_SetProductRecommendations);
            }
            return Json(new { result = true });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddRecomCategories(int categoryId, List<int> relCategories, RelatedType type)
        {
            var categoryIds = CategoryService.GetRelatedCategoryIds(categoryId, type);
            if (categoryIds == null)
                categoryIds = new List<int>();
            foreach (var relCategoryId in relCategories)
            {
                if (!categoryIds.Contains(relCategoryId))
                {
                    CategoryService.AddRelatedCategory(categoryId, relCategoryId, type);
                    categoryIds.Add(relCategoryId);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_SetProductRecommendations);
                }
            }
            
            return Json(new { result = true });
        }

        // properties
        public JsonResult GetRecomProperties(int categoryId, RelatedType type)
        {
            var props = CategoryService.GetRelatedProperties(categoryId, type);

            return Json(props.Select(x => new { Id = x.PropertyId, Name = x.Name, Type = type, IsSame = x.IsSame }));
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteRecomProperty(int categoryId, int propertyId, RelatedType type)
        {
            CategoryService.DeleteRelatedProperty(categoryId, propertyId, type);

            return Json(new { result = true });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddRecomProperty(int categoryId, int propertyId, RelatedType type, bool isSame)
        {
            var props = CategoryService.GetRelatedPropertyIds(categoryId, type, isSame);

            if (propertyId > 0 && (props == null || props.All(x => x != propertyId)))
            {
                CategoryService.AddRelatedProperty(categoryId, propertyId, type, isSame);
            }

            return Json(new { result = true });
        }


        // properties with values
        public JsonResult GetRecomPropertiesWithValues(int categoryId, RelatedType type)
        {
            var propVals = CategoryService.GetRelatedPropertyValues(categoryId, type);

            return Json(propVals.Select(x => new { Id = x.PropertyValueId, Name = x.Property.Name, Value = x.Value, Type = type }));
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteRecomPropertyWithValue(int categoryId, int propertyValueId, RelatedType type)
        {
            CategoryService.DeleteRelatedPropertyValue(categoryId, propertyValueId, type);

            return Json(new { result = true });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddRecomPropertyWithValue(int categoryId, int propertyValueId, RelatedType type)
        {
            var props = CategoryService.GetRelatedPropertyValuesIds(categoryId, type);

            if (propertyValueId > 0 && (props == null || props.All(x => x != propertyValueId)))
                CategoryService.AddRelatedPropertyValue(categoryId, propertyValueId, type);

            return Json(new { result = true });
        }

        //properties and values
        public JsonResult GetProperties()
        {
            var props = PropertyService.GetAllProperties();

            return Json(props.Select(x => new { x.PropertyId, x.Name }));
        }

        public JsonResult GetPropertyValues(int propertyId)
        {
            var values = PropertyService.GetValuesByPropertyId(propertyId);

            return Json(values.Select(x => new { x.PropertyValueId, x.Value }));
        }

        #endregion

        #region AddCategoryList

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategoryList(int categoryId, List<string> categories)
        {
            var handler = new AddCategoryList(categoryId, categories);
            handler.Execute();

            return Json(new { result = true });
        }

        [HttpGet]
        public JsonResult GetCategoryForList(int categoryId)
        {
            var category = CategoryService.GetCategory(categoryId);
            var model = category != null
                ? new CategoryForListModel() {CategoryId = category.CategoryId, Name = category.Name}
                : null;

            return Json(new {category = model});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetCategoriesByCategoryIds(List<int> categoryIds)
        {
            var list = new List<CategoryForListModel>();

            foreach (var categoryId in categoryIds)
            {
                var category = CategoryService.GetCategory(categoryId);
                if (category != null)
                    list.Add(new CategoryForListModel() {CategoryId = category.CategoryId, Name = category.Name});
            }

            return Json(list);
        }

        #endregion

        #region Automap Categories

        public JsonResult GetAutomapCategories(int categoryId)
        {
            var items = CategoryService.GetAutomapCategories(categoryId);
            var result = new List<AutomapCategoryModel>();
            foreach (var item in items)
            {
                var parents = CategoryService.GetParentCategories(item.NewCategoryId).Reverse().ToList();
                result.Add(new AutomapCategoryModel
                {
                    CategoryId = item.NewCategoryId,
                    Name = parents[parents.Count - 1].Name,
                    Path = parents.Select(x => x.Name).AggregateString(" / "),
                    Main = item.Main
                });
            }
            result = result.OrderBy(x => x.Path).ToList();

            var automapAction = CategoryService.GetCategoryAutomapAction(categoryId);
            return JsonOk(new
            {
                automapCategories = result,
                automapAction = automapAction == ECategoryAutomapAction.None || automapAction.HasFlag(ECategoryAutomapAction.DoNothing)
                    ? ECategoryAutomapAction.DoNothing
                    : automapAction,
                mainAutomapCategory = automapAction.HasFlag(ECategoryAutomapAction.CopyAndSetMain)
                    ? categoryId 
                    : items.Any(x => x.Main) ? items.First(x => x.Main).NewCategoryId : (int?)null
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddAutomapCategories(int categoryId, List<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any())
                return JsonError("Выберите категории");
            foreach (var id in categoryIds)
            {
                if (categoryId == id || !CategoryService.IsExistCategory(id))
                    continue;
                CategoryService.AddAutomapCategory(new CategoryAutomap
                {
                    CategoryId = categoryId,
                    NewCategoryId = id,
                    Main = false
                });
            }
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetMainAutomapCategory(int categoryId, int? automapCategoryId)
        {
            CategoryService.ClearMainAutomapCategory(categoryId);
            if (automapCategoryId.HasValue)
            {
                CategoryService.UpdateAutomapCategory(new CategoryAutomap
                {
                    CategoryId = categoryId,
                    NewCategoryId = automapCategoryId.Value,
                    Main = true
                });
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetCategoryAutomapAction(int categoryId, ECategoryAutomapAction automapAction)
        {
            CategoryService.SetCategoryAutomapAction(categoryId, automapAction);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ClearMainAutomapCategory(int categoryId)
        {
            CategoryService.ClearMainAutomapCategory(categoryId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAutomapCategory(int categoryId, int automapCategoryId)
        {
            CategoryService.DeleteAutomapCategory(categoryId, automapCategoryId);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAutomapCategories(int categoryId)
        {
            CategoryService.DeleteAutomapCategories(categoryId);
            return JsonOk();
        }

        #endregion
    }
}
