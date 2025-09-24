//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using System.Linq;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL2;

namespace AdvantShop.Catalog
{

    public class BrandProductCount
    {
        public int InCategoryCount { set; get; }
        public int InChildsCategoryCount { set; get; }

        public int CategoryId { set; get; }
        public int ParentId { set; get; }
        public string Url { set; get; }
        public string Name { set; get; }
        public int Level { set; get; }
    }

    public class BrandService
    {
        private const string BrandCacheKey = "Brand_";
        private const string BrandIdCacheKey = "BrandId_";

        private static string GetBrandCategorySql(string sql, bool onlyVisibleCategories = false)
        {
            return @"with brandCategory as (
                        SELECT p.BrandId,
                                                pc.CategoryId,
                                                COUNT(*) AS [Count],
                                                COUNT(c.enabled) AS [CountDeep]
                                        FROM [Catalog].Product p
                                                INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
                                                INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
                                                INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
                                        WHERE p.BrandId IS NOT NULL               
                                                AND p.[Enabled] = 1
                                                AND p.Hidden = 0
                                                AND p.CategoryEnabled = 1
                                                AND c.Enabled = 1 " + 
                                                (onlyVisibleCategories ? "AND c.Hidden = 0 " : "") + 
                                                (SettingsCatalog.ShowOnlyAvalible ? " AND c.Available_Products_Count > 0 " : "") +
                                        @"GROUP BY pc.CategoryId,
                                                    p.BrandId
                        ) " + sql;
        }

