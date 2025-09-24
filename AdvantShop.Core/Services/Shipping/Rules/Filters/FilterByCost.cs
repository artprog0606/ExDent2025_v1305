using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    [DifficultyOfFilter(ECostFilter.Easy)]
    public class FilterByCost: BaseFilterBiLogic
    {
        protected readonly float Cost;
        protected readonly float RateCurrency;

        public FilterByCost(float cost, float rateCurrency, bool filterIsPositive): base(filterIsPositive)
        {
            Cost = cost;
            RateCurrency = rateCurrency;
        }

        public override bool Check(IObjectForRule obj)
        {
            if (obj.Currency is null)
                // не знаем в контексте каких валют необходимо сравнивать
                return false;

            var costByObjectCurrency = CurrencyService.ConvertCurrency(Cost, obj.Currency.Rate, RateCurrency);
            return
                FilterIsPositive
                    ? obj.Cost == costByObjectCurrency
                    : obj.Cost != costByObjectCurrency;
        }
    }
}