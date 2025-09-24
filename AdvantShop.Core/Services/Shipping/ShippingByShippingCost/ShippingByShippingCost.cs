//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.ShippingByShippingCost
{
    [ShippingKey("ShippingByShippingCost")]
    public class ShippingByShippingCost : BaseShipping, IShippingNoUseCurrency, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval, IShippingRequiresSpecifyingTypeOfDelivery
    {
        private readonly bool _byMaxShippingCost;
        private readonly bool _useAmount;
        private readonly float _defaultShippingCost;

        public ShippingByShippingCost(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _byMaxShippingCost = _method.Params.ElementOrDefault(ShippingByShippingCostTemplate.ByMaxShippingCost).TryParseBool();
            _useAmount = _method.Params.ElementOrDefault(ShippingByShippingCostTemplate.UseAmount).TryParseBool();
            _defaultShippingCost = _method.Params.ElementOrDefault(ShippingByShippingCostTemplate.DefaultShippingCost).TryParseFloat(0f);
        }

        private float GetRate()
        {
            if (_items.Count == 0)
                return 0F;

            _items.Where(x => x.ShippingPrice == 0f).ForEach(x => x.ShippingPrice = _defaultShippingCost);

            if (!_useAmount)
            {
                return _byMaxShippingCost
                    ? _items.Max(item => item.ShippingPrice)
                    : _items.Sum(item => item.ShippingPrice);
            }
            return _byMaxShippingCost
                ? _items.Max(item => item.ShippingPrice * item.Amount)
                : _items.Sum(item => item.ShippingPrice * item.Amount);
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var option = new BaseShippingOption(_method, _totalPrice)
            {
                Rate = GetRate(),
                DeliveryTime = _method.Params.ElementOrDefault(ShippingByShippingCostTemplate.DeliveryTime)
            };
            return new List<BaseShippingOption> { option };
        }
    }
}