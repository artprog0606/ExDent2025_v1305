using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Shipping.LPost.Api;

namespace AdvantShop.Shipping.LPost.PickPoints
{
    public class LPostPickPointService
    {
        public static LPostPickPoint Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Shipping].[LPostPickPoints] WHERE [Id] = @Id",
                CommandType.Text,
                FromReader,
                new SqlParameter("@Id", id));
        }

        public static LPostPickPoint FromReader(SqlDataReader reader)
        {
            return new LPostPickPoint
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                RegionCode = SQLDataHelper.GetInt(reader, "RegionCode"),
                City = SQLDataHelper.GetString(reader, "City"),
                Address = SQLDataHelper.GetString(reader, "Address"),
                AddressDescription = SQLDataHelper.GetString(reader, "AddressDescription"),
                Latitude = SQLDataHelper.GetFloat(reader, "Latitude"),
                Longitude = SQLDataHelper.GetFloat(reader, "Longitude"),
                Cash = SQLDataHelper.GetBoolean(reader, "Cash"),
                Card = SQLDataHelper.GetBoolean(reader, "Card"),
                IsCourier = SQLDataHelper.GetBoolean(reader, "IsCourier"),
                RegionName = SQLDataHelper.GetString(reader, "RegionName")
            };
        }

        public static bool ExistsPickPoints()
        {
            return SQLDataAccess.ExecuteScalar<bool>("SELECT CASE WHEN EXISTS(SELECT TOP 1 [Id] FROM [Shipping].[LPostPickPoints]) THEN 1 ELSE 0 END", CommandType.Text);
        }

        public static List<LPostPickPoint> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[LPostPickPoints]",
                CommandType.Text,
                FromReader);
        }

        public static void Add(LPostPickPoint pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Shipping].[LPostPickPoints] ([Id],[RegionCode],[City],[Address],[AddressDescription],[Latitude],[Longitude],
                    [Cash],[Card],[IsCourier],[LastUpdate],[RegionName])
                VALUES
	                (@Id,@RegionCode,@City,@Address,@AddressDescription,@Latitude,@Longitude,@Cash,@Card,@IsCourier,@LastUpdate,@RegionName)",
                CommandType.Text,
                new SqlParameter("@Id", pickPoint.Id),
                new SqlParameter("@RegionCode", pickPoint.RegionCode),
                new SqlParameter("@City", pickPoint.City ?? (object)DBNull.Value),
                new SqlParameter("@Address", pickPoint.Address ?? (object)DBNull.Value),
                new SqlParameter("@AddressDescription", pickPoint.AddressDescription ?? (object)DBNull.Value),
                new SqlParameter("@Latitude", pickPoint.Latitude),
                new SqlParameter("@Longitude", pickPoint.Longitude),
                new SqlParameter("@Cash", pickPoint.Cash),
                new SqlParameter("@Card", pickPoint.Card),
                new SqlParameter("@IsCourier", pickPoint.IsCourier),
                new SqlParameter("@LastUpdate", DateTime.Now),
                new SqlParameter("@RegionName", pickPoint.RegionName)
            );
        }
  
        public static void Update(LPostPickPoint pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Shipping].[LPostPickPoints]
                   SET [RegionCode] = @RegionCode
                      ,[City] = @City
                      ,[Address] = @Address
                      ,[AddressDescription] = @AddressDescription
                      ,[Latitude] = @Latitude
                      ,[Longitude] = @Longitude
                      ,[Cash] = @Cash
                      ,[Card] = @Card
                      ,[IsCourier] = @IsCourier
                      ,[LastUpdate] = @LastUpdate
                      ,[RegionName] = @RegionName
                 WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", pickPoint.Id),
                new SqlParameter("@RegionCode", pickPoint.RegionCode),
                new SqlParameter("@City", pickPoint.City ?? (object) DBNull.Value),
                new SqlParameter("@Address", pickPoint.Address ?? (object) DBNull.Value),
                new SqlParameter("@AddressDescription", pickPoint.AddressDescription ?? (object) DBNull.Value),
                new SqlParameter("@Latitude", pickPoint.Latitude),
                new SqlParameter("@Longitude", pickPoint.Longitude),
                new SqlParameter("@Cash", pickPoint.Cash),
                new SqlParameter("@Card", pickPoint.Card),
                new SqlParameter("@IsCourier", pickPoint.IsCourier),
                new SqlParameter("@LastUpdate", DateTime.Now),
                new SqlParameter("@RegionName", pickPoint.RegionName)
                );
        }
   
        public static void RemoveOld(DateTime startAt)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"DELETE FROM [Shipping].[LPostPickPoints] WHERE [LastUpdate] < @startAt",
                CommandType.Text,
                new SqlParameter("startAt", startAt));
        }

        public static List<LPostPickPoint> Find(string region, string city, bool? isCourier)
        {
            if (region.IsNullOrEmpty() && city.IsNullOrEmpty() && !isCourier.HasValue)
                return GetList();

            var listParams = new List<SqlParameter>();
            var where = new List<string>();

            if (region.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@RegionName", region));
                where.Add("[RegionName] = @RegionName");
            }

            if (city.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@City", city));
                where.Add("[City] LIKE '%' + @City + '%'");
            }

            if (isCourier.HasValue)
            {
                listParams.Add(new SqlParameter("@IsCourier", isCourier));
                where.Add("[IsCourier] = @IsCourier");
            }

            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[LPostPickPoints] " +
                "WHERE " + string.Join(" AND ", where),
                CommandType.Text,
                FromReader,
                listParams.ToArray());
        }

        public static void Sync(LPostApiService apiService)
        {
            var isEmptyPickPoints = !ExistsPickPoints();
            var startDate = DateTime.Now;

            var tablePickPointsBulk = 
                isEmptyPickPoints
                    ? SQLDataAccess.ExecuteTable(@"SELECT * FROM [Shipping].[LPostPickPoints]", CommandType.Text)
                    : null; 
            
            var pickPoints = apiService.GetPickupPoints(null) ?? new List<Api.LPostPickPoint>();

            foreach (var pickPointSource in pickPoints)
            {
                var pickPoint = isEmptyPickPoints
                    ? null
                    : Get(pickPointSource.PickPointId);

                var isNew = pickPoint == null;

                if (pickPoint == null)
                    pickPoint = new LPostPickPoint();

                pickPoint.Id = pickPointSource.PickPointId;
                pickPoint.Address = pickPointSource.Address.Reduce(255);
                pickPoint.AddressDescription = pickPointSource.AddressDescription;
                pickPoint.City = pickPointSource.City.Reduce(255);
                pickPoint.RegionCode = pickPointSource.RegionCode;
                pickPoint.Latitude = pickPointSource.Latitude;
                pickPoint.Longitude = pickPointSource.Longitude;
                pickPoint.Cash = pickPointSource.IsCash == EPickPointCash.HaveCash;
                pickPoint.Card = pickPointSource.IsCard == EPickPointCard.HaveCard;
                pickPoint.IsCourier = pickPointSource.IsCourier == EPickPointCourier.Courier;
                pickPoint.RegionName = LPostApiService.GetRegionNameByCode(pickPointSource.RegionCode);

                if (!isEmptyPickPoints)
                {
                    if (isNew)
                        Add(pickPoint);
                    else
                        Update(pickPoint);
                }
                else
                {
                    var row = tablePickPointsBulk.NewRow();

                    row.SetField("Id", pickPoint.Id);
                    row.SetField("City", pickPoint.City);
                    row.SetField("RegionCode", pickPoint.RegionCode);
                    row.SetField("Address", pickPoint.Address);
                    row.SetField("AddressDescription", pickPoint.AddressDescription);
                    row.SetField("Latitude", pickPoint.Latitude);
                    row.SetField("Longitude", pickPoint.Longitude);
                    row.SetField("Cash", pickPoint.Cash);
                    row.SetField("Card", pickPoint.Card);
                    row.SetField("IsCourier", pickPoint.IsCourier);
                    row.SetField("RegionName", pickPoint.RegionName);
                    row.SetField("LastUpdate", startDate);

                    tablePickPointsBulk.Rows.Add(row);

                    if (tablePickPointsBulk.Rows.Count % 100 == 0)
                        InsertBulk(tablePickPointsBulk);
                }
            }

            if (isEmptyPickPoints)
                InsertBulk(tablePickPointsBulk);
            else
                RemoveOld(startDate);
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
                        sqlBulkCopy.DestinationTableName = "[Shipping].[LPostPickPoints]";
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }
    }
}