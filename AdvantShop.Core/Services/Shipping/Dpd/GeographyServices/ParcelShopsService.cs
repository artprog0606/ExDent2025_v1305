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
    public class ParcelShopsService
    {
        public static ParcelShop Get(string code)
        {
            if (code.IsNotEmpty())
            {
                return SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Shipping].[DpdParcelShops] WHERE [Code] = @Code",
                    CommandType.Text,
                    FromReader,
                    new SqlParameter("@Code", code ?? (object)DBNull.Value));
            }
            return null;
        }

        public static List<ParcelShop> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[DpdParcelShops]",
                CommandType.Text,
                FromReader);
        }

        public static List<ParcelShop> Find(string countryCode, string region, string city, long? cityId)
        {
            if (cityId.HasValue)
            {
                return SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Shipping].[DpdParcelShops] WHERE [CityId] = @CityId",
                    CommandType.Text,
                    FromReader,
                    new SqlParameter("@CityId", cityId.Value));
            }
            else if (countryCode.IsNotEmpty() || region.IsNotEmpty() || city.IsNotEmpty())
            {
                return CacheManager.Get(string.Format("DpdParcelShops-Find-{0}", ((countryCode ?? string.Empty).ToLower() + (region ?? string.Empty).ToLower() + (city ?? string.Empty).ToLower()).GetHashCode()), 60, () =>
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
                        "SELECT * FROM [Shipping].[DpdParcelShops] WHERE " + string.Join(" and ", where),
                        CommandType.Text,
                        FromReader,
                        listParams.ToArray());
                });

            }
            return null;
        }

        public static List<ParcelShop> FindByBounds(float topLeftLatitude, float topLeftLongitude,
            float bottomRightLatitude, float bottomRightLongitude)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[DpdParcelShops] WHERE @topLeftLatitude > [Latitude] AND @topLeftLongitude < [Longitude] "
                + "AND @bottomRightLatitude < [Latitude] AND @bottomRightLongitude > [Longitude]",
                CommandType.Text,
                FromReader,
                new SqlParameter("@topLeftLatitude", topLeftLatitude),
                new SqlParameter("@topLeftLongitude", topLeftLongitude),
                new SqlParameter("@bottomRightLatitude", bottomRightLatitude),
                new SqlParameter("@bottomRightLongitude", bottomRightLongitude));
        }

        public static ParcelShop FromReader(SqlDataReader reader)
        {
            return new ParcelShop
            {
                Code = SQLDataHelper.GetString(reader, "Code"),
                CityId = SQLDataHelper.GetLong(reader, "CityId"),
                CityName = SQLDataHelper.GetString(reader, "CityName"),
                RegionName = SQLDataHelper.GetString(reader, "RegionName"),
                CountryCode = SQLDataHelper.GetString(reader, "CountryCode"),
                Address = SQLDataHelper.GetString(reader, "Address"),
                AddressDescription = SQLDataHelper.GetString(reader, "AddressDescription"),
                Latitude = SQLDataHelper.GetDouble(reader, "Latitude"),
                Longitude = SQLDataHelper.GetDouble(reader, "Longitude"),
                IsSelfPickup = SQLDataHelper.GetBoolean(reader, "IsSelfPickup"),
                IsSelfDelivery = SQLDataHelper.GetBoolean(reader, "IsSelfDelivery"),
                SelfDeliveryTimes = SQLDataHelper.GetString(reader, "SelfDeliveryTimes"),
                ExtraServices = SQLDataHelper.GetString(reader, "ExtraServices"),
                Services = SQLDataHelper.GetString(reader, "Services"),
                MaxWeight = SQLDataHelper.GetNullableDouble(reader, "MaxWeight"),
                DimensionSum = SQLDataHelper.GetNullableDouble(reader, "DimensionSum"),
                MaxHeight = SQLDataHelper.GetNullableDouble(reader, "MaxHeight"),
                MaxWidth = SQLDataHelper.GetNullableDouble(reader, "MaxWidth"),
                MaxLength = SQLDataHelper.GetNullableDouble(reader, "MaxLength"),
                Type = SQLDataHelper.GetString(reader, "Type"),
            };
        }

        public static bool ExistsParcelShops()
        {
            return SQLDataAccess.ExecuteScalar<bool>("SELECT CASE WHEN EXISTS(SELECT [Code] FROM [Shipping].[DpdParcelShops]) THEN 1 ELSE 0 END", CommandType.Text);
        }

        public static void Add(ParcelShop parcelShop)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Shipping].[DpdParcelShops] ([Code],[CityId],[CityName],[RegionName],[CountryCode],[Address],[Latitude],
	                [Longitude],[IsSelfPickup],[IsSelfDelivery],[SelfDeliveryTimes],[ExtraServices],[Services],[AddressDescription],
                    [MaxWeight],[DimensionSum],[MaxHeight],[MaxWidth],[MaxLength],[Type])
                VALUES (@Code,@CityId,@CityName,@RegionName,@CountryCode,@Address,@Latitude,
	                @Longitude,@IsSelfPickup,@IsSelfDelivery,@SelfDeliveryTimes,@ExtraServices,@Services,@AddressDescription,
                    @MaxWeight,@DimensionSum,@MaxHeight,@MaxWidth,@MaxLength,@Type)",
                CommandType.Text,
                new SqlParameter("@Code", parcelShop.Code ?? (object)DBNull.Value),
                new SqlParameter("@CityId", parcelShop.CityId),
                new SqlParameter("@CityName", parcelShop.CityName ?? (object)DBNull.Value),
                new SqlParameter("@RegionName", parcelShop.RegionName ?? (object)DBNull.Value),
                new SqlParameter("@CountryCode", parcelShop.CountryCode ?? (object)DBNull.Value),
                new SqlParameter("@Address", parcelShop.Address ?? (object)DBNull.Value),
                new SqlParameter("@AddressDescription", parcelShop.AddressDescription ?? (object)DBNull.Value),
                new SqlParameter("@Latitude", parcelShop.Latitude),
                new SqlParameter("@Longitude", parcelShop.Longitude),
                new SqlParameter("@IsSelfPickup", parcelShop.IsSelfPickup),
                new SqlParameter("@IsSelfDelivery", parcelShop.IsSelfDelivery),
                new SqlParameter("@SelfDeliveryTimes", parcelShop.SelfDeliveryTimes ?? (object)DBNull.Value),
                new SqlParameter("@ExtraServices", parcelShop.ExtraServices ?? (object)DBNull.Value),
                new SqlParameter("@Services", parcelShop.Services ?? (object)DBNull.Value),
                new SqlParameter("@MaxWeight", parcelShop.MaxWeight ?? (object)DBNull.Value),
                new SqlParameter("@DimensionSum", parcelShop.DimensionSum ?? (object)DBNull.Value),
                new SqlParameter("@MaxHeight", parcelShop.MaxHeight ?? (object)DBNull.Value),
                new SqlParameter("@MaxWidth", parcelShop.MaxWidth ?? (object)DBNull.Value),
                new SqlParameter("@MaxLength", parcelShop.MaxLength ?? (object)DBNull.Value),
                new SqlParameter("@Type", parcelShop.Type ?? (object)DBNull.Value)
                );
        }

        public static void Update(ParcelShop parcelShop)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Shipping].[DpdParcelShops]
                   SET [CityId] = @CityId
                      ,[CityName] = @CityName
                      ,[RegionName] = @RegionName
                      ,[CountryCode] = @CountryCode
                      ,[Address] = @Address
                      ,[Latitude] = @Latitude
                      ,[Longitude] = @Longitude
                      ,[IsSelfPickup] = @IsSelfPickup
                      ,[IsSelfDelivery] = @IsSelfDelivery
                      ,[SelfDeliveryTimes] = @SelfDeliveryTimes
                      ,[ExtraServices] = @ExtraServices
                      ,[Services] = @Services
                      ,[AddressDescription] = @AddressDescription
                      ,@MaxWeight = @MaxWeight
                      ,@DimensionSum = @DimensionSum
                      ,@MaxHeight = @MaxHeight
                      ,@MaxWidth = @MaxWidth
                      ,@MaxLength = @MaxLength
                      ,@Type = @Type
                 WHERE [Code] = @Code",
                CommandType.Text,
                new SqlParameter("@Code", parcelShop.Code ?? (object)DBNull.Value),
                new SqlParameter("@CityId", parcelShop.CityId),
                new SqlParameter("@CityName", parcelShop.CityName ?? (object)DBNull.Value),
                new SqlParameter("@RegionName", parcelShop.RegionName ?? (object)DBNull.Value),
                new SqlParameter("@CountryCode", parcelShop.CountryCode ?? (object)DBNull.Value),
                new SqlParameter("@Address", parcelShop.Address ?? (object)DBNull.Value),
                new SqlParameter("@AddressDescription", parcelShop.AddressDescription ?? (object)DBNull.Value),
                new SqlParameter("@Latitude", parcelShop.Latitude),
                new SqlParameter("@Longitude", parcelShop.Longitude),
                new SqlParameter("@IsSelfPickup", parcelShop.IsSelfPickup),
                new SqlParameter("@IsSelfDelivery", parcelShop.IsSelfDelivery),
                new SqlParameter("@SelfDeliveryTimes", parcelShop.SelfDeliveryTimes ?? (object)DBNull.Value),
                new SqlParameter("@ExtraServices", parcelShop.ExtraServices ?? (object)DBNull.Value),
                new SqlParameter("@Services", parcelShop.Services ?? (object)DBNull.Value),
                new SqlParameter("@MaxWeight", parcelShop.MaxWeight ?? (object)DBNull.Value),
                new SqlParameter("@DimensionSum", parcelShop.DimensionSum ?? (object)DBNull.Value),
                new SqlParameter("@MaxHeight", parcelShop.MaxHeight ?? (object)DBNull.Value),
                new SqlParameter("@MaxWidth", parcelShop.MaxWidth ?? (object)DBNull.Value),
                new SqlParameter("@MaxLength", parcelShop.MaxLength ?? (object)DBNull.Value),
                new SqlParameter("@Type", parcelShop.Type ?? (object)DBNull.Value)
                );
        }

        public static void Delete(string code)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Shipping].[DpdParcelShops] WHERE [Code] = @Code",
                CommandType.Text,
                new SqlParameter("@Code", code ?? (object)DBNull.Value));
        }

        public static bool Sync(Api.DpdApiService dpdApiClient)
        {
            var scheduleSelfPickup = new Api.Geography.schedule() { operation = "SelfPickup" };
            var scheduleComparer = new ScheduleComparerByOperation();
            Api.Geography.schedule tempSchedule;

            var parcelShopsSource = dpdApiClient.GetParcelShops() ?? new parcelShop[] {};
            var isEmptyParcelShops = !ExistsParcelShops();

            if (isEmptyParcelShops == false)
            {
                var currentParcelShops = GetList();

                foreach (var parcelShopSource in parcelShopsSource)
                {
                    var parcelShop = Get(parcelShopSource.code);
                    var isNew = parcelShop == null;

                    if (parcelShop == null)
                        parcelShop = new ParcelShop();

                    parcelShop.Code = parcelShopSource.code;
                    if (parcelShopSource.address != null)
                    {
                        parcelShop.CityId = parcelShopSource.address.cityId;
                        parcelShop.CityName = parcelShopSource.address.cityName;
                        parcelShop.RegionName = parcelShopSource.address.regionName.RemoveTypeFromRegion();
                        parcelShop.CountryCode = parcelShopSource.address.countryCode;
                        parcelShop.Address = new[] {
                                ((parcelShopSource.address.streetAbbr.IsNotEmpty() ? parcelShopSource.address.streetAbbr + ". " : null) + parcelShopSource.address.street),
                                parcelShopSource.address.houseNo.IsNotEmpty() ? "д. " + parcelShopSource.address.houseNo : null,
                                parcelShopSource.address.building.IsNotEmpty() ? "корп. " + parcelShopSource.address.building : null,
                                parcelShopSource.address.structure.IsNotEmpty() ? "стр. " + parcelShopSource.address.structure : null
                            }.Where(x => x.IsNotEmpty())
                                    .AggregateString(", ");
                        parcelShop.AddressDescription = parcelShopSource.address.descript;
                    }

                    if (parcelShopSource.geoCoordinates != null)
                    {
                        parcelShop.Latitude = (double)parcelShopSource.geoCoordinates.latitude;
                        parcelShop.Longitude = (double)parcelShopSource.geoCoordinates.longitude;
                    }

                    if (parcelShopSource.schedule != null)
                    {
                        parcelShop.IsSelfPickup = parcelShopSource.schedule.Contains(scheduleSelfPickup, scheduleComparer);
                        parcelShop.IsSelfDelivery = (tempSchedule = parcelShopSource.schedule.FirstOrDefault(x => x.operation == "SelfDelivery")) != null;
                        parcelShop.SelfDeliveryTimes =
                            tempSchedule != null
                                ? string.Join(", ", tempSchedule.timetable.Select(x => string.Format("{0}: {1}", x.weekDays, x.workTime)))
                                : string.Empty;
                    }

                    if (parcelShopSource.extraService != null)
                        parcelShop.ExtraServices = string.Join(", ", parcelShopSource.extraService.Select(es => es.esCode));
                    else
                        parcelShop.ExtraServices = string.Empty;

                    if (parcelShopSource.services != null)
                        parcelShop.Services = string.Join(", ", parcelShopSource.services);
                    else
                        parcelShop.Services = string.Empty;

                    if (parcelShopSource.limits != null)
                    {
                        parcelShop.MaxWeight = (double)parcelShopSource.limits.maxWeight;
                        parcelShop.DimensionSum = (double)parcelShopSource.limits.dimensionSum;
                        parcelShop.MaxHeight = (double)parcelShopSource.limits.maxHeight;
                        parcelShop.MaxWidth = (double)parcelShopSource.limits.maxWidth;
                        parcelShop.MaxLength = (double)parcelShopSource.limits.maxLength;
                    }
                    else
                    {
                        parcelShop.MaxWeight = null;
                        parcelShop.DimensionSum = null;
                        parcelShop.MaxHeight = null;
                        parcelShop.MaxWidth = null;
                        parcelShop.MaxLength = null;
                    }

                    parcelShop.Type = string.Equals(parcelShopSource.parcelShopType, "П", StringComparison.OrdinalIgnoreCase)
                        ? "Постамат"
                        : "ПВЗ";

                    if (isNew)
                        Add(parcelShop);
                    else
                        Update(parcelShop);
                }

                // удаляем отсутствующие
                if (parcelShopsSource.Length > 0)
                {
                    var parcelShopsSourceCodes = parcelShopsSource.Select(x => x.code).ToList();
                    currentParcelShops
                        .Where(x => !parcelShopsSourceCodes.Contains(x.Code, StringComparer.OrdinalIgnoreCase))
                        .ForEach(x => Delete(x.Code));
                }
            }
            else
            {
                var table = SQLDataAccess.ExecuteTable(@"SELECT [Code],[CityId],[CityName],[RegionName],[CountryCode],[Address],[Latitude],
                    [Longitude],[IsSelfPickup],[IsSelfDelivery],[SelfDeliveryTimes],[ExtraServices],[Services],[AddressDescription],
                    [MaxWeight],[DimensionSum],[MaxHeight],[MaxWidth],[MaxLength],[Type] FROM [Shipping].[DpdParcelShops]", CommandType.Text);

                foreach (var parcelShopSource in parcelShopsSource)
                {
                    var row = table.NewRow();
                    row.SetField("Code", parcelShopSource.code);
                    if (parcelShopSource.address != null)
                    {
                        row.SetField("CityId", parcelShopSource.address.cityId);
                        row.SetField("CityName", parcelShopSource.address.cityName);
                        row.SetField("RegionName", parcelShopSource.address.regionName.RemoveTypeFromRegion());
                        row.SetField("CountryCode", parcelShopSource.address.countryCode);
                        row.SetField("Address", new[] {
                                ((parcelShopSource.address.streetAbbr.IsNotEmpty() ? parcelShopSource.address.streetAbbr + ". " : null) + parcelShopSource.address.street),
                                parcelShopSource.address.houseNo.IsNotEmpty() ? "д. " + parcelShopSource.address.houseNo : null,
                                parcelShopSource.address.building.IsNotEmpty() ? "корп. " + parcelShopSource.address.building : null,
                                parcelShopSource.address.structure.IsNotEmpty() ? "стр. " + parcelShopSource.address.structure : null
                            }.Where(x => x.IsNotEmpty())
                            .AggregateString(", "));
                        row.SetField("AddressDescription", parcelShopSource.address.descript);
                    }

                    if (parcelShopSource.geoCoordinates != null)
                    {
                        row.SetField("Latitude", (double)parcelShopSource.geoCoordinates.latitude);
                        row.SetField("Longitude", (double)parcelShopSource.geoCoordinates.longitude);
                    }

                    if (parcelShopSource.schedule != null)
                    {
                        row.SetField("IsSelfPickup", parcelShopSource.schedule.Contains(scheduleSelfPickup, scheduleComparer));
                        row.SetField("IsSelfDelivery", (tempSchedule = parcelShopSource.schedule.FirstOrDefault(x => x.operation == "SelfDelivery")) != null);
                        row.SetField("SelfDeliveryTimes",
                            tempSchedule != null
                                ? string.Join(", ", tempSchedule.timetable.Select(x => string.Format("{0}: {1}", x.weekDays, x.workTime)))
                                : string.Empty);
                    }

                    if (parcelShopSource.extraService != null)
                        row.SetField("ExtraServices", string.Join(", ", parcelShopSource.extraService.Select(es => es.esCode)));
                    else
                        row.SetField("ExtraServices", string.Empty);

                    if (parcelShopSource.services != null)
                        row.SetField("Services", string.Join(", ", parcelShopSource.services));
                    else
                        row.SetField("Services", string.Empty);

                    if (parcelShopSource.limits != null)
                    {
                        row.SetField("MaxWeight", (double)parcelShopSource.limits.maxWeight);
                        row.SetField("DimensionSum", (double)parcelShopSource.limits.dimensionSum);
                        row.SetField("MaxHeight", (double)parcelShopSource.limits.maxHeight);
                        row.SetField("MaxWidth", (double)parcelShopSource.limits.maxWidth);
                        row.SetField("MaxLength", (double)parcelShopSource.limits.maxLength);
                    }
                    else
                    {
                        row.SetField("MaxWeight", (double?)null);
                        row.SetField("DimensionSum", (double?)null);
                        row.SetField("MaxHeight", (double?)null);
                        row.SetField("MaxWidth", (double?)null);
                        row.SetField("MaxLength", (double?)null);
                    }

                    row.SetField("Type", 
                        string.Equals(parcelShopSource.parcelShopType, "П", StringComparison.OrdinalIgnoreCase)
                            ? "Постамат"
                            : "ПВЗ");

                    table.Rows.Add(row);

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
                        sqlBulkCopy.DestinationTableName = "[Shipping].[DpdParcelShops]";
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }
    }
}
