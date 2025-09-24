//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.UrlRewriter;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Mails
{
    public abstract class BaseBookingMailTemplate : MailTemplate
    {
        private readonly Booking _booking;

        public BaseBookingMailTemplate(Booking booking)
        {
            _booking = booking;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#BOOKING_ID#", _booking.Id.ToString());
            formatedStr = formatedStr.Replace("#NAME#", new List<string> { _booking.LastName, _booking.FirstName, _booking.Patronymic }.Where(x => x.IsNotEmpty()).AggregateString(" "));
            formatedStr = formatedStr.Replace("#PHONE#", _booking.Phone);
            formatedStr = formatedStr.Replace("#EMAIL#", _booking.Email);
            formatedStr = formatedStr.Replace("#DATE#", _booking.BeginDate.Date == _booking.EndDate.Date
                        ? string.Format("{0} {1}-{2}",
                            _booking.BeginDate.ToShortDateString(),
                            _booking.BeginDate.ToShortTimeString(),
                            _booking.EndDate.ToShortTimeString())
                        : string.Format("{0} {1} - {2} {3}",
                            _booking.BeginDate.ToShortDateString(),
                            _booking.BeginDate.ToShortTimeString(),
                            _booking.EndDate.ToShortDateString(),
                            _booking.EndDate.ToShortTimeString()));
            formatedStr = formatedStr.Replace("#RESERVATIONRESOURCE#", _booking.ReservationResource != null ? _booking.ReservationResource.Name : string.Empty);
            var bookingItemsTable = _booking.BookingItems.Count > 0
                    ? BookingService.GenerateHtmlBookingItemsTable(_booking.BookingItems, _booking.BookingCurrency)
                    : "";
            formatedStr = formatedStr.Replace("#ORDERTABLE#", bookingItemsTable);
            formatedStr = formatedStr.Replace("#STORE_NAME#", SettingsMain.ShopName);
            formatedStr = formatedStr.Replace("#BOOKING_URL#", UrlService.GetAdminUrl("booking#?modal=" + _booking.Id));
            return formatedStr;
        }
    }

    public class BookingCreatedMailTemplate : BaseBookingMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnBookingCreated; }
        }

        //private readonly string _bookingId;
        //private readonly string _name;
        //private readonly string _phone;
        //private readonly string _date;
        //private readonly string _reservationResource;
        //private readonly string _email;
        //private readonly string _orderTable;

        public BookingCreatedMailTemplate(Booking booking) : base(booking)
        {
        }
    }

    public class BookingCommentAddedMailTemplate : BaseBookingMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnBookingCommentAdded; }
        }

        private readonly string _author;
        private readonly string _comment;

        public BookingCommentAddedMailTemplate(Booking booking, string author, string comment) : base(booking)
        {
            _author = author;
            _comment = comment;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = base.FormatString(formatedStr);
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);
            return formatedStr;
        }
    }

}
