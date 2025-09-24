//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Payment
{
    public class SberBankPaymentOption : BasePaymentOption
    {
        public SberBankPaymentOption()
        {
        }

        public SberBankPaymentOption(PaymentMethod method, float preCoast) : base(method, preCoast)
        {
        }

        public string INN { get; set; }

        public override PaymentDetails GetDetails()
        {
            return new PaymentDetails {INN = INN};
        }

        public override void SetDetails(PaymentDetails details)
        {
            INN = details.INN;
        }

        public override string Template => UrlService.GetUrl() + "scripts/_partials/payment/extendTemplate/SberBankPaymentOption.html";

        public override bool Update(BasePaymentOption temp)
        {
            var current = temp as SberBankPaymentOption;
            if (current == null) return false;
            INN = current.INN;
            return true;
        }
    }

    /// <summary>
    /// Summary description for SberBank
    /// </summary>
    [PaymentKey("SberBank")]
    public class SberBank : PaymentMethod
    {
        //public float CurrencyValue { get; set; }
        public string CompanyName { get; set; }
        public string TransAccount { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string BankName { get; set; }
        public string CorAccount { get; set; }
        public string BIK { get; set; }


        public override ProcessType ProcessType => ProcessType.Javascript;

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {SberBankTemplate.BankName, BankName},
                    {SberBankTemplate.CompanyName, CompanyName},
                    {SberBankTemplate.TransAccount, TransAccount},
                    {SberBankTemplate.INN, INN},
                    {SberBankTemplate.KPP, KPP},
                    {SberBankTemplate.CorAccount, CorAccount},
                    {SberBankTemplate.BIK, BIK},
                    //{SberBankTemplate.CurrencyValue, CurrencyValue.ToString()}
                };
            }
            set
            {
                BankName = value.ElementOrDefault(SberBankTemplate.BankName);
                CompanyName = value.ElementOrDefault(SberBankTemplate.CompanyName);
                TransAccount = value.ElementOrDefault(SberBankTemplate.TransAccount);
                INN = value.ElementOrDefault(SberBankTemplate.INN);
                KPP = value.ElementOrDefault(SberBankTemplate.KPP);
                BIK = value.ElementOrDefault(SberBankTemplate.BIK);
                CorAccount = value.ElementOrDefault(SberBankTemplate.CorAccount);
                //CurrencyValue = value.ElementOrDefault(SberBankTemplate.CurrencyValue).TryParseFloat(1);
            }
        }

        public string GetBillingLink(Order order) =>
            $"paymentreceipt/sberbank?ordernumber={order.Number}{OrderService.GetPaymentReceiptHashUrlPrefix(order)}";
        
        public override string ProcessJavascriptButton(Order order)
        {
            return $"javascript:window.open('{GetBillingLink(order)}');";
        }

        public override string ButtonText => LocalizationService.GetResource("Core.Payment.Sberbank.Print");

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            var option = new SberBankPaymentOption(this, preCoast);          
            return option;
        }
    }
}