using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Geocoder.Yandex
{
    public class YandexGeocoderClient
    {
        protected const string BaseUrl = "https://geocode-maps.yandex.ru/1.x/";
    
        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        protected JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        protected JsonSerializerSettings DeserializationSettings { get; private set; }       

        protected readonly string ApiKey;
        protected readonly Uri UrlReferer;

        /// <param name="apiKey">API-ключ</param>
        /// <param name="urlReferer">Адрес сайта/страницы для прохождения ограничений API-ключ по доменам. <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/concepts/limit.html">Подробнее</a></param>
        public YandexGeocoderClient(string apiKey, Uri urlReferer)
        {
            ApiKey = apiKey;
            UrlReferer = urlReferer;
       
            Initialize();
        }

        public GeocodeResult Geocode(GeocodeParams @params, out GeocodeError error)
        {
            var requestResult = MakeRequest<GeocodeResult>("", GetSimpleObjectParams(@params));
            
            error = requestResult?.Error;
            return requestResult?.Result;
        }
        
        #region PrivateMethods
            
        private Dictionary<string, string> GetSimpleObjectParams(object obj)
        {
            if (obj is null)
                return new Dictionary<string, string>();
            
            if (!(SerializationSettings.ContractResolver is DefaultContractResolver contractResolver))
                throw new Exception("contractResolver");

            var getParams = new Dictionary<string, string>();
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(obj);
                if (value != null)
                {
                    getParams.Add(
                        contractResolver.NamingStrategy.GetPropertyName(propertyInfo.Name, false),
                        String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", value));
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
         
        private RequestResult<T> MakeRequest<T>(string urlPart, Dictionary<string, string> data = null, string method = WebRequestMethods.Http.Get)
            where T : class, new()
        {
            return Task.Run<RequestResult<T>>((Func<Task<RequestResult<T>>>) (async () =>
                            await MakeRequestAsync<T>(urlPart, data, method).ConfigureAwait(false)))
                       .Result;
        }
        
        private async Task<RequestResult<T>> MakeRequestAsync<T>(string urlPart, Dictionary<string, string> data = null, string method = WebRequestMethods.Http.Get)
            where T : class, new()
        {
            try
            {
                var request = CreateRequest(urlPart, data, method);
                //request.Timeout = 10000;

                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                    using (var stream = response.GetResponseStream())
                        return new RequestResult<T>(await DeserializeObjectAsync<T>(stream).ConfigureAwait(false));
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

                                    if (statusCode == HttpStatusCode.BadRequest ||
                                        statusCode == HttpStatusCode.Unauthorized ||
                                        statusCode == HttpStatusCode.Forbidden ||
                                        statusCode == HttpStatusCode.NotFound ||
                                        statusCode == HttpStatusCode.InternalServerError)
                                    {
                                        var error = DeserializeObjectAsync<GeocodeError>(eStream).ConfigureAwait(false).GetAwaiter().GetResult();

                                        if (error != null)
                                        {
                                            Debug.Log.Warn(
                                                $"YandexGeocoderClient Url: {eResponse.ResponseUri}, Error: {JsonConvert.SerializeObject(error, SerializationSettings)}, Data: {JsonConvert.SerializeObject(data, SerializationSettings)}");

                                            return new RequestResult<T>(error);
                                        }
                                    }

                                    using (var reader = new StreamReader(eStream))
                                    {
                                        var error = reader.ReadToEnd();

                                        if (string.IsNullOrEmpty(error))
                                            Debug.Log.Warn(ex);
                                        else
                                        {
                                            Debug.Log.Warn($"YandexGeocoderClient Url: {eResponse.ResponseUri}, Error: {error}, Data: {JsonConvert.SerializeObject(data, SerializationSettings)}", ex);
                                        }
                                        
                                        return new RequestResult<T>(new GeocodeError{Message = error});
                                    }
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                    }

                    Debug.Log.Warn(ex);
                    return new RequestResult<T>(new GeocodeError{Message = ex.Message});
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new RequestResult<T>(new GeocodeError{Message = ex.Message});
            }
        }

        private HttpWebRequest CreateRequest(string urlPart, Dictionary<string, string> data, string method)
        {
            data = data ?? new Dictionary<string, string>();
            data.Add("apikey", ApiKey);
            data.Add("format", "json");
            
            var query = string.Join("&", data.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));

            var request = WebRequest.CreateHttp(BaseUrl + urlPart + (urlPart.Contains("?") ? "&" : "?") + query);
            request.Method = method;
            request.Referer = UrlReferer?.ToString();

            return request;
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

        #endregion PrivateMethods
    }
    
    public class RequestResult<T>
        where T : class, new()
    {
        public RequestResult(T result)
        {
            Result = result;
        }
        
        public RequestResult(GeocodeError error)
        {
            Error = error;
        }

        public T Result { get; }
        public GeocodeError Error { get; }
    }
}