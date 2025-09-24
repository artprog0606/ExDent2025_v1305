using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Interkassa2")]
    public class Interkassa2PaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ShopId
        {
            get { return Parameters.ElementOrDefault(Interkassa2Template.ShopId); }
            set { Parameters.TryAddValue(Interkassa2Template.ShopId, value.DefaultOrEmpty()); }
        }

        public bool IsCheckSign
        {
            get { return Parameters.ElementOrDefault(Interkassa2Template.IsCheckSign).TryParseBool(); }
            set { Parameters.TryAddValue(Interkassa2Template.IsCheckSign, value.ToString()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(Interkassa2Template.SecretKey); }
            set { Parameters.TryAddValue(Interkassa2Template.SecretKey, value.DefaultOrEmpty()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/interkassa-2-0", "Инструкция. Подключение платежного модуля Interkassa"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ShopId) ||
                string.IsNullOrWhiteSpace(SecretKey))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
