using System;
using System.Security.Cryptography;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;

namespace AdvantShop.ExportImport
{
    public class ExportFeedSettings : IExportFeedSettings
    {
        public ExportFeedSettings() {}

        public ExportFeedSettings(ExportFeedSettings exportFeedSettings) : this()
        {
            this.FileName = exportFeedSettings.FileName;
            this.FileExtention = exportFeedSettings.FileExtention;
            this.PriceMarginInPercents = exportFeedSettings.PriceMarginInPercents;
            this.PriceMarginInNumbers = exportFeedSettings.PriceMarginInNumbers;
            this.AdditionalUrlTags = exportFeedSettings.AdditionalUrlTags;
            this.Active = exportFeedSettings.Active;
            this.IntervalType = exportFeedSettings.IntervalType;
            this.Interval = exportFeedSettings.Interval;
            this.JobStartTime = exportFeedSettings.JobStartTime;
            this.ExportAllProducts = exportFeedSettings.ExportAllProducts;
            this.ExportAdult = exportFeedSettings.ExportAdult;
        }
        
        public int ExportFeedId { get; set; }
        public string FileName { get; set; }
        public string FileExtention { get; set; }
        public float PriceMarginInPercents { get; set; }
        public float PriceMarginInNumbers { get; set; }
        public string AdditionalUrlTags { get; set; }
        public bool Active { get; set; }
        public TimeIntervalType IntervalType { get; set; }
        public int Interval { get; set; }
        public DateTime JobStartTime { get; set; }
        public bool ExportAllProducts { get; set; }

        private string _fileFullName;
        public string FileFullName
        {
            get
            {
                if (_fileFullName.IsNullOrEmpty())
                {
                    _fileFullName = FileName
                        .Replace("#DATE#", DateTime.Now.ToString("yyyyMMddHHmmss"))
                        .Replace("#SALT#", BitConverter.ToString(new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(SettingsLic.LicKey ?? string.Empty)), 0, 10));
                }
                return _fileFullName + "." + FileExtention;
            }
        }
        public string FileFullPath { get { return SettingsGeneral.AbsolutePath + FileFullName; } }
        public string FileUrl => SettingsMain.SiteUrl + "/" + FileFullName;
        public bool ExportAdult { get; set; }    
    }
    
    public class ExportFeedSettings<TAdvancedSettings> : ExportFeedSettings
    {
        public ExportFeedSettings() : base(){}

        public ExportFeedSettings(ExportFeedSettings exportFeedSettings) : base(exportFeedSettings){}
        public ExportFeedSettings(ExportFeedSettings<TAdvancedSettings> exportFeedSettings) : base(exportFeedSettings)
        {
            AdvancedSettings = exportFeedSettings.AdvancedSettings;
        }
        public TAdvancedSettings AdvancedSettings { get; set; }
    }

    public enum PriceMarginType
    {
        Percent = 0,
        Fixed = 1
    }
}