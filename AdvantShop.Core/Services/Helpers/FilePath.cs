//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.FilePath
{
    public enum ProductImageType
    {
        [Localize("Core.FilePath.ProductViewMode.Big")]
        Big,
        [Localize("Core.FilePath.ProductViewMode.Middle")]
        Middle,
        [Localize("Core.FilePath.ProductViewMode.Small")]
        Small,
        [Localize("Core.FilePath.ProductViewMode.XSmall")]
        XSmall,
        [Localize("Core.FilePath.ProductViewMode.Original")]
        Original,
        [Localize("Core.FilePath.ProductViewMode.Rotate")]
        Rotate
    }

    public enum ReviewImageType
    {
        Big,
        Small,
    }

    public enum CategoryImageType
    {
        Big,
        Small,
        Icon,
        BigOriginal,
        SmallOriginal
    }

    public enum ColorImageType
    {
        Details,
        Catalog,
    }

    public enum MobileAppIcon
    {
        x48,
        x72,
        x96,
        x128,
        x144,
        x152,
        x192,
        x384,
        x512
    }

    public enum FolderType
    {
        Pictures,
        MenuIcons,
        Product,
        Color,
        Carousel,
        Category,
        News,
        StaticPage,
        BrandLogo,
        PaymentLogo,
        PresetPaymentLogo,
        ShippingLogo,
        PresetShippingLogo,
        PriceTemp,
        ImageTemp,
        OneCTemp,
        ManagerPhoto,
        ReviewImage,
        Combine,
        AdminContent,
        Avatar,
        TaskAttachment,
        LeadAttachment,
        UserFiles,
        BookingCategory,
        BookingService,
        SocialWidget,
        BookingAttachment,
        TemplateDocx,
        BookingReservationResource,
        PartnerPayoutReports,
        PartnerActReports,
        MobileAppIcon,
        LandingVideo,
        ApplicationTempData,
        CarouselApi,
        CustomOptions,
        ShippingZones,
        CityAddressPoints,
        StaticPageApi,
        CheckoutAttachment,
        OrderCustomerAttachment,
        OrderAdminAttachment
    }


    public enum CssType {
        extra,
        saas,
        extra_admin
    }

    public enum Fonts
    {
        Store = 0,
        Mobile = 1,
        Template = 2,
        Icons = 3

    }

    public class FoldersHelper
    {
        public static readonly Dictionary<FolderType, string> PhotoFoldersPath = new Dictionary<FolderType, string>
        {
            {FolderType.Pictures, "pictures/"},
            {FolderType.MenuIcons, "pictures/icons/"},
            {FolderType.Product, "pictures/product/"},
            {FolderType.Color, "pictures/color/"},
            {FolderType.Carousel, "pictures/carousel/"},
            {FolderType.News, "pictures/news/"},
            {FolderType.Category, "pictures/category/"},
            {FolderType.BrandLogo, "pictures/brand/"},
            {FolderType.ManagerPhoto, "pictures/manager/"},
            {FolderType.PaymentLogo, "pictures/payment/"},
            {FolderType.PresetPaymentLogo, "images/payment/"},
            {FolderType.ShippingLogo, "pictures/shipping/"},
            {FolderType.PresetShippingLogo, "images/shipping/"},
            {FolderType.StaticPage, "pictures/staticpage/"},
            {FolderType.ReviewImage, "pictures/review/"},
            {FolderType.PriceTemp, "content/price_temp/"},
            {FolderType.ImageTemp, "content/upload_images/"},
            {FolderType.OneCTemp, "content/1c_temp/"},
            {FolderType.Combine, "combine/"},
            {FolderType.AdminContent, "areas/admin/content/"},
            {FolderType.Avatar, "pictures/avatar/"},
            {FolderType.TaskAttachment, "content/attachments/tasks/"},
            {FolderType.LeadAttachment, "content/attachments/leads/"},
            {FolderType.UserFiles, "userfiles/" },
            {FolderType.BookingCategory, "pictures/booking-category/"},
            {FolderType.BookingService, "pictures/booking-service/"},
            {FolderType.SocialWidget, "pictures/socialwidget/"},
            {FolderType.BookingAttachment, "content/attachments/booking/"},
            {FolderType.TemplateDocx, "content/template_docx/"},
            {FolderType.BookingReservationResource, "pictures/booking-resource/"},
            {FolderType.PartnerPayoutReports, "content/partner_payout_reports/"},
            {FolderType.PartnerActReports, "content/partner_act-reports/"},
            {FolderType.MobileAppIcon, "pictures/mobileapp/"},
            {FolderType.LandingVideo, "userfiles/lp-video/"},
            {FolderType.ApplicationTempData, "app_tempdata/"},
            {FolderType.CarouselApi, "pictures/carouselapi/"},
            {FolderType.CustomOptions, "pictures/customoptions/"},
            {FolderType.ShippingZones, "pictures/shippingzones/"},
            {FolderType.CityAddressPoints, "pictures/cityaddresspoints/"},
            {FolderType.StaticPageApi, "pictures/staticpageapi/"},
            {FolderType.CheckoutAttachment, "content/attachments/checkoutimage/"},
            {FolderType.OrderCustomerAttachment, "content/attachments/ordercustomerimage/"},
            {FolderType.OrderAdminAttachment, "content/attachments/orderadminimage/"},
        };

        public static readonly Dictionary<CategoryImageType, string> CategoryPhotoPrefix = new Dictionary<CategoryImageType, string>
        {
            {CategoryImageType.Small, "small/"},
            {CategoryImageType.Big, ""},
            {CategoryImageType.Icon, "icon/"},
            {CategoryImageType.BigOriginal, "big_original/"},
            {CategoryImageType.SmallOriginal, "small_original/"},
        };

        public static readonly Dictionary<ReviewImageType, string> ReviewPhotoPrefix = new Dictionary<ReviewImageType, string>
        {
            {ReviewImageType.Small, ""},
            {ReviewImageType.Big, "big/"},
        };

        public static readonly Dictionary<ColorImageType, string> ColorPhotoPrefix = new Dictionary<ColorImageType, string>
        {
            {ColorImageType.Details, "details/"},
            {ColorImageType.Catalog, "catalog/"},
        };

        public static readonly Dictionary<ProductImageType, string> ProductPhotoPrefix = new Dictionary<ProductImageType, string>
        {
            {ProductImageType.XSmall, "xsmall/"},
            {ProductImageType.Small,  "small/"},
            {ProductImageType.Middle, "middle/"},
            {ProductImageType.Big,    "big/"},
            {ProductImageType.Original, "original/"},
            {ProductImageType.Rotate, "rotate/"}
        };

        public static readonly Dictionary<ProductImageType, string> ProductPhotoPostfix = new Dictionary<ProductImageType, string>
        {
            {ProductImageType.XSmall, "_xsmall"},
            {ProductImageType.Small,  "_small"},
            {ProductImageType.Middle, "_middle"},
            {ProductImageType.Big,    "_big"},
            {ProductImageType.Original, "_original"},
            {ProductImageType.Rotate, "_rotate"}

        };

        public static readonly Dictionary<MobileAppIcon, string> MobileAppIconPostfix = new Dictionary<MobileAppIcon, string>
        {
            {MobileAppIcon.x48, "_48x48"},
            {MobileAppIcon.x72, "_72x72"},
            {MobileAppIcon.x96, "_96x96"},
            {MobileAppIcon.x128, "_128x128"},
            {MobileAppIcon.x144, "_144x144"},
            {MobileAppIcon.x152, "_152x152"},
            {MobileAppIcon.x192, "_192x192"},
            {MobileAppIcon.x384, "_384x384"},
            {MobileAppIcon.x512, "_512x512"},
        };

        public static readonly Dictionary<Fonts, string> FontsFileNames = new Dictionary<Fonts, string>
        {
            {Fonts.Store, "fonts.store"},
            {Fonts.Mobile, "fonts.mobile"},
            {Fonts.Template, "fonts"},
            {Fonts.Icons, "fonts.icons"}
        };

        private static string GetPath(string imagePathBase, bool isForAdministration, string remoteUrl)
        {
            if (!string.IsNullOrWhiteSpace(remoteUrl))
            {
                return remoteUrl + imagePathBase;
            }

            if (HttpContext.Current == null)
                return (isForAdministration ? "../" : SettingsMain.SiteUrl.Trim('/') + '/') + imagePathBase;

            return UrlService.GetUrl() + imagePathBase;
        }

        private static string GetPathAbsolut(string imagePathBase)
        {
            return SettingsGeneral.AbsolutePath + imagePathBase;
        }

        //_____________
        public static string GetPath(FolderType type, string photoPath, bool isForAdministration)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPath(PhotoFoldersPath[type], isForAdministration, string.Empty);

            if (photoPath.Contains("://"))
            {
                return photoPath;
            }

            return GetPath(PhotoFoldersPath[type], isForAdministration, string.Empty) + photoPath;
        }

        public static string GetPathRelative(FolderType type, string photoPath, bool isForAdministration)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return (isForAdministration ? "../" : string.Empty) + PhotoFoldersPath[type];

            return (isForAdministration ? "../" : string.Empty) + PhotoFoldersPath[type] + photoPath;
        }

        public static string GetPathAbsolut(FolderType type, string photoPath = "")
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[type]);
            return GetPathAbsolut(PhotoFoldersPath[type]) + photoPath;
        }
        //_____________


        #region ProductImage

        public static string GetImageProductPath(ProductImageType type, string photoPath, bool isForAdministration, bool isRelative = false)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return (!isRelative ? UrlService.GetUrl() : "") + "images/nophoto" + ProductPhotoPostfix[type] + ".png";

            if (photoPath.Contains("://"))
            {
                if (photoPath.Contains("://cs71"))
                {
                    var fileName = Path.GetFileName(photoPath);
                    var path = photoPath.Replace(fileName, "");
                    return GetPath(PhotoFoldersPath[FolderType.Product], isForAdministration, path) + ProductPhotoPrefix[type] + fileName.Replace(".", ProductPhotoPostfix[type] + ".");
                }

                return photoPath;
            }

            var photoFolderPath = !isRelative
                ? GetPath(PhotoFoldersPath[FolderType.Product], isForAdministration, string.Empty)
                : PhotoFoldersPath[FolderType.Product];

            return photoFolderPath + ProductPhotoPrefix[type] + photoPath.Replace(".", ProductPhotoPostfix[type] + ".");
        }

        public static string GetImageProductPathRelative(ProductImageType type, string photoPath, bool isForAdministration)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return "images/nophoto" + ProductPhotoPostfix[type] + ".png";

            return (isForAdministration ? "../" : string.Empty) + PhotoFoldersPath[FolderType.Product] +
                   ProductPhotoPrefix[type] + photoPath.Replace(".", ProductPhotoPostfix[type] + ".");
        }

        public static string GetImageProductPathAbsolut(ProductImageType type, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.Product]) + ProductPhotoPrefix[type];
            return GetPathAbsolut(PhotoFoldersPath[FolderType.Product]) + ProductPhotoPrefix[type] + Path.GetFileNameWithoutExtension(photoPath) + ProductPhotoPostfix[type] + Path.GetExtension(photoPath);
        }
        
        public static string GetRotateImageProductPathAbsolut(string folderPath, string photoName = null)
        {
            if (string.IsNullOrWhiteSpace(photoName))
            {
                return GetImageProductPathAbsolut(ProductImageType.Rotate, string.Empty) + folderPath + "/";
            }
            return GetImageProductPathAbsolut(ProductImageType.Rotate, string.Empty) + folderPath + "/" + photoName;
        }
        #endregion


        #region CategoryImage
        public static string GetImageCategoryPathAbsolut(CategoryImageType type, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.Category]) + CategoryPhotoPrefix[type];

            return GetPathAbsolut(PhotoFoldersPath[FolderType.Category]) + CategoryPhotoPrefix[type] + photoPath;
        }

        public static string GetImageCategoryPath(CategoryImageType type, string photoPath, bool isForAdministration)
        {
            if (photoPath.Contains("://"))
            {
                if (photoPath.Contains("://cs71"))
                {
                    var fileName = Path.GetFileName(photoPath);
                    var path = photoPath.Replace(fileName, "");
                    return GetPath(PhotoFoldersPath[FolderType.Category], isForAdministration, path) + CategoryPhotoPrefix[type] + fileName;
                }
                return photoPath;
            }

            return GetPath(PhotoFoldersPath[FolderType.Category], isForAdministration, string.Empty) + CategoryPhotoPrefix[type] + photoPath;
        }
        #endregion

        #region ReviewImage
        public static string GetImageReviewPathAbsolut(ReviewImageType type, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.ReviewImage]) + ReviewPhotoPrefix[type];

            return GetPathAbsolut(PhotoFoldersPath[FolderType.ReviewImage]) + ReviewPhotoPrefix[type] + photoPath;
        }

        public static string GetImageReviewPath(ReviewImageType type, string photoPath, bool isForAdministration)
        {
            if (photoPath.Contains("://"))
            {
                var fileName = Path.GetFileName(photoPath);
                var path = photoPath.Replace(fileName, "");
                return GetPath(PhotoFoldersPath[FolderType.ReviewImage], isForAdministration, path) + ReviewPhotoPrefix[type] + fileName;
            }

            return GetPath(PhotoFoldersPath[FolderType.ReviewImage], isForAdministration, string.Empty) + ReviewPhotoPrefix[type] + photoPath;
        }
        #endregion

        public static string GetImageColorPathAbsolut(ColorImageType type, string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.Color]) + ColorPhotoPrefix[type];

            return GetPathAbsolut(PhotoFoldersPath[FolderType.Color]) + ColorPhotoPrefix[type] + photoPath;
        }

        public static string GetImageColorPath(ColorImageType type, string photoPath, bool isForAdministration)
        {
            if (photoPath.Contains("://"))
            {
                if (photoPath.Contains("://cs71"))
                {
                    var fileName = Path.GetFileName(photoPath);
                    var path = photoPath.Replace(fileName, "");
                    return GetPath(PhotoFoldersPath[FolderType.Color], isForAdministration, path) + ColorPhotoPrefix[type] + fileName;
                }

                return photoPath;
            }

            return GetPath(PhotoFoldersPath[FolderType.Color], isForAdministration, string.Empty) + ColorPhotoPrefix[type] + photoPath;
        }

        public static string GetMobileAppIconPath(MobileAppIcon type, string photoName, string photoPath = null)
        {
            if (string.IsNullOrWhiteSpace(photoName) && string.IsNullOrWhiteSpace(photoPath))
                return GetPath(PhotoFoldersPath[FolderType.MobileAppIcon], false, string.Empty);
            if (string.IsNullOrWhiteSpace(photoPath))
            {
                return GetPath(PhotoFoldersPath[FolderType.MobileAppIcon], false, string.Empty) + Path.GetFileNameWithoutExtension(photoName) + MobileAppIconPostfix[type] + Path.GetExtension(photoName);
            }
            else
            {
                return GetPath(photoPath, false, string.Empty) + Path.GetFileNameWithoutExtension(photoName) + MobileAppIconPostfix[type] + Path.GetExtension(photoName);
            }
        }

        public static string GetMobileAppIconPathAbsolut(MobileAppIcon type, string photoName, string photoPath = null)
        {
            if (string.IsNullOrWhiteSpace(photoName) && string.IsNullOrWhiteSpace(photoPath))
                return GetPathAbsolut(PhotoFoldersPath[FolderType.MobileAppIcon]);
            if (string.IsNullOrWhiteSpace(photoPath))
            {
                return GetPathAbsolut(PhotoFoldersPath[FolderType.MobileAppIcon]) + Path.GetFileNameWithoutExtension(photoName) + MobileAppIconPostfix[type] + Path.GetExtension(photoName);
            }
            else
            {
                return photoPath + Path.GetFileNameWithoutExtension(photoName) + MobileAppIconPostfix[type] + Path.GetExtension(photoName);
            }
        }

        public static string GetFontFileName(Fonts type)
        {
            return FontsFileNames[type];
        }

        public static void InitFolders()
        {
            foreach (var key in PhotoFoldersPath.Keys)
            {
                try
                {
                    var path = GetPathAbsolut(key);

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        public static void InitExtraCss()
        {
            try
            {
                var userfiles = GetPathAbsolut(FolderType.UserFiles);

                if (!Directory.Exists(userfiles))
                    Directory.CreateDirectory(userfiles);

                var extraCssPath = GetPathAbsolut(FolderType.UserFiles, CssType.extra + ".css");
                if (!File.Exists(extraCssPath))
                    FileHelpers.CreateFile(extraCssPath);

                var saasCssPath = GetPathAbsolut(FolderType.UserFiles, CssType.saas + ".css");
                if (!File.Exists(saasCssPath))
                    FileHelpers.CreateFile(saasCssPath);
                
                var extraAdminCssPath = GetPathAbsolut(FolderType.UserFiles, CssType.extra_admin + ".css");
                if (File.Exists(extraAdminCssPath))
                    FileHelpers.BackupFile(extraAdminCssPath);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private static readonly object SaveCssLockObj = new object();

        public static bool SaveCSS(string cssContent, CssType cssType)
        {
            lock (SaveCssLockObj)
            {
                try
                {
                    using (TextWriter writer = new StreamWriter(GetPathAbsolut(FolderType.UserFiles, cssType + ".css"), false))
                        writer.Write(cssContent);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                    return false;
                }
                return true;
            }
        }

        private static readonly object ReadCssLockObj = new object();

        public static string ReadCss(CssType cssType)
        {
            string cssContent = string.Empty;
            var path = GetPathAbsolut(FolderType.UserFiles, cssType + ".css");
            try
            {
                lock (ReadCssLockObj)
                {
                    if (!File.Exists(path))
                        using (File.Create(path)) {}//nothing here, just  create file
                }
                
                using (TextReader reader = new StreamReader(path))
                    cssContent = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return cssContent;
        }

    }
}