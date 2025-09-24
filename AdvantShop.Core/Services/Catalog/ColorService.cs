using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Statistic;

namespace AdvantShop.Catalog
{
    public class ColorService
    {
        private const string ColorCaheKey = "Color_";
        private const string ColorNameCaheKey = "ColorName_";

        public static Color GetColor(int? colorId)
        {
            if (!colorId.HasValue)
                return null;

            return
                CacheManager.Get(ColorCaheKey + colorId.Value, () =>
                    SQLDataAccess.ExecuteReadOne(
                        "Select TOP 1 * From Catalog.Color Left Join Catalog.Photo On Photo.ObjId=Color.ColorId and Type=@Type Where Color.ColorID=@colorId",
                        CommandType.Text, GetFromReader,
                        new SqlParameter("@colorid", colorId),
                        new SqlParameter("@Type", PhotoType.Color.ToString())));
        }

        public static Color GetColor(string colorName)
        {
            return
                CacheManager.Get(ColorNameCaheKey + colorName, () =>
                    SQLDataAccess.ExecuteReadOne(
                        "Select Top 1 * from Catalog.Color Left Join Catalog.Photo On Photo.ObjId=Color.ColorId and Type=@Type Where ColorName=@colorName Order by SortOrder",
                        CommandType.Text,
                        GetFromReader,
                        new SqlParameter("@colorName", colorName),
                        new SqlParameter("@Type", PhotoType.Color.ToString())));
        }

        public static List<Color> GetAllColors()
        {
            return
                SQLDataAccess.ExecuteReadList<Color>(
                    "Select * From Catalog.Color Left Join Catalog.Photo On Photo.ObjId=Color.ColorId and Type=@Type Order by SortOrder, ColorName",
                    CommandType.Text, GetFromReader, new SqlParameter("@Type", PhotoType.Color.ToString()));
        }
        
        public static List<Color> GetAllColorsByPaging(int limit, int currentPage, string q)
        {
            var p = new Core.SQL2.SqlPaging(currentPage, limit)
                .Select("ColorId", "ColorName")
                .From("Catalog.Color")
                .OrderBy("ColorName");
            
            if (!string.IsNullOrWhiteSpace(q))
                p.Where("ColorName like '%' + {0} + '%'", q);

            return p.PageItemsList<Color>();
        }


        private static Color GetFromReader(SqlDataReader reader)
        {
            return new Color()
            {
                ColorId = SQLDataHelper.GetInt(reader, "ColorId"),
                ColorCode = SQLDataHelper.GetString(reader, "ColorCode"),
                ColorName = SQLDataHelper.GetString(reader, "ColorName"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                IconFileName =
                    new ColorPhoto(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"))
                    {
                        PhotoName = SQLDataHelper.GetString(reader, "PhotoName")
                    },
            };
        }

        private static Color ReplaceBadSymbols(Color color)
        {
            color.ColorName = color.ColorName.Replace('\\', '/');
            color.ColorCode = color.ColorCode.Replace('\\', '/');
            return color;
        }
        
        public static int AddColor(Color color)
        {
            if (color == null)
                throw new ArgumentNullException("color");
            color = ReplaceBadSymbols(color);

            color.ColorId = SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddColor]", CommandType.StoredProcedure,
                                                        new SqlParameter("@ColorName", color.ColorName.Replace("\\", "/")),
                                                        new SqlParameter("@ColorCode", color.ColorCode),
                                                        new SqlParameter("@SortOrder", color.SortOrder));

            return color.ColorId;
        }

        public static void UpdateColor(Color color)
        {
            color = ReplaceBadSymbols(color);
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateColor]", CommandType.StoredProcedure,
                                                 new SqlParameter("@ColorId", color.ColorId),
                                                 new SqlParameter("@ColorName", color.ColorName.Replace("\\", "/")),
                                                 new SqlParameter("@ColorCode", color.ColorCode),
                                                 new SqlParameter("@SortOrder", color.SortOrder));

