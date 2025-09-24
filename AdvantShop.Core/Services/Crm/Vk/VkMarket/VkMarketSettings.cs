using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public class VkMarketSettings
    {
        public static string CurrencyIso3
        {
            get { return SettingProvider.Items["CurrencyIso3"]; }
            set { SettingProvider.Items["CurrencyIso3"] = value; }
        }

        public static int TokenErrorsCount
        {
            get { return SettingProvider.Items["TokenErrorsCount"].TryParseInt(); }
            set { SettingProvider.Items["TokenErrorsCount"] = value.ToString(); }
        }

        /// <summary>
        /// Переход на api с группировкой оферов
        /// </summary>
        public static bool IsGroupingApi
        {
            get { return SettingProvider.Items["IsGroupingApi"].TryParseBool(); }
            set { SettingProvider.Items["IsGroupingApi"] = value.ToString(); }
        }
    }
}
