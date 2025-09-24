//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Customers
{
    [Serializable]
    public class CustomerContact
    {
        public Guid ContactId { get; set; }

        public Guid CustomerGuid { get; set; }

        [Compare("Core.Customers.Contact.Name")]
        public string Name { get; set; }

        public int CountryId { get; set; }

        [Compare("Core.Customers.Contact.Country")]
        public string Country { get; set; }

        [Compare("Core.Customers.Contact.City")]
        public string City { get; set; }
        
        [Compare("Core.Customers.Contact.District")]
        public string District { get; set; }

        public int? RegionId { get; set; }

        [Compare("Core.Customers.Contact.Region")]
        public string Region { get; set; }
        
        [Compare("Core.Customers.Contact.Zip")]
        public string Zip { get; set; }

        [Localize("Core.Customers.Contact.Street")]
        [Compare("Core.Customers.Contact.Street")]
        public string Street { get; set; }
        
        [Localize("Core.Customers.Contact.House")]
        [Compare("Core.Customers.Contact.House")]
        public string House { get; set; }
        
        [Localize("Core.Customers.Contact.Apartment")]
        [Compare("Core.Customers.Contact.Apartment")]
        public string Apartment { get; set; }
        
        [Localize("Core.Customers.Contact.Structure")]
        [Compare("Core.Customers.Contact.Structure")]
        public string Structure { get; set; }
        
        [Localize("Core.Customers.Contact.Entrance")]
        [Compare("Core.Customers.Contact.Entrance")]
        public string Entrance { get; set; }
        
        [Localize("Core.Customers.Contact.Floor")]
        [Compare("Core.Customers.Contact.Floor")]
        public string Floor { get; set; }
        
        public string DadataJson { get; set; }
        
        [Localize("Core.Customers.Contact.IsMain")]
        [Compare("Core.Customers.Contact.IsMain")]
        public bool IsMain { get; set; }

        public CustomerContact()
        {
            ContactId = Guid.Empty;
            CustomerGuid = Guid.Empty;
            Name = string.Empty;
            District = string.Empty;
            Country = string.Empty;
            Region = string.Empty;
            City = string.Empty;
            Zip = string.Empty;
        }
    }
}