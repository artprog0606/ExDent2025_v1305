using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("RsbCredit")]
    public class RsbCreditPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string PartnerId
        {
            get { return Parameters.ElementOrDefault(RsbCreditTemplate.PartnerId); }
            set { Parameters.TryAddValue(RsbCreditTemplate.PartnerId, value.DefaultOrEmpty()); }
        }
        
        public string MinimumPrice
        {
            get { return Parameters.ElementOrDefault(RsbCreditTemplate.MinimumPrice); }
            set { Parameters.TryAddValue(RsbCreditTemplate.MinimumPrice, value.TryParseFloat().ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(RsbCreditTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(RsbCreditTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public string FirstPayment
        {
            get { return Parameters.ElementOrDefault(RsbCreditTemplate.FirstPayment); }
            set { Parameters.TryAddValue(RsbCreditTemplate.FirstPayment, value.TryParseFloat().ToInvariantString()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/rsb-v-kredit", "Инструкция. Подключение платежного модуля Русский Стандарт Банк (В кредит)"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(PartnerId))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
