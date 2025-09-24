using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;

namespace AdvantShop.Core.Common
{
    public interface IBaseFabricCalculationParameters<out T> where T: class
    {
        T Build();
    }

    #region ShippingCalculation

    public interface IConfiguratorShippingCalculation : IBaseFabricCalculationParameters<ShippingCalculationParameters>
    {
        IConfiguratorShippingCalculation WithCountry(string country);
        IConfiguratorShippingCalculation WithRegion(string region);
        IConfiguratorShippingCalculation WithDistrict(string district);
        IConfiguratorShippingCalculation WithCity(string city);
        IConfiguratorShippingCalculation WithStreet(string street);
        IConfiguratorShippingCalculation WithHouse(string house);
        IConfiguratorShippingCalculation WithStructure(string structure);
        IConfiguratorShippingCalculation WithApartment(string apartment);
        IConfiguratorShippingCalculation WithEntrance(string entrance);
        IConfiguratorShippingCalculation WithFloor(string floor);
        IConfiguratorShippingCalculation WithZip(string zip);
        IConfiguratorShippingCalculation WithLocation(float? longitude, float? latitude);
        IConfiguratorShippingCalculation WithShippingOption(BaseShippingOption shippingOption);
        IConfiguratorShippingCalculation WithPreOrderItems(List<PreOrderItem> preOrderItems);
        IConfiguratorShippingCalculation WithCurrency(Currency currency);
        IConfiguratorShippingCalculation WithItemsTotalPriceWithDiscounts(float itemsTotalPriceWithDiscounts);
        IConfiguratorShippingCalculation WithItemsTotalPriceWithDiscountsWithoutBonuses(float itemsTotalPriceWithDiscountsWithoutBonuses);
    
        IConfiguratorShippingCalculation WithTotalWeight(float? totalWeight);
        IConfiguratorShippingCalculation WithTotalLength(float? totalLength);
        IConfiguratorShippingCalculation WithTotalWidth(float? totalWidth);
        IConfiguratorShippingCalculation WithTotalHeight(float? totalHeight);
        IConfiguratorShippingCalculation WithBonusCard(Guid? bonusCardId);
        IConfiguratorShippingCalculation BonusUse(float? appliedBonuses);

        IConfiguratorShippingCalculation FromAdminArea();
        IConfiguratorShippingCalculation ShowOnlyInDetails();
        IConfiguratorShippingCalculation ByShoppingCart(ShoppingCart shoppingCart);
        IConfiguratorShippingCalculation ByOrder(Order order);
        IConfiguratorShippingCalculation ByOrder(Order order, bool actualizeShipping);
        IConfiguratorShippingCalculation ByMyCheckout(MyCheckout myCheckout);
    }

    #endregion ShippingCalculation

    #region PaymentCalculation

    public interface IConfiguratorPaymentCalculation : IBaseFabricCalculationParameters<PaymentCalculationParameters>
    {
        IConfiguratorPaymentCalculation WithCountry(string country);
        IConfiguratorPaymentCalculation WithRegion(string region);
        IConfiguratorPaymentCalculation WithDistrict(string district);
        IConfiguratorPaymentCalculation WithCity(string city);
        // IConfiguratorPaymentCalculation WithAddress(string address);
        IConfiguratorPaymentCalculation WithZip(string zip);
        IConfiguratorPaymentCalculation WithCustomerType(CustomerType? customerType);
        IConfiguratorPaymentCalculation WithShippingOption(BaseShippingOption shippingOption);
        IConfiguratorPaymentCalculation WithPaymentOption(BasePaymentOption paymentOption);
        IConfiguratorPaymentCalculation WithPreOrderItems(List<PreOrderItem> preOrderItems);
        IConfiguratorPaymentCalculation WithItemsTotalPriceWithDiscounts(float itemsTotalPriceWithDiscounts);
        IConfiguratorPaymentCalculation WithCertificate(GiftCertificate certificate);
        IConfiguratorPaymentCalculation WithBonusCard(Guid? bonusCardId);
        IConfiguratorPaymentCalculation BonusUse(float? appliedBonuses);

