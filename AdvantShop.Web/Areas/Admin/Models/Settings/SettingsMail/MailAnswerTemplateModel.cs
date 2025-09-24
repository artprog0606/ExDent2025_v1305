using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Mails;

namespace AdvantShop.Web.Admin.Models.Settings.SettingsMail
{
    public class MailAnswerTemplateModel : MailAnswerTemplate, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Subject) ||
                string.IsNullOrWhiteSpace(Body))
            {
                yield return new ValidationResult("Введите обязательные поля");
            }
        }
    }
}
