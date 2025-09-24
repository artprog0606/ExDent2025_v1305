using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Shipping
{
    public class ShippingCatalogMaping
    {
        public static bool IsExistLinksToShipping(int methodId)
        {
            var temp = SQLDataAccess.ExecuteScalar<int>(@"if exists (select ProductId from [Order].[ShippingProductExcluded] where MethodId=@MethodId)	
																select 1
															else
															if  exists (select CategoryId from [Order].[ShippingCategoryExcluded] where MethodId=@MethodId)
																select 1
															else
																select 0",
                CommandType.Text, new SqlParameter { ParameterName = "@MethodId", Value = methodId });
            return temp > 0;
        }

        public static bool CheckShippingEnabled(int methodId, params int[] productIds)
        {
            return !CheckShippingExcludedProduct(methodId, productIds) &&
                   !CheckShippingExcludedCategory(methodId, productIds);
        }

        #region Product links

        private static bool CheckShippingExcludedProduct(int methodId, params int[] productIds)
        {
            return productIds == null || productIds.Length == 0
                ? false
                : SQLDataAccess.ExecuteScalar<int>(
                      @"IF EXISTS(SELECT ProductId FROM [Order].[ShippingProductExcluded] WHERE MethodId=@MethodId AND ProductId IN (" +
                      string.Join(",", productIds) + @"))
                        SELECT 1
                      ELSE
                        SELECT 0",
                      CommandType.Text,
                      new SqlParameter {ParameterName = "@MethodId", Value = methodId}) > 0;
        }


        public static void AddExcludedProductLinkToShipping(int methodId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Insert into [Order].[ShippingProductExcluded] (MethodId,  ProductId) values (@MethodId, @ProductId)",
                                            CommandType.Text,
                                            new SqlParameter("@MethodId", methodId),
                                            new SqlParameter("@ProductId", productId));
        }

        public static List<int> GetExcludedProductsIDsByShipping(int methodId)
        {
            return SQLDataAccess.ExecuteReadList<int>("Select ProductId from [Order].[ShippingProductExcluded] where MethodId=@MethodId",
                                                           CommandType.Text,
                                                           reader => SQLDataHelper.GetInt(reader, "ProductId"),
                                                           new SqlParameter("@MethodId", methodId));
        }

        public static int GetCountExcludedProductsByShipping(int methodId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(ProductId) from [Order].[ShippingProductExcluded] where MethodId=@MethodId",
                                                           CommandType.Text,
                                                           new SqlParameter("@MethodId", methodId));
        }

        public static void DeleteExcludedProductLinkFromShipping(int methodId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].[ShippingProductExcluded] Where MethodId=@MethodId and ProductId=@ProductId",
                                            CommandType.Text,
                                            new SqlParameter("@MethodId", methodId),
                                            new SqlParameter("@ProductId", productId));
        }

        public static void DeleteAllExcludedProductsLinkFromShipping(int methodId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].[ShippingProductExcluded] Where MethodId=@MethodId",
                                            CommandType.Text,
                                            new SqlParameter("@MethodId", methodId));
        }

        #endregion

        #region Categories link

        private static bool CheckShippingExcludedCategory(int methodId, params int[] productIds)
        {
            return productIds == null || productIds.Length == 0
                ? false
                : SQLDataAccess.ExecuteScalar<int>(
                      @"IF EXISTS(Select ProductID From [Order].[ShippingCategoryExcluded] INNER JOIN Catalog.ProductCategories on [ShippingCategoryExcluded].CategoryID = ProductCategories.CategoryID Where MethodId=@MethodId AND [Main] = 1 AND ProductID IN (" +
                      string.Join(",", productIds) + @"))
                        SELECT 1
                      ELSE
                        SELECT 0",
                      CommandType.Text,
                      new SqlParameter {ParameterName = "@MethodId", Value = methodId}) > 0;
        }

        public static void AddCategoryLinkToShipping(int methodId, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("Insert into [Order].[ShippingCategoryExcluded] (MethodId, categoryId) values (@MethodId, @categoryId)",
                                            CommandType.Text,
                                            new SqlParameter("@MethodId", methodId),
                                            new SqlParameter("@categoryId", categoryId));
        }

        public static List<int> GetCategoriesIDsByShipping(int methodId)
        {
            return SQLDataAccess.ExecuteReadList<int>("Select CategoryID from [Order].[ShippingCategoryExcluded] where MethodId=@MethodId",
                                                           CommandType.Text,
                                                           reader => SQLDataHelper.GetInt(reader, "CategoryID"),
                                                           new SqlParameter("@MethodId", methodId));
        }

        public static void DeleteCategoryLinkFromShipping(int methodId, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].[ShippingCategoryExcluded] Where MethodId=@MethodId and categoryID=@categoryID",
                                            CommandType.Text,
                                            new SqlParameter("@MethodId", methodId),
                                            new SqlParameter("@categoryID", categoryId));
        }

        public static void DeleteAllCategoriesLinkFromShipping(int methodId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].[ShippingCategoryExcluded] Where MethodId=@MethodId",
                                            CommandType.Text,
                                            new SqlParameter("@MethodId", methodId));
        }

        #endregion

    }
}
