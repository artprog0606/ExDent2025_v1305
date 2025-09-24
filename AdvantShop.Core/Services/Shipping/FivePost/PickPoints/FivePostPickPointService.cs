using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using AdvantShop.Helpers;
using AdvantShop.Shipping.FivePost.Api;
using Newtonsoft.Json;
using AdvantShop.Core.Services.Shipping.FivePost.Helpers;
using System.Linq;

namespace AdvantShop.Shipping.FivePost.PickPoints
{
    public class FivePostPickPointService
    {
        private static FivePostPickPoint FromReader(SqlDataReader reader)
        {
            return new FivePostPickPoint
            {
                Id = SQLDataHelper.GetString(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Type = (EFivePostPickPointType)SQLDataHelper.GetInt(reader, "Type"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                ReturnAllowed = SQLDataHelper.GetBoolean(reader, "ReturnAllowed"),
                IsCash = SQLDataHelper.GetBoolean(reader, "IsCash"),
                IsCard = SQLDataHelper.GetBoolean(reader, "IsCard"),
                FiasCode = SQLDataHelper.GetString(reader, "FiasCode"),
                FullAddress = SQLDataHelper.GetString(reader, "Address"),
                RegionName = SQLDataHelper.GetString(reader, "Region"),
                CityName = SQLDataHelper.GetString(reader, "City"),
                Lattitude = SQLDataHelper.GetFloat(reader, "Lattitude"),
                Longitude = SQLDataHelper.GetFloat(reader, "Longitude"),
                Phone = SQLDataHelper.GetString(reader,"Phone"),
                TimeWork = SQLDataHelper.GetString(reader, "TimeWork"),
                WeightDimensionsLimit = new FivePostWeightDimension
                {
                    MaxHeightInMillimeters = SQLDataHelper.GetInt(reader, "MaxHeight"),
                    MaxWidthInMillimeters = SQLDataHelper.GetInt(reader, "MaxWidth"),
                    MaxLengthInMillimeters = SQLDataHelper.GetInt(reader, "MaxLength"),
                    MaxWeightInMilligrams = SQLDataHelper.GetLong(reader, "MaxWeight"),
                },
                RateList = JsonConvert.DeserializeObject<List<FivePostRate>>(
                    SQLDataHelper.GetString(reader, "RateList")),

                PossibleDeliveryList = JsonConvert.DeserializeObject<List<FivePostPossibleDelivery>>(
                    SQLDataHelper.GetString(reader, "PossibleDeliveryList"))
            };
        }

        public static List<FivePostPickPoint> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[FivePostPickPoints]",
                CommandType.Text,
                FromReader);
        }

