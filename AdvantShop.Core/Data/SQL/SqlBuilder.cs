using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.SQL2
{
    public class SqlBuilder
    {
        private readonly SqlWhere _whereCondition;
        private readonly List<SqlParam> _listParams;
        private readonly List<SqlCritera> _selectFields;
        private readonly List<SqlCritera> _orderFields;
        private string _tablename;
        private readonly List<string> _joinTable;
        private int? _topCount;

        public SqlBuilder()
        {
            _whereCondition = new SqlWhere();
            _listParams = new List<SqlParam>();
            _selectFields = new List<SqlCritera>();
            _orderFields = new List<SqlCritera>();
            _joinTable = new List<string>();
        }

        public string GetQuery()
        {
            var select = _selectFields.Select(x => x.SelectSql()).AggregateString(", ");

            var orderBy = _orderFields.Count > 0
                ? "ORDER BY " + _orderFields.Select(x => x.OrderSql(byOrigName: true)).AggregateString(", ")
                : null;

            return string.Join(" \n",
                new string[]
                {
                    "SELECT " + (_topCount.HasValue ? $"TOP({_topCount.Value})" : string.Empty),
                    select,
                    $"FROM {_tablename}",
                    _joinTable.AggregateString(" "),
                    _whereCondition.Sql(),
                    orderBy
                }.Where(x => x.IsNotEmpty()));
        }

        public object GetSqlParamsObject()
        {
            return _listParams.Select(x => new KeyValuePair<string, object>(x.ParamName, x.ParamValue)).ToArray();
        }

        public SqlParameter[] GetSqlParams()
        {
            return _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray();
        }

        //public List<T> GetItems<T>(Func<SqlDataReader, T> function)
        //{
        //    List<T> items;
        //    var query = 
        //    items =
        //        function == null
        //            ? SQLDataAccess.Query<T>(query,
        //                _listParams.Select(x => new KeyValuePair<string, object>(x.ParamName, x.ParamValue)).ToArray())
        //                .ToList()
        //            : SQLDataAccess.ExecuteReadList<T>(query, CommandType.Text, function,
        //                _listParams.Select(x => new SqlParameter(x.ParamName, x.ParamValue)).ToArray());

        //    return items;
        //}

        private string[] GetParamString(params object[] args)
        {
            if (args == null) return new string[0];

            var returnArr = new string[args.Length];
            var i = 0;

            foreach (var arg in args)
            {
                var argArr = arg as Array;
                if (argArr != null)
                    foreach (var argItem in argArr)
                    {
                        var pName = $"@p{_listParams.Count}";
                        _listParams.Add(new SqlParam { ParamName = pName, ParamValue = argItem });
                        if (!string.IsNullOrEmpty(returnArr[i]))
                            returnArr[i] += ",";
                        returnArr[i] += pName;
                    }
                else
                {
                    var pName = $"@p{_listParams.Count}";
                    _listParams.Add(new SqlParam { ParamName = pName, ParamValue = arg });
                    returnArr[i] = pName;
                }
                i++;
            }
            return returnArr.ToArray();
        }

        public SqlBuilder Top(int count)
        {
            _topCount = count;
            return this;
        }

        public SqlBuilder Select(params SqlCritera[] field)
        {
            _selectFields.AddRange(field);
            return this;
        }

        public SqlBuilder Where(SqlWhereCondition condition, params object[] args)
        {
            var temp = GetParamString(args);

            _whereCondition.Add(condition, temp);

            return this;
        }

        private SqlBuilder OrderBy(SqlSort sort, params SqlCritera[] condition)
        {
            foreach (var item in condition)
            {
                item.SetSort(sort);
                _orderFields.Add(item);
            }

            return this;
        }

        public SqlBuilder OrderBy(params SqlCritera[] condition)
        {
            return OrderBy(SqlSort.Asc, condition);
        }

        public SqlBuilder OrderByDesc(params SqlCritera[] condition)
        {
            return OrderBy(SqlSort.Desc, condition);
        }

        private SqlBuilder Join(string joinTable, params object[] args)
        {
            var temp = GetParamString(args);
            _joinTable.Add(string.Format(joinTable, temp));

            return this;
        }

        public SqlBuilder InnerJoin(string joinTable, params object[] args)
        {
            return Join($"INNER JOIN {joinTable}", args);
        }

        public SqlBuilder LeftJoin(string joinTable, params object[] args)
        {
            return Join($"LEFT JOIN {joinTable}", args);
        }

        public SqlBuilder CrossApply(string joinTable, params object[] args)
        {
            return Join($"CROSS APPLY {joinTable}", args);
        }
        
        public SqlBuilder OuterApply(string joinTable, params object[] args)
        {
            return Join($"OUTER APPLY {joinTable}", args);
        }

        public SqlBuilder From(string tableName)
        {
            _tablename = tableName;
            return this;
        }
    }
}