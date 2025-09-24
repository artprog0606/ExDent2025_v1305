using System;
using System.Linq;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Admin.Models.Shared.Common;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Shared.Common
{
    public class GetInformers : AbstractCommandHandler<NotificationGroupModel>
    {
        protected override NotificationGroupModel Handle()
        {
            var events = AdminInformerService.GetList();

            var model = new NotificationGroupModel();
            var modelEvents =
                events.Select(LeadEventModel.GetLeadEventModel)
                    .Where(x => x != null)
                    .OrderByDescending(item => item.CreatedDate)
                    .ToList();

            foreach (var eventItem in modelEvents.OrderByDescending(x => x.CreatedDate))
            {
                var group = model.EventGroups.Find(x => x.CreatedDate.Year == eventItem.CreatedDate.Year &&
                                                        x.CreatedDate.Month == eventItem.CreatedDate.Month &&
                                                        x.CreatedDate.Day == eventItem.CreatedDate.Day);
                if (group == null)
                {
                    var date = new DateTime(eventItem.CreatedDate.Year, eventItem.CreatedDate.Month, eventItem.CreatedDate.Day);
                    var newGroup = new LeadEventGroupModel() { Title = GetTitleByDate(eventItem.CreatedDate), CreatedDate = date };
                    newGroup.Events.Add(eventItem);
                    model.EventGroups.Add(newGroup);
                }
                else
                {
                    group.Events.Add(eventItem);
                }
            }
            
            AdminInformerService.ClearOld();
            
            return model;
        }

        private string GetTitleByDate(DateTime date)
        {
            var now = DateTime.Now;

            if (date.Day == now.Day && date.Month == now.Month && date.Year == now.Year)
                return T("Admin.Today");

            if (date.Day == now.Day - 1 && date.Month == now.Month && date.Year == now.Year)
                return T("Admin.Yesteday");

            if (date.Year == now.Year)
                return date.ToString("dd MMMM");

            return date.ToString("D");
        }
    }
}
