//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AdvantShop.CMS;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using AdvantShop.Core;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.ChangeHistories;

namespace AdvantShop.Catalog
{
    public enum ECategoryDisplayStyle
    {
        [Localize("Core.Catalog.ECategoryDisplayStyle.None")]
        None = 0,

        [Localize("Core.Catalog.ECategoryDisplayStyle.Tile")]
        Tile = 1,

        [Localize("Core.Catalog.ECategoryDisplayStyle.List")]
        List = 2,
    }

    public enum ESortOrder
    {
        [Localize("Core.Catalog.ESortOrder.NoSorting")]
        NoSorting = 0,

        [Localize("Core.Catalog.ESortOrder.DescByPopular")]
        DescByPopular = 100,

        [Localize("Core.Catalog.ESortOrder.DescByAddingDate")]
        DescByAddingDate = 200,

        [Localize("Core.Catalog.ESortOrder.AscByPrice")]
        AscByPrice = 300,

        [Localize("Core.Catalog.ESortOrder.DescByPrice")]
        DescByPrice = 400,

        [Localize("Core.Catalog.ESortOrder.DescByDiscount")]
        DescByDiscount = 500,

        [Localize("Core.Catalog.ESortOrder.DescByRatio")]
        DescByRatio = 600,

        [Localize("Core.Catalog.ESortOrder.AscByName")]
        AscByName = 700,

        [Localize("Core.Catalog.ESortOrder.DescByName")]
        DescByName = 800,


        [EnumIgnore]
        [Localize("Core.Catalog.ESortOrder.AscByAddingDate")]
        AscByAddingDate = 1,

        [EnumIgnore]
        [Localize("Core.Catalog.ESortOrder.AscByRatio")]
        AscByRatio = 7
    }

    [Flags]
    public enum ECategoryAutomapAction
    {
        [Localize("Core.Catalog.ECategoryAutomapAction.None")]
        None = 0,
        [Localize("Core.Catalog.ECategoryAutomapAction.Copy")]
        Copy = 1,
        [Localize("Core.Catalog.ECategoryAutomapAction.Move")]
        Move = 2,
        [Localize("Core.Catalog.ECategoryAutomapAction.CopyAndSetMain")]
        CopyAndSetMain = 4 | Copy,
        [Localize("Core.Catalog.ECategoryAutomapAction.DoNothing")]
        DoNothing = 8
    }


    public class CategoryService
    {
        public struct CategoryPictures
        {
            public string Picture;
            public string MiniPicture;
        }

        /// <summary>
        /// Geting new ID for category contains all products without categories
        /// </summary>
        /// <returns>"-1" as default ID</returns>
        /// <remarks></remarks>
        public const int DefaultNonCategoryId = -1;


        public static IEnumerable<int> GetProductIDs(int categoryId)
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>(
                "SELECT [ProductID] FROM [Catalog].[ProductCategories] WHERE [CategoryID] = @CategoryID",
                CommandType.Text,
                "ProductID",
                new SqlParameter("@CategoryID", categoryId));
        }

        /// <summary>
        /// return child categories by parent categoryId
        /// </summary>
        public static List<Category> GetChildCategoriesByCategoryId(int categoryId, bool? hasProducts = null, List<int> warehouseIds = null) 
            => GetChildCategoriesByCategoryId(categoryId, hasProducts, noCache: false, warehouseIds);

        /// <summary>
        /// return child categories by parent categoryId
        /// </summary>
        public static List<Category> GetChildCategoriesByCategoryId(int categoryId, bool? hasProducts, bool noCache, 
                                                                        List<int> warehouseIds = null)
        {
            return noCache 
                ? _getChildCategoriesByCategoryId(categoryId, hasProducts, warehouseIds)
                : CacheManager.Get(
                    CacheNames.GetChildCategoriesByCategoryId(categoryId, hasProducts, warehouseIds, SettingsCatalog.ShowOnlyAvalible), 
                    () => _getChildCategoriesByCategoryId(categoryId, hasProducts, warehouseIds));
        }

