using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Booking.Cart
{
    public class ShoppingCart : List<ShoppingCartItem>
    {
        public ShoppingCart() { }

        public ShoppingCart(Guid customerId)
        {
            _customer = CustomerService.GetCustomer(customerId) ?? new Customer(CustomerGroupService.DefaultCustomerGroup) { Id = customerId, CustomerRole = Role.Guest };
        }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer ?? (_customer = CustomerContext.CurrentCustomer); }
        }

        private float _totalPrice = float.MinValue;
        public float TotalPrice
        {
            get
            {
                return _totalPrice != float.MinValue
                    ? _totalPrice
                    : (_totalPrice = this.Sum(x => x.Sum));
            }
        }

        public bool HasItems
        {
            get { return this.Count > 0; }
        }

        public bool CanOrder
        {
            get
            {
                if (!HasItems)
                    return false;

                return
                    this.All(
                        item =>
                            item.BeginDate < item.EndDate
                            && item.ReservationResourceId.HasValue
                                ? ReservationResourceService.CheckDateRangeIsWork(item.BeginDate, item.EndDate,
                                    DictionaryReservationResourceAdditionalTime[new AffiliateAndResourceIds(item.AffiliateId,item.ReservationResourceId.Value)],
                                    DictionaryReservationResourceTimesOfBookingDayOfWeek[new AffiliateAndResourceIds(item.AffiliateId,item.ReservationResourceId.Value)],
                                    DictionaryAffiliateAdditionalTime[item.AffiliateId],
                                    DictionaryAffiliateTimesOfBookingDayOfWeek[item.AffiliateId])
                                : AffiliateService.CheckDateRangeIsWork(item.BeginDate, item.EndDate,
                                      DictionaryAffiliateAdditionalTime[item.AffiliateId],
                                      DictionaryAffiliateTimesOfBookingDayOfWeek[item.AffiliateId])
                            && !BookingService.Exist(item.AffiliateId, item.ReservationResourceId, item.BeginDate, item.EndDate));
            }
        }

        #region Help
        //Для оптимизации количиства обращений к БД

        private SortedDictionary<int, SortedDictionary<DateTime, List<AffiliateAdditionalTime>>>
            _dictionaryAffiliateAdditionalTime;
        private SortedDictionary<int, SortedDictionary<DateTime, List<AffiliateAdditionalTime>>>
            DictionaryAffiliateAdditionalTime
        {
            get
            {
                return _dictionaryAffiliateAdditionalTime ?? (_dictionaryAffiliateAdditionalTime =
                           new SortedDictionary<int, SortedDictionary<DateTime, List<AffiliateAdditionalTime>>>(
                               this.GroupBy(item => item.AffiliateId).ToDictionary(group => group.Key, group =>
                                   new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                                       AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(group.Key,
                                               group.Min(x => x.BeginDate).Date,
                                               group.Max(x => x.EndDate).Date.AddDays(1))
                                           .GroupBy(x => x.StartTime.Date)
                                           .ToDictionary(x => x.Key, x => x.ToList())
                                   ))));
            }
        }

        private SortedDictionary<int, SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>>
            _dictionaryAffiliateTimesOfBookingDayOfWeek;
        private SortedDictionary<int, SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>>
            DictionaryAffiliateTimesOfBookingDayOfWeek
        {
            get
            {
                return _dictionaryAffiliateTimesOfBookingDayOfWeek ?? (_dictionaryAffiliateTimesOfBookingDayOfWeek =
                           new SortedDictionary<int, SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>>(
                               this.GroupBy(item => item.AffiliateId).ToDictionary(group => group.Key, group =>
                                   new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                                       AffiliateTimeOfBookingService.GetByAffiliate(group.Key)
                                           .GroupBy(x => x.DayOfWeek)
                                           .ToDictionary(x => x.Key, x => x.ToList())
                                   ))));
            }
        }

        private SortedDictionary<AffiliateAndResourceIds, SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>>
            _dictionaryReservationResourceAdditionalTime;
        private SortedDictionary<AffiliateAndResourceIds, SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>>
            DictionaryReservationResourceAdditionalTime
        {
            get
            {
                return _dictionaryReservationResourceAdditionalTime ?? (_dictionaryReservationResourceAdditionalTime =
                           new SortedDictionary<AffiliateAndResourceIds, SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>>(
                               this.Where(item => item.ReservationResourceId.HasValue)
                                   .GroupBy(item => new AffiliateAndResourceIds(item.AffiliateId, item.ReservationResourceId.Value))
                                   .ToDictionary(group => group.Key, group =>
                                       new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                                           ReservationResourceAdditionalTimeService
                                               .GetByDateFromTo(group.Key.AffiliateId, group.Key.ResourceId,
                                                   group.Min(x => x.BeginDate).Date,
                                                   group.Max(x => x.EndDate).Date.AddDays(1))
                                               .GroupBy(x => x.StartTime.Date)
                                               .ToDictionary(x => x.Key, x => x.ToList())
                                       ))));
            }
        }

        private SortedDictionary<AffiliateAndResourceIds, SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>>
            _dictionaryReservationResourceTimesOfBookingDayOfWeek;
        private SortedDictionary<AffiliateAndResourceIds, SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>>
            DictionaryReservationResourceTimesOfBookingDayOfWeek
        {
            get
            {
                return _dictionaryReservationResourceTimesOfBookingDayOfWeek ?? (_dictionaryReservationResourceTimesOfBookingDayOfWeek =
                           new SortedDictionary<AffiliateAndResourceIds, SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>>(
                               this.Where(item => item.ReservationResourceId.HasValue)
                                   .GroupBy(item => new AffiliateAndResourceIds(item.AffiliateId, item.ReservationResourceId.Value))
                                   .ToDictionary(group => group.Key, group =>
                                       new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                                           ReservationResourceTimeOfBookingService.GetBy(group.Key.AffiliateId, group.Key.ResourceId)
                                               .GroupBy(x => x.DayOfWeek)
                                               .ToDictionary(x => x.Key, x => x.ToList())
                                       ))));
            }
        }

        private class AffiliateAndResourceIds : IComparable
        {
            public AffiliateAndResourceIds(int affiliateId, int resourceId)
            {
                AffiliateId = affiliateId;
                ResourceId = resourceId;
            }

            public int AffiliateId { get; set; }
            public int ResourceId { get; set; }

            public override int GetHashCode()
            {
                return unchecked(AffiliateId ^ ResourceId);
            }

            public int CompareTo(object obj)
            {
                if (obj == null) return 1;
                AffiliateAndResourceIds temp = obj as AffiliateAndResourceIds;
                if (temp != null)
                    return this.AffiliateId.CompareTo(temp.AffiliateId) + this.ResourceId.CompareTo(temp.ResourceId);
                else
                    throw new ArgumentException("Object is not a AffiliateAndReservationResourceIds");
            }
        }
        #endregion
    }
}
