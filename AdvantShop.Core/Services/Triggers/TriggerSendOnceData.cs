using System;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerSendOnceData
    {
        public int TriggerId { get; set; }

        public int EntityId { get; set; }

        public Guid CustomerId { get; set; }
        
        public int? TriggerType { get; set; }
        
        public string CustomerMail { get; set; }
        
        public long? CustomerPhone { get; set; }
    }
}
