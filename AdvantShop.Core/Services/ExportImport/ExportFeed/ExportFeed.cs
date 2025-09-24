//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using System;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.ExportImport
{
    public enum EExportFeedType
    {
        None,

        [Localize("Core.ExportImport.ExportFeed.YandexType")]
        [StringName("Yandex")]
        YandexMarket,

        [Localize("Core.ExportImport.ExportFeed.GoogleType")]
        [StringName("Google")]
        GoogleMerchentCenter,

        [Localize("Core.ExportImport.ExportFeed.CsvType")]
        [StringName("Csv")]
        Csv,

        [Localize("Core.ExportImport.ExportFeed.CsvV2Type")]
        [StringName("Csv")]
        CsvV2,

        [Localize("Core.ExportImport.ExportFeed.ResellerType")]
        [StringName("Reseller")]
        Reseller,

        [Localize("Core.ExportImport.ExportFeed.Avito")]
        [StringName("Avito")]
        Avito,

        [Localize("Core.ExportImport.ExportFeed.Facebook")]
        [StringName("Facebook")]
        Facebook,

        [Localize("Core.ExportImport.ExportFeed.YandexDirectType")]
        [StringName("YandexDirect")]
        YandexDirect,

        [Localize("Core.ExportImport.ExportFeed.YandexWebmasterType")]
        [StringName("YandexWebmaster")]
        YandexWebmaster,
    }

    public enum EncodingsEnum
    {
        [StringName("Windows-1251")]
        Windows1251,

        [StringName("UTF-8")]
        Utf8,

        [StringName("UTF-16")]
        Utf16,

        [StringName("KOI8-R")]
        Koi8R
    }

    public enum SeparatorsEnum
    {
        [StringName(",")]
        [Localize("Core.ExportImport.Separarator.Comma")]
        CommaSeparated,

        [StringName("\t")]
        [Localize("Core.ExportImport.Separarator.Tab")]
        TabSeparated,

        [StringName(";")]
        [Localize("Core.ExportImport.Separarator.Semicolon")]
        SemicolonSeparated,

        [StringName("")]
        [Localize("Core.ExportImport.Separarator.Custom")]
        Custom
    }

    public enum EExportFeedCatalogType
    {
        [StringName("AllProducts")]
        [Localize("Core.ExportImport.ExportCatalogType.AllProducts")]
        AllProducts = 0,

        [StringName("Categories")]
        [Localize("Core.ExportImport.ExportCatalogType.Categories")]
        Categories = 1
    }

    public enum EImportRemainsType
    {
        [StringName("normal")]
        [Localize("Admin.Catalog.NormalMode")]
        Normal,

        [StringName("remains")]
        [Localize("Admin.Catalog.ResiduesMode")]
        Remains
    }

    public class ExportFeed
    {
        public int Id { get; set; }

        public string Name { get; set; }

        private EExportFeedType? _type;
        public EExportFeedType FeedType => _type ?? (_type = Type.TryParseEnum<EExportFeedType>()).Value;
        
        public string Type { get; set; }

        public string Description { get; set; }

        public DateTime? LastExport { get; set; }

        public string LastExportFileFullName { get; set; }

        public ExportFeed()
        {
            
        }

        public ExportFeed(EExportFeedType feedType)
        {
            _type = feedType;
            Type = feedType.ToString();
        }
    }
}