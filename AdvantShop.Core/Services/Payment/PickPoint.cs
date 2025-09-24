//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Customers;

namespace AdvantShop.Payment
{
    public class PickPointOption : BasePaymentOption
    {
        public PickPointOption()
        {
        }

        public PickPointOption(PaymentMethod method, float preCoast) : base(method, preCoast)
        {
        }

        public override PaymentDetails GetDetails()
        {
            return new PaymentDetails
            {
                IsPickPointPayment = true
            };
        }
    }

    [PaymentKey("PickPoint")]
    public class PickPoint : PaymentMethod
    {
        public int ShippingMethodId { get; set; }

        public override ProcessType ProcessType => ProcessType.None;

        public const string ShippingMethodTemplate = "ShippingMethod";

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

        public static List<string> _availableShippingKeys;
        public static List<string> GetAvailableShippingKeys()
        {
            if (_availableShippingKeys == null)
                _availableShippingKeys =
                    ReflectionExt.GetTypesWith<ShippingKeyAttribute>(false)
                        .Where(x => x.GetInterfaces().Contains(typeof(IShippingSupportingPaymentPickPoint)) && x.IsSubclassOf(typeof(BaseShipping)))
                        .Select(AttributeHelper.GetAttributeValue<ShippingKeyAttribute, string>)
                        .ToList();

            return _availableShippingKeys;
        }

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            if (this.Parameters[ShippingMethodTemplate].TryParseInt() != shippingOption.MethodId ||
                !shippingOption.IsAvailablePaymentPickPoint)
                return null;

            var option = new PickPointOption(this, preCoast);

            var descriptionForPayment = shippingOption.GetDescriptionForPayment();
            if (descriptionForPayment.IsNotEmpty())
                option.Desc += "<br />" + descriptionForPayment;

            return option;
        }
    }
}