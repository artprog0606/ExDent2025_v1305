using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Handlers.User;
using AdvantShop.Models.User;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AdvantShop.Core.Test
{
    //[TestFixture]
    public class SendSmsTest
    {
        private bool _smsTestingState;
        private bool _authByCodeActiveState;
        

        [OneTimeSetUp]
        public void Init()
        {
            InitConnectionString();

            _smsTestingState = SettingsSms.SmsTesting;
            _authByCodeActiveState = SettingsOAuth.AuthByCodeActive;
            
            SettingsSms.SmsTesting = true;
            SettingsOAuth.AuthByCodeActive = true;
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            SettingsSms.SmsTesting = _smsTestingState;
            SettingsOAuth.AuthByCodeActive = _authByCodeActiveState;
        }
        
        #region Unit tests

        /// <summary>
        /// Проверка что прошло разрешенное время с последнего смс для телефона 
        /// </summary>
        //[Test]
        public void Test_CheckByTimeBetweenSms_UnitTest()
        {
            var phone =  new Random().Next(791700000, 791800000) * 100L;
            var model = new SendCodeModel() {Phone = phone.ToString(), SignUp = true};
            
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var result = new SendCode(model).Execute();
                    Thread.Sleep(1*1000);
                }
                catch (BlException ex)
                {
                    if (i != 0)
                        Assert.IsTrue(ex.Message.Contains("Следующее sms можно будет послать через") ||
                                      ex.Message.Contains("Слишком много sms-запросов. Попробуйте позже."));
                }
            }
        }
        
        /// <summary>
        /// Проверка что не было слишком много попыток. 
        /// </summary>
        //[Test]
        public void Test_CheckByCountBetweenSms_UnitTest()
        {
            var phone =  new Random().Next(791700000, 791800000) * 100L;
            var model = new SendCodeModel() {Phone = phone.ToString(), SignUp = true};
            
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var result = new SendCode(model).Execute();
                    Thread.Sleep(60*1000);
                }
                catch (BlException ex)
                {
                    if (i > 5)
                        Assert.IsTrue(ex.Message.Contains("Слишком много sms-запросов"));
                }
            }
        }
        
        #region help methods
        
        private void InitConnectionString()
        {
            if (ConfigurationManager.ConnectionStrings["AdvantConnectionString"] != null)
                return;

            typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ConfigurationManager.ConnectionStrings, false);
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("AdvantConnectionString", GetConnectionString()));
        }

        private string GetConnectionString()
        {
            return "Data Source='.\\SQLEXPRESS2012'; Connect Timeout='3'; Initial Catalog='AdvantShop_dev'; Persist Security Info='True'; User ID='sa'; Password='ewqEWQ321#@!';";
        }
        
        #endregion

        #endregion

        #region Intagration tests
        
        /*
         * For integration test uncomment and set _baseUrl
         */

        private readonly string _baseUrl = "http://localhost:8825/";
        
        /// <summary>
        /// Проверка что прошло разрешенное время с последнего смс для телефона 
        /// </summary>
        //[Test]
        public void Test_CheckByTimeBetweenSms_IntegrationTest()
        {
            var (token, cookies) = GetRegistrationPage();
            
            long phone =  new Random().Next(791700000, 791800000) * 100L;
            
            for (int i = 0; i < 10; i++)
            {
                var response = SendSms(token, cookies, new {phone, signUp = true});
                Thread.Sleep(1*1000);
                if (i != 0)
                    Assert.IsTrue(response.Contains("Следующее sms можно будет послать через") ||
                                  response.Contains("Слишком много sms-запросов. Попробуйте позже."));
            }
        }
        
        /// <summary>
        /// Проверка что не было слишком много попыток. 
        /// </summary>
        //[Test]
        public void Test_CheckByCountBetweenSms_IntegrationTest()
        {
            var (token, cookies) = GetRegistrationPage();
            
            long phone =  new Random().Next(791700000, 791800000) * 100L;
            
            for (int i = 0; i < 10; i++)
            {
                var response = SendSms(token, cookies, new {phone, signUp = true});
                Thread.Sleep(60*1000);
                if (i > 5)
                    Assert.IsTrue(response.Contains("Слишком много sms-запросов"));
            }
        }
        
        #region help methods
        
        private (string, string) GetRegistrationPage()
        {
            var token = "";
            var cookies = "";
            
            var request = WebRequest.Create(_baseUrl + "registration") as HttpWebRequest;
            request.Method = "GET";
            
            var responseContent = "";
            var cookiesStr = "";
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                    responseContent = reader.ReadToEnd();

                cookiesStr = response.Headers[HttpResponseHeader.SetCookie];
            }

            var regex = new Regex(@"__RequestVerificationToken"" type=""hidden"" value=""(\S*)""");
            var matches = regex.Matches(responseContent);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
                token = matches[0].Groups[1].Value;
            
            var keys = new List<string>() {"f", "customer"};
            var items = 
                cookiesStr
                    .Split("; ")
                    .SelectMany(x => x.Split(','))
                    .Where(x => keys.Contains(x.Split('=')[0]))
                    .ToArray();
            cookies = String.Join("; ", items);

            return (token, cookies);
        }

        private string SendSms(string token, string cookies, object data)
        {
            var request = WebRequest.Create(_baseUrl + "user/sendcode") as HttpWebRequest;
            request.Method = "POST";
            request.Headers.Clear();
            request.Headers.Add("__requestverificationtoken", token);
            request.Headers.Add("Cookie", cookies);

            request.ContentType = "application/json;";
            
            var stringData = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(stringData);
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            
            var responseContent = "";
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                    responseContent = reader.ReadToEnd();
            }

            return responseContent;
        }
        
        #endregion

        #endregion
    }
}