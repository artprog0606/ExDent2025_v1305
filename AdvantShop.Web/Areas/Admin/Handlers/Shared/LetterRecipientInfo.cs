using System;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Customers;
using AdvantShop.Web.Admin.Models.Shared.Smses;

namespace AdvantShop.Web.Admin.Handlers.Shared
{
    /// <summary>
    /// Информация о получаетеле
    /// </summary>
    public class BaseRecipientInfo
    {
        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
    
    public class RecipientInfo
    {
        public string Email { get; set; }
        public long Phone { get; set; }
        public Guid? CustomerId { get; set; }
        public RecipientInfoCustomer Customer { get; set; }

        public static explicit operator LetterRecipientInfo(RecipientInfo info) =>
            new LetterRecipientInfo(info.Email, info.CustomerId, (Customer) info.Customer);
        
        public static explicit operator SmsRecipientInfo(RecipientInfo info) =>
            new SmsRecipientInfo(info.Phone, info.CustomerId, info.Customer);
    }
    
    public class RecipientInfoCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Organization { get; set; }
        
        public RecipientInfoCustomer(){}

        public RecipientInfoCustomer(string firstName, string lastName, string patronymic, string organization)
        {
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Organization = organization;
        }

        public RecipientInfoCustomer(Customer customer)
        {
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Patronymic = customer.Patronymic;
            Organization = customer.Organization;
        }

        public static explicit operator Customer(RecipientInfoCustomer recipient) =>
            new Customer()
            {
                FirstName = recipient.FirstName,
                LastName = recipient.LastName,
                Patronymic = recipient.Patronymic,
                Organization = recipient.Organization
            };
    }
}