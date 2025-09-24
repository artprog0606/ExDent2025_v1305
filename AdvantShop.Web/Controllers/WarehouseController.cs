using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Handlers.Warehouse;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    public class WarehouseController : BaseClientController
    {
        // GET
        public ActionResult Warehouses()
        {
            var metaInfo = MetaInfoService.GetMetaInfo(0, MetaType.PageWarehouses) ??
                           new MetaInfo
                           {
                               Type = MetaType.PageWarehouses,
                               Title = SettingsSEO.PageWarehousesMetaTitle,
                               H1 = SettingsSEO.PageWarehousesMetaH1,
                               MetaKeywords = SettingsSEO.PageWarehousesMetaKeywords,
                               MetaDescription = SettingsSEO.PageWarehousesMetaDescription
                           };

            SetMetaInformation(metaInfo);
            
            return View();
        }
        
        public JsonResult GetWarehousesInfo()
        {
            return ProcessJsonResult(new GetWarehousesInfo());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetCartStockInWarehouses(params int[] warehousesId)
        {
            return ProcessJsonResult(new GetCartStockInWarehousesHandler(), warehousesId);
        }
        

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SetWarehouses(List<int> warehouses)
        {
            WarehouseService.SetCookie(warehouses ?? new List<int>());
            return JsonOk();
        }
    }
}