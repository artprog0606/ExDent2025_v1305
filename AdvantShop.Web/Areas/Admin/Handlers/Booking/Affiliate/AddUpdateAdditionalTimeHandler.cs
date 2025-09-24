using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Admin.Models.Booking.Affiliate;
using AdvantShop.Web.Admin.Handlers.Booking.ReservationResources;

namespace AdvantShop.Web.Admin.Handlers.Booking.Affiliate
{
    public class AddUpdateAdditionalTimeHandler
    {
        private readonly AddUpdateAdditionalTimeModel _model;

        public List<string> Errors { get; set; }

        public AddUpdateAdditionalTimeHandler(AddUpdateAdditionalTimeModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var affiliate = AffiliateService.Get(_model.AffiliateId.Value);

            if (affiliate == null)
            {
                Errors.Add("Филиал не найден");
                return false;
            }

            if (!AffiliateService.CheckAccessToEditing(affiliate))
            {
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                return false;
            }

            if (Errors.Count == 0)
            {
                if (_model.Date.HasValue)
                {
                    AddUpdate(affiliate, _model.Date.Value);

                    new CorrectingReservationResourcesAdditionalTimeHandler(affiliate.Id, _model.Date.Value, TypeCorrectingReservationResourcesTimes.All).Execute();
                }
                else //if (_model.StartDate.HasValue && _model.EndDate.HasValue)
                {
                    var currentDate = _model.StartDate.Value.Date;
                    do
                    {
                        AddUpdate(affiliate, currentDate);
                        currentDate = currentDate.AddDays(1);
                    } while (currentDate.Date <= _model.EndDate.Value.Date);

                    new CorrectingReservationResourcesAdditionalTimeHandler(affiliate.Id, _model.StartDate.Value, _model.EndDate.Value, TypeCorrectingReservationResourcesTimes.All).Execute();

                }

                return true;
            }
            return false;
        }

        private void AddUpdate(Core.Services.Booking.Affiliate affiliate, DateTime date)
        {
            var newTimes = ParseTimes(date);

            var currentTimes = AffiliateAdditionalTimeService.GetByAffiliateAndDate(affiliate.Id, date);

            currentTimes.Where(
                currentTime =>
                    !newTimes.Any(
                        newTime =>
                            newTime.IsWork == currentTime.IsWork && newTime.StartTime == currentTime.StartTime &&
                            newTime.EndTime == currentTime.EndTime))
                .ForEach(x => AffiliateAdditionalTimeService.Delete(x.Id));

            newTimes.Where(
                newTime =>
                    !currentTimes.Any(
                        currentTime =>
                            newTime.IsWork == currentTime.IsWork && newTime.StartTime == currentTime.StartTime &&
                            newTime.EndTime == currentTime.EndTime))
                .ForEach(x => AffiliateAdditionalTimeService.Add(x));
        }

        private List<AffiliateAdditionalTime> ParseTimes(DateTime date)
        {
            string[] tempTimes;
            return _model.Times == null
                ? new List<AffiliateAdditionalTime>
                {
                    new AffiliateAdditionalTime()
                    {
                        AffiliateId = _model.AffiliateId.Value,
                        StartTime = date.Date,
                        EndTime = date.Date.AddDays(1),
                        IsWork = false
                    }
                }
                : _model.Times
                    .Select(
                        x =>
                            new AffiliateAdditionalTime
                            {
                                AffiliateId = _model.AffiliateId.Value,
                                StartTime = date.Date +
                                    TimeSpan.ParseExact((tempTimes = x.Split('-'))[0], "hh\\:mm",
                                        CultureInfo.InvariantCulture),
                                EndTime = date.Date +
                                    TimeSpan.ParseExact(tempTimes[1], "hh\\:mm", CultureInfo.InvariantCulture),
                                IsWork = true
                            })
                    .ToList();
        }
    }
}
