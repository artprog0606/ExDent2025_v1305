using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Web.Admin.Models.Partners
{
    public class PartnerViewModel
    {
        public PartnerViewModel()
        {
        }

        public Partner Partner { get; set; }

        public string TypeFormatted { get; set; }
        public string BalanceFormatted { get; set; }
        public int CustomersCount { get; set; }
        public string TimeFromCreated { get; set; }
        public string ContractDateFormatted { get; set; }
        public string CouponCode { get; set; }
        public string PaymentTypeName { get; set; }

        public bool CouponTemplateExists { get; set; }
    }
}