        private static List<Category> _getChildCategoriesByCategoryId(int categoryId, bool? hasProducts,
                                                                        List<int> warehouseIds = null)
        {
            if (warehouseIds is null)
            {
                if (hasProducts is null && SettingsCatalog.ShowOnlyAvalible)
                {
                    return SQLDataAccess.ExecuteReadList(
                $@"SELECT   
                                    p.*,  
                                    (SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count] ,  
                                    (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,  
                                    (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture  
                                FROM [Catalog].[Category] AS p 
                                WHERE [ParentCategory] = @ParentCategoryID 
                                        AND p.CategoryID <> 0 
                                        AND p.Available_Products_Count > 0 
                                ORDER BY p.SortOrder, p.Name",
                            CommandType.Text,
                            GetChildCategoriesByCategoryIdReader,
                            new SqlParameter("@ParentCategoryID", categoryId),
                            new SqlParameter("@bigType", PhotoType.CategoryBig.ToString()),
                            new SqlParameter("@smallType", PhotoType.CategorySmall.ToString()));
                }
                
                return SQLDataAccess.ExecuteReadList(
                    "[Catalog].[sp_GetChildCategoriesByParentID]", 
                    CommandType.StoredProcedure,
                    GetChildCategoriesByCategoryIdReader,
                    new SqlParameter("@ParentCategoryID", categoryId),
                    new SqlParameter("@hasProducts", hasProducts ?? false),
                    new SqlParameter("@bigType", PhotoType.CategoryBig.ToString()),
                    new SqlParameter("@smallType", PhotoType.CategorySmall.ToString()));
            }
            
            var warehousesSql = string.Join(",", warehouseIds);
            
            return SQLDataAccess.ExecuteReadList(
                $@"if @hasProducts = 0  
                                    SELECT   
                                        p.*,  
                                        (SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count],  
                                        (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,  
                                        (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture  
                                    FROM [Catalog].[Category] AS p 
                                    WHERE [ParentCategory] = @ParentCategoryID 
                                            AND p.CategoryID <> 0 
                                            AND Exists(Select 1 
					                                   From [Catalog].[Category_Warehouse] as cw 
					                                   Where cw.CategoryId = p.CategoryId 
							                                    AND cw.WarehouseId in ({warehousesSql}) 
							                                    AND cw.HasProducts = 1
				                                      ) 
                                    ORDER BY p.SortOrder, p.Name
                                else  
                                    SELECT   
                                        p.*,  
                                        (SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count] ,  
                                        (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,  
                                        (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture  
                                    FROM [Catalog].[Category] AS p 
                                    WHERE [ParentCategory] = @ParentCategoryID 
                                            AND p.CategoryID <> 0 
                                            AND p.Products_Count > 0  
                                            AND Exists(Select 1 
					                                   From [Catalog].[Category_Warehouse] as cw 
					                                   Where cw.CategoryId = p.CategoryId 
							                                    AND cw.WarehouseId in ({warehousesSql}) 
							                                    AND cw.HasProducts = 1
				                                      ) 
                                    ORDER BY p.SortOrder, p.Name",
                CommandType.Text,
                GetChildCategoriesByCategoryIdReader,
                new SqlParameter("@ParentCategoryID", categoryId),
                new SqlParameter("@hasProducts", hasProducts ?? false),
                new SqlParameter("@bigType", PhotoType.CategoryBig.ToString()),
                new SqlParameter("@smallType", PhotoType.CategorySmall.ToString()));
        }

        private static Category GetChildCategoriesByCategoryIdReader(SqlDataReader reader)
        {
            var category = new Category
            {
                CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                ExternalId = SQLDataHelper.GetString(reader, "ExternalId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                ProductsCount = SQLDataHelper.GetInt(reader, "Products_Count"),
                Available_Products_Count = SQLDataHelper.GetInt(reader, "Available_Products_Count"),
                TotalProductsCount = SQLDataHelper.GetInt(reader, "Total_Products_Count"),
                Current_Products_Count = SQLDataHelper.GetInt(reader, "Current_Products_Count"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                DisplayStyle = (ECategoryDisplayStyle)SQLDataHelper.GetInt(reader, "DisplayStyle"),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                ParentsEnabled = SQLDataHelper.GetBoolean(reader, "HirecalEnabled"),
                DisplaySubCategoriesInMenu = SQLDataHelper.GetBoolean(reader, "DisplaySubCategoriesInMenu"),
                Sorting = (ESortOrder)SQLDataHelper.GetInt(reader, "Sorting"),
                ShowOnMainPage = SQLDataHelper.GetBoolean(reader, "ShowOnMainPage")
            };
            var childCounts = SQLDataHelper.GetInt(reader, "ChildCategories_Count");
            category.HasChild = childCounts > 0;
            category.Picture = new CategoryPhoto(category.CategoryId, PhotoType.CategoryBig, SQLDataHelper.GetString(reader, "Picture"));
            category.MiniPicture = new CategoryPhoto(category.CategoryId, PhotoType.CategorySmall, SQLDataHelper.GetString(reader, "MiniPicture"));
            category.ProductsCount = SettingsCatalog.ShowOnlyAvalible
                ? category.Available_Products_Count
                : category.ProductsCount;
            return category;
        }

        /// <summary>
        /// Получить все дочерние категории со всеми подкатегориями
        /// </summary>
        public static List<int> GetAllChildCategoriesIdsByCategoryId(int categoryId)
        {
            return
                SQLDataAccess.Query<int>("select id from Settings.GetChildCategoryByParent(@categoryId)",
                    new { categoryId }).ToList();
        }

        /// <summary>
        /// return child categories by parent categoryId
        /// </summary>
        public static List<Category> GetChildCategoriesByCategoryIdForMenu(int categoryId, int? count = null, List<int> warehouseIds = null)
        {
            var countSql = count.HasValue ? $"TOP ({count})" : "";
            var warehousesSql = warehouseIds != null && warehouseIds.Count > 0 ? string.Join(",", warehouseIds) : null;
            
            return SQLDataAccess.ExecuteReadList(
                $@"IF (Select COUNT(CategoryID)
                                From Catalog.Category
                                Where ParentCategory = @CurrentCategoryID
                                  and Enabled = 1
                                  and Hidden = 0) > 0
                                BEGIN
                                    SELECT {countSql} 
                                           cat.[CategoryID],
                                           [Name],
                                           [Description],
                                           [BriefDescription],
                                           [ParentCategory],
                                           [Products_Count],
                                           [Total_Products_Count],
                                           Available_Products_Count,
                                           [SortOrder],
                                           [Enabled],
                                           [Hidden],
                                           [DisplayStyle],
                                           [DisplayBrandsInMenu],
                                           [DisplaySubCategoriesInMenu],
                                           [UrlPath],
                                           (SELECT Count(CategoryID)
                                            FROM [Catalog].[Category] AS c
                                            WHERE c.ParentCategory = cat.CategoryID
                                              and Enabled = 1
                                              and Hidden = 0) AS [ChildCategories_Count]
                                    FROM [Catalog].[Category] AS cat
                                    WHERE [ParentCategory] = @CurrentCategoryID
                                      AND cat.CategoryID <> 0
                                      and Enabled = 1
                                      AND Hidden = 0
                                      AND HirecalEnabled = 1 
                                      {(warehousesSql != null 
                                          ? " AND Exists(Select 1 " + 
                                                        "From [Catalog].[Category_Warehouse] as cw " +
                                                        "Where cw.CategoryId = cat.CategoryId " +
                                                                "AND cw.WarehouseId in (" + warehousesSql + ") " +
                                                                "AND cw.HasProducts = 1" +
                                                        ") " 
                                          : "")}
                                      {(warehouseIds is null && SettingsCatalog.ShowOnlyAvalible
                                          ? " AND cat.Available_Products_Count > 0 "
                                          : "")}
                                    ORDER BY SortOrder, Name
                                END
                            ELSE
                                BEGIN
                                    SELECT {countSql} 
                                           cat.[CategoryID],
                                           [Name],
                                           [Description],
                                           [BriefDescription],
                                           [ParentCategory],
                                           [Products_Count],
                                           [Total_Products_Count],
                                           Available_Products_Count,
                                           [SortOrder],
                                           [Enabled],
                                           [Hidden],
                                           [DisplayStyle],
                                           [DisplayBrandsInMenu],
                                           [DisplaySubCategoriesInMenu],
                                           [UrlPath],
                                           (SELECT Count(CategoryID)
                                            FROM [Catalog].[Category] AS c
                                            WHERE c.ParentCategory = cat.CategoryID
                                              and Enabled = 1
                                              and Hidden = 0) AS [ChildCategories_Count]
                                    FROM [Catalog].[Category] AS cat 
                                    WHERE [ParentCategory] = (Select ParentCategory
                                                              From Catalog.Category
                                                              Where CategoryID = @CurrentCategoryID)
                                      AND cat.CategoryID <> 0
                                      AND Enabled = 1
                                      AND Hidden = 0
                                      AND HirecalEnabled = 1 
                                      {(warehousesSql != null 
                                          ? " AND Exists(Select 1 " + 
                                                        "From [Catalog].[Category_Warehouse] as cw " +
                                                        "Where cw.CategoryId = cat.CategoryId " +
                                                                "AND cw.WarehouseId in (" + warehousesSql + ") " +
                                                                "AND cw.HasProducts = 1" +
                                                        ")" 
                                          : "")}
                                      {(warehouseIds is null && SettingsCatalog.ShowOnlyAvalible
                                          ? " AND cat.Available_Products_Count > 0 "
                                          : "")}
                                    ORDER BY SortOrder, Name
                                END",
                CommandType.Text,
                reader => {
                    var category = new Category
                    {
                        CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                        ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                        ProductsCount = SQLDataHelper.GetInt(reader, "Products_Count"),
                        Available_Products_Count = SQLDataHelper.GetInt(reader, "Available_Products_Count"),
                        BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                        Description = SQLDataHelper.GetString(reader, "Description"),
                        Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                        Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                        DisplayStyle = (ECategoryDisplayStyle)SQLDataHelper.GetInt(reader, "DisplayStyle"),
                        UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                        DisplayBrandsInMenu = SQLDataHelper.GetBoolean(reader, "DisplayBrandsInMenu"),
                        DisplaySubCategoriesInMenu = SQLDataHelper.GetBoolean(reader, "DisplaySubCategoriesInMenu"),
                    };

                    var childCounts = SQLDataHelper.GetInt(reader, "ChildCategories_Count");
                    category.HasChild = childCounts > 0;
                    return category;
                },
                new SqlParameter("@CurrentCategoryID", categoryId));
        }

        public static IEnumerable<int> GetChildIDsHierarchical(int catId)
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("[Catalog].[sp_GetChildCategoriesIDHierarchical]",
                                                                    CommandType.StoredProcedure,
                                                                    "CategoryID",
                                                                    new SqlParameter("@parent", catId)
                                                                    );
        }

        /// <summary>
        /// return child categories and NonCategory
        /// </summary>
        public static IList<Category> GetChildCategoriesAndNonCategory(int categoryId)
        {
            var res = (List<Category>)GetChildCategoriesByCategoryId(categoryId, false);
            // Adding category witch  contents all products without categories
            var nonCategory = new Category
            {
                CategoryId = DefaultNonCategoryId,
                Name = LocalizationService.GetResource("Core.Catalog.Category.ProductsWithoutCategories"),
                Picture = null,
                ProductsCount = 0,
                ParentCategoryId = DefaultNonCategoryId,
                Enabled = true
            };
            res.Add(nonCategory);
            return res;
        }


        /// <summary>
        /// add category
        /// </summary>
        public static int AddCategory(Category category, bool updateCache, bool trackChanges, ChangedBy changedBy)
        {
            category.CategoryId =
                SQLDataHelper.GetInt(
                    SQLDataAccess.ExecuteScalar(
                        "[Catalog].[sp_AddCategory]", CommandType.StoredProcedure,
                        new SqlParameter("@Name", category.Name),
                        new SqlParameter("@ParentCategory", category.ParentCategoryId),
                        new SqlParameter("@Description", category.Description ?? (object)DBNull.Value),
                        new SqlParameter("@BriefDescription", category.BriefDescription ?? (object)DBNull.Value),
                        new SqlParameter("@SortOrder", category.SortOrder),
                        new SqlParameter("@Enabled", category.Enabled),
                        new SqlParameter("@Hidden", category.Hidden),
                        new SqlParameter("@DisplayStyle", category.DisplayStyle),
                        new SqlParameter("@DisplayChildProducts", category.DisplayChildProducts),
                        new SqlParameter("@DisplayBrandsInMenu", category.DisplayBrandsInMenu),
                        new SqlParameter("@DisplaySubCategoriesInMenu", category.DisplaySubCategoriesInMenu),
                        new SqlParameter("@UrlPath", category.UrlPath),
                        new SqlParameter("@Sorting", category.Sorting),
                        new SqlParameter("@ExternalId", category.ExternalId ?? (object)DBNull.Value),
                        new SqlParameter("@AutomapAction", category.AutomapAction),
                        new SqlParameter("@ModifiedBy", category.ModifiedBy ?? (object)DBNull.Value),
                        new SqlParameter("@ShowOnMainPage", category.ShowOnMainPage)
                        ));
            
            if (updateCache)
            {
                CategoryWriter.AddUpdate(category);
                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(category.ParentCategoryId));
                CacheManager.RemoveByPattern(CacheNames.ChildCategories);
                CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
                CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);

                if (category.ParentCategoryId == 0)
                {
                    CacheManager.RemoveByPattern(CacheNames.GetMenuCacheObjectName(EMenuType.Bottom));
                }
            }

            if (trackChanges)
                CategoryHistoryService.NewCategory(category, changedBy);

            return category.CategoryId;
        }

        /// <summary>
        /// add category
        /// </summary>
        public static int AddCategory(Category category, bool updateCache, bool setHierarchicallyEnabled = true, bool trackChanges = false, ChangedBy changedBy = null)
        {
            var id = AddCategory(category, updateCache, trackChanges, changedBy);
            if (id == -1)
                return -1;

            if (setHierarchicallyEnabled)
                SetCategoryHierarchicallyEnabled(id);

            if (category.Meta != null && !category.Meta.IsDefaultMeta)
            {
                if (!category.Meta.Title.IsNullOrEmpty() 
                    || !category.Meta.MetaKeywords.IsNullOrEmpty() 
                    || !category.Meta.MetaDescription.IsNullOrEmpty() 
                    || !category.Meta.H1.IsNullOrEmpty())
                {
                    category.Meta.ObjId = id;
                    category.Meta.Type = MetaType.Category;
                    
                    MetaInfoService.SetMeta(category.Meta);
                }
            }

            if (category.Tags != null)
            {
                var tags = category.Tags;
                for (var i = 0; i < tags.Count; i++)
                {
                    var tag = TagService.Get(tags[i].Name);
                    tags[i].Id = tag == null ? TagService.Add(tags[i]) : tag.Id;
                    TagService.AddMap(category.CategoryId, tags[i].Id, ETagType.Category, i * 10);
                }
            }

            if (category.SizeChart != null)
                SizeChartService.AddUpdateMap(category.SizeChart.Id, id, ESizeChartEntityType.Category);

            return id;
        }

        /// <summary>
        /// get parent categories by child category
        /// </summary>
        public static IList<Category> GetParentCategories(int childCategoryId)
        {
            try
            {
                return SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetParentCategories]", CommandType.StoredProcedure,
                    reader => new Category
                    {
                        CategoryId = SQLDataHelper.GetInt(reader, "id"),
                        Name = SQLDataHelper.GetString(reader, "name"),
                        UrlPath = SQLDataHelper.GetString(reader, "url")
                    },
                    new SqlParameter("@ChildCategoryId", childCategoryId));
            }
            catch (SqlException ex)
            {
                if (ex.Number == -2)
                {
                    Debug.Log.Error("Timeout on GetParentCategories of category " + childCategoryId + ". Maybe it's looping of parent categories.", ex);
                    return new List<Category>();
                }
                throw;
            }
        }

        public static List<int> GetParentCategoryIds(int childCategoryId)
        {
            return SQLDataAccess.Query<int>("SELECT id FROM [Settings].[GetParentsCategoryByChild](@CategoryId)",
                new { CategoryId = childCategoryId }).ToList();
        }

        /// <summary>
        /// delete category by categoryId
        /// </summary>
        private static List<int> DeleteCategory(int categoryId, bool updateCache, bool trackChanges = true, ChangedBy changedBy = null)
        {
            if (categoryId == 0)
                throw new Exception("deleting root category not allowed");

            var categoryIds = SQLDataAccess.ExecuteReadList("[Catalog].[sp_DeleteCategoryWithSubCategoies]",
                                                            CommandType.StoredProcedure,
                                                            reader => SQLDataHelper.GetInt(reader, "CategoryID"),
                                                            new SqlParameter("@id", categoryId));

            if (updateCache)
            {
                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(categoryId));
                CacheManager.RemoveByPattern(CacheNames.ChildCategories);
                CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
                CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);

                CacheManager.RemoveByPattern(CacheNames.GetMenuCacheObjectName(EMenuType.Bottom));

                CategoryWriter.Delete(categoryId);
            }

            if (trackChanges)
                CategoryHistoryService.DeleteCategory(categoryId, changedBy);

            return categoryIds;
        }

