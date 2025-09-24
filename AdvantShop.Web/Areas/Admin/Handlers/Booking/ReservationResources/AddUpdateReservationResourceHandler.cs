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
    public class AddUpdateReservationResourceHandler
    {
        private readonly AddUpdateReservationResourceModel _model;

        public List<string> Errors { get; set; }

        public AddUpdateReservationResourceHandler(AddUpdateReservationResourceModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var reservationResource = ReservationResourceService.Get(_model.Id) ?? new ReservationResource();
            var isNew = reservationResource.Id == 0;

            reservationResource.ManagerId = _model.ManagerId;
            reservationResource.Name = _model.Name;
            reservationResource.Description = _model.Description;
            reservationResource.SortOrder = _model.SortOrder;
            reservationResource.Enabled = _model.Enabled;

            if (reservationResource.Id <= 0)
            {
                reservationResource.Id = ReservationResourceService.Add(reservationResource);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_ResourceCreated);
            }
            else
            {
                ReservationResourceService.Update(reservationResource);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_EditResource);
            }


            if (_model.PhotoFile != null)
                new UploadImageCropped(ReservationResourceService.Get(reservationResource.Id), _model.Image, _model.PhotoFile).Execute();

            if (_model.Tags != null && _model.Tags.Count > 0)
            {
                var tags = TagService.GetList();

                var addingNewTag = false;
                for (int index = 0; index < _model.Tags.Count; index++)
                {
                    if (!tags.Any(t => t.Name.Equals(_model.Tags[index], StringComparison.OrdinalIgnoreCase)))
                    {
                        _model.Tags[index] = _model.Tags[index].TrimEnd(); // tagging-label
                        TagService.Add(new Tag()
                        {
                            Name = _model.Tags[index]
                        });
                        addingNewTag = true;
                    }
                }

                if (addingNewTag)
                    tags = TagService.GetList();

                var newTags = tags.Where(t => _model.Tags.Any(x => t.Name.Equals(x, StringComparison.OrdinalIgnoreCase))).ToList();
                var reservationResourceTags = TagService.GetReservationResourceTags(reservationResource.Id);

                // delete
                reservationResourceTags.Where(current => newTags.All(newItem => newItem.Id != current.Id))
                    .ForEach(x => TagService.DeleteMapTag(reservationResource.Id, x.Id));
                // add
                newTags.Where(newItem => reservationResourceTags.All(current => newItem.Id != current.Id))
                    .ForEach(x => TagService.AddMapTag(reservationResource.Id, x.Id));
            }
            else
                TagService.DeleteMapTag(reservationResource.Id);

            if (_model.AffiliateId.HasValue)
            {
                var affiliate = AffiliateService.Get(_model.AffiliateId.Value);
                if (affiliate == null)
                    Errors.Add("Филиал не найден");

                if (affiliate != null && !ReservationResourceService.CheckAccessToEditing(reservationResource, affiliate, null))
                    Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));

                if (Errors.Count == 0)
                {
                    var beforeBookingIntervalMinutes =
                        ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id, reservationResource.Id);

                    ReservationResourceService.AddUpdateRefAffiliate(affiliate.Id, reservationResource.Id, _model.BookingIntervalMinutes);


                    //Save time booking
                    var reservationResourceTimes = ReservationResourceTimeOfBookingService.GetBy(affiliate.Id, reservationResource.Id);

                    // Не нужна будет, когда перейдем на редактируемый график
                    var validateTimes = BookingService.GetListTimes(_model.BookingIntervalMinutes ?? affiliate.BookingIntervalMinutes);

                    var newTimes = ParseTimes(_model.MondayTimes, DayOfWeek.Monday, affiliate.Id, reservationResource.Id);
                    newTimes = newTimes.Where(parseTime => validateTimes.Any(validTime => parseTime.StartTime == validTime.Item1 && parseTime.EndTime == validTime.Item2)).ToList();
                    var current = reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Monday).ToList();
                    SaveTimes(current, newTimes);

                    newTimes = ParseTimes(_model.TuesdayTimes, DayOfWeek.Tuesday, affiliate.Id, reservationResource.Id);
                    newTimes = newTimes.Where(parseTime => validateTimes.Any(validTime => parseTime.StartTime == validTime.Item1 && parseTime.EndTime == validTime.Item2)).ToList();
                    current = reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Tuesday).ToList();
                    SaveTimes(current, newTimes);

                    newTimes = ParseTimes(_model.WednesdayTimes, DayOfWeek.Wednesday, affiliate.Id, reservationResource.Id);
                    newTimes = newTimes.Where(parseTime => validateTimes.Any(validTime => parseTime.StartTime == validTime.Item1 && parseTime.EndTime == validTime.Item2)).ToList();
                    current = reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Wednesday).ToList();
                    SaveTimes(current, newTimes);

                    newTimes = ParseTimes(_model.ThursdayTimes, DayOfWeek.Thursday, affiliate.Id, reservationResource.Id);
                    newTimes = newTimes.Where(parseTime => validateTimes.Any(validTime => parseTime.StartTime == validTime.Item1 && parseTime.EndTime == validTime.Item2)).ToList();
                    current = reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Thursday).ToList();
                    SaveTimes(current, newTimes);

                    newTimes = ParseTimes(_model.FridayTimes, DayOfWeek.Friday, affiliate.Id, reservationResource.Id);
                    newTimes = newTimes.Where(parseTime => validateTimes.Any(validTime => parseTime.StartTime == validTime.Item1 && parseTime.EndTime == validTime.Item2)).ToList();
                    current = reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Friday).ToList();
                    SaveTimes(current, newTimes);

                    newTimes = ParseTimes(_model.SaturdayTimes, DayOfWeek.Saturday, affiliate.Id, reservationResource.Id);
                    newTimes = newTimes.Where(parseTime => validateTimes.Any(validTime => parseTime.StartTime == validTime.Item1 && parseTime.EndTime == validTime.Item2)).ToList();
                    current = reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Saturday).ToList();
                    SaveTimes(current, newTimes);

                    newTimes = ParseTimes(_model.SundayTimes, DayOfWeek.Sunday, affiliate.Id, reservationResource.Id);
                    newTimes = newTimes.Where(parseTime => validateTimes.Any(validTime => parseTime.StartTime == validTime.Item1 && parseTime.EndTime == validTime.Item2)).ToList();
                    current = reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Sunday).ToList();
                    SaveTimes(current, newTimes);


                    var updateBookingInterval = beforeBookingIntervalMinutes != _model.BookingIntervalMinutes;
                    if (updateBookingInterval)
                    {
                        // вариант, когда новое значение совпадает с временем филиала
                        if (!beforeBookingIntervalMinutes.HasValue &&
                            _model.BookingIntervalMinutes.Value == affiliate.BookingIntervalMinutes)
                            updateBookingInterval = false;

                        // вариант, когда предыдущее значение совпадает с временем филиала
                        if (beforeBookingIntervalMinutes.HasValue && beforeBookingIntervalMinutes.Value == affiliate.BookingIntervalMinutes)
                            updateBookingInterval = false;
                    }

                    if (!isNew && updateBookingInterval)
                    {
                        UpdateAdditionalTime(reservationResource, _model.BookingIntervalMinutes, affiliate);
                    }

                }
            }

            return Errors.Count == 0;
        }

        private void SaveTimes(List<ReservationResourceTimeOfBooking> current, List<ReservationResourceTimeOfBooking> newTimes)
        {
            current.Where(
                currentTime =>
                    !newTimes.Any(
                        newTime => newTime.StartTime == currentTime.StartTime && newTime.EndTime == currentTime.EndTime))
                .ForEach(x => ReservationResourceTimeOfBookingService.Delete(x.Id));

            newTimes.Where(
                newTime =>
                    !current.Any(
                        currentTime => newTime.StartTime == currentTime.StartTime && newTime.EndTime == currentTime.EndTime))
                .ForEach(x => ReservationResourceTimeOfBookingService.Add(x));
        }

        private void UpdateAdditionalTime(ReservationResource reservationResource, int? bookingIntervalMinutes, Core.Services.Booking.Affiliate affiliate)
        {
            // Не нужна будет, когда перейдем на редактируемый график
            var listTimes = BookingService.GetListTimes(bookingIntervalMinutes ?? affiliate.BookingIntervalMinutes);

            var currentTimes =
                ReservationResourceAdditionalTimeService.GetBy(affiliate.Id, reservationResource.Id).GroupBy(x => x.StartTime.Date).ToList();

            if (currentTimes.Count > 0)
            {
                var dictionaryAffiliateTimesOfBooking = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                    AffiliateTimeOfBookingService.GetByAffiliate(affiliate.Id)
                        .GroupBy(x => x.DayOfWeek)
                        .ToDictionary(x => x.Key, x => x.ToList()));

                var dictionaryAffiliateAdditionalTimes = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                    AffiliateAdditionalTimeService.GetByAffiliate(affiliate.Id)
                        .GroupBy(x => x.StartTime.Date)
                        .ToDictionary(x => x.Key, x => x.ToList()));


                foreach (var dayCurrentTimes in currentTimes)
                {
                    var firstElement = dayCurrentTimes.First();

                    if (firstElement.IsWork)
                    {
                        var affiliateAdditionalTimes =
                            dictionaryAffiliateAdditionalTimes.ContainsKey(dayCurrentTimes.Key)
                                ? dictionaryAffiliateAdditionalTimes[dayCurrentTimes.Key]
                                : new List<AffiliateAdditionalTime>();

                        var affiliateTimesOfBookingDayOfWeek =
                            dictionaryAffiliateTimesOfBooking.ContainsKey(dayCurrentTimes.Key.DayOfWeek)
                                ? dictionaryAffiliateTimesOfBooking[dayCurrentTimes.Key.DayOfWeek]
                                : new List<AffiliateTimeOfBooking>();


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
                                ).ToList();

                        if (bookingIntervalMinutes == null)
                            newTimes
                                .Where(x =>
                                    AffiliateService.ExistDateRangeInTimeOfBooking(
                                        x.StartTime,
                                        x.EndTime,
                                        affiliateAdditionalTimes,
                                        affiliateTimesOfBookingDayOfWeek))
                                .ForEach(x => ReservationResourceAdditionalTimeService.Add(x));

                        if (bookingIntervalMinutes != null)
                            newTimes
                                .Where(x =>
                                    AffiliateService.CheckDateRangeIsWork(
                                        x.StartTime,
                                        x.EndTime,
                                        dictionaryAffiliateAdditionalTimes,
                                        dictionaryAffiliateTimesOfBooking))
                                .ForEach(x => ReservationResourceAdditionalTimeService.Add(x));

                        //newTimes.ForEach(x => ReservationResourceAdditionalTimeService.Add(x));
                    }
                }

                // не надо удалять нерабочее время, т.к. алгоритм добавляет только рабочее
                //new CorrectingReservationResourceAdditionalTimeHandler(reservationResource, dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking).Execute();

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

        private List<ReservationResourceTimeOfBooking> ParseTimes(List<string> times, DayOfWeek dayOfWeek, int affiliateId, int reservationResourceId)
        {
            string[] tempTimes;
            return times == null
                ? new List<ReservationResourceTimeOfBooking>()
                : times
                    .Select(
                        x =>
                            new ReservationResourceTimeOfBooking
                            {
                                AffiliateId = affiliateId,
                                ReservationResourceId = reservationResourceId,
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
