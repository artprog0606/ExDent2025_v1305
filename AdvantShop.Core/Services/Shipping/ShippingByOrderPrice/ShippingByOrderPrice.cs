using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.ShippingByOrderPrice
{
    [ShippingKey("ShippingByOrderPrice")]
    public class ShippingByOrderPrice : BaseShipping, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval, IShippingRequiresSpecifyingTypeOfDelivery
    {
        private readonly List<ShippingPriceRange> _priceRanges;
        private readonly bool _dependsOnCartPrice;

        public override bool CurrencyAllAvailable { get { return true; } }

        public ShippingByOrderPrice(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _priceRanges = new List<ShippingPriceRange>();
            var ranges = _method.Params.ElementOrDefault(ShippingByOrderPriceTemplate.PriceRanges);
            if (ranges.IsNullOrEmpty())
                return;
            _priceRanges = GetRange(ranges);
            _priceRanges = _priceRanges.OrderBy(item => item.OrderPrice).ToList();
            _dependsOnCartPrice = _method.Params.ElementOrDefault(ShippingByOrderPriceTemplate.DependsOnCartPrice).TryParseBool();
        }

        private List<ShippingPriceRange> GetRange(string strRange)
        {
            var priceRanges = new List<ShippingPriceRange>();

            foreach (var item in strRange.Split(';'))
            {
                if (item.Split('=').Length == 2)
                {
                    priceRanges.Add(new ShippingPriceRange()
                    {
                        OrderPrice = item.Split('=')[0].TryParseFloat(), // / _preOrder.Currency.Rate,
                        ShippingPrice = item.Split('=')[1].TryParseFloat() // / _preOrder.Currency.Rate
                    });
                }
            }
            return priceRanges;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var productPrice = _items.Sum(x=> x.Price * x.Amount);
            var rate = GetRate(productPrice, _totalPrice);

            if (rate != -1)
            {
                var option = new BaseShippingOption(_method, _totalPrice)
                {
                    Rate = rate,
                    DeliveryTime = _method.Params.ElementOrDefault(ShippingByOrderPriceTemplate.DeliveryTime)
                };

                return new List<BaseShippingOption> { option };
            }

            return null;
        }

        private float GetRate(float cartPrice, float orderPrice)
        {
            float comparePrice = _dependsOnCartPrice ? cartPrice : orderPrice;
            float shippingPrice = -1;
            foreach (var range in _priceRanges)
            {
                if (comparePrice >= range.OrderPrice)
                {
                    shippingPrice = range.ShippingPrice;
                }
            }
            return shippingPrice;
        }
    }
}