        public static IEnumerable<int> GetAllCategoryIDs(bool onlyDemo = false)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT CategoryID FROM [Catalog].[Category] WHERE CategoryId <> 0" + (onlyDemo ? " AND IsDemo = 1" : string.Empty),
                CommandType.Text, "CategoryID");
        }

        /// <summary>
        /// get all categories
        /// </summary>
        public static List<Category> GetCategories()
        {
            return
                SQLDataAccess.ExecuteReadList(
                    "SELECT [CategoryID], [Name], [ParentCategory], [SortOrder], [Hidden], [Enabled], [UrlPath], HirecalEnabled " + 
                    "FROM [Catalog].[Category] " + 
                    "Where CategoryId <> 0 " +
                    "Order By [SortOrder] Asc",
                    CommandType.Text,
                    reader => new Category
                    {
                        CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                        SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                        Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                        Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                        UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                        ParentsEnabled = SQLDataHelper.GetBoolean(reader, "HirecalEnabled")
                    });
        }

        public static Category GetFirstCategory()
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT top (1) * FROM [Catalog].[Category] Where CategoryId <> 0 and Enabled=1 and HirecalEnabled=1",
                    CommandType.Text, GetCategoryFromReader);
        }

        /// <summary>
        /// get category by categoryId from cache or db if cache null
        /// </summary>
        public static Category GetCategory(int categoryId)
        {
            var cacheName = CacheNames.GetCategoryCacheObjectName(categoryId);

            if (!CacheManager.TryGetValue(cacheName, out Category category))
            {
                category = GetCategoryFromDbByCategoryId(categoryId);

                if (category != null)
                    CacheManager.Insert(cacheName, category);
            }

            return category;
        }

        public static Category GetCategoryFromDbByExternalId(string externalId)
        {
            return SQLDataAccess.ExecuteReadOne(@"SELECT TOP 1 * FROM [Catalog].[Category] WHERE ExternalId = @ExternalId",
               CommandType.Text, GetCategoryFromReader, new SqlParameter("@ExternalId", externalId));
        }

        public static Category GetCategory(string url)
        {
            var categoryId = GetCategoryId(url);
            return categoryId == null ? null : GetCategory(categoryId.Value);
        }

        public static Category GetCategoryFromReader(SqlDataReader reader)
        {
            return new Category
            {
                CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                ExternalId = SQLDataHelper.GetString(reader, "ExternalId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                ProductsCount = SQLDataHelper.GetInt(reader, "Products_Count"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                Hidden = SQLDataHelper.GetBoolean(reader, "Hidden"),
                DisplayStyle = (ECategoryDisplayStyle)SQLDataHelper.GetInt(reader, "DisplayStyle"),
                DisplayChildProducts = SQLDataHelper.GetBoolean(reader, "DisplayChildProducts"),
                DisplayBrandsInMenu = SQLDataHelper.GetBoolean(reader, "DisplayBrandsInMenu"),
                DisplaySubCategoriesInMenu = SQLDataHelper.GetBoolean(reader, "DisplaySubCategoriesInMenu"),
                TotalProductsCount = SQLDataHelper.GetInt(reader, "Total_Products_Count"),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                ParentsEnabled = SQLDataHelper.GetBoolean(reader, "HirecalEnabled"),
                Sorting = (ESortOrder)SQLDataHelper.GetInt(reader, "Sorting"),
                Available_Products_Count = SQLDataHelper.GetInt(reader, "Available_Products_Count"),
                Current_Products_Count = SQLDataHelper.GetInt(reader, "Current_Products_Count"),
                AutomapAction = (ECategoryAutomapAction)SQLDataHelper.GetInt(reader, "AutomapAction"),
                ModifiedBy = SQLDataHelper.GetString(reader, "ModifiedBy"),
                ShowOnMainPage = SQLDataHelper.GetNullableBoolean(reader, "ShowOnMainPage") ?? false
            };
        }

        /// <summary>
        /// get category by categoryId from DB
        /// </summary>
        public static Category GetCategoryFromDbByCategoryId(int categoryId)
        {
            return SQLDataAccess.ExecuteReadOne(@"SELECT TOP 1 * FROM [Catalog].[Category] WHERE CategoryId = @CategoryId",
                CommandType.Text, GetCategoryFromReader, new SqlParameter("@CategoryId", categoryId));
        }

        public static List<Category> GetCategoriesByCategoryIds(List<int> categoryIds)
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Catalog].[Category] WHERE CategoryId in (" + String.Join(",", categoryIds) + ")", 
                CommandType.Text, GetCategoryFromReader);
        }

        public static List<int> GetParentCategoriesIdsByChildName(string categoryChildName)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT DISTINCT ParentCategory FROM [Catalog].[Category] WHERE Name like '%' + @Name + '%'",
                CommandType.Text, 
                (reader) => SQLDataHelper.GetInt(reader, "ParentCategory"),
                new SqlParameter("@Name", categoryChildName));
        }

        public static int? GetCategoryId(string url)
        {
            return SQLDataAccess.ExecuteReadOne<int?>("SELECT TOP 1 CategoryId FROM [Catalog].[Category] WHERE UrlPath = @UrlPath",
                CommandType.Text, (reader) => SQLDataHelper.GetNullableInt(reader, "CategoryId"), new SqlParameter("@UrlPath", url));
        }

        /// <summary>
        /// update category
        /// </summary>
        public static bool UpdateCategory(Category category, bool updateCache, bool trackChanges = false, ChangedBy changedBy = null)
        {
            var oldCat = GetCategoryFromDbByCategoryId(category.CategoryId);

            if (trackChanges)
                CategoryHistoryService.TrackCategoryChanges(category, oldCat, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                "[Catalog].[sp_UpdateCategory]", CommandType.StoredProcedure,
                new SqlParameter("@CategoryID", category.CategoryId),
                new SqlParameter("@ExternalId", category.ExternalId ?? (object)DBNull.Value),
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@ParentCategory", category.ParentCategoryId),
                new SqlParameter("@Description", category.Description),
                new SqlParameter("@BriefDescription", category.BriefDescription),
                new SqlParameter("@Enabled", category.Enabled),
                new SqlParameter("@Hidden", category.Hidden),
                new SqlParameter("@DisplayStyle", category.DisplayStyle),
                new SqlParameter("@displayChildProducts", category.DisplayChildProducts),
                new SqlParameter("@DisplayBrandsInMenu", category.DisplayBrandsInMenu),
                new SqlParameter("@DisplaySubCategoriesInMenu", category.DisplaySubCategoriesInMenu),
                new SqlParameter("@SortOrder", category.SortOrder),
                new SqlParameter("@UrlPath", category.UrlPath),
                new SqlParameter("@Sorting", category.Sorting),
                new SqlParameter("@AutomapAction", category.AutomapAction),
                new SqlParameter("@ModifiedBy", category.ModifiedBy ?? (object)DBNull.Value),
                new SqlParameter("@ShowOnMainPage", category.ShowOnMainPage)
                );

            if (category.Meta != null)
            {
                if (category.Meta.Title.IsNullOrEmpty() && category.Meta.MetaKeywords.IsNullOrEmpty() && category.Meta.MetaDescription.IsNullOrEmpty() && category.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(category.CategoryId, MetaType.Category))
                        MetaInfoService.DeleteMetaInfo(category.CategoryId, MetaType.Category);
                }
                else
                    MetaInfoService.SetMeta(category.Meta);
            }

            if (category.Tags != null)
            {
                var tags = category.Tags;
                TagService.DeleteMap(category.CategoryId, ETagType.Category);
                for (var i = 0; i < tags.Count; i++)
                {
                    var tag = tags[i].Id != 0
                        ? TagService.Get(tags[i].Id)
                        : TagService.Get(tags[i].Name);

                    tags[i].Id = tag == null ? TagService.Add(tags[i]) : tag.Id;
                    TagService.AddMap(category.CategoryId, tags[i].Id, ETagType.Category, i * 10);
                }
            }

            if (category.SizeChart != null)
                SizeChartService.AddUpdateMap(category.SizeChart.Id, category.CategoryId, ESizeChartEntityType.Category);
            else
                SizeChartService.DeleteMap(category.CategoryId, ESizeChartEntityType.Category);

            if (updateCache)
            {
                SetCategoryHierarchicallyEnabled(category.CategoryId);

                CategoryWriter.AddUpdate(category);

                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(category.CategoryId));
                CacheManager.RemoveByPattern(CacheNames.ChildCategories);
                CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
                CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);
                CacheManager.RemoveByPattern(CacheNames.SQLPagingItems);
                CacheManager.RemoveByPattern(CacheNames.SQLPagingCount);
                CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName() + "TopMenu_Mobile");
                CacheManager.RemoveByPattern(CacheNames.GetMainMenuAuthCacheObjectName() + "TopMenu_Mobile");

                if (category.ParentCategoryId == 0)
                {
                    CacheManager.RemoveByPattern(CacheNames.GetMenuCacheObjectName(EMenuType.Bottom));
                }
                
                if (category.Hidden != oldCat.Hidden || category.Enabled != oldCat.Enabled)
                {
                    CalculateHasProductsForAllWarehouseInAllCategories();
                    
                    CacheManager.RemoveByPattern(CacheNames.BrandsInCategory + category.CategoryId + "_" + category.DisplayChildProducts);
                }

                if (oldCat.Hidden != category.Hidden)
                    ProductWriter.CreateIndexByCategoryInTask(category.CategoryId);
            }

            if (!string.IsNullOrEmpty(category.UrlPath) && category.UrlPath != oldCat.UrlPath && category.CategoryId == 0)
            {
                ApplicationService.UpdateRoutes();
            }

            return true;
        }

        public static bool UpdateCategorySortOrder(string name, int sortOrder, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Catalog.Category set name=@name, SortOrder=@SortOrder where CategoryID = @CategoryID",
                CommandType.Text,
                new SqlParameter("@name", name),
                new SqlParameter("@SortOrder", sortOrder),
                new SqlParameter("@CategoryID", categoryId));

            CacheManager.RemoveByPattern(CacheNames.ChildCategories);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);
            CacheManager.RemoveByPattern(CacheNames.GetMenuCacheObjectName(EMenuType.Bottom));

            return true;
        }

        /// <summary>
        /// Расчет кол-ва товаров во всех категориях
        /// Warning!! Very heavy function!
        /// 
        /// Рассчитываемые поля Category: 
        /// Products_Count - кол-во активных не скрытых с учетом подкатегорий,
        /// Total_Products_Count - кол-во всех товаров с учетом подкатегорий,
        /// Available_Products_Count - кол-во товаров доступных к покупке с учетом подкатегорий (активных, не скрытых, есть кол-во или под заказ),
        /// Current_Products_Count - кол-во активных не скрытых в данной категории,
        /// CatLevel - уровень категории
        /// </summary>
        public static void RecalculateProductsCountManual()
        {
            try
            {
                SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_RecalculateProductsCount]",
                                               CommandType.StoredProcedure);
                ClearCategoryCache();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }
        
        /// <summary>
        /// Расчет кол-ва товаров по категориям товара
        /// </summary>
        public static void RecalculateProductsCountInCategories(int productId)
        {
            try
            {
                SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_RecalculateProductsCountInCategoriesByProduct]",
                    CommandType.StoredProcedure,
                    new SqlParameter("@ProductId", productId));
                
                CacheManager.RemoveByPattern(CacheNames.Category);
                CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }
        
        /// <summary>
        /// Нужно ли пересчитывать кол-во товаров в категориях при изменении товара
        /// </summary>
        public static bool IsNeedRecalculateProductsCountInCategories(Product productBefore, Product productAfter, List<Offer> offersBefore, List<Offer> offersAfter)
        {
            var before =
                productBefore.Enabled
                && !productBefore.Hidden
                && (productBefore.AllowPreOrder || offersBefore.Any(x => x.BasePrice > 0 && x.Amount > 0));
            
            var after = 
                productAfter.Enabled 
                && !productAfter.Hidden 
                && (productAfter.AllowPreOrder || offersAfter.Any(x => x.BasePrice > 0 && x.Amount > 0));

            return before != after;
        }

        /// <summary>
        /// clear all categories in cache
        /// </summary>
        public static void ClearCategoryCache()
        {
            CacheManager.RemoveByPattern(CacheNames.Category);
            CacheManager.RemoveByPattern(CacheNames.SQLPagingItems);
            CacheManager.RemoveByPattern(CacheNames.SQLPagingCount);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);
            CacheManager.RemoveByPattern(CacheNames.GetProductRelatedProductFromCategoryCacheObjectName());
        }

        public static void ClearCategoryCache(int id)
        {
            var category = GetCategory(id);
            if (category == null)
                return;
            
            CategoryWriter.AddUpdate(category);
            CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(category.CategoryId));
            CacheManager.RemoveByPattern(CacheNames.ChildCategories);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);
            CacheManager.RemoveByPattern(CacheNames.GetProductRelatedProductFromCategoryCacheObjectName());

            if (category.ParentCategoryId == 0)
            {
                CacheManager.RemoveByPattern(CacheNames.GetMenuCacheObjectName(EMenuType.Bottom));
            }
        }
        
        /// <summary>
        /// Посчитать есть ли доступные товары для склада в категориях товара
        /// </summary>
        public static void CalculateHasProductsForWarehouseInProductCategories(int offerId, int warehouseId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_CalcHasProductsForWarehouseInProductCategories]", 
                CommandType.StoredProcedure, 
                new SqlParameter("@OfferId", offerId),
                new SqlParameter("@WarehouseId", warehouseId)
            );
            
            RemoveCacheAfterCalculateHasProducts();
        }
        
        /// <summary>
        /// Посчитать есть ли доступные товары для всех складов в категориях товара
        /// </summary>
        public static void CalculateHasProductsForWarehousesInProductCategories(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_CalcHasProductsForAllWarehousesInProductCategories]", 
                CommandType.StoredProcedure, 
                new SqlParameter("@ProductId", productId)
            );

            RemoveCacheAfterCalculateHasProducts();
        }
        
        /// <summary>
        /// Посчитать есть ли доступные товары для всех складов во всех категориях
        /// </summary>
        public static void CalculateHasProductsForAllWarehouseInAllCategories()
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_CalcHasProductsForAllWarehousesInAllCategories]", 
                CommandType.StoredProcedure, 60 * 3
            );

            RemoveCacheAfterCalculateHasProducts();
        }

        private static void RemoveCacheAfterCalculateHasProducts()
        {
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);
        }

        public static bool GetCategoryWarehouseHasProducts(int categoryId, int warehouseId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "Select HasProducts From Catalog.Category_Warehouse Where CategoryId = @CategoryId and WarehouseId = @WarehouseId",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@WarehouseId", warehouseId));
        }

        /// <summary>
        /// get total count of products
        /// </summary>
        public static int GetTolatCounTofProducts()
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTofTotalProducts]", CommandType.StoredProcedure);
        }

        /// <summary>
        /// get total count of products without categories
        /// </summary>
        public static int GetTolatCounTofProductsWithoutCategories()
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTofProductsWithoutCategories]", CommandType.StoredProcedure);
        }

        /// <summary>
        /// get total count of products in categories
        /// </summary>
        public static int GetTolatCounTofProductsInCategories()
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTofProductsInCategories]", CommandType.StoredProcedure);
        }

        public static int GetEnabledProductsCountInCategory(int catId, bool displayChildProducts)
        {
            var comand = displayChildProducts
                ? "SELECT Count(Product.ProductID) " +
                  "FROM [Catalog].[Product] " +
                  "INNER JOIN [Catalog].[ProductCategories] on ProductCategories.ProductID = Product.ProductID " +
                  "WHERE [ProductCategories].CategoryID  IN (SELECT id FROM [Settings].[GetChildCategoryByParent](@categoryId)) AND [Product].[Enabled] = 1 AND [Product].[CategoryEnabled] = 1"
                  
                : "SELECT Count(Product.ProductID) " +
                  "FROM [Catalog].[Product] " +
                  "INNER JOIN [Catalog].[ProductCategories] on ProductCategories.ProductID = Product.ProductID " +
                  "WHERE [ProductCategories].CategoryID = @categoryId AND [Product].[Enabled] = 1 AND [Product].[CategoryEnabled] = 1";
            
            return SQLDataAccess.ExecuteScalar<int>(comand, CommandType.Text, new SqlParameter("@categoryId", catId));
        }

        /// <summary>
        /// delete relationship between category and product
        /// </summary>
        public static void DeleteCategoryAndLink(int prodId, int categId)
        {
            ProductService.DeleteProductLink(prodId, categId);
        }

        /// <summary>
        /// delete category and photo of category
        /// </summary>
        public static void DeleteCategoryAndPhotos(int categoryId)
        {
            foreach (var id in DeleteCategory(categoryId, true))
            {
                PhotoService.DeletePhotos(id, PhotoType.CategoryBig);
                PhotoService.DeletePhotos(id, PhotoType.CategorySmall);
                PhotoService.DeletePhotos(id, PhotoType.CategoryIcon);
                SizeChartService.DeleteMap(id, ESizeChartEntityType.Category);
            }
            CacheManager.RemoveByPattern(CacheNames.GetMenuCacheObjectName(EMenuType.Bottom));
        }

        /// <summary>
        /// delete all relationships with products
        /// </summary>
        public static void DeleteCategoryLink(int catId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteCategoryLinks]", CommandType.StoredProcedure, new SqlParameter("@CategoryID", catId));

            ProductService.UpdateProductMainCategoryId(catId);
            
            ClearCategoryCache();
        }

        public static bool IsExistCategory(int catId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTCategoryByID]", CommandType.StoredProcedure, new SqlParameter("@CategoryID", catId)) > 0;
        }

        
        private const string GetPriceRangeQuery =
            @"
		SELECT 
				min(((MinPrice - isnull(Product.DiscountAmount,0))* Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as minprice,
                max(((MaxPrice - isnull(Product.DiscountAmount,0))* Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as maxprice
		FROM Catalog.Product 
		Left Join Catalog.ProductExt on Product.ProductID = ProductExt.ProductID		
		{0} 
		INNER JOIN Catalog.Currency ON Catalog.Currency.CurrencyID = Catalog.Product.CurrencyID
		
		WHERE Product.Enabled = 1 
		        and product.CategoryEnabled = 1 
		        {1} 
		        and ProductExt.MinPrice > 0 
		        and ProductExt.MaxPrice > 0
";
        
        public static KeyValuePair<float, float> GetPriceRange(int categoryId, 
                                                                bool indepth, 
                                                                EProductOnMain? productOnMainType, 
                                                                int? productListId,
                                                                List<int> productIds,
                                                                bool onlyAvailable,
                                                                List<int> warehouseIds)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>();

            if (categoryId != 0)
            {
                queryInners.Add(
                    indepth
                        ? "Inner Join Catalog.ProductCategories on ProductCategories.ProductID = Product.ProductID and ProductCategories.CategoryID in (Select id From Settings.GetChildCategoryByParent(@CategoryID))"
                        : "Inner Join Catalog.ProductCategories on ProductCategories.ProductID = Product.ProductID and ProductCategories.CategoryID = @CategoryID");
                queryParams.Add(new SqlParameter("@CategoryID", categoryId));
            }
            
            if (productOnMainType != null)
            {
                switch (productOnMainType)
                {
                    case EProductOnMain.New:
                        queryWheres.Add("New = 1");
                        break;
                    case EProductOnMain.Best:
                        queryWheres.Add("Bestseller = 1");
                        break;
                    case EProductOnMain.Sale:
                        queryWheres.Add("(Discount > 0 or DiscountAmount > 0)");
                        break;
                    case EProductOnMain.NewArrivals:
                        queryInners.Add(
                            "Inner Join (Select top(1000) ProductId From Catalog.Product Order by ProductId desc) ids on Product.ProductId = ids.ProductId"
                        );
                        break;
                }
            }

            if (productListId != null && productListId != 0)
            {
                queryInners.Add(
                    "Inner Join [Catalog].[Product_ProductList] plist on plist.ProductId = Product.ProductId and plist.ListId = @ListId");
                queryParams.Add(new SqlParameter("@ListId", productListId));
            }

            if (productIds != null && productIds.Count > 0)
            {
                queryInners.Add(
                    "Inner Join (Select value From [Settings].[SPLIT_INT](@ProductIds, ',')) ids on Product.ProductId = ids.value"
                );
                queryParams.Add(new SqlParameter("@ProductIds", string.Join(",", productIds)));
            }

            if (onlyAvailable)
                queryWheres.Add("(MaxAvailable > 0 OR [Product].[AllowPreOrder] = 1)");
            
            if (warehouseIds != null)
            {
                queryWheres.Add(
                    "(Product.AllowPreOrder = 1 " +
                    "   OR Exists(" +
                    "       Select 1 from [Catalog].[Offer] " +
                    "       Inner Join [Catalog].[WarehouseStocks] on [Offer].[OfferID] = [WarehouseStocks].[OfferId] " +
                    "       Where [WarehouseStocks].[WarehouseId] in (Select value From [Settings].[SPLIT_INT](@warehouseIds, ',')) " +
                    "                AND Offer.ProductId = Product.ProductID " +
                    "                AND [WarehouseStocks].[Quantity] > 0))"
                );
                queryParams.Add(new SqlParameter("@warehouseIds", string.Join(",", warehouseIds)));
            }
            
            var sql = string.Format(GetPriceRangeQuery, 
                queryInners.AggregateString(" "), 
                queryWheres.Count > 0 ? " and " + queryWheres.AggregateString(" and ") : null);

            return SQLDataAccess.ExecuteReadOne(
                sql,
                CommandType.Text,
                reader =>
                {
                    var min = SQLDataHelper.GetFloat(reader, "minprice");
                    var max = SQLDataHelper.GetFloat(reader, "maxprice");

                    return new KeyValuePair<float, float>(min < 0 ? 0 : min, max < 0 ? 0 : max);
                },
                queryParams.ToArray()
            );
        }

        public static int? GetChildCategoryIdByName(int parentId, string name)
        {
            return
                CacheManager.Get(CacheNames.GetCategoryIdByNameAndParentId(name, parentId), 2,
                    () => SQLDataAccess.ExecuteReadOne(
                        "SELECT TOP 1 CategoryID FROM [Catalog].[Category] WHERE [Name] = @Name AND [ParentCategory] = @ParentID",
                        CommandType.Text,
                        reader => SQLDataHelper.GetNullableInt(reader, "CategoryID"),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@ParentID", parentId)));
        }

        public static int GetCategoryIdByName(string name)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT Top 1 CategoryID FROM [Catalog].[Category] WHERE [Name] = @Name",
                CommandType.Text,
                new SqlParameter("@Name", name));
        }

        public static string GetCategoryStringByProductId(int productId, string separator, bool onlySort = false, bool onlyMainCategory = false)
        {
            Dictionary<int, int> categoryDict = new Dictionary<int, int>();

            if (onlyMainCategory)
            {
                categoryDict = SQLDataAccess.ExecuteReadDictionary<int, int>(
                "Select Top 1 CategoryID, SortOrder from Catalog.ProductCategories where ProductID=@id and Main=1", CommandType.Text,
                "CategoryID", "SortOrder",
                new SqlParameter("@id", productId));
            }
                
            else
                categoryDict = SQLDataAccess.ExecuteReadDictionary<int, int>(
                "Select CategoryID, SortOrder from Catalog.ProductCategories where ProductID=@id order by Main Desc", CommandType.Text,
                "CategoryID", "SortOrder",
                new SqlParameter("@id", productId));

            if (onlySort)
            {
                var resSort = categoryDict.Select(
                    pair =>
                    String.Format("{0}", pair.Value)).AggregateString(separator);
                return resSort;
            }

            var res = categoryDict.Select(
                pair =>
                String.Format("[{0}]", GetParentCategoriesAsString(pair.Key))).AggregateString(",");
            return res;
        }

        public static List<KeyValuePair<string, string>> GetCategoriesPathAndSort(int productId)
        {
            var categoryDict = SQLDataAccess.ExecuteReadDictionary<int, int>(
                "Select CategoryID, SortOrder from Catalog.ProductCategories where ProductID=@id order by Main Desc", CommandType.Text,
                "CategoryID", "SortOrder",
                new SqlParameter("@id", productId));

            return categoryDict.Select(pair => new KeyValuePair<string, string>(GetParentCategoriesAsString(pair.Key), pair.Value.ToString())).ToList();
        }

        public static string GetCategoryIdByProductId(int productId, string separator)
        {
            var categoryDict = SQLDataAccess.ExecuteReadDictionary<int, int>(
                "Select CategoryID, SortOrder from Catalog.ProductCategories where ProductID=@id order by Main Desc", CommandType.Text,
                "CategoryID", "SortOrder",
                new SqlParameter("@id", productId));

            var res = categoryDict.Select(
                pair =>
                pair.Key).AggregateString(",");
            return res;
        }

        private static string GetParentCategoriesAsString(int childCategoryId)
        {
            var res = new StringBuilder();
            var categoies = GetParentCategories(childCategoryId);
            for (var i = categoies.Count - 1; i >= 0; i--)
            {
                if (i != categoies.Count - 1)
                {
                    res.Append(" >> ");
                }
                res.Append(categoies[i].Name);
            }
            return res.ToString();
        }

        private static bool SubParseAndCreateCategory(int productId, string categoryHierarchy, int? sorting, bool isMain, Dictionary<int, int> oldSorting, 
                                                        ChangedBy changedBy, bool trackChanges = false, bool addProductInParentCategory = false, bool isNewProduct = false)
        {
            // categoryHierarchy "Техника >> Игровые приставки >> PlayStation"
            
            var categories = categoryHierarchy.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries).Select(name => name.SupperTrim()).ToArray();
            if (categories.Any(c => c.IsNullOrEmpty()))
                return false;

            var parentId = 0;
            
            for (var i = 0; i < categories.Length; i++)
            {
                var categoryId = GetChildCategoryIdByName(parentId, categories[i]);
                if (categoryId != null)
                    parentId = categoryId.Value;
                else
                {
                    var category = new Category
                    {
                        Name = categories[i],
                        ParentCategoryId = parentId,
                        SortOrder = 0,
                        Enabled = true,
                        DisplayChildProducts = false,
                        UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Category, categories[i]),
                        DisplayStyle = ECategoryDisplayStyle.Tile,
                        ModifiedBy = changedBy?.Name
                    };
                    parentId = AddCategory(category, false, false, trackChanges, changedBy);
                }

                if (addProductInParentCategory && parentId != 0 && i != categories.Length - 1)
                    ProductService.AddProductLink(productId, parentId, 0, false, incrementProductsCount: false, trackChanges: trackChanges && !isNewProduct, changedBy: changedBy);
                
                if (i == categories.Length - 1 && parentId != 0)
                {
                    var sortOrder = !sorting.HasValue && oldSorting.TryGetValue(parentId, out var value) ? value : sorting ?? 0;

                    ProductService.AddProductLink(productId, parentId, sortOrder, false, incrementProductsCount: false, trackChanges: trackChanges && !isNewProduct, changedBy: changedBy);
                    if (isMain)
                        ProductService.SetMainLink(productId, parentId, trackChanges && !isNewProduct, changedBy);
                }
            }

            return true;
        }

        public static void SubParseAndCreateCategories(int productId, List<string> categories, List<string> sortings, ChangedBy changedBy, bool trackChanges = false, bool addProductInParentCategory = false, bool isNewProduct = false)
        {
            if (categories == null || !categories.Any())
                return;

            var oldSorting = !isNewProduct
                ? ProductService.GetCategoriesSortingByProductId(productId)
                : new Dictionary<int, int>();
            
            if (!isNewProduct)
                ProductService.DeleteAllProductLink(productId);

            bool isMain = true;
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].IsNullOrEmpty())
                    continue;
                var sorting = sortings.Count > i ? sortings[i].TryParseInt(isNullable: true) : (int?)null;
                
                if (!SubParseAndCreateCategory(productId, categories[i], sorting, isMain, oldSorting, changedBy, trackChanges, addProductInParentCategory, isNewProduct))
                    continue;
                
                isMain = false;
            }
        }

        public static void SubParseAndCreateCategory(string strCategory, int productId, string separator = ";", string sorting = "", ChangedBy changedBy = null, bool trackChanges = false, bool addProductInParentCategory = false, bool isNewProduct = false)
        {
            // strCategory "[Техника >> Игровые приставки >> PlayStation],[....]"
            
            if (strCategory.IsNullOrEmpty())
                return;

            var oldSorting = !isNewProduct
                ? ProductService.GetCategoriesSortingByProductId(productId)
                : new Dictionary<int, int>();

            if (!isNewProduct)
                ProductService.DeleteAllProductLink(productId);

            var sortCat = sorting.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            var isMainCategory = true;
            var cats = strCategory.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < cats.Length; i++)
            {
                if (cats[i].IsNullOrEmpty() || (cats[i] = cats[i].SupperTrim()).IsNullOrEmpty() || cats[i] == ",")
                    continue;
                
                var sort = sortCat.Length > i ? sortCat[i].TryParseInt(isNullable: true) : (int?)null;
                
                if (!SubParseAndCreateCategory(productId, cats[i], sort, isMainCategory, oldSorting, changedBy, trackChanges, addProductInParentCategory, isNewProduct))
                    continue;
                
                isMainCategory = false;
            }
        }
        
        public static void AddProductToExternalCategories(string externalCategoryIdsStr, int productId, string separator = ";", string sortingStr = "", bool isAddingNew = false)
        {
            if (externalCategoryIdsStr.IsNullOrEmpty())
                return;

            var sorting = sortingStr.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            var oldSorting = ProductService.GetExternalCategoriesSortingByProductId(productId).ToLookup(x => x.Key, x => x.Value);

            ProductService.DeleteAllProductLink(productId);

            var externalIds = externalCategoryIdsStr.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < externalIds.Length; i++)
            {
                var productSortOrder = sorting.Length > i
                    ? sorting[i].TryParseInt()
                    : oldSorting[externalIds[i]].FirstOrDefault();

                ProductService.AddProductLinkByExternalCategoryId(productId, externalIds[i], productSortOrder, false);
            }
        }

        public static void SetActive(int categoryId, bool active, bool updateCache = false)
        {
            if (categoryId == 0 && active == false)
                return;
            
            SQLDataAccess.ExecuteNonQuery(
                 "Update [Catalog].[Category] Set Enabled = @Enabled Where CategoryID = @CategoryID",
                 CommandType.Text,
                 new SqlParameter("@CategoryID", categoryId),
                 new SqlParameter("@Enabled", active));

            if (updateCache)
            {
                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(categoryId));
                CacheManager.RemoveByPattern(CacheNames.ChildCategories);
                CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
                CacheManager.RemoveByPattern(CacheNames.CategoriesOnMainPage);
                SetCategoryHierarchicallyEnabled(categoryId);
                CalculateHasProductsForAllWarehouseInAllCategories();
            }
        }

        [Obsolete]
        /// <summary>
        /// for elbuz import
        /// </summary>
        public static int SubParseAndCreateCategory(string strCategory, bool updateCache = false)
        {
            int categoryId = -1;
            //
            // strCategory "[Техника >> Игровые приставки >> PlayStation]-10;[....]" (10 - порядок сортировки товара в категории(* необязательно))
            //
            foreach (string strT in strCategory.Split(new[] { ';' }))
            {
                var st = strT;
                st = st.Replace("[", "");
                st = st.Replace("]", "");
                int parentId = 0;
                string[] temp = st.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i <= temp.Length - 1; i++)
                {
                    string name = temp[i].SupperTrim();
                    if (!String.IsNullOrEmpty(name))
                    {
                        var cat = GetChildCategoryIdByName(parentId, name);
                        if (cat.HasValue)
                        {
                            parentId = cat.Value;
                            categoryId = cat.Value;
                        }
                        else
                        {
                            parentId = AddCategory(
                                new Category
                                {
                                    Name = name,
                                    ParentCategoryId = parentId,
                                    SortOrder = 0,
                                    Enabled = true,
                                    DisplayChildProducts = false,
                                    UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Category, name.Reduce(140)),
                                    DisplayStyle = ECategoryDisplayStyle.Tile,
                                    Meta = MetaInfoService.GetDefaultMetaInfo(MetaType.Category, name)
                                },
                                updateCache, false, null);
                        }
                    }
                    if (i == temp.Length - 1)
                    {
                        categoryId = parentId;
                    }
                }
            }
            return categoryId;
        }



        public static int GetHierarchyProductsCount(int categoryId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                " SELECT COUNT([TProduct].[ProductId]) " +
                "FROM [Catalog].[Product] AS [TProduct] " +
                "WHERE (SELECT COUNT([OfferId]) FROM [Catalog].[Offer] WHERE [Price] > 0 AND [Amount] > 0 AND [OfferListId] = 6 AND [ProductId] = [TProduct].[ProductId]) > 0 " +
                    "AND (SELECT TOP(1) [Catalog].[ProductCategories].[CategoryId] FROM [Catalog].[ProductCategories] JOIN [Catalog].[Category] on [Category].[CategoryId] = [ProductCategories].[CategoryId] WHERE [ProductCategories].[ProductId] = [TProduct].[ProductId] AND [Enabled] = 1 AND [CategoryEnabled] = 1) IN (SELECT [ID] FROM [Settings].[GetChildCategoryByParent] (@CategoryId)) " +
                    "AND [Enabled] = 1 AND [CategoryEnabled] = 1", 
                CommandType.Text, 
                new SqlParameter("@CategoryID", categoryId));
        }

        public static bool FullEnabled(int categoryId)
        {
            bool fullEnabled = true;

            while (categoryId != 0)
            {
                var category = GetCategory(categoryId);
                categoryId = category.ParentCategoryId;

                fullEnabled &= category.Enabled;
            }

            return fullEnabled;
        }

        public static void SetCategoryHierarchicallyEnabled(int categoryId)
        {
            try
            {
                SQLDataAccess.ExecuteNonQuery("[Catalog].[SetCategoryHierarchicallyEnabled]",
                    CommandType.StoredProcedure, new SqlParameter("@CatParent", categoryId));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static string GetCategoryHierarchy(int categoryId)
        {
            return $"[{GetParentCategoriesAsString(categoryId)}]";
        }


        #region Related Categories

        public static List<int> GetRelatedCategoryIds(int categoryId, RelatedType type)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select RelatedCategoryId From Catalog.RelatedCategories " +
                "Where CategoryId=@CategoryId and RelatedType=@RelatedType",
                CommandType.Text,
                reader => SQLDataHelper.GetInt(reader, "RelatedCategoryId"),
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RelatedType", (int)type));
        }

        public static List<Category> GetRelatedCategories(int categoryId, RelatedType type)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select RelatedCategoryId, Name From Catalog.RelatedCategories " +
                "Left Join Catalog.Category On Category.CategoryId = RelatedCategories.RelatedCategoryId " +
                "Where RelatedCategories.CategoryId=@CategoryId and RelatedType=@RelatedType",
                CommandType.Text, reader => new Category()
                {
                    CategoryId = SQLDataHelper.GetInt(reader, "RelatedCategoryId"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                },
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RelatedType", (int)type));
        }

        public static void AddRelatedCategory(int categoryId, int relatedCategoryId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Catalog.RelatedCategories (CategoryId, RelatedCategoryId, RelatedType) Values (@CategoryId, @RelatedCategoryId, @RelatedType) ",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RelatedCategoryId", relatedCategoryId),
                new SqlParameter("@RelatedType", (int)type));
        }

        public static void DeleteRelatedCategory(int categoryId, int relatedCategoryId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Delete From Catalog.RelatedCategories Where CategoryId=@CategoryId and RelatedCategoryId=@RelatedCategoryId and RelatedType=@RelatedType",
                 CommandType.Text,
                 new SqlParameter("@CategoryId", categoryId),
                 new SqlParameter("@RelatedCategoryId", relatedCategoryId),
                 new SqlParameter("@RelatedType", (int)type));
        }

        public static void DeleteRelatedCategory(int categoryId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Delete From Catalog.RelatedCategories Where CategoryId=@CategoryId and RelatedType=@RelatedType",
                 CommandType.Text,
                 new SqlParameter("@CategoryId", categoryId),
                 new SqlParameter("@RelatedType", (int)type));
        }

        #endregion

        #region Related properties

        // related properties
        public static List<int> GetRelatedPropertyIds(int categoryId, RelatedType type, bool isSame)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select PropertyId From Catalog.RelatedProperties Where CategoryId=@CategoryId and RelatedType=@RelatedType and IsSame=@IsSame",
                CommandType.Text,
                reader => SQLDataHelper.GetInt(reader, "PropertyId"),
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RelatedType", (int)type),
                new SqlParameter("@IsSame", isSame));
        }

        public static List<RelatedProperty> GetRelatedProperties(int categoryId, RelatedType type)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select Property.PropertyId, Name, IsSame From Catalog.RelatedProperties " +
                "Left Join Catalog.Property On Property.PropertyId = RelatedProperties.PropertyId " +
                "Where CategoryId=@CategoryId and RelatedType=@RelatedType",
                CommandType.Text,
                reader => new RelatedProperty()
                {
                    PropertyId = SQLDataHelper.GetInt(reader, "PropertyId"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    IsSame = SQLDataHelper.GetBoolean(reader, "IsSame")
                },
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RelatedType", (int)type));
        }

        public static void AddRelatedProperty(int categoryId, int propertyId, RelatedType type, bool isSame)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Catalog.RelatedProperties (CategoryId, PropertyId, RelatedType, IsSame) Values (@CategoryId, @PropertyId, @RelatedType, @IsSame) ",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@PropertyId", propertyId),
                new SqlParameter("@RelatedType", (int)type),
                new SqlParameter("@IsSame", isSame));
        }

        public static void DeleteRelatedProperty(int categoryId, int propertyId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Delete From Catalog.RelatedProperties Where CategoryId=@CategoryId and PropertyId=@PropertyId and RelatedType=@RelatedType",
                 CommandType.Text,
                 new SqlParameter("@CategoryId", categoryId),
                 new SqlParameter("@PropertyId", propertyId),
                 new SqlParameter("@RelatedType", (int)type));
        }

        public static void DeleteRelatedProperties(int categoryId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Delete From Catalog.RelatedProperties Where CategoryId=@CategoryId and RelatedType=@RelatedType",
                 CommandType.Text,
                 new SqlParameter("@CategoryId", categoryId),
                 new SqlParameter("@RelatedType", (int)type));
        }

        // related property values
        public static List<PropertyValue> GetRelatedPropertyValues(int categoryId, RelatedType type)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select RelatedPropertyValues.PropertyValueId, Name, PropertyValue.Value, Property.PropertyId From Catalog.RelatedPropertyValues " +
                "Left Join Catalog.PropertyValue On PropertyValue.PropertyValueId = RelatedPropertyValues.PropertyValueId " +
                "Left Join Catalog.Property On Property.PropertyId = PropertyValue.PropertyId " +
                "Where CategoryId=@CategoryId and RelatedType=@RelatedType",
                CommandType.Text,
                reader => new PropertyValue()
                {
                    Property = new Property() { Name = SQLDataHelper.GetString(reader, "Name"), PropertyId = SQLDataHelper.GetInt(reader, "PropertyId") },
                    PropertyValueId = SQLDataHelper.GetInt(reader, "PropertyValueId"),
                    PropertyId = SQLDataHelper.GetInt(reader, "PropertyId"),
                    Value = SQLDataHelper.GetString(reader, "Value")
                },
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RelatedType", (int)type));
        }

        public static List<int> GetRelatedPropertyValuesIds(int categoryId, RelatedType type)
        {
            return SQLDataAccess.ExecuteReadList(
                "Select PropertyValueId From Catalog.RelatedPropertyValues Where CategoryId=@CategoryId and RelatedType=@RelatedType",
                CommandType.Text,
                reader => SQLDataHelper.GetInt(reader, "PropertyValueId"),
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RelatedType", (int)type));
        }

        public static void AddRelatedPropertyValue(int categoryId, int propertyValueId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Catalog.RelatedPropertyValues (CategoryId, PropertyValueId, RelatedType) Values (@CategoryId, @PropertyValueId, @RelatedType) ",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@PropertyValueId", propertyValueId),
                new SqlParameter("@RelatedType", (int)type));
        }

        public static void DeleteRelatedPropertyValue(int categoryId, int propertyValueId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Delete From Catalog.RelatedPropertyValues Where CategoryId=@CategoryId and PropertyValueId=@PropertyValueId and RelatedType=@RelatedType",
                 CommandType.Text,
                 new SqlParameter("@CategoryId", categoryId),
                 new SqlParameter("@PropertyValueId", propertyValueId),
                 new SqlParameter("@RelatedType", (int)type));
        }

        public static void DeleteRelatedPropertyValues(int categoryId, RelatedType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Delete From Catalog.RelatedPropertyValues Where CategoryId=@CategoryId and RelatedType=@RelatedType",
                 CommandType.Text,
                 new SqlParameter("@CategoryId", categoryId),
                 new SqlParameter("@RelatedType", (int)type));
        }

        #endregion

        #region Product Categories Automapping

        public static List<CategoryAutomap> GetAutomapCategories(int categoryId)
        {
            return SQLDataAccess.Query<CategoryAutomap>(
                "SELECT * FROM Catalog.CategoriesAutoMapping WHERE CategoryId = @CategoryId", new { categoryId }).ToList();
        }

        public static ECategoryAutomapAction GetCategoryAutomapAction(int categoryId)
        {
            return (ECategoryAutomapAction)SQLDataAccess.ExecuteScalar<int>("SELECT AutomapAction FROM Catalog.Category WHERE CategoryId = @CategoryId",
                CommandType.Text, new SqlParameter("@CategoryId", categoryId));
        }

        public static void SetCategoryAutomapAction(int categoryId, ECategoryAutomapAction automapAction)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE Catalog.Category SET AutomapAction = @AutomapAction WHERE CategoryId = @CategoryId", CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@AutomapAction", automapAction));
            CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(categoryId));
            CacheManager.RemoveByPattern(CacheNames.ChildCategories);
        }

        public static void AddAutomapCategory(CategoryAutomap categoryAutomap)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(CategoryId) FROM Catalog.CategoriesAutoMapping WHERE CategoryId = @CategoryId AND NewCategoryId = @NewCategoryId) = 0 " +
                    "INSERT INTO Catalog.CategoriesAutoMapping (CategoryId, NewCategoryId, Main) VALUES (@CategoryId, @NewCategoryId, @Main)",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryAutomap.CategoryId),
                new SqlParameter("@NewCategoryId", categoryAutomap.NewCategoryId),
                new SqlParameter("@Main", categoryAutomap.Main));
        }

        public static void UpdateAutomapCategory(CategoryAutomap categoryAutomap)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Catalog.CategoriesAutoMapping SET Main = @Main WHERE CategoryId = @CategoryId AND NewCategoryId = @NewCategoryId",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryAutomap.CategoryId),
                new SqlParameter("@NewCategoryId", categoryAutomap.NewCategoryId),
                new SqlParameter("@Main", categoryAutomap.Main));
        }

        public static void DeleteAutomapCategory(int categoryId, int automapCategoryId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.CategoriesAutoMapping WHERE CategoryId = @CategoryId AND NewCategoryId = @NewCategoryId",
                CommandType.Text, 
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@NewCategoryId", automapCategoryId));
        }

        public static void DeleteAutomapCategories(int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.CategoriesAutoMapping WHERE CategoryId = @CategoryId",
                CommandType.Text, new SqlParameter("@CategoryId", categoryId));
        }

        public static void ClearMainAutomapCategory(int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE Catalog.CategoriesAutoMapping SET Main = 0 WHERE Main = 1 AND CategoryId = @CategoryId",
                CommandType.Text, new SqlParameter("@CategoryId", categoryId));
        }

        #endregion

        public static DateTime? GetModifiedDate(int categoryId)
        {
            var date = SQLDataAccess.ExecuteScalar(
                "Select Top(1) DateModified From Catalog.Category Where CategoryId=@categoryId",
                CommandType.Text,
                new SqlParameter("@categoryId", categoryId));

            return date != null ? Convert.ToDateTime(date) : default(DateTime?);
        }

        public static List<Category> GetCategoriesOnMainPage(int count = 4, List<int> warehouseIds = null)
        {
            var warehouses = warehouseIds != null ? string.Join(",", warehouseIds) : null;
            
            return CacheManager.Get(
                CacheNames.CategoriesOnMainPage + count + "_" + warehouses + "_" + SettingsCatalog.ShowOnlyAvalible, 
                () =>
                    SQLDataAccess.ExecuteReadList(
                        "SELECT TOP (@Count) c.CategoryId, c.Name, c.SortOrder, c.UrlPath " +
                        "FROM [Catalog].[Category] c " +
                        "WHERE c.CategoryId <> 0 " +
                            "AND c.[Hidden] = 0 " +
                            "AND c.[Enabled] = 1 " +
                            "AND c.[ShowOnMainPage] = 1 " +
                            (warehouses != null 
                                ? " AND Exists(Select 1 " + 
                                              "From [Catalog].[Category_Warehouse] as cw " +
                                              "Where cw.CategoryId = c.CategoryId " +
                                                      "AND cw.WarehouseId in (" + warehouses + ") " +
                                                      "AND cw.HasProducts = 1" +
                                              ") " 
                                : null) +
                            (warehouseIds is null && SettingsCatalog.ShowOnlyAvalible
                                ? " AND c.Available_Products_Count > 0 " 
                                : "") +
                        "Order By c.[SortOrder] Asc",
                        CommandType.Text,
                        reader => new Category
                        {
                            CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                            Name = SQLDataHelper.GetString(reader, "Name"),
                            SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                            UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                        },
                        new SqlParameter("@Count", count)));
        }

        public static int GetAdaptiveRootCategoryId()
        {
            if (SettingsCatalog.UseAdaptiveRootCategory is false)
                return 0;

            return CacheManager.Get(CacheNames.MenuCatalog + "AdaptiveRootCategoryId", () =>
                SQLDataAccess.ExecuteReadOne(
                    @"SELECT TOP (1) parent.[CategoryID], parent.[ParentCategory], parent.Current_Products_Count
                                FROM [Catalog].[Category] as parent
                                WHERE parent.[Enabled] = 1
                                  AND parent.[Hidden] = 0
                                  AND parent.[HirecalEnabled] = 1
                                  AND ((SELECT Count(CategoryID)
                                        FROM [Catalog].[Category] AS c
                                        WHERE c.ParentCategory = parent.CategoryID
                                          AND c.[Enabled] = 1
                                          AND c.[Hidden] = 0
                                          AND c.[HirecalEnabled] = 1
                                          AND c.CategoryID <> 0) > 1
                                    OR Current_Products_Count > 0)
                                  AND CatLevel > 0
                                ORDER BY parent.CatLevel",
                    CommandType.Text, reader =>
                    {
                        var categoryId = SQLDataHelper.GetInt(reader, "CategoryID");
                        var parentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory");
                        var categoryCurrentProductsCount = SQLDataHelper.GetInt(reader, "Current_Products_Count");
                        return categoryCurrentProductsCount > 0 ? parentCategoryId : categoryId;
                    }));
        }

        #region Category Brand

        public static void AddCategoryBrand(int categoryId, int brandId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Catalog].[CategoryBrands] (BrandId, CategoryId)
                    VALUES (@BrandId, @CategoryId)", CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@BrandId", brandId));
        }

        public static void DeleteCategoryBrands(int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[CategoryBrands] WHERE CategoryId = @CategoryId", CommandType.Text, new SqlParameter("@CategoryId", categoryId));
        }

        public static List<Brand> GetCategoryBrands(int categoryId, bool onlyAvailable = false, int? count = null)
        {
            return SQLDataAccess.ExecuteReadList(
                $@"SELECT {(count.HasValue ? $"TOP({count})" : string.Empty)} map.BrandId, BrandName, UrlPath, SortOrder, PhotoName
                FROM [Catalog].[CategoryBrands] as map
                INNER JOIN [Catalog].[Brand] ON Brand.BrandId = map.BrandId
                Left Join Catalog.Photo on Photo.ObjId = Brand.BrandID and Type = @Type     
                WHERE CategoryId = @CategoryId",
                CommandType.Text,
                reader => new Brand
                {
                    BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                    Name = SQLDataHelper.GetString(reader, "BrandName"),
                    UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                    BrandLogo = new BrandPhoto() { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") },
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                },
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@Type", PhotoType.Brand.ToString()),
                new SqlParameter("@OnlyAvailable", onlyAvailable))
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();
        }

        #endregion
    }
}