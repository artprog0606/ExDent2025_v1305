using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Shared;

namespace AdvantShop.Web.Admin.Models.Shared.Smses
{
    public class SendSmsModel
    {
        public Guid? CustomerId { get; set; }
        public string Phone { get; set; }
        public int? OrderId { get; set; }
        public int? LeadId { get; set; }

        public List<Guid> CustomerIds { get; set; }
        public List<int> SubscriptionIds { get; set; }
        public List<RecipientInfo> Recipients { get; set; }

        public string Text { get; set; }

        public string PageType { get; set; }
        
        public bool? ThrowError { get; set; }
        
        public int? ModuleTemplateId { get; set; }
    }

    /// <summary>
    /// Информация о получаетеле sms
    /// </summary>
    public class SmsRecipientInfo : BaseRecipientInfo
    {
        public long? Phone { get; set; }
        
        public SmsRecipientInfo() { }

        public SmsRecipientInfo(long phone, Guid? customerId, RecipientInfoCustomer customer)
        {
            Phone = phone;
            CustomerId = customerId;
            Customer = (Customer) customer;
        }
        
        public SmsRecipientInfo(Customer c)
        {
            Phone = c.StandardPhone ?? StringHelper.ConvertToStandardPhone(c.Phone, true, true);
            CustomerId = c.Id;
            Customer = c;
        }

        public SmsRecipientInfo(Subscription subscription)
        {
            Phone = subscription.StandardPhone;
            CustomerId = subscription.CustomerId;
            Customer = subscription.CustomerId.HasValue
                ? new Customer()
                {
                    FirstName = subscription.FirstName,
                    LastName = subscription.LastName
                }
                : null;
        }
    }

    public class SendSmsModelItemComparer : IEqualityComparer<SmsRecipientInfo>
    {
        public bool Equals(SmsRecipientInfo x, SmsRecipientInfo y)
        {
            return x.Phone == y.Phone;
        }

        public int GetHashCode(SmsRecipientInfo obj)
        {
            return (obj.Phone ?? 0).GetHashCode();
        }
    }

}
