using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Shipping.Rules
{
    public interface IObjectForRule : IContainerCalculationParameters
    {
        int Id { get; }
        string Type { get; }
        float? Cost { get; }
        Currency Currency { get; }
        bool Enabled { get; }

        void ChangeCost(float cost);
        void ChangeEnable(bool enable);
    }
}