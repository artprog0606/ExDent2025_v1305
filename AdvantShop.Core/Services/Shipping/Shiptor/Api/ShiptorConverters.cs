using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Shipping.Shiptor.Api
{
    public class ShiptorBaseResponseConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Create generic type
            var baseTypeApiResponse = typeof(ShiptorBaseResponse<>);
            Type[] typeArgs = objectType.GetGenericArguments();
            var typeResponse = baseTypeApiResponse.MakeGenericType(typeArgs);

            // Check Error
            var item = serializer.Deserialize<JObject>(reader);
            if (item["error"] != null && item["error"].Type == JTokenType.String)
                return Activator.CreateInstance(typeResponse, new object[] { null, new ResponseError{ Message = item["error"].Value<string>() } });

            return item["result"] != null ? Activator.CreateInstance(typeResponse, new object[] { item["result"].ToObject(typeArgs[0], serializer), null }) : null;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
