using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.AdminPushNotification;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Repository.Currencies;
using AdvantShop.FilePath;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Core.Services.Triggers.DeferredDatas;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Core.Services.Crm.LeadFields;

namespace AdvantShop.Core.Services.Crm
{
    public class LeadService
    {
        #region Lead

        public static Lead GetLead(int id)
        {
            return SQLDataAccess.Query<Lead>("Select * From [Order].[Lead] Where Id=@Id", new { Id = id }).FirstOrDefault();
        }

        public static List<Lead> GetAllLeads()
        {
            return SQLDataAccess.Query<Lead>("Select * From [Order].[Lead]").ToList();
        }

        public static void AddLead(Lead lead, bool isFromAdminArea = false, ChangedBy changedBy = null, bool trackChanges = true,
           List<CustomerFieldWithValue> customerFields = null, List<LeadFieldWithValue> leadFields = null)
        {
            Customer customer = null;

            if (lead.Customer != null && !lead.Customer.RegistredUser)
            {
                if (lead.CustomerId != null && lead.CustomerId != Guid.Empty)
                    customer = CustomerService.GetCustomer(lead.CustomerId.Value);

                if (customer == null &&
                    !string.IsNullOrEmpty(lead.Customer.EMail) && !string.IsNullOrEmpty(lead.Customer.Phone))
                {
                    customer = CustomerService.GetCustomerByEmailAndPhone(lead.Customer.EMail, lead.Customer.Phone, lead.Customer.StandardPhone);
                }

                if (customer == null && !string.IsNullOrEmpty(lead.Customer.EMail))
                    customer = CustomerService.GetCustomerByEmail(lead.Customer.EMail);

                if (customer == null && !string.IsNullOrEmpty(lead.Customer.Phone))
                    customer = CustomerService.GetCustomerByPhone(lead.Customer.Phone, lead.Customer.StandardPhone);

                if (customer == null)
                {
                    lead.CustomerId = CustomerService.InsertNewCustomer(lead.Customer); // если пользователь с таким email,телефоном существует, то Guid.Empty

                    if (lead.CustomerId != Guid.Empty && lead.Customer.Contacts != null && lead.Customer.Contacts.Count > 0)
                    {
                        CustomerService.AddContact(lead.Customer.Contacts[0], lead.CustomerId.Value);
                    }
                }
                else
                {
                    lead.CustomerId = customer.Id;
                }
            }

            if (lead.SalesFunnelId == 0)
                lead.SalesFunnelId = SettingsCrm.DefaultSalesFunnelId;

            if (lead.DealStatusId == 0)
            {
                var dealStatus = DealStatusService.GetList(lead.SalesFunnelId).FirstOrDefault();
                if (dealStatus != null)
                    lead.DealStatusId = dealStatus.Id;
            }

            if (lead.CreatedDate == DateTime.MinValue)
                lead.CreatedDate = DateTime.Now;

            lead.Id = SQLDataAccess.ExecuteScalar<int>(
                "Insert Into [Order].[Lead] " +
                    "(CustomerId,FirstName,LastName,Patronymic,Title,Description,Sum,Phone,Email,ManagerId,CreatedDate,Discount,DiscountValue,OrderSourceId,Comment," +
                    "DealStatusId,IsFromAdminArea,DeliveryDate,DeliveryTime,ShippingMethodId,ShippingName,ShippingCost,ShippingPickPoint, SortOrder, " +
                    "SalesFunnelId, Organization, ModifiedDate, Country, Region, City, Zip, District, ShippingTaxType, AvailablePaymentCashOnDelivery, " +
                    "AvailablePaymentPickPoint, ShippingPaymentMethodType, ShippingPaymentSubjectType, CustomerType) " +
                "Values " +
                    "(@CustomerId,@FirstName,@LastName,@Patronymic,@Title,@Description,@Sum,@Phone,@Email,@ManagerId,@CreatedDate,@Discount,@DiscountValue,@OrderSourceId,@Comment," +
                    "@DealStatusId,@IsFromAdminArea,@DeliveryDate,@DeliveryTime,@ShippingMethodId,@ShippingName,@ShippingCost,@ShippingPickPoint, " +
                    "(SELECT ISNULL(MAX(SortOrder), 0) + 10 FROM [Order].[Lead] WHERE DealStatusId = @DealStatusId), " +
                    "@SalesFunnelId, @Organization, @CreatedDate, @Country, @Region, @City, @Zip, @District, @ShippingTaxType, @AvailablePaymentCashOnDelivery, " +
                    "@AvailablePaymentPickPoint, @ShippingPaymentMethodType, @ShippingPaymentSubjectType, @CustomerType); " +
                "SELECT scope_identity();",
                CommandType.Text,
                new SqlParameter("@CustomerId", lead.CustomerId != null && lead.CustomerId != Guid.Empty ? lead.CustomerId :(object)DBNull.Value),
                new SqlParameter("@FirstName", lead.FirstName ?? string.Empty),
                new SqlParameter("@LastName", lead.LastName ?? string.Empty),
                new SqlParameter("@Patronymic", lead.Patronymic ?? string.Empty),
                new SqlParameter("@Title", lead.Title ?? string.Empty),
                new SqlParameter("@Description", lead.Description ?? string.Empty),
                new SqlParameter("@Sum", lead.Sum),
                new SqlParameter("@Phone", lead.Phone ?? string.Empty),
                new SqlParameter("@Email", lead.Email ?? string.Empty),
                new SqlParameter("@ManagerId", lead.ManagerId ?? (object)DBNull.Value),
                new SqlParameter("@Discount", lead.Discount),
                new SqlParameter("@DiscountValue", lead.DiscountValue),
                new SqlParameter("@OrderSourceId", lead.OrderSourceId),
                new SqlParameter("@Comment", lead.Comment ?? string.Empty),
                new SqlParameter("@DealStatusId", lead.DealStatusId),
                new SqlParameter("@IsFromAdminArea", lead.IsFromAdminArea),
                new SqlParameter("@CreatedDate", lead.CreatedDate),

                new SqlParameter("@DeliveryDate", lead.DeliveryDate ?? (object)DBNull.Value),
                new SqlParameter("@DeliveryTime", lead.DeliveryTime ?? (object)DBNull.Value),
                new SqlParameter("@ShippingMethodId", lead.ShippingMethodId != 0 ? lead.ShippingMethodId : (object)DBNull.Value),
                new SqlParameter("@ShippingName", lead.ShippingName ?? (object)DBNull.Value),
                new SqlParameter("@ShippingCost", lead.ShippingCost),
                new SqlParameter("@ShippingPickPoint", lead.ShippingPickPoint ?? (object)DBNull.Value),
                new SqlParameter("@SalesFunnelId", lead.SalesFunnelId),
                new SqlParameter("@Organization", lead.Organization ?? (object)DBNull.Value),
                new SqlParameter("@CustomerType", lead.CustomerType),

                new SqlParameter("@Country", lead.Country ?? (object)DBNull.Value),
                new SqlParameter("@Region", lead.Region ?? (object)DBNull.Value),
                new SqlParameter("@District", lead.District ?? (object)DBNull.Value),
                new SqlParameter("@City", lead.City ?? (object)DBNull.Value),
                new SqlParameter("@Zip", lead.Zip ?? (object)DBNull.Value),
                
                new SqlParameter("@ShippingTaxType", (int)lead.ShippingTaxType),
                new SqlParameter("@AvailablePaymentCashOnDelivery", lead.AvailablePaymentCashOnDelivery),
                new SqlParameter("@AvailablePaymentPickPoint", lead.AvailablePaymentPickPoint),
                new SqlParameter("@ShippingPaymentMethodType", (int)lead.ShippingPaymentMethodType),
                new SqlParameter("@ShippingPaymentSubjectType", (int)lead.ShippingPaymentSubjectType)
            );

            if (string.IsNullOrEmpty(lead.Title))
            {
                lead.Title = LocalizationService.GetResource("Admin.Leads.Popup.LeadNumber") + lead.Id;
                SQLDataAccess.ExecuteNonQuery("Update [Order].[Lead] Set Title=@Title Where Id=@Id",
                    CommandType.Text,
                    new SqlParameter("@Id", lead.Id),
                    new SqlParameter("@Title", lead.Title));
            }

            LeadsHistoryService.NewLead(lead, changedBy);

            if (lead.LeadCurrency == null)
                lead.LeadCurrency = CurrencyService.CurrentCurrency;

            AddLeadCurrency(lead.Id, lead.LeadCurrency);

            if (lead.LeadItems == null)
                lead.LeadItems = new List<LeadItem>();

            foreach (var item in lead.LeadItems)
                AddLeadItem(lead.Id, item, changedBy, trackChanges);

            if (leadFields != null && leadFields.Count > 0)
            {
                foreach (var field in leadFields)
                    LeadFieldService.AddUpdateMap(lead.Id, field.Id, field.Value);
            }

            if (customerFields != null && customerFields.Count > 0 && lead.Customer != null && lead.Customer.Id != Guid.Empty)
            {
                foreach (var field in customerFields)
                    CustomerFieldService.AddUpdateMap(lead.Customer.Id, field.Id, field.Value ?? "", lead.Customer != null && !lead.Customer.RegistredUser);
            }

            Manager leadManager = null;
            if (lead.ManagerId.HasValue && (leadManager = ManagerService.GetManager(lead.ManagerId.Value)) != null)
            {
                var mailTemplate = new LeadAssignedMailTemplate(lead);
                MailService.SendMailNow(leadManager.Email, mailTemplate);
            }

            var salesFunnel = SalesFunnelService.Get(lead.SalesFunnelId);
            if (salesFunnel != null && !salesFunnel.NotSendNotificationsOnLeadCreation)
            {
                var emailsTo = new List<string> { SettingsMail.EmailForLeads };

                var managers = SalesFunnelService.GetSalesFunnelManagers(lead.SalesFunnelId);
                if (managers.Count > 0)
                {
                    emailsTo.AddRange(managers.Where(x => x.Email.IsNotEmpty() && x.Enabled && x.HasRoleAction(RoleAction.Crm)).Select(x => x.Email));
                }
                emailsTo = emailsTo.Where(x => leadManager == null || x != leadManager.Email).Distinct().ToList();

                if (emailsTo.Any())
                {
                    var mail = new LeadMailTemplate(lead);
                    MailService.SendMailNow(emailsTo.AggregateString(";"), mail);
                }
            }

            var loger = LoggingManager.GetTrafficSourceLogger();
            loger.LogOrderTafficSource(lead.Id, TrafficSourceType.Lead, isFromAdminArea);

            SmsNotifier.SendSmsOnLeadAdded(lead);
            BizProcessExecuter.LeadAdded(lead);
            DealStatusService.ClearCache();
            AdminPushNotificationService.NewLead(lead);

            ModulesExecuter.LeadAdded(lead);

            ModulesExecuter.Subscribe(new Subscription
            {
                Email = lead.Email,
                Phone = lead.Phone,
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                CustomerType = EMailRecipientType.LeadCustomer
            }, lead.SalesFunnelId.ToString());

            TriggerProcessService.ProcessEvent(ETriggerEventType.LeadCreated, lead);
        }


