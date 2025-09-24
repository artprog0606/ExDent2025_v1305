using AdvantShop.Core;
using AdvantShop.Core.Services.Shop;
using AdvantShop.Web.Admin.Models.Settings.CheckoutSettings;
using AdvantShop.Web.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Settings.WorkingTime
{
    public class GetAdditionalWorkingTimeSettingsHandler : AbstractCommandHandler<AdditionalWorkingTimeSettings>
    {
        private readonly DateTime? _date;
        private readonly DateTime? _dateFrom;
        private readonly DateTime? _dateTo;

        public GetAdditionalWorkingTimeSettingsHandler(DateTime? date, DateTime? dateFrom, DateTime? dateTo)
        {
            _date = date;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        protected override void Validate()
        {
            if (!_date.HasValue && !_dateFrom.HasValue && !_dateTo.HasValue)
                throw new BlException(T("Admin.WorkigTime.DateNotSpecified"));
        }

        protected override AdditionalWorkingTimeSettings Handle()
        {
            var additionalWorkingTimes = new List<AdditionalWorkingTime>();
            if (_date.HasValue)
                additionalWorkingTimes = ShopService.GetAdditionalWorkingTime(_date.Value);
            else if (_dateFrom.HasValue && _dateTo.HasValue)
                return new AdditionalWorkingTimeSettings
                {
                    WorkingTimes = new List<WorkingTimeInterval>(),
                    IsWork = false
                };
            var workingTimes = new List<WorkingTimeInterval>();
            if (additionalWorkingTimes.Count > 0)
                workingTimes.AddRange(additionalWorkingTimes.Where(x => x.IsWork)
                                                            .Select(workingTime => new WorkingTimeInterval(workingTime.StartTime.TimeOfDay, workingTime.EndTime.TimeOfDay)));
            else
                workingTimes.AddRange(ShopService.GetWorkingTime(_date.Value.DayOfWeek).Select(workingTime => new WorkingTimeInterval(workingTime.StartTime, workingTime.EndTime)));

            return new AdditionalWorkingTimeSettings
            {
                WorkingTimes = workingTimes,
                IsWork = workingTimes.Count > 0
            };
        }
    }
}