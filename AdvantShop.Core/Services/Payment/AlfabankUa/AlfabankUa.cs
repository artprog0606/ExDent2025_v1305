//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("AlfabankUa")]
    public class AlfabankUa : PaymentMethod, ICreditPaymentMethod
    {
        public string PartnerId { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public float FirstPayment { get; set; }
        public bool ActiveCreditPayment => true;
        public bool ShowCreditButtonInProductCard => true;
        public string CreditButtonTextInProductCard => null;

        public override Dictionary<string,string > Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {AlfabankUaTemplate.PartnerId, PartnerId},
                    {AlfabankUaTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                    {AlfabankUaTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                    {AlfabankUaTemplate.FirstPayment, FirstPayment.ToInvariantString()}
                };
            }
            set
            {
                PartnerId = value.ElementOrDefault(AlfabankUaTemplate.PartnerId);
                MinimumPrice = value.ElementOrDefault(AlfabankUaTemplate.MinimumPrice).TryParseFloat();
                MaximumPrice = value.ElementOrDefault(AlfabankUaTemplate.MaximumPrice).TryParseFloat(true);
                FirstPayment = value.ElementOrDefault(AlfabankUaTemplate.FirstPayment).TryParseFloat();
            }
        }

        public float? GetFirstPayment(float finalPrice)
        {
            return finalPrice * FirstPayment / 100;
        }

        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice) => default;

        public override PaymentForm GetPaymentForm(Order order)
        {
            return new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.GET,
                Url = "http://www.alfabank.ua/credit/bpk/index.php",
                InputValues = new NameValueCollection
                {
                    {"partner", PartnerId},
                    {"surname", order.OrderCustomer.LastName},
                    {"name", order.OrderCustomer.FirstName},
                    {"midname", ""},
                    {"phone", (order.OrderCustomer.StandardPhone.HasValue ? order.OrderCustomer.StandardPhone.ToString() : string.Empty)},
                    {"email", order.OrderCustomer.Email},
                    {"product", string.Join(",", order.OrderItems.Select(item => HttpUtility.HtmlEncode(item.Name)))}
                }
            };
        }
    }
}