using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public interface IAdminCustomerResult
    {
    }
    
    public class AdminCustomerModel : IAdminCustomerResult
    {
        public Guid CustomerId { get; set; }
        public string Name => string.IsNullOrEmpty(CompanyName) ? FullName : CompanyName;
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ManagerId { get; set; }
        public string Rating { get; set; }
        public int LastOrderId { get; set; }
        public string LastOrderNumber { get; set; }
        public DateTime LastOrderDate { get; set; }
        public float OrdersSum { get; set; }
        public int OrdersCount { get; set; }
        public string Location { get; set; }
        public string ManagerName { get; set; }
        public CustomerType CustomerType { get; set; }

        public DateTime? BirthDay { get; set; }

        public string BirthDayFormatted => BirthDay != null ? Culture.ConvertDateWithoutHours(BirthDay.Value) : null;

        public DateTime RegistrationDateTime { get; set; }

        public string RegistrationDateTimeFormatted => Culture.ConvertDate(RegistrationDateTime);

        public bool CanBeDeleted => CustomerService.CanDelete(CustomerId);

        public int GroupId { get; set; }

        public string CustomerTypeFormatted => CustomerType.Localize();
    }

    public class AdminCustomerExportToCsvModel : AdminCustomerModel, IAdminCustomerResult
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        
        public string Zip { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
        public string Structure { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public string District { get; set; }

        public long? CardNumber { get; set; }

        public bool IsAgreeForPromotionalNewsletter { get; set; }
        public DateTime? AgreeForPromotionalNewsletterDateTime { get; set; }
        public string AgreeForPromotionalNewsletterFrom { get; set; }
        public string AgreeForPromotionalNewsletterFromIp { get; set; }

        public string AdminComment { get; set; }

        public string RegisteredFrom { get; set; }
        public string RegisteredFromIp { get; set; }
    }
}
