using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Booking.Category;
using AdvantShop.Web.Admin.Models.Booking.Category;
using AdvantShop.Web.Admin.ViewModels.Booking;
using AdvantShop.Web.Admin.ViewModels.Booking.Category;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Booking
{
    public partial class BookingCategoryController : BaseBookingController
    {
        public ActionResult Index()
        {
            if (SelectedAffiliate == null || !AffiliateService.CheckAccessToEditing(SelectedAffiliate))
                return RedirectToAction("Index", "Booking");

            SetMetaInformation("Категории");
            SetNgController(NgControllers.NgControllersTypes.BookingCategoriesCtrl);

            var model = new CategoriesModel()
            {
                SelectedAffiliate = SelectedAffiliate
            };



            return View(model);
        }

        public ActionResult View(int id)
        {
            if (SelectedAffiliate == null || !AffiliateService.CheckAccessToEditing(SelectedAffiliate))
                return RedirectToAction("Index", "Booking");

            var category = CategoryService.Get(id);
            if (category == null || !CategoryService.ExistRefAffiliate(category.Id, SelectedAffiliate.Id))
                return RedirectToAction("Index");

            SetMetaInformation("Услуги");
            SetNgController(NgControllers.NgControllersTypes.BookingServicesCtrl);

            var model = (CategoryModel)category;
            model.AffiliateId = SelectedAffiliate.Id;

            return View(model);
        }

        public JsonResult GetListCategories()
        {
            return Json(new {items = CategoryService.GetList().Select(x => (CategoryModel)x).ToList()});
        }

        public JsonResult GetListCategoriesRefAffiliate(int affiliateId)
        {
            if (AffiliateService.CheckAccess(affiliateId))
            {
                var affiliateCatIds = CategoryService.GetListId(affiliateId);
                var items = CategoryService.GetList().Select(x => ((CategoryModel) x)).ToList();
                items.Where(x => affiliateCatIds.Contains(x.Id)).ForEach(x => x.AffiliateId = affiliateId);

                return Json(new {items = items});
            }
            return Json(null);
        }

        public JsonResult GetCategoriesTree(CategoriesTree model)
        {
            return Json(new GetCategoriesTree(model).Execute());
        }

        [ChildActionOnly]
        public ActionResult CategoriesTreeView(AdminCatalogTreeView model)
        {
            return PartialView(model);
        }

        public JsonResult GetCategory(int id)
        {
            var category = CategoryService.Get(id);
            if (category == null)
                return JsonError("Указанная категория отсутствует");

            return JsonOk((CategoryModel)category);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategory(CategoryModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var list = CategoryService.GetList();
            var sortOrder = (list.Count > 0 ? list.Max(x => x.SortOrder) : 0) + 10;

            var category = new Category()
            {
                Name = model.Name,
                Enabled = model.Enabled,
                SortOrder = sortOrder
            };
            var id = CategoryService.Add(category);

            if (model.PhotoFile != null)
                new UploadImageCropped(CategoryService.Get(id), model.Image, model.PhotoFile).Execute();

            if (model.AffiliateId.HasValue && AffiliateService.CheckAccessToEditing(model.AffiliateId.Value))
                CategoryService.AddRefAffiliate(id, model.AffiliateId.Value);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_CategoryCreated);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCategory(CategoryModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var category = CategoryService.Get(model.Id);
            if (category == null)
                return JsonError("Категория не найдена");

            category.Name = model.Name;
            category.Enabled = model.Enabled;

            CategoryService.Update(category);

            if (model.PhotoFile != null)
                new UploadImageCropped(category, model.Image, model.PhotoFile).Execute();

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeCategorySorting(int categoryId, int? prevCategoryId, int? nextCategoryId)
        {
            var category = CategoryService.Get(categoryId);
            if (category == null)
                return JsonError("Категория не найдена");

            if (!prevCategoryId.HasValue && !nextCategoryId.HasValue)
                return JsonError("Недостаточно данных для изменения сортировки");

            var categories = CategoryService.GetList().Where(x => x.Id != category.Id).ToList();

            if (prevCategoryId != null)
            {
                var index = categories.FindIndex(x => x.Id == prevCategoryId);
                categories.Insert(index + 1, category);
            }
            else if (nextCategoryId != null)
            {
                var index = categories.FindIndex(x => x.Id == nextCategoryId);
                categories.Insert(index > 0 ? index - 1 : 0, category);
            }

            for (int i = 0; i < categories.Count; i++)
            {
                categories[i].SortOrder = i * 10 + 10;
                CategoryService.Update(categories[i]);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult BindToAffiliate(int categoryId, int affiliateId, bool bind)
        {
            if (AffiliateService.CheckAccessToEditing(affiliateId))
            {
                if (bind)
                    CategoryService.AddRefAffiliate(categoryId, affiliateId);
                else
                    CategoryService.DeleteRefAffiliate(categoryId, affiliateId);

                return JsonOk();
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategoryImage(int id)
        {
            var category = CategoryService.Get(id);
            if (category == null)
                return JsonError("Категория не найдена");

            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingCategory, category.Image));

            category.Image = null;
            CategoryService.Update(category);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategory(int categoryId)
        {
            CategoryService.Delete(categoryId);
            return JsonOk();
        }
    }
}
