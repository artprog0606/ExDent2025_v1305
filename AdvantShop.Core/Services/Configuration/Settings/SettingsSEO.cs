//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.SEO;

namespace AdvantShop.Configuration
{
    public class SettingsSEO
    {
        #region GoogleAnalitics
        public static string GoogleAnalyticsNumber
        {
            get => SettingProvider.Items["GoogleAnalyticsNumber"];
            set => SettingProvider.Items["GoogleAnalyticsNumber"] = value;
        }

        public static bool GoogleAnalyticsEnabled
        {
            get => Convert.ToBoolean(SettingProvider.Items["GoogleAnalyticsEnabled"]);
            set => SettingProvider.Items["GoogleAnalyticsEnabled"] = value.ToString();
        }

        public static bool GoogleAnalyticsApiEnabled
        {
            get => Convert.ToBoolean(SettingProvider.Items["GoogleAnalyticsApiEnabled"]);
            set => SettingProvider.Items["GoogleAnalyticsApiEnabled"] = value.ToString();
        }

        public static bool GoogleAnalyticsEnableDemogrReports
        {
            get => Convert.ToBoolean(SettingProvider.Items["GoogleAnalyticsEnableDemogrReports"]);
            set => SettingProvider.Items["GoogleAnalyticsEnableDemogrReports"] = value.ToString();
        }

        public static bool GoogleAnalyticsSendOrderOnStatusChanged
        {
            get => Convert.ToBoolean(SettingProvider.Items["GoogleAnalyticsSendOrderOnStatusChanged"]);
            set => SettingProvider.Items["GoogleAnalyticsSendOrderOnStatusChanged"] = value.ToString();
        }

        public static int GoogleAnalyticsOrderStatusIdToSend
        {
            get => Convert.ToInt32(SettingProvider.Items["GoogleAnalyticsOrderStatusIdToSend"]);
            set => SettingProvider.Items["GoogleAnalyticsOrderStatusIdToSend"] = value.ToString();
        }

        //[Obsolete]
        //public static string GoogleAnalyticsClientID
        //{
        //    get { return SettingProvider.Items["GoogleAnalyticsClientID"]; }
        //    set { SettingProvider.Items["GoogleAnalyticsClientID"] = value; }
        //}

        //[Obsolete]
        //public static string GoogleAnalyticsClientSecret
        //{
        //    get { return SettingProvider.Items["GoogleAnalyticsClientSecret"]; }
        //    set { SettingProvider.Items["GoogleAnalyticsClientSecret"] = value; }
        //}

        public static string GoogleAnalyticsAccountID
        {
            get => SettingProvider.Items["GoogleAnalyticsAccountID"];
            set => SettingProvider.Items["GoogleAnalyticsAccountID"] = value;
        }

        //public static string GoogleAnalyticsUserName
        //{
        //    get { return SettingProvider.Items["GoogleAnalyticsUserName"]; }
        //    set { SettingProvider.Items["GoogleAnalyticsUserName"] = value; }
        //}

        //public static string GoogleAnalyticsPassword
        //{
        //    get { return SettingProvider.Items["GoogleAnalyticsPassword"]; }
        //    set { SettingProvider.Items["GoogleAnalyticsPassword"] = value; }
        //}

        //public static string GoogleAnalyticsAPIKey
        //{
        //    get { return SettingProvider.Items["GoogleAnalyticsAPIKey"]; }
        //    set { SettingProvider.Items["GoogleAnalyticsAPIKey"] = value; }
        //}

        public static string GoogleAnalyticsCachedData
        {
            get => SettingProvider.Items["GoogleAnalyticsCachedData"];
            set => SettingProvider.Items["GoogleAnalyticsCachedData"] = value;
        }

