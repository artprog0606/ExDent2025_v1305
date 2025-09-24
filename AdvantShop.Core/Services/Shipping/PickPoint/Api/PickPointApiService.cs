//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvantShop.Core.Caching;
using Newtonsoft.Json;
using Debug = AdvantShop.Diagnostics.Debug;

namespace AdvantShop.Core.Services.Shipping.PickPoint.Api
{
    public class PickPointApiService
    {
        private const int MillisecondsBetweenRequests = 0; // время в миллисекундах между запросами
        ////ToDo:Поменять на релиз
        //private const string BaseUrl = "https://e-solution.pickpoint.ru/apitest/";
        private const string BaseUrl = "https://e-solution.pickpoint.ru/api/";

        private static DateTime _lastRequestApi = DateTime.UtcNow.AddHours(-1);
        private static readonly ConcurrentDictionary<string, string> SessionIds = new ConcurrentDictionary<string, string>();

        private readonly string _login;
        private readonly string _password;

        protected string KeySessionId { get; private set; }

        public List<string> LastActionErrors { get; set; }

        public PickPointApiService(string login, string password)
        {
            _login = login;
            _password = password;

            KeySessionId = string.Format("{0}#{1}", _login, _password);
        }

        public AuthenticationResponse Authentication(string ikn)
        {
            return MakeRequest<AuthenticationParams, AuthenticationResponse>("authentication",
                new AuthenticationParams() { Login = _login, Password = _password, Ikn = ikn });
        }

        /// <summary>
        /// Регистрация отправлений
        /// </summary>
        public CreateShipmentResponse CreateShipment(CreateShipmentParams createShipmentParams)
        {
            return MakeRequest<CreateShipmentParams, CreateShipmentResponse>("CreateShipment", createShipmentParams);
        }

        /// <summary>
        /// Удаление отправления
        /// </summary>
        public CancelInvoiceResponse CancelInvoice(CancelInvoiceParams cancelInvoice)
        {
            return MakeRequest<CancelInvoiceParams, CancelInvoiceResponse>("cancelInvoice", cancelInvoice);
        }

        /// <summary>
        /// Отмена заказа
        /// </summary>
        public CancelInvoiceResponse RejectInvoice(CancelInvoiceParams cancelInvoice)
        {
            return MakeRequest<CancelInvoiceParams, CancelInvoiceResponse>("rejectInvoice", cancelInvoice);
        }

        /// <summary>
        /// Расчет тарифа
        /// </summary>
        public CalcTariffResponse Calc(CalcTariffParams calcTariff)
        {
            return MakeRequest<CalcTariffParams, CalcTariffResponse>("calctariffcms", calcTariff);
        }

        /// <summary>
        /// Получение списка городов
        /// </summary>
        public List<CityFromList> GetCities()
        {
            return CacheManager.Get("PickPoint-Cities", 30,
                () => MakeRequest<object, List<CityFromList>>("citylist",
                null, "GET"));
        }

        /// <summary>
        /// Получение списка терминалов по номеру контракта
        /// </summary>
        public List<ClientPostamat> GetPostamats(string ikn)
        {
            return MakeRequest<ClientPostamatListParams, List<ClientPostamat>>("clientpostamatlist",
                new ClientPostamatListParams { Ikn = ikn });
        }

        /// <summary>
        /// Получение справочника статусов отправления
        /// </summary>
        public List<StateFromList> GetStatuses()
        {
            return CacheManager.Get("PickPoint-getstates", 5,
                () => MakeRequest<object, List<StateFromList>>("getstates", null, "GET"));
        }

        /// <summary>
        /// Получение списка вложимых отправлений за заданный период со всеми прошедшими статусами
        /// </summary>
        public List<InvoiceChangeState> GetInvoicesChangeState(DateTime dateFrom, DateTime dateTo)
        {
            return MakeRequest<GetInvoicesChangeStateParams, List<InvoiceChangeState>>("getInvoicesChangeState",
                new GetInvoicesChangeStateParams
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                });
        }

        /// <summary>
        /// Получение информации по зонам
        /// <para>Если поле <paramref name="toPostamatNumber"/> не указано, возвращается список зон по всем пунктам выдачи.</para>
        /// </summary>
        /// <param name="ikn">Номер контракта</param>
        /// <param name="fromCity">Город отправитель груза</param>
        /// <param name="toPostamatNumber">Номер пункта выдачи</param>
        /// <returns></returns>
        public List<Zone> GetZones(string ikn, string fromCity, string toPostamatNumber = null)
        {
            var result = MakeRequest<GetZoneParams, GetZoneResponse>("getzone",
                new GetZoneParams {Ikn = ikn, FromCity = fromCity, ToPostamatNumber = toPostamatNumber});
            return result.ErrorCode == 0 ? result.Zones : null;
        }

