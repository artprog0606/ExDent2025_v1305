using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Loging
{
    public class ActivityRequest
    {
        private const string HeaderApiKey = "x-api-key";
        private readonly string _baseUrl;

        public ActivityRequest(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        
        public bool Post(string url, object data)
        {
            var dataStr =
                data != null
                    ? JsonConvert.SerializeObject(data,
                        new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })
                    : null;
            
            try
            {
                var request = WebRequest.Create(_baseUrl + url) as HttpWebRequest;
                request.Timeout = 60_000;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers[HeaderApiKey] = SettingsLic.LicKey;

                if (dataStr != null)
                {
                    var bytes = Encoding.UTF8.GetBytes(dataStr);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                    return (int)response.StatusCode == 201;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                    if (eResponse != null)
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    var wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;

                                    Debug.Log.Error($"ActivityRequest {(int)wRespStatusCode} error on POST {url} {dataStr} {error}", ex);
                                    return false;
                                }
                    
                Debug.Log.Error($"ActivityRequest error on POST {url} {dataStr}", ex);
            }

            return false;
        }

        public T Get<T>(string url)
        {
            try
            {
                var request = WebRequest.Create(_baseUrl + url) as HttpWebRequest;
                request.Timeout = 10_000;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers[HeaderApiKey] = SettingsLic.LicKey;

                var responseContent = "";
                
                using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                                responseContent = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                    if (eResponse != null)
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    var wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;

                                    Debug.Log.Error($"ActivityRequest {(int)wRespStatusCode} error on GET {url} {error}", ex);
                                    return default(T);
                                }
                
                Debug.Log.Error($"ActivityRequest error on GET {url}", ex);
            }
            
            return default(T);
        }
    }
}