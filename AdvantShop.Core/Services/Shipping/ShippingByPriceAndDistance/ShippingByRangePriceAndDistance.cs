//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping.ShippingByRangePriceAndDistance;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.RangePriceAndDistanceOption
{
    [ShippingKey("ShippingByPriceAndDistance")]
    public class ShippingByPriceAndDistance : BaseShipping, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval, IShippingRequiresSpecifyingTypeOfDelivery
    {
        private readonly List<PriceLimit> _orderLimits;
        private readonly string _deliveryTime;
        private readonly int _maxDistance;
        private readonly int _defaultDistance;

        public override bool CurrencyAllAvailable { get { return true; } }

        public ShippingByPriceAndDistance(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _orderLimits = JsonConvert.DeserializeObject<List<PriceLimit>>(_method.Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.PriceLimit));
            _deliveryTime = _method.Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.DeliveryTime);
            _maxDistance = _method.Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.MaxDistance).TryParseInt();
            _defaultDistance = _method.Params.ElementOrDefault(ShippingByRangePriceAndDistanceTemplate.DefaultDistance).TryParseInt();
        }

        private float GetRate(float distance)
        {
            if (distance > _maxDistance) 
                throw  new Exception("Current Distance more that max");

            if (_orderLimits == null || _orderLimits.Count == 0)
                throw new Exception("Delivery method is not configured: Shipping By Price And Distance");

            var totalCost = _items.Sum(item => item.Amount * item.Price);
            
            var price = 0.0F;
            
            var orderPrice = _orderLimits.Where(x => x.OrderPrice >= totalCost && (distance > x.Distance || x.Distance == 0)).OrderBy(x => x.OrderPrice).ThenByDescending(x => x.Distance).FirstOrDefault();
            if (orderPrice != null)
            {
                price += orderPrice.Price + (orderPrice.PerUnit ? (distance - orderPrice.Distance) * orderPrice.PriceDistance : orderPrice.PriceDistance );
            }

            return price;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var option = new RangePriceAndDistanceOption(_method, _totalPrice);
            float distance = 0;

            var shippingOption = _calculationParameters.ShippingOption as RangePriceAndDistanceOption;
            if (shippingOption != null && shippingOption.Id == option.Id)
            {
                distance = option.Distance = shippingOption.Distance;
            }

            option.Rate = GetRate(distance);
            option.DeliveryTime = _deliveryTime;
            option.MaxDistance = _maxDistance;
            return new List<RangePriceAndDistanceOption> {option};
        }
    }
}