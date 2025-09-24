using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class AddUpdateBookingHandler
    {
        private readonly AddUpdateBookingModel _model;

        public bool UserConfirmIsRequired { get; set; }
        public string ConfirmMessageIsRequired { get; set; }
        public string ConfirmMessage { get; set; }
        public string ConfirmButtomText { get; set; }
        public List<string> Errors { get; set; }

        public AddUpdateBookingHandler(AddUpdateBookingModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var affiliate = AffiliateService.Get(_model.AffiliateId);

            if (affiliate == null)
            {
                Errors.Add("Филиал не найден");
                return false;
            }

            if (_model.BeginDate >= _model.EndDate)
            {
                Errors.Add("Начало брони меньше или равно окончанию");
                return false;
            }

            if (_model.Id <= 0 && !AffiliateService.CheckAccessToEditing(affiliate))
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            if (Errors.Count == 0)
            {
                var booking = CreateUpdateBooking();

                if (booking == null)
                    return false;
            }
            return Errors.Count == 0;
        }

        private Core.Services.Booking.Booking CreateUpdateBooking()
        {
            if (BookingService.Exist(_model.AffiliateId, _model.ReservationResourceId, _model.BeginDate, _model.EndDate, ignoreBookingId: _model.Id))
            {
                Errors.Add("Пересечение броней по времени");
                return null;
            }

            if (!_model.UserConfirmed)
            {
                var confirmMessages = new List<string>();

                if (!CheckTimeIsWork())
                    confirmMessages.Add("Указанное время является нерабочим.");

                if (!IsAllServicesCanBeDoneByReservationResource())
                    confirmMessages.Add("Указанный сотрудник не осуществляет некоторые выбранные услуги.");

                if (confirmMessages.Count > 0)
                {
                    UserConfirmIsRequired = true;
                    ConfirmMessage = string.Format("{0} Продолжить?", string.Join(" ", confirmMessages));
                    ConfirmButtomText = "Да, продолжить";

                    return null;
                }
            }

            var booking = BookingService.Get(_model.Id) ?? new Core.Services.Booking.Booking()
            {
                AffiliateId = _model.AffiliateId,
                DateAdded = DateTime.Now
            };

            booking.IsFromAdminArea = true;

            if (booking.Id > 0 && !BookingService.CheckAccessToEditing(booking))
            {
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                return null;
            }

            var statusChanged = booking.Status != _model.Status;

            booking.BeginDate = _model.BeginDate;
            booking.EndDate = _model.EndDate;
            booking.ReservationResourceId = _model.ReservationResourceId;
            booking.Status = _model.Status;
            booking.OrderSourceId = _model.OrderSourceId;
            booking.ManagerId = _model.ManagerId;
            booking.AdminComment = _model.AdminComment.DefaultOrEmpty();

            // **** customer data
            booking.FirstName = _model.FirstName.DefaultOrEmpty();
            booking.LastName = _model.LastName.DefaultOrEmpty();
            booking.Patronymic = _model.Patronymic.DefaultOrEmpty();
            booking.Phone = _model.Phone.DefaultOrEmpty();
            booking.Email = _model.EMail.DefaultOrEmpty();
            booking.StandardPhone = StringHelper.ConvertToStandardPhone(_model.Phone, true, true);


            booking.CustomerId = _model.CustomerId;
            if (_model.CustomerId == null || booking.Customer == null)//_model
            {
                if (_model.FirstName.IsNotEmpty() ||
                    _model.LastName.IsNotEmpty() ||
                    _model.Patronymic.IsNotEmpty() ||
                    _model.Organization.IsNotEmpty() ||
                    _model.EMail.IsNotEmpty() ||
                    _model.Phone.IsNotEmpty() ||
                    _model.BirthDay != null ||
                    SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity)
                {
                    booking.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        FirstName = _model.FirstName.DefaultOrEmpty(),
                        LastName = _model.LastName.DefaultOrEmpty(),
                        Patronymic = _model.Patronymic.DefaultOrEmpty(),
                        Organization = _model.Organization.DefaultOrEmpty(),
                        Phone = _model.Phone.DefaultOrEmpty(),
                        EMail = _model.EMail.DefaultOrEmpty(),
                        StandardPhone =
                            !string.IsNullOrEmpty(_model.Phone)
                                ? StringHelper.ConvertToStandardPhone(_model.Phone, true, true)
                                : null,
                        BirthDay = _model.BirthDay,
                        CustomerType = _model.CustomerType
                    };
                }
            }
            else if (booking.Customer != null)//_model
            {
                //if (booking.Customer == null)
                //    booking.Customer = new Customer(CustomerGroupService.DefaultCustomerGroup);

                booking.CustomerId = _model.CustomerId;
                booking.Customer.FirstName = _model.FirstName.DefaultOrEmpty();
                booking.Customer.LastName = _model.LastName.DefaultOrEmpty();
                booking.Customer.Patronymic = _model.Patronymic.DefaultOrEmpty();
                booking.Customer.Organization = _model.Organization.DefaultOrEmpty();
                booking.Customer.Phone = _model.Phone.DefaultOrEmpty();
                booking.Customer.EMail = _model.EMail.DefaultOrEmpty();
                booking.Customer.BirthDay = _model.BirthDay;
                booking.Customer.CustomerType = _model.CustomerType;
            }

            if (booking.Customer != null)
            {
                if (booking.Customer.Contacts == null || booking.Customer.Contacts.Count == 0)
                {
                    var country = CountryService.GetCountry(SettingsMain.SellerCountryId);

                    booking.Customer.Contacts = new List<CustomerContact>()
                    {
                        new CustomerContact()
                        {
                            CustomerGuid = booking.Customer.Id,
                            Country = country != null ? country.Name : "",
                            City = SettingsMain.City
                        }
                    };
                }

                if (booking.Customer != null && booking.Customer.Contacts != null &&
                    booking.Customer.Contacts.Count > 0) //_model
                {
                    booking.Customer.Contacts[0].CustomerGuid = booking.Customer.Id;
                    booking.Customer.Contacts[0].City = booking.Customer.Contacts[0].City;
                }
            }

            // ****
            booking.BookingItems = _model.Items != null
                ? _model.Items.Select(x => new BookingItem
                    {
                        Id = x.Id,
                        BookingId = booking.Id,
                        ServiceId = x.ServiceId,
                        ArtNo = x.ArtNo,
                        Name = x.Name,
                        Amount = x.Amount,
                        Price = x.Price
                    }).ToList()
                : new List<BookingItem>();

            booking.BookingDiscount = _model.Summary.BookingDiscount;
            booking.BookingDiscountValue = _model.Summary.BookingDiscountValue;
            booking.PaymentMethodId = _model.Summary.PaymentMethodId;
            booking.ArchivedPaymentName = _model.Summary.PaymentName;
            booking.PaymentCost = _model.Summary.PaymentCost;
            booking.PaymentDetails = _model.Summary.PaymentDetails;

            float totalPrice = 0;
            float totalItemsPrice = 0;
            float totalDiscount = 0;

            totalItemsPrice =
                booking.BookingItems.Sum(item => item.Price * item.Amount);

            totalDiscount += booking.BookingDiscount > 0 ? (booking.BookingDiscount * totalItemsPrice / 100) : 0;
            totalDiscount += booking.BookingDiscountValue;

            totalDiscount = totalDiscount.RoundPrice(booking.BookingCurrency.CurrencyValue, booking.BookingCurrency);

            totalPrice = (totalItemsPrice - totalDiscount + booking.PaymentCost).RoundPrice(booking.BookingCurrency.CurrencyValue, booking.BookingCurrency);

            if (totalPrice < 0) totalPrice = 0;

            booking.Sum = totalPrice;
            booking.DiscountCost = totalDiscount;

            if (booking.Id > 0)
            {
                BookingService.Update(booking);

                BizProcessExecuter.BookingChanged(booking);

                if (statusChanged)
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_BookingStatusChanged);
                else
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_EditBooking);
            }
            else
            {
                booking.Id = BookingService.Add(booking);

                BizProcessExecuter.BookingAdded(booking);
            }

            BookingService.PayBooking(booking.Id, _model.Payed);

            if (booking.CustomerId != null && _model.CustomerFields != null)
            {
                foreach (var customerField in _model.CustomerFields)
                {
                    BookingHistoryService.TrackBookingCustomerFieldChanges(booking.Id, booking.CustomerId.Value, customerField.Id, customerField.Value, null);

                    CustomerFieldService.AddUpdateMap(booking.CustomerId.Value, customerField.Id, customerField.Value ?? "", booking.Customer != null && !booking.Customer.RegistredUser);
                }
            }

            return booking;
        }

        private bool IsAllServicesCanBeDoneByReservationResource()
        {
            if (_model.ReservationResourceId.HasValue && _model.Items != null)
            {
                var reservationResourceServices =
                    ServiceService.GetListIdsByReservationResourceServices(_model.AffiliateId, _model.ReservationResourceId.Value);

                return _model.Items.Where(x => x.ServiceId.HasValue).All(x => reservationResourceServices.Contains(x.ServiceId.Value));
            }

            return true;
        }

        private bool CheckTimeIsWork()
        {
            ReservationResource reservationResource = _model.ReservationResourceId.HasValue
                ? ReservationResourceService.Get(_model.ReservationResourceId.Value)
                : null;

            var oneDay = _model.BeginDate.Date == _model.EndDate.Date;

            var affiliateAdditionalTime = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(_model.AffiliateId, _model.BeginDate.Date, _model.EndDate.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            var affiliateTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                (oneDay
                    ? AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(_model.AffiliateId,
                        _model.BeginDate.DayOfWeek)
                    : AffiliateTimeOfBookingService.GetByAffiliate(_model.AffiliateId))

                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            if (reservationResource != null)
            {
                var reservationResourceAdditionalTime = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                    ReservationResourceAdditionalTimeService.GetByDateFromTo(_model.AffiliateId, reservationResource.Id, _model.BeginDate.Date, _model.EndDate.Date.AddDays(1))
                        .GroupBy(x => x.StartTime.Date)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    );

                var reservationResourceTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                    (oneDay
                        ? ReservationResourceTimeOfBookingService.GetByDayOfWeek(_model.AffiliateId,
                            reservationResource.Id, _model.BeginDate.DayOfWeek)
                        : ReservationResourceTimeOfBookingService.GetBy(_model.AffiliateId, reservationResource.Id))

                        .GroupBy(x => x.DayOfWeek)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    );

                return ReservationResourceService.CheckDateRangeIsWork(_model.BeginDate, _model.EndDate,
                    reservationResourceAdditionalTime,
                    reservationResourceTimesOfBookingDayOfWeek,
                    affiliateAdditionalTime,
                    affiliateTimesOfBookingDayOfWeek);
            }
            else
            {
                return AffiliateService.CheckDateRangeIsWork(_model.BeginDate, _model.EndDate,
                    affiliateAdditionalTime, affiliateTimesOfBookingDayOfWeek);
            }
        }
    }
}
