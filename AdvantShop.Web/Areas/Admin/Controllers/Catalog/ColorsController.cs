using System;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ExportImport;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.Colors;
using AdvantShop.Web.Admin.Models.Catalog.Colors;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class ColorsController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Colors.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.ColorsCtrl);

            return View();
        }

        public JsonResult GetColors(ColorsFilterModel model)
        {
            return Json(new GetColorsHandler(model).Execute());
        }

        #region Commands

        private void Command(ColorsFilterModel command, Action<int, ColorsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetColorsHandler(command).GetItemsIds("Color.ColorId");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteColors(ColorsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                if (!ColorService.IsColorUsed(id))
                    ColorService.DeleteColor(id);
            });
            return Json(true);
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteColor(int colorId)
        {
            if (!ColorService.IsColorUsed(colorId))
                ColorService.DeleteColor(colorId);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceColor(ColorModel model)
        {
            var color = ColorService.GetColor(model.ColorId);
            if (color == null)
                return Json(new { result = false });

            color.ColorName = model.ColorName.DefaultOrEmpty();
            color.SortOrder = model.SortOrder;

            ColorService.UpdateColor(color);

            return Json(new { result = true });
        }

        #region Add | Update color

        [HttpGet]
        public JsonResult GetColor(int colorId)
        {
            var color = ColorService.GetColor(colorId);
            return Json(new ColorModel()
            {
                ColorId = color.ColorId,
                ColorName = color.ColorName,
                ColorCode = color.ColorCode,
                PhotoName = color.IconFileName.PhotoName,
                SortOrder = color.SortOrder,
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddColor(ColorModel model, HttpPostedFileBase colorIconFile)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var result = new AddUpdateColor(model, colorIconFile, false).Execute();

            return Json(new { result = result });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateColor(ColorModel model, HttpPostedFileBase colorIconFile)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var result = new AddUpdateColor(model, colorIconFile, true).Execute();

            return Json(new { result = result });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteIcon(int colorId)
        {
            var color = ColorService.GetColor(colorId);
            if (color == null)
                return Json(new { result = false });

            PhotoService.DeletePhotos(colorId, PhotoType.Color);

            return Json(new { result = true });
        }

        #endregion


        public JsonResult GetFormData()
        {
            return Json(new
            {
                filesHelpText = FileHelpers.GetFilesHelpText(EFileType.Image, "15MB"),
                iconSizesText = string.Format(
                    T("Admin.Catalog.IconSizeInDirectory") + " {0} x {1} px <br/>" +
                    T("Admin.Catalog.IconSizeInProductCard") + " {2} x {3} px<br />" +
                    T("Admin.Catalog.CanChangeInSettings") + " <a href=\"{4}\" target=\"_blank\">" + T("Admin.Catalog.InSettings") + "</a>",
                    SettingsPictureSize.ColorIconWidthCatalog, SettingsPictureSize.ColorIconHeightCatalog,
                    SettingsPictureSize.ColorIconWidthDetails, SettingsPictureSize.ColorIconHeightDetails,
                    UrlService.GetAdminUrl("settingstemplate?settingsTab=catalog"))
            });
        }

        #region Export and Import

        public ActionResult Export()
        {
            var fileName = string.Format("colors_{0}.csv", DateTime.Now.ToString("yyMMddhhmmss"));
            var result = new ExportColors(fileName).Execute();

            return File(result, "application/octet-stream", fileName);
        }

        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Import(HttpPostedFileBase file, ImportColorSettings model)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return JsonError();
            try
            {
                new ImportColors(file, model).Execute();
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
            return Json(new {Result = true});
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadZipFile(HttpPostedFileBase file)
        {
            var result = new UploadColorImagesArchiveFile(file).Execute();
            return Json(result);
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteZipFile()
        {
            var result = new DeleteColorImagesArchiveFile().Execute();
            return Json(result);
        }

        #endregion

        public JsonResult UpdateUnSetColors()
        {
            ColorService.UpdateUnSetColorsFromAdvantshopCsv();
            return JsonOk();
        }
    }
}
