﻿using System;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class GetExportFeedAdvancedSettingsModel
    {
        private int _exportFeedId;
        private EExportFeedType _exportFeedType;
        private readonly object _advancedSettings;


        public GetExportFeedAdvancedSettingsModel(int exportFeedId, EExportFeedType exportFeedType, object advancedSettings)
        {
            _exportFeedId = exportFeedId;
            _exportFeedType = exportFeedType;
            _advancedSettings = advancedSettings;
        }

        public object Execute()
        {
            switch (_exportFeedType)
            {
                case EExportFeedType.Reseller:
                    return new ExportFeedSettingsResellerModel((ExportFeedResellerOptions)_advancedSettings);                    
                case EExportFeedType.YandexMarket:
                    return new ExportFeedSettingsYandexModel((ExportFeedYandexOptions)_advancedSettings) { ExportFeedId = _exportFeedId };
                case EExportFeedType.Csv:     
                    return new ExportFeedSettingsCsvModel((ExportFeedCsvOptions)_advancedSettings);
                case EExportFeedType.CsvV2:
                    return new ExportFeedSettingsCsvV2Model((ExportFeedCsvV2Options)_advancedSettings);
                case EExportFeedType.GoogleMerchentCenter:
                    return new ExportFeedSettingsGoogleModel((ExportFeedGoogleMerchantCenterOptions)_advancedSettings);
                case EExportFeedType.Avito:
                    return new ExportFeedSettingsAvitoModel((ExportFeedAvitoOptions)_advancedSettings);
                case EExportFeedType.Facebook:
                    return new ExportFeedSettingsFacebookModel((ExportFeedFacebookOptions)_advancedSettings);
                case EExportFeedType.YandexDirect:
                    return new ExportFeedSettingsYandexDirectModel((ExportFeedYandexDirectOptions)_advancedSettings);
                case EExportFeedType.YandexWebmaster:
                    return new ExportFeedSettingsYandexWebmasterModel((ExportFeedYandexWebmasterOptions)_advancedSettings);
                default:
                    throw new NotImplementedException("No implementation for exportfeed type " + _exportFeedType);
            }
        }
    }
}