using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Shipping.PickPoint.Api
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }

    public class StringToArrayConverter<T> : JsonConverter
        where T: struct
    {
        private readonly string separator;

        public StringToArrayConverter(string separator)
        {
            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            this.separator = separator;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(T));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.TokenType != JsonToken.String)
                return null;

            string value = (string)reader.Value;

            if (!string.IsNullOrEmpty(value))
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                    return value
                        .Split(new[] { this.separator }, StringSplitOptions.None)
                        .Select(item => (T)converter.ConvertFromInvariantString(item))
                        .ToArray();
            }

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToNullableFloatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(float?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Float && reader.TokenType != JsonToken.Integer && reader.TokenType != JsonToken.String)
                return null;

            if (reader.TokenType == JsonToken.String)
            {
                var value = (string)reader.Value;
                value = value ?? "";
                float floatval;
                return float.TryParse(value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out floatval) ? floatval : (float?)null;
            }

            return reader.Value;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
