//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AdvantShop.Shipping.Sdek
{
    [Serializable]
    public class SdekCityByTerm
    {
        //"id":16239,
        [JsonProperty("id")]
        public int Id { get; set; }

        // "postCodeArray":["000001"],
        [JsonProperty("postCodeArray")]
        public List<string> PostCodes { get; set; }

        // "cityName":"Ульяновского лесопарка пос., Москва",
        [JsonProperty("cityName")]
        public string Name { get; set; }

        // "regionId":"81",
        [JsonProperty("regionId")]
        public int RegionId { get; set; }

        // "regionName":"Москва",
        [JsonProperty("regionName")]
        public string RegionName { get; set; }

        // "countryId":"1",
        [JsonProperty("countryId")]
        public int CountryId { get; set; }

        // "countryName":"Россия",
        [JsonProperty("countryName")]
        public string CountryName { get; set; }

        // "countryIso":"RU",
        [JsonProperty("countryIso")]
        public string CountryIso { get; set; }

        // "name":"Ульяновского лесопарка пос., Москва, Москва, Россия"
        [JsonProperty("name")]
        public string FullName { get; set; }
    }

    public class SdekResponceGetCityByTerm
    {
        [JsonProperty("geonames")]
        public List<SdekCityByTerm> Cities { get; set; }
    }

    public class SdekCity
    {
        /// <summary>
        /// Название города
        /// </summary>
        [JsonProperty("cityName", NullValueHandling = NullValueHandling.Ignore)]
        public string CityName { get; set; }

        /// <summary>
        /// Код города по базе СДЭК
        /// </summary>
        [JsonProperty("cityCode", NullValueHandling = NullValueHandling.Ignore)]
        public string CityCode { get; set; }

        /// <summary>
        /// Идентификатор сущности в ИС СДЭК
        /// </summary>
        [JsonProperty("cityUuid", NullValueHandling = NullValueHandling.Ignore)]
        public string CityUuid { get; set; }

        /// <summary>
        /// Название страны
        /// </summary>
        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        /// <summary>
        /// Код страны
        /// </summary>
        [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryCode { get; set; }

        /// <summary>
        /// Название региона
        /// </summary>
        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public string Region { get; set; }

        /// <summary>
        /// Код региона в ИС СДЭК
        /// </summary>
        [JsonProperty("regionCode", NullValueHandling = NullValueHandling.Ignore)]
        public string RegionCode { get; set; }

        /// <summary>
        /// Код региона
        /// </summary>
        [JsonProperty("regionCodeExt", NullValueHandling = NullValueHandling.Ignore)]
        public string RegionCodeExt { get; set; }

        /// <summary>
        /// Код региона из ФИАС
        /// </summary>
        [JsonProperty("regionFiasGuid", NullValueHandling = NullValueHandling.Ignore)]
        public string RegionFiasGuid { get; set; }

        /// <summary>
        /// Название района региона
        /// </summary>
        [JsonProperty("subRegion", NullValueHandling = NullValueHandling.Ignore)]
        public string SubRegion { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Код города по КЛАДР
        /// </summary>
        [JsonProperty("kladr", NullValueHandling = NullValueHandling.Ignore)]
        public string Kladr { get; set; }

        /// <summary>
        /// Код адресного объекта в ФИАС
        /// </summary>
        [JsonProperty("fiasGuid", NullValueHandling = NullValueHandling.Ignore)]
        public string FiasGuid { get; set; }

        /// <summary>
        /// Ограничение на сумму наложенного платежа, возможные значения:
        /// <para>-1 - ограничения нет;</para>
        /// <para>0 - наложенный платеж не принимается;</para>
        /// <para>положительное значение - сумма наложенного платежа не более данного значения.</para>
        /// </summary>
        [JsonProperty("paymentLimit", NullValueHandling = NullValueHandling.Ignore)]
        public float? PaymentLimit { get; set; }

        /// <summary>
        /// Часовой пояс города
        /// </summary>
        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public string Timezone { get; set; }
    }

    [Serializable]
    public class SdekGoods
    {
        [JsonProperty(PropertyName = "weight")]
        public string Weight { get; set; }

        [JsonProperty(PropertyName = "length")]
        public string Length { get; set; }

        [JsonProperty(PropertyName = "width")]
        public string Width { get; set; }

        [JsonProperty(PropertyName = "height")]
        public string Height { get; set; }
        
        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Length.GetHashCode() ^ Height.GetHashCode();
        }
    }
}