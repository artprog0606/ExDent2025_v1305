using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Common;

namespace AdvantShop.Core.SQL2
{
    public enum SqlSort
    {
        None,
        Asc,
        Desc
    }

    public class SqlParam
    {
        public string ParamName { get; set; }
        public object ParamValue { get; set; }
    }

    public class SqlCritera
    {
        private readonly string _prefix;
        private readonly string _name;
        private readonly string _nameAs;
        private readonly string _origName;
        private SqlSort _sort;

        public SqlCritera(string name, string nameAs, SqlSort sort)
        {
            var items = name.Split('.');

            _name = items.Length == 2 ? items[1] : name;
            _prefix = items.Length == 2 ? items[0] : "";
            _nameAs = nameAs;
            _sort = sort;
            _origName = name;
        }

        public string WhereSql()
        {
            return _name;
        }

        public string SelectSql()
        {
            return _origName + (_nameAs.IsNotEmpty() ? $" as {_nameAs}" : null);
        }

        public string OrderSql(bool byOrigName = false)
        {
            return _nameAs.Default(byOrigName ? _origName : _name) + " " + (_sort == SqlSort.None ? SqlSort.Asc : _sort);
        }

        public static implicit operator SqlCritera(string nameField)
        {
            return new SqlCritera(nameField, "", SqlSort.None);
        }

        public void SetSort(SqlSort sort)
        {
            _sort = sort;
        }

        public string FieldName => string.IsNullOrWhiteSpace(_nameAs) ? _name.ToLower() : _nameAs.ToLower();
    }

    public class SqlWhereCondition
    {
        private string _condition;

        public SqlWhereCondition(string condition)
        {
            _condition = condition;
        }

        public bool IgnoreInCustomData { get; set; }

        public SqlWhereCondition(string condition, bool ignoreInCustomData) : this(condition)
        {
            IgnoreInCustomData = ignoreInCustomData;
        }

        public string Sql()
        {
            return _condition;
        }

        public bool Valid()
        {
            return !string.IsNullOrEmpty(_condition);
        }

        public static implicit operator SqlWhereCondition(string condition)
        {
            return new SqlWhereCondition(condition);
        }
    }

    public static class SqlWhereConditions
    {
        public static SqlWhereCondition IgnoreInCustomData(this string val)
        {
            return new SqlWhereCondition(val, true);
        }
    }

    public class SqlWhere : List<SqlWhereCondition>
    {
        public SqlWhere() : base() { }

        public void Add(SqlWhereCondition condition, params string[] args)
        {
            if (condition == null || !condition.Valid())
                return;

            var sql = condition.Sql();
            if (this.Count != 0 && !sql.ToLower().StartsWith("and "))
                sql = "AND " + sql;

            base.Add(new SqlWhereCondition(string.Format(sql, args), condition.IgnoreInCustomData));
        }

        public string Sql()
        {
            return this.Count > 0 
                ? " WHERE " + this.Select(c => c.Sql()).AggregateString(" ") 
                : "";
        }

        public string SqlCustomData()
        {
            return this.Count(c => !c.IgnoreInCustomData) > 0 
                ? " WHERE " + this.Where(c => !c.IgnoreInCustomData).Select(c => c.Sql()).AggregateString(" ")
                : "";
        }
    }

    public static class SqlFields
    {
        public static SqlCritera AsSqlField(this string val, string nameAs)
        {
            var model = new SqlCritera(val, nameAs, SqlSort.None);
            return model;
        }
    }

    public class SqlPaging
    {
        private readonly string _cacheNameKey;
        private readonly bool _useCache;

        private const string Advparam = "@p";
        private readonly SqlWhere _whereCondition;
        private readonly List<SqlParam> _listParams;
        private readonly List<SqlCritera> _selectFields;
        private readonly List<SqlCritera> _orderFields;
        private string _tableName;
        private readonly List<string> _joinTable;

        private readonly int? _limit;
        public int ItemsPerPage { get; set; }
        public int CurrentPageIndex { get; set; }


        public SqlPaging()
            : this(1, 10, "", false, null)
        {
        }

