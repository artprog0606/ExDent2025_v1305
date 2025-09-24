using AdvantShop.Repository;
using AdvantShop.Shipping.ApiShip.Api;
using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipShippingPoint : BaseShippingPoint
    {
        public List<Extra> Extra { get; set; }
        public string ProviderKey { get; set; }

        public ApiShipShippingPoint() { }
        public ApiShipShippingPoint(ApiShipPoint apiShipPoint)
        {
            Address = apiShipPoint.Address;
            Code = apiShipPoint.Code;
            Description = apiShipPoint.Description;
            Id = apiShipPoint.Id.ToString();
            Latitude = apiShipPoint.Lat;
            Longitude = apiShipPoint.Lng;
            Name = apiShipPoint.Name;
            Phones = new string[] { apiShipPoint.Phone };
            TimeWork = null;
            TimeWorkStr = apiShipPoint.Timetable;
            AvailableCashOnDelivery = apiShipPoint.PaymentCash.HasValue ? apiShipPoint.PaymentCash == 1 : true;
            AvailableCardOnDelivery = apiShipPoint.PaymentCard.HasValue ? apiShipPoint.PaymentCard == 1 : true;
            MaxHeightInMillimeters = apiShipPoint.Limits?.MaxSizeA != null
                                    ? MeasureUnits.ConvertLength(apiShipPoint.Limits.MaxSizeA.Value, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter)
                                    : (float?)null;
            MaxWidthInMillimeters = apiShipPoint.Limits?.MaxSizeB != null
                                    ? MeasureUnits.ConvertLength(apiShipPoint.Limits.MaxSizeB.Value, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter)
                                    : (float?)null;
            MaxLengthInMillimeters = apiShipPoint.Limits?.MaxSizeC != null
                                    ? MeasureUnits.ConvertLength(apiShipPoint.Limits.MaxSizeC.Value, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter)
                                    : (float?)null;
            DimensionSumInMillimeters = apiShipPoint.Limits?.MaxSizeSum != null
                ? MeasureUnits.ConvertLength(apiShipPoint.Limits.MaxSizeSum.Value, MeasureUnits.LengthUnit.Centimeter, MeasureUnits.LengthUnit.Millimeter)
                : (float?)null;
            DimensionVolumeInCentimeters = apiShipPoint.Limits?.MaxVolume;
            MaxWeightInGrams = apiShipPoint.Limits?.MaxWeight;
            MaxCost = apiShipPoint.Limits?.MaxCod != null
                    ? (float)apiShipPoint.Limits.MaxCod.Value
                    : (float?)null;
        }
    }
}
