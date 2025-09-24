using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Shop;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Checkout;
using AdvantShop.Web.Admin.Handlers.Settings.WorkingTime;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.CheckoutSettings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsCheckoutController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.Checkout.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCheckoutCtrl);

            var model = new GetCheckoutSettingsHandler().Execute();
            return View("index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(CheckoutSettingsModel model)
        {
            var isMobile = SettingsDesign.IsMobileTemplate;

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

            if (ModelState.IsValid)
            {
                new SaveCheckoutSettingsHandler(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }

            ShowErrorMessages();

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
        public JsonResult ChangeOrderId(int orderId)
        {
            var newOrderId = orderId - 1;
            var lastOrderId = OrderService.GetLastDbOrderId();

            if (newOrderId < 0 || (lastOrderId != 0 && newOrderId < lastOrderId))
            {
                return JsonError(T("Admin.Settings.ErrorWhileChangingOrderNumber"));
            }

            try
            {
                OrderService.ResetOrderID(newOrderId);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return JsonError(T("Admin.Settings.ErrorWhileChangingOrderNumber"));
            }

            return JsonOk();
        }


        #region Taxes

        public JsonResult GetPaging(TaxesFilterModel model)
        {
            return Json(new GetTaxesHandler(model).Execute());
        }

        #region Inplace Taxes

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(TaxModel model)
        {
            var tax = TaxService.GetTax(model.TaxId);
            if (tax == null)
                return JsonError();

            tax.Name = model.Name;
            tax.Rate = model.Rate;            
            tax.Enabled = model.Enabled;

            if (model.IsDefault)
                SettingsCatalog.DefaultTaxId = tax.TaxId;

            TaxService.UpdateTax(tax);            

            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(TaxesFilterModel command, Action<int, TaxesFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {                
                var ids = new GetTaxesHandler(command).GetItemsIds("TaxId");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteItems(TaxesFilterModel command)
        {
            Command(command, (id, c) => TaxService.DeleteTax(id));
            return Json(true);
        }

        #endregion

        #region CRUD tax

        public JsonResult GetTax(int id)
        {            
            var tax = TaxService.GetTax(id);
            if (tax == null)
                return Json(new { result = false });

            var model = new TaxModel()
            {
                TaxId = tax.TaxId,
                Name = tax.Name,
                Rate = tax.Rate,
                Enabled = tax.Enabled,
                TaxType = tax.TaxType
            };
            
            return Json(model);
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddTax(TaxModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return JsonError();

            var tax = new TaxElement
            {
                Name = model.Name.DefaultOrEmpty().Trim(),
                Rate = model.Rate,
                Enabled = model.Enabled,
                TaxType = model.TaxType
            };

            TaxService.AddTax(tax);

            if (model.IsDefault)
                SettingsCatalog.DefaultTaxId = tax.TaxId;

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateTax(TaxModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return JsonError();

            var tax = TaxService.GetTax(model.TaxId);
            if (tax == null)
                return JsonError();
            
            tax.Name = model.Name.DefaultOrEmpty().Trim();
            tax.Rate = model.Rate;
            var canChangeEnabled = TaxService.CanDeleteTax(model.TaxId);
            if(!model.Enabled && !canChangeEnabled)
                return JsonError(T("Admin.SettingsCheckout.ChangeEnabledIsImpossible"));
            tax.Enabled = model.Enabled;
            tax.TaxType = model.TaxType;

            TaxService.UpdateTax(tax);

            if (model.IsDefault)
                SettingsCatalog.DefaultTaxId = tax.TaxId;

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteTax(int id)
        {
            TaxService.DeleteTax(id);            
            return JsonOk();
        }

        public JsonResult GetTaxTypes()
        {
            return Json(Enum.GetValues(typeof(TaxType)).Cast<TaxType>().Select(x => new
            {
                label = x.Localize(),
                value = (int)x,
            }));
        }

        #endregion

        #endregion
        
        #region ThankYouPage Products

        [HttpGet]
        public JsonResult GetThankYouPageProducts()
        {
            var productIds = SettingsThankYouPage.SelectedProductIds;
            var products = ProductService.GetAllProductsByIds(productIds).Select(x => new
            {
                x.ProductId,
                x.Name,
                x.ArtNo,
                Url = Url.AbsoluteActionUrl("Edit", "Product", new { id = x.ProductId }),
                ImageSrc = x.Photo.ImageSrcSmall()
            });

            var ids = products.Select(x => x.ProductId).ToList();

            if (!productIds.SequenceEqual(ids))
            {
                SettingsThankYouPage.SelectedProductIds = ids;
            }

            return Json(new
            {
                products,
                productIds = ids
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteThankYouPageProduct(int productId)
        {
            var productIds = SettingsThankYouPage.SelectedProductIds;
            productIds.RemoveAll(x => x == productId);
            SettingsThankYouPage.SelectedProductIds = productIds;

            return Json(new { result = true });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddThankYouPageProducts(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return Json(new { result = false });

            var productIds = SettingsThankYouPage.SelectedProductIds;
            foreach (var id in ids.Where(x => !productIds.Contains(x)))
                productIds.Add(id);
            SettingsThankYouPage.SelectedProductIds = productIds;

            return Json(new { result = true });
        }

        #endregion

        #region WorkingTime

        public JsonResult GetWorkingTimeSettings() => ProcessJsonResult(new GetWorkingTimeSettingsHandler());

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveWorkingTimeSettings(WorkingTimeSettings model) => ProcessJsonResult(new SaveWorkingTimeSettingsHandler(model));

        public JsonResult GetAdditionalWorkingTime(DateTime? date, DateTime? dateFrom, DateTime? dateTo) => ProcessJsonResult(new GetAdditionalWorkingTimeSettingsHandler(date, dateFrom, dateTo));

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveAdditionalWorkingTimeSettings(AdditionalWorkingTimeSettings model) => ProcessJsonResult(new SaveAdditionalWorkingTimeSettingsHandler(model));

        [HttpPost]
        public JsonResult DeleteAdditionalWorkingTimes(DateTime dateStart, DateTime dateEnd)
        {
            ShopService.DeleteAdditionalWorkingTimes(dateStart, dateEnd.GetEndDay());
            return JsonOk();
        }

        #endregion
    }
}
