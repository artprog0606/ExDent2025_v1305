using System;

namespace AdvantShop.Core.Services.Partners
{
    public class PayoutReport
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime PeriodTo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
