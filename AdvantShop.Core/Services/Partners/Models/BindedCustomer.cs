using System;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Partners
{
    public class BindedCustomer
    {
        public BindedCustomer()
        {
            Enabled = true;
        }

        public int PartnerId { get; set; }
        public Guid CustomerId { get; set; }
        public bool Enabled { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime? VisitDate { get; set; }
        // if customer binded on applying coupon
        public string CouponCode { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmTerm { get; set; }
        public string UtmContent { get; set; }

        private Partner _partner;
        [JsonIgnore]
        public Partner Partner
        {
            get { return _partner ?? (_partner = PartnerService.GetPartner(PartnerId)); }
        }
    }
}
