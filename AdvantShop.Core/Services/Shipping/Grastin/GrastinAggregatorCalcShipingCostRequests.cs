using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping.Grastin.Api;

namespace AdvantShop.Core.Services.Shipping.Grastin
{
    public class GrastinAggregatorCalcShipingCostRequests : IDisposable
    {
        private readonly GrastinApiService _grastinApiService;

        private ConcurrentDictionary<CalcShippingCostOrder,
            TaskCompletionSource<List<CostResponse>>> _pendingCosts;

        private bool disposed = false;

        public GrastinAggregatorCalcShipingCostRequests(GrastinApiService grastinApiService)
        {
            _grastinApiService = grastinApiService ?? throw new ArgumentNullException(nameof(grastinApiService));
            _pendingCosts = new ConcurrentDictionary<CalcShippingCostOrder,
                TaskCompletionSource<List<CostResponse>>>();
        }

        public Task<List<CostResponse>> CalcShippingCostAsync(CalcShippingCostOrder shippingCostOrder)
        {
            var tcs = new TaskCompletionSource<List<CostResponse>>();
            while (!_pendingCosts.TryAdd(shippingCostOrder, tcs)) { }

            return tcs.Task;
        }

        public void CalcAllRequests()
        {
            if (_pendingCosts.IsEmpty) // вызывает обширную блокировку, но это не должно помешать
                return;
            
            var calcOrders = new List<CalcShippingCostOrder>();
            var oldNumbers = new Dictionary<string, string>();
            foreach (var kv in _pendingCosts)
            {
                var newNumber = kv.Key.GetHashCode().ToString();
                oldNumbers.Add(newNumber, kv.Key.Number);
                kv.Key.Number = newNumber;
                
                calcOrders.Add(kv.Key);
            }

            var result = _grastinApiService.CalcShippingCost(new CalcShippingCostContainer() {Orders = calcOrders});
            if (result != null)
            {
                foreach (var groupResponse in result.GroupBy(rspns => rspns.Number))
                {
                    if (groupResponse.Key == null || !oldNumbers.ContainsKey(groupResponse.Key)) continue;

                    var shippingCostOrderHashCode = groupResponse.Key.TryParseInt();
                    var pending = 
                        _pendingCosts.SingleOrDefault(kv => kv.Key.GetHashCode() == shippingCostOrderHashCode);
                    if (pending.IsDefault()) continue;
                    
                    var shippingCostOrder = pending.Key;
                    var tcs = pending.Value;
                    
                    groupResponse.ForEach(rspns => rspns.Number = oldNumbers[groupResponse.Key]);

                    _pendingCosts.TryRemove(shippingCostOrder, out _);
                    tcs.SetResult(groupResponse.ToList());
                }
            }

            // Всем не обработанным заданиям присваиваем нулевой результат
            foreach (var kv in _pendingCosts)
                kv.Value.SetResult(null);
                
            _pendingCosts.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                foreach (var kv in _pendingCosts)
                    kv.Value.TrySetCanceled();
                
                _pendingCosts.Clear();
                _pendingCosts = null;
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}