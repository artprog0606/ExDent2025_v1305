using System.Threading;
using System.Threading.Tasks;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Screenshot;

namespace AdvantShop.Web.Admin.Handlers.Dashboard
{
    public class CreateDashboardScreenShots
    {
        public void Execute()
        {
            var service = new ScreenshotService();

            service.UpdateStoreScreenShot();

            var channel = SalesChannelService.GetByType(ESalesChannelType.Funnel);
            if (channel != null && channel.Enabled)
            {
                var funnels = new LpSiteService().GetList();

                Task.Run(() =>
                {
                    foreach (var funnel in funnels)
                    {
                        service.UpdateFunnelScreenShot(funnel);
                        Thread.Sleep(300);
                    }
                });
            }
        }
    }
}
