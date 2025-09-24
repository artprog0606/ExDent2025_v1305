using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace AdvantShop.Core.Services.Landing.Pictures
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ePictureLoaderImageType
    {
        [EnumMember(Value = "image")]
        Image = 0,
        [EnumMember(Value = "svg")]
        Svg = 1
    }

    public enum ePictureLoaderLazyLoadType
    {
        [StringName("src")]
        None = 0,
        [StringName("src")]
        Default = 1,
        [StringName("data-lazy")]
        Carousel = 2,
        [StringName("ng-src")]
        Angular = 3
    }

    public enum ePictureLoaderReplacementMode
    {
        [StringName("default")]
        Default = 0,
        [StringName("compile")]
        Compile = 1
    }

    public static class PictureLoaderImageSize
    {
        public const int XSmallWidth = 100;
        public const int XSmallHeight = 100;

        public const int SmallWidth = 490;
        public const int SmallHeight = 490;

        public const int XSMiddleWidth = 768;
        public const int XSMiddleHeight = 400;

        public const int XMiddleWidth = 600;
        public const int XMiddleHeight = 600;

        public const int MiddleWidth = 1024;
        public const int MiddleHeight = 768;

        public const int LargeWidth = 1600;
        public const int LargeHeight = 1200;

        public const int WallWidth = 1920;
        public const int WallHeight = 1080;
    }

    public class PictureLoaderTriggerModel
    {
        #region pictureLoaderTrigger options
        public int LandingPageId { get; set; }
        public int BlockId { get; set; }
        public string OnUploadFile { get; set; }
        public string OnUploadByUrl { get; set; }
        public string OnUploadIcon { get; set; }
        public string OnResize { get; set; }
        public string OnDelete { get; set; }
        public string OnInit { get; set; }
        public string OnApply { get; set; }
        public string OnLazyLoadChange { get; set; }
        public string Parameters { get; set; }
        public string Current { get; set; }
        public string NgCurrent { get; set; }
        public string NgType { get; set; }
        public bool? DeletePicture { get; set; }
        public string UploadUrlFile { get; set; }
        public string UploadUrlByAddress { get; set; }
        public string DeleteUrl { get; set; }
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public int? MaxWidthPicture { get; set; }
        public int? MaxHeightPicture { get; set; }
        public CropperParams CropperParams { get; set; }
        public bool? GalleryIconsEnabled { get; set; }
        public bool? LazyLoadEnabled { get; set; }
        public string PictureShowType { get; set; }
        
        public ePictureLoaderReplacementMode ReplacementMode { get; set; }
        public ePictureLoaderImageType? Type { get; set; }
        public ePictureLoaderLazyLoadType LazyLoadType { get; set; }
        public string ImageLink { get; set; }

        public bool? BackgroundMode { get; set; }

        public int? WidthPicture { get; set; }
        public int? HeightPicture { get; set; }
 
        public string NgWidthPicture { get; set; }
        public string NgHeightPicture { get; set; }
        #endregion

    }

    public class CropperParams
    {
        [JsonProperty(PropertyName = "aspectRatio")]
        public string AspectRatio { get; set; }
    }
}