        public static FivePostPickPoint Get(string pickPointId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Shipping].[FivePostPickPoints] WHERE [Id]=@Id",
                CommandType.Text,
                FromReader,
                new SqlParameter("@Id", pickPointId));
        }

        public static FivePostPickPoint GetFirst()
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Shipping].[FivePostPickPoints]",
                CommandType.Text,
                FromReader);
        }

        public static bool ExistsPickPoints()
        {
            return SQLDataAccess.ExecuteScalar<bool>("SELECT CASE WHEN EXISTS(SELECT TOP 1 [Id] FROM [Shipping].[FivePostPickPoints]) THEN 1 ELSE 0 END", CommandType.Text);
        }

        public static void Add(FivePostPickPoint pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Shipping].[FivePostPickPoints] " +
                "(Id,Name,Type,Description,MaxWidth,MaxHeight,MaxLength,MaxWeight,ReturnAllowed,IsCash,IsCard,FiasCode,Lattitude,Longitude," +
                "Address,Region,City,RateList,PossibleDeliveryList,Phone,TimeWork,LastUpdate)" +
                "VALUES(@Id,@Name,@Type,@Description,@MaxWidth,@MaxHeight,@MaxLength,@MaxWeight,@ReturnAllowed,@IsCash,@IsCard,@FiasCode,@Lattitude,@Longitude," +
                "@Address,@Region,@City,@RateList,@PossibleDeliveryList,@Phone,@TimeWork,@LastUpdate)",
                CommandType.Text,
                new SqlParameter("@Id", pickPoint.Id),
                new SqlParameter("@Name", pickPoint.Name),
                new SqlParameter("@Type", (int)pickPoint.Type),
                new SqlParameter("@Description", pickPoint.Description),
                new SqlParameter("@MaxWidth", pickPoint.WeightDimensionsLimit.MaxWidthInMillimeters),
                new SqlParameter("@MaxHeight", pickPoint.WeightDimensionsLimit.MaxHeightInMillimeters),
                new SqlParameter("@MaxLength", pickPoint.WeightDimensionsLimit.MaxLengthInMillimeters),
                new SqlParameter("@MaxWeight", pickPoint.WeightDimensionsLimit.MaxWeightInMilligrams),
                new SqlParameter("@ReturnAllowed", pickPoint.ReturnAllowed),
                new SqlParameter("@IsCash", pickPoint.IsCash),
                new SqlParameter("@IsCard", pickPoint.IsCard),
                new SqlParameter("@FiasCode", pickPoint.FiasCode ?? string.Empty),
                new SqlParameter("@Address", pickPoint.FullAddress),
                new SqlParameter("@Region", pickPoint.RegionName),
                new SqlParameter("@City", pickPoint.CityName),
                new SqlParameter("@RateList", JsonConvert.SerializeObject(pickPoint.RateList) ?? (object)DBNull.Value),
                new SqlParameter("@PossibleDeliveryList", JsonConvert.SerializeObject(pickPoint.PossibleDeliveryList) ?? (object)DBNull.Value),
                new SqlParameter("@Lattitude", pickPoint.Lattitude),
                new SqlParameter("@Longitude", pickPoint.Longitude),
                new SqlParameter("@Phone", pickPoint.Phone),
                new SqlParameter("@TimeWork", pickPoint.TimeWork),
                new SqlParameter("@LastUpdate", DateTime.Now)
                );
        }

        public static void Update(FivePostPickPoint pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Shipping].[FivePostPickPoints]" +
                "SET " +
                "Name=@Name" +
                ",Type=@Type" +
                ",Description=@Description" +
                ",MaxWidth=@MaxWidth" +
                ",MaxHeight=@MaxHeight" +
                ",MaxLength=@MaxLength" +
                ",MaxWeight=@MaxWeight" +
                ",ReturnAllowed=@ReturnAllowed" +
                ",IsCash=@IsCash" +
                ",IsCard=@IsCard" +
                ",FiasCode=@FiasCode" +
                ",Lattitude=@Lattitude" +
                ",Longitude=@Longitude" +
                ",Address=@Address" +
                ",Region=@Region" +
                ",City=@City" +
                ",RateList=@RateList" +
                ",PossibleDeliveryList=@PossibleDeliveryList" +
                ",Phone=@Phone" +
                ",TimeWork=@TimeWork" +
                ",[LastUpdate]=@LastUpdate " +
                "WHERE Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", pickPoint.Id),
                new SqlParameter("@Name", pickPoint.Name),
                new SqlParameter("@Type", (int)pickPoint.Type),
                new SqlParameter("@Description", pickPoint.Description),
                new SqlParameter("@MaxWidth", pickPoint.WeightDimensionsLimit.MaxWidthInMillimeters),
                new SqlParameter("@MaxHeight", pickPoint.WeightDimensionsLimit.MaxHeightInMillimeters),
                new SqlParameter("@MaxLength", pickPoint.WeightDimensionsLimit.MaxLengthInMillimeters),
                new SqlParameter("@MaxWeight", pickPoint.WeightDimensionsLimit.MaxWeightInMilligrams),
                new SqlParameter("@ReturnAllowed", pickPoint.ReturnAllowed),
                new SqlParameter("@IsCash", pickPoint.IsCash),
                new SqlParameter("@IsCard", pickPoint.IsCard),
                new SqlParameter("@FiasCode", pickPoint.FiasCode ?? string.Empty),
                new SqlParameter("@Address", pickPoint.FullAddress),
                new SqlParameter("@Region", pickPoint.RegionName),
                new SqlParameter("@City", pickPoint.CityName),
                new SqlParameter("@Lattitude", pickPoint.Lattitude),
                new SqlParameter("@Longitude", pickPoint.Longitude),
                new SqlParameter("@Phone", pickPoint.Phone),
                new SqlParameter("@TimeWork", pickPoint.TimeWork),
                new SqlParameter("@RateList", JsonConvert.SerializeObject(pickPoint.RateList) ?? (object)DBNull.Value),
                new SqlParameter("@PossibleDeliveryList", JsonConvert.SerializeObject(pickPoint.PossibleDeliveryList) ?? (object)DBNull.Value),
                new SqlParameter("@LastUpdate", DateTime.Now)
                );
        }

        public static void RemoveOld(DateTime startAt)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"DELETE FROM [Shipping].[FivePostPickPoints] WHERE [LastUpdate] < @startAt",
                CommandType.Text,
                new SqlParameter("startAt", startAt));
        }

        public static List<FivePostPickPoint> Find(string region, string city, FivePostWeightDimension weightDimension = null)
        {
            if (region.IsNullOrEmpty() && city.IsNullOrEmpty())
                return GetList();

            var listParams = new List<SqlParameter>();
            var where = new List<string>();

            if (region.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@Region", region.RemoveTypeFromRegion()));
                where.Add("[Region] LIKE '%' + @Region + '%'");
            }

            if (city.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@City", city));
                where.Add("[City] LIKE '%' + @City + '%'");
            }

            if (weightDimension != null)
            {
                listParams.Add(new SqlParameter("@Weight", weightDimension.MaxWeightInMilligrams));
                where.Add("[MaxWeight] >= @Weight");

                listParams.Add(new SqlParameter("@Width", weightDimension.MaxWidthInMillimeters));
                where.Add("[MaxWidth] >= @Width");

                listParams.Add(new SqlParameter("@Length", weightDimension.MaxLengthInMillimeters));
                where.Add("[MaxLength] >= @Length");

                listParams.Add(new SqlParameter("@Height", weightDimension.MaxHeightInMillimeters));
                where.Add("[MaxHeight] >= @Height");
            }

            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[FivePostPickPoints] " +
                "WHERE " + string.Join(" AND ", where),
                CommandType.Text,
                FromReader,
                listParams.ToArray());
        }

        public static void Sync(FivePostApiService apiService)
        {
            var isEmptyPickPoints = !ExistsPickPoints();
            var startDate = DateTime.Now;

            var tablePickPointsBulk =
                isEmptyPickPoints
                    ? SQLDataAccess.ExecuteTable(@"SELECT * FROM [Shipping].[FivePostPickPoints]", CommandType.Text)
                    : null;

            var pickPoints = apiService.GetPickPoints(new FivePostPickPointParams()) ?? new List<Api.FivePostPickPoint>();

            foreach (var pickPointSource in pickPoints)
            {
                if (pickPointSource.PossibleDeliveryList == null || pickPointSource.PossibleDeliveryList.Count == 0
                    || pickPointSource.RateList == null || pickPointSource.RateList.Count == 0 || pickPointSource.RateList.All(x => x.ValueWithVat == 0 || x.ZoneName.IsNullOrEmpty()))
                    continue;

                var pickPoint = isEmptyPickPoints
                    ? null
                    : Get(pickPointSource.Id);

                var isNew = pickPoint == null;

                if (pickPoint == null)
                    pickPoint = new FivePostPickPoint();

                pickPoint.Id = pickPointSource.Id;
                pickPoint.Type = pickPointSource.Type;
                pickPoint.Name = pickPointSource.Name.Reduce(255);
                pickPoint.Description = pickPointSource.Description;
                pickPoint.WeightDimensionsLimit = pickPointSource.WeightDimensionsLimit;
                pickPoint.PossibleDeliveryList = pickPointSource.PossibleDeliveryList;
                pickPoint.ReturnAllowed = pickPointSource.ReturnAllowed;
                pickPoint.IsCard = pickPointSource.IsCard;
                pickPoint.IsCash = pickPointSource.IsCash;
                pickPoint.FiasCode = pickPointSource.FiasCode?.Reduce(255);
                pickPoint.RateList = pickPointSource.RateList;
                pickPoint.Lattitude = pickPointSource.Address.Latitude;
                pickPoint.Longitude = pickPointSource.Address.Longitude;
                pickPoint.FullAddress = pickPointSource.Address.ToString();
                pickPoint.RegionName = pickPointSource.Address.RegionName.Reduce(255);
                pickPoint.CityName = pickPointSource.Address.CityName.Reduce(255);
                pickPoint.Phone = pickPointSource.Phone.Reduce(30);
                pickPoint.TimeWork = FivePostHelper.TimeWorkToString(pickPointSource.WorkHours).Reduce(255);

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
                    row.SetField("Name", pickPoint.Name);
                    row.SetField("Type", (int)pickPoint.Type);
                    row.SetField("Description", pickPoint.Description);
                    row.SetField("MaxWidth", pickPoint.WeightDimensionsLimit.MaxWidthInMillimeters);
                    row.SetField("MaxHeight", pickPoint.WeightDimensionsLimit.MaxHeightInMillimeters);
                    row.SetField("MaxLength", pickPoint.WeightDimensionsLimit.MaxLengthInMillimeters);
                    row.SetField("MaxWeight", pickPoint.WeightDimensionsLimit.MaxWeightInMilligrams);
                    row.SetField("ReturnAllowed", pickPoint.ReturnAllowed);
                    row.SetField("IsCash", pickPoint.IsCash);
                    row.SetField("IsCard", pickPoint.IsCard);
                    row.SetField("FiasCode", pickPoint.FiasCode ?? string.Empty);
                    row.SetField("Address", pickPoint.FullAddress);
                    row.SetField("Region", pickPoint.RegionName);
                    row.SetField("City", pickPoint.CityName);
                    row.SetField("Lattitude", pickPoint.Lattitude);
                    row.SetField("Longitude", pickPoint.Longitude);
                    row.SetField("Phone", pickPoint.Phone);
                    row.SetField("TimeWork", pickPoint.TimeWork);
                    row.SetField("RateList", JsonConvert.SerializeObject(pickPoint.RateList) ?? (object)DBNull.Value);
                    row.SetField("PossibleDeliveryList", JsonConvert.SerializeObject(pickPoint.PossibleDeliveryList) ?? (object)DBNull.Value);
                    row.SetField("LastUpdate", startDate);

                    tablePickPointsBulk.Rows.Add(row);

                    if (tablePickPointsBulk.Rows.Count % 100 == 0)
                        InsertBulk(tablePickPointsBulk);
                }
            }

            if (isEmptyPickPoints)
                InsertBulk(tablePickPointsBulk);
            else if (pickPoints.Count != 0)
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
                        sqlBulkCopy.DestinationTableName = "[Shipping].[FivePostPickPoints]";
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }
    }
}
