using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("DirectCredit")]
    public class DirectCreditPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string PartnerId
        {
            get { return Parameters.ElementOrDefault(DirectCreditTemplate.PartnerId); }
            set { Parameters.TryAddValue(DirectCreditTemplate.PartnerId, value.DefaultOrEmpty()); }
        }
        
        public string CodeTT
        {
            get { return Parameters.ElementOrDefault(DirectCreditTemplate.CodeTT); }
            set { Parameters.TryAddValue(DirectCreditTemplate.CodeTT, value.DefaultOrEmpty()); }
        }
        
        public string MinimumPrice
        {
            get { return Parameters.ElementOrDefault(DirectCreditTemplate.MinimumPrice); }
            set { Parameters.TryAddValue(DirectCreditTemplate.MinimumPrice, value.TryParseFloat().ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(DirectCreditTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(DirectCreditTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public string FirstPayment
        {
            get { return Parameters.ElementOrDefault(DirectCreditTemplate.FirstPayment); }
            set { Parameters.TryAddValue(DirectCreditTemplate.FirstPayment, value.TryParseFloat().ToInvariantString()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/direkt-kredit", "Инструкция. Подключение платежного модуля Директ Кредит"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(PartnerId) || string.IsNullOrWhiteSpace(CodeTT))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
