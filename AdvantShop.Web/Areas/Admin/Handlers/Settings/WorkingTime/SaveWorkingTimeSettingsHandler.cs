using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shop;
using AdvantShop.Web.Admin.Models.Settings.CheckoutSettings;
using AdvantShop.Web.Infrastructure.Handlers;
using System;

namespace AdvantShop.Web.Admin.Handlers.Settings.WorkingTime
{
    public class SaveWorkingTimeSettingsHandler : AbstractCommandHandler
    {
        private readonly WorkingTimeSettings _model;

        public SaveWorkingTimeSettingsHandler(WorkingTimeSettings model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model == null)
                throw new BlException(T("Admin.WorkigTime.InvalidSettingsModel"));
            var timeZoneOffset = _model.TimeZoneOffset.TryParseTimeSpan();
            if (timeZoneOffset > TimeSpan.FromHours(14) || timeZoneOffset < TimeSpan.FromHours(-14))
                throw new BlException(T("Admin.WorkigTime.InvalidGMT"));
        }

        protected override void Handle()
        {
            ShopService.DeleteWorkingTimes();
            SettingsCheckout.TimeZoneWorkingTime = _model.TimeZoneOffset.TryParseTimeSpan();
            SettingsCheckout.NotAllowCheckoutText = _model.NotAllowCheckoutText;
            if (_model.WorkingTimes == null)
                return;

            foreach (var workingTime in _model.WorkingTimes)
                foreach (var interval in workingTime.Value)
                    ShopService.AddWorkingTime(new Core.Services.Shop.WorkingTime
                    {
                        DayOfWeek = (DayOfWeek)workingTime.Key,
                        StartTime = interval.TimeFrom.TryParseTimeSpan(),
                        EndTime = interval.TimeTo.TryParseTimeSpan(),
                    });
        }
    }
}
