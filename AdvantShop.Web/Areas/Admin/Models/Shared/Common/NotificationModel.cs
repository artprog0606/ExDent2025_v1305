using System.Collections.Generic;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Models.Shared.Common
{
    public class NotificationGroupModel
    {
        public NotificationGroupModel()
        {
            EventGroups = new List<LeadEventGroupModel>();
        }

        public List<LeadEventGroupModel> EventGroups { get; set; }
        public List<SelectItemModel> EventTypes { get; set; }
    }
}
