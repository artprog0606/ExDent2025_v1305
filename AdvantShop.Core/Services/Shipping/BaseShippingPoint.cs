using System;
using System.Collections.Generic;

namespace AdvantShop.Shipping
{    
    public class BaseShippingPoint
    {
        public string Id { get; set; }
        
        /// <summary>
        /// Информационное поле
        /// </summary>
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AddressComment { get; set; }
        public string Description { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public bool? AvailableCashOnDelivery { get; set; }
        public bool? AvailableCardOnDelivery { get; set; }
        public string[] Phones { get; set; }
        public string TimeWorkStr { get; set; }
        public List<TimeWork> TimeWork { get; set; }
        public int? StoragePeriodInDay { get; set; }
        public int? DeliveryPeriodInDay { get; set; }
        public float? MaxWeightInGrams { get; set; }
        public float? DimensionSumInMillimeters { get; set; }
        public double? DimensionVolumeInCentimeters { get; set; }
        public float? MaxHeightInMillimeters { get; set; }
        public float? MaxWidthInMillimeters { get; set; }
        public float? MaxLengthInMillimeters { get; set; }
        public float? MaxCost { get; set; }
        public int? WarehouseId { get; set; }
    }

    public class TimeWork
    {
        public string Label { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }
}