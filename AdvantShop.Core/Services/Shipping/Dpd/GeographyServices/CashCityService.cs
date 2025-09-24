using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Shipping.Dpd.Api.Geography;

namespace AdvantShop.Shipping.Dpd.GeographyServices
{
    public class CashCityService
    {
        public static CashCity Get(long cityId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Shipping].[DpdCashCity] WHERE [CityId] = @CityId",
                CommandType.Text,
                FromReader,
                new SqlParameter("@CityId", cityId));
        }

        public static List<CashCity> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[DpdCashCity]",
                CommandType.Text,
                FromReader);
        }

        public static List<CashCity> Find(string countryCode, string region, string city, long? cityId)
        {
            if (cityId.HasValue)
            {
                return SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Shipping].[DpdCashCity] WHERE [CityId] = @CityId",
                    CommandType.Text,
                    FromReader,
                    new SqlParameter("@CityId", cityId.Value));
            }
            else if (countryCode.IsNotEmpty() || region.IsNotEmpty() || city.IsNotEmpty())
            {
                return CacheManager.Get(string.Format("DpdCashCities-Find-{0}", ((countryCode ?? string.Empty).ToLower() + (region ?? string.Empty).ToLower() + (city ?? string.Empty).ToLower()).GetHashCode()), 60, () =>
                {
                    var listParams = new List<SqlParameter>();
                    var where = new List<string>();

                    if (countryCode.IsNotEmpty())
                    {
                        listParams.Add(new SqlParameter("@CountryCode", countryCode));
                        where.Add("[CountryCode] = @CountryCode");
                    }

                    if (region.IsNotEmpty())
                    {
                        listParams.Add(new SqlParameter("@RegionName", region.RemoveTypeFromRegion()));
                        where.Add("[RegionName] = @RegionName");
                    }

                    if (city.IsNotEmpty())
                    {
                        listParams.Add(new SqlParameter("@CityName", city));
                        where.Add("[CityName] = @CityName");
                    }

                    return SQLDataAccess.ExecuteReadList(
                        "SELECT * FROM [Shipping].[DpdCashCity] WHERE " + string.Join(" and ", where),
                        CommandType.Text,
                        FromReader,
                        listParams.ToArray());
                });

            }
            return null;
        }

        public static CashCity FromReader(SqlDataReader reader)
        {
            return new CashCity
            {
                CityId = SQLDataHelper.GetLong(reader, "CityId"),
                CityName = SQLDataHelper.GetString(reader, "CityName"),
                Abbreviation = SQLDataHelper.GetString(reader, "Abbreviation"),
                RegionName = SQLDataHelper.GetString(reader, "RegionName"),
                CountryCode = SQLDataHelper.GetString(reader, "CountryCode"),
            };
        }

        public static bool ExistsCities()
        {
            return SQLDataAccess.ExecuteScalar<bool>("SELECT CASE WHEN EXISTS(SELECT [CityId] FROM [Shipping].[DpdCashCity]) THEN 1 ELSE 0 END", CommandType.Text);
        }

        public static void Add(CashCity cashCity)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Shipping].[DpdCashCity] ([CityId],[CityName],[RegionName],[CountryCode],[Abbreviation])
                VALUES
	                (@CityId,@CityName,@RegionName,@CountryCode,@Abbreviation)",
                CommandType.Text,
                new SqlParameter("@CityId", cashCity.CityId),
                new SqlParameter("@CityName", cashCity.CityName ?? (object)DBNull.Value),
                new SqlParameter("@RegionName", cashCity.RegionName ?? (object)DBNull.Value),
                new SqlParameter("@CountryCode", cashCity.CountryCode ?? (object)DBNull.Value),
                new SqlParameter("@Abbreviation", cashCity.Abbreviation ?? (object)DBNull.Value)
                );
        }

        public static void Update(CashCity cashCity)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Shipping].[DpdCashCity]
                   SET [CityName] = @CityName
                      ,[RegionName] = @RegionName
                      ,[CountryCode] = @CountryCode
                      ,[Abbreviation] = @Abbreviation
                 WHERE [CityId] = @CityId",
                CommandType.Text,
                new SqlParameter("@CityId", cashCity.CityId),
                new SqlParameter("@CityName", cashCity.CityName ?? (object)DBNull.Value),
                new SqlParameter("@RegionName", cashCity.RegionName ?? (object)DBNull.Value),
                new SqlParameter("@CountryCode", cashCity.CountryCode ?? (object)DBNull.Value),
                new SqlParameter("@Abbreviation", cashCity.Abbreviation ?? (object)DBNull.Value)
                );
        }

        public static void Delete(long cityId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Shipping].[DpdCashCity] WHERE [CityId] = @CityId",
                CommandType.Text,
                new SqlParameter("@CityId", cityId));
        }

        public static bool Sync(Api.DpdApiService dpdApiClient)
        {
            var citiesSource = dpdApiClient.GetCitiesCashPay() ?? new city[]{};
            var isEmptyCities = !ExistsCities();

            if (isEmptyCities == false)
            {
                var currentCities = GetList();

                foreach (var citySource in citiesSource)
                {
                    var city = Get(citySource.cityId);
                    var isNew = city == null;

                    if (city == null)
                        city = new CashCity();

                    city.CityId = citySource.cityId;
                    city.CityName = citySource.cityName;
                    city.RegionName = citySource.regionName.RemoveTypeFromRegion();
                    city.CountryCode = citySource.countryCode;
                    city.Abbreviation = citySource.abbreviation;


                    if (isNew)
                        Add(city);
                    else
                        Update(city);
                }

                // удаляем отсутствующие
                if (citiesSource.Length > 0)
                {
                    var citiesSourceIds = citiesSource.Select(x => x.cityId).ToList();
                    currentCities
                        .Where(x => !citiesSourceIds.Contains(x.CityId))
                        .ForEach(x => Delete(x.CityId));
                }
            } 
            else
            {
                var table = SQLDataAccess.ExecuteTable("SELECT [CityId],[CityName],[RegionName],[CountryCode],[Abbreviation] FROM [Shipping].[DpdCashCity]", CommandType.Text);
                foreach (var citySource in citiesSource)
                {
                    table.Rows.Add(
                        citySource.cityId,
                        citySource.cityName,
                        citySource.regionName.RemoveTypeFromRegion(),
                        citySource.countryCode,
                        citySource.abbreviation);

                    if (table.Rows.Count % 1000 == 0)
                        InsertBulk(table);
                }
                InsertBulk(table);
            }

            return true;
        }

        private static void InsertBulk(DataTable data)
        {
            if (data.Rows.Count > 0)
            {
                using (SqlConnection dbConnection = new SqlConnection(Connection.GetConnectionString()))
                {
                    dbConnection.Open();
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(dbConnection))
                    {
                        sqlBulkCopy.DestinationTableName = "[Shipping].[DpdCashCity]";
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }

    }
}
