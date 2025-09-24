using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AdvantShop.ExportImport
{
    public class ExportFeedSettingsProvider
    {
        public static ExportFeedSettings GetExportFeedSettingsFromReader(SqlDataReader reader, ExportFeedSettings bindSettings)
        {
            bindSettings.ExportFeedId = SQLDataHelper.GetInt(reader, "ExportFeedId");
            bindSettings.FileName = SQLDataHelper.GetString(reader, "FileName");
            bindSettings.FileExtention = SQLDataHelper.GetString(reader, "FileExtention");
            bindSettings.PriceMarginInPercents = SQLDataHelper.GetFloat(reader, "PriceMarginInPercents");
            bindSettings.PriceMarginInNumbers = SQLDataHelper.GetFloat(reader, "PriceMarginInNumbers");
            bindSettings.AdditionalUrlTags = SQLDataHelper.GetString(reader, "AdditionalUrlTags");
            bindSettings.Active = SQLDataHelper.GetBoolean(reader, "Active");
            bindSettings.IntervalType = SQLDataHelper.GetString(reader, "IntervalType").TryParseEnum<TimeIntervalType>();
            bindSettings.Interval = SQLDataHelper.GetInt(reader, "Interval");
            bindSettings.JobStartTime = SQLDataHelper.GetDateTime(reader, "JobStartTime");
            bindSettings.ExportAllProducts = SQLDataHelper.GetBoolean(reader, "ExportAllProducts");
            bindSettings.ExportAdult = SQLDataHelper.GetBoolean(reader, "ExportAdult", defaultValue: true);
            return bindSettings;
        }
        
        public static ExportFeedSettings GetExportFeedSettingsFromReader(SqlDataReader reader)
        {
            return GetExportFeedSettingsFromReader(reader, new ExportFeedSettings());
        }
        
        public static ExportFeedSettings<TAdvancedSettings> GetExportFeedSettingsFromReader<TAdvancedSettings>(SqlDataReader reader)
        {
            var bindSettings = new ExportFeedSettings<TAdvancedSettings>();
            bindSettings = (ExportFeedSettings<TAdvancedSettings>)GetExportFeedSettingsFromReader(reader, bindSettings);
            bindSettings.AdvancedSettings =
                ConvertAdvancedSettings<TAdvancedSettings>(SQLDataHelper.GetString(reader, "AdvancedSettings"));

            return bindSettings;
        }

        public static ExportFeedSettings GetSettings(int exportFeedId)
        {
            return SQLDataAccess.ExecuteReadOne<ExportFeedSettings>(
                "SELECT TOP 1 * FROM [Settings].[ExportFeedSettings] WHERE [ExportFeedId] = @ExportFeedId",
                CommandType.Text,
                GetExportFeedSettingsFromReader,
                new SqlParameter("@ExportFeedId", exportFeedId)
            );
        }

        public static ExportFeedSettings<TAdvancedSettings> GetSettings<TAdvancedSettings>(int exportFeedId)
        {
            return SQLDataAccess.ExecuteReadOne<ExportFeedSettings<TAdvancedSettings>>(
                "SELECT TOP 1 * FROM [Settings].[ExportFeedSettings] WHERE [ExportFeedId] = @ExportFeedId",
                CommandType.Text,
                GetExportFeedSettingsFromReader<TAdvancedSettings>,
                new SqlParameter("@ExportFeedId", exportFeedId)
            );
        }

        public static ExportFeedSettings<TAdvancedSettings> GetSettingsByParam<TAdvancedSettings>(string name, string value)
        {
            return SQLDataAccess.ExecuteReadOne<ExportFeedSettings<TAdvancedSettings>>(
               string.Format("SELECT TOP 1 * FROM [Settings].[ExportFeedSettings] Where [AdvancedSettings] LIKE '%\"{0}\":\"{1}\"%'", name, value),
               CommandType.Text, GetExportFeedSettingsFromReader<TAdvancedSettings>);
        }

        public static void SetSettings<TAdvancedSettings>(int exportFeedId, ExportFeedSettings<TAdvancedSettings> exportFeedSettings)
        {
            SetSettings(exportFeedId, (ExportFeedSettings)exportFeedSettings);
            SetAdvancedSettings<TAdvancedSettings>(exportFeedId, exportFeedSettings.AdvancedSettings);
        }
        
        public static void SetSettings(int exportFeedId, ExportFeedSettings exportFeedSettings)
        {
            var command = 
                IsExistSettings(exportFeedId) 
                    ? "Update [Settings].[ExportFeedSettings] SET " +
                        "FileName=@FileName, FileExtention=@FileExtention, PriceMarginInPercents=@PriceMarginInPercents, PriceMarginInNumbers=@PriceMarginInNumbers, AdditionalUrlTags=@AdditionalUrlTags, " +
                        "Active=@Active, IntervalType=@IntervalType, Interval=@Interval, JobStartTime=@JobStartTime, " +
                        "ExportAllProducts=@ExportAllProducts, ExportAdult=@ExportAdult " +
                      "WHERE[ExportFeedId] = @ExportFeedId" 
                    
                    : "Insert Into [Settings].[ExportFeedSettings] ([ExportFeedId], [FileName], [FileExtention], [PriceMarginInPercents], [PriceMarginInNumbers], [AdditionalUrlTags], [Active], " +
                                                                    "[IntervalType], [Interval], [JobStartTime], [ExportAllProducts], [ExportAdult]) " +
                                                            "Values (@ExportFeedId, @FileName, @FileExtention, @PriceMarginInPercents, @PriceMarginInNumbers, @AdditionalUrlTags, @Active, " +
                                                                    "@IntervalType,@Interval, @JobStartTime, @ExportAllProducts, @ExportAdult)";

            SQLDataAccess.ExecuteNonQuery(command, CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId),
                new SqlParameter("@FileName", (object) exportFeedSettings.FileName ?? DBNull.Value),
                new SqlParameter("@FileExtention", (object) exportFeedSettings.FileExtention ?? DBNull.Value),
                new SqlParameter("@PriceMarginInPercents", exportFeedSettings.PriceMarginInPercents),
                new SqlParameter("@PriceMarginInNumbers", exportFeedSettings.PriceMarginInNumbers),
                new SqlParameter("@AdditionalUrlTags", (object) exportFeedSettings.AdditionalUrlTags ?? DBNull.Value),
                new SqlParameter("@Active", exportFeedSettings.Active),
                new SqlParameter("@IntervalType", exportFeedSettings.IntervalType),
                new SqlParameter("@Interval", exportFeedSettings.Interval),
                new SqlParameter("@JobStartTime", 
                    exportFeedSettings.JobStartTime == DateTime.MinValue 
                        ? (object)DBNull.Value 
                        : exportFeedSettings.JobStartTime),
                new SqlParameter("@ExportAllProducts", exportFeedSettings.ExportAllProducts),
                new SqlParameter("@ExportAdult", exportFeedSettings.ExportAdult));
        }

        public static bool IsExistSettings(int exportFeedId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "Select Count(ExportFeedId) From [Settings].[ExportFeedSettings] Where ExportFeedId=@ExportFeedId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId));
        }

        public static TAdvancedSettings GetAdvancedSettings<TAdvancedSettings>(int exportFeedId)
        {
            return ConvertAdvancedSettings<TAdvancedSettings>(
                SQLDataAccess.ExecuteScalar<string>(
                    "Select AdvancedSettings From [Settings].[ExportFeedSettings] WHERE [ExportFeedId] = @ExportFeedId",
                    CommandType.Text,
                    new SqlParameter("@ExportFeedId", exportFeedId)));
        }

        public static void SetAdvancedSettings<TAdvancedSettings>(int exportFeedId, TAdvancedSettings advancedSettings)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Settings].[ExportFeedSettings] Set AdvancedSettings = @AdvancedSettings WHERE [ExportFeedId] = @ExportFeedId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId),
                new SqlParameter("@AdvancedSettings",
                    ConvertAdvancedSettings<TAdvancedSettings>(advancedSettings) ?? (object)DBNull.Value));
        }

        public static T ConvertAdvancedSettings<T>(string advancedSettings)
        {
            if (advancedSettings.IsNullOrEmpty())
                return default;
            return JsonConvert.DeserializeObject<T>(advancedSettings);
        }

        public static string ConvertAdvancedSettings<T>(T advancedSettings)
        {
            if (advancedSettings == null)
                return null;
            return JsonConvert.SerializeObject(advancedSettings);
        }
    }
}