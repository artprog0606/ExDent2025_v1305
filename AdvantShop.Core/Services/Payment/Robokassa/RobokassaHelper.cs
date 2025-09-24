using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using AdvantShop.Diagnostics;
using AdvantShop.Localization;

namespace AdvantShop.Core.Services.Payment.Robokassa
{
    public class RobokassaHelper
    {

        public static GetCurrenciesResponse GetCurrencies(string merchantLogin, string gatewayCountry)
        {
            GetCurrenciesResponse responseObj = null;

            try
            {
                var currentLanguage = Culture.Language;
                var url =
                    (gatewayCountry == "kz"
                        ? "https://auth.robokassa.kz"
                        : "https://auth.robokassa.ru")
                    + string.Format(
                        "/Merchant/WebService/Service.asmx/GetCurrencies?MerchantLogin={0}&Language={1}",
                        HttpUtility.UrlEncode(merchantLogin),
                        currentLanguage == Culture.SupportLanguage.Russian
                        || currentLanguage == Culture.SupportLanguage.Ukrainian
                            ? "ru"
                            : "en");

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                request.Method = "GET";
                //request.Accept = "application/xml";

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
#if !DEBUG
// для Release режима десериализуем сразу из потока
                            responseObj = (GetCurrenciesResponse)Deserialize<GetCurrenciesResponse>(stream);
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
                                responseObj = (GetCurrenciesResponse)Deserialize<GetCurrenciesResponse>(memoryStream);
                            }
#endif
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

            return responseObj;
        }

        private static object Deserialize<T>(Stream stream) where T : class, new()
        {
            using (var reader = XmlReader.Create(stream))
            {
                reader.MoveToContent();

                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                return deserializer.Deserialize(reader);
            }
        }
    }


    [Serializable]
    [XmlRoot(ElementName = "CurrenciesList", Namespace = "http://merchant.roboxchange.com/WebService/")]
    public class GetCurrenciesResponse
    {
        [XmlElement("Result")]
        public ResultResponse Result { get; set; }

        [XmlElement("Groups")]
        public GroupsContainer GroupsContainer { get; set; }
    }

    [Serializable]
    public class ResultResponse
    {
        [XmlElement("Code")]
        public int Code { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
    }

    [Serializable]
    public class GroupsContainer
    {
        [XmlElement("Group")]
        public List<Group> Groups { get; set; }
    }

    [Serializable]
    public class Group
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlElement("Items")]
        public CurrencyContainer CurrencyContainer { get; set; }
    }

    [Serializable]
    public class CurrencyContainer
    {
        [XmlElement("Currency")]
        public List<Currency> Currencies { get; set; }
    }

    [Serializable]
    public class Currency
    {
        [XmlAttribute("Label")]
        public string Label { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Alias")]
        public string Alias { get; set; }
    }
}
