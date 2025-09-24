using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public class CustomerModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public long? Phone { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("patronymic")]
        public string Patronymic { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("subscribedForNews")]
        [Obsolete]
        public bool? SubscribedForNews => IsAgreeForPromotionalNewsletter;

        [JsonProperty("birthday")]
        public DateTime? BirthDay { get; set; }

        [JsonProperty("adminComment")]
        public string AdminComment { get; set; }

        [JsonProperty("managerId")]
        public int? ManagerId { get; set; }

        [JsonProperty("groupId")]
        public int? GroupId { get; set; }
        
        [JsonProperty("group")]
        public CustomerGroupApi Group { get; set; }

        [JsonProperty("password"), JsonIgnore]
        public string Password { get; set; }
        
        [JsonProperty("customerType")]
        public string CustomerType { get; set; }
        
        [JsonProperty("isAgreeForPromotionalNewsletter")]
        public bool? IsAgreeForPromotionalNewsletter { get; set; }

        [Obsolete("Use Contacts")]
        [JsonProperty("contact", NullValueHandling = NullValueHandling.Ignore)]
        public CustomerContactModel Contact { get; set; }
        
        [JsonProperty("contacts", NullValueHandling = NullValueHandling.Ignore)]
        public List<CustomerContactModel> Contacts { get; set; }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<CustomerFieldModel> Fields { get; set; }
    }

    public class CustomerContactModel
    {
        [JsonProperty("contactId")]
        public Guid ContactId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("house")]
        public string House { get; set; }

        [JsonProperty("apartment")]
        public string Apartment { get; set; }

        [JsonProperty("structure")]
        public string Structure { get; set; }

        [JsonProperty("entrance")]
        public string Entrance { get; set; }

        [JsonProperty("floor")]
        public string Floor { get; set; }
        
        [JsonProperty("dadataJson")]
        public string DadataJson { get; set; }
        
        [JsonProperty("isMain")]
        public bool IsMain { get; set; }

        public CustomerContactModel()
        {
        }

        public CustomerContactModel(CustomerContact contact)
        {
            ContactId = contact.ContactId;
            Name = contact.Name;
            Country = contact.Country;
            City = contact.City;
            District = contact.District;
            Region = contact.Region;
            Zip = contact.Zip;
            Street = contact.Street;
            House = contact.House;
            Apartment = contact.Apartment;
            Structure = contact.Structure;
            Entrance = contact.Entrance;
            Floor = contact.Floor;
            DadataJson = !string.IsNullOrEmpty(contact.DadataJson) ? contact.DadataJson : null;
            IsMain = contact.IsMain;
        }
    }

    public class CustomerFieldModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string FieldType { get; set; }
        public List<CustomerFieldValueItem> ValueItems { get; set; }
        public string CustomerType { get; set; }
        public string FieldAssignment { get; set; }
        public bool Required { get; set; }
        public bool ShowInRegistration { get; set; }
        public bool ShowInCheckout { get; set; }
        public bool DisableEditing { get; set; }
        
        public CustomerFieldModel(){}

        public CustomerFieldModel(CustomerFieldWithValue field)
        {
            Id = field.Id;
            Name = field.Name;
            Value = !string.IsNullOrEmpty(field.Value) ? field.Value : null;
            FieldType = field.FieldType.ToString().ToLower();

            var values = field.Values;
            ValueItems = values != null && values.Count > 0
                ? values.Select(x => new CustomerFieldValueItem(x)).ToList()
                : null;

            CustomerType = field.CustomerType.ToString();
            FieldAssignment = field.FieldAssignment.ToString();
            Required = field.Required;
            ShowInRegistration = field.ShowInRegistration;
            ShowInCheckout = field.ShowInCheckout;
            DisableEditing = field.DisableCustomerEditing;
        }
    }

    public class CustomerFieldValueItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
        
        public CustomerFieldValueItem(){}
        
        public CustomerFieldValueItem(SelectListItem item)
        {
            Text = item.Text;
            Value = !string.IsNullOrEmpty(item.Value) ? item.Value : null;
            Selected = item.Selected;
        }
    }

    public class CustomerGroupApi
    {
        public int Id { get; }
        public string Name { get; }
        public float DiscountPercent { get; }

        public CustomerGroupApi()
        {
        }
        
        public CustomerGroupApi(CustomerGroup group)
        {
            Id = group.CustomerGroupId;
            Name = group.GroupName;
            DiscountPercent = group.GroupDiscount;
        }
    }
}