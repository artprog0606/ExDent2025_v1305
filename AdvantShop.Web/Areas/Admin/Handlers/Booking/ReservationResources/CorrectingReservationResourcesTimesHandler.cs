using System;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    /// <summary>
    /// Удаляет время у сотрудников не рабочее для филиала
    /// </summary>
    public class CorrectingReservationResourcesTimesHandler
    {
        private readonly int _affiliateId;
        private readonly DateTime? _date;
        private readonly DateTime _dateFrom;
        private readonly DateTime? _dateTo;
        private readonly TypeCorrectingReservationResourcesTimes _typeCorrecting;

        public CorrectingReservationResourcesTimesHandler(int affiliateId) : this(affiliateId, TypeCorrectingReservationResourcesTimes.All) // корректируем данные с сегодняшнего дня, т.к. прошедшие дни уже не важны
        {
        }
        public CorrectingReservationResourcesTimesHandler(int affiliateId, TypeCorrectingReservationResourcesTimes typeCorrecting) : this(affiliateId, DateTime.Today, null, typeCorrecting) // корректируем данные с сегодняшнего дня, т.к. прошедшие дни уже не важны
        {
        }

        public CorrectingReservationResourcesTimesHandler(int affiliateId, DateTime date, TypeCorrectingReservationResourcesTimes typeCorrecting)
        {
            _affiliateId = affiliateId;
            _date = date;
            _typeCorrecting = typeCorrecting;
        }

        public CorrectingReservationResourcesTimesHandler(int affiliateId, DateTime dateFrom, DateTime? dateTo, TypeCorrectingReservationResourcesTimes typeCorrecting)
        {
            _affiliateId = affiliateId;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _typeCorrecting = typeCorrecting;
        }

        public void Execute()
        {
            new CorrectingReservationResourcesTimeOfBookingHandler(_affiliateId, _typeCorrecting).Execute();

            if (_date.HasValue)
                new CorrectingReservationResourcesAdditionalTimeHandler(_affiliateId, _date.Value, _typeCorrecting).Execute();
            else
                new CorrectingReservationResourcesAdditionalTimeHandler(_affiliateId, _dateFrom, _dateTo, _typeCorrecting).Execute();
        }
    }

    public enum TypeCorrectingReservationResourcesTimes
    {
        All,
        WithDefaultInterval,
        WithIndividualInterval
    }

}