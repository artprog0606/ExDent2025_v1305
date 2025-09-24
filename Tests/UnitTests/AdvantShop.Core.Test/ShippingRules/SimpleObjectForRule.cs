using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.Rules;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Test.ShippingRules
{
    public class SimpleObjectForRule: IObjectForRule
    {
        public SimpleObjectForRule()
        {
        }

        public SimpleObjectForRule(int id, string type, float? cost, Currency currency)
        {
            Id = id;
            Type = type;
            Cost = cost;
            Currency = currency;
        }
        
        public SimpleObjectForRule(int id, string type, float? cost, Currency currency, bool enabled,
            ShippingCalculationParameters calculationParameters) : this(id, type, cost, currency)
        {
            Enabled = enabled;
            CalculationParameters = calculationParameters;
        }

        public ShippingCalculationParameters CalculationParameters { get; }
        public int Id { get; }
        public string Type { get; }
        public float? Cost { get; private set; }
        public Currency Currency { get; }
        public bool Enabled { get; private set; }

        public void ChangeCost(float cost)
        {
            Cost = cost;
        }

        public void ChangeEnable(bool enable)
        {
            Enabled = enable;
        }
    }
}