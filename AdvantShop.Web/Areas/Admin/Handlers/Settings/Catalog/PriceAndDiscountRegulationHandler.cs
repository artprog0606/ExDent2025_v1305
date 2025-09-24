using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Settings.CatalogSettings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Catalog
{
    public static class PriceAndDiscountRegulationHandler
    {
        public static Result<string> ChangePrices(PriceRegulationModel model)
        {
            if (model.Value <= 0)
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.GreaterZero")));
            }
            
            if (model.ValueOption == PriceRegulationValueOption.Percent && 
                model.Action == PriceRegulationAction.Decrement && 
                (model.Value < 0 || model.Value > 100))
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.GreaterZero")));
            }
                
            if (model.ChooseProducts && 
                (model.CategoryIds == null || model.CategoryIds.Count == 0))
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.NoSelectedCategories")));
            }
            
            try
            {
                var allProducts = !model.ChooseProducts;
                var percent = model.ValueOption == PriceRegulationValueOption.Percent;
                var categoryIds = !allProducts ? GetCategories(model.CategoryIds) : new List<int>();
                var message = "";

                if (model.Action == PriceRegulationAction.Decrement)
                {
                    ProductService.DecrementProductsPrice(model.Value, false, categoryIds, percent, allProducts);
                    message = string.Format(LocalizationService.GetResource("Admin.Settings.PriceRegulation.DecrementMsg"), model.Value, percent
                        ? "%"
                        : "");
                }

                if (model.Action == PriceRegulationAction.Increment)
                {
                    ProductService.IncrementProductsPrice(model.Value, false, categoryIds, percent, allProducts);
                    message = string.Format(LocalizationService.GetResource("Admin.Settings.PriceRegulation.IncrementMsg"), model.Value, percent
                        ? "%"
                        : "");
                }

                if (model.Action == PriceRegulationAction.IncBySupply)
                {
                    ProductService.IncrementProductsPrice(model.Value, true, categoryIds, percent, allProducts);
                    message = string.Format(LocalizationService.GetResource("Admin.Settings.PriceRegulation.IncBySupplyMsg"), model.Value, percent
                        ? "%"
                        : "");
                }

                ProductService.PreCalcProductParamsMass();

                var newValue =
                    $"{(model.Action == PriceRegulationAction.Decrement ? "-" : (model.Action == PriceRegulationAction.Increment ? "+" : "+ от закупочной"))} {model.Value} {(percent ? "%" : "")}";

                ProductChangeHistoryService.InsertMassByCategoriesBackground(
                    allProducts, categoryIds,
                    "Цена (Регулирование цен)", newValue,
                    new ChangedBy(CustomerContext.CurrentCustomer));

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_ChangePricesByPriceRegulation);

                return Result.Success(message);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Result.Failure<string>(new Error(ex.Message));
            }
        }

        public static Result<string> ChangeDiscountsByCategory(CategoryDiscountRegulationModel model)
        {
            if (model.ValueOption == CategoryDiscountRegulationValueOption.AbsoluteValue && 
                model.Value < 0)
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.GreaterZero")));
            }
                
            if (model.ValueOption == CategoryDiscountRegulationValueOption.Percent && 
                (model.Value < 0 || model.Value > 100))
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.ZeroToHundred")));
            }
                
            if (model.ChooseProducts && 
                (model.CategoryIds == null || model.CategoryIds.Count == 0))
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.NoSelectedCategories")));
            }
            
            try
            {
                var allProducts = !model.ChooseProducts;
                var percent = model.ValueOption == CategoryDiscountRegulationValueOption.Percent;
                var categoryIds = !allProducts ? GetCategories(model.CategoryIds) : new List<int>();

                
                ProductService.ChangeProductsDiscountByCategories(model.Value, categoryIds, percent, allProducts);
                ProductService.PreCalcProductParamsMass();

                ProductChangeHistoryService.InsertMassByCategoriesBackground(
                    allProducts, 
                    categoryIds, 
                    "Скидка (Регулирование цен)", 
                    model.Value + (percent ? "%" : ""), 
                    new ChangedBy(CustomerContext.CurrentCustomer));
                
                var message = string.Format(
                    LocalizationService.GetResource("Admin.Settings.PriceRegulation.ChangeDiscountMsg"), 
                    model.Value,
                    percent
                        ? "%"
                        : " " + LocalizationService.GetResource("Admin.Settings.PriceRegulation.ChangedInProductCurrency"));
                
                return Result.Success(message);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Result.Failure<string>(new Error(ex.Message));
            }
        }
        
        public static Result<string> ChangeDiscountsByBrands(BrandDiscountRegulationModel model)
        {
            if (model.ValueOption == CategoryDiscountRegulationValueOption.AbsoluteValue && model.Value < 0)
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.GreaterZero")));
            }
            
            if (model.ValueOption == CategoryDiscountRegulationValueOption.Percent && (model.Value < 0 || model.Value > 100))
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.ZeroToHundred")));
            }
                
            if (model.BrandIds.Count == 0)
            {
                return Result.Failure<string>(new Error(LocalizationService.GetResource("Admin.Settings.PriceRegulation.SelectManufacturer")));
            }
            
            try
            {
                var percent = model.ValueOption == CategoryDiscountRegulationValueOption.Percent;

                ProductService.ChangeProductsDiscountByBrands(model.Value, model.BrandIds, percent);
                ProductService.PreCalcProductParamsMass();
                
                ProductChangeHistoryService.InsertMassByBrandsBackground(
                    model.BrandIds, 
                    "Скидка (Регулирование цен)", 
                    model.Value + (percent ? "%" : ""), 
                    new ChangedBy(CustomerContext.CurrentCustomer));

                var message =
                    string.Format(
                        LocalizationService.GetResource("Admin.Settings.PriceRegulation.ChangeDiscountByBrandMsg"),
                        model.Value,
                        percent
                            ? "%"
                            : " " + LocalizationService.GetResource("Admin.Settings.PriceRegulation.ChangedInProductCurrency"));

                return Result.Success(message);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Result.Failure<string>(new Error(ex.Message));
            }
        }

        #region help methods
        
        private static List<int> GetCategories(List<int> modelCategoryIds)
        {
            var categoryIds = new List<int>();
            
            foreach (var categoryId in modelCategoryIds)
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

            return categoryIds;
        }

        private static void GetCategoryWithSubCategoriesIds(List<Category> categories, ICollection<int> result)
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
        #endregion
    }
}
