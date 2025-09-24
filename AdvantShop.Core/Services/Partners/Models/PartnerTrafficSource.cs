using System;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Partners
{
    public class PartnerTrafficSource
    {
        [JsonProperty(PropertyName = "c", NullValueHandling = NullValueHandling.Ignore)]
        public string CouponCode { get; set; }

        [JsonProperty(PropertyName = "vd", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? VisitDate { get; set; }

        [JsonProperty(PropertyName = "ac")]
        public bool AppliedCoupon { get; set; }

        [JsonProperty(PropertyName = "r", NullValueHandling = NullValueHandling.Ignore)]
        public string Referrer { get; set; }

        [JsonProperty(PropertyName = "u", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "s", NullValueHandling = NullValueHandling.Ignore)]
        public string UtmSource { get; set; }

        [JsonProperty(PropertyName = "m", NullValueHandling = NullValueHandling.Ignore)]
        public string UtmMedium { get; set; }

        [JsonProperty(PropertyName = "ca", NullValueHandling = NullValueHandling.Ignore)]
        public string UtmCampaign { get; set; }

        [JsonProperty(PropertyName = "co", NullValueHandling = NullValueHandling.Ignore)]
        public string UtmContent { get; set; }

        [JsonProperty(PropertyName = "t", NullValueHandling = NullValueHandling.Ignore)]
        public string UtmTerm { get; set; }

        [JsonProperty(PropertyName = "h")]
        public string Hash { get; set; }
    }
}
