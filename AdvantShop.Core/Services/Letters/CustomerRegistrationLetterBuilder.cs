using System;
using AdvantShop.Customers;

namespace AdvantShop.Letters
{
    public sealed class CustomerRegistrationLetterBuilder : BaseLetterTemplateBuilder<Customer, CustomerRegistrationLetterTemplateKey>
    {
        public CustomerRegistrationLetterBuilder(Customer customer) : base(customer, null)
        {
        }

        protected override string GetValue(CustomerRegistrationLetterTemplateKey key)
        {
            var customer = _entity;

            switch (key)
            {
                case CustomerRegistrationLetterTemplateKey.RegistrationDate:
                    return Localization.Culture.ConvertDate(
                        customer.RegistrationDateTime != DateTime.MinValue
                            ? customer.RegistrationDateTime
                            : DateTime.Now);
                
                case CustomerRegistrationLetterTemplateKey.Password: return customer.Password;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}