        public static DateTime GoogleAnalyticsCachedDate
        {
            get
            {
                DateTime date;
                DateTime.TryParse(SettingProvider.Items["GoogleAnalyticsCachedDate"], CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                return date;
            }
            set => SettingProvider.Items["GoogleAnalyticsCachedDate"] = value.ToString(CultureInfo.InvariantCulture);
        }


        public static bool UseGTM
        {
            get => Convert.ToBoolean(SettingProvider.Items["UseGTM"]);
            set => SettingProvider.Items["UseGTM"] = value.ToString();
        }


        public static string GTMContainerID
        {
            get => SettingProvider.Items["GTMContainerID"];
            set => SettingProvider.Items["GTMContainerID"] = !string.IsNullOrEmpty(value) ? value.Replace("GTM-", "") : value;
        }
        
        public static string GTMOfferIdType
        {
            get => SettingProvider.Items["GTMOfferIdType"];
            set => SettingProvider.Items["GTMOfferIdType"] = value;
        }

        #endregion

        public static bool Enabled301Redirects
        {
            get => Convert.ToBoolean(SettingProvider.Items["Enabled301Redirects"]);
            set => SettingProvider.Items["Enabled301Redirects"] = value.ToString();
        }

        public static string DefaultMetaTitle
        {
            get => SettingProvider.Items[MetaType.Default + "Title"];
            set => SettingProvider.Items[MetaType.Default + "Title"] = value;
        }

        public static string DefaultMetaKeywords
        {
            get => SettingProvider.Items[MetaType.Default + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.Default + "MetaKeywords"] = value;
        }
        public static string DefaultMetaDescription
        {
            get => SettingProvider.Items[MetaType.Default + "MetaDescription"];
            set => SettingProvider.Items[MetaType.Default + "MetaDescription"] = value;
        }

        public static string DefaultH1
        {
            get => SettingProvider.Items[MetaType.Default + "H1"];
            set => SettingProvider.Items[MetaType.Default + "H1"] = value;
        }

        public static string BrandMetaTitle
        {
            get => SettingProvider.Items[MetaType.Default + "BrandTitle"];
            set => SettingProvider.Items[MetaType.Default + "BrandTitle"] = value;
        }
        public static string BrandMetaH1
        {
            get => SettingProvider.Items[MetaType.Default + "BrandH1"];
            set => SettingProvider.Items[MetaType.Default + "BrandH1"] = value;
        }
        public static string BrandMetaKeywords
        {
            get => SettingProvider.Items[MetaType.Default + "BrandMetaKeywords"];
            set => SettingProvider.Items[MetaType.Default + "BrandMetaKeywords"] = value;
        }
        public static string BrandMetaDescription
        {
            get => SettingProvider.Items[MetaType.Default + "BrandMetaDescription"];
            set => SettingProvider.Items[MetaType.Default + "BrandMetaDescription"] = value;
        }

        public static string BrandItemMetaTitle
        {
            get => SettingProvider.Items[MetaType.Brand + "Title"];
            set => SettingProvider.Items[MetaType.Brand + "Title"] = value;
        }
        public static string BrandItemMetaKeywords
        {
            get => SettingProvider.Items[MetaType.Brand + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.Brand + "MetaKeywords"] = value;
        }
        public static string BrandItemMetaDescription
        {
            get => SettingProvider.Items[MetaType.Brand + "MetaDescription"];
            set => SettingProvider.Items[MetaType.Brand + "MetaDescription"] = value;
        }
        public static string BrandItemH1
        {
            get => SettingProvider.Items[MetaType.Brand + "H1"];
            set => SettingProvider.Items[MetaType.Brand + "H1"] = value;
        }

        public static string ProductMetaTitle
        {
            get => SettingProvider.Items[MetaType.Product + "Title"];
            set => SettingProvider.Items[MetaType.Product + "Title"] = value;
        }
        public static string ProductMetaKeywords
        {
            get => SettingProvider.Items[MetaType.Product + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.Product + "MetaKeywords"] = value;
        }
        public static string ProductMetaDescription
        {
            get => SettingProvider.Items[MetaType.Product + "MetaDescription"];
            set => SettingProvider.Items[MetaType.Product + "MetaDescription"] = value;
        }
        public static string ProductMetaH1
        {
            get => SettingProvider.Items[MetaType.Product + "H1"];
            set => SettingProvider.Items[MetaType.Product + "H1"] = value;
        }
        public static string ProductAdditionalDescription
        {
            get => SettingProvider.Items[MetaType.Product + "AdditionalDescription"];
            set => SettingProvider.Items[MetaType.Product + "AdditionalDescription"] = value;
        }

        public static string CategoryMetaTitle
        {
            get => SettingProvider.Items[MetaType.Category + "Title"];
            set => SettingProvider.Items[MetaType.Category + "Title"] = value;
        }
        public static string CategoryMetaKeywords
        {
            get => SettingProvider.Items[MetaType.Category + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.Category + "MetaKeywords"] = value;
        }
        public static string CategoryMetaDescription
        {
            get => SettingProvider.Items[MetaType.Category + "MetaDescription"];
            set => SettingProvider.Items[MetaType.Category + "MetaDescription"] = value;
        }
        public static string CategoryMetaH1
        {
            get => SettingProvider.Items[MetaType.Category + "H1"];
            set => SettingProvider.Items[MetaType.Category + "H1"] = value;
        }

        public static string StaticPageMetaTitle
        {
            get => SettingProvider.Items[MetaType.StaticPage + "Title"];
            set => SettingProvider.Items[MetaType.StaticPage + "Title"] = value;
        }
        public static string StaticPageMetaKeywords
        {
            get => SettingProvider.Items[MetaType.StaticPage + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.StaticPage + "MetaKeywords"] = value;
        }
        public static string StaticPageMetaDescription
        {
            get => SettingProvider.Items[MetaType.StaticPage + "MetaDescription"];
            set => SettingProvider.Items[MetaType.StaticPage + "MetaDescription"] = value;
        }
        public static string StaticPageMetaH1
        {
            get => SettingProvider.Items[MetaType.StaticPage + "H1"];
            set => SettingProvider.Items[MetaType.StaticPage + "H1"] = value;
        }

        public static string TagsMetaTitle
        {
            get => SettingProvider.Items[MetaType.Tag + "Title"];
            set => SettingProvider.Items[MetaType.Tag + "Title"] = value;
        }
        public static string TagsMetaKeywords
        {
            get => SettingProvider.Items[MetaType.Tag + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.Tag + "MetaKeywords"] = value;
        }
        public static string TagsMetaDescription
        {
            get => SettingProvider.Items[MetaType.Tag + "MetaDescription"];
            set => SettingProvider.Items[MetaType.Tag + "MetaDescription"] = value;
        }
        public static string TagsMetaH1
        {
            get => SettingProvider.Items[MetaType.Tag + "H1"];
            set => SettingProvider.Items[MetaType.Tag + "H1"] = value;
        }

        public static string NewsMetaTitle
        {
            get => SettingProvider.Items[MetaType.News + "Title"];
            set => SettingProvider.Items[MetaType.News + "Title"] = value;
        }
        public static string NewsMetaKeywords
        {
            get => SettingProvider.Items[MetaType.News + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.News + "MetaKeywords"] = value;
        }
        public static string NewsMetaDescription
        {
            get => SettingProvider.Items[MetaType.News + "MetaDescription"];
            set => SettingProvider.Items[MetaType.News + "MetaDescription"] = value;
        }
        public static string NewsMetaH1
        {
            get => SettingProvider.Items[MetaType.News + "H1"];
            set => SettingProvider.Items[MetaType.News + "H1"] = value;
        }

        public static string MainPageProductsMetaTitle
        {
            get => SettingProvider.Items[MetaType.MainPageProducts + "Title"];
            set => SettingProvider.Items[MetaType.MainPageProducts + "Title"] = value;
        }
        public static string MainPageProductsMetaKeywords
        {
            get => SettingProvider.Items[MetaType.MainPageProducts + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.MainPageProducts + "MetaKeywords"] = value;
        }
        public static string MainPageProductsMetaDescription
        {
            get => SettingProvider.Items[MetaType.MainPageProducts + "MetaDescription"];
            set => SettingProvider.Items[MetaType.MainPageProducts + "MetaDescription"] = value;
        }
        public static string MainPageProductsMetaH1
        {
            get => SettingProvider.Items[MetaType.MainPageProducts + "H1"];
            set => SettingProvider.Items[MetaType.MainPageProducts + "H1"] = value;
        }
        
        public static string PageWarehousesMetaTitle
        {
            get => SettingProvider.Items[MetaType.PageWarehouses + "Title"];
            set => SettingProvider.Items[MetaType.PageWarehouses + "Title"] = value;
        }
        public static string PageWarehousesMetaKeywords
        {
            get => SettingProvider.Items[MetaType.PageWarehouses + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.PageWarehouses + "MetaKeywords"] = value;
        }
        public static string PageWarehousesMetaDescription
        {
            get => SettingProvider.Items[MetaType.PageWarehouses + "MetaDescription"];
            set => SettingProvider.Items[MetaType.PageWarehouses + "MetaDescription"] = value;
        }
        public static string PageWarehousesMetaH1
        {
            get => SettingProvider.Items[MetaType.PageWarehouses + "H1"];
            set => SettingProvider.Items[MetaType.PageWarehouses + "H1"] = value;
        }
        
        public static string BonusProgramMetaTitle
        {
            get => SettingProvider.Items[MetaType.BonusProgram + "Title"];
            set => SettingProvider.Items[MetaType.BonusProgram + "Title"] = value;
        }
        public static string BonusProgramMetaKeywords
        {
            get => SettingProvider.Items[MetaType.BonusProgram + "MetaKeywords"];
            set => SettingProvider.Items[MetaType.BonusProgram + "MetaKeywords"] = value;
        }
        public static string BonusProgramMetaDescription
        {
            get => SettingProvider.Items[MetaType.BonusProgram + "MetaDescription"];
            set => SettingProvider.Items[MetaType.BonusProgram + "MetaDescription"] = value;
        }
        public static string BonusProgramMetaH1
        {
            get => SettingProvider.Items[MetaType.BonusProgram + "H1"];
            set => SettingProvider.Items[MetaType.BonusProgram + "H1"] = value;
        }
        
        public static string GetDefaultTitle(MetaType type)
        {
            return SettingProvider.Items[type.ToString() + "Title"];
        }
        public static void SetDefaultTitle(MetaType type, string value)
        {
            SettingProvider.Items[type.ToString() + "Title"] = value;
        }

        public static string GetDefaultMetaDescription(MetaType metaType)
        {
            return SettingProvider.Items[metaType.ToString() + "MetaDescription"];
        }
        public static void SetDefaultMetaDescription(MetaType metaType, string value)
        {
            SettingProvider.Items[metaType.ToString() + "MetaDescription"] = value;
        }

        public static string GetDefaultMetaKeywords(MetaType metaType)
        {
            return SettingProvider.Items[metaType.ToString() + "MetaKeywords"];
        }

        public static void SetDefaultMetaKeywords(MetaType metaType, string value)
        {
            SettingProvider.Items[metaType.ToString() + "MetaKeywords"] = value;
        }

        public static string GetDefaultH1(MetaType metaType)
        {
            return SettingProvider.Items[metaType.ToString() + "H1"];
        }
        public static void SetDefaultH1(MetaType metaType, string value)
        {
            SettingProvider.Items[metaType.ToString() + "H1"] = value;
        }

        public static string CustomMetaString
        {
            get => SettingProvider.Items["CustomMetaString"];
            set => SettingProvider.Items["CustomMetaString"] = value;
        }

        public static bool OpenGraphEnabled
        {
            get => SettingProvider.Items["SettingsSEO.OpenGraphEnabled"].TryParseBool();
            set => SettingProvider.Items["SettingsSEO.OpenGraphEnabled"] = value.ToString();
        }

        public static string OpenGraphFbAdmins
        {
            get => SettingProvider.Items["SettingsSEO.OpenGraphFbAdmins"];
            set => SettingProvider.Items["SettingsSEO.OpenGraphFbAdmins"] = value;
        }
    }
}
