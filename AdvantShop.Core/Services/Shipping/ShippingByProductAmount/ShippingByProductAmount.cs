using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.ShippingByProductAmount
{
    [ShippingKey("ShippingByProductAmount")]
    public class ShippingByProductAmount : BaseShipping, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval, IShippingRequiresSpecifyingTypeOfDelivery
    {
        private readonly List<ShippingAmountRange> _priceRanges;

        public override bool CurrencyAllAvailable { get { return true; } }

        public ShippingByProductAmount(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _priceRanges = GetRange();
            _priceRanges = _priceRanges.OrderBy(item => item.Amount).ToList();
        }

        private List<ShippingAmountRange> GetRange()
        {
            var priceRanges = new List<ShippingAmountRange>();

            var ranges = _method.Params.ElementOrDefault(ShippingByProductAmountTemplate.PriceRanges);
            if (ranges.IsNullOrEmpty())
                return priceRanges;
            
            foreach (var item in ranges.Split(';'))
            {
                var arr = item.Split('=');

                if (arr.Length != 2) continue;
                priceRanges.Add(new ShippingAmountRange()
                {
                    Amount = arr[0].TryParseFloat(),
                    ShippingPrice = arr[1].TryParseFloat()
                });
            }
            return priceRanges;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var rate = GetRate(_items.Sum(item => item.Amount));
            if (rate >= 0)
            {
                var option = new BaseShippingOption(_method, _totalPrice)
                {
                    Rate = rate,
                    DeliveryTime = _method.Params.ElementOrDefault(ShippingByProductAmountTemplate.DeliveryTime)
                };
                return new List<BaseShippingOption> { option };
            }

            return new List<BaseShippingOption>();
        }

        private float GetRate(float amount)
        {
            float shippingPrice = -1;
            foreach (var range in _priceRanges)
            {
                if (amount >= range.Amount)
                {
                    shippingPrice = range.ShippingPrice;
                }
            }
            return shippingPrice;
        }
    }
}