        public SqlPaging(string cacheNameKey)
            : this(1, 10, cacheNameKey, true, null)
        {
        }

        public SqlPaging(string cacheNameKey, int? limit)
        : this(1, 10, cacheNameKey, true, limit)
        {
        }


        public SqlPaging(int currentPageIndex, int itemsPerPage)
            : this(currentPageIndex, itemsPerPage, "", false, null)
        {
        }

        public SqlPaging(int currentPageIndex, int itemsPerPage, string cacheNameKey, bool useCache, int? limit)
        {
            CurrentPageIndex = currentPageIndex;
            ItemsPerPage = itemsPerPage;
            _whereCondition = new SqlWhere();
            _listParams = new List<SqlParam>();
            _selectFields = new List<SqlCritera>();
            _orderFields = new List<SqlCritera>();
            _tableName = string.Empty;
            _joinTable = new List<string>();
            _cacheNameKey = cacheNameKey;
            _useCache = useCache;
            _limit = limit ?? 0;
        }

        public int PageCount(int rowsCount, int itemsPerPage)
        {
            return (int)(Math.Ceiling((double)rowsCount / itemsPerPage));
        }

        public int PageCount(int rowsCount)
        {
            return (int)(Math.Ceiling((double)rowsCount / ItemsPerPage));
        }

        public int PageCount()
        {
            return (int)(Math.Ceiling((double)TotalRowsCount / ItemsPerPage));
        }

        private int? _totalRowsCount = null;
        public int TotalRowsCount
        {
            get
            {
                if (_selectFields.Count == 0) throw new Exception("set any select fields");

                if (_totalRowsCount.HasValue)
                    return _totalRowsCount.Value;

                var query = "SELECT COUNT( " + /*_selectFields.First().SelectSql()*/ "*" + ") FROM "
                            + _tableName
                            + _joinTable.Aggregate(" ", (a, b) => a + " " + b)
                            + _whereCondition.Sql();

                var cacheName = _useCache ? CacheNames.SQlPagingCountCacheName(_cacheNameKey, query, _listParams.Select(p => p.ParamName + p.ParamValue).AggregateString("")) : null;

                if (_useCache && CacheManager.TryGetValue(cacheName, out _totalRowsCount))
                    return _totalRowsCount.Value;

                _totalRowsCount = ExecuteRetryDeadlock(() => 
                    SQLDataAccess.ExecuteScalar<int>(query, CommandType.Text, _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray()));

                if (_limit > 0 && _totalRowsCount > _limit)
                {
                    _totalRowsCount = _limit;
                }
                if (_useCache)
                    CacheManager.Insert(cacheName, _totalRowsCount);

                return _totalRowsCount.Value;
            }
        }

        public bool LimitReached => _limit != 0 && _limit == _totalRowsCount;

        public DataTable PageItems
        {
            get
            {
                var needRow = CurrentPageIndex * ItemsPerPage;
                var keyid = (CurrentPageIndex - 1) * ItemsPerPage;

                var selecStr = _selectFields.Select(x => x.SelectSql())
                             .Union(_orderFields.Where(o => _selectFields.All(s => s.FieldName != o.FieldName)).Select(x => x.SelectSql()))
                             .AggregateString(", ");

                var order =
                    _orderFields.Count > 0
                        ? _orderFields.Select(x => x.OrderSql()).AggregateString(", ")
                        : _selectFields[0].OrderSql();


                var query = string.Format("WITH TEMP " +
                             "AS ( " +
                                 "SELECT {7} {0} " +
                                 "FROM {1} {2} {3}" +
                                 ")" +
                              "SELECT * " +
                              "FROM ( " +
                                    "SELECT TOP ({4}) Row_Number() OVER ( " +
                                            "ORDER BY {5} " +
                                            ") AS RowNum " +
                                        ",* " +
                                    "FROM TEMP " +
                                    ") AS t " +
                            "WHERE RowNum > {6} ",
                             selecStr,
                            _tableName,
                            _joinTable.AggregateString(" "),
                            _whereCondition.Sql(),
                            needRow,
                            order,
                            keyid,
                            _limit != 0 ? "Top " + _limit : ""
                            );



                var tbl = ExecuteRetryDeadlock(() => 
                    SQLDataAccess.ExecuteTable(query, CommandType.Text,
                    _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray()));
                return tbl;

            }
        }

