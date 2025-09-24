using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.ReservationResources;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class BookingsJournalModel
    {
        public BookingsJournalModel()
        {
            Columns = new List<BookingsJournalColumnModel>();
        }

        public string Name { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public TimeSpan DefaultBookingDuration { get; set; }
        public List<BookingsJournalColumnModel> Columns { get; set; }
        public int TotalEventsCount { get { return Columns.Sum(x => x.TotalEventsCount); } }
        public string TotalString { get; set; }
    }

    public class BookingsJournalColumnModel
    {
        private readonly ReservationResourceModel _reservationResource;

        public BookingsJournalColumnModel()
        {
            Events = new List<BookingJournalModel>();
        }

        public BookingsJournalColumnModel(string id): this()
        {
            Id = id;
        }

        public BookingsJournalColumnModel(ReservationResource reservationResource) : this(reservationResource.Id.ToString())
        {
            _reservationResource = (ReservationResourceModel)reservationResource;
        }


        public string Id { get; set; }
        public string Name { get; set; }

        public object Obj
        {
            get { return Id.IsNotEmpty() ? ReservationResource : null; }
            set { }
        }

        public string Class { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan BookingDuration { get; set; }

        [IgnoreDataMember]
        public DateTime MinDateTime { get; set; }
        public string MinTime {
            get { return (MinDateTime - Date.Date).ToString("c"); }
        }
        

        [IgnoreDataMember]
        public DateTime MaxDateTime { get; set; }
        public string MaxTime
        {
            get { return (MaxDateTime - Date.Date).ToString("c"); }
        }

        public int TotalEventsCount{ get; set; }

        public string TotalString { get; set; }

        [IgnoreDataMember]
        public ReservationResourceModel ReservationResource
        {
            get { return _reservationResource; }
        }

        [IgnoreDataMember]
        public List<Tuple<TimeSpan, TimeSpan>> ListTimes { get; set; }

        [IgnoreDataMember]
        public List<ReservationResourceTimeOfBooking> TimesOfBooking { get; set; }

        [IgnoreDataMember]
        public List<ReservationResourceAdditionalTime> AdditionalTimes { get; set; }

        public List<BookingJournalModel> Events { get; set; }
        public bool IsNotWork { get; set; }
    }


    public enum EnRendering
    {
        [EnumMember(Value = "")]
        None,
        [EnumMember(Value = "background")]
        Background,
        [EnumMember(Value = "inverse-background")]
        InverseBackground
    }

    [DataContract]
    public class EventModel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "start")]
        public DateTime Start { get; set; }

        [DataMember(Name = "end")]
        public DateTime End { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "className")]
        public string ClassName { get; set; }

        [IgnoreDataMember]
        public EnRendering EnRendering { get; set; }

        [DataMember(Name = "rendering")]
        public string Rendering {
            get
            {
                switch (EnRendering)
                {
                    case EnRendering.None:
                        return string.Empty;
                    case EnRendering.Background:
                        return "background";
                    case EnRendering.InverseBackground:
                        return "inverse-background";
                }
                return string.Empty;
            }
        }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "backgroundColor")]
        public string BackgroundColor { get; set; }

        [DataMember(Name = "borderColor")]
        public string BorderColor { get; set; }

        [DataMember(Name = "textColor")]
        public string TextColor { get; set; }

        [DataMember(Name = "constraint")]
        public Constraint Constraint { get; set; }
    }

    [DataContract]
    public class Constraint
    {
        [DataMember(Name = "start")]
        public DateTime Start { get; set; }

        [DataMember(Name = "end")]
        public DateTime End { get; set; }
    }

    [DataContract]
    public class BookingJournalModel : EventModel
    {
        //[DataMember(Name = "columnId")]
        //public string ColumnId { get; set; }

        [DataMember(Name = "affiliateId")]
        public int AffiliateId { get; set; }

        [DataMember(Name = "reservationResourceId")]
        public int? ReservationResourceId { get; set; }

        [DataMember(Name = "isWork")]
        public bool? IsWork { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        public static BookingJournalModel CreateFromBooking(Core.Services.Booking.Booking booking)
        {
            return new BookingJournalModel()
            {
                Id = booking.Id.ToString(),
                AffiliateId = booking.AffiliateId,
                ReservationResourceId = booking.ReservationResourceId,
                Start = booking.BeginDate,
                End = booking.EndDate,
                Title =
                    (booking.Customer != null
                        ? string.Format("{0} {1} {2}", booking.Customer.LastName, booking.Customer.FirstName, booking.Customer.Patronymic)
                        : string.Format("{0} {1} {2}", booking.LastName, booking.FirstName, booking.Patronymic))
                    + (booking.AdminComment.IsNotEmpty() ? " \\ " + booking.AdminComment : string.Empty),
                Description = booking.AdminComment,

                ClassName = "booking-item " +
                            (booking.Status == BookingStatus.New
                                ? "booking-new"
                                : booking.Status == BookingStatus.Confirmed
                                    ? "booking-confirmed"
                                    : booking.Status == BookingStatus.Completed
                                        ? "booking-completed"
                                        : booking.Status == BookingStatus.Cancel
                                            ? "booking-cancel"
                                            : string.Empty),
                //Constraint = new Constraint
                //{
                //    Start = booking.BeginDate.Date,
                //    End = booking.BeginDate.Date.AddDays(1)
                //}
            };
        }
    }
}
