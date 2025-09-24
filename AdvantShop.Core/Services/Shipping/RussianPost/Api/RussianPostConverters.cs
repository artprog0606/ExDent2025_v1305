using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.RussianPost.Api
{
    public class ArrayStringToSingleLineConverter : JsonConverter
    {
        private readonly string separator;

        public ArrayStringToSingleLineConverter(string separator)
        {
            if (separator == null)
                throw new ArgumentNullException("separator");

            this.separator = separator;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<string>) || objectType == typeof(string[]));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.StartArray)
            {
                return string.Join(this.separator, serializer.Deserialize<string[]>(reader));
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

    public class FloatToIntegerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(float));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            return (int)serializer.Deserialize<float>(reader);
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

    public class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? 1 : 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString() == "1";
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }

}