        IConfiguratorPaymentCalculation ByShoppingCart(ShoppingCart shoppingCart);
        IConfiguratorPaymentCalculation ByOrder(Order order);
        IConfiguratorPaymentCalculation ByOrder(Order order, bool actualizeShippingAndPayment);
        IConfiguratorPaymentCalculation ByMyCheckout(MyCheckout myCheckout);
    }

    #endregion PaymentCalculation

    public class BaseFabricCalculationParameters : IConfiguratorShippingCalculation, IConfiguratorPaymentCalculation
    {
        private string _country;
        private string _region;
        private string _district;
        private string _city;
        private string _street;
        private string _house;
        private string _structure;
        private string _apartment;
        private string _entrance;
        private string _floor;
        private string _zip;
        private float? _longitude;
        private float? _latitude;
        private CustomerType? _customerType;
        private BaseShippingOption _shippingOption;
        private BasePaymentOption _paymentOption;
        private List<PreOrderItem> _preOrderItems;
        private Currency _currency;
        private GiftCertificate _certificate;
        private ShoppingCart _shoppingCart;
        private Order _order;
        private float? _shippingItemsTotalPriceWithDiscounts;
        private float? _shippingItemsTotalPriceWithDiscountsWithoutBonuses;
        private float? _paymentItemsTotalPriceWithDiscounts;
        private float? _totalWeight;
        private float? _totalLength;
        private float? _totalWidth;
        private float? _totalHeight;
        private bool? _isFromAdminArea;
        private bool? _showOnlyInDetails;
        private Guid? _bonusCardId;
        private float? _appliedBonuses;

        private BaseFabricCalculationParameters WithCountry(string country)
        {
            _country = country;
            return this;
        }

        private BaseFabricCalculationParameters WithRegion(string region)
        {
            _region = region;
            return this;
        }

        private BaseFabricCalculationParameters WithDistrict(string district)
        {
            _district = district;
            return this;
        }

        private BaseFabricCalculationParameters WithCity(string city)
        {
            _city = city;
            return this;
        }

        private BaseFabricCalculationParameters WithStreet(string street)
        {
            _street = street;
            return this;
        }

        private BaseFabricCalculationParameters WithHouse(string house)
        {
            _house = house;
            return this;
        }

        private BaseFabricCalculationParameters WithStructure(string structure)
        {
            _structure = structure;
            return this;
        }

        private BaseFabricCalculationParameters WithApartment(string apartment)
        {
            _apartment = apartment;
            return this;
        }

        private BaseFabricCalculationParameters WithEntrance(string entrance)
        {
            _entrance = entrance;
            return this;
        }

        private BaseFabricCalculationParameters WithFloor(string floor)
        {
            _floor = floor;
            return this;
        }

        private BaseFabricCalculationParameters WithZip(string zip)
        {
            _zip = zip;
            return this;
        }

        private BaseFabricCalculationParameters WithLocation(float? longitude, float? latitude)
        {
            if (longitude.HasValue
                && latitude.HasValue)
            {
                _longitude = longitude;
                _latitude = latitude;
            }
            else
            {
                _longitude = null;
                _latitude = null;
            }

            return this;
        }

        private BaseFabricCalculationParameters WithCustomerType(CustomerType? customerType)
        {
            _customerType = customerType;
            return this;
        }

        private BaseFabricCalculationParameters WithShippingOption(BaseShippingOption shippingOption)
        {
            _shippingOption = shippingOption;
            return this;
        }

        private BaseFabricCalculationParameters WithPaymentOption(BasePaymentOption paymentOption)
        {
            _paymentOption = paymentOption;
            return this;
        }

        private BaseFabricCalculationParameters WithPreOrderItems(List<PreOrderItem> preOrderItems)
        {
            _preOrderItems = preOrderItems;
            return this;
        }

        private BaseFabricCalculationParameters WithCurrency(Currency currency)
        {
            _currency = currency;
            return this;
        }

