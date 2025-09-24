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
    public class BillPaymentOption : BasePaymentOption
    {
        public readonly bool RequiredPaymentDetails;

        public bool ShowPaymentDetails { get; set; }
        public string CompanyName { get; set; }
        public string INN { get; set; }
        public string Kpp { get; set; }
        
        public BillPaymentOption()
        {
        }

        public BillPaymentOption(Bill method, float preCoast) : base(method, preCoast)
        {
            RequiredPaymentDetails = method.RequiredPaymentDetails;
        }

        protected override PaymentDetails GetDetailsByOrder(Order order)
        {
            if (order == null || order.OrderCustomer == null || order.PaymentMethod == null)
                return null;
            
            var method = order.PaymentMethod as Bill;
            if (method == null)
                return null;
            
            if (method.GetCustomerDataMethod == EGetCustomerDataMethod.FromAdditionalFields)
            {
                var customerCompanyNameField =
                    CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerCompanyNameField.TryParseInt());
                var customerInnField =
                    CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerINNField.TryParseInt());
                var customerKppField =
                    CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID, method.CustomerKppField.TryParseInt());

                return new PaymentDetails
                {
                    CompanyName = customerCompanyNameField?.Value, 
                    INN = customerInnField?.Value, 
                    Kpp = customerKppField?.Value
                };
            }
            
            var isLegalEntity = order.OrderCustomer.CustomerType == CustomerType.LegalEntity;
            
            var showPaymentDetails =
                method.GetCustomerDataMethod == EGetCustomerDataMethod.InPayment
                    ? method.ShowPaymentDetails
                    : !isLegalEntity;
            
            return showPaymentDetails
                ? new PaymentDetails {CompanyName = CompanyName, INN = INN, Kpp = Kpp}
                : null;
        }
        
        public override PaymentDetails GetDetails()
        {
            return !string.IsNullOrEmpty(CompanyName) || !string.IsNullOrEmpty(INN) 
                ? new PaymentDetails {CompanyName = CompanyName, INN = INN, Kpp = Kpp}
                : null;
        }

        public override void SetDetails(PaymentDetails details)
        {
            CompanyName = details.CompanyName;
            INN = details.INN;
            Kpp = details.Kpp;
        }

        public override string Template => 
            ShowPaymentDetails 
                ? UrlService.GetUrl() + "scripts/_partials/payment/extendTemplate/BillPaymentOption.html" 
                : "";

        public override bool Update(BasePaymentOption temp)
        {
            var current = temp as BillPaymentOption;
            if (current == null) return false;
            INN = current.INN;
            CompanyName = current.CompanyName;
            Kpp = current.Kpp;
            return true;
        }
    }

    public enum EGetCustomerDataMethod
    {
        [Localize("Core.Payments.Bill.GetCustomerDataMethod.InPayment")]
        InPayment,

        [Localize("Core.Payments.Bill.GetCustomerDataMethod.FromAdditionalFields")]
        FromAdditionalFields
    }

    /// <summary>
    /// Summary description for Bill
    /// </summary>
    [PaymentKey("Bill")]
    public class Bill : PaymentMethod
    {
        public string CompanyName { get; set; }
        public string TransAccount { get; set; }
        public string CorAccount { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
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
        public bool RequiredPaymentDetails { get; set; }
        public string CustomerCompanyNameField { get; set; }
        public string CustomerINNField { get; set; }
        public string CustomerKppField { get; set; }
        public EGetCustomerDataMethod GetCustomerDataMethod { get; set; }
        public override ProcessType ProcessType => ProcessType.Javascript;

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {BillTemplate.CompanyName, CompanyName},
                    {BillTemplate.TransAccount, TransAccount},
                    {BillTemplate.CorAccount, CorAccount},
                    {BillTemplate.Address, Address},
                    {BillTemplate.Telephone, Telephone},
                    {BillTemplate.INN, INN},
                    {BillTemplate.KPP, KPP},
                    {BillTemplate.BIK, BIK},
                    {BillTemplate.BankName, BankName},
                    {BillTemplate.Director, Director},
                    {BillTemplate.Accountant, Accountant},
                    {BillTemplate.Manager, Manager},
                    {BillTemplate.StampImageName, StampImageName},
                    {BillTemplate.PosAccountant, PosAccountant},
                    {BillTemplate.PosDirector, PosDirector},
                    {BillTemplate.PosManager, PosManager},
                    {BillTemplate.ShowPaymentDetails, ShowPaymentDetails.ToString()},
                    {BillTemplate.RequiredPaymentDetails, RequiredPaymentDetails.ToString()},
                    {BillTemplate.CustomerCompanyNameField, CustomerCompanyNameField},
                    {BillTemplate.CustomerINNField, CustomerINNField},
                    {BillTemplate.CustomerKppField, CustomerKppField},
                    {BillTemplate.GetCustomerDataMethod, GetCustomerDataMethod.ToString()}
                };
            }
            set
            {
                CompanyName = value.ElementOrDefault(BillTemplate.CompanyName);
                Accountant = value.ElementOrDefault(BillTemplate.Accountant);
                TransAccount = value.ElementOrDefault(BillTemplate.TransAccount);
                CorAccount = value.ElementOrDefault(BillTemplate.CorAccount);
                Address = value.ElementOrDefault(BillTemplate.Address);
                Telephone = value.ElementOrDefault(BillTemplate.Telephone);
                INN = value.ElementOrDefault(BillTemplate.INN);
                KPP = value.ElementOrDefault(BillTemplate.KPP);
                BIK = value.ElementOrDefault(BillTemplate.BIK);
                BankName = value.ElementOrDefault(BillTemplate.BankName);
                Director = value.ElementOrDefault(BillTemplate.Director);
                Manager = value.ElementOrDefault(BillTemplate.Manager);
                StampImageName = value.ElementOrDefault(BillTemplate.StampImageName);
                PosDirector = value.ElementOrDefault(BillTemplate.PosDirector);
                PosAccountant = value.ElementOrDefault(BillTemplate.PosAccountant);
                PosManager = value.ElementOrDefault(BillTemplate.PosManager);
                ShowPaymentDetails = value.ElementOrDefault(BillTemplate.ShowPaymentDetails).TryParseBool();
                RequiredPaymentDetails = value.ElementOrDefault(BillTemplate.RequiredPaymentDetails).TryParseBool();
                CustomerCompanyNameField = value.ElementOrDefault(BillTemplate.CustomerCompanyNameField);
                CustomerINNField = value.ElementOrDefault(BillTemplate.CustomerINNField);
                CustomerKppField = value.ElementOrDefault(BillTemplate.CustomerKppField);
                GetCustomerDataMethod = value.ElementOrDefault(BillTemplate.GetCustomerDataMethod).TryParseEnum<EGetCustomerDataMethod>();
            }
        }

        public string GetBillingLink(Order order) =>
            $"paymentreceipt/bill?ordernumber={order.Number}{OrderService.GetPaymentReceiptHashUrlPrefix(order)}";
        
        public override string ProcessJavascriptButton(Order order)
        {
            return $"javascript:window.open('{GetBillingLink(order)}');";
        }

        public override string ButtonText => LocalizationService.GetResource("Core.Payment.Bill.PrintBill");

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            return new BillPaymentOption(this, preCoast)
            {
                ShowPaymentDetails =
                    GetCustomerDataMethod == EGetCustomerDataMethod.InPayment
                        ? ShowPaymentDetails
                        : customerType != Customers.CustomerType.LegalEntity

            };
        }

        public override PaymentDetails PaymentDetails()
        {
            return new PaymentDetails() { CompanyName = CompanyName, INN = INN };
        }
    }
}