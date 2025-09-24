using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.IPTelephony.Zadarma
{
    // docs: https://novofon.com/instructions/api/
    public class Zadarma : IPTelephonyOperator
    {
        private const string ServiceUrl = "https://api.novofon.com"; //"https://api.zadarma.com";

        private readonly string _key;
        private readonly string _secret;

        public Zadarma()
        {
            _key = SettingsTelephony.ZadarmaKey;
            _secret = SettingsTelephony.ZadarmaSecret;
        }
        
        public override EOperatorType Type => EOperatorType.Zadarma;
        public override CallBack.CallBack CallBack => new ZadarmaCallBack();
        
        public override string GetRecordLink(int callId)
        {
            var call = CallService.GetCall(callId);
            if (call == null || call.RecordLink.IsNullOrEmpty())
                return string.Empty;

            var @params = new Dictionary<string, string>();
            if (call.RecordLink.IsNotEmpty())
                @params.Add("call_id", call.RecordLink);
            else
                @params.Add("pbx_call_id", call.CallId);

            var result = MakeRequest<ZadarmaRecordResponse>("/v1/pbx/record/request/", @params);

            if (result == null)
                return string.Empty;
            
            return result.Link.IsNullOrEmpty() 
                ? result.Links.FirstOrDefault() 
                : result.Link;
        }

        public ZadarmaCallbackResponse CreateCallBack(string from, string to)
        {
            return MakeRequest<ZadarmaCallbackResponse>("/v1/request/callback/",
                new Dictionary<string, string>()
                {
                    { "from", from },
                    { "to", to }
                });
        }
        
        private T MakeRequest<T>(string methodUrl, Dictionary<string, string> paramsDict) where T : ZadarmaResponse
        {
            if (_key.IsNullOrEmpty() || _secret.IsNullOrEmpty())
                return null;

            var paramsStr =
                paramsDict.OrderBy(key => key.Key)
                          .Select(pair => $"{pair.Key}={pair.Value}")
                          .AggregateString("&");

            var paramsMd5 =
                BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(paramsStr)))
                    .Replace("-", "")
                    .ToLower();
            
            var sign = StringHelper.EncodeTo64(
                SecurityHelper.EncodeWithHmacSha1(methodUrl + paramsStr + paramsMd5, _secret));

            var url = ServiceUrl + methodUrl;

            try
            {
                return RequestHelper.MakeRequest<T>(
                    url,
                    data: paramsStr,
                    headers: new Dictionary<string, string> { { "Authorization", _key + ":" + sign } },
                    contentType: ERequestContentType.FormUrlencoded,
                    method: ERequestMethod.GET);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message + " URL: " + url, ex);
            }
            return null;
        }
    }
}
