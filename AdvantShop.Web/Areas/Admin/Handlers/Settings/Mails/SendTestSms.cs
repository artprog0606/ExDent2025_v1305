using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mails
{
    public class SendTestSms
    {
        private readonly string _phone;
        private readonly string _text;

        public SendTestSms(string phone, string text)
        {
            _phone = phone;
            _text = text;
        }

        public void Execute()
        {
            try
            {
                var phone = StringHelper.ConvertToStandardPhone(_phone, true, true);
                if (phone.HasValue)
                    SmsNotifier.SendTestSms(phone.Value, _text);
            }
            catch (Exception ex)
            {
                throw new BlException(ex.Message);
            }
        }
    }
}
