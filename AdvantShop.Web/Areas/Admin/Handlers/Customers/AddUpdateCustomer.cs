using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Customers;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Handlers.Customers.CustomerSegments;
using AdvantShop.Web.Admin.Models.Customers;

namespace AdvantShop.Web.Admin.Handlers.Customers
{
    public class AddUpdateCustomer
    {
        private readonly CustomersModel _model;

        public AddUpdateCustomer(CustomersModel model)
        {
            _model = model;
        }
        
        public bool Execute(bool notSaveContact = false)
        {
            var customer = _model.IsEditMode 
                                ? CustomerService.GetCustomer(_model.CustomerId) 
                                : new Customer();

            if (customer == null)
                return false;

            if (_model.IsEditMode && !CustomerService.CheckAccess(customer))
                return false;
            
            if (_model.CustomerId != Guid.Empty)
                customer.Id = _model.CustomerId;

            customer.EMail = _model.Customer.EMail.DefaultOrEmpty();
            customer.FirstName = _model.Customer.FirstName.DefaultOrEmpty();
            customer.LastName = _model.Customer.LastName.DefaultOrEmpty();
            customer.Patronymic = _model.Customer.Patronymic.DefaultOrEmpty();
            customer.Organization = _model.Customer.Organization.DefaultOrEmpty();
            customer.Phone = _model.Customer.Phone.DefaultOrEmpty();
            customer.StandardPhone = StringHelper.ConvertToStandardPhone(customer.Phone, true, true);
            customer.IsAgreeForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter
                ? _model.Customer.IsAgreeForPromotionalNewsletter
                : SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked;
            customer.BirthDay = _model.Customer.BirthDay;
            
            customer.CustomerGroupId = _model.Customer.CustomerGroupId;
            customer.ManagerId = _model.Customer.ManagerId;
            customer.AdminComment = _model.Customer.AdminComment.DefaultOrEmpty();
            customer.CustomerType = _model.Customer.CustomerType;

            if (_model.Tags != null && _model.Tags.Count > 0)
            {
                var prevTags = customer.Id != Guid.Empty 
                    ? TagService.Gets(customer.Id).Select(x => x.Name).ToList() 
                    : new List<string>();

                if (_model.Tags.Any(x => !prevTags.Contains(x)))
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_AddTagToCustomer);

                customer.Tags = _model.Tags.Select(x => new Tag
                {
                    Name = x,
                    Enabled = true
                }).ToList();
            }
            else
            {
                customer.Tags = new List<Tag>();
            }
            
            if (!_model.IsEditMode)
            {
                customer.Password = _model.Customer.Password.DefaultOrEmpty();
                customer.RegistrationDateTime = DateTime.Now;

                _model.CustomerId = customer.Id = CustomerService.InsertNewCustomer(customer);

                if (_model.CustomerId == Guid.Empty)
                    return false;

                if (_model.PartnerId.HasValue && PartnerService.GetPartner(_model.PartnerId.Value) != null)
                    PartnerService.AddBindedCustomer(new BindedCustomer { CustomerId = _model.CustomerId, PartnerId = _model.PartnerId.Value });

                if (_model.OrderId != null)
                {
                    var order = OrderService.GetOrder(_model.OrderId.Value);
                    if (order != null && order.OrderCustomer != null)
                    {
                        order.OrderCustomer.CustomerID = customer.Id;
                        OrderService.UpdateOrderCustomer(order.OrderCustomer);
                    }
                }

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_CustomerCreated);
            }
            else
            {
                CustomerService.UpdateCustomer(customer);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_EditCustomer);
            }

            if (_model.CustomerContact != null && notSaveContact == false)
            {
                var contact = CustomerService.GetCustomerContact(_model.CustomerContact.ContactId) ??
                              new CustomerContact();

                var notEmptyContact =
                    _model.CustomerContact.City.IsNotEmpty() || _model.CustomerContact.Zip.IsNotEmpty() ||
                    _model.CustomerContact.Country.IsNotEmpty() || _model.CustomerContact.Region.IsNotEmpty() ||
                    _model.CustomerContact.Street.IsNotEmpty() || _model.CustomerContact.House.IsNotEmpty() ||
                    _model.CustomerContact.Apartment.IsNotEmpty() || _model.CustomerContact.Structure.IsNotEmpty() ||
                    _model.CustomerContact.Entrance.IsNotEmpty() || _model.CustomerContact.Floor.IsNotEmpty() ||
                    _model.CustomerContact.District.IsNotEmpty();

                contact.Name = (customer.FirstName + " " + customer.LastName).Trim();
                contact.City = _model.CustomerContact.City.DefaultOrEmpty();
                contact.District = _model.CustomerContact.District.DefaultOrEmpty();
                contact.Zip = _model.CustomerContact.Zip.DefaultOrEmpty();

                contact.Country = _model.CustomerContact.Country.DefaultOrEmpty();
                var country = CountryService.GetCountryByName(contact.Country);
                contact.CountryId = country?.CountryId ?? 0;

                contact.Region = _model.CustomerContact.Region.DefaultOrEmpty();
                contact.RegionId = RegionService.GetRegionIdByName(contact.Region);

                contact.Street = _model.CustomerContact.Street.DefaultOrEmpty();
                contact.House = _model.CustomerContact.House.DefaultOrEmpty();
                contact.Apartment = _model.CustomerContact.Apartment.DefaultOrEmpty();
                contact.Structure = _model.CustomerContact.Structure.DefaultOrEmpty();
                contact.Entrance = _model.CustomerContact.Entrance.DefaultOrEmpty();
                contact.Floor = _model.CustomerContact.Floor.DefaultOrEmpty();

                if (contact.ContactId == Guid.Empty)
                {
                    if (notEmptyContact)
                        CustomerService.AddContact(contact, customer.Id);
                }
                else
                {
                    CustomerService.UpdateContact(contact);
                }
            }

            if (_model.CustomerFieldsJs != null)
            {
                foreach (var field in _model.CustomerFieldsJs)
                    CustomerFieldService.AddUpdateMap(customer.Id, field.Value.Id, field.Value.Value ?? "", !_model.IsEditMode);
            }
            else if (_model.CustomerFields != null)
            {
                foreach (var field in _model.CustomerFields)
                    CustomerFieldService.AddUpdateMap(customer.Id, field.Id, field.Value ?? "", !_model.IsEditMode);
            }

            Task.Run(() => new CustomerSegmentsJob().Execute(null));

            return true;
        }
    }
}

