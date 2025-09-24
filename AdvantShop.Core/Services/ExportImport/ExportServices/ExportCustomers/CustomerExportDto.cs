using System;

namespace AdvantShop.Core.Services.ExportImport.ExportServices.ExportCustomers
{
    public class CustomerExportDto
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Organization { get; set; }
        public string CustomerGroupName { get; set; }
        public int CustomerType { get; set; }
        public bool Enabled { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public string RegisteredFrom { get; set; }
        public string RegisteredFromIp { get; set; }
        public string AdminComment { get; set; }
        public string ManagerName { get; set; }
        public string ManagerId { get; set; }
        public bool IsAgreeForPromotionalNewsletter { get; set; }
        public DateTime? AgreeForPromotionalNewsletterDateTime { get; set; }
        public string AgreeForPromotionalNewsletterFrom { get; set; }
        public string AgreeForPromotionalNewsletterFromIp { get; set; }
        public string Zip { get; set; }
        public int CountryId { get; set; }
        public int RegionId { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public string Structure { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        
        public string LastOrderNumber { get; set; }
        public int LastOrderId { get; set; }
        
        public int PaidOrdersCount { get; set; }
        public float PaidOrdersSum { get; set; }
        
        public string BonusCardNumber { get; set; }
    }
}