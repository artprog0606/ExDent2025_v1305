//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.Pec.Api
{
    public class PecApiService
    {
        private const string BaseUrl = "https://kabinet.pecom.ru/api/v1";

        private readonly string _login;
        private readonly string _apiKey;

        public List<string> LastActionErrors { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        public PecApiService(string login, string apiKey)
        {
            //if (string.IsNullOrEmpty(token))
            //    throw new ArgumentNullException("token");

            _login = login;
            _apiKey = apiKey;

            Initialize();
        }

        /// <summary>
        /// Поиск городов и филиалов по названию
        /// </summary>
        /// <param name="title">Название филиала или города</param>
        /// <returns></returns>
        public PecBaseResponse<BranchesFindByTitleResponse> FindBranchesByTitle(string title)
        {
            return MakeRequest<BranchesFindByTitleResponse>(
                "/branches/findbytitle/",
                new BranchesFindByTitleParams { Title = title, Exact = false });
        }

        /// <summary>
        /// Поиск городов и филиалов по названию
        /// </summary>
        /// <param name="title">Название филиала или города</param>
        /// <returns></returns>
        public PecBaseResponse<BranchesFindByIdResponse> FindBranchesById(long id)
        {
            return MakeRequest<BranchesFindByIdResponse>(
                "/branches/findbyid/",
                new BranchesFindByIdParams { Id = id });
        }

        /// <summary>
        /// Расчет стоимости и сроков перевозки
        /// </summary>
        /// <param name="params">Параметры расчета</param>
        /// <returns></returns>
        public PecBaseResponse<CalculateResponse> Calculate(CalculateParams @params)
        {
            return MakeRequest<CalculateResponse>(
                "/calculator/calculateprice/",
                @params);
        }

        /// <summary>
        /// Асинхронный расчет стоимости и сроков перевозки
        /// </summary>
        /// <param name="params">Параметры расчета</param>
        /// <returns></returns>
        public async Task<PecBaseResponse<CalculateResponse>> CalculateAsync(CalculateParams @params)
        {
            return await MakeRequestAsync<CalculateResponse>(
                "/calculator/calculateprice/",
                @params).ConfigureAwait(false);
        }

        /// <summary>
        /// Получение списока филиалов и городов
        /// </summary>
        /// <returns></returns>
        public PecBaseResponse<AllBranchesResponse> GetAllBranches()
        {
            return MakeRequest<AllBranchesResponse>(
                "/branches/all/",
                new object());
        }

        /// <summary>
        /// Асинхронное получение списока филиалов и городов
        /// </summary>
        /// <returns></returns>
        public async Task<PecBaseResponse<AllBranchesResponse>> GetAllBranchesAsync()
        {
            return await MakeRequestAsync<AllBranchesResponse>(
                "/branches/all/",
                new object());
        }

        /// <summary>
        /// Подача заявки на предварительное оформление грузов
        /// </summary>
        /// <returns></returns>
        public PecBaseResponse<PreregistrationSubmitResponse> Preregistration(PreregistrationSubmitParams @params)
        {
            return MakeRequest<PreregistrationSubmitResponse>(
                "/preregistration/submit/",
                @params);
        }

        /// <summary>
        /// Запрос статусов грузов
        /// </summary>
        /// <param name="cargoCodes">Коды грузов. Максимальное количество кодов грузов в одном запросе: 15</param>
        /// <returns></returns>
        public PecBaseResponse<GetCargosStatusesResponse> GetCargosStatuses(params string[] cargoCodes)
        {
            return MakeRequest<GetCargosStatusesResponse>(
                "/cargos/status/",
                new GetCargosStatusesParams { CargoCodes = cargoCodes });
        }

        /// <summary>
        /// История статусов по грузу
        /// </summary>
        /// <param name="cargoCode">Код груза</param>
        /// <returns></returns>
        public PecBaseResponse<List<GetCargoStatusHistoryItem>> GetCargoStatusHistory(string cargoCode)
        {
            return MakeRequest<List<GetCargoStatusHistoryItem>>(
                "/cargos/statushistory/",
                new GetCargoStatusHistoryParams { CargoCode = cargoCode });
        }
        /// <summary>
        /// Запрос статусов грузов
        /// </summary>
        /// <param name="cargoCodes">Коды грузов. Максимальное количество кодов грузов в одном запросе: 15</param>
        /// <returns></returns>
        public PecBaseResponse<List<GetStatusesItem>> GetStatuses()
        {
            return MakeRequest<List<GetStatusesItem>>(
                "/cargos/statustable/",
                new object { });
        }
        /// <summary>
        /// Запрос статусов грузов
        /// </summary>
        /// <param name="cargoCodes">Коды грузов. Максимальное количество кодов грузов в одном запросе: 15</param>
        /// <returns></returns>
        public PecBaseResponse<List<CancellationCargosItem>> CancellationCargos(params string[] cargoCodes)
        {
            return MakeRequest<List<CancellationCargosItem>>(
                "/requests/requestcancellation/",
                cargoCodes);
        }

        #region PrivateMethods

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
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            DeserializationSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        private PecBaseResponse<T> MakeRequest<T>(string url, object data = null, string method = WebRequestMethods.Http.Post)
            where T : class, new()
        {
            return Task.Run<PecBaseResponse<T>>((Func<Task<PecBaseResponse<T>>>)(async () => await MakeRequestAsync<T>(url, data, method).ConfigureAwait(false))).Result;
        }

        private async Task<PecBaseResponse<T>> MakeRequestAsync<T>(string url, object data = null, string method = WebRequestMethods.Http.Post)
            where T : class, new()
        {
            ClearErrors();

            try
            {
                var request = await CreateRequestAsync(url, data, method).ConfigureAwait(false);
                //request.Timeout = 10000;

                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var result = await DeserializeObjectAsync<PecBaseResponse<T>>(stream).ConfigureAwait(false);
                            if (result != null && result.Success == false)
                            {
                                if (result.Error != null)
                                {
                                    Debug.Log.Warn(string.Format("Pec Title: {0}, Message: {1}", result.Error.Title, result.Error.Message));
                                    AddErrors(result.Error.Title);

                                    if (result.Error.Fields != null)
                                        result.Error.Fields.ForEach(x =>
                                        {
                                            Debug.Log.Warn(string.Format("Pec ValidateField Key: {0}, Values: {1}", x.Key, string.Join(", ", x.Value ?? new List<string>())));
                                            AddErrors(string.Format("{0}: {1}", x.Key, string.Join(", ", x.Value ?? new List<string>())));
                                        });
                                }
                            }

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
                                        if (data != null)
                                        {
                                            error += " data:" + JsonConvert.SerializeObject(data, SerializationSettings);
                                        }
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
            var request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json;charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //request.Credentials = new NetworkCredential(_login, _password);
            request.Headers.Add(HttpRequestHeader.Authorization,
                string.Format("Basic {0}",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _login, _apiKey)))));

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


        #endregion
    }
}
