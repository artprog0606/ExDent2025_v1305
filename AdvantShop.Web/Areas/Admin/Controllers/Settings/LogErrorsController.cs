using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [ExcludeFilter(typeof(CacheFilterAttribute))]
    public partial class LogErrorsController : BaseAdminController
    {
        #region Add/Edit/Get/Delete

        public JsonResult GetLogErrors(ErrlogFilterModel model)
        {
            var result = GetErrors(model);

            return result != null
                ? JsonOk(result)
                : JsonError();
        }

        public JsonResult GetItemLogError(ErrType type, DateTime time, int page = 0)
        {
            var result = GetError(type, time, page);

            return result != null
                ? JsonOk(result)
                : JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult RemoveLogs(ErrType type)
        {
            if (!CustomerContext.CurrentCustomer.IsVirtual)
                return JsonError();

            var files = Directory.GetFiles(Debug.ErrFilesPath, type + "*").Where(file => new FileInfo(file).Length > 3).ToList();
            foreach (var file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch
                {
                    //ignore
                }
            }
            return JsonOk();
        }

        #endregion

        #region Help method

        private FilterResult<LogEntry> GetErrors(ErrlogFilterModel filter)
        {
            if (!System.IO.File.Exists(Debug.GetErrFileName(filter.Type)))
                return null;

            var type = filter.Type.ToString();
            var pages = Directory.GetFiles(Debug.ErrFilesPath, type + "*").Count(file => new FileInfo(file).Length > 3);

            var model = new FilterResult<LogEntry>
            {
                DataItems = new List<LogEntry>(),
                TotalPageCount = pages,
                TotalItemsCount = pages * filter.ItemsPerPage,
                PageIndex = filter.Page != 0
                    ? filter.Page
                    : 1
            };

            try
            {
                var filePath = Debug.GetErrFileName(filter.Type) +
                               (filter.Page != 1
                                   ? "." + (filter.Page - 1)
                                   : "");
                var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
                csvConfiguration.Delimiter = Debug.CharSeparate;
                csvConfiguration.HasHeaderRecord = false;
                csvConfiguration.BadDataFound = null;
                using (var csv = new CsvHelper.CsvReader(new StreamReader(filePath, Encoding.UTF8, true), csvConfiguration))
                {
                    while (csv.Read())
                    {
                        if (csv.Parser.Record == null || csv.Parser.Record.Count(x => !string.IsNullOrEmpty(x)) < 4)
                            continue;

                        var item = new LogEntry
                        {
                            DateTime = csv.Parser.Record[0].TryParseDateTime(),
                            Level = csv.Parser.Record[1] ?? "",
                            Message = csv.Parser.Record[2] ?? "",
                            ErrorMessage = csv.Parser.Record[3] ?? "",
                            Type = type
                        };
                        model.DataItems.Add(item);
                    }
                }
                model.DataItems.Reverse();

                return model;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }

        private AdvException GetError(ErrType errType, DateTime time, int page)
        {
            var filePath = Debug.GetErrFileName(errType) + (page > 1
                ? "." + (page - 1)
                : "");
            if (!System.IO.File.Exists(filePath))
                return null;

            try
            {
                var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
                csvConfiguration.Delimiter = Debug.CharSeparate;
                csvConfiguration.HasHeaderRecord = false;
                csvConfiguration.BadDataFound = null;
                using (var csv = new CsvHelper.CsvReader(new StreamReader(filePath, Encoding.UTF8, true), csvConfiguration))
                {
                    while (csv.Read())
                    {
                        if (csv.Parser.Record == null || csv.Parser.Record.Count(x => !string.IsNullOrEmpty(x)) < 4)
                            continue;

                        if (csv.Parser.Record[0].TryParseDateTime() != time)
                            continue;

                        var error = new AdvException();
                        try
                        {
                            if (csv.Parser.Record[4] != "none")
                                error = AdvException.GetFromJsonString(csv.Parser.Record[4]);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error(ex);
                        }

                        error.ExceptionData.ManualMessage = csv.Parser.Record[2];
                        error.ExceptionData.Date = time.ToString("G");

                        return error;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }

        #endregion
    }
}
