using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetShippings
    {
        private readonly int _orderId;
        private string _country;
        private string _region;
        private string _district;
        private string _city;
        private string _zip;
        private string _street;
        private string _house;
        private string _structure;
        private string _apartment;
        private string _entrance;
        private string _floor;
        private readonly BaseShippingOption _shipping;
        private readonly bool _getAll;
        private readonly bool _applyPay;

        public GetShippings(int orderId, string country, string city, string district, string region, string zip, 
                            string street, string house, string structure, string apartment, string entrance, string floor,
                            BaseShippingOption shipping = null, bool getAll = true, bool applyPay = true)
        {
            _orderId = orderId;
            _country = country;
            _region = region;
            _district = district;
            _city = city;
            _zip = zip;
            _street = street;
            _house = house;
            _structure = structure;
            _apartment = apartment;
            _entrance = entrance;
            _floor = floor;
            _shipping = shipping;
            _getAll = getAll;
            _applyPay = applyPay;
        }

        public OrderShippingsModel Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return null;

            var model = new OrderShippingsModel();

            if (order.OrderItems == null || order.OrderItems.Count == 0)
                return model;

            if (!order.IsDraft && order.OrderCustomer != null)
            {
                _country = order.OrderCustomer.Country;
                _region = order.OrderCustomer.Region;
                _district = order.OrderCustomer.District;
                _city = order.OrderCustomer.City;
                _zip = order.OrderCustomer.Zip;
                _street = order.OrderCustomer.Street;
                _house = order.OrderCustomer.House;
                _structure = order.OrderCustomer.Structure;
                _apartment = order.OrderCustomer.Apartment;
                _entrance = order.OrderCustomer.Entrance;
                _floor = order.OrderCustomer.Floor;
            }

            var shippingCalculationParameters =
                ShippingCalculationConfigurator.Configure()
                                               .ByOrder(order)
                                               .WithCountry(_country ?? "")
                                               .WithRegion(_region ?? "")
                                               .WithDistrict(_district ?? "")
                                               .WithCity(_city ?? "")
                                               .WithStreet(_street ?? "")
                                               .WithHouse(_house ?? "")
                                               .WithStructure(_structure ?? "")
                                               .WithApartment(_apartment ?? "")
                                               .WithEntrance(_entrance ?? "")
                                               .WithFloor(_floor ?? "")
                                               .WithZip(_zip ?? "")
                                               .WithShippingOption(_shipping)
                                               .FromAdminArea()
                                               .Build();
            var manager = new ShippingManager(shippingCalculationParameters);
#if !DEBUG
            manager.TimeLimitMilliseconds = 20_000; // 20 seconds
#endif
            if (!_getAll)
                manager.PreferShippingOptionFromParameters();
            model.Shippings = manager.GetOptions();

            if (_applyPay && model.Shippings != null && model.Shippings.Count > 0)
            {
                if (order.PaymentMethod != null)
                {
                    var preCoast = shippingCalculationParameters.ItemsTotalPriceWithDiscounts;
                    foreach (var shippingOption in model.Shippings)
                    {
                        var paymentOption = order.PaymentMethod.GetOption(shippingOption, preCoast + shippingOption.FinalRate, order.OrderCustomer.CustomerType);
                        if (paymentOption != null)
                            shippingOption.ApplyPay(paymentOption);
                    }
                }
                else
                {
                    var paymentCalculationParameters =
                        PaymentCalculationConfigurator.Configure()
                                                      .ByOrder(order)
                                                      .Build();
                    if (paymentCalculationParameters.PaymentOption != null)
                        foreach (var shippingOption in model.Shippings)
                            shippingOption.ApplyPay(paymentCalculationParameters.PaymentOption);
                }

            }

            if (model.Shippings != null)
                foreach (var shipping in model.Shippings) {
                    // *** Warrning!!! First change currency
                    shipping.CurrentCurrency = order.OrderCurrency;
                    shipping.ManualRate = shipping.FinalRate;
                    // ***
                }

            model.CustomShipping = new BaseShippingOption()
            {
                Name = "",
                Rate = _shipping != null ? _shipping.ManualRate : 0,
                ManualRate = _shipping != null ? _shipping.ManualRate : 0,
                IsCustom = true
            };

            if (_shipping != null)
            {
                model.SelectShipping = _shipping;
            }
            else if (order.ShippingMethodId != 0)
            {
                var selectedShipping = GetSelectedShipping(order, model.Shippings);
                if (selectedShipping != null)
                {
                    if (order.OrderPickPoint != null)
                        selectedShipping.UpdateFromOrderPickPoint(order.OrderPickPoint);
                    model.SelectShipping = selectedShipping;
                }
            }
            else if(order.ShippingMethodId == 0)
            {
                model.CustomShipping.Name = order.ShippingMethodName;
                model.CustomShipping.Rate = order.ShippingCost;

                model.SelectShipping = model.CustomShipping;
            }

            return model;
        }

        private BaseShippingOption GetSelectedShipping(Order order, List<BaseShippingOption> shippings)
        {
            var selectedShipping = 
                (order.OrderPickPoint != null 
                    ? shippings.FirstOrDefault(x => x.MethodId == order.ShippingMethodId && x.Name == order.ArchivedShippingName && PickPointEquals(x, order.OrderPickPoint)) 
                    : null) ??
                shippings.FirstOrDefault(x => x.MethodId == order.ShippingMethodId && x.Name == order.ArchivedShippingName) ??
                shippings.FirstOrDefault(x => x.MethodId == order.ShippingMethodId);

            return selectedShipping;
        }

        private bool PickPointEquals(BaseShippingOption shipping, OrderPickPoint pickPoint)
        {
            if (pickPoint == null)
                return true;

            var option = shipping as IComparePickPoint;
            return option != null && option.ComparePickPoint(pickPoint);
        }
    }
}
