//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping.ShippingByRangeWeightAndDistance;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.RangeWeightAndDistanceOption
{
    [ShippingKey("ShippingByRangeWeightAndDistance")]
    public class ShippingByRangeWeightAndDistance : BaseShippingWithWeight, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval, IShippingRequiresSpecifyingTypeOfDelivery
    {
        private readonly List<WeightLimit> _weightLimits;
        private readonly List<DistanceLimit> _distanceLimits;
        private readonly bool _useDistance;
        private readonly string _deliveryTime;
        private readonly int _maxDistance;

        public override bool CurrencyAllAvailable { get { return true; } }

        public ShippingByRangeWeightAndDistance(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {

            var weightLimitParam = _method.Params.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.WeightLimit);
            var distanceLimitParam = _method.Params.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.DistanceLimit);

            _weightLimits = weightLimitParam.IsNotEmpty()
                ? JsonConvert.DeserializeObject<List<WeightLimit>>(weightLimitParam)
                : new List<WeightLimit>();

            _distanceLimits = distanceLimitParam.IsNotEmpty()
                ? JsonConvert.DeserializeObject<List<DistanceLimit>>(distanceLimitParam)
                : new List<DistanceLimit>();

            _useDistance = _method.Params.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.UseDistance).TryParseBool();
            _deliveryTime = _method.Params.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.DeliveryTime);
            _maxDistance = _method.Params.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.MaxDistance).TryParseInt();
        }

        private float GetRate(float distance)
        {
            if (distance > _maxDistance)
                throw new Exception("Current distance more than max");

            var totalWeight = GetTotalWeight();
            var price = 0.0F;
            var weightPrice = _weightLimits.Where(x => x.Amount >= totalWeight).OrderBy(x => x.Amount).FirstOrDefault();
            if (weightPrice != null)
                price += weightPrice.PerUnit ? weightPrice.Price * totalWeight : weightPrice.Price;

            if (!_useDistance) return price;
            if (distance < 0) return price;

            var distancePrice = _distanceLimits.Where(x => x.Amount >= distance).OrderBy(x => x.Amount).FirstOrDefault();
            if (distancePrice != null)
                price += distancePrice.PerUnit ? distancePrice.Price * distance : distancePrice.Price;
            return price;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            this.DefaultIfNotSet();
            var option = new RangeWeightAndDistanceOption(_method, _totalPrice, _useDistance);
            float distance = 0;
            var shippingOption = _calculationParameters.ShippingOption as RangeWeightAndDistanceOption;
            if (shippingOption != null && shippingOption.Id == option.Id)
            {
                distance = option.Distance = shippingOption.Distance;
            }
            option.Rate = GetRate(distance);
            option.DeliveryTime = _deliveryTime;
            option.MaxDistance = _maxDistance;
            return new List<RangeWeightAndDistanceOption> { option };
        }
    }
}