            CacheManager.RemoveByPattern(ColorCaheKey + color.ColorId);
            CacheManager.RemoveByPattern(ColorNameCaheKey);
        }

        public static void DeleteColor(int colorId)
        {
            PhotoService.DeletePhotos(colorId, PhotoType.Color);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.Color WHERE ColorID = @ColorId", CommandType.Text, new SqlParameter("@ColorId", colorId));

            CacheManager.RemoveByPattern(ColorCaheKey + colorId);
            CacheManager.RemoveByPattern(ColorNameCaheKey);
        }

        private const string GetColorsQuery =
@"
  ;with cte as (   
    Select distinct ColorID 
    From Catalog.Offer     
    Inner Join Catalog.Product on Offer.ProductID = Product.ProductID 
    Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]      
    {0} 
    Where Product.Enabled=1 and Product.CategoryEnabled=1  
          {1} 
  )  
  Select top(150) Color.ColorID, ColorName, ColorCode, PhotoId, ObjId, PhotoName, SortOrder 
  From Catalog.Color color  
  Left Join Catalog.Photo On Photo.ObjId = color.ColorID and Type = @type   
  Inner Join cte on cte.ColorID = color.ColorID  
  Order by Color.SortOrder, ColorName 
";

        public static List<Color> GetColorsByCategoryId(int categoryId, bool inDepth, bool onlyAvailable, List<int> warehouseIds)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>()
            {
                new SqlParameter("@CategoryID", categoryId),
                new SqlParameter("@Type", PhotoType.Color.ToString())
            };

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
            
            var sql = string.Format(GetColorsQuery, 
                queryInners.AggregateString(" "), 
                queryWheres.Count > 0 ? " and " + queryWheres.AggregateString(" and ") : null);

            return SQLDataAccess.ExecuteReadList(sql,
                CommandType.Text,
                GetFromReader,
                queryParams.ToArray());
        }

        public static List<Color> GetColorsByFilter(EProductOnMain? productOnMainType, 
                                                    int? productListId,
                                                    List<int> productIds,
                                                    bool onlyAvailable, 
                                                    List<int> warehouseIds)
        {
            var queryInners = new List<string>();
            var queryWheres = new List<string>();
            var queryParams = new List<SqlParameter>() { new SqlParameter("@Type", PhotoType.Color.ToString()) };
            
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

            var sql = string.Format(GetColorsQuery, 
                queryInners.AggregateString(" "), 
                queryWheres.Count > 0 ? " and " + queryWheres.AggregateString(" and ") : null);
            
            return SQLDataAccess.ExecuteReadList(sql,
                CommandType.Text,
                GetFromReader,
                queryParams.ToArray());
        }

        public static bool IsColorUsed(int colorId)
        {
            return Convert.ToInt32(
                SQLDataAccess.ExecuteScalar("Select Count(ColorID) from Catalog.Offer where ColorID=@ColorID", CommandType.Text,
                    new SqlParameter("@ColorID", colorId))) > 0;
        }

        public static void UpdateUnSetColorsFromAdvantshopCsv(bool useCS = false, string colorName = null)
        {
            try
            {
                var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                var fileName = "color-icon.csv";
                var fullFileName = filePath + fileName.FileNamePlusDate();

                new WebClient().DownloadFile("https://modules.advantshop.net/static/color-icon/color-icon.csv?param=" + Guid.NewGuid(), fullFileName);

                new CsvImportColors(fullFileName,
                    new ImportColorSettings() { UpdateOnlyColorWithoutCodeOrIcon = true, DownloadIconByLink = false, NameUpdatedColor = colorName, UseCommonStatistic = useCS })
                    .Process();
                FileHelpers.DeleteFile(fullFileName);
            }
            catch (Exception ex)
            {
                if(useCS)
                    CommonStatistic.WriteLog(DateTime.Now.ToString("[dd.MM.yy HH:mm]") + " Ошибка " + ex.Message);
                Debug.Log.Error(ex);
            }
        }
    }
}