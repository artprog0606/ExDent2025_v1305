using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.BizProcessRules
{
    public class BizProcessRulesFilterModel : BaseFilterModel<int>
    {
        public EBizProcessEventType EventType { get; set; }
    }
}
