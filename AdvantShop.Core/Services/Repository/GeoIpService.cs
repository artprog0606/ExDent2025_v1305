using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;

namespace AdvantShop.Repository
{
    public class GeoIpService
    {
        private static string _geoServiceUrl = SettingsLic.GeoIPServiceUrl + "v1/geoip/";
        //private const string _geoServiceUrl = "http://localhost:50357/v1/geoip/";
        //private const string GeoServiceUrl = "http://ipgeobase.ru:7020/geo?ip=";

        /// <summary>
        /// Get city name by ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static GeoIpData GetGeoIpData(string ip)
        {
            if (ip.IsLocalIP())
                return null;

            try
            {
                var url = string.Format("{0}{1}?lickey={2}", _geoServiceUrl, ip, SettingsLic.LicKey);
                var request = WebRequest.Create(url);
                request.Timeout = 700;

                using (var dataStream = request.GetResponse().GetResponseStream())
                using (var reader = new StreamReader(dataStream, Encoding.UTF8))
                {
                    var responseFromServer = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(responseFromServer))
                    {
                        var ipElement = XElement.Parse(responseFromServer).Element("ip");
                        if (ipElement != null)
                        {
                            var countryIso = ipElement.Element("country");
                            var city = ipElement.Element("city");
                            var state = ipElement.Element("region");

                            if (countryIso != null)
                                return new GeoIpData()
                                {
                                    Country = countryIso.Value,
                                    City = city != null ? city.Value : string.Empty,
                                    State = state != null ? state.Value : string.Empty
                                };
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Debug.Log.Error("GeoIpService", ex);
            }

            return null;
        }

        public static void SendGeoIpDataAsync(string ip, GeoIpData data, bool trustedCity)
        {
            Task.Factory.StartNew(() => { SendGeoIpData(ip, data, trustedCity); });
        }

        public static void SendGeoIpData(string ip, GeoIpData data, bool trustedCity)
        {
            if (ip.IsLocalIP())
                return;

            try
            {
                var request = WebRequest.Create(_geoServiceUrl + "setlocation");
                request.Timeout = 500;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                var postData = new StringBuilder()
                    .AppendFormat("countryiso2={0}", data.Country)
                    .AppendFormat("&region={0}", data.State)
                    .AppendFormat("&city={0}", data.City)
                    .AppendFormat("&ip={0}", ip)
                    .AppendFormat("&trustedCity={0}", trustedCity)
                    .AppendFormat("&lickey={0}", SettingsLic.LicKey).ToString();

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                request.GetResponse();
            }
            catch (Exception)
            {
                //Debug.Log.Error("GeoIpService", ex);
            }
        }
    }
}