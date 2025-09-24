using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdvantShop.Core.Common.Attributes;
using Newtonsoft.Json;

namespace AdvantShop.Core.Common
{
    #region Enums

    // Enum поддерживающие неизвестные значения
    // При десереализации не будет ложить все взаимодействие при новом не известном значении

    [JsonConverter(typeof(StringEnumObjectConverter))]
    public abstract class StringEnum<T> : IEquatable<T>, IComparable<T> where T : StringEnum<T>
    {
        /// <summary>
        ///     The enum item value.
        /// </summary>
        public readonly string Value;

        /// <summary>
        ///     The enum item constructor.
        /// </summary>
        /// <param name="value">The enum item value.</param>
        protected StringEnum(string value)
        {
            Value = value;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Compare enum items.
        /// </summary>
        /// <param name="other">The enum item value.</param>
        /// <returns>The comparison result.</returns>
        public bool Equals(T other)
        {
            if ((object)other == null) return false;
            return Value == other.Value;
        }

        public int CompareTo(T other)
        {
            if ((object)other == null) return 1;
            return String.Compare(this.Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        ///     To string.
        /// </summary>
        /// <returns>The enum item value.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        ///     Get all enum items.
        /// </summary>
        /// <returns>The enum items.</returns>
        public static List<T> AsList()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .Where(p => p.CanRead && p.PropertyType == typeof(T))
                .Select(p => (T)p.GetValue(null))
                .ToList();
        }

        /// <summary>
        ///     String to enum item.
        /// </summary>
        /// <param name="value">The enum item value.</param>
        /// <returns></returns>
        public static T Parse(string value)
        {
            var all = AsList();
            return all.All(a => a.Value != value) ? null : all.Single(a => a.Value == value);
        }

        public string Localize()
        {
            return AttributeHelper.GetAttributeValueProperty<LocalizeAttribute, string>(this) ?? Value;
        }

        /// <summary>
        ///     Compare enum items.
        /// </summary>
        /// <param name="obj">The enum item value.</param>
        /// <returns>The comparison result.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is T)
            {
                var other = (T)obj;
                return Equals(other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///     Get hash code.
        /// </summary>
        /// <returns>The enum item value hash code.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        ///     The equals operator.
        /// </summary>
        /// <param name="a">The enum item.</param>
        /// <param name="b">The enum item.</param>
        /// <returns>The comparison result.</returns>
        public static bool operator ==(StringEnum<T> a, StringEnum<T> b)
        {
            return (object)a != null ? a.Equals(b) : (object)a == (object)b;
        }

        /// <summary>
        ///     The not equals operator.
        /// </summary>
        /// <param name="a">The enum item.</param>
        /// <param name="b">The enum item.</param>
        /// <returns>The comparison result.</returns>
        public static bool operator !=(StringEnum<T> a, StringEnum<T> b)
        {
            return !((object)a != null ? a.Equals(b) : (object)a == (object)b);
        }
    }


    [JsonConverter(typeof(IntegerEnumObjectConverter))]
    public abstract class IntegerEnum<T> : IEquatable<T>, IComparable<T> where T : IntegerEnum<T>
    {
        /// <summary>
        ///     The enum item value.
        /// </summary>
        public readonly int Value;

        /// <summary>
        ///     The enum item constructor.
        /// </summary>
        /// <param name="value">The enum item value.</param>
        protected IntegerEnum(int value)
        {
            Value = value;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Compare enum items.
        /// </summary>
        /// <param name="other">The enum item value.</param>
        /// <returns>The comparison result.</returns>
        public bool Equals(T other)
        {
            if ((object)other == null) return false;
            return Value == other.Value;
        }

        public int CompareTo(T other)
        {
            if ((object)other == null) return 1;
            return this.Value.CompareTo(other.Value);
        }

        /// <summary>
        ///     To string.
        /// </summary>
        /// <returns>The enum item value.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        ///     Get all enum items.
        /// </summary>
        /// <returns>The enum items.</returns>
        public static List<T> AsList()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .Where(p => p.CanRead && p.PropertyType == typeof(T))
                .Select(p => (T)p.GetValue(null))
                .ToList();
        }

        public string Localize()
        {
            return AttributeHelper.GetAttributeValueProperty<LocalizeAttribute, string>(this) ?? Value.ToString();
        }

        /// <summary>
        ///     Integer to enum item.
        /// </summary>
        /// <param name="value">The enum item value.</param>
        /// <returns></returns>
        public static T Parse(int value)
        {
            var all = AsList();
            return all.All(a => a.Value != value) ? null : all.Single(a => a.Value == value);
        }

        /// <summary>
        ///     Compare enum items.
        /// </summary>
        /// <param name="obj">The enum item value.</param>
        /// <returns>The comparison result.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is T)
            {
                var other = (T)obj;
                return Equals(other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///     Get hash code.
        /// </summary>
        /// <returns>The enum item value hash code.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        ///     The equals operator.
        /// </summary>
        /// <param name="a">The enum item.</param>
        /// <param name="b">The enum item.</param>
        /// <returns>The comparison result.</returns>
        public static bool operator ==(IntegerEnum<T> a, IntegerEnum<T> b)
        {
            return (object)a != null ? a.Equals(b) : (object)a == (object)b;
        }

        /// <summary>
        ///     The not equals operator.
        /// </summary>
        /// <param name="a">The enum item.</param>
        /// <param name="b">The enum item.</param>
        /// <returns>The comparison result.</returns>
        public static bool operator !=(IntegerEnum<T> a, IntegerEnum<T> b)
        {
            return !((object)a != null ? a.Equals(b) : (object)a == (object)b);
        }
    }
    #endregion

    #region JsonConverters

    public class StringEnumObjectConverter : JsonConverter
    {

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.String)
            {
                return Activator.CreateInstance(objectType, new object[] { serializer.Deserialize<string>(reader) });
            }

            return null;
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value.ToString());
        }
    }

    public class IntegerEnumObjectConverter : JsonConverter
    {

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Integer)
            {
                return Activator.CreateInstance(objectType, new object[] { serializer.Deserialize<int>(reader) });
            }

            return null;
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

    #endregion
}
