using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Letters;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Shared.Smses;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Shared.Sms
{
    public class SendSmsHandler : AbstractCommandHandler
    {
        private readonly SendSmsModel _model;

        public SendSmsHandler(SendSmsModel model)
        {
            _model = model;
        }
        
        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_model.Text))
                throw new BlException("Укажите текст sms");
            
            var smsModule = SmsNotifier.GetActiveSmsModule();
            if (smsModule == null)
                throw new BlException("Нет активного sms модуля");

            if (_model.ModuleTemplateId != null && !(smsModule is ISmsAndSocialMediaService))
                throw new BlException("Модуль не поддерживает отправку шаблона");
        }

        protected override void Handle()
        {
            var recipients = GetSmsRecipients();

            var order = _model.OrderId != null ? OrderService.GetOrder(_model.OrderId.Value) : null;
            var lead = _model.LeadId != null ? LeadService.GetLead(_model.LeadId.Value) : null;

            var errors = new List<string>();

            foreach (var item in recipients)
            {
                try
                {
                    var smsText = _model.Text;
                    var templateId = _model.ModuleTemplateId;
                    Dictionary<string, string> templateParameters = null;
                    
                    if (order != null)
                    {
                        smsText = GetFormattedSmsAndParameters(smsText, order, templateId, templateParameters);
                    }
                    else if (lead != null)
                    {
                        smsText = GetFormattedSmsAndParameters(smsText, lead, templateId, templateParameters);
                    }
                    
                    smsText = GetFormattedSmsAndParameters(smsText, item.Customer, templateId, templateParameters);
                    
                    SmsNotifier.SendSms(item.Phone.Value, smsText,
                        item.CustomerId ?? Guid.Empty,
                        _model.ThrowError ?? false,
                        inBackground: false,
                        isInternal: true,
                        templateId: templateId, templateParameters: templateParameters);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            TrackEvent();

            if (errors.Any())
                throw new BlException(string.Join(", ", errors));
        }

        private List<SmsRecipientInfo> GetSmsRecipients()
        {
            var recipients = new List<SmsRecipientInfo>();

            if (_model.CustomerId.HasValue)
            {
                var c = CustomerService.GetCustomer(_model.CustomerId.Value);
                recipients.Add(new SmsRecipientInfo
                {
                    Phone = StringHelper.ConvertToStandardPhone(_model.Phone, true, true) ?? c?.StandardPhone,
                    CustomerId = c?.Id,
                    Customer = c
                });
            }
            else if (!string.IsNullOrWhiteSpace(_model.Phone))
            {
                recipients.Add(new SmsRecipientInfo
                {
                    Phone = StringHelper.ConvertToStandardPhone(_model.Phone, true, true)
                });
            }
            else if (_model.CustomerIds != null)
            {
                foreach (var customerId in _model.CustomerIds)
                {
                    var c = CustomerService.GetCustomer(customerId);
                    
                    if (c != null && (c.IsAgreeForPromotionalNewsletter || !SettingsDesign.ShowUserAgreementForPromotionalNewsletter))
                        recipients.Add(new SmsRecipientInfo(c));
                }
            }
            else if (_model.SubscriptionIds != null)
            {
                foreach (var id in _model.SubscriptionIds)
                {
                    var s = SubscriptionService.GetSubscriptionExt(id);
                    if (s != null)
                        recipients.Add(new SmsRecipientInfo(s));
                }
            }
            else if (_model.Recipients != null)
            {
                foreach (var recipient in _model.Recipients)
                    recipients.Add((SmsRecipientInfo) recipient);
            }

            var result = recipients.Where(x => x.Phone != null).Distinct(new SendSmsModelItemComparer()).ToList();

            return result;
        }

        private void TrackEvent()
        {
            switch (_model.PageType)
            {
                case "order":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_SendSmsToCustomer);
                    break;
                case "customer":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_SendSmsToCustomer);
                    break;
                case "customerSegment":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Customers_BulkSmsSendingBySegment);
                    break;
                case "leads":
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_BulkSmsSending);
                    break;
            }
        }
        
        
        private string GetFormattedSmsAndParameters<TEntity>(string smsText, TEntity entity, int? templateId, Dictionary<string, string> templateParameters)
        {
            if (entity == null)
                return smsText;
            
            var builder = LetterBuilderHelper.Create(entity);

            var templateParametersByEntity = 
                templateId != null 
                    ? builder.GetFormattedParams(smsText) 
                    : null;

            if (templateParametersByEntity != null && templateParametersByEntity.Count > 0)
            {
                if (templateParameters == null)
                    templateParameters = new Dictionary<string, string>();

                foreach (var item in templateParametersByEntity)
                    if (!templateParameters.ContainsKey(item.Key))
                        templateParameters.Add(item.Key, item.Value);
            }
                        
            var sms = builder.FormatText(smsText);

            return sms;
        }
    }
}