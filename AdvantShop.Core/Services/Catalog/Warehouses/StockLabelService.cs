using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class StockLabelService
    {
        public static int Add(StockLabel stockLabel)
        {
            if (stockLabel is null) throw new ArgumentNullException(nameof(stockLabel));

            stockLabel.Id = SQLDataAccess.ExecuteScalar<int>(
                @"INSERT INTO [Catalog].[StockLabel]
	                ([Name],[ClientName],[AmountUpTo],[Color])
                VALUES
	                (@Name,@ClientName,@AmountUpTo,@Color);
                SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", stockLabel.Name ?? (object) DBNull.Value),
                new SqlParameter("@ClientName", stockLabel.ClientName ?? (object) DBNull.Value),
                new SqlParameter("@AmountUpTo", stockLabel.AmountUpTo),
                new SqlParameter("@Color", stockLabel.Color ?? (object) DBNull.Value));

            CacheManager.RemoveByPattern("StockLabel_");
            
            return stockLabel.Id;
        }

        public static void Update(StockLabel stockLabel)
        {
            if (stockLabel is null) throw new ArgumentNullException(nameof(stockLabel));
            
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[StockLabel]
                   SET [Name] = @Name
                      ,[ClientName] = @ClientName
                      ,[AmountUpTo] = @AmountUpTo
                      ,[Color] = @Color
                 WHERE [Id] = @Id", 
                CommandType.Text,
                new SqlParameter("@Id", stockLabel.Id),
                new SqlParameter("@Name", stockLabel.Name ?? (object) DBNull.Value),
                new SqlParameter("@ClientName", stockLabel.ClientName ?? (object) DBNull.Value),
                new SqlParameter("@AmountUpTo", stockLabel.AmountUpTo),
                new SqlParameter("@Color", stockLabel.Color ?? (object) DBNull.Value));
            
            CacheManager.RemoveByPattern("StockLabel_");
        }
          
        public static StockLabel Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[StockLabel] WHERE [Id] = @Id",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@Id", id));
        }
          
        public static List<StockLabel> GetAll()
        {
            return CacheManager.Get("StockLabel_All",
                () => SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[StockLabel] Order By [AmountUpTo]",
                CommandType.Text, 
                GetFromReader));
        }
           
        public static StockLabel GetFromReader(SqlDataReader reader)
        {
            return new StockLabel
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                ClientName = SQLDataHelper.GetString(reader, "ClientName"),
                AmountUpTo = SQLDataHelper.GetFloat(reader, "AmountUpTo"),
                Color = SQLDataHelper.GetString(reader, "Color"),
            };
        }
  
        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Catalog].[StockLabel] WHERE [Id] = @Id",
                CommandType.Text, 
                new SqlParameter("@Id", id));
            
            CacheManager.RemoveByPattern("StockLabel_");
        }

        public static string GetLabel(float amount)
        {
            return GetLabel(amount, GetAll());
        }

        public static string GetLabel(float amount, List<StockLabel> stockLabels)
        {
            foreach (var stockLabel in stockLabels)
                if (amount < stockLabel.AmountUpTo)
                    return stockLabel.ClientName;
            
            return amount.ToInvariantString();
        }

        public static StockLabel GetStockLabel(float amount)
        {
            return GetStockLabel(amount, GetAll());
        }

        public static StockLabel GetStockLabel(float amount, List<StockLabel> stockLabels)
        {
            foreach (var stockLabel in stockLabels)
                if (amount < stockLabel.AmountUpTo)
                    return stockLabel;
            
            return null;
        }
    }
}