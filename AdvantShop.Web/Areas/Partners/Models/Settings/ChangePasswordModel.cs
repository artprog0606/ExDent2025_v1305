using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Helpers;

namespace AdvantShop.Areas.Partners.Models.Settings
{
    public class ChangePasswordModel : IValidatableObject
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!(Password.IsNotEmpty() && NewPassword.IsNotEmpty() && NewPasswordConfirm.IsNotEmpty()))
                yield return new ValidationResult("Заполните обязательные поля");
            else
            {
                if (PartnerContext.CurrentPartner.Password != SecurityHelper.GetPasswordHash(Password))
                    yield return new ValidationResult("Указан неверный пароль");

                if (NewPassword.Length < 6)
                    yield return new ValidationResult("Длина пароля должна быть не менее 6 символов");

                if (NewPassword != NewPasswordConfirm)
                    yield return new ValidationResult("Введенные пароли не совпадают");
            }
        }
    }
}