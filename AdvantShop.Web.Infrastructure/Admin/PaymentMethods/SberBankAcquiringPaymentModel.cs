using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("SberBankAcquiring")]
    public class SberBankAcquiringPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string UserName
        {
            get { return Parameters.ElementOrDefault(SberBankAcquiringTemplate.UserName); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.UserName, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(SberBankAcquiringTemplate.Password); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.Password, value.DefaultOrEmpty()); }
        }

        public bool TestMode
        {
            get { return Parameters.ElementOrDefault(SberBankAcquiringTemplate.TestMode).TryParseBool(); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.TestMode, value.ToString()); }
        }

        public string MerchantLogin
        {
            get { return Parameters.ElementOrDefault(SberBankAcquiringTemplate.MerchantLogin); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.MerchantLogin, value.DefaultOrEmpty()); }
        }

        
        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(SberBankAcquiringTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.SendReceiptData, value.ToString()); }
        }
        
        public byte TypeFfd
        {
            get { return (byte)Parameters.ElementOrDefault(SberBankAcquiringTemplate.TypeFfd).TryParseInt((int)SberBankAcquiring.EnTypeFfd.Less1_2); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.TypeFfd, value.ToString()); }
        }

        public string Inn
        {
            get { return Parameters.ElementOrDefault(SberBankAcquiringTemplate.Inn); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.Inn, value.DefaultOrEmpty()); }
        }


        public string Email
        {
            get { return Parameters.ElementOrDefault(SberBankAcquiringTemplate.Email); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.Email, value.DefaultOrEmpty()); }
        }


        public List<SelectListItem> TypesFfd
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ФФД 1.2 и старше", Value = ((byte)SberBankAcquiring.EnTypeFfd.From1_2).ToString()},
                    new SelectListItem() {Text = "До ФФД 1.2", Value = ((byte)SberBankAcquiring.EnTypeFfd.Less1_2).ToString()},
                };

                var type = types.Find(x => x.Value == TypeFfd.ToString());
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public byte Sno
        {
            get { return (byte)Parameters.ElementOrDefault(SberBankAcquiringTemplate.Sno).TryParseInt((int)SberBankAcquiring.enSno.osn); }
            set { Parameters.TryAddValue(SberBankAcquiringTemplate.Sno, value.ToString()); }
        }

        public List<SelectListItem> TypesSno
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Общая система налогообложения", Value = ((byte)SberBankAcquiring.enSno.osn).ToString()},
                    new SelectListItem() {Text = "Упрощенная система налогообложения (доходы)", Value = ((byte)SberBankAcquiring.enSno.usn_income).ToString()},
                    new SelectListItem() {Text = "Упрощенная система налогообложения (доходы минус расходы)", Value = ((byte)SberBankAcquiring.enSno.usn_income_outcome).ToString()},
                    new SelectListItem() {Text = "Единый сельскохозяйственный налог", Value = ((byte)SberBankAcquiring.enSno.esn).ToString()},
                    new SelectListItem() {Text = "Патентная система налогообложения", Value = ((byte)SberBankAcquiring.enSno.patent).ToString()},
                };

                var type = types.Find(x => x.Value == Sno.ToString());
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }


        public bool ExistsUnitsWithOutMeasure => UnitService.GetList().Any(unit => unit.MeasureType is null);

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/sberbank-acquiring", "Инструкция. Подключение платежного модуля Сбербанк-Эквайринг"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
