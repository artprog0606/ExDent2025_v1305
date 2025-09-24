using System;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class DeferredMailsJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var deferredMails = DeferredMailService.GetListByDate(DateTime.Now.AddMinutes(-5));

            foreach (var mail in deferredMails)
            {
                if (mail.EntityType == DeferredMailType.Order)
                {
                    var order = OrderService.GetOrder(mail.EntityId);
                    if (order != null)
                    {
                        DeferredMailService.SendMailByOrder(order);
                        continue;
                    }
                }
                else if (mail.EntityType == DeferredMailType.Lead)
                {
                    var lead = LeadService.GetLead(mail.EntityId);
                    if (lead != null)
                    {
                        DeferredMailService.SendMailByLead(lead);
                        continue;
                    }
                }

                DeferredMailService.Delete(mail.EntityId, mail.EntityType);
            }

            context.WriteLastRun();
        }
    }
}
