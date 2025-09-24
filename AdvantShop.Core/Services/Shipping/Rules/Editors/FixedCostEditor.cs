using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    public class FixedCostEditor: IEditor
    {
        protected readonly float Cost;
        protected readonly float RateCurrency;

        public FixedCostEditor(float cost, float rateCurrency)
        {
            // Нельзя уводить стоимость в отрицательное значение
            Cost = cost >= 0f ? cost : 0f;
            RateCurrency = rateCurrency > 0f ? rateCurrency : 1f;
        }

        public void Change(IObjectForRule obj)
        {
            if (obj.Currency is null)
                // не знаем в контекста валют
                return;
            
            var costByObjectCurrency = CurrencyService.ConvertCurrency(Cost, obj.Currency.Rate, RateCurrency);
            obj.ChangeCost(costByObjectCurrency);
        }
    }
}