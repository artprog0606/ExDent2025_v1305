using AdvantShop.Core.Services.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerTags
{
    public class CustomerTagModel : IValidatableObject
    {
        public bool IsEditMode { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Customers.Cusrtomertags.Error.Name"), new[] { "Name" });
            }
        }
    }
}