        public List<T> PageItemsList<T>()
        {
            return PageItemsList<T>(null);
        }

        public List<T> PageItemsList<T>(Func<SqlDataReader, T> function)
        {
            var select =
                _selectFields
                    .Select(x => x.SelectSql())
                    .Union(_orderFields.Where(o => _selectFields.All(s => s.FieldName != o.FieldName)).Select(x => x.SelectSql()))
                    .AggregateString(", ");

            var order =
                _orderFields.Count > 0
                    ? _orderFields.Select(x => x.OrderSql()).AggregateString(", ") // .Union(_selectFields.Take(1).Select(x => x.OrderSql())).AggregateString(", ")
                    : _selectFields[0].OrderSql();
            
            var rowsFrom = (CurrentPageIndex - 1) * ItemsPerPage;
            var rowsTo = CurrentPageIndex * ItemsPerPage;

            var query = 
                string.Format(
                    "WITH TEMP AS ( " +
                        "SELECT {0} {1} " +
                        "FROM {2} {3} {4}" +
                    ")" +
                    "SELECT * " +
                    "FROM ( SELECT Row_Number() OVER ( ORDER BY {5} ) AS RowNum, * FROM TEMP ) AS t " +
                    "WHERE RowNum > {6} AND RowNum <= {7}",
                    _limit != 0 ? "Top " + _limit : "",
                    select,
                    _tableName,
                    _joinTable.AggregateString(" "),
                    _whereCondition.Sql(),
                    order,
                    rowsFrom,
                    rowsTo
                );

            var cacheName = 
                _useCache 
                    ? CacheNames.SQlPagingItemsCacheName(_cacheNameKey, query, _listParams.Select(p => p.ParamName + p.ParamValue).AggregateString("")) 
                    : null;

            if (_useCache && CacheManager.TryGetValue(cacheName, out List<T> items))
                return items;

            items =
                function == null
                    ? ExecuteRetryDeadlock(() => 
                        SQLDataAccess
                            .Query<T>(query, _listParams.Select(x => new KeyValuePair<string, object>(x.ParamName, x.ParamValue)).ToArray())
                            .ToList())
                    : ExecuteRetryDeadlock(() => 
                        SQLDataAccess
                            .ExecuteReadList<T>(query, CommandType.Text, function,
                                _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray()));

            if (_useCache)
                CacheManager.Insert(cacheName, items);

            return items;
        }
        
