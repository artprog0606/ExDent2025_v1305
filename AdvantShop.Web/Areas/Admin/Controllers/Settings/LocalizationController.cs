using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.System;
using AdvantShop.Web.Admin.Models.Catalog.Import;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Filters;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class LocalizationController : BaseAdminController
    {
        #region Add/Edit/Get/Delete
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddEditLocalization(LocalizedResource model)
        {
            try
            {
                var languageId = LanguageService.GetLanguage(model.LanguageCode).LanguageId;
                LocalizationService.AddOrUpdateResource(languageId, model.ResourceKey, model.ResourceValue ?? "", CustomerContext.CustomerId.ToString());

                var key = model.ResourceKey.ToLower();
                if (key.StartsWith("js.") || key.StartsWith("admin.js"))
                    LocalizationService.GenerateJsResourcesFile();
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        public JsonResult GetLocalizeItem(string languageCode, string resourceKey)
        {
            var languageId = LanguageService.GetLanguage(languageCode).LanguageId;
            var localize = SQLDataAccess.Query<LocalizedResource>(
                    "Select Top(1) LanguageId,ResourceKey,ResourceValue From Settings.Localization " +
                    "Where LanguageId=@languageId and ResourceKey=@resourceKey", new { languageId, resourceKey }, CommandType.Text);

            if (localize == null)
                return Json(false);

            return Json(localize);
        }

        public JsonResult GetLocalizations(AdminLocalizationsFilterModel model)
        {
            var handler = new GetLocalizations(model);
            var result = handler.Execute();

            return Json(result);
        }

        public JsonResult GetLanguage(string lang)
        {
            return Json(LanguageService.GetList()
                .Select(x => new SelectListItem { Text = x.Name, Value = x.LanguageCode, Selected = lang == x.LanguageCode })
                .ToList());
        }

        #endregion

        public ActionResult Export(string lang)
        {
            var language = LanguageService.GetLanguage(lang);
            var fileName = "localization.csv";
            var fileDirectory = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);

            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);

            using (var csvWriter = new CsvHelper.CsvWriter(new StreamWriter(fileDirectory + fileName, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                foreach (var item in new[] { "ResourceKey", "ResourceValue", "LanguageCode" })
                    csvWriter.WriteField(item);

                csvWriter.NextRecord();

                foreach (var resource in LocalizationService.GetResources(language.LanguageCode))
                {
                    csvWriter.WriteField(resource.Key);
                    csvWriter.WriteField(resource.Value);
                    csvWriter.WriteField(language.LanguageCode);

                    csvWriter.NextRecord();
                }
            }

            return File(fileDirectory + fileName, "application/octet-stream", fileName);
        }

        private static Encoding CheckEncoding(string filename)
        {
            // https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            var encoding = StringHelper.DetectFileTextEncoding(filename);

            return encoding;
        }

        [Auth(RoleAction.Customers)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Import(HttpPostedFileBase file)
        {
            if (file.ContentLength < 1)
                return Json(new { result = false });

            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            const string fileName = "localizationImport.csv";
            var fullFileName = filePath + fileName;

            try
            {
                FileHelpers.CreateDirectory(filePath);

                file.SaveAs(fullFileName);

                if (CheckEncoding(fullFileName) != Encoding.UTF8)
                    return Json(new UploadFileResult {Error = T("Admin.Localization.Import.Error")});

                using (var csvReader = new CsvHelper.CsvReader(new StreamReader(fullFileName), CsvConstants.DefaultCsvConfiguration))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                    {
                        var cultureName = csvReader.GetField<string>("LanguageCode");
                        var languageId = 0;

                        if (!string.IsNullOrEmpty(cultureName))
                        {
                            var language = LanguageService.GetLanguage(cultureName);
                            if (language != null)
                            {
                                languageId = language.LanguageId;
                            }
                            else
                            {
                                try
                                {
                                    var ci = CultureInfo.GetCultureInfo(cultureName);
                                    languageId = LanguageService.Add(new Language { Name = ci.DisplayName, LanguageCode = ci.Name });
                                    LocalizationService.ReloadLocalizedSets();
                                }
                                catch (Exception ex)
                                {
                                    Debug.Log.Error(ex);
                                }
                            }
                        }

                        if (languageId == 0)
                            continue;

                        var key = csvReader.GetField<string>("ResourceKey");
                        var value = csvReader.GetField<string>("ResourceValue");
                        
                        LocalizationService.AddOrUpdateResource(languageId, key, value ?? "", "Import localization from csv");
                    }
                }

                FileHelpers.DeleteFile(fullFileName);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Json(new UploadFileResult {Error = T("Admin.Localization.Import.Error")});
            }
            return Json(new UploadFileResult { Result = true });
        }
    }
}
