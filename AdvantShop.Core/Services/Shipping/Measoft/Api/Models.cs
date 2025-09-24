using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.Measoft.Api
{
    [Serializable]
    [XmlRoot(ElementName = "auth")]
    public class MeasoftAuthOption
    {
        [XmlAttribute("login")]
        public string Login { get; set; }
        
        [XmlAttribute("pass")]
        public string Password { get; set; }
        
        [XmlAttribute("extra")]
        public string Extra { get; set; }
    }

    public class MeasoftOrderTrackNumber : MeasoftApiResponse
    {
        public string Number { get; set; }
        public string Barcode { get; set; }
        public int OrderPrice { get; set; }
    }

    public class MeasoftDeleteOrderResult : MeasoftApiResponse
    {
        public string Result { get; set; }
    }

    public class MeasoftApiResponse
    {
        public string Error { get; set; }
    }

    public class MeasoftOrderStatus : MeasoftApiResponse
    {
        public EMeasoftStatus Status { get; set; }
        public string OrderNo { get; set; }
    }

    public class MeasoftPoint : BaseShippingPoint
    {
        public int ParentCode { get; set; }
    }

    public class MeasoftCalcOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public string DeliveryTime { get; set; }
        public int DeliveryId { get; set; }
        public DateTime MinDeliveryDate { get; set; }
        public bool WithInsure { get; set; }
    }

    public class MeasoftCalcOptionParams
    {
        public string PvzCode { get; set; }
        public float Weight { get; set; }
        public int[] Dimensions { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string RegionCode { get; set; }
        public bool WithInsure { get; set; }
        public int ExtraDeliveryDays { get; set; }
        public bool WithPrice { get; set; }
        public List<int> DeliveryServiceIds { get; set; }
    }

    [Serializable]
    [XmlRoot("town")]
    public class MeasoftCity
    {
        [XmlElement("code")]
        public string Code { get; set; }
        
        [XmlElement("name")]
        public string Name { get; set; }
        
        [XmlElement("kladrcode")]
        public string KladrCode { get; set; }
        
        [XmlElement("fiascode")]
        public string FiasCode { get; set; }
        
        [XmlElement("city")]
        public MeasoftRegion Region { get; set; }
        
        [XmlElement("coords")]
        public Coordinates Coordinates  { get; set; }
    }

    public class MeasoftRegion
    {
        [XmlElement("code")]
        public string Code { get; set; }
        
        [XmlElement("name")]
        public string Name { get; set; }
    }

    public class Coordinates
    {
        [XmlIgnore] 
        public decimal? Latitude => LatitudeForXml.TryParseDecimal(true);
        [XmlAttribute("lat")]
        public string LatitudeForXml { get; set; }
        
        [XmlIgnore]
        public decimal? Longitude => LongitudeForXml.TryParseDecimal(true);
        
        [XmlAttribute("lon")]
        public string LongitudeForXml { get; set; }
    }

    public class MeasoftItem
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }
        public float Weight { get; set; }
        public float Price { get; set; }
        public float Amount { get; set; }
    }

    public class MeasoftDeliveryService : MeasoftApiResponse
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public enum TypeViewPoints
    {
        [Localize("Через Яндекс.Карты")]
        YaWidget = 0,

        [Localize("Списком")]
        List = 1
    }

    #region Next Models

    [Serializable]
    [XmlRoot(ElementName = "townlist")]
    public class GetCitiesParams
    {
        [XmlElement("auth")]
        public MeasoftAuthOption Auth { get; set; }
        
        /// <summary>
        /// Поиск по кодам.
        /// <remarks>В случае использования — контейнеры <see cref="ConditionsParams"/> и <see cref="LimitParams"/> игнорируются</remarks>
        /// </summary>
        [XmlElement("codesearch")]
        public CodeSearchParams CodeSearchParams { get; set; }
        
        /// <summary>
        /// Задает условия поиска.
        /// <remarks>Все вложенные элементы одновременно накладывают условие «И»</remarks>
        /// </summary>
        [XmlElement("conditions")]
        public ConditionsParams ConditionsParams { get; set; }
        
        /// <summary>
        /// Ограничивает вывод результата
        /// </summary>
        [XmlElement("limit")]
        public LimitParams LimitParams { get; set; }
    }

    [Serializable]
    public class CodeSearchParams
    {
        /// <summary>
        /// Поиск по индексу.
        /// <remarks>Обратите внимание на то, что один почтовый индекс может распространяться на несколько населенных пунктов. В этом случае система вернет несколько записей.</remarks>
        /// </summary>
        [XmlElement("zipcode")]
        public string ZipCode { get; set; }
        
        /// <summary>
        /// Поиск по 13-ти значному коду КЛАДР.
        /// </summary>
        [XmlElement("kladrcode")]
        public string KladrCode { get; set; }
        
        /// <summary>
        /// Поиск по коду ФИАС (AOGUID).
        /// </summary>
        [XmlElement("fiascode")]
        public string FiasCode { get; set; }
        
        /// <summary>
        /// Поиск по коду в системе.
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }
    }

    [Serializable]
    public class ConditionsParams
    {
        /// <summary>
        /// Поиск по всем населенным пунктам региона
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }
        
        /// <summary>
        /// Поиск населенных пунктов, название которых содержит указанный текст.
        /// </summary>
        [XmlElement("namecontains")]
        public string NameContains { get; set; }
        
        /// <summary>
        /// Поиск населенных пунктов, название которых содержит все указанные слова, с разбиением поисковой фразы через пробел.
        /// <example>Например "моск моло" найдет деревню "Молоково" в Московской области.</example>
        /// </summary>
        [XmlElement("namecontainsparts")]
        public string NameContainsParts { get; set; }
        
        /// <summary>
        /// Поиск населенных пунктов, название которых начинается с указанного текста.
        /// </summary>
        [XmlElement("namestarts")]
        public string NameStarts { get; set; }
        
        /// <summary>
        /// Поиск населенных пунктов, название которых соответствует указанному тексту.
        /// <remarks>Замечено, что города находит без типа, а разные села/деревни только по полному названию с типом</remarks>
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Поиск населенных пунктов, название вместе с типом населенного пункта которых соответствует указанному тексту.
        /// </summary>
        [XmlElement("fullname")]
        public string FullName { get; set; }
        
        /// <summary>
        /// Поиск только по стране с указанным внутренним кодом или текстовым кодом в соответствии стандартом ISO_3166-1
        /// <example>например, «RU», «RUS» для России</example>
        /// </summary>
        [XmlElement("country")]
        public string Country { get; set; }
    }

    [Serializable]
    public class LimitParams
    {
        /// <summary>
        /// Задает номер записи результата, начиная с которой выдавать ответ.
        /// </summary>
        [XmlElement("limitfrom")]
        public int? From { get; set; }
        
        /// <summary>
        /// Задает количество записей результата, которые нужно вернуть.
        /// </summary>
        [XmlElement("limitcount")]
        public int? Count { get; set; }
        
        /// <summary>
        /// <see cref="EnCountAll.Yes"/> указывает на необходимость подсчета общего количества найденных совпадений.
        /// <remarks>Это может замедлять выполнение запроса. Если отключено — в ответе не указываются <see cref="GetCitiesResponse.TotalCount"/> и <see cref="GetCitiesResponse.TotalPages"/>.</remarks>
        /// </summary>
        [XmlElement("countall")]
        public EnCountAll? CountAll { get; set; }
    }

    public enum EnCountAll
    {
        [XmlEnum(Name = "YES")]
        [EnumMember(Value = "YES")]
        Yes
    }

    [Serializable]
    [XmlRoot(ElementName = "townlist")]
    public class GetCitiesResponse
    {
        [XmlAttribute("count")]
        public int Count { get; set; }
        
        [XmlAttribute("page")]
        public int PageNumber { get; set; }

        [XmlIgnore] 
        public int? TotalCount => TotalCountForXml.TryParseInt(true);
        
        [XmlAttribute("totalcount")]
        public string TotalCountForXml { get; set; }
        
        [XmlIgnore] 
        public int? TotalPages => TotalPagesForXml.TryParseInt(true);
        
        [XmlAttribute("totalpages")]
        public string TotalPagesForXml { get; set; }

        [XmlElement("town")]
        public List<MeasoftCity> Cities { get; set; }

    } 

    [Serializable]
    [XmlRoot(ElementName = "request")]
    public class ErrorResponse
    {
        [XmlElement("error")]
        public ErrorData Error { get; set; }
    }

    [Serializable]
    public class ErrorData
    {
        [XmlAttribute("error")]
        public string Code { get; set; }
        
        [XmlAttribute("errormsg")]
        public string Message { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "storelist")]
    public class GetStoreListParams
    {
        [XmlElement("auth")]
        public MeasoftAuthOption Auth { get; set; }
    }
    
    [Serializable]
    [XmlRoot(ElementName = "storelist")]
    public class MesoftStoreListResponse
    {
        [XmlElement("store")]
        public List<MeasoftStoreList> StoreList { get; set; }
    }

    [Serializable]
    public class MeasoftStoreList
    {
        [XmlElement("code")]
        public int Code { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
    }

    #endregion
}
