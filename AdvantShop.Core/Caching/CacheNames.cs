//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Caching
{
    /// <summary>
    /// Retun the special formated cache object names
    /// </summary>
    /// <remarks></remarks>
    public class CacheNames
    {
        public const string TemplateSetPref = "TemplateSettings_";
        public const string Category = "Category_";
        public const string CategoriesOnMainPage = Category + "onmainpage_";

        public const string StaticBlock = "StaticBlock_";

        public const string News = "News_";
        public const string Voiting = "Voiting_";
        public const string MenuPrefix = "MenuCache_";
        public const string PaymentOptions = "PaymentOptions_";
        public const string ShippingOptions = "ShippingOptions_";

        public const string MenuCatalog = "MenuCatalog_";
        public const string SQLPagingCount = "SQLPagingCount_";
        public const string SQLPagingItems = "SQLPagingItems_";

        public const string ProductList = "ProductList_";
        public const string ProductRelatedProducts = "ProductRelatedProducts_";

        public const string BrandsInCategory = "BrandsInCategory_";
        public const string PropertiesInCategory = "PropertiesInCategory_";

        public const string ShippingForCityAndCountry = "ShippingForCityAndCountry_";

        public const string AdvantShopMail = "AdvantShopMail_";

        public const string AdminMenu = "AdminMenu";

        public const string BonusCard = "BonusCard_";

        public const string Customer = "Customer_";

        public const string IsDebug = "IsDebug";

        public const string OrderPrefix = "Orders_";
        public const string ChildCategories = Category + "_ChildCategories_";

        public static string GetModuleSettingsCacheObjectName()
        {
            return "ModuleSettings";
        }

        public static string GetTemplateSettings(string template)
        {
            return TemplateSetPref + template;
        }

        public static string GetTemplateSettingsCacheObjectName(string template, string strName)
        {
            return TemplateSetPref + template + "_" + strName;
        }

        public static string GetCategoryCacheObjectPrefix()
        {
            return Category;
        }

        public static string GetCategoryCacheObjectName(int id)
        {
            return Category + id;
        }

        public static string GetStaticBlockCacheObjectName(string strName)
        {
            return StaticBlock + strName;
        }

        public static string GetBestSellersCacheObjectName()
        {
            return "GetBestSellers";
        }

        public static string GetMenuCacheObjectName(EMenuType menuType, EMenuItemShowMode showMode = EMenuItemShowMode.All)
        {
            return MenuPrefix + menuType +
                (showMode == EMenuItemShowMode.Authorized ? "_Auth" : string.Empty);
        }

        public static string GetMainMenuCacheObjectName()
        {
            return GetMenuCacheObjectName(EMenuType.Top);
        }

        public static string GetMainMenuAuthCacheObjectName()
        {
            return GetMenuCacheObjectName(EMenuType.Top, EMenuItemShowMode.Authorized);
        }

        public static string GetBottomMenuCacheObjectName()
        {
            return GetMenuCacheObjectName(EMenuType.Bottom);
        }

        public static string GetBottomMenuAuthCacheObjectName()
        {
            return GetMenuCacheObjectName(EMenuType.Bottom, EMenuItemShowMode.Authorized);
        }

        public static string GetOrderPriceDiscountCacheObjectName()
        {
            return "OrderPriceDiscount";
        }

        public static string GetXmlSettingsCacheObjectName()
        {
            return "XMLSettings";
        }

        public static string GetRoutesCacheObjectName()
        {
            return "Routes";
        }

        public static string GetNewsForMainPage()
        {
            return News + "ForMainPage";
        }

        public static string GetUrlCacheObjectName()
        {
            return "UrlSynonyms";
        }

        public static string GetAltSessionCacheObjectName(string sessionId)
        {
            return "AltSession_" + sessionId;
        }

        internal static string GetCurrenciesCacheObjectName()
        {
            return "Currencies";
        }

        public static string GetRoleActionsCacheObjectName(string customerId)
        {
            return "RoleActions_" + customerId;
        }

        public static string GetDesignCacheObjectName(string designType)
        {
            return "Designs_" + designType;
        }

        public static string GetPaymentOptionsCacheName(int preorderHash)
        {
            return PaymentOptions + preorderHash;
        }

        public static string GetShippingOptionsCacheName(int preorderHash)
        {
            return ShippingOptions + preorderHash;
        }

        public static string SQlPagingCountCacheName(string cashKey, string queryHash, string paramsHashCode)
        {
            return SQLPagingCount + cashKey + (queryHash + "_" + paramsHashCode).GetHashCode();
        }

        public static string SQlPagingItemsCacheName(string cashKey, string queryHash, string paramsHashCode)
        {
            var customerGroupId = Customers.CustomerContext.CurrentCustomer.CustomerGroupId;
            return SQLPagingItems + cashKey + (queryHash + "_" + paramsHashCode).GetHashCode() + "_" + customerGroupId;
        }

        public static string MainPageProductsCacheName(string type, int count, string query)
        {
            var currency = CurrencyService.CurrentCurrency;
            var customerGroupId = Customers.CustomerContext.CurrentCustomer?.CustomerGroupId;

            return SQLPagingItems + "_MainPageProducts_" + type + "_" + count.ToString() + "_" +
                   currency.Iso3 + "_" + currency.Rate.ToString() + "_" + customerGroupId + "_" +
                   query.GetHashCode();
        }

        public static string MainPageProductsCountCacheName(string type, bool enabled)
        {
            return SQLPagingItems + "_MainPageProducts_Count_" + type + enabled.ToString();
        }

        public static string ProductListCacheName(int listId, int count, string query)
        {
            var currency = CurrencyService.CurrentCurrency;
            var customerGroupId = Customers.CustomerContext.CurrentCustomer?.CustomerGroupId;
            return SQLPagingItems + ProductList + listId.ToString() + "_" + count.ToString() + "_" +
                   currency.Iso3 + "_" + currency.Rate.ToString() + "_" + customerGroupId +
                   query.GetHashCode();
        }

        public static string GetProductRelatedProductCacheObjectName(int productId)
        {
            return ProductRelatedProducts + productId;
        }

        public static string GetProductRelatedProductsCacheName(int productId, RelatedType relatedType, List<int> warehouseIds = null)
        {
            var currency = CurrencyService.CurrentCurrency;
            var customerGroupId = Customers.CustomerContext.CurrentCustomer?.CustomerGroupId;
            return ProductRelatedProducts + productId + "_" + relatedType + "_" +
                   currency.Iso3 + "_" + currency.Rate + "_" + customerGroupId + "_" +
                   (warehouseIds != null ? string.Join(",", warehouseIds) : null);
        }

        public static string GetProductRelatedProductFromCategoryCacheObjectName()
        {
            return ProductRelatedProducts + "_fromCategories_";
        }

        public static string GetProductRelatedProductsFromCategoryCacheName(IEnumerable<int> categoryIds, RelatedType relatedType, int count, List<int> warehouseIds)
        {
            var currency = CurrencyService.CurrentCurrency;
            var customerGroupId = Customers.CustomerContext.CurrentCustomer?.CustomerGroupId;
            return ProductRelatedProducts + "_fromCategories_" + string.Join(".", categoryIds) + "_" + relatedType + "_" + count + "_" +
                   currency.Iso3 + "_" + currency.Rate + "_" + customerGroupId + "_" + (warehouseIds != null ? string.Join(",", warehouseIds) : null);
        }

        public static string BrandsInCategoryCacheName(int categoryId, bool indepth, bool onlyAvailable, List<int> warehouseIds)
        {
            return BrandsInCategory + categoryId + "_" + indepth + "_" +
                   onlyAvailable + "_" +
                   (warehouseIds != null ? String.Join(",", warehouseIds) : "");
        }
        
        public static string BrandsByFilterCacheName(int flag, int listId, List<int> productIds, bool onlyAvailable, List<int> warehouseIds)
        {
            return BrandsInCategory + flag + "_" + listId + "_" + 
                   (productIds != null ? String.Join(",", productIds) : "") + "_" +
                   onlyAvailable + "_" +
                   (warehouseIds != null ? String.Join(",", warehouseIds) : "");
        }

        public static string PropertiesInCategoryCacheName(int categoryId, bool indepth, List<int> productIds, List<int> warehouseIds, bool showOnlyAvailable)
        {
            return PropertiesInCategory + categoryId + "_" + indepth + "_" +
                   (productIds != null ? string.Join(",", productIds) : "") + "_" + 
                   (warehouseIds != null ? string.Join(",", warehouseIds) : "") + "_" +
                    showOnlyAvailable;
        }

        public static string PropertiesInCategoryCacheName(int productOnMainType, int? productListId, List<int> warehouseIds, bool showOnlyAvailable)
        {
            return PropertiesInCategory + productOnMainType + "_" + productListId + "_" +
                   (warehouseIds != null ? string.Join(",", warehouseIds) : "") + "_" + 
                    showOnlyAvailable;
        }

        public static string GetShippingForCityRegionAndCountry(int methodId, string countryName, string regionName, string cityName, string districtName)
        {
            return ShippingForCityAndCountry + methodId + "_" + countryName + "_" + regionName + "_" + cityName + "_" + districtName;
        }

        public static string GetChildCategoriesByCategoryId(int categoryId, bool? hasProducts, List<int> warehouseIds, bool showOnlyAvailable) =>
            ChildCategories + "_" + categoryId + "_" + 
            (hasProducts is null ? "" : hasProducts.Value.ToString()) + "_" + 
            (warehouseIds != null ? string.Join(",", warehouseIds) : null) + "_" + showOnlyAvailable;

        public static string GetCategoryIdByNameAndParentId(string name, int parentId) =>
            ChildCategories + "_CategoryIdByName_" + name + "_" + parentId;
        
        public static string GetCustomDataCacheName(string cashKey, string queryHash, string paramsHashCode)
        {
            return SQLPagingItems + cashKey + (queryHash + "_" + paramsHashCode).GetHashCode();
        }
    }
}