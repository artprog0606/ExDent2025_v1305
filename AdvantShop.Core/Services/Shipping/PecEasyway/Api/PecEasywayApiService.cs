//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.PecEasyway.Api
{
    public class PecEasywayApiService
    {
        private const string BaseUrl = "https://lk.easyway.ru/EasyWay/hs/EWA_API/v2";

        private readonly string _login;
        private readonly string _password;

        public List<string> LastActionErrors { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        public PecEasywayApiService(string login, string password)
        {
            //if (string.IsNullOrEmpty(token))
            //    throw new ArgumentNullException("token");

            _login = login;
            _password = password;

            Initialize();
        }

        public PecBaseResponse<List<CalculateDelivery>> CalculateDelivery(string locationFrom, string locationTo, float weight, double volume)
        {
            return MakeRequest<List<CalculateDelivery>>(
                string.Format("/getTariff?locationFrom={0}&locationTo={1}&weight={2}&volume={3}", 
                    locationFrom, 
                    locationTo,
                    weight.ToInvariantString(),
                    volume.ToInvariantString()),
                method: "GET");
        }

        public PecBaseResponse<List<PickupPoint>> GetPickupPoints()
        {
            return MakeRequest<List<PickupPoint>>(
                "/getPickupPoints",
                method: "GET");
        }

        public PecBaseResponse<CreatedOrderInfo> CreateOrder(PecOrder orderInfo)
        {
            return MakeRequest<CreatedOrderInfo>(
                "/createOrder",
                orderInfo);
        }

        public PecBaseResponse<CancelOrdersInfo> CancelOrder(params string[] ids)
        {
            return MakeRequest<CancelOrdersInfo>(
                "/cancelOrder",
                ids);
        }

        public PecBaseResponse<List<OrdersStatuses>> GetOrdersStatuses(params string[] ids)
        {
            return MakeRequest<List<OrdersStatuses>>(
                "/getStatus?number=" + string.Join(",", ids),
                method: "GET");
        }

        public string GetOrderLabel(params string[] ids)
        {
            return MakeRequestAsString(
                "/getLabel?number=" + string.Join(",", ids),
                method: "GET");
        }

        #region PrivateMethods

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
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        private PecBaseResponse<T> MakeRequest<T>(string url, object data = null, string method = "POST")
            where T : class, new()
        {
            return Task.Run<PecBaseResponse<T>>((Func<Task<PecBaseResponse<T>>>)(async () => await MakeRequestAsync<T>(url, data, method).ConfigureAwait(false))).Result;
        }

        private string MakeRequestAsString(string url, object data = null, string method = "POST")
        {
            return Task.Run<string>((Func<Task<string>>)(async () => await MakeRequestAsStringAsync(url, data, method).ConfigureAwait(false))).Result;
        }

        private async Task<string> MakeRequestAsStringAsync(string url, object data = null, string method = "POST")
        {
            ClearErrors();

            try
            {
                var request = await CreateRequestAsync(url, data, method).ConfigureAwait(false);
                //request.Timeout = 10000;

                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                return await reader.ReadToEndAsync().ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = (HttpWebResponse)ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(eStream))
                                    {
                                        var error = reader.ReadToEnd();
                                        if (data != null)
                                        {
                                            error += " data:" + JsonConvert.SerializeObject(data, SerializationSettings);
                                        }
                                        AddErrors(string.IsNullOrEmpty(error) ? ex.Message : error);

                                        if (string.IsNullOrEmpty(error))
                                            Debug.Log.Warn(ex);
                                        else
                                        {
                                            Debug.Log.Warn(error, ex);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    AddErrors(ex.Message);
                                    Debug.Log.Warn(ex);
                                }
                            }
                            else
                            {
                                AddErrors(ex.Message);
                                Debug.Log.Warn(ex);
                            }
                    }
                    else
                    {
                        AddErrors(ex.Message);
                        Debug.Log.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Error(ex);
            }

            return null;
        }

        private async Task<PecBaseResponse<T>> MakeRequestAsync<T>(string url, object data = null, string method = "POST")
            where T : class, new()
        {
            ClearErrors();

            try
            {
                var request = await CreateRequestAsync(url, data, method).ConfigureAwait(false);
                //request.Timeout = 10000;

                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var result = await DeserializeObjectAsync<PecBaseResponse<T>>(stream).ConfigureAwait(false);
                            if (result != null && result.Success == false)
                            {
                                if (result.Error != null && result.Error.Errors != null)
                                    result.Error.Errors.ForEach(x => {
                                        Debug.Log.Warn(string.Format("PecEasyway ErrorCode: {0}, ErrorMessage: {1}", x.Code, x.Description));
                                        AddErrors(x.Description);
                                        });
                            }

                            return result;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = (HttpWebResponse)ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(eStream))
                                    {
                                        var error = reader.ReadToEnd();
                                        if (data != null)
                                        {
                                            error += " data:" + JsonConvert.SerializeObject(data, SerializationSettings);
                                        }
                                        AddErrors(string.IsNullOrEmpty(error) ? ex.Message : error);

                                        if (string.IsNullOrEmpty(error))
                                            Debug.Log.Warn(ex);
                                        else
                                        {
                                            Debug.Log.Warn(error, ex);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    AddErrors(ex.Message);
                                    Debug.Log.Warn(ex);
                                }
                            }
                            else
                            {
                                AddErrors(ex.Message);
                                Debug.Log.Warn(ex);
                            }
                    }
                    else
                    {
                        AddErrors(ex.Message);
                        Debug.Log.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                Debug.Log.Error(ex);
            }

            return null;
        }

        private void ClearErrors()
        {
            LastActionErrors = null;
        }

        private void AddErrors(params string[] message)
        {
            if (LastActionErrors == null)
                LastActionErrors = new List<string>();

            LastActionErrors.AddRange(message);
        }

        private async Task<HttpWebRequest> CreateRequestAsync(string url, object data, string method)
        {
            var request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json;charset=UTF-8";
            //request.Credentials = new NetworkCredential(_login, _password);
            request.Headers.Add(HttpRequestHeader.Authorization,
                string.Format("Basic {0}",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _login, _password)))));

            if (data != null)
            {
                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    await SerializeObjectAsync(data, requestStream).ConfigureAwait(false);
                }
            }
            return request;
        }

        private Task SerializeObjectAsync(object data, Stream stream)
        {
#if DEBUG
            string dataPost = JsonConvert.SerializeObject(data, SerializationSettings);

            byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
            //request.ContentLength = bytes.Length;

            return stream.WriteAsync(bytes, 0, bytes.Length);
#endif
#if !DEBUG
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer serializer = JsonSerializer.Create(SerializationSettings);
                serializer.Serialize(jsonWriter, data);
                return jsonWriter.FlushAsync();
            }
#endif
        }

        private async Task<T> DeserializeObjectAsync<T>(Stream stream)
            where T : class
        {
            using (var reader = new StreamReader(stream))
            {
#if DEBUG
                var responseContent = "";
                responseContent = await reader.ReadToEndAsync().ConfigureAwait(false);
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

        #endregion
    }
}
