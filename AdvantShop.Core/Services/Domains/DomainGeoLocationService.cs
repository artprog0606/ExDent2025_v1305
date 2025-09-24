using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.SQL;
using AdvantShop.Repository;

namespace AdvantShop.Core.Services.Domains
{
    public class DomainGeoLocationService
    {
        public const string CacheKey = "DomainGeoLocation_";
        
        public static int Add(DomainGeoLocation domainGeoLocation)
        {
            if (domainGeoLocation.Url.IsNullOrEmpty())
                throw new ArgumentException("Url is null");
            
            domainGeoLocation.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into [Settings].[DomainGeoLocation] ([Url], [GeoName]) Values (@Url, @GeoName); Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Url", domainGeoLocation.Url),
                    new SqlParameter("@GeoName", domainGeoLocation.GeoName)
                );
            
            CacheManager.RemoveByPattern(CacheKey);

            return domainGeoLocation.Id;
        }
        
        public static int Update(DomainGeoLocation domainGeoLocation)
        {
            if (domainGeoLocation.Url.IsNullOrEmpty())
                throw new ArgumentException("Url is null");
            
            if (domainGeoLocation.GeoName.IsNullOrEmpty())
                throw new ArgumentException("Geo Name is null");
            
            SQLDataAccess.ExecuteNonQuery(
                    "Update [Settings].[DomainGeoLocation] Set [Url] = @Url, [GeoName] = @GeoName Where Id = @Id",
                    CommandType.Text,
                    new SqlParameter("@Url", domainGeoLocation.Url),
                    new SqlParameter("@Id", domainGeoLocation.Id),
                    new SqlParameter("@GeoName", domainGeoLocation.GeoName)
                );
            
            CacheManager.RemoveByPattern(CacheKey);

            return domainGeoLocation.Id;
        }

        public static void Delete(int domainId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Settings].[DomainGeoLocation] Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", domainId)
            );
            
            CacheManager.RemoveByPattern(CacheKey);
        }

        public static DomainGeoLocation Get(int domainId)
        {
            return SQLDataAccess
                .Query<DomainGeoLocation>("Select top(1) * From [Settings].[DomainGeoLocation] Where Id = @domainId",
                    new { domainId })
                .FirstOrDefault();
        }
        
        public static List<DomainGeoLocation> GetList()
        {
            return CacheManager.Get(CacheKey + "list", () =>
                SQLDataAccess
                    .Query<DomainGeoLocation>("Select * From [Settings].[DomainGeoLocation]")
                    .ToList()
            );
        }

        public static void AddCity(DomainCityLink link)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Settings].[DomainGeoLocation_City] (DomainGeoLocationId, CityId) Values (@DomainGeoLocationId, @CityId)",
                CommandType.Text,
                new SqlParameter("@DomainGeoLocationId", link.DomainGeoLocationId),
                new SqlParameter("@CityId", link.CityId)
            );
            
            CacheManager.RemoveByPattern(CacheKey);
        }
        
        public static void DeleteCity(DomainCityLink link)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Settings].[DomainGeoLocation_City] Where DomainGeoLocationId=@DomainGeoLocationId and CityId=@CityId",
                CommandType.Text,
                new SqlParameter("@DomainGeoLocationId", link.DomainGeoLocationId),
                new SqlParameter("@CityId", link.CityId)
            );
            
            CacheManager.RemoveByPattern(CacheKey);
        }
        
        public static void DeleteCities(int domainId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Settings].[DomainGeoLocation_City] Where DomainGeoLocationId=@DomainGeoLocationId",
                CommandType.Text,
                new SqlParameter("@DomainGeoLocationId", domainId)
            );
            
            CacheManager.RemoveByPattern(CacheKey);
        }

        public static List<CityOfWarehouse> GetCities(int domainId)
        {
            return SQLDataAccess
                .Query<CityOfWarehouse>("Select City.CityID, City.CityName " +
                                        "From Customers.City " +
                                        "Inner Join [Settings].[DomainGeoLocation_City] dc on dc.CityId = City.CityID " +
                                        "Where dc.DomainGeoLocationId = @domainId",
                    new { domainId })
                .ToList();
        }
        
        public static City GetCity(int domainId)
        {
            return CacheManager.Get(CacheKey + "city_" + domainId, () =>
                SQLDataAccess
                    .Query<City>(
                        "Select top(1) City.CityId, City.RegionId, City.CityName as Name, City.District, City.Zip, City.PhoneNumber, City.MobilePhoneNumber " +
                        "From Customers.City " +
                        "Inner Join [Settings].[DomainGeoLocation_City] dc on dc.CityId = City.CityID " +
                        "Where dc.DomainGeoLocationId = @domainId " +
                        "Order by DisplayInPopup desc, CitySort desc",
                        new { domainId })
                    .FirstOrDefault()
            );
        }

        public static List<int> GetWarehouseIds(int domainGeoLocationId)
        {
            return CacheManager.Get(CacheKey + "warehouseIds_" + domainGeoLocationId, () =>
                SQLDataAccess
                    .Query<int>(
                        @";with cte as (
	                        Select TOP 100 PERCENT WarehouseId 
	                        From [Catalog].[Warehouse_City] as wc 
	                        Inner Join [Settings].[DomainGeoLocation_City] as dc on dc.CityId = wc.CityId 
	                        Where dc.DomainGeoLocationId = @domainGeoLocationId  
	                        Order By wc.SortOrder
                        )
                        Select Id 
                        From [Catalog].[Warehouse] 
                        Where Id in (Select distinct WarehouseId From cte) and Enabled = 1",
                        new { domainGeoLocationId })
                    .ToList()
            );
        }

        public static DomainGeoLocation GetDomain(int cityId)
        {
            return SQLDataAccess
                .Query<DomainGeoLocation>(
                    "Select top(1) d.Id, d.Url " +
                    "From [Settings].[DomainGeoLocation] d " +
                    "Inner Join [Settings].[DomainGeoLocation_City] dc on dc.DomainGeoLocationId = d.Id " +
                    "Where dc.CityId = @cityId",
                    new { cityId })
                .FirstOrDefault();
        } 
    }
}