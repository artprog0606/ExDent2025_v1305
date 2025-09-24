using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;


using Newtonsoft.Json;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping.Boxberry;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository;

namespace AdvantShop.Core.Services.Shipping.Boxberry
{
    public class BoxberryApiService
    {
        private enum _methods
        {
            ListCities,
            ListCitiesFull,
            ListPoints,
            ListPointsShort,
            ListZips,
            ZipCheck,
            ListStatuses,
            ListStatusesFull,
            ListServices,
            CourierListCities,
            DeliveryCosts,
            PointsForParcels,
            PointsByPostCode,
            PointsDescription,

            ParselCreate,
            ParselCheck,
            ParselList,
            ParselDel,
            ParselStory,
            ParselSend,
            ParselSendStory,
            CreateIntake,
            OrdersBalance,

            WidgetSettings,
            GetKeyIntegration
        }

        private Dictionary<_methods, string> _methodsNames = new Dictionary<_methods, string>
        {
            {_methods.ListCities, "ListCities" },
            {_methods.ListCitiesFull, "ListCitiesFull" },
            {_methods.ListPoints, "ListPoints" },
            {_methods.ListPointsShort, "ListPointsShort" },
            { _methods.ListZips, "ListZips" },
            {_methods.ZipCheck, "ZipCheck" },
            {_methods.ListStatuses, "ListStatuses" },
            {_methods.ListStatusesFull, "ListStatusesFull" },
            {_methods.ListServices, "ListServices" },
            {_methods.CourierListCities, "CourierListCities" },
            {_methods.DeliveryCosts, "DeliveryCosts" },
            {_methods.PointsForParcels, "PointsForParcels" },
            {_methods.PointsByPostCode, "PointsByPostCode" },
            {_methods.PointsDescription, "PointsDescription" },

            {_methods.ParselCreate, "ParselCreate" },
            {_methods.ParselDel, "ParselDel" },

            {_methods.WidgetSettings, "WidgetSettings" },
            
            {_methods.GetKeyIntegration, "GetKeyIntegration" },
        };

        private Dictionary<_methods, string> _cacheKeys = new Dictionary<_methods, string>
        {
            {_methods.ListCitiesFull, "BoxberryListCitiesFull" },
            {_methods.ListPoints, "BoxberryListPoints" },
            {_methods.CourierListCities, "BoxberryCourierListCities" },
            {_methods.ListZips, "BoxberryListZips" }
        };

        private readonly string _apiUrl;
        private readonly string _token;
        private readonly string _receptionPointCode;

        public BoxberryApiService()
        {
        }

        public BoxberryApiService(string apiUrl, string token, string receptionPointCode)
        {
            _apiUrl = apiUrl;
            _token = token;
            _receptionPointCode = receptionPointCode;
        }

        public List<BoxberryCity> GetListCities(string countryIso3Number = null)
        {
            var cities = CacheManager.Get<List<BoxberryCity>>(_cacheKeys[_methods.ListCitiesFull] + countryIso3Number);
            if (cities == null)
            {
                cities = MakeRequest<List<BoxberryCity>>(
                    _methods.ListCitiesFull,
                    urlParams:
                    countryIso3Number.IsNotEmpty()
                        ? "CountryCode=" + countryIso3Number
                        : null);
                if (cities != null)
                    CacheManager.Insert<List<BoxberryCity>>(_cacheKeys[_methods.ListCitiesFull] + countryIso3Number, cities, 1440);
            }
            return cities;
        }

        public List<BoxberryCityCourier> GetCourierListCities()
        {
            var cities = CacheManager.Get<List<BoxberryCityCourier>>(_cacheKeys[_methods.CourierListCities]);
            if (cities == null)
            {
                cities = MakeRequest<List<BoxberryCityCourier>>(_methods.CourierListCities);
                if (cities != null)
                    CacheManager.Insert<List<BoxberryCityCourier>>(_cacheKeys[_methods.CourierListCities], cities, 1440);
            }
            return cities;
        }

        public List<BoxberryObjectPoint> GetListPoints(string cityCode, bool prepaid = true)
        {
            var points = CacheManager.Get<List<BoxberryObjectPoint>>(_cacheKeys[_methods.ListPoints] + cityCode);
            if (points == null)
            {
                var queryParams = new Dictionary<string, string>
                    {
                        { "CityCode", cityCode },
                        { "prepaid", prepaid ? "1" : "0" }
                    }.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value))).AggregateString("&");

