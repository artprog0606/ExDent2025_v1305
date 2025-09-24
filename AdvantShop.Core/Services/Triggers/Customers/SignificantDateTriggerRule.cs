using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Letters;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Customers
{
    public class SignificantDateTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType => ETriggerObjectType.Customer;

        public override ETriggerEventType EventType => ETriggerEventType.SignificantDate;

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
            var customer = (Customer)triggerObject;

            _mail = new CustomerMailTemplate(customer);

            return  _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var customer = (Customer)triggerObject;

            _mail = new CustomerMailTemplate(customer);

            return  _mail.GetFormattedParams(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate() => null;

        public override IEnumerable<ITriggerObject> GeTriggerObjects()
        {
            var triggerParams = (TriggerParamsDate) TriggerParams;

            var days = (triggerParams.Since == TriggerParamsDateSince.After ? 1 : -1) * (triggerParams.Days ?? 0);
            var triggerDate = new DateTime(triggerParams.DateTime.Year, triggerParams.DateTime.Month, triggerParams.DateTime.Day).AddDays(days);
            var now = DateTime.Now;
            var nowDate = new DateTime(now.Year, now.Month, now.Day);

            IEnumerable<Customer> customers = null;

            if (!triggerParams.IgnoreYear)
            {
                if (triggerDate == nowDate)
                    customers = GetCustomers(500);
            }
            else
            {
                if (triggerDate.Day == nowDate.Day && triggerDate.Month == nowDate.Month)
                    customers = GetCustomers(500);
            }

            return customers?.Select(x => (ITriggerObject)x);
        }

        private IEnumerable<Customer> GetCustomers(int itemsPerPage)
        {
            var page = 1;
            do
            {
                var customers =
                    new SqlPaging(page, itemsPerPage)
                        .Select(
                            "c.CustomerId".AsSqlField("Id"), 
                            "c.*"
                        )
                        .From("[Customers].[Customer] as c")
                        .Where("c.CustomerRole = {0}", (int)Role.User)
                        .PageItemsList<Customer>();
                
                if (customers.Count == 0)
                    break;

                foreach (var customer in customers)
                    yield return customer;
                
                page++;
            } while (true);
        }
    }
}
