using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Handlers.Shared;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public class GetLetterToCustomerModel
    {
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        
        public int TemplateId { get; set; }
        
        public string ReId { get; set; }
        
        public int? OrderId { get; set; }
        public int? LeadId { get; set; }
    }

    public class GetLetterToCustomerResult
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("userNotAgree")]
        public bool UserNotAgree { get; set; }

        public GetLetterToCustomerResult()
        {
            Subject = "";
            Text = "";
        }
    }

    public class SendLetterToCustomerModel
    {
        public Guid? CustomerId { get; set; }
        public List<Guid> CustomerIds { get; set; }
        public List<int> SubscriptionIds { get; set; }
        public List<RecipientInfo> Recipients { get; set; }
        public string Email { get; set; }
        
        public int? OrderId { get; set; }
        public int? LeadId { get; set; }

        public string Subject { get; set; }
        public string Text { get; set; }

        public string PageType { get; set; }
    }

    /// <summary>
    /// Информация о получаетеле письма
    /// </summary>
    public class LetterRecipientInfo : BaseRecipientInfo
    {
        public string Email { get; set; }
        
        public LetterRecipientInfo(){}
        
        public LetterRecipientInfo(string email, Guid? customerId, Customer customer)
        {
            Email = email;
            CustomerId = customerId;
            Customer = customer;
        }

        public LetterRecipientInfo(string email, Guid? customerId)
        {
            Email = email;
            CustomerId = customerId;
            if (customerId != null && customerId != Guid.Empty)
            {
                Customer = CustomerService.GetCustomer(customerId.Value);
            }
        }
        
        public LetterRecipientInfo(Customer c)
        {
            CustomerId = c.Id;
            Email = c.EMail;
            Customer = c;
        }

        public LetterRecipientInfo(AdvantShop.Customers.Subscription s)
        {
            CustomerId = s.CustomerId;
            Email = s.Email;
            Customer = s.CustomerId.HasValue
                ? new Customer()
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName
                }
                : null;
        }
    }
}
