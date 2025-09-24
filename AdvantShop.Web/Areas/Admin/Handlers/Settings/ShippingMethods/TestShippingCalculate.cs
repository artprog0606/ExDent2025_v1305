using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Settings.ShippingMethods;
using AdvantShop.Web.Infrastructure.ActionResults;
using System;
using System.Collections.Generic;
using AdvantShop.Core.Common;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Settings.ShippingMethods
{
    public class TestShippingCalculate
    {
        private readonly ShippingCalculateModel _calculateModel;
        public TestShippingCalculate(ShippingCalculateModel calculateModel)
        {
            _calculateModel = calculateModel;
        }

        public CommandResult Execute()
        {
            var shippingMethod = ShippingMethodService.GetShippingMethod(_calculateModel.Id).DeepCloneJson();
            if (shippingMethod == null)
                return new CommandResult { Errors = new List<string> { "Не найден способ доставки" }, Result = false };

            var config = ShippingCalculationConfigurator.Configure()
                                                        .FromAdminArea();
            if (_calculateModel.PreOrder != null)
            {
                config
                   .WithCountry(_calculateModel.PreOrder.CountryDest)
                   .WithRegion(_calculateModel.PreOrder.RegionDest)
                   .WithCity(_calculateModel.PreOrder.CityDest)
                   .WithZip(_calculateModel.PreOrder.Zip)
                   .WithDistrict(_calculateModel.PreOrder.District)
                   .WithStreet(_calculateModel.PreOrder.Street)
                   .WithApartment(_calculateModel.PreOrder.Apartment)
                   .WithEntrance(_calculateModel.PreOrder.Entrance)
                   .WithFloor(_calculateModel.PreOrder.Floor)
                   .WithHouse(_calculateModel.PreOrder.House)
                   .WithStructure(_calculateModel.PreOrder.Structure);
            }

            PreOrderItem item = new PreOrderItem();
            if (_calculateModel.Item != null)
            {
                item.Amount = _calculateModel.Item.Amount;
                item.Width = _calculateModel.Item.Width;
                item.Height = _calculateModel.Item.Height;
                item.Price = _calculateModel.Item.Price;
                item.Length = _calculateModel.Item.Length;
                item.Weight = _calculateModel.Item.Weight;
            }

            config.WithPreOrderItems(new List<PreOrderItem> { item });

            config.WithCurrency(
                !string.IsNullOrEmpty(_calculateModel.Iso3)
                    ? CurrencyService.GetCurrencyByIso3(_calculateModel.Iso3)
                    : CurrencyService.CurrentCurrency);

            if (!_calculateModel.MarginEnabled)
            {
                shippingMethod.ExtrachargeInNumbers = 0;
                shippingMethod.ExtrachargeInPercents = 0;
            }
            if (!_calculateModel.ExtrachargeCargoEnabled)
            {
                if (shippingMethod.Params.ContainsKey(DefaultWeightParams.ExtrachargeWeight))
                    shippingMethod.Params[DefaultWeightParams.ExtrachargeWeight] = "0";
                if (shippingMethod.Params.ContainsKey(DefaultCargoParams.ExtrachargeCargo))
                    shippingMethod.Params[DefaultCargoParams.ExtrachargeCargo] = "0";
            }
            var type = ReflectionExt.GetTypeByAttributeValue<ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, shippingMethod.ShippingType);
            var shipping = (BaseShipping)Activator.CreateInstance(type, shippingMethod, config.Build());
            var result = _calculateModel.GeoEnabled ? shipping.CalculateOptions(CalculationVariants.All) : shipping.CalculateOptionsWithoutGeo(CalculationVariants.All);
            return new CommandResult { Obj = result?.Where(x => x != null).Select(x => new
            {
                x.Name,
                x.FormatRate,
                x.DeliveryTime
            }), Result = true };
        }
    }
}
