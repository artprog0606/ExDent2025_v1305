using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Core.Services.Screenshot
{
    public class ScreenshotCreateDto
    {
        public string LicKey { get; set; }
        public string Url { get; set; }
        public string ScreenShotName { get; set; }

        [Range(0, 1920)]
        public int? Width { get; set; }

        [Range(0, 1080)]
        public int? Height { get; set; }
        

        public int? CropWidth { get; set; }
        public int? CropHeight { get; set; }

        public bool IsMobile { get; set; }
    }
}
