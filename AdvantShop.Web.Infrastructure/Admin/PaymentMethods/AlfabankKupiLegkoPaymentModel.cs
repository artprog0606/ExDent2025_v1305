using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("AlfabankKupiLegko")]
    public class AlfabankKupiLegkoPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string Inn
        {
            get { return Parameters.ElementOrDefault(AlfabankKupiLegkoTemplate.Inn); }
            set { Parameters.TryAddValue(AlfabankKupiLegkoTemplate.Inn, value.DefaultOrEmpty()); }
        }

        public bool TestMode
        {
            get { return Parameters.ElementOrDefault(AlfabankKupiLegkoTemplate.TestMode).TryParseBool(true) ?? true; }
            set { Parameters.TryAddValue(AlfabankKupiLegkoTemplate.TestMode, value.ToString()); }
        }

        public string PromoCode
        {
            get { return Parameters.ElementOrDefault(AlfabankKupiLegkoTemplate.PromoCode); }
            set { Parameters.TryAddValue(AlfabankKupiLegkoTemplate.PromoCode, value.DefaultOrEmpty()); }
        }

        public string MinimumPrice
        {
            get { return Parameters.ElementOrDefault(AlfabankKupiLegkoTemplate.MinimumPrice) ?? "10"; }
            set { Parameters.TryAddValue(AlfabankKupiLegkoTemplate.MinimumPrice, value.TryParseFloat().ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(AlfabankKupiLegkoTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(AlfabankKupiLegkoTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public string FirstPayment
        {
            get { return Parameters.ElementOrDefault(AlfabankKupiLegkoTemplate.FirstPayment) ?? "0"; }
            set { Parameters.TryAddValue(AlfabankKupiLegkoTemplate.FirstPayment, value.TryParseFloat().ToInvariantString()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/alfabank-kupi-legko", "Инструкция. Подключение платежного модуля АльфаБанк (Купи Легко)"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Inn))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
