using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerCategoryService
    {
        public static TriggerCategory Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne<TriggerCategory>(
                "SELECT * FROM [CRM].[TriggerCategory] WHERE Id = @Id", CommandType.Text,
                GetTriggerCategoryFromReader, new SqlParameter("@Id", id));
        }

        public static List<TriggerCategory> GetList()
        {
            return
                SQLDataAccess.ExecuteReadList<TriggerCategory>(
                    "SELECT * FROM [CRM].[TriggerCategory]",
                    CommandType.Text, GetTriggerCategoryFromReader);
        }

        private static TriggerCategory GetTriggerCategoryFromReader(SqlDataReader reader)
        {
            return new TriggerCategory
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static int Add(TriggerCategory triggercategory)
        {
            triggercategory.Id = SQLDataAccess.ExecuteScalar<int>(
                " INSERT INTO [CRM].[TriggerCategory] " +
                " ([Name], [SortOrder]) " +
                " VALUES (@Name, @SortOrder); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", triggercategory.Name ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", triggercategory.SortOrder));

            return triggercategory.Id;
        }

        public static void Update(TriggerCategory triggercategory)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [CRM].[TriggerCategory] SET [Name] = @Name, [SortOrder] = @SortOrder " +
                " WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", triggercategory.Id),
                new SqlParameter("@Name", triggercategory.Name ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", triggercategory.SortOrder));
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [CRM].[TriggerCategory] WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }
    }
}
