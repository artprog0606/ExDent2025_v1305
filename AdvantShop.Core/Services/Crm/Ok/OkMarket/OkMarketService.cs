using AdvantShop.Catalog;
using AdvantShop.Core.SQL;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket
{
    public static class OkMarketService
    {
        public static void AddCatalog(OkMarketCatalog catalog)
        {
            catalog.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into Ok.OkCatalog (Name, OkCatalogId) Values (@Name, @OkCatalogId); " +
                    "Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Name", catalog.Name ?? ""),
                    new SqlParameter("@OkCatalogId", catalog.OkCatalogId));
        }

        public static void AddCatalogLink(int categoryId, int OkCatalogId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Ok.OkCatalog_Category (CategoryId, OkCatalogId) Values (@CategoryId, @OkCatalogId) ",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@OkCatalogId", OkCatalogId));
        }

        public static void AddProduct(OkMarketProductModel product)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Ok.OkProduct (Id, ProductId, PhotoIds, CatalogId) Values (@Id, @ProductId, @PhotoIds, @CatalogId) ",
                CommandType.Text,
                new SqlParameter("@Id", product.OkProductId),
                new SqlParameter("@ProductId", product.ProductId),
                new SqlParameter("@PhotoIds", product.OkPhotoIds != null ? string.Join(",", product.OkPhotoIds) : ""),
                new SqlParameter("@CatalogId", product.OkCatalogId));
        }

        public static IEnumerable<OkMarketProductModel> GetProducts(long OkCatalogId)
        {
            return SQLDataAccess.Query<OkMarketProductModel>(
                "Select " +
                "p.ProductId, " +
                "p.Name, " +
                "p.UrlPath, " +
                "p.Description, " +
                "p.BriefDescription, " +
                "p.Discount, " +
                "p.DiscountAmount, " +
                "p.Enabled, " +
                "p.MinAmount, " +
                "p.Unit, " +

                "o.ColorID, " +
                "o.SizeID, " +
                "o.Price, " +
                "o.Main, " +
                "o.Amount, " +

                "ColorName, " +
                "SizeName, " +
                "CurrencyValue, " +
                "CurrencyIso3 as CurrencyName, " +

                "OkProduct.Id as OkProductId, " +
                "OkProduct.PhotoIds as OkPhotoIds " +

                "From Catalog.Product p " +
                "Left Join Catalog.Offer o On o.ProductID = p.ProductID " +
                "Inner Join Catalog.ProductCategories pc On pc.ProductID = p.ProductID and pc.Main = 1 " +
                "Left Join Catalog.Category cat On cat.CategoryId = pc.CategoryId " +

                "Left Join Catalog.Color On Color.ColorID = o.ColorID " +
                "Left Join Catalog.Size On Size.SizeID = o.SizeID " +
                "Left Join Ok.OkProduct On OkProduct.ProductId = p.ProductId " +
                "Inner Join Catalog.Currency On Currency.CurrencyID = p.CurrencyID " +

                "Where (p.Enabled = 1 OR @exportNotAvailable = 1) " +
                "AND (o.Main = 1 OR o.Main is null) " +
                "AND (o.Price > 0 OR @exportNotAvailable = 1) " +
                "AND (o.Amount > 0 OR (@ExportPreOrder = 1 AND p.AllowPreOrder = 1) OR @exportNotAvailable = 1) " +

                "AND pc.CategoryId in (Select CategoryId From Ok.OkCatalog_Category Where OkCatalogId=@OkCatalogId) " +

                "Order By cat.SortOrder, pc.SortOrder",

                new { OkCatalogId, exportNotAvailable = Export.OkMarketExportSettings.ExportUnavailableProducts, ExportPreOrder = Export.OkMarketExportSettings.ExportPreorderProducts });
        }

        public static OkMarketProductModel GetProductByOkId(long OkProductId)
        {
            return SQLDataAccess.Query<OkMarketProductModel>("Select Id as OkProductId, ProductId, PhotoIds as OkPhotoIds, CatalogId From Ok.OkProduct Where Id=@OkProductId ", new { OkProductId }).FirstOrDefault();
        }

        public static IEnumerable<OkMarketCatalog> GetCatalogList()
        {
            return SQLDataAccess.Query<OkMarketCatalog>("Select * From Ok.OkCatalog").ToList();
        }

        public static int GetExportProductsCount()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select count(o.ProductID) " +
                "From Ok.OkCatalog_Category occ " +
                "Join Catalog.ProductCategories pc On pc.CategoryID = occ.CategoryId and pc.Main = 1 " +
                "Join Catalog.Product p  On pc.ProductID = p.ProductId " +
                "Join Catalog.Offer o On o.ProductID = p.ProductID " +
                "Where (p.Enabled = 1 Or @exportNotAvailable = 1) " +
                "AND(o.Main = 1 Or o.Main is null) " +
                "AND(o.Price > 0 OR @exportNotAvailable = 1) " +
                "AND(o.Amount > 0 OR p.AllowPreOrder = 1 OR @exportNotAvailable = 1)",
                CommandType.Text, 60 * 3,
                new SqlParameter("@exportNotAvailable", Export.OkMarketExportSettings.ExportUnavailableProducts));
        }

        public static OkMarketCatalog GetCatalog(int id)
        {
            return SQLDataAccess.Query<OkMarketCatalog>("Select * From Ok.OkCatalog Where Id=@id", new { id }).FirstOrDefault();
        }

        public static OkMarketCatalog GetCatalogByOkCatalogId(long id)
        {
            return SQLDataAccess.Query<OkMarketCatalog>("Select * From Ok.OkCatalog Where OkCatalogId=@id", new { id }).FirstOrDefault();
        }

        public static IEnumerable<Category> GetLinkedCategories(int id)
        {
            return
               SQLDataAccess.Query<Category>(
                   "Select CategoryId, Name From Catalog.Category " +
                   "Where CategoryId in (Select CategoryId From Ok.OkCatalog_Category Where OkCatalogId=@id)",
                   new { id })
                   .ToList();
        }

        public static void DeleteCatalog(int id, long OkCatalogId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Ok.OkCatalog Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", id));

            SQLDataAccess.ExecuteNonQuery("Delete From Ok.OkProduct where CatalogId=@OkCatalogId", CommandType.Text, new SqlParameter("@OkCatalogId", OkCatalogId));
        }

        public static void RemoveLinkedCategories(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Ok.OkCatalog_Category Where OkCatalogId=@OkCatalogId",
                CommandType.Text,
                new SqlParameter("@OkCatalogId", id));
        }

        public static void DeleteProduct(int id, long OkProductId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Ok.OkProduct Where Id=@Id and ProductId=@ProductId",
                CommandType.Text,
                new SqlParameter("@Id", OkProductId),
                new SqlParameter("@ProductId", id));
        }

        public static void UpdateCatalog(OkMarketCatalog catalog)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Ok.OkCatalog SET OkCatalogId=@OkCatalogId, Name=@Name Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", catalog.Id),
                new SqlParameter("@OkCatalogId", catalog.OkCatalogId),
                new SqlParameter("@Name", catalog.Name ?? ""));
        }

        public static void UpdateProduct(OkMarketProductModel product)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Ok.OkProduct SET ProductId=@ProductId, PhotoIds=@PhotoIds, CatalogId=@CatalogId Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", product.OkProductId),
                new SqlParameter("@ProductId", product.ProductId),
                new SqlParameter("@PhotoIds", product.OkPhotoIdsList != null ? string.Join(",", product.OkPhotoIds) : ""),
                new SqlParameter("@CatalogId", product.OkCatalogId));
        }
    }
}