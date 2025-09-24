using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Handlers.Shared.Sms;
using AdvantShop.Web.Admin.Models.Shared.Smses;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Letters;
using AdvantShop.Core.Services.Bonuses;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public partial class SmsController : BaseAdminController
    {
        public JsonResult GetSmsModuleEnabled()
        {
            var smsModule = SmsNotifier.GetActiveSmsModule();

            return Json(new {result = smsModule != null});
        }

        public JsonResult SendSms(SendSmsModel model)
        {
            return ProcessJsonResult(new SendSmsHandler(model));
        }

        [HttpGet]
        public JsonResult GetSmsToCustomer(GetSmsToCustomerModel model)
        {
            var text = "";
            
            if (model.TemplateId != -1)
            {
                var template = new SmsAnswerTemplateService().Get(model.TemplateId, false);
                if (template != null)
                {
                    text = template.Text;

                    if (model.OrderId.HasValue)
                    {
                        var order = OrderService.GetOrder(model.OrderId.Value);
                        if (order != null)
                            text = new OrderLetterBuilder(order).FormatText(text);
                    }
                    else if (model.LeadId.HasValue)
                    {
                        var lead = LeadService.GetLead(model.LeadId.Value);
                        if (lead != null)
                            text = new LeadLetterBuilder(lead).FormatText(text);
                    }
                    else if (model.CustomerId.HasValue)
                    {
                        var customer = CustomerService.GetCustomer(model.CustomerId.Value);
                        if (customer != null)
                            text = new CustomerLetterBuilder(customer).FormatText(text);
                    }
                }
            }

            var userNotAgree = false;

            if (!model.IsMassSending && SettingsDesign.ShowUserAgreementForPromotionalNewsletter)
            {
                var customerId = default(Guid?);

                if (model.CustomerId.HasValue)
                    customerId = model.CustomerId.Value;
                else if (model.OrderId.HasValue)
                    customerId = OrderService.GetOrderCustomer(model.OrderId.Value)?.CustomerID;
                else if (model.LeadId.HasValue)
                    customerId = LeadService.GetLead(model.LeadId.Value)?.CustomerId;

                if (customerId != null)
                {
                    var customer = CustomerService.GetCustomer(customerId.Value);
                    if (customer != null)
                        userNotAgree = !customer.IsAgreeForPromotionalNewsletter;
                }
            }

            var module = SmsNotifier.GetActiveSmsModule();
            var isSmsAndSocialMediaActive = module != null && module is ISmsAndSocialMediaService;
            
            return Json(new
            {
                text, 
                userNotAgree, 
                isSmsAndSocialMediaActive,
                customerKeys = BonusSystem.IsActive
                    ? LetterBuilderHelper.GetLetterFormatKeysHtml<CustomerLetterTemplateKey>()
                    : LetterBuilderHelper.GetLetterFormatKeysHtml(LetterBuilderHelper.GetLetterFormatKeys(Enum.GetValues(typeof(CustomerLetterTemplateKey))
                    .Cast<CustomerLetterTemplateKey>()
                    .Where(x => x != CustomerLetterTemplateKey.BonusBalance)
                    .ToList())),
                orderKeys = LetterBuilderHelper.GetLetterFormatKeysHtml<OrderLetterTemplateKey>(),
                leadKeys = LetterBuilderHelper.GetLetterFormatKeysHtml<LeadLetterTemplateKey>(),
            });
        }

        public JsonResult GetAnswerTemplates()
        {
            var templates = new List<SmsAnswerTemplate>
            {
                new SmsAnswerTemplate { TemplateId = -1, Name = T("Admin.Customers.Empty") }
            };

            templates.AddRange(new SmsAnswerTemplateService().Gets(true));
            return JsonOk(templates);
        }

        [HttpGet]
        public JsonResult GetTemplates(BaseFilterModel filter)
        {
            var module = SmsNotifier.GetActiveSmsModule() as ISmsAndSocialMediaService;
            if (module == null)
                return Json(null);

            var list = module.GetTemplates();
            var totalCount = list.Count;
            
            if (!string.IsNullOrEmpty(filter.Search))
                list = list.Where(x => x.Text != null && x.Text.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)).ToList();
            
            var model = new FilterResult<ISmsTemplate>
            {
                DataItems = 
                    list.Skip((filter.Page - 1) * filter.ItemsPerPage)
                        .Take(filter.ItemsPerPage)
                        .ToList(),
                TotalItemsCount = totalCount,
                TotalPageCount = (int) Math.Ceiling((double) totalCount / filter.ItemsPerPage),
                TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FieldTotal", totalCount)
            };

            return Json(model);
        }
    }
}

