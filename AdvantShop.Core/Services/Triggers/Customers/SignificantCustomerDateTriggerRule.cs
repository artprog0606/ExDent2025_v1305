using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Customers;
using AdvantShop.Letters;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Customers
{
    public class SignificantCustomerDateTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType => ETriggerObjectType.Customer;

        public override ETriggerEventType EventType => ETriggerEventType.SignificantCustomerDate;

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
            var triggerParams = (TriggerParamsDaysBeforeDate) TriggerParams;

            var days = (triggerParams.Since == TriggerParamsDateSince.After ? -1 : 1) * triggerParams.Days;
            var date = DateTime.Now.AddDays(days);

            var customers = 
                CustomerService.GetCustomersByTriggersDateParams(date, triggerParams.IgnoreYear,
                    !triggerParams.IsCustomField, triggerParams.CustomFieldId.TryParseInt());

            return customers.Select(x => (ITriggerObject) x).ToList();
        }
    }
}