        public static void UpdateLead(Lead lead, bool updateCustomer = true, ChangedBy changedBy = null, bool trackChanges = true,
            List<CustomerFieldWithValue> customerFields = null, List<LeadFieldWithValue> leadFields = null)
        {
            if (trackChanges)
                LeadsHistoryService.TrackLeadChanges(lead, changedBy);

            var prevState = GetLead(lead.Id).DeepClone();

            SQLDataAccess.ExecuteNonQuery(
                "Update [Order].[Lead] " +
                    "Set CustomerId = @CustomerId, " +
                        "FirstName = @FirstName, " +
                        "LastName = @LastName, " +
                        "Patronymic = @Patronymic, " +
                        "Title = @Title, " +
                        "Description = @Description, " +
                        "Sum = @Sum, " +
                        "Phone = @Phone, " +
                        "Email = @Email, " +
                        "OrderSourceId = @OrderSourceId," +
                        "ManagerId = @ManagerId, " +
                        "Discount = @Discount, " +
                        "DiscountValue = @DiscountValue, " +
                        "DealStatusId = @DealStatusId, " +
                        "SalesFunnelId = @SalesFunnelId, " +

                        "DeliveryDate = @DeliveryDate, " +
                        "DeliveryTime = @DeliveryTime, " +
                        "ShippingMethodId = @ShippingMethodId, " +
                        "ShippingName = @ShippingName, " +
                        "ShippingCost = @ShippingCost, " +
                        "ShippingPickPoint = @ShippingPickPoint, " +
                        "Organization = @Organization, " +
                        "Country = @Country, " +
                        "Region = @Region, " +
                        "District = @District, " +
                        "City = @City, " +
                        "Zip = @Zip, " +
                        "ShippingTaxType = @ShippingTaxType, " +
                        "AvailablePaymentCashOnDelivery = @AvailablePaymentCashOnDelivery, " +
                        "AvailablePaymentPickPoint = @AvailablePaymentPickPoint, " +
                        "ShippingPaymentMethodType = @ShippingPaymentMethodType, " +
                        "ShippingPaymentSubjectType = @ShippingPaymentSubjectType, " +
                        "ModifiedDate = GETDATE(), " +
                        "CustomerType = @CustomerType " +
                "Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", lead.Id),
                new SqlParameter("@CustomerId", lead.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@FirstName", lead.FirstName ?? string.Empty),
                new SqlParameter("@LastName", lead.LastName ?? string.Empty),
                new SqlParameter("@Patronymic", lead.Patronymic ?? string.Empty),
                new SqlParameter("@Title", lead.Title ?? string.Empty),
                new SqlParameter("@Description", lead.Description ?? string.Empty),
                new SqlParameter("@Sum", lead.Sum),
                new SqlParameter("@Phone", lead.Phone ?? string.Empty),
                new SqlParameter("@Email", lead.Email ?? string.Empty),
                new SqlParameter("@ManagerId", lead.ManagerId ?? (object)DBNull.Value),
                new SqlParameter("@Discount", lead.Discount),
                new SqlParameter("@DiscountValue", lead.DiscountValue),
                new SqlParameter("@OrderSourceId", lead.OrderSourceId),
                new SqlParameter("@DealStatusId", lead.DealStatusId),
                new SqlParameter("@SalesFunnelId", lead.SalesFunnelId),

                new SqlParameter("@DeliveryDate", lead.DeliveryDate ?? (object)DBNull.Value),
                new SqlParameter("@DeliveryTime", lead.DeliveryTime ?? (object)DBNull.Value),
                new SqlParameter("@ShippingMethodId", lead.ShippingMethodId != 0 ? lead.ShippingMethodId : (object)DBNull.Value),
                new SqlParameter("@ShippingName", lead.ShippingName ?? (object)DBNull.Value),
                new SqlParameter("@ShippingCost", lead.ShippingCost),
                new SqlParameter("@ShippingPickPoint", lead.ShippingPickPoint ?? (object)DBNull.Value),
                new SqlParameter("@Organization", lead.Organization ?? (object)DBNull.Value),
                new SqlParameter("@CustomerType", lead.CustomerType),

                new SqlParameter("@Country", lead.Country ?? (object)DBNull.Value),
                new SqlParameter("@Region", lead.Region ?? (object)DBNull.Value),
                new SqlParameter("@District", lead.District ?? (object)DBNull.Value),
                new SqlParameter("@City", lead.City ?? (object)DBNull.Value),
                new SqlParameter("@Zip", lead.Zip ?? (object)DBNull.Value),
                
                new SqlParameter("@ShippingTaxType", (int)lead.ShippingTaxType),
                new SqlParameter("@AvailablePaymentCashOnDelivery", lead.AvailablePaymentCashOnDelivery),
                new SqlParameter("@AvailablePaymentPickPoint", lead.AvailablePaymentPickPoint),
                new SqlParameter("@ShippingPaymentMethodType", (int)lead.ShippingPaymentMethodType),
                new SqlParameter("@ShippingPaymentSubjectType", (int)lead.ShippingPaymentSubjectType)
                );

            if (updateCustomer)
            {
                if (lead.CustomerId != null && lead.CustomerId != Guid.Empty && lead.Customer != null && lead.Customer.Id != Guid.Empty)
                {
                    if (trackChanges)
                        LeadsHistoryService.TrackLeadCustomerChanges(lead, changedBy);

                    CustomerService.UpdateCustomer(lead.Customer);

                    if (lead.Customer.Contacts != null && lead.Customer.Contacts.Count > 0)
                    {
                        foreach (var contact in lead.Customer.Contacts)
                        {
                            if (contact.ContactId == Guid.Empty ||
                                CustomerService.GetCustomerContact(contact.ContactId.ToString()) == null)
                            {
                                CustomerService.AddContact(contact, lead.Customer.Id);
                            }
                            else
                            {
                                CustomerService.UpdateContact(contact);
                            }
                        }
                    }
                }
                else if (lead.Customer != null)
                    CustomerService.InsertNewCustomer(lead.Customer);
            }

            var history = new List<ChangeHistory>();

            var leadItems = GetLeadItems(lead.Id);
            foreach (var item in leadItems)
            {
                if (lead.LeadItems.Find(x => x.LeadItemId == item.LeadItemId) == null)
                {
                    DeleteLeadItem(item);

                    history.Add(new ChangeHistory(changedBy)
                    {
                        ObjId = lead.Id,
                        ObjType = ChangeHistoryObjType.Lead,
                        ParameterName =
                            "Удален товар " + item.Name +
                            (!string.IsNullOrEmpty(item.ArtNo) ? " (" + item.ArtNo + ")" : ""),
                        ParameterId = item.ProductId,
                    });
                }
            }

            foreach (var item in lead.LeadItems)
            {
                if (leadItems.Find(x => x.LeadItemId == item.LeadItemId) != null)
                {
                    history.AddRange(ChangeHistoryService.GetChanges(lead.Id, ChangeHistoryObjType.Lead, GetLeadItem(item.LeadItemId), item, changedBy, item.LeadItemId));

                    UpdateLeadItem(lead.Id, item);
                }
                else
                {
                    AddLeadItem(lead.Id, item);

                    history.Add(new ChangeHistory(changedBy)
                    {
                        ObjId = lead.Id,
                        ObjType = ChangeHistoryObjType.Lead,
                        ParameterName =
                            "Добавлен товар " + item.Name +
                            (!string.IsNullOrEmpty(item.ArtNo) ? " (" + item.ArtNo + ")" : ""),
                        ParameterId = item.ProductId,
                    });
                }
            }

            if (prevState.SalesFunnelId != lead.SalesFunnelId)
                LeadFieldService.DeleteFieldsMap(lead.Id);
            if (leadFields != null && leadFields.Any())
            {
                foreach (var field in leadFields)
                {
                    LeadsHistoryService.TrackLeadFieldChanges(lead.Id, field.Id, field.Value, null);
                    LeadFieldService.AddUpdateMap(lead.Id, field.Id, field.Value);
                }
            }

            if (customerFields != null && customerFields.Count > 0 && lead.Customer.Id != Guid.Empty)
            {
                foreach (var field in customerFields)
                    CustomerFieldService.AddUpdateMap(lead.Customer.Id, field.Id, field.Value ?? "");
            }

            if (prevState.DealStatusId != lead.DealStatusId)
            {
                BizProcessExecuter.LeadStatusChanged(lead);
                TriggerProcessService.ProcessEvent(ETriggerEventType.LeadStatusChanged, lead);
            }

            OnLeadChanged(prevState, lead, history, changedBy);

            DealStatusService.ClearCache();
            ModulesExecuter.LeadUpdated(lead);
        }

