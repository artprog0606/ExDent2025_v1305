using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.ExportImport;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class ExportLeadsHandler
    {
        private readonly List<LeadsFilterResultModel> _collection;

        private readonly string _fileName;
        private readonly List<CustomerField> _customerFields;
        private readonly List<string> _leadFieldNames;

        private ExportLeadsHandler(string fileName, int salesFunnelId)
        {
            _fileName = fileName;
            _customerFields = (!SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.CustomerAdditionFields))
                ? CustomerFieldService.GetCustomerFields(true)
                : new List<CustomerField>();
            _leadFieldNames = (salesFunnelId > 0 ? LeadFieldService.GetLeadFields(salesFunnelId) : LeadFieldService.GetLeadFields())
                .Select(x => x.Name).Distinct().ToList();
        }

        public ExportLeadsHandler(LeadsFilterResult filterResult, int salesFunnelId, string fileName) : this(fileName, salesFunnelId)
        {
            if (filterResult != null && filterResult.DataItems != null)
            {
                _collection = filterResult.DataItems;
            }
        }

        public ExportLeadsHandler(List<int> leadIds, int salesFunnelId, string fileName) : this(fileName, salesFunnelId)
        {
            if (leadIds != null)
            {
                _collection = new List<LeadsFilterResultModel>();
                foreach (var id in leadIds)
                {
                    var lead = LeadService.GetLead(id);
                    _collection.Add(new LeadsFilterResultModel
                    {
                        Id = lead.Id,
                        SalesFunnelId = lead.SalesFunnelId,
                        DealStatusName = lead.DealStatus.Name,
                        ManagerName = lead.ManagerId != null ? ManagerService.GetManager(lead.ManagerId.Value).FullName : string.Empty,
                        Title = lead.Title,
                        Description = lead.Description,
                        CustomerId = lead.CustomerId,
                        FirstName = lead.FirstName,
                        LastName = lead.LastName,
                        Patronymic = lead.Patronymic,
                        Organization = lead.Organization,
                        Email = lead.Email,
                        Phone = lead.Phone,
                        Country = lead.Country,
                        Region = lead.Region,
                        District = lead.District,
                        City = lead.City,
                        CustomerBirthDay = lead.Customer?.BirthDay,
                        Comment = lead.Comment,
                        CreatedDate = lead.CreatedDate,
                        CurrencySymbol = lead.LeadCurrency?.CurrencySymbol,
                        Discount = lead.Discount,
                        DiscountValue = lead.DiscountValue,
                        ShippingCost = lead.ShippingCost,
                        ShippingName = lead.ShippingName,
                        Sum = lead.Sum
                    });
                }
            }
        }

        public string Execute()
        {
            if (_collection == null)
                return "";

            var fileDirectory = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);

            using (var writer = new CsvWriter(new StreamWriter(fileDirectory + _fileName, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                WriteHeader(writer);

                foreach (var item in _collection)
                    WriteItem(writer, item);
            }

            return fileDirectory + _fileName;
        }

        public string GetFileUrl()
        {
            var path = Execute();

            return !string.IsNullOrEmpty(path)
                ? UrlService.GetUrl(FoldersHelper.GetPathRelative(FolderType.PriceTemp, _fileName, false))
                : path;
        }

        private void WriteHeader(CsvWriter writer)
        {
            writer.WriteField(ELeadFields.Id.StrName());
            writer.WriteField(ELeadFields.SalesFunnel.StrName());
            writer.WriteField(ELeadFields.DealStatus.StrName());
            writer.WriteField(ELeadFields.ManagerName.StrName());
            writer.WriteField(ELeadFields.Title.StrName());
            writer.WriteField(ELeadFields.Description.StrName());

            foreach (var fieldName in _leadFieldNames)
                writer.WriteField(fieldName);

            writer.WriteField(ELeadFields.CustomerId.StrName());
            writer.WriteField(ELeadFields.FirstName.StrName());
            writer.WriteField(ELeadFields.LastName.StrName());
            writer.WriteField(ELeadFields.Patronymic.StrName());
            writer.WriteField(ELeadFields.Email.StrName());
            writer.WriteField(ELeadFields.Phone.StrName());
            writer.WriteField(ELeadFields.Organization.StrName());
            writer.WriteField(ELeadFields.Country.StrName());
            writer.WriteField(ELeadFields.Region.StrName());
            writer.WriteField(ELeadFields.District.StrName());
            writer.WriteField(ELeadFields.City.StrName());
            writer.WriteField(ELeadFields.BirthDay.StrName());

            foreach (var field in _customerFields)
                writer.WriteField(field.Name);

            writer.WriteField(ELeadFields.MultiOffer.StrName());
            writer.WriteField(ELeadFields.Currency.StrName());
            writer.WriteField(ELeadFields.Discount.StrName());
            writer.WriteField(ELeadFields.DiscountValue.StrName());
            writer.WriteField(ELeadFields.ShippingCost.StrName());
            writer.WriteField(ELeadFields.ShippingName.StrName());
            writer.WriteField(ELeadFields.TotalSum.StrName());
            writer.WriteField(ELeadFields.Comment.StrName());
            writer.WriteField(ELeadFields.CreatedDate.StrName());

            writer.NextRecord();
        }

        private void WriteItem(CsvWriter writer, LeadsFilterResultModel lead)
        {
            writer.WriteField(lead.Id);
            
            writer.WriteField(lead.SalesFunnelName.IsNotEmpty()
                ? lead.SalesFunnelName
                : SalesFunnelService.Get(lead.SalesFunnelId).Name);

            writer.WriteField(lead.DealStatusName);
            writer.WriteField(lead.ManagerName);
            writer.WriteField(lead.Title);
            writer.WriteField(lead.Description);

            if (_leadFieldNames.Any())
            {
                var values = LeadFieldService.GetLeadFieldsWithValue(lead.Id);
                foreach (var fieldName in _leadFieldNames)
                {
                    var leadFieldValue = values.FirstOrDefault(item => item.Name == fieldName);
                    writer.WriteField(leadFieldValue != null ? leadFieldValue.Value : string.Empty);
                }
            }
            
            writer.WriteField(lead.CustomerId != null ? lead.CustomerId.ToString() : "");
            writer.WriteField(lead.FirstName);
            writer.WriteField(lead.LastName);
            writer.WriteField(lead.Patronymic);
            writer.WriteField(lead.Email);
            writer.WriteField(lead.Phone);
            writer.WriteField(lead.Organization);

            writer.WriteField(lead.Country);
            writer.WriteField(lead.Region);
            writer.WriteField(lead.District);
            writer.WriteField(lead.City);

            if (lead.CustomerBirthDay.HasValue)
                writer.WriteField(lead.CustomerBirthDay.Value);
            else
                writer.WriteField(string.Empty);

            if (lead.CustomerId.HasValue && _customerFields.Any())
            {
                var values = CustomerFieldService.GetCustomerFieldsWithValue(lead.CustomerId.Value);
                foreach (var field in _customerFields)
                {
                    var customerFieldValue = values.FirstOrDefault(item => item.Id == field.Id);
                    writer.WriteField(customerFieldValue != null ? customerFieldValue.Value : string.Empty);
                }
            }

            var leadItems = LeadService.GetLeadItems(lead.Id);
            var multiOffer = leadItems.Aggregate("",
                (current, item) =>
                    current + ((current == "" ? "" : ";") + item.ArtNo + ":" + item.Price + ":" + item.Amount));

            writer.WriteField(multiOffer);
            
            writer.WriteField(lead.CurrencySymbol);
            writer.WriteField(lead.Discount);
            writer.WriteField(lead.DiscountValue);
            writer.WriteField(lead.ShippingCost);
            writer.WriteField(lead.ShippingName);
            writer.WriteField(lead.Sum);

            writer.WriteField(lead.Comment);

            writer.WriteField(lead.CreatedDateFormatted);

            writer.NextRecord();
        }
    }
}
