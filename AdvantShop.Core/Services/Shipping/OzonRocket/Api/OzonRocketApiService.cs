using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.OzonRocket.Api
{
    // https://docs.ozon.ru/api/rocket/
    public class OzonRocketApiService
    {
        private const int MillisecondsBetweenRequests = 0; // время в миллисекундах между запросами
        // private const string BaseUrl = "https://api-stg.ozonru.me";
        private const string BaseUrl = "https://xapi.ozon.ru";

        private static DateTime _lastRequestApi = DateTime.UtcNow.AddHours(-1);
        private static readonly ConcurrentDictionary<string, string> AccessTokens = new ConcurrentDictionary<string, string>();
  
        protected string KeyAccessToken { get; private set; }
        public List<string> LastActionErrors { get; set; }
    
        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }       

        private readonly string _clientId;
        private readonly string _clientSecret;

        public OzonRocketApiService(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;

            KeyAccessToken = $"{_clientId}#{_clientSecret}";

            Initialize();
        }
        
        /// <summary>
        /// Способы доставки
        /// </summary>
        public DeliveryVariantsPart GetDeliveryVariants(GetDeliveryVariantsParams @params)
        {
            string query = null;
            if (@params != null)
            {
                var getParams = GetObjectParams(@params);

                query = getParams.Count > 0 
                    ? "?" + string.Join("&", getParams)
                    : null;
            }

            return MakeRequest<DeliveryVariantsPart>($"/principal-integration-api/v1/delivery/variants{query}", method: WebRequestMethods.Http.Get);
        }
           
        /// <summary>
        /// Способы доставки
        /// </summary>
        public DeliveryVariants GetDeliveryVariantsByIds(GetDeliveryVariantsByIds variantsIds)
        {
            return MakeRequest<DeliveryVariants>(
                "/principal-integration-api/v1/delivery/variants/byids",
                variantsIds,
                method: WebRequestMethods.Http.Post);
        }
           
        /// <summary>
        /// Способы доставки по адресу
        /// </summary>
        public DeliveryVariants GetDeliveryVariantsByAddress(GetDeliveryVariantsByAddressParams @params)
        {
            return MakeRequest<DeliveryVariants>(
                "/principal-integration-api/v1/delivery/variants/byaddress",
                @params,
                method: WebRequestMethods.Http.Post);
        }
     
        /// <summary>
        /// Расчёт стоимости доставки
        /// </summary>
        public DeliveryCalculate GetDeliveryCalculate(GetDeliveryCalculateParams @params)
        {
            string query = null;
            if (@params != null)
            {
                var getParams = GetObjectParams(@params);

                query = getParams.Count > 0 
                    ? "?" + string.Join("&", getParams)
                    : null;
            }

            return MakeRequest<DeliveryCalculate>($"/principal-integration-api/v1/delivery/calculate{query}", method: WebRequestMethods.Http.Get);
        }
               
        /// <summary>
        /// Метод расчета стоимости и срока доставки по адресу
        /// </summary>
        public DeliveryCalculateInformation GetDeliveryCalculateInformation(GetDeliveryCalculateInformationParams @params)
        {
            return MakeRequest<DeliveryCalculateInformation>(
                "/principal-integration-api/v1/delivery/calculate/information",
                @params,
                method: WebRequestMethods.Http.Post);
        }
    
        /// <summary>
        /// Сроки доставки
        /// </summary>
        public DeliveryTime GetDeliveryTime(GetDeliveryTimeParams @params)
        {
            string query = null;
            if (@params != null)
            {
                var getParams = GetObjectParams(@params);

                query = getParams.Count > 0 
                    ? "?" + string.Join("&", getParams)
                    : null;
            }
            return MakeRequest<DeliveryTime>($"/principal-integration-api/v1/delivery/time{query}", method: WebRequestMethods.Http.Get);
        }
        
        /// <summary>
        /// Cписок складов передачи отправлений DropOff
        /// </summary>
        public DeliveryFromPlaces GetDeliveryFromPlaces()
        {
            return MakeRequest<DeliveryFromPlaces>("/principal-integration-api/v1/delivery/from_places", method: WebRequestMethods.Http.Get);
        }
        
        /// <summary>
        /// Получение списка складов пикапа
        /// </summary>
        public DeliveryPickupPlaces GetDeliveryPickupPlaces()
        {
            return MakeRequest<DeliveryPickupPlaces>("/principal-integration-api/v1/delivery/pickup_places", method: WebRequestMethods.Http.Get);
        }

        public CreatedOrder CreateOrder(NewOrder order)
        {
            return MakeRequest<CreatedOrder>("/principal-integration-api/v1/order", order,
                method: WebRequestMethods.Http.Post);
        }

        public UpdatedOrder UpdateOrder(UpdateOrder order)
        {
            return MakeRequest<UpdatedOrder>("/principal-integration-api/v1/order", order,
                method: WebRequestMethods.Http.Put);
        }

        public CreatedDraftOrder CreateDraftOrder(NewDraftOrder order)
        {
            return MakeRequest<CreatedDraftOrder>("/principal-integration-api/v1/draftOrder", order,
                method: WebRequestMethods.Http.Put);
        }

        public ConvertedDraftToOrder ConvertDraftToOrder(string draftLogisticOrderNumber)
        {
            return MakeRequest<ConvertedDraftToOrder>($"/principal-integration-api/v1/order/{draftLogisticOrderNumber}/convert",
                method: WebRequestMethods.Http.Post);
        }

        public CancellationOrdersResult CancelOrders(params long[] ids)
        {
            return MakeRequest<CancellationOrdersResult>("/principal-integration-api/v1/order/status/canceled", 
                new CancellationOrders() {Ids = ids.ToList()},
                method: WebRequestMethods.Http.Put);
        }

        public TrackingByPostingNumber TrackingByPostingNumber(TrackingByPostingNumberParams @params)
        {
            if (@params == null)
                throw new ArgumentNullException(nameof(@params));
            
            string query = null;
            var getParams = GetObjectParams(@params);

            query = getParams.Count > 0 
                ? "?" + string.Join("&", getParams)
                : null;

            return MakeRequest<TrackingByPostingNumber>($"/principal-integration-api/v1/tracking/bypostingnumber{query}", method: WebRequestMethods.Http.Get);
        }

        public TrackingByPostingNumbersOrIds TrackingByPostingNumbersOrIds(TrackingByPostingNumbersOrIdsParams @params)
        {
            return MakeRequest<TrackingByPostingNumbersOrIds>("/principal-integration-api/v1/tracking/list", @params,
                method: WebRequestMethods.Http.Post);
        }
        
        #region PrivateMethods
     
        private List<string> GetObjectParams(object obj, string namePrefix = null)
        {
            if (obj == null)
                return new List<string>();
            
            if (!(SerializationSettings.ContractResolver is DefaultContractResolver contractResolver))
                throw new Exception("contractResolver");

            var getParams = new List<string>();
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var propertyName = namePrefix + contractResolver.NamingStrategy.GetPropertyName(propertyInfo.Name, false);
                if (!propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType == typeof(string))
                {
                    var value = propertyInfo.GetValue(obj);
                    if (value != null)
                    {
                        if (value is bool?) value = ((bool?) value).Value ? "1" : "0";
                        if (value is IEnumerable && !(value is string))
                            value = string.Join(",", ((IEnumerable) value).Cast<object>());


                        getParams.Add(
                            $"{propertyName}={HttpUtility.UrlEncode(String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", value))}");
                    }
                }
                else
                {
                    var propertyParams = GetObjectParams(
                        obj: propertyInfo.GetValue(obj),
                        namePrefix: propertyName + ".");
                    if (propertyParams != null)
                        getParams.AddRange(propertyParams);
                }
            }

            return getParams;
        }

        private void Initialize()
        {
            SerializationSettings = new JsonSerializerSettings
            {
#if DEBUG
                Formatting = Formatting.Indented,
#endif
#if !DEBUG
                Formatting = Formatting.None,
#endif
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()

            };
            DeserializationSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()

            };
        }   
         
        private T MakeRequest<T>(string urlPart, object data = null, string method = WebRequestMethods.Http.Post,
            string contentType = "application/json;charset=UTF-8")
            where T : class, new()
        {
            return Task.Run<T>((Func<Task<T>>) (async () =>
                    await MakeRequestAsync<T>(urlPart, data, method, contentType).ConfigureAwait(false)))
                .Result;
        }
        
        private T MakeRequestWithOutToken<T>(string urlPart, object data = null, string method = WebRequestMethods.Http.Post,
            string contentType = "application/json;charset=UTF-8")
            where T : class, new()
        {
            return Task.Run<T>((Func<Task<T>>) (async () =>
                    await MakeRequestAsync<T>(urlPart, data, method, contentType, withOutToken: true)
                        .ConfigureAwait(false)))
                .Result;
        }

        private async Task<T> MakeRequestAsync<T>(string urlPart, object data = null, string method = WebRequestMethods.Http.Post, 
            string contentType = "application/json;charset=UTF-8", bool withOutToken = false, bool noRecallAtUnauthorized = false)
            where T : class, new()
        {
            ClearErrors();

            try
            {
                var request = await CreateRequestAsync(urlPart, data, method, contentType, withOutToken).ConfigureAwait(false);
                //request.Timeout = 10000;

                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                    using (var stream = response.GetResponseStream())
                        if (stream != null)
                            return await DeserializeObjectAsync<T>(stream).ConfigureAwait(false);
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
                                    HttpStatusCode statusCode = eResponse.StatusCode;
                                    
                                    if (!noRecallAtUnauthorized && !withOutToken && statusCode == HttpStatusCode.Unauthorized)
                                    {
                                        if (AccessTokens.ContainsKey(KeyAccessToken))
                                        {
                                            AccessTokens.TryRemove(KeyAccessToken, out _);
                                            return await MakeRequestAsync<T>(urlPart, data, method, contentType, withOutToken, noRecallAtUnauthorized: true).ConfigureAwait(false);
                                        }
                                    }

                                    if (statusCode == HttpStatusCode.BadRequest ||
                                        statusCode == HttpStatusCode.Unauthorized ||
                                        statusCode == HttpStatusCode.Forbidden ||
                                        statusCode == HttpStatusCode.NotFound ||
                                        statusCode == HttpStatusCode.InternalServerError)
                                    {
                                        var error = DeserializeObjectAsync<Error>(eStream).ConfigureAwait(false).GetAwaiter().GetResult();

                                        if (error != null)
                                        {
                                            AddErrors(error.Message + 
                                                      string.Join("; ", 
                                                          error.Arguments
                                                              .Select(x => $"{x.Key}: {string.Join(", ", x.Value)}")));
                                            Debug.Log.Warn(
                                                $"OzonRocket Url: {urlPart}, Error: {JsonConvert.SerializeObject(error, SerializationSettings)}, Data: {JsonConvert.SerializeObject(data, SerializationSettings)}");

                                            return null;
                                        }
                                    }

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

        private Stream MakeRequestGetStream(string url, object data = null, string method = WebRequestMethods.Http.Post, 
            string contentType = null, bool withOutToken = false, bool noRecallAtUnauthorized = false)
        {
            ClearErrors();

            try
            {
                var request = CreateRequestAsync(url, data, method, contentType, withOutToken).ConfigureAwait(false).GetAwaiter().GetResult();

                var response = request.GetResponse();

                return response.GetResponseStream();
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
                                    HttpStatusCode statusCode = eResponse.StatusCode;
                                    
                                    if (!noRecallAtUnauthorized && !withOutToken && statusCode == HttpStatusCode.Unauthorized)
                                    {
                                        if (AccessTokens.ContainsKey(KeyAccessToken))
                                        {
                                            AccessTokens.TryRemove(KeyAccessToken, out _);
                                            return MakeRequestGetStream(url, data, method, contentType, withOutToken, noRecallAtUnauthorized: true);
                                        }
                                    }

                                    if (statusCode == HttpStatusCode.BadRequest ||
                                        statusCode == HttpStatusCode.Unauthorized ||
                                        statusCode == HttpStatusCode.Forbidden ||
                                        statusCode == HttpStatusCode.NotFound ||
                                        statusCode == HttpStatusCode.InternalServerError)
                                    {
                                        var error = DeserializeObjectAsync<Error>(eStream).ConfigureAwait(false).GetAwaiter().GetResult();

                                        if (error != null)
                                        {
                                            Debug.Log.Warn(
                                                $"OzonRocket ErrorCode: {error.ErrorCode}, ErrorMessage: {error.Message}, Data: {JsonConvert.SerializeObject(data, SerializationSettings)}");

                                            return null;
                                        }
                                    }

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
        private async Task<HttpWebRequest> CreateRequestAsync(string urlPart, object data, string method, 
            string contentType, bool withOutToken)
        {
            string token = null;
            if (!withOutToken)
            {
                token = await GetAccessTokenAsync().ConfigureAwait(false);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("OzonRocket AdvException: Не удалось получить токен авторизации.");
            }

            var isFullUrl = urlPart.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                            urlPart.StartsWith("https:", StringComparison.OrdinalIgnoreCase);
            var request = WebRequest.Create(isFullUrl ? urlPart : BaseUrl + urlPart) as HttpWebRequest;
            request.Method = method;
            request.ContentType = contentType;
            if (!withOutToken)
                request.Headers.Add(HttpRequestHeader.Authorization,
                    $"Bearer {token}");

            if (data != null)
                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    if (contentType.Contains("x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                        await WriteUrlEncodedData(data.ToString(), requestStream).ConfigureAwait(false);
                    else
                        await SerializeObjectAsync(data, requestStream).ConfigureAwait(false);
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

        private Task WriteUrlEncodedData(string data, Stream stream)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            //request.ContentLength = bytes.Length;

            return stream.WriteAsync(bytes, 0, bytes.Length);
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

        private async Task<OAuthData> OAuthAsync()
        {
            var grantType = "client_credentials";
            var clientId = _clientId;
            var clientSecret = _clientSecret;
            return
                await MakeRequestAsync<OAuthData>("/principal-auth-api/connect/token",
                    data: $"grant_type={HttpUtility.UrlEncode(grantType)}&client_id={HttpUtility.UrlEncode(clientId)}&client_secret={HttpUtility.UrlEncode(clientSecret)}",
                    contentType:"application/x-www-form-urlencoded;charset=UTF-8",
                    withOutToken: true,
                    noRecallAtUnauthorized: true).ConfigureAwait(false);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!AccessTokens.ContainsKey(KeyAccessToken))
            {
                var oAuthData = await OAuthAsync().ConfigureAwait(false);
                if (oAuthData == null || string.IsNullOrEmpty(oAuthData.AccessToken))
                    return null;

                AccessTokens.TryAdd(KeyAccessToken, oAuthData.AccessToken);
            }

            return AccessTokens[KeyAccessToken];
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
     
        #endregion
    }
    
}