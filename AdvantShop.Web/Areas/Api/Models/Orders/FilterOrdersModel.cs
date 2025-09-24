using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Web.Infrastructure.Api;

namespace AdvantShop.Areas.Api.Model.Orders
{
    public class FilterOrdersModel : EntitiesFilterModel, IValidatableObject
    {
        public FilterOrdersModel()
        {
        }

        public Guid? CustomerId { get; set; }
        public int? StatusId { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsCompleted { get; set; }
        public float? SumFrom { get; set; }
        public float? SumTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string ModifiedDateFrom { get; set; }
        public string ModifiedDateTo { get; set; }
        
        private bool loadItems;
        public bool LoadItems
        {
            get
            {
                return loadItems;
            }
            set
            {
                loadItems = value;

                if (loadItems)
                {
                    MaxItemsPerPage = 50;
                    DefaultItemsPerPage = 50;
                }
            }
        }
        
        public bool? PreparedPrices { get; set; }
        
        public bool? LoadCustomer { get; set; }
        
        public bool? LoadSource { get; set; }
        
        public bool? LoadReview { get; set; }
        
        public bool? LoadBillingApiLink { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(DateFrom) && !DateTime.TryParse(DateFrom, out _))
                yield return new ValidationResult("Неудалось преобразовать дату параметра DateFrom");

            if (!string.IsNullOrWhiteSpace(DateTo) && !DateTime.TryParse(DateTo, out _))
                yield return new ValidationResult("Неудалось преобразовать дату параметра DateTo");
        }
    }
}