        private BaseFabricCalculationParameters WithCertificate(GiftCertificate certificate)
        {
            _certificate = certificate;
            return this;
        }

        private BaseFabricCalculationParameters WithShippingItemsTotalPriceWithDiscounts(float shippingItemsTotalPriceWithDiscounts)
        {
            _shippingItemsTotalPriceWithDiscounts = shippingItemsTotalPriceWithDiscounts;
            return this;
        }
        
        private BaseFabricCalculationParameters WithShippingItemsTotalPriceWithDiscountsWithoutBonuses(float shippingItemsTotalPriceWithDiscountsWithoutBonuses)
        {
            _shippingItemsTotalPriceWithDiscountsWithoutBonuses = shippingItemsTotalPriceWithDiscountsWithoutBonuses;
            return this;
        }

        private BaseFabricCalculationParameters WithPaymentItemsTotalPriceWithDiscounts(float paymentItemsTotalPriceWithDiscounts)
        {
            _paymentItemsTotalPriceWithDiscounts = paymentItemsTotalPriceWithDiscounts;
            return this;
        }


        private BaseFabricCalculationParameters WithTotalWeight(float? totalWeight)
        {
            _totalWeight = totalWeight;
            return this;
        }

        private BaseFabricCalculationParameters WithTotalLength(float? totalLength)
        {
            _totalLength = totalLength;
            return this;
        }

        private BaseFabricCalculationParameters WithTotalWidth(float? totalWidth)
        {
            _totalWidth = totalWidth;
            return this;
        }

        private BaseFabricCalculationParameters WithTotalHeight(float? totalHeight)
        {
            _totalHeight = totalHeight;
            return this;
        }
        
        private BaseFabricCalculationParameters WithBonusCard(Guid? bonusCardId)
        {
            _bonusCardId = bonusCardId;
            return this;
        }

        private BaseFabricCalculationParameters BonusUse(float? appliedBonuses)
        {
            _appliedBonuses = appliedBonuses;
            return this;
        }

        private BaseFabricCalculationParameters FromAdminArea()
        {
            _isFromAdminArea = true;
            return this;
        }

        private BaseFabricCalculationParameters ShowOnlyInDetails()
        {
            _showOnlyInDetails = true;
            return this;
        }

        private void ClearByObjects()
        {
            _shoppingCart = null;
            _order = null;
        }

        private BaseFabricCalculationParameters ByShoppingCart(ShoppingCart shoppingCart)
        {
            ClearByObjects();
            _shoppingCart = shoppingCart;
            
            WithPreOrderItems(shoppingCart?.Select(x => new PreOrderItem(x)).ToList());
            WithCertificate(shoppingCart?.Certificate);
            // не выставляем итоговую стоимоитьс, т.к. может быть еще применение бонусов
            return this;
        }

