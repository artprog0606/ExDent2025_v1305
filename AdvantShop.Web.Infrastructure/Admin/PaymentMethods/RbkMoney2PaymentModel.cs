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
    [PaymentAdminModel("Rbkmoney2")]
    public class RbkMoney2PaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ShopId
        {
            get { return Parameters.ElementOrDefault(Rbkmoney2Template.ShopId); }
            set { Parameters.TryAddValue(Rbkmoney2Template.ShopId, value.DefaultOrEmpty()); }
        }

        public string ApiKey
        {
            get { return Parameters.ElementOrDefault(Rbkmoney2Template.ApiKey); }
            set { Parameters.TryAddValue(Rbkmoney2Template.ApiKey, value.DefaultOrEmpty()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/paymentrbkmoney", "Инструкция. Подключение платежного модуля RBK Money"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShopId.IsNullOrEmpty() || ApiKey.IsNullOrEmpty())
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
