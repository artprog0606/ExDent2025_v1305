using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Web.Admin.Models.Modules
{
    public class ModulesModel
    {
#if DEBUG
        public bool ShowDevelopingButtons => true;
#else
        public bool ShowDevelopingButtons => false;
#endif

        public bool AllowDeleteAllModules
        {
            get
            {
#if DEBUG
                return true;
#endif
                return !(SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled) ||
                       CustomerContext.IsDebug;
            }
        }
    }
}
