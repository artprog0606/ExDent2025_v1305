using AdvantShop.Configuration;
using AdvantShop.Core.Services.Shop;
using AdvantShop.Web.Admin.Models.Settings.CheckoutSettings;
using AdvantShop.Web.Infrastructure.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Settings.WorkingTime
{
    public class GetWorkingTimeSettingsHandler : AbstractCommandHandler<WorkingTimeSettings>
    {
        protected override WorkingTimeSettings Handle()
        {
            var workingTimes = ShopService.GetWorkingTimes();
            var schedule = new Dictionary<int, List<WorkingTimeInterval>>();
            foreach (var workingTime in workingTimes)
                if (schedule.ContainsKey((int)workingTime.DayOfWeek))
                    schedule[(int)workingTime.DayOfWeek].Add(new WorkingTimeInterval(workingTime.StartTime, workingTime.EndTime));
                else
                    schedule.Add((int)workingTime.DayOfWeek, new List<WorkingTimeInterval>
                    {
                        new WorkingTimeInterval(workingTime.StartTime, workingTime.EndTime)
                    });

            return new WorkingTimeSettings
            {
                WorkingTimes = schedule,
                TimeZoneOffset = SettingsCheckout.TimeZoneWorkingTime.ToString(),
                NotAllowCheckoutText = SettingsCheckout.NotAllowCheckoutText,
                AdditionalDate = ShopService.GetAdditionalWorkingTimes().Select(x => x.StartTime.Date.ToString("dMMyyyy")).ToList()
            };
        }
    }
}
