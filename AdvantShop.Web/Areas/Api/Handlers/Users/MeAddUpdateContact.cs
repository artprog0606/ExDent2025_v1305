using System;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Users
{
    public class MeAddUpdateContact : AbstractCommandHandler<AddUpdateContactResponse>
    {
        private readonly CustomerContactModel _contact;
        private readonly Guid _contactId;
        private readonly Customer _customer;
        private readonly bool _adding;
        
        public MeAddUpdateContact(Guid id, CustomerContactModel contact, bool adding) 
                            : this(CustomerContext.CurrentCustomer, contact, adding)
        {
            _contactId = id;
        }
        
        public MeAddUpdateContact(CustomerContactModel contact, bool adding) 
                           : this(CustomerContext.CurrentCustomer, contact, adding)
        {
        }
        
        public MeAddUpdateContact(Customer customer, CustomerContactModel contact, bool adding)
        {
            _contact = contact;
            _customer = customer;
            _adding = adding;

            if (_contact != null)
                _contactId = _contact.ContactId;
        }

        protected override void Validate()
        {
            if (_customer == null || !_customer.RegistredUser)
                throw new BlException("Пользователь не авторизован");
        }

        protected override AddUpdateContactResponse Handle()
        {
            CustomerContact contact;

            if (_adding)
            {
                contact = new CustomerContact() {CustomerGuid = _customer.Id};
            }
            else
            {
                contact = CustomerService.GetCustomerContact(_contactId);

                if (contact == null || contact.CustomerGuid != _customer.Id)
                    throw new BlException("Контакт не найден");
            }

            contact.Name =
                !string.IsNullOrWhiteSpace(_contact.Name.EncodeOrEmpty())
                    ? _contact.Name.EncodeOrEmpty()
                    : _customer.GetFullName();
            contact.City = _contact.City.EncodeOrEmpty();
            contact.District = _contact.District.EncodeOrEmpty();
            contact.Zip = _contact.Zip.EncodeOrEmpty();

            contact.Country = _contact.Country.EncodeOrEmpty();
            var country = CountryService.GetCountryByName(contact.Country);
            contact.CountryId = country?.CountryId ?? 0;

            contact.Region = _contact.Region.EncodeOrEmpty();
            contact.RegionId = RegionService.GetRegionIdByName(contact.Region);

            contact.Street = _contact.Street.EncodeOrEmpty();
            contact.House = _contact.House.EncodeOrEmpty();
            contact.Apartment = _contact.Apartment.EncodeOrEmpty();
            contact.Structure = _contact.Structure.EncodeOrEmpty();
            contact.Entrance = _contact.Entrance.EncodeOrEmpty();
            contact.Floor = _contact.Floor.EncodeOrEmpty();
            contact.DadataJson = _contact.DadataJson.IsNotEmpty() ? _contact.DadataJson : null;
            contact.IsMain = _contact.IsMain;

            if (_adding)
            {
                if (IsDuplicate(contact, out var contactId))
                    contact.ContactId = contactId;
                else
                    CustomerService.AddContact(contact, _customer.Id);
            }
            else
            {
                CustomerService.UpdateContact(contact);
            }

            return new AddUpdateContactResponse(contact.ContactId);
        }

        private bool IsDuplicate(CustomerContact contact, out Guid contactId)
        {
            contactId = Guid.Empty;
            
            var customerContacts = CustomerService.GetCustomerContacts(_customer.Id);
            
            if (customerContacts.Count == 0)
                return false;

            foreach (var customerContact in customerContacts)
            {
                var isEqual = 
                    customerContact.Country == contact.Country &&
                    customerContact.CountryId == contact.CountryId &&
                    customerContact.Region == contact.Region && 
                    customerContact.RegionId == contact.RegionId && 
                    customerContact.City == contact.City && 
                    customerContact.District == contact.District && 
                    customerContact.Zip == contact.Zip &&
                    customerContact.Street == contact.Street && 
                    customerContact.House == contact.House && 
                    customerContact.Apartment == contact.Apartment &&
                    customerContact.Structure == contact.Structure && 
                    customerContact.Entrance == contact.Entrance && 
                    customerContact.Floor == contact.Floor &&
                    customerContact.DadataJson == contact.DadataJson;

                if (isEqual)
                {
                    contactId = customerContact.ContactId;
                    return true;
                }
            }

            return false;
        } 
    }
}