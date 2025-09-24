using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Repository;

namespace AdvantShop.Core.Services.Tools.Recalculate
{
    public static class RecalculateResultItemsExtensions
    {
        public static IList<RecalculateResultItem<TId>> ToResultItems<TId>(this IList<RecalculateItem<TId>> items) 
            where TId : IEquatable<TId>
        {
            return items
                  .Select(x => new RecalculateResultItem<TId>(x))
                  .ToList();
        }

        public static IList<RecalculateResultItem<TId>> ResetPriceToZero<TId>(this IList<RecalculateResultItem<TId>> items) 
            where TId : IEquatable<TId>
        {
            items.ForEach(item => item.ResultPrice = 0);
            return items;
        }

        public static IList<RecalculateResultItem<TId>>  ResetToPristine<TId>(this IList<RecalculateResultItem<TId>> items) 
            where TId : IEquatable<TId>
        {
            items.ForEach(item => item.ResetToPristine());
            return items;
        }
 
        public static IList<RecalculateResultItem<TId>> CeilingQuantityToInteger<TId>(this IList<RecalculateResultItem<TId>> items) 
            where TId : IEquatable<TId>
        {
            foreach (var item in items.Where(x => x.ResultQuantity % 1 > 0))
            {
                var newQuantity = (float)Math.Ceiling(item.ResultQuantity);

                item.ConvertItemToNewQuantity(newQuantity);

                item.ResultQuantity = newQuantity;
            }
            return items;
        }
 
        public static IList<RecalculateResultItem<TId>> SetQuantityToOne<TId>(this IList<RecalculateResultItem<TId>> items) 
            where TId : IEquatable<TId>
        {
            foreach (var item in items)
            {
                var newQuantity = 1f;

                item.ConvertItemToNewQuantity(newQuantity);

                item.ResultQuantity = newQuantity;
            }
            return items;
        }
        
        public static void ConvertItemToNewQuantity<TId>(this RecalculateResultItem<TId> item, float newQuantity) 
            where TId : IEquatable<TId>
        {
            item.ResultPrice =
                (float)Math.Round(
                    MeasureHelper.ConvertUnitToNewAmount(oldUnit: item.ResultPrice, oldAmount: item.ResultQuantity, newAmount: newQuantity),
                    2);
        }
    }
}