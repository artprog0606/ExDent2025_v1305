using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Shipping.Sdek.Api;

namespace AdvantShop.Shipping.Sdek
{
    public class SdekService
    {
        public static int? GetSdekCityId(string cityName, string district, string regionName, string country, SdekApiService20 service20, out CityInfo city, bool allowedObsoleteFindCity = false)
        {
            city = null;
            int cityId = 0;
            string cacheCityIdKey = "sdek_CityIdByName_" + (cityName + "_" + district + "_" + regionName + "_" + country).GetHashCode();
            if (!CacheManager.TryGetValue<int>(cacheCityIdKey, out cityId))
            {
                var countryIso2 = country.IsNotEmpty() ? Repository.CountryService.GetIso2(country) : null;

                // новый алгоритм только по городу плохо ищет, нужен регион для точности
                if (regionName.IsNotEmpty() && service20 != null)
                    cityId = GetCityByNameV20(service20, cityName, district, regionName, country, countryIso2, out city) ?? 0;

                if (cityId == 0 && regionName.IsNotEmpty())
                    cityId = GetCityByNameV15(cityName, district, regionName, country, countryIso2) ?? 0;

                if (cityId == 0 && allowedObsoleteFindCity)
                {
                    // определение по старому алгоритму (api)
                    var cityValidName = country.IsNullOrEmpty() || country.Equals("Россия", StringComparison.OrdinalIgnoreCase) || country.Equals("Беларусь", StringComparison.OrdinalIgnoreCase) || country.Equals("Казахстан", StringComparison.OrdinalIgnoreCase)
                        ? CityCrutch(cityName)
                        : cityName;

                    cityId = GetCityByNameOld(cityValidName, district, regionName, country) ?? 0;
                    if (cityId == 0)
                        cityId = GetCityByNameOld(cityValidName = cityName, district, regionName, country) ?? 0;
                }

                CacheManager.Insert<int>(cacheCityIdKey, cityId, 1440);
            }

            return cityId == 0 ? null : (int?)cityId;
        }

        private static string CityCrutch(string cityName)
        {
            return cityName.Replace("ё", "е", StringComparison.OrdinalIgnoreCase);
        }

        private static string RegionCrutch(string regionName)
        {
            return regionName.Replace("ё", "е", StringComparison.OrdinalIgnoreCase);
        }

        private static int? GetCityByNameV20(SdekApiService20 sdekApiService20, string cityName, string district,
            string regionName, string country, string countryIso2, out CityInfo city)
        {
            int cityId = 0;

            var newRegionName = regionName.RemoveTypeFromRegion() ?? string.Empty;
            var newCountry = country ?? string.Empty;
            var newDistrict = district ?? string.Empty;

            var response = sdekApiService20.GetCities(
                new CitiesFilter
                {
                    City = cityName,
                    CountryCodes = countryIso2.IsNotEmpty()
                        ? new List<string>() {countryIso2}
                        : null
                });
            if (response != null && response.Count > 0)
            {
                var cities = response
                    .OrderBy(x => x.Code)
                    .ToList();

                cities.ForEach(x => {
                    x.Region = (x.Region.RemoveTypeFromRegion() ?? string.Empty);
                    x.SubRegion = x.SubRegion ?? string.Empty;
                });

                // в регионе тоже проблемы с буквой Ё
                var regionNameCrutch = RegionCrutch(newRegionName);

                city =
                    FindCity(cities, newDistrict, newRegionName, regionNameCrutch, newCountry) // по всем данным
                    ?? FindCity(cities, null, newRegionName, regionNameCrutch, newCountry) // без района
                    ?? FindCity(cities, null, newRegionName, regionNameCrutch, null); // без района и страны
                    //?? FindCity(cities, null, null, null, null); // берем первый (не стоит, может считать не туда)

                if (city != null)
                {
                    cityId = city.Code;
                    return cityId;
                }
            }

            city = null;
            return null;        
        }

        private static CityInfo FindCity(IEnumerable<CityInfo> cities, string district, string region, string regionCrutch, string country)
        {
            if (district.IsNotEmpty())
                cities = cities.Where(x => x.SubRegion.Contains(district, StringComparison.OrdinalIgnoreCase));
            if (region.IsNotEmpty() || regionCrutch.IsNotEmpty())
                cities = cities.Where(x => x.Region.Equals(region, StringComparison.OrdinalIgnoreCase) || x.Region.Equals(regionCrutch, StringComparison.OrdinalIgnoreCase));
            if (country.IsNotEmpty())
                cities = cities.Where(x => x.Country.Contains(country, StringComparison.OrdinalIgnoreCase));

            return cities.FirstOrDefault();
        }

