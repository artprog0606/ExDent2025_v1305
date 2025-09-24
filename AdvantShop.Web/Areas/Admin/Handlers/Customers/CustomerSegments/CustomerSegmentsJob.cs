using AdvantShop.Core.Services.CustomerSegments;
using Quartz;

namespace AdvantShop.Web.Admin.Handlers.Customers.CustomerSegments
{
    [DisallowConcurrentExecution]
    public class CustomerSegmentsJob : IJob
    {
        private static readonly object Sync = new object();

        public void Execute(IJobExecutionContext context)
        {
            lock (Sync)
            {
                if (!Saas.SaasDataService.IsEnabledFeature(Saas.ESaasProperty.HaveCustomerSegmets))
                    return;

                var segments = CustomerSegmentService.GetList();
                if (segments.Count == 0)
                    return;

                foreach (var segment in segments)
                    new RecalcCustomerSegment(segment.Id).Execute();
            }
        }

    }
}
