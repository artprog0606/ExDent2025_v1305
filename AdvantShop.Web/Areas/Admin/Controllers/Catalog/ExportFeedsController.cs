using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Saas;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog, RoleAction.Yandex, RoleAction.Google)]
    public partial class ExportFeedsController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new GetExportFeedsHandler(EExportFeedType.Csv, EExportFeedType.CsvV2).Execute();

            SetMetaInformation(T("Admin.Catalog.ProductsExport"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);

            return View(model);
        }

        public ActionResult _Index()
        {
            var model = new GetExportFeedsHandler(EExportFeedType.Csv, EExportFeedType.CsvV2, EExportFeedType.YandexMarket, EExportFeedType.YandexDirect, EExportFeedType.YandexWebmaster).Execute();

            return PartialView("Index", model);
        }

        public ActionResult IndexCsv()
        {
            SetMetaInformation(T("Admin.Catalog.Export"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);
            return View();
        }

        public ActionResult ExportFeed(int id)
        {
            SetMetaInformation(T("Admin.Catalog.ProductsExport"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);

            var model = new GetExportFeedModelHandler(id).Execute();
            if (model == null)
                return Error404();

            return View("ExportFeed", model);
        }

        #region Tabs

        #region ChoiceOfProducts

        public JsonResult CategoriesTree(CategoriesTree model, int exportFeedId)
        {
            return Json(new GetCategoriesTree(model, exportFeedId).Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCategoriesToExport(int exportFeedId, List<ExportFeedSelectedCategory> categories)
        {
            ExportFeedService.InsertCategories(exportFeedId, categories);
            return Json(new { result = true, reloadCatalogTree = true });
        }

        public JsonResult GetCatalog(ExportFeedCatalogFilterModel model)
        {
            return Json(new GetCatalog(model).Execute());
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult ExcludeProducts(ExportFeedCatalogFilterModel command)
        {
            Command(command, (id, c) => {
                ExportFeedService.AddExcludeProduct(command.ExportFeedId, id);
            });
            return JsonOk();
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult ExcludeListProducts(int exportId, List<string> listArtNo)
        {
            var notAddedArtNo = new List<string>();
			foreach (var artNo in listArtNo)
			{
                int id = ProductService.GetProductId(artNo);
                if(id == 0)
				{
                    notAddedArtNo.Add(artNo);
                    continue;
                }
                ExportFeedService.AddExcludeProduct(exportId, id);
			}
            return JsonOk(notAddedArtNo);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult IncludeListProducts(int exportId, List<string> listArtNo)
        {
            var notAddedArtNo = new List<string>();
            foreach (var artNo in listArtNo)
            {
                int id = ProductService.GetProductId(artNo);
                if (id == 0)
                {
                    notAddedArtNo.Add(artNo);
                    continue;
                }
                ExportFeedService.DeleteExcludeProduct(exportId, id);
            }
            return JsonOk(notAddedArtNo);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceExcludedProducts(ExportFeedCatalogProductModel model)
        {
            if (model.ExcludeFromExport)
            {
                ExportFeedService.AddExcludeProduct(model.ExportFeedId, model.ProductId);
            }
            else
            {
                ExportFeedService.DeleteExcludeProduct(model.ExportFeedId, model.ProductId);
            }
            return JsonOk();
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult IncludeProducts(ExportFeedCatalogFilterModel command)
        {
            Command(command, (id, c) => {
                ExportFeedService.DeleteExcludeProduct(command.ExportFeedId, id);
            });
            return JsonOk();
        }

        #endregion ChoiceOfProducts

        #region ChoiceOfFields

        [ChildActionOnly]
        public ActionResult ChoiceOfFields(int exportFeedId, EExportFeedType exportFeedType)
        {
            var handler = new GetExportFeedFields(exportFeedId, exportFeedType);
            var objectAdvancedSettings = handler.Execute();
            return PartialView("ChoiceOfFields", objectAdvancedSettings);
        }

        [ChildActionOnly]
        public ActionResult ChoiceOfCsvFieldsV2(int exportFeedId, object advancedSettings)
        {
            return PartialView(new GetExportFeedCsvFieldsV2(exportFeedId, (ExportFeedCsvV2Options)advancedSettings).Execute());
        }

        #endregion ChoiceOfFields

        #region Settings/AdvancedSettings

        [ChildActionOnly]
        public ActionResult GetAdvansedSettings(ExportFeedModel exportFeed)
        {
            return PartialView("ExportFeedSettings" + exportFeed.Type.ToString(), exportFeed.ExportFeedSettings.AdvancedSettingsModel);
        }

        public JsonResult AddGlobalDeliveryCosts(ExportFeedYandexDeliveryCostOption model)
        {
            return JsonOk();
        }

        #endregion Settings/AdvancedSettings

        #endregion Tabs

        #region Basic

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetAvalableTypes(EExportFeedType? type)
        {
            if (!type.HasValue || type.Value == EExportFeedType.None)
                return JsonError();

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveExportFeeds && (type == EExportFeedType.GoogleMerchentCenter))
                return JsonError("access denied");

            var types = new List<SelectItemModel> { new SelectItemModel(type.Value.Localize(), type.Value.ToString()) };
            if (type == EExportFeedType.Csv)
            {
                types.Insert(0, new SelectItemModel(EExportFeedType.CsvV2.Localize(), EExportFeedType.CsvV2.ToString()));
                if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveExportFeeds)
                {
                    types.Add(new SelectItemModel(EExportFeedType.YandexMarket.Localize(), EExportFeedType.YandexMarket.ToString()));
                    types.Add(new SelectItemModel(EExportFeedType.YandexDirect.Localize(), EExportFeedType.YandexDirect.ToString()));
                    types.Add(new SelectItemModel(EExportFeedType.YandexWebmaster.Localize(), EExportFeedType.YandexWebmaster.ToString()));
                }
            }

            return JsonOk(types);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(string name, string description, EExportFeedType type)
        {
            return ProcessJsonResult(new AddExportFeed(name, description, type));
        }

        public JsonResult SaveExportFeedSettings(int exportFeedId, string exportFeedName, string exportFeedDescription, ExportFeedSettingsModel commonSettings, string advancedSettings)
        {
            return Json(new SaveExportFeedSettingsHandler(exportFeedId, exportFeedName, exportFeedDescription, commonSettings, advancedSettings).Execute());
        }

        public JsonResult SaveExportFeedFields(int exportFeedId, List<string> exportFeedFields)
        {
            return ProcessJsonResult(new SaveExportFeedFields(exportFeedId, exportFeedFields));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteExport(int exportFeedId)
        {
            ExportFeedService.DeleteExportFeed(exportFeedId);
            return JsonOk();
        }

        public JsonResult DeleteExportFile(int exportFeedId, string fileFullName)
        {
            ExportFeedService.DeleteFileExportFeed(exportFeedId, fileFullName);
            return JsonOk();
        }

        public ActionResult Export(int? id)
        {
            var exportFeedModel = id.HasValue
                ? new GetExportFeedModelHandler(id.Value).Execute()
                : null;
            if (exportFeedModel == null)
                return RedirectToAction("Index");

            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);
            SetMetaInformation(exportFeedModel.Name);

            var model = new ExportFeedExportModel { ExportFeed = exportFeedModel };

            if (CommonStatistic.IsRun)
            {
                model.IsAlreadyRunning = true;
                return View("ExportFeedProgress", model);
            }

            var actionName = "export" + (ExportFeedService.AllowShowExportFeedInCatalog(exportFeedModel.Type)
                ? string.Empty
                : exportFeedModel.Type.StrName());
            
            CommonStatistic.StartNew(() =>
                {
                    var filePath = new StartingExportHandler(exportFeedModel.Id).Execute();
                    CommonStatistic.FileName = "../" + filePath;
                },
                $"exportfeeds/{actionName}/{exportFeedModel.Id}",
                exportFeedModel.Name);

            return View("ExportFeedProgress", model);
        }

        #endregion Basic

        #region Command

        private void Command(ExportFeedCatalogFilterModel command, Action<int, ExportFeedCatalogFilterModel> func)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            if (command.SelectMode == SelectModeCommand.None)
            {
                Parallel.ForEach(command.Ids, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (id) => {
                    try
                    {
                        func(id, command);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                });
            }
            else
            {
                var ids = new GetCatalog(command).GetItemsIds<int>("[Product].[ProductID]");

                Parallel.ForEach(ids, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (id) => {
                    try
                    {
                        if (command.Ids == null || !command.Ids.Contains(id))
                            func(id, command);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                });
            }

            if (exceptions.Any())
            {
                Debug.Log.Error(exceptions.AggregateString("<br/>^^^<br/>"));
            }
        }

        #endregion Command

        #region YandexMarket

        [SaasFeature(ESaasProperty.HaveExportFeeds)]
        [Auth(RoleAction.Yandex)]
        [SalesChannel(ESalesChannelType.Yandex)]
        public ActionResult IndexYandex()
        {
            SetMetaInformation(T("Admin.Catalog.UnloadingInYandexMarket"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);

            var model = new GetExportFeedsHandler(EExportFeedType.YandexMarket).Execute();
            if (model.ExportFeeds.Count == 0)
                return View("Preview", model);

            return View("Index", model);
        }

        public ActionResult ExportFeedYandex(int id)
        {
            return ExportFeed(id);
        }

        public ActionResult ExportYandex(int? id)
        {
            return Export(id);
        }

        [ChildActionOnly]
        [Auth(RoleAction.Yandex)]
        public ActionResult YandexPromoCode(ExportFeedModel exportFeed)
        {
            return PartialView($"ExportFeedSettings{exportFeed.Type}PromoCode", exportFeed.ExportFeedSettings.AdvancedSettingsModel);
        }

        [ChildActionOnly]
        [Auth(RoleAction.Yandex)]
        public ActionResult YandexPromoFlash(ExportFeedModel exportFeed)
        {
            return PartialView($"ExportFeedSettings{exportFeed.Type}PromoFlash", exportFeed.ExportFeedSettings.AdvancedSettingsModel);
        }

        [ChildActionOnly]
        [Auth(RoleAction.Yandex)]
        public ActionResult YandexPromoGift(ExportFeedModel exportFeed)
        {
            return PartialView($"ExportFeedSettings{exportFeed.Type}PromoGift", exportFeed.ExportFeedSettings.AdvancedSettingsModel);
        }

        [ChildActionOnly]
        [Auth(RoleAction.Yandex)]
        public ActionResult YandexPromoNPlusM(ExportFeedModel exportFeed)
        {
            return PartialView($"ExportFeedSettings{exportFeed.Type}PromoNPlusM", exportFeed.ExportFeedSettings.AdvancedSettingsModel);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        [Auth(RoleAction.Yandex)]
        public JsonResult GetYandexPromo(string promoId, int exportFeedId)
        {
            var guid = promoId.TryParseGuid();
            var advancedSettings = ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedYandexOptions>(exportFeedId);

            var promos = JsonConvert.DeserializeObject<List<ExportFeedYandexPromo>>(advancedSettings.Promos);
            var promo = promos?.FirstOrDefault(x => x.PromoID == guid);
            return Json(promo);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        [Auth(RoleAction.Yandex)]
        public JsonResult VerifyYandexPromo(int exportFeedId, ExportFeedSettingsYandexPromoModel model, bool editing = false)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var editedPromo = new ExportFeedYandexPromo();
                    editedPromo.PromoID = Guid.NewGuid();
                    if (editing)
                    {
                        editedPromo.PromoID = model.PromoID ?? editedPromo.PromoID;
                    }
                    editedPromo.Type = model.Type.TryParseEnum<YandexPromoType>();
                    editedPromo.Name = model.Name.Trim();
                    editedPromo.Description = model.Description;
                    editedPromo.PromoUrl = model.PromoUrl;
                    editedPromo.StartDate = model.StartDate;
                    editedPromo.ExpirationDate = model.ExpirationDate;
                    editedPromo.RequiredQuantity = model.RequiredQuantity;
                    editedPromo.FreeQuantity = model.FreeQuantity;
                    editedPromo.ProductIDs = model.ProductIDs;
                    editedPromo.RequiredQuantity = model.RequiredQuantity;
                    editedPromo.GiftID = model.GiftID;
                    editedPromo.CouponId = model.CouponId;
                    editedPromo.CategoryIDs = model.CategoryIDs;

                    var advancedSettings = ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedYandexOptions>(exportFeedId);
                    if (advancedSettings?.Promos == null)
                    {
                        return Json(new { result = true, promo = editedPromo });
                    }

                    var promos = JsonConvert.DeserializeObject<List<ExportFeedYandexPromo>>(advancedSettings.Promos);
                    if (promos == null)
                    {
                        return Json(new { result = true, promo = editedPromo });
                    }

                    var oldPromo = promos.FirstOrDefault(x => x.PromoID == editedPromo.PromoID);

                    if (editedPromo.Type == YandexPromoType.PromoCode)
                    {
                        if (promos.FirstOrDefault(x => x.Type == YandexPromoType.PromoCode && x.CouponId == editedPromo.CouponId) != null)
                        {
                            if (oldPromo == null || editedPromo.CouponId != oldPromo.CouponId)
                            {
                                return Json(new { result = false, errors = "Такой купон уже добавлен в качестве промоакции!" });
                            }
                        }
                    }

                    return Json(new { result = true, promo = editedPromo });
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex.Message, ex);
                }
            }

            var errors = ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors)
                .Aggregate("", (current, error) => $"{current} {error.ErrorMessage}<br />");
            return Json(new { result = false, errors = errors });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        [Auth(RoleAction.Yandex)]
        public JsonResult DeleteYandexPromo(string promoId, int exportFeedId)
        {
            var guid = promoId.TryParseGuid();
            var advancedSettings = ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedYandexOptions>(exportFeedId);

            var promos = JsonConvert.DeserializeObject<List<ExportFeedYandexPromo>>(advancedSettings.Promos);
            var promo = promos?.FirstOrDefault(x => x.PromoID == guid);
            if (promo != null)
            {
                promos.Remove(promo);
                advancedSettings.Promos = JsonConvert.SerializeObject(promos);
                ExportFeedSettingsProvider.SetAdvancedSettings(exportFeedId, JsonConvert.SerializeObject(advancedSettings));
            }

            return Json(new { result = true });
        }

        [HttpGet]
        [Auth(RoleAction.Yandex)]
        public JsonResult GetExportOnlyUseInDetailsPropertiesExceptions(int exportFeedId)
        {
            return ProcessJsonResult(new GetExportOnlyUseInDetailsPropertiesExceptions(exportFeedId));
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        [Auth(RoleAction.Yandex)]
        public JsonResult SaveExportOnlyUseInDetailsPropertiesExceptions(int exportFeedId, List<SelectItemModel<int>> exceptionProperties)
        {
            return ProcessJsonResult(new SaveExportOnlyUseInDetailsPropertiesExceptions(exportFeedId, exceptionProperties));
        }

        #endregion YandexMarket

        #region Google Merchant Center

        [SaasFeature(ESaasProperty.HaveExportFeeds)]
        [Auth(RoleAction.Google)]
        [SalesChannel(ESalesChannelType.Google)]
        public ActionResult IndexGoogle()
        {
            SetMetaInformation(T("Admin.Catalog.UnloadingToGoogleMerchantCenter"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);

            return View("index", new GetExportFeedsHandler(EExportFeedType.GoogleMerchentCenter).Execute());
        }

        public ActionResult ExportFeedGoogle(int id)
        {
            return ExportFeed(id);
        }

        public ActionResult ExportGoogle(int? id)
        {
            return Export(id);
        }

        #endregion Google Merchant Center


        #region Facebook

        [SaasFeature(ESaasProperty.HaveExportFeeds)]
        [Auth(RoleAction.FacebookFeed)]
        [SalesChannel(ESalesChannelType.FacebookFeed)]
        public ActionResult IndexFacebook()
        {
            SetMetaInformation(T("Admin.Catalog.UnloadingToFacebook"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);

            return View("index", new GetExportFeedsHandler(EExportFeedType.Facebook).Execute());
        }

        public ActionResult ExportFeedFacebook(int id)
        {
            return ExportFeed(id);
        }

        public ActionResult ExportFacebook(int? id)
        {
            return Export(id);
        }

        #endregion Facebook

        #region Avito

        [SaasFeature(ESaasProperty.HaveExportFeeds)]
        [Auth(RoleAction.Avito)]
        [SalesChannel(ESalesChannelType.Avito)]
        public ActionResult IndexAvito()
        {
            SetMetaInformation(T("Admin.Catalog.UnloadingToAvito"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);

            return View("index", new GetExportFeedsHandler(EExportFeedType.Avito).Execute());
        }

        public ActionResult ExportFeedAvito(int id)
        {
            return ExportFeed(id);
        }

        public ActionResult ExportAvito(int? id)
        {
            return Export(id);
        }

        #endregion Avito

        #region Reseller

        [Auth(RoleAction.Reseller)]
        [SalesChannel(ESalesChannelType.Reseller)]
        public ActionResult IndexReseller()
        {
            SetMetaInformation(T("Admin.Catalog.ExportFeeds.ResellerTitle"));
            SetNgController(NgControllers.NgControllersTypes.ExportFeedsCtrl);

            return View("index", new GetExportFeedsHandler(EExportFeedType.Reseller).Execute());
        }

        public ActionResult ExportFeedReseller(int id)
        {
            return ExportFeed(id);
        }

        public ActionResult ExportReseller(int? id)
        {
            return Export(id);
        }

        #endregion Reseller

        [HttpGet]
        public JsonResult GetWarehouses()
        {
            return Json(WarehouseService.GetList().Select(x => new { x.Id, x.Name }).ToList());
        }
    }
}
