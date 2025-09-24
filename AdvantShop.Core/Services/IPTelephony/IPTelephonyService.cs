using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.IPTelephony
{
    public class IPTelephonyService
    {
        public static Dictionary<long, int?> PhoneOrderSources
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<long, int?>>(SettingsTelephony.PhoneOrderSources
                    ?? (SettingsTelephony.PhoneOrderSources = JsonConvert.SerializeObject(new Dictionary<long, int?>())));
            }
            set
            {
                SettingsTelephony.PhoneOrderSources = JsonConvert.SerializeObject(value);
            }
        }

        public static OrderSource GetOrderSource(string phone)
        {
            var standartPhone = phone.TryParseLong(true);
            if (standartPhone.HasValue && PhoneOrderSources.ContainsKey(standartPhone.Value) && PhoneOrderSources[standartPhone.Value].HasValue)
                return OrderSourceService.GetOrderSource(PhoneOrderSources[standartPhone.Value].Value);
            return null;
        }

        public static OrderSource GetOrderSource(int callId)
        {
            var call = CallService.GetCall(callId);
            return call != null ? GetOrderSource(call.Type == ECallType.Out ? call.SrcNum : call.DstNum) : null;
        }


    }
}
