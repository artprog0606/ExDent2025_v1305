using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("NetPay")]
    public class NetPayPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ApiKey
        {
            get { return Parameters.ElementOrDefault(NetPayTemplate.ApiKey); }
            set { Parameters.TryAddValue(NetPayTemplate.ApiKey, value.DefaultOrEmpty()); }
        }

        public string AuthSign
        {
            get { return Parameters.ElementOrDefault(NetPayTemplate.AuthSign); }
            set { Parameters.TryAddValue(NetPayTemplate.AuthSign, value.DefaultOrEmpty()); }
        }

        public bool TestMode
        {
            get { return Parameters.ElementOrDefault(NetPayTemplate.TestMode).TryParseBool(true) ?? true; }
            set { Parameters.TryAddValue(NetPayTemplate.TestMode, value.ToString()); }
        }

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(NetPayTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(NetPayTemplate.SendReceiptData, value.ToString()); }
        }
        public string INN
        {
            get { return Parameters.ElementOrDefault(NetPayTemplate.INN); }
            set { Parameters.TryAddValue(NetPayTemplate.INN, value.DefaultOrEmpty()); }
        }

        public string PaymentAddress
        {
            get { return Parameters.ElementOrDefault(NetPayTemplate.PaymentAddress); }
            set { Parameters.TryAddValue(NetPayTemplate.PaymentAddress, value.DefaultOrEmpty()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/connect-netpay", "Инструкция. Подключение платежного модуля NetPay"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ApiKey) ||
                string.IsNullOrWhiteSpace(AuthSign) ||
                (SendReceiptData && string.IsNullOrWhiteSpace(INN)))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
