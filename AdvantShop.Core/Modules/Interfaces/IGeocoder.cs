namespace AdvantShop.Core.Modules.Interfaces
{
    public interface IGeocoder
    {
        GeocoderMetaData Geocode(string address);
        ReverseGeocoderData ReverseGeocode(Point point);
    }

    public class GeocoderMetaData
    {
        /// <summary>
        /// Точность координат
        /// </summary>
        public Precision Precision { get; set; }
        
        /// <summary>
        /// Вид найденного топонима
        /// </summary>
        public Kind Kind { get; set; }
        
        /// <summary>
        /// Координаты
        /// </summary>
        public Point Point { get; set; }
        
        /// <summary>
        /// Облатсть
        /// </summary>
        public BoundedBy BoundedBy { get; set; }
    }

    public struct Point
    {
        public Point(decimal longitude, decimal latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
        public decimal Longitude { get; }
        public decimal Latitude { get; }
    }

    public class BoundedBy
    {
        public Point LowerCorner { get; set; }
        public Point UpperCorner { get; set; }
    }

    public class ReverseGeocoderData
    {
        public ReverseGeocoderAddress Address { get; set; }
    }

    public class ReverseGeocoderAddress
    {
        public string AddressString { get; set; }
        public string Country { get; set; }
        // public string RegionKladrId { get; set; }
        // public string RegionFiasId { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        // public string CityKladrId { get; set; }
        // public string CityFiasId { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Structure { get; set; }
        // public string HomeKladrId { get; set; }
        // public string HomeFiasId { get; set; }
    }

    public enum Precision
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        None,
        
        /// <summary>
        /// Точные координаты дома
        /// </summary>
        Exact,
        
        /// <summary>
        /// Найден дом с указанным номером, но с другим номером строения или корпуса.
        /// </summary>
        Number,
        
        /// <summary>
        /// Найден дом с номером, близким к запрошенному.
        /// </summary>
        Near,
        
        /// <summary>
        /// Найдены приблизительные координаты запрашиваемого дома.
        /// </summary>
        Range,

        /// <summary>
        /// Улица
        /// </summary>
        Street,
        
        /// <summary>
        /// населённый пункт: город / поселок / деревня / село и т. п.
        /// </summary>
        Locality,
    }

    public enum Kind
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        None,

        /// <summary>
        /// подъезд / вход
        /// </summary>
        Entrance,

        /// <summary>
        /// отдельный дом
        /// </summary>
        House,

        /// <summary>
        /// улица
        /// </summary>
        Street,

        /// <summary>
        /// станция метро
        /// </summary>
        Metro,

        /// <summary>
        /// район города
        /// </summary>
        District,

        /// <summary>
        /// населённый пункт: город / поселок / деревня / село и т. п.
        /// </summary>
        Locality,

        /// <summary>
        /// район области
        /// </summary>
        Area,

        /// <summary>
        /// область
        /// </summary>
        Province,

        /// <summary>
        /// страна
        /// </summary>
        Country,

        /// <summary>
        /// река / озеро / ручей / водохранилище и т. п.
        /// </summary>
        Hydro,

        /// <summary>
        /// ж.д. станция
        /// </summary>
        RailwayStation,

        /// <summary>
        /// линия метро / шоссе / ж.д. линия
        /// </summary>
        Route,

        /// <summary>
        /// лес / парк / сад и т. п.
        /// </summary>
        Vegetation,

        /// <summary>
        /// аэропорт
        /// </summary>
        Airport,

        /// <summary>
        /// прочее
        /// </summary>
        Other,
    }
}