namespace AdvantShop.Areas.Api.Models.Bonuses
{
    public class SubstractFreeAmountBonusModel
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool SendSms { get; set; }
    }
}