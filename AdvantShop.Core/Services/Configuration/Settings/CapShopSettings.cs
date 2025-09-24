using System;
using AdvantShop;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class CapShopSettings
    {
        public const string dateTimePattern = "dd.MM.yyyy hh:mm:ss";

        public static string FromEmail => CapSettingProvider.Items["FromEmail"];

        //set { CapSettingProvider.Items["FromEmail"] = value; }
        public static string FromName => CapSettingProvider.Items["FromName"];

        //set { CapSettingProvider.Items["FromName"] = value; }
        public static string FromSms => CapSettingProvider.Items["FromSms"];

        //set { CapSettingProvider.Items["FromSms"] = value; }
        public static DateTime? ConfirmDate => CapSettingProvider.Items["ConfirmDate"].TryParseDateTime(true);

        //set { CapSettingProvider.Items["ConfirmDate"] = value.HasValue ? value.Value.ToString(dateTimePattern) : ""; }
        public static string HtmlMessage => CapSettingProvider.Items["HtmlMessage"];
    }
}
