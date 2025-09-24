//--------------------------------------------------
// Project: AdvantShop.NET (AVERA)
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using Dapper;

namespace AdvantShop.Core.SQL
{
    public class SQLDataAccess2
    {
        public static IEnumerable<T> ExecuteReadIEnumerable<T>(string sql, object obj = null, CommandType? commandType = null, string connectionString = null)
        {
            using (var connection = ConnectionFactory.CreateConnection(connectionString))
            {
                return connection.Query<T>(sql, obj, commandType: commandType);
            }
        }

        public static T Query<T>(string sql, object obj = null, CommandType? commandType = null, string connectionString = null)
        {
            using (var connection = ConnectionFactory.CreateConnection(connectionString))
            {
                return connection.Query<T>(sql, obj, commandType: commandType).FirstOrDefault();
            }
        }

        public static T ExecuteScalar<T>(string sql, object obj = null, CommandType? commandType = null, string connectionString = null)
        {
            DataModificationFlag.SetLastModifiedSql(sql, commandType ?? CommandType.Text);
            
            using (var connection = ConnectionFactory.CreateConnection(connectionString))
            {
                return connection.ExecuteScalar<T>(sql, obj, commandType: commandType);
            }
        }

        public static void ExecuteNonQuery(string sql, object obj = null, CommandType? commandType = null, string connectionString = null)
        {
            DataModificationFlag.SetLastModifiedSql(sql, commandType ?? CommandType.Text);
            
            using (var connection = ConnectionFactory.CreateConnection(connectionString))
            {
                connection.Execute(sql, obj, commandType: commandType);
            }
        }

    }

    /// <summary>
    /// Use to DB access into main internal functions. Class implements as IDisposable, use "using" statement is recommended
    /// </summary>
    /// <remarks></remarks>
    public class SQLDataAccess : IDisposable
    {
        public SqlCommand cmd;
        public SqlConnection cn;

        /// <summary>
        /// Define the internalSQLConnection with default connectionString
        /// </summary>
        /// <remarks></remarks>
        public SQLDataAccess()
        {
            cn = new SqlConnection(Connection.GetConnectionString());
            cmd = new SqlCommand { Connection = cn };
        }

        /// <summary>
        /// Define the internalSQLConnection with custom connectionString
        /// </summary>
        /// <param name="strConnectionString"></param>
        /// <remarks></remarks>
        public SQLDataAccess(string strConnectionString, int commandTimeout = 60)
        {
            cn = new SqlConnection(strConnectionString);
            cmd = new SqlCommand { Connection = cn, CommandTimeout = commandTimeout };
        }


        private void Inititialize(string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();

            if (parameters != null && parameters.Any(param => param != null && param.Value == null))
                throw new NoNullAllowedException("param name: " + parameters.Where(p => p != null && p.Value == null).Select(p => p.ParameterName).AggregateString(","));

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.Where(param => param != null).ToArray());

            //DataModificationFlag.SetLastModifiedSql(commandText, commandType);
        }

