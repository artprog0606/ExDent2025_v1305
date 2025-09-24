using System;
using System.IO;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Files;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Seo
{
    public class SaveSeoSettings
    {
        private SEOSettingsModel _model;

        public SaveSeoSettings(SEOSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            SettingsSEO.BrandItemH1 = _model.BrandItemDefaultH1;
            SettingsSEO.BrandItemMetaTitle = _model.BrandItemDefaultTitle;
            SettingsSEO.BrandItemMetaKeywords = _model.BrandItemDefaultMetaKeywords;
            SettingsSEO.BrandItemMetaDescription = _model.BrandItemDefaultMetaDescription;

            SettingsSEO.BrandMetaTitle = _model.BrandsDefaultTitle;
            SettingsSEO.BrandMetaH1 = _model.BrandsDefaultH1;
            SettingsSEO.BrandMetaKeywords = _model.BrandsDefaultMetaKeywords;
            SettingsSEO.BrandMetaDescription = _model.BrandsDefaultMetaDescription;

            SettingsSEO.CategoryMetaH1 = _model.CategoriesDefaultH1;
            SettingsSEO.CategoryMetaTitle = _model.CategoriesDefaultTitle;
            SettingsSEO.CategoryMetaKeywords = _model.CategoriesDefaultMetaKeywords;
            SettingsSEO.CategoryMetaDescription = _model.CategoriesDefaultMetaDescription;

            SettingsSEO.TagsMetaH1 = _model.TagsDefaultH1;
            SettingsSEO.TagsMetaTitle = _model.TagsDefaultTitle;
            SettingsSEO.TagsMetaKeywords = _model.TagsDefaultMetaKeywords;
            SettingsSEO.TagsMetaDescription = _model.TagsDefaultMetaDescription;

            SettingsSEO.DefaultMetaTitle = _model.DefaultTitle;
            SettingsSEO.DefaultMetaKeywords = _model.DefaultMetaKeywords;
            SettingsSEO.DefaultMetaDescription = _model.DefaultMetaDescription;

            SettingsSEO.NewsMetaH1 = _model.NewsDefaultH1;
            SettingsSEO.NewsMetaTitle = _model.NewsDefaultTitle;
            SettingsSEO.NewsMetaKeywords = _model.NewsDefaultMetaKeywords;
            SettingsSEO.NewsMetaDescription = _model.NewsDefaultMetaDescription;
            
            SettingsSEO.PageWarehousesMetaH1 = _model.ShopsDefaultH1;
            SettingsSEO.PageWarehousesMetaTitle = _model.ShopsDefaultTitle;
            SettingsSEO.PageWarehousesMetaKeywords = _model.ShopsDefaultMetaKeywords;
            SettingsSEO.PageWarehousesMetaDescription = _model.ShopsDefaultMetaDescription;

            SettingsSEO.BonusProgramMetaH1 = _model.BonusProgramDefaultH1;
            SettingsSEO.BonusProgramMetaTitle = _model.BonusProgramDefaultTitle;
            SettingsSEO.BonusProgramMetaKeywords = _model.BonusProgramDefaultMetaKeywords;
            SettingsSEO.BonusProgramMetaDescription = _model.BonusProgramDefaultMetaDescription;

            SettingsSEO.ProductMetaH1 = _model.ProductsDefaultH1;
            SettingsSEO.ProductMetaTitle = _model.ProductsDefaultTitle;
            SettingsSEO.ProductMetaKeywords = _model.ProductsDefaultMetaKeywords;
            SettingsSEO.ProductMetaDescription = _model.ProductsDefaultMetaDescription;
            SettingsSEO.ProductAdditionalDescription = _model.ProductsDefaultAdditionalDescription;

            SettingsSEO.StaticPageMetaH1 = _model.StaticPageDefaultH1;
            SettingsSEO.StaticPageMetaTitle = _model.StaticPageDefaultTitle;
            SettingsSEO.StaticPageMetaKeywords = _model.StaticPageDefaultMetaKeywords;
            SettingsSEO.StaticPageMetaDescription = _model.StaticPageDefaultMetaDescription;

            SettingsNews.NewsMetaH1 = _model.CategoryNewsDefaultH1;
            SettingsNews.NewsMetaTitle = _model.CategoryNewsDefaultTitle;
            SettingsNews.NewsMetaKeywords = _model.CategoryNewsDefaultMetaKeywords;
            SettingsNews.NewsMetaDescription = _model.CategoryNewsDefaultMetaDescription;

            SettingsSEO.MainPageProductsMetaH1 = _model.MainPageProductsDefaultH1;
            SettingsSEO.MainPageProductsMetaTitle = _model.MainPageProductsDefaultTitle;
            SettingsSEO.MainPageProductsMetaKeywords = _model.MainPageProductsDefaultMetaKeywords;
            SettingsSEO.MainPageProductsMetaDescription = _model.MainPageProductsDefaultMetaDescription;

            SettingsSEO.OpenGraphEnabled = _model.OpenGraphEnabled;
            SettingsSEO.OpenGraphFbAdmins = _model.OpenGraphFbAdmins;
            SettingsMain.EnableCyrillicUrl = _model.EnableCyrillicUrl;

            SettingsSEO.GoogleAnalyticsNumber = _model.GoogleAnalyticsNumber;
            SettingsSEO.GoogleAnalyticsEnableDemogrReports = _model.GoogleAnalyticsEnableDemogrReports;
            SettingsSEO.GoogleAnalyticsSendOrderOnStatusChanged = _model.GoogleAnalyticsSendOrderOnStatusChanged;
            SettingsSEO.GoogleAnalyticsOrderStatusIdToSend = _model.GoogleAnalyticsOrderStatusIdToSend;
            SettingsSEO.GoogleAnalyticsEnabled = _model.GoogleAnalyticsEnabled;

            SettingsSEO.GoogleAnalyticsApiEnabled = _model.GoogleAnalyticsApiEnabled;
            SettingsSEO.GoogleAnalyticsAccountID = _model.GoogleAnalyticsAccountID;
            //SettingsOAuth.GoogleClientId = _model.GoogleClientId;
            //SettingsOAuth.GoogleClientSecret = _model.GoogleClientSecret;

            SettingsSEO.UseGTM = _model.UseGTM;
            SettingsSEO.GTMContainerID = _model.GTMContainerId;
            SettingsSEO.GTMOfferIdType = _model.GTMOfferIdType;

            //SettingsSEO.Enabled301Redirects = _model.EnableRedirect301;

            CacheManager.RemoveByPattern(CacheNames.GetCategoryCacheObjectPrefix());

            try
            {
                //var filePath = HttpContext.Current.Server.MapPath("~/robots.txt");
                //using (var streamWriter = new StreamWriter(filePath, false))
                //{
                //    streamWriter.Write(_model.RobotsText);
                //}
                if (FileInDbService.IsExist("/robots.txt"))
                    FileInDbService.Update("/robots.txt", Encoding.UTF8.GetBytes(_model.RobotsText ?? ""));
                else
                {
                    if (!new FileExtensionContentTypeHelper().TryGetContentType("robots.txt", out var contentType))
                        contentType = "text/plain";

                    FileInDbService.Add(new FileInDb()
                    {
                        Name = "robots.txt",
                        Path = "/robots.txt",
                        ContentType = contentType,
                        Content = Encoding.UTF8.GetBytes(_model.RobotsText ?? ""),
                        Charset = "utf-8"
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException("Ошибка при сохранении robots.txt. Возможно нет прав на создание/редактирование файла");
            }
        }
    }
}
