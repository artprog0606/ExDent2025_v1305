using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Helpers;

namespace AdvantShop.Core.Services.Translators
{
    public class AdvantshopTranslateService
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>() { { "lic", SettingsLic.LicKey } };

        public string Translate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return CacheManager.Get("AdvantshopTranslate_" + text,
                () =>
                {
                    var translations = RequestHelper.MakeRequest<List<string>>(SettingsLic.ImageServiceUrl + "v1/translate?text=" + text, 
                        new { lickey = SettingsLic.LicKey}, _headers);

                    return translations != null && translations.Count > 0 ? translations[0] : "";
                });
        }
    }
}
