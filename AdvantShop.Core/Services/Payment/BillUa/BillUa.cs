using System.Collections.Generic;
using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("BillUa")]
    public class BillUa : PaymentMethod
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyEssentials { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Credit { get; set; }

        public override ProcessType ProcessType => ProcessType.Javascript;

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {BillUaTemplate.CompanyName, CompanyName},
                    {BillUaTemplate.CompanyCode, CompanyCode},
                    {BillUaTemplate.CompanyEssentials, CompanyEssentials},
                    {BillUaTemplate.BankName, BankName},
                    {BillUaTemplate.BankCode, BankCode},
                    {BillUaTemplate.Credit, Credit}
                };
            }
            set
            {
                CompanyName = value.ElementOrDefault(BillUaTemplate.CompanyName);
                CompanyCode = value.ElementOrDefault(BillUaTemplate.CompanyCode);
                CompanyEssentials = value.ElementOrDefault(BillUaTemplate.CompanyEssentials);
                BankName = value.ElementOrDefault(BillUaTemplate.BankName);
                BankCode = value.ElementOrDefault(BillUaTemplate.BankCode);
                Credit = value.ElementOrDefault(BillUaTemplate.Credit);
            }
        }

        public string GetBillingLink(Order order) =>
            $"paymentreceipt/billua?ordernumber={order.Number}{OrderService.GetPaymentReceiptHashUrlPrefix(order)}";

        public override string ProcessJavascriptButton(Order order)
        {
            return $"javascript:window.open('{GetBillingLink(order)}');";
        }

        public override string ButtonText => LocalizationService.GetResource("Core.Payment.Bill.PrintBill");
    }
}