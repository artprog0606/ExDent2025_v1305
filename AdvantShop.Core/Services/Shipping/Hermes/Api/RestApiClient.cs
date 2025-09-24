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
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.Hermes.Api
{
    public class RestApiClient
    {
        //private const string BaseUrl = "https://ci-delivery-api-test.hermesrussia.ru";
        //ToDo:
        private const string BaseUrl = "https://ci-delivery-api.hermesrussia.ru";

        private readonly string _securedToken;
        private readonly string _publicToken;
        public List<string> LastActionErrors { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        public RestApiClient(string securedToken, string publicToken)
        {
            //if (string.IsNullOrEmpty(token))
            //    throw new ArgumentNullException("token");

            _securedToken = securedToken;
            _publicToken = publicToken;

            Initialize();
        }

        public GetBusinessUnitsResponse GetBusinessUnits()
        {
            return MakeRequest<GetBusinessUnitsResponse>("/business-unit/get", new GetBusinessUnitsParams());
        }

        public GetDistributionCentersResponse GetDistributionCenters(string businessUnitCode)
        {
            return MakeRequest<GetDistributionCentersResponse>("/dc/get-direct-delivery-by-code", new GetDistributionCentersParams() { UnitClientCode = businessUnitCode });
        }

        public GetParcelShopsResponse GetParcelShops(string businessUnitCode)
        {
            return MakeRequest<GetParcelShopsResponse>("/parcel-shop/get-by-business-unit", new GetParcelShopsParams() { BusinessUnitCode = businessUnitCode });
        }

        public GetPriceCalculationByPointResponse GetPriceCalculationByPoint(GetPriceCalculationByPointParams param)
        {
            return MakeRequest<GetPriceCalculationByPointResponse>("/calculation/calculate-delivery-price-by-points", param);
        }

        /// <summary>
        /// Создание предзаказа с возвратом сопроводительных документов с доставкой от
        /// склада клиента до пункта выдачи заказов для бизнес-юнитов с услугой
        /// DIRECT_DELIVERY, DIRECT_RETURN, ACCOMPANYING_DOCUMENTS_RETURN
        /// </summary>
        /// Если предзаказ не подтвержден методом confirmed, он будет заменен по
        /// ClientParcelNumber
        /// Если включена автогенерация баркода, parcelBarcode будет заменен
        /// счетчиком,
        /// Если автогенерцаия выключена, нужно указать корректное регулярное
        /// выражение ШК клиента
        public CreateVsdParcelResponse CreateVsdParcel(CreateVsdParcelParams parcel)
        {
            return MakeRequest<CreateVsdParcelResponse>("/parcel/upsert-vsd-parcel", parcel);
        }

        /// <summary>
        /// Создание стандартного предзаказа с доставкой от склада клиента до пункта
        /// выдачи заказов для бизнес-юнитов с услугой DIRECT_DELIVERY, DIRECT_RETURN
        /// </summary>
        /// Если предзаказ не подтвержден методом confirmed, он будет заменен по
        /// ClientParcelNumber
        /// Если включена автогенерация баркода, parcelBarcode будет заменен
        /// счетчиком,
        /// Если автогенерцаия выключена, нужно указать корректное регулярное
        /// выражение ШК клиента
        public CreateStandardParcelResponse CreateStandardParcel(CreateStandardParcelParams parcel)
        {
            return MakeRequest<CreateStandardParcelResponse>("/parcel/upsert-standard-parcel", parcel);
        }

        /// <summary>
        /// Создание предзаказа с доставкой силами клиента до пункта выдачи заказов
        /// для бизнес-юнитов с услугой DROP_OFF_TO_TARGET_PARCELSHOP
        /// </summary>
        /// Если предзаказ не подтвержден методом confirmed, он будет заменен по
        /// ClientParcelNumber
        /// Если включена автогенерация баркода, parcelBarcode будет заменен
        /// счетчиком,
        /// Если автогенерцаия выключена, нужно указать корректное регулярное
        /// выражение ШК клиента
        public CreateDropParcelResponse CreateDropParcel(CreateDropParcelParams parcel)
        {
            return MakeRequest<CreateDropParcelResponse>("/parcel/upsert-drop-parcel", parcel);
        }

        /// <summary>
        /// Создание предзаказа с курьерской доставкой от склада клиента на домашний
        /// адрес получателя для бизнес-юнитов с услугой DOOR_DELIVERY
        /// </summary>
        /// Если предзаказ не подтвержден методом confirmed, он будет заменен по
        /// ClientParcelNumber
        /// Если включена автогенерация баркода, parcelBarcode будет заменен
        /// счетчиком,
        /// Если автогенерцаия выключена, нужно указать корректное регулярное
        /// выражение ШК клиента
        public CreateHomeCourierParcelResponse CreateHomeCourierParcel(CreateHomeCourierParcelParams parcel)
        {
            return MakeRequest<CreateHomeCourierParcelResponse>("/parcel/upsert-home-courier-parcel", parcel);
        }

        /// <summary>
        /// Создание и изменение контента посылки
        /// Create, update and delete parcel content
        /// </summary>
        /// Создание и изменение контента посылки. Контент можно обновить пока посылка
        /// не выдана.
        public CreateOrUpdateParcelContentResponse CreateOrUpdateParcelContent(CreateOrUpdateParcelContentParams content)
        {
            return MakeRequest<CreateOrUpdateParcelContentResponse>("/api/parcel-content/upsert", content);
        }

        /// <summary>
        /// Удаление предзаказа по штрихкоду
        /// </summary>
        public DeleteParcelResponse DeleteParcel(string parcelBarcode)
        {
            return MakeRequest<DeleteParcelResponse>("/parcel/delete-preadvices", new DeleteParcelParams { Barcode = parcelBarcode });
        }

        /// <summary>
        /// Получение списка статусов по баркоду
        /// </summary>
        /// Список статусов по баркоду, исключая статусы C2B отправлений (статусы
        /// клиентского возврата)
        public GetStatusesParcelResponse GetStatusesParcel(string parcelBarcode)
        {
            return MakeRequest<GetStatusesParcelResponse>("/statuses/get-by-barcode", new GetStatusesParcelParams { Barcode = parcelBarcode });
        }

        #region PrivateMethods

        private void Initialize()
        {
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            DeserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        private T MakeRequest<T>(string url, object data = null, string method = "POST")
            where T : BaseResponse
        {
            return Task.Run<T>((Func<Task<T>>)(async () => await MakeRequestAsync<T>(url, data, method).ConfigureAwait(false))).Result;
        }

        private async Task<T> MakeRequestAsync<T>(string url, object data = null, string method = "POST")
            where T : BaseResponse
        {
            LastActionErrors = null;

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
                            var result = await DeserializeObjectAsync<T>(stream).ConfigureAwait(false);
                            if (result != null)
                            {
                                BaseResponse baseResponse = (BaseResponse)result;
                                if (baseResponse.IsSuccess == false)
                                    Debug.Log.Warn(string.Format("Hermes ErrorCode: {0}, ErrorMessage: {1}", baseResponse.ErrorCode, baseResponse.ErrorMessage));
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
                                    HttpStatusCode statusCode = eResponse.StatusCode;

                                    //if ((int)statusCode == 400 || (int)statusCode == 401 || (int)statusCode == 500)
                                    //{
                                    //    var result = DeserializeObjectAsync<T>(eStream).ConfigureAwait(false).GetAwaiter().GetResult();

                                    //    if (result != null)
                                    //    {
                                    //        BaseResponse baseResponse = (BaseResponse)result;
                                    //        if (baseResponse.IsSuccess == false)
                                    //            Debug.Log.Warn(string.Format("Hermes ErrorCode: {0}, ErrorMessage: {1}", baseResponse.ErrorCode, baseResponse.ErrorMessage));
                                    //    }

                                    //    return result;
                                    //}

                                    using (var reader = new StreamReader(eStream))
                                    {
                                        var error = reader.ReadToEnd();
                                        if (data != null)
                                        {
                                            error += " data:" + JsonConvert.SerializeObject(data, SerializationSettings);
                                        }
                                        LastActionErrors = new List<string>()
                                        {
                                            string.IsNullOrEmpty(error) ? ex.Message : error
                                        };

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
            var request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json-patch+json; charset=utf-8";

            if (data != null)
            {
                if (data.GetType().IsSubclassOf(typeof(ParamsWithSecuredToken)))
                    ((ParamsWithSecuredToken)data).SecuredToken = _securedToken;
                if (data.GetType().IsSubclassOf(typeof(ParamsWithPublicToken)))
                    ((ParamsWithPublicToken)data).PublicToken = _publicToken;

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
