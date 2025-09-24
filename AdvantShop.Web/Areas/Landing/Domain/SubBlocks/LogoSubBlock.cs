using System.IO;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.FilePath;

namespace AdvantShop.App.Landing.Domain.SubBlocks
{
    public class LogoSubBlock : BaseLpSubBlock
    {
        public override dynamic GetSettings(LpBlock currentBlock, LpConfiguration configuration, dynamic settings)
        {
            if (settings == null)
                return null;

            settings.alt = SettingsMain.LogoImageAlt;
            
            //if (SettingsMain.IsDefaultLogo && settings.src != null && settings.src != "")
            //    return settings;

            if (!string.IsNullOrEmpty(SettingsMain.LogoImageName))
            {
                var logoPath = FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName);

                if (File.Exists(logoPath))
                {
                    var lp = new LpService().Get(currentBlock.LandingId);

                    var lpFolder = string.Format(LpFiles.UploadPictureFolderLandingBlockRelative, lp.LandingSiteId, currentBlock.LandingId, currentBlock.Id);
                    var lpFolderAbsolut = HostingEnvironment.MapPath("~/" + lpFolder);

                    if (!Directory.Exists(lpFolderAbsolut))
                        Directory.CreateDirectory(lpFolderAbsolut);

                    File.Copy(logoPath, lpFolderAbsolut + SettingsMain.LogoImageName);
                    settings.src = lpFolder + SettingsMain.LogoImageName;
                }

            }
            return settings;
        }
    }
}
