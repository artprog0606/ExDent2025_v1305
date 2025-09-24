//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Letters;

namespace AdvantShop.Mails
{
    public abstract class BaseLeadMailTemplate : MailTemplate
    {
        private readonly LeadLetterBuilder _letterBuilder;
        
        protected BaseLeadMailTemplate() { }

        protected BaseLeadMailTemplate(Lead lead)
        {
            _letterBuilder = new LeadLetterBuilder(lead);
        }

        protected override string FormatString(string text)
        {
            return _letterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _letterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new LeadLetterBuilder(null).GetKeyDescriptions();
        }
    }

    [MailTemplateType(MailType.OnLead)]
    public sealed class LeadMailTemplate : BaseLeadMailTemplate
    {
        public override MailType Type => MailType.OnLead;
        
        private LeadMailTemplate() { }

        public LeadMailTemplate(Lead lead) : base(lead)
        {
        }
    }

    [MailTemplateType(MailType.OnLeadAssigned)]
    public sealed class LeadAssignedMailTemplate : BaseLeadMailTemplate
    {
        public override MailType Type => MailType.OnLeadAssigned;
        
        private LeadAssignedMailTemplate() { }

        public LeadAssignedMailTemplate(Lead lead) : base(lead)
        {
        }
    }

    [MailTemplateType(MailType.OnLeadChanged)]
    public sealed class LeadChangedMailTemplate : BaseLeadMailTemplate
    {
        private readonly string _changesTable;
        private readonly string _modifier;

        public override MailType Type => MailType.OnLeadChanged;
        
        private LeadChangedMailTemplate() { }

        public LeadChangedMailTemplate(Lead lead, string changesTable, string modifier) : base(lead)
        {
            _changesTable = changesTable;
            _modifier = modifier;
        }

        protected override string FormatString(string formattedStr)
        {
            formattedStr = formattedStr.Replace("#MODIFIER#", _modifier);
            formattedStr = formattedStr.Replace("#CHANGES_TABLE#", _changesTable);

            formattedStr = base.FormatString(formattedStr);

            return formattedStr;
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            var dict =  base.GetFormattedParams(text);
            dict.Add("#MODIFIER#", _modifier);
            dict.Add("#CHANGES_TABLE#", _changesTable);
            return dict;
        }
        
        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return
                base.GetKeyDescriptions()
                    .Concat(new List<LetterFormatKey>()
                    {
                        new LetterFormatKey("#MODIFIER#", "Кто менял"),
                        new LetterFormatKey("#CHANGES_TABLE#", "Таблица изменений")
                    })
                    .ToList();
        }
    }

    [MailTemplateType(MailType.OnLeadCommentAdded)]
    public sealed class LeadCommentAddedMailTemplate : BaseLeadMailTemplate
    {
        public override MailType Type => MailType.OnLeadCommentAdded;

        private readonly string _author;
        private readonly string _comment;
        
        private LeadCommentAddedMailTemplate() { }

        public LeadCommentAddedMailTemplate(Lead lead, string author, string comment) : base(lead)
        {
            _author = author;
            _comment = comment;
        }

        protected override string FormatString(string formattedStr)
        {
            formattedStr = base.FormatString(formattedStr);
            formattedStr = formattedStr.Replace("#AUTHOR#", _author);
            formattedStr = formattedStr.Replace("#COMMENT#", _comment);
            return formattedStr;
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            var dict = base.GetFormattedParams(text);
            dict.Add("#AUTHOR#", _author);
            dict.Add("#COMMENT#", _comment);
            return dict;
        }
        
        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return
                base.GetKeyDescriptions()
                    .Concat(new List<LetterFormatKey>()
                    {
                        new LetterFormatKey("#AUTHOR#", "Автор"),
                        new LetterFormatKey("#COMMENT#", "Комментарий")
                    })
                    .ToList();
        }
    }

}