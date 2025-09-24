using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Shared;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping;
using AdvantShop.Shipping.SelfDelivery;

namespace AdvantShop.Areas.Api.Models.Deliveries
{
    public class GetShippingTypesResponse : List<ShippingTypeResponseItem>, IApiResponse
    {
        public GetShippingTypesResponse(List<ShippingTypeResponseItem> shippings)
        {
            this.AddRange(shippings);
        }
    }
    
    public class ShippingTypeResponseItem
    {
        public int Id { get; }
        public string Type { get; }
        public string Name { get; }
        public string Description { get; }
        public string ZeroCostText { get; }
        public string Icon { get; }
        
        public List<int> WarehouseIds { get; }
        public List<WarehouseApi> Warehouses { get; }
        
        public IGeoCoordinates Coordinates { get; }
        public string Address { get; }


        public ShippingTypeResponseItem(ShippingMethod method, bool? loadWarehouses)
        {
            Id = method.ShippingMethodId;
            Type = method.ShippingType;
            Name = method.Name;
            Description = !string.IsNullOrWhiteSpace(method.Description) ? method.Description : null;
            ZeroCostText = !string.IsNullOrWhiteSpace(method.ZeroPriceMessage) ? method.ZeroPriceMessage : null;

            Icon = method.IconFileName != null && !string.IsNullOrEmpty(method.IconFileName.PhotoName)
                ? ShippingIcons.GetShippingIcon(method.ShippingType, method.IconFileName.PhotoName, method.Name)
                : ShippingIcons.GetShippingIcon(method.ShippingType, null, Name);
            
            WarehouseIds = ShippingWarehouseMappingService.GetByMethod(method.ShippingMethodId);
            
            if (loadWarehouses != null 
                && loadWarehouses.Value 
                && WarehouseIds != null && WarehouseIds.Count > 0)
            {
                var warehouses = (from warehouseId in WarehouseIds
                    select WarehouseService.Get(warehouseId)
                    into warehouse
                    where warehouse != null && warehouse.Enabled
                    select new WarehouseApi(warehouse)).ToList();
                
                if (warehouses.Count > 0)
                    Warehouses = warehouses;
            }

            if (method.ShippingType == "SelfDelivery")
            {
                var point =
                    new SelfDelivery(method, null).LoadShippingPointInfo(method.ShippingMethodId.ToString());
                
                if (point != null)
                    Coordinates = new PointDeliveryShippingCoordinates()
                        {Latitude = point.Latitude ?? 0f, Longitude = point.Longitude ?? 0f};
            }

            if (ShippingMethodService.ShippingMethodTypesUseParamsForApi.Contains(method.ShippingType))
            {
                var type = ReflectionExt.GetTypeByAttributeValue<ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, method.ShippingType);

                var shipping = (BaseShipping)Activator.CreateInstance(type, method, null);
                if (shipping is IShippingUseParamsForApi shippingParams) 
                {
                    var paramsForApi = shippingParams.GetParamsForApi();
                    Address = paramsForApi?.SelfDeliveryAddress;
                }
            }
        }
    }
}