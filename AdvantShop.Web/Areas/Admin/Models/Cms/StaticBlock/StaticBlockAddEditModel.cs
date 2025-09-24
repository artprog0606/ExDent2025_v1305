using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Cms.StaticBlock
{
    public class StaticBlockAddEditModel : IValidatableObject
    {
        public int StaticBlockId { get; set; }

        public string Key { get; set; }

        public string InnerName { get; set; }

        public string Content { get; set; }

        public bool Enabled { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Key) || string.IsNullOrWhiteSpace(InnerName))
                yield return new ValidationResult("Введите обязательные поля");
            
            if (!string.IsNullOrEmpty(Key) && Key.Length > 255)
                yield return new ValidationResult("Ключ должен быть меньше 256 символов");
            
            if (!string.IsNullOrEmpty(InnerName) && InnerName.Length > 255)
                yield return new ValidationResult("Название должно быть меньше 256 символов");
        }
    }
}
