using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Payment;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetBookingHandler
    {
        private readonly Customer _currentCustomer;
        private readonly int _id;
        public List<string> Errors { get; set; }

        public GetBookingHandler(int id)
        {
            _currentCustomer = CustomerContext.CurrentCustomer;
            _id = id;
            Errors = new List<string>();
        }

        public AddUpdateBookingModel Execute()
        {
            var booking = BookingService.Get(_id);

            if (booking != null)
            {
                if (!BookingService.CheckAccess(booking))
                {
                    Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                    return null;
                }

                var model = new AddUpdateBookingModel()
                {
                    Id = booking.Id,
                    AffiliateId = booking.AffiliateId,
                    ReservationResourceId = booking.ReservationResourceId,
                    BeginDate = booking.BeginDate,
                    EndDate = booking.EndDate,
                    Status = booking.Status,
                    StatusName = booking.Status.ToString().ToLower(),
                    ManagerId = booking.ManagerId,
                    OrderSourceId = booking.OrderSourceId,
                    Payed = booking.Payed,
                    OrderId = booking.OrderId,
                    AdminComment = booking.AdminComment
                };

                if (booking.Customer != null)
                {
                    model.CustomerId = booking.Customer.Id;
                    model.FirstName = booking.Customer.FirstName;
                    model.LastName = booking.Customer.LastName;
                    model.Patronymic = booking.Customer.Patronymic;
                    model.Organization = booking.Customer.Organization;
                    model.Phone = booking.Customer.Phone;
                    model.StandardPhone = booking.Customer.StandardPhone;
                    model.EMail = booking.Customer.EMail;
                    model.BirthDay = booking.Customer.BirthDay;
                    model.CustomerType = booking.Customer.CustomerType;
                }
                else
                {
                    model.CustomerId = booking.CustomerId;
                    model.FirstName = booking.FirstName;
                    model.LastName = booking.LastName;
                    model.Patronymic = booking.Patronymic;
                    model.Phone = booking.Phone;
                    model.StandardPhone = booking.StandardPhone;
                    model.EMail = booking.Email;
                }

                model.CustomerFields =
                    CustomerFieldService.GetCustomerFieldsWithValue(booking.CustomerId != null
                        ? booking.CustomerId.Value
                        : Guid.Empty)
                    ?? new List<CustomerFieldWithValue>();

                model.Items = booking.BookingItems.Select(x => new BookingItemModel(x, booking.BookingCurrency)).ToList();

                var payment = booking.PaymentMethod;
                model.Summary = new BookingSummaryModel
                {
                    BookingCurrency = booking.BookingCurrency,
                    ServicesCost = booking.BookingItems.Sum(x => x.Price * x.Amount),
                    BookingDiscount = booking.BookingDiscount,
                    BookingDiscountValue = booking.BookingDiscountValue,
                    DiscountCost = booking.DiscountCost,
                    PaymentMethodId = booking.PaymentMethodId,
                    PaymentName = booking.PaymentMethodName,
                    PaymentCost = booking.PaymentCost,
                    PaymentDetails = booking.PaymentDetails,
                    PaymentKey = payment != null ? payment.PaymentKey.ToLower() : null,
                    Sum = booking.Sum,
                    ShowSendBillingLink = booking.Customer != null && booking.OrderId.HasValue && !booking.Payed && booking.Status != BookingStatus.Cancel,
                    ShowCreateBillingLink = booking.Customer != null && !booking.OrderId.HasValue && booking.Status != BookingStatus.Cancel,
                    OrderId = booking.OrderId,
                    CustomerType = booking.Customer != null && SettingsCustomers.IsRegistrationAsLegalEntity
                    ? booking.Customer.CustomerType
                    : SettingsCustomers.IsRegistrationAsPhysicalEntity ? CustomerType.PhysicalEntity : CustomerType.LegalEntity
                };
                if (model.Summary.PaymentKey == "bill")
                {
                    var getCustomerDataMethod = payment.Parameters[BillTemplate.GetCustomerDataMethod];
                    if (getCustomerDataMethod.TryParseEnum<EGetCustomerDataMethod>() == EGetCustomerDataMethod.InPayment && payment.Parameters[BillTemplate.ShowPaymentDetails].TryParseBool())
                    {
                        model.Summary.ShowCompanyNamePaymentDetails = true;
                        model.Summary.ShowInnPaymentDetails = true;
                    }
                }

                if (model.CustomerId.HasValue)
                    model.Social = new GetCustomerSocialHandler(model.CustomerId.Value).Execute();

                model.CanBeDeleted = _currentCustomer.IsAdmin;
                model.CanBeEditing = BookingService.CheckAccessToEditing(booking);

                return model;
            }

            Errors.Add("Бронь не найдена");
            return null;
        }
    }
}
