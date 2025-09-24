using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    [DifficultyOfFilter(ECostFilter.Easy)]
    public class FilterByItemsTotalPrice: BaseFilterBiLogic
    {
        protected readonly float TotalPrice;
        protected readonly float RateCurrency;

        public FilterByItemsTotalPrice(float totalPrice, float rateCurrency, bool filterIsPositive): base(filterIsPositive)
        {
            TotalPrice = totalPrice;
            RateCurrency = rateCurrency;
        }

        public override bool Check(IObjectForRule obj)
        {
            if (obj.CalculationParameters?.Currency is null)
                // не знаем в контексте каких валют необходимо сравнивать
                return false;

            var totalPriceByCalculationParametersCurrency =
                CurrencyService.ConvertCurrency(TotalPrice, obj.CalculationParameters.Currency.Rate, RateCurrency);
            return 
                FilterIsPositive
                    ? obj.CalculationParameters.ItemsTotalPriceWithDiscounts == totalPriceByCalculationParametersCurrency
                    : obj.CalculationParameters.ItemsTotalPriceWithDiscounts != totalPriceByCalculationParametersCurrency;
        }
    }
}