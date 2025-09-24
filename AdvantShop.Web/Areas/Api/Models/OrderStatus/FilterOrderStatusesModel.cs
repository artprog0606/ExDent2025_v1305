using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Web.Infrastructure.Api;

namespace AdvantShop.Areas.Api.Model.OrderStatus
{
    public class FilterOrderStatusesModel : EntitiesFilterModel, IValidatableObject
    {
        public bool? IsCanceled { get; set; }

        public bool? IsCompleted { get; set; }

        public bool? Hidden { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}