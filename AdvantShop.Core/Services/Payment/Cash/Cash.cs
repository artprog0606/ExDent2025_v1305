//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Shipping;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for Cash
    /// </summary>
    [PaymentKey("Cash")]
    public class Cash : PaymentMethod, IPaymentCurrencyHide
    {
        public bool ShowPaymentDetails { get; set; }
        public bool RequiredPaymentDetails { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.None; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {CashTemplate.ShowPaymentDetails, ShowPaymentDetails.ToString()},
                    {CashTemplate.RequiredPaymentDetails, RequiredPaymentDetails.ToString()},
                };
            }
            set
            {
                ShowPaymentDetails = value.ElementOrDefault(CashTemplate.ShowPaymentDetails).TryParseBool();
                RequiredPaymentDetails = value.ElementOrDefault(CashTemplate.RequiredPaymentDetails).TryParseBool();
            }
        }
   
        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            var option = new CashPaymentOption(this, preCoast);
            return option;
        }
    }
    
    public class CashPaymentOption : BasePaymentOption
    {
        private readonly bool _showPaymentDetails;
        public readonly bool RequiredPaymentDetails;

        public CashPaymentOption()
        {
        }

        public CashPaymentOption(Cash method, float preCoast) : base(method, preCoast)
        {
            _showPaymentDetails = method.ShowPaymentDetails;
            RequiredPaymentDetails = method.RequiredPaymentDetails;
        }

        public string Change { get; set; }

        public override PaymentDetails GetDetails()
        {
            return new PaymentDetails { Change = Change };
        }

        public override void SetDetails(PaymentDetails details)
        {
            Change = details.Change;
        }

        public override string Template => _showPaymentDetails ? UrlService.GetUrl() + "scripts/_partials/payment/extendTemplate/CashPaymentOption.html" : string.Empty;

        public override bool Update(BasePaymentOption temp)
        {
            var current = temp as CashPaymentOption;
            if (current == null) return false;
            Change = current.Change;
            return true;
        }
    }
}