using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using System;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket.Export
{
    public enum OkMarketAddLinkToSiteMode
    {
        No = 0,
        Top = 1,
        Bottom = 2
    }

    public enum OkMarketShowDescriptionMode
    {
        No = 0,
        Full = 1,
        Short = 2
    }

    public class OkMarketExportSettings
    {
        public static string CurrencyIso3
        {
            get => SettingProvider.Items["OkMarketExportSettings.Currency"];
            set => SettingProvider.Items["OkMarketExportSettings.Currency"] = value;
        }
        
        public static bool ExportPreorderProducts
        {
            get => !string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.ExportPreorderProducts"]) && Convert.ToBoolean(SettingProvider.Items["OkMarketExportSettings.ExportPreorderProducts"]);
            set => SettingProvider.Items["OkMarketExportSettings.ExportPreorderProducts"] = value.ToLowerString();
        }

        public static bool ExportUnavailableProducts
        {
            get => !string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.ExportUnavailableProducts"]) && Convert.ToBoolean(SettingProvider.Items["OkMarketExportSettings.ExportUnavailableProducts"]);
            set => SettingProvider.Items["OkMarketExportSettings.ExportUnavailableProducts"] = value.ToLowerString();
        }

        public static bool SizeAndColorInName
        {
            get => !string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.SizeAndColorInName"]) && Convert.ToBoolean(SettingProvider.Items["OkMarketExportSettings.SizeAndColorInName"]);
            set => SettingProvider.Items["OkMarketExportSettings.SizeAndColorInName"] = value.ToLowerString();
        }

        public static bool UpdateProductPhotos
        {
            get => !string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.UpdateProductPhotos"]) && Convert.ToBoolean(SettingProvider.Items["OkMarketExportSettings.UpdateProductPhotos"]);
            set => SettingProvider.Items["OkMarketExportSettings.UpdateProductPhotos"] = value.ToLowerString();
        }

        public static bool SizeAndColorInDescription
        {
            get => string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.SizeAndColorInDescription"]) || Convert.ToBoolean(SettingProvider.Items["OkMarketExportSettings.SizeAndColorInDescription"]);
            set => SettingProvider.Items["OkMarketExportSettings.SizeAndColorInDescription"] = value.ToLowerString();
        }

        public static bool ExportProperties
        {
            get => string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.ExportProperties"]) || Convert.ToBoolean(SettingProvider.Items["OkMarketExportSettings.ExportProperties"]);
            set => SettingProvider.Items["OkMarketExportSettings.ExportProperties"] = value.ToString();
        }

        public static bool ExportOnShedule
        {
            get =>
                !string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.ExportOnShedule"])
                    ? Convert.ToBoolean(SettingProvider.Items["OkMarketExportSettings.ExportOnShedule"])
                    : false;
            set => SettingProvider.Items["OkMarketExportSettings.ExportOnShedule"] = value.ToString();
        }

        public static OkMarketAddLinkToSiteMode ExportLinkToSite
        {
            get =>
                !string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.ExportLinkToSite"])
                    ? (OkMarketAddLinkToSiteMode)SettingProvider.Items["OkMarketExportSettings.ExportLinkToSite"].TryParseInt()
                    : OkMarketAddLinkToSiteMode.Top;
            set => SettingProvider.Items["OkMarketExportSettings.ExportLinkToSite"] = ((int)value).ToString();
        }

        public static OkMarketShowDescriptionMode ExportDescription
        {
            get =>
                !string.IsNullOrEmpty(SettingProvider.Items["OkMarketExportSettings.ExportDescription"])
                    ? (OkMarketShowDescriptionMode)SettingProvider.Items["OkMarketExportSettings.ExportDescription"].TryParseInt()
                    : OkMarketShowDescriptionMode.Short;
            set => SettingProvider.Items["OkMarketExportSettings.ExportDescription"] = ((int)value).ToString();
        }
    }
}