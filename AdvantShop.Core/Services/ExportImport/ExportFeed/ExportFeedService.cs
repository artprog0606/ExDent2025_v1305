using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.Jobs;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Saas;

namespace AdvantShop.ExportImport
{
    public class ExportFeedService
    {
        #region Add,Update,Delete

        public static int AddExportFeed(ExportFeed exportFeed)
        {
            return SQLDataAccess.ExecuteScalar<int>("INSERT INTO [Settings].[ExportFeed] ([Name], [Type], [Description],[LastExport],[LastExportFileFullName]) VALUES(@Name, @Type, @Description,NULL,NULL);  SELECT scope_identity();",
                                         CommandType.Text,
                                         new SqlParameter("@Name", exportFeed.Name),
                                         new SqlParameter("@Type", exportFeed.Type ?? exportFeed.FeedType.ToString()),
                                         new SqlParameter("@Description", exportFeed.Description ?? (object)DBNull.Value));
        }

        public static ExportFeed GetExportFeedFromReader(SqlDataReader reader)
        {
            return new ExportFeed
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Type = SQLDataHelper.GetString(reader, "Type"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                LastExport = SQLDataHelper.GetNullableDateTime(reader, "LastExport"),
                LastExportFileFullName = SQLDataHelper.GetString(reader, "LastExportFileFullName")
            };
        }

