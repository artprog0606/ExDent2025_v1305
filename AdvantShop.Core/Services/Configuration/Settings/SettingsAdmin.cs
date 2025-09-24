using System;

namespace AdvantShop.Configuration
{
    public class SettingsAdmin
    {
        public static bool ShowOrderItemsInGrid
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowOrderItemsInGrid"]);
            set => SettingProvider.Items["ShowOrderItemsInGrid"] = value.ToString();
        }

        public static bool ShowViberDesktopAppNotification
        {
            get 
            {
                var value = SettingProvider.Items["ShowViberDesktopAppNotification"];
                return value == null ? true : Convert.ToBoolean(value); 
            }
            set => SettingProvider.Items["ShowViberDesktopAppNotification"] = value.ToString();
        }

        public static bool ShowWhatsAppDesktopAppNotification
        {
            get
            {
                var value = SettingProvider.Items["ShowWhatsAppDesktopAppNotification"];
                return value == null ? true : Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["ShowWhatsAppDesktopAppNotification"] = value.ToString();
        }
        
        public static bool ShowTelegramDesktopAppNotification
        {
            get
            {
                var value = SettingProvider.Items["ShowTelegramDesktopAppNotification"];
                return value == null ? true : Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["ShowTelegramDesktopAppNotification"] = value.ToString();
        }
        
        public static bool CatalogGridWithoutLimits
        {
            get => Convert.ToBoolean(SettingProvider.Items["CatalogGridWithoutLimits"]);
            set => SettingProvider.Items["CatalogGridWithoutLimits"] = value.ToString();
        }
    }
}
