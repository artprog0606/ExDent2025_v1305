//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class RatingService
    {
        public static float Vote(int productId, int rating)
        {
            var customerId = CustomerContext.CurrentCustomer.Id;

            if (DoesUserVote(productId, customerId))
                return 0;

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_AddRatio]", CommandType.StoredProcedure,
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@ProductRatio", rating),
                new SqlParameter("@CustomerId", customerId));

            return RecalcProductRatio(productId);
        }

        private static int RecalcProductRatio(int productId)
        {
            var newRating = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("[Catalog].[sp_GetAVGRatioByProductID]",
                CommandType.StoredProcedure,
                new SqlParameter("@ProductID", productId)));

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateProductRatio]", CommandType.StoredProcedure,
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@Ratio", newRating));

            CacheManager.RemoveByPattern(CacheNames.SQLPagingCount);
            CacheManager.RemoveByPattern(CacheNames.SQLPagingItems);

            return newRating;
        }

        public static bool DoesUserVote(int productId, Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNT_Ratio]", CommandType.StoredProcedure,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@ProductID", productId)) > 0;
        }

        public static int GetProductRatioCount(int productId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNT_Ratio]", CommandType.StoredProcedure,
                new SqlParameter("@CustomerId", DBNull.Value),
                new SqlParameter("@ProductID", productId));
        }

        public static int? GetUserProductRating(int productId, Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<string>("SELECT [ProductRatio] FROM [Catalog].[Ratio] WHERE [CustomerId] = @CustomerId AND [ProductID] = @ProductID", CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@ProductID", productId)).TryParseInt(true);
        }

        public static void DeleteUserProductRating(int productId, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Ratio] WHERE [CustomerId] = @CustomerId AND [ProductID] = @ProductID", CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@ProductID", productId));
            RecalcProductRatio(productId);
        }
    }
}
