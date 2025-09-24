using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.AdminPushNotification
{
    public static class AdminPushNotificationServiceClient
    {
        private class AdminPushNotificationDto
        {
            public string LicKey { get; set; }
            public string Token { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
            public Dictionary<string, string> Data { get; set; }
        }

        private const string BaseUrl = "https://push.advsrvone.pw";

        public static void SendNotification(string token, string title, string body, Dictionary<string, string> data)
        {
            try
            {
                var dto = new AdminPushNotificationDto
                    { LicKey = SettingsLic.AdvId, Token = token, Title = title, Body = body, Data = data };

                var webRequest = (HttpWebRequest)WebRequest.Create(BaseUrl + "/api/v1/push/send");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.Timeout = 1000;

                using (var requestStream = webRequest.GetRequestStream())
                {
                    using (var writer = new StreamWriter(requestStream))
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        var serializer = new JsonSerializer
                        {
                            Formatting = Formatting.None,
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new CamelCaseNamingStrategy()
                            },
                            NullValueHandling = NullValueHandling.Ignore,
                        };
                        serializer.Serialize(jsonWriter, dto);
                        jsonWriter.Flush();
                    }
                }

                using (var response = (HttpWebResponse)webRequest.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Debug.Log.Error("Failed to send push notification for " +
                                        $"token: {token}. " +
                                        $"Status code: {response.StatusCode}. " +
                                        $"Body: {response.GetResponseStream()}.");
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.Log.Error($"Unexpected error occured on sending push notification for token: {token}. ",
                    exception);
            }
        }
    }
}