using AdvantShop.Configuration;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Socials;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System.Web;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsSocialController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.Social.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsSocialCtrl);

            var model = new GetSocialSettingsHandler().Execute();
            return View("index", model);
        }

        [HttpPost]
        public ActionResult Index(SocialSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                new SaveSocialSettingsHandler(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_EditSocialSettings);

                var isMobile = SettingsDesign.IsMobileTemplate;

                var referrerParams = new System.Web.Routing.RouteValueDictionary();

                if (HttpContext.Request.UrlReferrer != null)
                {
                    var nv = HttpUtility.ParseQueryString(HttpContext.Request.UrlReferrer.Query); //Преобразует урлхвост в NameValueCollection

                    foreach (var nvKey in nv.AllKeys)
                    {
                        if (referrerParams.ContainsKey(nvKey) is false)
                            referrerParams.Add(nvKey, nv.Get(nvKey));
                    }
                }

                if (isMobile)
                {
                    return RedirectToAction("Index", referrerParams);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                    foreach (var error in modelState.Errors)
                    {
                        ShowMessage(NotifyType.Error, error.ErrorMessage);
                    }
            }
            
            return Index();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadSocialWidgetIcon(HttpPostedFileBase file, int type)
        {
            var result = new UploadSocialWidgetPicture(file, type).Execute();
            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(T("Admin.Catalog.ErrorLoadingImage"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSocialWidgetIcon(int pictureId)
        {
            var picture = "";

            if (pictureId == 1)
            {
                picture = SettingsSocialWidget.CustomLinkIcon1;
                SettingsSocialWidget.CustomLinkIcon1 = null;
            }
            else if (pictureId == 2)
            {
                picture = SettingsSocialWidget.CustomLinkIcon2;
                SettingsSocialWidget.CustomLinkIcon2 = null;
            }
            else if (pictureId == 3)
            {
                picture = SettingsSocialWidget.CustomLinkIcon3;
                SettingsSocialWidget.CustomLinkIcon3 = null;
            }

            if (!string.IsNullOrEmpty(picture))
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.SocialWidget, picture));

            return JsonOk(new { picture = "" });
        }
    }
}