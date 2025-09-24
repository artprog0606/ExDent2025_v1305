namespace AdvantShop.Configuration
{
    public class SettingsWebhook
    {
        public static string WebhookSerices
        {
            get => SettingProvider.Items["SettingsWebhook.WebhookSerices"];
            set => SettingProvider.Items["SettingsWebhook.WebhookSerices"] = value.ToString();
        }
    }
}