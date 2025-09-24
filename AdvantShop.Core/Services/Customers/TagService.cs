using AdvantShop.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.Core.Services.Customers
{
    public class TagService
    {
        #region Tag
        public static Tag Get(int id)
        {
            return SQLDataAccess.Query<Tag>("SELECT * FROM [Customers].[Tag] WHERE Id=@id", new { id }).FirstOrDefault();
        }

        public static Tag Get(string name)
        {
            return SQLDataAccess.Query<Tag>("SELECT * FROM [Customers].[Tag] WHERE Name=@Name COLLATE SQL_Latin1_General_CP1_CS_AS", new { name }).FirstOrDefault();
        }

        public static List<Tag> GetAllTags(bool onlyEnabled = true)
        {
            return SQLDataAccess.Query<Tag>("SELECT * FROM [Customers].[Tag]" + (onlyEnabled ? " WHERE Enabled = 1" : string.Empty) + "ORDER BY [SortOrder]").ToList();
        }

        public static List<Tag> GetAutocompleteTags(int count = 1000, bool onlyEnabled = true)
        {
            return SQLDataAccess.Query<Tag>("SELECT TOP(@count) Id, Name FROM [Customers].[Tag]" +
                (onlyEnabled ? " WHERE Enabled = 1" : string.Empty) + "ORDER BY [SortOrder]",
                new { count }).ToList();
        }

        public static List<Tag> Gets(string name)
        {
            return SQLDataAccess.Query<Tag>("SELECT Id, Name FROM [Customers].[Tag] WHERE Name LIKE '%' + @Name + '%'", new { Name = name }).ToList();
        }

        public static List<Tag> Gets(Guid customerId, bool onlyEnabled = false)
        {
            return SQLDataAccess.Query<Tag>(
                "SELECT Id, Name, Enabled FROM [Customers].[Tag] INNER JOIN [Customers].[TagMap] ON Tag.Id=TagMap.TagId WHERE TagMap.CustomerId=@CustomerId" +
                (onlyEnabled ? " AND Enabled = 1" : string.Empty) +
                " ORDER BY TagMap.SortOrder ",
                new { CustomerId = customerId }).ToList();
        }

        public static int Add(Tag model)
        {
            model.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Customers].[Tag] (Name, Enabled, SortOrder) VALUES (@Name, @Enabled, @SortOrder); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Enabled", model.Enabled),
                new SqlParameter("@SortOrder", model.SortOrder));

            return model.Id;
        }

        public static void Update(Tag model)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Customers].[Tag] SET Name=@Name, Enabled=@Enabled, SortOrder=@SortOrder WHERE Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Enabled", model.Enabled),
                new SqlParameter("@SortOrder", model.SortOrder));
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[Tag] WHERE Id=@Id", CommandType.Text, new SqlParameter("@Id", id));
        }
        #endregion

        #region TagMap
        public static int AddMap(Guid customerId, int tagId, int sortOrder)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Customers].[TagMap] (CustomerId, TagId, SortOrder) VALUES (@CustomerId, @TagId, @SortOrder)",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@TagId", tagId),
                new SqlParameter("@SortOrder", sortOrder));
        }

        public static void DeleteMap(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[TagMap] WHERE CustomerID=@CustomerId", CommandType.Text, new SqlParameter("@CustomerId", customerId));
        }
        #endregion
    }
}
