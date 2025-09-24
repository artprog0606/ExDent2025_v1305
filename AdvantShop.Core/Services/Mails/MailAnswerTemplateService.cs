//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Mails
{
    public class MailAnswerTemplateService
    {
        public MailAnswerTemplate Get(int id, bool onlyActive)
        {
            return SQLDataAccess.Query<MailAnswerTemplate>(
                "SELECT * FROM [Settings].[MailTemplate] WHERE TemplateId = @id" + (onlyActive ? " AND Active=1" : string.Empty),
                new { id = id }).FirstOrDefault();
        }

        public List<MailAnswerTemplate> Gets(bool onlyActive)
        {
            return
                SQLDataAccess.ExecuteReadList<MailAnswerTemplate>(
                    "SELECT * FROM [Settings].[MailTemplate] " + (onlyActive ? " Where Active=1 " : string.Empty) + "Order By SortOrder",
                    CommandType.Text,
                    GetFromReader);
        }

        public void Update(MailAnswerTemplate mailTemplate)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Settings].[MailTemplate] SET Name = @Name, Body = @Body, SortOrder = @SortOrder, Active = @Active, Subject = @Subject WHERE TemplateId = @TemplateId",
                CommandType.Text,
                new SqlParameter("@TemplateId", mailTemplate.TemplateId),
                new SqlParameter("@Name", mailTemplate.Name),
                new SqlParameter("@Body", mailTemplate.Body),
                new SqlParameter("@Subject", mailTemplate.Subject),
                new SqlParameter("@SortOrder", mailTemplate.SortOrder),
                new SqlParameter("@Active", mailTemplate.Active));
        }

        public int Add(MailAnswerTemplate mailTemplate)
        {
            mailTemplate.TemplateId =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Settings].[MailTemplate] (Name, Body, SortOrder, Active, Subject) VALUES (@Name, @Body, @SortOrder, @Active, @Subject); SELECT SCOPE_IDENTITY()",
                    CommandType.Text,
                    new SqlParameter("@TemplateId", mailTemplate.TemplateId),
                    new SqlParameter("@Name", mailTemplate.Name),
                    new SqlParameter("@Body", mailTemplate.Body),
                    new SqlParameter("@Subject", mailTemplate.Subject),
                    new SqlParameter("@SortOrder", mailTemplate.SortOrder),
                    new SqlParameter("@Active", mailTemplate.Active));

            return mailTemplate.TemplateId;
        }

        public void Delete(int templateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[MailTemplate] WHERE TemplateId = @TemplateId",
                CommandType.Text, new SqlParameter("@TemplateId", templateId));
        }

        public string FormatLetter(string body, string firstName, string lastName, string patronymicName,
                                   string shopName, string trackNumber, string text, string managerName, string managerSign, 
                                   Guid? customerId, int? orderId, int? leadId)
        {
            var format = new StringBuilder(body);
            format = format.Replace("#FIRSTNAME#", firstName)
                .Replace("#LASTNAME#", lastName)
                .Replace("#PATRONYMIC#", patronymicName)
                .Replace("#STORE_NAME#", shopName)
                .Replace("#TEXT#", text)
                .Replace("#MANAGER_NAME#", managerName)
                .Replace("#MANAGER_SIGN#", (managerSign ?? string.Empty).Replace("\n", "<br />"));

            if (customerId != null && body.Contains("#LAST_ORDER_NUMBER#"))
            {
                var lastOrder = OrderService.GetCustomerOrderHistory(customerId.Value).FirstOrDefault();
                if (lastOrder != null)
                    format = format.Replace("#LAST_ORDER_NUMBER#", lastOrder.OrderNumber);
            }

            Order order = null;

            if (orderId != null && orderId != 0)
                order = OrderService.GetOrder(orderId.Value);
            
            OrderMailService.FormatOrderLetter(order, format, body);

            Lead lead = null;
            
            if (leadId != null && leadId != 0)
                lead = LeadService.GetLead(leadId.Value);
            
            MailFormatService.FormatLead(lead, format, body);

            return format.ToString();
        }

        private MailAnswerTemplate GetFromReader(SqlDataReader reader)
        {
            {
                return new MailAnswerTemplate
                {
                    TemplateId = SQLDataHelper.GetInt(reader, "TemplateId"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    Body = SQLDataHelper.GetString(reader, "Body"),
                    Subject = SQLDataHelper.GetString(reader, "Subject"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    Active = SQLDataHelper.GetBoolean(reader, "Active")
                };
            }
        }
    }
}
