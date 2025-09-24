using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Shipping.LPost.Api
{
    public class LPostTextJsonConverter<ListClass, DefaultClass> : JsonConverter where ListClass : LPostError, new()
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            string error = (obj.SelectToken("errorMessage") ?? obj.SelectToken("Message"))?.Value<string>();
            if (!string.IsNullOrEmpty(error))
                return new ListClass { Error = error };

            string jsontxt = obj.SelectToken("JSON_TXT").Value<string>();
            // оставляем только массив
            jsontxt = jsontxt.Remove(0, jsontxt.IndexOf('['));
            jsontxt = jsontxt.Remove(jsontxt.LastIndexOf('}'));

            List<DefaultClass> list = JsonConvert.DeserializeObject<List<DefaultClass>>(jsontxt);
            return Activator.CreateInstance(typeof(ListClass), list);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
