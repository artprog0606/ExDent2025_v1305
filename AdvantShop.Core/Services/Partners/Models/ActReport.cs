using System;

namespace AdvantShop.Core.Services.Partners
{
    public class ActReport
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public string FileName { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
