using System.Linq;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    public class FilterByWeight: BaseFilterBiLogic
    {
        protected readonly float Weight;

        public FilterByWeight(float weight, bool filterIsPositive): base(filterIsPositive)
        {
            Weight = weight;
        }

        public override bool Check(IObjectForRule obj)
        {
            if (obj.CalculationParameters?.TotalWeight != null)
                return FilterIsPositive
                    ? obj.CalculationParameters.TotalWeight == Weight
                    : obj.CalculationParameters.TotalWeight != Weight;

            if (obj.CalculationParameters?.PreOrderItems != null
                && obj.CalculationParameters.PreOrderItems.Any())
                return FilterIsPositive
                    ? obj.CalculationParameters.PreOrderItems.Sum(item => item.Weight) == Weight
                    : obj.CalculationParameters.PreOrderItems.Sum(item => item.Weight) != Weight;

            // сравнивать не с чем
            return false;
        }
    }
}