using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.ExportCategories;
using AdvantShop.Web.Admin.Models.Catalog.ExportCategories;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public partial class ExportCategoriesController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetNgController(NgControllers.NgControllersTypes.ExportCategoriesCtrl);

            var model = new GetExportCategoriesHandler().Execute();

            return View(model);
        }
        public ActionResult _Index()
        {
            var model = new GetExportCategoriesHandler().Execute();

            return PartialView("Index", model);
        }

        public ActionResult Export()
        {
            SetNgController(NgControllers.NgControllersTypes.ExportCategoriesCtrl);
            SetMetaInformation(T("Admin.ExportCategories.CategoriesExport"));

            if (CommonStatistic.IsRun)
                return View("ExportCategoriesProgress");

            CommonStatistic.StartNew(() =>
                {
                    var filePath = new StartingExportCategoriesHandler().Execute(useCommonStatistic: true);
                    CommonStatistic.FileName = "../" + filePath;
                },
                "exportcategories/export",
                T("Admin.ExportCategories.CategoriesExport"));

            return View("ExportCategoriesProgress");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveExportCategoriesSettings(ExportCategoriesSettingsModel model)
        {
            var result = new SaveExportCategoriesFieldsHandler(model).Execute();

            return result ? JsonOk() : JsonError(T("Admin.Catalog.ErrorSavingSettings"));
        }
    }
}
