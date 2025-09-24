using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.App.Landing.Handlers.Landings;
using AdvantShop.App.Landing.Models;
using AdvantShop.App.Landing.Models.Landing;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Booking.Cart;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking
{
    public class AddBookingHandler : AbstractCommandHandler<SubmitFormReturnModel>
    {
        private readonly AddBookingDto _model;
        private readonly LpFormService _lpFormService;
        private readonly LpService _lpService;
        private readonly HttpFileCollection _files;

        private Affiliate _affiliate;
        private ReservationResource _resource;
        private LpForm _form;
        private Lp _lp;
        private LpSite _site;
        private List<Service> _selectedServices;

        public AddBookingHandler(AddBookingDto model, HttpFileCollection files)
        {
            _model = model;
            _lpFormService = new LpFormService();
            _lpService = new LpService();
            _files = files;
        }

        protected override void Load()
        {
            _form = _model.Form != null ? _lpFormService.Get(_model.Form.Id) : null;
            _affiliate = AffiliateService.Get(_model.AffiliateId);
            _resource = ReservationResourceService.Get(_model.ResourceId);
            _selectedServices = _model.SelectedServices.Select(serviceId => ServiceService.Get(serviceId)).Where(x => x != null && x.Enabled).ToList();
            if (_form != null)
                _lp = _lpService.Get(_form.LpId);
            if (_lp != null)
                _site = new LpSiteService().Get(_lp.LandingSiteId);
        }

        protected override void Validate()
        {
            if (_form == null)
                throw new BlException("Форма не найдена");
            if (_affiliate == null)
                throw new BlException("Филиал не найден");
            if (_resource == null)
                throw new BlException("Ресурс не найден");

            if (_model.BeginDate >= _model.EndDate)
                throw new BlException("Начало брони меньше или равно окончанию");

            if (!CheckTimeIsWork())
                throw new BlException("Указанное время является нерабочим");

            if (BookingService.Exist(_model.AffiliateId, _model.ResourceId, _model.BeginDate, _model.EndDate))
                throw new BlException("Выбранное время занято");
        }

        protected override SubmitFormReturnModel Handle()
        {
            var booking = new Core.Services.Booking.Booking()
            {
                AffiliateId = _model.AffiliateId,
                ReservationResourceId = _model.ResourceId,
                BeginDate = _model.BeginDate,
                EndDate = _model.EndDate,
                DateAdded = DateTime.Now,
                Customer = new Customer() { Contacts = new List<CustomerContact>() { new CustomerContact() } },
            };

            var source = _site == null
                ? Orders.OrderSourceService.GetOrderSource(OrderType.LandingPage)
                : Orders.OrderSourceService.GetOrderSource(OrderType.LandingPage, _site.Id, _site.Name);
            booking.OrderSourceId = source.Id;

            var customerFields = new List<CustomerFieldWithValue>();
            var files = new List<HttpPostedFile>();

            int index = 0;
            foreach (var item in _model.Form.Fields)
            {
                var field = item.Value;
                field.Value = HttpUtility.HtmlEncode(field.Value.DefaultOrEmpty());

                if (field.ObjId == null)
                {
                    var type = (ELpFormFieldType)field.Type;

                    switch (type)
                    {
                        case ELpFormFieldType.FirstName:
                            booking.FirstName = field.Value;
                            booking.Customer.FirstName = field.Value;
                            break;
                        case ELpFormFieldType.LastName:
                            booking.LastName = field.Value;
                            booking.Customer.LastName = field.Value;
                            break;
                        case ELpFormFieldType.Patronymic:
                            booking.Patronymic = field.Value;
                            booking.Customer.Patronymic = field.Value;
                            break;
                        case ELpFormFieldType.Email:
                            booking.Email = field.Value;
                            booking.Customer.EMail = field.Value;
                            break;
                        case ELpFormFieldType.Phone:
                            booking.Phone = field.Value;
                            booking.Customer.Phone = field.Value;
                            booking.StandardPhone = booking.Customer.StandardPhone = StringHelper.ConvertToStandardPhone(field.Value);
                            break;
                        case ELpFormFieldType.Comment:
                        case ELpFormFieldType.TextArea:
                            break;
                        case ELpFormFieldType.Country:
                            booking.Customer.Contacts[0].Country = field.Value;
                            break;
                        case ELpFormFieldType.Region:
                            booking.Customer.Contacts[0].Region = field.Value;
                            break;
                        case ELpFormFieldType.City:
                            booking.Customer.Contacts[0].City = field.Value;
                            break;
                        case ELpFormFieldType.Address:
                            booking.Customer.Contacts[0].Street = field.Value;
                            break;
                        case ELpFormFieldType.Birthday:
                            booking.Customer.BirthDay = field.Value.TryParseDateTime();
                            break;
                        case ELpFormFieldType.Picture:
                            if (_files != null && _files.Count > 0)
                            {
                                //var siteUrl = UrlService.GetUrl();

                                foreach (var key in _files.AllKeys)
                                {
                                    if (!key.StartsWith("form[files][" + index + "]"))
                                        continue;
                                    var file = _files[key];
                                    // add to attachments
                                    if (!FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image))
                                        continue;

                                    files.Add(file);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    var customerField = CustomerFieldService.GetCustomerFieldsWithValue(field.ObjId.Value);
                    if (customerField != null)
                    {
                        if (customerField.FieldType == CustomerFieldType.Select)
                        {
                            if (customerField.Values.Find(x => x.Value == field.Value) == null)
                            {
                                index++;
                                continue;
                            }
                        }
                        customerField.Value = field.Value;
                        customerFields.Add(customerField);
                    }
                }

                index++;
            }

            booking.BookingItems = _selectedServices.Select(x => new BookingItem
            {
                ServiceId = x.Id,
                ArtNo = x.ArtNo,
                Name = x.Name,
                Amount = 1,
                Price = x.RoundedPrice
            }).ToList();

            booking.BookingCurrency = CurrencyService.CurrentCurrency;

            booking.Sum =
                booking.BookingItems.Sum(item => item.Price * item.Amount)
                    .RoundPrice(booking.BookingCurrency.CurrencyValue, booking.BookingCurrency);

            booking.Id = BookingService.Add(booking);

            BizProcessExecuter.BookingAdded(booking);

            if (customerFields.Count > 0 && booking.Customer.Id != Guid.Empty)
            {
                foreach (var field in customerFields)
                    CustomerFieldService.AddUpdateMap(booking.Customer.Id, field.Id, field.Value.DefaultOrEmpty());
            }

            if (files.Count > 0)
            {
                new UploadAttachmentsHandler(booking.Id).Execute<BookingAttachment>(files);
            }

            var model = new SubmitFormReturnModel() { PostAction = _form.PostAction.ToString() };

            switch (_form.PostAction)
            {
                case FormPostAction.ShowMessage:
                    model.Message = _form.PostMessageText;
                    break;

                case FormPostAction.RedrectToUrl:
                    {
                        model.RedirectUrl = (_form.PostMessageRedirectLpId.IsNotEmpty()
                            ? _lpService.GetLpLink(_form.PostMessageRedirectLpId.TryParseInt())
                            : _form.PostMessageRedirectUrl).DefaultOrEmpty();

                        if (_model.Lid != null)
                            model.RedirectUrl += (!model.RedirectUrl.Contains('?') ? "?" : "&") + "lid=" + _model.Lid;

                        model.RedirectUrl += (!model.RedirectUrl.Contains('?') ? "?" : "&") + "bid=" + booking.Id;
                        break;
                    }

                case FormPostAction.RedrectToUrlAndEmail:
                    {
                        model.RedirectUrl = (_form.PostMessageRedirectLpId.IsNotEmpty()
                            ? _lpService.GetLpLink(_form.PostMessageRedirectLpId.TryParseInt())
                            : _form.PostMessageRedirectUrl).DefaultOrEmpty();

                        if (_model.Lid != null)
                            model.RedirectUrl += (!model.RedirectUrl.Contains('?') ? "?" : "&") + "lid=" + _model.Lid;

                        model.RedirectUrl += (!model.RedirectUrl.Contains('?') ? "?" : "&") + "bid=" + booking.Id;

                        if (!string.IsNullOrWhiteSpace(_form.EmailText) && !string.IsNullOrEmpty(booking.Customer.EMail))
                            MailService.SendMailNow(booking.Customer.Id, booking.Customer.EMail, _form.EmailSubject.DefaultOrEmpty(), _form.EmailText, true);
                        break;
                    }
                case FormPostAction.RedirectToCheckout:
                    {
                        var order = Orders.OrderService.CreateOrder(BookingService.Get(booking.Id));

                        if (order != null)
                        {
                            var hash = Orders.OrderService.GetBillingLinkHash(order);
                            var billingLink = Core.UrlRewriter.UrlService.GetUrl("checkout/billing?code=" + order.Code + "&hash=" + hash);

                            model.RedirectUrl = billingLink;

                            if (_model.Lid != null)
                                model.RedirectUrl += (!model.RedirectUrl.Contains('?') ? "?" : "&") + "lid=" + _model.Lid;

                            model.RedirectUrl += (!model.RedirectUrl.Contains('?') ? "?" : "&") + "bid=" + booking.Id;
                        }

                        break;
                    }
            }

            return model;
        }

        private bool CheckTimeIsWork()
        {
            var affiliateAdditionalTime = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(_model.AffiliateId, _model.BeginDate.Date, _model.EndDate.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            var affiliateTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                AffiliateTimeOfBookingService.GetByAffiliate(_model.AffiliateId)
                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            var reservationResourceAdditionalTime = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                ReservationResourceAdditionalTimeService.GetByDateFromTo(_model.AffiliateId, _model.ResourceId, _model.BeginDate.Date, _model.EndDate.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            var reservationResourceTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                ReservationResourceTimeOfBookingService.GetBy(_model.AffiliateId, _model.ResourceId)
                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            return ReservationResourceService.CheckDateRangeIsWork(_model.BeginDate, _model.EndDate,
                    reservationResourceAdditionalTime,
                    reservationResourceTimesOfBookingDayOfWeek,
                    affiliateAdditionalTime,
                    affiliateTimesOfBookingDayOfWeek);
        }
    }
}
