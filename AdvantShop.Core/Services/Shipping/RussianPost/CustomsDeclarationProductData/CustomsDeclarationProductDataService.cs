using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Shipping.RussianPost.CustomsDeclarationProductData
{
    public class CustomsDeclarationProductDataService
    {

        public static List<CustomsDeclarationProductData> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[RussianPostCustomsDeclarationProductData]",
                CommandType.Text,
                GetFromReader);
        }

        public static CustomsDeclarationProductData Get(int productId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Shipping].[RussianPostCustomsDeclarationProductData] WHERE [ProductId] = @ProductId",
                CommandType.Text,
                GetFromReader,
                new SqlParameter("@ProductId", productId));
        }

        public static bool Exists(int productId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "SELECT (CASE WHEN EXISTS(SELECT * FROM [Shipping].[RussianPostCustomsDeclarationProductData] WHERE [ProductId] = @ProductId) THEN 1 ELSE 0 END)",
                CommandType.Text,
                new SqlParameter("@ProductId", productId));
        }

        private static CustomsDeclarationProductData GetFromReader(SqlDataReader reader)
        {
            return new CustomsDeclarationProductData
            {
                ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                CountryCode = SQLDataHelper.GetNullableInt(reader, "CountryCode"),
                TnvedCode = SQLDataHelper.GetString(reader, "TnvedCode")
            };
        }

        public static void AddOrUpdate(CustomsDeclarationProductData data)
        {
            SQLDataAccess.ExecuteScalar<int>(@"IF NOT EXISTS(SELECT * FROM [Shipping].[RussianPostCustomsDeclarationProductData] WHERE [ProductId] = @ProductId)
                BEGIN
                    INSERT INTO [Shipping].[RussianPostCustomsDeclarationProductData] ([ProductId],[CountryCode],[TnvedCode])
                        VALUES (@ProductId,@CountryCode,@TnvedCode)
                END
                ELSE
                BEGIN
                    UPDATE [Shipping].[RussianPostCustomsDeclarationProductData]
                       SET [CountryCode] = @CountryCode
                          ,[TnvedCode] = @TnvedCode
                     WHERE [ProductId] = @ProductId
                END",
                CommandType.Text,
                new SqlParameter("@ProductId", data.ProductId),
                new SqlParameter("@CountryCode", data.CountryCode ?? (object)DBNull.Value),
                new SqlParameter("@TnvedCode", data.TnvedCode ?? (object)DBNull.Value));
        }

        public static void Delete(int productId)
        {
            SQLDataAccess.ExecuteNonQuery(@"DELETE FROM [Shipping].[RussianPostCustomsDeclarationProductData] WHERE [ProductId] = @ProductId",
                CommandType.Text,
                new SqlParameter("@ProductId", productId));
        }
    }
}
