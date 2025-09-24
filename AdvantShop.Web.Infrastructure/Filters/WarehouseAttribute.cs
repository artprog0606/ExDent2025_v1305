using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Saas;

namespace AdvantShop.Web.Infrastructure.Filters
{
    /// <summary>
    /// Detect current store warehouse
    /// </summary>
    public class WarehouseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            
            var warehousesActive = !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HasWarehouses;
            if (!warehousesActive)
                return;

            var geoDomain =
                DomainGeoLocationService.GetList()
                    .Find(x => x.Url.Equals(HttpContext.Current.Request.Url.Host, StringComparison.OrdinalIgnoreCase));
            
            if (geoDomain != null)
            {
                var ids = DomainGeoLocationService.GetWarehouseIds(geoDomain.Id);
                if (ids.Count > 0)
                {
                    WarehouseContext.CurrentWarehouseIds = ids;
                    
                    // при первом заходе ставим куку складов и город
                    var warehouseCookie = CommonHelper.GetCookieString(WarehouseService.CookieName);
                    if (warehouseCookie.IsNullOrEmpty())
                        WarehouseService.SetCookie(ids);

                    if (!IpZoneContext.IsCookieExists())
                    {
                        var city = DomainGeoLocationService.GetCity(geoDomain.Id);
                        if (city != null)
                            IpZoneContext.SetCustomerCookie(IpZone.Create(city));
                    }
                }
            }

            var controller = filterContext.RequestContext.RouteData.Values["controller"] as string;
            if (!string.IsNullOrEmpty(controller) && controller.Equals("checkout", StringComparison.OrdinalIgnoreCase))
            {
                var checkoutData = OrderConfirmationService.Get(CustomerContext.CustomerId);
                if (checkoutData?.WarehouseId != null)
                {
                    WarehouseContext.CurrentWarehouseIds = new List<int>() { checkoutData.WarehouseId.Value };
                    return;
                }
            }
            
            var warehouseIdHeader = filterContext.HttpContext.Request.Headers["X-API-WAREHOUSES"];
            if (!string.IsNullOrWhiteSpace(warehouseIdHeader))
            {
                var ids = 
                    warehouseIdHeader
                        .Split(',')
                        .Select(x => x.TryParseInt())
                        .Where(x => x != 0 && WarehouseService.Exists(x, true))
                        .ToList();
                
                if (ids.Count > 0)
                {
                    WarehouseContext.CurrentWarehouseIds = ids;
                    return;
                }
            }
            
            if (filterContext.HttpContext.Request["warehouseIds"] != null)
            {
                var ids = 
                    filterContext.HttpContext.Request["warehouseIds"]
                        .Split(',')
                        .Select(x => x.TryParseInt())
                        .Where(x => x != 0 && WarehouseService.Exists(x, true))
                        .ToList();
                
                if (ids.Count > 0)
                {
                    WarehouseContext.CurrentWarehouseIds = ids;
                    return;
                }
            }
            
            var cookie = CommonHelper.GetCookieString(WarehouseService.CookieName);
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                var cookieIds = 
                    HttpUtility.UrlDecode(cookie)
                        .Split(',')
                        .Select(x => x.TryParseInt())
                        .ToList();

                var ids = cookieIds.Where(x => x != 0 && WarehouseService.Exists(x, true)).ToList();
                
                if (ids.Count > 0)
                {
                    WarehouseContext.CurrentWarehouseIds = ids;
                    
                    if (cookieIds.Count != ids.Count)
                        WarehouseService.SetCookie(ids);
                    
                    return;
                }
            }

            // если куки нет, то ищем по городу
            var zone = IpZoneContext.CurrentZone;
            if (zone != null && zone.CityId != 0)
            {
                var ids = WarehouseCityService.GetWarehouseIds(zone.CityId);
                if (ids.Count > 0)
                {
                    WarehouseContext.CurrentWarehouseIds = ids;
                    WarehouseService.SetCookie(ids);
                    return;
                }
            }
        }
    }
}