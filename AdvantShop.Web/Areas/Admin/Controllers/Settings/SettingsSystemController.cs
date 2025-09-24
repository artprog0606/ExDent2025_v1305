using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.TrafficStatistics;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Core.Services.Statistic.QuartzJobs;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings;
using AdvantShop.Web.Admin.Handlers.Settings.System;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.Jobs;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsSystemController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.System.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsSystemCtrl);

            var model = new GetSystemSettingsHandler().Execute();
            return View("index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(SystemSettingsModel model)
        {
            try
            {
                new SaveSystemSettingsHandler(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ShowErrorMessages();
            }

            bool isMobile = SettingsDesign.IsMobileTemplate;

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
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckLicense(string licKey)
        {
            return Json(new CommandResult { Result = SettingsLic.Activate(licKey) });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateSiteMaps()
        {
            return Json(new UpdateSiteMapsHandler().Execute());
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetSiteMaps()
        {
            return Json(new GetSiteMapsHandler().Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult FileStorageRecalc()
        {
            if (SettingsMain.CurrentFilesStorageLastUpdateTime < DateTime.Now.AddHours(-1) ||
                CustomerContext.CurrentCustomer.IsVirtual)
            {
                FilesStorageService.RecalcAttachmentsSizeInBackground();
            }

            return Json(new { result = true });
        }

        #region Jobs

        public JsonResult GetSchedulerJobs()
        {
            return Json(new GetSchedulerJobs().Execute());
        }

        public ActionResult GetJobRuns(JobRunsFilterModel filterModel)
        {
            var result = new GetJobRunsHandler(filterModel).Execute();

            if (filterModel.OutputDataType != FilterOutputDataType.Csv)
                return Json(result);

            var fileName = $"export_jobRuns_{DateTime.Now:ddMMyyhhmmss}.csv";
            var fullFilePath = new ExportJobRunsHandler(result, fileName).Execute();
            return File(fullFilePath, "application/octet-stream", fileName);
        }

        public JsonResult GetJobRunStatuses()
        {
            return Json(
                (from object val in Enum.GetValues(typeof(EQuartzJobStatus)) select val.ToString())
                .Select(x => new { label = x, value = x })
            );
        }

        public ActionResult GetJobRunLog(string jobRunId, JobRunLogFilterModel filterModel)
        {
            var isExport = filterModel.OutputDataType == FilterOutputDataType.Csv;
            var result = new GetJobRunLogHandler(jobRunId, filterModel, isExport).Execute();

            if (isExport is false)
                return Json(result);

            var fileName = $"export_jobRunLog_{DateTime.Now:ddMMyyhhmmss}.csv";
            var fullFilePath = new ExportJobRunLogHandler(result, fileName).Execute();
            return File(fullFilePath, "application/octet-stream", fileName);
        }

        public JsonResult GetJobRunLogEvents()
        {
            return Json(
                (from object val in Enum.GetValues(typeof(EQuartzJobEvent)) select val.ToString())
                .Select(x => new { label = x, value = x })
            );
        }

        #endregion

        #region sms settings

        [HttpGet]
        public JsonResult CheckPhoneDuplicates()
        {
            var result = new CheckPhoneDuplicateHandler().Execute();
            return JsonOk(new
            {
                haveDuplicates = result.haveDuplicates,
                urlPath = result.urlPath
            });
        }

        #endregion

        #region ip blacklist

        public JsonResult AddIpInBlacklist(List<string> ipWithMaskList)
        {
            var errorsWhenAdding = new AddIpInBlackListHandler(ipWithMaskList).Execute();
            if (errorsWhenAdding != null && errorsWhenAdding.Count > 0)
                return JsonError(errorsWhenAdding.ToArray());
            return JsonOk();
        }

        public JsonResult DeleteIpFromBlacklist(string ipWithMask)
        {
            var ipList = SettingsGeneral.BannedIpList;
            ipList.Remove(ipWithMask);
            SettingsGeneral.BannedIpList = ipList;
            return JsonOk();
        }

        public JsonResult GetIPBlacklist()
        {
            return Json(SettingsGeneral.BannedIpList);
        }

        public JsonResult GetAllowedCountriesForSite()
        {
            return Json(AdditionalOptionsService.Get(EnAdditionalOptionObjectType.Country, CountryAdditionalOptionNames.AllowSiteBrowsing)
                            .Where(x => x.Value.TryParseBool())
                            .Select(option => CountryService.GetCountry(option.ObjId))
                            .Where(country => country != null)
                            .Select(country => new
                            {
                                country.Name,
                                country.CountryId
                            }));
        }

        [HttpPost]
        public JsonResult AddCountryToAllowedSite(string countryName)
        {
            var country = CountryService.GetCountryByName(countryName);
            if (country == null)
                return JsonError(T("Admin.SettingsSystem.IpBlacklist.CountryNotFound"));
            CountryService.UpdateAdditionalSettings(new CountryAdditionalSettings
            {
                AllowSiteBrowsing = true,
                CountryId = country.CountryId
            }, true);
            return JsonOk();
        }

        public JsonResult DeleteCountryFromAllowedSite(int countryId)
        {
            if (countryId == 0)
                return JsonError();
            CountryService.DeleteAdditionalSetting(countryId, CountryAdditionalOptionNames.AllowSiteBrowsing, true);
            return JsonOk();
        }

        public JsonResult GetTrafficStatistics()
        {
            return Json(TrafficStatisticsService.GetTrafficStatistics(DateTime.Now.AddHours(-1)).Where(x => x.Value > 0).Select(traffic => new
            {
                Ip = traffic.Key,
                Text = traffic.Key + $" ({GetCountRequestFormatted(traffic.Value)})",
                Banned = UrlService.IsIpBanned(traffic.Key),
                ContainsInBannedIpList = SettingsGeneral.BannedIpList.Contains(traffic.Key),
                CountRequest = traffic.Value
            }));
        }

        private string GetCountRequestFormatted(int count)
        {
            return count + " " + Strings.Numerals(count, 
                                    T("Admin.SettingsSystem.TrafficStatistics.ZeroRequest"), 
                                    T("Admin.SettingsSystem.TrafficStatistics.OneRequest"), 
                                    T("Admin.SettingsSystem.TrafficStatistics.TwoRequest"), 
                                    T("Admin.SettingsSystem.TrafficStatistics.FiveRequest"));
        }

        #endregion
    }
}
