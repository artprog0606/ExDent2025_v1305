using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Crm.SalesFunnels;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Crm.SalesFunnels;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [Auth(RoleAction.Crm)]
    [SaasFeature(ESaasProperty.HaveCrm)]
    public partial class SalesFunnelsController : BaseAdminController
    {
        #region Get / Add / Update / Delete

        //public JsonResult GetList()
        //{
        //    return Json(new { items = SalesFunnelService.GetList().Select(x => new SalesFunnelModel(x)) });
        //}

        public JsonResult GetSalesFunnelsGrid(SalesFunnelsFilterModel model)
        {
            return Json(new GetSalesFunnels(model).Execute());
        }

        public JsonResult GetSalesFunnels()
        {
            return Json(SalesFunnelService.GetList().Select(x => new SelectItemModel(x.Name, x.Id)));
        }

        public JsonResult GetSalesFunnel(int? id)
        {
            if (!id.HasValue)
                return JsonOk(new SalesFunnelAddEditModel());
            SalesFunnel funnel;
            if (id.HasValue && (funnel = SalesFunnelService.Get(id.Value)) != null)
                return JsonOk(new SalesFunnelAddEditModel(funnel));

            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddSalesFunnel(SalesFunnelAddEditModel model)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new AddUpdateSalesFunnelHandler(model, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateSalesFunnel(SalesFunnelAddEditModel salesFunnel)
        {
            if (!ModelState.IsValid)
                return JsonError();

            return ProcessJsonResult(new AddUpdateSalesFunnelHandler(salesFunnel, true));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSalesFunnel(int id)
        {
            return ProcessJsonResult(() => SalesFunnelService.Delete(id));
        }

        private void Command(SalesFunnelsFilterModel model, Func<int, SalesFunnelsFilterModel, bool> func)
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
                var handler = new GetSalesFunnels(model);
                var ids = handler.GetItemsIds("Id");

                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSalesFunnels(SalesFunnelsFilterModel model)
        {
            try
            {
                Command(model, (id, c) =>
                {
                    SalesFunnelService.Delete(id);
                    return true;
                });
            }
            catch (BlException ex)
            {
                return JsonError(ex.Message);
            }
            return Json(true);
        }

        #endregion

        [HttpGet]
        public JsonResult GetDealStatuses(int salesFunnelId, bool all = true)
        {
            var statuses = all
                ? DealStatusService.GetList(salesFunnelId)
                : DealStatusService.GetList(salesFunnelId)
                    .Where(x => x.Status != SalesFunnelStatusType.Canceled && x.Status != SalesFunnelStatusType.FinalSuccess)
                    .ToList();

            return Json(statuses.Select(x => new SelectItemModel(x.Name, x.Id)));
        }

        [HttpGet]
        public JsonResult GetFormData()
        {
            return Json(new
            {
                canAddSalesFunnel = !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCrm
            });
        }

        public JsonResult GetLeadAutoCompleteProductsInfo(List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                return null;

            var productsInfo = SalesFunnelService.GetLeadAutoCompleteProductsInfo(productIds).Select(x => new SelectItemModel(x.Value, x.Key)).ToList();

            return JsonOk(productsInfo);
        }

        public JsonResult GetLeadAutoCompleteCategoriesInfo(List<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any())
                return null;

            var categoriesInfo = SalesFunnelService.GetLeadAutoCompleteCategoriesInfo(categoryIds).Select(x => new SelectItemModel(x.Value, x.Key)).ToList();

            return JsonOk(categoriesInfo);
        }

        [HttpGet]
        public JsonResult CreateOrderOnFinalSuccess(int salesFunnelId, int dealStatusId)
        {
            var funnel = SalesFunnelService.Get(salesFunnelId);
            if (funnel == null)
                return Json(false);

            var dealStatus = DealStatusService.Get(dealStatusId);
            if (dealStatus == null)
                return Json(false);
            
            if (!DealStatusService.GetList(funnel.Id).Any(x => x.Id == dealStatusId))
                return Json(false);

            var createOrderOnFinalSuccess = 
                funnel.FinalSuccessAction == SalesFunnelFinalSuccessAction.CreateOrder &&
                dealStatus.Status == SalesFunnelStatusType.FinalSuccess;

            return Json(createOrderOnFinalSuccess);
        }
    }
}
