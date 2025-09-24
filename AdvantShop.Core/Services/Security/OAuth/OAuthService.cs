//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Core.Services.Mails;

namespace AdvantShop.Security.OpenAuth
{
    public class OAuthService
    {
        public static void AuthOrRegCustomer(Customer customer)
        {
            AuthOrRegCustomer(customer, customer.EMail);
        }

        public static void AuthOrRegCustomer(Customer customer, string identifier)
        {
            if (!CustomerService.IsExistOpenIdLinkCustomer(identifier))
            {
                if (customer.EMail == null)
                    customer.EMail = Guid.NewGuid() + "@temp.adv";

                if (!CustomerService.IsEmailExist(customer.EMail))
                {
                    CustomerService.InsertNewCustomer(customer);
                    if (customer.Id == Guid.Empty)
                        return;
                    
                    customer = CustomerService.GetCustomerByEmail(customer.EMail);

                    var registrationMail = new RegistrationMailTemplate(customer);

                    MailService.SendMailNow(SettingsMail.EmailForRegReport, registrationMail);
                }
                else
                {
                    customer = CustomerService.GetCustomerByEmail(customer.EMail);
                }
                CustomerService.AddOpenIdLinkCustomer(customer.Id, identifier);
            }
            else
            {
                customer = CustomerService.GetCustomerByOpenAuthIdentifier(identifier);
            }

            AuthorizeService.SignIn(customer.EMail, customer.Password, true, true);
        }
    }
}