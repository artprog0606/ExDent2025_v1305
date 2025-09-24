using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.FilePath;

namespace AdvantShop.Areas.Partners.Models.Common
{
    public class LogoModel
    {
        public LogoModel()
        {
            ImgAlt = SettingsMain.LogoImageAlt;
            DisplayHref = true;

            if (SettingsMain.LogoImageName.IsNotEmpty())
                ImgSource = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false);

            var alt = ImgAlt.IsNotEmpty() ? string.Format(" alt=\"{0}\"", ImgAlt) : string.Empty;
            if (ImgSource.IsNotEmpty())
                Html = string.Format("<img id=\"logo\" src=\"{0}\"{1} class=\"site-head-logo-picture\"/>", ImgSource, alt);
        }

        public bool DisplayHref { get; set; }

        public string ImgAlt { get; set; }

        public string ImgSource { get; set; }

        public string CssClass { get; set; }

        public string Html { get; set; }
    }
}