using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Core.Services.Shipping.Boxberry
{
    #region Converters
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd.MM.yyyy HH:mm";
        }
    }

    public class BaseResponseConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Create generic type
            var baseTypeApiResponse = typeof(BoxberryApiResponse<>);
            Type[] typeArgs = objectType.GetGenericArguments();
            var typeResponse = baseTypeApiResponse.MakeGenericType(typeArgs);

            // Check Error
            var items = serializer.Deserialize<JArray>(reader);
            if (items.Count == 1 && items[0]["err"] != null)
                return Activator.CreateInstance(typeResponse, new object[] { null, items[0].ToObject<BoxberryErrorObject>().Error });

            return Activator.CreateInstance(typeResponse, new object[] { items.ToObject(typeArgs[0]), null });
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    [Serializable]
    public class BoxberryApiResponse
    {
        [JsonProperty(PropertyName = "err")]
        public string Error { get; set; }

        //public T Object { get; set; }

        //public List<T> ListObjects { get; set; }
    }

    [JsonConverter(typeof(BaseResponseConverter))]
    public class BoxberryApiResponse<T>
    {
        public BoxberryApiResponse(T result, string error)
        {
            Result = result;
            Error = error;
        }

        public T Result { get; private set; }

        public string Error { get; private set; }
    }

    [Serializable]
    public class BoxberryGetKeyIntegration : BoxberryApiResponse
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
    }

    [Serializable]
    public class BoxberryOrderDeleteAnswer : BoxberryApiResponse
    {
        [JsonProperty(PropertyName = "Text")]
        public string Result { get; set; }
    }

    [Serializable]
    public class BoxberryWaitingOrdersAnswer : BoxberryApiResponse
    {
        [JsonProperty(PropertyName = "ImIds")]
        public string ImIds { get; set; }
    }

    [Serializable]
    public class BoxberryStatusesFull : BoxberryApiResponse
    {
        public List<BoxberryStatuseFull> Statuses { get; set; }
    }

    [Serializable]
    public class BoxberryStatuseFull
    {
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Comment")]
        public string Comment { get; set; }
    }

    [Serializable]
    public class BoxberryStatuse
    {
        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Comment")]
        public string Comment { get; set; }
    }
}
