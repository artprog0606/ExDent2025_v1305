using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Quartz;

namespace AdvantShop.Shipping
{
    [DisallowConcurrentExecution]
    public class OrderStatusSyncForShippingsJob : IJob
    {
        private static readonly object Sync = new object();

        public void Execute(IJobExecutionContext context)
        {
            lock (Sync)
            {
                try
                {
                    var dictionaryShipping = GetDictionarySupportedShipping();
                    var orders = GetOrders(dictionaryShipping);
                    var dictionarySyncByAllOrders = new Dictionary<BaseShipping, List<Order>>();

                    foreach (var order in orders)
                    {
                        if (order.OrderStatus != null && !order.OrderStatus.IsCanceled && !order.OrderStatus.IsCompleted)
                        {
                            try
                            {
                                var orderShipping =
                                    dictionaryShipping.Single(x => x.Key.ShippingMethodId == order.ShippingMethodId);

                                var iShipping = (IShippingSupportingSyncOfOrderStatus)orderShipping.Value;

                                if (iShipping.SyncByAllOrders)
                                {
                                    if (dictionarySyncByAllOrders.ContainsKey(orderShipping.Value))
                                        dictionarySyncByAllOrders[orderShipping.Value].Add(order);
                                    else
                                        dictionarySyncByAllOrders.Add(orderShipping.Value, new List<Order> { order });
                                }
                                else
                                    iShipping.SyncStatusOfOrder(order);
                            }
                            catch (Exception ex)
                            {
                                Debug.Log.Error(ex);
                            }
                        }
                    }

                    // Синхронизация статусов сразу всех заказов
                    foreach(var kvShippingOrdes in dictionarySyncByAllOrders)
                    {
                        try
                        {
                            var shippingOrdes = kvShippingOrdes.Value
                                .Where(order => order.OrderStatus != null && !order.OrderStatus.IsCanceled && !order.OrderStatus.IsCompleted)
                                .ToList();

                            ((IShippingSupportingSyncOfOrderStatus)kvShippingOrdes.Key).SyncStatusOfOrders(shippingOrdes);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Error(ex);
                        }
                    }
                }
                catch (BlException ex)
                {
                    Debug.Log.Error(ex);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        private List<Order> GetOrders(Dictionary<ShippingMethod, BaseShipping> dictionaryShipping)
        {
            return dictionaryShipping.Count > 0
                ? SQLDataAccess.ExecuteReadList<Order>(
                    "SELECT o.* FROM [Order].[Order] o" +
                    " inner join [Order].[OrderStatus] os ON o.OrderStatusID = os.[OrderStatusID]" +
                    " Where o.IsDraft <> 1 AND o.OrderDate >= @MinOrderDate" +
                    " AND os.[IsCanceled] = 0 AND os.[IsCompleted] = 0" +
                    " AND o.[ShippingMethodID] in (" +
                    string.Join(",", dictionaryShipping.Select(x => x.Key.ShippingMethodId)) + ")",
                    CommandType.Text,
                    OrderService.GetOrderFromReader,
                    new SqlParameter("@MinOrderDate", DateTime.Today.AddMonths(-3)))
                : new List<Order>();
        }

        private static Dictionary<ShippingMethod, BaseShipping> GetDictionarySupportedShipping()
        {
            var supportedTypes = ReflectionExt.GetTypesWith<ShippingKeyAttribute>(false)
                .Where(x => x.GetInterfaces().Contains(typeof(IShippingSupportingSyncOfOrderStatus)) && x.IsSubclassOf(typeof(BaseShipping)))
                .Select(AttributeHelper.GetAttributeValue<ShippingKeyAttribute, string>)
                .ToList();

            return ShippingMethodService.GetAllShippingMethods()
                .Where(method => method.Enabled && supportedTypes.Contains(method.ShippingType))
                .ToDictionary(method => method, method =>
                {
                    var type = ReflectionExt.GetTypeByAttributeValue<ShippingKeyAttribute>(typeof (BaseShipping), atr => atr.Value, method.ShippingType);
                    return (BaseShipping) Activator.CreateInstance(type, method, null);
                })
                .Where(x => ((IShippingSupportingSyncOfOrderStatus)x.Value).StatusesSync)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
