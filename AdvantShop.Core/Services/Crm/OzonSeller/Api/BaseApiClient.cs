using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    public abstract class BaseApiClient
    {
        private const string ApiUrl = "https://cb-api.ozonru.me";
        //private const string ApiUrl = "https://api-seller.ozon.ru";
        private const int MillisecondsBetweenRequests = 0; // нет ограничений на кол-во запросов
        private static DateTime _lastRequestApi = DateTime.UtcNow.AddHours(-1);

        protected readonly string clientId;
        protected readonly string apiKey;

        public List<string> LastActionErrors { get; set; }
        protected bool LastActionIsBadGateway;

        public BaseApiClient(string clientId, string apiKey)
        {
            if (clientId == null)
                throw new ArgumentNullException("clientId");
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");

            this.clientId = clientId;
            this.apiKey = apiKey;
        }

        protected TR MakeRequest<TR, TD>(string url, TD data = default(TD), string requestMethod = "POST")
        {
            return System.Threading.Tasks.Task.Run<TR>((Func<Task<TR>>)(async () => await MakeRequestAsync<TR, TD>(url, data, requestMethod))).Result;
        }

        private async Task<TR> MakeRequestAsync<TR, TD>(string url, TD data, string requestMethod = "POST")
        {
            ClearErrors();
            LastActionIsBadGateway = false;

            try
            {
                var request = await CreateRequestAsync(url, data, requestMethod);

                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
#if DEBUG
                            var responseContent = "";
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = await reader.ReadToEndAsync();
                            }

                            return JsonConvert.DeserializeObject<TR>(responseContent,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new DefaultContractResolver
                                    {
                                        NamingStrategy = new SnakeCaseNamingStrategy()
                                    }
                                });
#endif
#if !DEBUG
                            using (StreamReader sr = new StreamReader(stream))
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = new JsonSerializer
                                {
                                    ContractResolver = new DefaultContractResolver
                                    {
                                        NamingStrategy = new SnakeCaseNamingStrategy()
                                    }
                                };
                                // read the json from a stream
                                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                                return serializer.Deserialize<TR>(reader);
                            }
#endif
                        }
                    }
                }

            }
            catch (WebException ex)
            {
                try
                {
                    var result = "";

                    using (var eResponse = (HttpWebResponse)ex.Response)
                    {
                        if (eResponse != null)
                        {
                            LastActionIsBadGateway = eResponse.StatusCode == HttpStatusCode.BadGateway;

                            using (var eStream = eResponse.GetResponseStream())
                            {
                                using (var reader = new StreamReader(eStream))
                                {
                                    result += reader.ReadToEnd();
                                    AddError(string.IsNullOrEmpty(result) ? ex.Message : result);
                                    Debug.Log.Warn("Error on url: " + url + ". " + result);
                                }
                            }
                        }
                    }
                }
                catch (Exception exIn)
                {
                    AddError(exIn.Message);
                    Debug.Log.Warn("Error on url: " + url, exIn);
                }
                AddError(ex.Message);
                Debug.Log.Warn("Error on url: " + url, ex);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                Debug.Log.Warn("Error on url: " + url, ex);
            }

            return default(TR);
        }

        private void AddError(string message)
        {
            if (LastActionErrors == null)
                LastActionErrors = new List<string>();

            LastActionErrors.Add(message);
        }

        private void ClearErrors()
        {
            if (LastActionErrors != null)
                LastActionErrors.Clear();
            LastActionErrors = null;
        }

        private async Task<HttpWebRequest> CreateRequestAsync(string url, object data, string requestMethod)
        {
            if (MillisecondsBetweenRequests > 0)
            {
                TimeSpan time = DateTime.UtcNow - _lastRequestApi;
                if (time.TotalMilliseconds < MillisecondsBetweenRequests)
                    await System.Threading.Tasks.Task.Delay(MillisecondsBetweenRequests - (int)time.TotalMilliseconds);
            }

            var request = WebRequest.Create(ApiUrl + url) as HttpWebRequest;
            request.Method = requestMethod;
            request.ContentType = "application/json";

            if (!string.IsNullOrEmpty(clientId))
                request.Headers.Add("Client-Id", clientId);
            if (!string.IsNullOrEmpty(clientId))
                request.Headers.Add("Api-Key", apiKey);

            if (data != null)
            {
#if DEBUG
                string dataPost = JsonConvert.SerializeObject(data,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        },
                        NullValueHandling = NullValueHandling.Ignore
                    });

                byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                request.ContentLength = bytes.Length;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                }
#endif
#if !DEBUG
                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    using (StreamWriter writer = new StreamWriter(requestStream))
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        JsonSerializer serializer = new JsonSerializer
                        {
                            Formatting = Formatting.None,
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new SnakeCaseNamingStrategy()
                            },
                            NullValueHandling = NullValueHandling.Ignore,
                        };
                        serializer.Serialize(jsonWriter, data);
                        await jsonWriter.FlushAsync();
                    }
                }
#endif
            }

            _lastRequestApi = DateTime.UtcNow;

            return request;
        }
    }
}