        public static ExportFeed GetExportFeed(int id)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM [Settings].[ExportFeed] WHERE [Id] = @id",
                                         CommandType.Text,
                                         GetExportFeedFromReader,
                                         new SqlParameter("@Id", id));
        }

        public static List<ExportFeed> GetExportFeeds()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[ExportFeed] Order By [Type],[Id]", 
                CommandType.Text, GetExportFeedFromReader);
        }

        public static List<ExportFeed> GetExportFeeds(EExportFeedType exportFeedType)
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Settings].[ExportFeed] Where [Type] = @ExportFeedType",
                CommandType.Text, GetExportFeedFromReader,
                new SqlParameter("@ExportFeedType", exportFeedType.ToString()));
        }

        public static void UpdateExportFeed(ExportFeed exportFeed)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Settings].[ExportFeed] Set [Name]=@Name, [Type]=@Type, [Description]=@Description, [LastExport]=@LastExport, [LastExportFileFullName]=@LastExportFileFullName Where [Id]=@Id",
                CommandType.Text,
                new SqlParameter("@Id", exportFeed.Id),
                new SqlParameter("@Name", exportFeed.Name),
                new SqlParameter("@Type",  exportFeed.Type ?? exportFeed.FeedType.ToString()),
                new SqlParameter("@Description", exportFeed.Description ?? (object) DBNull.Value),
                new SqlParameter("@LastExport", exportFeed.LastExport ?? (object) DBNull.Value),
                new SqlParameter("@LastExportFileFullName", string.IsNullOrEmpty(exportFeed.LastExportFileFullName)
                    ? (object) DBNull.Value
                    : exportFeed.LastExportFileFullName));
        }

        public static void UpdateExportFeedLastExport(int id, DateTime? date, string fileFullName)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Settings].[ExportFeed] Set [LastExport]=@LastExport, [LastExportFileFullName]=@LastExportFileFullName Where [Id]=@Id",
                CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@LastExport", date ?? (object)DBNull.Value),
                new SqlParameter("@LastExportFileFullName", fileFullName.IsNotEmpty() ? fileFullName : (object)DBNull.Value));
        }

        public static void DeleteExportFeed(int id)
        {
            DeleteTaskJob(id);
            
            SQLDataAccess.ExecuteNonQuery("Delete FROM [Settings].[ExportFeed] Where [Id]=@Id",
                                       CommandType.Text,
                                       new SqlParameter("@Id", id));
        }

        public static void DeleteTaskJob(int exportFeedId)
        {
            var feed = GetExportFeed(exportFeedId);
            if (feed == null)
                return;
            
            var feedSettings = ExportFeedSettingsProvider.GetSettings(feed.Id);
            if (feedSettings == null)
                return;

            var setting = GetTaskSettingByExportFeed(feed, feedSettings);
            TaskManager.TaskManagerInstance().RemoveTask(setting.GetUniqueName(), TaskManager.TaskGroup);
        }
        
        public static bool IsExistExportFile(string fileName, string fileExtension)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "if exists (Select 1 From [Settings].[ExportFeedSettings] Where FileName= @fileName and FileExtention = @fileExtension) Select 1 Else Select 0",
                CommandType.Text,
                new SqlParameter("@fileName", fileName),
                new SqlParameter("@fileExtension", fileExtension));
        }

        public static string GetNewExportFileName(int exportFeedId, string fileName, string fileExtension)
        {
            for (var i = 0; i < 10; i++)
            {
                if (File.Exists(SettingsGeneral.AbsolutePath + $"/{fileName}.{fileExtension}") ||
                    IsExistExportFile(fileName, fileExtension))
                {
                    fileName += exportFeedId + (i != 0 ? "-" + i : "");
                    continue;
                }

                return fileName;
            }

            return fileName + Guid.NewGuid().ToString("N");
        }

        public static void DeleteFileExportFeed(int id, string fullFileName)
        {
            if (!string.IsNullOrEmpty(fullFileName))
            {
                var filePath = HttpContext.Current.Server.MapPath("~/" + fullFileName);
                FileHelpers.DeleteFile(filePath);
            }

            SQLDataAccess.ExecuteNonQuery(
                 "Update [Settings].[ExportFeed] Set [LastExport]=NULL, [LastExportFileFullName]=NULL Where [Id]=@Id",
                 CommandType.Text,
                 new SqlParameter("@Id", id));
        }

        #endregion Add,Update,Delete

        #region Categories and Products

        public static bool CheckCategory(int exportFeedId, int catId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select count(*) from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId and CategoryID=@CategoryID",
                                                    CommandType.Text,
                                                    new SqlParameter("@exportFeedId", exportFeedId),
                                                    new SqlParameter("@CategoryID", catId)) > 0;
        }

        public static bool CheckCategoryHierical(int exportFeedId, int catId)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Select count(*) from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId " +
                    " and CategoryID in (Select id from [Settings].[GetParentsCategoryByChild](@CategoryID) union select 0) and CategoryID<> @CategoryID",
                    CommandType.Text, new SqlParameter("@exportFeedId", exportFeedId), new SqlParameter("@CategoryID", catId)) > 0;
        }

        public static void InsertCategory(int exportFeedId, int catId)
        {
            InsertCategory(exportFeedId, catId, false);
        }

        public static void InsertCategory(int exportFeedId, int catId, bool opened)
        {
            if (CheckCategory(exportFeedId, catId)) return;
            SQLDataAccess.ExecuteScalar<int>("Insert into Settings.ExportFeedSelectedCategories (ExportFeedId, CategoryID, Opened) VALUES (@exportFeedId, @CategoryID, @Opened)",
                                                   CommandType.Text,
                                                   new SqlParameter("@exportFeedId", exportFeedId),
                                                   new SqlParameter("@CategoryID", catId),
                                                   new SqlParameter("@Opened", opened));
        }

        public static void DeleteCategory(int exportFeedId, int catId)
        {
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId and CategoryID=@CategoryID",
                                                   CommandType.Text,
                                                   new SqlParameter("@exportFeedId", exportFeedId),
                                                   new SqlParameter("@CategoryID", catId));
        }

        /// <summary>
        /// for adminv2
        /// </summary>
        /// <param name="exportFeedId"></param>
        /// <param name="categories"></param>
        public static void InsertCategories(int exportFeedId, List<ExportFeedSelectedCategory> categories)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Settings.ExportFeedSelectedCategories Where ExportFeedId = @ExportFeedId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId));
            if (categories == null)
            {
                return;
            }

            foreach (var category in categories)
            {
                InsertCategory(exportFeedId, category.CategoryId, category.Opened);
            }
        }

        /// <summary>
        /// for adminv2
        /// </summary>
        /// <param name="exportFeedId"></param>
        /// <returns></returns>
        public static bool IsExportAllCategories(int exportFeedId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "Select CASE WHEN Count(CategoryID) = 1 THEN 1  Else 0  END From Settings.ExportFeedSelectedCategories Where ExportFeedId = @ExportFeedId and CategoryID = 0",
                CommandType.Text,
                new SqlParameter("@exportFeedId", exportFeedId));
        }

        public static EExportFeedCatalogType GetExportFeedCatalogType(int exportFeedId)
        {
            var isExportAllCategories = IsExportAllCategories(exportFeedId);
            return isExportAllCategories ? EExportFeedCatalogType.AllProducts : EExportFeedCatalogType.Categories;
        }

        public static List<ExportFeedSelectedCategory> GetExportFeedCategoriesId(int exportFeedId)
        {
            return SQLDataAccess.Query<ExportFeedSelectedCategory>(
                "Select [CategoryId], [Opened] From Settings.ExportFeedSelectedCategories Where ExportFeedId = @ExportFeedId",
                new { exportFeedId = exportFeedId }).ToList();
        }

        public static void DeleteModule(int exportFeedId)
        {
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId",
                                                   CommandType.Text, new SqlParameter("@exportFeedId", exportFeedId));
        }

        public static void AddExcludeProduct(int exportFeedId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT([ProductId]) FROM [Settings].[ExportFeedExcludedProducts] WHERE [ExportFeedId] = @ExportFeedId AND [ProductId] = @ProductId) = 0 " +
                "BEGIN INSERT INTO [Settings].[ExportFeedExcludedProducts] ([ExportFeedId],[ProductId]) VALUES (@ExportFeedId,@ProductId) END",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId),
                new SqlParameter("@ProductId", productId));
        }

        public static void DeleteExcludeProduct(int exportFeedId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Settings].[ExportFeedExcludedProducts] WHERE [ExportFeedId]=@ExportFeedId AND [ProductId]=@ProductId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId),
                new SqlParameter("@ProductId", productId));
        }


        public static void FillExportFeedCategoriesCache(int exportFeedId, bool exportNotAvailable)
        {
            // clear ExportFeedCategoriesCache inside stored procedure
            SQLDataAccess.ExecuteNonQuery("Settings.sp_FillExportFeedCategoriesCache", CommandType.StoredProcedure,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@exportNotAvailable", exportNotAvailable));
        }

        public static void ClearExportFeedCategoriesCache(int exportFeedId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[ExportFeedCategoriesCache] WHERE ExportFeedId = @ExportFeedId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId));
        }

        #endregion Categories and Products


        public static TaskSetting GetTaskSettingByExportFeed(ExportFeed feed, ExportFeedSettings feedSettings)
        {
            var item = new TaskSetting
            {
                Enabled = feedSettings.Active,
                JobType = typeof(GenerateExportFeedJob).ToString(),
                TimeInterval = feedSettings.Interval,
                TimeHours = feedSettings.IntervalType == TimeIntervalType.Days ? feedSettings.JobStartTime.Hour : 0,
                TimeMinutes = feedSettings.IntervalType == TimeIntervalType.Days ? feedSettings.JobStartTime.Minute : 0,
                TimeType = feedSettings.IntervalType,
                DataMap = feed.Id
            };

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.ExportFeedsAutoUpdate &&
                (feed.FeedType == EExportFeedType.YandexMarket ||
                 feed.FeedType == EExportFeedType.YandexDirect ||
                 feed.FeedType == EExportFeedType.YandexWebmaster ||
                 feed.FeedType == EExportFeedType.GoogleMerchentCenter || 
                 feed.FeedType == EExportFeedType.Avito))
            {
                item.Enabled = false;
            }

            return item;
        }
        
        public static List<TaskSetting> GetExportFeedTaskSettings()
        {
            var taskSettings = new List<TaskSetting>();
            foreach (var feed in GetExportFeeds())
            {
                var feedSettings = ExportFeedSettingsProvider.GetSettings(feed.Id);
                if (feedSettings == null)
                    continue;

                taskSettings.Add(GetTaskSettingByExportFeed(feed, feedSettings));
            }
            return taskSettings;
        }

        public static IExportFeed GetExportFeedInstance(string feedType, int exportFeedId, bool useCommonStatistic = false)
        {
            var type = ReflectionExt.GetTypeByAttributeValue<ExportFeedKeyAttribute>(typeof(IExportFeed), atr => atr.Value, feedType);
            return (IExportFeed)Activator.CreateInstance(type, exportFeedId, useCommonStatistic);
        }

        public static bool AllowShowExportFeedInCatalog(EExportFeedType exportFeedType)
        {
            return exportFeedType == EExportFeedType.Csv 
                    || exportFeedType == EExportFeedType.CsvV2 
                    || exportFeedType == EExportFeedType.YandexMarket 
                    || exportFeedType == EExportFeedType.YandexDirect
                    || exportFeedType == EExportFeedType.YandexWebmaster;
        }
    }
}