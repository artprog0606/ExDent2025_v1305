using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class SizeService
    {
        private const string SizeCacheKey = "Size_";
        private const string SizeNameCacheKey = "SizeName_";

        public static Size GetSize(int? sizeId)
        {
            if (!sizeId.HasValue)
                return null;

            return
                CacheManager.Get(SizeCacheKey + sizeId.Value, () =>
                    SQLDataAccess.ExecuteReadOne("Select TOP 1 * from Catalog.Size where sizeID=@sizeID",
                        CommandType.Text, GetFromReader, new SqlParameter("@sizeID", sizeId)));
        }

        public static SizeForCategory GetSizeForCategory(int? sizeId, int categoryId)
        {
            if (!sizeId.HasValue)
                return null;
            return CacheManager.Get(SizeCacheKey + sizeId.Value + "Category_" + categoryId, () =>
                    SQLDataAccess.ExecuteReadOne(
                        @"Select TOP 1 Size.*, SizeNameForCategory from Catalog.Size
                        Left Join [Catalog].[Category_Size] On Size.SizeId = Category_Size.SizeId and Category_Size.CategoryId = @CategoryId
                        where Size.SizeId=@sizeID",
                        CommandType.Text, (reader) => new SizeForCategory
                        {
                            SizeId = SQLDataHelper.GetInt(reader, "SizeID"),
                            SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                            SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                            SizeNameForCategory = SQLDataHelper.GetString(reader, "SizeNameForCategory"),
                            CategoryId = categoryId
                        },
                        new SqlParameter("@sizeID", sizeId),
                        new SqlParameter("@CategoryId", categoryId)));
        }

        public static List<Size> GetAllSizes()
        {
            return SQLDataAccess.ExecuteReadList("Select * from Catalog.Size Order by SortOrder, SizeName", CommandType.Text, GetFromReader);
        }

        public static List<SizeForCategory> GetAllSizesForCategory(int categoryId)
        {
            return SQLDataAccess.ExecuteReadList(
                @"Select Size.*, SizeNameForCategory from Catalog.Size
                    Left Join [Catalog].[Category_Size] On Size.SizeId = Category_Size.SizeId and Category_Size.CategoryId = @CategoryId
                    Order by SortOrder, SizeName", CommandType.Text, (reader) => new SizeForCategory
                {
                    SizeId = SQLDataHelper.GetInt(reader, "SizeID"),
                    SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    SizeNameForCategory = SQLDataHelper.GetString(reader, "SizeNameForCategory"),
                    CategoryId = categoryId
                },
                new SqlParameter("@CategoryId", categoryId));
        }

        public static List<Size> GetAllSizesByPaging(int limit, int currentPage, string q)
        {
            var p = new Core.SQL2.SqlPaging(currentPage, limit)
                .Select("SizeId", "SizeName")
                .From("Catalog.Size")
                .OrderBy("SizeName");
            
            if (!string.IsNullOrWhiteSpace(q))
                p.Where("SizeName like '%' + {0} + '%'", q);

            return p.PageItemsList<Size>();
        }

        public static Size GetSize(string sizeName)
        {
            return
                CacheManager.Get(SizeNameCacheKey + sizeName, () =>
                    SQLDataAccess.ExecuteReadOne<Size>(
                        "Select Top 1 * from Catalog.Size where sizeName=@sizeName order by SortOrder",
                        CommandType.Text, GetFromReader, new SqlParameter("@sizeName", sizeName)));
        }

        private static Size GetFromReader(SqlDataReader reader)
        {
            return new Size()
            {
                SizeId = SQLDataHelper.GetInt(reader, "SizeID"),
                SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static int AddSize(Size size)
        {
            if (size == null)
                throw new ArgumentNullException("size");
            
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddSize]", CommandType.StoredProcedure,
                                                        new SqlParameter("@SizeName", size.SizeName),
                                                        new SqlParameter("@SortOrder", size.SortOrder)
                                                        );
        }

        public static void UpdateSize(Size size)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateSize]", CommandType.StoredProcedure,
                                                 new SqlParameter("@SizeId", size.SizeId),
                                                 new SqlParameter("@SizeName", size.SizeName),
                                                 new SqlParameter("@SortOrder", size.SortOrder));

            CacheManager.RemoveByPattern(SizeCacheKey + size.SizeId);
            CacheManager.RemoveByPattern(SizeNameCacheKey);
        }

        public static void DeleteSize(int sizeId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.Size WHERE SizeID = @SizeId", CommandType.Text, new SqlParameter("@SizeId", sizeId));
            CacheManager.RemoveByPattern(SizeCacheKey + sizeId);
            CacheManager.RemoveByPattern(SizeNameCacheKey);
        }
        
        public static List<SizeForCategory> GetSizesByCategoryId(int categoryId, bool inDepth, bool onlyAvailable, List<int> warehouseIds)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>()
            {
                new SqlParameter("@CategoryID", categoryId)
            };
            
            var query = 
@"
    Select top(150) size.*, [Category_Size].SizeNameForCategory 
    From Catalog.Size size 
    Left Join [Catalog].[Category_Size] On size.SizeId = Category_Size.SizeId and Category_Size.CategoryId = @CategoryId
    Where size.SizeID in (   
                            Select distinct SizeID  
                            From Catalog.Offer 
                            Inner Join Catalog.Product on Offer.ProductID=Product.ProductID 
                            Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]     
                            {0}   
                            Where Product.Enabled = 1 and Product.CategoryEnabled = 1 
                                  {1} 
                        ) 
    Order by size.SortOrder, size.SizeName  
