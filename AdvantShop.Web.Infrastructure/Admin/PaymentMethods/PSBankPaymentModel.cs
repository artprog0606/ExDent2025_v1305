using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Core.Services.Payment.PSBank;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("PSBank")]
    public class PSBankPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string FirstComponent
        {
            get { return Parameters.ElementOrDefault(PSBankTemplate.FirstComponent); }
            set { Parameters.TryAddValue(PSBankTemplate.FirstComponent, value.DefaultOrEmpty()); }
        }

        public string SecondComponent
        {
            get { return Parameters.ElementOrDefault(PSBankTemplate.SecondComponent); }
            set { Parameters.TryAddValue(PSBankTemplate.SecondComponent, value.DefaultOrEmpty()); }
        }

        public string Terminal
        {
            get { return Parameters.ElementOrDefault(PSBankTemplate.Terminal); }
            set { Parameters.TryAddValue(PSBankTemplate.Terminal, value.DefaultOrEmpty()); }
        }

        public string Merchant
        {
            get { return Parameters.ElementOrDefault(PSBankTemplate.Merchant); }
            set { Parameters.TryAddValue(PSBankTemplate.Merchant, value.DefaultOrEmpty()); }
        }

        public string MerchantName
        {
            get { return Parameters.ElementOrDefault(PSBankTemplate.MerchantName); }
            set { Parameters.TryAddValue(PSBankTemplate.MerchantName, value.DefaultOrEmpty()); }
        }

        public bool UseTestMode
        {
            get { return Parameters.ElementOrDefault(PSBankTemplate.UseTestMode).TryParseBool(true) ?? true; }
            set { Parameters.TryAddValue(PSBankTemplate.UseTestMode, value.ToString().DefaultOrEmpty()); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FirstComponent) ||  string.IsNullOrWhiteSpace(SecondComponent) || string.IsNullOrWhiteSpace(Terminal) || string.IsNullOrWhiteSpace(Merchant) || string.IsNullOrWhiteSpace(MerchantName))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }

            if(FirstComponent.Length != SecondComponent.Length)
            {
                yield return new ValidationResult("Перепроверьте компоненты секретного ключа");
            }
        }
    }
}
