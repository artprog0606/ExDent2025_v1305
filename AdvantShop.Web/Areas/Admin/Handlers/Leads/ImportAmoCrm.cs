using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.CMS;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Shared.AdminComments;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class ImportAmoCrm
    {
        private readonly List<CustomerField> _customerFields;

        public ImportAmoCrm()
        {
            _customerFields = CustomerFieldService.GetCustomerFields();
        }

        public bool Execute()
        {
            var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
            if (file == null)
                throw new BlException("Файл импорта не найден");

            if (!file.FileName.Contains(".csv"))
                throw new BlException("Загрузите .csv файл");

            var folderPath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            var filePath = folderPath + "importAmoCrm.csv".FileNamePlusDate();

            try
            {
                FileHelpers.CreateDirectory(folderPath);

                file.SaveAs(filePath);

                using (var csvReader = new CsvReader(new StreamReader(filePath), CsvConstants.DefaultCsvConfiguration))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                        ReadRow(csvReader);
                }

                FileHelpers.DeleteFile(filePath);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }

        private void ReadRow(CsvReader csvReader)
        {
            try
            {
                var lead = new Lead()
                {
                    Title = csvReader.GetField<string>("Название сделки"),

                    Customer = new Customer()
                    {
                        Organization = csvReader.GetField<string>("Компания"),
                        FirstName = csvReader.GetField<string>("Основной контакт"),

                        EMail = csvReader.GetField<string>("Рабочий email (контакт)"),
                        Phone = csvReader.GetField<string>("Рабочий телефон (контакт)")
                    },

                    Sum = csvReader.GetField<string>("Бюджет").TryParseFloat(),
                    CreatedDate = csvReader.GetField<string>("Дата создания").TryParseDateTime()
                };

                var managerName = csvReader.GetField<string>("Ответственный");
                if (!string.IsNullOrWhiteSpace(managerName))
                {
                    lead.ManagerId = TryGetAddManager(managerName);
                }

                var salesFunnelName = csvReader.GetField<string>("Воронка");
                if (!string.IsNullOrWhiteSpace(salesFunnelName))
                {
                    lead.SalesFunnelId = TryGetAddSalesFunnel(salesFunnelName);
                }

                var dealStatusName = csvReader.GetField<string>("Этап сделки");
                if (!string.IsNullOrWhiteSpace(dealStatusName) && lead.SalesFunnelId != 0)
                {
                    lead.DealStatusId = TryGetAddDealStatus(lead.SalesFunnelId, dealStatusName);
                }

                LeadService.AddLead(lead, true, trackChanges: false);

                if (lead.Id == 0)
                    return;

                var comments = new List<string>();

                if (csvReader.HeaderRecord != null)
                    foreach (var header in csvReader.HeaderRecord)
                    {
                        var fielValue = csvReader.GetField<string>(header);

                        if (header.StartsWith("Примечание") && !string.IsNullOrWhiteSpace(fielValue))
                        {
                            comments.Add(fielValue);
                            continue;
                        }

                        var customerField = _customerFields.Find(x => x.Name == header);
                        if (customerField != null)
                            CustomerFieldService.AddUpdateMap(lead.Customer.Id, customerField.Id, fielValue);
                    }

                foreach (var comment in comments)
                    new AddAdminCommentHandler(new AdminCommentModel()
                    {
                        ObjId = lead.Id,
                        Type = AdminCommentType.Lead,
                        Text = comment
                    }, massAction: true).Execute();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }


        private int? TryGetAddManager(string managerName)
        {
            var manager = ManagerService.GetManagersList().Find(x => x.FirstName == managerName || x.FirstName + " " + x.LastName == managerName);
            if (manager != null)
                return manager.ManagerId;

            if (Saas.SaasDataService.IsSaasEnabled && ManagerService.GetManagersCount() >= Saas.SaasDataService.CurrentSaasData.EmployeesCount)
                return null;

            var c = new Customer()
            {
                FirstName = managerName,
                CustomerRole = Role.Moderator
            };
            CustomerService.InsertNewCustomer(c);

            var m = new Manager() { CustomerId = c.Id };
            ManagerService.AddOrUpdateManager(m);

            return m.ManagerId;
        }

        private int TryGetAddSalesFunnel(string salesFunnelName)
        {
            var funnel = SalesFunnelService.GetList().Find(x => x.Name == salesFunnelName);
            if (funnel != null)
                return funnel.Id;

            funnel = new SalesFunnel() {Name = salesFunnelName};
            try
            {
                SalesFunnelService.Add(funnel);
            }
            catch (BlException)
            {
                return 0;
            }

            return funnel.Id;
        }

        private int TryGetAddDealStatus(int salesFunnelId, string dealStatusName)
        {
            var status = DealStatusService.GetList(salesFunnelId).Find(x => x.Name == dealStatusName);
            if (status != null)
                return status.Id;
            
            status = new DealStatus() { Name = dealStatusName };
            DealStatusService.Add(status);
            SalesFunnelService.AddDealStatus(salesFunnelId, status.Id);

            return status.Id;
        }

    }
}
