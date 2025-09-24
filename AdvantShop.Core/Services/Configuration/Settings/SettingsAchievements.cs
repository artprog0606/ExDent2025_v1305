using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsAchievements
    {
        public static bool IsAchievementsEnabled
        {
            get => TrialService.IsTrialEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.ShowAchievements);
        }
    }
}