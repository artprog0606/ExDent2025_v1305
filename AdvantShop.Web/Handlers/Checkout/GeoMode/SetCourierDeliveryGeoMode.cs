using System;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.GeoModes;
using AdvantShop.Handlers.Home;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Checkout.GeoMode
{
    public sealed class SetCourierDeliveryGeoMode :  AbstractCommandHandler
    {
        private readonly Guid _contactId;
        private readonly Customer _customer;

        public SetCourierDeliveryGeoMode(Guid contactId)
        {
            _contactId = contactId;
            _customer = CustomerContext.CurrentCustomer;
        }

        protected override void Validate()
        {
            if (!_customer.RegistredUser)
                throw new BlException("Пользователь не зарегистрирован");
            
            if (_customer.Contacts.Find(x => x.ContactId == _contactId) == null)
                throw new BlException("Контакт не найден");
        }

        protected override void Handle()
        {
            CustomerService.SetMainContact(true, _customer.Id, _contactId);
            
            _customer.Contacts = CustomerService.GetCustomerContacts(_customer.Id);
            
            var contact = _customer.Contacts.FirstOrDefault(x => x.IsMain) ?? 
                          _customer.Contacts.FirstOrDefault();

            if (contact != null)
            {
                var current = MyCheckout.Factory(_customer.Id);
                current.Data.Contact = CheckoutAddress.Create(contact);
                
                var options = current.AvailableShippingOptions(null, CalculationVariants.Courier);
                
                var selectedOption = options.FirstOrDefault(x => x.TypeOfDelivery == EnTypeOfDelivery.Courier);
                if (selectedOption != null)
                    current.Data.SelectShipping = selectedOption;
                
                current.Update();
            }
            
            CommonHelper.SetCookie(GeoModeConfig.ShippingTypeCookieName, GeoModeConfig.CourierType);
        }
    }
}