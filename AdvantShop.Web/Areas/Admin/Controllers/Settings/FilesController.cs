using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Files;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Files;
using AdvantShop.Web.Admin.Models.Cms.Files;
using AdvantShop.Web.Admin.ViewModels.Settings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class FilesController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Files.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.FilesCtrl);

            var rootFiles = FileHelpers.GetFilesInRootDirectory();
            var filesInDb = FileInDbService.GetList().Where(x => x.Path == "/" + x.Name).Select(x => x.Name).ToList();
            var showWarning = rootFiles.Any(x => filesInDb.Contains(x.Name));

            var model = new FilesViewModel
            {
                Extensions = string.Join(", ", FileHelpers.GetAllowedFileExtensions(EFileType.Text | EFileType.Csv | EFileType.Html | EFileType.Xml)),
                ShowWarning = showWarning
            };

            return View(model);
        }

        public JsonResult GetFiles(FilesFilterModel model) => Json(new GetFilesHandler(model).Execute());

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFiles(FilesFilterModel model)
        {
            Command(model, (id, c) =>
            {
                FileInDbService.Delete(id);
                return true;
            });

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFile(int id)
        {
            FileInDbService.Delete(id);
            return JsonOk();
        }

        #region Commands

        private bool Command(FilesFilterModel command, Func<int, FilesFilterModel, bool> func)
        {
            bool result = true;

            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    if (func(id, command) == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                var ids = new GetFilesHandler(command).GetItemsIds("Id");
                foreach (var id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                    {
                        if (func(id, command) == false)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            if (file == null)
                return Json(new { Result = false, Error = T("Admin.Files.Index.FileNotFound") });

            if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Text | EFileType.Csv | EFileType.Html | EFileType.Xml))
                return Json(new { Result = false, Error = T("Admin.Files.Index.InvalidFileNameExtension") });

            const int allowedSizeInMb = 10;
            if (FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Html) 
                && file.ContentLength > allowedSizeInMb * 1024 * 1024) 
            {
                return Json(new { Result = false, Error = T("Admin.Files.Index.HtmlFileMoreThanAllowed", $"{allowedSizeInMb}Mb") });
            }

            var path = "/" + file.FileName;

            if (FileInDbService.IsExist(path))
                return Json(new { Result = false, Error = T("Admin.Files.Index.FileAlreadyExists") });
            
            if (System.IO.File.Exists(SettingsGeneral.AbsolutePath + file.FileName))
                return Json(new { Result = false, Error = T("Admin.Files.Index.FileAlreadyExistInRoot", file.FileName) });

            FileInDbService.Add(new FileInDb(file, "/"));

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Files_AddFile);

            return Json(new { Result = true });
        }
    }
}
