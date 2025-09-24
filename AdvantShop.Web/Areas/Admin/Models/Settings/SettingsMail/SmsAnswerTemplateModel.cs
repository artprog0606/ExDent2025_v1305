using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Services.Smses;

namespace AdvantShop.Web.Admin.Models.Settings.SettingsMail
{
    public class SmsAnswerTemplateModel : SmsAnswerTemplate, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Text))
            {
                yield return new ValidationResult("Введите обязательные поля");
            }
        }
    }
}