        public static void UpdateLeadManager(int leadId, int? managerId, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var lead = GetLead(leadId);
            var prevState = lead.DeepClone();

            lead.ManagerId = managerId;

            if (trackChanges)
                LeadsHistoryService.TrackLeadChanges(lead, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Lead] SET [ManagerId] = @ManagerId WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", leadId),
                new SqlParameter("@ManagerId", managerId ?? (object)DBNull.Value));

            OnLeadChanged(prevState, lead, changedBy:changedBy);
        }

        public static void DeleteLead(int leadId, ChangedBy changedBy = null, bool trackChanges = true)
        {
            AttachmentService.DeleteAttachments<LeadAttachment>(leadId);
            AdminCommentService.DeleteAdminComments(leadId, AdminCommentType.Lead);
            SQLDataAccess.ExecuteNonQuery("Delete From [Order].[Lead] Where Id=@Id", CommandType.Text, new SqlParameter("@Id", leadId));
            DealStatusService.ClearCache();

            if (trackChanges)
                LeadsHistoryService.DeleteLead(leadId, changedBy);

            ModulesExecuter.LeadDeleted(leadId);

            TriggerDeferredDataService.Delete(ETriggerObjectType.Lead, leadId);
        }

        public static int GetNewLeadsCount(int? managerId, int? orderSourceId = null)
        {
            var count = 0;

            foreach (var funnel in SalesFunnelService.GetList().Where(SalesFunnelService.CheckAccess))
            {
                var status = DealStatusService.GetList(funnel.Id).FirstOrDefault();
                if (status == null)
                    continue;

                var sql = "Select Count(Id) From [Order].[Lead] Where DealStatusId=@DealStatusId" + (orderSourceId.HasValue ? " AND OrderSourceId=@OrderSourceId" : null);

                if (managerId != null)
                {
                    switch (SettingsManager.ManagersLeadConstraint)
                    {
                        case ManagersLeadConstraint.Assigned:
                            sql += " And ManagerId=@ManagerId ";
                            break;

                        case ManagersLeadConstraint.AssignedAndFree:
                            sql += " And (ManagerId=@ManagerId OR ManagerId is null) ";
                            break;
                    }

                    count += SQLDataAccess.ExecuteScalar<int>(sql, CommandType.Text,
                        new SqlParameter("@DealStatusId", status.Id), 
                        new SqlParameter("@ManagerId", managerId),
                        new SqlParameter("@OrderSourceId", orderSourceId ?? (object)DBNull.Value));
                }
                else
                {
                    count += SQLDataAccess.ExecuteScalar<int>(sql, CommandType.Text, 
                        new SqlParameter("@DealStatusId", status.Id),
                        new SqlParameter("@OrderSourceId", orderSourceId ?? (object)DBNull.Value));
                }
            }

            return count;
        }

