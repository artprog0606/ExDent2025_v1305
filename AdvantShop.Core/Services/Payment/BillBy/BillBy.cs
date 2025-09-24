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
    public class BillByPaymentOption : BasePaymentOption
    {
        private readonly bool _showPaymentDetails;

        public BillByPaymentOption()
        {
        }

        public BillByPaymentOption(BillBy method, float preCoast) : base(method, preCoast)
        {
            _showPaymentDetails = method.ShowPaymentDetails;
        }

        public string CompanyName { get; set; }
        public string UNP { get; set; }

        public override PaymentDetails GetDetails()
        {
            return new PaymentDetails {CompanyName = CompanyName, INN = UNP };
        }

        public override void SetDetails(PaymentDetails details)
        {
            CompanyName = details.CompanyName;
            UNP = details.INN;
        }

        public override string Template => _showPaymentDetails ? UrlService.GetUrl() + "scripts/_partials/payment/extendTemplate/BillByPaymentOption.html" : string.Empty; //todo

        public override bool Update(BasePaymentOption temp)
        {
            var current = temp as BillByPaymentOption;
            if (current == null) return false;
            UNP = current.UNP;
            CompanyName = current.CompanyName;
            return true;
        }
    }

    /// <summary>
    /// Summary description for Bill
    /// </summary>
    [PaymentKey("BillBy")]
    public class BillBy : PaymentMethod
    {
        public string CompanyName { get; set; }
        public string TransAccount { get; set; }
        public string CorAccount { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string UNP { get; set; }
        public string OKPO { get; set; }
        public string BIK { get; set; }
        public string BankName { get; set; }
        public string Director { get; set; }
        public string PosDirector { get; set; }
        public string Accountant { get; set; }
        public string PosAccountant { get; set; }
        public string Manager { get; set; }
        public string PosManager { get; set; }
        public string StampImageName { get; set; }
        public bool ShowPaymentDetails { get; set; }

        public override ProcessType ProcessType => ProcessType.Javascript;

        public override Dictionary<string, string> Parameters
        {
            get =>
                new Dictionary<string, string>
                {
                    {BillByTemplate.CompanyName, CompanyName},
                    {BillByTemplate.TransAccount, TransAccount},
                    {BillByTemplate.CorAccount, CorAccount},
                    {BillByTemplate.Address, Address},
                    {BillByTemplate.Telephone, Telephone},
                    {BillByTemplate.UNP, UNP},
                    {BillByTemplate.OKPO, OKPO},
                    {BillByTemplate.BIK, BIK},
                    {BillByTemplate.BankName, BankName},
                    {BillByTemplate.Director, Director},
                    {BillByTemplate.Accountant, Accountant},
                    {BillByTemplate.Manager, Manager},
                    {BillByTemplate.StampImageName, StampImageName},
                    {BillByTemplate.PosAccountant, PosAccountant},
                    {BillByTemplate.PosDirector, PosDirector},
                    {BillByTemplate.PosManager, PosManager},
                    {BillByTemplate.ShowPaymentDetails, ShowPaymentDetails.ToString()}
                };
            set
            {
                CompanyName = value.ElementOrDefault(BillByTemplate.CompanyName);
                Accountant = value.ElementOrDefault(BillByTemplate.Accountant);
                TransAccount = value.ElementOrDefault(BillByTemplate.TransAccount);
                CorAccount = value.ElementOrDefault(BillByTemplate.CorAccount);
                Address = value.ElementOrDefault(BillByTemplate.Address);
                Telephone = value.ElementOrDefault(BillByTemplate.Telephone);
                UNP = value.ElementOrDefault(BillByTemplate.UNP);
                OKPO = value.ElementOrDefault(BillByTemplate.OKPO);
                BIK = value.ElementOrDefault(BillByTemplate.BIK);
                BankName = value.ElementOrDefault(BillByTemplate.BankName);
                Director = value.ElementOrDefault(BillByTemplate.Director);
                Manager = value.ElementOrDefault(BillByTemplate.Manager);
                StampImageName = value.ElementOrDefault(BillByTemplate.StampImageName);
                PosDirector = value.ElementOrDefault(BillByTemplate.PosDirector);
                PosAccountant = value.ElementOrDefault(BillByTemplate.PosAccountant);
                PosManager = value.ElementOrDefault(BillByTemplate.PosManager);
                ShowPaymentDetails = value.ElementOrDefault(BillByTemplate.ShowPaymentDetails).TryParseBool();
            }
        }

        public string GetBillingLink(Order order) =>
            $"paymentreceipt/billby?ordernumber={order.Number}{OrderService.GetPaymentReceiptHashUrlPrefix(order)}";

        public override string ProcessJavascriptButton(Order order)
        {
            return $"javascript:window.open('{GetBillingLink(order)}');";
        }

        public override string ButtonText => LocalizationService.GetResource("Core.Payment.Bill.PrintBill");

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            var option = new BillByPaymentOption(this, preCoast);            
            return option;
        }

        public override PaymentDetails PaymentDetails()
        {
            return new PaymentDetails() {CompanyName = CompanyName, INN = UNP};
        }
    }
}