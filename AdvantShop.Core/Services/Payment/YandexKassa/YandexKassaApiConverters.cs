using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvantShop.Core.Services.Payment.YandexKassa
{
    internal sealed class NumbersAsStringConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                    return true;

                default:
                    return false;
            }
        }

        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is null)
                writer.WriteNull();
            else if (value is float)
            {
                float number = (float)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is decimal)
            {
                decimal number = (decimal)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is double)
            {
                double number = (double)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is short)
            {
                short number = (short)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is int)
            {
                int number = (int)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is long)
            {
                long number = (long)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is byte)
            {
                byte number = (byte)value;
                writer.WriteValue(number.ToString(CultureInfo.InvariantCulture));
            }
            else
                throw new NotSupportedException();
        }

        public override object ReadJson(
            JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            JValue jValue = new JValue(reader.Value);
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Decimal:
                            return decimal.Parse((string)jValue, CultureInfo.InvariantCulture);
                        case TypeCode.Double:
                            return double.Parse((string)jValue, CultureInfo.InvariantCulture);
                        case TypeCode.Single:
                            return float.Parse((string)jValue, CultureInfo.InvariantCulture);
                        default:
                            throw new NotSupportedException();
                    }
                case JsonToken.Integer:
                case JsonToken.Float:
                    return (float)jValue.ToObject(type);
                default:
                    throw new NotSupportedException();
            }
        }
    }
    internal sealed class PaymentConfirmationConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(PaymentConfirmation);
        }

        public override object ReadJson(
            JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            var jobj = serializer.Deserialize<JObject>(reader);
            var typeConfirmation = jobj["type"].Value<string>();

            var obj = new PaymentConfirmation();

            switch (typeConfirmation)
            {
                case "external":
                    obj = new PaymentConfirmationExternal();
                    break;
                case "qr":
                    obj = new PaymentConfirmationQrCode();
                    break;
                case "redirect":
                    obj = new PaymentConfirmationRedirect();
                    break;
                case "embedded":
                    obj = new PaymentConfirmationEmbedded();
                    break;
            }

            serializer.Populate(jobj.CreateReader(), obj);
            return obj;
        }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