";
            
            queryInners.Add(
                inDepth
                    ? categoryId != 0 
                        ? "Inner Join Catalog.ProductCategories on ProductCategories.ProductID = Product.ProductID and ProductCategories.CategoryID in (Select id From Settings.GetChildCategoryByParent(@CategoryID))" 
                        : ""
                    : "Inner Join Catalog.ProductCategories on ProductCategories.ProductID = Product.ProductID and ProductCategories.CategoryID = @CategoryID"
            );
            
            if (onlyAvailable)
                queryWheres.Add("(MaxAvailable > 0 OR [Product].[AllowPreOrder] = 1)");
            
            if (warehouseIds != null)
            {
                queryWheres.Add(
                    "(Product.AllowPreOrder = 1 " +
                    "   OR Exists(" +
                    "       Select 1 " +
                    "       From [Catalog].[WarehouseStocks] " +
                    "       Where [WarehouseStocks].[OfferId] = [Offer].[OfferID] " +
                    "               and [WarehouseStocks].[WarehouseId] in (Select value From [Settings].[SPLIT_INT](@warehouseIds, ',')) " +
                    "               and [WarehouseStocks].[Quantity] > 0)" +
                    ")"
                );
                queryParams.Add(new SqlParameter("@warehouseIds", string.Join(",", warehouseIds)));
            }
            
            var sql = string.Format(query, 
                queryInners.AggregateString(" "), 
                queryWheres.Count > 0 ? " and " + queryWheres.AggregateString(" and ") : null);
            
            return SQLDataAccess.ExecuteReadList(sql,
                CommandType.Text,
                (reader) => new SizeForCategory
                {
                    SizeId = SQLDataHelper.GetInt(reader, "SizeID"),
                    SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    SizeNameForCategory = SQLDataHelper.GetString(reader, "SizeNameForCategory"),
                    CategoryId = categoryId
                },
                queryParams.ToArray());
        }
        
        public static List<SizeForCategory> GetSizesByFilter(EProductOnMain? productOnMainType, 
                                                                int? productListId,
                                                                List<int> productIds,
                                                                bool onlyAvailable, 
                                                                List<int> warehouseIds)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>();
            
            var query = 
@"
    Select top(150) size.*  
    From Catalog.Size size 
    Where size.SizeID in (   
                            Select distinct SizeID  
                            From Catalog.Offer 
                            Inner Join Catalog.Product on Offer.ProductID=Product.ProductID 
                            Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]     
                            {0}   
                            Where Product.Enabled = 1 and Product.CategoryEnabled = 1 
                                  {1} 
                        ) 
    Order by size.SortOrder, size.SizeName  
";
            
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
                    "       Select 1 " +
                    "       From [Catalog].[WarehouseStocks] " +
                    "       Where [WarehouseStocks].[OfferId] = [Offer].[OfferID] " +
                    "               and [WarehouseStocks].[WarehouseId] in (Select value From [Settings].[SPLIT_INT](@warehouseIds, ',')) " +
                    "               and [WarehouseStocks].[Quantity] > 0)" +
                    ")"
                );
                queryParams.Add(new SqlParameter("@warehouseIds", string.Join(",", warehouseIds)));
            }
            
            var sql = string.Format(query, 
                queryInners.AggregateString(" "), 
                queryWheres.Count > 0 ? " and " + queryWheres.AggregateString(" and ") : null);
            
            return SQLDataAccess.ExecuteReadList(sql,
                CommandType.Text,
                (reader) => new SizeForCategory
                {
                    SizeId = SQLDataHelper.GetInt(reader, "SizeID"),
                    SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
                },
                queryParams.ToArray());
        }

        public static bool IsSizeUsed(int sizeId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                       @"IF EXISTS (Select * From Catalog.Offer WHERE SizeID = @SizeId)
                        SELECT 1
                        ELSE
                        SELECT 0",
                       CommandType.Text,
                       new SqlParameter("@SizeId", sizeId)) > 0;
        }

        public static void AddUpdateSizeForCategory(int categoryId, int sizeId, string sizeName)
        {
            SQLDataAccess.ExecuteNonQuery(
                       @"IF EXISTS (Select TOP(1) 1 From Catalog.Category_Size WHERE CategoryId = @CategoryId AND SizeID = @SizeId)
                            UPDATE Catalog.Category_Size SET SizeNameForCategory = @SizeName WHERE CategoryId = @CategoryId AND SizeID = @SizeId
                        ELSE
                            INSERT INTO Catalog.Category_Size (CategoryId, SizeId, SizeNameForCategory)
                                VALUES (@CategoryId, @SizeId, @SizeName)",
                       CommandType.Text,
                       new SqlParameter("@CategoryId", categoryId),
                       new SqlParameter("@SizeId", sizeId),
                       new SqlParameter("@SizeName", sizeName));
        }

        public static void DeleteSizeForCategory(int categoryId, int sizeId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.Category_Size WHERE CategoryId = @CategoryId AND SizeID = @SizeId",
                       CommandType.Text,
                       new SqlParameter("@CategoryId", categoryId),
                       new SqlParameter("@SizeId", sizeId));
        }
    }
}