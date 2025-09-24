using AdvantShop.Core.Services.Bonuses.Model.Enums;
using System;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    [Obsolete]
    public class AdditionalBonusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public EBonusStatus Status { get; set; }
    }
}