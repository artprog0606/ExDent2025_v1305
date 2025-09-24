using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("InvoiceBox")]
    public class InvoiceBoxPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ShopId
        {
            get { return Parameters.ElementOrDefault(InvoiceBoxTemplate.ShopId); }
            set { Parameters.TryAddValue(InvoiceBoxTemplate.ShopId, value.DefaultOrEmpty()); }
        }

        public string ShopId2
        {
            get { return Parameters.ElementOrDefault(InvoiceBoxTemplate.ShopId2); }
            set { Parameters.TryAddValue(InvoiceBoxTemplate.ShopId2, value.DefaultOrEmpty()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(InvoiceBoxTemplate.SecretKey); }
            set { Parameters.TryAddValue(InvoiceBoxTemplate.SecretKey, value.DefaultOrEmpty()); }
        }

        public bool TestMode
        {
            get { return Parameters.ElementOrDefault(InvoiceBoxTemplate.TestMode).TryParseBool(); }
            set { Parameters.TryAddValue(InvoiceBoxTemplate.TestMode, value.ToString()); }
        }

        public string TypePayer
        {
            get { return Parameters.ElementOrDefault(InvoiceBoxTemplate.TypePayer); }
            set { Parameters.TryAddValue(InvoiceBoxTemplate.TypePayer, value.DefaultOrEmpty()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/invoisboks", "Инструкция. Подключение платежного модуля ИнвойсБокс"); }
        }

        public List<SelectListItem> TypePayers
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Частное/физическое лицо", Value = "PRIVATE"},
                    new SelectListItem() {Text = "Организация", Value = "LEGAL"},
                };

                var type = types.Find(x => x.Value == TypePayer);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ShopId) || string.IsNullOrWhiteSpace(ShopId2) || string.IsNullOrWhiteSpace(SecretKey))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
