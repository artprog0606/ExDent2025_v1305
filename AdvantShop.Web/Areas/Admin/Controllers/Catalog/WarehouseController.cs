using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.DomainOfCities;
using AdvantShop.Web.Admin.Handlers.Catalog.StockLabel;
using AdvantShop.Web.Admin.Handlers.Catalog.Warehouses;
using AdvantShop.Web.Admin.Handlers.Catalog.WarehouseTypes;
using AdvantShop.Web.Admin.Models.Catalog.DomainOfCities;
using AdvantShop.Web.Admin.Models.Catalog.Warehouses;
using AdvantShop.Web.Admin.Models.Catalog.Warehouses.StockLabel;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Settings)]
    [SaasFeature(Saas.ESaasProperty.HasWarehouses)]
    public class WarehouseController : BaseAdminController
    {
        #region Add Edit Delete

        public ActionResult Add()
        {
            var model = new WarehouseModel
            {
                DefaultMeta = true,
                Enabled = true,
                SortOrder = (WarehouseService.GetList()
                                            .Select(item => (int?)item.SortOrder)
                                            .Min() ?? 0) - 10
            };
        
            SetMetaInformation(T("Admin.Catalog.Index.WarehouseTitle"));
            SetNgController(NgControllers.NgControllersTypes.WarehouseCtrl);

            return View("Index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(WarehouseModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = new AddUpdateWarehouse(model).Execute();

                    if (model.Id.IsNotDefault())
                    {
                        ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                        return RedirectToAction("Edit", new {id = model.Id});
                    }
                }
                catch (BlException e)
                {
                    ModelState.AddModelError(e.Property, e.Message);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }
            
            ShowErrorMessages();
            
            SetMetaInformation(T("Admin.Catalog.Index.WarehouseTitle"));
            SetNgController(NgControllers.NgControllersTypes.WarehouseCtrl);

            return View("Index", model);
        }

        public ActionResult Edit(int id)
        {
            var warehouse = WarehouseService.Get(id);
            if (warehouse is null)
                return Error404();

            var model = new GetWarehouseModel(warehouse).Execute();

            SetMetaInformation($"{T("Admin.Catalog.Index.WarehouseTitle")} - {warehouse.Name}");
            SetNgController(NgControllers.NgControllersTypes.WarehouseCtrl);

            return View("Index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(WarehouseModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = new AddUpdateWarehouse(model).Execute();

                if (model.Id.IsNotDefault())
                {
                    ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
                    return RedirectToAction("Edit", new {id = model.Id});
                }
            }
            
            ShowErrorMessages();
            
            SetMetaInformation($"{T("Admin.Catalog.Index.WarehouseTitle")} - {model.Name}");
            SetNgController(NgControllers.NgControllersTypes.WarehouseCtrl);

            return View("Index", model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteWarehouse(int warehouseId)
        {
            try
            {
                WarehouseService.Delete(warehouseId);
                return JsonOk();
            }
            catch (Exception e)
            {
                Debug.Log.Error(e);
                return JsonError(e.Message);
            }
        }

        #endregion

        #region List

        public ActionResult GetWarehouses(WarehousesFilterModel model)
        {
            return Json(new GetWarehouses(model).Execute());
        }

        #region Commands

        private void Command(WarehousesFilterModel command, Action<int, WarehousesFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetWarehouses(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteWarehouses(WarehousesFilterModel command)
        {
            try
            {
                Command(command, (id, c) => WarehouseService.Delete(id));
                return JsonOk();
            }
            catch (Exception e)
            {
                Debug.Log.Error(e);
                return JsonError(e.Message);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ActivateWarehouses(WarehousesFilterModel model)
        {
            Command(model, (id, c) => WarehouseService.SetActive(id, true));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableWarehouses(WarehousesFilterModel model)
        {
            Command(model, (id, c) => WarehouseService.SetActive(id, false));
            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceWarehouse(WarehouseGridModel model)
        {
            var warehouse = WarehouseService.Get(model.WarehouseId);

            warehouse.Enabled = model.Enabled;
            warehouse.SortOrder = model.SortOrder;

            WarehouseService.Update(warehouse);

            return JsonOk();
        }
        
        public ActionResult GetWarehousesChooser(WarehousesFilterModel model)
        {
            return Json(new GetWarehouses(model).Execute());
        }

        public ActionResult GetWarehousesList()
        {
            return Json(WarehouseService.GetList().Select(x => new { label = x.Name, value = x.Id.ToString() }));
        }

        #endregion

        #region Warehouse type

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetWarehouseType(int id)
        {
            var typeWarehouse = TypeWarehouseService.Get(id);
            if (typeWarehouse is null)
                return JsonError("Тип склада не найден");

            var model = new WarehouseTypeModel()
            {
                Id = typeWarehouse.Id,
                Name = typeWarehouse.Name,
                Enabled = typeWarehouse.Enabled,
                SortOrder = typeWarehouse.SortOrder,
            };

            return JsonOk(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddWarehouseType(WarehouseTypeModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            
            var typeWarehouse = new TypeWarehouse()
            {
                Id = model.Id,
                Name = model.Name,
                Enabled = model.Enabled,
                SortOrder = model.SortOrder,
            };

            typeWarehouse.Id = TypeWarehouseService.Add(typeWarehouse);

            return typeWarehouse.Id == 0
                ? JsonError("Не удалось добавить тип склада")
                : JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateWarehouseType(WarehouseTypeModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            
            var typeWarehouse = TypeWarehouseService.Get(model.Id);
            if (typeWarehouse is null)
                return JsonError("Тип склада не найден");
            
            typeWarehouse.Name = model.Name;
            typeWarehouse.Enabled = model.Enabled;
            typeWarehouse.SortOrder = model.SortOrder;

            TypeWarehouseService.Update(typeWarehouse);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteWarehouseType(int warehouseTypeId)
        {
            TypeWarehouseService.Delete(warehouseTypeId);
            
            return JsonOk();
        }

        #region List

        public JsonResult GetWarehouseTypesList(string q, int page = 1, int? typeId = null)
        {
            var typesWarehouse =
                new GetWarehouseTypes(new WarehouseTypesFilterModel() {ItemsPerPage = q.IsNotEmpty() ? 25 : 50, Page = page, Search = q})
                   .Execute()
                   .DataItems;
            if (typeId.HasValue)
            {
                var typeWarehouse = TypeWarehouseService.Get(typeId.Value);
                if (typeWarehouse != null)
                    typesWarehouse.Add(new WarehouseTypeGridModel()
                    {
                        TypeId = typeWarehouse.Id,
                        TypeName = typeWarehouse.Name,
                        Enabled = typeWarehouse.Enabled,
                        SortOrder = typeWarehouse.SortOrder
                    });
            }
            return Json(typesWarehouse.Select(x => new { label = x.TypeName, value = x.TypeId }));
        }

        public ActionResult GetWarehouseTypes(WarehouseTypesFilterModel model)
        {
            return Json(new GetWarehouseTypes(model).Execute());
        }

        #region Commands

        private void Command(WarehouseTypesFilterModel command, Action<int, WarehouseTypesFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetWarehouseTypes(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteWarehouseTypes(WarehouseTypesFilterModel command)
        {
            Command(command, (id, c) => TypeWarehouseService.Delete(id));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ActivateWarehouseTypes(WarehouseTypesFilterModel model)
        {
            Command(model, (id, c) => TypeWarehouseService.SetActive(id, true));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DisableWarehouseTypes(WarehouseTypesFilterModel model)
        {
            Command(model, (id, c) => TypeWarehouseService.SetActive(id, false));
            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceWarehouseType(WarehouseTypeGridModel model)
        {
            var typeWarehouse = TypeWarehouseService.Get(model.TypeId);

            typeWarehouse.Name = model.TypeName;
            typeWarehouse.Enabled = model.Enabled;
            typeWarehouse.SortOrder = model.SortOrder;

            TypeWarehouseService.Update(typeWarehouse);

            return JsonOk();
        }

        #endregion

        #endregion

        #region Stock Label

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetStockLabel(int id)
        {
            var stockLabel = StockLabelService.Get(id);
            if (stockLabel is null)
                return JsonError("Ярлык не найден");
        
            var model = new StockLabelModel()
            {
                Id = stockLabel.Id,
                Name = stockLabel.Name,
                ClientName = stockLabel.ClientName,
                Color = stockLabel.Color,
                AmountUpTo = stockLabel.AmountUpTo,
            };
        
            return JsonOk(model);
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddStockLabel(StockLabelModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            
            var stockLabel = new StockLabel()
            {
                Id = model.Id,
                Name = model.Name,
                ClientName = model.ClientName,
                Color = model.Color,
                AmountUpTo = model.AmountUpTo,
            };
        
            stockLabel.Id = StockLabelService.Add(stockLabel);
        
            return stockLabel.Id == 0
                ? JsonError("Не удалось добавить ярлык")
                : JsonOk();
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateStockLabel(StockLabelModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            
            var stockLabel = StockLabelService.Get(model.Id);
            if (stockLabel is null)
                return JsonError("Ярлык не найден");
            
            stockLabel.Name = model.Name;
            stockLabel.ClientName = model.ClientName;
            stockLabel.Color = model.Color;
            stockLabel.AmountUpTo = model.AmountUpTo;
        
            StockLabelService.Update(stockLabel);
        
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteStockLabel(int stockLabelId)
        {
            StockLabelService.Delete(stockLabelId);
            
            return JsonOk();
        }

        #region List

        public ActionResult GetStockLabels(StockLabelsFilterModel model)
        {
            return Json(new GetStockLabels(model).Execute());
        }

        #region Commands

        private void Command(StockLabelsFilterModel command, Action<int, StockLabelsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetStockLabels(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteStockLabels(StockLabelsFilterModel command)
        {
            Command(command, (id, c) => StockLabelService.Delete(id));
            return JsonOk();
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceStockLabel(StockLabelGridModel model)
        {
            var stockLabel = StockLabelService.Get(model.LabelId);

            stockLabel.Name = model.Name;
            stockLabel.ClientName = model.ClientName;
            stockLabel.AmountUpTo = model.AmountUpTo;

            StockLabelService.Update(stockLabel);

            return JsonOk();
        }

        #endregion

        #endregion
        
        #region Cities

        [HttpGet]
        public JsonResult GetCitiesByWarehouse(int warehouseId)
        {
            var list = WarehouseCityService.GetCities(warehouseId);
            return Json(list);
        }
                
        [HttpGet]
        public JsonResult GetWarehousesByCity(int cityId)
        {
            var warehouses = WarehouseCityService.GetWarehouses(cityId, onlyEnabled: false);
            return Json(warehouses);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddWarehouseCities(List<WarehouseCity> warehouseCities)
        {
            if (warehouseCities != null && warehouseCities.Count > 0)
            {
                var sortOrder = WarehouseCityService.GetMaxSortOrder(warehouseCities[0].WarehouseId) + 10;
                
                foreach (var warehouseCity in warehouseCities)
                {
                    if (WarehouseCityService.Get(warehouseCity.WarehouseId, warehouseCity.CityId) != null)
                        continue;

                    warehouseCity.SortOrder = sortOrder;
                    
                    WarehouseCityService.Add(warehouseCity);

                    sortOrder += 10;
                }
            }

            return JsonOk();
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteWarehouseCity(int warehouseId, int cityId)
        {
            WarehouseCityService.Delete(warehouseId, cityId);
            return JsonOk();
        }

        #endregion
        
        #region DomainGeoLocation 
        
        #region List

        public ActionResult GetDomainGeoLocations(DomainGeoLocationFilterModel model)
        {
            return Json(new GetDomainGeoLocations(model).Execute());
        }

        #region Commands

        private void CommandDomainGeoLocation(DomainGeoLocationFilterModel command, Action<int, DomainGeoLocationFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetDomainGeoLocations(command).GetItemsIds();
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteDomainGeoLocations(DomainGeoLocationFilterModel command)
        {
            CommandDomainGeoLocation(command, (id, c) => DomainGeoLocationService.Delete(id));
            return JsonOk();
        }

        #endregion

        #endregion
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetDomainGeoLocation(int id)
        {
            var domain = DomainGeoLocationService.Get(id);
            if (domain is null)
                return JsonError("Домен не найден");

            var model = new DomainGeoLocationModel()
            {
                Id = domain.Id,
                Url = domain.Url,
                GeoName = domain.GeoName,
                Cities =
                    DomainGeoLocationService.GetCities(id)
                        .Select(x => new DomainCityItem() { CityId = x.CityId, CityName = x.CityName })
                        .ToList()
            };
        
            return JsonOk(model);
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddDomainGeoLocation(DomainGeoLocationModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            
            var domainOfCity = new DomainGeoLocation()
            {
                Id = model.Id,
                Url = model.Url,
                GeoName = model.GeoName
            };
            DomainGeoLocationService.Add(domainOfCity);

            foreach (var city in model.Cities)
                DomainGeoLocationService.AddCity(new DomainCityLink(domainOfCity.Id, city.CityId));
            
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Warehouse_AddDomain);
        
            return JsonOk();
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateDomainGeoLocation(DomainGeoLocationModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();
            
            var domain = DomainGeoLocationService.Get(model.Id);
            if (domain is null)
                return JsonError("Домен не найден");
            
            domain.Url = model.Url;
            domain.GeoName = model.GeoName;
            
            DomainGeoLocationService.Update(domain);
            DomainGeoLocationService.DeleteCities(domain.Id);

            foreach (var city in model.Cities)
                DomainGeoLocationService.AddCity(new DomainCityLink(domain.Id, city.CityId));
        
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteDomainGeoLocation(int id)
        {
            DomainGeoLocationService.Delete(id);
            return JsonOk();
        }

        #endregion
    }
}