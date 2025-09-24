using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.Import;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Crm)]
    public class SettingsCrmController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new CrmSettingsModel()
            {
                CrmActive = SettingsCrm.CrmActive,
                DefaultSalesFunnelId = SettingsCrm.DefaultSalesFunnelId,
                OrderStatusIdFromLead = SettingsCrm.OrderStatusIdFromLead,
                ImportLeadsModel = new GetImportLeadsModel().Execute()
            };

            SetMetaInformation(T("Admin.Settings.CRM.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCrmCtrl);

            return View(model);
        }
        
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveDefaultSalesFunnelId(int id)
        {
            var funnel = SalesFunnelService.Get(id);
            if (funnel == null)
                return JsonError();

            SettingsCrm.DefaultSalesFunnelId = id;
            //удалаяем последнюю просмотренную воронку если сменили дефолтную
            SalesFunnelService.SetLastAdminFunnel(null);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveOrderStatusIdFromLead(int id)
        {
            var status = OrderStatusService.GetOrderStatus(id);
            if (status == null)
                return JsonError();

            SettingsCrm.OrderStatusIdFromLead = id;

            return JsonOk();
        }

        public JsonResult GetOrderStatuses()
        {
            return Json(OrderStatusService.GetOrderStatuses().Select(x => new SelectItemModel(x.StatusName, x.StatusID)));
        }

        public JsonResult SetCrmActive(bool active)
        {
            SettingsCrm.CrmActive = active;
            return JsonOk();
        }

        //public JsonResult GetIntegrationsData()
        //{
        //    var limit = SaasDataService.IsSaasEnabled ? SaasDataService.CurrentSaasData.CRMIntegrationsCount : 0;
        //    var count = SocialNetworkService.GetIntegrationsCount();
        //    return Json(new
        //    {
        //        limit,
        //        count,
        //        limitRiched = limit > 0 && count >= limit
        //    });
        //}
    }
}
