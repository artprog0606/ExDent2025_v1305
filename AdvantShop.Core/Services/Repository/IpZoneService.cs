using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Repository
{
    public class IpZoneService
    {
        public static IpZone GetZoneByCityId(int cityId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select Top(1) City.CityID, City.CityName, City.District, City.Zip, Region.RegionID, Region.RegionName, Country.CountryID, Country.CountryName " +
                "From Customers.City " +
                "Inner Join Customers.Region On Region.RegionId = City.RegionId " +
                "Inner Join Customers.Country On Country.CountryId = Region.CountryId " +
                "Where CityID = @CityId " +
                "Order by Country.CountryID",
                CommandType.Text,
                reader => new IpZone()
                {
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId"),
                    CountryName = SQLDataHelper.GetString(reader, "CountryName"),
                    RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CityId = SQLDataHelper.GetInt(reader, "CityId"),
                    City = SQLDataHelper.GetString(reader, "CityName"),
                    District = SQLDataHelper.GetString(reader, "District"),
                    Zip = SQLDataHelper.GetString(reader, "Zip")
                },
                new SqlParameter("@CityId", cityId));
        }

        public static IpZone GetZoneByCity(string city, int? countryId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select Top(1) City.CityID, City.CityName, City.District, City.Zip, Region.RegionID, Region.RegionName, Country.CountryID, Country.CountryName " +
                "From Customers.City " +
                "Inner Join Customers.Region On Region.RegionId = City.RegionId " +
                "Inner Join Customers.Country On Country.CountryId = Region.CountryId " +
                "Where @City = CityName and (Country.CountryID=@countryID OR @countryID is null)" +
                "Order by Country.CountryID",
                CommandType.Text,
                reader => new IpZone()
                {
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId"),
                    CountryName = SQLDataHelper.GetString(reader, "CountryName"),
                    RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CityId = SQLDataHelper.GetInt(reader, "CityId"),
                    City = SQLDataHelper.GetString(reader, "CityName"),
                    District = SQLDataHelper.GetString(reader, "District"),
                    Zip = SQLDataHelper.GetString(reader, "Zip")
                },
                new SqlParameter("@City", city), new SqlParameter("@countryID", (object)countryId ?? DBNull.Value));
        }

        public static List<IpZone> GetIpZonesByCity(string cityName)
        {
            string translitRu = StringHelper.TranslitToRus(cityName);
            string translitKeyboard = StringHelper.TranslitToRusKeyboard(cityName);

            return SQLDataAccess.ExecuteReadList<IpZone>(
                "Select Top (10) CityName, CityID, Zip, District, " +
                //"(case when (select count(*) from [Customers].[City] where CityName = Cities.CityName) > 1 then (Region.RegionName) else '' end) as RegionName, " +
                "Region.RegionName, Region.RegionId, Country.CountryId, Country.CountryName " +
                "From Customers.City as Cities INNER JOIN Customers.Region ON Region.RegionID = Cities.RegionId INNER JOIN Customers.Country ON Country.CountryID = Region.CountryID " +
                "WHERE CityName like @name + '%' OR CityName like @translitRu + '%' " +
                " OR CityName like @translitKeyboard + '%' order by Cities.DisplayInPopup DESC, Cities.CitySort ASC, CityName",
                CommandType.Text, reader => new IpZone()
                {
                    CityId = SQLDataHelper.GetInt(reader, "CityID"),
                    City = SQLDataHelper.GetString(reader, "CityName"),
                    District = SQLDataHelper.GetString(reader, "District"),
                    Zip = SQLDataHelper.GetString(reader, "Zip"),
                    RegionId = SQLDataHelper.GetInt(reader, "RegionId"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId"),
                    CountryName = SQLDataHelper.GetString(reader, "CountryName")
                },
                new SqlParameter("@name", cityName),
                new SqlParameter("@translitRu", translitRu),
                new SqlParameter("@translitKeyboard", translitKeyboard));
        }

        public static List<IpZone> GetIpZonesByRegion(string regionName)
        {
            string translitRu = StringHelper.TranslitToRus(regionName);
            string translitKeyboard = StringHelper.TranslitToRusKeyboard(regionName);

            return SQLDataAccess.ExecuteReadList<IpZone>(
                "Select Top (10) RegionName, RegionID, " +
                "(case when (select count(*) from [Customers].[Region] where RegionName = Regions.RegionName) > 1 then (Country.CountryName) else '' end) as CountryName, " +
                "Country.CountryId " +
                "From Customers.Region as Regions INNER JOIN Customers.Country ON Country.CountryID = Regions.CountryID " +
                "WHERE Replace(RegionName,'ё','е') like '%' + @name + '%' OR Replace(RegionName,'ё','е') like '%' + @translitRu + '%' " +
                " OR Replace(RegionName,'ё','е') like '%' + @translitKeyboard + '%' " +
                "order by RegionName",
                CommandType.Text, reader => new IpZone()
                {
                    RegionId = SQLDataHelper.GetInt(reader, "RegionID"),
                    Region = SQLDataHelper.GetString(reader, "RegionName"),
                    CountryId = SQLDataHelper.GetInt(reader, "CountryId"),
                    CountryName = SQLDataHelper.GetString(reader, "CountryName"),
                },
                new SqlParameter("@name", regionName),
                new SqlParameter("@translitRu", translitRu),
                new SqlParameter("@translitKeyboard", translitKeyboard));
        }
    }
}