        /// <summary>
        /// Executes command with given parameters and returns first value
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            DataModificationFlag.SetLastModifiedSql(commandText, commandType);
            
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);
                db.cnOpen();
                return db.cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Executes command with given parameters and returns first value
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static TResult ExecuteScalar<TResult>(string commandText, CommandType commandType,
                                                     params SqlParameter[] parameters) where TResult : IConvertible
        {
            DataModificationFlag.SetLastModifiedSql(commandText, commandType);
            
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                object o = db.cmd.ExecuteScalar();
                return o is IConvertible ? (TResult)Convert.ChangeType(o, typeof(TResult)) : default(TResult);
            }
        }

        public static TResult ExecuteScalar<TResult>(string commandText, CommandType commandType, int commandTimeout,
                                                     params SqlParameter[] parameters) where TResult : IConvertible
        {
            DataModificationFlag.SetLastModifiedSql(commandText, commandType);
            
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cmd.CommandTimeout = commandTimeout;

                db.cnOpen();
                object o = db.cmd.ExecuteScalar();
                return o is IConvertible ? (TResult)Convert.ChangeType(o, typeof(TResult)) : default(TResult);
            }
        }
        
        public static void ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            DataModificationFlag.SetLastModifiedSql(commandText, commandType);
            
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                db.cmd.ExecuteNonQuery();
                db.cnClose();
            }
        }
        
        public static void ExecuteNonQuery(string commandText, CommandType commandType, int commandTimeout = 60, params SqlParameter[] parameters)
        {
            DataModificationFlag.SetLastModifiedSql(commandText, commandType);
            
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cmd.CommandTimeout = commandTimeout;

                db.cnOpen();
                db.cmd.ExecuteNonQuery();
                db.cnClose();
            }
        }

        //may be in future for async
        //public static async Task ExecuteNonQueryAsync(string commandText, CommandType commandType, params SqlParameter[] parameters)
        //{
        //    using (var conn = new SqlConnection(Connection.GetConnectionString() + "async=True;"))
        //{
        //    await conn.OpenAsync();
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        cmd.Connection = conn;
        //        cmd.CommandType = commandType;
        //        cmd.CommandText = commandText;
        //        if (parameters != null)
        //        db.cmd.Parameters.AddRange(parameters);
        //        return await cmd.ExecuteScalarAsync();
        //    }
        //}
        //}


        public static Dictionary<TKey, TValue> ExecuteReadDictionary<TKey, TValue>(string commandText, CommandType commandType, string keyColumnName,
            string valueColumnName, TKey defaultkey, TValue defaultValue, params SqlParameter[] parameters)
            where TKey : IConvertible
            where TValue : IConvertible
        {
            var res = new Dictionary<TKey, TValue>();
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(SQLDataHelper.GetValue(reader, keyColumnName, defaultkey), SQLDataHelper.GetValue(reader, valueColumnName, defaultValue));
                    }
                }
            }
            return res;
        }

        public static Dictionary<TKey, TValue> ExecuteReadDictionary<TKey, TValue>(string commandText, CommandType commandType,
                                                                                   string keyColumnName, string valueColumnName, params SqlParameter[] parameters)
            where TKey : IConvertible
            where TValue : IConvertible
        {
            return ExecuteReadDictionary(commandText, commandType, keyColumnName, valueColumnName, default(TKey), default(TValue), parameters);
        }

        public static Dictionary<TKey, TValue> ExecuteReadDictionary<TKey, TValue>(string commandText, CommandType commandType,
                                                               string keyColumnName, TKey defaultkey, Func<SqlDataReader, TValue> function,
                                                               params SqlParameter[] parameters) where TKey : IConvertible
        {
            var res = new Dictionary<TKey, TValue>();
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(SQLDataHelper.GetValue(reader, keyColumnName, defaultkey), function(reader));
                    }
                }
            }
            return res;
        }

        public static Dictionary<TKey, TValue> ExecuteReadDictionary<TKey, TValue>(string commandText, CommandType commandType, string keyColumnName,
                                                                                    Func<SqlDataReader, TValue> function, params SqlParameter[] parameters) where TKey : IConvertible
        {
            return ExecuteReadDictionary(commandText, commandType, keyColumnName, default(TKey), function, parameters);
        }

        public static List<TResult> ExecuteReadColumn<TResult>(string commandText, CommandType commandType, string columnName, TResult defaultValue, int commandTimeout,
                                                               params SqlParameter[] parameters) where TResult : IConvertible
        {
            var res = new List<TResult>();
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cmd.CommandTimeout = commandTimeout;

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(SQLDataHelper.GetValue(reader, columnName, defaultValue));
                    }
                }
                db.cnClose();
            }
            return res;
        }

        public static List<TResult> ExecuteReadColumn<TResult>(string commandText, CommandType commandType, string columnName, TResult defaultValue,
                                                               params SqlParameter[] parameters) where TResult : IConvertible
        {
            return ExecuteReadColumn<TResult>(commandText, commandType, columnName, defaultValue, 60, parameters);
        }

        public static List<TResult> ExecuteReadColumn<TResult>(string commandText, CommandType commandType,
                                                               string columnName, params SqlParameter[] parameters) where TResult : IConvertible
        {
            return ExecuteReadColumn(commandText, commandType, columnName, default(TResult), parameters);
        }


        private static IEnumerable<TResult> ExecuteReadColumnIEnumerable<TResult>(string commandText, CommandType commandType, string columnName, TResult defaultValue,
                                                                                 params SqlParameter[] parameters) where TResult : IConvertible
        {
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return SQLDataHelper.GetValue(reader, columnName, defaultValue);
                    }
                }
                db.cnClose();
            }
        }


        public static IEnumerable<TResult> ExecuteReadColumnIEnumerable<TResult>(string commandText, CommandType commandType, string columnName,
                                                                                 params SqlParameter[] parameters) where TResult : IConvertible
        {

            return ExecuteReadColumnIEnumerable(commandText, commandType, columnName, default(TResult), parameters);
        }

        public static List<TResult> ExecuteReadList<TResult>(string commandText, CommandType commandType, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            var res = new List<TResult>();
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(function(reader));
                    }
                }
                db.cnClose();
            }
            return res;
        }


        public static HashSet<TResult> ExecuteReadHashSet<TResult>(string commandText, CommandType commandType, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            var res = new HashSet<TResult>();
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(function(reader));
                    }
                }
                db.cnClose();
            }
            return res;
        }

        public static HashSet<TResult> ExecuteReadHashSet<TResult>(string commandText, CommandType commandType, int commandTimeout, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            var res = new HashSet<TResult>();
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cmd.CommandTimeout = commandTimeout;

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(function(reader));
                    }
                }
                db.cnClose();
            }
            return res;
        }

        private static IEnumerable<TResult> ExecuteReadIEnumerable<TResult>(string commandText, CommandType commandType,
            Func<SqlDataReader, TResult> function, TResult defaultValue,
            params SqlParameter[] parameters)
        {
            return ExecuteReadIEnumerable<TResult>(commandText, commandType, null, function, defaultValue, parameters);
        }

        private static IEnumerable<TResult> ExecuteReadIEnumerable<TResult>(string commandText, CommandType commandType, int? commandTimeout, Func<SqlDataReader, TResult> function, TResult defaultValue,
                                                                           params SqlParameter[] parameters)
        {
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);
                if (commandTimeout.HasValue)
                    db.cmd.CommandTimeout = commandTimeout.Value;

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return function != null ? function(reader) : defaultValue;
                    }
                }
                db.cnClose();
            }
        }

        public static IEnumerable<TResult> ExecuteReadIEnumerable<TResult>(string commandText, CommandType commandType, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            return ExecuteReadIEnumerable(commandText, commandType, null, function, default(TResult), parameters);
        }

        public static IEnumerable<TResult> ExecuteReadIEnumerable<TResult>(string commandText, CommandType commandType, int commandTimeout, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            return ExecuteReadIEnumerable(commandText, commandType, commandTimeout, function, default(TResult), parameters);
        }

        public static TResult ExecuteReadOne<TResult>(string commandText, CommandType commandType, Func<SqlDataReader, TResult> function, params SqlParameter[] parameters)
        {
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    return reader.Read() ? function(reader) : default(TResult);
                }
            }
        }

        public static void ExecuteForeach(string commandText, CommandType commandType, Action<SqlDataReader> mapFunction,
                                          params SqlParameter[] parameters)
        {
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                using (var reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mapFunction(reader);
                    }
                    reader.Close();
                }
                db.cnClose();
            }
        }

        /// <summary>
        /// return table of nonstruct data
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteTable(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var db = new SQLDataAccess())
            {
                db.Inititialize(commandText, commandType, parameters);

                db.cnOpen();
                var da = new SqlDataAdapter(db.cmd);
                var tbl = new DataTable();
                da.Fill(tbl);
                db.cnClose();
                return tbl;
            }
        }

        public static IEnumerable<T> Query<T>(string sql, object obj = null, CommandType? commandType = null)
        {
#if DEBUG
            Stopwatch sw = null;
            if (SettingProvider.GetConfigSettingValue("Profiling") == "true")
            {
                sw = new Stopwatch();
                sw.Start();
            }
#endif
            using (var connection = ConnectionFactory.CreateConnection())
            {
                var result = connection.Query<T>(sql, obj, commandType: commandType);

#if DEBUG
                if (HttpContext.Current != null && sw != null &&
                    SettingProvider.GetConfigSettingValue("Profiling") == "true")
                {
                    sw.Stop();
                    var profiler = HttpContext.Current.Items["MiniProfiler_Sql"] as List<Profiling> ?? new List<Profiling>();
                    profiler.Add(new Profiling(sql, new List<KeyValuePair<string, object>>(), sw.Elapsed.TotalMilliseconds));
                }
#endif
                return result;
            }
        }
        
        #region Connection functions

