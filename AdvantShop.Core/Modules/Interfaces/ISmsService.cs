using System.Collections.Generic;
using AdvantShop.Core.Services.Smses;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface ISmsService : IModule
    {
        string SendSms(SmsParameters parameters);
    }
    
    public interface ISmsAndSocialMediaService : IModule
    {
        /// <summary>
        /// Send sms or message on social media.
        /// If templateId and templateParameters are specified, message will be sent by cascade (social media -> sms). Otherwise sms.
        /// </summary>
        /// <returns></returns>
        string SendSms(SmsAndSocialMediaParameters parameters);

        List<ISmsTemplate> GetTemplates();
    }

    public interface ISmsTemplate
    {
        int Id { get; set; }
        string Text { get; set; }
    }

    public class SmsParameters
    {
        public long Phone { get; set; }
        public string Text { get; set; }
        public bool IsAuthCode { get; set; }
    }

    public class SmsAndSocialMediaParameters : SmsParameters
    {
        public int TemplateId { get; set; }
        public Dictionary<string, string> TemplateParameters { get; set; }
    }
}
