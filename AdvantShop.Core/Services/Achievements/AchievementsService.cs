using System.Collections.Generic;
using AdvantShop.Core.Services.Helpers;

namespace AdvantShop.Achievements
{
    public class AchievementsService
    {
        public static List<AchievementsGroup> GetJsonAchievements(string licKey)
        {
            return RequestHelper.MakeRequest<List<AchievementsGroup>>(
                "https://modules.advantshop.net/Achievements/GetLicenseAchievementSteps?licKey=" + licKey,
                method: ERequestMethod.GET);
        }
    }
}