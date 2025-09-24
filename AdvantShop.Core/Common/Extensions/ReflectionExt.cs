using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Common.Extensions
{
    public static class ReflectionExt
    {
        private const int CacheTime = 6*60;

        #region Field

        /// <summary>
        /// Проверяет имеет ли текущий объект поле с указанным <paramref name="name"/>
        /// </summary>
        /// <param name="name">Название поля</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        public static bool HasField(this object obj, string name, BindingFlags? bindingAttr = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            var fieldInfo = obj.GetFieldInfo(name, bindingAttr);

            return fieldInfo != null;
        }

        /// <summary>
        /// Получает значение поля объекта <paramref name="obj"/> с указанным <paramref name="name"/>
        /// </summary>
        /// <param name="name">Название поля</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <returns>Значение поля</returns>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null.</exception>
        /// <exception cref="ArgumentException">Если у объекта <paramref name="obj"/> отсуствует поле с указанным <paramref name="name"/></exception>
        /// <returns>Значение поля</returns>
        public static object GetFieldValue(this object obj, string name, BindingFlags? bindingAttr = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            var fieldInfo = obj.GetFieldInfo(name, bindingAttr);
            if (fieldInfo == null)
                throw new ArgumentException($"Не найдено поле с указанным названием \"{name}\"", nameof(name));

            return GetFieldValue(obj, fieldInfo);
        }
        
        /// <summary>
        /// Получает значение поля <paramref name="fieldInfo"/> объекта <paramref name="obj"/>
        /// </summary>
        /// <param name="fieldInfo">Метаданные поля</param>
        /// <returns>Значение поля</returns>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="fieldInfo"/> равен null.</exception>
        /// <returns>Значение поля</returns>
        public static object GetFieldValue(this object obj, FieldInfo fieldInfo)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            fieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));

            return fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// Получает значение поля объекта <paramref name="obj"/> с указанным <paramref name="name"/>
        /// </summary>
        /// <param name="name">Название поля</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <typeparam name="T">Тип значения поля</typeparam>
        /// <returns>Значение поля</returns>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        /// <exception cref="ArgumentException">Если у объекта <paramref name="obj"/> отсуствует поле с указанным <paramref name="name"/></exception>
        public static T GetFieldValue<T>(this object obj, string name, BindingFlags? bindingAttr = null)
        {
            // throws InvalidCastException if types are incompatible
            return (T) GetFieldValue(obj, name, bindingAttr);
        }

        /// <summary>
        /// Устанавливает значение поля <paramref name="name"/> указанного объекта <paramref name="obj"/>.
        /// </summary>
        /// <param name="name">Название поля</param>
        /// <param name="value">Значение</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null.</exception>
        /// <exception cref="ArgumentException">Если у объекта <paramref name="obj"/> отсуствует поле с указанным <paramref name="name"/></exception>
        public static void SetFieldValue(this object obj, string name, object value, BindingFlags? bindingAttr = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            var fieldInfo = obj.GetFieldInfo(name, bindingAttr);
            if (fieldInfo == null)
                throw new ArgumentException($"Не найдено поле с указанным названием \"{name}\"", nameof(name));

            SetFieldValue(obj, fieldInfo, value);
        }
        
        /// <summary>
        /// Устанавливает значение поля <paramref name="fieldInfo"/> указанного объекта <paramref name="obj"/>.
        /// </summary>
        /// <param name="fieldInfo">Метаданные поля</param>
        /// <param name="value">Значение</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="fieldInfo"/> равен null.</exception>
        public static void SetFieldValue(this object obj, FieldInfo fieldInfo, object value)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            fieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
   
            fieldInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Выполняет поиск указанного поля, используя заданные ограничения привязки– или – открытого поля.
        /// </summary>
        /// <param name="name">Название поля</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <returns>Объект, представляющий открытое поле с заданным именем, если такой метод найден; в противном случае — значение null.</returns>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        public static FieldInfo GetFieldInfo(this object obj, string name, BindingFlags? bindingAttr)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            Type type = obj.GetType();
            var fieldInfo = bindingAttr.HasValue
                ? type.GetField(name, bindingAttr.Value)
                : type.GetField(name);
            return fieldInfo;
        }

        #endregion Field

        #region Property

        /// <summary>
        /// Проверяет имеет ли текущий объект свойство с указанным <paramref name="name"/>
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        public static bool HasProp(this object obj, string name, BindingFlags? bindingAttr = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            var propertyInfo = obj.GetPropertyInfo(name, bindingAttr);

            return propertyInfo != null;
        }

        /// <summary>
        /// Ищет указанное свойство, используя заданные ограничения привязки– или – открытого свойства
        /// и возвращает значение свойства заданного объекта с дополнительными значениями индекса для индексированных свойств.
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <param name="index">Необязательные значения индекса для индексированных свойств. Для неиндексированных свойств это значение должно быть равно null.</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        /// <exception cref="ArgumentException">Если у объекта <paramref name="obj"/> отсуствует свойство с указанным <paramref name="name"/></exception>
        /// <returns>Значение свойства</returns>
        public static object GetPropValue(this object obj, string name, BindingFlags? bindingAttr = null, object[] index = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            var propertyInfo = obj.GetPropertyInfo(name, bindingAttr);
            if (propertyInfo == null)
                throw new ArgumentException($"Не найдено свойство с указанным названием \"{name}\"", nameof(name));

            return GetPropValue(obj, propertyInfo, index);
        }
        
        /// <summary>
        /// Возвращает значение свойства заданного объекта с дополнительными значениями индекса для индексированных свойств.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства</param>
        /// <param name="index">Необязательные значения индекса для индексированных свойств. Для неиндексированных свойств это значение должно быть равно null.</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="propertyInfo"/> равен null</exception>
        /// <returns>Значение свойства</returns>
        public static object GetPropValue(this object obj, PropertyInfo propertyInfo, object[] index = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            return propertyInfo.GetValue(obj, index);
        }

        /// <summary>
        /// Ищет указанное свойство, используя заданные ограничения привязки– или – открытого свойства
        /// и возвращает значение свойства заданного объекта с дополнительными значениями индекса для индексированных свойств.
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <param name="index">Необязательные значения индекса для индексированных свойств. Для неиндексированных свойств это значение должно быть равно null.</param>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        /// <exception cref="ArgumentException">Если у объекта <paramref name="obj"/> отсуствует свойство с указанным <paramref name="name"/></exception>
        /// <exception cref="InvalidCastException">Если типы несовместимы</exception>
        /// <returns>Значение свойства</returns>
        public static T GetPropValue<T>(this object obj, string name, BindingFlags? bindingAttr = null, object[] index = null)
        {
            var propValue = GetPropValue(obj, name, bindingAttr, index);

            // throws InvalidCastException if types are incompatible
            return (T)propValue;
        }

        /// <summary>
        /// Устанавливает значение свойства указанного объекта <paramref name="obj"/> с дополнительными значениями индекса для индексированных свойств.
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <param name="value">Значение</param>
        /// <param name="bindingAttr">Битовая маска, составленная из одного или нескольких объектов <see cref="System.Reflection.BindingFlags"/> и указывающая, как ведется поиск.– или – Нуль, чтобы было возвращено значение null.</param>
        /// <param name="index">Необязательные значения индекса для индексированных свойств. Для неиндексированных свойств это значение должно быть равно null.</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        /// <exception cref="ArgumentException">Если у объекта <paramref name="obj"/> отсуствует свойство с указанным <paramref name="name"/></exception>
        public static void SetPropValue(this object obj, string name, object value, BindingFlags? bindingAttr = null, object[] index = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            var propertyInfo = obj.GetPropertyInfo(name, bindingAttr);
            if (propertyInfo == null)
                throw new ArgumentException($"Не найдено свойство с указанным названием \"{name}\"", nameof(name));

            SetPropValue(obj, propertyInfo, value, index);
        }
         
        /// <summary>
        /// Устанавливает значение свойства <paramref name="propertyInfo"/> указанного объекта <paramref name="obj"/> с дополнительными значениями индекса для индексированных свойств.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства</param>
        /// <param name="value">Значение</param>
        /// <param name="index">Необязательные значения индекса для индексированных свойств. Для неиндексированных свойств это значение должно быть равно null.</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="propertyInfo"/> равен null.</exception>
        public static void SetPropValue(this object obj, PropertyInfo propertyInfo, object value, object[] index = null)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            propertyInfo.SetValue(obj, value, index);
        }
       
        /// <summary>
        /// Ищет указанное свойство, используя заданные ограничения привязки– или – открытого свойства.
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <exception cref="ArgumentNullException">Если один из параметров <paramref name="obj"/>– или –<paramref name="name"/> равен null</exception>
        public static PropertyInfo GetPropertyInfo(this object obj, string name, BindingFlags? bindingAttr)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));
            name = name ?? throw new ArgumentNullException(nameof(name));

            Type type = obj.GetType();
            PropertyInfo info = bindingAttr.HasValue
                ? type.GetProperty(name, bindingAttr.Value)
                : type.GetProperty(name);
            return info;
        }

        #endregion Property

        public static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit) where TAttribute : Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }

        public static Type GetTypeByAttributeValue<TAttribute>(Type t, Func<TAttribute, object> pred, object oValue)
        {
            var cacheName = "TypeByAttribute_" + t.ToString() + pred.ToString() + oValue.ToString();

            // todo: уместно ли тут кэширование?
            return CacheManager.Get(cacheName, CacheTime, () =>
            {
                var type = typeof (TAttribute);
                var currentType =
                    AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.GlobalAssemblyCache && a.FullName.StartsWith("AdvantShop"))
                        .SelectMany(s => s.GetTypes()).Where(x =>
                            t.IsAssignableFrom(x) &&
                            x.GetCustomAttributes(type, true)
                                .Cast<TAttribute>()
                                .Any(oTemp => Equals(pred(oTemp), oValue))).ToList();

                if (currentType.Count > 1)
                    throw new Exception("duplicate for " + oValue);
                return currentType.FirstOrDefault();
            });
        }

        public static List<string> GetAttributeValue<TAttribute>(Type t)
        {
            var currentTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                                        .Where(x=> t.IsAssignableFrom(x) && x.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().Any()).ToList();
            var temp = currentTypes.Select(AttributeHelper.GetAttributeValue<TAttribute, string>).ToList();
            return temp;
        }

        public static T ToObject<T>(this IDictionary<string, string> source)
         where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, string> item in source)
            {
                var property = someObjectType.GetProperty(item.Key);
                if (property == null) continue;
                property.SetValue(someObject, item.Value, BindingFlags.GetProperty, null, null, CultureInfo.InvariantCulture);
            }

            return someObject;
        }

        public static Dictionary<string, string> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return new ObjectToDictionary(source, bindingAttr).ToDictionary().ToDictionary(pair => pair.Key, pair => Convert.ToString(pair.Value, CultureInfo.InvariantCulture));
        }
    }
}