using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Shipping.RussianPost.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Shipping.RussianPost.PickPoints
{
    public class PickPointsServices
    {
        public static PickPointRussianPost Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Shipping].[RussianPostPickPoints] WHERE [Id] = @Id",
                CommandType.Text,
                FromReader,
                new SqlParameter("@Id", id));
        }

        public static PickPointRussianPost FromReader(SqlDataReader reader)
        {
            var pickPoint = new PickPointRussianPost
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                TypePoint = (EnTypePoint)SQLDataHelper.GetInt(reader, "TypePoint"),
                Region = SQLDataHelper.GetString(reader, "Region"),
                Area = SQLDataHelper.GetString(reader, "Area"),
                City = SQLDataHelper.GetString(reader, "City"),
                Address = SQLDataHelper.GetString(reader, "Address"),
                AddressDescription = SQLDataHelper.GetString(reader, "AddressDescription"),
                BrandName = SQLDataHelper.GetString(reader, "BrandName"),
                Latitude = SQLDataHelper.GetFloat(reader, "Latitude"),
                Longitude = SQLDataHelper.GetFloat(reader, "Longitude"),
                Cash = SQLDataHelper.GetNullableBoolean(reader, "Cash"),
                Card = SQLDataHelper.GetNullableBoolean(reader, "Card"),
                Type = SQLDataHelper.GetString(reader, "Type"),
                WorkTime = SQLDataHelper.GetString(reader, "WorkTime"),
                WeightLimit = SQLDataHelper.GetNullableFloat(reader, "WeightLimit"),
            };
            var dimensionLimit = SQLDataHelper.GetString(reader, "DimensionLimit");
            if (dimensionLimit.IsNotEmpty())
                pickPoint.DimensionLimit = new EnDimensionType(dimensionLimit);

            return pickPoint;
        }

        public static bool HasPickPoint(int id)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS (SELECT * FROM [Shipping].[RussianPostPickPoints] WHERE [Id] = @Id) 
                SELECT 1 
                ELSE
                SELECT 0",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static bool ExistsPickPoints()
        {
            return SQLDataAccess.ExecuteScalar<bool>("SELECT CASE WHEN EXISTS(SELECT [Id] FROM [Shipping].[RussianPostPickPoints]) THEN 1 ELSE 0 END", CommandType.Text);
        }

        public static List<PickPointRussianPost> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[RussianPostPickPoints]",
                CommandType.Text,
                FromReader);
        }

        public static void Add(PickPointRussianPost pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Shipping].[RussianPostPickPoints] ([Id],[TypePoint],[Region],[Area],[City],
                    [Address],[AddressDescription],[BrandName],[Latitude],[Longitude],[Cash],[Card],[Type],[WorkTime],
                    [WeightLimit],[DimensionLimit],[LastUpdate])
                VALUES
	                (@Id,@TypePoint,@Region,@Area,@City,@Address,@AddressDescription,@BrandName,@Latitude,@Longitude,
                    @Cash,@Card,@Type,@WorkTime,@WeightLimit,@DimensionLimit,@LastUpdate)",
                CommandType.Text,
                new SqlParameter("@Id", pickPoint.Id),
                new SqlParameter("@TypePoint", (byte) pickPoint.TypePoint),
                new SqlParameter("@Region", pickPoint.Region ?? (object) DBNull.Value),
                new SqlParameter("@Area", pickPoint.Area ?? (object) DBNull.Value),
                new SqlParameter("@City", pickPoint.City ?? (object) DBNull.Value),
                new SqlParameter("@Address", pickPoint.Address ?? (object) DBNull.Value),
                new SqlParameter("@AddressDescription", pickPoint.AddressDescription ?? (object) DBNull.Value),
                new SqlParameter("@BrandName", pickPoint.BrandName ?? (object) DBNull.Value),
                new SqlParameter("@Latitude", pickPoint.Latitude),
                new SqlParameter("@Longitude", pickPoint.Longitude),
                new SqlParameter("@Cash", pickPoint.Cash ?? (object) DBNull.Value),
                new SqlParameter("@Card", pickPoint.Card ?? (object) DBNull.Value),
                new SqlParameter("@Type", pickPoint.Type ?? (object) DBNull.Value),
                new SqlParameter("@WorkTime", pickPoint.WorkTime ?? (object) DBNull.Value),
                new SqlParameter("@WeightLimit", pickPoint.WeightLimit ?? (object) DBNull.Value),
                new SqlParameter("@DimensionLimit",
                    pickPoint.DimensionLimit != null
                        ? pickPoint.DimensionLimit.Value
                        : (object) DBNull.Value),
                new SqlParameter("@LastUpdate", DateTime.Now)
            );
        }
  
        public static void Update(PickPointRussianPost pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Shipping].[RussianPostPickPoints]
                   SET [TypePoint] = @TypePoint
                      ,[Region] = @Region
                      ,[Area] = @Area
                      ,[City] = @City
                      ,[Address] = @Address
                      ,[AddressDescription] = @AddressDescription
                      ,[BrandName] = @BrandName
                      ,[Latitude] = @Latitude
                      ,[Longitude] = @Longitude
                      ,[Cash] = @Cash
                      ,[Card] = @Card
                      ,[Type] = @Type
                      ,[WorkTime] = @WorkTime
                      ,[WeightLimit] = @WeightLimit
                      ,[DimensionLimit] = @DimensionLimit
                      ,[LastUpdate] = @LastUpdate
                 WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", pickPoint.Id),
                new SqlParameter("@TypePoint", (byte) pickPoint.TypePoint),
                new SqlParameter("@Region", pickPoint.Region ?? (object) DBNull.Value),
                new SqlParameter("@Area", pickPoint.Area ?? (object) DBNull.Value),
                new SqlParameter("@City", pickPoint.City ?? (object) DBNull.Value),
                new SqlParameter("@Address", pickPoint.Address ?? (object) DBNull.Value),
                new SqlParameter("@AddressDescription", pickPoint.AddressDescription ?? (object) DBNull.Value),
                new SqlParameter("@BrandName", pickPoint.BrandName ?? (object) DBNull.Value),
                new SqlParameter("@Latitude", pickPoint.Latitude),
                new SqlParameter("@Longitude", pickPoint.Longitude),
                new SqlParameter("@Cash", pickPoint.Cash ?? (object) DBNull.Value),
                new SqlParameter("@Card", pickPoint.Card ?? (object) DBNull.Value),
                new SqlParameter("@Type", pickPoint.Type ?? (object) DBNull.Value),
                new SqlParameter("@WorkTime", pickPoint.WorkTime ?? (object) DBNull.Value),
                new SqlParameter("@WeightLimit", pickPoint.WeightLimit ?? (object) DBNull.Value),
                new SqlParameter("@DimensionLimit",
                    pickPoint.DimensionLimit != null
                        ? pickPoint.DimensionLimit.Value
                        : (object) DBNull.Value),
                new SqlParameter("@LastUpdate", DateTime.Now)
                );
        }
     
        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(@"DELETE FROM [Shipping].[RussianPostPickPoints] WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }    
   
        public static void RemoveOld(DateTime startAt)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"DELETE FROM [Shipping].[RussianPostPickPoints] WHERE [LastUpdate] < @startAt",
                CommandType.Text,
                new SqlParameter("startAt", startAt));
        }

        public static List<PickPointRussianPost> Find(string region, string city, EnTypePoint? typePoint)
        {
            if (region.IsNullOrEmpty() && city.IsNullOrEmpty())
                return GetList();

            var listParams = new List<SqlParameter>();
            var where = new List<string>();

            if (region.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@Region", region.RemoveTypeFromRegion()));
                where.Add("[Region] = @Region");
            }

            if (city.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@City", city));
                where.Add("[City] LIKE '%' + @City + '%'");
            }

            if (typePoint.HasValue)
            {
                listParams.Add(new SqlParameter("@TypePoint", (byte)typePoint.Value));
                where.Add("[typePoint] = @typePoint");
            }

            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[RussianPostPickPoints] " +
                "WHERE " + string.Join(" AND ", where),
                CommandType.Text,
                FromReader,
                listParams.ToArray());
        }

        public static List<PickPointRussianPost> FindByBounds(float topLeftLatitude, float topLeftLongitude,
            float bottomRightLatitude, float bottomRightLongitude, EnTypePoint? typePoint)
        {
            var listParams = new List<SqlParameter>();
            var where = new List<string>();

            if (typePoint.HasValue)
            {
                listParams.Add(new SqlParameter("@TypePoint", (byte)typePoint.Value));
                where.Add("[typePoint] = @typePoint");
            }
            listParams.Add(new SqlParameter("@topLeftLatitude", topLeftLatitude));
            where.Add("@topLeftLatitude > [Latitude]");
            listParams.Add(new SqlParameter("@topLeftLongitude", topLeftLongitude));
            where.Add("@topLeftLongitude < [Longitude]");
            listParams.Add(new SqlParameter("@bottomRightLatitude", bottomRightLatitude));
            where.Add("@bottomRightLatitude < [Latitude]");
            listParams.Add(new SqlParameter("@bottomRightLongitude", bottomRightLongitude));
            where.Add("@bottomRightLongitude > [Longitude]");

            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[RussianPostPickPoints] " +
                "WHERE " + string.Join(" AND ", where),
                CommandType.Text,
                FromReader,
                listParams.ToArray());
        }

        public static void Sync(RussianPostApiService apiService)
        {
            var isEmptyPickPoints = !ExistsPickPoints();
            var startDate = DateTime.Now;

            var tablePickPointsBulk = 
                isEmptyPickPoints
                    ? SQLDataAccess.ExecuteTable(@"SELECT * FROM [Shipping].[RussianPostPickPoints]", CommandType.Text)
                    : null;

            LoadOffice(apiService, TypeOffice.Ops, isEmptyPickPoints, tablePickPointsBulk);
            LoadOffice(apiService, TypeOffice.Aps, isEmptyPickPoints, tablePickPointsBulk);
            
            if (isEmptyPickPoints)
                InsertBulk(tablePickPointsBulk);
            else
                RemoveOld(startDate);
        }
        
        private static void LoadOffice(RussianPostApiService apiService, TypeOffice typeOffice,
            bool isEmptyPickPoints, DataTable tablePickPointsBulk)
        {
            var type = string.Empty;
            switch (typeOffice)
            {
                case TypeOffice.Ops:
                    type = "OPS";
                    break;
                case TypeOffice.Pvz:
                    type = "PVZ";
                    throw new ArgumentException("Type office PVZ is not supported", nameof(typeOffice));
                    break;
                case TypeOffice.Aps:
                    type = "APS";
                    break;
                case TypeOffice.All:
                    type = "ALL";
                    throw new ArgumentException("Type office ALL is not supported", nameof(typeOffice));
                    break;
            }
            
            var path = Path.Combine(FoldersHelper.GetPathAbsolut(FolderType.PriceTemp), "RussianPostOffice");
            FileHelpers.DeleteDirectory(path);// очищаем
            FileHelpers.CreateDirectory(path);// создаем, если еще нет

            var fileZip = apiService.GetOfficesFile(type, path);
            if (fileZip.IsNullOrEmpty())
                throw new Exception("RussianPost: Не получен архив с объектами " + type);
      
            if (FileHelpers.UnZipFile(fileZip))
            {
                FileHelpers.DeleteFile(fileZip);
                var files = Directory.GetFiles(path);
                var fileOffices = files.FirstOrDefault();
                if (!string.IsNullOrEmpty(fileOffices))
                {
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        }
                    });
                    Office office = null;
                    
                    // читает объемные файлы с малым потреблением памяти
                    using (FileStream s = File.Open(fileOffices, FileMode.Open))
                    using (StreamReader sr = new StreamReader(s))
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonToken.StartObject)
                                    {
                                        office = serializer.Deserialize<Office>(reader);
                                        var id = office.Address.Index.TryParseInt(true);

                                        if (id.HasValue)
                                        {
                                            var pickPoint = isEmptyPickPoints
                                                ? null
                                                : Get(id.Value);

                                            var isNew = pickPoint == null;

                                            if (pickPoint == null)
                                                pickPoint = new PickPointRussianPost();
                                            
                                            pickPoint.Id = id.Value;
                                            pickPoint.TypePoint = (EnTypePoint)Enum.Parse(typeof(EnTypePoint), typeOffice.ToString(), true);
                                            
                                            pickPoint.Region = office.Address.Region?.RemoveTypeFromRegion().Reduce(255) ?? string.Empty;
                                            if (pickPoint.Region.StartsWith("г ", StringComparison.OrdinalIgnoreCase))
                                                pickPoint.Region = pickPoint.Region.Remove(0, 2);
                                            
                                            pickPoint.Area = office.Address.Area?.Reduce(255);
                                            pickPoint.City = office.Address.Place?.Reduce(255) ?? string.Empty;
                                            pickPoint.Address = office.Address.AddressFromLocation().Reduce(255);
                                            pickPoint.AddressDescription = office.EcomOptions?.Getto;
                                            pickPoint.BrandName = office.BrandName?.Reduce(255);
                                            pickPoint.Latitude = office.Latitude;
                                            pickPoint.Longitude = office.Longitude;
                                            if (typeOffice == TypeOffice.Aps)
                                            {
                                                // Для почтаматов наложенный платеж не доступен.
                                                pickPoint.Cash = false; // office.EcomOptions?.CashPayment;
                                                pickPoint.Card = false; // office.EcomOptions?.CardPayment;
                                            }
                                            else
                                            {
                                                // Для отделений оплата наличными всегда доступна, оплата картой не всегда в каких-то глухих глубинках.
                                                pickPoint.Cash = true;
                                                pickPoint.Card = true;
                                            }

                                            pickPoint.Type = office.Type?.Reduce(255);
                                            pickPoint.WorkTime =
                                                office.WorkTime != null
                                                    ? string.Join("; ", office.WorkTime).Reduce(455)
                                                    : null;

                                            if (typeOffice == TypeOffice.Aps)
                                            {
                                                pickPoint.WeightLimit = office.EcomOptions?.WeightLimit;
                                                if (pickPoint.WeightLimit == 0f)
                                                    pickPoint.WeightLimit = null;

                                                pickPoint.DimensionLimit =
                                                    EnDimensionType.Parse(
                                                        office.EcomOptions?.TypesizeVal?.Replace("Коробка ",
                                                            string.Empty));
                                            }
                                            else
                                            {
                                                pickPoint.WeightLimit = null;
                                                pickPoint.DimensionLimit = null;
                                            }
                                        
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
                                                row.SetField("TypePoint", (byte) pickPoint.TypePoint);
                                                row.SetField("Region", pickPoint.Region ?? (object) DBNull.Value);
                                                row.SetField("Area", pickPoint.Area ?? (object) DBNull.Value);
                                                row.SetField("City", pickPoint.City ?? (object) DBNull.Value);
                                                row.SetField("Address", pickPoint.Address ?? (object) DBNull.Value);
                                                row.SetField("AddressDescription",
                                                    pickPoint.AddressDescription ?? (object) DBNull.Value);
                                                row.SetField("BrandName", pickPoint.BrandName ?? (object) DBNull.Value);
                                                row.SetField("Latitude", pickPoint.Latitude);
                                                row.SetField("Longitude", pickPoint.Longitude);
                                                row.SetField("Cash", pickPoint.Cash ?? (object) DBNull.Value);
                                                row.SetField("Card", pickPoint.Card ?? (object) DBNull.Value);
                                                row.SetField("Type", pickPoint.Type ?? (object) DBNull.Value);
                                                row.SetField("WorkTime", pickPoint.WorkTime ?? (object) DBNull.Value);
                                                row.SetField("WeightLimit",
                                                    pickPoint.WeightLimit ?? (object) DBNull.Value);
                                                row.SetField("DimensionLimit",
                                                    pickPoint.DimensionLimit != null
                                                        ? pickPoint.DimensionLimit.Value
                                                        : (object) DBNull.Value);
                                                row.SetField("LastUpdate", DateTime.Now);

                                                tablePickPointsBulk.Rows.Add(row);

                                                if (tablePickPointsBulk.Rows.Count % 100 == 0)
                                                    InsertBulk(tablePickPointsBulk);
                                            }

                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }

                    FileHelpers.DeleteFile(fileOffices);
                }
            }
            else
            {
                FileHelpers.DeleteFile(fileZip);
                throw new Exception("RussianPost: Не удалось распокавать архив с объектами " + fileZip);
            }
            FileHelpers.DeleteFile(fileZip);
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
                        sqlBulkCopy.DestinationTableName = "[Shipping].[RussianPostPickPoints]";
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }
    }
    
    public enum TypeOffice
    {
        Ops,
        Pvz,
        Aps,
        All
    }
}