        public static int GetLeadsCount(int? managerId = null)
        {
            var dealStatus = DealStatusService.GetList().FirstOrDefault();
            if (dealStatus == null)
                return 0;

            if (managerId != null)
            {
                var sql = "Select Count(Id) From [Order].[Lead] Where DealStatusId=@DealStatusId";

                switch (SettingsManager.ManagersLeadConstraint)
                {
                    case ManagersLeadConstraint.Assigned:
                        sql += " And ManagerId=@ManagerId ";
                        break;

                    case ManagersLeadConstraint.AssignedAndFree:
                        sql += " And (ManagerId=@ManagerId OR ManagerId is null) ";
                        break;
                }

                return
                    Convert.ToInt32(
                        SQLDataAccess.ExecuteScalar(sql, CommandType.Text, new SqlParameter("@DealStatusId", dealStatus.Id), new SqlParameter("@ManagerId", managerId)));
            }

            return Convert.ToInt32(SQLDataAccess.ExecuteScalar("Select Count(Id) From [Order].[Lead] Where DealStatusId=@DealStatusId", CommandType.Text, new SqlParameter("@DealStatusId", dealStatus.Id)));
        }

        public static int GetLeadsCount(Guid customerId)
        {
            return
                Convert.ToInt32(
                    SQLDataAccess.ExecuteScalar("Select Count(Id) From [Order].[Lead] Where CustomerId=@customerId",
                        CommandType.Text, new SqlParameter("@CustomerId", customerId)));

        }

        public static List<Lead> GetLeadsByCustomer(Guid customerId)
        {
            return
                SQLDataAccess.Query<Lead>("Select * From [Order].[Lead] Where CustomerId=@customerId", new { customerId }).ToList();
        }

        public static List<Lead> GetOpenLeadsByCustomer(Guid customerId)
        {
            return SQLDataAccess.Query<Lead>(
                "SELECT Lead.* FROM [Order].Lead INNER JOIN CRM.DealStatus ON DealStatus.Id = Lead.DealStatusId " +
                "WHERE CustomerId = @CustomerId AND DealStatus.[Status] = @Status", 
                new { customerId, status = SalesFunnelStatusType.None }).ToList();
        }

        public static int GetLastLeadIdByCustomer(Guid customerId)
        {
            return
               Convert.ToInt32(SQLDataAccess.ExecuteScalar("Select Id From [Order].[Lead] Where CustomerId=@customerId order by CreatedDate desc",
                CommandType.Text, new SqlParameter("@customerId", customerId)));
        }


        public static List<Lead> GetLeadsByPhone(string phone)
        {
            return
                SQLDataAccess.Query<Lead>("Select * From [Order].[Lead] Where Phone=@phone", new { phone }).ToList();
        }

        public static void ChangeLeadSorting(int id, int? prevId, int? nextId)
        {
            SQLDataAccess.ExecuteNonQuery("CRM.ChangeLeadSorting", CommandType.StoredProcedure,
                new SqlParameter("@Id", id),
                new SqlParameter("@prevId", prevId ?? (object)DBNull.Value),
                new SqlParameter("@nextId", nextId ?? (object)DBNull.Value));
        }


        #endregion

        #region Lead Currency

        public static LeadCurrency GetLeadCurrency(int id)
        {
            return SQLDataAccess.Query<LeadCurrency>(
                "SELECT * FROM [Order].[LeadCurrency] WHERE [LeadId] = @LeadId", new { LeadId = id }).FirstOrDefault();
        }