#if DEBUG
        private Stopwatch _stopWatch = null;

        private void StartProfiling()
        {
            if (SettingProvider.GetConfigSettingValue("Profiling") == "true")
            {
                _stopWatch = new Stopwatch();
                _stopWatch.Start();
            }
        }

        private void StopProfiling()
        {
            if (HttpContext.Current != null && _stopWatch != null &&
                SettingProvider.GetConfigSettingValue("Profiling") == "true")
            {
                _stopWatch.Stop();

                var profiler = HttpContext.Current.Items["MiniProfiler_Sql"] as List<Profiling> ??
                               new List<Profiling>();

                var parametres =
                    cmd.Parameters.Cast<SqlParameter>()
                        .Select(x => new KeyValuePair<string, object>(x.ParameterName, x.Value))
                        .ToList();

                profiler.Add(new Profiling(cmd.CommandText, parametres, _stopWatch.Elapsed.TotalMilliseconds));

                HttpContext.Current.Items["MiniProfiler_Sql"] = profiler;
            }
        }
#endif

        /// <summary>
        /// Open connection to SQL DB
        /// </summary>
        /// <remarks></remarks>
        public void cnOpen()
        {
            //try
            //{
#if DEBUG
            StartProfiling();
#endif

            //if (cn.State != ConnectionState.Open)
            //{
            cn.Open();
            //}
            //}
            //catch (Exception ex)
            //{
            //    Debug.Log.Error(ex);
            //    Dispose(true);
            //    throw;
            //}
        }

        /// <summary>
        /// Close connection to DB
        /// </summary>
        /// <remarks></remarks>
        public void cnClose()
        {
            if ((cn != null) && (cn.State != ConnectionState.Closed))
            {
                cn.Close();
            }
        }

        /// <summary>
        /// Get status of current connection
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public ConnectionState cnStatus()
        {
            return cn.State;
        }

        #endregion

        #region  IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        // IDisposable

        ~SQLDataAccess()// the finalizer
        {
            Dispose(false);
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue && disposing)
            {
                if (cn.State != ConnectionState.Closed)
                    cn.Close();

#if DEBUG
                StopProfiling();
#endif

                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
                if (cn != null)
                {
                    cn.Dispose();
                    cn = null;
                }
            }
            _disposedValue = true;
        }

        #endregion
    }
}