        private static int? GetCityByNameV15(string cityName, string district, string regionName, string country, string countryIso2)
        {
            int cityId = 0;

            var newRegionName = regionName.RemoveTypeFromRegion() ?? string.Empty;
            var newCountry = country ?? string.Empty;
            var newDistrict = district ?? string.Empty;

            var response = RequestHelper.MakeRequest<List<SdekCity>>(
                $"https://proxy.advstatic.ru/integration.cdek.ru/v1/location/cities/json?cityName={HttpUtility.UrlEncode(cityName)}&countryCode={HttpUtility.UrlEncode(countryIso2)}",
                method: ERequestMethod.GET,
                contentType: ERequestContentType.TextJson,
                timeoutSeconds: 5);
            if (response != null && response.Count > 0)
            {
                var cities = response
                    .OrderBy(x => x.CityCode.Length)
                    .ThenBy(x => x.CityCode)
                    .ToList();

                cities.ForEach(x => {
                    x.Region = (x.Region.RemoveTypeFromRegion() ?? string.Empty);
                    x.SubRegion = x.SubRegion ?? string.Empty;
                });

                // в регионе тоже проблемы с буквой Ё
                var regionNameCrutch = RegionCrutch(newRegionName);

                var city =
                    FindCity(cities, newDistrict, newRegionName, regionNameCrutch, newCountry) // по всем данным
                    ?? FindCity(cities, null, newRegionName, regionNameCrutch, newCountry) // без района
                    ?? FindCity(cities, null, newRegionName, regionNameCrutch, null); // без района и страны
                    //?? FindCity(cities, null, null, null, null); // берем первый (не стоит, может считать не туда)

                if (city != null)
                {
                    cityId = city.CityCode.TryParseInt();
                    return cityId;
                }
            }
            return null;
        }

        private static SdekCity FindCity(IEnumerable<SdekCity> cities, string district, string region, string regionCrutch, string country)
        {
            if (district.IsNotEmpty())
                cities = cities.Where(x => x.SubRegion.Contains(district, StringComparison.OrdinalIgnoreCase));
            if (region.IsNotEmpty() || regionCrutch.IsNotEmpty())
                cities = cities.Where(x => x.Region.Equals(region, StringComparison.OrdinalIgnoreCase) || x.Region.Equals(regionCrutch, StringComparison.OrdinalIgnoreCase));
            if (country.IsNotEmpty())
                cities = cities.Where(x => x.Country.Contains(country, StringComparison.OrdinalIgnoreCase));

            return cities.FirstOrDefault();
        }

        [Obsolete]
        private static int? GetCityByNameOld(string cityName, string district, string regionName, string country)
        {
            int cityId = 0;

            var newRegionName = regionName.RemoveTypeFromRegion() ?? string.Empty;
            var newCountry = country ?? string.Empty;
            var newDistrict = district ?? string.Empty;

            var response = RequestHelper.MakeRequest<SdekResponceGetCityByTerm>(
                "https://proxy.advstatic.ru/api.cdek.ru/city/getListByTerm/json.php?q=" + HttpUtility.UrlEncode(cityName),
                method: ERequestMethod.GET,
                contentType: ERequestContentType.TextJson,
                timeoutSeconds: 5);
            
            if (response != null && response.Cities != null && response.Cities.Count > 0)
            {
                var cities = response.Cities
                    .OrderBy(x => x.Id)
                    .ToList();

                cities.ForEach(x => x.RegionName = (x.RegionName.RemoveTypeFromRegion() ?? string.Empty));

                // в регионе тоже проблемы с буквой Ё
                var regionNameCrutch = RegionCrutch(newRegionName);

                var city =
                    FindCity(cities, newDistrict, newRegionName, regionNameCrutch, newCountry) // по всем данным
                    ?? FindCity(cities, null, newRegionName, regionNameCrutch, newCountry) // без района
                    ?? FindCity(cities, null, newRegionName, regionNameCrutch, null); // без района и страны
                    //?? FindCity(response.Cities, null, null, null, null); // берем первый (не стоит, может считать не туда)

                if (city != null)
                {
                    cityId = city.Id;
                    return cityId;
                }
            }
            return null;
        }

        private static SdekCityByTerm FindCity(IEnumerable<SdekCityByTerm> cities, string district, string region, string regionCrutch, string country)
        {
            if (district.IsNotEmpty())
                cities = cities.Where(x => x.Name.Contains(district, StringComparison.OrdinalIgnoreCase));
            if (region.IsNotEmpty() || regionCrutch.IsNotEmpty())
                cities = cities.Where(x => x.RegionName.Equals(region, StringComparison.OrdinalIgnoreCase) || x.RegionName.Equals(regionCrutch, StringComparison.OrdinalIgnoreCase));
            if (country.IsNotEmpty())
                cities = cities.Where(x => x.CountryName.Contains(country, StringComparison.OrdinalIgnoreCase));

            return cities.FirstOrDefault();
        }
    }
}
