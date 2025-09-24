using System;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Helpers;

namespace AdvantShop.Repository
{
    public static class IpZoneContext
    {
        private const string IpZoneContextKey = "IpZoneContext";
        private const string IpZoneCookie = "ipzone";

        public static IpZone CurrentZone
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var contextZone = HttpContext.Current.Items[IpZoneContextKey] as IpZone;
                    if (contextZone != null) return contextZone;

                    var zone = GetZoneFromCookie();
                    if (zone == null)
                    {
                        if (SettingsDesign.AutodetectCity)
                        {
                            zone = GetZoneByIp();
                        }
                        else
                        {
                            if ((zone = GetZoneByName()) is null)
                                return new IpZone();
                        }

                        SetCustomerCookie(zone);
                    }

                    HttpContext.Current.Items[IpZoneContextKey] = zone;
                    return zone;
                }

                return GetDefaultZone();
            }
        }

        private static IpZone GetZoneFromCookie()
        {
            var cookie = HttpContext.Current.Request.Cookies[IpZoneCookie];

            if (cookie == null || cookie.Value.IsNullOrEmpty())
                return null;

            var cookieValues = HttpUtility.UrlDecode(cookie.Value).Split(new[] { ';' }, StringSplitOptions.None);
            if (cookieValues.Length < 5)
                return null;

            var countryId = cookieValues[0].TryParseInt();
            var regionId = cookieValues[1].TryParseInt();
            var cityId = cookieValues[2].TryParseInt();

            var region = cookieValues[3].Trim();
            var city = cookieValues[4].Trim();

            var dialCode = cookieValues.Length > 5 ? cookieValues[5].TryParseInt(true) : null;

            var zip = cookieValues.Length > 6 ? cookieValues[6].Trim() : null;
            var district = cookieValues.Length > 7 ? cookieValues[7].Trim() : null;

            var country = CountryService.GetCountry(countryId);
            if (country == null || country.CountryId == 0)
                return null;

            if (cityId == 0 && city.IsNullOrEmpty())
                return null;

            if (dialCode == null)
                dialCode = country.DialCode;

            if (regionId != 0)
            {
                var regionTemp = RegionService.GetRegion(regionId);
                regionId = regionTemp == null || regionTemp.RegionId == 0 ? 0 : regionTemp.RegionId;
                region = regionTemp == null || regionTemp.RegionId == 0 ? string.Empty : regionTemp.Name;
            }

            var zone = new IpZone()
            {
                CountryId = country.CountryId,
                CountryName = country.Name,
                RegionId = regionId,
                Region = region,
                CityId = cityId,
                City = city,
                District = district,
                DialCode = dialCode,
                Zip = zip
            };

            return zone;
        }

        private static IpZone GetZoneByIp()
        {
            var zone = new IpZone();

            Country country = null;
            Region region = null;
            City city = null;

            if (HttpContext.Current.Request != null && !BrowsersHelper.IsBot())
            {
                var ip = HttpContext.Current.Request.Headers["X-1Gb-Client-IP"]
                         ?? HttpContext.Current.Request.Headers["x-forwarded-for"]
                         ?? HttpContext.Current.Request.UserHostAddress;

                var moduleType = AttachedModules.GetModules<IGeoIp>().FirstOrDefault();
                if (!ip.IsLocalIP() && moduleType != null)
                {
                    var instance = (IGeoIp)Activator.CreateInstance(moduleType, null);
                    var moduleZone = instance.GetIpZone(ip);
                    if (moduleZone != null)
                        return moduleZone;
                }

                var geoIpData = GeoIpService.GetGeoIpData(ip);
                if (geoIpData != null)
                {
                    if (geoIpData.Country.IsNotEmpty())
                        country = CountryService.GetCountryByIso2(geoIpData.Country);
                    zone.Region = geoIpData.State;
                    zone.City = geoIpData.City;
                }
            }

            if (zone.City.IsNotEmpty())
            {
                city = CityService.GetCityByName(zone.City);
                if (city != null &&
                    (region = RegionService.GetRegion(city.RegionId)) != null &&
                    (country != null || (country = CountryService.GetCountry(SettingsMain.SellerCountryId)) != null) && 
                    !CityService.IsCityInCountry(city.CityId, country.CountryId))
                {
                    country = CountryService.GetCountry(region.CountryId);
                }
            }
            else
            {
                if (zone.Region.IsNullOrEmpty() || (region = RegionService.GetRegionByName(zone.Region)) == null)
                {
                    region = RegionService.GetRegion(SettingsMain.SellerRegionId);
                }

                if ((zone.City.IsNullOrEmpty() || (city = CityService.GetCityByName(zone.City)) == null) 
                    && SettingsMain.City.IsNotEmpty())
                {
                    city = CityService.GetCityByName(SettingsMain.City);
                }
                
                country = region != null ? CountryService.GetCountry(region.CountryId) : null;
            }

            if (country != null)
            {
                zone.CountryId = country.CountryId;
                zone.CountryName = country.Name;
                zone.DialCode = country.DialCode;
            }

            if (region != null)
            {
                zone.RegionId = region.RegionId;
                zone.Region = region.Name;
            }

            if (city != null)
            {
                zone.City = city.Name;
                zone.CityId = city.CityId;
                zone.District = city.District;
                zone.Zip = city.Zip;
            }

            return zone;
        }

        private static IpZone GetZoneByName()
        {
            var city = SettingsDesign.DefaultCityIdIfNotAutodetect.HasValue
                ? CityService.GetCity(SettingsDesign.DefaultCityIdIfNotAutodetect.Value)
                : null;
            if (city != null)
            {
                var zone = new IpZone
                {
                    City = city.Name,
                    CityId = city.CityId,
                    District = city.District,
                    Zip = city.Zip
                };

                var region = RegionService.GetRegion(city.RegionId);
                if (region != null)
                {
                    zone.Region = region.Name;
                    zone.RegionId = region.RegionId;

                    var country = CountryService.GetCountry(region.CountryId);
                    if (country != null)
                    {
                        zone.CountryId = country.CountryId;
                        zone.CountryName = country.Name;
                    }
                }

                return zone;
            }

            return null;
        }

        [Obsolete]
        private static IpZone GetZoneByIpOld()
        {
            var zone = new IpZone();

            if (SettingsDesign.AutodetectCity)
            {
                var geoIpData = HttpContext.Current.Request != null && !Helpers.BrowsersHelper.IsBot() ? 
                        GeoIpService.GetGeoIpData(HttpContext.Current.Request.Headers["X-1Gb-Client-IP"] ?? HttpContext.Current.Request.UserHostAddress) : null;
                if (geoIpData != null)
                {
                    if (geoIpData.Country.IsNotEmpty())
                    {
                        var country = CountryService.GetCountryByIso2(geoIpData.Country);
                        if (country != null)
                        {
                            zone.CountryId = country.CountryId;
                            zone.CountryName = country.Name;
                            zone.DialCode = country.DialCode;
                        }

                        zone.Region = geoIpData.State;
                        zone.City = geoIpData.City;
                    }
                }
            }

            if (zone.CountryId == 0)
            {
                zone.CountryId = SettingsMain.SellerCountryId;
                var country = CountryService.GetCountry(zone.CountryId);
                if (country != null)
                    zone.CountryName = country.Name;
            }

            if (zone.City.IsNullOrEmpty())
            {
                var state = RegionService.GetRegion(SettingsMain.SellerRegionId);
                if (state != null)
                {
                    var country = CountryService.GetCountry(state.CountryId);
                    if (country != null)
                    {
                        zone.CountryId = state.CountryId;
                        zone.CountryName = country.Name;
                        zone.DialCode = country.DialCode;
                    }
                    zone.Region = state.Name;
                    zone.RegionId = state.RegionId;
                }

                zone.City = SettingsMain.City;
            }
            else
            {
                var city = CityService.GetCityByName(zone.City);

                if (city != null)
                {
                    zone.CityId = city.CityId;

                    if (!CityService.IsCityInCountry(city.CityId, zone.CountryId))
                    {
                        var region = RegionService.GetRegion(city.RegionId);
                        if (region != null)
                        {
                            zone.RegionId = region.RegionId;
                            zone.Region = region.Name;

                            var country = CountryService.GetCountry(region.CountryId);
                            if (country != null)
                            {
                                zone.CountryId = country.CountryId;
                                zone.CountryName = country.Name;
                                zone.DialCode = country.DialCode;
                            }
                        }
                    }
                }
            }

            if (zone.Region.IsNullOrEmpty())
            {
                var state = RegionService.GetRegion(SettingsMain.SellerRegionId);
                if (state != null)
                {
                    zone.Region = state.Name;
                    zone.RegionId = state.RegionId;
                }
            }

            return zone;
        }

        private static IpZone GetDefaultZone()
        {
            var city = CityService.GetCityByName(SettingsMain.City, SettingsMain.SellerRegionId);
            return new IpZone()
            {
                CountryId = SettingsMain.SellerCountryId,
                CountryName = CountryService.GetCountry(SettingsMain.SellerCountryId)?.Name,
                RegionId = SettingsMain.SellerRegionId,
                Region = RegionService.GetRegion(SettingsMain.SellerRegionId)?.Name,
                City = city != null ? city.Name : SettingsMain.City,
                Zip = city?.Zip
            };
        }

        public static void SetCustomerCookie(IpZone zone)
        {
            CommonHelper.SetCookie(IpZoneCookie,
                $"{zone.CountryId};{zone.RegionId};{zone.CityId};{zone.Region};{zone.City};{zone.DialCode};{zone.Zip};{zone.District}",
                new TimeSpan(365, 24, 0),
                httpOnly: true,
                setOnMainDomain: true);
        }

        public static bool IsCookieExists()
        {
            var cookie = HttpContext.Current.Request.Cookies[IpZoneCookie];
            return cookie != null && cookie.Value.IsNotEmpty();
        }

        public static void SendGeoIpData(GeoIpData data, bool trustedCity = false)
        {
            GeoIpService.SendGeoIpDataAsync(HttpContext.Current.Request.Headers["X-1Gb-Client-IP"] ?? 
                HttpContext.Current.Request.Headers["CF-Connecting-IP"] ??
                HttpContext.Current.Request.UserHostAddress, data, trustedCity);
        }

        public static bool ShowBubble()
        {
            return SettingsDesign.AutodetectCity && SettingsDesign.DisplayCityBubbleType == SettingsDesign.EDisplayCityBubbleType.DisplayZonePopover;
        }
    }
}