using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Payment.Mokka.Api
{
    public class MokkaApiService
    {
        // Doc: https://revotechnology.github.io/api-factoring

        private const string SandboxBaseUrl = "https://backend.demo.revoup.ru/";
        private const string BaseUrl = "https://r.revo.ru/";
        
        private readonly string _storeId;
        private readonly string _secretKey;
        private readonly bool _sandbox;

        public MokkaApiService(string storeId, string secretKey, bool sandbox)
        {
            _storeId = storeId;
            _secretKey = secretKey;
            _sandbox = sandbox;

            Initialize();
        }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        public T DeserializeObject<T>(string payload) =>
            JsonConvert.DeserializeObject<T>(payload, DeserializationSettings);
  
        public RegistrationResponse Registration(RegistrationParameters registrationParameters)
        {
            return MakeRequest<RegistrationResponse, RegistrationParameters>("factoring/v1/limit/auth", registrationParameters);
        }
  
        public CheckoutResponse Checkout(CheckoutParameters checkoutParameters)
        {
            return MakeRequest<CheckoutResponse, CheckoutParameters>("factoring/v1/precheck/auth", checkoutParameters);
        }
  
        public ScheduleResponse Schedule(ScheduleParameters scheduleParameters)
        {
            return MakeRequest<ScheduleResponse, ScheduleParameters>("factoring/v1/schedule", scheduleParameters);
        }
  
        public StatusResponse GetStatus(StatusParameters statusParameters)
        {
            return MakeRequest<StatusResponse, StatusParameters>("factoring/v1/status", statusParameters);
        }
  
        public CancelResponse Cancel(CancelParameters cancelParameters)
        {
            return MakeRequest<CancelResponse, CancelParameters>("factoring/v1/precheck/cancel", cancelParameters);
        }
  
        public FinishResponse Finish(FinishParameters finishParameters)
        {
            return MakeRequest<FinishResponse, FinishParameters>("factoring/v1/precheck/finish", finishParameters);
        }
  
        public FinishResponse Finish(FinishParameters finishParameters, Stream checkStream)
        {
            var boundary = Guid.NewGuid().ToString();
            var form = new MokkaMultipartFormDataContent(boundary);
            string bodyJson =
                finishParameters != null
                    ? JsonConvert.SerializeObject(finishParameters, SerializationSettings)
                    : null;

            form.Add(new StringContent(bodyJson, Encoding.UTF8), "body");
            form.Add(new StreamContent(checkStream), "check", "bill.pdf");

            var signature = (bodyJson + _secretKey).Sha1(false, Encoding.UTF8);
            
            return MakeRequestMultipart<FinishResponse>("factoring/v1/precheck/finish", signature, boundary, form);
        }
   
        public ReturnResponse Return(ReturnParameters returnParameters)
        {
            return MakeRequest<ReturnResponse, ReturnParameters>("factoring/v1/return", returnParameters);
        }
   
        public LimitResponse Limit(LimitParameters limitParameters)
        {
            return MakeRequest<LimitResponse, LimitParameters>("api/external/v1/client/limit", limitParameters);
        }
 
        #region Private methods

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
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
            DeserializationSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }
        private TResult MakeRequestMultipart<TResult>(string url, string signature, string boundary, MokkaMultipartFormDataContent content = null)
            where TResult : BaseResponse
        {
            try
            {
                var query = string.Format("?store_id={0}&signature={1}",
                    HttpUtility.UrlEncode(_storeId),
                    HttpUtility.UrlEncode(signature));
                
                var request = WebRequest.Create((_sandbox ? SandboxBaseUrl : BaseUrl) + url + query) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";
                request.ContentType = $"multipart/form-data; boundary=\"{boundary}\"";

                if (content != null)
                {
                    using (var requestStream = request.GetRequestStream())
                    {
                        content.SerializeToStream(requestStream, null);
                    }
                }

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                TResult result;
#if DEBUG
                                var responseContent = "";
                                responseContent = reader.ReadToEnd();
                                result = DeserializeObject<TResult>(responseContent);
#endif
#if !DEBUG
                                using (JsonReader jsonReader = new JsonTextReader(reader))
                                {
                                    JsonSerializer serializer = JsonSerializer.Create(DeserializationSettings);

                                    // read the json from a stream
                                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                    result = serializer.Deserialize<TResult>(jsonReader);
                                }
#endif
                                if (result != null && result.Status != 0)
                                    Debug.Log.Warn($"Mokka error[{result.Status}]: {result.Message}");
                                
                                return result;
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
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Warn(error, ex);
                                }
                            else
                                Debug.Log.Warn(ex);
                    }
                    else
                        Debug.Log.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        private TResult MakeRequest<TResult, TData>(string url, TData data = null)
            where TResult : BaseResponse
            where TData : class
        {
            try
            {
                string dataPost =
                    data != null
                        ? JsonConvert.SerializeObject(data, SerializationSettings)
                        : null;
                
                var query = string.Format("?store_id={0}&signature={1}",
                    HttpUtility.UrlEncode(_storeId),
                    dataPost.IsNotEmpty() 
                        ? HttpUtility.UrlEncode(
                            // signature
                            (dataPost + _secretKey).Sha1(false, Encoding.UTF8))
                        : null);
                
                var request = WebRequest.Create((_sandbox ? SandboxBaseUrl : BaseUrl) + url + query) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (data != null)
                {
                    using (var requestStream = request.GetRequestStream())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                        //request.ContentLength = bytes.Length;

                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                TResult result;
#if DEBUG
                                var responseContent = "";
                                responseContent = reader.ReadToEnd();
                                result = DeserializeObject<TResult>(responseContent);
#endif
#if !DEBUG
                                using (JsonReader jsonReader = new JsonTextReader(reader))
                                {
                                    JsonSerializer serializer = JsonSerializer.Create(DeserializationSettings);

                                    // read the json from a stream
                                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                    result = serializer.Deserialize<TResult>(jsonReader);
                                }
#endif
                                if (result != null && result.Status != 0)
                                    Debug.Log.Warn($"Mokka error[{result.Status}]: {result.Message}");
                                
                                return result;
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
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Warn(error, ex);
                                }
                            else
                                Debug.Log.Warn(ex);
                    }
                    else
                        Debug.Log.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        #endregion
    }
}