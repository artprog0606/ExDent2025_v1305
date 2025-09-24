using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Payment;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public class PaymentDetailsTemplate
    {
        [TemplateDocxProperty("CompanyName", LocalizeDescription = "Core.Payment.PaymentDetails.CompanyName")]
        public string CompanyName { get; set; }

        [TemplateDocxProperty("INN", LocalizeDescription = "Core.Payment.PaymentDetails.INN")]
        public string INN { get; set; }

        [TemplateDocxProperty("Phone", LocalizeDescription = "Core.Payment.PaymentDetails.Phone")]
        public string Phone { get; set; }

        [TemplateDocxProperty("Contract", LocalizeDescription = "Core.Payment.PaymentDetails.Contract")]
        public string Contract { get; set; }

        public static explicit operator PaymentDetailsTemplate(PaymentDetails paymentDetails)
        {
            var template = new PaymentDetailsTemplate();

            template.CompanyName = paymentDetails.CompanyName;
            template.INN = paymentDetails.INN;
            template.Phone = paymentDetails.Phone;
            template.Contract = paymentDetails.Contract;

            return template;
        }
    }
}
