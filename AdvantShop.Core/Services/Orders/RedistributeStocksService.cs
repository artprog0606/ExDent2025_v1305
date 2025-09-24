using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Orders
{
    public class RedistributeStocksService
    {
        public static void Redistribute(Order order)
        {
            var defaultWarehouseId = SettingsCatalog.DefaultWarehouse;
            var warehouseIds = order.OrderPickPoint?.WarehouseIds;
            
            if ((warehouseIds is null || warehouseIds.Count is 0)
                && order.ShippingMethodId != 0)
            {
                warehouseIds = Shipping.ShippingWarehouseMappingService.GetByMethod(order.ShippingMethodId);
            }

            if ((warehouseIds is null || warehouseIds.Count is 0)
                && order.WarehouseIds.Count > 0)
            {
                warehouseIds = WarehouseService.GetListIds().Where(x => order.WarehouseIds.Contains(x)).ToList();
            }

            if (warehouseIds != null && warehouseIds.Count == 0)
                warehouseIds = null;

            warehouseIds = warehouseIds ?? WarehouseService.GetListIds(enabled: true);
            var currentDistributeOrder = DistributionOfOrderItemService.GetDistributionOfOrderItems(order.OrderID)
                                        .GroupBy(item => item.OrderItemId)
                                        .ToDictionary(group => group.Key, group => group.ToList());

            var cacheOffers = new Dictionary<string, Offer>();
            var cacheOfferStocks = new Dictionary<int, List<WarehouseStock>>();

            var orderItems =
                order.OrderItems
                      // вначале обрабатываем не измененные позиции, чтобы виртуально зарезервировать на них остатки
                     .OrderBy(
                          item => currentDistributeOrder.TryGetValue(item.OrderItemID, out var distributed)
                                  && Math.Abs(distributed.Sum(_ => _.Amount) - item.Amount) < 0.0001f // значение не изменилось
                              ? 0
                              : 1
                      )
                      // вначале обрабатываем позиции по которым нужно "вернуть" остатки на склад (чтобы было больше для "списывания")
                     .ThenBy(
                          item => currentDistributeOrder.TryGetValue(item.OrderItemID, out var distributed)
                                  && distributed.Sum(_ => _.Amount) > item.Amount
                              ? 0
                              : 1
                      );
            foreach (var orderItem in orderItems)
            {
                if (orderItem.TypeItem != TypeOrderItem.Product)
                    continue;
                
                // позиция еще не записана в БД
                // если у заказа флаг Decremented is true,
                // то при обработке добавления нескольких позиций в заказ,
                // после добавления каждой будет вызвано Redistribute,
                // но в orderItems будут еще не добавленные в БД записи (будут обработаны позже)
                if (orderItem.OrderItemID <= 0)
                    continue;

                currentDistributeOrder.TryGetValue(orderItem.OrderItemID, out var currentDistributionOfOrderItem);
                
                Offer offer;
                if (cacheOffers.ContainsKey(orderItem.ArtNo))
                    offer = cacheOffers[orderItem.ArtNo];
                else
                {
                    offer = OfferService.GetOffer(orderItem.ArtNo);
                    cacheOffers.Add(orderItem.ArtNo, offer);
                }

                if (offer != null
                    && !cacheOfferStocks.ContainsKey(offer.OfferId))
                    cacheOfferStocks.Add(offer.OfferId, WarehouseStocksService.GetOfferStocks(offer.OfferId));

                var offerStocks = offer != null
                    ? cacheOfferStocks[offer.OfferId]
                    : null;

                var amountDistributed = currentDistributionOfOrderItem?.Sum(_ => _.Amount) ?? 0f;
                if (amountDistributed < orderItem.Amount)
                    IncrementDistribution(orderItem, amountDistributed, warehouseIds, defaultWarehouseId, currentDistributionOfOrderItem, offer, offerStocks);
                else if (amountDistributed > orderItem.Amount)
                    DecrementDistribution(orderItem, amountDistributed, warehouseIds, defaultWarehouseId, currentDistributionOfOrderItem, offer, offerStocks);
                else
                {
                    if (orderItem.Amount < 0f
                        || currentDistributionOfOrderItem is null
                        || offerStocks is null)
                        continue;
    
                    // виртуально резервируем на позицию остатки
                    foreach (var warehouseId in warehouseIds)
                    {
                        var warehouseDistributed =
                            currentDistributionOfOrderItem.Find(item => item.WarehouseId == warehouseId);
                    
                        if (warehouseDistributed is null)
                            continue;    
                                       
                        var warehouseStock = offerStocks.Find(offerStock => offerStock.WarehouseId == warehouseId);
                        if (warehouseStock is null)
                            continue;

                        // виртуально резервируем на позицию остатки
                        warehouseStock.Quantity = warehouseStock.Quantity - warehouseDistributed.Amount + warehouseDistributed.DecrementedAmount;
                    }
                }

            }
        }

        private static void IncrementDistribution(OrderItem orderItem, float amountDistributed, List<int> warehouseIds,
            int defaultWarehouseId, List<DistributionOfOrderItem> currentDistributionOfOrderItem,
            Offer offer, List<WarehouseStock> offerStocks)
        {
            currentDistributionOfOrderItem = currentDistributionOfOrderItem ?? new List<DistributionOfOrderItem>();
            
            var amount = orderItem.Amount - amountDistributed;
            if (offer != null)
            {
                foreach (var warehouseId in warehouseIds)
                {
                    if ((amount > 0.0001f) is false)
                        break;

                    var warehouseStock = offerStocks.Find(offerStock => offerStock.WarehouseId == warehouseId);
                    if (warehouseStock is null
                        || !(warehouseStock.Quantity > 0f))
                        continue;
                    
                    var warehouseDistributed = currentDistributionOfOrderItem.Find(item => item.WarehouseId == warehouseId);
                    var availibleStock = warehouseStock.Quantity 
                                        + (warehouseDistributed?.DecrementedAmount ?? 0f)
                                        - (warehouseDistributed?.Amount ?? 0f);
                    if (availibleStock > 0f)
                    {
                        if (warehouseDistributed is null)
                            currentDistributionOfOrderItem.Add(
                                warehouseDistributed = new DistributionOfOrderItem
                                {
                                    OrderItemId = orderItem.OrderItemID,
                                    WarehouseId = warehouseId
                                });

                        warehouseDistributed.Amount += Math.Min(availibleStock, amount);
                        DistributionOfOrderItemService.AddOrUpdateDistributionOfOrderItem(warehouseDistributed);

                        // виртуально резервируем на позицию остатки
                        warehouseStock.Quantity = warehouseStock.Quantity - warehouseDistributed.Amount + warehouseDistributed.DecrementedAmount;
                        amount -= availibleStock;// намеренно availibleStock
                    }
                }
            }

            if (amount > 0.0001f)
            {
                var warehouseDistributed = currentDistributionOfOrderItem.Find(item => item.WarehouseId == defaultWarehouseId);
                if (warehouseDistributed is null)
                    currentDistributionOfOrderItem.Add(
                        warehouseDistributed = new DistributionOfOrderItem
                        {
                            OrderItemId = orderItem.OrderItemID,
                            WarehouseId = defaultWarehouseId
                        });
                warehouseDistributed.Amount += amount;
                DistributionOfOrderItemService.AddOrUpdateDistributionOfOrderItem(warehouseDistributed);
            
                var warehouseStock = offerStocks?.Find(offerStock => offerStock.WarehouseId == defaultWarehouseId);
                if (warehouseStock != null)
                    // виртуально резервируем на позицию остатки
                    warehouseStock.Quantity = warehouseStock.Quantity - warehouseDistributed.Amount + warehouseDistributed.DecrementedAmount;
            }
        }

        private static void DecrementDistribution(OrderItem orderItem, float amountDistributed, List<int> warehouseIds,
            int defaultWarehouseId, List<DistributionOfOrderItem> currentDistributionOfOrderItem,
            Offer offer, List<WarehouseStock> offerStocks)
        {
            if (orderItem.Amount < 0f)
                return;
            
            // уменьшать можно только от уже распределенного
            // если расределения нет, значит ошибочно сюда зашли
            if (currentDistributionOfOrderItem is null)
                return;
            
            var amount = amountDistributed - orderItem.Amount;
            if (offer != null)
            {
                // Уменьшаем в обратном порядке чем увеличиваем только
                // у складов с отрицательным остатком,
                // чтобы приводить их к нормальному остатку (не отрицательному)
                amount = DecrementDistributionOnlyWarehouseWithNegativeStocks(warehouseIds,
                    currentDistributionOfOrderItem, offerStocks, amount, orderItem.OrderItemID);
                
                // Если не все, уменьшаем у все складов
                if (amount > 0.0001f)
                    // Уменьшаем в обратном порядке чем увеличиваем
                    amount = DecrementDistributionDefaultAlgorithm(warehouseIds, currentDistributionOfOrderItem,
                        offerStocks, amount);
            }

            if (amount > 0.0001f)
            {
                // предполагается, что до этого случая должно редко доходить
                
                var warehouseDistributed =
                    currentDistributionOfOrderItem.Find(item => item.WarehouseId == defaultWarehouseId);
                    
                if (warehouseDistributed != null)
                {
                    var subtractAmount = Math.Min(warehouseDistributed.Amount, amount);
                    warehouseDistributed.Amount -= subtractAmount;
                       
                    // if (warehouseDistributed.Amount > 0.0001f)
                        DistributionOfOrderItemService.AddOrUpdateDistributionOfOrderItem(warehouseDistributed);
                    // else
                    // {
                    //     DeleteDistributionOfOrderItem(warehouseDistributed.OrderItemId, warehouseDistributed.WarehouseId);
                    //     currentDistributionOfOrderItem.Remove(warehouseDistributed);
                    // }
                    
                    var warehouseStock = offerStocks?.Find(offerStock => offerStock.WarehouseId == defaultWarehouseId);
                    if (warehouseStock != null)
                        // виртуально резервируем на позицию остатки
                        warehouseStock.Quantity = warehouseStock.Quantity - warehouseDistributed.Amount + warehouseDistributed.DecrementedAmount;
                }
            }
        }

        private static float DecrementDistributionDefaultAlgorithm(List<int> warehouseIds, List<DistributionOfOrderItem> currentDistributionOfOrderItem,
            List<WarehouseStock> offerStocks, float amount)
        {
            for (var index = warehouseIds.Count - 1; index >= 0; index--)
            {
                var warehouseId = warehouseIds[index];
                if ((amount > 0.0001f) is false)
                    break;

                var warehouseDistributed =
                    currentDistributionOfOrderItem.Find(item => item.WarehouseId == warehouseId);

                if (warehouseDistributed is null)
                    continue;

                var subtractAmount = Math.Min(warehouseDistributed.Amount, amount);
                warehouseDistributed.Amount -= subtractAmount;
                amount -= subtractAmount;

                // if (warehouseDistributed.Amount > 0.0001f)
                    DistributionOfOrderItemService.AddOrUpdateDistributionOfOrderItem(warehouseDistributed);
                // else
                // {
                //     DeleteDistributionOfOrderItem(warehouseDistributed.OrderItemId, warehouseDistributed.WarehouseId);
                //     currentDistributionOfOrderItem.Remove(warehouseDistributed);
                // }

                var warehouseStock = offerStocks.Find(offerStock => offerStock.WarehouseId == warehouseId);
                if (warehouseStock is null)
                    continue;

                // виртуально резервируем на позицию остатки
                warehouseStock.Quantity = warehouseStock.Quantity - warehouseDistributed.Amount
                                          + warehouseDistributed.DecrementedAmount;
            }

            return amount;
        }

        private static float DecrementDistributionOnlyWarehouseWithNegativeStocks(List<int> warehouseIds, 
            List<DistributionOfOrderItem> currentDistributionOfOrderItem, List<WarehouseStock> offerStocks, 
            float amount, int orderItemID)
        {
            for (var index = warehouseIds.Count - 1; index >= 0; index--)
            {
                var warehouseId = warehouseIds[index];
                if ((amount > 0.0001f) is false)
                    break;

                var warehouseStock = offerStocks.Find(offerStock => offerStock.WarehouseId == warehouseId);
                if (warehouseStock is null)
                    continue;
                    
                var warehouseDistributed =
                    currentDistributionOfOrderItem.Find(item => item.WarehouseId == warehouseId);
                var availibleStock = warehouseStock.Quantity 
                                     + (warehouseDistributed?.DecrementedAmount ?? 0f)
                                     - (warehouseDistributed?.Amount ?? 0f);
                
                if (availibleStock >= 0f)
                    continue;

                if (warehouseDistributed is null)
                    currentDistributionOfOrderItem.Add(
                        warehouseDistributed = new DistributionOfOrderItem
                        {
                            OrderItemId = orderItemID,
                            WarehouseId = warehouseId
                        });

                var subtractAmount = Math.Min(warehouseDistributed.Amount, amount);
                warehouseDistributed.Amount -= subtractAmount;
                amount -= subtractAmount;

                // if (warehouseDistributed.Amount > 0.0001f)
                    DistributionOfOrderItemService.AddOrUpdateDistributionOfOrderItem(warehouseDistributed);
                // else
                // {
                //     DeleteDistributionOfOrderItem(warehouseDistributed.OrderItemId, warehouseDistributed.WarehouseId);
                //     currentDistributionOfOrderItem.Remove(warehouseDistributed);
                // }

                // виртуально резервируем на позицию остатки
                warehouseStock.Quantity = warehouseStock.Quantity - warehouseDistributed.Amount
                                          + warehouseDistributed.DecrementedAmount;
            }

            return amount;
        }
    }
}