using AdvantShop.Core.Services.Shipping.FivePost.Helpers;

namespace AdvantShop.Shipping.FivePost.CalculateCost
{
    public class FivePostCalculationHandler
    {
        private readonly FivePostCalculationParams _params;

        public FivePostCalculationHandler(FivePostCalculationParams @params)
        {
            _params = @params;
        }

        public FivePostCalculationResult Execute()
        {
            var sum = FivePostHelper.Calculate(_params.Rate.ValueWithVat, _params.Rate.ExtraValueWithVat, _params.Weight);
            var sumWithInsure = _params.WithInsure 
                ? sum + _params.InsureValue * FivePostHelper.InsureCoef
                : sum;

            return new FivePostCalculationResult
            {
                DeliveryCost = _params.WithInsure ? sumWithInsure : sum,
                DeliveryCostWithInsure = sumWithInsure
            };
        }
    }
}
