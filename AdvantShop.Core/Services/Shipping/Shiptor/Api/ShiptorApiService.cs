using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.Shiptor.Api
{
    public class ShiptorCheckoutApiService
    {
        private const string BaseUrl = "https://checkout.shiptor.ru/api";
        private const int MillisecondsBetweenRequests = 0; // время в миллисекундах между запросами
        private static DateTime _lastRequestApi = DateTime.UtcNow.AddHours(-1);
        private readonly string _apiKey;
        public List<string> LastActionErrors { get; set; }

        public ShiptorCheckoutApiService(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// Метод, добавляющий или обновляющий существующий товар
        /// </summary>
        public SetProductResponse SetProduct(SetProductParams product)
        {
            return MakeRequest<SetProductParams, SetProductResponse>(EnMethod.SetProduct, product);
        }

        /// <summary>
        /// Расчет стоимости доставки
        /// </summary>
        public CalculateShippingResponse CalculateShipping(CalculateShippingParams calculateParams)
        {
            return MakeRequest<CalculateShippingParams, CalculateShippingResponse>(EnMethod.CalculateShipping, calculateParams);
        }

        /// <summary>
        /// Расчет стоимости доставки с настройками кабинета чекаута
        /// </summary>
        public SimpleCalculateResponse SimpleCalculate(SimpleCalculateParams calculateParams)
        {
            return MakeRequest<SimpleCalculateParams, SimpleCalculateResponse>(EnMethod.SimpleCalculate, calculateParams);
        }

        /// <summary>
        /// Получение справочника населенных пунктов
        /// </summary>
        public GetSettlementsResponse GetSettlements(GetSettlementsParams @params)
        {
            return MakeRequest<GetSettlementsParams, GetSettlementsResponse>(EnMethod.GetSettlements, @params);
        }

        /// <summary>
        /// Поиск населенного пункта
        /// </summary>
        public List<SuggestSettlement> SuggestSettlement(string query, string countryIso2 = null)
        {
            return MakeRequest<SimpleSuggestSettlementParams, List<SuggestSettlement>>(EnMethod.SuggestSettlement, new SimpleSuggestSettlementParams { Query = query, CountryIso2 = countryIso2});
        }

        /// <summary>
        /// Поиск населенного пункта
        /// </summary>
        public List<SuggestSettlement> SimpleSuggestSettlement(string query, string countryIso2 = null)
        {
            return MakeRequest<SimpleSuggestSettlementParams, List<SuggestSettlement>>(EnMethod.SimpleSuggestSettlement, new SimpleSuggestSettlementParams { Query = query, CountryIso2 = countryIso2});
        }

        /// <summary>
        /// Получение статуса посылки
        /// </summary>
        public GetPackageResponse GetPackage(GetPackageParams @params)
        {
            return MakeRequest<GetPackageParams, GetPackageResponse>(EnMethod.GetPackage, @params);
        }

        /// <summary>
        /// Добавление посылки
        /// </summary>
        public AddOrderResponse AddOrder(AddOrderParams package)
        {
            return MakeRequest<AddOrderParams, AddOrderResponse>(EnMethod.AddOrder, package);
        }

        /// <summary>
        /// Получение статусов посылок с их описанием
        /// </summary>
        public Dictionary<string, PackageStatus> GetStatusList()
        {
            return CacheManager.Get(string.Format("ShiptorApi-{0}-{1}", _apiKey, EnMethod.GetStatusList), 30, () => MakeRequest<List<object>, Dictionary<string, PackageStatus>>(EnMethod.GetStatusList));
        }

        #region PrivateMethods

        //todo: remove
        //private Stream MakeRequestGetStream<D>(enMethod method, D data = null)
        //    where D : class, new()
        //{
        //    LastActionErrors = null;

        //    try
        //    {
        //        var dataRpc = new ShiptorBaseObject<D>(method) { Params = data };
        //        var request = CreateRequest(dataRpc, "POST");

        //        var response = request.GetResponse();

        //        return response.GetResponseStream();
        //    }
        //    catch (WebException ex)
        //    {
        //        using (var eResponse = ex.Response)
        //        {
        //            if (eResponse != null)
        //            {
        //                using (var eStream = eResponse.GetResponseStream())
        //                    if (eStream != null)
        //                    {
        //                        try
        //                        {
        //                            using (var reader = new StreamReader(eStream))
        //                            {
        //                                var error = reader.ReadToEnd();
        //                                LastActionErrors = new List<string>()
        //                                {
        //                                    string.IsNullOrEmpty(error) ? ex.Message : error
        //                                };

        //                                if (string.IsNullOrEmpty(error))
        //                                    Debug.Log.Error(ex);
        //                                else
        //                                    Debug.Log.Error(error, ex);
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {
        //                            LastActionErrors = new List<string>() { ex.Message };
        //                            Debug.Log.Error(ex);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        LastActionErrors = new List<string>() { ex.Message };
        //                        Debug.Log.Error(ex);
        //                    }
        //            }
        //            else
        //            {
        //                LastActionErrors = new List<string>() { ex.Message };
        //                Debug.Log.Error(ex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LastActionErrors = new List<string>() { ex.Message };
        //        Debug.Log.Error(ex);
        //    }

        //    return null;
        //}

        private TR MakeRequest<TD, TR>(EnMethod method, TD data = null)
            where TD : class, new()
            where TR : class, new()
        {
            return Task.Run<TR>((Func<Task<TR>>)(async() => await MakeRequestAsync<TD, TR>(method, data).ConfigureAwait(false))).Result;
        }

        private async Task<TR> MakeRequestAsync<TD, TR>(EnMethod method, TD data = null)
            where TD : class, new()
            where TR : class, new()
        {
            LastActionErrors = null;

            try
            {
                var dataRpc = new ShiptorBaseObject<TD>(method) { ApiKey = _apiKey };
                if (data != null)
                    dataRpc.Params = data;

                var request = await CreateRequestAsync(dataRpc, "POST").ConfigureAwait(false);

                var responseContent = "";
                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = await reader.ReadToEndAsync().ConfigureAwait(false);
                            }
                    }
                }

                var result = JsonConvert.DeserializeObject<ShiptorBaseResponse<TR>>(responseContent, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });

                if (result != null && result.Error != null)
                {
                    LastActionErrors = new List<string>() { result.Error.Message };
                    Debug.Log.Warn(result.Error.Message, new Exception(responseContent));
                }

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
                            {
                                try
                                {
                                    using (var reader = new StreamReader(eStream))
                                    {
                                        var error = reader.ReadToEnd();
                                        LastActionErrors = new List<string>()
                                        {
                                            string.IsNullOrEmpty(error) ? ex.Message : error
                                        };

                                        if (string.IsNullOrEmpty(error))
                                            Debug.Log.Warn(ex);
                                        else
                                            Debug.Log.Warn(error, ex);
                                    }
                                }
                                catch (Exception)
                                {
                                    LastActionErrors = new List<string>() { ex.Message };
                                    Debug.Log.Warn(ex);
                                }
                            }
                            else
                            {
                                LastActionErrors = new List<string>() { ex.Message };
                                Debug.Log.Warn(ex);
                            }
                    }
                    else
                    {
                        LastActionErrors = new List<string>() { ex.Message };
                        Debug.Log.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LastActionErrors = new List<string>() { ex.Message };
                Debug.Log.Error(ex);
            }

            return null;
        }

        private async Task<HttpWebRequest> CreateRequestAsync(object data, string method)
        {
            TimeSpan time = DateTime.UtcNow - _lastRequestApi;
            if (time.TotalMilliseconds < MillisecondsBetweenRequests)
                await Task.Delay(MillisecondsBetweenRequests - (int)time.TotalMilliseconds);

            var request = WebRequest.Create(BaseUrl) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json;charset=UTF-8";

            if (data != null)
            {
                string dataPost = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });

                byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                request.ContentLength = bytes.Length;

                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                }
            }
            return request;
        }

        #endregion
    }
}
