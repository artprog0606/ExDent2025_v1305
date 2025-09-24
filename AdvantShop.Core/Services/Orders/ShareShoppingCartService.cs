//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.Orders
{
    public static class ShareShoppingCartService
    {
        public static string AddShareShoppingCart(ShareShoppingCart cart) => AddShareShoppingCart(cart, CustomerContext.CustomerId);

        public static string AddShareShoppingCart(ShareShoppingCart cart, Guid customerId)
        {
            var carts = GetShoppingCarts(customerId);
            var existsCart = carts.FirstOrDefault(x => cart.Equals(x));
            if (existsCart != null)
                return existsCart.Key;
            if (carts.Count >= 10)
                foreach (var cartForDelete in carts.Skip(9))
                    DeleteShareShoppingCartItemByKey(cartForDelete.Key);

            var key = GenerateShareShoppingCartKey();
            foreach (var item in cart)
            {
                item.CustomerId = customerId;
                item.Key = key;
                InsertShareShoppingCartItem(item);
            }
            return key;
        }

        public static List<ShareShoppingCart> GetShoppingCarts(Guid customerId)
        {
            var items =
                SQLDataAccess.ExecuteReadList(
                    @"SELECT * FROM [Catalog].[ShareShoppingCart] 
                    WHERE [CustomerId] = @CustomerId
                    ORDER BY [DateCreated] DESC",
                    CommandType.Text,
                    reader => GetFromReader(reader),
                    new SqlParameter("@CustomerId", customerId));

            var shoppingCarts = new List<ShareShoppingCart>();
            foreach (var item in items)
            {
                var existsCart = shoppingCarts.FirstOrDefault(x => x.Key == item.Key);
                if (existsCart == null)
                {
                    existsCart = new ShareShoppingCart();
                    shoppingCarts.Add(existsCart);
                }
                existsCart.Add(item);
            }
            return shoppingCarts;
        }

        public static List<ShareShoppingCartItem> GetShareShoppingCartItems(string key)
        {
            return
                SQLDataAccess.ExecuteReadList(
                    @"SELECT * FROM [Catalog].[ShareShoppingCart] 
                    WHERE [Key] = @Key",
                    CommandType.Text,
                    reader => GetFromReader(reader),
                    new SqlParameter("@Key", key));
        }

        private static void InsertShareShoppingCartItem(ShareShoppingCartItem item)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Catalog.ShareShoppingCart ([Key], CustomerId, OfferId, AttributesXml, Amount, DateCreated) " +
                "VALUES (@Key, @CustomerId, @OfferId, @AttributesXml, @Amount, GetDate());",
                CommandType.Text,
                new SqlParameter("@Key", item.Key),
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@OfferId", item.OfferId),
                new SqlParameter("@AttributesXml", item.AttributesXml ?? (object)DBNull.Value),
                new SqlParameter("@Amount", item.Amount));
        }

        public static void DeleteExpiredShareShoppingCartItems(DateTime olderThan)
        {
            var expiredKeys = SQLDataAccess.ExecuteReadColumn<string>("SELECT [Key] FROM Catalog.ShareShoppingCart WHERE DateCreated<@olderThan GROUP BY [Key]",
                CommandType.Text, "[Key]", new SqlParameter("@olderThan", olderThan));
            foreach (var expiredKey in expiredKeys)
                DeleteShareShoppingCartItemByKey(expiredKey);
        }

        public static void DeleteShareShoppingCartItemByKey(string key)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ShareShoppingCart WHERE [Key] = @Key", 
                CommandType.Text, 
                new SqlParameter("@Key", key));
        }

        private static ShareShoppingCartItem GetFromReader(SqlDataReader reader, int? paymentId = null, int? shippingId = null)
        {
            return new ShareShoppingCartItem
            {
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                AttributesXml = SQLDataHelper.GetString(reader, "AttributesXml"),
                Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                DateCreated = SQLDataHelper.GetDateTime(reader, "DateCreated"),
                Key = SQLDataHelper.GetString(reader, "Key")
            };
        }

        public static bool IsExistsShareShoppingCartKey(string key)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "SELECT (CASE WHEN EXISTS(SELECT * FROM Catalog.ShareShoppingCart WHERE [Key]=@Key) THEN 1 ELSE 0 END)", 
                CommandType.Text,
                new SqlParameter("@Key", key));
        }

        public static string GenerateShareShoppingCartKey()
        {
            var key = Strings.GetRandomUrlString(8);
            while (IsExistsShareShoppingCartKey(key))
                key = Strings.GetRandomUrlString(8);
            return key;
        }
    }
}