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
    public class TerminalsService
    {
        public static Terminal Get(string code)
        {
            if (code.IsNotEmpty())
            {
                return SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Shipping].[DpdTerminals] WHERE [Code] = @Code",
                    CommandType.Text,
                    FromReader,
                    new SqlParameter("@Code", code ?? (object)DBNull.Value));
            }
            return null;
        }

        public static List<Terminal> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[DpdTerminals]",
                CommandType.Text,
                FromReader);
        }

        public static List<Terminal> Find(string countryCode, string region, string city, long? cityId)
        {
            if (cityId.HasValue)
            {
                return SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Shipping].[DpdTerminals] WHERE [CityId] = @CityId",
                    CommandType.Text,
                    FromReader,
                    new SqlParameter("@CityId", cityId.Value));
            }
            else if (countryCode.IsNotEmpty() || region.IsNotEmpty() || city.IsNotEmpty())
            {
                return CacheManager.Get(string.Format("DpdTerminals-Find-{0}", ((countryCode ?? string.Empty).ToLower() + (region ?? string.Empty).ToLower() + (city ?? string.Empty).ToLower()).GetHashCode()), 60, () =>
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
                        "SELECT * FROM [Shipping].[DpdTerminals] WHERE " + string.Join(" and ", where),
                        CommandType.Text,
                        FromReader,
                        listParams.ToArray());
                });

            }
            return null;
        }

        public static List<Terminal> FindByBounds(float topLeftLatitude, float topLeftLongitude,
            float bottomRightLatitude, float bottomRightLongitude)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[DpdTerminals] WHERE @topLeftLatitude > [Latitude] AND @topLeftLongitude < [Longitude] "
                + "AND @bottomRightLatitude < [Latitude] AND @bottomRightLongitude > [Longitude]",
                CommandType.Text,
                FromReader,
                new SqlParameter("@topLeftLatitude", topLeftLatitude),
                new SqlParameter("@topLeftLongitude", topLeftLongitude),
                new SqlParameter("@bottomRightLatitude", bottomRightLatitude),
                new SqlParameter("@bottomRightLongitude", bottomRightLongitude));
        }

        public static Terminal FromReader(SqlDataReader reader)
        {
            return new Terminal
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
            };
        }

        public static bool ExistsTerminals()
        {
            return SQLDataAccess.ExecuteScalar<bool>("SELECT CASE WHEN EXISTS(SELECT [Code] FROM [Shipping].[DpdTerminals]) THEN 1 ELSE 0 END", CommandType.Text);
        }

        public static void Add(Terminal terminal)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Shipping].[DpdTerminals] ([Code],[CityId],[CityName],[RegionName],[CountryCode],[Address],[Latitude],
	                [Longitude],[IsSelfPickup],[IsSelfDelivery],[SelfDeliveryTimes],[ExtraServices],[Services],[AddressDescription])
                VALUES
	                (@Code,@CityId,@CityName,@RegionName,@CountryCode,@Address,@Latitude,
	                @Longitude,@IsSelfPickup,@IsSelfDelivery,@SelfDeliveryTimes,@ExtraServices,@Services,@AddressDescription)",
                CommandType.Text,
                new SqlParameter("@Code", terminal.Code ?? (object)DBNull.Value),
                new SqlParameter("@CityId", terminal.CityId),
                new SqlParameter("@CityName", terminal.CityName ?? (object)DBNull.Value),
                new SqlParameter("@RegionName", terminal.RegionName ?? (object)DBNull.Value),
                new SqlParameter("@CountryCode", terminal.CountryCode ?? (object)DBNull.Value),
                new SqlParameter("@Address", terminal.Address ?? (object)DBNull.Value),
                new SqlParameter("@AddressDescription", terminal.AddressDescription ?? (object)DBNull.Value),
                new SqlParameter("@Latitude", terminal.Latitude),
                new SqlParameter("@Longitude", terminal.Longitude),
                new SqlParameter("@IsSelfPickup", terminal.IsSelfPickup),
                new SqlParameter("@IsSelfDelivery", terminal.IsSelfDelivery),
                new SqlParameter("@SelfDeliveryTimes", terminal.SelfDeliveryTimes ?? (object)DBNull.Value),
                new SqlParameter("@ExtraServices", terminal.ExtraServices ?? (object)DBNull.Value),
                new SqlParameter("@Services", terminal.Services ?? (object)DBNull.Value)
                );
        }

        public static void Update(Terminal terminal)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Shipping].[DpdTerminals]
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
                 WHERE [Code] = @Code",
                CommandType.Text,
                new SqlParameter("@Code", terminal.Code ?? (object)DBNull.Value),
                new SqlParameter("@CityId", terminal.CityId),
                new SqlParameter("@CityName", terminal.CityName ?? (object)DBNull.Value),
                new SqlParameter("@RegionName", terminal.RegionName ?? (object)DBNull.Value),
                new SqlParameter("@CountryCode", terminal.CountryCode ?? (object)DBNull.Value),
                new SqlParameter("@Address", terminal.Address ?? (object)DBNull.Value),
                new SqlParameter("@AddressDescription", terminal.AddressDescription ?? (object)DBNull.Value),
                new SqlParameter("@Latitude", terminal.Latitude),
                new SqlParameter("@Longitude", terminal.Longitude),
                new SqlParameter("@IsSelfPickup", terminal.IsSelfPickup),
                new SqlParameter("@IsSelfDelivery", terminal.IsSelfDelivery),
                new SqlParameter("@SelfDeliveryTimes", terminal.SelfDeliveryTimes ?? (object)DBNull.Value),
                new SqlParameter("@ExtraServices", terminal.ExtraServices ?? (object)DBNull.Value),
                new SqlParameter("@Services", terminal.Services ?? (object)DBNull.Value)
                );
        }

        public static void Delete(string code)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Shipping].[DpdTerminals] WHERE [Code] = @Code",
                CommandType.Text,
                new SqlParameter("@Code", code ?? (object)DBNull.Value));
        }

        public static bool Sync(Api.DpdApiService dpdApiClient)
        {
            var scheduleSelfPickup = new Api.Geography.schedule() { operation = "SelfPickup" };
            var scheduleComparer = new ScheduleComparerByOperation();
            Api.Geography.schedule tempSchedule;

            var terminalsSource = dpdApiClient.GetTerminals() ?? new terminalSelf[]{};
            var isEmptyParcelShops = !ExistsTerminals();

            if (isEmptyParcelShops == false)
            {
                var currentTerminals = GetList();

                foreach (var terminalSource in terminalsSource)
                {
                    var terminal = Get(terminalSource.terminalCode);
                    var isNew = terminal == null;

                    if (terminal == null)
                        terminal = new Terminal();

                    terminal.Code = terminalSource.terminalCode;
                    if (terminalSource.address != null)
                    {
                        terminal.CityId = terminalSource.address.cityId;
                        terminal.CityName = terminalSource.address.cityName;
                        terminal.RegionName = terminalSource.address.regionName.RemoveTypeFromRegion();
                        terminal.CountryCode = terminalSource.address.countryCode;
                        terminal.Address = new[] {
                                ((terminalSource.address.streetAbbr.IsNotEmpty() ? terminalSource.address.streetAbbr + ". " : null) + terminalSource.address.street),
                                terminalSource.address.houseNo.IsNotEmpty() ? "д. " + terminalSource.address.houseNo : null,
                                terminalSource.address.building.IsNotEmpty() ? "корп. " + terminalSource.address.building : null,
                                terminalSource.address.structure.IsNotEmpty() ? "стр. " + terminalSource.address.structure : null
                            }.Where(x => x.IsNotEmpty())
                                    .AggregateString(", ");
                        terminal.AddressDescription = terminalSource.address.descript;
                    }

                    if (terminalSource.geoCoordinates != null)
                    {
                        terminal.Latitude = (double)terminalSource.geoCoordinates.latitude;
                        terminal.Longitude = (double)terminalSource.geoCoordinates.longitude;
                    }

                    if (terminalSource.schedule != null)
                    {
                        terminal.IsSelfPickup = terminalSource.schedule.Contains(scheduleSelfPickup, scheduleComparer);
                        terminal.IsSelfDelivery = (tempSchedule = terminalSource.schedule.FirstOrDefault(x => x.operation == "SelfDelivery")) != null;
                        terminal.SelfDeliveryTimes =
                            tempSchedule != null
                                ? string.Join(", ", tempSchedule.timetable.Select(x => string.Format("{0}: {1}", x.weekDays, x.workTime)))
                                : string.Empty;
                    }

                    if (terminalSource.extraService != null)
                        terminal.ExtraServices = string.Join(", ", terminalSource.extraService.Select(es => es.esCode));
                    else
                        terminal.ExtraServices = string.Empty;

                    if (terminalSource.services != null)
                        terminal.Services = string.Join(", ", terminalSource.services);
                    else
                        terminal.Services = string.Empty;


                    if (isNew)
                        Add(terminal);
                    else
                        Update(terminal);
                }

                // удаляем отсутствующие
                if (terminalsSource.Length > 0)
                {
                    var terminalsSourceCodes = terminalsSource.Select(x => x.terminalCode).ToList();
                    currentTerminals
                        .Where(x => !terminalsSourceCodes.Contains(x.Code, StringComparer.OrdinalIgnoreCase))
                        .ForEach(x => Delete(x.Code));
                }
            }
            else
            {
                var table = SQLDataAccess.ExecuteTable(@"SELECT [Code],[CityId],[CityName],[RegionName],[CountryCode],[Address],[Latitude],
	                [Longitude],[IsSelfPickup],[IsSelfDelivery],[SelfDeliveryTimes],[ExtraServices],[Services],[AddressDescription]
                    FROM [Shipping].[DpdTerminals]", CommandType.Text);

                foreach (var terminalSource in terminalsSource)
                {
                    var row = table.NewRow();
                    row.SetField("Code", terminalSource.terminalCode);
                    if (terminalSource.address != null)
                    {
                        row.SetField("CityId", terminalSource.address.cityId);
                        row.SetField("CityName", terminalSource.address.cityName);
                        row.SetField("RegionName", terminalSource.address.regionName.RemoveTypeFromRegion());
                        row.SetField("CountryCode", terminalSource.address.countryCode);
                        row.SetField("Address", new[] {
                                ((terminalSource.address.streetAbbr.IsNotEmpty() ? terminalSource.address.streetAbbr + ". " : null) + terminalSource.address.street),
                                terminalSource.address.houseNo.IsNotEmpty() ? "д. " + terminalSource.address.houseNo : null,
                                terminalSource.address.building.IsNotEmpty() ? "корп. " + terminalSource.address.building : null,
                                terminalSource.address.structure.IsNotEmpty() ? "стр. " + terminalSource.address.structure : null
                            }.Where(x => x.IsNotEmpty())
                            .AggregateString(", "));
                        row.SetField("AddressDescription", terminalSource.address.descript);
                    }

                    if (terminalSource.geoCoordinates != null)
                    {
                        row.SetField("Latitude", (double)terminalSource.geoCoordinates.latitude);
                        row.SetField("Longitude", (double)terminalSource.geoCoordinates.longitude);
                    }

                    if (terminalSource.schedule != null)
                    {
                        row.SetField("IsSelfPickup", terminalSource.schedule.Contains(scheduleSelfPickup, scheduleComparer));
                        row.SetField("IsSelfDelivery", (tempSchedule = terminalSource.schedule.FirstOrDefault(x => x.operation == "SelfDelivery")) != null);
                        row.SetField("SelfDeliveryTimes",
                            tempSchedule != null
                                ? string.Join(", ", tempSchedule.timetable.Select(x => string.Format("{0}: {1}", x.weekDays, x.workTime)))
                                : string.Empty);
                    }

                    if (terminalSource.extraService != null)
                        row.SetField("ExtraServices", string.Join(", ", terminalSource.extraService.Select(es => es.esCode)));
                    else
                        row.SetField("ExtraServices", string.Empty);

                    if (terminalSource.services != null)
                        row.SetField("Services", string.Join(", ", terminalSource.services));
                    else
                        row.SetField("Services", string.Empty);

                    table.Rows.Add(row);

                    if (table.Rows.Count % 100 == 0)
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
                        sqlBulkCopy.DestinationTableName = "[Shipping].[DpdTerminals]";
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }
    }
}
