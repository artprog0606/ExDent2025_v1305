using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Core.Services.Files
{
    public class FileInDbService
    {
        private const string FileInDbCacheKey = "FileInDb_";

        public static void Add(FileInDb file)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Settings].[Files] (Name, Path, ContentType, Content, CreatedDate, ModifiedDate, Charset) " +
                "Values (@Name, @Path, @ContentType, @Content, getdate(), getdate(), @Charset)",
                CommandType.Text,
                new SqlParameter("@Name", file.Name.ToLower()),
                new SqlParameter("@Path", file.Path.ToLower()),
                new SqlParameter("@ContentType", file.ContentType),
                new SqlParameter("@Content", file.Content ?? new byte[0]),
                new SqlParameter("@Charset", file.Charset ?? (object)DBNull.Value));

            UpdateCache();
        }

        public static void Update(string path, byte[] content)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Settings].[Files] Set Content=@Content, ModifiedDate = getdate() Where Path=@Path",
                CommandType.Text,
                new SqlParameter("@Path", path),
                new SqlParameter("@Content", content ?? new byte[0]));

            UpdateCache();
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Settings].[Files] Where Id=@Id", CommandType.Text, new SqlParameter("@Id", id));

            UpdateCache();
        }

        public static bool IsExist(string path)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(Id) from [Settings].[Files] Where Path=@Path", CommandType.Text, 
                new SqlParameter("@Path", path)) > 0;
        }

        public static HashSet<string> GetPaths()
        {
            return new HashSet<string>(SQLDataAccess.Query<string>("Select [Path] from [Settings].[Files]"));
        }

        public static FileInDb Get(string path)
        {
            return CacheManager.Get(FileInDbCacheKey + path,
                () => SQLDataAccess.Query<FileInDb>("Select * from [Settings].[Files] Where path=@path", new { path })
                    .FirstOrDefault());
        }

        public static List<FileInDb> GetList()
        {
            return SQLDataAccess.Query<FileInDb>("Select * from [Settings].[Files]").ToList();
        }

        private static void UpdateCache()
        {
            CacheManager.RemoveByPattern(FileInDbCacheKey);
            UrlService.UpdateFileInDbPaths();
        }
    }
}
