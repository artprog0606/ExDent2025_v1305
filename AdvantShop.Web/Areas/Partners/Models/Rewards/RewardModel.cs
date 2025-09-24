using System;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Areas.Partners.Models.Rewards
{
    public class RewardModel
    {
        public decimal RewardSum { get; set; }
        public string RewardSumFormatted { get { return RewardSum.FormatRoundPriceDefault(); } }
        public DateTime DateCreated { get; set; }
        public string Basis { get; set; }
    }
}