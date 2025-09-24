using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipShippingService
    {
        public ApiShipShippingService(string apiKey)
        {
            isTest = apiKey.IsNullOrEmpty();
            apiToken = !apiKey.IsNullOrEmpty() ? apiKey : GetToken("test", "test");
        }

        private readonly string apiToken;
        private bool isTest;
        private const string urlApi = "https://api.apiship.ru/v1/";
        private const string urlTestApi = "http://api.dev.apiship.ru/v1/";

        private class AddParam
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public ApiShipCalculatorResponseModel GetCalculator(ApiShipCalculatorRequestModel model, string method)
        {
            var calculatorResult = PostApiResponse<ApiShipCalculatorResponseModel, ApiShipCalculatorRequestModel>(model, method);

            return calculatorResult;
        }

        public List<ApiShipPoint> GetApiShipPoints(string city, string region, List<string> providerKeys)
        {
            var pointModel = GetApiResponse<ApiShipPointsModel>("lists/points?filter=city=" + HttpUtility.UrlEncode(city) + 
                ";providerKey=" + (providerKeys.Count == 1 ? providerKeys[0] : "[" + String.Join(",", providerKeys.ToArray()) + "]") + "&limit=1000");
            return pointModel?.rows ?? new List<ApiShipPoint>();
        }

        public List<ApiShipTariff> GetApiShipTariffs()
        {
            var tariffModel = GetApiResponse<ApiShipTariffsResponseModel>("lists/tariffs");
            return tariffModel?.rows ?? new List<ApiShipTariff>();
        }

        public List<ApiShipProvider> GetApiShipProviders()
        {
            var tariffModel = GetApiResponse<ApiShipPovidersModel>("lists/providers");
            return tariffModel?.rows ?? new List<ApiShipProvider>();
        }

        public ApiShipAddOrderResponseModel ApiShipSendOrder(ApiShipAddOrderRequestModel order)
        {
            var param = new AddParam { Name = "Platform", Value = "advantshop" };
            var paramHeaders = new List<AddParam>();
            paramHeaders.Add(param);

            var result = PostApiResponse<ApiShipAddOrderResponseModel, ApiShipAddOrderRequestModel>(order, "orders", paramHeaders);
            return result;
        }

        public string GetToken(string login, string password)
        {
            string token = "";
            var model = new ApiShipTokenRequestModel();
            model.login = login;
            model.password = password;

            try
            {
                string jsonData = JsonConvert.SerializeObject(model);
                string url = urlTestApi + "users/login";
                string responseString = "";
                using (var client = new HttpClient())
                {
                    //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var result = JsonConvert.DeserializeObject<ApiShipTokenResponseModel>(responseString);
                        token = result?.token;
                    }
                    else if (((int)response.StatusCode) >= 400 && ((int)response.StatusCode) < 500)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var errorResult = JsonConvert.DeserializeObject<ApiShipErrorModel>(responseString);
                        Debug.Log.Error(String.Format("Ошибка: {0}. Описание: {1}. Поле ошибки: {2}. Расшифровка ошибки: {3}", errorResult.message, errorResult.description, errorResult.errors.FirstOrDefault()?.field, errorResult.errors.FirstOrDefault()?.message));
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
            string orderNumber = isTest ? "adv" + orderId : orderId;
            var orderStatus = GetApiResponse<ApiShipOrderStatusModel>("orders/status?clientNumber=" + orderNumber);
            return orderStatus;
        }

        private T GetApiResponse<T>(string method)
        {
            try
            {
                string url = (isTest ? urlTestApi : urlApi) + method;
                string responseString = "";
                using (var client = new HttpClient())
                {
                    //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.DefaultRequestHeaders.Add("Authorization", apiToken);
                    
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var result = JsonConvert.DeserializeObject<T>(responseString);
                        return result;
                    }
                    else if (((int)response.StatusCode) >= 400 && ((int)response.StatusCode) < 500)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var errorResult = JsonConvert.DeserializeObject<ApiShipErrorModel>(responseString);
                        Debug.Log.Error(String.Format("Ошибка: {0}. Описание: {1}. Поле ошибки: {2}. Расшифровка ошибки: {3}", errorResult.message, errorResult.description, errorResult.errors.FirstOrDefault()?.field, errorResult.errors.FirstOrDefault()?.message));
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return default(T);
        }

        private T PostApiResponse<T, M>(M model, string method, List<AddParam> additionalParameters = null)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(model);
                string url = (isTest ? urlTestApi : urlApi) + method;
                string responseString = "";
                using (var client = new HttpClient())
                {
                    //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.DefaultRequestHeaders.Add("Authorization", apiToken);
                    if (additionalParameters != null && additionalParameters.Any())
                    {
                        foreach (var parameter in additionalParameters)
                        {
                            client.DefaultRequestHeaders.Add(parameter.Name, parameter.Value);
                        }
                    }
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var result = JsonConvert.DeserializeObject<T>(responseString);
                        return result;
                    }
                    else if (((int)response.StatusCode) >= 400 && ((int)response.StatusCode) < 500)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var errorResult = JsonConvert.DeserializeObject<ApiShipErrorModel>(responseString);
                        Debug.Log.Error(String.Format("Ошибка: {0}. Описание: {1}. Поле ошибки: {2}. Расшифровка ошибки: {3}", errorResult.message, errorResult.description, errorResult.errors.FirstOrDefault()?.field, errorResult.errors.FirstOrDefault()?.message));
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
                string responseString = "";
                using (var client = new HttpClient())
                {
                    //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.DefaultRequestHeaders.Add("Authorization", apiToken);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.DeleteAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var result = JsonConvert.DeserializeObject<ApiShipDeleteOrderModel>(responseString);
                        return result;
                    }
                    else if (((int)response.StatusCode) >= 400 && ((int)response.StatusCode) < 500)
                    {
                        var stringData = response.Content.ReadAsStringAsync();
                        if (stringData != null) responseString = stringData.Result;
                        var errorResult = JsonConvert.DeserializeObject<ApiShipErrorModel>(responseString);
                        Debug.Log.Error(String.Format("Ошибка: {0}. Описание: {1}. Поле ошибки: {2}. Расшифровка ошибки: {3}", errorResult.message, errorResult.description, errorResult.errors.FirstOrDefault()?.field, errorResult.errors.FirstOrDefault()?.message));
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return default(ApiShipDeleteOrderModel);
        }
    }
}
