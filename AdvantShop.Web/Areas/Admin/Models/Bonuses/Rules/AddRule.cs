using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Model.Rules;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Models.Bonuses.Rules
{
    public class AddRule : AbstractCommandHandler<ERule>
    {
        private ERule _type;
        private CustomRule _rule;
        public AddRule(ERule ruleType)
        {
            _type = ruleType;
        }

        protected override void Load()
        {
            _rule = CustomRuleService.Get(_type);
        }

        protected override void Validate()
        {
            if (_rule != null)
                throw new BlException(T("Admin.Rules.AddRule.Error.RuleExist"));
        }

        protected override ERule Handle()
        {
            var b = new CustomRule { RuleType = _type, Name = _type.Localize()};
            b.Params = BaseRule.Set(BaseRule.Get(b));
            CustomRuleService.Add(b);
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Bonuses_AddBonusTrigger);
            return _type;
        }
    }
}
