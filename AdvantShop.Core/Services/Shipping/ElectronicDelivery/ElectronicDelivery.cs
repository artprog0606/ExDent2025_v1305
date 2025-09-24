//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.ElectronicDelivery
{
    [ShippingKey("ElectronicDelivery")]
    public class ElectronicDelivery : BaseShipping, IShippingNoUseExtraDeliveryTime, IShippingNoUseExtracharge, IShippingMethodsOnlyForDigitalProducts
    {
        private readonly float _shippingPrice;
        public override EnTypeOfDelivery? TypeOfDelivery => EnTypeOfDelivery.SelfDelivery;

        public override bool CurrencyAllAvailable { get { return true;} }

        public ElectronicDelivery(ShippingMethod method, ShippingCalculationParameters calculationParameters)
            : base(method, calculationParameters)
        {
            _shippingPrice = method.Params.ElementOrDefault(ElectronicDeliveryTemplate.ShippingPrice).TryParseFloat();
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            if (!calculationVariants.HasFlag(CalculationVariants.PickPoint))
                return null;

            if (_items == null || _items.Any(item => !item.IsDigital))
                return null;

            var option = new BaseShippingOption(_method, _totalPrice)
            {
                Rate = _shippingPrice,
                HideAddressBlock = true
            };
            return new List<BaseShippingOption> { option };
        }
    }
}
