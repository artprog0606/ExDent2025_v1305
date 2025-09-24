using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdvantShop.Shipping.FivePost.Api
{
    public class FivePostApiService
    {
        private readonly string _apiKey;

        private static readonly ConcurrentDictionary<string, FivePostToken> Tokens = new ConcurrentDictionary<string, FivePostToken>();
        private FivePostToken Token => _apiKey.IsNotEmpty() && Tokens.ContainsKey(_apiKey) ? Tokens[_apiKey] : null;

        private const string _apiUrl = "https://api-omni.x5.ru/";

        public JsonSerializerSettings SerializationSettings { get; private set; }
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        public List<string> LastActionErrors { get; set; }

        private readonly FivePostGetTokenParams GetTokenParams = new FivePostGetTokenParams
        {
            Subject = "OpenAPI",
            Audience = "A122019!"
        };

        public FivePostApiService(string apiKey)
        {
            Initialize();

            _apiKey = apiKey;
            if (_apiKey.IsNotEmpty())
                InitializeToken();
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

        public List<FivePostPickPoint> GetPickPoints(FivePostPickPointParams @params)
        {
            List<FivePostPickPoint> result = null;
            var minTimeBetweenRequests = 1000 / (3000 / @params.PageSize);
            while (true)
            {
                var paginationCollection =
                    MakeRequest<FivePostPickPointList>($"{_apiUrl}api/v1/pickuppoints/query", @params, contentType: "application/json");

                if (paginationCollection == null)
                    break;

                if (result is null)
                    result = new List<FivePostPickPoint>(paginationCollection.TotalElements);

                result.AddRange(paginationCollection?.PickPoints);

                if (@params.PageNumber++ >= paginationCollection.TotalPages)
                    break;
                Task.Delay(minTimeBetweenRequests).Wait();//максимум можно запросить 3000 элементов в секунду
            }

            return result;
        }

        public FivePostPickPoint GetPickPoint(FivePostPickPointParams @params)
        {
            var list =
                MakeRequest<FivePostPickPointList>($"{_apiUrl}api/v1/pickuppoints/query", @params, contentType: "application/json");

            if ((list?.PickPoints?.Count ?? 0) == 0)
                return null;

            return list?.PickPoints?[0];
        }

        public List<FivePostRate> GetRateList()
        {
            var @params = new FivePostPickPointParams(1, 1);
            var pickPoint = GetPickPoint(@params);

            return pickPoint?.RateList;
        }

        public List<FivePostPossibleDelivery> GetDeliveryTypeList()
        {
            var @params = new FivePostPickPointParams(1, 1);
            var pickPoint = GetPickPoint(@params);

            return pickPoint?.PossibleDeliveryList;
        }

        public FivePostWarehouse GetWarehouse(string warehouseId)
        {
            return MakeRequest<FivePostWarehouse>($"{_apiUrl}api/v1/warehouse/{warehouseId}", method: WebRequestMethods.Http.Get);
        }

        public List<FivePostWarehouse> GetWarehouses(FivePostGetWarehouseParams @params)
        {
            var warehouseCollection = new List<FivePostWarehouse>();
            var pageNumber = 0;

            while (true)
            {
                var urlParams = new Dictionary<string, string> { { "page", pageNumber.ToString() } };
                var paginationCollection = MakeRequest<FivePostWarehouseList>($"{_apiUrl}api/v1/getWarehouseAll", @params, WebRequestMethods.Http.Get,
                    urlParams: urlParams);

                if (paginationCollection == null)
                    break;

                warehouseCollection.AddRange(paginationCollection.Warehouses);
                pageNumber++;

                if (paginationCollection.TotalPages <= pageNumber)
                    break;
            }

            return warehouseCollection;
        }

        public FivePostCreateOrderResult CreateOrder(FivePostCreateOrderParams @params)
        {
            return MakeRequest<List<FivePostCreateOrderResult>>
                ($"{_apiUrl}api/v3/orders", @params, contentType: "application/json")?[0];
        }

        public FivePostDeleteOrderResult DeleteOrder(string orderId)
        {
            return MakeRequest<FivePostDeleteOrderResult>($"{_apiUrl}api/v2/cancelOrder/bySenderOrderId/{orderId}", method: "DELETE");
        }

        public List<FivePostStatusResult> SyncStatuses(FivePostSyncStatusParams @params)
        {
            var result = new List<FivePostStatusResult>();
            int pageNumber = 0;
            int maxPageSize = 100;
            int totalPages = Convert.ToInt32(Math.Ceiling(@params.OrderIds.Count / (float)maxPageSize));

            while (true)
            {
                var currentParams = new FivePostSyncStatusParams
                {
                    OrderIds = @params.OrderIds
                    .Skip(pageNumber * maxPageSize)
                    .Take(maxPageSize)
                    .ToList()
                };
                var paginationResult = MakeRequest<List<FivePostStatusResult>>($"{_apiUrl}api/v1/getOrderStatus", currentParams, contentType: "application/json");

                if (paginationResult == null)
                    break;

                result.AddRange(paginationResult);
                pageNumber++;

                if (totalPages <= pageNumber)
                    break;
            }

            return result;
        }

        public bool IsValidToken => Token != null && !Token.NeedUpdate;

        private void InitializeToken()
        {
            if (Token == null || Token.NeedUpdate)
            {
                var urlParams = new Dictionary<string, string> { { "apikey", _apiKey } };
                var token = MakeRequest<FivePostToken>($"{_apiUrl}jwt-generate-claims/rs256/1", GetTokenParams,
                    contentType: "application/x-www-form-urlencoded", urlParams: urlParams, withoitToken: true);

                if (token != null)
                    AddToken(token, _apiKey);
            }
        }

        private async Task CheckTokenAsync(bool forced = false)
        {
            if (Token == null || Token.NeedUpdate || forced)
            {
                var urlParams = new Dictionary<string, string> { { "apikey", _apiKey } };
                var token = await MakeRequestAsync<FivePostToken>($"{_apiUrl}jwt-generate-claims/rs256/1", GetTokenParams,
                    contentType: "application/x-www-form-urlencoded", urlParams: urlParams, withoitToken: true).ConfigureAwait(false);

                if (token != null)
                    AddToken(token, _apiKey);
            }
        }

        private T MakeRequest<T>(string urlPart, object data = null, string method = WebRequestMethods.Http.Post,
            string contentType = "application/json; charset=utf-8", bool withoitToken = false, Dictionary<string, string> urlParams = null)
            where T : class, new()
        {
            return Task.Run(async () =>
                    await MakeRequestAsync<T>(urlPart, data, method, contentType, urlParams: urlParams, withoitToken: withoitToken).ConfigureAwait(false))
                .Result;
        }

        private async Task<T> MakeRequestAsync<T>(string urlPart, object data = null, string method = WebRequestMethods.Http.Post,
            string contentType = "application/json; charset=utf-8", bool withoitToken = false, bool noRecallAtUnauthorized = false,
            Dictionary<string, string> urlParams = null)
            where T : class, new()
        {
            ClearErrors();
            try
            {
                var request = await CreateRequestAsync(urlPart, data, method, contentType, withoitToken, urlParams: urlParams).ConfigureAwait(false);
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

                                    if (!noRecallAtUnauthorized && !withoitToken && statusCode == HttpStatusCode.Unauthorized)
                                    {
                                        await CheckTokenAsync(forced: true).ConfigureAwait(false);
                                        return await MakeRequestAsync<T>(urlPart, data, method, contentType, withoitToken, noRecallAtUnauthorized: true).ConfigureAwait(false);
                                    }

                                    if (statusCode == HttpStatusCode.BadRequest ||
                                        statusCode == HttpStatusCode.Unauthorized ||
                                        statusCode == HttpStatusCode.Forbidden ||
                                        statusCode == HttpStatusCode.NotFound ||
                                        statusCode == HttpStatusCode.InternalServerError)
                                    {
                                        var errorModel = DeserializeObjectAsync<T>(eStream).ConfigureAwait(false).GetAwaiter().GetResult();

                                        if (errorModel != null && errorModel is FivePostError error)
                                        {
                                            AddErrors(error.FullError);
                                            Debug.Log.Warn(
                                                $"FivePost Url: {urlPart}, Error: {JsonConvert.SerializeObject(error, SerializationSettings)}, Data: {JsonConvert.SerializeObject(data, SerializationSettings)}");

                                            return null;
                                        }
                                        if (errorModel != null && errorModel is FivePostErrorList errorList)
                                        {
                                            foreach (var item in errorList.Errors)
                                            {
                                                AddErrors(item.FullError);
                                                Debug.Log.Warn(
                                                    $"FivePost Url: {urlPart}, Error: {JsonConvert.SerializeObject(item, SerializationSettings)}, Data: {JsonConvert.SerializeObject(data, SerializationSettings)}");
                                            }

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
            string contentType, bool withoitToken, Dictionary<string, string> urlParams = null)
        {
            if (!withoitToken)
            {
                await CheckTokenAsync().ConfigureAwait(false);
                if (Token == null || Token.NeedUpdate)
                    throw new BlException("FivePost: Не удалось получить токен авторизации.");
            }

            if (urlParams != null && urlParams.Count > 0)
            {
                var queryParams = urlParams.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value))).AggregateString("&");
                urlPart += (urlPart.Contains("?") ? "&" : "?") + queryParams;
            }

            var request = WebRequest.Create(urlPart) as HttpWebRequest;
            request.Method = method;
            request.ContentType = contentType;
            if (!withoitToken)
                request.Headers.Add(HttpRequestHeader.Authorization,
                    $"Bearer {Token.JwtToken}");

            if (data != null)
                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    if (contentType == "application/x-www-form-urlencoded")
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

        private Task WriteUrlEncodedData(string data, Stream stream)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            //request.ContentLength = bytes.Length;

            return stream.WriteAsync(bytes, 0, bytes.Length);
        }

        private void ClearErrors()
        {
            LastActionErrors = new List<string>();
        }

        private void AddErrors(params string[] message)
        {
            LastActionErrors.AddRange(message);
        }

        private void AddToken(FivePostToken token, string apiKey)
        {
            if (Tokens.ContainsKey(apiKey))
                Tokens.TryRemove(apiKey, out _);

            Tokens.TryAdd(apiKey, token);
        }
    }
}
