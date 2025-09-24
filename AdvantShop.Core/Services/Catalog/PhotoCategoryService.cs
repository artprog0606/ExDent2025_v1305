using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using VkNet.Categories;

namespace AdvantShop.Core.Services.Catalog
{
    public class PhotoCategoryService
    {
        private const string PhotoCategoryCaheKey = "PhotoCategory_";
        private const string AllPhotoCategoryCaheKey = "PhotoCategory_All";

        public static PhotoCategory Get(int photoCategoryId)
        {
            return
                CacheManager.Get(PhotoCategoryCaheKey + photoCategoryId, () =>
                    SQLDataAccess.ExecuteReadOne(
                        "SELECT TOP 1 * FROM Catalog.PhotoCategory WHERE Id=@Id",
                        CommandType.Text, GetFromReader,
                        new SqlParameter("@Id", photoCategoryId)));
        }

        public static List<PhotoCategory> GetAllPhotoCategories(bool enabled = false)
        {
            return CacheManager.Get(AllPhotoCategoryCaheKey, () =>
                        SQLDataAccess.ExecuteReadList("SELECT * FROM Catalog.PhotoCategory" + (enabled ? " where Enabled = 1" : string.Empty), CommandType.Text, GetFromReader));
        }

        private static PhotoCategory GetFromReader(SqlDataReader reader)
        {
            return new PhotoCategory()
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static int Add(PhotoCategory photoCategory)
        {
            photoCategory.Id = SQLDataAccess.ExecuteScalar<int>("INSERT INTO [Catalog].[PhotoCategory] (Name, Enabled, SortOrder) VALUES (@Name, @Enabled, @SortOrder)", CommandType.Text,
                                                        new SqlParameter("@Name", photoCategory.Name),
                                                        new SqlParameter("@Enabled", photoCategory.Enabled),
                                                        new SqlParameter("@SortOrder", photoCategory.SortOrder));

            CacheManager.Remove(AllPhotoCategoryCaheKey);
            return photoCategory.Id;
        }

        public static void Update(PhotoCategory photoCategory)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Catalog].[PhotoCategory] SET Name = @Name, Enabled = @Enabled, SortOrder = @SortOrder WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", photoCategory.Id),
                new SqlParameter("@Name", photoCategory.Name),
                new SqlParameter("@Enabled", photoCategory.Enabled),
                new SqlParameter("@SortOrder", photoCategory.SortOrder));

            CacheManager.RemoveByPattern(PhotoCategoryCaheKey);
        }

        public static void Delete(int photoCategoryId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.PhotoCategory WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", photoCategoryId));

            CacheManager.RemoveByPattern(PhotoCategoryCaheKey);
        }
    }
}
