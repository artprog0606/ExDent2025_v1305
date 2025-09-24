using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AdvantShop.Core.Caching;
using AdvantShop.Orders;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Repository;

namespace AdvantShop.Shipping.Measoft.Api
{
    // документация https://wiki.courierexe.ru/index.php/API
    public class MeasoftApiService
    {
        private const string Url = "https://home.courierexe.ru/api";
        private readonly MeasoftXmlConverter _xmlConverter;
        private const string _cacheKey = "MeasoftCache_";

        public MeasoftApiService(MeasoftXmlConverter xmlConverter)
        {
            _xmlConverter = xmlConverter;
        }

        public static string MakeRequest(string data, string method = "POST")
        {
            WebRequest request = HttpWebRequest.Create(Url);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);

            request.Method = method;
            request.ContentType = "text/xml; charset=utf-8";
            request.ContentLength = byteArray.Length;

            using (Stream streamWriter = request.GetRequestStream())
            {
                streamWriter.Write(byteArray, 0, byteArray.Length);
                streamWriter.Close();
            }

            using (var response = request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                        return null;

                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        return responseFromServer;
                    }
                }
            }
        } 

        public IEnumerable<MeasoftCalcOption> CalcOptions(MeasoftCalcOptionParams calcOption, List<MeasoftItem> items)
        {
            calcOption.WithPrice = false;
            var data = _xmlConverter.GetXmlCalculate(calcOption, items);
            var response = MakeRequest(data);
            var basePriceCalcOption =
                _xmlConverter.ParseAnswerCalculate(response, calcOption.ExtraDeliveryDays)
                             .Where(x => calcOption.DeliveryServiceIds.Contains(x.DeliveryId));

            calcOption.WithPrice = true;
            data = _xmlConverter.GetXmlCalculate(calcOption, items);
            response = MakeRequest(data);
            var priceCashCalcOption =
                _xmlConverter.ParseAnswerCalculate(response, calcOption.ExtraDeliveryDays)
                             .Where(x => calcOption.DeliveryServiceIds.Contains(x.DeliveryId));

            foreach (var item in basePriceCalcOption)
            {
                var priceCashItem = priceCashCalcOption.FirstOrDefault(x => x.DeliveryId == item.DeliveryId);
                item.PriceCash = priceCashItem?.BasePrice ?? item.BasePrice;
                item.WithInsure = calcOption.WithInsure;
            }

            return basePriceCalcOption;
        }

        public MeasoftOrderStatus SyncStatusOrder(string trackNumber)
        {
            var data = _xmlConverter.GetXmlSyncOrderStatus(trackNumber);
            var response = MakeRequest(data);
            var result = _xmlConverter.ParseAnswerOrderStatus(response);

            return result;
        }

        public List<MeasoftOrderStatus> SyncStatusOrderList()
        {
            var data = _xmlConverter.GetXmlSyncOrderStatus();
            var response = MakeRequest(data);
            List<MeasoftOrderStatus> orderStatusList = _xmlConverter.ParseAnswerOrderStatusList(response);

            // подтверждаем, что получили список обновлённых статусов
            var dataCommit = _xmlConverter.GetXmlCommitStatuses();
            MakeRequest(dataCommit);

            return orderStatusList;
        }

        public MeasoftOrderTrackNumber CreateOrder(Order order, float weight, int[] dimensionsInSm, int? paymentCodCardId)
        {
            var data = _xmlConverter.GetXmlCreateOrder(order, weight, dimensionsInSm, paymentCodCardId);
            var response = MakeRequest(data);
            var result = _xmlConverter.ParseAnswerCreateOrder(response);

            return result;
        }

        public MeasoftDeleteOrderResult DeleteOrder(string trackNumber)
        {
            var data = _xmlConverter.GetXmlDeleteOrder(trackNumber);
            var response = MakeRequest(data);
            var result = _xmlConverter.ParseAnswerDeleteOrder(response);

            return result;
        }

        public List<MeasoftPoint> GetPoints(MeasoftCity measoftCity, float weightInKilogramm)
        {
            var weightInGrams = MeasureUnits.ConvertWeight(
                weightInKilogramm,
                MeasureUnits.WeightUnit.Kilogramm,
                MeasureUnits.WeightUnit.Grams);
            string cacheKey = $"{_cacheKey}GetPoints_{measoftCity.Region.Code}_{measoftCity.Name}";
            var points = CacheManager.Get<List<MeasoftPoint>>(cacheKey);
            if (points == null)
            {
                var data = _xmlConverter.GetXmlPoints(measoftCity.Region.Code, measoftCity.Name);
                var response = MakeRequest(data);
                points = _xmlConverter.ParseAnswerPoints(response)?
                                      .Where(x => x.MaxWeightInGrams is null || x.MaxWeightInGrams >= weightInGrams)
                                      .OrderBy(x => x.Address)
                                      .ToList();

#if DEBUG
                // В тестовой базе Measoft корректно работает 1 ПВЗ =)
                var testWorkPoint = points.FirstOrDefault(x => x.Code == "56519");
                if (testWorkPoint != null)
                    testWorkPoint.Address = "Рабочий ПВЗ для тестов";
#endif

                CacheManager.Insert(cacheKey, points, 1440);
            }

            return points;
        }

        public MeasoftCity GetCity(string city, string district, string region, string country)
        {
            string cacheKey = $"{_cacheKey}GetCity{city}_{district}_{region}_{country}";
            var measoftCity = CacheManager.Get<MeasoftCity>(cacheKey, 1440, () =>
            {
                var countryIso2 = country.IsNotEmpty() ? Repository.CountryService.GetIso2(country) : null;

                var result = MakeRequest<GetCitiesResponse, GetCitiesParams>(new GetCitiesParams
                {
                    ConditionsParams = new ConditionsParams()
                    {
                        City = region.IsNotEmpty() ? region : null,
                        Name = city,
                        Country = countryIso2
                    }
                });
                
                var findedCity = result?.Count > 0
                    ? result.Cities.FirstOrDefault(x => district.IsNullOrEmpty() || x.Name.Contains(district))
                    : null;

                if (findedCity is null)
                {
                    // если не нашли город с названием региона, ищем без региона
                    result = MakeRequest<GetCitiesResponse, GetCitiesParams>(new GetCitiesParams
                    {
                        ConditionsParams = new ConditionsParams()
                        {
                            City = null,
                            NameStarts = city,
                            Country = countryIso2
                        }
                    });

                    findedCity = result?.Count > 0
                        ? result.Cities.FirstOrDefault(x => district.IsNullOrEmpty() || x.Name.Contains(district))
                          ?? result.Cities.FirstOrDefault()
                        : null;
                }

                return findedCity;
            });

            return measoftCity;
        }

        public static List<MeasoftDeliveryService> GetDeliveryServices(string extra)
        {
            if (string.IsNullOrEmpty(extra))
                return new List<MeasoftDeliveryService>();

            string cacheKey = $"{_cacheKey}GetDeliveryServices_{extra}";
            var deliveryServices = CacheManager.Get<List<MeasoftDeliveryService>>(cacheKey);
            if (deliveryServices == null)
            {
                var data = MeasoftXmlConverter.GetXmlDeliveryServices(extra);
                var response = MakeRequest(data);
                deliveryServices = MeasoftXmlConverter.ParseAnswerDeliveryServices(response);

                CacheManager.Insert(cacheKey, deliveryServices, 1440);
            }

            return deliveryServices;
        }

        public List<MeasoftStoreList> GetStoreList(string extra)
        {
            if (string.IsNullOrEmpty(extra))
                return new List<MeasoftStoreList>();

            var result = MakeRequest<MesoftStoreListResponse, GetStoreListParams>(new GetStoreListParams
            {
                Auth = new MeasoftAuthOption
                {
                    Extra = extra
                }
            });

            return result?.StoreList;
        }

        private T MakeRequest<T, TD>(TD data)
            where T : class , new()
        {
            T responseObj = default(T);

            try
            {
                var request = CreateRequest(data);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                    {
                        object obj;
#if !DEBUG
                        // для Release режима десериализуем сразу из потока
                        obj = Deserialize<T>(stream);
#endif
#if DEBUG
                        // для режима отладки десериализуем так,
                        // чтобы можно было посмотреть ответ сервера
                        string result;
                        using (var reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }

                        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                        {
                            obj = Deserialize<T>(memoryStream);
                        }
#endif
                        var errorResponse = obj as ErrorResponse;
                        if (errorResponse?.Error != null)
                        {
                            Debug.Log.Warn(
                                $"Measoft: {errorResponse.Error.Message} (code {errorResponse.Error.Code})");
                        }
                        else
                            responseObj = (T) obj;

                    }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return responseObj;
        }

        private HttpWebRequest CreateRequest<T>(T data)
        {
            var request = WebRequest.CreateHttp(Url);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";

            if (data != null)
                using (var requestStream = request.GetRequestStream())
                {
                    _xmlConverter.WriteDataToStream(requestStream, data, writeXmlDeclaration: true);
                    requestStream.Close();
                }

            return request;
        }

        private object Deserialize<T>(Stream stream) where T : class, new()
        {
            using (var reader = XmlReader.Create(stream))
            {

                reader.MoveToContent();

                var isError = reader.Name ==
                              typeof(ErrorResponse).GetCustomAttributes<XmlRootAttribute>().First().ElementName;


                XmlSerializer deserializer = new XmlSerializer(isError ? typeof(ErrorResponse) : typeof(T));
                return deserializer.Deserialize(reader);
            }
        }
    }
}
