//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using RazorEngine.Compilation.ImpromptuInterface;

namespace AdvantShop.Catalog
{
    public enum PhotoType
    {
        None,
        Product,
        Product360,
        CategoryBig,
        CategorySmall,
        CategoryIcon,
        News,
        StaticPage,
        Brand,
        Carousel,
        MenuIcon,
        Shipping,
        Payment,
        Color,
        Manager,
        Review,
        Logo,
        DarkThemeLogo,
        Favicon,
        LogoMobile,
        MobileApp,
        LandingMobileApp,
        LogoSvg,
        FaviconSvg,
        LogoBlog,
        CarouselApi,
        ShippingZones,
        CityAddressPoints,
        CustomOptions
    }

    public class Photo
    {
        public int PhotoId { get; set; }

        public int ObjId { get; set; }

        public PhotoType Type { get; set; }

        public string PhotoName { get; set; }

        /// <summary>
        /// Small
        /// </summary>
        public string PhotoNameSize1 { get; set; }

        /// <summary>
        /// XSmall
        /// </summary>
        public string PhotoNameSize2 { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Description { get; set; }

        public int PhotoSortOrder { get; set; }

        public bool Main { get; set; }

        public string OriginName { get; set; }

        public int? ColorID { get; set; }

        public int? PhotoCategoryId { get; set; }

        public Photo(int photoId, int objId, PhotoType type)
        {
            PhotoId = photoId;
            ObjId = objId;
            Type = type;
        }
    }


    public class BrandPhoto : Photo
    {
        #region Constructor

        public BrandPhoto() : base(0, 0, PhotoType.Brand) { }

        public BrandPhoto(int photoId, int objId) : base(photoId, objId, PhotoType.Brand) { }

        #endregion

        public string ImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? UrlService.GetUrl() + "images/nophoto_xsmall.png"
                : FoldersHelper.GetPath(FolderType.BrandLogo, PhotoName, false);
        }
    }

    public class NewsPhoto : Photo
    {
        #region Constructor

        public NewsPhoto() : base(0, 0, PhotoType.News) { }

        public NewsPhoto(int photoId, int objId) : base(photoId, objId, PhotoType.News) { }

        #endregion

