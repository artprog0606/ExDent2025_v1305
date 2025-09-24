using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.App.Landing.Handlers.Pictures;
using AdvantShop.App.Landing.Models.Landing;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Landing.LandingEmails;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Landings
{
    public class SubmitForm : AbstractCommandHandler<SubmitFormReturnModel>
    {
        #region Ctor

        private readonly SubmitFormModel _model;
        private readonly HttpFileCollection _files;
        private readonly LpFormService _formService;
        private readonly LpService _lpService;
        private readonly LpBlockService _blockService;

        private LpForm _form;
        private Lp _lp;
        private LpSite _site;

        public SubmitForm(SubmitFormModel model, HttpFileCollection files)
        {
            _model = model;
            _files = files;
            _formService = new LpFormService();
            _lpService = new LpService();
            _blockService = new LpBlockService();
        }

        #endregion

        protected override void Load()
        {
            _form = _formService.Get(_model.Id);
            if (_form != null)
                _lp = _lpService.Get(_form.LpId);
            if (_lp != null)
                _site = new LpSiteService().Get(_lp.LandingSiteId);
        }

        protected override void Validate()
        {
            if (_form == null)
                throw new BlException("Форма не найдена");
        }

        protected override SubmitFormReturnModel Handle()
        {
            if (_form.PostAction == FormPostAction.RedirectToCheckout)
                return CreateOrder();

            return CreateLead();
        }

        private SubmitFormReturnModel CreateLead()
        {
            Lead lead = null;

            if (_model.EntityId != null && _model.EntityType == "lead")
            {
                lead = LeadService.GetLead(_model.EntityId.Value);

                if (lead.Customer == null)
                    lead.Customer = new Customer();

                if (lead.Customer.Contacts == null || lead.Customer.Contacts.Count == 0)
                    lead.Customer.Contacts = new List<CustomerContact>() { new CustomerContact() };
            }

            if (lead == null)
            {
                lead = new Lead()
                {
                    Customer = new Customer() {Contacts = new List<CustomerContact>() {new CustomerContact()}},
                    CustomerId = CustomerContext.CurrentCustomer.RegistredUser
                        ? CustomerContext.CustomerId
                        : default(Guid?),
                    LeadItems = new List<LeadItem>()
                };

                if (_model.CustomFields != null)
                {
                    lead.Description += String.Join("; ", _model.CustomFields.Select(x => string.Format("{0}: {1}", x.Key, x.Value)));
                    lead.Description += !string.IsNullOrEmpty(lead.Description) ? ". " : "";
                }
                lead.Description += "Лид из лендинга ";

                if (_lp != null)
                {
                    lead.Description += (_site != null ? "\"" + _site.Name + "\" " : " ") +
                                        _lpService.GetLpLinkByMain(_lp).Replace("http://", "").Replace("https://", "");
                }
                
                var source = _site == null
                    ? OrderSourceService.GetOrderSource(OrderType.LandingPage)
                    : OrderSourceService.GetOrderSource(OrderType.LandingPage, _site.Id, _site.Name);

                lead.OrderSourceId = source.Id;
                lead.Title = lead.Description;

                if (_form.SalesFunnelId != null && SalesFunnelService.GetList().Any(x => x.Id == _form.SalesFunnelId.Value))
                    lead.SalesFunnelId = _form.SalesFunnelId.Value;
            }

            var descrSb = new StringBuilder();
            var customerFields = new List<CustomerFieldWithValue>();
            var leadFields = new List<LeadFieldWithValue>();
            var files = new List<HttpPostedFile>();

            int index = 0;
            foreach (var item in _model.Fields)
            {
                var field = item.Value;
                field.Value = field.Value ?? "";

                var type = (ELpFormFieldType)field.Type;
                if (field.ObjId == null)
                {
                    switch (type)
                    {
                        case ELpFormFieldType.FirstName:
                            lead.FirstName = field.Value;
                            lead.Customer.FirstName = field.Value;
                            break;

                        case ELpFormFieldType.LastName:
                            lead.LastName = field.Value;
                            lead.Customer.LastName = field.Value;
                            break;

                        case ELpFormFieldType.Patronymic:
                            lead.Patronymic = field.Value;
                            lead.Customer.Patronymic = field.Value;
                            break;

                        case ELpFormFieldType.Email:
                            lead.Email = field.Value;
                            lead.Customer.EMail = field.Value;
                            break;

                        case ELpFormFieldType.Phone:
                            lead.Phone = field.Value;
                            lead.Customer.Phone = field.Value;
                            lead.Customer.StandardPhone = Helpers.StringHelper.ConvertToStandardPhone(field.Value, true, true);
                            break;

                        case ELpFormFieldType.Comment:
                        case ELpFormFieldType.TextArea:
                            lead.Comment = field.Value;
                            break;

                        case ELpFormFieldType.Country:
                            lead.Customer.Contacts[0].Country = field.Value;
                            break;

                        case ELpFormFieldType.Region:
                            lead.Customer.Contacts[0].Region = field.Value;
                            break;

                        case ELpFormFieldType.City:
                            lead.Customer.Contacts[0].City = field.Value;
                            break;

                        case ELpFormFieldType.Address:
                            lead.Customer.Contacts[0].Street = field.Value;
                            break;

                        case ELpFormFieldType.Picture:
                        case ELpFormFieldType.FileArchive:
                            if (_files != null && _files.Count > 0)
                            {
                                var currentIndex = 0;
                                foreach (var key in _files.AllKeys)
                                {
                                    if (!key.StartsWith("files[" + index + "]"))
                                        continue;
                                    
                                    if (currentIndex == 10) // max count 10
                                        break;
                                    
                                    var file = _files[key];
                                    
                                    var allowedTypes = type == ELpFormFieldType.Picture
                                        ? EFileType.Image
                                        : EFileType.Archive | EFileType.Document;
                                    
                                    if (!FileHelpers.CheckFileExtensionByType(file.FileName, allowedTypes) || 
                                        file.ContentLength > 15*1024*1024) // 15mb
                                        continue;

                                    files.Add(file); // add to attachments
                                    currentIndex++;
                                }
                            }
                            break;

                        case ELpFormFieldType.Birthday:
                            lead.Customer.BirthDay = field.Value.TryParseDateTime();
                            break;
                    }
                    descrSb.AppendFormat("{0}: {1} \r\n", field.FieldName.IsNotEmpty() ? field.FieldName : type.Localize(), field.Value);
                }
                else if (type == ELpFormFieldType.CustomerField)
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
                        descrSb.AppendFormat("{0}: {1} \r\n", customerField.Name, field.Value);
                    }
                }
                else if (type == ELpFormFieldType.LeadField)
                {
                    var leadField = LeadFieldService.GetLeadFieldWithValue(field.ObjId.Value);
                    if (leadField != null)
                    {
                        if (leadField.FieldType == LeadFieldType.Select && !leadField.Values.Any(x => x.Value == field.Value))
                        {
                            index++;
                            continue;
                        }

                        leadField.Value = field.Value;

                        leadFields.Add(leadField);
                        descrSb.AppendFormat("{0}: {1} \r\n", leadField.Name, field.Value);
                    }
                }
                index++;
            }

            if (!string.IsNullOrEmpty(_model.ButtonTitle))
            {
                descrSb.AppendFormat("\r\n{0}\r\n", _model.ButtonTitle);
            }
            else
            {
                descrSb.AppendFormat("\r\nКнопка: {0}\r\n", _form.ButtonText);
            }

            lead.Description += " \r\n" + descrSb.ToString();

            var block = _model.BlockId != null ? _blockService.Get(_model.BlockId.Value) : null;
            var button = block != null ? block.TryGetSetting<LpButton>("button") : null;

            if (_model.OfferId != null || (_model.OfferIds != null && _model.OfferIds.Count > 0))
            {
                var offerIds = _model.OfferIds != null && _model.OfferIds.Count > 0
                    ? _model.OfferIds
                    : new List<int>() {_model.OfferId.Value};

                foreach (var offerId in offerIds)
                    AddLeadItem(offerId, lead, button);
            }
            else if (_model.BlockId != null)
            {
                if (button != null && button.ActionOfferItems != null && button.ActionOfferItems.Count > 0)
                {
                    foreach (var offerItem in button.ActionOfferItems)
                    {
                        var offer = OfferService.GetOffer(offerItem.OfferId.TryParseInt());
                        if (offer != null)
                            lead.LeadItems.Add(new LeadItem(offer, offerItem.OfferAmount.TryParseFloat(1), offerItem.OfferPrice.TryParseFloat(true)));
                    }

                    var shippingPrice = button.ActionShippingPrice.TryParseFloat(true);
                    if (shippingPrice != null)
                    {
                        lead.ShippingName = LocalizationService.GetResource("Lead.LaningShippingName");
                        lead.ShippingCost = shippingPrice.Value;
                    }
                }
            }

            if (lead.LeadItems != null && lead.LeadItems.Count > 0)
                lead.Sum = lead.LeadItems.Sum(x => x.Amount*x.Price);

            if (lead.Id == 0)
            {
                LeadService.AddLead(lead, trackChanges: false, customerFields: customerFields, leadFields: leadFields);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_SalesFunnel);
            }
            else
                LeadService.UpdateLead(lead, trackChanges: false, customerFields: customerFields, leadFields: leadFields);

            if (files.Count > 0)
            {
                new UploadAttachmentsHandler(lead.Id).Execute<LeadAttachment>(files);
            }
            
            var model = new SubmitFormReturnModel() { PostAction = _form.PostAction.ToString() };

            switch (_form.PostAction)
            {
                case FormPostAction.ShowMessage:
                    model.Message = _form.PostMessageText;
                    break;

                case FormPostAction.RedrectToUrl:
                {
                    var url = !string.IsNullOrEmpty(_form.PostMessageRedirectLpId)
                        ? _lpService.GetLpLink(_form.PostMessageRedirectLpId.TryParseInt())
                        : _form.PostMessageRedirectUrl;
                    url = url ?? (_lp != null ?_lpService.GetLpLink(_lp.Id) : "");

                    model.RedirectUrl = url + ((_form.DontSendLeadId ?? false) ? string.Empty : string.Format("{0}lid={1}", !url.Contains('?') ? "?" : "&", lead.Id));

                    if (_form.PostMessageRedirectShowMessage != null && _form.PostMessageRedirectShowMessage.Value)
                    {
                        model.Message = _form.PostMessageText;
                        model.RedirectDelay = _form.PostMessageRedirectDelay ?? 0;
                    }
                    break;
                }

                case FormPostAction.RedrectToUrlAndEmail:
                {
                    var url = !string.IsNullOrEmpty(_form.PostMessageRedirectLpId)
                        ? _lpService.GetLpLink(_form.PostMessageRedirectLpId.TryParseInt())
                        : _form.PostMessageRedirectUrl;
                    url = url ?? (_lp != null ?_lpService.GetLpLink(_lp.Id) : "");

                    model.RedirectUrl = url + ((_form.DontSendLeadId ?? false) ? string.Empty : string.Format("{0}lid={1}", !url.Contains('?') ? "?" : "&", lead.Id));

                    if (_form.PostMessageRedirectShowMessage != null && _form.PostMessageRedirectShowMessage.Value)
                    {
                        model.Message = _form.PostMessageText;
                        model.RedirectDelay = _form.PostMessageRedirectDelay ?? 0;
                    }

                    if (!string.IsNullOrEmpty(lead.Customer.EMail))
                    {
                        var mailTemplate = new LeadMailTemplate(lead);

                        if (!string.IsNullOrWhiteSpace(_form.EmailText))
                        {
                            var subject = mailTemplate.FormatValue(_form.EmailSubject);
                            var body = mailTemplate.FormatValue(_form.EmailText);

                            MailService.SendMailNow(lead.Customer.Id, lead.Customer.EMail, subject, body, true);
                        }

                        var deferredEmailService = new LandingDeferredEmailService();
                        var templates = new LandingEmailTemplateService().GetList(_form.BlockId.Value);

                        foreach (var template in templates)
                        {
                            deferredEmailService.Add(new LandingDeferredEmail()
                            {
                                CustomerId = lead.Customer.Id,
                                Email = lead.Customer.EMail,
                                Subject = mailTemplate.FormatValue(template.Subject),
                                Body = mailTemplate.FormatValue(template.Body),
                                SendingDate = DateTime.Now.AddMinutes(template.SendingTime)
                            });
                        }
                    }
                    break;
                }

                case FormPostAction.AddToCartAndRedrectToUrl:
                {
                    ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

                    if (button != null)
                    {
                        var offerItems =
                            button.UseManyOffers != null && button.UseManyOffers.Value &&
                            button.ActionOfferItems != null && button.ActionOfferItems.Count > 0
                                ? button.ActionOfferItems
                                : (button.ActionOfferId != null
                                    ? new List<LpButtonOfferItem>() {new LpButtonOfferItem() {OfferId = button.ActionOfferId, OfferPrice = button.ActionOfferPrice}}
                                    : new List<LpButtonOfferItem>());

                        foreach (var offerItem in offerItems)
                        {
                            var offer = OfferService.GetOffer(offerItem.OfferId.TryParseInt());
                            if (offer == null)
                                continue;

                            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem()
                            {
                                OfferId = offer.OfferId,
                                Amount = offerItem.OfferAmount.TryParseFloat(1),
                                CustomPrice = offerItem.OfferPrice.TryParseFloat(true)
                            });
                        }
                    }
                    else if (_form.OfferItems != null && _form.OfferItems.Count > 0)
                    {
                        foreach (var offerItem in _form.OfferItems)
                        {
                            var offer = OfferService.GetOffer(offerItem.OfferId.TryParseInt());
                            if (offer == null)
                                continue;

                            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem()
                            {
                                OfferId = offer.OfferId,
                                Amount = offerItem.OfferAmount.TryParseFloat(1),
                                CustomPrice = offerItem.OfferPrice.TryParseFloat(true)
                            });
                        }
                    }

                    var actionUpsellLpId = "";

                    if (button != null && !string.IsNullOrEmpty(button.ActionUpsellLpId))
                        actionUpsellLpId = button.ActionUpsellLpId;
                    else if (_form.ActionUpsellLpId != null)
                        actionUpsellLpId = _form.ActionUpsellLpId.Value.ToString();

                    model.RedirectUrl =
                        UrlService.GetUrl("checkout/lp?lpid=" + _lp.Id +
                                          (!string.IsNullOrEmpty(actionUpsellLpId) ? "&lpupid=" + actionUpsellLpId : ""));
                    break;
                }
            }

            return model;
        }

        private SubmitFormReturnModel CreateOrder()
        {
            var order = new Order()
            {
                OrderCustomer = new OrderCustomer(),
                OrderItems = new List<OrderItem>()
            };

            var source = _site == null
                ? OrderSourceService.GetOrderSource(OrderType.LandingPage)
                : OrderSourceService.GetOrderSource(OrderType.LandingPage, _site.Id, _site.Name);
            order.OrderSourceId = source.Id;

            var descrSb = new StringBuilder();
            descrSb.AppendFormat("Лид из лендинга {0}<br/>", _lp != null ? _lp.Name + ". " + UrlService.GetUrl("lp/" + _lp.Url) : "");
            var customerFields = new List<CustomerFieldWithValue>();

            foreach (var item in _model.Fields)
            {
                var field = item.Value;
                field.Value = field.Value != null ? HttpUtility.HtmlEncode(field.Value) : "";

                if (field.ObjId == null)
                {
                    var type = (ELpFormFieldType)field.Type;

                    switch (type)
                    {
                        case ELpFormFieldType.FirstName:
                            order.OrderCustomer.FirstName = field.Value;
                            break;

                        case ELpFormFieldType.LastName:
                            order.OrderCustomer.LastName = field.Value;
                            break;

                        case ELpFormFieldType.Patronymic:
                            order.OrderCustomer.Patronymic = field.Value;
                            break;

                        case ELpFormFieldType.Email:
                            order.OrderCustomer.Email = field.Value;
                            break;

                        case ELpFormFieldType.Phone:
                            order.OrderCustomer.Phone = field.Value;
                            break;

                        case ELpFormFieldType.Comment:
                        case ELpFormFieldType.TextArea:
                            order.CustomerComment = field.Value;
                            break;

                        case ELpFormFieldType.Country:
                            order.OrderCustomer.Country = field.Value;
                            break;

                        case ELpFormFieldType.Region:
                            order.OrderCustomer.Region = field.Value;
                            break;

                        case ELpFormFieldType.City:
                            order.OrderCustomer.City = field.Value;
                            break;

                        case ELpFormFieldType.Address:
                            order.OrderCustomer.Street = field.Value;
                            break;
                    }
                    descrSb.AppendFormat("{0}: {1}<br/>", type.Localize(), field.Value);
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
                        descrSb.AppendFormat("{0}: {1}<br/>", customerField.Name, field.Value);
                    }
                }
            }

            descrSb.AppendFormat("<br/>Кнопка: {0}<br/>", _form.ButtonText);

            order.AdminOrderComment = descrSb.ToString();

            order.OrderID = OrderService.AddOrder(order);
            
            OrderStatusService.ChangeOrderStatusForNewOrder(order.OrderID);
            
            if (order.OrderID != 0)
            {
                OrderMailService.SendMail(order);

                if (customerFields.Count > 0)
                {
                    var customer = OrderService.GetOrderCustomer(order.OrderID);
                    if (customer != null)
                    {
                        foreach (var field in customerFields)
                            CustomerFieldService.AddUpdateMap(customer.CustomerID, field.Id, field.Value ?? "");
                    }
                }
            }
            
            var model = new SubmitFormReturnModel() { PostAction = _form.PostAction.ToString() };

            return model;
        }


        private void AddLeadItem(int offerId, Lead lead, LpButton button)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return;

            if (_model.ColorId != null)
            {
                var offerByColor = OfferService.GetProductOffers(offer.ProductId).OrderByDescending(x => x.Main).FirstOrDefault(x => x.ColorID == _model.ColorId);
                if (offerByColor != null)
                    offer = offerByColor;
            }

            float? actionOfferPrice = null;
            float amount = 1;

            if (button != null)
            {
                if (!string.IsNullOrEmpty(button.ActionOfferPrice))
                    actionOfferPrice = button.ActionOfferPrice.TryParseFloat();

                var shippingPrice = button.ActionShippingPrice.TryParseFloat(true);
                if (shippingPrice != null)
                {
                    lead.ShippingName = LocalizationService.GetResource("Lead.LaningShippingName");
                    lead.ShippingCost = shippingPrice.Value;
                }
            }
            else
            {
                var offerItem = _form.OfferItems?.Find(x => x.OfferId == offerId.ToString());
                if (offerItem != null)
                {
                    if (!string.IsNullOrEmpty(offerItem.OfferPrice))
                        actionOfferPrice = offerItem.OfferPrice.TryParseFloat(true);

                    if (!string.IsNullOrEmpty(offerItem.OfferAmount))
                    {
                        var offerAmount = offerItem.OfferAmount.TryParseFloat();
                        if (offerAmount >= 0)
                            amount = offerItem.OfferAmount.TryParseFloat();
                    }
                }
            }

            if (_model.Amount != null && _model.Amount.Value > 0)
                amount = _model.Amount.Value;

            lead.LeadItems.Add(new LeadItem(offer, amount, actionOfferPrice));
        }
    }
}
