using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.ViewModel.Mobile;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class MobileController : BaseClientController
    {
        public ActionResult MobileOverlap()
        {
            //if (!SettingsMobile.IsMobileTemplateActive)
            //    return new EmptyResult();

            //var forcedCookie = CommonHelper.GetCookie("deviceMode");
            //if (forcedCookie != null)
            //    return new EmptyResult();

            var model = new MobileOverlapViewModel()
            {
                LogoPath =
                    !string.IsNullOrEmpty(SettingsMain.LogoImageName)
                        ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false)
                        : null,
                LogoAlt = SettingsMain.LogoImageAlt
            };

            return PartialView(model);
        }

        public ActionResult ToMobileIcon()
        {
            if (!SettingsMobile.IsMobileTemplateActive)
                return new EmptyResult();

            if (!MobileHelper.IsMobileBrowser())
                return new EmptyResult();

            var model = new MobileOverlapViewModel()
            {
                ToolbarEnabled = SettingsDesign.DisplayToolBarBottom
            };
            
            return PartialView(model);
        }
    }
}