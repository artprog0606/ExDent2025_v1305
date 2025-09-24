using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.ChangeHistories
{
    public class ProductChangeHistoryService
    {
        public static int Add(ChangeHistory history)
        {
            if (history.ObjType != ChangeHistoryObjType.Product)
                throw new ArgumentException("This is not a product change", nameof(history.ObjType));
            
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Catalog].[ProductChangeHistory] ([ProductId],ParameterName,OldValue,NewValue,ParameterType,ParameterId,ChangedByName,ChangedById,ModificationTime) " +
                    "Values (@ProductId,@ParameterName,@OldValue,@NewValue,@ParameterType,@ParameterId,@ChangedByName,@ChangedById,@ModificationTime);" +
                    "Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@ProductId", history.ObjId),

                    new SqlParameter("@ParameterName", history.ParameterName.Reduce(350)),
                    new SqlParameter("@OldValue", history.OldValue ?? ""),
                    new SqlParameter("@NewValue", history.NewValue ?? ""),
                    new SqlParameter("@ParameterType", (int) history.ParameterType),
                    new SqlParameter("@ParameterId", history.ParameterId ?? (object) DBNull.Value),

                    new SqlParameter("@ChangedByName", history.ChangedByName ?? ""),
                    new SqlParameter("@ChangedById", history.ChangedById ?? (object) DBNull.Value),
                    new SqlParameter("@ModificationTime",
                        history.ModificationTime != DateTime.MinValue ? history.ModificationTime : DateTime.Now)
                );
        }

        public static List<ChangeHistory> Get(int productId)
        {
            return SQLDataAccess.Query<ChangeHistory>(
                                     "SELECT * FROM [Catalog].[ProductChangeHistory] WHERE [ProductId]=@productId Order By ModificationTime desc",
                                     new {productId})
                                .ToList();
        }

        public static ChangeHistory GetLast(int productId)
        {
            return SQLDataAccess.Query<ChangeHistory>(
                                     "SELECT top(1) * FROM [Catalog].[ProductChangeHistory] WHERE [ProductId]=@productId Order By ModificationTime desc",
                                     new {productId})
                                .FirstOrDefault();
        }

        public static void DeleteExpiredHistory()
        {
            try
            {
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandTimeout = 60 * 10; // 10 mins
                    db.cmd.CommandText = "DELETE FROM [Catalog].[ProductChangeHistory] Where ModificationTime < @date";
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Now.AddMonths(-6)));

                    db.cnOpen();
                    db.cmd.ExecuteNonQuery();
                    db.cnClose();
                }

            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }


            try
            {
                // храним только 10 последних изменений одного поля одной сущности из одного источника
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandTimeout = 60 * 30; // 30 mins
                    db.cmd.CommandText = 
                        @"DECLARE @ProductId INT,  
                            @ParameterName NVARCHAR(max), @ChangedByName NVARCHAR(max);  
                          
                        DECLARE history_cursor CURSOR FOR   
                          
	                            SELECT [ProductId], [ParameterName], [ChangedByName]
	                            FROM [Catalog].[ProductChangeHistory]
	                            GROUP BY [ProductId], [ParameterName], [ChangedByName]
	                            HAVING COUNT(*) > @Count

                        OPEN history_cursor  
                          
                        FETCH NEXT FROM history_cursor   
                        INTO @ProductId, @ParameterName, @ChangedByName

                        WHILE @@FETCH_STATUS = 0  
                        BEGIN  
	                        DELETE FROM [Catalog].[ProductChangeHistory]
	                        WHERE [ProductId] = @ProductId and [ParameterName] = @ParameterName and [ChangedByName] = @ChangedByName
	                        and [id] not in 
	                        (
		                        SELECT TOP (@Count) [id] FROM [Catalog].[ProductChangeHistory]
		                        WHERE [ProductId] = @ProductId and [ParameterName] = @ParameterName and [ChangedByName] = @ChangedByName
		                        ORDER BY [id] DESC
	                        )

	                        FETCH NEXT FROM history_cursor   
                            INTO @ProductId, @ParameterName, @ChangedByName  
                        END   
                        CLOSE history_cursor;  
                        DEALLOCATE history_cursor;";
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.Add(new SqlParameter("@Count", 10));

                    db.cnOpen();
                    db.cmd.ExecuteNonQuery();
                    db.cnClose();
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        public static void InsertMassByCategoriesBackground(bool allProducts, List<int> categoryIds, string parameterName,
                                                            string newValue, ChangedBy changedBy)
        {
            Task.Run(() => InsertMassByCategories(allProducts, categoryIds, parameterName, newValue, changedBy));
        }
        
        

        public static void InsertMassByCategories(bool allProducts, List<int> categoryIds, string parameterName,
                                                    string newValue, ChangedBy changedBy)
        {
            try
            {
                if (allProducts && ProductService.GetProductsCount() > 15_000)
                    return;
                
                var cmd = string.Format(
                    "DECLARE @ProductIds TABLE(id int); " +
                    "Insert into @ProductIds " +
                    (allProducts
                        ? "Select ProductId From Catalog.Product"
                        : "Select ProductId FROM Catalog.ProductCategories WHERE CategoryID IN ({0}) AND Catalog.ProductCategories.Main = 1") +
                    " " +
                    "if ((Select count(id) From @ProductIds) < 15000) " +
                    "begin " +
                        "Insert into [Catalog].[ProductChangeHistory] " +
                        "([ProductId],ParameterName,OldValue,NewValue,ParameterType,ParameterId,ChangedByName,ChangedById,ModificationTime) " +
                        "Select id, @ParameterName, '', @NewValue, 0, null, @ChangedByName, @ChangedById, @ModificationTime " +
                        "From @ProductIds " +
                    
                    "end",
                    categoryIds != null ? categoryIds.AggregateString(",") : "");

                SQLDataAccess.ExecuteNonQuery(cmd, 
                    CommandType.Text, 
                    new SqlParameter("@ParameterName", parameterName),
                    new SqlParameter("@OldValue", ""),
                    new SqlParameter("@NewValue", newValue),
                    new SqlParameter("@ChangedByName", changedBy.Name ?? ""),
                    new SqlParameter("@ChangedById", changedBy.CustomerId ?? (object)DBNull.Value),
                    new SqlParameter("@ModificationTime", DateTime.Now));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }
        
        public static void InsertMassByBrandsBackground(List<int> brandIds, string parameterName, string newValue, ChangedBy changedBy)
        {
            Task.Run(() => InsertMassByBrands(brandIds, parameterName, newValue, changedBy));
        }

        public static void InsertMassByBrands(List<int> brandIds, string parameterName, string newValue, ChangedBy changedBy)
        {
            try
            {
                if (brandIds == null || brandIds.Count == 0)
                    return;
                
                var cmd = 
                    "DECLARE @ProductIds TABLE(id int); " +
                    $"INSERT INTO @ProductIds SELECT ProductId FROM [Catalog].[Product] WHERE [BrandID] In ({string.Join(",", brandIds)}); " +
                    "if ((Select count(id) From @ProductIds) < 15000) " +
                    "begin " +
                        "Insert into [Catalog].[ProductChangeHistory] " +
                        "([ProductId],ParameterName,OldValue,NewValue,ParameterType,ParameterId,ChangedByName,ChangedById,ModificationTime) " +
                        "Select id, @ParameterName, '', @NewValue, 0, null, @ChangedByName, @ChangedById, @ModificationTime " +
                        "From @ProductIds " +
                    "end";

                SQLDataAccess.ExecuteNonQuery(cmd, 
                    CommandType.Text, 
                    new SqlParameter("@ParameterName", parameterName),
                    new SqlParameter("@OldValue", ""),
                    new SqlParameter("@NewValue", newValue),
                    new SqlParameter("@ChangedByName", changedBy.Name ?? ""),
                    new SqlParameter("@ChangedById", changedBy.CustomerId ?? (object)DBNull.Value),
                    new SqlParameter("@ModificationTime", DateTime.Now));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }
    }
}