        public List<T> PageItemsListByField<T>(string indexField, Func<SqlDataReader, T> function = null)
        {
            var rowsTo = CurrentPageIndex * ItemsPerPage;
            var rowsFrom = (CurrentPageIndex - 1) * ItemsPerPage;

            var selectFields = _selectFields.Select(x => x.SelectSql()).AggregateString(", ");
            var joinTables = _joinTable.AggregateString(" ");
            
            var orderByFields =
                _orderFields.Count > 0
                    ? _orderFields.Select(x => x.OrderSql()).AggregateString(", ")
                    : _selectFields[0].OrderSql();
            
            var orderByFieldsInCteSelect =
                _orderFields.Count > 0
                    ? ", " + _orderFields.Select(x => x.SelectSql()).AggregateString(", ")
                    : "";

            var indexFieldArr = indexField.Split('.');
            var indexFieldName = indexFieldArr.Length > 1 ? indexFieldArr[1] : indexFieldArr[0];
            var joinFromTable = $"Inner Join {_tableName} On temp.{indexFieldName} = {indexField} ";
            
            var query = 
                string.Format("WITH cte AS (SELECT {0} {1} FROM {2} {3} {4} ORDER BY {5}) " +
                              "SELECT {6} " +
                              "FROM (SELECT Row_Number() OVER (ORDER BY {5}) AS RowNum,* FROM cte) AS temp " +
                              "{7} " +
                              "WHERE RowNum > {8} AND RowNum <= {9}",
                        _limit != 0 ? "Top " + _limit : "",
                        indexField + orderByFieldsInCteSelect,
                        _tableName,
                        joinTables,
                        _whereCondition.Sql(),
                        orderByFields,
                        selectFields,
                        joinFromTable + joinTables,
                        rowsFrom,
                        rowsTo);

            var cacheName = _useCache ? CacheNames.SQlPagingItemsCacheName(_cacheNameKey, query, _listParams.Select(p => p.ParamName + p.ParamValue).AggregateString("")) : null;

            if (_useCache && CacheManager.TryGetValue(cacheName, out List<T> items))
                return items;

            items =
                function == null
                    ? ExecuteRetryDeadlock(() => 
                        SQLDataAccess.Query<T>(query,
                            _listParams.Select(x => new KeyValuePair<string, object>(x.ParamName, x.ParamValue)).ToArray())
                            .ToList())
                    : ExecuteRetryDeadlock(() => 
                        SQLDataAccess.ExecuteReadList<T>(query, CommandType.Text, function,
                            _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray()));

            if (_useCache)
                CacheManager.Insert(cacheName, items);

            return items;
        }

        public int GetRowsCount(string field)
        {
            if (_totalRowsCount.HasValue)
                return _totalRowsCount.Value;

            var query = "SELECT COUNT(" + field + ") FROM "
                        + _tableName
                        + _joinTable.Aggregate(" ", (a, b) => a + " " + b)
                        + _whereCondition.Sql();

            var cacheName = _useCache
                ? CacheNames.SQlPagingCountCacheName(_cacheNameKey, query,
                    _listParams.Select(p => p.ParamName + p.ParamValue).AggregateString(""))
                : null;

            if (_useCache && CacheManager.TryGetValue(cacheName, out _totalRowsCount) && _totalRowsCount != null)
                return _totalRowsCount.Value;

            _totalRowsCount = ExecuteRetryDeadlock(() => 
                SQLDataAccess.ExecuteScalar<int>(query, CommandType.Text, 
                    _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray()));

            if (_limit > 0 && _totalRowsCount > _limit)
            {
                _totalRowsCount = _limit;
            }
            if (_useCache)
                CacheManager.Insert(cacheName, _totalRowsCount);

            return _totalRowsCount.Value;
        }

        public List<T> GetCustomData<T>(string selectFields, string newCondition, Func<IDataReader, T> getFromReader, bool useDistinct, string jointable = "")
        {
            var query = String.Format("SELECT {5}{0} FROM {1} {2} {3} {4}",
                selectFields, _tableName,
                _joinTable.AggregateString(" ") + " " + jointable,
                _whereCondition.SqlCustomData(),
                newCondition,
                useDistinct ? "Distinct " : ""
                );

            var table = ExecuteRetryDeadlock(() => 
                SQLDataAccess.ExecuteReadList(query, CommandType.Text, getFromReader,
                    _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray()));
            return table;
        }
        
        public List<T> GetCustomDataCached<T>(string cacheNameKey, string selectFields, string whereCondition, Func<IDataReader, T> getFromReader, bool useDistinct, string jointable = null)
        {
            var query = string.Format("SELECT {5}{0} FROM {1} {2} {3} {4}",
                selectFields, 
                _tableName,
                _joinTable.AggregateString(" ") + " " + jointable,
                _whereCondition.SqlCustomData(),
                whereCondition,
                useDistinct ? "Distinct " : ""
            );
            
            var cacheName = CacheNames.GetCustomDataCacheName(cacheNameKey, query, _listParams.Select(p => p.ParamName + p.ParamValue).AggregateString(""));

            return CacheManager.Get(cacheName, () =>
                ExecuteRetryDeadlock(
                    () => SQLDataAccess.ExecuteReadList(query, CommandType.Text,
                        getFromReader,
                        _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray())
                ));
        }

