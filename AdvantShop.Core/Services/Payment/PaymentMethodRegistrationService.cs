using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Helpers;

namespace AdvantShop.Core.Services.Payment
{
    public static class PaymentMethodRegistrationService
    {
        private class ApiHelloResponseDto
        {
            public string JsFilePath { get; set; }
            public string Version { get; set; }
        }

        public static string GetTBankScript()
        {
            try
            {
                var url = SettingProvider.GetConfigSettingValue("PaymentTBankRegistrationServiceUrl");

                var uri = new Uri(url);

                var responseDto = RequestHelper.MakeRequest<ApiHelloResponseDto>(
                    url + "/api/v1/hello",
                    method: ERequestMethod.GET,
                    headers: new Dictionary<string, string>
                    {
                        { "X-Adv-Id", SettingsLic.AdvId }
                    },
                    timeoutSeconds: 5);

                return new Uri(uri, responseDto.JsFilePath).ToString();
            }
            catch (Exception)
            {
                // ignore
                return string.Empty;
            }
        }
    }
}