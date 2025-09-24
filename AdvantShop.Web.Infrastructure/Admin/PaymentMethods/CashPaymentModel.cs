using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Cash")]
    public class CashPaymentModel : PaymentMethodAdminModel
    {

        public bool ShowPaymentDetails
        {
            get { return Parameters.ElementOrDefault(CashTemplate.ShowPaymentDetails).TryParseBool(); }
            set { Parameters.TryAddValue(CashTemplate.ShowPaymentDetails, value.ToString()); }
        }

        public bool RequiredPaymentDetails
        {
            get { return Parameters.ElementOrDefault(CashTemplate.RequiredPaymentDetails).TryParseBool(); }
            set { Parameters.TryAddValue(CashTemplate.RequiredPaymentDetails, value.ToString()); }
        }

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/cash-payment", "Инструкция. Способ оплаты Наличными"); }
        }
    }
}
