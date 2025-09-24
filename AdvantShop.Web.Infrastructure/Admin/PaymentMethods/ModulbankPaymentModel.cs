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
    [PaymentAdminModel("Modulbank")]
    public class ModulbankPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string MerchantId
        {
            get { return Parameters.ElementOrDefault(ModulbankTemplate.MerchantId); }
            set { Parameters.TryAddValue(ModulbankTemplate.MerchantId, value.DefaultOrEmpty()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(ModulbankTemplate.SecretKey); }
            set { Parameters.TryAddValue(ModulbankTemplate.SecretKey, value.DefaultOrEmpty()); }
        }

        public bool DemoMode
        {
            get { return Parameters.ElementOrDefault(ModulbankTemplate.DemoMode).TryParseBool(true) ?? true; }
            set { Parameters.TryAddValue(ModulbankTemplate.DemoMode, value.ToString()); }
        }

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(ModulbankTemplate.SendReceiptData).TryParseBool(true) ?? true; }
            set { Parameters.TryAddValue(ModulbankTemplate.SendReceiptData, value.ToString()); }
        }

        public string Taxation
        {
            get { return Parameters.ElementOrDefault(ModulbankTemplate.Taxation); }
            set { Parameters.TryAddValue(ModulbankTemplate.Taxation, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> Taxations
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Общая", Value = "osn"},
                    new SelectListItem() {Text = "Упрощенная (доходы)", Value = "usn_income"},
                    new SelectListItem() {Text = "Упрощенная (доходы минус расходы)", Value = "usn_income_outcome"},
                    new SelectListItem() {Text = "Единый налог на вмененный доход", Value = "envd"},
                    new SelectListItem() {Text = "Единый сельскохозяйственный налог", Value = "esn"},
                    new SelectListItem() {Text = "Патентная", Value = "patent"},
                };

                var type = types.Find(x => x.Value == Taxation);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/connect-modulbank", "Инструкция. Подключение платежного модуля Модульбанк"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(MerchantId) ||
                string.IsNullOrWhiteSpace(SecretKey) ||
                (SendReceiptData && string.IsNullOrWhiteSpace(Taxation)))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
