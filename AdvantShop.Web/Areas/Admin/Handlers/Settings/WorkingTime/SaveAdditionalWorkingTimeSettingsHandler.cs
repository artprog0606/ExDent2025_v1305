using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shop;
using AdvantShop.Web.Admin.Models.Settings.CheckoutSettings;
using AdvantShop.Web.Infrastructure.Handlers;
using System;

namespace AdvantShop.Web.Admin.Handlers.Settings.WorkingTime
{
    public class SaveAdditionalWorkingTimeSettingsHandler : AbstractCommandHandler
    {
        private readonly AdditionalWorkingTimeSettings _model;

        public SaveAdditionalWorkingTimeSettingsHandler(AdditionalWorkingTimeSettings model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model == null)
                throw new BlException(T("Admin.WorkigTime.InvalidSettingsModel"));
            if (_model.IsWork && (_model.WorkingTimes == null || _model.WorkingTimes.Count == 0))
                throw new BlException(T("Admin.WorkigTime.WorkingTimesNotSpecified"));
        }

        protected override void Handle()
        {
            ShopService.DeleteAdditionalWorkingTimes(_model.DateStart, _model.DateEnd.GetEndDay());
            var dateStart = _model.DateStart.Date;
            do
            {
                if (_model.IsWork && _model.WorkingTimes != null)
                    foreach (var workingTime in _model.WorkingTimes)
                    {
                        var timeTo = workingTime.TimeTo.TryParseTimeSpan();
                        ShopService.AddAdditionalWorkingTime(new AdditionalWorkingTime
                        {
                            IsWork = true,
                            StartTime = dateStart.Date.Add(workingTime.TimeFrom.TryParseTimeSpan()),
                            EndTime = (timeTo == TimeSpan.Zero ? dateStart.AddDays(1) : dateStart).Date.Add(timeTo),
                        });
                        if (timeTo == TimeSpan.Zero && timeTo == workingTime.TimeFrom.TryParseTimeSpan())
                            break;
                    }
                else
                    ShopService.AddAdditionalWorkingTime(new AdditionalWorkingTime
                    {
                        IsWork = false,
                        StartTime = dateStart.Date,
                        EndTime = dateStart.Date.AddDays(1),
                    });
                dateStart = dateStart.Date.AddDays(1);
            } while (dateStart <= _model.DateEnd);
        }
    }
}
