using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class WarehouseStocksService
    {
        public static void AddUpdateStocks(WarehouseStock warehouseStock, bool trackChanges = false, ChangedBy changedBy = null, bool calcHasProductsForWarehouse = true)
        {
            if (warehouseStock is null) throw new ArgumentNullException(nameof(warehouseStock));
            if (warehouseStock.OfferId <= 0) throw new ArgumentException("Invalid OfferId value (less or equal to 0)", nameof(warehouseStock.OfferId));
            if (warehouseStock.WarehouseId <= 0) throw new ArgumentException("Invalid WarehouseId value (less or equal to 0)", nameof(warehouseStock.WarehouseId));

            WarehouseStock oldStock = null;
            Offer oldOffer = null;
            if (trackChanges)
            {
                oldStock = Get(warehouseStock.OfferId, warehouseStock.WarehouseId);
                oldOffer = OfferService.GetOffer(warehouseStock.OfferId);
            }

            SQLDataAccess.ExecuteNonQuery(
                @"IF NOT EXISTS (SELECT * FROM [Catalog].[WarehouseStocks] WHERE [OfferId] = @OfferId AND [WarehouseId] = @WarehouseId)
                BEGIN
                    INSERT INTO [Catalog].[WarehouseStocks] ([OfferId], [WarehouseId], [Quantity]) Values (@OfferId, @WarehouseId, @Quantity)
                END
                ELSE
                BEGIN
                    UPDATE [Catalog].[WarehouseStocks] 
                        SET [Quantity] = @Quantity 
                    WHERE [OfferId] = @OfferId AND [WarehouseId] = @WarehouseId
                END",
                CommandType.Text,
                new SqlParameter("@OfferId", warehouseStock.OfferId),
                new SqlParameter("@WarehouseId", warehouseStock.WarehouseId),
                new SqlParameter("@Quantity", warehouseStock.Quantity));
            
            OfferService.RecalculateOfferAmount(warehouseStock.OfferId);
            
            if (calcHasProductsForWarehouse)
                CategoryService.CalculateHasProductsForWarehouseInProductCategories(warehouseStock.OfferId, warehouseStock.WarehouseId);

            if (trackChanges)
            {
                var warehouse = WarehouseService.Get(warehouseStock.WarehouseId);
                ProductHistoryService.TrackStockChanges(warehouseStock, oldStock, oldOffer, warehouse.Name, changedBy);
                ProductHistoryService.TrackOfferAmountChanges(
                    oldOffer.Amount + (warehouseStock.Quantity - (oldStock?.Quantity ?? 0f)),
                    oldOffer, 
                    changedBy);
            }
        }

        public static WarehouseStock Get(int offerId, int warehouseId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Catalog].[WarehouseStocks] WHERE [OfferId] = @OfferId AND [WarehouseId] = @WarehouseId",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@OfferId", offerId),
                new SqlParameter("@WarehouseId", warehouseId));

        }

        public static List<WarehouseStock> GetOfferStocks(int offerId)
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Catalog].[WarehouseStocks] WHERE [OfferId] = @OfferId",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@OfferId", offerId));
        }

        public static List<WarehouseStock> GetProductStocks(int productId)
        {
            return SQLDataAccess.ExecuteReadList(
                @"SELECT [WarehouseStocks].*
                FROM [Catalog].[WarehouseStocks] 
                    Inner Join [Catalog].[Offer] ON [Offer].[OfferID] = [WarehouseStocks].[OfferId] 
                WHERE Offer.ProductId = @ProductId",
                CommandType.Text, 
                GetFromReader, 
                new SqlParameter("@ProductId", productId));
        }
     
        public static WarehouseStock GetFromReader(SqlDataReader reader)
        {
            return new WarehouseStock
            {
                OfferId = SQLDataHelper.GetInt(reader, "OfferId"),
                WarehouseId = SQLDataHelper.GetInt(reader, "WarehouseId"),
                Quantity = SQLDataHelper.GetFloat(reader, "Quantity"),
            };
        }

        public static float SumStocksOfWarehouse(int warehouseId)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "SELECT ISNULL(SUM([Quantity]),0) FROM [Catalog].[WarehouseStocks] WHERE [WarehouseId] = @WarehouseId",
                CommandType.Text,
                new SqlParameter("@WarehouseId", warehouseId));
        }

        public static float GetStocksInWarehouses(int? productId, string offerArtNo, IList<int> warehouseIds)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                @"Select ISNULL(SUM([WarehouseStocks].[Quantity]),0) from [Catalog].[WarehouseStocks] 
                Inner Join [Catalog].[Offer] ON [Offer].[OfferID] = [WarehouseStocks].[OfferId] 
                where [WarehouseStocks].[WarehouseId] IN ("+ string.Join(",", warehouseIds) + @") 
                     AND (@ProductId IS NULL OR Offer.ProductId = @ProductId)
                     AND Offer.ArtNo = @ArtNo
                     AND [WarehouseStocks].[Quantity] > 0", // отфильтровываем отрицательные значения, чтобы они не "съедали" остатки по другим складам
                CommandType.Text,
                new SqlParameter("@ProductId", productId ?? (object) DBNull.Value),
                new SqlParameter("@ArtNo", offerArtNo ?? (object) DBNull.Value));
        }

        public static void DeleteZeroStocksByWarehouse(int warehouseId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"DELETE FROM [Catalog].[WarehouseStocks] 
                WHERE [WarehouseId] = @WarehouseId
                    AND [Quantity] = 0",
                CommandType.Text,
                new SqlParameter("@WarehouseId", warehouseId));
        }
    }
}