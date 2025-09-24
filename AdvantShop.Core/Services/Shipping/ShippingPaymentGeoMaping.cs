//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core;
using System.Collections.Generic;
using AdvantShop.Core.SQL;
using AdvantShop.Repository;
using AdvantShop.Core.Caching;
using AdvantShop.Helpers;

namespace AdvantShop
{
    public class ShippingPaymentGeoMaping
    {
        //******* IsExist
        public static bool IsExistPaymentCountry(int methodId, int countryId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].PaymentCountry where MethodId=@MethodId and CountryId=@CountryId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            ) > 0;
        }

        public static bool IsExistShippingCountry(int methodId, int countryId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].ShippingCountry where MethodId=@MethodId and CountryId=@CountryId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            ) > 0;
        }

        public static bool IsExistPaymentCity(int methodId, int countryId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].PaymentCity where MethodId=@MethodId and CityId=@CityId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = countryId }
                                            ) > 0;
        }

        public static bool IsExistShippingCity(int methodId, int cityId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].ShippingCity where MethodId=@MethodId and CityId=@CityId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = cityId }
                                            ) > 0;
        }

        public static bool IsExistShippingCityExcluded(int methodId, int cityId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].ShippingCityExcluded where MethodId=@MethodId and CityId=@CityId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = cityId }
                                            ) > 0;
        }

        public static bool IsExistShippingRegion(int methodId, int regionId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].ShippingRegion where MethodId=@MethodId and RegionId=@RegionId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@RegionId", Value = regionId }
                                            ) > 0;
        }

        public static bool IsExistShippingRegionExcluded(int methodId, int regionId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].ShippingRegionExcluded where MethodId=@MethodId and RegionId=@RegionId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@RegionId", Value = regionId }
                                            ) > 0;
        }

        public static bool IsExistShippingCountryExcluded(int methodId, int countryId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(*) from [order].ShippingCountryExcluded where MethodId=@MethodId and CountryId=@CountryId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            ) > 0;
        }

        //********** Add
        public static void AddPaymentCountry(int methodId, int countryId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].PaymentCountry (MethodId,CountryId) values (@MethodId,@CountryId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            );
        }

        public static void AddShippingCountry(int methodId, int countryId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].ShippingCountry (MethodId,CountryId) values (@MethodId,@CountryId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void AddPaymentCity(int methodId, int countryId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].PaymentCity (MethodId,CityId) values (@MethodId,@CityId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = countryId }
                                            );
        }

        public static void AddShippingCity(int methodId, int cityId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].ShippingCity (MethodId,CityId) values (@MethodId,@CityId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = cityId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void AddShippingCityExcluded(int methodId, int cityId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].ShippingCityExcluded (MethodId,CityId) values (@MethodId,@CityId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = cityId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void AddShippingRegion(int methodId, int regionId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].ShippingRegion (MethodId,RegionId) values (@MethodId,@RegionId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@RegionId", Value = regionId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void AddShippingRegionExcluded(int methodId, int regionId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].ShippingRegionExcluded (MethodId,RegionId) values (@MethodId,@RegionId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@RegionId", Value = regionId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void AddShippingCountryExcluded(int methodId, int countryId)
        {
            SQLDataAccess.ExecuteNonQuery("insert into [order].ShippingCountryExcluded (MethodId,CountryId) values (@MethodId,@CountryId)",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        //********** Delete
        public static void DeletePaymentCountry(int methodId, int countryId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].PaymentCountry where MethodId=@MethodId and CountryId=@CountryId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            );
        }

        public static void DeleteShippingCountry(int methodId, int countryId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].ShippingCountry where MethodId=@MethodId  and CountryId=@CountryId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = countryId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void DeletePaymentCity(int methodId, int cityId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].PaymentCity where MethodId=@MethodId and CityId=@CityId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = cityId }
                                            );
        }

        public static void DeleteShippingRegion(int methodId, int regionId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].ShippingRegion where MethodId=@MethodId and RegionId=@RegionId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@RegionId", Value = regionId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void DeleteShippingRegionExcluded(int methodId, int regionId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].ShippingRegionExcluded where MethodId=@MethodId and RegionId=@RegionId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@RegionId", Value = regionId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void DeleteShippingCity(int methodId, int cityId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].ShippingCity where MethodId=@MethodId and CityId=@CityId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = cityId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void DeleteShippingCityExcluded(int methodId, int cityId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].ShippingCityExcluded where MethodId=@MethodId and CityId=@CityId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CityId", Value = cityId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        public static void DeleteShippingCountryExcluded(int methodId, int CountryId)
        {
            SQLDataAccess.ExecuteNonQuery("delete from [order].ShippingCountryExcluded where MethodId=@MethodId and CountryId=@CountryId",
                                            CommandType.Text,
                                            new SqlParameter { ParameterName = "@MethodId", Value = methodId },
                                            new SqlParameter { ParameterName = "@CountryId", Value = CountryId }
                                            );
            CacheManager.RemoveByPattern(CacheNames.ShippingForCityAndCountry + methodId);
        }

        //****** Get by Shipping
        public static List<Country> GetCountryByShippingId(int shippingId)
        {
            return SQLDataAccess.ExecuteReadList<Country>("select * from [Customers].Country where CountryID in (select CountryID from [Order].ShippingCountry where MethodId=@MethodId)",
                                                            CommandType.Text,
                                                            CountryService.GetCountryFromReader,
                                                            new SqlParameter { ParameterName = "@MethodId", Value = shippingId }
                                                            );
        }

        public static List<IpZone> GetRegionsByShippingId(int shippingId)
        {
            return GetRegionsByShippingId(shippingId, false);
        }

        public static List<IpZone> GetRegionsByShippingIdExcluded(int shippingId)
        {
            return GetRegionsByShippingId(shippingId, true);
        }

        public static List<IpZone> GetRegionsByShippingId(int shippingId, bool excluded)
        {
            return SQLDataAccess.ExecuteReadList<IpZone>(
                "SELECT RegionName, RegionID, Country.CountryId, " +
                "(case when (select count(*) from [Customers].[Region] where RegionName = Regions.RegionName) > 1 then (Country.CountryName) else '' end) as CountryName " +
                "FROM Customers.Region as Regions INNER JOIN Customers.Country ON Country.CountryID = Regions.CountryID " +
                "WHERE RegionId IN (SELECT RegionID FROM [Order]." + (excluded ? "ShippingRegionExcluded" : "ShippingRegion") + " WHERE MethodId = @MethodId)",
                CommandType.Text, reader => new IpZone
                {
                    RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId"),
                    CountryName = SQLDataHelper.GetString(reader, "CountryName"),
                },
                new SqlParameter("@MethodId", shippingId));
        }

        public static List<IpZone> GetCityByShippingId(int shippingId)
        {
            return GetCitiesByShippingId(shippingId, false);
        }

        public static List<IpZone> GetCityByShippingIdExcluded(int shippingId)
        {
            return GetCitiesByShippingId(shippingId, true);
        }

        public static List<IpZone> GetCitiesByShippingId(int shippingId, bool excluded)
        {
            return SQLDataAccess.ExecuteReadList<IpZone>(
                "SELECT CityName, CityID, Region.RegionId, CountryId, District, " +
                "(CASE WHEN (SELECT COUNT(*) FROM [Customers].[City] WHERE CityName = Cities.CityName) > 1 THEN (Region.RegionName) ELSE '' END) AS RegionName " +
                "FROM [Customers].City as Cities INNER JOIN Customers.Region ON Region.RegionID = Cities.RegionId " +
                "WHERE CityID IN (SELECT CityID FROM [Order]." + (excluded ? "ShippingCityExcluded" : "ShippingCity") + " WHERE MethodId = @MethodId)",
                CommandType.Text, reader => new IpZone
                {
                    CityId = SQLDataHelper.GetInt(reader, "CityID"),
                    City = SQLDataHelper.GetString(reader, "CityName"),
                    District = SQLDataHelper.GetString(reader, "District"),
                    RegionId = SQLDataHelper.GetInt(reader, "RegionId"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId")
                },
                new SqlParameter("@MethodId", shippingId));
        }

        public static List<Country> GetCountryByShippingIdExcluded(int shippingId)
        {
            return SQLDataAccess.ExecuteReadList<Country>("select * from [Customers].Country where CountryID in (select CountryID from [Order].ShippingCountryExcluded where MethodId=@MethodId)",
                                                            CommandType.Text,
                                                            CountryService.GetCountryFromReader,
                                                            new SqlParameter { ParameterName = "@MethodId", Value = shippingId }
                                                            );
        }

        //***** Get By Payment
        public static List<Country> GetCountryByPaymentId(int paymentId)
        {
            return SQLDataAccess.ExecuteReadList<Country>("select * from [Customers].Country where CountryID in (select CountryID from [Order].PaymentCountry where MethodId=@MethodId)",
                                                            CommandType.Text,
                                                            CountryService.GetCountryFromReader,
                                                            new SqlParameter { ParameterName = "@MethodId", Value = paymentId }
                                                            );
        }

        public static List<IpZone> GetCityByPaymentId(int paymentId)
        {
            return SQLDataAccess.ExecuteReadList<IpZone>(
                "SELECT CityName, CityID, Region.RegionId, CountryId, District, " +
                "(CASE WHEN (SELECT COUNT(*) FROM [Customers].[City] WHERE CityName = Cities.CityName) > 1 THEN (Region.RegionName) ELSE '' END) AS RegionName " +
                "FROM [Customers].City as Cities INNER JOIN Customers.Region ON Region.RegionID = Cities.RegionId " +
                "WHERE CityID IN (SELECT CityID FROM [Order].PaymentCity WHERE MethodId = @MethodId)",
                CommandType.Text, reader => new IpZone
                {
                    CityId = SQLDataHelper.GetInt(reader, "CityID"),
                    City = SQLDataHelper.GetString(reader, "CityName"),
                    District = SQLDataHelper.GetString(reader, "District"),
                    RegionId = SQLDataHelper.GetInt(reader, "RegionId"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId")
                },
                new SqlParameter("@MethodId", paymentId));
        }

        //***** check Shipping
        public static bool CheckShippingEnabledGeo(int methodId, string countryName, string regionName, string cityName, string districtName)
        {
            return CacheManager.Get(CacheNames.GetShippingForCityRegionAndCountry(methodId, countryName, regionName, cityName, districtName), () =>
            {
                if (CheckGeoShippingExcluded(methodId, countryName, regionName, cityName, districtName)) 
                    return false;

                return CheckGeoShippingIncluded(methodId, countryName, regionName, cityName, districtName);
            });
        }

        /// <returns>Метод доступен для указанных гео-данных</returns>
        private static bool CheckGeoShippingIncluded(int methodId, string countryName, string regionName, string cityName, string districtName)
        {
            if (!IsExistGeoShippingIncluded(methodId))
                return true;

            if (!string.IsNullOrEmpty(cityName) && CheckShippingIncludedCity(methodId, cityName, districtName, regionName))
                return true;

            if (!string.IsNullOrEmpty(regionName) && CheckShippingIncludedRegion(methodId, regionName))
                // Дополнительно проверяем, что не указаны Included города, т.к. тогда вернуть true
                // должна была предыдущая проверка
                if (CheckShippingNotIncludedCityByRegion(methodId, regionName))
                    return true;

            if (!string.IsNullOrEmpty(countryName) && CheckShippingIncludedCountry(methodId, countryName))
                // Дополнительно проверяем, что не указаны Included города и регионы, т.к. тогда вернуть true
                // должны были предыдущие проверки
                if (CheckShippingNotIncludedCityByCountry(methodId, countryName) && CheckShippingNotIncludedRegionByCountry(methodId, countryName))
                    return true;

            // Т.к. присутствуют ограничения на доступность метода определенным гео-регионам (страна, регион или город)
            // Т.е. метод доступен только определенному списку гео-регионов, а значит не может быть доступен иным гео-регионам
            return false;
        }

        /// <returns>Метод не доступен для указанных гео-данных</returns>
        private static bool CheckGeoShippingExcluded(int methodId, string countryName, string regionName, string cityName, string districtName)
        {
            if (!IsExistGeoShippingExcluded(methodId))
                return false;

            if (IsExistGeoShippingExcluded(methodId))
            {
                if (!string.IsNullOrEmpty(cityName) && CheckShippingExcludedCity(methodId, cityName, districtName, regionName))
                    return true;

                if (!string.IsNullOrEmpty(regionName) && CheckShippingExcludedRegion(methodId, regionName))
                    return true;

                if (!string.IsNullOrEmpty(countryName) && CheckShippingExcludedCountry(methodId, countryName))
                    return true;
            }
            
            return false;
        }

        private static bool CheckShippingIncludedCity(int methodId, string cityName, string districtName, string regionName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from Customers.City where CityName=@CityName AND (ISNULL([District], '')=@District) and CityId in (select CityId from [Order].ShippingCity where MethodId=@MethodId) AND (ISNULL(@RegionName, '') = '' OR City.RegionID IN (SELECT RegionID FROM [Customers].[Region] WHERE RegionName = @RegionName))",
                    CommandType.Text, 
                    new SqlParameter { ParameterName = "@CityName", Value = cityName ?? ""},
                    new SqlParameter { ParameterName = "@District", Value = districtName ?? "" },
                    new SqlParameter { ParameterName = "@RegionName", Value = regionName ?? "" },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }

        private static bool CheckShippingNotIncludedCityByRegion(int methodId, string regionName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from [Order].ShippingCity where MethodId=@MethodId and CityId in (Select CityId From Customers.City where RegionID in (select RegionId from [Order].ShippingRegion where MethodId=@MethodId and RegionId in (Select RegionId from Customers.Region where RegionName=@RegionName)))",
                    CommandType.Text,
                    new SqlParameter { ParameterName = "@RegionName", Value = regionName },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) == 0;
        }

        private static bool CheckShippingExcludedCity(int methodId, string cityName, string districtName, string regionName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from Customers.City where CityName=@CityName AND (ISNULL([District], '')=@District) and CityId in (select CityId from [Order].ShippingCityExcluded where MethodId=@MethodId) AND (ISNULL(@RegionName, '') = '' OR City.RegionID IN (SELECT RegionID FROM [Customers].[Region] WHERE RegionName = @RegionName))",
                    CommandType.Text, 
                    new SqlParameter { ParameterName = "@CityName", Value = cityName ?? ""},
                    new SqlParameter { ParameterName = "@District", Value = districtName ?? ""},
                    new SqlParameter { ParameterName = "@RegionName", Value = regionName ?? ""},
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }

        private static bool CheckShippingIncludedRegion(int methodId, string regionName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from Customers.Region where RegionName=@RegionName and RegionId in (select RegionId from [Order].[ShippingRegion] where MethodId=@MethodId)",
                    CommandType.Text, 
                    new SqlParameter { ParameterName = "@RegionName", Value = regionName },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }

        private static bool CheckShippingExcludedRegion(int methodId, string regionName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from Customers.Region where RegionName=@RegionName and RegionId in (select RegionId from [Order].[ShippingRegionExcluded] where MethodId=@MethodId)",
                    CommandType.Text, 
                    new SqlParameter { ParameterName = "@RegionName", Value = regionName },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }

        private static bool CheckShippingIncludedCountry(int methodId, string countryName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    @"if  exists (select CountryId from Customers.Country where CountryName=@CountryName and CountryId in (select CountryId from [Order].ShippingCountry where MethodId=@MethodId))
						select 1
					else
					select 0",
                    CommandType.Text, new SqlParameter { ParameterName = "@CountryName", Value = countryName ?? "" },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }

        private static bool CheckShippingExcludedCountry(int methodId, string countryName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    @"if exists(select ShippingCountryExcluded.CountryID from [Order].ShippingCountryExcluded left join Customers.Country on Country.CountryID = ShippingCountryExcluded.CountryId
                    where Country.CountryName = @CountryName and ShippingCountryExcluded.MethodId=@MethodId)
                        select 1
                    else 
                        select 0",
                    CommandType.Text, new SqlParameter { ParameterName = "@CountryName", Value = countryName ?? "" },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }

        // TODO: Check it!
        // Насколько я полня: == 0 - потому что фильтр по стране должен срабатывать
        // только при отсутствии разрешенных городов для данной страны, т.к. иначе
        // до данной функции не должно было дойти т.к. CheckShippingEnabledCity венурл бы true
        private static bool CheckShippingNotIncludedCityByCountry(int methodId, string countryName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from [Order].ShippingCity where MethodId=@MethodId and cityId in (Select cityId From Customers.City where RegionID in (SELECT RegionID FROM [Customers].[Region] WHERE CountryID in (select CountryId from [Order].ShippingCountry where MethodId=@MethodId and CountryId in (Select CountryId from Customers.Country where CountryName=@CountryName))))",
                    CommandType.Text,
                    new SqlParameter { ParameterName = "@CountryName", Value = countryName ?? "" },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) == 0;
        }

        // аналогично CheckShippingEnabledCityByCountry
        private static bool CheckShippingNotIncludedRegionByCountry(int methodId, string countryName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from [Order].ShippingRegion where MethodId=@MethodId and RegionId in (SELECT RegionID FROM [Customers].[Region] WHERE CountryID in (select CountryId from [Order].ShippingCountry where MethodId=@MethodId and CountryId in (Select CountryId from Customers.Country where CountryName=@CountryName)))",
                    CommandType.Text,
                    new SqlParameter { ParameterName = "@CountryName", Value = countryName },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) == 0;
        }

        public static bool IsExistGeoShipping(int methodId)
        {
			return IsExistGeoShippingIncluded(methodId) || IsExistGeoShippingExcluded(methodId);
        }

        public static bool IsExistGeoShippingIncluded(int methodId)
        {
            var temp = SQLDataAccess.ExecuteScalar<int>(@"if exists (select CityId from [Order].ShippingCity where MethodId=@MethodId)	
																select 1
															else
															if  exists (select RegionId from [Order].[ShippingRegion] where MethodId=@MethodId)
																select 1
															else
															if  exists (select CountryId from [Order].ShippingCountry where MethodId=@MethodId)
																select 1
															else
																select 0",
                CommandType.Text, new SqlParameter { ParameterName = "@MethodId", Value = methodId });
            return temp > 0;
        }

        public static bool IsExistGeoShippingExcluded(int methodId)
        {
            var temp = SQLDataAccess.ExecuteScalar<int>(@"if  exists (select CityId from [Order].ShippingCityExcluded where MethodId=@MethodId)
																select 1
															else
															if  exists (select RegionId from [Order].[ShippingRegionExcluded] where MethodId=@MethodId)
																select 1
															else
                                                            if  exists (select CountryId from [Order].ShippingCountryExcluded where MethodId=@MethodId)
																select 1
															else
																select 0",
                CommandType.Text, new SqlParameter { ParameterName = "@MethodId", Value = methodId });
            return temp > 0;
        }


        //***** check payment
        public static bool CheckPaymentEnabledGeo(int methodId, string countryName, string cityName)
        {
            if (CheckPaymentEnabledCity(methodId, cityName))
                return true;
            if (CheckPaymentEnabledCountry(methodId, countryName))
                if (CheckPaymentEnabledCityByCountry(methodId, countryName))
                    return true;
            return false;
        }

        private static bool CheckPaymentEnabledCity(int methodId, string cityName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from Customers.City where CityName=@CityName and CityId in (select CityId from [Order].PaymentCity where MethodId=@MethodId)",
                    CommandType.Text, new SqlParameter { ParameterName = "@CityName", Value = cityName  ?? ""},
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }

        private static bool CheckPaymentEnabledCountry(int methodId, string countryName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from Customers.Country where CountryName=@CountryName and CountryId in (select CountryId from [Order].PaymentCountry where MethodId=@MethodId)",
                    CommandType.Text, new SqlParameter { ParameterName = "@CountryName", Value = countryName  ?? ""},
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) > 0;
        }


        private static bool CheckPaymentEnabledCityByCountry(int methodId, string countryName)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "select Count(*) from [Order].PaymentCity where MethodId=@MethodId and CityId in (Select CityId From Customers.City where RegionID in (SELECT RegionID FROM [Customers].[Region] WHERE CountryID in (select CountryId from [Order].PaymentCountry where MethodId=@MethodId and CountryId = (Select CountryId from Customers.Country where CountryName=@CountryName))))",
                    CommandType.Text,
                    new SqlParameter { ParameterName = "@CountryName", Value = countryName ?? "" },
                    new SqlParameter { ParameterName = "@MethodId", Value = methodId }) == 0;
        }


        public static bool IsExistGeoPayment(int methodId)
        {
            var recordsCount = SQLDataAccess.ExecuteScalar<int>("select count(CityId) from [Order].PaymentCity where MethodId=@MethodId",
                                                        CommandType.Text, new SqlParameter { ParameterName = "@MethodId", Value = methodId });
            recordsCount += SQLDataAccess.ExecuteScalar<int>("select count(CountryId) from [Order].PaymentCountry where MethodId=@MethodId",
                                                        CommandType.Text, new SqlParameter { ParameterName = "@MethodId", Value = methodId });
            return recordsCount > 0;
        }
    }
}