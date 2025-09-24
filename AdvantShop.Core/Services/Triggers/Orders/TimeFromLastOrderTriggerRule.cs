using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Customers;
using AdvantShop.Letters;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Orders
{
    public class TimeFromLastOrderTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType => ETriggerObjectType.Customer;

        public override ETriggerEventType EventType => ETriggerEventType.TimeFromLastOrder;

        public override ETriggerProcessType ProcessType => ETriggerProcessType.Datetime;

        public override List<LetterFormatKey> AvailableVariables
        {
            get
            {
                var customerVariables = BonusSystem.IsActive
                    ? LetterBuilderHelper.GetLetterFormatKeys<CustomerLetterTemplateKey>()
                    : LetterBuilderHelper.GetLetterFormatKeys(Enum.GetValues(typeof(CustomerLetterTemplateKey))
                    .Cast<CustomerLetterTemplateKey>()
                    .Where(x => x != CustomerLetterTemplateKey.BonusBalance)
                    .ToList());

                return customerVariables
                    .Concat(LetterBuilderHelper.GetLetterFormatKeys<TriggerLetterTemplateKey>())
                    .ToList();
            }
        }

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

        public override TriggerMailFormat GetDefaultMailTemplate() => null;

        public override IEnumerable<ITriggerObject> GeTriggerObjects()
        {
            if (EventObjValue == null || EventObjValue < 0)
                return null;

            var customers = CustomerService.GetCustomersByDaysFromLastOrder(EventObjValue.Value);

            return customers.Select(x => (ITriggerObject)x).ToList();
        }
    }
}
