using System;

namespace AdvantShop.Core.Services.Triggers.DeferredDatas
{
    /// <summary>
    /// Отложенные данные для триггера
    /// </summary>
    public class TriggerDeferredData
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int TriggerActionId { get; set; }
        public ETriggerObjectType TriggerObjectType { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
