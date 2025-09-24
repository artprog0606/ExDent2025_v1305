using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Letters;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Customers
{
    public abstract class CustomerTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType => ETriggerObjectType.Customer;
    }

    public class CustomerCreatedTriggerRule : CustomerTriggerRule
    {
        public override ETriggerEventType EventType => ETriggerEventType.CustomerCreated;

        public override List<LetterFormatKey> AvailableVariables =>  
            new CustomerMailTemplate().GetKeyDescriptions()
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<TriggerLetterTemplateKey>())
                .ToList();
       
        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var customer = (Customer) triggerObject;

            _mail = new CustomerMailTemplate(customer);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var customer = (Customer) triggerObject;

            _mail = new CustomerMailTemplate(customer);

            return _mail.GetFormattedParams(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnRegistration.ToString());
            if (mail != null)
                return new TriggerMailFormat(mail);

            return null;
        }
    }
}
