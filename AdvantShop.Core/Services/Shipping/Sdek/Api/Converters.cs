using Newtonsoft.Json.Converters;

namespace AdvantShop.Shipping.Sdek.Api
{
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter(string format)
        {
            base.DateTimeFormat = format;
        }
    }
}