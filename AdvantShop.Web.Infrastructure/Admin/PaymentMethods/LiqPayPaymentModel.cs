using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("LiqPay")]
    public class LiqPayPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string MerchantId
        {
            get { return Parameters.ElementOrDefault(LiqPayTemplate.MerchantId); }
            set { Parameters.TryAddValue(LiqPayTemplate.MerchantId, value.DefaultOrEmpty()); }
        }

        public string MerchantSig
        {
            get { return Parameters.ElementOrDefault(LiqPayTemplate.MerchantSig); }
            set { Parameters.TryAddValue(LiqPayTemplate.MerchantSig, value.DefaultOrEmpty()); }
        }
        
        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/connect-liqpay", "Инструкция. Подключение платежного модуля LiqPay"); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(MerchantId) ||
                string.IsNullOrWhiteSpace(MerchantSig))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
