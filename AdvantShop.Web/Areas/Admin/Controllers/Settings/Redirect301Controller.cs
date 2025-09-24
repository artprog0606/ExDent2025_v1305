using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Seo;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class Redirect301Controller : BaseAdminController
    {
        #region Add/Edit/Get/Delete

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add301Redirect(RedirectSeo model)
        {
            return ProcessJsonResult(new AddUpdate301RedirectHandler(model, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Edit301Redirect(RedirectSeo model)
        {
            return ProcessJsonResult(new AddUpdate301RedirectHandler(model, true));
        }

        public JsonResult GetRedirect301Item(int Id)
        {
            var redirect = RedirectSeoService.GetRedirectSeoById(Id);
            if(redirect == null)
                return Json(false);
            
            return Json(redirect);
        }

        public JsonResult GetRedirect301(Admin301RedirectFilterModel model)
        {
            var hendler = new Get301Redirect(model);
            var result = hendler.Execute();

            return Json(result);
        }

        public JsonResult DeleteRedirect301(Admin301RedirectFilterModel model)
        {
            Command(model, (id, c) =>
            {
                RedirectSeoService.DeleteRedirectSeo(id);
                return true;
            });

            return Json(true);
        }
        #endregion

        public JsonResult GetActive(bool? active)
        {
            if(active == null)
                return Json(SettingsSEO.Enabled301Redirects);
            
            var currentActive = SettingsSEO.Enabled301Redirects;

            if (active != currentActive)
                SettingsSEO.Enabled301Redirects = (bool)active;
            
            return Json((bool)active);
        }

        public ActionResult Export()
        {
            var fileName = "redirects.csv";
            var fileDirectory = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);

            if (System.IO.File.Exists(fileDirectory + fileName))
            {
                System.IO.File.Delete(fileDirectory + fileName);
            }

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            try
            {
                using (var csvWriter = new CsvHelper.CsvWriter(new StreamWriter(fileDirectory + fileName), CsvConstants.DefaultCsvConfiguration))
                {

                    foreach (var item in new[] { "RedirectFrom", "RedirectTo", "ProductArtNo" })
                        csvWriter.WriteField(item);
                    csvWriter.NextRecord();

                    foreach (var redirect in RedirectSeoService.GetRedirectsSeo())
                    {
                        csvWriter.WriteField(redirect.RedirectFrom);
                        csvWriter.WriteField(redirect.RedirectTo);
                        csvWriter.WriteField(redirect.ProductArtNo);

                        csvWriter.NextRecord();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Log.Error(ex);
                return Json(false);
            }


            return File(fileDirectory + fileName, "application/octet-stream", fileName);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Import(HttpPostedFileBase file)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return JsonError();

            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            var fileName = "redirectsImport.csv";
            var fullFileName = filePath + fileName.FileNamePlusDate();

            FileHelpers.CreateDirectory(filePath);

            file.SaveAs(fullFileName);

            using (var csvReader = new CsvHelper.CsvReader(new StreamReader(fullFileName), CsvConstants.DefaultCsvConfiguration))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                while (csvReader.Read())
                {
                    try
                    {
                        var currentRecord = new RedirectSeo
                        {
                            RedirectFrom = HttpUtility.UrlDecode(csvReader.GetField<string>("RedirectFrom").ToLower()),
                            RedirectTo = HttpUtility.UrlDecode(csvReader.GetField<string>("RedirectTo").ToLower()),
                            ProductArtNo = csvReader.GetField<string>("ProductArtNo")
                        };

                        if (string.IsNullOrWhiteSpace(currentRecord.RedirectFrom) || currentRecord.RedirectFrom == "*")
                            continue;

                        var redirect = RedirectSeoService.GetRedirectsSeoByRedirectFrom(currentRecord.RedirectFrom);

                        if (redirect != null)
                            currentRecord.ID = redirect.ID;

                        if (RedirectSeoService.CheckOnSystemUrl(currentRecord.RedirectFrom) || RedirectSeoService.CheckOnSystemUrl(currentRecord.RedirectTo))
                        {
                            Debug.Log.Warn(string.Format(T("Admin.Js.Settings.AddEdit301RedCtrl.SystemUrl"), csvReader.Parser.RawRecord));
                            continue;
                        }

                        if (RedirectSeoService.IsToManyRedirects(currentRecord))
                        {
                            Debug.Log.Warn(string.Format(T("Admin.Js.Settings.Import301RedCtrl.ErrorToManyRed"), csvReader.Parser.RawRecord));
                            continue;
                        }

                        if (redirect == null)
                            RedirectSeoService.AddRedirectSeo(currentRecord);
                        else
                            RedirectSeoService.UpdateRedirectSeo(currentRecord);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                    }
                }
            }
            return Json(new {Result = true});
        }

        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceRedirect301(RedirectSeo model)
        {
            return ProcessJsonResult(new AddUpdate301RedirectHandler(model, true));
        }

        #endregion

        #region Command

        private void Command(Admin301RedirectFilterModel model, Func<int, Admin301RedirectFilterModel, bool> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                {
                    func(id, model);
                }
            }
            else
            {
                var handler = new Get301Redirect(model);
                var Ids = handler.GetItemsIds();

                foreach (int id in Ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        #endregion
    }
}