        private BaseFabricCalculationParameters ByOrder(Order order) => ByOrder(order, false);
        private BaseFabricCalculationParameters ByOrder(Order order, bool actualizeShippingAndPayment)
        {
            ClearByObjects();
            _order = order ?? throw new ArgumentNullException(nameof(order));
            
            WithCountry(order.OrderCustomer?.Country);
            WithRegion(order.OrderCustomer?.Region);
            WithDistrict(order.OrderCustomer?.District);
            WithCity(order.OrderCustomer?.City);
            WithZip(order.OrderCustomer?.Zip);
            WithStreet(order.OrderCustomer?.Street);
            WithHouse(order.OrderCustomer?.House);
            WithStructure(order.OrderCustomer?.Structure);
            WithApartment(order.OrderCustomer?.Apartment);
            WithEntrance(order.OrderCustomer?.Entrance);
            WithFloor(order.OrderCustomer?.Floor);

            WithCustomerType(order.OrderCustomer?.CustomerType);
            
            WithCurrency(order.OrderCurrency);
            
            WithTotalWeight(order.TotalWeight);
            WithTotalLength(order.TotalLength);
            WithTotalWidth(order.TotalWidth);
            WithTotalHeight(order.TotalHeight);
            
            BaseShippingOption shippingOption;
            if (actualizeShippingAndPayment && order.ShippingMethod != null)
            {
                shippingOption = new BaseShippingOption(order.ShippingMethod, order.Sum - order.ShippingCostWithDiscount);

                shippingOption.Rate = order.ShippingCost;
                shippingOption.IsAvailablePaymentCashOnDelivery = order.AvailablePaymentCashOnDelivery;
                shippingOption.IsAvailablePaymentPickPoint = order.AvailablePaymentPickPoint;

                // сбрасываем настройки по наценке, т.к. order.ShippingCost 
                // содержит уже наценку и вызов FinalRate приведет к повтоной наценке
                shippingOption.UseExtracharge = default(bool);
                shippingOption.ExtrachargeInNumbers = default(float);
                shippingOption.ExtrachargeInPercents = default(float);
                shippingOption.ExtrachargeFromOrder = default(bool);

                WithShippingOption(shippingOption);
            }
            else
            {
                shippingOption = new BaseShippingOption();

                shippingOption.MethodId = order.ShippingMethodId;
                shippingOption.Name = order.ArchivedShippingName;
                shippingOption.Rate = order.ShippingCost;
                shippingOption.PreCost = order.Sum - order.ShippingCostWithDiscount;
                shippingOption.IsAvailablePaymentCashOnDelivery = order.AvailablePaymentCashOnDelivery;
                shippingOption.IsAvailablePaymentPickPoint = order.AvailablePaymentPickPoint;
                
                WithShippingOption(shippingOption);
            }
     
            BasePaymentOption paymentOption = null; 
            if (actualizeShippingAndPayment && order.PaymentMethod != null)
                paymentOption = order.PaymentMethod.GetOption(shippingOption, order.Sum - order.PaymentCost, _customerType);// может вернуть null
            
            if (paymentOption == null)
            {
                paymentOption = order.PaymentDetails == null || (!order.PaymentDetails.IsCashOnDeliveryPayment && !order.PaymentDetails.IsPickPointPayment)
                    ? new BasePaymentOption()
                    : order.PaymentDetails.IsCashOnDeliveryPayment
                        ? (BasePaymentOption)new CashOnDeliverytOption()
                        : new PickPointOption();

                paymentOption.Id = order.PaymentMethodId;
                paymentOption.Name = order.ArchivedPaymentName;
                paymentOption.Rate = order.PaymentCost;
            }

            WithPaymentOption(paymentOption);

            WithPreOrderItems(order.OrderItems.Select(x => new PreOrderItem(x)).ToList());
            
            // по заказу стоимость товаров является итоговой, поэтому сразу выставляем
            var productsPrice = _preOrderItems.Sum(x => x.Price * x.Amount);
            var totalDiscount = order.TotalDiscount;
            WithShippingItemsTotalPriceWithDiscounts(productsPrice - totalDiscount - order.BonusCost);
            WithShippingItemsTotalPriceWithDiscountsWithoutBonuses(productsPrice - totalDiscount);
            WithPaymentItemsTotalPriceWithDiscounts(productsPrice - totalDiscount - order.BonusCost);

            WithCertificate(
                order.Certificate != null
                    ? GiftCertificateService.GetCertificateByCode(order.Certificate.Code)
                    : null);
            
            return this;
        }

