namespace AdvantShop.Areas.Api.Models.Bonuses
{
    public class SubstractBonusModel
    {
        public int BonusId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool SendSms { get; set; }
    }
}