        public string GetMakeLabel(string invoiceNumber, string path)
        {
            string filePath = null;

            using (var responseStream = MakeRequestGetStream("makelabel", new MakeLabelParams { Invoices = new List<string> { invoiceNumber } }))
            {
                if (responseStream != null)
                {
                    filePath = Path.Combine(path, string.Format("{0}.pdf", GetValidFileName("PickPointMakeLabel-" + invoiceNumber)));

                    using (var filestream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        responseStream.CopyTo(filestream);
                }
            }

            return filePath;
        }

        public string GetMakeZebraLabel(string invoiceNumber, string path)
        {
            string filePath = null;

            using (var responseStream = MakeRequestGetStream("makeZLabel", new MakeZebraLabelParams { Invoices = new List<string> { invoiceNumber } }))
            {
                if (responseStream != null)
                {
                    filePath = Path.Combine(path, string.Format("{0}.pdf", GetValidFileName("PickPointMakeZebraLabel-" + invoiceNumber)));

                    using (var filestream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        responseStream.CopyTo(filestream);
                }
            }

            return filePath;
        }

        #region PrivateMethods

        private string GetValidFileName(string filename)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex regex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

            return regex.Replace(filename, "-");
        }

        private async Task<LoginResponse> LoginAsync()
        {
            return
                await MakeRequestAsync<LoginParams, LoginResponse>("login",
                    new LoginParams() { Login = _login, Password = _password }).ConfigureAwait(false);
        }

        private async Task<string> GetSessionIdAsync()
        {
            if (!SessionIds.ContainsKey(KeySessionId))
            {
                var loginResponse = await LoginAsync().ConfigureAwait(false);
                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.SessionId))
                    return null;

                SessionIds.TryAdd(KeySessionId, loginResponse.SessionId);
            }

            return SessionIds[KeySessionId];
        }

        private Stream MakeRequestGetStream(string url, object data = null, string method = "POST")
        {
            LastActionErrors = null;

            try
            {
                var request = CreateRequestAsync(url, data, method).ConfigureAwait(false).GetAwaiter().GetResult();

                var response = request.GetResponse();

                return response.GetResponseStream();
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
                                            Debug.Log.Warn(ex);
                                        else
                                            Debug.Log.Warn(error, ex);
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

        private TR MakeRequest<TD, TR>(string url, TD data = null, string requestMethod = "POST")
            where TD : class, new()
            where TR : class, new()
        {
            return Task.Run<TR>((Func<Task<TR>>)(async () => await MakeRequestAsync<TD, TR>(url, data, requestMethod).ConfigureAwait(false))).Result;
        }

        private async Task<TR> MakeRequestAsync<TD, TR>(string url, TD data = null, string requestMethod = "POST", bool noRecallAtExpiredSession = false)
            where TD : class, new()
            where TR : class, new()
        {
            LastActionErrors = null;

            try
            {
                var request = await CreateRequestAsync(url, data, requestMethod).ConfigureAwait(false);
                TR result = null;

                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
#if DEBUG
                            var responseContent = "";
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = await reader.ReadToEndAsync().ConfigureAwait(false);
                            }
                            result = JsonConvert.DeserializeObject<TR>(responseContent);
#endif
#if !DEBUG
                            using (StreamReader sr = new StreamReader(stream))
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = new JsonSerializer();

                                // read the json from a stream
                                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                result = serializer.Deserialize<TR>(reader);
                            }
#endif
                        }
                    }
                }

                if (result != null)
                {
                    var typeResult = typeof(TR);
                    var listErrorCodes = HasAttributeProperties<PickPointErrorCodeAttribute>(typeResult)
                        ? GetValuesProperties<int, PickPointErrorCodeAttribute>(result)
                        : null;
                    var listErrorMessagess = HasAttributeProperties<PickPointErrorMessageAttribute>(typeResult)
                        ? GetValuesProperties<string, PickPointErrorMessageAttribute>(result)
                            .Where(message => !string.IsNullOrEmpty(message)).ToList()
                        : null;

                    if (listErrorMessagess != null && listErrorMessagess.Count > 0)
                    {
                        LastActionErrors = listErrorMessagess;
                        listErrorMessagess.ForEach(Debug.Log.Warn);
                    }

                    if (listErrorCodes != null && !noRecallAtExpiredSession && listErrorCodes.Contains(-2014))
                    {
                        if (SessionIds.ContainsKey(KeySessionId))
                        {
                            SessionIds.TryRemove(KeySessionId, out string @out);
                            result = await MakeRequestAsync<TD, TR>(url, data, requestMethod, noRecallAtExpiredSession: true).ConfigureAwait(false);
                        }
                    }
                }

                return result;
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
                                            Debug.Log.Warn(ex);
                                        else
                                            Debug.Log.Warn(error, ex);
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

        private async Task<HttpWebRequest> CreateRequestAsync(string url, object data, string method)
        {
            TimeSpan time = DateTime.UtcNow - _lastRequestApi;
            if (time.TotalMilliseconds < MillisecondsBetweenRequests)
                await Task.Delay(MillisecondsBetweenRequests - (int)time.TotalMilliseconds);

            var request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json";

            if (data != null)
            {
                if (data.GetType().IsSubclassOf(typeof(ObjectOfSession)))
                {
                    var sessionId = await GetSessionIdAsync().ConfigureAwait(false);
                    if (string.IsNullOrEmpty(sessionId))
                        throw new Exception("Неудалось получить идентификатор сессии");

                    ((ObjectOfSession)data).SessionId = sessionId;
                }

#if DEBUG
                string dataPost = JsonConvert.SerializeObject(data);

                byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                request.ContentLength = bytes.Length;

                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                }
#endif
#if !DEBUG
                using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    using (StreamWriter writer = new StreamWriter(requestStream))
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jsonWriter, data);
                        await jsonWriter.FlushAsync().ConfigureAwait(false);
                    }
                }
