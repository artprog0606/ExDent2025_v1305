using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Booking
{
    public class BookingItemsService
    {
        public static BookingItem Get(int id)
        {
            return
                SQLDataAccess.Query<BookingItem>("SELECT * FROM [Booking].[BookingItems] WHERE Id = @Id", new {Id = id})
                    .FirstOrDefault();
        }

        public static List<BookingItem> GetList(int bookingId)
        {
            return
                SQLDataAccess.Query<BookingItem>("SELECT * FROM [Booking].[BookingItems] WHERE BookingId = @BookingId",
                    new {BookingId = bookingId})
                    .ToList();
        }

        public static List<int> GetListId(int bookingId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    "SELECT [Id] FROM [Booking].[BookingItems] WHERE BookingId = @BookingId", CommandType.Text, "Id",
                    new SqlParameter("@BookingId", bookingId));
        }

        public static int Add(BookingItem bookingitem, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                BookingHistoryService.TrackBookingItemChanges(bookingitem.BookingId, bookingitem, null, changedBy);

            return SQLDataAccess.ExecuteScalar<int>(
                " INSERT INTO [Booking].[BookingItems] " +
                " ([BookingId], [ServiceId], [Name], [Price], [Amount], [ArtNo]) " +
                " VALUES (@BookingId, @ServiceId, @Name, @Price, @Amount, @ArtNo); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@BookingId", bookingitem.BookingId),
                new SqlParameter("@ServiceId", bookingitem.ServiceId ?? (object) DBNull.Value),
                new SqlParameter("@ArtNo", bookingitem.ArtNo ?? (object) DBNull.Value),
                new SqlParameter("@Name", bookingitem.Name ?? (object) DBNull.Value),
                new SqlParameter("@Price", bookingitem.Price),
                new SqlParameter("@Amount", bookingitem.Amount)
                );
        }

        public static void Update(BookingItem bookingitem, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                BookingHistoryService.TrackBookingItemChanges(bookingitem.BookingId, bookingitem, Get(bookingitem.Id), changedBy);

            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[BookingItems] SET [ServiceId] = @ServiceId, [Name] = @Name, [Price] = @Price, [Amount] = @Amount, [ArtNo] = @ArtNo " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", bookingitem.Id),
                new SqlParameter("@ServiceId", bookingitem.ServiceId ?? (object) DBNull.Value),
                new SqlParameter("@ArtNo", bookingitem.ArtNo ?? (object) DBNull.Value),
                new SqlParameter("@Name", bookingitem.Name ?? (object) DBNull.Value),
                new SqlParameter("@Price", bookingitem.Price),
                new SqlParameter("@Amount", bookingitem.Amount)
                );
        }

        public static void Delete(int id, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
            {
                var bookingitem = Get(id);
                if (bookingitem != null)
                    BookingHistoryService.TrackBookingItemChanges(bookingitem.BookingId, null, bookingitem, changedBy);
            }

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[BookingItems] WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static void ClearByBooking(int bookingId, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                foreach (var bookingitem in GetList(bookingId))
                    BookingHistoryService.TrackBookingItemChanges(bookingitem.BookingId, null, bookingitem, changedBy);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[BookingItems] WHERE BookingId = @BookingId", CommandType.Text,
                new SqlParameter("@BookingId", bookingId));
        }
    }
}
