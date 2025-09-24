//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class CategoryService
    {
        public static Category Get(int id)
        {
            return
                SQLDataAccess.Query<Category>("SELECT * FROM [Booking].[Category] WHERE Id = @Id", new {Id = id})
                    .FirstOrDefault();
        }

        public static List<Category> GetList()
        {
            return SQLDataAccess.Query<Category>("SELECT * FROM [Booking].[Category] Order by SortOrder").ToList();
        }

        public static List<int> GetListId()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[Category]", CommandType.Text, "Id");
        }

        public static int Add(Category category)
        {
            return SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[Category] " +
                                                    " ([Name], [Image], [SortOrder], [Enabled]) " +
                                                    " VALUES (@Name, @Image, @SortOrder, @Enabled); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", category.Name ?? (object) DBNull.Value),
                new SqlParameter("@Image", category.Image ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", category.SortOrder),
                new SqlParameter("@Enabled", category.Enabled)
                );
        }

        public static void Update(Category category)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[Category] SET [Name] = @Name, [Image] = @Image, [SortOrder] = @SortOrder, [Enabled] = @Enabled " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", category.Id),
                new SqlParameter("@Name", category.Name ?? (object) DBNull.Value),
                new SqlParameter("@Image", category.Image ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", category.SortOrder),
                new SqlParameter("@Enabled", category.Enabled)
                );
        }

        public static void Delete(int id)
        {
            BeforeDelete(id);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[Category] WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        private static void BeforeDelete(int id)
        {
            var category = Get(id);
            if (category != null)
            {
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingCategory, category.Image));

                ServiceService.DeleteByCategory(id);
            }
        }

        #region AffiliateCategory

        public static List<Category> GetList(int affiliateId)
        {
            return
                SQLDataAccess.Query<Category>("SELECT [Category].* FROM [Booking].[Category]" +
                                              "   INNER JOIN [Booking].[AffiliateCategory] ON [AffiliateCategory].[CategoryId] = [Category].[Id]" +
                                              "WHERE AffiliateId = @AffiliateId Order by [Category].SortOrder",
                    new { AffiliateId = affiliateId }).ToList();
        }

        public static List<int> GetListId(int affiliateId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT [Category].[Id] FROM [Booking].[Category]" +
                "   INNER JOIN [Booking].[AffiliateCategory] ON [AffiliateCategory].[CategoryId] = [Category].[Id]" +
                "WHERE AffiliateId = @AffiliateId", CommandType.Text, "Id",
                new SqlParameter("@AffiliateId", affiliateId));
        }

        public static bool ExistRefAffiliate(int categoryId, int affiliateId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS(SELECT 1 FROM [Booking].[AffiliateCategory] WHERE [AffiliateId] = @AffiliateId AND [CategoryId] = @CategoryId)
                begin 
                    SELECT 1
                end
                else
                begin 
                    SELECT 0
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@CategoryId", categoryId)
                );
        }

        public static void AddRefAffiliate(int categoryId, int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[AffiliateCategory] WHERE [AffiliateId] = @AffiliateId AND [CategoryId] = @CategoryId)
                begin 
                    INSERT INTO [Booking].[AffiliateCategory] ([AffiliateId],[CategoryId]) VALUES (@AffiliateId, @CategoryId)
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@CategoryId", categoryId)
                );

            var category = Get(categoryId);
            category.Enabled = true;
            Update(category);
        }

        public static void DeleteRefByAffiliate(int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateCategory] WHERE AffiliateId = @AffiliateId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId));

            ServiceService.DeleteRefByAffiliate(affiliateId);
        }

        public static void DeleteRefAffiliate(int categoryId, int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateCategory] WHERE AffiliateId = @AffiliateId AND CategoryId = @CategoryId", CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@CategoryId", categoryId));

            ServiceService.GetListId(categoryId).ForEach(id => ServiceService.DeleteRefAffiliate(id, affiliateId));
            var category = Get(categoryId);
            category.Enabled = false;
            Update(category);
        }

        #endregion
    }
}