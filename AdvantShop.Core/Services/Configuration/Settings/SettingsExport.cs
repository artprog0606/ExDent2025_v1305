using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public class SettingsExport
    {
        public static int ExportOffersBatchSize
        {
            get => SettingProvider.Items["ExportOffersBatchSize"].TryParseInt();
            set => SettingProvider.Items["ExportOffersBatchSize"] = value.ToString();
        }

        public static int ExportProductsBatchSize
        {
            get => SettingProvider.Items["ExportProductsBatchSize"].TryParseInt();
            set => SettingProvider.Items["ExportProductsBatchSize"] = value.ToString();
        }
    }
}
