using System.Collections.Generic;
using System.Linq;
using AdvantShop.Achievements;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetAchievementsHandler : ICommandHandler<AchievementsModel>
    {
        public AchievementsModel Execute()
        {
            var licKey = SettingsLic.LicKey;
            
            var achievementsInfo = AchievementsService.GetJsonAchievements(licKey);

            var bonuses = achievementsInfo
                .Select(achievementGroup => achievementGroup.Bonuses)
                .Sum();
            
            var model = new AchievementsModel
            {
                AllDone = SettingsCongratulationsDashboard.AllDone,
                FirstVisit = !SettingsCongratulationsDashboard.NotFirstVisit,
                Groups = new List<AchievementsGroup> {achievementsInfo[0]},
                Bonuses = bonuses
            };

            foreach (var achivement in model.Groups.SelectMany(group => group.Achievements))
            {
                if (!string.IsNullOrEmpty(achivement.ActionButtonLink) && !achivement.ActionButtonLink.StartsWith("http"))
                    achivement.ActionButtonLink = UrlService.GetAbsoluteLink(achivement.ActionButtonLink);
            }

            return model;
        }
    }
}
