//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Repository
{
    public class RegionService
    {
        private const string RegionCacheKey = "Region_";
        /// <summary>
        /// Get list of regions ordered by SortOrder and Name
        /// </summary>
        public static List<Region> GetRegions(int countryId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Customers].[Region] WHERE CountryID = @Id ORDER BY [RegionSort], RegionName",
                CommandType.Text, ReadRegion, new SqlParameter("@Id", countryId));
        }

        public static int GetRegionIdByName(string regionName)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT RegionID FROM Customers.Region WHERE RegionName = @name",
                                                      CommandType.Text, new SqlParameter("@name", regionName));
        }

        public static Region GetRegionByName(string regionName)
        {
            return SQLDataAccess.ExecuteReadOne<Region>("SELECT * FROM Customers.Region WHERE RegionName = @name",
                                                      CommandType.Text, ReadRegion, new SqlParameter("@name", regionName));
        }

        public static void UpdateRegion(Region region)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[Region] set RegionName=@name, RegionCode=@RegionCode, RegionSort=@RegionSort, CountryID = @CountryID where RegionID = @id",
                CommandType.Text,
                new SqlParameter("@id", region.RegionId),
                new SqlParameter("@name", region.Name),
                new SqlParameter("@RegionCode", region.RegionCode ?? (object) DBNull.Value),
                new SqlParameter("@RegionSort", region.SortOrder),
                new SqlParameter("@CountryID", region.CountryId));
            
            CacheManager.RemoveByPattern(RegionCacheKey);
        }

        public static void InsertRegion(Region region)
        {
            region.RegionId =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Customers].[Region] (RegionName, RegionCode, RegionSort, CountryID) VALUES (@Name, @RegionCode, @Sort, @CountryID);SELECT scope_identity();",
                    CommandType.Text, new SqlParameter("@Name", region.Name),
                    new SqlParameter("@RegionCode", region.RegionCode ?? (object)DBNull.Value),
                    new SqlParameter("@Sort", region.SortOrder),
                    new SqlParameter("@CountryID", region.CountryId));
        }

        public static void DeleteRegion(int regionId)
        {
            if (regionId != SettingsMain.SellerRegionId)
            {
                SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.Region WHERE RegionID = @RegionID",
                                              CommandType.Text,
                                              new SqlParameter("@regionID", regionId));
                AdditionalOptionsService.Delete(regionId, EnAdditionalOptionObjectType.Region);
                CacheManager.Remove(RegionCacheKey + regionId);
            }
        }

        public static Region GetRegion(int regionId)
        {
            return CacheManager.Get<Region>(RegionCacheKey + regionId,
                () => SQLDataAccess.ExecuteReadOne("SELECT * FROM [Customers].[Region] WHERE [RegionID] = @regionId",
                    CommandType.Text, ReadRegion, new SqlParameter("@regionId", regionId)));
        }

        public static Region GetRegion(string regionName, int countryId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Customers].[Region] WHERE [RegionName] = @RegionName AND [CountryID] = @CountryID",
                CommandType.Text,
                ReadRegion, 
                new SqlParameter("@RegionName", regionName),
                new SqlParameter("@CountryID", countryId));
        }

        public static List<Region> GetRegionsListByName(string regionName)
        {
            return SQLDataAccess.ExecuteReadList<Region>(
                "SELECT TOP(1) * FROM [Customers].[Region] WHERE RegionName like @RegionName + '%' or RegionName like '%' + @RegionName",
                CommandType.Text,
                ReadRegion,
                new SqlParameter("@RegionName", regionName));
        }

        public static List<string> GetRegionsByName(string name)
        {
            var translit = StringHelper.TranslitToRusKeyboard(name);

            return
                SQLDataAccess.ExecuteReadList<string>(
                    "Select RegionName From [Customers].[Region] WHERE RegionName like @name + '%' or RegionName like @trname + '%'",
                    CommandType.Text, reader => SQLDataHelper.GetString(reader, "RegionName"),
                    new SqlParameter("@name", name),
                    new SqlParameter("@trname", translit));
        }

        public static Region ReadRegion(SqlDataReader reader)
        {
            return new Region
            {
                RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                CountryId = SQLDataHelper.GetInt(reader, "CountryID"),
                Name = SQLDataHelper.GetString(reader, "RegionName"),
                RegionCode = SQLDataHelper.GetString(reader, "RegionCode"),
                SortOrder = SQLDataHelper.GetInt(reader, "RegionSort")
            };
        }

        #region AdditionalSettings

        public static RegionAdditionalSettings GetAdditionalSettings(int regionId)
        {
            if (regionId is 0)
                return null;
            
            var additionalOptions = AdditionalOptionsService.Get(regionId, EnAdditionalOptionObjectType.Region);
            return new RegionAdditionalSettings
            {
                RegionId = regionId,
                FiasId = 
                    additionalOptions.FirstOrDefault(option => 
                        RegionAdditionalOptionNames.FiasId.Equals(option.Name))?.Value,
                KladrId = 
                    additionalOptions.FirstOrDefault(option => 
                        RegionAdditionalOptionNames.KladrId.Equals(option.Name))?.Value,

            };
        }

        public static void UpdateAdditionalSettings(RegionAdditionalSettings additionalSettings)
        {
            AdditionalOptionsService.AddOrUpdate(new AdditionalOption()
            {
                ObjId = additionalSettings.RegionId,
                ObjType = EnAdditionalOptionObjectType.Region,
                Name = RegionAdditionalOptionNames.FiasId,
                Value = additionalSettings.FiasId ?? string.Empty
            });
            AdditionalOptionsService.AddOrUpdate(new AdditionalOption()
            {
                ObjId = additionalSettings.RegionId,
                ObjType = EnAdditionalOptionObjectType.Region,
                Name = RegionAdditionalOptionNames.KladrId,
                Value = additionalSettings.KladrId ?? string.Empty
            });
        }

        #endregion
    }
}