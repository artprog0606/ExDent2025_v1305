using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Web.Infrastructure.Api;

namespace AdvantShop.Areas.Api.Models.Leads
{
    public sealed class LeadsFilter : EntitiesFilterModel, IValidatableObject
    {
        public Guid? CustomerId { get; set; }
        public float? SumFrom { get; set; }
        public float? SumTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public bool LoadItems { get; set; }
        public bool LoadCustomerFields { get; set; }

        public int? DealStatusId { get; set; }
        public int? SalesFunnelId { get; set; }
        public string Search { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public int? ManagerId { get; set; }
        public int? SourceId { get; set; }
        public string City { get; set; }
        public Dictionary<string, FiledFilterModelApi> CustomerFields { get; set; }
        public Dictionary<string, FiledFilterModelApi> LeadFields { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime temp;
            if (!string.IsNullOrWhiteSpace(DateFrom) && !DateTime.TryParse(DateFrom, out temp))
                yield return new ValidationResult("Не удалось преобразовать дату параметра DateFrom");

            if (!string.IsNullOrWhiteSpace(DateTo) && !DateTime.TryParse(DateTo, out temp))
                yield return new ValidationResult("Не удалось преобразовать дату параметра DateTo");
        }
    }
    
    public class FiledFilterModelApi
    {
        public string Value { get; set; }
        public string ValueExact { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}