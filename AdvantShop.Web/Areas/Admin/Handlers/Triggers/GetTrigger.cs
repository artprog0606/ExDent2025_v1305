using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Admin.Models.Triggers;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class GetTrigger
    {
        private readonly int _id;

        public GetTrigger(int id)
        {
            _id = id;
        }

        public TriggerModel Execute()
        {
            var trigger = TriggerRuleService.GetTrigger(_id);
            if (trigger == null)
                return null;

            var model = new TriggerModel()
            {
                Id = trigger.Id,
                Name = trigger.Name,
                CategoryId = trigger.CategoryId ?? 0,
                EventType = trigger.EventType,
                EventObjId = trigger.EventObjId,
                EventObjValue = trigger.EventObjValue,
                WorksOnlyOnce = trigger.WorksOnlyOnce,
                PreferredHour = trigger.PreferredHour,
                ProcessType = trigger.ProcessType.ToString().ToLower(),
                Actions = trigger.Actions,
                Filter = trigger.Filter,
                TriggerParams = trigger.TriggerParams,
                Coupon = trigger.Coupon
            };

            return model;
        }
    }
}
