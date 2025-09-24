using System;
using AdvantShop.Core.Common.Extensions;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.NovaPoshta
{
    public class FloatConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((float)value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString().TryParseFloat();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(float);
        }
    }
}