        public BaseFabricCalculationParameters ByMyCheckout(MyCheckout myCheckout)
        {
            ClearByObjects();
            myCheckout = myCheckout ?? throw new ArgumentNullException(nameof(myCheckout));
            
            WithCountry(myCheckout.Data?.Contact?.Country);
            WithRegion(myCheckout.Data?.Contact?.Region);
            WithDistrict(myCheckout.Data?.Contact?.District);
            WithCity(myCheckout.Data?.Contact?.City);
            WithZip(myCheckout.Data?.Contact?.Zip);
            WithStreet(myCheckout.Data?.Contact?.Street);
            WithHouse(myCheckout.Data?.Contact?.House);
            WithStructure(myCheckout.Data?.Contact?.Structure);
            WithApartment(myCheckout.Data?.Contact?.Apartment);
            WithEntrance(myCheckout.Data?.Contact?.Entrance);
            WithFloor(myCheckout.Data?.Contact?.Floor);

            WithCustomerType(myCheckout.Data?.User.CustomerType);
            
            WithShippingOption(myCheckout.Data?.SelectShipping);
            WithPaymentOption(myCheckout.Data?.SelectPayment);

            WithBonusCard(myCheckout.Data?.User?.BonusCardId);
            if (myCheckout.Data?.Bonus?.UseIt is true) BonusUse(myCheckout.Data.Bonus.AppliedBonuses);

            if (myCheckout.Cart != null) ByShoppingCart(myCheckout.Cart);
            
            return this;
        }

        private ShippingCalculationParameters BuildShippingCalculation()
        {
            var calculationParameters = new ShippingCalculationParameters();
            
            if (_country != null) calculationParameters.Country = _country;
            if (_region != null) calculationParameters.Region = _region;
            if (_district != null) calculationParameters.District = _district;
            if (_city != null) calculationParameters.City = _city;
            if (_street != null) calculationParameters.Street = _street;
            if (_house != null) calculationParameters.House = _house;
            if (_structure != null) calculationParameters.Structure = _structure;
            if (_apartment != null) calculationParameters.Apartment = _apartment;
            if (_entrance != null) calculationParameters.Entrance = _entrance;
            if (_floor != null) calculationParameters.Floor = _floor;
            if (_zip != null) calculationParameters.Zip = _zip;
            if (_longitude != null) calculationParameters.Longitude = _longitude;
            if (_latitude != null) calculationParameters.Latitude = _latitude;
            if (_shippingOption != null) calculationParameters.ShippingOption = _shippingOption;
            if (_preOrderItems != null) calculationParameters.PreOrderItems = _preOrderItems;
            if (_currency != null) calculationParameters.Currency = _currency;
            if (_totalWeight != null) calculationParameters.TotalWeight = _totalWeight;
            if (_totalLength != null) calculationParameters.TotalLength = _totalLength;
            if (_totalWidth != null) calculationParameters.TotalWidth = _totalWidth;
            if (_totalHeight != null) calculationParameters.TotalHeight = _totalHeight;
            if (_isFromAdminArea != null) calculationParameters.IsFromAdminArea = _isFromAdminArea.Value;
            if (_showOnlyInDetails != null) calculationParameters.ShowOnlyInDetails = _showOnlyInDetails.Value;
            if (_bonusCardId != null) calculationParameters.BonusCardId = _bonusCardId;
            if (_appliedBonuses != null) calculationParameters.AppliedBonuses = _appliedBonuses.Value;
            
            if (_shippingItemsTotalPriceWithDiscounts is null || _shippingItemsTotalPriceWithDiscountsWithoutBonuses is null)
            {
                var (total, totalWithoutBonusCost) = GetTotalPrice();

                calculationParameters.ItemsTotalPriceWithDiscounts = total;
                calculationParameters.ItemsTotalPriceWithDiscountsWithoutBonuses = totalWithoutBonusCost;
            }
            else
            {
                calculationParameters.ItemsTotalPriceWithDiscounts = _shippingItemsTotalPriceWithDiscounts.Value;
                calculationParameters.ItemsTotalPriceWithDiscountsWithoutBonuses = _shippingItemsTotalPriceWithDiscountsWithoutBonuses.Value;
            }

            return calculationParameters;
        }

