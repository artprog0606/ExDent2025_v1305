using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.System;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.Location;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Web.Admin.Models;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class RegionsController : BaseAdminController
    {
        [ExcludeFilter(typeof(AuthAttribute))]
        public JsonResult GetRegionsAutocomplete(string q)
        {
            var result = RegionService.GetRegionsByName(q);
            return Json(result);
        }

        [ExcludeFilter(typeof(AuthAttribute))]
        public JsonResult GetRegionsAutocompleteExt(string q)
        {
            var result = IpZoneService.GetIpZonesByRegion(q);
            return Json(result);
        }

        #region Add/Edit/Get/Delete

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddRegion(RegionModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return Json(new { result = false });

            try
            {
                var region = new Region()
                {
                    Name = model.Name,
                    CountryId = model.CountryId,
                    RegionCode = model.RegionCode,
                    SortOrder = model.SortOrder
                };

                RegionService.InsertRegion(region);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        public JsonResult EditRegion(RegionModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return Json(new { result = false });

            try
            {
                var region = new Region()
                {
                    RegionId = model.RegionId,
                    Name = model.Name,
                    CountryId = model.CountryId,
                    RegionCode = model.RegionCode,
                    SortOrder = model.SortOrder,
                };

                RegionService.UpdateRegion(region);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("", ex);
                return Json(new { result = false });
            }

            return Json(new { result = true });
        }

        public JsonResult GetAdditionalSettings(int regionId)
        {
            var additionalSettings = RegionService.GetAdditionalSettings(regionId);
            var model = new RegionAdditionalSettingsModel
            {
                RegionId = additionalSettings.RegionId,
                FiasId = additionalSettings.FiasId,
                KladrId = additionalSettings.KladrId,
            };
            var region = RegionService.GetRegion(regionId);
            var country =
                region != null
                    ? CountryService.GetCountry(region.CountryId)
                    : null;
            model.CountryIso2 = country?.Iso2;
            return JsonOk(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult EditAdditionalSettings(RegionAdditionalSettingsModel model)
        {
            if (model.RegionId == 0)
                return JsonError();

            try
            {
                RegionService.UpdateAdditionalSettings(new RegionAdditionalSettings
                {
                    RegionId = model.RegionId,
                    FiasId = model.FiasId,
                    KladrId = model.KladrId,
                });
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return JsonError();
            }

            return JsonOk();
        }
        
        public JsonResult GetRegionItem(int RegionID)
        {
            var region = RegionService.GetRegion(RegionID);
            var model = new RegionModel
            {
                CountryId = region.CountryId,
                Name = region.Name,
                RegionCode = region.RegionCode,
                RegionId = region.RegionId,
                SortOrder = region.SortOrder
            };
            var country = CountryService.GetCountry(region.CountryId);
            model.CountryIso2 = country?.Iso2;
            return Json(model);
        }

        public JsonResult GetFormData()
        {
            return Json(new
            {
                Countries = CountryService.GetAllCountries().Select(x => new SelectListItem { Text = x.Name, Value = x.CountryId.ToString() })
            });
        }

        public JsonResult GetRegions(AdminRegionFilterModel model)
        {
            if(model.id != null)
            {
                model.CountryId = (int)model.id;
            }
            var hendler = new GetRegion(model);
            var result = hendler.Execute();

            return Json(result);
        }

        public JsonResult DeleteRegion(AdminRegionFilterModel model)
        {
            Command(model, (id, c) =>
            {
                RegionService.DeleteRegion(id);
                return true;
            });

            return Json(true);
        }

        #endregion    
        
        #region Inplace

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult InplaceRegion(AdminRegionFilterModel model)
        { 
            var region = RegionService.GetRegion(model.RegionId);

            region.Name = model.Name;
            region.CountryId = model.CountryId;
            region.RegionCode = model.RegionCode;
            region.SortOrder = model.SortOrder;

            RegionService.UpdateRegion(region);

            return Json(new { result = true });
        }

        #endregion

        #region Command

        private void Command(AdminRegionFilterModel model, Func<int, AdminRegionFilterModel, bool> func)
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
                model.CountryId = model.id != null ? (int)model.id : 0;
                var handler = new GetRegion(model);
                var RegioniD = handler.GetItemsIds();

                foreach (int id in RegioniD)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }
        
        #endregion
        
        [HttpGet]
        public JsonResult GetRegionsList(int countryId)
        {
            var result = 
                RegionService.GetRegions(countryId)
                    .Select(x => new SelectItemModel<int>(x.Name, x.RegionId))
                    .ToList();
            
            return Json(result);
        }
    }
}
