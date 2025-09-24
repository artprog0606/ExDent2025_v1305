using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Shipping.Yandex.Api
{
    public class YandexErrorConverter : JsonConverter
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
            if (reader.TokenType != JsonToken.StartObject)
                return null;

            var resultError = new ErrorResponse();
            var jObject = serializer.Deserialize<JObject>(reader);
            if (jObject["error"] != null && jObject["error"].Type == JTokenType.Object)
                resultError.Message = String.Join(", ", jObject["error"].Values());
            if (jObject["error_details"]?.Count() > 0 && jObject["error_details"].Type == JTokenType.Array)
                resultError.Message = String.Join(", ", jObject["error_details"]);
            else if (jObject["details"] != null)
                resultError.Message = String.Join(", ", jObject["details"].Values());
            else
                serializer.Populate(jObject.CreateReader(), resultError);

            return resultError;
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