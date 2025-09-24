using AdvantShop.Core.Caching;
using AdvantShop.Repository.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping
{
    public abstract class BaseShipping : ICalculateShipping, IReceiveShippingPoints
    {
        protected readonly ShippingMethod _method;
        protected readonly ShippingCalculationParameters _calculationParameters;
        protected readonly List<PreOrderItem> _items;
        protected readonly float _totalPrice;
        protected readonly float _totalPriceWithoutBonuses;
        private bool _disableReplaceGeo = false;

        protected BaseShipping()
        {
        }

        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null</exception>
        /// <exception cref="ArgumentException">If PreOrderItems property of <paramref name="calculationParameters"/> is null</exception>
        /// <exception cref="ArgumentException">If ShippingCurrency property of <paramref name="method"/> is not null and Currency property of <paramref name="calculationParameters"/> is null</exception>
        protected BaseShipping(ShippingMethod method, ShippingCalculationParameters calculationParameters)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
            
            /*  calculationParameters может быть null,
                т.к. метод может создаваться не для рассчета
                (для взятия каких-то его данных/свойств) */
            _calculationParameters = calculationParameters;
            
            if (_calculationParameters != null)
            {
                _items = _calculationParameters.PreOrderItems ?? throw new ArgumentException($"The {nameof(_calculationParameters.PreOrderItems)} property of the {nameof(_calculationParameters)} parameter is null");
                _totalPrice = _calculationParameters.ItemsTotalPriceWithDiscounts;
                _totalPriceWithoutBonuses = _calculationParameters.ItemsTotalPriceWithDiscountsWithoutBonuses;
                
                if (_method.ShippingCurrency != null)
                {
                    _calculationParameters.Currency = _calculationParameters.Currency ?? throw new ArgumentException($"The {nameof(_calculationParameters.Currency)} property of the {nameof(_calculationParameters)} parameter is null");; 
                    if (_calculationParameters.Currency.Rate != _method.ShippingCurrency.Rate)
                    {
                        _items.ForEach(item => item.Price = CurrencyService.ConvertCurrency(item.Price, _method.ShippingCurrency.Rate, _calculationParameters.Currency.Rate));
                        
                        _calculationParameters.ItemsTotalPriceWithDiscounts = CurrencyService.ConvertCurrency(_calculationParameters.ItemsTotalPriceWithDiscounts, _method.ShippingCurrency.Rate, _calculationParameters.Currency.Rate);
                        _totalPrice = _calculationParameters.ItemsTotalPriceWithDiscounts;

                        _calculationParameters.ItemsTotalPriceWithDiscountsWithoutBonuses = CurrencyService.ConvertCurrency(_calculationParameters.ItemsTotalPriceWithDiscountsWithoutBonuses, _method.ShippingCurrency.Rate, _calculationParameters.Currency.Rate);
                        _totalPriceWithoutBonuses = _calculationParameters.ItemsTotalPriceWithDiscountsWithoutBonuses;
                        
                        _calculationParameters.Currency = _method.ShippingCurrency;
                    }
                }
            }
            
            _items = _items ?? new List<PreOrderItem>();
        }

        public virtual bool CurrencyAllAvailable => false;

        public virtual string[] CurrencyIso3Available => null;
        public virtual EnTypeOfDelivery? TypeOfDelivery => null;

        protected void ReplaceGeo()
        {
            if (_disableReplaceGeo)
                return;
            
            var cacheKey = "ShippingReplaceGeo-PreOrder_" + _method.ShippingType +
                "_" + (_calculationParameters.Country ?? string.Empty).ToLower().GetHashCode() +
                "_" + (_calculationParameters.Region ?? string.Empty).ToLower().GetHashCode() +
                "_" + (_calculationParameters.City ?? string.Empty).ToLower().GetHashCode() +
                "_" + (_calculationParameters.District ?? string.Empty).ToLower().GetHashCode();

            var replacedInfo = CacheManager.Get(cacheKey, 5, () =>
                {
                    string outCountry, outRegion, outDistrict, outCity, outZip;
                    var replaced = ShippingReplaceGeoService.ReplaceGeo(
                        _method.ShippingType,
                        _calculationParameters.Country, _calculationParameters.Region, _calculationParameters.District, _calculationParameters.City, _calculationParameters.Zip,
                        out outCountry, out outRegion, out outDistrict, out outCity, out outZip);

                    return replaced
                        ? new Tuple<ShippingCalculationParameters>(new ShippingCalculationParameters
                            {
                                Country = outCountry,
                                Region = outRegion,
                                District = outDistrict,
                                City = outCity,
                                Zip = outZip,
                            })
                        : new Tuple<ShippingCalculationParameters>(null);// Tuple, т.к. в кэш null не пишется, а нужно запоминать и отрицательный результат тоже
                });

            if (replacedInfo != null && replacedInfo.Item1 != null)
            {
                _calculationParameters.Country = replacedInfo.Item1.Country;
                _calculationParameters.Region = replacedInfo.Item1.Region;
                _calculationParameters.District = replacedInfo.Item1.District;
                _calculationParameters.City = replacedInfo.Item1.City;
                _calculationParameters.Zip = replacedInfo.Item1.Zip;
            }
        }

        protected bool NeedToCalc(CalculationVariants calculationVariants)
        {
            // метод не требует уточнения типа доставки или расчет всего
            if (!_method.RequiresSpecifyingTypeOfDelivery
                || calculationVariants.HasFlag(CalculationVariants.All))
            {
                return true;
            }

            // не указан тип доставки
            if (_method.TypeOfDelivery is null)
                return false;
            
            // это курьерская доставка, но считать ее не надо
            if (_method.TypeOfDelivery is EnTypeOfDelivery.Courier
                && !calculationVariants.HasFlag(CalculationVariants.Courier))
                return false;
            
            // это самовывоз, но считать ее не надо
            if (_method.TypeOfDelivery is EnTypeOfDelivery.SelfDelivery
                && !calculationVariants.HasFlag(CalculationVariants.PickPoint))
                return false;

            return true;
        }

        protected virtual IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<BaseShippingOption> CalculateOptions(CalculationVariants calculationVariants)
        {
            if (!NeedToCalc(calculationVariants))
                return null;
            
            ReplaceGeo();
            return CalcOptions(calculationVariants);
        }

        public IEnumerable<BaseShippingOption> CalculateOptionsWithoutGeo(CalculationVariants calculationVariants)
        {
            var prevDisable = _disableReplaceGeo;
            _disableReplaceGeo = true;
            var options = CalculateOptions(calculationVariants);
            _disableReplaceGeo = prevDisable;

            return options;
        }

        protected virtual IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId) => null;

        public IEnumerable<BaseShippingOption> CalculateOptionsToPoint(string pointId)
        {
            return CalcOptionsToPoint(pointId);
        }

        public virtual IEnumerable<BaseShippingPoint> CalcShippingPoints(
            float topLeftLatitude, float topLeftLongitude,
            float bottomRightLatitude, float bottomRightLongitude) => null;

        public IEnumerable<BaseShippingPoint> CalculateShippingPoints(float[,] coordinatesOfBounds, TypeBonds typeBonds)
        {
            if (coordinatesOfBounds.GetLength(0) != 2
                || coordinatesOfBounds.GetLength(1) != 2)
                throw new ArgumentException("Возможен только массив float[2,2].", nameof(coordinatesOfBounds));

            if (typeBonds == TypeBonds.BottomToTop)
            {
                // конвертируем в TopToBottom
                float temp = coordinatesOfBounds[0, 0];
                coordinatesOfBounds[0, 0] = coordinatesOfBounds[1, 0];
                coordinatesOfBounds[1, 0] = temp;
            }

            return CalcShippingPoints(
                topLeftLatitude: coordinatesOfBounds[0, 0], 
                topLeftLongitude: coordinatesOfBounds[0, 1], 
                bottomRightLatitude: coordinatesOfBounds[1, 0],
                bottomRightLongitude: coordinatesOfBounds[1, 1]);
        }

        public virtual IEnumerable<BaseShippingPoint> LoadShippingPoints() => null;

        public IEnumerable<BaseShippingPoint> GetShippingPoints()
        {
            return LoadShippingPoints();
        }

        public virtual BaseShippingPoint LoadShippingPointInfo(string pointId) => null;

        public BaseShippingPoint GetShippingPointInfo(string pointId)
        {
            return LoadShippingPointInfo(pointId);
        }
    }
}