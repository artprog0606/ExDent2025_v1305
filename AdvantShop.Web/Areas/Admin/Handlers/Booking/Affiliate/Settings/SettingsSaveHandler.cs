using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Handlers.Booking.ReservationResources;
using AdvantShop.Web.Admin.ViewModels.Booking.Settings;

namespace AdvantShop.Web.Admin.Handlers.Booking.Affiliate.Settings
{
    public class SettingsSaveHandler
    {
        private readonly SettingsModel _model;

        public List<string> Errors { get; set; }

        public SettingsSaveHandler(SettingsModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var affiliate = AffiliateService.Get(_model.Affiliate.Id);
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

            try
            {
                var updateBookingInterval = affiliate.BookingIntervalMinutes !=
                                            _model.Affiliate.BookingIntervalMinutes;

                affiliate.Name = _model.Affiliate.Name.DefaultOrEmpty();
                affiliate.Description = _model.Affiliate.Description;
                affiliate.Address = _model.Affiliate.Address;
                affiliate.Phone = _model.Affiliate.Phone;
                affiliate.SortOrder = _model.Affiliate.SortOrder;
                affiliate.Enabled = _model.Affiliate.Enabled;
                affiliate.BookingIntervalMinutes = _model.Affiliate.BookingIntervalMinutes;

                affiliate.IsActiveSmsNotification = _model.Affiliate.IsActiveSmsNotification;
                affiliate.ForHowManyMinutesToSendSms = _model.Affiliate.ForHowManyMinutesToSendSms;
                affiliate.SmsTemplateBeforeStartBooiking = _model.Affiliate.SmsTemplateBeforeStartBooiking;

                affiliate.CancelUnpaidViaMinutes = _model.Affiliate.CancelUnpaidViaMinutes;

                if (CustomerContext.CurrentCustomer.IsAdmin)
                {
                    affiliate.AccessForAll = _model.Affiliate.AccessForAll;
                    affiliate.ManagerIds = _model.Affiliate.ManagerIds ?? new List<int>();
                    affiliate.AnalyticsAccessForAll = _model.Affiliate.AnalyticsAccessForAll;
                    affiliate.AnalyticManagerIds = _model.Affiliate.AnalyticManagerIds ?? new List<int>();
                    affiliate.AccessToViewBookingForResourceManagers = _model.Affiliate.AccessToViewBookingForResourceManagers;
                }

                AffiliateService.Update(affiliate);

                var affiliateTimes = AffiliateTimeOfBookingService.GetByAffiliate(affiliate.Id);
                var wasDeleting = false;

                var newTimes = ParseTimes(_model.MondayTimes, DayOfWeek.Monday, affiliate);
                var current = affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Monday).ToList();
                wasDeleting |= SaveAffiliateTimes(current, newTimes);

                newTimes = ParseTimes(_model.TuesdayTimes, DayOfWeek.Tuesday, affiliate);
                current = affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Tuesday).ToList();
                wasDeleting |= SaveAffiliateTimes(current, newTimes);

                newTimes = ParseTimes(_model.WednesdayTimes, DayOfWeek.Wednesday, affiliate);
                current = affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Wednesday).ToList();
                wasDeleting |= SaveAffiliateTimes(current, newTimes);