        public static List<char> GetEngBrandChars()
        {
            return new List<char>(){ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                                         'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        }

        public static List<char> GetRusBrandChars()
        {
            return new List<char>(){ 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и','й', 'к', 'л', 'м', 'н', 'о',
                                         'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'э', 'ю', 'я'};
        }

        #region Get Add Update Delete

        public static void DeleteBrand(int brandId)
        {
            DeleteBrandLogo(brandId);
            SQLDataAccess.ExecuteNonQuery("Delete From Catalog.Brand Where BrandID=@BrandID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@BrandId", Value = brandId });
            CacheManager.RemoveByPattern(BrandCacheKey + brandId);
        }


        public static int AddBrand(Brand brand)
        {
            if (!GetBrandIdByName(brand.Name).IsDefault())
                return 0;

            brand.BrandId = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "[Catalog].[sp_AddBrand]",
                CommandType.StoredProcedure,
                new SqlParameter("@BrandName", brand.Name),
                new SqlParameter("@BrandDescription", brand.Description ?? (object)DBNull.Value),
                new SqlParameter("@BrandBriefDescription", brand.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", brand.Enabled),
                new SqlParameter("@SortOrder", brand.SortOrder),
                new SqlParameter("@CountryID", brand.CountryId == 0 ? (object)DBNull.Value : brand.CountryId),
                new SqlParameter("@CountryOfManufactureID", brand.CountryOfManufactureId == 0 ? (object)DBNull.Value : brand.CountryOfManufactureId),
                new SqlParameter("@UrlPath", brand.UrlPath),
                new SqlParameter("@BrandSiteUrl", brand.BrandSiteUrl.IsNotEmpty() ? brand.BrandSiteUrl : (object)DBNull.Value)
                ));

            if (brand.BrandId == 0)
                return 0;

            if (brand.Meta != null)
            {
                if (!brand.Meta.Title.IsNullOrEmpty() || !brand.Meta.MetaKeywords.IsNullOrEmpty() ||
                    !brand.Meta.MetaDescription.IsNullOrEmpty() || !brand.Meta.H1.IsNullOrEmpty())
                {
                    brand.Meta.ObjId = brand.BrandId;
                    MetaInfoService.SetMeta(brand.Meta);
                }
            }

            CacheManager.RemoveByPattern(CacheNames.BrandsInCategory);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(BrandIdCacheKey);
            CacheManager.RemoveByPattern(BrandCacheKey);

            return brand.BrandId;
        }

        public static void UpdateBrand(Brand brand)
        {
            var existingBrand = GetBrandIdByName(brand.Name);
            if (existingBrand != 0 && (brand.BrandId == 0 || brand.BrandId != existingBrand))
                return;

            SQLDataAccess.ExecuteNonQuery(
                "[Catalog].[sp_UpdateBrandById]",
                CommandType.StoredProcedure,
                new SqlParameter("@BrandID", brand.BrandId),
                new SqlParameter("@BrandName", brand.Name),
                new SqlParameter("@BrandDescription", brand.Description ?? (object)DBNull.Value),
                new SqlParameter("@BrandBriefDescription", brand.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", brand.Enabled),
                new SqlParameter("@SortOrder", brand.SortOrder),
                new SqlParameter("@CountryID", brand.CountryId == 0 ? (object)DBNull.Value : brand.CountryId),
                new SqlParameter("@CountryOfManufactureID", brand.CountryOfManufactureId == 0 ? (object)DBNull.Value : brand.CountryOfManufactureId),
                new SqlParameter("@UrlPath", brand.UrlPath),
                new SqlParameter("@BrandSiteUrl", brand.BrandSiteUrl.IsNotEmpty() ? brand.BrandSiteUrl : (object)DBNull.Value)
                );

            if (brand.Meta != null)
            {
                if (brand.Meta.Title.IsNullOrEmpty() && brand.Meta.MetaKeywords.IsNullOrEmpty() && brand.Meta.MetaDescription.IsNullOrEmpty() && brand.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(brand.BrandId, MetaType.Brand))
                    {
                        MetaInfoService.DeleteMetaInfo(brand.BrandId, MetaType.Brand);
                    }
                }
                else
                    MetaInfoService.SetMeta(brand.Meta);
            }

            CacheManager.RemoveByPattern(CacheNames.BrandsInCategory);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(BrandIdCacheKey);
            CacheManager.RemoveByPattern(BrandCacheKey);
        }

        private static Brand GetBrandFromReader(SqlDataReader reader)
        {
            return new Brand
            {
                BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                Name = SQLDataHelper.GetString(reader, "BrandName"),
                Description = SQLDataHelper.GetString(reader, "BrandDescription", string.Empty),
                BriefDescription = SQLDataHelper.GetString(reader, "BrandBriefDescription", string.Empty),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                CountryId = SQLDataHelper.GetInt(reader, "CountryID", 0),
                CountryOfManufactureId = SQLDataHelper.GetInt(reader, "CountryOfManufactureID", 0),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                BrandSiteUrl = SQLDataHelper.GetString(reader, "BrandSiteUrl")
            };
        }

        public static Brand GetBrandById(int brandId)
        {
            if (brandId == 0)
                return null;

            return CacheManager.Get(BrandCacheKey + brandId, () =>
                SQLDataAccess.ExecuteReadOne("Select TOP 1 * From Catalog.Brand where BrandID=@BrandID",
                    CommandType.Text, GetBrandFromReader, 
                    new SqlParameter("@BrandID", brandId)));
        }

        public static Brand GetBrand(string url)
        {
            return CacheManager.Get(BrandCacheKey + url, () =>
                SQLDataAccess.ExecuteReadOne("Select TOP 1 * From Catalog.Brand where UrlPath=@UrlPath",
                    CommandType.Text, GetBrandFromReader, 
                    new SqlParameter("@UrlPath", url)));
        }

        public static Brand GetBrandByName(string brandName)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM Catalog.Brand WHERE LOWER ([BrandName])=@BrandName",
                CommandType.Text,
                GetBrandFromReader, new SqlParameter("@BrandName", brandName.ToLower()));
        }

        public static IEnumerable<int> GetAllBrandIDs(bool onlyDemo = false)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT BrandID FROM [Catalog].[Brand]" + (onlyDemo ? " WHERE IsDemo = 1" : string.Empty),
                CommandType.Text, "BrandID");
        }

        public static List<Brand> GetBrands()
        {
            return SQLDataAccess.ExecuteReadList<Brand>("Select * from Catalog.Brand order by BrandName", CommandType.Text, GetBrandFromReader);
        }

        public static List<Brand> GetBrandsOnLimit(int count)
        {
            return SQLDataAccess.ExecuteReadList<Brand>("SELECT TOP(@Count) * FROM Catalog.Brand WHERE enabled=1 AND EXISTS (Select 1 FROM Catalog.Photo WHERE Photo.ObjId=Brand.BrandID AND Type=@Type) ORDER BY SortOrder",
                CommandType.Text,
                GetBrandFromReader, 
                new SqlParameter("@Count", count),
                new SqlParameter("@Type", PhotoType.Brand.ToString()));
        }

        public static List<Brand> GetBrands(bool haveProducts, bool enabled = true)
        {
            var cmd = haveProducts
                ? "SELECT * FROM [Catalog].[Brand] left join Catalog.Photo on Photo.ObjId=Brand.BrandID and Type=@Type Where " + (enabled ? "enabled=1 and " : "") + "(SELECT COUNT(ProductID) From [Catalog].[Product] Where [Product].[BrandID]=Brand.[BrandID]) > 0 ORDER BY [SortOrder], [BrandName]"
                : "SELECT * FROM [Catalog].[Brand] left join Catalog.Photo on Photo.ObjId=Brand.BrandID and Type=@Type " + (enabled ? "Where enabled=1" : "") + " Order by [SortOrder], [BrandName]";
            return SQLDataAccess.ExecuteReadList(cmd, CommandType.Text, reader => new Brand
            {
                BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                Name = SQLDataHelper.GetString(reader, "BrandName"),
                Description = SQLDataHelper.GetString(reader, "BrandDescription", string.Empty),
                BriefDescription = SQLDataHelper.GetString(reader, "BrandBriefDescription", string.Empty),
                BrandLogo =
                    new BrandPhoto(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"))
                    { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") },
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                CountryId = SQLDataHelper.GetInt(reader, "CountryID", 0),
                CountryOfManufactureId = SQLDataHelper.GetInt(reader, "CountryOfManufactureID", 0),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                BrandSiteUrl = SQLDataHelper.GetString(reader, "BrandSiteUrl")
            },
            new SqlParameter("@Type", PhotoType.Brand.ToString()));
        }
        
        public static List<Brand> GetBrandsBySearch(int limit, int currentPage, string q, bool existsInProduct = true)
        {
            var p = new Core.SQL2.SqlPaging(currentPage, limit)
                .Select("BrandId", "BrandName".AsSqlField("Name"))
                .From("Catalog.Brand")
                .OrderBy("BrandName");

            if (existsInProduct)
                p.Where("exists (Select 1 From Catalog.Product Where Product.BrandId = Brand.BrandId)");

            if (!string.IsNullOrWhiteSpace(q))
                p.Where("BrandName like '%' + {0} + '%'", q);

            return p.PageItemsList<Brand>();
        }

        #endregion

        #region ProductLinks

        public static void DeleteProductLink(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalig.Product Set BrandID=Null Where ProductID=@ProductID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@ProductID", Value = productId });
        }

        public static void AddProductLink(int productId, int brandId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Product Set BrandID=@BrandID Where ProductID=@ProductID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@ProductID", Value = productId },
                                            new SqlParameter { ParameterName = "@BrandId", Value = brandId });
        }

        #endregion

        private const string GetBrandsQuery =
            @"
    Select BrandID, BrandName, UrlPath, SortOrder 
    From Catalog.Brand 
    Where BrandID in (Select distinct BrandID 
                      From Catalog.Product 
                      {0}
                      Where Enabled = 1 and CategoryEnabled = 1
                            {1}
                      ) 
        and Enabled = 1";

        public static List<Brand> GetBrandsByFilter(EProductOnMain type, int? listId, List<int> productIds, bool onlyAvailable, List<int> warehouseIds)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>();
            
            switch (type)
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
                    return new List<Brand>();
            }
            
            if (listId != null && listId != 0)
            {
                queryInners.Add(
                    "Inner Join [Catalog].[Product_ProductList] plist on plist.ProductId = Product.ProductId and plist.ListId = @ListId");
                queryParams.Add(new SqlParameter("@ListId", listId));
            }
            
            if (productIds != null && productIds.Count > 0)
            {
                queryInners.Add(
                    "Inner Join (Select value From [Settings].[SPLIT_INT](@ProductIds, ',')) ids on Product.ProductId = ids.value"
                );
                queryParams.Add(new SqlParameter("@ProductIds", string.Join(",", productIds)));
            }

            if (onlyAvailable)
            {
                queryInners.Add("Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]");
                queryWheres.Add("(MaxAvailable > 0 OR [Product].[AllowPreOrder] = 1)");
            }

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
            
            var sql = string.Format(GetBrandsQuery, 
                queryInners.AggregateString(" "), 
                queryWheres.Count > 0 ? " and " + queryWheres.AggregateString(" and ") : null);

            return
                SQLDataAccess.ExecuteReadList(sql, CommandType.Text,
                        reader => new Brand
                        {
                            BrandId = SQLDataHelper.GetInt(reader, "BrandID"),
                            Name = SQLDataHelper.GetString(reader, "BrandName"),
                            UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                            SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
                        },
                        queryParams.ToArray())
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.Name)
                    .ToList();
        }

        #region BrandCategory

        public static List<Brand> GetBrandsByCategoryId(int categoryId, bool inDepth, bool onlyAvailable = false, List<int> warehouseIds = null)
        {
            if (!inDepth)
            {
                var queryInners = new List<string>()
                {
                    "INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = Product.ProductID and CategoryId = @CategoryId"
                };
                var queryParams = new List<SqlParameter>()
                {
                    new SqlParameter("@CategoryId", categoryId)
                };
                var queryWheres = new List<string>();
                
                if (onlyAvailable)
                {
                    queryInners.Add("Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]");
                    queryWheres.Add("(MaxAvailable > 0 OR [Product].[AllowPreOrder] = 1)");
                }

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
                
                var sql = string.Format(GetBrandsQuery, 
                    queryInners.AggregateString(" "), 
                    queryWheres.Count > 0 ? " and " + queryWheres.AggregateString(" and ") : null);
                
                return SQLDataAccess.ExecuteReadList(sql, 
                        CommandType.Text,
                        reader => new Brand
                        {
                            BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                            Name = SQLDataHelper.GetString(reader, "BrandName"),
                            UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                            SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                        },
                        queryParams.ToArray())
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.Name)
                    .ToList();
            }
            
            return SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetBrandsByCategoryId]", CommandType.StoredProcedure,
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
                    new SqlParameter("@Indepth", inDepth),
                    new SqlParameter("@OnlyAvailable", onlyAvailable))
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public static Dictionary<int, BrandProductCount> GetCategoriesByBrand(int brandId)
        {
            return
                SQLDataAccess.ExecuteReadDictionary<int, BrandProductCount>(GetBrandCategorySql("select CategoryId, [Count], [CountDeep] from brandCategory where BrandId=@BrandID", onlyVisibleCategories: true),
                    CommandType.Text, "CategoryID", 
                    reader => new BrandProductCount
                    {
                        InCategoryCount = SQLDataHelper.GetInt(reader, "Count"),
                        InChildsCategoryCount = SQLDataHelper.GetInt(reader, "CountDeep")
                    }, 
                    new SqlParameter("@BrandID", brandId));
        }

        public static List<BrandProductCount> GetParentCategoriesbyChildsId(List<int> list)
        {
            if (list == null || !list.Any())
            {
                return new List<BrandProductCount>();
            }
            var _params = list.AggregateString(",");
            var sqlcomand = @";with parents as 
                                (
                                   select CategoryID, ParentCategory
                                   from Catalog.Category
                                   where CategoryID in ({0})
                                   union all
                                   select C.CategoryID, C.ParentCategory 
                                   from Catalog.Category c
                                   join parents p on C.CategoryID = P.ParentCategory                                   
                                   AND (C.CategoryID<>C.ParentCategory Or C.CategoryID <>0)
                                   AND C.Hidden = 0 AND C.Enabled = 1
                                )
                                Select c.CategoryId, c.ParentCategory, c.Name, c.UrlPath, c.CatLevel from 
                                ( select distinct *  from parents) t 
                                inner join Catalog.Category c on  c.CategoryID = t.CategoryID 
                                Order by c.SortOrder";
            return SQLDataAccess.ExecuteReadList(string.Format(sqlcomand, _params), CommandType.Text, reader => new BrandProductCount
            {
                CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                ParentId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                Url = SQLDataHelper.GetString(reader, "UrlPath"),
                Level = SQLDataHelper.GetInt(reader, "CatLevel")
            });
        }

        #endregion

        public static bool IsBrandEnabled(int brandId)
        {
            var res = SQLDataAccess.ExecuteScalar<bool>(
                "SELECT [Enabled] FROM [Catalog].[Brand] WHERE [BrandId] = @brandId", CommandType.Text,
                new SqlParameter { ParameterName = "@brandId", Value = brandId });

            return res;
        }

        /// <summary>
        /// get products count
        /// </summary>
        /// <returns></returns>
        public static int GetProductsCount(int brandId)
        {
            var res = SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([ProductID]) FROM [Catalog].[Product] Where BrandID=@BrandID", CommandType.Text,
                new SqlParameter { ParameterName = "@BrandID", Value = brandId });
            return res;
        }

        public static void DeleteBrandLogo(int brandId)
        {
            PhotoService.DeletePhotos(brandId, PhotoType.Brand);

            CacheManager.RemoveByPattern(CacheNames.BrandsInCategory);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(BrandIdCacheKey);
            CacheManager.RemoveByPattern(BrandCacheKey);
        }

        public static int GetBrandIdByName(string brandName)
        {
            return CacheManager.Get(BrandIdCacheKey + brandName.ToLower(), () =>
                SQLDataAccess.ExecuteScalar<int>("select BrandID from Catalog.Brand where BrandName=@BrandName", CommandType.Text,
                    new SqlParameter("@BrandName", brandName.ToLower()))
            );
        }

        public static string BrandToString(int brandId)
        {
            var brand = GetBrandById(brandId);
            return brand != null ? brand.Name : string.Empty;
        }

        public static int BrandFromString(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return 0;

            var brandId = GetBrandIdByName(brandName);
            if (brandId != 0)
                return brandId;
            var brand = new Brand
            {
                Enabled = true,
                Name = brandName,
                Description = brandName,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Brand, brandName),
                Meta = null
            };
            return AddBrand(brand);
        }

        public static int GetBrandsCount(string condition = null, params SqlParameter[] parameters)
        {
            if (condition == null)
            {
                return SQLDataAccess.ExecuteScalar<int>("SELECT Count([BrandId]) FROM [Catalog].[Brand]",
                                                           CommandType.Text);
            }
            return SQLDataAccess.ExecuteScalar<int>("SELECT Count([BrandId]) FROM [Catalog].[Brand]" + " " + condition,
                                                    CommandType.Text, parameters);
        }

        public static void SetActive(int brandId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Update [Catalog].[Brand] Set Enabled = @Enabled Where BrandId = @BrandId",
                 CommandType.Text,
                 new SqlParameter("@BrandId", brandId),
                 new SqlParameter("@Enabled", active));
        }

        public static void ImportBrands(
            AdvantShop.Core.Services.ExportImport.ImportServices.ImportMode importMode = Core.Services.ExportImport.ImportServices.ImportMode.Full,
            bool updateOnlyByEmptyDescription = false,
            bool useCommonStatistic = false)
        {
            try
            {
                var filePath = FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.ApplicationTempData);
                var fileName = "brands.csv";
                var fullFileName = System.IO.Path.Combine(filePath, fileName.FileNamePlusDate());
                var photoZipFileName = System.IO.Path.Combine(filePath, "brands-photos.zip".FileNamePlusDate());
                FileHelpers.DeleteFile(fullFileName);
                FileHelpers.DeleteFile(photoZipFileName);

                new System.Net.WebClient().DownloadFile(
                    "https://modules.advantshop.net/static/brands/brands.csv?param=" + Guid.NewGuid(), fullFileName);
                new System.Net.WebClient().DownloadFile(
                    "https://modules.advantshop.net/static/brands/brands.zip?param=" + Guid.NewGuid(), photoZipFileName);

                FileHelpers.UnZipFile(
                    photoZipFileName,
                    FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.ImageTemp));
                FileHelpers.DeleteFile(photoZipFileName);

                new AdvantShop.Core.Services.ExportImport.ImportServices.CsvImportBrands(
                    fullFileName,
                    new AdvantShop.Core.Services.ExportImport.ImportServices.ImportBrandsSettings
                    {
                        HasHeaders = true,
                        ColumnSeparator = ";",
                        Encodings = Encoding.UTF8.WebName,
                        ImportMode = importMode,
                        UpdateOnlyByEmptyDescription = updateOnlyByEmptyDescription,
                        UseCommonStatistic = useCommonStatistic
                    }).Process();
            }
            catch (Exception ex)
            {
                if (useCommonStatistic)
                    Statistic.CommonStatistic.WriteLog(DateTime.Now.ToString("[dd.MM.yy HH:mm]") + " Ошибка "
                        + ex.Message);
                Diagnostics.Debug.Log.Error(ex);
            }
        }
        
        public static System.Threading.Tasks.Task<bool> ImportBrandsThroughACommonStatistic(
            string currentProcess, 
            string currentProcessName,
            Action onBeforeImportAction = null,
            AdvantShop.Core.Services.ExportImport.ImportServices.ImportMode importMode = Core.Services.ExportImport.ImportServices.ImportMode.Full,
            bool updateOnlyByEmptyDescription = false)
        {
                var filePath = FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.ApplicationTempData);
                var fileName = "brands.csv";
                var fullFileName = System.IO.Path.Combine(filePath, fileName.FileNamePlusDate());

                return new AdvantShop.Core.Services.ExportImport.ImportServices.CsvImportBrands(
                    fullFileName,
                    new AdvantShop.Core.Services.ExportImport.ImportServices.ImportBrandsSettings
                    {
                        HasHeaders = true,
                        ColumnSeparator = ";",
                        Encodings = Encoding.UTF8.WebName,
                        ImportMode = importMode,
                        UpdateOnlyByEmptyDescription = updateOnlyByEmptyDescription
                    }).ProcessThroughACommonStatistic(currentProcess, currentProcessName, () =>
                {
                    var photoZipFileName = System.IO.Path.Combine(filePath, "brands-photos.zip".FileNamePlusDate());
                    FileHelpers.DeleteFile(fullFileName);
                    FileHelpers.DeleteFile(photoZipFileName);

                    new System.Net.WebClient().DownloadFile(
                        "https://modules.advantshop.net/static/brands/brands.csv?param=" + Guid.NewGuid(), fullFileName);
                    new System.Net.WebClient().DownloadFile(
                        "https://modules.advantshop.net/static/brands/brands.zip?param=" + Guid.NewGuid(), photoZipFileName);

                    FileHelpers.UnZipFile(
                        photoZipFileName,
                        FilePath.FoldersHelper.GetPathAbsolut(FilePath.FolderType.ImageTemp));
      
                    onBeforeImportAction?.Invoke();
                });
        }
    }
}