using AdvantShop.Core.Services.Triggers;

namespace AdvantShop.Web.Admin.Models.Triggers
{
    public class TriggerEditModel
    {
        public int? Id { get; set; }
        public ETriggerEventType EventType { get; set; }

        public bool DisableAddition { get; set; }
    }
}
