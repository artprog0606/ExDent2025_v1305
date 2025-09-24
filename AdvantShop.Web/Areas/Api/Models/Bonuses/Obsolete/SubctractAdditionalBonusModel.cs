using System;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    [Obsolete]
    public class SubctractAdditionalBonusModel
    {
        public int AdditionalBonusId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool SendSms { get; set; }
    }
}