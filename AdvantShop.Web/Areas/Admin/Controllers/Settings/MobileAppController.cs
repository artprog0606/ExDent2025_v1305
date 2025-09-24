using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Mobiles;
using AdvantShop.Web.Admin.ViewModels.Settings;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public class MobileAppController : BaseAdminController
    {
        [SaasFeature(ESaasProperty.HaveMobileApp)]
        public ActionResult Index()
        {
            var model = new MobileAppModel()
            {
                Active = SettingsMobile.MobileAppActive,
                AppName = SettingsMobile.MobileAppName,
                ShortName = SettingsMobile.MobileAppShortName,
                AppleAppStoreLink = SettingsMobile.MobileAppAppleAppStoreLink,
                GooglePlayMarket = SettingsMobile.MobileAppGooglePlayMarket,
                ImgSrc = string.IsNullOrEmpty(SettingsMobile.MobileAppIconImageName)
                    ? null
                    : FoldersHelper.GetPath(FolderType.MobileAppIcon, SettingsMobile.MobileAppIconImageName, true),
                ShowBadges = SettingsMobile.MobileAppShowBadges
            };

            SetMetaInformation("Мобильное приложение");
            SetNgController(NgControllers.NgControllersTypes.SettingsMobileCtrl);

            return View(model);
        }

        [SaasFeature(ESaasProperty.HaveMobileApp)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(MobileAppModel model)
        {
            SettingsMobile.MobileAppActive = model.Active;
            SettingsMobile.MobileAppName = model.AppName;
            SettingsMobile.MobileAppShortName = model.ShortName;
            SettingsMobile.MobileAppAppleAppStoreLink = model.AppleAppStoreLink;
            SettingsMobile.MobileAppGooglePlayMarket = model.GooglePlayMarket;
            SettingsMobile.MobileAppShowBadges = model.ShowBadges;
            new WebManifestHandler().Execute();

            return RedirectToAction("Index");
        }

        [SaasFeature(ESaasProperty.HaveMobileApp)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadIcon()
        {
            var result = new UploadMobileAppIconHandler().Execute();

            if (!result.Result)
                return JsonError(result.Error);

            new WebManifestHandler().Execute();
            return JsonOk(new
            {
                picture = result.Picture,
                pictureId = result.PictureId
            });
        }

        [SaasFeature(ESaasProperty.HaveMobileApp)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadIconByLink(string fileLink)
        {
            var result = new UploadMobileAppIconByLinkHandler(fileLink).Execute();
            if (!result.Result)
                return JsonError(result.Error);

            new WebManifestHandler().Execute();
            return JsonOk(new
            {
                picture = result.Picture,
                pictureId = result.PictureId
            });
        }

        [SaasFeature(ESaasProperty.HaveMobileApp)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteIcon()
        {
            FileHelpers.DeleteMobileAppIcons(SettingsMobile.MobileAppIconImageName);
            SettingsMobile.MobileAppIconImageName = string.Empty;

            new WebManifestHandler().Execute();
            return JsonOk(new { picture = string.Empty });
        }
    }
}
