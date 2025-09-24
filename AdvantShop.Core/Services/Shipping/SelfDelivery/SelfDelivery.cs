//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.SelfDelivery
{
    [ShippingKey("SelfDelivery")]
    public class SelfDelivery : BaseShipping, IShippingNoUseExtracharge, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval, IShippingUseParamsForApi
    {
        private readonly float _shippingPrice;
        private readonly float? _latitude;
        private readonly float? _longitude;
        private readonly string _address;

        public override bool CurrencyAllAvailable => true;
        public override EnTypeOfDelivery? TypeOfDelivery => EnTypeOfDelivery.SelfDelivery;

        public SelfDelivery(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _shippingPrice = _method.Params.ElementOrDefault(SelfDeliveryTemplate.ShippingPrice, "").TryParseFloat();
            _latitude = _method.Params.ElementOrDefault(SelfDeliveryTemplate.Latitude, "").TryParseFloat(true);
            _longitude = _method.Params.ElementOrDefault(SelfDeliveryTemplate.Longitude, "").TryParseFloat(true);
            _address = _method.Params.ElementOrDefault(SelfDeliveryTemplate.Address);
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            if (!calculationVariants.HasFlag(CalculationVariants.PickPoint))
                return null;

            var (longitude, latitude) = GetCoordinates(out var warehouse);

            var option = new BaseShippingOption(_method, _totalPrice)
            {
                DeliveryTime = _method.Params.ElementOrDefault(SelfDeliveryTemplate.DeliveryTime),
                Rate = _shippingPrice,
                HideAddressBlock = true
            };

            if (longitude.HasValue
                && latitude.HasValue)
            {
                option.SelectedPoint = CreateShippingPoint(warehouse, latitude.Value, longitude.Value);
            }

            return new List<BaseShippingOption> { option };
        }

        protected override IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId)
        {
            if (pointId != _method.ShippingMethodId.ToString())
                return null;

            var resultCalc = CalcOptions(CalculationVariants.PickPoint);

            var option = resultCalc?.FirstOrDefault();
            if (option?.SelectedPoint is null)
                return null;

            return resultCalc;
        }

        public override IEnumerable<BaseShippingPoint> CalcShippingPoints(float topLeftLatitude, float topLeftLongitude, float bottomRightLatitude,
            float bottomRightLongitude)
        {
            var resultCalc = CalcOptions(CalculationVariants.PickPoint);

            var option = resultCalc?.FirstOrDefault();
            if (option?.SelectedPoint is null)
                return null;

            if (topLeftLatitude > option.SelectedPoint.Latitude
                && topLeftLongitude < option.SelectedPoint.Longitude
                && bottomRightLatitude < option.SelectedPoint.Latitude
                && bottomRightLongitude > option.SelectedPoint.Longitude)
                return new[] {option.SelectedPoint};

            return null;
        }

        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            if (pointId != _method.ShippingMethodId.ToString())
                return null;

            var (longitude, latitude) = GetCoordinates(out var warehouse);

            if (longitude.HasValue
                && latitude.HasValue)
                return CreateShippingPoint(warehouse, latitude.Value, longitude.Value);

            return null;
        }


        #region IShippingUseParamsForApi

        public ShippingParamsForApi GetParamsForApi()
        {
            var warehouse =
                ShippingWarehouseMappingService.GetByMethod(_method.ShippingMethodId)
                                               .Select(WarehouseService.Get)
                                               .FirstOrDefault(w => w.Address.IsNotEmpty());

            return new ShippingParamsForApi
            {
                SelfDeliveryAddress = warehouse?.Address ?? _address
            };
        }

        #endregion

        private BaseShippingPoint CreateShippingPoint(Warehouse warehouse, float latitude, float longitude)
        {
            var shippingPoint = new BaseShippingPoint()
            {
                Id = _method.ShippingMethodId.ToString(),
                Name = _method.Name,
                Latitude = latitude,
                Longitude = longitude,
                Address = _address
            };

            if (warehouse != null)
            {
                shippingPoint.Address = warehouse.Address.IsNotEmpty() ? warehouse.Address : warehouse.Name;
                shippingPoint.AddressComment = warehouse.AddressComment.IsNotEmpty() ? warehouse.AddressComment : null;
                shippingPoint.Description = warehouse.Description;
                
                var phones = new List<string>(){warehouse.Phone, warehouse.Phone2}.Where(x => x.IsNotEmpty()).ToArray();
                if (phones.Length > 0)
                    shippingPoint.Phones = phones;
                
                var timeOfWorkList = TimeOfWorkService.GetWarehouseTimeOfWork(warehouse.Id)
                    .Select(TimeOfWorkService.FormatTimeOfWork)
                    .ToArray();
                if (timeOfWorkList.Length > 0)
                    shippingPoint.TimeWorkStr = string.Join("<br/> ", timeOfWorkList);
            }
            
            return shippingPoint;
        }

        private (float? longitude, float? latitude) GetCoordinates(out Warehouse warehouse)
        {
            var longitude = _longitude;
            var latitude = _latitude;
            
            warehouse =
                ShippingWarehouseMappingService.GetByMethod(_method.ShippingMethodId)
                                               .Select(WarehouseService.Get)
                                               .Where(w => w.Longitude.HasValue && w.Longitude != 0f)
                                               .Where(w => w.Latitude.HasValue && w.Latitude != 0f)
                                               .FirstOrDefault();
            if (warehouse != null)
            {
                longitude = warehouse.Longitude;
                latitude = warehouse.Latitude;
            }

            return (longitude, latitude);
        }
    }
}