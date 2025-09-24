using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Catalog
{
    public static class ProductListService
    {
        #region CRUD methods

        public static int Add(ProductList productList)
        {
            ClearCache();

            productList.Id = SQLDataAccess.ExecuteScalar<int>(
                @"Insert Into [Catalog].[ProductList] (
                    [Name]
                    ,[SortOrder]
                    ,[Enabled]
                    ,[Description]
                    ,[ShuffleList]
                    ,[ShowOnMainPage]
                    ,[UrlPath]) 
                Values (
                    @Name
                    ,@SortOrder
                    ,@Enabled
                    ,@Description
                    ,@ShuffleList
                    ,@ShowOnMainPage
                    ,@UrlPath);
                Select scope_identity();",
                CommandType.Text,
                new SqlParameter("@Name", productList.Name),
                new SqlParameter("@SortOrder", productList.SortOrder),
                new SqlParameter("@Enabled", productList.Enabled),
                new SqlParameter("@Description", productList.Description ?? ""),
                new SqlParameter("@ShuffleList", productList.ShuffleList),
                new SqlParameter("@ShowOnMainPage", productList.ShowOnMainPage),
                new SqlParameter("@UrlPath", productList.UrlPath ?? ""));
            
            if (string.IsNullOrEmpty(productList.UrlPath))
                SQLDataAccess.ExecuteNonQuery(
                    @"UPDATE
                        [Catalog].[ProductList]
                    SET 
                        [UrlPath] = @UrlPath
                    WHERE 
                        [Id] = @Id",
                    CommandType.Text,
                    new SqlParameter("@UrlPath", productList.Id.ToString()),
                    new SqlParameter("@Id", productList.Id));

            if (productList.Meta != null)
            {
                if (!productList.Meta.Title.IsNullOrEmpty() || !productList.Meta.MetaKeywords.IsNullOrEmpty() ||
                    !productList.Meta.MetaDescription.IsNullOrEmpty() || !productList.Meta.H1.IsNullOrEmpty())
                {
                    productList.Meta.ObjId = productList.Id;
                    MetaInfoService.SetMeta(productList.Meta);
                }
            }

            return productList.Id;
        }

        public static void Update(ProductList productList)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"Update [Catalog].[ProductList] 
                Set 
                      Name=@Name
                    , SortOrder=@SortOrder
                    , Enabled=@Enabled
                    , Description=@Description
                    , ShuffleList=@ShuffleList
                    , ShowOnMainPage=@ShowOnMainPage 
                    , UrlPath=@UrlPath
                Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", productList.Id),
                new SqlParameter("@Name", productList.Name),
                new SqlParameter("@SortOrder", productList.SortOrder),
                new SqlParameter("@Enabled", productList.Enabled),
                new SqlParameter("@Description", productList.Description ?? ""),
                new SqlParameter("@ShuffleList", productList.ShuffleList),
                new SqlParameter("@ShowOnMainPage", productList.ShowOnMainPage),
                new SqlParameter("@UrlPath", productList.UrlPath));

            // ---- Meta
            if (productList.Meta != null)
            {
                if (productList.Meta.Title.IsNullOrEmpty() && productList.Meta.MetaKeywords.IsNullOrEmpty() &&
                    productList.Meta.MetaDescription.IsNullOrEmpty() && productList.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(productList.Id, productList.MetaType))
                        MetaInfoService.DeleteMetaInfo(productList.Id, productList.MetaType);
                }
                else
                    MetaInfoService.SetMeta(productList.Meta);
            }

            ClearCache();
        }

        public static void UpdateSortOrder(int id, int sortOrder)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Catalog].[ProductList] Set SortOrder=@SortOrder Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@SortOrder", sortOrder));

            ClearCache();
        }

        public static void Delete(int productListId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete FROM [Catalog].[ProductList] WHERE Id=@Id", CommandType.Text,
                new SqlParameter("@Id", productListId));

            ClearCache();
        }

        private static ProductList GetProductListFromReader(SqlDataReader reader)
        {
            return new ProductList
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                ShuffleList = SQLDataHelper.GetBoolean(reader, "ShuffleList"),
                ShowOnMainPage = SQLDataHelper.GetBoolean(reader, "ShowOnMainPage"),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath")
            };
        }

        public static ProductList Get(int productListId)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM [Catalog].[ProductList] WHERE Id = @Id",
                CommandType.Text,
                GetProductListFromReader, new SqlParameter("@Id", productListId));
        }
        
        public static ProductList GetByPath(string urlPath) => 
            string.IsNullOrEmpty(urlPath)
            ? new ProductList() 
            : SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM [Catalog].[ProductList] WHERE UrlPath = @UrlPath",
                CommandType.Text,
                GetProductListFromReader, new SqlParameter("@UrlPath", urlPath));

        public static List<ProductList> GetList()
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Catalog].[ProductList] Order by SortOrder",
                CommandType.Text,
                GetProductListFromReader);
        }

        public static int GetCount()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT Count(*) FROM [Catalog].[ProductList]", CommandType.Text);
        }
        
        public static List<ProductList> GetMainPageList(bool withShowOnMainPage = true)
        {
            if (withShowOnMainPage)
                return CacheManager.Get(CacheNames.ProductList + "MainPage",
                    () =>
                        SQLDataAccess.ExecuteReadList(
                            "SELECT * FROM [Catalog].[ProductList] " +
                            "WHERE Enabled = 1 and ShowOnMainPage = 1 and Exists(Select 1 From [Catalog].[Product_ProductList] Where [Product_ProductList].[ListId] = [ProductList].[Id]) " +
                            "Order By SortOrder", CommandType.Text, GetProductListFromReader));
            else
                return SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Catalog].[ProductList] " +
                    "WHERE Enabled = 1 " +
                    "and Exists(Select 1 From [Catalog].[Product_ProductList] Where [Product_ProductList].[ListId] = [ProductList].[Id]) " +
                    "Order By SortOrder", CommandType.Text, GetProductListFromReader);
        }
        
        #endregion

        #region Product List mapping

        public static int AddProduct(int listId, int productId, int sort)
        {
            ClearCache();

            return SQLDataAccess.ExecuteScalar<int>(
                "Insert Into [Catalog].[Product_ProductList] ([ListId],[ProductId],[SortOrder]) Values (@ListId,@ProductId,@SortOrder)",
                CommandType.Text,
                new SqlParameter("@ListId", listId),
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SortOrder", sort));
        }

        public static void UpdateProduct(int listId, int productId, int sort)
        {
            SQLDataAccess.ExecuteScalar<int>(
                "Update [Catalog].[Product_ProductList] Set SortOrder=@SortOrder Where ListId=@ListId and ProductId=@ProductId",
                CommandType.Text,
                new SqlParameter("@ListId", listId),
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SortOrder", sort));

            ClearCache();
        }

        public static void DeleteProduct(int listId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete FROM [Catalog].[Product_ProductList] WHERE ListId=@ListId and ProductId=@ProductId",
                CommandType.Text,
                new SqlParameter("@ListId", listId),
                new SqlParameter("@ProductId", productId));
        }

        public static List<int> GetProductIds(int listId, bool withPositiveSortOrder = false)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT ProductId FROM [Catalog].[Product_ProductList] WHERE ListId = @ListId" +
                (withPositiveSortOrder ? " AND SortOrder>=0" : string.Empty),
                CommandType.Text,
                reader => SQLDataHelper.GetInt(reader, "ProductId"),
                new SqlParameter("@ListId", listId));
        }

        public static List<ProductModel> GetProducts(int listId, int count, List<int> warehouseIds = null)
        {
            var warehouseSql = warehouseIds != null && warehouseIds.Count > 0 
                ? string.Join(",", warehouseIds) 
                : null;
            
            var query = 
                "Select Top(@Count) [Product].[ProductID], Product.BriefDescription, Product.ArtNo, Product.Name, Recomended as Recomend, Bestseller, New, OnSale as Sales, Discount, DiscountAmount, " +
                "Product.Enabled, Product.UrlPath, AllowPreOrder, Ratio, ManualRatio, Product.Multiplicity, Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, " +
                "CountPhoto, Photo.PhotoId,  PhotoName, PhotoNameSize1, PhotoNameSize2, Photo.Description as PhotoDescription, Offer.ColorID, Product.DateAdded, " +
                "null as AdditionalPhoto, Product.DoNotApplyOtherDiscounts, Product.MainCategoryId, " +
                (SettingsCatalog.ComplexFilter 
                    ? "Colors, NotSamePrices as MultiPrices, MinPrice as BasePrice, " 
                    : "null as Colors, 0 as MultiPrices, Price as BasePrice, ") +
                (warehouseSql != null ? "Offer.ColorID as PreSelectedColorId, " : "") +
                " Comments, CurrencyValue, Gifts " +
                (SettingsCatalog.MoveNotAvaliableToEnd ? ", AmountSort " : "") +
                "From [Catalog].[Product] " +
                "LEFT JOIN [Catalog].[Product_ProductList] ON [Product].[ProductID] = [Product_ProductList].[ProductId] " +
                "LEFT JOIN [Catalog].[ProductExt]  ON [Product].[ProductID] = [ProductExt].[ProductID]  " +
                (warehouseSql is null
                        ? "Left Join [Catalog].[Photo] On [Photo].[PhotoId] = [ProductExt].[PhotoId] And Type=@Type " +
                          "Left Join [Catalog].[Offer] On [ProductExt].[OfferID] = [Offer].[OfferID] "
                        : "Outer Apply (Select top (1) Offer.OfferID, Amount, ArtNo, ColorID, SizeID, Price " +
                                        "From [Catalog].[Offer] " +
                                        "Inner Join [Catalog].[WarehouseStocks] ON [Offer].[OfferID] = [WarehouseStocks].[OfferId] " +
                                        "Where Offer.ProductId = Product.ProductID " +
                                        "Order by (Case When [WarehouseStocks].[WarehouseId] = @warehouseId AND [WarehouseStocks].[Quantity] > 0 Then 1 Else 0 End) desc, Main desc" +
                                        ") as Offer " +
                          "Outer Apply (Select top (1) PhotoId, PhotoName, PhotoNameSize1, PhotoNameSize2, Description " +
                                        "From Catalog.Photo " +
                                        "Where Photo.ObjId = Product.ProductId and (Photo.ColorID = Offer.ColorID OR Photo.ColorID IS NULL) AND Type = 'Product' " +
                                        "Order by [Photo].Main DESC, [Photo].[PhotoSortOrder], [PhotoId]" +
                                        ") as Photo "
                    ) +
                "Inner Join [Catalog].[Currency] On [Currency].[CurrencyID] = [Product].[CurrencyID] " +
                "Where ListId=@ListId and Product.Enabled=1 and Product.Hidden=0 and CategoryEnabled=1" +
                (SettingsCatalog.ShowOnlyAvalible ? " AND (MaxAvailable>0 OR [Product].[AllowPreOrder] = 1)" : "") +
                (warehouseSql != null 
                    ? " and (Product.AllowPreOrder = 1 " +
                            "OR Exists(" +
                                "Select 1 from [Catalog].[Offer] " + 
                                "Inner Join [Catalog].[WarehouseStocks] on [Offer].[OfferID] = [WarehouseStocks].[OfferId] " + 
                                "Where [WarehouseStocks].[WarehouseId] in (" + warehouseSql + ") " + 
                                "     AND Offer.ProductId = Product.ProductID " + 
                                "     AND [WarehouseStocks].[Quantity] > 0)) "
                    : "") +
                " Order by " +
                (SettingsCatalog.MoveNotAvaliableToEnd ? "(CASE WHEN PriceTemp=0 THEN 0 ELSE 1 END) desc, AmountSort desc, " : "") +
                "[Product_ProductList].SortOrder, Product.ProductId";

            return
                CacheManager.Get(CacheNames.ProductListCacheName(listId, count, query), () =>
                    SQLDataAccess
                        .Query<ProductModel>(query, new
                        {
                            ListId = listId, 
                            Count = count, 
                            Type = PhotoType.Product.ToString(),
                            warehouseId = warehouseIds != null && warehouseIds.Count > 0 ? warehouseIds[0] : default(int?)
                        })
                        .ToList());
        }

        public static bool IsExistsProduct(int listId, int productId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Catalog].[Product_ProductList] WHERE ListId=@ListId and ProductId=@ProductId",
                CommandType.Text,
                new SqlParameter("@ListId", listId),
                new SqlParameter("@ProductId", productId)) > 0;
        }

        private static void ClearCache()
        {
            CacheManager.RemoveByPattern(CacheNames.ProductList);

            CacheManager.RemoveByPattern(CacheNames.SQLPagingItems);
            CacheManager.RemoveByPattern(CacheNames.SQLPagingCount);
        }

        #endregion
    }
}
