using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.App.Landing.Models.Landing;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Landing.LandingEmails;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking
{
    public class UpdateBookingCustomerHandler : AbstractCommandHandler<SubmitFormReturnModel>
    {
        #region Ctor

        private readonly SubmitFormModel _model;
        private readonly LpFormService _lpFormService;
        private readonly LpService _lpService;
        
        private LpForm _form;
        private Core.Services.Booking.Booking _booking;

        public UpdateBookingCustomerHandler(SubmitFormModel model)
        {
            _model = model;
            _lpFormService = new LpFormService();
            _lpService = new LpService();
        }
        
        #endregion

        protected override void Load()
        {
            _form = _lpFormService.Get(_model.Id);
            _booking = _model.EntityId != null ? BookingService.Get(_model.EntityId.Value) : null;
        }

        protected override void Validate()
        {
            if (_form == null)
                throw new BlException("Форма не найдена");

            if (_model.EntityId == null || _booking == null)
                throw new BlException("Booking не найден");
        }

        protected override SubmitFormReturnModel Handle()
        {
            var customerFields = new List<CustomerFieldWithValue>();
            foreach (var item in _model.Fields)
            {
                var field = item.Value;
                field.Value = HttpUtility.HtmlEncode(field.Value.DefaultOrEmpty());

                if (field.ObjId == null)
                {
                    var type = (ELpFormFieldType)field.Type;

                    switch (type)
                    {
                        case ELpFormFieldType.FirstName:
                            _booking.FirstName = field.Value;
                            _booking.Customer.FirstName = field.Value;
                            break;
                        case ELpFormFieldType.LastName:
                            _booking.LastName = field.Value;
                            _booking.Customer.LastName = field.Value;
                            break;
                        case ELpFormFieldType.Patronymic:
                            _booking.Patronymic = field.Value;
                            _booking.Customer.Patronymic = field.Value;
                            break;
                        case ELpFormFieldType.Email:
                            _booking.Email = field.Value;
                            _booking.Customer.EMail = field.Value;
                            break;
                        case ELpFormFieldType.Phone:
                            _booking.Phone = field.Value;
                            _booking.Customer.Phone = field.Value;
                            _booking.StandardPhone = _booking.Customer.StandardPhone = StringHelper.ConvertToStandardPhone(field.Value);
                            break;
                        case ELpFormFieldType.Comment:
                        case ELpFormFieldType.TextArea:
                            break;
                        case ELpFormFieldType.Country:
                            _booking.Customer.Contacts[0].Country = field.Value;
                            break;
                        case ELpFormFieldType.Region:
                            _booking.Customer.Contacts[0].Region = field.Value;
                            break;
                        case ELpFormFieldType.City:
                            _booking.Customer.Contacts[0].City = field.Value;
                            break;
                        case ELpFormFieldType.Address:
                            _booking.Customer.Contacts[0].Street = field.Value;
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
                                continue;
                        }
                        customerField.Value = field.Value;
                        customerFields.Add(customerField);
                    }
                }
            }

            if (customerFields.Count > 0 && _booking.Customer.Id != Guid.Empty)
            {
                foreach (var field in customerFields)
                    CustomerFieldService.AddUpdateMap(_booking.Customer.Id, field.Id, field.Value.DefaultOrEmpty());
            }

            BookingService.Update(_booking);
            

            var model = new SubmitFormReturnModel() { PostAction = _form.PostAction.ToString() };

            switch (_form.PostAction)
            {
                case FormPostAction.ShowMessage:
                    model.Message = _form.PostMessageText;
                    break;

                case FormPostAction.RedrectToUrl:
                    {
                        var url = !string.IsNullOrEmpty(_form.PostMessageRedirectLpId) ? _lpService.GetLpLink(_form.PostMessageRedirectLpId.TryParseInt()) : _form.PostMessageRedirectUrl;
                        url = url ?? "";

                        model.RedirectUrl = url + ((_form.DontSendLeadId ?? false) ? string.Empty : string.Format("{0}lid={1}", !url.Contains('?') ? "?" : "&", _form.Id));
                        break;
                    }

                case FormPostAction.RedrectToUrlAndEmail:
                    {
                        var url = !string.IsNullOrEmpty(_form.PostMessageRedirectLpId) ? _lpService.GetLpLink(_form.PostMessageRedirectLpId.TryParseInt()) : _form.PostMessageRedirectUrl;
                        url = url ?? "";

                        model.RedirectUrl = url + ((_form.DontSendLeadId ?? false) ? string.Empty : string.Format("{0}lid={1}", !url.Contains('?') ? "?" : "&", _form.Id));

                        if (!string.IsNullOrEmpty(_booking.Customer.EMail))
                        {
                            if (!string.IsNullOrWhiteSpace(_form.EmailText))
                                MailService.SendMailNow(_booking.Customer.Id, _booking.Customer.EMail, _form.EmailSubject ?? "", _form.EmailText, true);

                            var deferredEmailService = new LandingDeferredEmailService();
                            var templates = new LandingEmailTemplateService().GetList(_form.BlockId.Value);

                            foreach (var template in templates)
                            {
                                deferredEmailService.Add(new LandingDeferredEmail()
                                {
                                    CustomerId = _booking.Customer.Id,
                                    Email = _booking.Customer.EMail,
                                    Subject = template.Subject,
                                    Body = template.Body,
                                    SendingDate = DateTime.Now.AddMinutes(template.SendingTime)
                                });
                            }
                        }
                        break;
                    }
            }

            return model;
        }
    }
}
