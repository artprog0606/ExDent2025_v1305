using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Payment.BePaid
{
    public class BePaidService
    {
        private readonly string _shopId;
        private readonly string _secretKey;

        public BePaidService(string shopId, string secretKey)
        {
            _shopId = shopId;
            _secretKey = secretKey;
        }

        public CreateCheckoutResult CreateCheckout(CheckoutContainer checkout)
        {
            return MakeRequest<CreateCheckoutResult>("https://checkout.bepaid.by/ctp/api/checkouts", checkout);
        }

        public GatewayTransactionContainer GetPaymentInfoFromGateway(string uid)
        {
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            return MakeRequest<GatewayTransactionContainer>("https://gateway.bepaid.by/transactions/" + uid, requestMethod: "GET");
        }

        public ApiTransactionContainer GetPaymentInfoFromApi(string uid)
        {
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            return MakeRequest<ApiTransactionContainer>("https://api.bepaid.by/beyag/payments/" + uid, requestMethod: "GET");
        }

        public NotifyData ReadNotifyData(string postPayload)
        {
            return JsonConvert.DeserializeObject<NotifyData>(postPayload, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
        }

        #region Private methods

        private T MakeRequest<T>(string url, object data = null, string requestMethod = "POST")
            where T : class, new()
        {
            return Task.Run<T>((Func<Task<T>>)(async () => await MakeRequestAsync<T>(url, data, requestMethod))).Result;
        }

        private async Task<T> MakeRequestAsync<T>(string url, object data = null, string requestMethod = "POST")
            where T : class, new()
        {
            try
            {
                var request = await CreateRequestAsync(url, data, requestMethod);
                BePaidServiceResponse<T> result = null;

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
                            result = JsonConvert.DeserializeObject<BePaidServiceResponse<T>>(responseContent, jsonSerializerSettings);
#endif
#if !DEBUG
                            using (StreamReader sr = new StreamReader(stream))
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = JsonSerializer.Create(jsonSerializerSettings);

                                // read the json from a stream
                                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                result = serializer.Deserialize<BePaidServiceResponse<T>>(reader);
                            }
#endif
                        }
                    }
                }

                if (result != null && !string.IsNullOrEmpty(result.Error))
                    Debug.Log.Warn(result.Error);

                return result != null ? result.Result : null;
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

        private async Task<HttpWebRequest> CreateRequestAsync(string url, object data, string requestMethod)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 5000;
            request.Method = requestMethod;
            if (!requestMethod.Equals("get", StringComparison.OrdinalIgnoreCase))
                request.ContentType = "application/json";
            request.Accept = "application/json";
            //request.Credentials = new NetworkCredential(_shopId, _secretKey);
            request.Headers.Add(HttpRequestHeader.Authorization,
                string.Format("Basic {0}",
                    Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                        .GetBytes(string.Format("{0}:{1}", _shopId, _secretKey)))));

            if (data != null)
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    },
                    NullValueHandling = NullValueHandling.Ignore
                };
#if DEBUG
                string dataPost = JsonConvert.SerializeObject(data, jsonSerializerSettings);

                byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                request.ContentLength = bytes.Length;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                }
#endif
#if !DEBUG
                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    using (StreamWriter writer = new StreamWriter(requestStream))
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        JsonSerializer serializer = JsonSerializer.Create(jsonSerializerSettings);
                        serializer.Serialize(jsonWriter, data);
                        await jsonWriter.FlushAsync();
                    }
                }
#endif
            }

            return request;
        }
        #endregion
    }
}
