using System;
using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Shared;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Shipping;
using AdvantShop.Shipping.PointDelivery;

namespace AdvantShop.Areas.Api.Models.Deliveries
{
    public class PointDeliveryResponse : List<PointDeliveryShipping>, IApiResponse
    {

        public PointDeliveryResponse(List<PointDeliveryShipping> items)
        {
            this.AddRange(items);
        }
    }
    
    public class PointDeliveryShipping
    {
        public string Id { get; }
        
        [Obsolete("Use PointStringId")]
        public int PointId { get; }
        
        public string PointStringId { get; }
        
        public bool InHouse { get; }
        public string Name { get; }
        public string Description { get; }
        public string ZeroCostText { get; }
        public float Price { get; }
        public string PreparedPrice { get; }
        public string DeliveryTime { get; }
        public string Icon { get; }
        public int? WarehouseId { get; }
        
        public IGeoCoordinates Coordinates { get; }

        public PointDeliveryShipping(BaseShippingOption option, DeliveryPointShipping point, bool inHouse)
        {
            var pointId = point.Id ?? "";
            
            Id = option.Id;
            PointId = pointId.TryParseInt(true) ?? 
                      pointId.Split('-', '_')[0].TryParseInt(true) ?? 
                      0;
            PointStringId = point.Id;
            Name = point.Address;
            Description = point.Description;
            ZeroCostText = option.ZeroPriceMessage;
            Price = option.FinalRate;
            PreparedPrice = option.FormatRate;
            DeliveryTime = option.DeliveryTime;

            Icon = option.IconName;
            
            Coordinates = new PointDeliveryShippingCoordinates() {Latitude = point.Latitude ?? 0f, Longitude = point.Longitude ?? 0f};

            InHouse = inHouse;

            WarehouseId = point.WarehouseId;
        }
    }

    public class PointDeliveryShippingCoordinates : IGeoCoordinates
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}