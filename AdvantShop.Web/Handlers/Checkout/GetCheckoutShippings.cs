using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Handlers.Checkout
{
    public sealed class GetCheckoutShippings
    {
        private readonly List<PreOrderItem> _preorderList;
        private CalculationVariants? _typeCalculationVariants;
        private readonly bool _usePassedTypeCalculationVariants;

        public GetCheckoutShippings(List<PreOrderItem> preorderList, CalculationVariants? typeCalculationVariants = null)
        {
            _preorderList = preorderList;
            _typeCalculationVariants = typeCalculationVariants;
        }
        
        public GetCheckoutShippings(CalculationVariants? typeCalculationVariants, bool usePassedTypeCalculationVariants)
        {
            _typeCalculationVariants = typeCalculationVariants;
            _usePassedTypeCalculationVariants = usePassedTypeCalculationVariants;
        }
        
        public GetCheckoutShippingsResponse Execute()
        {
            var options = new List<BaseShippingOption>();
            
            var current = MyCheckout.Factory(CustomerContext.CustomerId);
            
            if (!current.Data.HideShippig)
            {
                _typeCalculationVariants = GetCalculationVariants(_typeCalculationVariants, current);
                
                options = current.AvailableShippingOptions(_preorderList, _typeCalculationVariants);

                if (!string.IsNullOrEmpty(current.Data.PreSelectedShippingId))
                {
                    var selectedOption =
                        options.FirstOrDefault(x => x.Id == current.Data.PreSelectedShippingId) ??
                        options.FirstOrDefault(x => x.Id.StartsWith(current.Data.PreSelectedShippingId + "_"));
                    
                    if (selectedOption != null)
                    {
                        current.Data.SelectShipping = selectedOption;

                        if (current.Data.PreSelectedShippingPointId.IsNotEmpty() &&
                            selectedOption is ISelectShippingPoint pointOption)
                        {
                            pointOption.SelectShippingPoint(current.Data.PreSelectedShippingPointId);
                        }
                    }

                    current.Data.PreSelectedShippingId = null;
                    current.Data.PreSelectedShippingPointId = null;
                }

                if (current.Data.SelectShipping == null || !options.Any(x => x.Id == current.Data.SelectShipping.Id))
                    current.Data.SelectShipping = null;

                current.UpdateSelectShipping(_preorderList, current.Data.SelectShipping, options, _typeCalculationVariants, current.Data.WarehouseId);
            }
            else
            {
                options.Add(current.Data.SelectShipping);
            }

            return new GetCheckoutShippingsResponse
            {
                selectShipping = current.Data.SelectShipping,
                option = options,
                typeCalculationVariants = _typeCalculationVariants?.StrName()
            };
        }
        
        private CalculationVariants? GetCalculationVariants(CalculationVariants? typeCalculationVariants, MyCheckout current)
        {
            if (_usePassedTypeCalculationVariants)
                return typeCalculationVariants;
            
            if (!SettingsCheckout.SplitShippingByType)
                return null;
            
            if (typeCalculationVariants == null || typeCalculationVariants == CalculationVariants.All)
            {
                if (!string.IsNullOrEmpty(current.Data.PreSelectedShippingId))
                {
                    var shippingId = current.Data.PreSelectedShippingId.Split('_')[0].TryParseInt();
                    var shipping = ShippingMethodService.GetShippingMethod(shippingId);
                    
                    if (shipping?.TypeOfDelivery != null)
                        return shipping.TypeOfDelivery == EnTypeOfDelivery.SelfDelivery
                            ? CalculationVariants.PickPoint
                            : CalculationVariants.Courier;
                }
                
                return 
                    current.Data?.TypeCalculationVariants != null && current.Data.TypeCalculationVariants != CalculationVariants.All
                        ? current.Data.TypeCalculationVariants.Value
                        : SettingsCheckout.DefaultShippingType == EnTypeOfDelivery.SelfDelivery
                            ? CalculationVariants.PickPoint
                            : CalculationVariants.Courier;
            }

            return typeCalculationVariants;
        }
    }
}