using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Admin.Models.Booking.ReservationResources;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    public class AddUpdateAdditionalTimeHandler
    {
        private readonly AddUpdateAdditionalTimeModel _model;

        public bool UserConfirmIsRequired { get; set; }
        public string ConfirmMessage { get; set; }
        public string ConfirmButtomText { get; set; }
        public List<string> Errors { get; set; }

        public AddUpdateAdditionalTimeHandler(AddUpdateAdditionalTimeModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var reservationResource = ReservationResourceService.Get(_model.ReservationResourceId.Value);
            if (reservationResource == null)
                Errors.Add("Указанный ресурс не найден");

            if (reservationResource != null && !ReservationResourceService.CheckAccessToEditing(reservationResource, _model.AffiliateId.Value))
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            if (Errors.Count == 0)
            {
                var newTimes = ParseTimes(_model.AffiliateId.Value, reservationResource);

                if (!_model.UserConfirmed && !CheckTimeIsWork(newTimes, _model.AffiliateId.Value))
                {
                    Errors.Add("Указанное время пересекается с нерабочим или данный день является нерабочим для филиала");
                    return false;
                }

                var currentTimes = ReservationResourceAdditionalTimeService.GetByDate(_model.AffiliateId.Value, reservationResource.Id, _model.Date.Value);

                currentTimes.Where(
                    currentTime =>
                        !newTimes.Any(
                            newTime =>
                                newTime.IsWork == currentTime.IsWork && newTime.StartTime == currentTime.StartTime && newTime.EndTime == currentTime.EndTime))
                    .ForEach(x => ReservationResourceAdditionalTimeService.Delete(x.Id));

                newTimes.Where(
                    newTime =>
                        !currentTimes.Any(
                            currentTime =>
                                newTime.IsWork == currentTime.IsWork && newTime.StartTime == currentTime.StartTime && newTime.EndTime == currentTime.EndTime))
                    .ForEach(x => ReservationResourceAdditionalTimeService.Add(x));

                return true;
            }
            return false;
        }

        private bool CheckTimeIsWork(List<ReservationResourceAdditionalTime> newTimes, int affiliateId)
        {
            if (newTimes.Count > 0 && newTimes[0].IsWork)
            {
                var affiliateAdditionalTime = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                    AffiliateAdditionalTimeService.GetByAffiliateAndDate(affiliateId, _model.Date.Value)
                        .GroupBy(x => x.StartTime.Date)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    );
                var affiliateTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                    AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, _model.Date.Value.DayOfWeek)
                        .GroupBy(x => x.DayOfWeek)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    );

                return
                    AffiliateService.CheckDateRangeIsWork(
                        newTimes.Select(x => new Tuple<DateTime, DateTime>(x.StartTime, x.EndTime)).ToList(),
                        affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
            }
            return true;
        }

        private bool PredicateTimeOfBooking(AffiliateTimeOfBooking timeOfBooking, ReservationResourceAdditionalTime newTime)
        {
            var startTime = newTime.StartTime.TimeOfDay;
            var endTime = newTime.EndTime.TimeOfDay;

            if (endTime < startTime)
                endTime += new TimeSpan(1, 0, 0, 0);

            return startTime == timeOfBooking.StartTime && endTime == timeOfBooking.EndTime;
        }

        private List<ReservationResourceAdditionalTime> ParseTimes(int affiliateId, ReservationResource reservationResource)
        {
            string[] tempTimes;
            return _model.Times == null
                ? new List<ReservationResourceAdditionalTime>
                {
                    new ReservationResourceAdditionalTime()
                    {
                        AffiliateId = affiliateId,
                        ReservationResourceId = reservationResource.Id,
                        StartTime = _model.Date.Value.Date,
                        EndTime = _model.Date.Value.Date.AddDays(1),
                        IsWork = false
                    }
                }
                : _model.Times
                    .Select(
                        x =>
                            new ReservationResourceAdditionalTime
                            {
                                AffiliateId = affiliateId,
                                ReservationResourceId = reservationResource.Id,
                                StartTime = _model.Date.Value.Date +
                                    TimeSpan.ParseExact((tempTimes = x.Split('-'))[0], "hh\\:mm",
                                        CultureInfo.InvariantCulture),
                                EndTime = _model.Date.Value.Date +
                                    TimeSpan.ParseExact(tempTimes[1], "hh\\:mm", CultureInfo.InvariantCulture),
                                IsWork = true
                            })
                    .ToList();
        }
    }
}
