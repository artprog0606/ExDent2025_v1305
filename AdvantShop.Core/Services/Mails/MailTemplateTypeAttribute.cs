using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Mails
{
    public class MailTemplateTypeAttribute : Attribute, IAttribute<MailType>
    {
        private readonly MailType _key;

        public MailTemplateTypeAttribute(MailType key)
        {
            _key = key;
        }

        public MailType Value => _key;
    }
}