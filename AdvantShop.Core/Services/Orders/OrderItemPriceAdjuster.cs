using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Orders
{
    public class OrderItemPriceAdjuster : IOrderItemPriceAdjuster
    {
        protected readonly Order _order;
        protected float? _acceptableDifference;
        protected float? _roundNumbers;
        protected bool _setRoundNumbers;
        protected bool? _noChangeAmount;
        protected bool? _noSeparate;
        protected Currency _currency;
        private bool _ceilingAmountToInteger;

        public OrderItemPriceAdjuster(Order order)
        {
            _order = order;
        }

        public IOrderItemPriceAdjuster AcceptableDifference(float acceptableDifference)
        {
            _acceptableDifference = acceptableDifference;
            return this;
        }

        public IOrderItemPriceAdjuster RoundNumbers(float? roundNumbers)
        {
            _roundNumbers = roundNumbers;
            _setRoundNumbers = true;
            return this;
        }

        public IOrderItemPriceAdjuster NoChangeAmount()
        {
            _noChangeAmount = true;
            return this;
        }

        public IOrderItemPriceAdjuster NoSeparate()
        {
            _noSeparate = true;
            return this;
        }

        public IOrderItemPriceAdjuster WithCurrency(Currency currency)
        {
            _currency = currency;
            return this;
        }

        public IOrderItemPriceAdjuster CeilingAmountToInteger()
        {
            _ceilingAmountToInteger = true;
            return this;
        }

        public IList<OrderItem> GetItems() => GetItems(out _);

        public IList<OrderItem> GetItems(out float difference)
        {
            IList<OrderItem> items = _order.OrderItems;
            var changedIds = new Dictionary<int, int>();
            
            /*
             * если нужно изменить позиции заказа перед расчетом
             * то клонируем, чтобы не влиять на заказ
             */
            if (_ceilingAmountToInteger
                || _currency != null)
                items = items
                       .Select(item => item.DeepClone())
                       .ToList();

            if (_ceilingAmountToInteger)
                items = items.CeilingAmountToInteger() as IList<OrderItem>;

            if (_currency != null)
                items = items.ConvertCurrency(_order.OrderCurrency, _currency) as IList<OrderItem>;
   
            var newCurrency = _currency ?? _order.OrderCurrency;

            items = ApplyDiscountToItems(
                items,
                item => item.IgnoreOrderDiscount is false,
                _order.GetOrderDiscountPrice().ConvertCurrency(_order.OrderCurrency, newCurrency));
            SetUniqueId(items, changedIds);// позиция может быть разбита на несколько, а ид нужен уникальный

            items = ApplyDiscountToItems(
                items,
                item => item.IsCouponApplied,
                _order.GetOrderCouponPrice().ConvertCurrency(_order.OrderCurrency, newCurrency));
            SetUniqueId(items, changedIds);// позиция может быть разбита на несколько, а ид нужен уникальный

            if (_order.BonusCost > 0f)
            {
                var shippingDiscountByBonus = _order.BonusCost - _order.OrderItems.Where(item => item.DoNotApplyOtherDiscounts is false).Sum(item => item.Price * item.Amount);
                var orderBonusCost = _order.BonusCost - (shippingDiscountByBonus > 0f ? shippingDiscountByBonus : 0f);
                items = ApplyDiscountToItems(
                    items,
                    item => item.DoNotApplyOtherDiscounts is false,
                    orderBonusCost.ConvertCurrency(_order.OrderCurrency, newCurrency));
                SetUniqueId(items, changedIds);// позиция может быть разбита на несколько, а ид нужен уникальный
            }

            var recalculate = CreateRecalculate(items);
            var sum = _order.Sum.ConvertCurrency(_order.OrderCurrency, newCurrency);
            var shippingCostWithDiscount = _order.ShippingCostWithDiscount.ConvertCurrency(_order.OrderCurrency, newCurrency);
            items = recalculate.ToSum(sum - shippingCostWithDiscount, out difference);

            ResetIdToPristine(items, changedIds);
            
            return items;
        }

        private IList<OrderItem> ApplyDiscountToItems(
            IList<OrderItem> orderItems,
            Func<OrderItem, bool> predicateDiscount,
            float discount)
        {
            var itemsForDiscount = orderItems.Where(predicateDiscount).ToList();

            if (itemsForDiscount.Count != 0
                && discount != 0f)
            {
                var recalculate = CreateRecalculate(itemsForDiscount);

                var recalculateItems = recalculate.ToSum(Math.Max(itemsForDiscount.Sum(x => x.Amount * x.Price) - discount, 0f).SimpleRoundPrice(_order.OrderCurrency));

                foreach (var orderItem in orderItems.Where(x => predicateDiscount(x) is false))
                    recalculateItems.Add(orderItem);

                return recalculateItems;
            }

            return orderItems;
        }

        private void SetUniqueId(IList<OrderItem> items, IDictionary<int, int> changedIds)
        {
            /*
             * Позиция может быть разбита на несколько, а ид нужен уникальный.
             * Потому-что RecalculateOrderItemsToSum ожидает во входных данных позиции с уникальными Id
             */
            
            var ids = new HashSet<int>();
            foreach (var orderItem in items)
            {
                if (ids.Contains(orderItem.OrderItemID))
                {
                    var newId = Guid.NewGuid().GetHashCode();
                    changedIds.Add(newId, orderItem.OrderItemID);
                    orderItem.OrderItemID = newId;
                }
                ids.Add(orderItem.OrderItemID);
            }
        }

        private void ResetIdToPristine(IList<OrderItem> items, IDictionary<int, int> changedIds)
        {
            foreach (var orderItem in items)
                if (changedIds.TryGetValue(orderItem.OrderItemID, out var pristineId))
                    orderItem.OrderItemID = pristineId;
        }

        private RecalculateOrderItemsToSum CreateRecalculate(IEnumerable<OrderItem> items)
        {
            var recalculate = new RecalculateOrderItemsToSum(items);
            if (_acceptableDifference.HasValue)
                recalculate.AcceptableDifference = _acceptableDifference.Value;
            if (_noChangeAmount.HasValue)
                recalculate.NoChangeAmount = _noChangeAmount.Value;
            if (_noSeparate.HasValue)
                recalculate.NotSeparate = _noSeparate.Value;
            if (_setRoundNumbers)
                recalculate.RoundNumbers = _roundNumbers;
            return recalculate;
        }
    }

    public interface IOrderItemPriceAdjuster
    {
        IOrderItemPriceAdjuster AcceptableDifference(float acceptableDifference);
        IOrderItemPriceAdjuster RoundNumbers(float? roundNumbers);
        IOrderItemPriceAdjuster NoChangeAmount();
        IOrderItemPriceAdjuster NoSeparate();
        IOrderItemPriceAdjuster WithCurrency(Currency currency);
        IOrderItemPriceAdjuster CeilingAmountToInteger();
        IList<OrderItem> GetItems();
        IList<OrderItem> GetItems(out float difference);
    }
}