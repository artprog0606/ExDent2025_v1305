using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("PayPalExpressCheckout")]
    public class PayPalExpressCheckoutPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string User
        {
            get { return Parameters.ElementOrDefault(PayPalExpressCheckoutTemplate.User); }
            set { Parameters.TryAddValue(PayPalExpressCheckoutTemplate.User, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(PayPalExpressCheckoutTemplate.Password); }
            set { Parameters.TryAddValue(PayPalExpressCheckoutTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string Signature
        {
            get { return Parameters.ElementOrDefault(PayPalExpressCheckoutTemplate.Signature); }
            set { Parameters.TryAddValue(PayPalExpressCheckoutTemplate.Signature, value.DefaultOrEmpty()); }
        }
        
        public bool Sandbox
        {
            get { return Parameters.ElementOrDefault(PayPalExpressCheckoutTemplate.Sandbox).TryParseBool(); }
            set { Parameters.TryAddValue(PayPalExpressCheckoutTemplate.Sandbox, value.ToString()); }
        }
        
        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/paypal", "Инструкция. Подключение платежного модуля \"Pay Pal Express Checkout\""); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(User) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Signature))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
