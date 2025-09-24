using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Letters;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers.Leads
{
    public abstract class LeadTriggerRule : TriggerRule
    {
        public override ETriggerObjectType ObjectType => ETriggerObjectType.Lead;
    }

    public class LeadCreatedTriggerRule : LeadTriggerRule
    {
        public override ETriggerEventType EventType => ETriggerEventType.LeadCreated;

        public override List<LetterFormatKey> AvailableVariables =>
            LetterBuilderHelper.GetLetterFormatKeys<LeadLetterTemplateKey>()
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<CommonLetterTemplateKey>())
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<TriggerLetterTemplateKey>())
                .ToList();

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var lead = (Lead)triggerObject;

            _mail = new LeadMailTemplate(lead);

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var lead = (Lead)triggerObject;

            _mail = new LeadMailTemplate(lead);

            return _mail.GetFormattedParams(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnLead.ToString());
            return mail != null ? new TriggerMailFormat(mail) : null;
        }
    }


    public class LeadStatusChangedTriggerRule : LeadTriggerRule
    {
        public override ETriggerEventType EventType => ETriggerEventType.LeadStatusChanged;

        public override List<LetterFormatKey> AvailableVariables => 
            LetterBuilderHelper.GetLetterFormatKeys<LeadLetterTemplateKey>()
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<CommonLetterTemplateKey>())
                .Concat(LetterBuilderHelper.GetLetterFormatKeys<TriggerLetterTemplateKey>())
                .ToList();

        public override string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var lead = (Lead)triggerObject;

            _mail = new LeadMailTemplate(lead);     // нет шаблона на изменение статуса лида

            return _mail.FormatValue(value, coupon, triggerCouponCode);
        }

        public override Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject, Coupon coupon, string triggerCouponCode)
        {
            var lead = (Lead)triggerObject;

            _mail = new LeadMailTemplate(lead);     // нет шаблона на изменение статуса лида

            return _mail.GetFormattedParams(value, coupon, triggerCouponCode);
        }

        public override TriggerMailFormat GetDefaultMailTemplate()
        {
            var mail = MailFormatService.GetByType(MailType.OnLead.ToString());
            return mail != null ? new TriggerMailFormat(mail) : null;
        }
    }

}