#endif
            }

            _lastRequestApi = DateTime.UtcNow;

            return request;
        }

        private static readonly Type IEnumerableType = typeof(IEnumerable);

        private static List<T> GetValuesProperties<T, TA>(object obj)
        where T : IConvertible
        where TA : Attribute
        {
            var list = new List<T>();
            if (obj == null)
                return list;

            var type = obj.GetType();
            if (type.IsPrimitive || type == typeof(string) || !type.IsClass)
                return list;

            var typeAttribute = typeof(TA);
            var typeValue = typeof(T);

            if (!type.GetInterfaces().Contains(IEnumerableType))
            { 
                var properties = type.GetProperties();

                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.IsDefined(typeAttribute))
                    {
                        var propertyType = propertyInfo.PropertyType;
                        if (!typeValue.IsGenericType && propertyType.IsGenericType &&
                            propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                            propertyInfo.GetValue(obj) != null)
                        {
                            propertyType = Nullable.GetUnderlyingType(propertyType);
                        }

                        if (propertyType == typeValue)
                            list.Add((T)((IConvertible)propertyInfo.GetValue(obj)));
                    }
                    else if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(string))
                    {
                        if (propertyInfo.PropertyType.GetInterfaces().Contains(IEnumerableType) ||
                            propertyInfo.PropertyType.IsClass)
                        {
                            list.AddRange(GetValuesProperties<T, TA>(propertyInfo.GetValue(obj)));
                        }
                    }
                }
            }
            else
            {
                var values = (IEnumerable)obj;

                foreach (var value in values)
                    list.AddRange(GetValuesProperties<T, TA>(value));
            }

            return list;
        }

        private static bool HasAttributeProperties<TA>(Type type)
            where TA : Attribute
        {
            if (type.IsPrimitive || type == typeof(string) || !type.IsClass)
                return false;

            var typeAttribute = typeof(TA);

            var iEnumerableType =
                type.GetInterfaces()
                    .Where(t => t.IsGenericType
                                && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(t => t.GetGenericArguments()[0])
                    .FirstOrDefault();

            if (iEnumerableType == null)
            {
                var properties = type.GetProperties();

                if (properties.Any(propertyInfo => propertyInfo.IsDefined(typeAttribute)))
                    return true;

                foreach (var propertyInfo in properties)
                {
                    if (HasAttributeProperties<TA>(propertyInfo.PropertyType))
                        return true;
                }
            }
            else
            {
                return HasAttributeProperties<TA>(iEnumerableType);
            }

            return false;
        }
#endregion
    }
}
