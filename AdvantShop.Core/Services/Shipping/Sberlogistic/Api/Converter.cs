using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Shipping.Sberlogistic.Api
{
    public class SberlogisticArrayToObjectConverter<T> : JsonConverter
    {
        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var type = typeof(T);

            if (reader.TokenType == JsonToken.StartArray)
            {
                var items = serializer.Deserialize<JArray>(reader);
                var item = items?.FirstOrDefault();
                if (item is null)
                    return Activator.CreateInstance(type);
                return Activator.CreateInstance(type, item);
            }
            return serializer.Deserialize(reader, type);
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }

    public class DateFormatConverter : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
