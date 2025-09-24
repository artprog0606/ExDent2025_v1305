//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Customers;

namespace AdvantShop.Orders
{
    [Serializable]
    public class OrderCustomer : IOrderCustomer
    {
        public int OrderID { get; set; }

        [Compare("Core.Orders.OrderCustomer.CustomerId")]
        public Guid CustomerID { get; set; }

        [Compare("Core.Orders.OrderCustomer.CustomerIP")]
        public string CustomerIP { get; set; }

        [Compare("Core.Orders.OrderCustomer.FirstName")]
        public string FirstName { get; set; }

        [Compare("Core.Orders.OrderCustomer.LastName")]
        public string LastName { get; set; }

        [Compare("Core.Orders.OrderCustomer.Patronymic")]
        public string Patronymic { get; set; }

        [Compare("Core.Orders.OrderCustomer.Organization")]
        public string Organization { get; set; }

        [Compare("Core.Orders.OrderCustomer.Email")]
        public string Email { get; set; }

        [Compare("Core.Orders.OrderCustomer.Phone")]
        public string Phone { get; set; }

        [Compare("Core.Orders.OrderCustomer.StandardPhone")]
        public long? StandardPhone { get; set; }

        [Compare("Core.Orders.OrderCustomer.CustomerType")]
        public CustomerType CustomerType { get; set; }



        [Compare("Core.Orders.OrderContact.Country")]
        public string Country { get; set; }

        [Compare("Core.Orders.OrderContact.Zone")]
        public string Region { get; set; }

        [Compare("Core.Orders.OrderContact.City")]
        public string City { get; set; }

        [Compare("Core.Orders.OrderContact.District")]
        public string District { get; set; }

        [Compare("Core.Orders.OrderContact.Zip")]
        public string Zip { get; set; }

        //[Compare("Core.Orders.OrderContact.Address")]
        //public string Address { get; set; }

        [Compare("Core.Orders.OrderContact.CustomField1")]
        public string CustomField1 { get; set; }

        [Compare("Core.Orders.OrderContact.CustomField2")]
        public string CustomField2 { get; set; }

        [Compare("Core.Orders.OrderContact.CustomField3")]
        public string CustomField3 { get; set; }


        [Compare("Core.Orders.OrderContact.Street")]
        public string Street { get; set; }

        /// <summary>
        /// Дом
        /// </summary>
        [Compare("Core.Orders.OrderContact.House")]
        public string House { get; set; }

        /// <summary>
        /// Квартира
        /// </summary>
        [Compare("Core.Orders.OrderContact.Apartment")]
        public string Apartment { get; set; }

        /// <summary>
        /// Строение/Корпус
        /// </summary>
        [Compare("Core.Orders.OrderContact.Structure")]
        public string Structure { get; set; }

        /// <summary>
        /// Подъезд
        /// </summary>
        [Compare("Core.Orders.OrderContact.Entrance")]
        public string Entrance { get; set; }

        /// <summary>
        /// Этаж
        /// </summary>
        [Compare("Core.Orders.OrderContact.Floor")]
        public string Floor { get; set; }


        private string _customerCompanyName;
        public string CustomerCompanyName
        {
            get
            {
                if (!string.IsNullOrEmpty(_customerCompanyName))
                    return _customerCompanyName;
                if (CustomerID == Guid.Empty || CustomerType != CustomerType.LegalEntity)
                    return string.Empty;
                var customerField = CustomerFieldService.GetCustomerFieldWithValueByFieldAssignment(CustomerID, CustomerFieldAssignment.CompanyName);
                _customerCompanyName = customerField?.Value;

                return customerField?.Value;
            }
        }

        public static explicit operator Customer(OrderCustomer customer)
        {
            var customerContactList = new List<CustomerContact>();
            
            if (!string.IsNullOrWhiteSpace(customer.Country)
                || !string.IsNullOrWhiteSpace(customer.Region)
                || !string.IsNullOrWhiteSpace(customer.District)
                || !string.IsNullOrWhiteSpace(customer.City)
                || !string.IsNullOrWhiteSpace(customer.Zip)
                || !string.IsNullOrWhiteSpace(customer.Street)
                || !string.IsNullOrWhiteSpace(customer.House)
                || !string.IsNullOrWhiteSpace(customer.Apartment)
                || !string.IsNullOrWhiteSpace(customer.Structure)
                || !string.IsNullOrWhiteSpace(customer.Entrance)
                || !string.IsNullOrWhiteSpace(customer.Floor))
            {
                customerContactList.Add(new CustomerContact
                {
                    Country = customer.Country,
                    Region = customer.Region,
                    District = customer.District,
                    City = customer.City,
                    Zip = customer.Zip,
                    Street = customer.Street,
                    House = customer.House,
                    Apartment = customer.Apartment,
                    Structure = customer.Structure,
                    Entrance = customer.Entrance,
                    Floor = customer.Floor
                });
            }

            return new Customer
            {
                Id = customer.CustomerID,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Patronymic = customer.Patronymic,
                Organization = customer.Organization,
                EMail = customer.Email,
                Phone = customer.Phone,
                StandardPhone = customer.StandardPhone,
                CustomerType = customer.CustomerType,
                Contacts = customerContactList,
                IsAgreeForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter 
                                                  && SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
            };
        }

        public static explicit operator OrderCustomer(Customer customer)
        {
            var orderCustomer = new OrderCustomer()
            {
                CustomerID = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Patronymic = customer.Patronymic,
                Organization = customer.Organization,
                Email = customer.EMail,
                Phone = customer.Phone,
                StandardPhone = customer.StandardPhone,
                CustomerType= customer.CustomerType,
            };

            if (customer.Contacts != null && customer.Contacts.Count > 0)
            {
                var contact = customer.Contacts[0];

                orderCustomer.Country = contact.Country;
                orderCustomer.Region = contact.Region;
                orderCustomer.District = contact.District;
                orderCustomer.City = contact.City;
                orderCustomer.Zip = contact.Zip;
                orderCustomer.Street = contact.Street;
                orderCustomer.House = contact.House;
                orderCustomer.Apartment = contact.Apartment;
                orderCustomer.Structure = contact.Structure;
                orderCustomer.Entrance = contact.Entrance;
                orderCustomer.Floor = contact.Floor;
            }

            return orderCustomer;
        }
    }
}