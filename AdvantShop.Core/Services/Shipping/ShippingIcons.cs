//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvantShop.FilePath;

namespace AdvantShop.Shipping
{
    public static class ShippingIcons
    {
        private static readonly List<string> DeliveryAggregators = new List<string>
        {
            "edost",
            "sdek",
            "yandexdelivery",
            "yandexnewdelivery",
            "hermes",
            "russianpost",
            "dpd",
            "pec",
            "pickpoint",
            "apiship"
        };

        private static readonly Dictionary<string, string> ExternalFragmentMap = new Dictionary<string, string>
        {
            { "ems", "russianpost" },
            { "почта россии", "russianpost" },
            { "почта онлайн", "russianpost" },
            { "спср экспресс", "spsrexpress" },
            { "спср", "spsrexpress" },
            { "сдэк", "sdek" },
            { "пэк", "pec" },
            { "деловые линии", "деловыелинии" },
            { "гарантпост", "grandpost" },
            { "pony", "ponyexpress" },
            { "энергия", "energy" },
            { "боксберри", "boxberry" },
            { "dpd", "dpd"},
            { "pickpoint", "pickpoint" }
        };

        private static readonly Dictionary<string, string> ExternalTypeMap = new Dictionary<string, string>
        {
            { "pointdelivery", "selfdelivery" },
        };

        public static string GetShippingIcon(string type, string iconName, string nameFragment)
        {
            const string defaultLogo = "default.svg";

            if (!string.IsNullOrWhiteSpace(iconName) && File.Exists(FoldersHelper.GetPathAbsolut(FolderType.ShippingLogo, iconName)))
            {
                return FoldersHelper.GetPath(FolderType.ShippingLogo, iconName, false);
            }

            var presetsFolderPath = FoldersHelper.GetPath(FolderType.PresetShippingLogo, null, false);

            var typeLower = type?.ToLower() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(nameFragment)
                && (string.IsNullOrWhiteSpace(type) || DeliveryAggregators.Contains(typeLower)))
            {
                var mappedFragment = ExternalFragmentMap.FirstOrDefault(x => nameFragment.ToLower().Contains(x.Key));
                if (mappedFragment.Equals(default(KeyValuePair<string, string>)) is false)
                {
                    var shippingFragmentLogo = $"{mappedFragment.Value}.svg";

                    if (File.Exists(FoldersHelper.GetPathAbsolut(FolderType.PresetShippingLogo, shippingFragmentLogo)))
                    {
                        return presetsFolderPath + shippingFragmentLogo;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(typeLower))
            {
                return presetsFolderPath + defaultLogo;
            }

            if (ExternalTypeMap.TryGetValue(typeLower, out var externalMapValue))
            {
                var mappedName = $@"{externalMapValue}.svg";

                if (File.Exists(FoldersHelper.GetPathAbsolut(FolderType.PresetShippingLogo, mappedName)))
                {
                    return presetsFolderPath + mappedName;
                }
            }

            var shippingLogo = $"{typeLower}.svg";
            if (File.Exists(FoldersHelper.GetPathAbsolut(FolderType.PresetShippingLogo, shippingLogo)))
            {
                return presetsFolderPath + shippingLogo;
            }
            
            var module = Core.Modules.ModulesService.GetModuleShippings()
                             .FirstOrDefault(shippingModule => string.Equals(shippingModule.ShippingKey, typeLower, StringComparison.OrdinalIgnoreCase));
            if (module != null)
                return module.Icon; //base64 svg

            return presetsFolderPath + defaultLogo;
        }
    }
}
