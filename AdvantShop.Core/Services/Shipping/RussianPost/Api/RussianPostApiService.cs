using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.RussianPost.Api
{
    public class RussianPostApiService
    {
        private const string BaseUrl = "https://otpravka-api.pochta.ru";

        private readonly string _accessToken;
        private readonly string _login;
        private readonly string _password;
        public List<string> LastActionErrors { get; set; }

        public bool IsLimitedExceeded {
            get {
                return CacheManager.Get<bool>(string.Format("RussianPostApiIsLimitedExceeded_{0}", _accessToken));
            }

            private set
            {
                CacheManager.Insert(string.Format("RussianPostApiIsLimitedExceeded_{0}", _accessToken), value, 30);
            }
        }

        public RussianPostApiService(string login, string password, string accessToken)
        {
            _login = login;
            _password = password;
            _accessToken = accessToken;
        }

        /// <summary>
        /// Рассчет стоимости пересылки
        /// https://otpravka.pochta.ru/specification#/nogroup-rate_calculate
        /// </summary>
        /// <param name="calculateOptions">Параметры пересылки</param>
        /// <returns>Стоимость пересылки</returns>
        public CalculateResponse Calculate(CalculateOptions calculateOptions)
        {
            return MakeRequest<CalculateResponse>("/1.0/tariff", calculateOptions);
        }

        /// <summary>
        /// Рассчет стоимости пересылки
        /// https://otpravka.pochta.ru/specification#/nogroup-rate_calculate
        /// </summary>
        /// <param name="calculateOptions">Параметры пересылки</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Стоимость пересылки</returns>
        public async Task<CalculateResponse> CalculateAsync(CalculateOptions calculateOptions, CancellationToken cancellationToken)
        {
            return await
                (cancellationToken.CancelIfRequestedAsync<CalculateResponse>()
                 ?? MakeRequestAsync<CalculateResponse>("/1.0/tariff", cancellationToken, calculateOptions))
               .ConfigureAwait(false);
        }

        /// <summary>
        /// Нормализация адреса
        /// </summary>
        /// <param name="address">Адреса для нормализации</param>
        /// <returns>Нормализованные адреса</returns>
        public List<CleanAddressResponse> CleanAddress(params CleanAddress[] address)
        {
            return MakeRequest<List<CleanAddressResponse>>("/1.0/clean/address", address);
        }

        /// <summary>
        /// Создание заказа
        /// </summary>
        /// <param name="orders">Заказы</param>
        /// <returns>Ошибки и идентификаторы Почты России добавленных заказов</returns>
        public OrderRussianPostAddResponse CreateOrder(params OrderRussianPostAdd[] orders)
        {
            return MakeRequest<OrderRussianPostAddResponse>("/1.0/user/backlog", orders, "PUT");
        }

        /// <summary>
        /// Получение заказа по идентификатору
        /// </summary>
        /// <param name="russianPostOrderId">Идентификатор заказа Почты России</param>
        /// <returns>Заказ</returns>
        public OrderRussianPost GetOrder(long russianPostOrderId)
        {
            return MakeRequest<OrderRussianPost>("/1.0/backlog/" + russianPostOrderId, null, "GET");
        }

        /// <summary>
        /// Удаление заказа
        /// </summary>
        /// <param name="russianPostOrderIds">Идентификаторы заказов Почты России</param>
        /// <returns>Ошибки и идентификаторы удаленных заказов</returns>
        public OrderRussianPostDeleteResponse DeleteOrder(params long[] russianPostOrderIds)
        {
            return MakeRequest<OrderRussianPostDeleteResponse>("/1.0/backlog", russianPostOrderIds, "DELETE");
        }

        /// <summary>
        /// Получение текущих настроек пользователя
        /// </summary>
        /// <returns>Настройки пользователя</returns>
        public AccountSettings GetAccountSettings()
        {
            return MakeRequest<AccountSettings>("/1.0/settings", null, "GET");
        }

        /// <summary>
        /// Получение текущих точек сдачи
        /// </summary>
        /// <returns>Текущие точки сдачи</returns>
        public List<ShippingPoint> GetShippingPoints()
        {
            return MakeRequest<List<ShippingPoint>>("/1.0/user-shipping-points", null, "GET");
        }

        /// <summary>
        /// Генерация печатных форм для заказа
        /// </summary>
        /// <param name="russianPostOrderId">Идентификатор заказа Почты России</param>
        /// <param name="path">Директория куда необходимо сохранить файл</param>
        /// <param name="sendingDate">Дата отправки в почтовое отделение</param>
        /// <param name="printType">Тип печати</param>
        /// <returns>Путь к файлу</returns>
        public string GetDocuments(long russianPostOrderId, string path, DateTime? sendingDate = null, EnPrintType printType = null)
        {
            string filePath = null;

            var url = string.Format("/1.0/forms/{0}/forms", russianPostOrderId);

            if (sendingDate.HasValue || printType != null)
            {
                url += "?";

                if (sendingDate.HasValue)
                    url += string.Format("sending-date={0}&", sendingDate.Value.ToString("yyyy-MM-dd"));

                if (printType != null)
                    url += string.Format("print-type={0}&", JsonConvert.SerializeObject(printType.Value));
            }

            using (var responseStream = MakeRequestGetStream(url, CancellationToken.None, null, "GET"))
            {
                if (responseStream != null)
                {
                    filePath = Path.Combine(path, string.Format("RussianPostDocuments{0}.pdf", russianPostOrderId));

                    using (var filestream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        responseStream.CopyTo(filestream);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Генерация печатных форм для заказа (до формирования партии)
        /// </summary>
        /// <param name="russianPostOrderId">Идентификатор заказа Почты России</param>
        /// <param name="path">Директория куда необходимо сохранить файл</param>
        /// <param name="sendingDate">Дата отправки в почтовое отделение</param>
        /// <param name="printType">Тип печати</param>
        /// <returns>Путь к файлу</returns>
        public string GetDocumentsBeforShipment(long russianPostOrderId, string path, DateTime? sendingDate = null)
        {
            string filePath = null;

            var url = string.Format("/1.0/forms/backlog/{0}/forms", russianPostOrderId);

            if (sendingDate.HasValue)
                url += string.Format("?sending-date={0}&", sendingDate.Value.ToString("yyyy-MM-dd"));

            using (var responseStream = MakeRequestGetStream(url, CancellationToken.None, null, "GET"))
            {
                if (responseStream != null)
                {
                    filePath = Path.Combine(path, string.Format("RussianPostDocumentsBeforShipment{0}.pdf", russianPostOrderId));

                    using (var filestream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        responseStream.CopyTo(filestream);
                }
            }

            return filePath;
        }
        public string GetOfficesFile(string type, string path)
        {
            string filePath = null;

            var url = $"/1.0/unloading-passport/zip?type={type}";

            using (var responseStream = MakeRequestGetStream(url, CancellationToken.None, null, "GET"))
            {
                if (responseStream != null)
                {
                    filePath = Path.Combine(path, $"RussianPostOffices{type}.zip");
                    
                    using (var filestream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        responseStream.CopyTo(filestream);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Получение все точек ПВЗ для ЕКОМ
        /// </summary>
        /// <returns>Текущие точки сдачи</returns>
        public List<DeliveryPoint> GetDeliveryPoints()
        {
            return MakeRequest<List<DeliveryPoint>>("/1.0/delivery-point/findAll", null, "GET");
        }

        /// <summary>
        /// Получение все точек ПВЗ для ЕКОМ
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Текущие точки сдачи</returns>
        public async Task<List<DeliveryPoint>> GetDeliveryPointsAsync(CancellationToken cancellationToken)
        {
            return await
                (cancellationToken.CancelIfRequestedAsync<List<DeliveryPoint>>()
                 ?? MakeRequestAsync<List<DeliveryPoint>>("/1.0/delivery-point/findAll", cancellationToken, null, "GET"))
               .ConfigureAwait(false);
        }

        /// <param name="settlement">Название населённого пункта (например Екатеринбург)</param>
        /// <param name="region">Область/край/республика, где расположен населённый пункт (например Свердловская)</param>
        /// <param name="district">Район, где расположен населённый пункт (для деревень, посёлков и т. д. - например Сухоложский)</param>
        public List<string> GetCityPostOfficesCodes(string settlement, string region, string district)
        {
            string query = null;
            var getParams = new List<string>();
            if (!string.IsNullOrEmpty(settlement))
                getParams.Add("settlement=" + HttpUtility.UrlEncode(settlement));
            if (!string.IsNullOrEmpty(region))
                getParams.Add("region=" + HttpUtility.UrlEncode(region));
            if (!string.IsNullOrEmpty(district))
                getParams.Add("district=" + HttpUtility.UrlEncode(district));
            
            query = getParams.Count > 0 
                ? "?" + string.Join("&", getParams)
                : null;

            return MakeRequest<List<string>>($"/postoffice/1.0/settlement.offices.codes{query}", null, "GET");
        }

        #region PrivateMethods

        private Stream MakeRequestGetStream(string url, CancellationToken cancellationToken, object data = null, string method = "POST")
        {
            if (IsLimitedExceeded)
            {
                LastActionErrors = new List<string>() { "Лимит запросов к API исчерпан" };
                return null;
            }
            LastActionErrors = null;

            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return default;
                
                var request = CreateRequestAsync(url, data, method, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                using (cancellationToken.Register(request.Abort))
                {
                    var response = request.GetResponse();

                    return response.GetResponseStream();
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
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
                                        LastActionErrors = new List<string>()
                                        {
                                            string.IsNullOrEmpty(error) ? ex.Message : error
                                        };

                                        if (string.IsNullOrEmpty(error))
                                            Debug.Log.Error(ex);
                                        else
                                        {
                                            if (error.Contains("Token requests limit exceeded"))
                                                IsLimitedExceeded = true;
                                            Debug.Log.Error(error, ex);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    LastActionErrors = new List<string>() { ex.Message };
                                    Debug.Log.Error(ex);
                                }
                            }
                            else
                            {
                                LastActionErrors = new List<string>() { ex.Message };
                                Debug.Log.Error(ex);
                            }
                    }
                    else
                    {
                        LastActionErrors = new List<string>() { ex.Message };
                        Debug.Log.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LastActionErrors = new List<string>() { ex.Message };
                Debug.Log.Error(ex);
            }

            return null;
        }

        private T MakeRequest<T>(string url, object data = null, string method = "POST")
            where T : class
        {
            return Task.Run<T>((Func<Task<T>>)(async () =>await MakeRequestAsync<T>(url, CancellationToken.None, data, method).ConfigureAwait(false))).Result;
        }

        private async Task<T> MakeRequestAsync<T>(string url, CancellationToken cancellationToken, object data = null, string method = "POST")
            where T : class
        {
            if (IsLimitedExceeded)
            {
                LastActionErrors = new List<string>() { "Лимит запросов к API исчерпан" };
                return null;
            }
            LastActionErrors = null;

            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return default;
                
                var request = await CreateRequestAsync(url, data, method, cancellationToken, 10_000).ConfigureAwait(false);

                using(cancellationToken.Register(request.Abort))
                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var serializerSettings = new JsonSerializerSettings
                            {
                                ContractResolver = new DefaultContractResolver()
                                {
                                    NamingStrategy = new SnakeCaseNamingStrategy()
                                }
                            };
#if DEBUG
                            var responseContent = "";
                            using (var reader = new StreamReader(stream))
                                responseContent = await (cancellationToken.CancelIfRequestedAsync<string>() ?? reader.ReadToEndAsync()).ConfigureAwait(false);
                            return cancellationToken.IsCancellationRequested ? null : JsonConvert.DeserializeObject<T>(responseContent, serializerSettings);
#endif
#if !DEBUG
                            using (StreamReader sr = new StreamReader(stream))
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = JsonSerializer.Create(serializerSettings);

                                // read the json from a stream
                                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                return cancellationToken.IsCancellationRequested ? null : serializer.Deserialize<T>(reader);
                            }
#endif
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
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
                                        var error = await (cancellationToken.CancelIfRequestedAsync<string>() ?? reader.ReadToEndAsync()).ConfigureAwait(false);
                                        if (data != null)
                                        {
                                            var serializerSettings = new JsonSerializerSettings
                                            {
                                                ContractResolver = new DefaultContractResolver()
                                                {
                                                    NamingStrategy = new SnakeCaseNamingStrategy()
                                                }
                                            };
                                            error += " data:" + JsonConvert.SerializeObject(data, serializerSettings);
                                        }
                                        LastActionErrors = new List<string>()
                                        {
                                            string.IsNullOrEmpty(error) ? ex.Message : error
                                        };

                                        if (string.IsNullOrEmpty(error))
                                            Debug.Log.Warn(ex);
                                        else
                                        {
                                            if (error.Contains("Token requests limit exceeded"))
                                                IsLimitedExceeded = true;
                                            Debug.Log.Warn(error, ex);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    LastActionErrors = new List<string>() { ex.Message };
                                    Debug.Log.Warn(ex);
                                }
                            }
                            else
                            {
                                LastActionErrors = new List<string>() { ex.Message };
                                Debug.Log.Warn(ex);
                            }
                    }
                    else
                    {
                        LastActionErrors = new List<string>() { ex.Message };
                        Debug.Log.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LastActionErrors = new List<string>() { ex.Message };
                Debug.Log.Error(ex);
            }

            return null;
        }

        private async Task<HttpWebRequest> CreateRequestAsync(string url, object data, string method,
            CancellationToken cancellationToken, int? timeoutInMilliseconds = null)
        {
            var request = WebRequest.CreateHttp(BaseUrl + url);
            using (cancellationToken.Register(request.Abort))
            {
                if (timeoutInMilliseconds.HasValue)
                    request.Timeout = timeoutInMilliseconds.Value;
                request.Method = method;
                request.ContentType = "application/json;charset=UTF-8";

                if (!string.IsNullOrEmpty(_accessToken))
                    request.Headers.Add("Authorization", string.Format("AccessToken {0}", _accessToken));

                if (!string.IsNullOrEmpty(_login))
                    request.Headers.Add("X-User-Authorization",
                        string.Format("Basic {0}",
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _login, _password)))));

                if (data != null)
                {
                    var serializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        }
                    };
#if DEBUG
                    string dataPost = JsonConvert.SerializeObject(data, serializerSettings);

                    byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        await requestStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
                    }
#endif
#if !DEBUG
                    using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        using (StreamWriter writer = new StreamWriter(requestStream))
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                        {
                            JsonSerializer serializer = JsonSerializer.Create(serializerSettings);
                            serializer.Serialize(jsonWriter, data);
                            await jsonWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
#endif
                }
            }
            return request;
        }

        #endregion
    }
}
