using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm.Ok.OkMarket;
using AdvantShop.Core.Services.Crm.Ok.OkMarket.Export;
using AdvantShop.Core.Services.Crm.Ok.OkMarket.Import;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.OkMarket;
using AdvantShop.Web.Admin.Models.OkMarket;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Ok)]
    public partial class OkMarketController : BaseAdminController
    {
        #region Catalogs

        [HttpGet]
        public JsonResult GetCatalogs()
        {
            return Json(new GetOkCatalogs().Execute());
        }

        [HttpGet]
        public JsonResult GetCatalog(int id)
        {
            var result = new OkMarketCatalogModel(OkMarketService.GetCatalog(id))
            {
                CategoryIds = OkMarketService.GetLinkedCategories(id).Select(x => x.CategoryId).ToList()
            };
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveCatalog(OkMarketCatalogModel model)
        {
            return ProcessJsonResult(new AddUpdateOkCatalog(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(OkMarketCatalogModel model)
        {
            return ProcessJsonResult(new AddUpdateOkCatalog(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCatalog(int id, long OkCatalogId)
        {
            try
            {
                OkMarketDeleteState.Start();

                new OkMarketApiService().DeleteCatalogWithPhotos(OkCatalogId);
                OkMarketService.DeleteCatalog(id, OkCatalogId);

                OkMarketDeleteState.Stop();
                return JsonOk();
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCatalogs(BaseFilterModel model)
        {
            try
            {
                OkMarketDeleteState.Start();

                Command(model, (id, c) =>
                {
                    var cat = OkMarketService.GetCatalog(id);
                    if (cat != null)
                    {
                        new OkMarketApiService().DeleteCatalogWithPhotos(cat.OkCatalogId);
                        OkMarketService.DeleteCatalog(cat.Id, cat.OkCatalogId);
                    }
                });

                OkMarketDeleteState.Stop();
                return JsonOk();
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public JsonResult GetDeleteState()
        {
            return Json(new
            {
                isRun = OkMarketDeleteState.IsRun
            });
        }

        #endregion

        #region Command

        private void Command(BaseFilterModel model, Action<int, BaseFilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                Parallel.ForEach(model.Ids, new ParallelOptions { MaxDegreeOfParallelism = SettingsMain.UseMultiThreads ? 10 : 1 }, (id) =>
                {
                    func(id, model);
                });
            }
            else
            {
                var ids = new GetOkCatalogs().Execute().DataItems.Select(x => x.Id);
                Parallel.ForEach(ids, new ParallelOptions { MaxDegreeOfParallelism = SettingsMain.UseMultiThreads ? 10 : 1 }, (id) =>
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                });
            }
        }

        #endregion

        #region Export

        [HttpGet]
        public JsonResult GetExportSettings()
        {
            OkMarketExportState.DeleteExpiredLogs();

            return Json(new OkMarketExportSettingsModel
            {
                ExportPreorderProducts = OkMarketExportSettings.ExportPreorderProducts,
                ExportUnavailableProducts = OkMarketExportSettings.ExportUnavailableProducts,
                CurrencyIso3 = OkMarketExportSettings.CurrencyIso3 ?? CurrencyService.CurrentCurrency.Iso3,
                ExportSizeAndColorInName = OkMarketExportSettings.SizeAndColorInName,
                ExportUpdateProductPhotos = OkMarketExportSettings.UpdateProductPhotos,
                ExportSizeAndColorInDescription = OkMarketExportSettings.SizeAndColorInDescription,
                ExportProperties = OkMarketExportSettings.ExportProperties,
                ExportLinkToSite = (int)OkMarketExportSettings.ExportLinkToSite,
                ExportDescription = (int)OkMarketExportSettings.ExportDescription,
                ExportOnShedule = OkMarketExportSettings.ExportOnShedule,
                IsExportRun = OkMarketExportState.IsRun,
                Reports = OkMarketExportState.GetReports()
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveExportSettings(OkMarketExportSettingsModel model)
        {
            OkMarketExportSettings.ExportPreorderProducts = model.ExportPreorderProducts;
            OkMarketExportSettings.ExportUnavailableProducts = model.ExportUnavailableProducts;
            OkMarketExportSettings.SizeAndColorInDescription = model.ExportSizeAndColorInDescription;
            OkMarketExportSettings.SizeAndColorInName = model.ExportSizeAndColorInName;
            OkMarketExportSettings.ExportDescription = (OkMarketShowDescriptionMode)model.ExportDescription;
            OkMarketExportSettings.ExportProperties = model.ExportProperties;
            OkMarketExportSettings.ExportLinkToSite = (OkMarketAddLinkToSiteMode)model.ExportLinkToSite;
            OkMarketExportSettings.CurrencyIso3 = model.CurrencyIso3;
            OkMarketExportSettings.UpdateProductPhotos = model.ExportUpdateProductPhotos;

            if (OkMarketExportSettings.ExportOnShedule != model.ExportOnShedule)
            {
                OkMarketExportSettings.ExportOnShedule = model.ExportOnShedule;
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Export()
        {
            if (OkMarketExportState.IsRun)
                return JsonError("Перенос товаров уже запущен");

            Task.Run(() => new OkMarketExport().StartExport());

            return JsonOk();
        }

        public JsonResult GetExportProgress()
        {
            var res = OkExportProgress.State();
            return Json(new { Total = res.Item1, Current = res.Item2, IsRun = OkMarketExportState.IsRun });
        }

        [HttpGet]
        public JsonResult GetExportState()
        {
            return Json(new
            {
                isRun = OkMarketExportState.IsRun
            });
        }

        [HttpGet]
        public JsonResult GetExportReports()
        {
            OkMarketExportState.DeleteExpiredLogs();
            return Json(new { reports = OkMarketExportState.GetReports() });
        }

        #endregion

        #region Import

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ImportProducts()
        {
            new OkMarketImport().Process();
            return JsonOk();
        }

        public JsonResult GetImportProgress()
        {
            var res = OkImportProgress.State();
            return Json(new { Total = res.Item1, Current = res.Item2, IsRun = OkMarketImportState.IsRun });
        }

        [HttpGet]
        public JsonResult GetImportReports()
        {
            OkMarketImportState.DeleteExpiredLogs();
            return Json(new { reports = OkMarketImportState.GetReports() });
        }

        [HttpGet]
        public JsonResult GetImportState()
        {
            return Json(new
            {
                isRun = OkMarketImportState.IsRun
            });
        }

        #endregion
    }
}