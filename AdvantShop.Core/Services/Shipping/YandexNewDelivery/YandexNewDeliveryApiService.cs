using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewDeliveryApiService
    {
        private readonly string _authorizationToken;
        private const string Url = "https://api.delivery.yandex.ru";

        public YandexNewDeliveryApiService(string authorizationToken)
        {
            _authorizationToken = authorizationToken;
        }

        public int GetCityGeoId(string cityName)
        {
            var cacheKey = "YandexNewDelivery_GetCityGeoId_" + cityName;
            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<int>(cacheKey);

            var response = MakeRequest(Url + "/location?term=" + cityName, method: "GET");
            try
            {
                var dto = JsonConvert.DeserializeObject<List<YandexNewDeliveryLocationDto>>(response);
                if (dto != null && dto.Count > 0)
                {
                    var geoId = dto.First().GeoId;
                    CacheManager.Insert(cacheKey, geoId);
                    return geoId;
                }
            }
            catch(Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return 0;
        }

        public YandexNewDeliveryOrderStatuses GetOrderStatuses(string id)
        {
            var response = MakeRequest($"{Url}/orders/{id}/statuses",  method: "GET");
            try
            {
                var dto = JsonConvert.DeserializeObject<YandexNewDeliveryOrderStatuses>(response);
                return dto;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }

        public List<YandexNewDeliveryPickupPointDto> GetPickupPointsInfo(List<long> pickupPointsIds)
        {
            var result = new List<YandexNewDeliveryPickupPointDto>();

            foreach(var group in pickupPointsIds.Select( (value, index) => new { Value = value, Index = index / 100}).GroupBy(i => i.Index, v => v.Value))
            {
                var pickPointsInfo = GetPickupPointsInfoPartial(group.ToList());
                if (pickPointsInfo != null)
                    result.AddRange(pickPointsInfo);
            }

            return result;
        }

        private List<YandexNewDeliveryPickupPointDto> GetPickupPointsInfoPartial(List<long> pickupPointsIds)
        {
            var data = new
            {
                pickupPointIds = pickupPointsIds
            };
            var response = MakeRequest(Url + "/pickup-points", postDataString: JsonConvert.SerializeObject(data), method: "PUT");
            try
            {
                var dto = JsonConvert.DeserializeObject<List<YandexNewDeliveryPickupPointDto>>(response);
                return dto;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }

        public string GetOrderTrackNumber(string yandexOrderId)
        {
            var response = MakeRequest(Url + $"/orders/{yandexOrderId}", method: "GET");
            try
            {
                var dto = JsonConvert.DeserializeObject<YandexNewDeliveryOrderDto>(response);
                return dto?.DeliveryServiceExternalId;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            
            return string.Empty;
        }
        
        public string MakeRequest(string url, string postDataString = null, string method = "POST")
        {
            string responseData = null;

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = method;
            webRequest.Headers.Add("Authorization", "OAuth " + _authorizationToken);

            if (!string.IsNullOrEmpty(postDataString))
            {
                var bytes = Encoding.UTF8.GetBytes(postDataString);
                webRequest.ContentLength = bytes.Length;

                using (var requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
            }

            try
            {
                using (var response = webRequest.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            responseData = reader.ReadToEnd();
                        }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var errorData = string.Empty;

                    using (var errorResponse = ex.Response as HttpWebResponse)
                    using (var stream = errorResponse?.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                errorData = reader.ReadToEnd();
                            }
                    }

                    if (errorData.ToLower().Contains("unable to add orders to shipment"))
                        throw new BlException("Не удалось создать черновик заказа потому, что уже есть отгрузка в статусе \"Реестры отправлены\". Подробнее в <a target=\"_blank\" href=\"https://www.advantshop.net/help/pages/delivery-yandex\">инструкции</a>");

                    if (errorData.ToLower().Contains("failed to find [logistics_point]"))
                        throw new BlException("ПВЗ не найден. Попробуйте выбрать ПВЗ ещё раз.");

                    try
                    {
                        var err = " url: " + url + " data: " + postDataString;

                        var errorDto = JsonConvert.DeserializeObject<YandexNewDeliveryErrorDto>(errorData);
                        Debug.Log.Error(
                            errorDto != null
                                ? errorDto.Message + err
                                : "Error on request to yandex new delivery api " + err,
                            new Exception(errorData + err));
                    }
                    catch
                    {
                        Debug.Log.Error(errorData);
                    }
                }
                else
                {
                    Debug.Log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return responseData ?? "";
        }
    }
}
