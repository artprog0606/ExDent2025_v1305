using System;

namespace AdvantShop.Core.Services.Orders
{
    public enum DeferredMailType
    {
        Order = 0,
        Lead = 1,
    }

    public class DeferredMail
    {
        public int EntityId { get; set; }

        public DeferredMailType EntityType { get; set; }

        public DateTime CreatedDate { get; set; }

        public DeferredMail() { }

        public DeferredMail(int entityId, DeferredMailType entityType)
        {
            EntityId = entityId;
            EntityType = entityType;
        }
    }
}
