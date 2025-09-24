using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;

namespace AdvantShop.Core.Services.ChangeHistories
{
    public class BookingHistoryService
    {
        public static void NewBooking(Booking.Booking booking, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = booking.Id,
                ObjType = ChangeHistoryObjType.Booking,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.BookingHistory.BookingCreated", booking.Id)
            });
        }

        public static void DeleteBooking(Booking.Booking booking, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = booking.Id,
                ObjType = ChangeHistoryObjType.Booking,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.BookingHistory.BookingDeleted", booking.Id)
            });
        }

        public static void TrackBookingChanges(Booking.Booking newBooking, Booking.Booking oldBooking, ChangedBy changedBy)
        {
            if (oldBooking == null)
                return;

            var history = ChangeHistoryService.GetChanges(newBooking.Id, ChangeHistoryObjType.Booking, oldBooking, newBooking, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackBookingCustomerChanges(int bookingId, Customer newCustomer, Customer oldCustomer, ChangedBy changedBy)
        {
            if (newCustomer == null || newCustomer.Id == Guid.Empty || oldCustomer == null || oldCustomer.Id == Guid.Empty)
                return;

            var history = ChangeHistoryService.GetChanges(bookingId, ChangeHistoryObjType.Booking, oldCustomer, newCustomer, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackChangingPaymentDetails(int bookingId, PaymentDetails details, ChangedBy changedBy)
        {
            var oldDetails = BookingPaymentDetailsService.GetPaymentDetails(bookingId);
            if (oldDetails == null || details == null)
                return;

            var history = ChangeHistoryService.GetChanges(bookingId, ChangeHistoryObjType.Booking, oldDetails, details, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackBookingItemChanges(int bookingId, BookingItem newBookingItem, BookingItem oldBookingItem, ChangedBy changedBy)
        {
            var history = new List<ChangeHistory>();

            if (oldBookingItem != null && newBookingItem != null)
                history = ChangeHistoryService.GetChanges(bookingId, ChangeHistoryObjType.Booking, oldBookingItem, newBookingItem, changedBy, oldBookingItem.Id);

            else if (oldBookingItem == null && newBookingItem != null)
            {
                history = new List<ChangeHistory>()
                {
                    new ChangeHistory(changedBy)
                    {
                        ObjId = bookingId,
                        ObjType = ChangeHistoryObjType.Booking,
                        ParameterName =
                            "Добавлена услуга " + newBookingItem.Name +
                            (!string.IsNullOrEmpty(newBookingItem.ArtNo) ? " (" + newBookingItem.ArtNo + ")" : ""),
                        ParameterId = newBookingItem.ServiceId,
                    }
                };
            }
            else if (oldBookingItem != null && newBookingItem == null)
            {
                history = new List<ChangeHistory>()
                {
                    new ChangeHistory(changedBy)
                    {
                        ObjId = bookingId,
                        ObjType = ChangeHistoryObjType.Booking,
                        ParameterName =
                            "Удалена услуга " + oldBookingItem.Name +
                            (!string.IsNullOrEmpty(oldBookingItem.ArtNo) ? " (" + oldBookingItem.ArtNo + ")" : ""),
                        ParameterId = oldBookingItem.ServiceId,
                    }
                };
            }

            ChangeHistoryService.Add(history);
        }

        public static void TrackBookingCustomerFieldChanges(int bookingId, Guid customerId, int id, string newValue, ChangedBy changedBy)
        {
            var oldField = CustomerFieldService.GetCustomerFieldsWithValue(customerId, id);
            if (oldField == null)
                return;

            var oldValue = oldField.Value ?? "";
            newValue = newValue ?? "";

            if (oldValue == newValue)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = bookingId,
                ObjType = ChangeHistoryObjType.Booking,
                ParameterName = oldField.Name,
                OldValue = oldValue,
                NewValue = newValue ?? ""
            });
        }

        public static void TrackChangingBookingTotal(int bookingId, OnRefreshTotalBooking newRefreshTotalOrder,
            OnRefreshTotalBooking oldRefreshTotalBooking, ChangedBy changedBy)
        {
            var history = ChangeHistoryService.GetChanges(bookingId, ChangeHistoryObjType.Booking, oldRefreshTotalBooking, newRefreshTotalOrder, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void AddOrder(int bookingId, Order order, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = bookingId,
                ObjType = ChangeHistoryObjType.Booking,
                ParameterId = order.OrderID,
                ParameterType = ChangeHistoryParameterType.Order,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.BookingHistory.AddOrder", order.OrderID, order.Number)
            });
        }
    }

    public class OnRefreshTotalBooking
    {
        [Compare("Core.ChangeHistories.OnRefreshTotalBooking.Sum")]
        public float Sum { get; set; }
    }
}