        private (float, float) GetTotalPrice()
        {
            var total = 0f;
            var totalWithoutBonusCost = 0f;
            
            if (_shoppingCart != null)
                total = _shoppingCart.TotalPrice - _shoppingCart.TotalDiscount;
            else if (_preOrderItems != null)
                total = _preOrderItems.Sum(x => x.Price * x.Amount);
                
            totalWithoutBonusCost = total;

            if (_appliedBonuses > 0
                && _bonusCardId != null
                && _shoppingCart != null
                && BonusSystem.IsActive)
            {
                var bonusCard = BonusSystemService.GetCard(_bonusCardId);
                if (bonusCard != null && !bonusCard.Blocked && bonusCard.BonusesTotalAmount > 0)
                {
                    total -= BonusSystemService.GetBonusCost(bonusCard, _shoppingCart, 0f, _appliedBonuses.Value).BonusPrice;
                }
            }
            
            total = total > 0 ? total : 0f;
            totalWithoutBonusCost = totalWithoutBonusCost > 0 ? totalWithoutBonusCost : 0f;
            
            return (total, totalWithoutBonusCost);
        }

        private PaymentCalculationParameters BuildPaymentCalculation()
        {
            var calculationParameters = new PaymentCalculationParameters();
            
            if (_country != null) calculationParameters.Country = _country;
            if (_region != null) calculationParameters.Region = _region;
            if (_district != null) calculationParameters.District = _district;
            if (_city != null) calculationParameters.City = _city;
            // if (_address != null) calculationParameters.Address = _address;
            if (_zip != null) calculationParameters.Zip = _zip;
            if (_customerType != null) calculationParameters.CustomerType = _customerType;
            if (_shippingOption != null) calculationParameters.ShippingOption = _shippingOption;
            if (_paymentOption != null) calculationParameters.PaymentOption = _paymentOption;
            if (_preOrderItems != null) calculationParameters.PreOrderItems = _preOrderItems;
            if (_bonusCardId != null) calculationParameters.BonusCardId = _bonusCardId;
            if (_appliedBonuses != null) calculationParameters.BonusAmount = _appliedBonuses.Value;
            if (_certificate != null) calculationParameters.Certificate = _certificate;

            if (_paymentItemsTotalPriceWithDiscounts is null)
            {
                var total = 0f;
                if (_shoppingCart != null)
                    total = _shoppingCart.TotalPrice - _shoppingCart.TotalDiscount;
                else if (_preOrderItems != null)
                    total = _preOrderItems.Sum(x => x.Price * x.Amount);

                if (_appliedBonuses > 0
                    && _bonusCardId != null
                    && _shoppingCart != null
                    && BonusSystem.IsActive)
                {
                    var bonusCard = BonusSystemService.GetCard(_bonusCardId);
                    if (bonusCard != null && !bonusCard.Blocked && bonusCard.BonusesTotalAmount > 0)
                    {
                        total -= BonusSystemService.GetBonusCost(bonusCard, _shoppingCart, _shippingOption?.FinalRate ?? 0f, _appliedBonuses.Value).BonusPrice;
                    }
                }

                calculationParameters.ItemsTotalPriceWithDiscounts = total > 0 ? total : 0f;
            }
            else
                calculationParameters.ItemsTotalPriceWithDiscounts = _paymentItemsTotalPriceWithDiscounts.Value;

            return calculationParameters;
        }

