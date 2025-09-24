using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.SalesChannels
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ESalesChannelType
    {
        Store,
        Funnel,
        Triggers,
        Vk,
        Instagram,
        Yandex,
        Avito,
        Google,
        Facebook,
        Bonus,
        Reseller,
        Ok,
        Telegram,
        Partners,
        OzonSeller,
        Dashboard,
        Module,
        FacebookFeed
    }
}
