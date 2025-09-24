//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Statistic;

namespace AdvantShop.ExportImport
{
    public abstract class BaseExportFeed<TAdvancedSettings> : IExportFeed where TAdvancedSettings : IExportFeedOptions
    {
        protected readonly int _exportFeedId;
        protected readonly bool UseCommonStatistic;

        protected ExportFeedSettings<TAdvancedSettings> Settings;

        protected const string TempPrefix = ".temp";

        protected readonly List<ProductDiscount> ProductDiscountModels;

        protected BaseExportFeed(int exportFeedId)
        {
            _exportFeedId = exportFeedId;
            var discountModule = AttachedModules.GetModules<IDiscount>().FirstOrDefault();
            if (discountModule != null)
            {
                var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                ProductDiscountModels = classInstance.GetProductDiscountsList();
            }
            Settings = ExportFeedSettingsProvider.GetSettings<TAdvancedSettings>(exportFeedId);
        }

        protected BaseExportFeed(int exportFeedId, bool useCommonStatistic) : this(exportFeedId)
        {
            UseCommonStatistic = useCommonStatistic;
        }

        public virtual string Export()
        {
            try
            {
                var exportFile = new FileInfo(Settings.FileFullPath);

                if (!string.IsNullOrEmpty(exportFile.Directory?.FullName))
                    FileHelpers.CreateDirectory(exportFile.Directory.FullName);

                CsSetFileName("../" + Settings.FileFullName);
                CsSetFileUrl(Settings.FileUrl);
                CsSetZipFile(NeedZip());

                var extension = Settings.FileExtention;
                // write to temporary file
                Settings.FileExtention += TempPrefix;
                FileHelpers.DeleteFile(Settings.FileFullPath);

                ExportFeedService.FillExportFeedCategoriesCache(_exportFeedId, Settings.AdvancedSettings.ExportNotAvailable);
                Handle();
                ExportFeedService.ClearExportFeedCategoriesCache(_exportFeedId);

                var tempFilePath = Settings.FileFullPath;
                // set original file extension
                Settings.FileExtention = extension;
                FileHelpers.ReplaceFile(tempFilePath, Settings.FileFullPath);

                if (NeedZip())
                {
                    var tempZipPath = Settings.FileFullPath + ArchiveExt + TempPrefix;
                    FileHelpers.ZipFiles(Settings.FileFullPath, tempZipPath);
                    FileHelpers.ReplaceFile(tempZipPath, Settings.FileFullPath + ArchiveExt);
                }

                ExportFeedService.UpdateExportFeedLastExport(_exportFeedId, DateTime.Now, Settings.FileFullName);

                return Settings.FileFullName;
            }
            catch (Exception ex)
            {
                CsRowError(ex.Message);
                Debug.Log.Error(ex);
            }
            return null;
        }

        protected virtual void Handle() { }

        protected virtual string ArchiveExt => ".zip";
        protected virtual bool NeedZip() => false;

        public abstract object GetAdvancedSettings();

        public abstract void SetDefaultSettings();

        public abstract List<string> GetAvailableVariables();

        public abstract List<string> GetAvailableFileExtentions();

        public virtual string GetDownloadableExportFeedFileLink()
        {
            return SettingsMain.SiteUrl + "/" + Settings.FileFullName + "?rnd=" + (new Random()).Next();
        }

        protected string GetImageProductPath(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                photoPath = "";

            photoPath = photoPath.Trim();

            if (photoPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                photoPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return HttpUtility.HtmlEncode(photoPath);
            }

            return SettingsMain.SiteUrl + "/" + FoldersHelper.GetImageProductPathRelative(ProductImageType.Big, photoPath, false);
        }

        protected string GetAdditionalUrlTags(ExportProductModel row)
        {
            var urlTags = Settings.AdditionalUrlTags;
            if (string.IsNullOrEmpty(urlTags))
                return string.Empty;

            if (urlTags.IndexOf('#') >= 0)
            {
                urlTags = urlTags.Replace("#STORE_NAME#", HttpUtility.UrlEncode(SettingsMain.ShopName));
                urlTags = urlTags.Replace("#STORE_URL#", HttpUtility.UrlEncode(SettingsMain.SiteUrl));
                urlTags = urlTags.Replace("#PRODUCT_NAME#", HttpUtility.UrlEncode(row.Name));
                urlTags = urlTags.Replace("#PRODUCT_ID#", row.ProductId.ToString());
                urlTags = urlTags.Replace("#OFFER_ID#", row.OfferId.ToString());
                urlTags = urlTags.Replace("#PRODUCT_ARTNO#", HttpUtility.UrlEncode(row.ArtNo));
            }
            return urlTags;
        }
        
        protected Currency _markupCurrency;
        protected Currency MarkupCurrency => _markupCurrency ??
                                             (_markupCurrency =
                                                 CurrencyService.BaseCurrency ??
                                                 CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3));
        
        /// <summary>
        /// Получить наценку
        /// </summary>
        protected float GetMarkup(float price, float renderCurrencyRate)
        {
            return price * Settings.PriceMarginInPercents / 100 +
                   (Settings.PriceMarginInNumbers > 0 
                       ? Settings.PriceMarginInNumbers / renderCurrencyRate * MarkupCurrency.Rate
                       : 0);
        }

        protected decimal GetPriceWithDiscount(float price, Discount priceDiscount, Currency renderingCurrency, float baseCurrencyValue)
        {
            var discountPrice = priceDiscount.Type == DiscountType.Percent
                ? price * priceDiscount.Percent / 100
                : priceDiscount.Amount;

            var resultPrice =
                PriceService.RoundPrice(price, renderingCurrency, baseCurrencyValue) -
                PriceService.RoundPrice(discountPrice, renderingCurrency, baseCurrencyValue);

            var markup = GetMarkup(resultPrice, renderingCurrency.Rate);

            var newPrice = (decimal) PriceService.SimpleRoundPrice(resultPrice + markup, renderingCurrency);

            return newPrice;
        }

        #region CommonStatistic

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (UseCommonStatistic)
                commonStatisticAction();
        }

        protected void CsSetFileName(string fileName)
        {
            DoForCommonStatistic(() => CommonStatistic.FileName = fileName);
        }
        
        protected void CsSetFileUrl(string fileUrl)
        {
            DoForCommonStatistic(() => CommonStatistic.FileUrl = fileUrl);
        }

        protected void CsSetTotalRow(int count)
        {
            DoForCommonStatistic(() => CommonStatistic.TotalRow = count);
        }

        protected void CsSetZipFile(bool zipFile)
        {
            DoForCommonStatistic(() => CommonStatistic.ZipFile = zipFile);
        }

        protected void CsNextRow()
        {
            DoForCommonStatistic(() => CommonStatistic.RowPosition++);
        }

        protected void CsRowError()
        {
            DoForCommonStatistic(() => CommonStatistic.TotalErrorRow++);
        }

        protected void CsRowError(string message)
        {
            DoForCommonStatistic(() =>
            {
                CommonStatistic.WriteLog(message);
                CommonStatistic.TotalErrorRow++;
            });

        }

        #endregion
    }
}