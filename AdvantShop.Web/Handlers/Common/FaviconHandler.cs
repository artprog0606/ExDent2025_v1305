using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Models.Common;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Common
{
    public class FaviconHandler : ICommandHandler<FaviconModel>
    {
        private const string ImgTag = "<img id=\"favicon\" src=\"{0}\" {1} />";
        private const string LinkTag = "<link rel=\"{0}\" type=\"{1}\" href=\"{2}\"{3} />";
        
        private readonly FaviconModel _faviconModel;
        private readonly string _imagePath;
        private readonly string _browser;

        public FaviconHandler(FaviconModel faviconModel, string imgSource, string browser)
        {
            _faviconModel = faviconModel;
            _imagePath = GetImagePath(imgSource);
            _browser = browser;
        }
        
        public FaviconModel Execute()
        {
            SetImgSource();

            if (!string.IsNullOrEmpty(_faviconModel.ImgSource))
                SetHtml(UrlService.GetUrl(_faviconModel.ImgSource));

            return _faviconModel;
        }
        
        private static string GetImagePath(string imageSource) =>
            imageSource.IsNotEmpty() 
                ? imageSource 
                : SettingsMain.FaviconImageName.IsNotEmpty() 
                    ? SettingsMain.FaviconImageName 
                    : "favicon.ico";
        
        private void SetImgSource() =>
            _faviconModel.ImgSource = FoldersHelper.GetPathRelative(FolderType.Pictures, _imagePath, _faviconModel.ForAdmin);

        private void SetHtml(string source) =>
            _faviconModel.Html = SettingsMain.IsFaviconImageSvg 
                ? GetHtmlForSvg(source)
                : GetHtmlForImage(source);

        private static string GetHtmlForSvg(string source) =>
            $"\r\n    <link rel=\"icon\" href=\"{source}\" type=\"image/svg+xml\">";

        private string GetHtmlForImage(string source)
        {
            var styleClass = !string.IsNullOrEmpty(_faviconModel.CssClassImage) 
                ? $"class=\"{_faviconModel.CssClassImage}\""
                : string.Empty;
            
            var rel = _browser == "IE" 
                ? "SHORTCUT ICON" 
                : "shortcut icon";
            
            string contentType;
                    
            switch (FileHelpers.GetExtension(_faviconModel.ImgSource))
            {
                case ".ico":
                    contentType = "image/x-icon";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                default:
                    contentType = "image/png";
                    break;
            }

            return _faviconModel.GetOnlyImage 
                ? string.Format(ImgTag, source, styleClass) 
                : string.Format(LinkTag, rel, contentType, source, string.Empty);
        }
    }
}