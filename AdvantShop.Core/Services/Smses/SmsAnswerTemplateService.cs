//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Smses
{
    public class SmsAnswerTemplateService
    {
        public SmsAnswerTemplate Get(int id, bool onlyActive)
        {
            return SQLDataAccess.Query<SmsAnswerTemplate>(
                "SELECT * FROM [Settings].[SmsTemplate] WHERE TemplateId = @id" + (onlyActive ? " AND Active=1" : string.Empty),
                new { id = id }).FirstOrDefault();
        }

        public List<SmsAnswerTemplate> Gets(bool onlyActive)
        {
            return
                SQLDataAccess.ExecuteReadList<SmsAnswerTemplate>(
                    "SELECT * FROM [Settings].[SmsTemplate] " + (onlyActive ? " Where Active=1 " : string.Empty) + "Order By SortOrder",
                    CommandType.Text,
                    GetFromReader);
        }

        public void Update(SmsAnswerTemplate smsTemplate)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Settings].[SmsTemplate] SET Name = @Name, Text = @Text, SortOrder = @SortOrder, Active = @Active WHERE TemplateId = @TemplateId",
                CommandType.Text,
                new SqlParameter("@TemplateId", smsTemplate.TemplateId),
                new SqlParameter("@Name", smsTemplate.Name),
                new SqlParameter("@Text", smsTemplate.Text),
                new SqlParameter("@SortOrder", smsTemplate.SortOrder),
                new SqlParameter("@Active", smsTemplate.Active));
        }

        public int Add(SmsAnswerTemplate smsTemplate)
        {
            smsTemplate.TemplateId =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Settings].[SmsTemplate] (Name, Text, SortOrder, Active) VALUES (@Name, @Text, @SortOrder, @Active); SELECT SCOPE_IDENTITY()",
                    CommandType.Text,
                    new SqlParameter("@TemplateId", smsTemplate.TemplateId),
                    new SqlParameter("@Name", smsTemplate.Name),
                    new SqlParameter("@Text", smsTemplate.Text),
                    new SqlParameter("@SortOrder", smsTemplate.SortOrder),
                    new SqlParameter("@Active", smsTemplate.Active));

            return smsTemplate.TemplateId;
        }

        public void Delete(int templateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[SmsTemplate] WHERE TemplateId = @TemplateId",
                CommandType.Text, new SqlParameter("@TemplateId", templateId));
        }

        private SmsAnswerTemplate GetFromReader(SqlDataReader reader)
        {
            {
                return new SmsAnswerTemplate
                {
                    TemplateId = SQLDataHelper.GetInt(reader, "TemplateId"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    Text = SQLDataHelper.GetString(reader, "Text"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    Active = SQLDataHelper.GetBoolean(reader, "Active")
                };
            }
        }
    }
}
