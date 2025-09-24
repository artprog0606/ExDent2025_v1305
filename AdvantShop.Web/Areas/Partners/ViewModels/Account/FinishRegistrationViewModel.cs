using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.Partners.Models.Account;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Areas.Partners.ViewModels.Account
{
    public class FinishRegistrationViewModel : IValidatableObject
    {
        public FinishRegistrationViewModel()
        {
            PaymentTypes = new List<SelectListItem> { new SelectListItem() { Text = "Не указан", Value = string.Empty } };
            PaymentTypes.AddRange(PaymentTypeService.GetPaymentTypes().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }));
        }

        public EPartnerType PartnerType { get; set; }
        public bool CanSkip { get; set; }

        public NaturalPersonModel NaturalPerson { get; set; }
        public LegalEntityModel LegalEntity { get; set; }

        public List<SelectListItem> PaymentTypes { get; private set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PartnerType == EPartnerType.None || !Enum.IsDefined(typeof(EPartnerType), PartnerType))
                yield return new ValidationResult("Укажите, юридическим или физическим лицом вы являетесь");

            switch (PartnerType)
            {
                case EPartnerType.LegalEntity:
                    Validator.ValidateObject(LegalEntity, new ValidationContext(LegalEntity));
                    break;
                case EPartnerType.NaturalPerson:
                    Validator.ValidateObject(NaturalPerson, new ValidationContext(NaturalPerson));
                    break;
            }
        }
    }
}