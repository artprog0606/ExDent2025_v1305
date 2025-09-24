using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Areas.Partners.Models.Settings
{
    public class CommonSettingsModel : IValidatableObject
    {
        public CommonSettingsModel()
        {
            SendMessages = new Dictionary<string, bool>();
        }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public Dictionary<string, bool> SendMessages { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.IsNullOrEmpty() || Phone.IsNullOrEmpty())
                yield return new ValidationResult("Заполните обязательные поля");
        }
    }
}