        #region IConfiguratorShippingCalculation

        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithCountry(string country) => WithCountry(country);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithRegion(string region) => WithRegion(region);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithDistrict(string district) => WithDistrict(district);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithCity(string city) => WithCity(city);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithStreet(string street) => WithStreet(street);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithHouse(string house) => WithHouse(house);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithStructure(string structure) => WithStructure(structure);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithApartment(string apartment) => WithApartment(apartment);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithEntrance(string entrance) => WithEntrance(entrance);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithFloor(string floor) => WithFloor(floor);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithZip(string zip) => WithZip(zip);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithLocation(float? longitude, float? latitude) => WithLocation(longitude, latitude);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithShippingOption(BaseShippingOption shippingOption) => WithShippingOption(shippingOption);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithPreOrderItems(List<PreOrderItem> preOrderItems) => WithPreOrderItems(preOrderItems);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithCurrency(Currency currency) => WithCurrency(currency);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithItemsTotalPriceWithDiscounts(float itemsTotalPriceWithDiscounts) => WithShippingItemsTotalPriceWithDiscounts(itemsTotalPriceWithDiscounts);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithItemsTotalPriceWithDiscountsWithoutBonuses(float itemsTotalPriceWithDiscountsWithoutBonuses) => WithShippingItemsTotalPriceWithDiscountsWithoutBonuses(itemsTotalPriceWithDiscountsWithoutBonuses);
        
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithTotalWeight(float? totalWeight) => WithTotalWeight(totalWeight);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithTotalLength(float? totalLength) => WithTotalLength(totalLength);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithTotalWidth(float? totalWidth) => WithTotalWidth(totalWidth);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithTotalHeight(float? totalHeight) => WithTotalHeight(totalHeight);

        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.WithBonusCard(Guid? bonusCardId) => WithBonusCard(bonusCardId);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.BonusUse(float? appliedBonuses) => BonusUse(appliedBonuses);

        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.FromAdminArea() => FromAdminArea();
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.ShowOnlyInDetails() => ShowOnlyInDetails();
        
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.ByShoppingCart(ShoppingCart shoppingCart) => ByShoppingCart(shoppingCart);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.ByOrder(Order order) => ByOrder(order);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.ByOrder(Order order, bool actualizeShipping) => ByOrder(order, actualizeShipping);
        IConfiguratorShippingCalculation IConfiguratorShippingCalculation.ByMyCheckout(MyCheckout myCheckout) => ByMyCheckout(myCheckout);

        ShippingCalculationParameters IBaseFabricCalculationParameters<ShippingCalculationParameters>.Build() => BuildShippingCalculation();

        #endregion IConfiguratorShippingCalculation

        #region IConfiguratorPaymentCalculation

        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithCountry(string country) => WithCountry(country);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithRegion(string region) => WithRegion(region);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithDistrict(string district) => WithDistrict(district);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithCity(string city) => WithCity(city);
        // IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithAddress(string address) => WithAddress(address);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithZip(string zip) => WithZip(zip);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithCustomerType(CustomerType? customerType) => WithCustomerType(customerType);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithShippingOption(BaseShippingOption shippingOption) => WithShippingOption(shippingOption);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithPaymentOption(BasePaymentOption paymentOption) => WithPaymentOption(paymentOption);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithPreOrderItems(List<PreOrderItem> preOrderItems) => WithPreOrderItems(preOrderItems);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithItemsTotalPriceWithDiscounts(float itemsTotalPriceWithDiscounts) => WithPaymentItemsTotalPriceWithDiscounts(itemsTotalPriceWithDiscounts);
        
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithCertificate(GiftCertificate certificate) => WithCertificate(certificate);

        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.WithBonusCard(Guid? bonusCardId) => WithBonusCard(bonusCardId);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.BonusUse(float? appliedBonuses) => BonusUse(appliedBonuses);
        
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.ByShoppingCart(ShoppingCart shoppingCart) => ByShoppingCart(shoppingCart);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.ByOrder(Order order) => ByOrder(order);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.ByOrder(Order order, bool actualizeShippingAndPayment) => ByOrder(order, actualizeShippingAndPayment);
        IConfiguratorPaymentCalculation IConfiguratorPaymentCalculation.ByMyCheckout(MyCheckout myCheckout) => ByMyCheckout(myCheckout);

        PaymentCalculationParameters IBaseFabricCalculationParameters<PaymentCalculationParameters>.Build() => BuildPaymentCalculation();

        #endregion IConfiguratorPaymentCalculation
    }

    public class ShippingCalculationConfigurator : BaseFabricCalculationParameters
    {
        private ShippingCalculationConfigurator(){}
        public static IConfiguratorShippingCalculation Configure() => new ShippingCalculationConfigurator();
    }

    public class PaymentCalculationConfigurator : BaseFabricCalculationParameters
    {
        private PaymentCalculationConfigurator(){}
        public static IConfiguratorPaymentCalculation Configure() => new PaymentCalculationConfigurator();
    }
}