using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class AddLeadModel : IValidatableObject
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public float Sum { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string Organization { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? ManagerId { get; set; }

        public int SalesFunnelId { get; set; }
        public int DealStatusId { get; set; }

        public Guid? CustomerId { get; set; }

        public int? CallId { get; set; }

        public CustomerType CustomerType { get; set; }


        public List<LeadTempProduct> Products { get; set; }
        public List<CustomerFieldWithValue> CustomerFields { get; set; }
        public List<LeadFieldWithValue> LeadFields { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
