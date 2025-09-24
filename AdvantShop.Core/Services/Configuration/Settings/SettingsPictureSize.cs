//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public class SettingsPictureSize
    {
        #region Brand

        public static int BrandLogoWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["BrandLogoWidth"]);
            set => TemplateSettingsProvider.Items["BrandLogoWidth"] = value.ToString();
        }

        public static int BrandLogoHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["BrandLogoHeight"]);
            set => TemplateSettingsProvider.Items["BrandLogoHeight"] = value.ToString();
        }

        #endregion

        #region Product

        public static int BigProductImageWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["BigProductImageWidth"]);
            set => TemplateSettingsProvider.Items["BigProductImageWidth"] = value.ToString();
        }

        public static int BigProductImageHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["BigProductImageHeight"]);
            set => TemplateSettingsProvider.Items["BigProductImageHeight"] = value.ToString();
        }

        public static int MiddleProductImageWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["MiddleProductImageWidth"]);
            set => TemplateSettingsProvider.Items["MiddleProductImageWidth"] = value.ToString();
        }

        public static int MiddleProductImageHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["MiddleProductImageHeight"]);
            set => TemplateSettingsProvider.Items["MiddleProductImageHeight"] = value.ToString();
        }

        public static int SmallProductImageWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["SmallProductImageWidth"]);
            set => TemplateSettingsProvider.Items["SmallProductImageWidth"] = value.ToString();
        }

        public static int SmallProductImageHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["SmallProductImageHeight"]);
            set => TemplateSettingsProvider.Items["SmallProductImageHeight"] = value.ToString();
        }

        public static int XSmallProductImageWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["XSmallProductImageWidth"]);
            set => TemplateSettingsProvider.Items["XSmallProductImageWidth"] = value.ToString();
        }

        public static int XSmallProductImageHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["XSmallProductImageHeight"]);
            set => TemplateSettingsProvider.Items["XSmallProductImageHeight"] = value.ToString();
        }

        #endregion

        #region Category

        public static int BigCategoryImageWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["BigCategoryImageWidth"]);
            set => TemplateSettingsProvider.Items["BigCategoryImageWidth"] = value.ToString();
        }

        public static int BigCategoryImageHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["BigCategoryImageHeight"]);
            set => TemplateSettingsProvider.Items["BigCategoryImageHeight"] = value.ToString();
        }

        public static int SmallCategoryImageWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["SmallCategoryImageWidth"]);
            set => TemplateSettingsProvider.Items["SmallCategoryImageWidth"] = value.ToString();
        }

        public static int SmallCategoryImageHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["SmallCategoryImageHeight"]);
            set => TemplateSettingsProvider.Items["SmallCategoryImageHeight"] = value.ToString();
        }


        public static int IconCategoryImageWidth
        {
            get => string.IsNullOrEmpty(TemplateSettingsProvider.Items["CategoryMenuIconWidth"])  ? 30 : SQLDataHelper.GetInt(TemplateSettingsProvider.Items["CategoryMenuIconWidth"]);
            set => TemplateSettingsProvider.Items["CategoryMenuIconWidth"] = value.ToString();
        }

        public static int IconCategoryImageHeight
        {
            get => string.IsNullOrEmpty(TemplateSettingsProvider.Items["CategoryMenuIconHeight"]) ? 30 : SQLDataHelper.GetInt(TemplateSettingsProvider.Items["CategoryMenuIconHeight"]);
            set => TemplateSettingsProvider.Items["CategoryMenuIconHeight"] = value.ToString();
        }


        #endregion

        #region News

        public static int NewsImageWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["NewsImageWidth"]);
            set => TemplateSettingsProvider.Items["NewsImageWidth"] = value.ToString();
        }

        public static int NewsImageHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["NewsImageHeight"]);
            set => TemplateSettingsProvider.Items["NewsImageHeight"] = value.ToString();
        }

        #endregion

        #region Carousel

        public static int CarouselBigWidth => 1920;

        public static int CarouselBigHeight => 1000;

        #endregion
        
        #region Payment Icons

        public static int PaymentIconWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["PaymentIconWidth"]);
            set => TemplateSettingsProvider.Items["PaymentIconWidth"] = value.ToString();
        }

        public static int PaymentIconHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["PaymentIconHeight"]);
            set => TemplateSettingsProvider.Items["PaymentIconHeight"] = value.ToString();
        }

        #endregion

        #region Shipping Icons

        public static int ShippingIconWidth
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["ShippingIconWidth"]);
            set => TemplateSettingsProvider.Items["ShippingIconWidth"] = value.ToString();
        }

        public static int ShippingIconHeight
        {
            get => SQLDataHelper.GetInt(TemplateSettingsProvider.Items["ShippingIconHeight"]);
            set => TemplateSettingsProvider.Items["ShippingIconHeight"] = value.ToString();
        }

        #endregion


        #region Color Icons

        public static int ColorIconWidthCatalog
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["ColorIconWidthCatalog"]);
            set => SettingProvider.Items["ColorIconWidthCatalog"] = value.ToString();
        }

        public static int ColorIconHeightCatalog
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["ColorIconHeightCatalog"]);
            set => SettingProvider.Items["ColorIconHeightCatalog"] = value.ToString();
        }

        public static int ColorIconWidthDetails
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["ColorIconWidthDetails"]);
            set => SettingProvider.Items["ColorIconWidthDetails"] = value.ToString();
        }

        public static int ColorIconHeightDetails
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["ColorIconHeightDetails"]);
            set => SettingProvider.Items["ColorIconHeightDetails"] = value.ToString();
        }

        #endregion

        #region Manager

        public static int ManagerFotoWidth => 110;

        public static int ManagerFotoHeight => 140;

        #endregion

        #region Review Image

        public static int ReviewImageWidth
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["ReviewImageWidth"]);
            set => SettingProvider.Items["ReviewImageWidth"] = value.ToString();
        }

        public static int ReviewImageHeight
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["ReviewImageHeight"]);
            set => SettingProvider.Items["ReviewImageHeight"] = value.ToString();
        }

        #endregion

        #region Booking

        public static int BookingCategoryImageWidth
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["BookingCategoryImageWidth"]);
            set => SettingProvider.Items["BookingCategoryImageWidth"] = value.ToString();
        }

        public static int BookingCategoryImageHeight
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["BookingCategoryImageHeight"]);
            set => SettingProvider.Items["BookingCategoryImageHeight"] = value.ToString();
        }

        public static int BookingServiceImageWidth
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["BookingServiceImageWidth"]);
            set => SettingProvider.Items["BookingServiceImageWidth"] = value.ToString();
        }

        public static int BookingServiceImageHeight
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["BookingServiceImageHeight"]);
            set => SettingProvider.Items["BookingServiceImageHeight"] = value.ToString();
        }

        public static int BookingReservationResourceImageWidth
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["BookingReservationResourceImageWidth"]);
            set => SettingProvider.Items["BookingReservationResourceImageWidth"] = value.ToString();
        }

        public static int BookingReservationResourceImageHeight
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["BookingReservationResourceImageHeight"]);
            set => SettingProvider.Items["BookingReservationResourceImageHeight"] = value.ToString();
        }


        #endregion

        #region CustomOptions

        public static int CustomOptionsImageWidth
        {
            get => string.IsNullOrEmpty(TemplateSettingsProvider.Items["CustomOptionsImageWidth"]) ? 100 : SQLDataHelper.GetInt(TemplateSettingsProvider.Items["CustomOptionsImageWidth"]);
            set => TemplateSettingsProvider.Items["CustomOptionsImageWidth"] = value.ToString();
        }

        public static int CustomOptionsImageHeight
        {
            get => string.IsNullOrEmpty(TemplateSettingsProvider.Items["CustomOptionsImageHeight"]) ? 100 : SQLDataHelper.GetInt(TemplateSettingsProvider.Items["CustomOptionsImageHeight"]);
            set => TemplateSettingsProvider.Items["CustomOptionsImageHeight"] = value.ToString();
        }

        #endregion
    }
}