namespace AdvantShop.Core.Services.Payment.Robokassa
{
    public static class RobokassaExtention
    {
        public static float SubtractFee(this float price, float fee)
        {
            return fee != 0f ? (price * 100) / (100 + fee) : price;
        }
    }
}
