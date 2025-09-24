namespace AdvantShop.Shipping
{
    public class DefaultWeightParams
    {
        public const string DefaultWeight = "DefaultWeight";
        public const string ExtrachargeTypeWeight = "ExtrachargeTypeWeight";
        public const string ExtrachargeWeight = "ExtrachargeWeight";
    }

    public class DefaultCargoParams : DefaultWeightParams
    {
        public const string DefaultLength = "DefaultLength";
        public const string DefaultWidth = "DefaultWidth";
        public const string DefaultHeight = "DefaultHeight";
        public const string ExtrachargeTypeCargo = "ExtrachargeTypeCargo";
        public const string ExtrachargeCargo = "ExtrachargeCargo";
    }
}