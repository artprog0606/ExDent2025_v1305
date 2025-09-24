using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.Shipping.RussianPost.TrackingApi
{
    public class RussianPostTrackingApiService
    {
        private const string BaseUrl = "https://tracking.russianpost.ru/rtm34";
        private readonly string _login;
        private readonly string _password;

        public List<string> LastActionErrors { get; set; }

        public RussianPostTrackingApiService(string login, string password)
        {
            _login = login;
            _password = password;
        }

        public GetOperationHistoryResponse GetBarcodeHistory(string barcode)
        {
            var data = new RequestContentBase<GetOperationHistoryBody, GetOperationHistory>
            {
                Body = new GetOperationHistoryBody()
                {
                    BodyData = new GetOperationHistory()
                    {
                        OperationHistoryRequest = new OperationHistoryRequest()
                        {
                            Barcode = barcode,
                            MessageType = "0",
                            Language = "RUS"
                        },
                    }
                }
            };

            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add("soap", "http://www.w3.org/2003/05/soap-envelope");
            xmlSerializerNamespaces.Add("oper", "http://russianpost.org/operationhistory");
            xmlSerializerNamespaces.Add("data", "http://russianpost.org/operationhistory/data");
            xmlSerializerNamespaces.Add("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");

            var response =
                MakeRequest<GetOperationHistoryResponse, GetOperationHistoryBody, GetOperationHistory>(data,
                xmlSerializerNamespaces,
                new Dictionary<string, string>() { { "BodyData>", "getOperationHistory>" } });

            return response;
        }


        #region Private Methods

        public T MakeRequest<T, TB, TBD>(RequestContentBase<TB, TBD> data, XmlSerializerNamespaces xmlSerializerNamespaces, Dictionary<string, string> replaceValues)
            where T : class, new()
            where TB : BodyBase<TBD> where TBD : AuthorizationHeader
        {
            LastActionErrors = null;

            object obj;
            T responseObj = default(T);

            try
            {
                var request = CreateRequest(data, xmlSerializerNamespaces, replaceValues);

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
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
                            //var errorResponse = obj as RussianPostErrorResponse;
                            //if (errorResponse != null)
                            //{
                            //    LastActionErrors = errorResponse.Errors;
                            //    Debug.Log.Error(string.Format("RussianPost errors: {0}",
                            //        errorResponse.Errors != null ? string.Join(" ", errorResponse.Errors) : string.Empty));
                            //}
                            //else
                            responseObj = (T)obj;
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
#if !DEBUG
                            // для Release режима десериализуем сразу из потока
                            obj = Deserialize<T>(eStream);
#endif
#if DEBUG
                                // для режима отладки десериализуем так,
                                // чтобы можно было посмотреть ответ сервера
                                string result;
                                using (var reader = new StreamReader(eStream))
                                {
                                    result = reader.ReadToEnd();
                                }
                                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                                {
                                    obj = Deserialize<T>(memoryStream);
                                }
#endif
                                responseObj = (T)obj;
                                //using (var reader = new StreamReader(eStream))
                                //{
                                //    var error = reader.ReadToEnd();
                                //    LastActionErrors = new List<string>() { error };
                                //    Debug.Log.Error(error, ex);
                                //}
                            }
                            else
                                Debug.Log.Error(ex);
                    }
                    else
                        Debug.Log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return responseObj;
        }

        private object Deserialize<T>(Stream stream)
            where T : class, new()
        {
            using (var reader = XmlReader.Create(stream))
            {

                reader.MoveToContent();

                //var isError = reader.Name ==
                //              typeof(RussianPostErrorResponse).GetCustomAttributes<XmlRootAttribute>().First().ElementName;


                //XmlSerializer deserializer = new XmlSerializer(isError ? typeof(RussianPostErrorResponse) : typeof(T));
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                return deserializer.Deserialize(reader);
            }
        }

        private HttpWebRequest CreateRequest<TB, TBD>(RequestContentBase<TB, TBD> data, XmlSerializerNamespaces xmlSerializerNamespaces, Dictionary<string, string> replaceValues)
            where TB : BodyBase<TBD> where TBD : AuthorizationHeader
        {
            data.Body.BodyData.AuthorizationHeaderData = new AuthorizationHeaderData()
            {
                Login = _login,
                Password = _password
            };

            string dataText;
            using (MemoryStream stream = new MemoryStream())
            {
                WriteDataToStream(stream, data, xmlSerializerNamespaces);

                stream.Seek(0, SeekOrigin.Begin);
                dataText = Encoding.UTF8.GetString(stream.ToArray());

                if (replaceValues != null)
                    foreach (var replaceValue in replaceValues)
                        dataText = dataText.Replace(replaceValue.Key, replaceValue.Value);
            }

            var request = WebRequest.Create(BaseUrl) as HttpWebRequest;
            request.Headers.Add("SOAP:Action");
            request.Method = "POST";
            request.ContentType = "application/soap+xml;charset=UTF-8";
            //request.ContentType = "text/xml;charset=\"utf-8\"";
            //request.Accept = "application/xml";
            //request.Accept = "text/xml";

            byte[] bytes = Encoding.UTF8.GetBytes(dataText);
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            return request;
        }

        private void WriteDataToStream<TB, TBD>(Stream stream, RequestContentBase<TB, TBD> data, XmlSerializerNamespaces xmlSerializerNamespaces)
            where TB : BodyBase<TBD> where TBD : AuthorizationHeader
        {
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, CheckCharacters = true, OmitXmlDeclaration = true }))
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());

                if (xmlSerializerNamespaces != null)
                    serializer.Serialize(writer, data, xmlSerializerNamespaces);
                else
                    serializer.Serialize(writer, data);
            }
        }

        #endregion

    }
}
