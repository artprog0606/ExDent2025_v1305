using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Payment;
using AdvantShop.Repository;

namespace AdvantShop.Shipping
{
    public abstract class BaseShippingWithCargo : BaseShippingWithWeight
    {
        protected float _defaultLength;
        protected float _defaultWidth;
        protected float _defaultHeight;
        protected ExtrachargeType _extrachargeTypeCargo { get; set; }
        protected float _extrachargeCargo { get; set; }

        protected BaseShippingWithCargo(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            //base call
            //_defaultWeight = _method.Params.ElementOrDefault(DefaultWeightParams.DefaultWeight).TryParseFloat();
            _defaultLength = _method.Params.ElementOrDefault(DefaultCargoParams.DefaultLength).TryParseFloat();
            _defaultWidth = _method.Params.ElementOrDefault(DefaultCargoParams.DefaultWidth).TryParseFloat();
            _defaultHeight = _method.Params.ElementOrDefault(DefaultCargoParams.DefaultHeight).TryParseFloat();
            _extrachargeTypeCargo = (ExtrachargeType)_method.Params.ElementOrDefault(DefaultCargoParams.ExtrachargeTypeCargo).TryParseInt();
            _extrachargeCargo = _method.Params.ElementOrDefault(DefaultCargoParams.ExtrachargeCargo).TryParseFloat();
        }

        private bool CallDefaultIfNotSet;
        protected override void DefaultIfNotSet()
        {
            CallDefaultIfNotSet = true;

            base.DefaultIfNotSet();
            foreach (var item in _items)
            {
                item.Height = item.Height == 0 ? _defaultHeight : item.Height;
                item.Length = item.Length == 0 ? _defaultLength : item.Length;
                item.Width = item.Width == 0 ? _defaultWidth : item.Width;
                //base call
                //item.Weight = item.Weight == 0 ? _defaultWeight : item.Weight;
            }

            //base call
            //if (_preOrder.TotalWeight == 0)
            //    _preOrder.TotalWeight = _defaultWeight;

            if (_calculationParameters.TotalHeight == 0)
                _calculationParameters.TotalHeight = _defaultHeight;
            if (_calculationParameters.TotalLength == 0)
                _calculationParameters.TotalLength = _defaultLength;
            if (_calculationParameters.TotalWidth == 0)
                _calculationParameters.TotalWidth = _defaultWidth;
        }

        public float[] GetDimensions(float rate = 1)
        {
            if (!CallDefaultIfNotSet)
                DefaultIfNotSet();

            // конвертацию по rate делаем сами, т.к. иначе последующая фиксированная надбавка габаритов
            // будет произведена не корректно (без конвертации)
            return MeasureHelper.GetDimensions(_calculationParameters, _defaultHeight, _defaultWidth, _defaultLength)
                .Select(x => (x + GetExtracharge(_extrachargeTypeCargo, _extrachargeCargo, x)) / rate)
                .ToArray();
        }

        public override IEnumerable<BaseShippingOption> CalculateOptions(CalculationVariants calculationVariants)
        {
            if (!NeedToCalc(calculationVariants))
                return null;
            
            return base.CalculateOptions(calculationVariants);
        }
    }
}