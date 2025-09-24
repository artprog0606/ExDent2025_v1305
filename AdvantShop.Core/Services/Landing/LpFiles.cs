
namespace AdvantShop.Core.Services.Landing
{
    public class LpFiles
    {
        public const string UploadPictureFolder = "~/pictures/landing/";

        public const string LpSitePath = "~/pictures/landing/{0}/";           // siteId
        public const string LpSitePathRelative = "pictures/landing/{0}/";     // siteId

        public const string LpSiteLandingPagePath = "~/pictures/landing/{0}/{1}";   // siteId, lpId

        public const string UploadPictureFolderLandingBlock = "~/pictures/landing/{0}/{1}/{2}/";       // siteId, lpId, blockId
        public const string UploadPictureFolderLandingBlockRelative = "pictures/landing/{0}/{1}/{2}/"; // siteId, lpId, blockId
        
        public const string LpGeneratedCss = "~/pictures/landing/{0}/head.css";       // siteId
        public const string LpGeneratedCssRelative = "pictures/landing/{0}/head.css"; // siteId

        public const string LpStaticPath = "~/areas/landing/";


        public const string TepmlateFolder = "~/Areas/Landing/Templates";
        public const string ViewsFolder = "~/Areas/Landing/Views";
    }
}
