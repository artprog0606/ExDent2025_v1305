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
    [PaymentAdminModel("BePaid")]
    public class BePaidPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ShopId
        {
            get { return Parameters.ElementOrDefault(BePaidTemplate.ShopId); }
            set { Parameters.TryAddValue(BePaidTemplate.ShopId, value.DefaultOrEmpty()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(BePaidTemplate.SecretKey); }
            set { Parameters.TryAddValue(BePaidTemplate.SecretKey, value.DefaultOrEmpty()); }
        }

        public bool DemoMode
        {
            get { return Parameters.ElementOrDefault(BePaidTemplate.DemoMode).TryParseBool(true) ?? true; }
            set { Parameters.TryAddValue(BePaidTemplate.DemoMode, value.ToString()); }
        }

        public int? EripServiceNo
        {
            get { return Parameters.ElementOrDefault(BePaidTemplate.EripServiceNo).TryParseInt(true); }
            set { Parameters.TryAddValue(BePaidTemplate.EripServiceNo, value.ToString()); }
        }

        public string[] ActivePayments
        {
            get { return (Parameters.ElementOrDefault(BePaidTemplate.ActivePayments) ?? string.Empty).Split(","); }
            set { Parameters.TryAddValue(BePaidTemplate.ActivePayments, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> Payments
        {
            get
            {
                var partners = new List<SelectListItem>
                {
                    new SelectListItem() {Text = "Оплата банковской картой", Value = "credit_card"},
                    new SelectListItem() {Text = "Оплата через ЕРИП", Value = "erip"}
                };

                partners.Where(x => ActivePayments.Contains(x.Value)).ForEach(x => x.Selected = true);

                return partners;
            }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/bepaid", "Инструкция. Подключение платежного модуля BePaid"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ShopId) ||
                string.IsNullOrWhiteSpace(SecretKey) ||
                ActivePayments.Length == 0)
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
