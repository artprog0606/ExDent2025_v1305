using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking.Cart
{
    public class ShoppingCartService
    {
        private const string ShoppingCartContextKey = "BookingShoppingCartContext";

        public static ShoppingCart CurrentShoppingCart
        {
            get { return GetShoppingCart(); }
        }

        public static ShoppingCart GetShoppingCart()
        {
            if (HttpContext.Current != null)
            {
                var cachedCart = HttpContext.Current.Items[ShoppingCartContextKey] as ShoppingCart;
                if (cachedCart != null) return cachedCart;
            }

            var cart = GetShoppingCart(CustomerContext.CustomerId);

            if (cart != null && HttpContext.Current != null)
            {
                HttpContext.Current.Items[ShoppingCartContextKey] = cart;
            }

            return cart;
        }

        public static ShoppingCart GetShoppingCart(Guid customerId)
        {
            var templist =
                SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM Booking.ShoppingCart WHERE CustomerId = @CustomerId",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@CustomerId", customerId));

            var shoppingCart = new ShoppingCart(customerId);
            shoppingCart.AddRange(templist);
            return shoppingCart;
        }

        public static int AddShoppingCartItem(ShoppingCartItem item)
        {
            return AddShoppingCartItem(item, CustomerContext.CustomerId);
        }

        public static int AddShoppingCartItem(ShoppingCartItem item, Guid customerId)
        {
            var shoppingcartItemId = 0;
            item.CustomerId = customerId;

            var shoppingCartItem = GetExistsShoppingCartItem(customerId, item.AffiliateId, item.ReservationResourceId, item.BeginDate, item.EndDate);
            if (shoppingCartItem != null)
            {
                    shoppingCartItem.Services = item.Services;
                    UpdateShoppingCartItem(shoppingCartItem);
                    shoppingcartItemId = shoppingCartItem.ShoppingCartItemId;
            }
            else
            {
                InsertShoppingCartItem(item);
                shoppingcartItemId = item.ShoppingCartItemId;
            }

            if (HttpContext.Current != null)
                HttpContext.Current.Items[ShoppingCartContextKey] = null;

            return shoppingcartItemId;
        }

        public static ShoppingCartItem GetExistsShoppingCartItem(Guid customerId, int affiliateId, int? reservationResourceId, DateTime start, DateTime end)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT TOP 1 * FROM Booking.ShoppingCart WHERE [CustomerId] = @CustomerId  AND [AffiliateId] = @AffiliateId AND " +
                    (reservationResourceId.HasValue
                        ? "[ReservationResourceId] = @ReservationResourceId"
                        : "[ReservationResourceId] IS NULL") +
                    " AND [BeginDate] = @BeginDate AND EndDate = @EndDate ",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId ?? (object) DBNull.Value),
                    new SqlParameter("@BeginDate", start),
                    new SqlParameter("@EndDate", end));
        }

        private static void InsertShoppingCartItem(ShoppingCartItem item)
        {
            item.ShoppingCartItemId = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Booking.ShoppingCart ([CustomerId],[AffiliateId],[ReservationResourceId],[BeginDate],[EndDate],[CreatedOn],[UpdatedOn],[Services]) " +
                "VALUES (@CustomerId, @AffiliateId, @ReservationResourceId, @BeginDate, @EndDate, GetDate(), GetDate(), @Services); Select SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@AffiliateId", item.AffiliateId),
                new SqlParameter("@ReservationResourceId", item.ReservationResourceId ?? (object) DBNull.Value),
                new SqlParameter("@BeginDate", item.BeginDate),
                new SqlParameter("@EndDate", item.EndDate),
                new SqlParameter("@Services",
                    string.Join(",", item.Services.Select(x => x.ServiceId + "$" + x.Amount))));
        }

        public static void UpdateShoppingCartItem(ShoppingCartItem item, bool useModule = true)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Booking.ShoppingCart SET [CustomerId] = @CustomerId, [AffiliateId] = @AffiliateId, [ReservationResourceId] = @ReservationResourceId, [UpdatedOn] = GetDate(), " +
                "[BeginDate] = @BeginDate, [EndDate]=@EndDate, Services=@Services WHERE [ShoppingCartItemId] = @ShoppingCartItemId",
                CommandType.Text,
                new SqlParameter("@ShoppingCartItemId", item.ShoppingCartItemId),
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@AffiliateId", item.AffiliateId),
                new SqlParameter("@ReservationResourceId", item.ReservationResourceId ?? (object)DBNull.Value),
                new SqlParameter("@BeginDate", item.BeginDate),
                new SqlParameter("@EndDate", item.EndDate),
                new SqlParameter("@Services",
                    string.Join(",", item.Services.Select(x => x.ServiceId + "$" + x.Amount))));
        }

        public static void ClearShoppingCart()
        {
            ClearShoppingCart(CustomerContext.CustomerId);
        }

        public static void ClearShoppingCart(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Booking.ShoppingCart WHERE CustomerId = @CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId));

            if (HttpContext.Current != null)
                HttpContext.Current.Items[ShoppingCartContextKey] = null;
        }

        public static void DeleteExpiredShoppingCartItems(DateTime olderThan)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Booking.ShoppingCart WHERE UpdatedOn<@olderThan",
                CommandType.Text, new SqlParameter("@olderThan", olderThan));
        }

        public static void DeleteShoppingCartItem(ShoppingCartItem cartItem)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Booking.ShoppingCart WHERE ShoppingCartItemId = @ShoppingCartItemId", CommandType.Text,
                new SqlParameter("@ShoppingCartItemId", cartItem.ShoppingCartItemId));

            if (HttpContext.Current != null)
                HttpContext.Current.Items[ShoppingCartContextKey] = null;
        }

        public static void MergeShoppingCarts(Guid oldCustomerId, Guid currentCustomerId)
        {
            if (oldCustomerId == currentCustomerId)
                return;

            foreach (var item in GetShoppingCart(oldCustomerId))
            {
                AddShoppingCartItem(item, currentCustomerId);
            }
        }

        private static ShoppingCartItem GetFromReader(SqlDataReader reader)
        {
            return new ShoppingCartItem
            {
                ShoppingCartItemId = SQLDataHelper.GetInt(reader, "ShoppingCartItemId"),
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                AffiliateId = SQLDataHelper.GetInt(reader, "AffiliateId"),
                ReservationResourceId = SQLDataHelper.GetNullableInt(reader, "ReservationResourceId"),
                BeginDate = SQLDataHelper.GetDateTime(reader, "BeginDate"),
                EndDate = SQLDataHelper.GetDateTime(reader, "EndDate"),
                CreatedOn = SQLDataHelper.GetDateTime(reader, "CreatedOn"),
                UpdatedOn = SQLDataHelper.GetDateTime(reader, "UpdatedOn"),
                Services = SQLDataHelper.GetString(reader, "Services")
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x =>
                    {
                        var arr = x.Split('$');
                        return new ServiceItem {ServiceId = arr[0].TryParseInt(), Amount = arr[1].TryParseInt()};
                    }).ToList()
            };
        }
    }
}
