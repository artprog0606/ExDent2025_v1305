using System.Linq;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Admin.ViewModels.Emailings;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetTriggerAnalyticsViewHandler
    {
        private int _triggerId;

        public GetTriggerAnalyticsViewHandler(int triggerId)
        {
            _triggerId = triggerId;
        }

        public TriggerAnalyticsViewModel Execute()
        {
            var trigger = TriggerRuleService.GetTrigger(_triggerId);
            var actions = TriggerActionService.GetTriggerActions(_triggerId).Where(x => x.ActionType == ETriggerActionType.Email).ToList();
            if (trigger == null)
                return null;

            var model = new TriggerAnalyticsViewModel()
            {
                TriggerId = _triggerId,
                Name = trigger.Name,
                TriggerActions = actions
            };

            return model;
        }
    }
}
