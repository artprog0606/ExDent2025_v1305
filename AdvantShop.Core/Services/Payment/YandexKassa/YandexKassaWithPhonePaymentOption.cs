using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;

namespace AdvantShop.Payment
{
    public class YandexKassaWithPhonePaymentOption : BasePaymentOption
    {
        public YandexKassaWithPhonePaymentOption()
        { }

        public YandexKassaWithPhonePaymentOption(PaymentMethod method, float preCoast) : base(method, preCoast)
        { }

        public string Phone { get; set; }

        public override PaymentDetails GetDetails()
        {
            var phone = StringHelper.ConvertToStandardPhone(Phone);
            return new PaymentDetails { Phone = phone != null ? phone.ToString() : Phone };
        }

        public override void SetDetails(PaymentDetails details)
        {
            Phone = details.Phone;
        }

        public override string Template => UrlService.GetUrl() + "scripts/_partials/payment/extendTemplate/YandexKassaWithPhonePaymentOption.html";

        public override bool Update(BasePaymentOption temp)
        {
            var current = temp as YandexKassaWithPhonePaymentOption;
            if (current == null) return false;
            Phone = current.Phone;
            return true;
        }
    }
}