                newTimes = ParseTimes(_model.ThursdayTimes, DayOfWeek.Thursday, affiliate);
                current = affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Thursday).ToList();
                wasDeleting |= SaveAffiliateTimes(current, newTimes);

                newTimes = ParseTimes(_model.FridayTimes, DayOfWeek.Friday, affiliate);
                current = affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Friday).ToList();
                wasDeleting |= SaveAffiliateTimes(current, newTimes);

                newTimes = ParseTimes(_model.SaturdayTimes, DayOfWeek.Saturday, affiliate);
                current = affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Saturday).ToList();
                wasDeleting |= SaveAffiliateTimes(current, newTimes);

                newTimes = ParseTimes(_model.SundayTimes, DayOfWeek.Sunday, affiliate);
                current = affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Sunday).ToList();
                wasDeleting |= SaveAffiliateTimes(current, newTimes);

                if (updateBookingInterval)
                {
                    UpdateBookingTimeReservationResources(affiliate);
                    UpdateAdditionalTime(affiliate);

                    new CorrectingReservationResourcesTimesHandler(affiliate.Id,
                        TypeCorrectingReservationResourcesTimes.WithIndividualInterval).Execute();
                }
                else if (wasDeleting)
                {
                    // если было удаление времени в расписании, 
                    // значит нужно удалить время у пользователей, 
                    // которое теперь считается не рабочим
                    new CorrectingReservationResourcesTimesHandler(affiliate.Id).Execute();
                }

                return true;
            }
            catch (BlException ex)
            {
                Errors.Add(ex.Message);
            }
            return false;
        }

        private bool SaveAffiliateTimes(List<AffiliateTimeOfBooking> current, List<AffiliateTimeOfBooking> newTimes)
        {
            var wasDeleting = false;
            current.Where(
                currentTime =>
                    !newTimes.Any(
                        newTime => newTime.StartTime == currentTime.StartTime && newTime.EndTime == currentTime.EndTime))
                .ForEach(x =>
                {
                    wasDeleting = true;
                    AffiliateTimeOfBookingService.Delete(x.Id);
                });

            newTimes.Where(
                newTime =>
                    !current.Any(
                        currentTime => newTime.StartTime == currentTime.StartTime && newTime.EndTime == currentTime.EndTime))
                .ForEach(x => AffiliateTimeOfBookingService.Add(x));

            return wasDeleting;
        }

        private void UpdateAdditionalTime(Core.Services.Booking.Affiliate affiliate)
        {
            var listTimes = BookingService.GetListTimes(affiliate.BookingIntervalMinutes);

            UpdateAffiliateAdditionalTime(affiliate, listTimes);
            UpdateReservationResourceAdditionalTime(affiliate, listTimes);
        }

        private void UpdateAffiliateAdditionalTime(Core.Services.Booking.Affiliate affiliate, List<Tuple<TimeSpan, TimeSpan>> listTimes)
        {
            var currentTimes =
                AffiliateAdditionalTimeService.GetByAffiliate(affiliate.Id).GroupBy(x => x.StartTime.Date).ToList();

            if (currentTimes.Count > 0)
            {
                foreach (var dayCurrentTimes in currentTimes)
                {
                    var firstElement = dayCurrentTimes.First();

                    if (firstElement.IsWork)
                    {
                        AffiliateAdditionalTimeService.DeleteByAffiliateAndDate(affiliate.Id, dayCurrentTimes.Key);

                        var newTimes =
                            listTimes.Where(listTime => dayCurrentTimes.Any(
                                currentTime =>
                                    TimeCross(listTime.Item1, listTime.Item2, currentTime.StartTime, currentTime.EndTime)))
                                .Select(x =>
                                    new AffiliateAdditionalTime
                                    {
                                        AffiliateId = affiliate.Id,
                                        StartTime = dayCurrentTimes.Key + x.Item1,
                                        EndTime = dayCurrentTimes.Key + x.Item2,
                                        IsWork = firstElement.IsWork
                                    }
                                );

                        newTimes.ForEach(x => AffiliateAdditionalTimeService.Add(x));
                    }
                }
            }
        }

        private void UpdateReservationResourceAdditionalTime(Core.Services.Booking.Affiliate affiliate, List<Tuple<TimeSpan, TimeSpan>> listTimes)
        {
            foreach (var reservationResource in ReservationResourceService.GetByAffiliate(affiliate.Id))
            {
                if (ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id, reservationResource.Id) != null)
                    continue;

                var currentTimes =
                    ReservationResourceAdditionalTimeService.GetBy(affiliate.Id, reservationResource.Id).GroupBy(x => x.StartTime.Date).ToList();

                if (currentTimes.Count > 0)
                {
                    foreach (var dayCurrentTimes in currentTimes)
                    {
                        var firstElement = dayCurrentTimes.First();

                        if (firstElement.IsWork)
                        {
                            ReservationResourceAdditionalTimeService.DeleteByDate(affiliate.Id, reservationResource.Id, dayCurrentTimes.Key);

                            var newTimes =
                                listTimes.Where(listTime => dayCurrentTimes.Any(
                                    currentTime =>
                                        TimeCross(listTime.Item1, listTime.Item2, currentTime.StartTime, currentTime.EndTime)))
                                    .Select(x =>
                                        new ReservationResourceAdditionalTime
                                        {
                                            AffiliateId = affiliate.Id,
                                            ReservationResourceId = reservationResource.Id,
                                            StartTime = dayCurrentTimes.Key + x.Item1,
                                            EndTime = dayCurrentTimes.Key + x.Item2,
                                            IsWork = firstElement.IsWork
                                        }
                                    );

                            newTimes.ForEach(x => ReservationResourceAdditionalTimeService.Add(x));
                        }
                    }
                }
            }
        }

        private void UpdateBookingTimeReservationResources(Core.Services.Booking.Affiliate affiliate)
        {
            var affiliateCurrentTimes = AffiliateTimeOfBookingService.GetByAffiliate(affiliate.Id);

            foreach (var reservationResource in ReservationResourceService.GetByAffiliate(affiliate.Id))
            {
                if (ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id, reservationResource.Id) != null)
                    continue;

                var reservationResourceCurrentTimes = ReservationResourceTimeOfBookingService.GetBy(affiliate.Id, reservationResource.Id);

                if (reservationResourceCurrentTimes.Count > 0)
                {
                    ReservationResourceTimeOfBookingService.DeleteBy(affiliate.Id, reservationResource.Id);

                    foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        var dayAffiliateCurrentTimes = affiliateCurrentTimes.Where(x => x.DayOfWeek == dayOfWeek);
                        var dayReservationResourceCurrentTimes = reservationResourceCurrentTimes.Where(x => x.DayOfWeek == dayOfWeek);
                        var newTimes =
                            dayAffiliateCurrentTimes.Where(listTime => dayReservationResourceCurrentTimes.Any(currentTime => TimeCross(listTime.StartTime, listTime.EndTime, currentTime.StartTime, currentTime.EndTime)))
                                .Select(x =>
                                    new ReservationResourceTimeOfBooking
                                    {
                                        AffiliateId = affiliate.Id,
                                        ReservationResourceId = reservationResource.Id,
                                        DayOfWeek = dayOfWeek,
                                        StartTime = x.StartTime,
                                        EndTime = x.EndTime
                                    }
                                );

                        newTimes.ForEach(x => ReservationResourceTimeOfBookingService.Add(x));
                    }
                }
            }
        }

        private bool TimeCross(TimeSpan from, TimeSpan to, DateTime start, DateTime end)
        {
            var startTime = start.TimeOfDay;
            var endTime = end.TimeOfDay;

            if (endTime < startTime)
                endTime += new TimeSpan(1, 0, 0, 0);

            if (to < from)
                to += new TimeSpan(1, 0, 0, 0);

            return startTime < to && from < endTime;
            //return (start <= from && from < end) || (start < to && to <= end);
        }

        private bool TimeCross(TimeSpan from, TimeSpan to, TimeSpan start, TimeSpan end)
        {
            if (to < from)
                to += new TimeSpan(1, 0, 0, 0);

            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            return start < to && from < end;
            //return (start <= from && from < end) || (start < to && to <= end);
        }

        private List<AffiliateTimeOfBooking> ParseTimes(string times, DayOfWeek dayOfWeek, Core.Services.Booking.Affiliate affiliate)
        {
            string[] tempTimes;
            return times.IsNullOrEmpty()
                ? new List<AffiliateTimeOfBooking>()
                : times.Trim(new[] {'[', ']'})
                    .Replace("\"", "")
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(
                        x =>
                            new AffiliateTimeOfBooking
                            {
                                AffiliateId = affiliate.Id,
                                DayOfWeek = dayOfWeek,
                                StartTime =
                                    TimeSpan.ParseExact((tempTimes = x.Split('-'))[0], "hh\\:mm",
                                        CultureInfo.InvariantCulture),
                                EndTime =
                                    TimeSpan.ParseExact(tempTimes[1], "hh\\:mm", CultureInfo.InvariantCulture)
                            })
                    .ToList();
        }
    }
}
