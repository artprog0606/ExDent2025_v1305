using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.Units;
using AdvantShop.Web.Admin.Models.Catalog.Units;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class UnitsController : BaseAdminController
    {
        public JsonResult GetUnits(UnitsFilterModel model)
        {
            return Json(new GetUnitsHandler(model).Execute());
        }
   
        #region Commands

        private void Command(UnitsFilterModel command, Action<int, UnitsFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetUnitsHandler(command).GetItemsIds("Id");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteUnits(UnitsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                if (!UnitService.IsUsed(id))
                    UnitService.Delete(id);
            });
            return Json(true);
        }

        #endregion
     
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteUnit(int unitId)
        {
            if (!UnitService.IsUsed(unitId))
            {
                UnitService.Delete(unitId);
                return Json(true);
            }
            return Json(false);
       }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceUnit(UnitModel model)
        {
            var unit = UnitService.Get(model.Id);
            if (unit == null)
                return JsonError();

            unit.Name = model.Name;
            // unit.DisplayName = model.DisplayName;
            unit.SortOrder = model.SortOrder;

            UnitService.Update(unit);

            return JsonOk();
        }

        #region Add | Update

        [HttpGet]
        public JsonResult GetUnit(int unitId)
        {
            var unit = UnitService.Get(unitId);
            var model = UnitModel.CreateByUnit(unit);
            return JsonOk(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddUnit(UnitModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.DisplayName))
                return JsonError();

            var unit = new Unit()
            {
                Name = model.Name,
                DisplayName = model.DisplayName,
                MeasureType = (MeasureType?)model.MeasureType,
                SortOrder = model.SortOrder
            };

            UnitService.Add(unit);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateUnit(UnitModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.DisplayName))
                return JsonError();

            var unit = UnitService.Get(model.Id);

            unit.Name = model.Name;
            unit.DisplayName = model.DisplayName;
            unit.MeasureType = (MeasureType?) model.MeasureType;
            unit.SortOrder = model.SortOrder;

            UnitService.Update(unit);

            return JsonOk();
        }

        #endregion

        public JsonResult GetUnitData()
        {
            var measureTypes = (from MeasureType item in Enum.GetValues(typeof (MeasureType))
                select new
                {
                    Text = item.Localize(), 
                    Value = (byte?)item
                }).ToList();
            
            measureTypes.Insert(0, new {Text = T("Admin.Units.NotSelected"), Value = (byte?)null});

            return JsonOk(new {measureTypes});
        }
    }
}