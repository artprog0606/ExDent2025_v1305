using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipShippingService
    {
        public ApiShipShippingService(string apiKey)
        {
            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            isTest = apiKey == "test";
            apiToken = isTest ? GetToken("test", "test") :apiKey;

        }

        private readonly string apiToken;
        private bool isTest;
        private const string urlApi = "https://api.apiship.ru/v1/";
        private const string urlTestApi = "http://api.dev.apiship.ru/v1/";
        private JsonSerializerSettings serializerSettings;

        private Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>
                {
                    { "Authorization", apiToken },
                    { "Platform", "advantshop" }
                };
        }

        private class AddParam
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public ApiShipCalculatorResponseModel GetCalculator(ApiShipCalculatorRequestModel model)
        {
            var calculatorResult = PostApiResponse<ApiShipCalculatorResponseModel, ApiShipCalculatorRequestModel>(model, "calculator");
            return calculatorResult;
        }

        public List<ApiShipPoint> GetApiShipPoints(string city, List<string> providerKeys = null)
        {
            return CacheManager.Get("ApiShip_PreparePoint_" + city + providerKeys != null && providerKeys.Count > 0 ? string.Join(",", providerKeys) : "", 60 * 24, () =>
             {
                 List<ApiShipPoint> apiShipPoints = new List<ApiShipPoint>();
                 ApiShipPointsModel pointModel = new ApiShipPointsModel();
                 int i = 0;
                 int countPointOnOneRequest = 1000;
                 do
                 {
                     pointModel = GetApiResponse<ApiShipPointsModel>("lists/points?filter=city=" + HttpUtility.UrlEncode(city) +
                         ";providerKey=" + (providerKeys != null && providerKeys.Count == 1 ? providerKeys[0] : "[" + String.Join(",", providerKeys.ToArray()) + "]") + $@"&limit={countPointOnOneRequest}" + (i > 0 ? $@"&offset={i * countPointOnOneRequest}" : string.Empty));
                     i++;
                     if (pointModel != null && pointModel.Rows != null && pointModel.Rows.Count > 0)
                     {
                         apiShipPoints.AddRange(pointModel.Rows);
                     }
                 }
                 while (pointModel != null && !(pointModel != null && pointModel.Meta.Total == apiShipPoints.Count));
                 return apiShipPoints;
             });
        }

        public List<ApiShipTariff> GetApiShipTariffs()
        {
            return CacheManager.Get("ApiShip_List_Tariffs", 60 * 24, () =>
            {
                List<ApiShipTariff> apiShipTariff = new List<ApiShipTariff>();
                ApiShipTariffsResponseModel tariffResponse = new ApiShipTariffsResponseModel();
                int i = 0;
                int countItemOnOneRequest = 1000;
                do
                {
                    tariffResponse = GetApiResponse<ApiShipTariffsResponseModel>("lists/tariffs?filter=" + $@"&limit={countItemOnOneRequest}" + (i > 0 ? $@"&offset={i * countItemOnOneRequest}" : string.Empty));
                    i++;
                    if (tariffResponse != null && tariffResponse.Rows != null && tariffResponse.Rows.Count > 0)
                    {
                        apiShipTariff.AddRange(tariffResponse.Rows);
                    }
                }
                while (tariffResponse != null && !(tariffResponse != null && tariffResponse.Meta.Total == apiShipTariff.Count));
                return apiShipTariff;
            });
        }

        public List<ApiShipProvider> GetApiShipProviders()
        {
            var tariffModel = GetApiResponse<ApiShipPovidersModel>("lists/providers");
            return tariffModel?.Rows ?? new List<ApiShipProvider>();
        }

        public ApiShipAddOrderResponseModel ApiShipSendOrder(ApiShipAddOrderRequestModel order)
        {
            var result = PostApiResponse<ApiShipAddOrderResponseModel, ApiShipAddOrderRequestModel>(order, "orders");
            return result;
        }

        public string GetToken(string login, string password)
        {
            string token = "";
            var model = new ApiShipTokenRequestModel();
            model.Login = login;
            model.Password = password;
            try
            {
                string url = (isTest ? urlTestApi : urlApi) + "users/login";
                var request = CreateRequestAsync(url, model, WebRequestMethods.Http.Post, "application/json", null).ConfigureAwait(false).GetAwaiter().GetResult();
                using (var response = request.GetResponseAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                {
                    var result = DeserializeObjectAsync<ApiShipTokenResponseModel>(response.GetResponseStream())
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                    token = result?.Token;
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = (HttpWebResponse)ex.Response)
                {
                    if (eResponse != null)
                    {
                        LogRequestError<ApiShipErrorModel>(eResponse);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return token;
        }

        public ApiShipOrderStatusModel GetApiShipOrderStatus(string orderId)
        {
            string orderNumber = orderId;
            var orderStatus = GetApiResponse<ApiShipOrderStatusModel>("orders/status?clientNumber=" + orderNumber);
            return orderStatus;
        }

        private T GetApiResponse<T>(string method) where T : class
        {
            try
            {
                string url = (isTest ? urlTestApi : urlApi) + method;
                var request = CreateRequestAsync(url, null, WebRequestMethods.Http.Get, "application/json", "application/json", GetHeaders()).ConfigureAwait(false).GetAwaiter().GetResult();
                using (var response = request.GetResponse())
                {
                    return DeserializeObjectAsync<T>(response.GetResponseStream())
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = (HttpWebResponse)ex.Response)
                {
                    if (eResponse != null)
                    {
                        LogRequestError<ApiShipErrorModel>(eResponse);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return default(T);
        }

        private T PostApiResponse<T, M>(M model, string method) where T : ApiShipErrorModel
        {
            try
            {
                string url = (isTest ? urlTestApi : urlApi) + method;

                var request = CreateRequestAsync(url, model, WebRequestMethods.Http.Post, "application/json", "application/json", GetHeaders()).ConfigureAwait(false).GetAwaiter().GetResult();
                using (var response = request.GetResponse())
                {
                    return DeserializeObjectAsync<T>(response.GetResponseStream())
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = (HttpWebResponse)ex.Response)
                {
                    if (eResponse != null)
                    {
                        return LogRequestError<T>(eResponse);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return default(T);
        }

        public ApiShipDeleteOrderModel ApiShipDeleteOrder(int apiShipOrderId, string method)
        {
            try
            {
                string url = (isTest ? urlTestApi : urlApi) + method + "/" + apiShipOrderId;

                var request = CreateRequestAsync(url, null, "DELETE", "application/json", "application/json", GetHeaders()).ConfigureAwait(false).GetAwaiter().GetResult();
                using (var response = request.GetResponseAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                {
                    return DeserializeObjectAsync<ApiShipDeleteOrderModel>(response.GetResponseStream())
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = (HttpWebResponse)ex.Response)
                {
                    if (eResponse != null)
                    {
                        LogRequestError<ApiShipErrorModel>(eResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return default(ApiShipDeleteOrderModel);
        }

        private T LogRequestError<T>(HttpWebResponse response) where T : ApiShipErrorModel
        {
            string tracingId = response.Headers.Get("X-Tracing-Id");
            ApiShipErrorModel errorResult = new ApiShipErrorModel();
            errorResult = DeserializeObjectAsync<T>(response.GetResponseStream())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            Debug.Log.Error(String.Format("Ошибка: {0}. Описание: {1}. Поле ошибки: {2}. Расшифровка ошибки: {3}. x-tracing-id: {4}",
                errorResult.Message, errorResult.Description, 
                (errorResult.Errors != null && errorResult.Errors.Count > 0 ? errorResult.Errors[0].Field : ""), 
                (errorResult.Errors != null && errorResult.Errors.Count > 0 ? errorResult.Errors[0].Message : ""), 
                tracingId));
            return (T)errorResult;
        }

        private async Task<HttpWebRequest> CreateRequestAsync(string url, object data, string method, string contentType, string accept, Dictionary<string,string> headers = null)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = contentType;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            if (!string.IsNullOrEmpty(accept))
            {
                request.Accept = accept;
            }

            if (data != null)
                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    await SerializeObjectAsync(data, requestStream).ConfigureAwait(false);
            return request;
        }

        private Task SerializeObjectAsync(object data, Stream stream)
        {
#if DEBUG
            string dataPost = JsonConvert.SerializeObject(data, serializerSettings);

            byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
            //request.ContentLength = bytes.Length;

            return stream.WriteAsync(bytes, 0, bytes.Length);
#endif
#if !DEBUG
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer serializer = JsonSerializer.Create(serializerSettings);
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
                return JsonConvert.DeserializeObject<T>(responseContent, serializerSettings);
#endif
#if !DEBUG
                using (JsonReader jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = JsonSerializer.Create(serializerSettings);

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    return serializer.Deserialize<T>(jsonReader);
                }
#endif
            }
        }
    }
}
