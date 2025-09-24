using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Orders
{
    public static class OrderItemsExtensions
    {
        public static IEnumerable<OrderItem> OnlyWithPrice(this IEnumerable<OrderItem> items)
            => items.Where(item => item.Price > 0);

        public static IEnumerable<OrderItem> ResetPriceToZero(this IEnumerable<OrderItem> items)
        {
            items.ForEach(item => item.Price = 0);
            return items;
        }

        public static IEnumerable<OrderItem> CeilingAmountToInteger(this IEnumerable<OrderItem> items)
        {
            foreach (var item in items.Where(x => x.Amount % 1 > 0))
            {
                var newAmount = (float)Math.Ceiling(item.Amount);

                item.ConvertOrderItemToNewAmount(newAmount);

                item.Amount = newAmount;
            }
            return items;
        }

        public static void ConvertOrderItemToNewAmount(this OrderItem item, float newAmount)
        {
            item.Price =
                (float)Math.Round(
                    MeasureHelper.ConvertUnitToNewAmount(oldUnit: item.Price, oldAmount: item.Amount, newAmount: newAmount),
                    2);

            item.Weight = 
                (float)Math.Round(
                    MeasureHelper.ConvertUnitToNewAmount(oldUnit: item.Weight, oldAmount: item.Amount, newAmount: newAmount),
                    3);

            var min = Math.Min(Math.Min(item.Length, item.Width), item.Height);
            if (item.Length == min)
                item.Length = (float)Math.Ceiling(MeasureHelper.ConvertUnitToNewAmount(oldUnit: item.Length, oldAmount: item.Amount, newAmount: newAmount));
            else if (item.Width == min)
                item.Width = (float)Math.Ceiling(MeasureHelper.ConvertUnitToNewAmount(oldUnit: item.Width, oldAmount: item.Amount, newAmount: newAmount));
            else if (item.Height == min)
                item.Height = (float)Math.Ceiling(MeasureHelper.ConvertUnitToNewAmount(oldUnit: item.Height, oldAmount: item.Amount, newAmount: newAmount));
        }

        public static IEnumerable<OrderItem> SetAmountToOne(this IEnumerable<OrderItem> items)
        {
            foreach (var item in items)
            {
                var newAmount = 1f;

                item.ConvertOrderItemToNewAmount(newAmount);

                item.Amount = newAmount;
            }
            return items;
        }

        public static IEnumerable<OrderItem> ConvertCurrency(this IEnumerable<OrderItem> items, Currency sourceCurrency, Currency newCurrency)
        {
            foreach (var item in items)
                item.Price = item.Price.ConvertCurrency(sourceCurrency, newCurrency);

            return items;
        }

        public static float GetPrice(this OrderItem orderItem)
        {
            return orderItem.IsCustomPrice 
                ? orderItem.Price 
                : orderItem.BasePrice ?? orderItem.Price;
        }
    }
}
