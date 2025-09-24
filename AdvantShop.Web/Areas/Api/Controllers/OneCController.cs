using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.OneC;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Statistic;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Controllers
{
    public class OneCController : BaseApiController
    {
        /// GET: api/1c/init
        public ActionResult Init(string login, string password)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            if (String.IsNullOrWhiteSpace(login) || String.IsNullOrWhiteSpace(password))
                return Content("error auth");

            var customer = CustomerService.GetCustomerByEmailAndPassword(login, password, false);

            if (customer == null || (!customer.IsAdmin && !customer.IsModerator))
                return Content("error auth");

            var apikey = SettingsApi.ApiKey;

            var doc = new XDocument(
                new XElement("settings",
                    new XElement("check_apikey_url", Url.AbsoluteRouteUrl("Api_1C", new { action = "chekapikey", apikey })),
                    new XElement("import_products_url",
                        Url.AbsoluteRouteUrl("Api_1C", new { action = "importproducts", apikey })),
                    new XElement("import_photos_url",
                        Url.AbsoluteRouteUrl("Api_1C", new { action = "importphotos", apikey })),
                    new XElement("export_products_url",
                        Url.AbsoluteRouteUrl("Api_1C", new { action = "exportproducts", apikey })),
                    new XElement("export_deletedproducts_url",
                        Url.AbsoluteRouteUrl("Api_1C", new { action = "deletedproducts", apikey })),
                    new XElement("export_orders_url",
                        Url.AbsoluteRouteUrl("Api_1C", new { action = "exportorders", apikey })),
                    new XElement("export_deletedorders_url",
                        Url.AbsoluteRouteUrl("Api_1C", new { action = "deletedorders", apikey })),
                    new XElement("change_orderstatus_url",
                        Url.AbsoluteRouteUrl("Api_1C", new { action = "changeorderstatus", apikey }))
                    )
                );

            var xml = doc.ToString();
            return Content(xml);
        }


        // GET: api/1c/importphotos
        [LogRequest, AuthApiKey, OneC]
        public ActionResult ImportPhotos(string apikey)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            if (Request.Files.Count == 0)
                return Json(new ApiError("Missing .zip file"));

            try
            {
                var oneCFolder = FoldersHelper.GetPathAbsolut(FolderType.OneCTemp);
                var zipfile = oneCFolder + "import.zip";
                var importDirectory = oneCFolder + "import/";

                FileHelpers.CreateDirectory(oneCFolder);
                FileHelpers.CreateDirectory(importDirectory);

                foreach (string fileName in Request.Files)
                {
                    var postedFile = Request.Files[fileName];
                    if (postedFile == null)
                        break;

                    postedFile.SaveAs(zipfile);
                    FileHelpers.UnZipFile(zipfile, importDirectory);
                    FileHelpers.DeleteFile(zipfile);

                    foreach (var file in Directory.GetFiles(importDirectory))
                    {
                        var fileUpload = file.Replace("1c_temp/import", "upload_images");

                        System.IO.File.Delete(fileUpload);
                        System.IO.File.Move(file, fileUpload);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Json(new ApiError("Error: " + ex.Message));
            }

            return Json(new ApiResponse());
        }


        // GET: api/1c/importproducts
        [LogRequest, AuthApiKey, OneC]
        public ActionResult ImportProducts(string apikey, bool updatePhotos = false)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            if (Request.Files.Count == 0 || (Request.Files[0] != null && !Request.Files[0].FileName.EndsWith(".csv")))
            {
                return Json(new ApiError("Missing .csv file"));
            }

            try
            {
                var oneCFolder = FoldersHelper.GetPathAbsolut(FolderType.OneCTemp);
                var importDirectory = oneCFolder + "import/";

                FileHelpers.CreateDirectory(oneCFolder);
                FileHelpers.CreateDirectory(importDirectory);

                foreach (string fileName in Request.Files)
                {
                    var postedFile = Request.Files[fileName];
                    if (postedFile == null)
                        break;

                    var importCsvFile = importDirectory + postedFile.FileName;

                    postedFile.SaveAs(importCsvFile);

                    if (CommonStatistic.IsRun) return Json(new ApiError("It is not possible to products import, another process is being performed"));

                    if (System.IO.File.Exists(CommonStatistic.FileLog))
                        System.IO.File.WriteAllText(CommonStatistic.FileLog, String.Empty);

                    CsvImport.Factory(importCsvFile, true, EImportProductActionType.Nothing, ";", "UTF-8", null, "&&", ":", true, false, updatePhotos, trackChanges:false, modifiedBy:"1c import", stocksToWarehouseId: Settings1C.StocksToWarehouse)
                        .ProcessThroughACommonStatistic("1c import products", "1c import products").Wait();

                    FileHelpers.DeleteFile(importCsvFile);

                    return Json(new ApiResponse() { Status = ApiStatus.Ok, Errors = CommonStatistic.ReadLog() });
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                return Json(new ApiError("Error: " + ex.Message));
            }

            return Json(new ApiError("Error"));
        }


        // GET: api/1c/exportproducts
        [LogRequest, AuthApiKey, OneC]
        public ActionResult ExportProducts(string apikey, string from, string to)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            var productFields = new List<ProductFields>()
            {
                ProductFields.Sku,
                ProductFields.Name,
                ProductFields.ParamSynonym,
                ProductFields.Category,
                ProductFields.Sorting,
                ProductFields.Enabled,
                ProductFields.Price,
                ProductFields.PurchasePrice,
                ProductFields.Amount,
                ProductFields.MultiOffer,
                ProductFields.Unit,
                ProductFields.Discount,
                ProductFields.ShippingPrice,
                ProductFields.Weight,
                ProductFields.Size,
                ProductFields.BriefDescription,
                ProductFields.Description,
                ProductFields.Title,
                ProductFields.MetaKeywords,
                ProductFields.MetaDescription,
                ProductFields.H1,
                ProductFields.Photos,
                ProductFields.Videos,
                ProductFields.Markers,
                ProductFields.Properties,
                ProductFields.Producer,
                ProductFields.OrderByRequest,
                ProductFields.SalesNotes,
                ProductFields.Related,
                ProductFields.Alternative,
                ProductFields.CustomOption,
                ProductFields.Gtin,
                ProductFields.GoogleProductCategory,
                ProductFields.Adult,
                ProductFields.BarCode
            };

            const string strFileNamePrx = "products";
            const string strFileExt = "csv";

            try
            {
                var strFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                FileHelpers.CreateDirectory(strFilePath);

                foreach (var file in Directory.GetFiles(strFilePath).Where(f => f.Contains(strFileNamePrx)).ToList())
                {
                    FileHelpers.DeleteFile(file);
                }

                var strFileName = strFileNamePrx.FileNamePlusDate();

                var fromDate = DateTime.MinValue;
                var toDate = DateTime.MinValue;
                if (!String.IsNullOrWhiteSpace(from) && !String.IsNullOrWhiteSpace(to))
                {
                    if (DateTime.TryParseExact(from, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) &&
                        DateTime.TryParseExact(to, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                    {
                        toDate = toDate.AddDays(1);
                    }
                }

                var commonSettings = new ExportFeedSettings<ExportFeedCsvOptions>
                {
                    FileName = FoldersHelper.PhotoFoldersPath[FolderType.PriceTemp] + strFileName,
                    FileExtention = strFileExt,
                    AdvancedSettings = new ExportFeedCsvOptions
                    {
                        CsvEnconing = EncodingsEnum.Windows1251.StrName(),
                        CsvSeparator = SeparatorsEnum.SemicolonSeparated.ToString(),
                        CsvColumSeparator = "&&",
                        CsvPropertySeparator = ":",
                        FieldMapping = productFields,
                        CsvExportNoInCategory = true,
                        StocksFromWarehouses = new List<int>{Settings1C.StocksToWarehouse}
                    }
                };

                var products = OneCService.GetProducts(fromDate, toDate, !Settings1C.SendAllProducts, commonSettings);
                CsvExport.Factory(products, commonSettings, 0).Process();

                CommonHelper.WriteResponseFile(commonSettings.FileFullPath, strFileName + "." + strFileExt);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return Json(new ApiError("Error: " + ex.Message));
            }

            return Content("");
        }


        // GET: api/1c/exportorders
        [LogRequest, AuthApiKey, OneC]        
        public ActionResult ExportOrders(string apikey, string from, string to)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            try
            {
                List<Order> orders = null;

                if (!String.IsNullOrWhiteSpace(from) && !String.IsNullOrWhiteSpace(to))
                {
                    DateTime fromDate;
                    DateTime toDate;

                    if (DateTime.TryParseExact(from, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) &&
                        DateTime.TryParseExact(to, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                    {
                        orders = OrderService.GetOrdersFor1C(fromDate, toDate.AddDays(1), Settings1C.OnlyUseIn1COrders);
                    }
                }
                else
                {
                    orders = OrderService.GetOrdersFor1C(new DateTime(2000, 1, 1), DateTime.Now, Settings1C.OnlyUseIn1COrders);
                }

                if (orders == null)
                    return Content("");

                var result = OrderService.SerializeToXml(orders, true);

                return Content(result, "text/xml", Encoding.Unicode);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                return Content("Error: " + ex.Message);
            }
        }


        // GET: api/1c/changeorderstatus
        [LogRequest, OneC]
        public ActionResult ChangeOrderStatus(string apikey, string packageid)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            if (!Settings1C.Enabled || string.IsNullOrWhiteSpace(apikey) || string.IsNullOrWhiteSpace(SettingsApi.ApiKey) ||
            apikey != SettingsApi.ApiKey)
            {
                return Json(new ApiError("Check apikey"));
            }

            if (string.IsNullOrWhiteSpace(packageid))
                return Json(new ApiError("Wrong packageid"));

            try
            {
                var oneCFolder = FoldersHelper.GetPathAbsolut(FolderType.OneCTemp);
                FileHelpers.CreateDirectory(oneCFolder);

                var errors = "";
                var warnings = "";

                if (Request.Files.Count == 0)
                    return Json(new OneCResponse(ApiStatus.Error, "Missing .csv file") { packageid = packageid });

                foreach (string fileName in Request.Files)
                {
                    var postedFile = Request.Files[fileName];
                    if (postedFile == null)
                        break;

                    if (Path.GetExtension(postedFile.FileName) != ".csv")
                        break;

                    var csvFilePath = oneCFolder + postedFile.FileName;

                    postedFile.SaveAs(csvFilePath);

                    using (var reader = new CsvReader(new StreamReader(csvFilePath, Encoding.GetEncoding("UTF-8")), CsvConstants.DefaultCsvConfiguration))
                    {
                        reader.Read();
                        reader.ReadHeader();
                        int i = 1;

                        while (reader.Read())
                        {
                            try
                            {
                                var orderIdString = reader[0];

                                var orderStatus1C = new OrderStatus1C()
                                {
                                    OrderId = orderIdString.TryParseInt(),
                                    Status1C = reader[1],
                                    OrderId1C = reader[2],
                                    OrderDate = reader[3]
                                };

                                var order = OrderService.GetOrder(orderStatus1C.OrderId);
                                if (order != null)
                                {

                                    if (string.IsNullOrWhiteSpace(orderStatus1C.Status1C))
                                    {
                                        warnings += string.Format("Статус заказа #{0} пустая строка; ", orderIdString);
                                        continue;
                                    }

                                    OrderStatus1CService.AddOrUpdateOrderStatus1C(orderStatus1C);

                                    if (Settings1C.UpdateStatuses)
                                    {
                                        var status = OrderStatusService.GetOrderStatusByName(orderStatus1C.Status1C);
                                        if (status == null)
                                        {
                                            status = new OrderStatus() { StatusName = orderStatus1C.Status1C, IsCanceled = orderStatus1C.Status1C.Trim().ToLower() == "удален" };
                                            status.StatusID = OrderStatusService.AddOrderStatus(status);
                                        }

                                        if (order.OrderStatusId != status.StatusID)
                                        { 
                                            OrderStatusService.ChangeOrderStatus(orderStatus1C.OrderId, status.StatusID, "Обновление статуса заказа из 1C");
                                            order.OrderStatusId = status.StatusID;
                                        }
                                    }

                                    if (orderStatus1C.Status1C.Trim().ToLower() == "удален")
                                    {
                                        if (!order.OrderStatus.IsCanceled)
                                            OrderStatusService.ChangeOrderStatus(orderStatus1C.OrderId, OrderStatusService.CanceledOrderStatus, "Удален из 1С");
                                    }
                                }
                                else
                                {
                                    errors += string.Format("Заказ #{0} не найден; ", orderIdString);
                                }
                            }
                            catch (Exception ex)
                            {
                                errors += string.Format("Ошибка в {0} строке {1}; ", i, ex.Message);
                                Debug.Log.Error(ex);
                            }

                            i++;
                        }
                    }

                    FileHelpers.DeleteFile(csvFilePath);
                }

                return
                    Json(new OneCResponse() { packageid = packageid, Status = ApiStatus.Ok, Errors = errors, Warnings = warnings });
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                return Json(new OneCResponse() { packageid = packageid, Status = ApiStatus.Error, Errors = "Error: " + ex.Message });
            }
        }


        // GET: api/1c/deletedorders
        [LogRequest, AuthApiKey, OneC]
        public ActionResult DeletedOrders(string apikey, string from, string to)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            try
            {
                List<int> orderids = null;

                if (!String.IsNullOrWhiteSpace(from) && !String.IsNullOrWhiteSpace(to))
                {
                    DateTime fromDate;
                    DateTime toDate;

                    if (DateTime.TryParseExact(from, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) &&
                        DateTime.TryParseExact(to, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                    {
                        orderids = OrderService.GetDeletedOrders(fromDate, toDate);
                    }
                }
                else
                {
                    orderids = OrderService.GetDeletedOrders(null, null);
                }

                return Json(new OneCDeletedItemsResponse() { status = "ok", ids = orderids != null ? String.Join(",", orderids) : "" });
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                return Json(new OneCDeletedItemsResponse() { status = "error", Errors = "Error: " + ex.Message });
            }
        }


        // GET: api/1c/deletedproducts
        [LogRequest, AuthApiKey, OneC]
        public ActionResult DeletedProducts(string apikey, string from, string to)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.Have1C)
                return Content("error saas plan");

            try
            {
                List<string> productIds = null;

                if (!String.IsNullOrWhiteSpace(from) && !String.IsNullOrWhiteSpace(to))
                {
                    DateTime fromDate;
                    DateTime toDate;

                    if (DateTime.TryParseExact(from, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) &&
                        DateTime.TryParseExact(to, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                    {
                        productIds = ProductService.GetDeletedProducts(fromDate, toDate);
                    }
                }
                else
                {
                    productIds = ProductService.GetDeletedProducts(null, null);
                }

                return Json(new OneCDeletedItemsResponse() { status = "ok", ids = productIds != null ? String.Join(",", productIds) : "" });
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                return Json(new OneCDeletedItemsResponse() { status = "error", Errors = "Error: " + ex.Message });
            }
        }
    }
}