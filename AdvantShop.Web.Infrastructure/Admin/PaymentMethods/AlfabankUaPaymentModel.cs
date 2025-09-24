using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("AlfabankUa")]
    public class AlfabankUaPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string PartnerId
        {
            get { return Parameters.ElementOrDefault(AlfabankUaTemplate.PartnerId); }
            set { Parameters.TryAddValue(AlfabankUaTemplate.PartnerId, value.DefaultOrEmpty()); }
        }
        
        public string MinimumPrice
        {
            get { return Parameters.ElementOrDefault(AlfabankUaTemplate.MinimumPrice); }
            set { Parameters.TryAddValue(AlfabankUaTemplate.MinimumPrice, value.TryParseFloat().ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(AlfabankUaTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(AlfabankUaTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public string FirstPayment
        {
            get { return Parameters.ElementOrDefault(AlfabankUaTemplate.FirstPayment); }
            set { Parameters.TryAddValue(AlfabankUaTemplate.FirstPayment, value.TryParseFloat().ToInvariantString()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/integratsiya-s-servisom-alfa-bank-ukraina", "Инструкция. Подключение платежного модуля Альфа-Банк Украина (В кредит)"); }
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
