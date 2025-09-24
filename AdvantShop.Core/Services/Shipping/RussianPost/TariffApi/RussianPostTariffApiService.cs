using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.RussianPost.TariffApi
{
    public class RussianPostTariffApiService
    {
        private const string BaseUrl = "https://tariff.pochta.ru/v2";

        //public List<string> LastActionErrors { get; set; }

        /// <summary>
        /// Рассчет стоимости пересылки
        /// </summary>
        /// <returns>Стоимость пересылки</returns>
        public static CalculateResponse Calculate(bool termOfDelivery, CalculateParams @params)
        {

            return MakeRequest<CalculateResponse>(
                "/calculate/tariff" +
                    (termOfDelivery
                        ? "/delivery"
                        : string.Empty),
                @params.ToDictionary());
        }

        /// <summary>
        /// Рассчет стоимости пересылки
        /// </summary>
        /// <returns>Стоимость пересылки</returns>
        public static async Task<CalculateResponse> CalculateAsync(bool termOfDelivery, CalculateParams @params, CancellationToken cancellationToken)
        {
            return await
                (cancellationToken.CancelIfRequestedAsync<CalculateResponse>()
                 ?? MakeRequestAsync<CalculateResponse>(
                     "/calculate/tariff" +
                     (termOfDelivery
                         ? "/delivery"
                         : string.Empty),
                     @params.ToDictionary(),
                     cancellationToken))
               .ConfigureAwait(false);
        }

        /// <summary>
        /// Получение реквизитов объекта расчета
        /// </summary>
        public static GetDeliveryParamsResponse GetDeliveryParams(long objectId)
        {

            return MakeRequest<GetDeliveryParamsResponse>(
                "/v2/dictionary/object/tariff/delivery",
            new Dictionary<string, string>() {{"id", objectId.ToString()}});
        }

        /// <summary>
        /// Получение реквизитов объекта расчета
        /// </summary>
        public static async Task<GetDeliveryParamsResponse> GetDeliveryParamsAsync(long objectId, CancellationToken cancellationToken)
        {
            return await
                (cancellationToken.CancelIfRequestedAsync<GetDeliveryParamsResponse>()
                 ?? MakeRequestAsync<GetDeliveryParamsResponse>(
                     "/v2/dictionary/object/tariff/delivery",
                     new Dictionary<string, string>() {{"id", objectId.ToString()}},
                     cancellationToken))
               .ConfigureAwait(false);
        }

        #region PrivateMethods

        private static T MakeRequest<T>(string url, Dictionary<string, string> data)
            where T : BaseResponse
        {
            return Task.Run<T>((Func<Task<T>>)(async () => await MakeRequestAsync<T>(url, data, CancellationToken.None).ConfigureAwait(false))).Result;
        }

        private static async Task<T> MakeRequestAsync<T>(string url, Dictionary<string, string> data, CancellationToken cancellationToken)
            where T : BaseResponse
        {
            //LastActionErrors = null;

            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return default;
                
                var request = CreateRequest(url, data, cancellationToken);
                T result = null;

                using(cancellationToken.Register(request.Abort))
                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var serializerSettings = new JsonSerializerSettings
                            {
                                ContractResolver = new DefaultContractResolver()
                                {
                                    NamingStrategy = new KebabCaseNamingStrategy()
                                }
                            };
#if DEBUG
                            var responseContent = "";
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = await (cancellationToken.CancelIfRequestedAsync<string>() ?? reader.ReadToEndAsync()).ConfigureAwait(false);
                            }
                            result = cancellationToken.IsCancellationRequested ? null : JsonConvert.DeserializeObject<T>(responseContent, serializerSettings);
#else
                            using (StreamReader sr = new StreamReader(stream))
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = JsonSerializer.Create(serializerSettings);

                                // read the json from a stream
                                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                result = cancellationToken.IsCancellationRequested ? null : serializer.Deserialize<T>(reader);
                            }
#endif
                        }
                    }
                }

                //if (result != null && result.Errors != null)
                //{
                //    LastActionErrors = new List<string>();
                //    LastActionErrors.AddRange(result.Errors.Select(x => x.Msg));
                //}

                return result;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
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
                                        var error = await (cancellationToken.CancelIfRequestedAsync<string>() ?? reader.ReadToEndAsync()).ConfigureAwait(false);
                                        if (data != null)
                                        {
                                            var serializerSettings = new JsonSerializerSettings
                                            {
                                                ContractResolver = new DefaultContractResolver()
                                                {
                                                    NamingStrategy = new KebabCaseNamingStrategy()
                                                }
                                            };
                                            error += " data:" + JsonConvert.SerializeObject(data, serializerSettings);
                                        }
                                        //LastActionErrors = new List<string>()
                                        //{
                                        //    string.IsNullOrEmpty(error) ? ex.Message : error
                                        //};

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
                                    //LastActionErrors = new List<string>() { ex.Message };
                                    Debug.Log.Warn(ex);
                                }
                            }
                            else
                            {
                                //LastActionErrors = new List<string>() { ex.Message };
                                Debug.Log.Warn(ex);
                            }
                    }
                    else
                    {
                        //LastActionErrors = new List<string>() { ex.Message };
                        Debug.Log.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                //LastActionErrors = new List<string>() { ex.Message };
                Debug.Log.Error(ex);
            }

            return null;
        }

        private static HttpWebRequest CreateRequest(string url, Dictionary<string, string> data, CancellationToken cancellationToken)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (!url.Contains("?"))
                url += "?";

#if DEBUG
            url += "jsontext&";
#else
            url += "json&";
#endif

            url += string.Join("&", data.Keys.Select(key => data[key] == null ? key : key + "=" + HttpUtility.UrlEncode(data[key])));

            var request = WebRequest.CreateHttp(BaseUrl + url);
            // using (cancellationToken.Register(request.Abort))
            // {
                request.Timeout = 10_000;
                request.Method = "GET";
                //request.ContentType = "charset=UTF-8";
            // }

            return request;
        }

#endregion
    }
}
