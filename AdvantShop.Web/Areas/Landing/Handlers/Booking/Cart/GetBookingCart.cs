using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Booking.Cart;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking.Cart
{
    public class GetBookingCart : AbstractCommandHandler<object>
    {
        protected override object Handle()
        {
            var cart = ShoppingCartService.CurrentShoppingCart;

            var cartItems =
                cart.Select(item =>
                {
                    var resourse = item.ReservationResourceId.HasValue
                        ? ReservationResourceService.Get(item.ReservationResourceId.Value)
                        : null;
                    var affiliate = AffiliateService.Get(item.AffiliateId);

                    return new
                    {
                        ShoppingCartItemId = item.ShoppingCartItemId,
                        AffiliateId = item.AffiliateId,
                        ReservationResourceId = item.ReservationResourceId,
                        AffiliateName = affiliate.Name,
                        ReservationResourceName = resourse != null ? resourse.Name : null,
                        ReservationResourcePhoto = resourse != null ? resourse.ImageSrc : null,
                        BeginDate = item.BeginDate,
                        EndDate = item.EndDate,
                        BookingDate = (item.BeginDate.Date == item.EndDate.Date
                            ? string.Format("{0:dd.MM.yyyy HH:mm}-{1:HH:mm}", item.BeginDate, item.EndDate)
                            : string.Format("{0:dd.MM.yyyy HH:mm}-{1:dd.MM.yyyy HH:mm}", item.BeginDate, item.EndDate)),
                        Sum = item.Sum.FormatPrice(),
                        Avalible = GetAvalibleState(item),
                    };
                }).ToList();

            var totalPrice = cart.TotalPrice;
            var totalItems = cart.Count;

            var count = string.Format("{0} {1}",
                totalItems == 0 ? "" : totalItems.ToString(CultureInfo.InvariantCulture),
                Strings.Numerals(totalItems,
                    "пусто",
                    "бронь",
                    "брони",
                    "броней"));

            string isValidCart = IsValidCart(cartItems, totalItems, totalPrice);

            var model = new
            {
                CartItems = cartItems,
                Count = count,
                TotalItems = totalItems,
                TotalPrice = totalPrice > 0 ? totalPrice.FormatPrice() : 0F.FormatPrice(),
                Valid = isValidCart,
            };

            return model;
        }

        private string IsValidCart(object cartItems, int itemsCount, float totalPrice)
        {
            if (itemsCount == 0)
                return "Список броней пуст";

            //if (cartProducts.Any(x => x.Avalible.IsNotEmpty()))
            //    return LocalizationService.GetResource("Cart.Error.NotAvailableProducts");

            return string.Empty;
        }

        private string GetAvalibleState(ShoppingCartItem item)
        {
            if (item.BeginDate >= item.EndDate)
                return "Начало брони меньше или равно окончанию";

            if (BookingService.Exist(item.AffiliateId, item.ReservationResourceId, item.BeginDate, item.EndDate))
                return "Выбранное время занято";

            if (!CheckTimeIsWork(item))
                return "Указанное время является нерабочим";

            return string.Empty;
        }

        private bool CheckTimeIsWork(ShoppingCartItem item)
        {
            var affiliateAdditionalTime = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(item.AffiliateId, item.BeginDate.Date, item.EndDate.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            var affiliateTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                AffiliateTimeOfBookingService.GetByAffiliate(item.AffiliateId)
                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList())
            );

            if (item.ReservationResourceId.HasValue) { 
                var reservationResourceAdditionalTime = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                    ReservationResourceAdditionalTimeService.GetByDateFromTo(item.AffiliateId, item.ReservationResourceId.Value, item.BeginDate.Date, item.EndDate.Date.AddDays(1))
                        .GroupBy(x => x.StartTime.Date)
                        .ToDictionary(x => x.Key, x => x.ToList())
                );

                var reservationResourceTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                    ReservationResourceTimeOfBookingService.GetBy(item.AffiliateId, item.ReservationResourceId.Value)
                        .GroupBy(x => x.DayOfWeek)
                        .ToDictionary(x => x.Key, x => x.ToList())
                );

                return ReservationResourceService.CheckDateRangeIsWork(item.BeginDate, item.EndDate,
                    reservationResourceAdditionalTime,
                    reservationResourceTimesOfBookingDayOfWeek,
                    affiliateAdditionalTime,
                    affiliateTimesOfBookingDayOfWeek);
            }

            return AffiliateService.CheckDateRangeIsWork(item.BeginDate, item.EndDate,
                affiliateAdditionalTime,
                affiliateTimesOfBookingDayOfWeek);
        }
    }
}
