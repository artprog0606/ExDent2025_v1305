//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Configuration
{ 
    public class SettingsPriceRules
    {
        public static bool ShowAmountsTableInProduct
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowAmountsTableInProduct"]);
            set => SettingProvider.Items["ShowAmountsTableInProduct"] = value.ToString();
        }

        public static bool ShowAmountsTableInCatalog
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowAmountsTableInCatalog"]);
            set => SettingProvider.Items["ShowAmountsTableInCatalog"] = value.ToString();
        }
        
        public static bool ShowPriceAmountNextDiscountsInCart
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowPriceAmountNextDiscountsInCart"]);
            set => SettingProvider.Items["ShowPriceAmountNextDiscountsInCart"] = value.ToString();
        }
    }
}