namespace AdvantShop.Core.Services.Landing.Pictures
{
    public class PhotoModel
    {
        public string Src { get; set; }
        public string Preview { get; set; }
        public bool LazyLoadEnabled { get; set; }
        public ePictureLoaderImageType? Type { get; set; }
        
        public int? Width { get; set; }
        public int? Height { get; set; }
        
        public PhotoModel()
        {
            LazyLoadEnabled = true;
        }
    }

    public class VideoModel
    {
        public string urlVideo { get; set; }
        public string heightVideo { get; set; }
        public string widthVideo { get; set; } 
        public string alignButton { get; set; }
        public bool autoplayVideo { get; set; }
        public bool inModal { get; set; }
        public bool? preview { get; set; }
        public bool? asBackground { get; set; }
        public string insertionMethod { get; set; }
        public UploadVideoModel upload { get; set; }
        public PhotoModel coverVideo { get; set; }
    }

    public class UploadVideoModel
    {
        public string urlVideo { get; set; }
    }
}
