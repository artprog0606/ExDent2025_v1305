using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Crm.Vk.VkMarket;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Export;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Import;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.VkMarkets;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Crm.Vk;
using AdvantShop.Web.Admin.Models.VkMarkets;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Vk)]
    public partial class VkMarketController : BaseAdminController
    {
        #region Ctor

        private readonly VkApiService _vkService;

        public VkMarketController()
        {
            _vkService = new VkApiService();
        }

        #endregion

        public ActionResult Index()
        {
            SetMetaInformation("ВКонтакте");
            SetNgController(NgControllers.NgControllersTypes.VkMarketExportCtrl);

            return View();
        }

        #region Categories

        [HttpGet]
        public JsonResult GetCategories()
        {
            return Json(new GetVkCategories().Execute());
        }

        [HttpGet]
        public JsonResult GetCategory(int id)
        {
            var category = new VkCategoryService().Get(id);
            return Json(category == null ? null : new VkCategoryModel(category));
        }
        
        [HttpGet]
        public JsonResult FilterMarketCategories(string q, long? categoryId)
        {
            try
            {
                var cats = new VkMarketApiService().FilterMarketCategories(q, categoryId);
                return JsonOk(cats?.Select(x => new FilterMarketCategory(x)));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategory(VkCategoryModel category)
        {
            return ProcessJsonResult(new AddUpdateVkCategory(category));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCategory(VkCategoryModel category)
        {
            return ProcessJsonResult(new AddUpdateVkCategory(category));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(VkCategoryModel category)
        {
            return ProcessJsonResult(new AddUpdateVkCategory(category));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategory(int id, long vkId)
        {
            return ProcessJsonResult(() => new VkCategoryService().Delete(id, vkId));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCategories(BaseFilterModel model)
        {
            var categoryService = new VkCategoryService();

            Command(model, (id, c) =>
            {
                var cat = categoryService.Get(id);
                if (cat != null)
                    categoryService.Delete(cat.Id, cat.VkId);
            });
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteAllProducts()
        {
            new VkProductService().DeleteAllProducts();
            return JsonOk();
        }

        #region Command

        private void Command(BaseFilterModel model, Action<int, BaseFilterModel> func)
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
                var ids = new GetVkCategories().Execute().DataItems.Select(x => x.Id);
                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        #endregion

        #endregion
        
        #region Settings

        [HttpGet]
        public JsonResult GetExportSettings()
        {
            VkMarketExportState.DeleteExpiredLogs();

            return Json(new VkMarketExportSettingsModel()
            {
                ExportUnavailableProducts = VkMarketExportSettings.ExportUnavailableProducts,
                ExportPreorderProducts = VkMarketExportSettings.ExportPreorderProducts,
                AddSizeAndColorInDescription = VkMarketExportSettings.AddSizeAndColorInDescription,
                AddSizeAndColorInName = VkMarketExportSettings.AddSizeAndColorInName,
                ShowDescription = (int) VkMarketExportSettings.ShowDescription,
                AddLinkToSite = (int) VkMarketExportSettings.AddLinkToSite,
                TextBeforeLinkToSite = VkMarketExportSettings.TextBeforeLinkToSite ?? "Подробное описание на сайте: ",
                Group = SettingsVk.Group,
                CurrencyIso3 = VkMarketSettings.CurrencyIso3 ?? CurrencyService.CurrentCurrency.Iso3,
                ExportOnShedule = VkMarketExportSettings.ExportOnShedule,
                ShowProperties = VkMarketExportSettings.ShowProperties,
                IsExportRun = VkMarketExportState.IsRun,
                Reports = 
                    CustomerContext.CurrentCustomer.IsVirtual || Request.IsLocal 
                        ? VkMarketExportState.GetReports() 
                        : null,
                ConsiderMinimalAmount = VkMarketExportSettings.ConsiderMinimalAmount,
                ExportMode = (int) VkMarketExportSettings.ExportMode,
                ExportAmount = (int) VkMarketExportSettings.ExportAmount,
                ExportAmounts = new List<SelectItemModel<int>>()
                {
                    new SelectItemModel<int>("Да", (int) VkExportAmount.Yes),
                    new SelectItemModel<int>("Нет", (int) VkExportAmount.No)
                }
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveExportSettings(VkMarketExportSettingsModel model)
        {
            VkMarketExportSettings.ExportUnavailableProducts = model.ExportUnavailableProducts;
            VkMarketExportSettings.ExportPreorderProducts = model.ExportPreorderProducts;
            VkMarketExportSettings.AddSizeAndColorInDescription = model.AddSizeAndColorInDescription;
            VkMarketExportSettings.AddSizeAndColorInName = model.AddSizeAndColorInName;
            VkMarketExportSettings.ShowDescription = (ShowDescriptionMode)model.ShowDescription;
            VkMarketExportSettings.ShowProperties = model.ShowProperties;
            VkMarketExportSettings.AddLinkToSite = (AddLinkToSiteMode)model.AddLinkToSite;
            VkMarketExportSettings.TextBeforeLinkToSite = model.TextBeforeLinkToSite.DefaultOrEmpty();
            VkMarketSettings.CurrencyIso3 = model.CurrencyIso3;
            VkMarketExportSettings.ConsiderMinimalAmount = model.ConsiderMinimalAmount;
            VkMarketExportSettings.ExportMode = (VkExportMode)model.ExportMode;
            VkMarketExportSettings.ExportAmount = (VkExportAmount)model.ExportAmount;

            if (VkMarketExportSettings.ExportOnShedule != model.ExportOnShedule)
            {
                VkMarketExportSettings.ExportOnShedule = model.ExportOnShedule;
            }

            return JsonOk();
        }

        [HttpGet]
        public JsonResult GetReports()
        {
            VkMarketExportState.DeleteExpiredLogs();
            
            return Json(new
            {
                reports = CustomerContext.CurrentCustomer.IsVirtual || Request.IsLocal
                    ? VkMarketExportState.GetReports()
                    : null
            });
        }

        #endregion

        #region Export 

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Export()
        {
            if (VkMarketExportState.IsRun)
                return JsonError("Экспорт товаров уже запущен");

            Task.Run(() => new VkMarketExportService().StartExport());

            return JsonOk();
        }

        public JsonResult GetExportProgress()
        {
            var res = VkProgress.State();
            return Json(new { Total = res.Item1, Current = res.Item2, Error = res.Item3 });
        }

        #endregion

        #region Import
        
        public ActionResult Import()
        {
            SetMetaInformation("ВКонтакте");
            return View();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ImportProducts(bool createImVkRelations)
        {
            new VkMarketImportServiceWithVariants(createImVkRelations).Import();
            return JsonOk();
        }

        public JsonResult GetImportProgress()
        {
            var res = VkProgress.State();
            return Json(new { Total = res.Item1, Current = res.Item2 });
        }

        #endregion
    }
}
