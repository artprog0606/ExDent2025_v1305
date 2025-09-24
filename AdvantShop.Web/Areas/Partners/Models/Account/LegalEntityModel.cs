using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Areas.Partners.Models.Account
{
    public class LegalEntityModel : LegalEntity, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!(CompanyName.IsNotEmpty() &&
                INN.IsNotEmpty() &&
                KPP.IsNotEmpty() &&
                LegalAddress.IsNotEmpty() &&
                ActualAddress.IsNotEmpty() &&
                SettlementAccount.IsNotEmpty() &&
                Bank.IsNotEmpty() &&
                CorrespondentAccount.IsNotEmpty() &&
                BIK.IsNotEmpty() &&
                PostAddress.IsNotEmpty() &&
                Zip.IsNotEmpty() &&
                Phone.IsNotEmpty() &&
                ContactPerson.IsNotEmpty() &&
                Director.IsNotEmpty() &&
                Accountant.IsNotEmpty()
                ))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}