                points = MakeRequest<List<BoxberryObjectPoint>>(_methods.ListPoints, queryParams, ERequestMethod.GET);
                if (points != null && points.Count > 0 && string.IsNullOrEmpty(points[0].Error))
                {
                    CacheManager.Insert<List<BoxberryObjectPoint>>(_cacheKeys[_methods.ListPoints] + cityCode, points, 60);
                }
            }
            return points;
        }

        public BoxberryObjectPointDescription GetPointDescription(string code, bool isLoadPhoto = false)
        {
                var queryParams = new Dictionary<string, string>
                    {
                        { "code", code },
                        { "photo", isLoadPhoto ? "1" : "0" }
                    }.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value))).AggregateString("&");

                var boxberryPoint = MakeRequest<BoxberryObjectPointDescription>(_methods.PointsDescription, queryParams, ERequestMethod.GET);
                if (boxberryPoint != null
                    && boxberryPoint.Name.IsNullOrEmpty()
                    && boxberryPoint.CityName.IsNullOrEmpty()
                    && boxberryPoint.Address.IsNullOrEmpty())
                    // несуществующая ПВЗ (api присылает незаполненный объект)
                    return null;
                if (boxberryPoint != null)
                    boxberryPoint.Code = code;
                return boxberryPoint;
        }

        public BoxberryDeliveryCost GetDeliveryCosts(string target, float weight, float? ordersum, float deliverysum, float height, float width, float depth, string zip, float? paySum)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "target", target },
                { "weight", weight.ToInvariantString() },
                { "ordersum", ordersum.HasValue ? ordersum.ToInvariantString() : string.Empty },
                { "deliverysum", deliverysum.ToInvariantString() },
                { "targetstart", string.IsNullOrEmpty(_receptionPointCode) ? "0" : _receptionPointCode },
                { "height", height.ToInvariantString() },
                { "width", width.ToInvariantString() },
                { "depth", depth.ToInvariantString() },
                { "zip", zip.ToString() },
                { "paysum", paySum.HasValue ? paySum.ToInvariantString() : string.Empty },
                { "sucrh", "1" },
            }.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value))).AggregateString("&");

            return MakeRequest<BoxberryDeliveryCost>(_methods.DeliveryCosts, queryParams);
        }

        public List<BoxberryZip> GetListZips()
        {
            var zips = CacheManager.Get<List<BoxberryZip>>(_cacheKeys[_methods.ListZips]);
            if (zips == null)
            {
                zips = MakeRequest<List<BoxberryZip>>(_methods.ListZips);
                if (zips != null)
                    CacheManager.Insert<List<BoxberryZip>>(_cacheKeys[_methods.ListZips], zips, 1440);
            }
            return zips;
        }

        public bool ZipCheck(string zipCode, string countryIso3Number = null)
        {
            var zipCheckAnswerObject = MakeRequest<List<BoxberryZipCheck>>(_methods.ZipCheck, $"Zip={zipCode}&CountryCode={countryIso3Number}");

            return zipCheckAnswerObject != null && zipCheckAnswerObject[0].ExpressDelivery == "1";
        }

        public BoxberryWaitingOrdersAnswer GetWaitingOrders()
        {
            return MakeRequest<BoxberryWaitingOrdersAnswer>(_methods.ParselList);
        }

        public BoxberryObjectOptions GetBoxberryOptions()
        {
            return MakeRequest<BoxberryObjectOptions>(_methods.WidgetSettings);
        }

        public BoxberryApiResponse<List<BoxberryStatuse>> ListStatuses(string orderIdOrTrackNumber)
        {
            return MakeRequest<BoxberryApiResponse<List<BoxberryStatuse>>>(_methods.ListStatuses, "ImId=" + orderIdOrTrackNumber);
        }

        public BoxberryStatusesFull ListStatusesFull(string orderIdOrTrackNumber)
        {
            return MakeRequest<BoxberryStatusesFull>(_methods.ListStatusesFull, "ImId=" + orderIdOrTrackNumber);
        }

        #region Parsel

        public BoxberryOrderTrackNumber ParselCreate(Order order, int totalWeight, int[] dimentions, bool withInsure, Currency shippingCurrency)
        {
            var boxberryCustomer = new BoxberryOrderCustomer
            {
                Fio =
                    (string.IsNullOrEmpty(order.OrderRecipient.LastName) ? string.Empty : order.OrderRecipient.LastName + " ") +
                    (string.IsNullOrEmpty(order.OrderRecipient.FirstName) ? string.Empty : order.OrderRecipient.FirstName + " ") +
                    (string.IsNullOrEmpty(order.OrderRecipient.Patronymic) ? string.Empty : order.OrderRecipient.Patronymic + " "),
                Email = order.OrderCustomer.Email,
                Phone = order.OrderRecipient.Phone,
                Address = string.Join(
                            ", ",
                            new[] {
                                order.OrderCustomer.Country,
                                order.OrderCustomer.Region,
                                order.OrderCustomer.City,
                                order.OrderCustomer.Street,
                                order.OrderCustomer.House,
                                order.OrderCustomer.Apartment
                            }.Where(x => x.IsNotEmpty()))
            };

            var boxberryOrderItems = new List<BoxberryOrderItem>();

            var orderSum = order.Sum;
            var shippingCost = order.ShippingCostWithDiscount;

            if (shippingCurrency != null)
            {
                // Конвертируем в валюту доставки
                shippingCost = shippingCost.ConvertCurrency(order.OrderCurrency, shippingCurrency);
                orderSum = orderSum.ConvertCurrency(order.OrderCurrency, shippingCurrency);
            }

            var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                                  .AcceptableDifference(0.1f)
                                  .WithCurrency(shippingCurrency)
                                  .CeilingAmountToInteger()
                                  .GetItems();

            foreach (var orderItem in orderItems)
            {
                boxberryOrderItems.Add(new BoxberryOrderItem
                {
                    Id = orderItem.ArtNo,
                    Name = orderItem.Name,
                    Nds = orderItem.TaxRate?.ToString("F0"),
                    Price = orderItem.Price,
                    Quantity = orderItem.Amount,
                    UnitName = orderItem.Unit
                });
            }

            var isDelivertyToPoint = false;
            BoxberryObjectPoint boxberryPoint = null;
            
            if (order.OrderPickPoint != null)
                if (order.OrderPickPoint.AdditionalData != null
                    && order.OrderPickPoint.AdditionalData.Contains("ModelType"))
                {
                    // в старых заказах хранится сериализованный BoxberryOption курьерской доставки
                    // boxberryOption = JsonConvert.DeserializeObject<BoxberryOption>(order.OrderPickPoint.AdditionalData);
                    isDelivertyToPoint = false;
                }
                else if (order.OrderPickPoint.AdditionalData != null)
                {
                    boxberryPoint =
                        JsonConvert.DeserializeObject<BoxberryObjectPoint>(order.OrderPickPoint.AdditionalData);
                    isDelivertyToPoint = true;
                }

            var countryIso2 = CountryService.GetIso2(order.OrderCustomer.Country);

            var data = new BoxberryOrder
            {
                OrderId = order.Number,
                TrackCode = order.TrackNumber,
                Price =
                    withInsure || !order.Payed
                        ? (orderSum - shippingCost).ToInvariantString()
                        // для ПВЗ и курьера по РФ не обязательно
                        : boxberryPoint != null || "ru".Equals(countryIso2, StringComparison.OrdinalIgnoreCase)
                            ? null
                            :"0",
                PaymentSum = order.Payed ? "0" : orderSum.ToInvariantString(),
                DeliverySum = shippingCost.ToInvariantString(),
                DeliveryType = isDelivertyToPoint ? "1" : "2",
                Customer = boxberryCustomer,
                Items = boxberryOrderItems,
                Weights = new BoxberryOrderWeight 
                { 
                    Weight = totalWeight.ToString(),
                    Height = Convert.ToString(dimentions[0]),
                    Width = Convert.ToString(dimentions[1]),
                    Length = Convert.ToString(dimentions[2])
                }
            };

            if (!isDelivertyToPoint)
            {
                var countryIso3Number = CountryService.Iso2ToIso3Number(countryIso2);

                // для России
                if (countryIso3Number.IsNotEmpty()
                    || countryIso3Number == "643")
                {
                    data.Kurdost = new BoxberryOrderShippingInfo
                    {
                        Address =
                            string.Join(
                                ", ",
                                new[]
                                {
                                    order.OrderCustomer.Street,
                                    order.OrderCustomer.House,
                                    order.OrderCustomer.Apartment
                                }.Where(x => x.IsNotEmpty())),
                        City = order.OrderCustomer.City,
                        Index = order.OrderCustomer.Zip,
                        Comment = order.CustomerComment
                    };
                }
                else
                {
                    BoxberryCity boxberryCity = null;
                    var cities = GetListCities(countryIso3Number);
                    if (cities != null)
                    {
                        cities.ForEach(x => x.Name.Replace("ё", "е"));
                        cities.ForEach(x => x.Region = x.Region?.RemoveTypeFromRegion());

                        var normolizeCityName = order.OrderCustomer.City.Replace("ё", "е");
                        var normolizeRegionDest = order.OrderCustomer.Region?.RemoveTypeFromRegion();
                        boxberryCity =
                            AdvantShop.Shipping.Boxberry.Boxberry.FindCity(cities, normolizeCityName, order.OrderCustomer.District, normolizeRegionDest) // по всем данным
                            ?? AdvantShop.Shipping.Boxberry.Boxberry.FindCity(cities, normolizeCityName, null, normolizeRegionDest); // без района
                        //?? AdvantShop.Shipping.Boxberry.Boxberry.FindCity(cities, normolizeCityName, null, null); // без района и региона (ненадо, считает не втот город)
                    }

                    data.Export = new BoxberryOrderExportInfo()
                    {
                        Index = order.OrderCustomer.Zip,
                        CountryCode = countryIso3Number,
                        CityCode = boxberryCity?.Code,
                        Area = order.OrderCustomer.Region,
                        Street = order.OrderCustomer.Street,
                        House = order.OrderCustomer.House,
                        Flat = order.OrderCustomer.Apartment,
                        TransporterGuid = "fd85a8b6-4688-404f-9993-30b9e55d2950"
                    };
                }
            }

            data.Shop = new BoxberryOrderShopInfo
            {
                CodeDestination = boxberryPoint != null ? boxberryPoint.Code : "",
                CodeDeparture = _receptionPointCode
            };

            return MakeRequest<BoxberryOrderTrackNumber>(_methods.ParselCreate, "sdata=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(data)));
        }

        public BoxberryOrderDeleteAnswer ParselDelete(string trackNumber)
        {
            return MakeRequest<BoxberryOrderDeleteAnswer>(_methods.ParselDel, "ImId=" + trackNumber);
        }

        public List<BoxberryParcelPoint> GetListPointsForParcels()
        {
            var result = MakeRequest<List<BoxberryParcelPoint>>(_methods.PointsForParcels, requestMethod: ERequestMethod.GET);
            if (result == null || result.Count == 1 || result[0].Code == null)
                return new List<BoxberryParcelPoint>();

            return result.OrderBy(point => point.City).ThenBy(pont => pont.Name).ToList();
        }

        public BoxberryGetKeyIntegration GetKeyIntegration()
        {
            return MakeRequest<BoxberryGetKeyIntegration>(_methods.GetKeyIntegration);
        }

        #endregion

        #region API request

        private T MakeRequest<T>(_methods method, string urlParams = null,
            ERequestMethod requestMethod = ERequestMethod.POST,
            ERequestContentType contentType = ERequestContentType.FormUrlencoded)
        {
            try
            {
                var data = new Dictionary<string, string>
                {
                    { "platform", "advantshop" },
                    { "cms", "advantshop" },
                    { "url", AdvantShop.Configuration.SettingsMain.SiteUrlPlain },
                    { "version", AdvantShop.Configuration.SettingsGeneral.SiteVersionDev },
                    { "token", _token },
                    { "method", _methodsNames[method] }
                };

                var queryParams = data.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value))).AggregateString("&") + 
                    (urlParams.IsNotEmpty() ? "&" + urlParams : string.Empty);

                var httpsProtocol = "https://";
                var url = 
                    _apiUrl.StartsWith(httpsProtocol, StringComparison.OrdinalIgnoreCase)
                        ? _apiUrl.Insert(httpsProtocol.Length, "proxy.advstatic.ru/")
                        : _apiUrl;
                
                if (requestMethod == ERequestMethod.GET)
                    url += (url.Contains("?") ? "&" : "?") + queryParams;

                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = requestMethod.StrName();
                request.ContentType = contentType.StrName();

                if (requestMethod == ERequestMethod.POST)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(queryParams);
                    request.ContentLength = bytes.Length;
                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                string resultString = null;
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                resultString = reader.ReadToEnd();
                            }
                    }
                }

                if (resultString.IsNotEmpty() && resultString.Contains("\"err\""))
                {
                    string error = null;
                    if (resultString.StartsWith("["))
                    {
                        var deserializeObbject = JsonConvert.DeserializeObject<List<BoxberryErrorObject>>(resultString);
                        error = deserializeObbject != null ? string.Join(", ", deserializeObbject.Select(x => x.Error)) : null;
                    }
                    else
                    {
                        var deserializeObbject = JsonConvert.DeserializeObject<BoxberryErrorObject>(resultString);
                        error = deserializeObbject != null ? deserializeObbject.Error : null;
                    }
                    if (error.IsNotEmpty())
                        Debug.Log.Warn("Boxberry " + method.ToString() + " error: " + error);
                }
                else if (resultString.IsNotEmpty())
                {
                    return JsonConvert.DeserializeObject<T>(resultString);
                }
            }
            catch (WebException ex)
            {
                string error = null;
                if (ex.Response != null)
                    using (var eResponse = ex.Response)
                    using (var eStream = eResponse.GetResponseStream())
                        if (eStream != null)
                            using (var reader = new StreamReader(eStream))
                                error = reader.ReadToEnd();
                Debug.Log.Warn("Boxberry " + method.ToString() + " error: " + (error.IsNotEmpty() ? error : ex.Message), ex);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Boxberry " + method.ToString() + " error: " + ex.Message, ex);
            }
            return default(T);
        }

        #endregion
    }
}
