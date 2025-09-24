using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Shipping.OzonRocket.Api
{
    public class ErrorConverter : JsonConverter<Error>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, Error value, JsonSerializer serializer)
            => throw new NotImplementedException();

        public override Error ReadJson(JsonReader reader, Type objectType, Error existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                return null;
            
            var resultError = new Error();
            var jObject = serializer.Deserialize<JObject>(reader);
            // при получении токена ошибка в другой структуре объетка возвращается
            if (jObject["error"] != null && jObject["error"].Type == JTokenType.String && jObject.Count == 1)
                resultError.Message = jObject["error"].Value<string>();
            else
                serializer.Populate(jObject.CreateReader(), resultError);
            
            return resultError;
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