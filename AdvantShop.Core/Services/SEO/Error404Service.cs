//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;

namespace AdvantShop.SEO
{
    public class Error404Service
    {
        private static Error404 GetFromReader(SqlDataReader reader)
        {
            return new Error404
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Url = SQLDataHelper.GetString(reader, "Url"),
                UrlReferer = SQLDataHelper.GetString(reader, "UrlReferer"),
                UserAgent = SQLDataHelper.GetString(reader, "UserAgent"),
                IpAddress = SQLDataHelper.GetString(reader, "IpAddress"),
                DateAdded = SQLDataHelper.GetDateTime(reader, "DateAdded")
            };
        }
        
        public static Error404 GetError404(string url, string urlReferer, string ipAddress, string userAgent)
        {
            return SQLDataAccess.ExecuteReadOne<Error404>(
                "SELECT TOP 1 * FROM Settings.Error404 WHERE Url = @Url AND UrlReferer = @UrlReferer AND IpAddress = @IpAddress AND UserAgent = @UserAgent",
                CommandType.Text, 
                GetFromReader,
                new SqlParameter("@Url", url),
                new SqlParameter("@UrlReferer", urlReferer ?? (object)DBNull.Value),
                new SqlParameter("@UserAgent", userAgent ?? (object)DBNull.Value),
                new SqlParameter("@IpAddress", ipAddress ?? (object)DBNull.Value));
        }
        public static Error404 GetError404ByHash(int hash)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM Settings.Error404 WHERE Hash = @Hash",
                CommandType.Text, 
                GetFromReader,
                new SqlParameter("@Hash", hash));
        }

        public static void AddError404(Error404 error404)
        {
            if (error404.Url.IsNullOrEmpty())
                return;

            var hash = error404.GetHashCode();
            
            var error = GetError404ByHash(hash);
            if (error != null)
            {
                if ((DateTime.Now - error.DateAdded).TotalHours > 6)
                {
                    UpdateError404Time(error.Id);
                }
                return;
            }

            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Settings.Error404 (Url, UrlReferer, IpAddress, UserAgent, DateAdded, Hash) VALUES (@Url, @UrlReferer, @IpAddress, @UserAgent, getdate(), @Hash)",
                CommandType.Text,
                new SqlParameter("@Url", error404.Url),
                new SqlParameter("@UrlReferer", error404.UrlReferer ?? (object)DBNull.Value),
                new SqlParameter("@UserAgent", error404.UserAgent ?? (object)DBNull.Value),
                new SqlParameter("@IpAddress", error404.IpAddress ?? (object)DBNull.Value),
                new SqlParameter("@Hash", hash));
        }

        public static void UpdateError404Time(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Update Settings.Error404 Set DateAdded=getdate() WHERE Id=@Id",
                CommandType.Text, new SqlParameter("@Id", id));
        }

        public static void DeleteError404(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Settings.Error404 WHERE Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }
        
        public static void DeleteExpired()
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete from Settings.Error404 Where DATEADD(month, 1, DateAdded) > getdate()",
                CommandType.Text);
        }
    }
}