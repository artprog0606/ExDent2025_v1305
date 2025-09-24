using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Orders;

namespace AdvantShop.Letters
{
    public sealed class LeadLetterBuilder : BaseLetterTemplateBuilder<Lead, LeadLetterTemplateKey>
    {
        private const string RowFormat = "<div class='l-row'><div class='l-name vi cs-light' style='color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;'>{0}:</div><div class='l-value vi' style='display: inline-block; margin: 5px 0;'>{1}</div></div>";
        
        public LeadLetterBuilder(Lead lead) : base(lead, null) {  }
        
        protected override string GetValue(LeadLetterTemplateKey key)
        {
            var lead = _entity;
            
            switch (key)
            {
                case LeadLetterTemplateKey.LeadId: return lead.Id.ToString();
                case LeadLetterTemplateKey.Title: return lead.Title;
                case LeadLetterTemplateKey.DescriptionHtml: 
                    return lead.Description.IsNotEmpty() 
                        ? lead.Description.Replace("\r\n", "<br />").Replace("\n", "<br />") 
                        : "";
                case LeadLetterTemplateKey.Description: return lead.Description;
                case LeadLetterTemplateKey.DealStatus: return lead.DealStatus?.Name;
                case LeadLetterTemplateKey.SalesFunnel: return SalesFunnelService.Get(lead.SalesFunnelId)?.Name;
                case LeadLetterTemplateKey.LeadSource: return OrderSourceService.GetOrderSource(lead.OrderSourceId)?.Name;
                case LeadLetterTemplateKey.AttachmentHtmlLinks:
                    return AttachmentService.GetAttachments<LeadAttachment>(lead.Id)
                        .Select(x => $"<a href=\"{x.Path}\">{x.FileName}</a>")
                        .AggregateString(", ");
                case LeadLetterTemplateKey.LeadUrl: return UrlService.GetAdminUrl("leads#?leadIdInfo=" + lead.Id);
                case LeadLetterTemplateKey.Comment: return lead.Comment;
                case LeadLetterTemplateKey.DateCreated: return Culture.ConvertDate(lead.CreatedDate);

                case LeadLetterTemplateKey.FullName:
                {
                    var name = new List<string> { lead.LastName, lead.FirstName, lead.Patronymic }.Where(x => x.IsNotEmpty()).AggregateString(" ");

                    if (name.IsNullOrEmpty() && lead.Customer != null)
                        name = new List<string> { lead.Customer.LastName, lead.Customer.FirstName, lead.Customer.Patronymic }.Where(x => x.IsNotEmpty()).AggregateString(" ");

                    return name;
                }
                case LeadLetterTemplateKey.FirstName: return lead.FirstName.Default(lead.Customer?.FirstName);
                case LeadLetterTemplateKey.LastName: return lead.LastName.Default(lead.Customer?.LastName);
                case LeadLetterTemplateKey.Phone: return lead.Phone.Default(lead.Customer?.Phone);
                case LeadLetterTemplateKey.Email: return lead.Email.Default(lead.Customer?.EMail);
                case LeadLetterTemplateKey.Organization: return lead.Organization.Default(lead.Customer?.Organization);
                case LeadLetterTemplateKey.AdditionalCustomerFields:
                {
                    var additionalCustomerFields = "";
                    if (lead.Customer != null)
                    {
                        foreach (var customerField in CustomerFieldService.GetMappedCustomerFieldsWithValue(lead.Customer.Id)
                                                                          .Where(x => !string.IsNullOrEmpty(x.Value)))
                            additionalCustomerFields += string.Format(RowFormat, customerField.Name, customerField.Value);
                    }
                    return additionalCustomerFields;
                }
                case LeadLetterTemplateKey.ManagerName:
                {
                    var manager = lead.ManagerId.HasValue ? ManagerService.GetManager(lead.ManagerId.Value) : null;
                    return manager?.FullName;
                }
                case LeadLetterTemplateKey.Country: return lead.Country.Default(lead.Customer?.Contacts?.FirstOrDefault()?.Country);
                case LeadLetterTemplateKey.Region: return lead.Region.Default(lead.Customer?.Contacts?.FirstOrDefault()?.Region);
                case LeadLetterTemplateKey.District: return lead.District.Default(lead.Customer?.Contacts?.FirstOrDefault()?.District);
                case LeadLetterTemplateKey.City: return lead.City.Default(lead.Customer?.Contacts?.FirstOrDefault()?.City);
                
                case LeadLetterTemplateKey.ShippingName: return lead.ShippingName + (lead.ShippingPickPoint.IsNotEmpty() ? "<br />" + lead.ShippingPickPoint : "");
                case LeadLetterTemplateKey.ShippingNameWithoutHtml: return lead.ShippingName + (lead.ShippingPickPoint.IsNotEmpty() ? lead.ShippingPickPoint : "");
                case LeadLetterTemplateKey.Sum: return PriceFormatService.FormatPrice(lead.Sum, lead.LeadCurrency);
                case LeadLetterTemplateKey.SumWithoutCurrency: return lead.Sum.FormatPriceInvariant();
                case LeadLetterTemplateKey.LeadItemsHtml: return lead.LeadItems.Count > 0 
                    ? LeadService.GenerateHtmlLeadItemsTable(lead.LeadItems, lead.LeadCurrency) 
                    : "";
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}