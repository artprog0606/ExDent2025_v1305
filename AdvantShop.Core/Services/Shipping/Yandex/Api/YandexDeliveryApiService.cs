using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdvantShop.Shipping.Yandex.Api
{
    public class YandexDeliveryApiService
    {
        //private const string BaseUrlB2B = "https://b2b.taxi.tst.yandex.net/api/b2b/platform/"; //test
        private const string BaseUrlB2B = "https://b2b-authproxy.taxi.yandex.net/api/b2b/platform/";
        private readonly string _apiToken;
        private readonly string _stationId;

        public JsonSerializerSettings SerializationSettings { get; private set; }
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        public List<string> LastActionErrors { get; set; }

        public YandexDeliveryApiService(string stationId, string apiToken)
        {
            _stationId = stationId;
            _apiToken = apiToken;

            Initialize();
        }

        public GetGeoIdResponse GetCityGeoId(string city)
        {
            return MakeRequest<GetGeoIdResponse>(
                "location/detect",
                new { location = city });
        }

        public PickPointResponse GetPickPoints(PickPointParams @params = null)
        {
            return MakeRequest<PickPointResponse>(
                "pickup-points/list",
                @params);
        }

        public CalculateResponse Calculate(CalculateParams @params)
        {
            @params.Source = new Location { PlatformStationId = _stationId };
            return MakeRequest<CalculateResponse>(
                "pricing-calculator",
                @params);
        }

        public List<IntervalOffer> GetDeliveryInterval(DeliveryIntervalParams @params)
        {
            var url = string.Format("offers/info?station_id={0}{1}{2}{3}",
                _stationId,
                !@params.FullAddress.IsNullOrEmpty()
                    ? "&full_address=" + @params.FullAddress
                    : string.Empty,
                @params.GeoId.HasValue
                    ? "&geo_id=" + @params.GeoId
                    : string.Empty,
                !@params.SelfPickupId.IsNullOrEmpty()
                    ? "&self_pickup_id=" + @params.SelfPickupId
                    : string.Empty);
            var interval = MakeRequest<DeliveryInterval>(url, method: WebRequestMethods.Http.Get);
            return interval?.Offers;
        }

        public CreateOrderResponse CreateOrder(CreateOrderParams @params)
        {
            @params.Source = new Source 
            { 
                PlatformStation = new PlatformStation { PlatformId = _stationId } 
            };

            return MakeRequest<CreateOrderResponse>(
                "offers/create",
                @params);
        }

        public string ConfirmOrder(string offerId)
        {
            var result = MakeRequest<ConfirmOrderResponse>(
                "offers/confirm",
                new { offer_id = offerId });
            return result?.RequestId;
        }

        public CancelOrderResponse CancelOrder(string requestId)
        {
            return MakeRequest<CancelOrderResponse>(
                "request/cancel",
                new { request_id = requestId });
        }

        public StatusesResponse GetHistoryOfStatuses(string requestId)
        {
            return MakeRequest<StatusesResponse>(
                $"request/history?request_id={requestId}", method: WebRequestMethods.Http.Get);
        }

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
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
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

        private T MakeRequest<T>(string url, object data = null, string method = WebRequestMethods.Http.Post)
            where T : class, new()
        {
            return Task.Run<T>((Func<Task<T>>)(async () => await MakeRequestAsync<T>(url, data, method).ConfigureAwait(false))).Result;
        }

        private async Task<T> MakeRequestAsync<T>(string url, object data = null, string method = WebRequestMethods.Http.Post)
            where T : class, new()
        {
            ClearErrors();

            try
            {
                var request = await CreateRequestAsync(url, data, method).ConfigureAwait(false);

                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var result = await DeserializeObjectAsync<T>(stream).ConfigureAwait(false);

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
                                        HttpStatusCode statusCode = eResponse.StatusCode;
                                        if (statusCode == HttpStatusCode.BadRequest ||
                                            statusCode == HttpStatusCode.Unauthorized ||
                                            statusCode == HttpStatusCode.Forbidden ||
                                            statusCode == HttpStatusCode.NotFound ||
                                            statusCode == HttpStatusCode.InternalServerError)
                                        {
                                            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(error);

                                            if (errorResponse != null)
                                            {
                                                if (errorResponse.Message.IsNotEmpty())
                                                {
                                                    AddErrors(errorResponse.Message);
                                                    Debug.Log.Warn(
                                                        $"YandexDelivery Url: {url}, Error: {JsonConvert.SerializeObject(errorResponse, SerializationSettings)}, Data: {JsonConvert.SerializeObject(data, SerializationSettings)}");
                                                    return null;
                                                }
                                            };
                                        }
                                        if (data != null)
                                            error += " data:" + JsonConvert.SerializeObject(data, SerializationSettings);
                                        
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
            var request = WebRequest.Create(BaseUrlB2B + url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json";
            request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Bearer {0}", _apiToken));

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
    }
}
