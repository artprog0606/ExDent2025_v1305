using System.Collections.Generic;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.Marketing.Certificates
{
    public class CertificatesSettings
    {
        public List<SelectListItem> PaymentMethods { get; set; }
        public List<SelectItemModel> Taxes { get; set; }
        public int? Tax { get; set; }

        public int PaymentSubjectType { get; set; }
        public List<SelectItemModel> PaymentSubjectTypes { get; set; }

        public int PaymentMethodType { get; set; }
        public List<SelectItemModel> PaymentMethodTypes { get; set; }

        public bool ShowCertificatePaymentMetodOnlyCoversSum { get; set; }
    }
}
