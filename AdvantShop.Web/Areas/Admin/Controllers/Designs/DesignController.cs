using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Design;
using AdvantShop.Web.Admin.Models.Designs;
using AdvantShop.Web.Admin.ViewModels.Design;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Controls;
using AdvantShop.FilePath;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Admin.Controllers.Designs
{
    [Auth(RoleAction.Store)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class DesignController : BaseAdminController
    {
        public ActionResult Index(bool showCommon = false)
        {
            if (Request["clearCache"] != null)
                CacheManager.Clean();

            var model = new DesignHandler(Request["stringid"]).Execute();

            SetMetaInformation(T("Admin.Design.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.DesignCtrl);

            if (!SettingsCongratulationsDashboard.DesignDone && !SettingsDesign.IsMobileTemplate)
            {
                SettingsCongratulationsDashboard.DesignDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_DesignDone);
            }
            var isMobile = SettingsDesign.IsMobileTemplate;
            
            return View(isMobile && showCommon ? "Common" : "Index", model);
        }

        [ChildActionOnly]
        public ActionResult MenuJson(bool isOpen)
        {
            return PartialView(new MenuJsonModel
            {
                IsOpen = isOpen,
                MobileAppActive = true //FeaturesService.IsEnabled(EFeature.MobileApp)
            });
        }

        public ActionResult ApplyTemplate(string templateId)
        {
            if (templateId.IsNullOrEmpty())
            {
                ShowMessage(Core.Controls.NotifyType.Error, T("Admin.Design.ErrorApplyingTemplate"));
            }
            else
            {
                SettingsDesign.ChangeTemplate(templateId);
                CacheManager.Clean();
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Design_TemplateApplied, templateId);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult PreviewTemplate(int id, string previewTemplateId)
        {
            var installTemplate = TemplateService.InstallTemplate(id, previewTemplateId, true);

            if (!installTemplate)
                return Json(false);

            SettingsDesign.PreviewTemplate = previewTemplateId;
            CacheManager.Clean();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Trial_PreviewDesignTemplate);

            return Json(true);
        }

        public ActionResult CancelPreviewTemplate()
        {
            SettingsDesign.PreviewTemplate = null;
            CacheManager.Clean();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetDesigns(string stringId = null)
        {
            var handler = new DesignHandler(stringId);
            var model = handler.Execute();

            var extModel = new
            {
                Themes = model.Themes,
                BackGrounds = model.BackGrounds,
                ColorSchemes = model.ColorSchemes,
                DesignCurrent = new
                {
                    model.CurrentTheme,
                    model.CurrentBackGround,
                    model.CurrentColorScheme
                }
            };

            return Json(extModel);
        }

        [HttpGet]
        public JsonResult GetThemes()
        {
            return Json(new
            {
                Themes = DesignService.GetDesigns(eDesign.Theme, false),
                BackGrounds = DesignService.GetDesigns(eDesign.Background, false),
                ColorSchemes = DesignService.GetDesigns(eDesign.Color, false),

                CurrentTheme = SettingsDesign.ThemeInDb,
                CurrentBackGround = SettingsDesign.BackgroundInDb,
                CurrentColorScheme = SettingsDesign.ColorSchemeInDb,
            });
        }

        [HttpPost]
        public JsonResult SaveDesign(eDesign designType, string name)
        {
            return Json(new DesignHandler().SaveDesign(designType, name));
        }

        [HttpPost]
        public JsonResult UploadDesign(eDesign designType, HttpPostedFileBase file)
        {
            try
            {
                return JsonOk(new DesignHandler().UploadDesignFile(designType, file));
            }
            catch (BlException e)
            {
                ModelState.AddModelError(e.Property, e.Message);
                return JsonError();
            }
        }

        [HttpPost]
        public JsonResult DeleteDesign(eDesign designType, string name)
        {
            return Json(new DesignHandler().DeleteDesign(designType, name));
        }

        public ActionResult CssEditor()
        {
            var css = new CssEditorHandler().GetFileContent();

            SetMetaInformation(T("Admin.Design.CssEditor.Title"));
            SetNgController(NgControllers.NgControllersTypes.CssEditorCtrl);

            return View((object)css);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CssEditor(string value)
        {
            var result = new CssEditorHandler().SaveFileContent(value);
            return Json(new { result = result });
        }

        public JsonResult TemplateSettings()
        {
            var model = new TemplateSettingsHandler().Execute();
            return Json(model);
        }

        [HttpPost]
        public JsonResult SaveTemplateSettings(string settings)
        {
            var model = new TemplateSettingsHandler().SaveSettings(settings);
            return Json(model);
        }

        [HttpPost]
        public JsonResult ResizePictures()
        {
            if (CommonStatistic.IsRun) return Json(new CommandResult() { Result = false, Error = T("Admin.Designs.NotPossibleToCompressPhotos") });

            try
            {
                CommonStatistic.StartNew(() =>
                    {
                        CommonStatistic.TotalRow = PhotoService.GetCountPhotos(0, PhotoType.Product);
                        Helpers.FileHelpers.ResizeAllProductPhotos();
                    },
                    "settingstemplate?settingsTab=catalog",
                    "Пережатие фотографии товаров");
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Design_ResizePictures);

            return Json(new CommandResult() { Result = true });
        }
        
        [HttpPost]
        public JsonResult ResizeCategoryPictures(CategoryImageType type)
        {
            if (CommonStatistic.IsRun) 
                return Json(new CommandResult() { Result = false, Error = T("Admin.Designs.NotPossibleToCompressPhotos") });

            try
            {
                CommonStatistic.StartNew(() =>
                    {
                        FileHelpers.ResizeCategoryPhotos(type);
                    },
                    "settingstemplate?settingsTab=catalog",
                    "Пережатие фотографии категорий");
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return Json(new CommandResult() { Result = true });
        }

        #region Install, update, delete template

        [HttpPost]
        public JsonResult InstallTemplate(string stringId, int id, string version)
        {
            if (stringId != TemplateService.DefaultTemplateId)
            {
                if (string.IsNullOrWhiteSpace(stringId) || string.IsNullOrWhiteSpace(version))
                {
                    return JsonError(T("Admin.Designs.TemplateNotFound"));
                }

                if (!TemplateService.InstallTemplate(id, stringId, false))
                {
                    return JsonError(T("Admin.Designs.TemplateInstallationError"));
                }
            }

            SettingsDesign.ChangeTemplate(stringId);
            CacheManager.Clean();
            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Design_TemplateInstalled, stringId);

            return JsonOk(true, T("Admin.Designs.TemplateInstalled"));
        }

        [HttpPost]
        public ActionResult UpdateTemplate(int id, string stringId)
        {
            if (!TemplateService.UpdateTemplate(id, stringId))
            {
                ShowMessage(NotifyType.Error, T("Admin.Designs.UpdatingTemplateError"));
            }
            else
            {
                ShowMessage(NotifyType.Success, T("Admin.Designs.TemplateUpdated"));
            }

            CacheManager.Clean();

            return Redirect("?tabsDesignTemplates=home&clearCache=true");
        }

        [HttpPost]
        public JsonResult DeleteTemplate(string stringId)
        {
            if (!TemplateService.UninstallTemplate(stringId))
            {
                return Json(new CommandResult() { Result = false });
            }

            if (SettingsDesign.Template.ToLower() == stringId.ToLower())
            {
                SettingsDesign.ChangeTemplate(TemplateService.DefaultTemplateId);
            }

            return Json(new CommandResult() { Result = true });
        }

        #endregion

        #region Edit theme

        public ActionResult Theme(string theme, eDesign design)
        {
            var themes = DesignService.GetDesigns(design);

            var selectedTheme = themes.Find(x => x.Name.ToLower() == theme.ToLower());
            if (selectedTheme == null)
                return RedirectToAction("Index");

            var model = new GetTheme(selectedTheme, design).Execute();

            SetMetaInformation(T("Admin.Design.Theme.Title"));
            SetNgController(NgControllers.NgControllersTypes.DesignCtrl);

            return View(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveTheme(string theme, eDesign design, string themeCss)
        {
            var themes = DesignService.GetDesigns(design);

            var selectedTheme = themes.Find(x => x.Name.ToLower() == theme.ToLower());
            if (selectedTheme == null)
                return JsonError("Theme not exist");

            new SaveTheme(selectedTheme, design, themeCss).Execute();

            return JsonOk();
        }

        [HttpPost]
        public JsonResult ThemeFiles(ThemeFilesModel model)
        {
            var themes = DesignService.GetDesigns(model.Design);

            var selectedTheme = themes.Find(x => x.Name.ToLower() == model.Theme.ToLower());
            if (selectedTheme == null)
                return JsonError("Theme not exist");

            var designFolderPath = SettingsDesign.Template != TemplateService.DefaultTemplateId
                                            ? "Templates/" + SettingsDesign.Template + "/"
                                            : "";

            var themeFolderPath = string.Format("{0}design/{1}s/{2}/images", designFolderPath, model.Design, selectedTheme.Name);
            var themeFolder = HostingEnvironment.MapPath("~/" + themeFolderPath);

            switch (model.Action)
            {
                case "getfiles":
                    var dir = new DirectoryInfo(themeFolder);
                    if (!dir.Exists)
                        FileHelpers.CreateDirectory(themeFolder);

                    var files =
                        dir.GetFiles().Select(x =>
                            new
                            {
                                x.Name,
                                Preview =
                                    new[] { ".jpg", ".png", ".jpeg", ".gif", ".bmp" }.Any(ext => Path.GetExtension(x.FullName) == ext)
                                        ? Path.Combine(UrlService.GetUrl(), themeFolderPath, x.Name)
                                        : null
                            });
                    return Json(new { files });

                case "remove":
                    FileHelpers.DeleteFile(themeFolder + "/" + model.RemoveFile);
                    break;

                case "upload":
                    if (Request.Files == null || Request.Files.Count == 0)
                        return JsonError("file is null");

                    var hasErrors = false;

                    for (var i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files.Get(i);
                        if (file == null)
                            continue;

                        if (file.ContentLength > 5000000 ||
                            !FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image))
                        {
                            hasErrors = true;
                            continue;
                        }
                        file.SaveAs(Path.Combine(themeFolder, file.FileName));
                    }
                    if (hasErrors)
                        return JsonError(T("Admin.Designs.FileSizeLimit5MB"));
                    break;
            }
            return JsonOk();
        }

        [HttpPost]
        public JsonResult CopyTheme(string theme, eDesign design, string newThemeTitle, string newThemeCss)
        {
            var themes = DesignService.GetDesigns(design);

            var selectedTheme = themes.Find(x => x.Name.ToLower() == theme.ToLower());
            if (selectedTheme == null)
                return JsonError("Theme not exist");

            var newThemeName = new CopyThemeHandler(selectedTheme, design, newThemeTitle, newThemeCss).Execute();

            return JsonOk(new
            {
                newThemeEditUrl = Url.AbsoluteActionUrl("Theme", "Design", new { theme = newThemeName, design })
            });
        }

        #endregion Edit theme

        public ActionResult TemplateShop()
        {
            var model = new DesignHandler(Request["stringid"]).Execute();

            SetMetaInformation(T("Admin.Design.TemplateShop.Title"));
            SetNgController(NgControllers.NgControllersTypes.DesignCtrl);

            return View(model);
        }
    }
}