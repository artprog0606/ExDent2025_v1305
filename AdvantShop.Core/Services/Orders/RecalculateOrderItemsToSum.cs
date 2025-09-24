using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Tools.Recalculate;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Orders
{
    public class RecalculateOrderItemsToSum
    {
        private readonly IList<OrderItem> _sourceItems;

        /// <summary>
        /// Допустимое отклонение
        /// </summary>
        public float AcceptableDifference { get; set; }

        /// <summary>
        /// Не менять кол-во у позиций
        /// </summary>
        public bool NoChangeAmount { get; set; }

        /// <summary>
        /// Не разделять позиции на части
        /// </summary>
        public bool NotSeparate { get; set; }

        /// <summary>
        /// Округление до какого значения
        /// </summary>
        public float? RoundNumbers { get; set; }

        public RecalculateOrderItemsToSum(IEnumerable<OrderItem> items)
        {
            AcceptableDifference = 0f;

            _sourceItems = items == null
                ? new List<OrderItem>()
                : items as IList<OrderItem> ?? items.ToList();
        }

        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="sum"/> отрицательная</exception>
        public IList<OrderItem> ToSum(float sum)
        {
            return ToSum(sum, out _);
        }

        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="sum"/> отрицательная</exception>
        public IList<OrderItem> ToSum(float sum, out float difference)
        {
            if (sum < 0f)
                throw new ArgumentOutOfRangeException(nameof(sum), sum,
                    "Нельзя приводить позиции заказа к отрицательной сумме");
            
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(
                _sourceItems
                ?.Select(x => new RecalculateItem<int>(x.OrderItemID, x.Price, x.Amount))
                ?? new List<RecalculateItem<int>>());
            recalculateItemsToSum.AcceptableDifference = AcceptableDifference;
            recalculateItemsToSum.NoChangeAmount = NoChangeAmount;
            recalculateItemsToSum.NotSeparate = NotSeparate;
            recalculateItemsToSum.RoundNumbers = RoundNumbers;

            var recalculateResultItems = recalculateItemsToSum.ToSum(sum, out difference);

            if (_sourceItems == null || _sourceItems.Count == 0)
            {
                difference = 0f;
                return _sourceItems;
            }

            if (recalculateResultItems.Any(x => x.ResultPrice < 0f))
                throw new Exception("Одна или более позиций заказа привелись к отрицательной цене");

            var resultItems = new List<OrderItem>();

            var resultQuantityById = recalculateResultItems
                                   .GroupBy(x => x.Id)
                                   .ToDictionary(x => x.Key, x => x.Sum(item => item.ResultQuantity));
            
            foreach (var recalculateResultItem in recalculateResultItems)
            {
                var sourceOrderItem = _sourceItems.Single(x => x.OrderItemID == recalculateResultItem.Id);
                var newOrderItem = sourceOrderItem.DeepClone();
                
                // позиции могут быть разбиты на несколько, поэтому смотрим общее кол-во по идентификатору
                var newAmount = resultQuantityById[sourceOrderItem.OrderItemID];
                var changeAmount = Math.Abs(sourceOrderItem.Amount - newAmount) > 0.0001;
                if (changeAmount)
                    newOrderItem.ConvertOrderItemToNewAmount(newAmount);
                    
                newOrderItem.Price = recalculateResultItem.ResultPrice;
                newOrderItem.Amount = recalculateResultItem.ResultQuantity;
                
                resultItems.Add(newOrderItem);
            }

            return resultItems;
        }
    }
}