        public List<T> ItemsIds<T>(string fieldName)
        {
            var query =
                string.Format("SELECT {0} FROM {1} {2} {3}",
                    fieldName,
                    _tableName,
                    _joinTable.Aggregate(" ", (a, b) => a + " " + b),
                    _whereCondition.Sql());

            return ExecuteRetryDeadlock(() =>  
                SQLDataAccess.Query<T>(query,
                        _listParams.Select(x => new KeyValuePair<string, object>(x.ParamName, x.ParamValue)).ToArray(), CommandType.Text)
                        .ToList());
        }

        private string[] GetParamString(params object[] args)
        {
            if (args == null) return Array.Empty<string>();

            var returnArr = new string[args.Length];
            var i = 0;

            foreach (var arg in args)
            {
                if (arg is Array argArr)
                {
                    foreach (var argItem in argArr)
                    {
                        var temp = Advparam + _listParams.Count;
                        _listParams.Add(new SqlParam { ParamName = temp, ParamValue = argItem });

                        if (!string.IsNullOrEmpty(returnArr[i]))
                            returnArr[i] += ",";
                        returnArr[i] += temp;
                    }
                }
                else
                {
                    var temp = Advparam + _listParams.Count;
                    _listParams.Add(new SqlParam { ParamName = temp, ParamValue = arg });
                    
                    returnArr[i] = temp;
                }
                i++;
            }
            return returnArr.ToArray();
        }

        public SqlPaging Select(params SqlCritera[] field)
        {
            _selectFields.AddRange(field);
            return this;
        }

        // new version
        public SqlPaging Where(SqlWhereCondition condition, params object[] args)
        {
            var temp = GetParamString(args);

            _whereCondition.Add(condition, temp);

            return this;
        }

        // old version. don't break modules
        public SqlPaging Where(string condition, params object[] args)
        {
            var temp = GetParamString(args);

            _whereCondition.Add(condition, temp);

            return this;
        }

        private SqlPaging OrderBy(SqlSort sort, params SqlCritera[] condition)
        {
            foreach (var item in condition)
            {
                item.SetSort(sort);
                _orderFields.Add(item);
            }

            return this;
        }

        public SqlPaging OrderBy(params SqlCritera[] condition)
        {
            return OrderBy(SqlSort.Asc, condition);
        }

        public SqlPaging OrderByDesc(params SqlCritera[] condition)
        {
            return OrderBy(SqlSort.Desc, condition);
        }
        
        private SqlPaging Join(string joinTable)
        {
            _joinTable.Add(joinTable);
            return this;
        }
        
        private SqlPaging Join(string joinTable, params object[] args)
        {
            var temp = GetParamString(args);
            _joinTable.Add(string.Format(joinTable, temp));

            return this;
        }

        public SqlPaging Inner_Join(string joinTable)
        {
            return Join("Inner Join " + joinTable);
        }
        
        public SqlPaging Inner_Join(string joinTable, params object[] args)
        {
            return Join("Inner Join " + joinTable, args);
        }

        public SqlPaging Left_Join(string joinTable)
        {
            return Join("Left Join " + joinTable);
        }
        
        public SqlPaging Left_Join(string joinTable, params object[] args)
        {
            return Join("Left Join " + joinTable, args);
        }

        public SqlPaging Outer_Apply(string joinTable)
        {
            return Join("Outer Apply " + joinTable);
        }

        public SqlPaging Outer_Apply(string joinTable, params object[] args)
        {
            return Join("Outer Apply " + joinTable, args);
        }

        public SqlPaging ClearJoins()
        {
            _joinTable.Clear();
            return this;
        }

        public SqlPaging From(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public List<SqlCritera> SelectFields()
        {
            return _selectFields;
        }

        private T ExecuteRetryDeadlock<T>(Func<T> action)
        {
            return RetryHelper.Do(
                action,
                retryInterval: TimeSpan.FromMilliseconds(100),
                retryCount: 2,
                ReturnException.LastException,
                funcHandleException: exception => (exception as SqlException)?.Number is 1205);
        }
    }
}