//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Payment.Kupivkredit
{
    public class TinkoffCreditService
    {
        // Doc: https://forma.tinkoff.ru/docs/credit/help/methods/

        private const string BaseUrl = "https://forma.tinkoff.ru/api/partners/v2/";

        public TinkoffCreditService()
        {
            Initialize();
        }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        public TinkoffCreateOrderResponse CreateOrder(TinkoffCreateOrder order, bool demo = false)
        {
            return MakeRequest<TinkoffCreateOrderResponse, TinkoffCreateOrder>(demo ? "orders/create-demo" : "orders/create", order);
        }

        public TinkoffOrderInfo GetInfoOrder(string orderNumber, string login, string password)
        {
            return MakeRequest<TinkoffOrderInfo, object>("orders/" + orderNumber  + "/info", null, login, password);
        }

        #region Private methods

        private void Initialize()
        {
            SerializationSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
#if DEBUG
                Formatting = Formatting.Indented,
#endif
#if !DEBUG
                Formatting = Formatting.None,
#endif
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            DeserializationSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
        };
        }

        private T MakeRequest<T, TData>(string url, TData data = null, string login = null, string password = null)
            where T : class
            where TData : class
        {
            try
            {
                var request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (login.IsNotEmpty() && password.IsNotEmpty())
                {
                    request.Headers.Add(HttpRequestHeader.Authorization,
                        string.Format("Basic {0}",
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", login, password)))));
                }

                if (data != null)
                {
                    using (var requestStream = request.GetRequestStream())
                    {
#if DEBUG
                        string dataPost = JsonConvert.SerializeObject(data, SerializationSettings);

                        byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                        //request.ContentLength = bytes.Length;

                        requestStream.Write(bytes, 0, bytes.Length);
#endif
#if !DEBUG
                        using (StreamWriter writer = new StreamWriter(requestStream))
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                        {
                            JsonSerializer serializer = JsonSerializer.Create(SerializationSettings);
                            serializer.Serialize(jsonWriter, data);
                            jsonWriter.Flush();
                        }
#endif
                        requestStream.Close();
                    }
                }

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
#if DEBUG
                                var responseContent = "";
                                responseContent = reader.ReadToEnd();
                                return JsonConvert.DeserializeObject<T>(responseContent, DeserializationSettings);
#endif
#if !DEBUG
                                using (JsonReader jsonReader = new JsonTextReader(reader))
                                {
                                    JsonSerializer serializer = JsonSerializer.Create(DeserializationSettings);

                                    // read the json from a stream
                                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                    return serializer.Deserialize<T>(jsonReader);
                                }
#endif
                            }
                    }
                }
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
                                    Debug.Log.Error(error, ex);
                                }
                            else
                                Debug.Log.Error(ex);
                    }
                    else
                        Debug.Log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        #endregion

        #region Help

        public TinkoffNotifyData ReadNotifyData(string postPayload)
        {
            return JsonConvert.DeserializeObject<TinkoffNotifyData>(postPayload, DeserializationSettings);
        }

        #endregion
    }
}
