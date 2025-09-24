using System;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Core.Services.Mails;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mails
{
    public class SendTestMessageHandler
    {
        public bool Execute(SendTestMessageModel model)
        {
            MailService.SendMail(Guid.Empty, model.To, model.Subject, model.Body, true, needretry: false, logging: false);
            return true;
        }
    }
}
