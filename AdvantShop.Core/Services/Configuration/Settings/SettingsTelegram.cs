using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Telegram;
using Newtonsoft.Json;

namespace AdvantShop.Configuration
{
    public class SettingsTelegram
    {
        public static string Token
        {
            get => SettingProvider.Items["SettingsTelegram.Token"];
            set => SettingProvider.Items["SettingsTelegram.Token"] = value;
        }

        public static TelegramUser BotUser
        {
            get
            {
                var u = SettingProvider.Items["SettingsTelegram.BotUser"];
                return !string.IsNullOrWhiteSpace(u) ? JsonConvert.DeserializeObject<TelegramUser>(u) : null;
            }
            set => SettingProvider.Items["SettingsTelegram.BotUser"] = value != null ? JsonConvert.SerializeObject(value) : "";
        }
    }
}