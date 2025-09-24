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
    public class BillKzPaymentOption : BasePaymentOption
    {
        private readonly bool _showPaymentDetails;

        public BillKzPaymentOption()
        {
        }

        public BillKzPaymentOption(BillKz method, float preCoast) : base(method, preCoast)
        {
            _showPaymentDetails = method.ShowPaymentDetails;
        }

        public string CompanyName { get; set; }
        public string BINIIN { get; set; }
        public string Contract { get; set; }

        public override PaymentDetails GetDetails()
        {
            return new PaymentDetails {CompanyName = CompanyName, INN = BINIIN, Contract = Contract };
        }

        public override void SetDetails(PaymentDetails details)
        {
            CompanyName = details.CompanyName;
            BINIIN = details.INN;
            Contract = details.Contract;
        }

        public override string Template
        {
            get { return _showPaymentDetails ? UrlService.GetUrl() + "scripts/_partials/payment/extendTemplate/BillKzPaymentOption.html" : string.Empty; }//todo
        }

        public override bool Update(BasePaymentOption temp)
        {
            var current = temp as BillKzPaymentOption;
            if (current == null) return false;
            BINIIN = current.BINIIN;
            CompanyName = current.CompanyName;
            Contract = current.Contract;
            return true;
        }
    }

    /// <summary>
    /// Summary description for Bill
    /// </summary>
    [PaymentKey("BillKz")]
    public class BillKz : PaymentMethod
    {
        public string CompanyName { get; set; }
        public string IIK { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string BINIIN { get; set; }
        public string KNP { get; set; }
        public string KBE { get; set; }
        public string BIK { get; set; }
        public string BankName { get; set; }
        public string Contractor { get; set; }
        public string PosContractor { get; set; }
        public string StampImageName { get; set; }
        public bool ShowPaymentDetails { get; set; }

        public override ProcessType ProcessType => ProcessType.Javascript;

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {BillKzTemplate.CompanyName, CompanyName},
                    {BillKzTemplate.IIK, IIK},
                    {BillKzTemplate.Address, Address},
                    {BillKzTemplate.Telephone, Telephone},
                    {BillKzTemplate.BINIIN, BINIIN},
                    {BillKzTemplate.KNP, KNP},
                    {BillKzTemplate.KBE, KBE},
                    {BillKzTemplate.BIK, BIK},
                    {BillKzTemplate.BankName, BankName},
                    {BillKzTemplate.Contractor, Contractor},
                    {BillKzTemplate.StampImageName, StampImageName},
                    {BillKzTemplate.PosContractor, PosContractor},
                    {BillKzTemplate.ShowPaymentDetails, ShowPaymentDetails.ToString()}
                };
            }
            set
            {
                CompanyName = value.ElementOrDefault(BillKzTemplate.CompanyName);
                Contractor = value.ElementOrDefault(BillKzTemplate.Contractor);
                IIK = value.ElementOrDefault(BillKzTemplate.IIK);
                Address = value.ElementOrDefault(BillKzTemplate.Address);
                Telephone = value.ElementOrDefault(BillKzTemplate.Telephone);
                BINIIN = value.ElementOrDefault(BillKzTemplate.BINIIN);
                KNP = value.ElementOrDefault(BillKzTemplate.KNP);
                KBE = value.ElementOrDefault(BillKzTemplate.KBE);
                BIK = value.ElementOrDefault(BillKzTemplate.BIK);
                BankName = value.ElementOrDefault(BillKzTemplate.BankName);
                StampImageName = value.ElementOrDefault(BillKzTemplate.StampImageName);
                PosContractor = value.ElementOrDefault(BillKzTemplate.PosContractor);
                ShowPaymentDetails = value.ElementOrDefault(BillKzTemplate.ShowPaymentDetails).TryParseBool();
            }
        }

        public string GetBillingLink(Order order) =>
            $"paymentreceipt/billkz?ordernumber={order.Number}{OrderService.GetPaymentReceiptHashUrlPrefix(order)}";

        public override string ProcessJavascriptButton(Order order)
        {
            return $"javascript:window.open('{GetBillingLink(order)}');";
        }

        public override string ButtonText => LocalizationService.GetResource("Core.Payment.Bill.PrintBill");

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            var option = new BillKzPaymentOption(this, preCoast);            
            return option;
        }

        public override PaymentDetails PaymentDetails()
        {
            return new PaymentDetails() {CompanyName = CompanyName, INN = BINIIN};
        }
    }
}