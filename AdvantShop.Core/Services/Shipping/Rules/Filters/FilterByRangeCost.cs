using System;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    [DifficultyOfFilter(ECostFilter.Easy)]
    [Obsolete("Рассмотреть общий фильтр по полю Cost, либо группировать фильтры по специальному атрибуту", true)]
    public class FilterByRangeCost: BaseFilterBiLogic
    {
        protected readonly float? From;
        protected readonly float? To;

        public FilterByRangeCost(float? from, float? to, bool filterIsPositive): base(filterIsPositive)
        {
            From = from;
            To = to;
        }

        public override bool Check(IObjectForRule obj)
        {
            if (From is null
                && To is null)
                return false;

            var result = (From is null || obj.Cost >= From)
                   && (To is null || obj.Cost <= To);
            
            return FilterIsPositive ? result : !result;
        }
    }
}