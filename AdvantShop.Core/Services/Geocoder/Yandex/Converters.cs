using System;
using System.Globalization;
using Newtonsoft.Json;

namespace AdvantShop.Geocoder.Yandex
{
    public class CoordinatesConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var coordinates = value as Coordinates;
            serializer.Serialize(writer, coordinates.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var values = ((string) reader.Value).Split(' ');
            return new Coordinates
            {
                Longitude = decimal.Parse(values[0], NumberFormatInfo.InvariantInfo),
                Latitude = decimal.Parse(values[1], NumberFormatInfo.InvariantInfo),
            };
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(string);
    }
}