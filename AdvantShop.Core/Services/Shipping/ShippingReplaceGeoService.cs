//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public class ShippingReplaceGeoService
    {
        public static ShippingReplaceGeo Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = @Id",
                CommandType.Text,
                FromReader,
                new SqlParameter("@Id", id));
        }

        public static List<ShippingReplaceGeo> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Order].[ShippingReplaceGeo] ORDER BY [ShippingType], [Sort] DESC",
                CommandType.Text,
                FromReader);
        }

        public static List<ShippingReplaceGeo> GetList(string shippingType)
        {
            return CacheManager.Get("ShippingReplaceGeo-List-" + shippingType, 5, () =>
                SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [ShippingType] = @ShippingType ORDER BY [Sort] DESC",
                    CommandType.Text,
                    FromReader,
                    new SqlParameter("@ShippingType", shippingType ?? (object)DBNull.Value)));
        }

        private static ShippingReplaceGeo FromReader(SqlDataReader reader)
        {
            return new ShippingReplaceGeo
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                ShippingType = SQLDataHelper.GetString(reader, "ShippingType"),
                InCountryName = SQLDataHelper.GetString(reader, "InCountryName"),
                InCountryISO2 = SQLDataHelper.GetString(reader, "InCountryISO2"),
                InRegionName = SQLDataHelper.GetString(reader, "InRegionName"),
                InCityName = SQLDataHelper.GetString(reader, "InCityName"),
                InDistrict = SQLDataHelper.GetString(reader, "InDistrict"),
                InZip = SQLDataHelper.GetString(reader, "InZip"),
                OutCountryName = SQLDataHelper.GetString(reader, "OutCountryName"),
                OutRegionName = SQLDataHelper.GetString(reader, "OutRegionName"),
                OutCityName = SQLDataHelper.GetString(reader, "OutCityName"),
                OutDistrict = SQLDataHelper.GetString(reader, "OutDistrict"),
                OutDistrictClear = SQLDataHelper.GetBoolean(reader, "OutDistrictClear"),
                OutZip = SQLDataHelper.GetString(reader, "OutZip"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                Sort = SQLDataHelper.GetInt(reader, "Sort"),
                Comment = SQLDataHelper.GetString(reader, "Comment"),
            };
        }

        public static int Add(ShippingReplaceGeo shippingReplaceGeo)
        {
            shippingReplaceGeo.Id = SQLDataAccess.ExecuteScalar<int>(
                @"INSERT INTO [Order].[ShippingReplaceGeo] ([ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName]
	                ,[InDistrict],[InZip],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[OutZip],[Enabled],[Sort],[Comment])
                  VALUES (@ShippingType,@InCountryName,@InCountryISO2,@InRegionName,@InCityName,@InDistrict,@InZip,@OutCountryName
	                ,@OutRegionName,@OutCityName,@OutDistrict,@OutDistrictClear,@OutZip,@Enabled,@Sort,@Comment); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@ShippingType", shippingReplaceGeo.ShippingType ?? (object)DBNull.Value),
                new SqlParameter("@InCountryName", shippingReplaceGeo.InCountryName ?? (object)DBNull.Value),
                new SqlParameter("@InCountryISO2", shippingReplaceGeo.InCountryISO2 ?? (object)DBNull.Value),
                new SqlParameter("@InRegionName", shippingReplaceGeo.InRegionName ?? (object)DBNull.Value),
                new SqlParameter("@InCityName", shippingReplaceGeo.InCityName ?? (object)DBNull.Value),
                new SqlParameter("@InDistrict", shippingReplaceGeo.InDistrict ?? (object)DBNull.Value),
                new SqlParameter("@InZip", shippingReplaceGeo.InZip ?? (object)DBNull.Value),
                new SqlParameter("@OutCountryName", shippingReplaceGeo.OutCountryName ?? (object)DBNull.Value),
                new SqlParameter("@OutRegionName", shippingReplaceGeo.OutRegionName ?? (object)DBNull.Value),
                new SqlParameter("@OutCityName", shippingReplaceGeo.OutCityName ?? (object)DBNull.Value),
                new SqlParameter("@OutDistrict", shippingReplaceGeo.OutDistrict ?? (object)DBNull.Value),
                new SqlParameter("@OutDistrictClear", shippingReplaceGeo.OutDistrictClear),
                new SqlParameter("@OutZip", shippingReplaceGeo.OutZip ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", shippingReplaceGeo.Enabled),
                new SqlParameter("@Sort", shippingReplaceGeo.Sort),
                new SqlParameter("@Comment", shippingReplaceGeo.Comment ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern("ShippingReplaceGeo-");

            return shippingReplaceGeo.Id;
        }

        public static void Update(ShippingReplaceGeo shippingReplaceGeo)
        {
            // Правила созданные не вручную\правила из sqlfix нельзя изменять
            if (shippingReplaceGeo.Id < 5000)
                return;

            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Order].[ShippingReplaceGeo]
                   SET [InCountryName] = @InCountryName
                      ,[InCountryISO2] = @InCountryISO2
                      ,[InRegionName] = @InRegionName
                      ,[InCityName] = @InCityName
                      ,[InDistrict] = @InDistrict
                      ,[InZip] = @InZip
                      ,[OutCountryName] = @OutCountryName
                      ,[OutRegionName] = @OutRegionName
                      ,[OutCityName] = @OutCityName
                      ,[OutDistrict] = @OutDistrict
                      ,[OutDistrictClear] = @OutDistrictClear
                      ,[OutZip] = @OutZip
                      ,[Enabled] = @Enabled
                      ,[Sort] = @Sort
                      ,[Comment] = @Comment
                 WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", shippingReplaceGeo.Id),
                new SqlParameter("@InCountryName", shippingReplaceGeo.InCountryName ?? (object)DBNull.Value),
                new SqlParameter("@InCountryISO2", shippingReplaceGeo.InCountryISO2 ?? (object)DBNull.Value),
                new SqlParameter("@InRegionName", shippingReplaceGeo.InRegionName ?? (object)DBNull.Value),
                new SqlParameter("@InCityName", shippingReplaceGeo.InCityName ?? (object)DBNull.Value),
                new SqlParameter("@InDistrict", shippingReplaceGeo.InDistrict ?? (object)DBNull.Value),
                new SqlParameter("@InZip", shippingReplaceGeo.InZip ?? (object)DBNull.Value),
                new SqlParameter("@OutCountryName", shippingReplaceGeo.OutCountryName ?? (object)DBNull.Value),
                new SqlParameter("@OutRegionName", shippingReplaceGeo.OutRegionName ?? (object)DBNull.Value),
                new SqlParameter("@OutCityName", shippingReplaceGeo.OutCityName ?? (object)DBNull.Value),
                new SqlParameter("@OutDistrict", shippingReplaceGeo.OutDistrict ?? (object)DBNull.Value),
                new SqlParameter("@OutDistrictClear", shippingReplaceGeo.OutDistrictClear),
                new SqlParameter("@OutZip", shippingReplaceGeo.OutZip ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", shippingReplaceGeo.Enabled),
                new SqlParameter("@Sort", shippingReplaceGeo.Sort),
                new SqlParameter("@Comment", shippingReplaceGeo.Comment ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern("ShippingReplaceGeo-");
        }

        public static void Delete(int id)
        {
            // Правила созданные не вручную\правила из sqlfix нельзя удалить
            if (id < 5000)
                return;

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[ShippingReplaceGeo] WHERE [Id] = @Id",
                CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern("ShippingReplaceGeo-");
        }

        public static ShippingReplaceGeo FindReplaceGeo(List<ShippingReplaceGeo> replaceList, string country, string countryIso2, 
            string region, string district, string city, string zip)
        {
            if (replaceList != null && replaceList.Count > 0)
            {
                return replaceList
                    .Where(x => x.Enabled)
                    // Страна
                    .Where(x => x.InCountryName.IsNullOrEmpty() || x.InCountryName.Equals(country, StringComparison.OrdinalIgnoreCase) ||
                                x.InCountryISO2.IsNullOrEmpty() || x.InCountryISO2.Equals(countryIso2, StringComparison.OrdinalIgnoreCase))
                    // Регион
                    .Where(x => x.InRegionName.IsNullOrEmpty() || x.InRegionName.Equals(region, StringComparison.OrdinalIgnoreCase))
                    // Район региона
                    .Where(x => x.InDistrict.IsNullOrEmpty() || x.InDistrict.Equals(district, StringComparison.OrdinalIgnoreCase))
                    // Город
                    .Where(x => x.InCityName.IsNullOrEmpty() || x.InCityName.Equals(city, StringComparison.OrdinalIgnoreCase))
                    // Индекс
                    .Where(x => x.InZip.IsNullOrEmpty() || x.InZip.Equals(zip, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
            }

            return null;
        }

        /// <returns>Были ли изменены данные</returns>
        public static bool ReplaceGeo(string shippingType, string country, string region, string district, string city, string zip,
            out string outCountry, out string outRegion, out string outDistrict, out string outCity, out string outZip)
        {
            return ReplaceGeo(
                GetList(shippingType),
                country, region, district, city, zip,
                out outCountry, out outRegion, out outDistrict, out outCity, out outZip);
        }

        /// <returns>Были ли изменены данные</returns>
        public static bool ReplaceGeo(List<ShippingReplaceGeo> replaceList, 
            string country, string region, string district, string city, string zip,
            out string outCountry, out string outRegion, out string outDistrict, out string outCity, out string outZip)
        {
            outCountry = country;
            outRegion = region;
            outDistrict = district;
            outCity = city;
            outZip = zip;

            if (replaceList.Count > 0)
            {
                var counter = 0;
                ShippingReplaceGeo replaceGeo;
                bool replaced = false;
                string outCountryIso2 = null;
                bool needToUpdateOutCountryIso2 = true;

                do
                {
                    if (outCountry.IsNotEmpty() && needToUpdateOutCountryIso2)
                    {
                        outCountryIso2 = Repository.CountryService.GetIso2(outCountry);
                        needToUpdateOutCountryIso2 = false;
                    }

                    replaceGeo = FindReplaceGeo(
                        replaceList,
                        outCountry,
                        outCountryIso2,
                        outRegion,
                        outDistrict,
                        outCity,
                        outZip);

                    if (replaceGeo != null)
                    {
                        if (replaceGeo.OutCountryName.IsNotEmpty())
                        {
                            outCountry = replaceGeo.OutCountryName;
                            needToUpdateOutCountryIso2 = true;
                        }
                        if (replaceGeo.OutRegionName.IsNotEmpty())
                            outRegion = replaceGeo.OutRegionName;
                        if (replaceGeo.OutDistrict.IsNotEmpty() || replaceGeo.OutDistrictClear)
                            outDistrict = replaceGeo.OutDistrict;
                        if (replaceGeo.OutCityName.IsNotEmpty())
                            outCity = replaceGeo.OutCityName;
                        if (replaceGeo.OutZip.IsNotEmpty())
                            outZip = replaceGeo.OutZip;

                        replaced = true;
                    }
                    counter++;
                } while (counter < 10 && replaceGeo != null);

                return replaced;
            }
            return false;

        }

        public static void ReplaceGeo(string shippingType, OrderCustomer orderCustomer)
        {
            if (orderCustomer != null)
            {
                string outCountry, outRegion, outDistrict, outCity, outZip;
                if (ReplaceGeo(
                        shippingType,
                        orderCustomer.Country, orderCustomer.Region, orderCustomer.District, orderCustomer.City, orderCustomer.Zip,
                        out outCountry, out outRegion, out outDistrict, out outCity, out outZip))
                {
                    orderCustomer.Country = outCountry;
                    orderCustomer.Region = outRegion;
                    orderCustomer.District = outDistrict;
                    orderCustomer.City = outCity;
                    orderCustomer.Zip = outZip;
                }
            }
        }
    }
}
