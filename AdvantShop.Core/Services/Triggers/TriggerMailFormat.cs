using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerMailFormat
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public TriggerMailFormat()
        {

        }

        public TriggerMailFormat(MailFormat mail)
        {
            Subject = mail.FormatSubject;
            Body = mail.FormatText;
        }
    }
}