        public static void AddLeadCurrency(int leadId, LeadCurrency currency)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Order].[LeadCurrency] " +
                "(LeadId,CurrencyCode,CurrencyNumCode,CurrencyValue,CurrencySymbol,IsCodeBefore,RoundNumbers,EnablePriceRounding) " +
                "Values " +
                "(@LeadId,@CurrencyCode,@CurrencyNumCode,@CurrencyValue,@CurrencySymbol,@IsCodeBefore,@RoundNumbers,@EnablePriceRounding) ",
                CommandType.Text,
                new SqlParameter("@LeadId", leadId),
                new SqlParameter("@CurrencyCode", currency.CurrencyCode),
                new SqlParameter("@CurrencyNumCode", currency.CurrencyNumCode),
                new SqlParameter("@CurrencyValue", currency.CurrencyValue),
                new SqlParameter("@CurrencySymbol", currency.CurrencySymbol),
                new SqlParameter("@IsCodeBefore", currency.IsCodeBefore),
                new SqlParameter("@RoundNumbers", currency.RoundNumbers),
                new SqlParameter("@EnablePriceRounding", currency.EnablePriceRounding)
                );
        }

        #endregion

        #region Lead Items

        public static LeadItem GetLeadItem(int leadItemId)
        {
            return
                SQLDataAccess.Query<LeadItem>("Select * From [Order].[LeadItem] Where LeadItemId=@leadItemId",
                    new { leadItemId }).FirstOrDefault();
        }

        public static List<LeadItem> GetLeadItems(int leadId)
        {
            return
                SQLDataAccess.Query<LeadItem>("Select * From [Order].[LeadItem] Where LeadId=@LeadId",
                    new { LeadId = leadId }).ToList();
        }

        public static void AddLeadItem(int leadId, LeadItem item, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                LeadsHistoryService.TrackLeadItemChanges(leadId, null, item, changedBy);

            item.LeadItemId = SQLDataAccess.ExecuteScalar<int>(
                "Insert Into [Order].[LeadItem] " +
                    "(LeadId,ProductId,Name,ArtNo,Price,Amount,Weight,Color,Size,PhotoId,Width,Length,Height,CustomOptionsJson,BarCode) " +
                "Values " +
                    "(@LeadId,@ProductId,@Name,@ArtNo,@Price,@Amount,@Weight,@Color,@Size,@PhotoId,@Width,@Length,@Height,@CustomOptionsJson,@BarCode); " +
                "SELECT scope_identity();",
                CommandType.Text,
                new SqlParameter("@LeadId", leadId),
                new SqlParameter("@ProductId", item.ProductId ?? (object)DBNull.Value),
                new SqlParameter("@Name", item.Name ?? ""),
                new SqlParameter("@ArtNo", item.ArtNo ?? ""),
                new SqlParameter("@Price", item.Price),
                new SqlParameter("@Amount", item.Amount),
                new SqlParameter("@Weight", item.Weight),
                new SqlParameter("@Color", item.Color ?? (object)DBNull.Value),
                new SqlParameter("@Size", item.Size ?? (object)DBNull.Value),
                new SqlParameter("@PhotoId", item.PhotoId != 0 && item.PhotoId != null ? item.PhotoId : (object)DBNull.Value),
                new SqlParameter("@Width", item.Width),
                new SqlParameter("@Length", item.Length),
                new SqlParameter("@Height", item.Height),
                new SqlParameter("@CustomOptionsJson", item.CustomOptionsJson ?? (object)DBNull.Value),
                new SqlParameter("@BarCode", item.BarCode ?? (object)DBNull.Value)
                );
        }

        public static void UpdateLeadItem(int leadId, LeadItem item, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                LeadsHistoryService.TrackLeadItemChanges(leadId, GetLeadItem(item.LeadItemId), item, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                "Update [Order].[LeadItem] " +
                    "Set LeadId=@LeadId, " +
                        "ProductId=@ProductId, " +
                        "Name=@Name, " +
                        "ArtNo=@ArtNo, " +
                        "Price=@Price, " +
                        "Amount=@Amount, " +
                        "Weight=@Weight, " +
                        "Color=@Color, " +
                        "Size=@Size, " +
                        "PhotoId=@PhotoId, " +
                        "Width=@Width, " +
                        "Height=@Height, " +
                        "Length=@Length, " +
                        "CustomOptionsJson=@CustomOptionsJson, " +
                        "BarCode=@BarCode " +
                "Where LeadItemId=@LeadItemId",
                CommandType.Text,
                new SqlParameter("@LeadItemId", item.LeadItemId),
                new SqlParameter("@LeadId", leadId),
                new SqlParameter("@ProductId", item.ProductId ?? (object)DBNull.Value),
                new SqlParameter("@Name", item.Name ?? ""),
                new SqlParameter("@ArtNo", item.ArtNo ?? ""),
                new SqlParameter("@Price", item.Price),
                new SqlParameter("@Amount", item.Amount),
                new SqlParameter("@Weight", item.Weight),
                new SqlParameter("@Color", item.Color ?? (object)DBNull.Value),
                new SqlParameter("@Size", item.Size ?? (object)DBNull.Value),
                new SqlParameter("@PhotoId", item.PhotoId != 0 && item.PhotoId != null ? item.PhotoId : (object)DBNull.Value),
                new SqlParameter("@Width", item.Width),
                new SqlParameter("@Height", item.Height),
                new SqlParameter("@Length", item.Length),
                new SqlParameter("@CustomOptionsJson", item.CustomOptionsJson ?? (object)DBNull.Value),
                new SqlParameter("@BarCode", item.BarCode ?? (object)DBNull.Value)
                );
        }

        public static void DeleteLeadItem(LeadItem leadItem, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (trackChanges)
                LeadsHistoryService.TrackLeadItemChanges(leadItem.LeadId, leadItem, null, changedBy);

            SQLDataAccess.ExecuteNonQuery("Delete From [Order].[LeadItem] Where LeadItemId=@LeadItemId",
                CommandType.Text, new SqlParameter("@LeadItemId", leadItem.LeadItemId));
        }

        #endregion

        public static void OnLeadChanged(Lead prevState, Lead lead, List<ChangeHistory> currentHistory = null, ChangedBy changedBy = null)
        {
            if (changedBy == null && CustomerContext.CurrentCustomer != null)
                changedBy = new ChangedBy(CustomerContext.CurrentCustomer);

            var customersToNotify = new Dictionary<Guid, Customer>();
            var notifiedCustomerIds = new List<Guid>();

            Manager leadManager = lead.ManagerId.HasValue
                ? ManagerService.GetManager(lead.ManagerId.Value)
                : null;

            // сменился исполнитель
            if (lead.ManagerId.HasValue && prevState.ManagerId != lead.ManagerId && leadManager != null)
            {
                notifiedCustomerIds.Add(leadManager.CustomerId);

                var mailTpl = new LeadAssignedMailTemplate(lead);
                mailTpl.BuildMail();

                SendMails(mailTpl, new Dictionary<Guid, Customer> {{leadManager.CustomerId, leadManager.Customer}});
            }
            else if (leadManager != null)
                customersToNotify.TryAddValue(leadManager.CustomerId, leadManager.Customer);



            var history = ChangeHistoryService.GetChanges(lead.Id, ChangeHistoryObjType.Lead, prevState, lead, changedBy);
            if (currentHistory != null)
                history.AddRange(currentHistory);
            var changesTable = BuildLeadChangesTable(history, prevState, lead);

            SendOnLeadChanged(lead, changedBy, changesTable, customersToNotify, notifiedCustomerIds);
            BizProcessExecuter.LeadChanged(lead);
        }

        public static void OnLeadChanged(Lead lead, List<ChangeHistory> history, ChangedBy changedBy = null)
        {
            if (changedBy == null && CustomerContext.CurrentCustomer != null)
                changedBy = new ChangedBy(CustomerContext.CurrentCustomer);

            var changesTable = BuildLeadChangesTable(history, null, null);

            SendOnLeadChanged(lead, changedBy, changesTable);
            BizProcessExecuter.LeadChanged(lead);
        }


        private static void SendOnLeadChanged(Lead lead, ChangedBy changedBy, string changesTable, 
            Dictionary<Guid, Customer> customersToNotify = null,
            List<Guid> notifiedCustomerIds = null)
        {
            if (changesTable.IsNullOrEmpty())
                return;

            if (customersToNotify == null)
            {
                customersToNotify = new Dictionary<Guid, Customer>();
                if (lead.ManagerId.HasValue)
                {
                    var leadManager = ManagerService.GetManager(lead.ManagerId.Value);
                    if (leadManager != null)
                        customersToNotify.TryAddValue(leadManager.CustomerId, leadManager.Customer);
                }
            }

            if (notifiedCustomerIds == null)
                notifiedCustomerIds = new List<Guid>();

            var salesFunnel = SalesFunnelService.Get(lead.SalesFunnelId);
            if (salesFunnel != null && !salesFunnel.NotSendNotificationsOnLeadChanged) // 
            {
                SalesFunnelService.GetSalesFunnelManagers(lead.SalesFunnelId)
                    .ForEach(x => customersToNotify.TryAddValue(x.CustomerId, x.Customer));
            }

            var mailTemplate = new LeadChangedMailTemplate(lead,
                changesTable: changesTable,
                modifier: changedBy != null ? changedBy.Name : string.Empty);
            mailTemplate.BuildMail();

            SendMails(mailTemplate, customersToNotify, notifiedCustomerIds);
        }

        private static void SendMails(MailTemplate mailTemplate, Dictionary<Guid, Customer> customersToNotify, List<Guid> excludingCustomerIds = null)
        {
            var temp =
                customersToNotify.Where(
                        x =>
                            (CustomerContext.CurrentCustomer != null && x.Key != CustomerContext.CustomerId)
                            && x.Value != null
                            && x.Value.Enabled
                            && x.Value.HasRoleAction(RoleAction.Crm)
                            && (excludingCustomerIds == null || !excludingCustomerIds.Contains(x.Key)))
                    .ToList();
            
            foreach (var item in temp)
            {
                MailService.SendMailNow(item.Key, item.Value.EMail, mailTemplate, lettercount: temp.Count, logging: false);
            }
        }

        private static string BuildLeadChangesTable(List<ChangeHistory> history, Lead prevState, Lead lead)
        {
            var sbRows = new StringBuilder();

            if (history != null)
            {
                foreach (var changeHistory in history)
                {
                    sbRows.Append(GetHtmlRow(changeHistory.ParameterName,
                        StringHelper.GenerateDiffHtml(changeHistory.OldValue ?? string.Empty,
                            changeHistory.NewValue ?? string.Empty)));
                }
            }

            if (prevState != null && lead != null)
            {
                var deletedAttachments = prevState.Attachments.Select(x => x.FileName)
                    .Where(x => !lead.Attachments.Select(y => y.FileName).Contains(x)).ToList();
                var newAttachments = lead.Attachments
                    .Where(x => !prevState.Attachments.Select(y => y.FileName).Contains(x.FileName))
                    .Select(x => GetLinkHTML(x.Path, x.FileName)).ToList();

                if (deletedAttachments.Any() || newAttachments.Any())
                    sbRows.Append(GetHtmlChangesRow(
                        LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Attachments"),
                        deletedAttachments.AggregateString(", "), newAttachments.AggregateString(", ")));
            }

            var rowsHtml = sbRows.ToString();
            if (rowsHtml.IsNullOrEmpty())
                return string.Empty;

            return string.Format("<table>{0}</table>",
                rowsHtml);
        }

        private static string GetHtmlRow(string fieldName, string value)
        {
            return $"<tr><td style='color: #acacac; padding: 5px 15px 5px 0;'>{fieldName}:</td><td>{value}</td></tr>";
        }

        private static string GetHtmlChangesRow(string fieldName, string oldValue, string newValue)
        {
            return GetHtmlRow(fieldName, string.Format("{0} {1}", GetHtmlOldValue(oldValue), GetHtmlNewValue(newValue)));
        }

        private static string GetHtmlNewValue(string value)
        {
            return value.IsNotEmpty() ? string.Format("<span style='background-color:#ddfade;'>{0}</span>", value) : string.Empty;
        }

        private static string GetHtmlOldValue(string value)
        {
            return value.IsNotEmpty() ? string.Format("<span style='background-color:#ffe7e7;text-decoration:line-through;'>{0}</span>", value) : string.Empty;
        }

        private static string GetLinkHTML(string url, string name)
        {
            return string.Format("<a href=\"{0}\">{1}</a>", url, name);
        }

        public static List<Lead> GetLeadsForAutocomplete(string query)
        {
            if (query.IsDecimal())
            {
                return SQLDataAccess.Query<Lead>(
                    "SELECT * FROM [Order].[Lead] " +
                    "Left Join [Customers].[Customer] on [Lead].[CustomerId] = [Customer].[CustomerId] " +
                    "WHERE convert(nvarchar,[id]) LIKE '%' + @q + '%' " +
                    "OR [Customer].[Phone] LIKE '%' + @q + '%' " +
                    "OR [Customer].[StandardPhone] LIKE '%' + @q + '%' " +
                    "Order by [Lead].Id desc",
                    new { q = query }).ToList();
            }

            return SQLDataAccess.Query<Lead>(
                "SELECT * FROM [Order].[Lead] " +
                "Left Join [Customers].[Customer] on [Lead].[CustomerId] = [Customer].[CustomerId] " +
                "WHERE convert(nvarchar,[id]) LIKE '%' + @q + '%' " +
                "OR [Lead].[Description] LIKE '%' + @q + '%' " +
                "OR [Customer].[Email] LIKE @q + '%' " +
                "OR [Customer].[FirstName] LIKE @q + '%' " +
                "OR [Customer].[LastName] LIKE @q + '%' " +
                "OR [Customer].[Phone] LIKE '%' + @q + '%' " +
                "Order by [Lead].Id desc",
                new { q = query }).ToList();
        }


        public static string GenerateHtmlLeadItemsTable(IList<LeadItem> leadItems, LeadCurrency leadCurrency)
        {
            var htmlOrderTable = new StringBuilder();

            htmlOrderTable.Append("<table class='orders-table' style='border-collapse: collapse; width: 100%;'>");
            htmlOrderTable.Append("<tr class='orders-table-header'>");
            htmlOrderTable.AppendFormat("<th class='photo' style='border-bottom: 1px solid #e3e3e3; margin-right: 15px; padding: 20px 0; padding-left: 20px; text-align: left;'>{0}</th>", LocalizationService.GetResource("Core.Lead.Letter.Goods"));
            htmlOrderTable.Append("<th class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left; width: 50%;'></th>");
            htmlOrderTable.AppendFormat("<th class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</th>", LocalizationService.GetResource("Core.Lead.Letter.Price"));
            htmlOrderTable.AppendFormat("<th class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;' >{0}</th>", LocalizationService.GetResource("Core.Lead.Letter.Count"));
            htmlOrderTable.AppendFormat("<th class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}<span class='curency' style='font-weight: normal;'> ({1})</span></th>", LocalizationService.GetResource("Core.Lead.Letter.Cost"), CurrencyService.CurrentCurrency.Symbol);
            htmlOrderTable.Append("</tr>");

            var currency = new Currency
            {
                Iso3 = leadCurrency.CurrencyCode,
                NumIso3 = leadCurrency.CurrencyNumCode,
                IsCodeBefore = leadCurrency.IsCodeBefore,
                Rate = leadCurrency.CurrencyValue,
                Symbol = leadCurrency.CurrencySymbol,
                RoundNumbers = leadCurrency.RoundNumbers,
                EnablePriceRounding = leadCurrency.EnablePriceRounding
            };
            // Добавление заказанных товаров
            foreach (var item in leadItems)
            {
                if (item.ProductId.HasValue)
                {
                    htmlOrderTable.Append("<tr>");
                    if (item.ProductId != null)
                    {
                        Photo photo;
                        if (item.PhotoId.HasValue && item.PhotoId != 0 && (photo = PhotoService.GetPhoto((int)item.PhotoId)) != null)
                        {
                            htmlOrderTable.AppendFormat("<td class='photo' style='border-bottom: 1px solid #e3e3e3; margin-right: 15px; padding: 20px 0; padding-left: 20px; text-align: left;'><img src='{0}' /></td>",
                                                         FoldersHelper.GetImageProductPath(ProductImageType.XSmall, photo.PhotoName, false), SettingsPictureSize.XSmallProductImageWidth);
                        }
                        else
                        {
                            htmlOrderTable.AppendFormat("<td>&nbsp;</td>");
                        }
                    }

                    var product = item.ProductId.HasValue ? ProductService.GetProduct(item.ProductId.Value) : null;

                    htmlOrderTable.AppendFormat("<td class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left; width: 50%;'>" +
                                                        "<div class='description' style='display: inline-block;'>" +
                                                            "{0} {1} {2} {3}" +
                                                        "</div>" +
                                                "</td>",

                                                         "<div class='prod-name' style='font-size: 18px; font-weight: bold; margin-bottom: 5px;'><a href='" +
                                                            (product != null ?
                                                                SettingsMain.SiteUrl.Trim('/') + "/" + UrlService.GetLink(ParamType.Product, product.UrlPath, product.ProductId)
                                                            : "") +
                                                            "' class='cs-link' style='color: #0764c3; text-decoration: none;'>" + item.Name + "</a></div>",
                                                            item.Color.IsNotEmpty() ? "<div class='prod-option' style='margin-bottom: 5px;'><span class='cs-light' style='color: #acacac;'>" + SettingsCatalog.ColorsHeader + ":</span><span class='value cs-link' style='color: #0764c3; font-weight: bold; padding-left: 10px;'>" + item.Color + "</span></div>" : "",
                                                            item.Size.IsNotEmpty() ? "<div class='prod-option' style='margin-bottom: 5px;'><span class='cs-light' style='color: #acacac;'>" + SettingsCatalog.SizesHeader + ":</span><span class='value cs-link' style='color: #0764c3; font-weight: bold; padding-left: 10px;'>" + item.Size + "</span></div>" : "",
                                                            string.Empty);
                    htmlOrderTable.AppendFormat("<td class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", PriceFormatService.FormatPrice(item.Price, currency));
                    htmlOrderTable.AppendFormat("<td class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", item.Amount);
                    htmlOrderTable.AppendFormat("<td class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", PriceFormatService.FormatPrice(item.Price * item.Amount, currency));
                    htmlOrderTable.Append("</tr>");
                }
            }

            // Стоимость заказа
            htmlOrderTable.Append("<tr>");
            htmlOrderTable.AppendFormat("<td class='footer-name' colspan='4' style='border-bottom: none; font-weight: bold; padding: 10px 0; text-align: right;'>{0}:</td>", LocalizationService.GetResource("Core.Lead.Letter.OrderCost"));
            htmlOrderTable.AppendFormat("<td class='footer-value' style='border-bottom: none; font-weight: bold; padding: 10px 0; text-align: center;'>{0}</td>", PriceFormatService.FormatPrice(leadItems.Sum(item => item.Price * item.Amount), currency));
            htmlOrderTable.Append("</tr>");

            var total = leadItems.Sum(item => item.Price * item.Amount);
            if (total < 0) total = 0;

            // Итого
            htmlOrderTable.Append("<tr>");
            htmlOrderTable.AppendFormat("<td class='footer-name' colspan='4' style='border-bottom: none; font-weight: bold; padding: 10px 0; text-align: right;'>{0}:</td>", LocalizationService.GetResource("Core.Lead.Letter.OrderTotal"));
            htmlOrderTable.AppendFormat("<td class='footer-value' style='border-bottom: none; font-weight: bold; padding: 10px 0; text-align: center;'>{0}</td>", PriceFormatService.FormatPrice(total, currency));
            htmlOrderTable.Append("</tr>");

            htmlOrderTable.Append("</table>");

            return htmlOrderTable.ToString();
        }

        #region Statistics

        public static Dictionary<DateTime, float> GetLeadsCountStatistics(string group, DateTime minDate, DateTime maxDate)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [CreatedDate]), 0) as 'Date', Count([Id]) as Count " +
                "FROM [Order].[Lead] " +
                "WHERE [CreatedDate] > @MinDate and [CreatedDate] <= @MaxDate " +
                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [CreatedDate]), 0)",
                CommandType.Text,
                "Date", "Count",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate));
        }

        [Obsolete("LeadStatus not exist")]
        public static Dictionary<string, int> GetLeadsStatusesStatistics(DateTime minDate, DateTime maxDate)
        {
            return SQLDataAccess.ExecuteReadDictionary<string, int>(
                "Select LeadStatus, Count([Id]) as Count " +
                "FROM [Order].[Lead] " +
                "WHERE [CreatedDate] > @MinDate and [CreatedDate] <= @MaxDate " +
                "GROUP BY LeadStatus",
                CommandType.Text,
                "LeadStatus", "Count",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate));
        }

        public static int GetLeadsCountByListId(int leadsListId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(Id) From [Order].[Lead] Where [SalesFunnelId] = @SalesFunnelId",
                CommandType.Text,
                new SqlParameter("@SalesFunnelId", leadsListId)
                );
        }

        public static int GetLeadsListDealsCount(int leadsListId, SalesFunnelStatusType dealStatus)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count([Lead].[Id]) From [Order].[Lead] Inner Join [Crm].[DealStatus] On [DealStatus].[Id] = [Lead].[DealStatusId] Where [SalesFunnelId] = @SalesFunnelId and [DealStatus].[Status] = @Status",
                CommandType.Text,
                new SqlParameter("@SalesFunnelId", leadsListId),
                new SqlParameter("@Status", dealStatus)
                );
        }

        public static float GetLeadsListDealsSum(int leadsListId, SalesFunnelStatusType dealStatus)
        {
            return SQLDataAccess.ExecuteScalar<float>(
                "Select ISNULL(Sum([Lead].[Sum]*ISNULL([LeadCurrency].[CurrencyValue],1)), 0) From [Order].[Lead] Inner Join [Crm].[DealStatus] On [DealStatus].[Id] = [Lead].[DealStatusId] left join [Order].[LeadCurrency] ON [Lead].[Id] = [LeadCurrency].[LeadId] Where [SalesFunnelId] = @SalesFunnelId and [DealStatus].[Status] = @Status and Sum IS NOT NULL",
                CommandType.Text,
                new SqlParameter("@SalesFunnelId", leadsListId),
                new SqlParameter("@Status", (int)dealStatus)
                );
        }

        public static List<LeadsListSourceItem> GetLeadsListSources(int leadsListId)
        {
            return SQLDataAccess.Query<LeadsListSourceItem>(
                "SELECT  [OrderSource].[Id] as OrderSourceId,[OrderSource].[Name], Count([Lead].[Id]) as LeadsCount FROM[Order].[OrderSource] " +
                "Inner Join[Order].[Lead] on[Lead].[OrderSourceId] = [OrderSource].[Id] " +
                "WHERE[Lead].SalesFunnelId = @SalesFunnelId " +
                "Group by[OrderSource].[Id], [OrderSource].[Name]",
              new { SalesFunnelId = leadsListId }).ToList();
        }

        public static Dictionary<DateTime, int> GetLeadsCountByDays(DateTime minDate, DateTime maxDate, int leadsListId)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, int>(
                "Select DATEADD(dd, 0, DATEDIFF(dd, 0, [CreatedDate])) as 'Date', " +
                "COUNT([Lead].[Id]) as 'LeadsCount' " +
                "FROM[Order].[Lead] Inner Join [Order].[OrderSource] On [Lead].[OrderSourceId] = [OrderSource].[Id]" +
                "WHERE [SalesFunnelId] = @SalesFunnelId and [CreatedDate] > @MinDate and [CreatedDate] < @MaxDate " +                
                "GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, [CreatedDate]))",
                CommandType.Text,
                "Date", "LeadsCount",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate),
                new SqlParameter("@SalesFunnelId", leadsListId));          
        }

        #endregion

        public static bool CheckAccess(Lead lead)
        {
            return CheckAccess(lead, CustomerContext.CurrentCustomer);
        }

        public static bool CheckAccess(Lead lead, Customer customer)
        {
            if (customer.IsAdmin || customer.IsVirtual)
                return true;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.All)
                        return true;

                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.Assigned &&
                        lead.ManagerId == manager.ManagerId)
                        return true;

                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.AssignedAndFree &&
                        (lead.ManagerId == manager.ManagerId || lead.ManagerId == null))
                        return true;
                }
            }
            return false;
        }

        public static List<ISubscriber> GetLeadEmailsBySalesFunnel(int salesFunnelId)
        {
            return SQLDataAccess.ExecuteReadList<ISubscriber>(
                "Select * From [Order].[Lead] Where SalesFunnelId=@salesFunnelId",
                CommandType.Text,
                (reader) => new Subscription
                {
                    Email = SQLDataHelper.GetString(reader, "Email"),
                    FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                    LastName = SQLDataHelper.GetString(reader, "LastName"),
                    Phone = SQLDataHelper.GetString(reader, "Phone"),
                    CustomerType = EMailRecipientType.LeadCustomer
                },
                new SqlParameter("@salesFunnelId", salesFunnelId));
        }

        public static void CompleteLeadsOnOrderPaid(Order order)
        {
            CompleteLeads(order.OrderCustomer.CustomerID, ELeadAutoCompleteActionType.OrderPaid, order.Number,
                checkProductsFunc: (productIds) => productIds.All(id => order.OrderItems.Any(oi => oi.ProductID == id)),
                checkCategoriesFunc: (categoryIds) => AnyProductInAnyCategory(order.OrderItems.Select(x => x.ProductID).Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList(), categoryIds));
        }

        public static void CompleteLeadsOnNewOrder(Order order)
        {
            CompleteLeads(order.OrderCustomer.CustomerID, ELeadAutoCompleteActionType.OrderCreated, order.Number,
                checkProductsFunc: (productIds) => productIds.All(id => order.OrderItems.Any(oi => oi.ProductID == id)),
                checkCategoriesFunc: (categoryIds) => AnyProductInAnyCategory(order.OrderItems.Select(x => x.ProductID).Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList(), categoryIds));
        }

        public static void CompleteLeadsOnNewBooking(Booking.Booking booking)
        {
            if (!booking.CustomerId.HasValue)
                return;

            CompleteLeads(booking.CustomerId.Value, ELeadAutoCompleteActionType.BookingCreated, booking.Id.ToString());
        }

        private static void CompleteLeads(Guid customerId, ELeadAutoCompleteActionType actionType, string objId,
            Func<List<int>, bool> checkProductsFunc = null,
            Func<List<int>, bool> checkCategoriesFunc = null)
        {
            if (actionType == ELeadAutoCompleteActionType.None)
                return;

            var leads = GetOpenLeadsByCustomer(customerId);
            if (!leads.Any())
                return;

            var funnels = SalesFunnelService.GetByLeadAutoCompleteAction(actionType);
            foreach (var funnel in funnels)
            {
                var funnelLeads = leads.Where(x => x.SalesFunnelId == funnel.Id).ToList();
                if (!funnelLeads.Any())
                    continue;

                if (checkProductsFunc != null)
                {
                    var productIds = SalesFunnelService.GetLeadAutoCompleteProductIds(funnel.Id);
                    if (productIds.Any() && !checkProductsFunc(productIds))
                        continue;
                }

                if (checkCategoriesFunc != null)
                {
                    var categoryIds = SalesFunnelService.GetLeadAutoCompleteCategoryIds(funnel.Id);
                    if (categoryIds.Any() && !checkCategoriesFunc(categoryIds))
                        continue;
                }

                var dealStatuses = DealStatusService.GetList(funnel.Id);
                var cancelStatus = dealStatuses.FirstOrDefault(x => x.Status == SalesFunnelStatusType.Canceled);
                var completeStatus = dealStatuses.FirstOrDefault(x => x.Status == SalesFunnelStatusType.FinalSuccess);

                if (cancelStatus == null || completeStatus == null)
                {
                    Debug.Log.Warn(LocalizationService.GetResourceFormat("Core.Crm.Leads.LeadAutocompetion.NoSystemDealStatuses", funnel.Name));
                    return;
                }

                for (int i = 0; i < funnelLeads.Count; i++)
                {
                    var changeBy = new ChangedBy(string.Format(actionType.DescriptionKey(), funnelLeads[i].Id, objId));

                    if (i == funnelLeads.Count - 1) // успешно завершен последний лид
                    {
                        Order order = null;
                        switch (actionType)
                        {
                            case ELeadAutoCompleteActionType.OrderCreated:
                            case ELeadAutoCompleteActionType.OrderPaid:
                                order = OrderService.GetOrderByNumber(objId);
                                if (order != null && !order.LeadId.HasValue)
                                {
                                    order.LeadId = funnelLeads[i].Id;
                                    OrderService.UpdateOrderMain(order, changedBy: new OrderChangedBy(changeBy.Name));
                                }
                                break;
                            case ELeadAutoCompleteActionType.BookingCreated:
                                break;
                        }
                        if (funnel.FinalSuccessAction == SalesFunnelFinalSuccessAction.None || order != null)
                        {
                            funnelLeads[i].DealStatusId = completeStatus.Id;
                            UpdateLead(funnelLeads[i], false, changeBy);
                        }
                        else if (order == null)
                        {
                            OrderService.CreateOrder(funnelLeads[i], changeBy, false);
                        }
                    }
                    else // остальные лиды отклонены
                    {
                        funnelLeads[i].DealStatusId = cancelStatus.Id;
                        UpdateLead(funnelLeads[i], false, changeBy);
                    }
                }
            }
        }

        private static bool AnyProductInAnyCategory(List<int> productIds, List<int> categoryIds)
        {
            foreach (var productId in productIds)
            {
                foreach (var categoryId in ProductService.GetCategoriesIDsByProductId(productId, false).ToList())
                {
                    var parentIds = CategoryService.GetParentCategoryIds(categoryId);
                    if (parentIds.Any(id => categoryIds.Contains(id)))
                        return true;
                }
            }
            return false;
        }

        public static void UpdateLeadCustomerIdByEmailPhone(Guid customerId, string email, string phone, long? standardPhone)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Order].[Lead] " +
                "Set CustomerId=@CustomerId " +
                "Where CustomerId is null and ((@Email <> '' and Email=@Email) or (@Phone <> '' and Phone=@Phone))",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@Email", email ?? ""),
                new SqlParameter("@Phone", phone ?? "")
            );
        }
    }
}
