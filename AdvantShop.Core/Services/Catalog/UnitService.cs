using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Catalog
{
    public class UnitService
    {
        private const string UnitCacheKey = "Unit_";
        private const string UnitIdCacheKey = "UnitId_";

        public static int Add(Unit unit)
        {
            unit.Id = SQLDataAccess.ExecuteScalar<int>(
                @"INSERT INTO [Catalog].[Units] ([Name],[DisplayName],[MeasureType],[SortOrder],[DateAdded],[DateModified])
                    VALUES (@Name,@DisplayName,@MeasureType,@SortOrder,GetDate(),GetDate()); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", unit.Name ?? (object) DBNull.Value),
                new SqlParameter("@DisplayName", unit.DisplayName ?? (object) DBNull.Value),
                new SqlParameter("@MeasureType", unit.MeasureType.HasValue ? (byte)unit.MeasureType : (object) DBNull.Value),
                new SqlParameter("@SortOrder", unit.SortOrder)
                );

            CacheManager.RemoveByPattern(UnitCacheKey);

            return unit.Id;
        }

        public static void Update(Unit unit)
        {
            SQLDataAccess.ExecuteNonQuery(@"UPDATE [Catalog].[Units]
                   SET [Name] = @Name
                      ,[DisplayName] = @DisplayName
                      ,[MeasureType] = @MeasureType
                      ,[SortOrder] = @SortOrder
                      ,[DateModified] = GetDate()
                 WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", unit.Id),
                new SqlParameter("@Name", unit.Name ?? (object) DBNull.Value),
                new SqlParameter("@DisplayName", unit.DisplayName ?? (object) DBNull.Value),
                new SqlParameter("@MeasureType", unit.MeasureType.HasValue ? (byte)unit.MeasureType : (object) DBNull.Value),
                new SqlParameter("@SortOrder", unit.SortOrder)
            );
            CacheManager.RemoveByPattern(UnitIdCacheKey + unit.Id);
            CacheManager.RemoveByPattern(UnitCacheKey);
        }

        public static Unit Get(int id)
        {
            return CacheManager.Get(UnitIdCacheKey + id, () =>
                SQLDataAccess.ExecuteReadOne(
                "SELECT TOP(1) * FROM [Catalog].[Units] WHERE [Id] = @Id",
                CommandType.Text,
                GetFromReader,
                new SqlParameter("@Id", id)));
        }

        public static int GetIdByDisplayName(string unitName)
        {
            return CacheManager.Get(UnitCacheKey + "Display_" + unitName.ToLower(), () =>
                SQLDataAccess.ExecuteScalar<int>("select [Id] from [Catalog].[Units] where DisplayName=@DisplayName", CommandType.Text,
                    new SqlParameter("@DisplayName", unitName))
            );
        }

        public static int GetIdByName(string name)
        {
            return CacheManager.Get(UnitCacheKey + "Name_" + name.ToLower(), () =>
                SQLDataAccess.ExecuteScalar<int>("select [Id] from [Catalog].[Units] where [Name]=@Name", CommandType.Text,
                    new SqlParameter("@Name", name))
            );
        }

        public static List<Unit> GetList()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[Units] ORDER BY SortOrder",
                CommandType.Text,
                GetFromReader);
        }

        private static Unit GetFromReader(SqlDataReader reader)
        {
            return new Unit
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                DisplayName = SQLDataHelper.GetString(reader, "DisplayName"),
                MeasureType = SQLDataHelper.IsDbNull(reader, "MeasureType") ? null : (MeasureType?)SQLDataHelper.GetInt(reader, "MeasureType"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                DateAdded = SQLDataHelper.GetDateTime(reader, "DateAdded"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified"),
            };
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery(@"DELETE FROM [Catalog].[Units] WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
            CacheManager.RemoveByPattern(UnitIdCacheKey + id);
            CacheManager.RemoveByPattern(UnitCacheKey);
        }

        public static bool IsUsed(int id)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                @"IF EXISTS (Select * From [Catalog].[Product] WHERE [Unit] = @UnitId)
                        SELECT 1
                        ELSE
                        SELECT 0",
                CommandType.Text,
                new SqlParameter("@UnitId", id)) > 0;
        }

        public static int? UnitFromString(string unitName)
        {
            if (string.IsNullOrWhiteSpace(unitName))
                return null;

            var unitId = GetIdByName(unitName);
            if (unitId != 0)
                return unitId;
            var unit = new Unit
            {
                Name = unitName,
                DisplayName = unitName,
                MeasureType = GetMeasureTypeByUnitName(unitName)
            };
            return Add(unit);
        }

        public static string UnitToString(int unitId)
        {
            var unit = Get(unitId);
            return unit != null ? unit.Name : string.Empty;
        }

        private static MeasureType? GetMeasureTypeByUnitName(string unitName)
        {
            if (unitName.IsNullOrEmpty()) return null;

            switch (unitName.ToLower())
            {
                case "шт":
                case "шт.":
                case "штука":
                case "штук":
                case "штуки":
                case "1 шт.":
                case "1шт":
                case "1 шт":
                case "шт.,":
                case " шт.":
                case "(шт)":
                case "ед.":
                case "рулон":
                case "рул":
                case "рул.":
                case "пара":
                case "пар":
                case "пар.":
                case "комплект":
                case "компл":
                case "компл.":
                case "комп.":
                case "комлпект":
                case "набор":
                case "наб":
                case "наб.":
                case "упак":
                case "упаковка":
                case "упак.":
                case "уп.":
                case "уп":
                case " упак.":
                    return MeasureType.Piece;
                
                case "г":
                case "г.":
                case "гр":
                case "гр.":
                case "грам":
                case "грамм":
                    return MeasureType.Gram;

                case "кг":
                case "кг.":
                case "килограмм":
                case "киллограмм":
                    return MeasureType.Kilogram;

                case "тонна":
                case "тонны":
                case "т":
                case "тн":
                    return MeasureType.Ton;

                case "сантиметр":
                case "сантиметры":
                case "см":
                case "см.":
                    return MeasureType.Centimetre;

                case "дециметр":
                case "дециметры":
                case "дм":
                case "дм.":
                    return MeasureType.Decimeter;

                case "метр":
                case "метры":
                case "м":
                case "м.":
                case "пог. м":
                case "пог.м":
                case "пог.м.":
                case "пог. м.":
                case "погонный метр":
                case "пог.метр":
                case "пог. метр":
                case "1 метр":
                    return MeasureType.Metre;

                case "квадратный сантиметр":
                case "квадратные сантиметры":
                case "кв. см":
                case "кв. см.":
                case "кв.см.":
                    return MeasureType.SquareCentimeter;

                case "квадратный дециметр":
                case "квадратные дециметры":
                case "кв. дм":
                case "кв. дм.":
                case "кв.дм.":
                    return MeasureType.SquareDecimeter;

                case "квадратный метр":
                case "квадратные метры":
                case "кв. м":
                case "кв. м.":
                case "кв.м":
                case "кв.м.":
                case "м.кв":
                case "м2":
                case "м²":
                    return MeasureType.SquareMeter;

                case "миллилитр":
                case "миллилитры":
                case "мл":
                case "мл.":
                    return MeasureType.Milliliter;

                case "литр":
                case "литры":
                case "л":
                case "л.":
                    return MeasureType.Liter;

                case "кубический метр":
                case "куб. м":
                case "куб. м.":
                case "куб.м.":
                case "м3":
                case "м³":
                    return MeasureType.CubicMeter;

                case "киловатт час":
                    return MeasureType.KilowattHour;

                case "гигакалория":
                case "гигакалории":
                case "гкал":
                case "гкал.":
                    return MeasureType.Gigacaloria;

                case "сутки":
                case "день":
                case "дни":
                    return MeasureType.Day;

                case "час":
                case "часы":
                    return MeasureType.Hour;

                case "минута":
                case "минуты":
                case "мин":
                case "мин.":
                    return MeasureType.Minute;

                case "секунда":
                case "секунды":
                case "сек":
                case "сек.":
                case "с":
                case "с.":
                    return MeasureType.Second;

            }

            return null;
        }
    }
}