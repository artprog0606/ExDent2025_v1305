using System;
using System.Collections.Generic;
using System.Globalization;
using AdvantShop.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Geocoder.Yandex
{
    public class GeocodeResult
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        [JsonProperty("GeoObjectCollection")]
        public GeoObjectCollection GeoObjectCollection { get; set; }
    }
    
    public class GeoObjectCollection
    {
        public GeoObjectCollectionMetaDataProperty MetaDataProperty { get; set; }
        public List<FeatureMember> FeatureMember { get; set; }
    }
    
    public class GeoObjectCollectionMetaDataProperty
    {
        [JsonProperty("GeocoderResponseMetaData")]
        public GeocoderResponseMetaData GeocoderResponseMetaData { get; set; }
    }
    
    public class GeocoderResponseMetaData
    {
        public string Request { get; set; }
        public string Suggest { get; set; }
        public int Results { get; set; }
        public int Found { get; set; }
        public int? Skip { get; set; }
    }
    
    public class FeatureMember
    {
        [JsonProperty("GeoObject")]
        public GeoObject GeoObject { get; set; }
    }
    
    public class GeoObject
    {
        public GeoObjectMetaDataProperty MetaDataProperty { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BoundedBy BoundedBy { get; set; }
        
        [JsonProperty("Point")]
        public Point Point { get; set; }
    }
    
    public class Point
    {
        public Coordinates Pos { get; set; }
    }
    
    [JsonConverter(typeof(CoordinatesConverter))]
    public class Coordinates
    {
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        
        public override string ToString() => $"{Longitude.ToString(CultureInfo.InvariantCulture)} {Latitude.ToString(CultureInfo.InvariantCulture)}";
    }
    
    public class BoundedBy
    {
        [JsonProperty("Envelope")]
        public Envelope Envelope { get; set; }
    }
    
    public class Envelope
    {
        public Coordinates LowerCorner { get; set; }
        public Coordinates UpperCorner { get; set; }
    }
    
    public class GeoObjectMetaDataProperty
    {
        [JsonProperty("GeocoderMetaData")]
        public GeocoderMetaData GeocoderMetaData { get; set; }
    }
    
    public class GeocoderMetaData
    {
        /// <summary>
        /// Описывает точность соответствия дома в запросе и результате.
        /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/precision.html">Подробнее</a>
        /// </summary>
        public Precision Precision { get; set; }
        
        public string Text { get; set; }
        
        /// <summary>
        /// Вид найденного топонима.
        /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/kind.html">Подробнее</a>
        /// </summary>
        public Kind Kind { get; set; }
        
        /// <summary>
        /// Содержит информацию о найденном топониме в иерархическом порядке.
        /// </summary>
        public Address Address { get; set; }
    }

    /// <summary>
    /// Вид найденного топонима.
    /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/kind.html">Подробнее</a>
    /// </summary>
    public class Kind : StringEnum<Kind>
    {
        public Kind(string value) : base(value) { }
        
        /// <summary>
        /// отдельный дом
        /// </summary>
        public static Kind House => new Kind("house");

        /// <summary>
        /// улица
        /// </summary>
        public static Kind Street => new Kind("street");

        /// <summary>
        /// станция метро
        /// </summary>
        public static Kind Metro => new Kind("metro");

        /// <summary>
        /// район города
        /// </summary>
        public static Kind District => new Kind("district");

        /// <summary>
        /// населённый пункт: город / поселок / деревня / село и т. п.
        /// </summary>
        public static Kind Locality => new Kind("locality");

        /// <summary>
        /// район области
        /// </summary>
        public static Kind Area => new Kind("area");

        /// <summary>
        /// область
        /// </summary>
        public static Kind Province => new Kind("province");

        /// <summary>
        /// страна
        /// </summary>
        public static Kind Country => new Kind("country");

        /// <summary>
        /// устаревший тип, не используется
        /// </summary>
        [Obsolete("Устаревший тип, не используется")]
        public static Kind Region => new Kind("region");

        /// <summary>
        /// река / озеро / ручей / водохранилище и т. п.
        /// </summary>
        public static Kind Hydro => new Kind("hydro");

        /// <summary>
        /// ж.д. станция
        /// </summary>
        public static Kind RailwayStation => new Kind("railway_station");

        /// <summary>
        /// станции, не относящиеся к железной дороге. Например, канатные станции.
        /// </summary>
        public static Kind Station => new Kind("station");

        /// <summary>
        /// линия метро / шоссе / ж.д. линия
        /// </summary>
        public static Kind Route => new Kind("route");

        /// <summary>
        /// лес / парк / сад и т. п.
        /// </summary>
        public static Kind Vegetation => new Kind("vegetation");

        /// <summary>
        /// аэропорт
        /// </summary>
        public static Kind Airport => new Kind("airport");

        /// <summary>
        /// подъезд / вход
        /// </summary>
        public static Kind Entrance => new Kind("entrance");

        /// <summary>
        /// прочее
        /// </summary>
        public static Kind Other => new Kind("other");
    }

    /// <summary>
    /// Описывает точность соответствия дома в запросе и результате.
    /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/precision.html">Подробнее</a>
    /// </summary>
    public class Precision : StringEnum<Precision>
    {
        public Precision(string value) : base(value) { }
        
        /// <summary>
        /// Найден дом с указанным номером дома.
        /// </summary>
        public static Precision Exact => new Precision("exact");

        /// <summary>
        /// Найден дом с указанным номером, но с другим номером строения или корпуса.
        /// </summary>
        public static Precision Number => new Precision("number");

        /// <summary>
        /// Найден дом с номером, близким к запрошенному.
        /// </summary>
        public static Precision Near => new Precision("near");

        /// <summary>
        /// Найдены приблизительные координаты запрашиваемого дома.
        /// </summary>
        public static Precision Range => new Precision("range");

        /// <summary>
        /// Найдена только улица.
        /// </summary>
        public static Precision Street => new Precision("street");

        /// <summary>
        /// Не найдена улица, но найден, например, посёлок, район и т. п.
        /// </summary>
        public static Precision Other => new Precision("other");
    }
    
    public class Address
    {
        /// <summary>
        /// Код страны
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string CountryCode { get; set; }
        
        /// <summary>
        /// Почтовый индекс
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string PostalCode { get; set; }
        
        /// <summary>
        /// Адрес топонима в одной строке
        /// </summary>
        public string Formatted { get; set; }
        
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Additional { get; set; }
        
        /// <summary>
        /// Содержит разбитый на компоненты адрес топонима.
        /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/component.html">Подробнее</a>
        /// </summary>
        [JsonProperty("Components")]
        public List<Component> Components { get; set; }
    }

    /// <summary>
    /// Содержит разбитый на компоненты адрес топонима.
    /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/component.html">Подробнее</a>
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Тип компонента.
        /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/akind.html">Подробнее</a>
        /// </summary>
        public AKind Kind { get; set; }
        
        /// <summary>
        /// Значение компонента
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Тип компонента.
    /// <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/reference/akind.html">Подробнее</a>
    /// </summary>
    public class AKind : StringEnum<AKind>
    {
        public AKind(string value) : base(value) { }

        /// <summary>
        /// подъезд / вход
        /// </summary>
        public static AKind Entrance => new AKind("entrance");

        /// <summary>
        /// отдельный дом
        /// </summary>
        public static AKind House => new AKind("house");

        /// <summary>
        /// улица
        /// </summary>
        public static AKind Street => new AKind("street");

        /// <summary>
        /// станция метро
        /// </summary>
        public static AKind Metro => new AKind("metro");

        /// <summary>
        /// район города
        /// </summary>
        public static AKind District => new AKind("district");

        /// <summary>
        /// населённый пункт: город / поселок / деревня / село и т. п.
        /// </summary>
        public static AKind Locality => new AKind("locality");

        /// <summary>
        /// район области
        /// </summary>
        public static AKind Area => new AKind("area");

        /// <summary>
        /// область
        /// </summary>
        public static AKind Province => new AKind("province");

        /// <summary>
        /// страна
        /// </summary>
        public static AKind Country => new AKind("country");

        /// <summary>
        /// река / озеро / ручей / водохранилище и т. п.
        /// </summary>
        public static AKind Hydro => new AKind("hydro");

        /// <summary>
        /// ж.д. станция
        /// </summary>
        public static AKind Railway => new AKind("railway");

        /// <summary>
        /// линия метро / шоссе / ж.д. линия
        /// </summary>
        public static AKind Route => new AKind("route");

        /// <summary>
        /// лес / парк / сад и т. п.
        /// </summary>
        public static AKind Vegetation => new AKind("vegetation");

        /// <summary>
        /// аэропорт
        /// </summary>
        public static AKind Airport => new AKind("airport");

        /// <summary>
        /// прочее
        /// </summary>
        public static AKind Other => new AKind("other");
    }
}