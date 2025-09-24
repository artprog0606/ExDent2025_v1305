//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Shipping;

namespace AdvantShop.Payment
{
    public class CashOnDeliverytOption : BasePaymentOption
    {
        public CashOnDeliverytOption()
        {
        }

        public CashOnDeliverytOption(PaymentMethod method, float preCoast)
            : base(method, preCoast)
        {
        }

        public override PaymentDetails GetDetails()
        {
            return new PaymentDetails
            {
                IsCashOnDeliveryPayment = true
            };
        }
    }

    /// <summary>
    /// cash on delivery
    /// </summary>
    [PaymentKey("CashOnDelivery")]
    public class CashOnDelivery : PaymentMethod, IPaymentCurrencyHide
    {
        public int ShippingMethodId { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {ShippingMethodTemplate, ShippingMethodId.ToString()}
                };
            }
            set { ShippingMethodId = value.ElementOrDefault(ShippingMethodTemplate).TryParseInt(); }
        }

        public override ProcessType ProcessType => ProcessType.None;

        public const string ShippingMethodTemplate = "ShippingMethod";

        private static List<string> _availableShippingKeys;

        public static List<string> GetAvailableShippingKeys()
        {
            return _availableShippingKeys ??
                   (_availableShippingKeys =
                       ReflectionExt.GetTypesWith<ShippingKeyAttribute>(false)
                           .Where(x => x.GetInterfaces().Contains(typeof(IShippingSupportingPaymentCashOnDelivery)) &&
                                       x.IsSubclassOf(typeof(BaseShipping)))
                           .Select(AttributeHelper.GetAttributeValue<ShippingKeyAttribute, string>)
                           .ToList());
        }

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            if (Parameters[ShippingMethodTemplate].TryParseInt() != shippingOption.MethodId || !shippingOption.IsAvailablePaymentCashOnDelivery)
                return null;

            var option = new CashOnDeliverytOption(this, preCoast);

            var descriptionForPayment = shippingOption.GetDescriptionForPayment();
            if (descriptionForPayment.IsNotEmpty())
                option.Desc += "<br />" + descriptionForPayment;

            return option;
        }
    }
}