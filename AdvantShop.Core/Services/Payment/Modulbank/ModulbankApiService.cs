using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Payment.Modulbank
{
    public class ModulbankApiService
    {
        private readonly string _merchantId;
        private readonly string _secretKey;

        public ModulbankApiService(string merchantId, string secretKey)
        {
            _merchantId = merchantId;
            _secretKey = secretKey;
        }

        public Transaction GetTransaction(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentNullException("transactionId");
            var transactionContainer = 
                MakeRequest<TransactionContainer>("https://pay.modulbank.ru/api/v1/transaction/", new Dictionary<string, string>{ {"transaction_id", transactionId} }, requestMethod: "GET");

            return transactionContainer != null ? transactionContainer.Transaction : null;
        }


        #region Private methods


        private T MakeRequest<T>(string url, Dictionary<string, string> data = null, string requestMethod = "POST")
            where T : BaseResponseObject
        {
            return Task.Run<T>((Func<Task<T>>)(async () => await MakeRequestAsync<T>(url, data, requestMethod))).Result;
        }

        private async Task<T> MakeRequestAsync<T>(string url, Dictionary<string, string> data = null, string requestMethod = "POST")
            where T : BaseResponseObject
        {
            try
            {
                var request = await CreateRequestAsync(url, data, requestMethod);
                T result = null;

                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var jsonSerializerSettings = new JsonSerializerSettings
                            {
                                ContractResolver = new DefaultContractResolver()
                                {
                                    NamingStrategy = new SnakeCaseNamingStrategy()
                                }
                            };
#if DEBUG
                            var responseContent = "";
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = await reader.ReadToEndAsync();
                            }
                            result = JsonConvert.DeserializeObject<T>(responseContent, jsonSerializerSettings);
#endif
#if !DEBUG
                            using (StreamReader sr = new StreamReader(stream))
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = JsonSerializer.Create(jsonSerializerSettings);

                                // read the json from a stream
                                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                result = serializer.Deserialize<T>(reader);
                            }
#endif
                        }
                    }
                }

                //if (result != null && !string.IsNullOrEmpty(result.Error))
                //    Debug.Log.Warn(result.Error);

                return result != null && result.Status.Equals("ok", StringComparison.OrdinalIgnoreCase) 
                    ? result 
                    : null;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Warn(error, ex);
                                }
                            else
                                Debug.Log.Warn(ex);
                    }
                    else
                        Debug.Log.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;

        }

        private async Task<HttpWebRequest> CreateRequestAsync(string url, Dictionary<string, string> data, string requestMethod)
        {
            data = data ?? new Dictionary<string, string>();

            if (data.ContainsKey("signature"))
                data.Remove("signature");

            if (data.ContainsKey("merchant"))
                data["merchant"] = _merchantId;
            else
                data.Add("merchant", _merchantId);

            if (!data.ContainsKey("salt"))
                data["salt"] = Guid.NewGuid().ToString("N");

            if (!data.ContainsKey("unix_timestamp"))
                data["unix_timestamp"] = DateTime.UtcNow.ToUnixTime().ToString();

            data.Add("signature", GetSignature(_secretKey, data));

            var queryParams = string.Join("&", data.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value))));

            if (requestMethod.Equals("get", StringComparison.OrdinalIgnoreCase))
                url += (url.Contains("?") ? "&" : "?") + queryParams;

            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 5000;
            request.Method = requestMethod;

            if (requestMethod.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(queryParams);
                request.ContentLength = bytes.Length;
                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                }
            }

            return request;
        }

        #endregion

        #region Help

        public static string GetSignature(string key, Dictionary<string, string> data)
        {
            var nameValueCollection = new NameValueCollection();
            foreach(var kvp in data)
                nameValueCollection.Add(kvp.Key, kvp.Value);

            return GetSignature(key, nameValueCollection);
        }
        
        public static string GetSignature(string key, NameValueCollection data)
        {
            var vals = new List<string>();
            
            foreach (var k in data.AllKeys
                         .Where(x => x != "signature")
                         .OrderBy(x => x))
            foreach (var value in data.GetValues(k) ?? new []{string.Empty})
            {
                if (value.IsNullOrEmpty())
                    continue;
                vals.Add($"{k}={GetBase64Val(value)}");
            }

            var values = string.Join("&", vals);

            var signature = SHA1(key + SHA1(key + values));

            return signature;
        }
        private static string GetBase64Val(object plainText)
        {
            var value = plainText == null ? string.Empty : plainText.ToString();

            var plainTextBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(plainTextBytes);
        }


        // ReSharper disable once InconsistentNaming
        private static string SHA1(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        #endregion
    }
}
