using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Services.Triggers;

namespace AdvantShop.Web.Admin.Models.Triggers.Category
{
    public class CategoryModel : IValidatableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }

        public static explicit operator CategoryModel(TriggerCategory category)
        {
            return new CategoryModel()
            {
                Id = category.Id,
                Name = category.Name,
                SortOrder = category.SortOrder
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Укажите название");
        }
    }
}
