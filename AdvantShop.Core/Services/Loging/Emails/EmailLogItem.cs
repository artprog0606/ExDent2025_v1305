using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public class EmailLogItem
    {
        public DateTime CreateOn { get; set; }
        public DateTime? Updated { get; set; }        
        public Guid CustomerId { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EmailStatus Status { get; set; }
        public string ShopId { get; set; }

        public int Id
        {
            get
            {
                unchecked
                {
                    int hash = (int)2166136261;
                    hash = (hash * 16777619) ^ (!string.IsNullOrEmpty(Subject) ? Subject.GetHashCode() : 0);
                    hash = (hash * 16777619) ^ (!string.IsNullOrEmpty(Body) ? Body.GetHashCode() : 0);
                    hash = (hash * 16777619) ^ (!string.IsNullOrEmpty(EmailAddress) ? EmailAddress.GetHashCode() : 0);
                    hash = (hash * 16777619) ^ (!string.IsNullOrEmpty(ShopId) ? ShopId.GetHashCode() : 0);
                    hash = (hash * 16777619) ^ Status.GetHashCode();
                    hash = (hash * 16777619) ^ CustomerId.GetHashCode();
                    hash = (hash * 16777619) ^ CreateOn.GetHashCode();

                    return hash;
                }
            }
        }
    }
}
