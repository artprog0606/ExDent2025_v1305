using System;
using System.Linq;
using System.Reflection;

namespace AdvantShop.Core.Common.Attributes
{
    public static class AttributeHelper
    {
        public static TReturn GetAttributeValue<TAttribute, TReturn>(object obj)
        {
            var attributeValue = default(TReturn);
            if (obj == null) return attributeValue;
            return GetAttributeValue<TAttribute, TReturn>(obj.GetType());
        }

        public static TReturn GetAttributeValue<TAttribute, TReturn>(Type obj)
        {
            var attributeValue = default(TReturn);

            if (obj == null) return attributeValue;
            var attributes = Attribute.GetCustomAttributes(obj).Where(x => x is TAttribute).ToList();

            if (attributes.Count <= 0) return attributeValue;
            var attribute = attributes[0] as IAttribute<TReturn>;

            if (attribute != null)
            {
                attributeValue = attribute.Value;
            }
            return attributeValue;
        }

        public static TReturn GetAttributeValueField<TAttribute, TReturn>(object obj)
        {
            var attributeValue = default(TReturn);

            if (obj == null) return attributeValue;
            var fi = obj.GetType().GetField(obj.ToString());

            if (fi == null) return attributeValue;
            var attributes = fi.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

            if (attributes == null || attributes.Length <= 0) return attributeValue;
            var attribute = attributes[0] as IAttribute<TReturn>;

            if (attribute != null)
            {
                attributeValue = attribute.Value;
            }
            return attributeValue;
        }

        public static TReturn GetAttributeValueProperty<TAttribute, TReturn>(object obj)
        {
            var attributeValue = default(TReturn);

            if (obj == null) return attributeValue;
            var fi = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .SingleOrDefault(p => p.CanRead && p.GetValue(null).Equals(obj));

            if (fi == null) return attributeValue;
            var attributes = fi.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

            if (attributes == null || attributes.Length <= 0) return attributeValue;
            var attribute = attributes[0] as IAttribute<TReturn>;

            if (attribute != null)
            {
                attributeValue = attribute.Value;
            }
            return attributeValue;
        }
        
        public static IFormatKeyAttribute<TReturn> GetFormatKeyAttributeValue<TAttribute, TReturn>(object obj)
        {
            if (obj == null) return null;
            
            var fi = obj.GetType().GetField(obj.ToString());

            if (fi == null) return null;
            var attributes = fi.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

            if (attributes == null || attributes.Length <= 0) return null;
            var attribute = attributes[0] as IFormatKeyAttribute<TReturn>;

            return attribute;
        }
    }
}