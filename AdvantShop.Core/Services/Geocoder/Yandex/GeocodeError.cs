namespace AdvantShop.Geocoder.Yandex
{
    public class GeocodeError
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
    }
}