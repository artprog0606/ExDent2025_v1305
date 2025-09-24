using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public enum EnFeedbackAction
    {
        SendEmail,
        CreateLead,
    }

    public class SettingsFeedback
    {
        public static EnFeedbackAction FeedbackAction
        {
            get => SettingProvider.Items["FeedbackAction"].TryParseEnum<EnFeedbackAction>();
            set => SettingProvider.Items["FeedbackAction"] = value.ToString();
        }
    }
}