        public string ImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? ""
                : FoldersHelper.GetPath(FolderType.News, PhotoName, false);
        }
    }

    [Obsolete("Not used since 6.0")]
    public class ManagerPhoto : Photo
    {
        #region Constructor

        public ManagerPhoto() : base(0, 0, PhotoType.Manager) { }

        public ManagerPhoto(int photoId, int objId) : base(photoId, objId, PhotoType.Manager) { }

        #endregion

        public string ImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? ""
                : FoldersHelper.GetPath(FolderType.ManagerPhoto, PhotoName, false);
        }
    }


    public class CategoryPhoto : Photo
    {
        #region Constructor

        public CategoryPhoto() : base(0, 0, PhotoType.None) { }

        public CategoryPhoto(int objId, PhotoType type, string photoName) : base(0, objId, type)
        {
            PhotoName = photoName;
        }

        #endregion

        public string ImageSrcSmall()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? UrlService.GetUrl() + "images/nophoto_small.png"
                : FoldersHelper.GetImageCategoryPath(CategoryImageType.Small, PhotoName, false);
        }

        public string ImageSrcBig()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? UrlService.GetUrl() + "images/nophoto_big.png"
                : FoldersHelper.GetImageCategoryPath(CategoryImageType.Big, PhotoName, false);
        }

        public string IconSrc(bool noPhoto = false)
        {
            return string.IsNullOrEmpty(PhotoName)
                ? (noPhoto ? UrlService.GetUrl() + "images/nophoto_xsmall.png" : "")
                : FoldersHelper.GetImageCategoryPath(CategoryImageType.Icon, PhotoName, false);
        }
    }


    public class ProductPhoto : Photo
    {
        #region Constructor

        public ProductPhoto() : base(0, 0, PhotoType.None) { }

        public ProductPhoto(int objId, PhotoType type, string photoName)
            : base(0, objId, type)
        {
            PhotoName = photoName;
        }

        #endregion

        public string Title { get; set; }

        public string Alt { get; set; }

        public string ProductImageSrcBig => ImageSrcBig();

        public string ImageSrcXSmall()
        {
            return ImageSrc(ProductImageType.XSmall);
        }

        public string ImageSrcSmall()
        {
            return ImageSrc(ProductImageType.Small);
        }

        public string ImageSrcMiddle()
        {
            return ImageSrc(ProductImageType.Middle);
        }

        public string ImageSrcBig()
        {
            return ImageSrc(ProductImageType.Big);
        }

        public string ImageSrcRotate()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? UrlService.GetUrl() + "images/nophoto_small.png"
                : UrlService.GetUrl() + "pictures/product/rotate/" + ObjId + "/" + PhotoName;
        }

        public string ImageSrc(ProductImageType imageType)
        {
            var src = "";
            switch (imageType)
            {
                case ProductImageType.Original:
                case ProductImageType.Big:
                case ProductImageType.Rotate:
                    src = FoldersHelper.GetImageProductPath(imageType, PhotoName, false);
                    break;
                case ProductImageType.Middle:
                    src = FoldersHelper.GetImageProductPath(imageType, PhotoName, false);
                    break;
                case ProductImageType.Small:
                    src = FoldersHelper.GetImageProductPath(imageType, PhotoNameSize1.IsNotEmpty() ? PhotoNameSize1 : PhotoName, false);
                    break;
                case ProductImageType.XSmall:
                    src = FoldersHelper.GetImageProductPath(imageType, PhotoNameSize2.IsNotEmpty() ? PhotoNameSize2 : PhotoName, false);
                    break;
            }
            return src;
        }

        public string ImageSrcRelative(ProductImageType imageType)
        {
            return FoldersHelper.GetImageProductPath(imageType, PhotoName, false, true);
        }
    }

    public class CarouselPhoto : Photo
    {
        #region Constructor

        public CarouselPhoto() : base(0, 0, PhotoType.Carousel) { }

        public CarouselPhoto(int photoId, int objId) : base(photoId, objId, PhotoType.Carousel) { }

        #endregion

        public string ImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? UrlService.GetUrl() + "images/nophoto_carousel_" + SettingsMain.Language + ".jpg"
                : FoldersHelper.GetPath(FolderType.Carousel, PhotoName, false);
        }
    }
    
    public class CarouselApiPhoto : Photo
    {
        #region Constructor

        public CarouselApiPhoto() : base(0, 0, PhotoType.CarouselApi) { }

        public CarouselApiPhoto(int photoId, int objId) : base(photoId, objId, PhotoType.CarouselApi) { }

        #endregion

        public string ImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? UrlService.GetUrl() + "images/nophoto_carousel_" + SettingsMain.Language + ".jpg"
                : FoldersHelper.GetPath(FolderType.CarouselApi, PhotoName, false);
        }
    }

    public class ColorPhoto : Photo
    {
        #region Constructor

        public ColorPhoto() : base(0, 0, PhotoType.Color) { }

        public ColorPhoto(int photoId, int objId) : base(photoId, objId, PhotoType.Color) { }

        #endregion

        public int ImageWidthDetails
        {
            get { return SettingsPictureSize.ColorIconWidthDetails; }
        }

        public int ImageHeightDetails
        {
            get { return SettingsPictureSize.ColorIconHeightDetails; }
        }

        public int ImageWidthCatalog
        {
            get { return SettingsPictureSize.ColorIconWidthCatalog; }
        }

        public int ImageHeightCatalog
        {
            get { return SettingsPictureSize.ColorIconHeightCatalog; }
        }


        public string ImageSrc(ColorImageType type)
        {
            return string.IsNullOrEmpty(PhotoName)
                ? string.Empty
                : FoldersHelper.GetImageColorPath(type, PhotoName, false);
        }
    }

    public class ReviewPhoto : Photo
    {
        #region Constructor

        public ReviewPhoto() : base(0, 0, PhotoType.Review) { }

        public ReviewPhoto(int photoId, int objId) : base(photoId, objId, PhotoType.Review) { }

        #endregion

        public int ImageWidth
        {
            get { return SettingsPictureSize.ReviewImageWidth; }
        }

        public int ImageHeight
        {
            get { return SettingsPictureSize.ReviewImageHeight; }
        }

        public string ImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? string.Empty
                : FoldersHelper.GetImageReviewPath(ReviewImageType.Small, PhotoName, false);
        }

        public string BigImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? string.Empty
                : System.IO.File.Exists(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Big, PhotoName))
                    ? FoldersHelper.GetImageReviewPath(ReviewImageType.Big, PhotoName, false)
                    : FoldersHelper.GetImageReviewPath(ReviewImageType.Small, PhotoName, false);
        }
    }

    public class OptionItemPhoto : Photo
    {

        #region Constructor
        public OptionItemPhoto() : base (0,0, PhotoType.CustomOptions) { }

        public OptionItemPhoto(int photoId, int objId, PhotoType type) : base(photoId, objId, type) { }

        #endregion

        public string PictureUrl { get; set; }
        public bool NeedUpdate { get; set; }

        public string ImageSrc()
        {
            return string.IsNullOrEmpty(PhotoName)
                ? UrlService.GetUrl() + "images/nophoto_xsmall.png"
                : FoldersHelper.GetPath(FolderType.CustomOptions, PhotoName, true);
        }
    }
}