using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Shipping.PecEasyway.Api
{
    public class PecBaseResponseConverter : JsonConverter
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
            // Create generic type
            var baseTypeApiResponse = typeof(PecBaseResponse<>);
            Type[] typeArgs = objectType.GetGenericArguments();
            var typeResponse = baseTypeApiResponse.MakeGenericType(typeArgs);

            // Check Error
            if (reader.TokenType == JsonToken.StartObject)
            {
                var item = serializer.Deserialize<JObject>(reader);
                if (item["isError"] != null && item["isError"].Type == JTokenType.Boolean && item["isError"].Value<bool>() == true)
                    return Activator.CreateInstance(typeResponse, new object[] { null, item.ToObject(typeof(ResponseError), serializer) });
                return Activator.CreateInstance(typeResponse, new object[] { item.ToObject(typeArgs[0], serializer), null });
            }
            return Activator.CreateInstance(typeResponse, new object[] { serializer.Deserialize(reader, typeArgs[0]), null });
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}
