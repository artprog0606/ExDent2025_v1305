using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.Import;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    public partial class ImportController : BaseAdminController
    {
        private readonly string _csvProductsFileName = "importCSV.csv";
        private readonly string _csvCategoriesFileName = "importCsvCategories.csv";
        private readonly string _csvCustomersFileName = "importCsvCustomers.csv";
        private readonly string _csvLeadsFileName = "importCsvLeads.csv";
        private readonly string _csvBrandsFileName = "importCsvBrands.csv";
        private readonly string _csvOrdersFileName = "importCsvOrders.csv";

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetSaasBlockInformation()
        {
            var currentSaasData = SaasDataService.CurrentSaasData;
            var productsCount = ProductService.GetProductsCount("[Enabled] = 1");

            return Json(new
            {
                productsInTariff = currentSaasData.ProductsCount, productsCount, isSaas = SaasDataService.IsSaasEnabled
            });
        }

        [Auth(RoleAction.Catalog)]
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Catalog.Import"));
            SetNgController(NgControllers.NgControllersTypes.ImportCtrl);

            return View();
        }

        #region Import Products

        [Auth(RoleAction.Catalog)]
        public ActionResult ImportProducts()
        {
            //SetMetaInformation(T("Admin.Catalog.ImportProducts"));
            //SetNgController(NgControllers.NgControllersTypes.ImportCtrl);

            var model = new ImportProductsModel
            {
                HaveHeader = true,
                ProductActionType = EImportProductActionType.Nothing,
                OnlyUpdateProducts = false,
                ImportRemainsType = EImportRemainsType.Normal.StrName(),
                PropertySeparator = ";",
                PropertyValueSeparator = ":",
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                Encoding = EncodingsEnum.Windows1251.StrName(),
                CurrentSaasData = SaasDataService.CurrentSaasData,
                IsStartExport =
                    CommonStatistic
                        .IsRun /*&& CommonStatistic.CurrentProcess != null && CommonStatistic.CurrentProcess.Equals("import/importProducts")*/,
                StocksToWarehouseId = SettingsCatalog.DefaultWarehouse
            };

            model.SelectItemModelList = EImportProductActionType.GetValues(typeof(EImportProductActionType))
                .Cast<EImportProductActionType>()
                .Select(item => new SelectItemModel<int>
                {
                    label = item.Localize(),
                    value = (int) item
                })
                .ToList();

            return PartialView(model);
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult StartProductsImport(ImportProductsModel model)
        {
            var inputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvProductsFileName;
            return model.CsvV2
                ? ProcessJsonResult(new StartImportCsvV2Handler(model, inputFilePath))
                : ProcessJsonResult(new StartImportProductsHandler(model, inputFilePath));
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadCsvFile(HttpPostedFileBase file)
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(filePath);
            var outputFilePath = filePath + _csvProductsFileName;

            var result = new UploadCsvFileHandler(file, outputFilePath).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetFieldsFromCsvFile(ImportProductsModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvProductsFileName;
            return model.CsvV2
                ? ProcessJsonResult(new GetFieldsFromCsvV2File(model, outputFilePath))
                : ProcessJsonResult(new GetFieldsFromCsvFile(model, outputFilePath));
        }

        #endregion

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadZipFile(HttpPostedFileBase file)
        {
            var result = new UploadImagesArchiveFileHandler(file).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteZipFile()
        {
            var result = new DeleteImagesArchiveFileHandler().Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCsvFile(HttpPostedFileBase file)
        {
            return Json(new {result = true});
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadCsvFileByLink()
        {
            return Json(new {result = true});
        }

        #region Import categories

        [Auth(RoleAction.Catalog)]
        public ActionResult ImportCategories()
        {
            //SetMetaInformation(T("Admin.Catalog.ImportCategories"));
            //SetNgController(NgControllers.NgControllersTypes.ImportCtrl);

            var model = new ImportCategoriesModel
            {
                HaveHeader = true,
                Encoding = EncodingsEnum.Windows1251.StrName(),
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                CurrentSaasData = SaasDataService.CurrentSaasData,
                PropertySeparator = ";",
                NameSameProductProperty = "SameProperty",
                NameNotSameProductProperty = "NotSameProperty"
            };

            return PartialView(model);
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult StartCategoriesImport(ImportCategoriesModel model)
        {
            var inputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvCategoriesFileName;
            return ProcessJsonResult(new StartImportCategoriesHandler(model, inputFilePath));
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadCsvCategoriesFile(HttpPostedFileBase file)
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(filePath);
            var outputFilePath = filePath + _csvCategoriesFileName;

            var result = new UploadCsvFileHandler(file, outputFilePath).Execute();
            return Json(result);
        }

        [Auth(RoleAction.Catalog)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetFieldsFromCategoriesCsvFile(ImportCategoriesModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvCategoriesFileName;
            return ProcessJsonResult(new GetFieldsFromCategoriesCsvFile(model, outputFilePath));
        }

        #endregion

        #region Import customers

        [Auth(RoleAction.Customers)]
        public ActionResult ImportCustomers()
        {
            SetMetaInformation(T("Admin.Import.ImportCustomers.Title"));
            SetNgController(NgControllers.NgControllersTypes.ImportCtrl);

            var model = new GetImportCustomersModel().Execute();

            return View("~/Areas/Admin/Views/Import/ImportCustomers.cshtml", model);
        }

        [Auth(RoleAction.Customers)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetFieldsFromCustomersCsvFile(ImportCustomersModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvCustomersFileName;
            return ProcessJsonResult(new GetFieldsFromCustomersCsvFile(model, outputFilePath));
        }

        [Auth(RoleAction.Customers)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult StartCustomersImport(ImportCustomersModel model)
        {
            var inputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvCustomersFileName;
            return ProcessJsonResult(new StartImportCustomersHandler(model, inputFilePath));
        }

        [Auth(RoleAction.Customers)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadCsvCustomersFile(HttpPostedFileBase file)
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(filePath);
            var outputFilePath = filePath + _csvCustomersFileName;

            var result = new UploadCsvFileHandler(file, outputFilePath).Execute();
            return Json(result);
        }

        public FileStreamResult GetExampleCustomersFile(ImportCustomersModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + "exampleCustomersFile.csv";
            new GetExampleCustomersFileHandler(model, outputFilePath).Execute();

            var exampleFile = new FileInfo(outputFilePath);
            if (exampleFile.Exists)
            {
                return File(exampleFile.OpenRead(), "text/plain");
            }

            return null;
        }

        #endregion

        #region Import leads

        [Auth(RoleAction.Crm)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetFieldsFromLeadsCsvFile(ImportLeadsModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvLeadsFileName;
            return ProcessJsonResult(new GetFieldsFromLeadsCsvFile(model, outputFilePath));
        }

        [Auth(RoleAction.Crm)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult StartLeadsImport(ImportLeadsModel model)
        {
            var inputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvLeadsFileName;
            return ProcessJsonResult(new StartImportLeadsHandler(model, inputFilePath));
        }

        [Auth(RoleAction.Crm)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadCsvLeadsFile(HttpPostedFileBase file)
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(filePath);
            var outputFilePath = filePath + _csvLeadsFileName;

            var result = new UploadCsvFileHandler(file, outputFilePath).Execute();
            return Json(result);
        }

        public FileStreamResult GetExampleLeadsFile(ImportLeadsModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + "exampleLeadsFile.csv";
            new GetExampleLeadsFileHandler(model, outputFilePath).Execute();

            var exampleFile = new FileInfo(outputFilePath);
            return exampleFile.Exists ? File(exampleFile.OpenRead(), "text/plain") : null;
        }

        #endregion

        #region Import brands

        [Auth(RoleAction.Settings)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetFieldsFromBrandsCsvFile(ImportBrandsModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvBrandsFileName;
            return ProcessJsonResult(new GetFieldsFromBrandsCsvFile(model, outputFilePath));
        }

        [Auth(RoleAction.Settings)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult StartBrandsImport(ImportBrandsModel model)
        {
            var inputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvBrandsFileName;
            return ProcessJsonResult(new StartImportBrandsHandler(model, inputFilePath));
        }

        [Auth(RoleAction.Settings)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadCsvBrandsFile(HttpPostedFileBase file)
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(filePath);
            var outputFilePath = filePath + _csvBrandsFileName;

            var result = new UploadCsvFileHandler(file, outputFilePath).Execute();
            return Json(result);
        }

        public FileStreamResult GetExampleBrandsFile(ImportBrandsModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + "exampleBrandsFile.csv";
            new GetExampleBrandsFileHandler(model, outputFilePath).Execute();

            var exampleFile = new FileInfo(outputFilePath);

            return exampleFile.Exists ? File(exampleFile.OpenRead(), "text/plain") : null;
        }

        #endregion Import brands

        #region Import orders

        [Auth(RoleAction.Orders)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetFieldsFromOrdersCsvFile(ImportOrdersModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvOrdersFileName;
            return ProcessJsonResult(new GetFieldsFromOrdersCsvFile(model, outputFilePath));
        }

        [Auth(RoleAction.Crm)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult StartOrdersImport(ImportOrdersModel model)
        {
            var inputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + _csvOrdersFileName;
            return ProcessJsonResult(new StartImportOrdersHandler(model, inputFilePath));
        }

        [Auth(RoleAction.Crm)]
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadCsvOrdersFile(HttpPostedFileBase file)
        {
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(filePath);
            var outputFilePath = filePath + _csvOrdersFileName;

            var result = new UploadCsvFileHandler(file, outputFilePath).Execute();
            return Json(result);
        }

        public FileStreamResult GetExampleOrdersFile(ImportOrdersModel model)
        {
            var outputFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp) + "exampleOrdersFile.csv";
            new GetExampleOrdersFileHandler(model, outputFilePath).Execute();

            var exampleFile = new FileInfo(outputFilePath);
            return exampleFile.Exists ? File(exampleFile.OpenRead(), "text/plain") : null;
        }

        #endregion
    }

    public partial class ImportCrmController : ImportController
    {
    }
}
