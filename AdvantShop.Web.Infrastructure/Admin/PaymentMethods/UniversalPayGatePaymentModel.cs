using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("UniversalPayGate")]
    public class UniversalPayGatePaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string MerchantLogin
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.MerchantLogin); }
            set { Parameters.TryAddValue(UniversalPayGateTemplate.MerchantLogin, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.Password); }
            set { Parameters.TryAddValue(UniversalPayGateTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string PasswordNotify
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.PasswordNotify); }
            set { Parameters.TryAddValue(UniversalPayGateTemplate.PasswordNotify, value.DefaultOrEmpty()); }
        }

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(UniversalPayGateTemplate.SendReceiptData, value.ToString()); }
        }

        public bool IsTest
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.IsTest).TryParseBool(); }
            set { Parameters.TryAddValue(UniversalPayGateTemplate.IsTest, value.ToString()); }
        }

        public string Code
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.Code); }
            set
            {
                Parameters.TryAddValue(UniversalPayGateTemplate.Code, value);
                var temp = UniversalPayGates.FirstOrDefault(x => x.Code == value);
                if (temp != null)
                {
                    Parameters.TryAddValue(UniversalPayGateTemplate.Url, temp.Url);
                    Parameters.TryAddValue(UniversalPayGateTemplate.UrlTest, temp.UrlTest);
                }
            }
        }

        public string Url
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.Url); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value)
                    && !Parameters.ContainsKey(UniversalPayGateTemplate.Url))
                    Parameters.TryAddValue(UniversalPayGateTemplate.Url, value);
            }
        }

        public string UrlTest
        {
            get { return Parameters.ElementOrDefault(UniversalPayGateTemplate.UrlTest); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value)
                    && !Parameters.ContainsKey(UniversalPayGateTemplate.UrlTest))
                    Parameters.TryAddValue(UniversalPayGateTemplate.UrlTest, value);
            }
        }

        private List<UniversalPayGateDto> _universalPayGates;
        public List<UniversalPayGateDto> UniversalPayGates
        {
            get { return _universalPayGates ?? (_universalPayGates = UniversalPayGateService.GetAvalibleMethod()); }
            set { _universalPayGates = value; }
        }

        public override Tuple<string, string> Instruction
        {
            get
            {
                var name = "";
                var link = "";

                var temp = UniversalPayGates.FirstOrDefault(x => x.Code == Code);
                if (temp != null)
                {
                    if (!string.IsNullOrEmpty(temp.LinkHelp))
                        link = temp.LinkHelp;
                    if (!string.IsNullOrEmpty(temp.TextLinkHelp))
                        name = temp.TextLinkHelp;

                    if (string.IsNullOrEmpty(name))
                        name = string.Format("Инструкция. Подключение платежного модуля \"{0}\"", temp.Name);
                    if (string.IsNullOrEmpty(link))
                        link = " https://www.advantshop.net/help/pages/invoice";
                }
                else
                {
                    name = "Инструкция по использованию универсальным шлюзом";
                    link = "http://www.advantshop.net/help/pages/connect-universal-pay-gate";
                }

                return new Tuple<string, string>(link, name);
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(MerchantLogin) ||
                string.IsNullOrWhiteSpace(PasswordNotify) ||
                string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
