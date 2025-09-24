using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AdvantShop.Core.Services.Crm.OzonSeller.Api
{
    /// <summary>
    /// Запрашивает коллекцию объектво частями, по мере чтения.
    /// </summary>
    /// <typeparam name="TR">Тип объектво списка.</typeparam>
    public class EntityList<TR, TD> : BaseApiClient, IEnumerator<TR>, IEnumerable<TR>
    {
        private TypeResult _typeContainer;
        private TD _data;
        private List<TR> _collection;
        private int _index;
        private readonly string _url;
        private readonly int _limit;
        private readonly bool _keepActualCollection; // хранить только последнюю полученную коллекцию, чтобы не хранить всю коллекцию в памяти
        private int _offset;
        private int _page;
        private readonly ParamSystem _paramSystem;
        private PropertyInfo _propOffset;
        private PropertyInfo _propOffsetValue;
        private PropertyInfo _propPage;
        private List<TR> _lastData;
        private TR _current;

        public static EntityList<TR, TD> FactoryOffsetParam(string clientId, string apiKey, string url, TD data,
            Expression<Func<TD, object>> expressionPropertyOffset,
            Expression<Func<TD, object>> expressionPropertyLimit,
            TypeResult typeContainer = TypeResult.BaseResultList,
            bool keepActualCollection = false)
        {
            return new EntityList<TR, TD>(clientId, apiKey, url, data,
                propertyNameOffset: GetPropertyName<TD>(expressionPropertyOffset),
                propertyNameLimit: GetPropertyName<TD>(expressionPropertyLimit),
                typeContainer: typeContainer,
                keepActualCollection: keepActualCollection);
        }

        public static EntityList<TR, TD> FactoryOffsetValueParam(string clientId, string apiKey, string url, TD data,
            Expression<Func<TD, object>> expressionPropertyOffset,
            Expression<Func<TD, object>> expressionPropertyLimit,
            Expression<Func<TR, object>> expressionPropertyOffsetValue,
            TypeResult typeContainer = TypeResult.BaseResultList,
            bool keepActualCollection = false)
        {
            return new EntityList<TR, TD>(clientId, apiKey, url, data,
                propertyNameOffset: GetPropertyName<TD>(expressionPropertyOffset),
                propertyNameLimit: GetPropertyName<TD>(expressionPropertyLimit),
                propertyNameOffsetValue: GetPropertyName<TR>(expressionPropertyOffsetValue),
                typeContainer: typeContainer,
                keepActualCollection: keepActualCollection);
        }

        public static EntityList<TR, TD> FactoryPageParam(string clientId, string apiKey, string url, TD data,
            Expression<Func<TD, object>> expressionPropertyPage,
            TypeResult typeContainer = TypeResult.BaseResultList,
            bool keepActualCollection = false)
        {
            return new EntityList<TR, TD>(clientId, apiKey, url, data,
                propertyNamePage: GetPropertyName<TD>(expressionPropertyPage),
                typeContainer: typeContainer,
                keepActualCollection: keepActualCollection);
        }

        #region Private methods

        private static string GetPropertyName<T>(Expression<Func<T, object>> expressionProperty)
        {
            MemberInfo member = null;

            var unaryExpression = expressionProperty.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                var memberExpression = unaryExpression.Operand as MemberExpression;
                if (memberExpression != null)
                    member = memberExpression.Member;
            }

            if (member == null)
            {
                var memberExpression = expressionProperty.Body as MemberExpression;
                if (memberExpression != null)
                    member = memberExpression.Member;
            }

            if (member != null)
                return member.Name;

            throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
        }
        #endregion

        public EntityList(string clientId, string apiKey, string url, TD data, string propertyNamePage = null,
            TypeResult typeContainer = TypeResult.BaseResultList, bool keepActualCollection = false)
            : base(clientId, apiKey)
        {
            //if (limit < 0 || limit > 100)
            //    throw new ArgumentException(nameof(limit));

            if (string.IsNullOrEmpty(propertyNamePage))
                throw new ArgumentNullException("propertyNamePage");

            _data = data;
            _index = -1;
            _url = url;
            _keepActualCollection = keepActualCollection;
            _paramSystem = ParamSystem.Page;
            _typeContainer = typeContainer;

            var typeData = typeof(TD);
            var propertiesData = typeData.GetProperties();
            _propPage = propertiesData.First(x => x.Name == propertyNamePage);

            if (_propPage == null)
                throw new ArgumentException("Not found property page");

            _propPage.SetValue(_data, 0);
        }

        /// <param name="url">Размещение списка</param>
        /// <param name="limit">Максимальное количество сущностей для извлечения.</param>
        /// <param name="keepActualCollection">Хранение в памяти только последней полученной коллекции. Оптималь подходит для единоразового прочтения коллекции.</param>

        /// <param name="url">Размещение списка</param>
        /// <param name="limit">Максимальное количество сущностей для извлечения.</param>
        /// <param name="keepActualCollection">Хранение в памяти только последней полученной коллекции. Оптималь подходит для единоразового прочтения коллекции.</param>
        public EntityList(string clientId, string apiKey, string url, TD data, string propertyNameOffset, string propertyNameLimit, 
            bool offsetIsValueLastItem = false, string propertyNameOffsetValue = null,
            TypeResult typeContainer = TypeResult.BaseResultList, bool keepActualCollection = false)
            : base(clientId, apiKey)
        {
            //if (limit < 0 || limit > 100)
            //    throw new ArgumentException(nameof(limit));

            if (string.IsNullOrEmpty(propertyNameOffset))
                throw new ArgumentNullException("propertyNameOffset");
            if (string.IsNullOrEmpty(propertyNameLimit))
                throw new ArgumentNullException("propertyNameLimit");

            _data = data;
            _index = -1;
            _url = url;
            _keepActualCollection = keepActualCollection;
            _paramSystem = offsetIsValueLastItem ? ParamSystem.OffsetByLastValue : ParamSystem.Offset;
            _typeContainer = typeContainer;

            var typeData = typeof(TD);
            var propertiesData = typeData.GetProperties();
            _propOffset = propertiesData.First(x => x.Name == propertyNameOffset);
            var propLimit = propertiesData.First(x => x.Name == propertyNameLimit);

            if (_propOffset == null)
                throw new ArgumentException("Not found property offset");
            if (propLimit == null)
                throw new ArgumentException("Not found property limit");

            _propOffset.SetValue(_data, 0);
            _limit = (int)propLimit.GetValue(_data);

            if (_paramSystem == ParamSystem.OffsetByLastValue)
            {
                var typeResult = typeof(TR);
                var propertiesResult = typeResult.GetProperties();

                _propOffsetValue = propertiesResult.First(x => x.Name == propertyNameOffsetValue);

                if (_propOffsetValue == null)
                    throw new ArgumentException("Not found property offset value");
                if (_propOffsetValue.PropertyType != _propOffset.PropertyType)
                    throw new ArgumentException("Type property offset is not equals property offset value");
            }
        }

        public bool MoveNext()
        {
            if (_collection == null || (_index + 1 >= _collection.Count && _lastData.Count > 0))
            {
                object response = null;
                int countReRequest = 0;
                if (_paramSystem == ParamSystem.Offset)
                    _propOffset.SetValue(_data, _offset);
                else if (_paramSystem == ParamSystem.OffsetByLastValue && _lastData != null)
                    _propOffset.SetValue(_data, _propOffsetValue.GetValue(_lastData.Last()));
                else if (_paramSystem == ParamSystem.Page)
                    _propPage.SetValue(_data, _page++);

                do
                {
                    if (_typeContainer == TypeResult.BaseResultList)
                        response = MakeRequest<BaseResultList<TR>, TD>(_url, _data);
                    else if (_typeContainer == TypeResult.BaseResultItemsList)
                        response = MakeRequest<BaseResultItemsList<TR>, TD>(_url, _data);

                    if (countReRequest > 0)
                        System.Threading.Thread.Sleep(100);
                    countReRequest++;
                } while (response == null && LastActionIsBadGateway && countReRequest <= 10);

                if (response != null)
                {
                    _collection = _collection ?? (_collection = new List<TR>());
                    if (_keepActualCollection)
                    {
                        _collection.Clear();
                        _index = -1;
                    }

                    _collection.AddRange(GetResult(response));

                    _offset += _limit;
                    _lastData = GetResult(response);
                }
            }
            if (_collection == null || ++_index >= _collection.Count)
            {
                return false;
            }
            _current = _collection[_index];
            return true;
        }

        public void Reset()
        {
            _index = -1;
            if (_keepActualCollection)
            {
                _collection = null;
                _lastData = null;
                _offset = 0;
                _page = 0;
            }
        }

        public TR Current
        {
            get { return _current; }
        }


        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose() { }

        public IEnumerator<TR> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private enum ParamSystem
        {
            Offset,
            OffsetByLastValue,
            Page
        }

        private List<TR> GetResult(object response)
        {
            if (_typeContainer == TypeResult.BaseResultList)
                return ((BaseResultList<TR>)response).Result;

            if (_typeContainer == TypeResult.BaseResultItemsList)
                return ((BaseResultItemsList<TR>)response).Result.Items;

            return null;
        }
    }

    public enum TypeResult
    {
        BaseResultList,
        BaseResultItemsList
    }
}
