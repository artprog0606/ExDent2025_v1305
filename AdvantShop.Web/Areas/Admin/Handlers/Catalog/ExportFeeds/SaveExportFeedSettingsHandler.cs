using System;
using AdvantShop.Core;
using AdvantShop.Core.Scheduler;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class SaveExportFeedSettingsHandler
    {
        private readonly ExportFeedSettingsModel _commonSettings;
        private readonly string _advancedSettingsJson;
        private readonly int _exportFeedId;
        private readonly string _exportFeedName;
        private readonly string _exportFeedDescription;

        public SaveExportFeedSettingsHandler(int exportFeedId, string exportFeedName, string exportFeedDescription, ExportFeedSettingsModel commonSettings, string advancedSettings)
        {
            _exportFeedId = exportFeedId;
            _exportFeedName = exportFeedName;
            _exportFeedDescription = exportFeedDescription;

            _commonSettings = commonSettings;
            _advancedSettingsJson = advancedSettings;
        }

        public CommandResult Execute()
        {
            var exportFeed = ExportFeedService.GetExportFeed(_exportFeedId);
            if (exportFeed == null)
                return new CommandResult { Result = false, Error = "Not found export feed" };

            if (!string.IsNullOrEmpty(_exportFeedName))
            {
                exportFeed.Name = _exportFeedName;
            }

            exportFeed.Description = _exportFeedDescription;
            ExportFeedService.UpdateExportFeed(exportFeed);

            //удаляем корневую категорию, еслт включено выгружать все категории то добавляем снова
            ExportFeedService.DeleteCategory(exportFeed.Id, 0);

            var exportFeedSettings = ExportFeedSettingsProvider.GetSettings(_exportFeedId);

            var jobEnabled = _commonSettings.Active && !exportFeedSettings.Active;

            exportFeedSettings.Active = _commonSettings.Active;
            exportFeedSettings.Interval = _commonSettings.Interval;
            exportFeedSettings.IntervalType = _commonSettings.IntervalType;
            exportFeedSettings.JobStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _commonSettings.JobStartHour, _commonSettings.JobStartMinute, 0);

            exportFeedSettings.FileName = _commonSettings.FileName;
            exportFeedSettings.FileExtention = _commonSettings.FileExtention;
            exportFeedSettings.PriceMarginInPercents = _commonSettings.PriceMarginInPercents;
            exportFeedSettings.PriceMarginInNumbers = _commonSettings.PriceMarginInNumbers;
            exportFeedSettings.AdditionalUrlTags = _commonSettings.AdditionalUrlTags;

            exportFeedSettings.ExportAllProducts = _commonSettings.ExportCatalogType == EExportFeedCatalogType.AllProducts; //_commonSettings.ExportAllProducts;
            exportFeedSettings.ExportAdult = !_commonSettings.DoNotExportAdult;

            ExportFeedSettingsProvider.SetSettings(_exportFeedId, exportFeedSettings);

            if (!string.IsNullOrEmpty(_advancedSettingsJson))
            {
                switch (exportFeed.FeedType)
                {
                    case EExportFeedType.Avito:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedAvitoOptions>(_exportFeedId, 
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedAvitoOptions>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.Csv:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedCsvOptions>(_exportFeedId, 
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedCsvOptions>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.CsvV2:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedCsvV2Options>(_exportFeedId, 
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedCsvV2Options>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.Facebook:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedFacebookOptions>(_exportFeedId, 
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedFacebookOptions>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.YandexMarket:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedYandexOptions>(_exportFeedId, 
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedYandexOptions>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.GoogleMerchentCenter:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedGoogleMerchantCenterOptions>(_exportFeedId, 
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedGoogleMerchantCenterOptions>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.Reseller:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedResellerOptions>(_exportFeedId, 
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedResellerOptions>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.YandexDirect:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedYandexDirectOptions>(_exportFeedId,
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedYandexDirectOptions>(_advancedSettingsJson));
                        break;
                    case EExportFeedType.YandexWebmaster:
                        ExportFeedSettingsProvider.SetAdvancedSettings<ExportFeedYandexWebmasterOptions>(_exportFeedId,
                            ExportFeedSettingsProvider.ConvertAdvancedSettings<ExportFeedYandexWebmasterOptions>(_advancedSettingsJson));
                        break;

                    default:
                        throw new BlException("Unknown type of advanced settings");

                }
            }

            if (exportFeedSettings.ExportAllProducts)
            {
                //добавляем корневую категорию, при этом не удаляются выбранные отдельные категории
                ExportFeedService.InsertCategory(exportFeed.Id, 0, false);
            }

            var taskSetting = ExportFeedService.GetTaskSettingByExportFeed(exportFeed, exportFeedSettings);
            TaskManager.TaskManagerInstance().AddUpdateTask(taskSetting, TaskManager.TaskGroup);


            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_EditExportFeed, exportFeed.FeedType.ToString());
            if (jobEnabled)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_EnableJob, exportFeed.FeedType.ToString());

            return new CommandResult { Result = true };
        }
    }
}
