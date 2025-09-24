using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class SaveTriggerName :ICommandHandler
    {
        private readonly int _id;
        private readonly string _name;

        public SaveTriggerName(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public void Execute()
        {
            TriggerRuleService.SetName(_id, _name);
        }
    }
}
