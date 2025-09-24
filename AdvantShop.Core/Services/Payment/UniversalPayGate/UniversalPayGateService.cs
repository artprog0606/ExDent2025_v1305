using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using System;
using System.Collections.Generic;

namespace AdvantShop.Payment
{
    public class UniversalPayGateService
    {
        private const string RequestUrl = "http://modules.advantshop.net";
        private const int CacheTime = 6 * 60;

        public static List<UniversalPayGateDto> GetAvalibleMethod()
        {
            var cacheName = "capAvaliblePayMethod";
            var data = CacheManager.Get(cacheName, CacheTime, () =>
             {
                 try
                 {
                     return string.IsNullOrEmpty(SettingsLic.LicKey)
                     ? new List<UniversalPayGateDto>()
                     : RequestHelper.MakeRequest<List<UniversalPayGateDto>>(RequestUrl + "/shop/GetPaymentGateWay/" + SettingsLic.LicKey, method: ERequestMethod.GET);
                 }
                 catch (Exception e)
                 {
                     Debug.Log.Error(e);
                 }
                 return new List<UniversalPayGateDto>();
             });
            return data;

        }
    }
}
