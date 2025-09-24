using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Catalog.CategoryDiscountRegulation;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class CategoryDiscountRegulationController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation("Регулирование скидок для категории");
            SetNgController(NgControllers.NgControllersTypes.CategoryDiscountRegulationCtrl);

            return View();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ChangeDiscounts(CategoryDiscountRegulationModel model)
        {
            string msg = null;

            try
            {
                if (model.ChooseProducts && (model.CategoryIds == null || model.CategoryIds.Count == 0))
                {
                    return Json(new { result = false, msg = T("Admin.PriceRegulation.Index.NoSelectedCategories") });
                }

                var allProducts = !model.ChooseProducts;
                var percent = model.ValueOption == CategoryDiscountRegulationValueOption.Percent;
                var categoryIds = new List<int>();

                if (!allProducts)
                {
                    foreach (var categoryId in model.CategoryIds)
                    {
                        var category = CategoryService.GetCategory(categoryId);
                        if (category != null)
                        {
                            if (!categoryIds.Contains(category.CategoryId))
                                categoryIds.Add(category.CategoryId);

                            var subCategories = CategoryService.GetChildCategoriesByCategoryId(category.CategoryId, false);

                            if (subCategories != null && subCategories.Count > 0)
                                GetCategoryWithSubCategoriesIds(subCategories, categoryIds);
                        }
                    }
                }

                ProductService.ChangeProductsDiscountByCategories(model.Value, categoryIds, percent, allProducts);
                msg = string.Format(T("Admin.PriceRegulation.Index.DecrementMsg"), model.Value, percent ? "%" : "");
                //if (model.Action == CategoryDiscountRegulationAction.Decrement)
                //{
                //    ProductService.ChangeProductsDiscountByCategories(model.Value, true, categoryIds, percent, allProducts);
                //    //ProductService.DecrementProductsPrice(model.Value, false, categoryIds, percent, allProducts);
                //    msg = string.Format(T("Admin.PriceRegulation.Index.DecrementMsg"), model.Value, percent ? "%" : "");
                //}

                //if (model.Action == CategoryDiscountRegulationAction.Increment)
                //{
                //    ProductService.ChangeProductsDiscountByCategories(model.Value, false, categoryIds, percent, allProducts);
                //    //ProductService.IncrementProductsPrice(model.Value, false, categoryIds, percent, allProducts);
                //    msg = string.Format(T("Admin.PriceRegulation.Index.IncrementMsg"), model.Value, percent ? "%" : "");
                //}             

                ProductService.PreCalcProductParamsMass();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Json(new { result = false, msg = ex.Message });
            }

            return Json(new { result = true, msg });
        }

        private void GetCategoryWithSubCategoriesIds(List<Category> categories, List<int> result)
        {
            if (categories == null)
                return;

            foreach (var category in categories)
            {
                if (!result.Contains(category.CategoryId))
                    result.Add(category.CategoryId);

                if (category.HasChild)
                    GetCategoryWithSubCategoriesIds(CategoryService.GetChildCategoriesByCategoryId(category.CategoryId, false), result);
            }
        }

    }
}
