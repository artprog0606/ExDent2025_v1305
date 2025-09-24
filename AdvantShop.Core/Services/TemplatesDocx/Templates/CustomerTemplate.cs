using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Localization;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public class CustomerTemplate : IImplementsDynamicProperty
    {
        [TemplateDocxProperty("FirstName", LocalizeDescription = "Имя")]
        public string FirstName { get; set; }

        [TemplateDocxProperty("LastName", LocalizeDescription = "Фамилия")]
        public string LastName { get; set; }

        [TemplateDocxProperty("Patronymic", LocalizeDescription = "Отчество")]
        public string Patronymic { get; set; }

        [TemplateDocxProperty("LastName with initials", LocalizeDescription = "Фамилия с инициалами", Hide = true)]
        [TemplateDocxProperty("LastNameWithInitials", LocalizeDescription = "Фамилия с инициалами")]
        public string LastNameWithInitials {
            get
            {
                return string.Format("{0}{1}{2}", LastName,
                    string.IsNullOrEmpty(FirstName) ? string.Empty : string.Format(" {0}.", FirstName[0]),
                    string.IsNullOrEmpty(Patronymic) ? string.Empty : string.Format(" {0}.", Patronymic[0]));
            }
        }

        [TemplateDocxProperty("Organization", LocalizeDescription = "Организация")]
        public string Organization { get; set; }

        [TemplateDocxProperty("FullNameWithOrganization", LocalizeDescription = "ФИО и организация")]
        public string FullNameWithOrganization { get; set; }

        [TemplateDocxProperty("Phone", LocalizeDescription = "Телефон")]
        public string Phone { get; set; }

        [TemplateDocxProperty("Email", LocalizeDescription = "E-mail")]
        public string EMail { get; set; }

        [TemplateDocxProperty("BonusCardNumber", LocalizeDescription = "Бонусная карта")]
        public long? BonusCardNumber { get; set; }

        [TemplateDocxProperty("Customer Group", LocalizeDescription = "Группа покупателей", Hide = true)]
        [TemplateDocxProperty("CustomerGroup", LocalizeDescription = "Группа покупателей")]
        public string CustomerGroupString
        {
            get { return CustomerGroup != null ? CustomerGroup.GroupName : string.Empty; }
        }

        public CustomerGroup CustomerGroup { get; set; }

        [TemplateDocxProperty("BirthDay", LocalizeDescription = "День рождения")]
        public DateTime? BirthDay { get; set; }

        [TemplateDocxProperty("BirthDayFormatted", LocalizeDescription = "День рождения")]
        public string BirthDayFormatted
        {
            get { return BirthDay != null ? Culture.ConvertDateWithoutHours(BirthDay.Value) : null; }
        }

        [TemplateDocxProperty("Zip", LocalizeDescription = "Индекс")]
        public string Zip { get; set; }

        [TemplateDocxProperty("Country", LocalizeDescription = "Страна")]
        public string Country { get; set; }

        [TemplateDocxProperty("Region", LocalizeDescription = "Регион")]
        public string Region { get; set; }

        [TemplateDocxProperty("District", LocalizeDescription = "Район региона")]
        public string District { get; set; }

        [TemplateDocxProperty("City", LocalizeDescription = "Город")]
        public string City { get; set; }

        [TemplateDocxProperty("Street", LocalizeDescription = "Улица")]
        public string Street { get; set; }

        [TemplateDocxProperty("House", LocalizeDescription = "Дом")]
        public string House { get; set; }

        [TemplateDocxProperty("Structure", LocalizeDescription = "Строение/корпус")]
        public string Structure { get; set; }

        [TemplateDocxProperty("Apartment", LocalizeDescription = "Квартира")]
        public string Apartment { get; set; }

        [TemplateDocxProperty("Entrance", LocalizeDescription = "Подъезд")]
        public string Entrance { get; set; }

        [TemplateDocxProperty("Floor", LocalizeDescription = "Этаж")]
        public string Floor { get; set; }

        [TemplateDocxProperty("AggregatedAddress", LocalizeDescription = "Полный адрес")]
        public string AggregatedAddress { get; set; }

        public Type GetTypeWithDynamicProperties()
        {
            var fields = CustomerFieldService.GetCustomerFieldsWithValue(Guid.Empty) ?? new List<CustomerFieldWithValue>();

            if (fields.Count > 0)
            {
                var builderExtendedCustomerTemplate = new TypeBuilderHelper("CustomerTemplate", typeof(CustomerTemplate));
                foreach (var field in fields)
                    builderExtendedCustomerTemplate.AddProperty<string>(
                        "CustomerField_" + field.Id,
                        () => new TemplateDocxPropertyAttribute(field.Name) { LocalizeDescription = field.Name });

                //вариант с постоянными ключами
                //foreach (var field in fields)
                //    builderExtendedCustomerTemplate.AddProperty<string>(
                //        "CustomerField_2_" + field.Id,
                //        () => new TemplateDocxPropertyAttribute("CustomerField_" + field.Id) { LocalizeDescription = field.Name });

                return builderExtendedCustomerTemplate.CreateType();
            }

            return typeof(CustomerTemplate);
        }

        public static explicit operator CustomerTemplate(Customer customer)
        {
            CustomerTemplate template;

            var fields = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id) ?? new List<CustomerFieldWithValue>();

            if (fields.Count > 0)
            {
                var builderExtendedCustomerTemplate = new TypeBuilderHelper("CustomerTemplate", typeof(CustomerTemplate));
                foreach (var field in fields)
                    builderExtendedCustomerTemplate.AddProperty<string>(
                        "CustomerField_" + field.Id,
                        field.FieldType == CustomerFieldType.Date ? field.ValueDateFormat : field.Value,
                        () => new TemplateDocxPropertyAttribute(field.Name) {LocalizeDescription = field.Name});

                template = (CustomerTemplate)builderExtendedCustomerTemplate.CreateInstance();
            }
            else
                template = new CustomerTemplate();

            template.FirstName = customer.FirstName;
            template.LastName = customer.LastName;
            template.Patronymic = customer.Patronymic;
            template.Organization = customer.CustomerType == CustomerType.LegalEntity
                ? customer.CustomerCompanyName 
                : customer.Organization;
            template.Phone = customer.Phone;
            template.EMail = customer.EMail;
            template.BonusCardNumber = customer.BonusCardNumber;
            template.CustomerGroup = customer.CustomerGroup;
            template.BirthDay = customer.BirthDay;

            if (customer.Contacts != null && customer.Contacts.Count > 0)
            {
                var contact = customer.Contacts[0];

                template.Zip = contact.Zip;
                template.Country = contact.Country;
                template.Region = contact.Region;
                template.District = contact.District;
                template.City = contact.City;
                template.Street = contact.Street;
                template.House = contact.House;
                template.Apartment = contact.Apartment;
                template.Structure = contact.Structure;
                template.Entrance = contact.Entrance;
                template.Floor = contact.Floor;

                template.AggregatedAddress = CustomerService.ConvertAddressToString(contact,
                    f => f.Zip,
                    f => f.Country,
                    f => f.Region,
                    f => f.District,
                    f => f.City,
                    f => f.Street,
                    f => f.House,
                    f => f.Structure,
                    f => f.Apartment,
                    f => f.Entrance,
                    f => f.Floor);
            }
            var fullName = customer.GetFullName();
            template.FullNameWithOrganization = fullName.IsNullOrEmpty() ? template.Organization : $"{fullName}, {template.Organization}";
            return template;
        }

    }
}
