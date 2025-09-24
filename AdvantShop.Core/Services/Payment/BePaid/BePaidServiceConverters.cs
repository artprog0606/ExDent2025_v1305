using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Core.Services.Payment.BePaid
{
    public class BePaidServiceResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BePaidServiceResponse<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var baseTypeApiResponse = typeof(BePaidServiceResponse<>);
            Type[] typeArgs = objectType.GetGenericArguments();
            var typeResponse = baseTypeApiResponse.MakeGenericType(typeArgs);

            var jobj = serializer.Deserialize<JObject>(reader);
            if (jobj.Count == 2 &&
                jobj["errors"] != null &&
                jobj["message"] != null && jobj["message"].Type == JTokenType.String)
            {
                return Activator.CreateInstance(typeResponse, jobj["message"].Value<string>());
            }

            return Activator.CreateInstance(typeResponse, jobj.ToObject(typeArgs[0], serializer));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
