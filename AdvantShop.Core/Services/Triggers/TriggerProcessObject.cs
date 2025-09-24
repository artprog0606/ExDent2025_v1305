using System;

namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerProcessObject
    {
        /// <summary>
        /// Id of order, lead and etc
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Id of status..
        /// </summary>
        public int EventObjId { get; set; }

        public string Email { get; set; }

        public long Phone { get; set; }

        public Guid CustomerId { get; set; }
    }
}
