using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdvantShop.Core.Common.Extensions
{
    public class ObjectToDictionary
    {
        protected readonly object Object;
        protected readonly BindingFlags? BindingFlags;

        public ObjectToDictionary(object o)
        {
            Object = o;
        }

        public ObjectToDictionary(object o, BindingFlags bindingFlags) : this(o)
        {
            BindingFlags = bindingFlags;
        }

        public virtual Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();
            GetValues(Object, dictionary);
            
            return dictionary;
        }

        protected virtual string GetPropertyName(string propertyName, string parentPropertyName)
            => string.IsNullOrEmpty(parentPropertyName)
                ? propertyName
                : $"{parentPropertyName}.{propertyName}";

        protected virtual string GetIEnumerableItemName(string entityName, int index)
            => $"{entityName}.{index}";
        
        protected virtual void GetValues(object obj, Dictionary<string, object> dictionary, string parentPropertyName = null)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            
            var type = obj.GetType();
            var properties =
                BindingFlags is null
                    ? type.GetProperties()
                    : type.GetProperties(BindingFlags.Value);
            
            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;
                if (propertyType.IsValueType
                    || propertyType == typeof(string)
                    || propertyType.IsEnum)
                {
                    dictionary.Add(GetPropertyName(propertyInfo.Name, parentPropertyName), GetValue(obj, propertyInfo));
                }
                else if (propertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    GetItemsByIEnumerable(obj, propertyInfo, dictionary, parentPropertyName);
                }
                else if (propertyType.IsClass)
                {
                    var value = propertyInfo.GetValue(obj);
                    if (value == null)
                        dictionary.Add(GetPropertyName(propertyInfo.Name, parentPropertyName), null);
                    else
                        GetValues(
                            value, 
                            dictionary, 
                            GetPropertyName(propertyInfo.Name, parentPropertyName)
                            );
                } 
                else
                {
                    throw new NotImplementedException(string.Format("Не реализована поддержка типа \"{0}\".", propertyType.FullName));
                }
            }
        }

        protected virtual void GetItemsByIEnumerable(object obj, PropertyInfo property, Dictionary<string, object> dictionary, string parentPropertyName)
        {
            Type typeEnumerableItem = null;

            if (property.PropertyType.IsConstructedGenericType &&
                property.PropertyType.GenericTypeArguments.Length == 1)
            {
                typeEnumerableItem = property.PropertyType.GenericTypeArguments[0];
            }
            else if (property.PropertyType.IsArray)
            {
                typeEnumerableItem = property.PropertyType.GetElementType();
            }

            if (typeEnumerableItem != null)
            {
                var values = obj == null ? null : (IEnumerable)property.GetValue(obj);

                if (obj == null || values == null)
                    dictionary.Add(GetPropertyName(property.Name, parentPropertyName), null);
                else
                {
                    int index = 0;
                    foreach (var enumerableItem in values)
                    {
                        GetValues(
                            enumerableItem, 
                            dictionary, 
                            GetIEnumerableItemName(GetPropertyName(property.Name, parentPropertyName), index)
                            );
                        index++;
                    }
                }
            }
            else 
                throw new NotImplementedException(string.Format("Не реализована поддержка типа \"{0}\".", property.PropertyType.FullName));        
        }

        protected virtual object GetValue(object objType, PropertyInfo property)
        {
            return property.GetValue(objType);
        }
    }
}