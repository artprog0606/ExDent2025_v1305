using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace AdvantShop.Shipping.LPost.Api
{
    public class LPostApiService
    {
        private const string _tokenCacheKey = "LPostToken_";
        //private const string _apiUrl = "https://apitest.l-post.ru/"; // тестовый апи
        private const string _apiUrl = "https://api.l-post.ru/";
        private const string _cacheKey = "LPostCache_";

        private readonly List<LPostMethod> _methodList = new List<LPostMethod>
        {
            new LPostMethod {Method = EMethod.Auth, Name = "Auth"},
            new LPostMethod {Method = EMethod.GetReceivePoints, Name = "GetReceivePoints", Version = 1},
            new LPostMethod {Method = EMethod.GetPickupPoints, Name = "GetPickupPoints", Version = 1},
            new LPostMethod {Method = EMethod.GetAddressPoints, Name = "GetAddressPoints", Version = 1},
            new LPostMethod {Method = EMethod.GetServicesCalc, Name = "GetServicesCalc", Version = 1}
        };
        private readonly string _secretKey;
        private LPostToken _token;

        public LPostApiService(string secretKey)
        {
            if (secretKey.IsNotEmpty())
            {
                _secretKey = secretKey;
                CheckToken();
            }
        }

        public string GetToken() => _token?.Token;

        public List<LPostReceivePoint> GetReceivePoints()
        {
            string cacheKey = _cacheKey + "receivePoints";
            var receivePoints = CacheManager.Get<List<LPostReceivePoint>>(cacheKey);
            if (receivePoints == null)
            {
                if (!CheckToken())
                    return new List<LPostReceivePoint>();
                var receivePointList = MakeRequest<LPostReceivePointList>(EMethod.GetReceivePoints);
                if (receivePointList != null)
                {
                    receivePoints = receivePointList.ReceivePoints;
                    CacheManager.Insert(cacheKey, receivePoints, 60);
                }
            }
            return receivePoints ?? new List<LPostReceivePoint>();
        }

        public List<LPostPickPoint> GetPickupPoints(PickPointParams param)
        {
            if (!CheckToken())
                return null;
            var pickupPointList = MakeRequest<LPostPickPointList>(EMethod.GetPickupPoints, data: param);
            return pickupPointList?.PickPoints ?? new List<LPostPickPoint>();
        }

        public List<LPostAddressPoint> GetAddressPoints()
        {
            string cacheKey = _cacheKey + "addressPoints";
            var addressPoints = CacheManager.Get<List<LPostAddressPoint>>(cacheKey);
            if (addressPoints == null)
            {
                if (!CheckToken())
                    return null;
                var addressPointList = MakeRequest<LPostAddressPointList>(EMethod.GetAddressPoints);
                if (addressPointList != null)
                {
                    addressPoints = addressPointList.AddressPoints;
                    CacheManager.Insert(cacheKey, addressPoints, 60);
                }
            }
            return addressPoints;
        }

        public LPostDeliveryCost GetDeliveryCost(LPostOptionParams param)
        {
            if (CheckToken())
                return MakeRequest<LPostDeliveryCostList>(EMethod.GetServicesCalc, data: param)?.DeliveryCost;
            return null;
        }

        private bool CheckToken()
        {
            var cacheKey = _tokenCacheKey + _secretKey;
            if (_token == null)
                _token = CacheManager.Get<LPostToken>(cacheKey);
            if (_token == null || _token.NeedUpdate)
            {
                var method = _methodList.FirstOrDefault(x => x.Method == EMethod.Auth);
                var methodParams = new Dictionary<string, string>
                    {
                        {"method", method.Name },
                        {"secret", _secretKey }
                    };
                _token = MakeRequest<LPostToken>(method.Method, methodParams, requestMethod: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);
                if (_token != null)
                    CacheManager.Insert(cacheKey, _token, 60);
                else
                    return false;
            }
            return true;
        }

        private T MakeRequest<T>(EMethod eMethod, Dictionary<string, string> methodParams = null,
            object data = null,
            ERequestMethod requestMethod = ERequestMethod.GET,
            ERequestContentType contentType = ERequestContentType.TextJson) where T : LPostError
        {
            var method = _methodList.FirstOrDefault(x => x.Method == eMethod);
            try
            {
                if (methodParams == null)
                    methodParams = new Dictionary<string, string>
                    {
                        { "method", method.Name },
                        { "token", _token?.Token },
                        { "ver", method.Version.ToString() }
                    };

                JsonSerializerSettings jsonSettings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
                if (contentType == ERequestContentType.TextJson)
                {
                    string dataPost = JsonConvert.SerializeObject(data, jsonSettings);
                    methodParams.Add("json", data != null ? dataPost : "{}");
                }

                var queryParams = methodParams.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value))).AggregateString("&");
                var url = _apiUrl;
                if (requestMethod == ERequestMethod.GET)
                    url += (url.Contains("?") ? "&" : "?") + queryParams;

                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = requestMethod.StrName();
                request.ContentType = contentType.StrName();

                if (requestMethod == ERequestMethod.POST)
                {
                    request.Accept = "*/*";
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

                var dataAnswer = JsonConvert.DeserializeObject<T>(resultString);
                if (!string.IsNullOrEmpty(dataAnswer.Error))
                {
                    Debug.Log.Warn("LPost " + method.Name +
                    (data != null ? " data: " + JsonConvert.SerializeObject(data, jsonSettings) : "") +
                    " error: " + dataAnswer.Error);
                    return null;
                }
                return dataAnswer;
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
                Debug.Log.Warn("LPost " + method.Name + " error: " + (!string.IsNullOrEmpty(error) ? error : ex.Message), ex);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("LPost " + method.Name +
                    (data != null ? " data: " + JsonConvert.SerializeObject(data) : "") +
                    " error: " + ex.Message, ex);
            }
            return default(T);
        }

        public static int GetRegionCodeByName(string regionName)
        {
            if (regionName == "Республика Адыгея") return 1;
            if (regionName == "Республика Башкортостан") return 2;
            if (regionName == "Республика Бурятия") return 3;
            if (regionName == "Республика Алтай") return 4;
            if (regionName == "Республика Дагестан") return 5;
            if (regionName == "Республика Ингушетия") return 6;
            if (regionName == "Кабардино-Балкарская республика") return 7;
            if (regionName == "Республика Калмыкия") return 8;
            if (regionName == "Карачаево-Черкесская Республика") return 9;
            if (regionName == "Республика Карелия") return 10;
            if (regionName == "Республика Коми") return 11;
            if (regionName == "Республика Марий Эл") return 12;
            if (regionName == "Республика Мордовия") return 13;
            if (regionName == "Республика Саха (Якутия)") return 14; 
            if (regionName == "Республика Северная Осетия - Алания") return 15;
            if (regionName == "Республика Татарстан") return 16;
            if (regionName == "Республика Тыва") return 17;
            if (regionName == "Удмуртская Республика") return 18;
            if (regionName == "Республика Хакасия") return 19;
            if (regionName == "Чувашская Республика") return 21;
            if (regionName == "Алтайский край") return 22;
            if (regionName == "Краснодарский край") return 23;
            if (regionName == "Красноярский край") return 24;
            if (regionName == "Приморский край") return 25;
            if (regionName == "Ставропольский край") return 26;
            if (regionName == "Хабаровский край") return 27;
            if (regionName == "Амурская область") return 28;
            if (regionName == "Архангельская область") return 29;
            if (regionName == "Астраханская область") return 30;
            if (regionName == "Белгородская область") return 31;
            if (regionName == "Брянская область") return 32;
            if (regionName == "Владимирская область") return 33;
            if (regionName == "Волгоградская область") return 34;
            if (regionName == "Вологодская область") return 35;
            if (regionName == "Воронежская область") return 36;
            if (regionName == "Ивановская область") return 37;
            if (regionName == "Иркутская область") return 38;
            if (regionName == "Калининградская область") return 39;
            if (regionName == "Калужская область") return 40;
            if (regionName == "Камчатский край") return 41;
            if (regionName == "Кемеровская область") return 42;
            if (regionName == "Кировская область") return 43;
            if (regionName == "Костромская область") return 44;
            if (regionName == "Курганская область") return 45;
            if (regionName == "Курская область") return 46;
            if (regionName == "Ленинградская область") return 47;
            if (regionName == "Липецкая область") return 48;
            if (regionName == "Магаданская область") return 49;
            if (regionName == "Московская область") return 50;
            if (regionName == "Мурманская область") return 51;
            if (regionName == "Нижегородская область") return 52;
            if (regionName == "Новгородская область") return 53;
            if (regionName == "Новосибирская область") return 54;
            if (regionName == "Омская область") return 55;
            if (regionName == "Оренбургская область") return 56;
            if (regionName == "Орловская область") return 57;
            if (regionName == "Пензенская область") return 58;
            if (regionName == "Пермский край") return 59;
            if (regionName == "Псковская область") return 60;
            if (regionName == "Ростовская область") return 61;
            if (regionName == "Рязанская область") return 62;
            if (regionName == "Самарская область") return 63;
            if (regionName == "Саратовская область") return 64;
            if (regionName == "Сахалинская область") return 65;
            if (regionName == "Свердловская область") return 66;
            if (regionName == "Смоленская область") return 67;
            if (regionName == "Тамбовская область") return 68;
            if (regionName == "Тверская область") return 69;
            if (regionName == "Томская область") return 70;
            if (regionName == "Тульская область") return 71;
            if (regionName == "Тюменская область") return 72;
            if (regionName == "Ульяновская область") return 73;
            if (regionName == "Челябинская область") return 74;
            if (regionName == "Забайкальский край") return 75;
            if (regionName == "Ярославская область") return 76;
            if (regionName == "Москва") return 77;
            if (regionName == "Санкт-Петербург") return 78;
            if (regionName == "Еврейская АО") return 79;
            if (regionName == "Агинский Бурятский автономный округ") return 80;
            if (regionName == "Коми-Пермяцкий автономный округ") return 81;
            if (regionName == "Республика Крым") return 82;
            if (regionName == "Ненецкий АО") return 83;
            if (regionName == "Таймырский (Долгано - Ненецкий) АО") return 84; 
            if (regionName == "Усть-Ордынский Бурятский автономный округ") return 85;
            if (regionName == "Ханты-Мансийский АО - Югра") return 86;
            if (regionName == "Чукотский АО") return 87;
            if (regionName == "Эвенкийский автономный округ") return 88;
            if (regionName == "Ямало-Ненецкий АО") return 89;
            if (regionName == "Севастополь") return 92;
            if (regionName == "Чеченская Республика") return 95;

            return 0;
        }

        public static string GetRegionNameByCode(int regionCode)
        {
            switch (regionCode)
            {
                case 1: return "Республика Адыгея";
                case 2: return "Республика Башкортостан";
                case 3: return "Республика Бурятия";
                case 4: return "Республика Алтай";
                case 5: return "Республика Дагестан";
                case 6: return "Республика Ингушетия";
                case 7: return "Кабардино-Балкарская республика";
                case 8: return "Республика Калмыкия";
                case 9: return "Карачаево-Черкесская Республика";
                case 10: return "Республика Карелия";
                case 11: return "Республика Коми";
                case 12: return "Республика Марий Эл";
                case 13: return "Республика Мордовия";
                case 14: return "Республика Саха (Якутия)";
                case 15: return "Республика Северная Осетия - Алания";
                case 16: return "Республика Татарстан";
                case 17: return "Республика Тыва";
                case 18: return "Удмуртская Республика";
                case 19: return "Республика Хакасия";
                case 21: return "Чувашская Республика";
                case 22: return "Алтайский край";
                case 23: return "Краснодарский край";
                case 24: return "Красноярский край";
                case 25: return "Приморский край";
                case 26: return "Ставропольский край";
                case 27: return "Хабаровский край";
                case 28: return "Амурская область";
                case 29: return "Архангельская область";
                case 30: return "Астраханская область";
                case 31: return "Белгородская область";
                case 32: return "Брянская область";
                case 33: return "Владимирская область";
                case 34: return "Волгоградская область";
                case 35: return "Вологодская область";
                case 36: return "Воронежская область";
                case 37: return "Ивановская область";
                case 38: return "Иркутская область";
                case 39: return "Калининградская область";
                case 40: return "Калужская область";
                case 41: return "Камчатский край";
                case 42: return "Кемеровская область";
                case 43: return "Кировская область";
                case 44: return "Костромская область";
                case 45: return "Курганская область";
                case 46: return "Курская область";
                case 47: return "Ленинградская область";
                case 48: return "Липецкая область";
                case 49: return "Магаданская область";
                case 50: return "Московская область";
                case 51: return "Мурманская область";
                case 52: return "Нижегородская область";
                case 53: return "Новгородская область";
                case 54: return "Новосибирская область";
                case 55: return "Омская область";
                case 56: return "Оренбургская область";
                case 57: return "Орловская область";
                case 58: return "Пензенская область";
                case 59: return "Пермский край";
                case 60: return "Псковская область";
                case 61: return "Ростовская область";
                case 62: return "Рязанская область";
                case 63: return "Самарская область";
                case 64: return "Саратовская область";
                case 65: return "Сахалинская область";
                case 66: return "Свердловская область";
                case 67: return "Смоленская область";
                case 68: return "Тамбовская область";
                case 69: return "Тверская область";
                case 70: return "Томская область";
                case 71: return "Тульская область";
                case 72: return "Тюменская область";
                case 73: return "Ульяновская область";
                case 74: return "Челябинская область";
                case 75: return "Забайкальский край";
                case 76: return "Ярославская область";
                case 77: return "Москва";
                case 78: return "Санкт-Петербург";
                case 79: return "Еврейская АО";
                case 80: return "Агинский Бурятский автономный округ";
                case 81: return "Коми-Пермяцкий автономный округ";
                case 82: return "Республика Крым";
                case 83: return "Ненецкий АО";
                case 84: return "Таймырский (Долгано - Ненецкий) АО";
                case 85: return "Усть-Ордынский Бурятский автономный округ";
                case 86: return "Ханты-Мансийский АО - Югра";
                case 87: return "Чукотский АО";
                case 88: return "Эвенкийский автономный округ";
                case 89: return "Ямало-Ненецкий АО";
                case 92: return "Севастополь";
                case 95: return "Чеченская Республика";
                case 101: return "Республика Татарстан";
                default: return string.Empty;
            }
        }
    }
}
