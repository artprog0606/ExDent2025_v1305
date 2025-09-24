using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Payment;

namespace AdvantShop.Shipping
{
    public abstract class BaseShippingWithWeight : BaseShipping
    {
        protected float _defaultWeight;
        protected ExtrachargeType _extrachargeTypeWeight { get; set; }
        protected float _extrachargeWeight { get; set; }


        protected BaseShippingWithWeight(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _defaultWeight = _method.Params.ElementOrDefault(DefaultWeightParams.DefaultWeight).TryParseFloat();
            _extrachargeTypeWeight = (ExtrachargeType)_method.Params.ElementOrDefault(DefaultWeightParams.ExtrachargeTypeWeight).TryParseInt();
            _extrachargeWeight = _method.Params.ElementOrDefault(DefaultWeightParams.ExtrachargeWeight).TryParseFloat();
        }


        private bool _callDefaultIfNotSet;
        protected virtual void DefaultIfNotSet()
        {
            _callDefaultIfNotSet = true;

            foreach (var item in _items)
                item.Weight = item.Weight == 0 ? _defaultWeight : item.Weight;

            if (_calculationParameters.TotalWeight == 0)
                _calculationParameters.TotalWeight = _defaultWeight;
        }

        public float GetTotalWeight(int rate = 1)
        {
            if (!_callDefaultIfNotSet)
                DefaultIfNotSet();

            var weight =
                _calculationParameters.TotalWeight != null
                    ? _calculationParameters.TotalWeight.Value
                    : _items.Sum(x => x.Weight*x.Amount);

            return (weight + GetExtracharge(_extrachargeTypeWeight, _extrachargeWeight, weight)) * rate;
        }

        protected float GetExtracharge(ExtrachargeType extrachargeType, float extracharge, float value)
        {
            if (extrachargeType == ExtrachargeType.Fixed)
                return extracharge;

            return extracharge * value / 100;
        }


        public override IEnumerable<BaseShippingOption> CalculateOptions(CalculationVariants calculationVariants)
        {
            if (!NeedToCalc(calculationVariants))
                return null;
            
            DefaultIfNotSet();
            return base.CalculateOptions(calculationVariants);
        }
    }
}