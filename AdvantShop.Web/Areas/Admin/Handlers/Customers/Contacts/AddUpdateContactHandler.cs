using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Customers;
using AdvantShop.Repository;
using System;

namespace AdvantShop.Web.Admin.Handlers.Customers
{
    public class AddUpdateContactHandler
    {

        public CustomerContact Execute(Guid customerId, CustomerAccountModel account)
        {

            var customer = CustomerService.GetCustomer(customerId);

            if (customer == null || !customer.RegistredUser)
                return null;


            if (account.IsShowName)
            {
                var updateCustomer = false;

                if(customer.FirstName != account.FirstName && !string.IsNullOrWhiteSpace(account.FirstName))
                {
                    customer.FirstName = account.FirstName;
                    updateCustomer = true;
                }
                if (customer.LastName != account.LastName && 
                    SettingsCheckout.IsShowLastName && 
                    (!SettingsCheckout.IsRequiredLastName || 
                     SettingsCheckout.IsRequiredLastName && !string.IsNullOrWhiteSpace(account.LastName)))
                {
                    customer.LastName = account.LastName;
                    updateCustomer = true;
                }
                if (customer.Patronymic != account.Patronymic  && 
                    SettingsCheckout.IsShowPatronymic && 
                    (!SettingsCheckout.IsRequiredPatronymic || 
                     SettingsCheckout.IsRequiredPatronymic && !string.IsNullOrWhiteSpace(account.Patronymic)))
                {
                    customer.Patronymic = account.Patronymic;
                    updateCustomer = true;
                }

                if(updateCustomer)
                    CustomerService.UpdateCustomer(customer);
            }

            var ipZone = IpZoneContext.CurrentZone;

            var contact = account.ContactId.IsNullOrEmpty()
                                ? new CustomerContact()
                                : CustomerService.GetCustomerContact(account.ContactId);

            contact.Name = customer != null 
                ? customer.GetFullName() 
                : StringHelper.AggregateStrings(" ", account.LastName, account.FirstName, account.Patronymic);
            contact.City = account.City.IsNotEmpty() ? account.City : ipZone.City;
            contact.District = account.District.IsNotEmpty() ? account.District : ipZone.District;
            contact.Zip = account.Zip ?? string.Empty;
            contact.IsMain = account.IsMain;
            var country = CountryService.GetCountry(account.CountryId);
            
            var countryId = country?.CountryId ?? ipZone.CountryId;
            var countryName = country?.Name ?? ipZone.CountryName;
            
            if (countryId != 0)
                contact.CountryId = countryId;
            
            if (!string.IsNullOrEmpty(countryName))
                contact.Country = countryName;

            contact.Street = account.Street ?? string.Empty;
            contact.House = account.House ?? string.Empty;
            contact.Apartment = account.Apartment ?? string.Empty;
            contact.Structure = account.Structure ?? string.Empty;
            contact.Entrance = account.Entrance ?? string.Empty;
            contact.Floor = account.Floor ?? string.Empty;

            if (!string.IsNullOrEmpty(account.Region))
            {
                var regionId = RegionService.GetRegionIdByName(HttpUtility.HtmlDecode(account.Region));
                contact.RegionId = regionId != 0 ? regionId : ipZone.RegionId;
                contact.Region = account.Region.IsNotEmpty() ? HttpUtility.HtmlDecode(account.Region) : ipZone.Region;
            }
            else if (SettingsCheckout.IsShowState == true && SettingsCheckout.IsRequiredState == false)
            {
                contact.RegionId = null;
                contact.Region = string.Empty;
            }

            if (account.ContactId.IsNullOrEmpty())
            {
                contact.IsMain = true;
                CustomerService.AddContact(contact, customer.Id);
            }
            else
            {
                CustomerService.UpdateContact(contact);
            }

            return contact;
        }

    }
}