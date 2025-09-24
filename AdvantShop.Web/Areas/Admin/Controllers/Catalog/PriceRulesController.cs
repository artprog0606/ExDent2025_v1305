using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.PriceRules;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Catalog.PriceRules;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public class PriceRulesController : BaseAdminController
    {
        public JsonResult GetList(PriceRuleFilterModel model)
        {
            return Json(new GetPriceRulesHandler(model).Execute());
        }

        #region Commands

        private void Command(PriceRuleFilterModel command, Action<int, PriceRuleFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, command);
            }
            else
            {
                var ids = new GetPriceRulesHandler(command).GetItemsIds("Id");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeletePriceRules(PriceRuleFilterModel command)
        {
            Command(command, (id, c) =>
            {
                if (!PriceRuleService.IsUsed(id))
                    PriceRuleService.Delete(id);
            });
            return Json(true);
        }

        #endregion

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            if (!PriceRuleService.IsUsed(id))
                PriceRuleService.Delete(id);

            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ForceDelete(int id)
        {
            PriceRuleService.Delete(id);
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Inplace(PriceRuleModel model)
        {
            var priceRule = PriceRuleService.Get(model.Id);
            if (priceRule == null)
                return Json(new {result = false});

            priceRule.Name = model.Name.DefaultOrEmpty();
            priceRule.SortOrder = model.SortOrder;
            priceRule.Enabled = model.Enabled;

            var result = PriceRuleService.Update(priceRule);

            return result.IsSuccess ? Json(new {result = true}) : JsonError(result.Error.Message);
        }

        #region Add | Update

        [HttpGet]
        public JsonResult Get(int id)
        {
            var rule = PriceRuleService.Get(id);
            return Json(rule != null ? new PriceRuleViewModel(rule) : null);
        }

        [HttpGet]
        public JsonResult GetInfo()
        {
            var defaultCustomerGroupId = CustomerGroupService.DefaultCustomerGroup;
            
            var customerGroups =
                CustomerGroupService.GetCustomerGroupList()
                    .OrderBy(x => x.CustomerGroupId == defaultCustomerGroupId ? 0 : 1)
                    .ThenBy(x => x.CustomerGroupId)
                    .Select(x => new {Id = x.CustomerGroupId, Name = x.GroupName})
                    .ToList();
            
            var paymentMethods = new List<SelectItemModel<int?>>() {new SelectItemModel<int?>("Нет", null)};

            foreach (var method in PaymentService.GetAllPaymentMethods(false))
            {
                paymentMethods.Add(new SelectItemModel<int?>(method.Name, method.PaymentMethodId));
            }

            var shippingMethods = new List<SelectItemModel<int?>>() {new SelectItemModel<int?>("Нет", null)};

            foreach (var method in ShippingMethodService.GetAllShippingMethods())
            {
                shippingMethods.Add(new SelectItemModel<int?>(method.Name, method.ShippingMethodId));
            }

            var warehouses = WarehouseService.GetList()
                .Select(x => new {Id = x.Id, x.Name})
                .ToList();

            var warehousesActive = 
                FeaturesService.IsEnabled(EFeature.PriceTypeWithWarehouse) &&
                (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HasWarehouses);

            var sortOrder = SQLDataAccess.Query<int>("Select isNull(Max(SortOrder), 0) From Catalog.PriceRule")
                                         .FirstOrDefault() + 10;
            
            return Json(new
            {
                customerGroups,
                paymentMethods,
                shippingMethods,
                warehouses,
                warehousesActive,
                sortOrder
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(PriceRuleViewModel model)
        {
            var result = PriceRuleService.Add((PriceRule) model);
            return result.IsSuccess ? JsonOk() : JsonError(result.Error.Message);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(PriceRuleViewModel model)
        {
            var result = PriceRuleService.Update((PriceRule) model);
            return result.IsSuccess ? JsonOk() : JsonError(result.Error.Message);
        }
        
        #endregion
    }
}
