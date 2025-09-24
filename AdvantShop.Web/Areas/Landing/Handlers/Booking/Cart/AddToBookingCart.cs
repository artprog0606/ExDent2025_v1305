using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Models.Booking.Cart;
using AdvantShop.Core;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Booking.Cart;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking.Cart
{
    public class AddToBookingCart : AbstractCommandHandler<AddToCartResult>
    {
        private readonly CartItemAddingModel _item;
        private Affiliate _affiliate;
        private ReservationResource _resource;
        private List<Service> _selectedServices;

        public AddToBookingCart(CartItemAddingModel item)
        {
            _item = item;
        }

        protected override void Load()
        {
            _affiliate = AffiliateService.Get(_item.AffiliateId);
            _resource = ReservationResourceService.Get(_item.ResourceId);
            _selectedServices = _item.SelectedServices.Select(serviceId => ServiceService.Get(serviceId)).Where(x => x != null && x.Enabled).ToList();
        }

        protected override void Validate()
        {
            if (_affiliate == null)
                throw new BlException("Филиал не найден");
            if (_resource == null)
                throw new BlException("Ресурс не найден");

            if (_item.BeginDate >= _item.EndDate)
                throw new BlException("Начало брони меньше или равно окончанию");

            if (!CheckTimeIsWork())
                throw new BlException("Указанное время является нерабочим");

            if (BookingService.Exist(_item.AffiliateId, _item.ResourceId, _item.BeginDate, _item.EndDate))
                throw new BlException("Выбранное время занято");
        }

        protected override AddToCartResult Handle()
        {
            var cartId = ShoppingCartService.AddShoppingCartItem(GetBookingItem());

            return new AddToCartResult("success")
            {
                CartId = cartId,
                TotalItems = ShoppingCartService.CurrentShoppingCart.Count,
            };
        }

        private ShoppingCartItem GetBookingItem()
        {
            var item = new ShoppingCartItem
            {
                AffiliateId = _item.AffiliateId,
                ReservationResourceId = _item.ResourceId,
                BeginDate = _item.BeginDate,
                EndDate = _item.EndDate,
            };

            item.Services.AddRange(_selectedServices.Select(x => new ServiceItem
            {
                ServiceId = x.Id,
                Amount = 1,
            }));

            return item;
        }

        private bool CheckTimeIsWork()
        {
            var affiliateAdditionalTime = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(_item.AffiliateId, _item.BeginDate.Date, _item.EndDate.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            var affiliateTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                AffiliateTimeOfBookingService.GetByAffiliate(_item.AffiliateId)
                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            var reservationResourceAdditionalTime = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                ReservationResourceAdditionalTimeService.GetByDateFromTo(_item.AffiliateId, _item.ResourceId, _item.BeginDate.Date, _item.EndDate.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            var reservationResourceTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                ReservationResourceTimeOfBookingService.GetBy(_item.AffiliateId, _item.ResourceId)
                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            return ReservationResourceService.CheckDateRangeIsWork(_item.BeginDate, _item.EndDate,
                reservationResourceAdditionalTime,
                reservationResourceTimesOfBookingDayOfWeek,
                affiliateAdditionalTime,
                affiliateTimesOfBookingDayOfWeek);
        }

    }
}
