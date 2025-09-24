using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Controls;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Handlers.Settings.Catalog;
using AdvantShop.Web.Admin.Models.Settings.CatalogSettings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.FullSearch;
using System.Collections.Generic;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Settings.Currencies;
using System.Web;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(EAuthKeysComparer.And, RoleAction.Catalog, RoleAction.Settings)]
    public partial class SettingsCatalogController : BaseAdminController
    {
        private static readonly object Sync = new object();

        public ActionResult Index()
        {
            var model = new GetCatalogSettings().Execute();

            SetMetaInformation(T("Admin.Settings.Catalog.CatalogTitle"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCatalogCtrl);

            return View("Index", model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(CatalogSettingsModel model)
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
                new SaveCatalogSettingsHandler(model).Execute();
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


        #region Currencies 

        public JsonResult GetCurrenciesPaging(CurrencyFilterModel model)
        {
            return Json(new GetCurrenciesHandler(model).Execute());
        }

        public JsonResult GetCurrencies()
        {
            return Json(CurrencyService.GetAllCurrencies(true).Select(x => new { label = x.Name, value = x.Iso3.Trim() }));
        }

        [HttpPost]
        public JsonResult UpdateCb()
        {
            return CurrencyService.UpdateCurrenciesFromCentralBank() ? JsonOk(true) : JsonError();
        }

        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult CurrencyInplace(CurrencyModel model)
        {
            var currency = CurrencyService.GetCurrency(model.CurrencyId);
            if (currency == null)
                return Json(new { result = false });

            if (!ModelState.IsValid)
                return JsonError();


            bool isDefaultCurrency = false;

            if (SettingsCatalog.DefaultCurrencyIso3 == currency.Iso3)
            {
                isDefaultCurrency = true;
            }

            currency.Name = model.Name;
            currency.Rate = model.Rate;
            currency.EnablePriceRounding = model.EnablePriceRounding;
            currency.IsCodeBefore = model.IsCodeBefore;
            currency.Iso3 = model.Iso3;
            currency.NumIso3 = model.NumIso3;
            currency.RoundNumbers = model.RoundNumbers;
            currency.Symbol = model.Symbol;

            CurrencyService.UpdateCurrency(currency);

            if (isDefaultCurrency == true)
            {
                SettingsCatalog.DefaultCurrencyIso3 = currency.Iso3;
            }

            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(CurrencyFilterModel command, Func<int, CurrencyFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var ids = new GetCurrenciesHandler(command).GetItemsIds("CurrencyId");

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteItems(CurrencyFilterModel command)
        {
            try
            {
                Command(command, (id, c) =>
                {
                    var currency = CurrencyService.GetCurrency(id);
                    if (currency == null || currency.Iso3 == SettingsCatalog.DefaultCurrencyIso3)
                        throw new BlException(T("Admin.Settings.CantDeleteDefaultCurrency"));

                    if (!CurrencyService.DeleteCurrency(id))
                        throw new BlException(T("Admin.Settings.CheckIfCurrencyUsed"));
                    return true;
                });
            }
            catch (BlException ex)
            {
                return JsonError(ex.Message);
            }
            return JsonOk();
        }

        #endregion

        #region CRUD 

        public JsonResult GetCurrency(int id)
        {
            var dbModel = CurrencyService.GetCurrency(id);
            return Json(dbModel);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddCurrency(CurrencyModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            CurrencyService.InsertCurrency(model);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateCurrency(CurrencyModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            var currency = CurrencyService.GetCurrency(model.CurrencyId);
            if (currency == null)
                return JsonError();

            currency.Name = model.Name;
            currency.Rate = model.Rate;
            currency.EnablePriceRounding = model.EnablePriceRounding;
            currency.IsCodeBefore = model.IsCodeBefore;
            currency.Iso3 = model.Iso3;
            currency.NumIso3 = model.NumIso3;
            currency.RoundNumbers = model.RoundNumbers;
            currency.Symbol = model.Symbol;

            CurrencyService.UpdateCurrency(currency);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteCurrency(int id)
        {
            var currency = CurrencyService.GetCurrency(id);
            if (currency == null || currency.Iso3 == SettingsCatalog.DefaultCurrencyIso3)
                return JsonError(T("Admin.Settings.CantDeleteDefaultCurrency"));

            if (!CurrencyService.DeleteCurrency(id))
                return JsonError(T("Admin.Settings.CheckIfCurrencyUsed"));
            return JsonOk();
        }

        #endregion

        #endregion

        #region DisplayPrices
        [HttpPost]
        public JsonResult UpdateAvalableCustomerGroups(List<SelectListItem> avalableCustomerGroups)
        {
            SettingsCatalog.AvalableCustomerGroups = avalableCustomerGroups != null ? avalableCustomerGroups.Select(item => item.Value).Aggregate((group, next) => next + ";" + group) : string.Empty;
            return JsonOk(true);
        }
        #endregion

        [HttpPost]
        public JsonResult ReindexLucene()
        {
            lock (Sync)
            {
                if (!ReIndexLucene(false) && !ReIndexLucene(true))
                    return JsonError(T("Admin.Settings.CouldNotRebuildCurrency"));
            }
            return JsonOk(true);
        }

        private bool ReIndexLucene(bool secondTry = false)
        {
            try
            {
                if (secondTry)
                {
                    try
                    {
                        var dir = new DirectoryInfo(HostingEnvironment.MapPath("~/App_Data/Lucene"));
                        foreach (var file in dir.GetFiles())
                            file.Delete();
                        foreach (var directory in dir.GetDirectories())
                            directory.Delete(true);
                    }
                    catch
                    {
                    }
                }

                LuceneSearch.CreateAllIndex();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
            return true;
        }
        
        #region Price and discount regulation

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ChangePrices(PriceRegulationModel model)
        {
            var result = PriceAndDiscountRegulationHandler.ChangePrices(model);

            return Json(new { result = result.IsSuccess, message = result.IsSuccess ? result.Value : result.Error.Message });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ChangeDiscounts(CategoryDiscountRegulationModel model)
        {
            var result = PriceAndDiscountRegulationHandler.ChangeDiscountsByCategory(model);

            return Json(new { result = result.IsSuccess, message = result.IsSuccess ? result.Value : result.Error.Message });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ChangeDiscountsByBrands(BrandDiscountRegulationModel model)
        {
            var result = PriceAndDiscountRegulationHandler.ChangeDiscountsByBrands(model);

            return Json(new { result = result.IsSuccess, message = result.IsSuccess ? result.Value : result.Error.Message });
        }
        
        #endregion
    }
}
