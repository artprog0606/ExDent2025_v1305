using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Catalog
{
    public class ProductGiftService
    {
        public static List<GiftModel> GetGiftsByProductIdWithPrice(int productId, bool onlyAvailable = true)
        {
            return SQLDataAccess.Query<GiftModel>(
                @"SELECT o.OfferID, o.ProductID, o.ArtNo, o.Price, o.Amount, o.SupplyPrice, o.ColorID, o.SizeID, o.Main, CurrencyValue, pg.ProductCount, pg.OfferId as ProductOfferId 
                    FROM Catalog.Offer o 
                    Inner Join Catalog.ProductGifts pg on pg.GiftOfferId = o.OfferId 
                    Inner Join Catalog.Product on o.ProductID = Product.ProductID 
                    Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID 
                    WHERE pg.ProductId = @productId " +
                (onlyAvailable ? " and Product.Enabled = 1 and o.Amount > 0" : ""),
                new {productId}).ToList();
        }

        public static List<OfferGift> GetGifts(int productId)
        {
            return SQLDataAccess.Query<OfferGift>(
                @"SELECT pg.ProductId, pg.OfferId, pg.GiftOfferId, pg.ProductCount, o.ArtNo, Product.Name, o.ColorId, o.ProductId as GiftProductId  
                    FROM Catalog.Offer o 
                    Inner Join Catalog.ProductGifts pg on pg.GiftOfferId = o.OfferId 
                    Inner Join Catalog.Product on o.ProductID = Product.ProductID 
                    WHERE pg.ProductId = @productId  ",
                new { productId }).ToList();
        }
        
        public static List<OfferGift> GetGifts(int productId, int offerId, bool onlyAvailable = true)
        {
            return SQLDataAccess.Query<OfferGift>(
                @"SELECT pg.ProductId, pg.OfferId, pg.GiftOfferId, pg.ProductCount, o.ArtNo, p.Name, o.ColorId, o.ProductId as GiftProductId  
                    FROM Catalog.Offer o 
                    Inner Join Catalog.ProductGifts pg on pg.GiftOfferId = o.OfferId 
                    Inner Join Catalog.Product p on p.ProductID = o.ProductID 
                    WHERE pg.ProductId = @productId and (pg.OfferId is null or pg.OfferId = @offerId)  " +
                (onlyAvailable ? " and p.Enabled = 1 and o.Amount > 0" : ""),
                new { productId, offerId }).ToList();
        }
        
        public static List<int> GetGiftOfferIds(int productId, int offerId, bool onlyAvailable = true)
        {
            return SQLDataAccess.Query<int>(
                @"SELECT pg.GiftOfferId  
                    FROM Catalog.ProductGifts pg  
                    Inner Join Catalog.Offer o on o.OfferId = pg.GiftOfferId 
                    Inner Join Catalog.Product p on p.ProductID = o.ProductID  
                    WHERE pg.ProductId = @productId and (pg.OfferId is null or pg.OfferId = @offerId)" +
                (onlyAvailable ? " and p.Enabled = 1 and o.Amount > 0" : ""),
                new {productId, offerId}).ToList();
        }

        public static List<ProductGift> GetGiftsByGiftOfferId(int giftOfferId)
        {
            return SQLDataAccess.Query<ProductGift>(
                "SELECT * FROM Catalog.ProductGifts WHERE GiftOfferId = @giftOfferId",
                new {giftOfferId}).ToList();
        }
        
        #region CRUD

        public static bool IsExistProductGift(int productId, int giftOfferId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "if exists (Select 1 FROM Catalog.ProductGifts WHERE ProductId = @ProductId AND GiftOfferId = @GiftOfferId) " +
                            "Select 1 Else Select 0",
                CommandType.Text, 
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId)) == 1;
        }
        
        public static void AddGift(int productId, int giftOfferId, int? offerId, int productCount)
        {
            if (IsExistProductGift(productId, giftOfferId))
                return;

            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Catalog.ProductGifts (ProductId, GiftOfferId, ProductCount, OfferId) VALUES (@ProductId, @GiftOfferId, @ProductCount, @OfferId)",
                CommandType.Text,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId),
                new SqlParameter("@ProductCount", productCount),
                new SqlParameter("@OfferId", (object) offerId ?? DBNull.Value)
            );
        }
        
        public static void UpdateGift(int productId, int giftOfferId, int? offerId, int productCount)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Catalog.ProductGifts " +
                "SET ProductCount = @ProductCount " +
                "WHERE ProductId = @ProductId And GiftOfferId = @GiftOfferId And ((@OfferId is null And OfferId is null) or OfferId = @OfferId)",
                CommandType.Text,
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId),
                new SqlParameter("@ProductCount", productCount),
                new SqlParameter("@OfferId", (object)offerId ?? DBNull.Value));
        }

        public static void DeleteGift(int productId, int giftOfferId, int? offerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ProductGifts " +
                "WHERE ProductId = @ProductId And GiftOfferId = @GiftOfferId And (@OfferId is null or OfferId = @OfferId)",
                CommandType.Text, new SqlParameter("@ProductId", productId),
                new SqlParameter("@GiftOfferId", giftOfferId),
                new SqlParameter("@OfferId", (object)offerId ?? DBNull.Value));
        }

        public static void ClearGifts(int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ProductGifts WHERE ProductId = @ProductId",
                CommandType.Text, new SqlParameter("@ProductId", productId));
        }
        
        #endregion
        
        public static void RefreshGiftsInCart(ShoppingCart cart)
        {
            var giftsCartItems = cart.Where(x => x.IsGift && string.IsNullOrEmpty(x.ModuleKey)).ToList();
            
            foreach (var gift in giftsCartItems)
            {
                var gifts = GetGiftsByGiftOfferId(gift.OfferId);
                
                var giftItemGroups =
                    from item in cart
                    let giftProduct = gifts.Find(x => x.Equals(item.Offer.ProductId, gift.OfferId, item.OfferId))
                    where giftProduct != null && !item.IsGift
                    select new {item, giftProduct};

                var giftCount =
                    giftItemGroups
                        .GroupBy(x => x.item.Offer.ProductId)
                        .Select(x => new
                        {
                            ProductId = x.Key,
                            Amount = x.Sum(y => y.item.Amount),
                            GiftProductCount = x.FirstOrDefault()?.giftProduct.ProductCount
                        })
                        .Sum(x => x.Amount >= x.GiftProductCount
                            ? SettingsCheckout.MultiplyGiftsCount
                                ? (int) Math.Floor(x.Amount / (float) x.GiftProductCount)
                                : 1
                            : 0);

                // remove the gifts that have no products in cart
                if (giftCount == 0 || gift.Offer.Amount <= 0 || !gift.Offer.Product.Enabled)
                {
                    ShoppingCartService.DeleteShoppingCartItem(gift);
                    cart.Remove(gift);
                }
                else if (giftCount > 0 && gift.Amount != giftCount)
                {
                    gift.Amount = giftCount;
                    ShoppingCartService.UpdateShoppingCartItem(gift);
                }
            }
        }

        public static void AddGiftByOfferToCart(Offer offer)
        {
            var cart = ShoppingCartService.CurrentShoppingCart;

            foreach (var giftOfferId in GetGiftOfferIds(offer.ProductId, offer.OfferId))
            {
                var gifts = GetGiftsByGiftOfferId(giftOfferId);

                var giftItemGroups =
                    from item in cart
                    let giftProduct = gifts.Find(x => x.Equals(item.Offer.ProductId, giftOfferId, item.OfferId))
                    where giftProduct != null && !item.IsGift
                    select new {item, giftProduct};

                var giftCount =
                    giftItemGroups
                        .GroupBy(x => x.item.Offer.ProductId)
                        .Select(x => new
                        {
                            ProductId = x.Key,
                            Amount = x.Sum(y => y.item.Amount),
                            GiftProductCount = x.FirstOrDefault()?.giftProduct.ProductCount
                        })
                        .Sum(x => x.Amount >= x.GiftProductCount
                            ? SettingsCheckout.MultiplyGiftsCount
                                ? (int) Math.Floor(x.Amount / (float) x.GiftProductCount)
                                : 1
                            : 0);

                if (giftCount <= 0)
                    continue;

                var giftItem = cart.Find(x => x.OfferId == giftOfferId && x.IsGift);
                if (giftItem != null)
                {
                    giftItem.Amount = giftCount;
                    ShoppingCartService.UpdateShoppingCartItem(giftItem);
                }
                else
                {
                    ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem()
                    {
                        OfferId = giftOfferId,
                        Amount = giftCount,
                        IsGift = true
                    });
                }
            }
        }

        public static void UpdateGiftByOfferToCart(ShoppingCart cart, Offer offer, Dictionary<int, float> cartItemIdAndAmounts)
        {
            foreach (var giftOfferId in GetGiftOfferIds(offer.ProductId, offer.OfferId))
            {
                var gifts = GetGiftsByGiftOfferId(giftOfferId);

                var giftItemGroups =
                    from item in cart
                    let giftProduct = gifts.Find(x => x.Equals(item.Offer.ProductId, giftOfferId, item.OfferId))
                    where giftProduct != null && !item.IsGift
                    select new {item, giftProduct};

                var giftCount =
                    giftItemGroups
                        .GroupBy(x => x.item.Offer.ProductId)
                        .Select(x => new
                        {
                            ProductId = x.Key,
                            Amount = x.Sum(y =>
                                cartItemIdAndAmounts.TryGetValue(y.item.ShoppingCartItemId, out var amount)
                                    ? amount
                                    : y.item.Amount),
                            GiftProductCount = x.FirstOrDefault()?.giftProduct.ProductCount
                        })
                        .Sum(x => x.Amount >= x.GiftProductCount
                            ? SettingsCheckout.MultiplyGiftsCount
                                ? (int) Math.Floor(x.Amount / (float) x.GiftProductCount)
                                : 1
                            : 0);

                var giftItem = cart.Find(x => x.OfferId == giftOfferId && x.IsGift);
                if (giftItem != null)
                {
                    if (giftCount == 0)
                        ShoppingCartService.DeleteShoppingCartItem(giftItem);
                    else if (giftCount != giftItem.Amount)
                    {
                        giftItem.Amount = giftCount;
                        ShoppingCartService.UpdateShoppingCartItem(giftItem);
                    }
                }
                else if (giftCount > 0)
                {
                    ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem()
                    {
                        OfferId = giftOfferId,
                        Amount = giftCount,
                        IsGift = true
                    });
                }
            }
        }
        
        public static void RemoveGiftByOfferToCart(ShoppingCart cart, int cartItemIdToRemove, Offer offer)
        {
            foreach (var giftOfferId in GetGiftOfferIds(offer.ProductId, offer.OfferId))
            {
                var giftItem = cart.Find(x => x.OfferId == giftOfferId && x.IsGift);
                if (giftItem == null)
                    continue;
                
                var gifts = GetGiftsByGiftOfferId(giftOfferId);

                var giftItemGroups =
                    from item in cart
                    let giftProduct = gifts.Find(x => x.Equals(item.Offer.ProductId, giftOfferId, item.OfferId))
                    where giftProduct != null && !item.IsGift && item.ShoppingCartItemId != cartItemIdToRemove
                    select new {item, giftProduct};

                var giftCount =
                    giftItemGroups
                        .GroupBy(x => x.item.Offer.ProductId)
                        .Select(x => new
                        {
                            ProductId = x.Key,
                            Amount = x.Sum(y => y.item.Amount),
                            GiftProductCount = x.FirstOrDefault()?.giftProduct.ProductCount
                        })
                        .Sum(x => x.Amount >= x.GiftProductCount
                            ? SettingsCheckout.MultiplyGiftsCount
                                ? (int) Math.Floor(x.Amount / (float) x.GiftProductCount)
                                : 1
                            : 0);

                if (giftCount > 0 && giftItem.Amount != giftCount)
                {
                    giftItem.Amount = giftCount;
                    ShoppingCartService.UpdateShoppingCartItem(giftItem);
                }
                else if (giftCount <= 0)
                {
                    ShoppingCartService.DeleteShoppingCartItem(giftItem);
                }
            }
        }

        public static void AddGiftByOfferToOrder(Order order, Offer offer,
                                                Func<Product, Offer, float, float, string, bool, OrderItem> getOrderItem)
        {
            foreach (var giftOfferId in GetGiftOfferIds(offer.ProductId, offer.OfferId))
            {
                var gifts = GetGiftsByGiftOfferId(giftOfferId);

                var giftItemGroups =
                    from orderItem in order.OrderItems
                    let orderItemOffer = OfferService.GetOffer(orderItem.ArtNo)
                    let giftProduct = orderItemOffer != null
                        ? gifts.Find(x => x.Equals(orderItemOffer.ProductId, giftOfferId, orderItemOffer.OfferId))
                        : null
                    where orderItem.ProductID != null && giftProduct != null && !orderItem.IsGift
                    select new {orderItem, giftProduct};

                var giftCount =
                    giftItemGroups
                        .GroupBy(x => x.orderItem.ProductID)
                        .Select(x => new
                        {
                            ProductId = x.Key,
                            Amount = x.Sum(y => y.orderItem.Amount),
                            GiftProductCount = x.FirstOrDefault()?.giftProduct.ProductCount
                        })
                        .Sum(x => x.Amount >= x.GiftProductCount
                            ? SettingsCheckout.MultiplyGiftsCount
                                ? (int) Math.Floor(x.Amount / (float) x.GiftProductCount)
                                : 1
                            : 0);

                if (giftCount <= 0)
                    continue;

                var giftOffer = OfferService.GetOffer(giftOfferId);

                var giftItem = order.OrderItems.Find(x => x.ArtNo == giftOffer?.ArtNo && x.IsGift);
                if (giftItem != null)
                {
                    giftItem.Amount = giftCount;
                }
                else
                {
                    var giftOrderItem = getOrderItem(giftOffer.Product, giftOffer, 0, giftCount, null, true);

                    order.OrderItems.Add(giftOrderItem);
                }
            }
        }
        
        #region Convert to CSV

        public static bool GiftsFromString(int productId, string value, string columnSeparator)
        {
            ClearGifts(productId);

            if (string.IsNullOrEmpty(value))
                return true;

            var arrArt = value.Split(new[] { columnSeparator }, StringSplitOptions.None);
            foreach (var str in arrArt)
            {
                var artNo = str.Trim();
                
                if (string.IsNullOrWhiteSpace(artNo))
                    continue;
                
                var giOffer = OfferService.GetOffer(artNo);
                if (giOffer != null)
                    AddGift(productId, giOffer.OfferId, null, 1);
            }
            return true;
        }

        public static string GiftsToString(int productId, string columnSeparator)
        {
            var items = SQLDataAccess.ExecuteReadColumn<string>(
                "Select ArtNo from Catalog.Offer inner join Catalog.ProductGifts on ProductGifts.GiftOfferId = Offer.OfferId where ProductGifts.ProductId = @productId",
                CommandType.Text, "ArtNo", new SqlParameter("@productId", productId));
            return items.AggregateString(columnSeparator);
        }
        
        #endregion
    }
}