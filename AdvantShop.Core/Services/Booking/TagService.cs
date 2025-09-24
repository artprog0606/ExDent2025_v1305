using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class TagService
    {
        public static Tag Get(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Booking].[ReservationResourceTag] WHERE Id = @Id", CommandType.Text,
                GetTagFromReader, new SqlParameter("@Id", id));
        }

        public static List<Tag> GetList()
        {
            return
                SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM [Booking].[ReservationResourceTag] ORDER BY SortOrder",
                    CommandType.Text, GetTagFromReader);
        }

        public static List<int> GetIds()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[ReservationResourceTag]",
                CommandType.Text, "Id");
        }

        private static Tag GetTagFromReader(SqlDataReader reader)
        {
            return new Tag
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static int Add(Tag tag)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                " INSERT INTO [Booking].[ReservationResourceTag] " +
                " ([Name], [SortOrder]) " +
                " VALUES (@Name, @SortOrder); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", tag.Name ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", tag.SortOrder));
        }

        public static void Update(Tag tag)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[ReservationResourceTag] SET [Name] = @Name, [SortOrder] = @SortOrder " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", tag.Id),
                new SqlParameter("@Name", tag.Name),
                new SqlParameter("@SortOrder", tag.SortOrder));
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[ReservationResourceTag] WHERE Id = @Id",
                CommandType.Text, new SqlParameter("@Id", id));
        }

        #region ReservationResourceTagsMap

        public static List<Tag> GetReservationResourceTags(int reservationResourceId)
        {
            return
                SQLDataAccess.ExecuteReadList(
                    @"SELECT t.* FROM [Booking].[ReservationResourceTag] as t
                        INNER JOIN [Booking].[ReservationResourceTagsMap] as rrt ON rrt.[ReservationResourceTagId] = t.[Id]
                    WHERE rrt.[ReservationResourceId] = @ReservationResourceId
                    Order by t.SortOrder",
                    CommandType.Text,
                    GetTagFromReader,
                    new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static void AddMapTag(int reservationResourceId, int tagId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT 1 FROM [Booking].[ReservationResourceTagsMap] WHERE [ReservationResourceId] = @ReservationResourceId AND [ReservationResourceTagId] = @ReservationResourceTagId)
                begin 
                    INSERT INTO [Booking].[ReservationResourceTagsMap] ([ReservationResourceId],[ReservationResourceTagId]) VALUES (@ReservationResourceId, @ReservationResourceTagId)
                end",
                CommandType.Text,
                new SqlParameter("@ReservationResourceId", reservationResourceId),
                new SqlParameter("@ReservationResourceTagId", tagId)
                );
        }

        public static void DeleteMapTag(int reservationResourceId, int tagId)
        {
            SQLDataAccess.ExecuteNonQuery(
                " DELETE FROM [Booking].[ReservationResourceTagsMap] " +
                " WHERE [ReservationResourceId] = @ReservationResourceId AND [ReservationResourceTagId] = @ReservationResourceTagId ",
                CommandType.Text,
                new SqlParameter("@ReservationResourceId", reservationResourceId),
                new SqlParameter("@ReservationResourceTagId", tagId)
                );
        }

        public static void DeleteMapTag(int reservationResourceId)
        {
            SQLDataAccess.ExecuteNonQuery(
                " DELETE FROM [Booking].[ReservationResourceTagsMap] " +
                " WHERE [ReservationResourceId] = @ReservationResourceId ",
                CommandType.Text,
                new SqlParameter("@ReservationResourceId", reservationResourceId)
                );
        }

        #endregion
    }
}
