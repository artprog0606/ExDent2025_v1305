using System;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Models.Settings.WarehouseSettings;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(EAuthKeysComparer.And, RoleAction.Catalog, RoleAction.Settings)]
    [SaasFeature(Saas.ESaasProperty.HasWarehouses)]
    public partial class SettingsWarehousesController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.Warehouses.WarehousesTitle"));
            SetNgController(NgControllers.NgControllersTypes.SettingsWarehousesCtrl);

            var warehouse = WarehouseService.Get(SettingsCatalog.DefaultWarehouse);

            var model = new WarehouseSettingsModel
            {
                DefaultWarehouseId = warehouse.Id,
                DefaultWarehouseName = warehouse.Name,
                SetCookieOnMainDomain = SettingsMain.SetCookieOnMainDomain,
                StoreUrl = SettingsMain.SiteUrl
            };

            model.ShowSetCookieOnMainDomain =
                !string.IsNullOrWhiteSpace(model.StoreUrl)
                && Uri.TryCreate(model.StoreUrl.StartsWith("http") ? model.StoreUrl : "http://" + model.StoreUrl, UriKind.Absolute, out var uri)
                && !SettingsMain.IsTechDomain(uri);

            if (!model.ShowSetCookieOnMainDomain && model.SetCookieOnMainDomain)
                model.SetCookieOnMainDomain = SettingsMain.SetCookieOnMainDomain = false;

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(WarehouseSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                SettingsCatalog.DefaultWarehouse = model.DefaultWarehouseId;
                SettingsMain.SetCookieOnMainDomain = model.SetCookieOnMainDomain;
                
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
          
            ShowErrorMessages();

            return Index();
        }
    }
}