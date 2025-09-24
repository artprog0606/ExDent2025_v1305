//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Payment
{
    public interface IPaymentGiftCertificate
    {
        PaymentMethod GetDefaultPaymentCertificate();
    }

    [PaymentKey("GiftCertificate")]
    public class PaymentGiftCertificate : PaymentMethod, IPaymentGiftCertificate, IPaymentCurrencyHide
    {
        public override ProcessType ProcessType
        {
            get { return ProcessType.None; }
        }

        public PaymentMethod GetDefaultPaymentCertificate()
        {
            var certificateMethod = new PaymentGiftCertificate
            {
                Enabled = true,
                Name = LocalizationService.GetResource("Core.Payment.GiftCertificate.PaymentTitle"),
                Description = LocalizationService.GetResource("Core.Payment.GiftCertificate.PaymentDescription"),
                SortOrder = 0,
            };
            return certificateMethod;
        }
    }
}