using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Core.Services.Payment.Robokassa;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Robokassa")]
    public class RobokassaPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public override string SuccessUrlLabel => "Success url";
        public override string NotificationUrlLabel => "Result url";
        public override string CancelUrlLabel => "Fail url";

        public string GatewayCountry
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.GatewayCountry, "ru"); }
            set { Parameters.TryAddValue(RobokassaTemplate.GatewayCountry, value.DefaultOrEmpty()); }
        }
     
        public List<SelectListItem> Gateways
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = $"Россия", Value = "ru"},
                    new SelectListItem() {Text = $"Казахстан", Value = "kz"},
                };

                var type = types.Find(x => x.Value == GatewayCountry);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }
   
        public string MerchantLogin
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.MerchantLogin); }
            set { Parameters.TryAddValue(RobokassaTemplate.MerchantLogin, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.Password); }
            set { Parameters.TryAddValue(RobokassaTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string[] CurrencyLabels
        {
            get { return (Parameters.ElementOrDefault(RobokassaTemplate.CurrencyLabels) ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); }
            set { Parameters.TryAddValue(RobokassaTemplate.CurrencyLabels, value != null ? string.Join(",", value.Where(x => x.IsNotEmpty())) : string.Empty); }
        }

        public string CurrencyLabelsJsonString
        {
            get { return JsonConvert.SerializeObject(CurrencyLabels); }
        }

        public float MinimumPrice
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.MinimumPrice).TryParseFloat(); }
            set { Parameters.TryAddValue(RobokassaTemplate.MinimumPrice, value.ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(RobokassaTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public float FirstPayment
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.FirstPayment).TryParseFloat(); }
            set { Parameters.TryAddValue(RobokassaTemplate.FirstPayment, value.ToInvariantString()); }
        }

        public string CurrencyLabelSelectListItems
        {
            get
            {
                var listCurrencyLabels = new List<SelectListItem>();

                var merchantLogin = MerchantLogin;
                if (!string.IsNullOrEmpty(merchantLogin))
                {
                    var currencies = RobokassaHelper.GetCurrencies(merchantLogin, GatewayCountry);

                    if (currencies != null && currencies.Result != null && currencies.Result.Code == 0)
                    {

                        foreach (var group in currencies.GroupsContainer.Groups)
                        {
                            var selectListGroup = new SelectListGroup { Name = group.Description };

                            var sbp = group.CurrencyContainer.Currencies.FirstOrDefault(x => string.Equals(x.Alias, "SBP", StringComparison.InvariantCultureIgnoreCase));
                            if(sbp != null)
                                sbp.Name = "СБП";

                            var uniqueAlias = new HashSet<string>();
                            listCurrencyLabels.AddRange(group.CurrencyContainer.Currencies.Where(x => uniqueAlias.Add(x.Alias)).Select(x => new SelectListItem() { Text = x.Name, Value = x.Alias, /*Group = selectListGroup*/ }));
                        }
                    }
                }

                //var selectingListItem = listCurrencyLabels.Find(x => x.Value == CurrencyLabel);
                //if (selectingListItem != null)
                //    selectingListItem.Selected = true;

                return JsonConvert.SerializeObject(listCurrencyLabels);
            }
        }

        public string PasswordNotify
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.PasswordNotify); }
            set { Parameters.TryAddValue(RobokassaTemplate.PasswordNotify, value.DefaultOrEmpty()); }
        }

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(RobokassaTemplate.SendReceiptData, value.ToString()); }
        }

        public bool IsTest
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.IsTest).TryParseBool(); }
            set { Parameters.TryAddValue(RobokassaTemplate.IsTest, value.ToString()); }
        }

        public float Fee
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.Fee).TryParseFloat(); }
            set { Parameters.TryAddValue(RobokassaTemplate.Fee, value.ToInvariantString()); }
        }

        public string Protocol
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.Protocol, Robokassa.ProtocolForm); }
            set { Parameters.TryAddValue(RobokassaTemplate.Protocol, value); }
        }

        public List<SelectListItem> Protocols
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Платежный модуль", Value = Robokassa.ProtocolForm},
                    new SelectListItem() {Text = "Виджет", Value = Robokassa.ProtocolIframe},
                };

                var type = types.Find(x => x.Value == Protocol);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/connect-robokassa", "Инструкция. Подключение к системе Robokassa"); }
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
