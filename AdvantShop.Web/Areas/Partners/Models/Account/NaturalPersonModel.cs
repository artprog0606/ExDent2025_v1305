using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Areas.Partners.Models.Account
{
    public class NaturalPersonModel : NaturalPerson, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!(FirstName.IsNotEmpty() &&
                LastName.IsNotEmpty() &&
                Patronymic.IsNotEmpty() &&
                PassportSeria.IsNotEmpty() &&
                PassportNumber.IsNotEmpty() &&
                PassportWhoGive.IsNotEmpty() &&
                PassportWhenGive.HasValue &&
                RegistrationAddress.IsNotEmpty() &&
                Zip.IsNotEmpty()
                ))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}