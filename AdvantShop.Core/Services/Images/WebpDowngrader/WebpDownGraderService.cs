using System;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Images.WebpDowngrader
{
    public static class WebpDownGraderService
    {
        private const string MethodPath = "/api/v1/downgrade";
        private const string ErrorMessage = "Error on downgrading webp ";
        private const string AdvantshopIdHeaderKey = "ADVANTSHOP-ID";

        public static bool DetectWebpFromLink(string uri)
        {
            var extension = FileHelpers.GetExtension(uri);
            if (extension == ".webp")
                return true;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if ((response.StatusCode == HttpStatusCode.OK
                         || response.StatusCode == HttpStatusCode.Moved
                         || response.StatusCode == HttpStatusCode.Redirect) &&
                        response.ContentType.EndsWith("webp",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Downgrades webp image to jpeg by external service
        /// </summary>
        /// <param name="uri">absolute url to webp image</param>
        /// <param name="filePath">path where to save image</param>
        /// <returns>true on success</returns>
        public static bool DowngradeImageByUri(string uri, string filePath)
        {
            var serviceUri = SettingsLic.WebpDownGraderServiceUrl;
            if (string.IsNullOrWhiteSpace(serviceUri) ||
                Uri.IsWellFormedUriString(serviceUri, UriKind.Absolute) is false)
            {
                throw new NotSupportedException();
            }

            if (Uri.IsWellFormedUriString(uri, UriKind.Absolute) is false)
            {
                throw new NotSupportedException();
            }

            try
            {
                var basePath = new Uri(serviceUri);
                var request = WebRequest.Create(new Uri(basePath, MethodPath));
                
                var timeoutSettingsString = SettingProvider.GetInternalSetting("WebpDownGraderServiceTimeout");
                var parsed = int.TryParse(timeoutSettingsString, out var timeout);

                request.Timeout = (parsed && timeout > 0 ? timeout : 10) * 1000;
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add(AdvantshopIdHeaderKey, SettingsLic.AdvId);

                var data = JsonConvert.SerializeObject(new RequestDto
                {
                    ImageUri = new Uri(uri)
                });

                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                using (var response = request.GetResponse())
                {
                    FileHelpers.DeleteFile(filePath);

                    var notEmptyFile = false;

                    // if the remote file was found, download it 
                    using (var inputStream = response.GetResponseStream())
                    using (Stream outputStream = File.Create(filePath))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);

                            notEmptyFile |= bytesRead != 0;
                        } while (bytesRead != 0);
                    }

                    return notEmptyFile;
                }
            }
            catch (WebException webException)
            {
                if (webException.Response is HttpWebResponse webExceptionResponse &&
                    webExceptionResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    var responseContent = string.Empty;
                    using (var stream = webExceptionResponse.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }

                    var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);

                    Debug.Log.Error(ErrorMessage + responseDto.Cause, webException);

                    throw new BlException(string.IsNullOrWhiteSpace(responseDto.Cause) 
                        ? "unexpected error on processing webp image" 
                        : responseDto.Cause);
                }
            }
            catch (Exception exception)
            {
                Debug.Log.Error(ErrorMessage, exception);

                throw new BlException(exception.Message);
            }

            return false;
        }
    }
}