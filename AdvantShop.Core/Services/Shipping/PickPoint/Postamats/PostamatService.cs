using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Shipping.PickPoint.Postamats
{
    public class PostamatService
    {
        public static Postamat Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Shipping].[PickPointPostamats] WHERE [Id] = @Id",
                CommandType.Text,
                FromReader,
                new SqlParameter("@Id", id));
        }

        public static Postamat Get(string number)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Shipping].[PickPointPostamats] WHERE [Number] = @Number",
                CommandType.Text,
                FromReader,
                new SqlParameter("@Number", number ?? (object)DBNull.Value));
        }
        public static bool HasPostamat(int id)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS (SELECT * FROM [Shipping].[PickPointPostamats] WHERE [Id] = @Id) 
                SELECT 1 
                ELSE
                SELECT 0",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static bool HasPostamat(string number)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS (SELECT * FROM [Shipping].[PickPointPostamats] WHERE [Number] = @Number) 
                SELECT 1 
                ELSE
                SELECT 0",
                CommandType.Text,
                new SqlParameter("@Number", number ?? (object)DBNull.Value));
        }

        public static Postamat FromReader(SqlDataReader reader)
        {
            return new Postamat
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Number = SQLDataHelper.GetString(reader, "Number"),
                OwnerName = SQLDataHelper.GetString(reader, "OwnerName"),
                TypeTitle = SQLDataHelper.GetString(reader, "TypeTitle"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                City = SQLDataHelper.GetString(reader, "City"),
                Region = SQLDataHelper.GetString(reader, "Region"),
                Country = SQLDataHelper.GetString(reader, "Country"),
                CountryIso = SQLDataHelper.GetString(reader, "CountryIso"),
                Address = SQLDataHelper.GetString(reader, "Address"),
                AddressDescription = SQLDataHelper.GetString(reader, "AddressDescription"),
                AmountTo = SQLDataHelper.GetNullableFloat(reader, "AmountTo"),
                WorkTimeSMS = SQLDataHelper.GetString(reader, "WorkTimeSMS"),
                Latitude = SQLDataHelper.GetFloat(reader, "Latitude"),
                Longitude = SQLDataHelper.GetFloat(reader, "Longitude"),
                Cash = (byte)SQLDataHelper.GetInt(reader, "Cash"),
                Card = (byte)SQLDataHelper.GetInt(reader, "Card"),
                PayPassAvailable = SQLDataHelper.GetBoolean(reader, "PayPassAvailable"),
                MaxWeight = SQLDataHelper.GetNullableFloat(reader, "MaxWeight"),
                DimensionSum = SQLDataHelper.GetNullableFloat(reader, "DimensionSum"),
                MaxHeight = SQLDataHelper.GetNullableFloat(reader, "MaxHeight"),
                MaxWidth = SQLDataHelper.GetNullableFloat(reader, "MaxWidth"),
                MaxLength = SQLDataHelper.GetNullableFloat(reader, "MaxLength"),
                SelfDelivery = SQLDataHelper.GetBoolean(reader, "SelfDelivery"),
            };
        }

        public static bool ExistsPostamats()
        {
            return SQLDataAccess.ExecuteScalar<bool>("SELECT CASE WHEN EXISTS(SELECT [Id] FROM [Shipping].[PickPointPostamats]) THEN 1 ELSE 0 END", CommandType.Text);
        }

        public static bool ExistsPostamats(string ikn)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "SELECT CASE WHEN EXISTS(SELECT * FROM [Shipping].[PickPointPostamatsIkn] WHERE [Ikn]=@Ikn) THEN 1 ELSE 0 END",
                CommandType.Text,
                new[] { new SqlParameter("@Ikn", ikn) });
        }

        public static List<Postamat> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[PickPointPostamats]",
                CommandType.Text,
                FromReader);
        }

        public static List<Postamat> GetList(string ikn)
        {
            if (ikn.IsNullOrEmpty())
                return GetList();

            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[PickPointPostamats] INNER JOIN [Shipping].[PickPointPostamatsIkn] ON [PickPointPostamats].[Id] = [PickPointPostamatsIkn].[PostamatId] WHERE [PickPointPostamatsIkn].[Ikn] = @Ikn",
                CommandType.Text,
                FromReader,
                new SqlParameter("@Ikn", ikn ?? (object)DBNull.Value));
        }

        public static List<Postamat> GetSelfDeliveryList(string region, string city, string ikn)
        {
            var listParams = new List<SqlParameter>();
            var where = new List<string>();

            if (region.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@Region", region));
                where.Add("[Region] = @Region");
            }

            if (city.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@City", city));
                where.Add("[City] = @City");
            }

            if (ikn.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@Ikn", ikn));
                where.Add("[PickPointPostamatsIkn].[Ikn] = @Ikn");
            }

            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[PickPointPostamats] " +
                (ikn.IsNotEmpty() ? "INNER JOIN [Shipping].[PickPointPostamatsIkn] ON [PickPointPostamats].[Id] = [PickPointPostamatsIkn].[PostamatId]" : string.Empty) +
                "WHERE " + string.Join(" and ", where),
                CommandType.Text,
                FromReader,
                listParams.ToArray());
        }

        public static void Add(Postamat postamat)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Shipping].[PickPointPostamats] ([Id],[Number],[OwnerName],[TypeTitle],[Name],[City],[Region],[CountryIso],
                    [Country],[Address],[AddressDescription],[AmountTo],[WorkTimeSMS],[Latitude],[Longitude],[Cash],[Card],[PayPassAvailable],
                    [MaxWeight],[DimensionSum],[MaxHeight],[MaxWidth],[MaxLength],[SelfDelivery])
                VALUES
	                (@Id,@Number,@OwnerName,@TypeTitle,@Name,@City,@Region,@CountryIso,@Country,@Address,@AddressDescription,@AmountTo,
                    @WorkTimeSMS,@Latitude,@Longitude,@Cash,@Card,@PayPassAvailable,@MaxWeight,@DimensionSum,@MaxHeight,@MaxWidth,@MaxLength,
                    @SelfDelivery)",
                CommandType.Text,
                new SqlParameter("@Id", postamat.Id),
                new SqlParameter("@Number", postamat.Number ?? (object)DBNull.Value),
                new SqlParameter("@OwnerName", postamat.OwnerName ?? (object)DBNull.Value),
                new SqlParameter("@TypeTitle", postamat.TypeTitle ?? (object)DBNull.Value),
                new SqlParameter("@Name", postamat.Name ?? (object)DBNull.Value),
                new SqlParameter("@City", postamat.City ?? (object)DBNull.Value),
                new SqlParameter("@Region", postamat.Region ?? (object)DBNull.Value),
                new SqlParameter("@CountryIso", postamat.CountryIso ?? (object)DBNull.Value),
                new SqlParameter("@Country", postamat.Country ?? (object)DBNull.Value),
                new SqlParameter("@Address", postamat.Address ?? (object)DBNull.Value),
                new SqlParameter("@AddressDescription", postamat.AddressDescription ?? (object)DBNull.Value),
                new SqlParameter("@AmountTo", postamat.AmountTo ?? (object)DBNull.Value),
                new SqlParameter("@WorkTimeSMS", postamat.WorkTimeSMS ?? (object)DBNull.Value),
                new SqlParameter("@Latitude", postamat.Latitude),
                new SqlParameter("@Longitude", postamat.Longitude),
                new SqlParameter("@Cash", postamat.Cash),
                new SqlParameter("@Card", postamat.Card),
                new SqlParameter("@PayPassAvailable", postamat.PayPassAvailable),
                new SqlParameter("@MaxWeight", postamat.MaxWeight ?? (object)DBNull.Value),
                new SqlParameter("@DimensionSum", postamat.DimensionSum ?? (object)DBNull.Value),
                new SqlParameter("@MaxHeight", postamat.MaxHeight ?? (object)DBNull.Value),
                new SqlParameter("@MaxWidth", postamat.MaxWidth ?? (object)DBNull.Value),
                new SqlParameter("@MaxLength", postamat.MaxLength ?? (object)DBNull.Value),
                new SqlParameter("@SelfDelivery", postamat.SelfDelivery)
                );
        }

        public static void Update(Postamat postamat)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Shipping].[PickPointPostamats]
                   SET [Number] = @Number
                      ,[OwnerName] = @OwnerName
                      ,[TypeTitle] = @TypeTitle
                      ,[Name] = @Name
                      ,[City] = @City
                      ,[Region] = @Region
                      ,[CountryIso] = @CountryIso
                      ,[Country] = @Country
                      ,[Address] = @Address
                      ,[AddressDescription] = @AddressDescription
                      ,[AmountTo] = @AmountTo
                      ,[WorkTimeSMS] = @WorkTimeSMS
                      ,[Latitude] = @Latitude
                      ,[Longitude] = @Longitude
                      ,[Cash] = @Cash
                      ,[Card] = @Card
                      ,[PayPassAvailable] = @PayPassAvailable
                      ,[MaxWeight] = @MaxWeight
                      ,[DimensionSum] = @DimensionSum
                      ,[MaxHeight] = @MaxHeight
                      ,[MaxWidth] = @MaxWidth
                      ,[MaxLength] = @MaxLength
                      ,[SelfDelivery] = @SelfDelivery
                 WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", postamat.Id),
                new SqlParameter("@Number", postamat.Number ?? (object)DBNull.Value),
                new SqlParameter("@OwnerName", postamat.OwnerName ?? (object)DBNull.Value),
                new SqlParameter("@TypeTitle", postamat.TypeTitle ?? (object)DBNull.Value),
                new SqlParameter("@Name", postamat.Name ?? (object)DBNull.Value),
                new SqlParameter("@City", postamat.City ?? (object)DBNull.Value),
                new SqlParameter("@Region", postamat.Region ?? (object)DBNull.Value),
                new SqlParameter("@CountryIso", postamat.CountryIso ?? (object)DBNull.Value),
                new SqlParameter("@Country", postamat.Country ?? (object)DBNull.Value),
                new SqlParameter("@Address", postamat.Address ?? (object)DBNull.Value),
                new SqlParameter("@AddressDescription", postamat.AddressDescription ?? (object)DBNull.Value),
                new SqlParameter("@AmountTo", postamat.AmountTo ?? (object)DBNull.Value),
                new SqlParameter("@WorkTimeSMS", postamat.WorkTimeSMS ?? (object)DBNull.Value),
                new SqlParameter("@Latitude", postamat.Latitude),
                new SqlParameter("@Longitude", postamat.Longitude),
                new SqlParameter("@Cash", postamat.Cash),
                new SqlParameter("@Card", postamat.Card),
                new SqlParameter("@PayPassAvailable", postamat.PayPassAvailable),
                new SqlParameter("@MaxWeight", postamat.MaxWeight ?? (object)DBNull.Value),
                new SqlParameter("@DimensionSum", postamat.DimensionSum ?? (object)DBNull.Value),
                new SqlParameter("@MaxHeight", postamat.MaxHeight ?? (object)DBNull.Value),
                new SqlParameter("@MaxWidth", postamat.MaxWidth ?? (object)DBNull.Value),
                new SqlParameter("@MaxLength", postamat.MaxLength ?? (object)DBNull.Value),
                new SqlParameter("@SelfDelivery", postamat.SelfDelivery)
                );
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(@"DELETE FROM [Shipping].[PickPointPostamats] WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }    

        public static void Delete(string number)
        {
            SQLDataAccess.ExecuteNonQuery(@"DELETE FROM [Shipping].[PickPointPostamats] WHERE [Number] = @Number",
                CommandType.Text,
                new SqlParameter("@Number", number ?? (object)DBNull.Value));
        }    

        public static void AddRef(int postamatId, string ikn)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF (NOT EXISTS(SELECT [PostamatId] FROM [Shipping].[PickPointPostamatsIkn] WHERE [PostamatId] = @PostamatId AND [Ikn] = @Ikn))
                BEGIN
	                INSERT INTO [Shipping].[PickPointPostamatsIkn] ([PostamatId],[Ikn],[LastUpdate]) VALUES (@PostamatId, @Ikn, @LastUpdate)
                END
                ELSE
                BEGIN
	                UPDATE [Shipping].[PickPointPostamatsIkn] SET [LastUpdate] = @LastUpdate WHERE [PostamatId] = @PostamatId AND [Ikn] = @Ikn
                END",
                CommandType.Text,
                new SqlParameter("@PostamatId", postamatId),
                new SqlParameter("@Ikn", ikn ?? (object)DBNull.Value),
                new SqlParameter("@LastUpdate", DateTime.Now)
                );
        }

        public static void RemoveRef(string ikn)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"DELETE FROM [Shipping].[PickPointPostamatsIkn] WHERE [Ikn] = @Ikn",
                CommandType.Text,
                new SqlParameter("@Ikn", ikn ?? (object)DBNull.Value));
        }

        public static void RemoveOldRef(string ikn, DateTime startAt)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"DELETE FROM [Shipping].[PickPointPostamatsIkn] WHERE [LastUpdate] < @startAt AND [Ikn] = @Ikn",
                CommandType.Text,
                new SqlParameter("startAt", startAt),
                new SqlParameter("@Ikn", ikn ?? (object)DBNull.Value));
        }

        public static List<Postamat> Find(string countryIso, string country, string region, string city, string ikn)
        {
            if (countryIso.IsNullOrEmpty() && country.IsNullOrEmpty() && region.IsNullOrEmpty() && city.IsNullOrEmpty())
                return GetList(ikn);

            var listParams = new List<SqlParameter>();
            var where = new List<string>();

            if (countryIso.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@CountryIso", countryIso));
                where.Add("[CountryIso] = @CountryIso");
            }
            else if (country.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@Country", country));
                where.Add("[Country] = @Country");
            }

            if (region.IsNotEmpty())
            {
                if (region.Equals("москва", StringComparison.OrdinalIgnoreCase))
                    region = "Московская обл.";
                if (region.Equals("Санкт-Петербург", StringComparison.OrdinalIgnoreCase))
                    region = "Ленинградская обл.";

                listParams.Add(new SqlParameter("@Region", region.RemoveTypeFromRegion()));
                where.Add("[Region] = @Region");
            }

            if (city.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@City", city));
                where.Add("[City] = @City");
            }

            if (ikn.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@Ikn", ikn));
                where.Add("[PickPointPostamatsIkn].[Ikn] = @Ikn");
            }

            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Shipping].[PickPointPostamats] " +
                (ikn.IsNotEmpty() ? "INNER JOIN [Shipping].[PickPointPostamatsIkn] ON [PickPointPostamats].[Id] = [PickPointPostamatsIkn].[PostamatId]" : string.Empty) +
                "WHERE " + string.Join(" and ", where),
                CommandType.Text,
                FromReader,
                listParams.ToArray());

        }

        public static List<Postamat> FindByBounds(string ikn, float topLeftLatitude, float topLeftLongitude,
            float bottomRightLatitude, float bottomRightLongitude)
        {
            var listParams = new List<SqlParameter>();
            var where = new List<string>();

            if (ikn.IsNotEmpty())
            {
                listParams.Add(new SqlParameter("@Ikn", ikn));
                where.Add("[PickPointPostamatsIkn].[Ikn] = @Ikn");
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
                "SELECT * FROM [Shipping].[PickPointPostamats] " +
                (ikn.IsNotEmpty() ? "INNER JOIN [Shipping].[PickPointPostamatsIkn] ON [PickPointPostamats].[Id] = [PickPointPostamatsIkn].[PostamatId]" : string.Empty) +
                "WHERE " + string.Join(" and ", where),
                CommandType.Text,
                FromReader,
                listParams.ToArray());
        }

        public static bool Sync(Api.PickPointApiService apiClient, string ikn)
        {
            var isEmptyPostamats = !ExistsPostamats();
            var startDate = DateTime.Now;

            var tablePostamatsBulk = 
                isEmptyPostamats
                    ? SQLDataAccess.ExecuteTable(@"SELECT * FROM [Shipping].[PickPointPostamats]", CommandType.Text)
                    : null;

            var tablePostamatsIknBulk = 
                isEmptyPostamats
                    ? SQLDataAccess.ExecuteTable(@"SELECT * FROM [Shipping].[PickPointPostamatsIkn]", CommandType.Text)
                    : null;


            var postamats = apiClient.GetPostamats(ikn) ?? new List<Api.ClientPostamat>();
            foreach (var postamatSource in postamats)
            {
                var postamat = isEmptyPostamats
                    ? null
                    : Get(postamatSource.Id);

                var isNew = postamat == null;

                if (postamat == null)
                    postamat = new Postamat();

                postamat.Id = postamatSource.Id;
                postamat.Number = postamatSource.Number;
                postamat.OwnerName = postamatSource.OwnerName.Reduce(255);
                postamat.Name = postamatSource.Name.Reduce(100);

                postamat.TypeTitle = 
                    postamatSource.TypeTitle == "АПТ"
                        ? "Постамат"
                        : postamatSource.TypeTitle == "ПВЗ"
                            ? "Пункт выдачи заказов"
                            : postamatSource.TypeTitle.Reduce(100);

                postamat.City = postamatSource.CitiName.Reduce(255);
                postamat.Region = postamatSource.Region.RemoveTypeFromRegion().Reduce(255);
                postamat.Country = postamatSource.CountryName.Reduce(255);
                postamat.CountryIso = postamatSource.CountryIso;
                postamat.Address = postamatSource.Address.Reduce(255);
                postamat.AmountTo = postamatSource.AmountTo;
                postamat.WorkTimeSMS = postamatSource.WorkTimeSMS.Reduce(100);
                postamat.AddressDescription = postamatSource.InDescription + Environment.NewLine + postamatSource.OutDescription;
                postamat.Latitude = postamatSource.Latitude;
                postamat.Longitude = postamatSource.Longitude;
                postamat.Cash = (byte)postamatSource.Cash;
                postamat.Card = (byte)postamatSource.Card;
                postamat.PayPassAvailable = postamatSource.PayPassAvailable;
                postamat.MaxWeight = postamatSource.MaxWeight;
                postamat.SelfDelivery = postamatSource.SelfDelivery;

                if (postamatSource.MaxSize == null || postamatSource.MaxSize.Length == 0)
                {
                    postamat.DimensionSum = null;
                    postamat.MaxHeight = null;
                    postamat.MaxWidth = null;
                    postamat.MaxLength = null;
                }
                else if(postamatSource.MaxSize.Length == 1)
                {
                    postamat.DimensionSum = postamatSource.MaxSize[0];
                    postamat.MaxHeight = null;
                    postamat.MaxWidth = null;
                    postamat.MaxLength = null;
                }
                else if (postamatSource.MaxSize.Length > 1)
                {
                    postamat.DimensionSum = null;

                    if (postamatSource.MaxSize.Length >= 3)
                    {
                        postamat.MaxLength = postamatSource.MaxSize[0];
                        postamat.MaxWidth = postamatSource.MaxSize[1];
                        postamat.MaxHeight = postamatSource.MaxSize[2];
                    }
                    else
                    {
                        postamat.MaxHeight = null;
                        postamat.MaxWidth = null;
                        postamat.MaxLength = null;
                    }
                }


                if (!isEmptyPostamats)
                {
                    if (isNew)
                    {
                        if (HasPostamat(postamat.Number))
                            Delete(postamat.Number);
                        Add(postamat);
                    }
                    else
                        Update(postamat);

                    AddRef(postamat.Id, ikn);
                }
                else
                {
                    var row = tablePostamatsBulk.NewRow();

                    row.SetField("Id", postamat.Id);
                    row.SetField("Number", postamat.Number);
                    row.SetField("OwnerName", postamat.OwnerName);
                    row.SetField("TypeTitle", postamat.TypeTitle);
                    row.SetField("Name", postamat.Name);
                    row.SetField("City", postamat.City);
                    row.SetField("Region", postamat.Region);
                    row.SetField("CountryIso", postamat.CountryIso);
                    row.SetField("Country", postamat.Country);
                    row.SetField("Address", postamat.Address);
                    row.SetField("AddressDescription", postamat.AddressDescription);
                    row.SetField("AmountTo", postamat.AmountTo);
                    row.SetField("WorkTimeSMS", postamat.WorkTimeSMS);
                    row.SetField("Latitude", postamat.Latitude);
                    row.SetField("Longitude", postamat.Longitude);
                    row.SetField("Cash", postamat.Cash);
                    row.SetField("Card", postamat.Card);
                    row.SetField("PayPassAvailable", postamat.PayPassAvailable);
                    row.SetField("MaxWeight", postamat.MaxWeight);
                    row.SetField("DimensionSum", postamat.DimensionSum);
                    row.SetField("MaxHeight", postamat.MaxHeight);
                    row.SetField("MaxWidth", postamat.MaxWidth);
                    row.SetField("MaxLength", postamat.MaxLength);
                    row.SetField("SelfDelivery", postamat.SelfDelivery);

                    tablePostamatsBulk.Rows.Add(row);

                    if (tablePostamatsBulk.Rows.Count % 100 == 0)
                        InsertBulk(tablePostamatsBulk, "[Shipping].[PickPointPostamats]");

                    row = tablePostamatsIknBulk.NewRow();
                    row.SetField("PostamatId", postamat.Id);
                    row.SetField("Ikn", ikn);
                    row.SetField("LastUpdate", DateTime.Now);

                    tablePostamatsIknBulk.Rows.Add(row);

                    if (tablePostamatsIknBulk.Rows.Count % 100 == 0)
                        InsertBulk(tablePostamatsIknBulk, "[Shipping].[PickPointPostamatsIkn]");

                }
            }

            if (isEmptyPostamats)
            {
                InsertBulk(tablePostamatsBulk, "[Shipping].[PickPointPostamats]");
                InsertBulk(tablePostamatsIknBulk, "[Shipping].[PickPointPostamatsIkn]");
            }
            else if (postamats.Count > 0)
                RemoveOldRef(ikn, startDate);

            return true;
        }

        private static void InsertBulk(DataTable data, string destinationTableName)
        {
            if (data.Rows.Count > 0)
            {
                using (SqlConnection dbConnection = new SqlConnection(Connection.GetConnectionString()))
                {
                    dbConnection.Open();
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(dbConnection))
                    {
                        sqlBulkCopy.DestinationTableName = destinationTableName;
                        sqlBulkCopy.WriteToServer(data);
                        data.Rows.Clear();
                    }
                    dbConnection.Close();
                }
            }
        }
    }
}
