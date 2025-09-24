//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public class OfferService
    {
        public static int AddOffer(Offer offer, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (trackChanges)
                ProductHistoryService.NewOffer(offer, changedBy);

            offer.OfferId =
                SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_AddOffer]", CommandType.StoredProcedure,
                                          new SqlParameter("@ProductID", offer.ProductId),
                                          new SqlParameter("@ArtNo", offer.ArtNo),
                                          new SqlParameter("@Amount", (object)0f),
                                          new SqlParameter("@Price", offer.BasePrice),
                                          new SqlParameter("@SupplyPrice", offer.SupplyPrice),
                                          new SqlParameter("@ColorID", offer.ColorID ?? (object)DBNull.Value),
                                          new SqlParameter("@SizeID", offer.SizeID ?? (object)DBNull.Value),
                                          new SqlParameter("@Main", offer.Main),
                                          new SqlParameter("@Weight", offer.Weight ?? (object)DBNull.Value),
                                          new SqlParameter("@Length", offer.Length ?? (object)DBNull.Value),
                                          new SqlParameter("@Width", offer.Width ?? (object)DBNull.Value),
                                          new SqlParameter("@Height", offer.Height ?? (object)DBNull.Value),
                                          new SqlParameter("@BarCode", offer.BarCode ?? (object)DBNull.Value)
                                          );
            return offer.OfferId;
        }

        public static void UpdateOffer(Offer offer, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (trackChanges)
                ProductHistoryService.TrackOfferChanges(offer, changedBy);

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateOffer]", CommandType.StoredProcedure,
                                            new SqlParameter("@OfferId", offer.OfferId),
                                            new SqlParameter("@ProductID", offer.ProductId),
                                            new SqlParameter("@ArtNo", offer.ArtNo),
                                            // new SqlParameter("@Amount", offer.Amount),
                                            new SqlParameter("@Price", offer.BasePrice),
                                            new SqlParameter("@SupplyPrice", offer.SupplyPrice),
                                            new SqlParameter("@ColorID", offer.ColorID ?? (object)DBNull.Value),
                                            new SqlParameter("@SizeID", offer.SizeID ?? (object)DBNull.Value),
                                            new SqlParameter("@Main", offer.Main),
                                            new SqlParameter("@Weight", offer.Weight ?? (object)DBNull.Value),
                                            new SqlParameter("@Length", offer.Length ?? (object)DBNull.Value),
                                            new SqlParameter("@Width", offer.Width ?? (object)DBNull.Value),
                                            new SqlParameter("@Height", offer.Height ?? (object)DBNull.Value),
                                            new SqlParameter("@BarCode", offer.BarCode ?? (object)DBNull.Value)
                                            );
        }

        public static void DeleteOffer(int offerId, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (trackChanges)
                ProductHistoryService.DeleteOffer(offerId, changedBy);

            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_RemoveOffer]",
                                          CommandType.StoredProcedure,
                                          new SqlParameter("@OfferID", offerId));
        }


        public static void DeleteOldOffers(int productId, List<Offer> newOffers, bool trackChanges = false, ChangedBy changedBy = null)
        {
            if (newOffers == null || !newOffers.Any())
            {
                GetProductOffers(productId).ForEach(offer => DeleteOffer(offer.OfferId, trackChanges, changedBy));
            }
            else
            {
                var currentOffers = GetProductOffers(productId);
                currentOffers
                    .Where(offer => !newOffers.Any(x => x.OfferId == offer.OfferId))
                    .ForEach(offer => DeleteOffer(offer.OfferId, trackChanges, changedBy));
            }
        }

        public static void SetMainOfferForProductsWithoutMainOffer()
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[Offer]
                  SET [Main] = 1
                  WHERE NOT EXISTS(SELECT * FROM [Catalog].[Offer] AS OF2 WHERE OF2.[ProductID] = [Offer].[ProductId] AND OF2.[Main] = 1)
	                    AND [OfferID] IN (SELECT TOP (1) OF2.[OfferID] FROM [Catalog].[Offer] AS OF2 WHERE OF2.[ProductID] = [Offer].[ProductId])",
                CommandType.Text);
        }

        public static List<Offer> GetProductOffers(int productId)
        {
            return SQLDataAccess.ExecuteReadList<Offer>(
                     "SELECT Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE Offer.ProductID = @ProductID",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ProductID", productId));
        }

        public static int GetCountProductOffers(int productId)
        {
            return SQLDataAccess.ExecuteReadOne(
                     "SELECT COUNT(OfferID) AS Count FROM Catalog.Offer WHERE Offer.ProductID = @ProductID",
                     CommandType.Text,
                     (reader) => SQLDataHelper.GetInt(reader, "Count"),
                     new SqlParameter("@ProductID", productId));
        }

        public static Offer GetOffer(int offerId)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT TOP 1 Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE [offerID] = @offerID",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@offerID", offerId));
        }

        public static Offer GetOffer(string artNo)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT TOP 1 Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE Offer.ArtNo = @ArtNo",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ArtNo", artNo));
        }

        public static void RecalculateOfferAmount(int offerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[Offer]
                    SET [Amount] = ISNULL((SELECT SUM([Quantity]) FROM [Catalog].[WarehouseStocks] WHERE [WarehouseStocks].[OfferId] = [Offer].[OfferID]), 0)
                WHERE [OfferID] = @OfferID",
                CommandType.Text,
                new SqlParameter("@OfferID", offerId));
        }

        public static void RecalculateAmountAllOffer()
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Catalog].[Offer]
                    SET [Amount] = ISNULL((SELECT SUM([Quantity]) FROM [Catalog].[WarehouseStocks] WHERE [WarehouseStocks].[OfferId] = [Offer].[OfferID]), 0)",
                CommandType.Text,
                commandTimeout: 60*5);
        }

        public static void RecalculateAmountAllOffer(DateTime startAt, string createdBy)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE ofr
                    SET [Amount] = ISNULL((SELECT SUM([Quantity]) FROM [Catalog].[WarehouseStocks] WHERE [WarehouseStocks].[OfferId] = ofr.[OfferID]), 0) 
                    FROM [Catalog].[Offer] AS ofr 
                    INNER JOIN [Catalog].[Product] AS prd ON ofr.ProductId = prd.ProductId 
                    WHERE prd.DateModified < @startAt AND prd.CreatedBy = @createdBy",
                CommandType.Text,
                commandTimeout: 60*5,
                new SqlParameter("startAt", startAt),
                new SqlParameter("createdBy", createdBy));
        }

        public static Offer GetMainOffer(List<Offer> offers, bool allowPreOrder, int? colorId = null, int? sizeId = null, int? warehouseId = null)
        {
            if (offers == null || !offers.Any())
                return null;

            Offer currentOffer = null;

            // сначала доступные к покупке
            if (colorId.HasValue && sizeId.HasValue)
            {
                currentOffer = GetMainOffer(offers, o => o.ColorID == colorId && o.SizeID == sizeId, warehouseId);
            }
            else if (colorId.HasValue)
            {
                currentOffer = GetMainOffer(offers, o => o.ColorID == colorId, warehouseId);
            }
            else if (sizeId.HasValue)
            {
                currentOffer = GetMainOffer(offers, o => o.SizeID == sizeId, warehouseId);
            }

            if (currentOffer == null)
            {
                currentOffer = GetMainOffer(offers, o => IsAvailableOffer(o, false), warehouseId)
                            ?? GetMainOffer(offers, o => IsAvailableOffer(o, allowPreOrder), warehouseId); // сначала доступные к покупке, потом под заказ
            }

            return currentOffer ?? offers.FirstOrDefault();
        }

        private static Offer GetMainOffer(List<Offer> offers, Func<Offer, bool> condition, int? warehouseId)
        {
            if (warehouseId != null)
                return offers.OrderByDescending(o => o.Amount > 0 && o.BasePrice > 0)
                             .ThenByDescending(o => o.Main)
                             .FirstOrDefault(condition);
            
            return offers.OrderByDescending(o=> o.Amount > 0 && o.BasePrice > 0)
                         .ThenByDescending(o => o.Main)
                         .ThenByDescending(o => o.ColorID == offers.FirstOrDefault(x=>x.Main)?.ColorID)
                         .ThenBy(o => o.Color != null ? o.Color.SortOrder : 0)
                         .ThenBy(o => o.Color != null ? o.Color.ColorName : "")
                         .ThenBy(o => o.Size != null ? o.Size.SortOrder : 0)
                         .ThenBy(o => o.Size != null ? o.Size.SizeName : "")
                         .FirstOrDefault(condition);
        }

        public static Offer GetMainOfferForExport(int productId)
        {
            return SQLDataAccess.ExecuteReadOne<Offer>(
                     "SELECT top(1) Offer.*, CurrencyValue " +
                     "FROM Catalog.Offer " +
                     "Inner Join Catalog.Product on Offer.ProductID = Product.ProductID " +
                     "Inner Join Catalog.Currency on Currency.CurrencyID = Product.CurrencyID " +
                     "WHERE Offer.ProductID = @ProductID and Main=1",
                     CommandType.Text,
                     GetOfferFromReader,
                     new SqlParameter("@ProductID", productId));
        }

        private static bool IsAvailableOffer(Offer o, bool allowPreOrder)
        {
            return (o.RoundedPrice > 0 && o.Amount > 0) || o.Product.AllowBuyOutOfStockProducts(allowPreOrder); // || allowPreOrder
        }

        private static Offer GetOfferFromReader(SqlDataReader reader)
        {
            var offer = new Offer(amount: SQLDataHelper.GetFloat(reader, "Amount"))
            {
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                BasePrice = SQLDataHelper.GetFloat(reader, "Price"),
                SupplyPrice = SQLDataHelper.GetFloat(reader, "SupplyPrice"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                ColorID = SQLDataHelper.GetNullableInt(reader, "ColorID"),
                SizeID = SQLDataHelper.GetNullableInt(reader, "SizeID"),
                Main = SQLDataHelper.GetBoolean(reader, "Main"),
                Weight = SQLDataHelper.GetNullableFloat(reader, "Weight"),
                Length = SQLDataHelper.GetNullableFloat(reader, "Length"),
                Width = SQLDataHelper.GetNullableFloat(reader, "Width"),
                Height = SQLDataHelper.GetNullableFloat(reader, "Height"),
                BarCode = SQLDataHelper.GetString(reader, "BarCode"),
            };

            return offer;
        }

        public static bool IsArtNoExist(string artNo, int offerId)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Select Count(OfferID) from Catalog.Offer Where ArtNo=@ArtNo and OfferID<>@OfferID",
                    CommandType.Text, new SqlParameter("@ArtNo", artNo),
                    new SqlParameter("@offerID", offerId)
                    ) > 0;
        }

        public static string OffersToString(List<Offer> offers, string columSeparator, string propertySeparator)
        {
            return offers.OrderByDescending(o => o.Main)
                         .Select(offer =>
                                 offer.ArtNo + propertySeparator + (offer.Size != null ? offer.Size.SizeName : "null") + propertySeparator +
                                 (offer.Color != null ? offer.Color.ColorName : "null") + propertySeparator + offer.BasePrice +
                                 propertySeparator + offer.SupplyPrice + propertySeparator + offer.Amount)
                         .AggregateString(columSeparator);
        }

        public static void OffersFromString(Product product, string offersString, string columSeparator, string propertySeparator, bool importRemains, 
                                            out List<(Offer offer, float amount)> loadedAmounts, out List<int> newColorIds)
        {
            if (string.IsNullOrEmpty(columSeparator) || string.IsNullOrEmpty(propertySeparator))
                _OffersFromString(product, offersString, ";", ":", importRemains, out loadedAmounts, out newColorIds);
            else
                _OffersFromString(product, offersString, columSeparator, propertySeparator, importRemains, out loadedAmounts, out newColorIds);
        }

        private static void _OffersFromString(Product product, string offersString, string columSeparator, string propertySeparator, bool importRemains, 
                                                out List<(Offer offer, float amount)> loadedAmounts, out List<int> newColorIds)
        {
            loadedAmounts = new List<(Offer offer, float amount)>();
            newColorIds = null;
            
            product.HasMultiOffer = true;

            var oldOffers = new List<Offer>(product.Offers);
            product.Offers.Clear();

            var mainOffer = true;

            foreach (string[] fields in offersString.Replace("[", "").Replace("]", "")
                .Split(new[] { columSeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Split(new[] { propertySeparator }, StringSplitOptions.RemoveEmptyEntries)))
            {

                for (int i = 0; i < fields.Length; i++)
                {
                    fields[i] = fields[i].SupperTrim();
                }

                if (fields.Length == 6)
                {
                    var multiOffer = oldOffers.FirstOrDefault(offer => offer.ArtNo.Equals(fields[0], StringComparison.OrdinalIgnoreCase)) ?? new Offer();
                    multiOffer.ProductId = product.ProductId;
                    multiOffer.Main = mainOffer;

                    multiOffer.ArtNo = fields[0]; // ArtNo

                    if (fields[1] != "null") // Size
                    {
                        var size = SizeService.GetSize(fields[1]);
                        if (size == null)
                        {
                            size = new Size { SizeName = fields[1] };
                            size.SizeId = SizeService.AddSize(size);
                        }

                        multiOffer.SizeID = size.SizeId;
                    }
                    else
                    {
                        multiOffer.SizeID = null;
                    }

                    if (fields[2] != "null") // Color
                    {
                        var color = ColorService.GetColor(fields[2]);
                        if (color == null)
                        {
                            color = new Color { ColorName = fields[2], ColorCode = "#000000" };
                            color.ColorId = ColorService.AddColor(color);

                            if (newColorIds == null)
                                newColorIds = new List<int>() { color.ColorId };
                            else 
                                newColorIds.Add(color.ColorId);
                        }

                        multiOffer.ColorID = color.ColorId;
                    }
                    else
                    {
                        multiOffer.ColorID = null;
                    }

                    multiOffer.BasePrice = fields[3].TryParseFloat(); // Price
                    multiOffer.SupplyPrice = fields[4].TryParseFloat(); // SupplyPrice

                    if (importRemains)
                    {
                        var amount =
                            (multiOffer.Amount + fields[5].TryParseFloat()) > 0
                                ? multiOffer.Amount + fields[5].TryParseFloat()
                                : 0;
                        loadedAmounts.Add((multiOffer, amount));
                    }
                    else
                    {
                        loadedAmounts.Add((multiOffer, fields[5].TryParseFloat()));
                    }
                    //Amount

                    product.Offers.Add(multiOffer);
                    mainOffer = false;
                }
            }
        }

        public static void OfferFromFields(Product product, float? price, float? purchase, float? amount, bool importRemains, out (Offer offer, float amount) loadedAmount)
        {
            loadedAmount = default;
            if (price == null && purchase == null && amount == null)
                return;

            product.HasMultiOffer = false;

            var singleOffer = product.Offers.FirstOrDefault() ?? new Offer();
            product.Offers.Clear();

            singleOffer.ArtNo = product.ArtNo;
            singleOffer.Main = true;
            singleOffer.ProductId = product.ProductId;
            singleOffer.BasePrice = price ?? singleOffer.BasePrice;
            singleOffer.SupplyPrice = purchase ?? singleOffer.SupplyPrice;

            if (amount.HasValue)
            {
                if (importRemains)
                {
                    amount =
                        (singleOffer.Amount + amount.Value) > 0
                            ? singleOffer.Amount + amount.Value
                            : 0;
                }

                //проблема в том что некоторые добавляют 1 млрд и может не работать выгрузка в маркет  taskid 10451
                amount = amount > 1000000 ? 1000000 : amount;
                loadedAmount = (singleOffer, amount.Value);
            }
            product.Offers.Add(singleOffer);
        }
    }
}