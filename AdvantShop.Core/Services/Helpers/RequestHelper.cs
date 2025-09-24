using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AdvantShop.Core.Services.Helpers
{
    public enum ERequestMethod
    {
        [StringName("POST")]
        POST,
        [StringName("GET")]
        GET,
        [StringName("PUT")]
        PUT,
        [StringName("DELETE")]
        DELETE,
    }

    public enum ERequestContentType
    {
        [StringName("application/json")]
        TextJson,

        [StringName("application/json; charset=utf-8")]
        TextJsonUtf8,

        [StringName("application/x-www-form-urlencoded")]
        FormUrlencoded
    }

    public class RequestHelper
    {
        public static T MakeRequest<T>(string urlAction, object data = null,
            Dictionary<string, string> headers = null,
            ERequestMethod method = ERequestMethod.POST,
            ERequestContentType contentType = ERequestContentType.TextJson,
            int timeoutSeconds = 60,
            JsonSerializerSettings jsonSettings = null) //where T : class
        {
            try
            {
                if (data != null && method == ERequestMethod.GET)
                {
                    urlAction += (urlAction.Contains("?") ? "&" : "?") + data.ToString();
                }

                var request = WebRequest.Create(urlAction) as HttpWebRequest;
                request.Timeout = timeoutSeconds * 1000;
                request.Method = method.StrName();
                if (headers != null)
                {
                    if (headers.ContainsKey("Accept"))
                    {
                        request.Accept = headers["Accept"];
                        headers.Remove("Accept");
                    }

                    foreach (var key in headers.Keys)
                    {
                        request.Headers[key] = headers[key];
                    }
                }
                request.ContentType = contentType.StrName();

                if (data != null && (method == ERequestMethod.POST || method == ERequestMethod.PUT))
                {
                    var stringData = contentType == ERequestContentType.FormUrlencoded && data.GetType() == typeof(string)
                        ? data.ToString()
                        : JsonConvert.SerializeObject(data, jsonSettings);

                    byte[] bytes = Encoding.UTF8.GetBytes(stringData);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }


                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }
                }

                var dataAnswer = typeof(T) != typeof(string) ? JsonConvert.DeserializeObject<T>(responseContent, jsonSettings) : (T)Convert.ChangeType(responseContent, typeof(T));
                return dataAnswer;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                        {
                            if (eStream == null) throw ex;

                            using (var reader = new StreamReader(eStream))
                            {
                                var error = reader.ReadToEnd();
                                var wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                                if (wRespStatusCode == HttpStatusCode.BadRequest)
                                {
                                    throw new BlException(error);
                                }
                                throw new Exception(error);
                            }
                        }
                    }
                    throw ex;
                }
            }
        }
    }
}
