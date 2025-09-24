using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Landing.LandingEmails
{
    public class LandingEmailTemplateService
    {
        public LandingEmailTemplate Get(int id)
        {
            return
                SQLDataAccess.Query<LandingEmailTemplate>("Select * From CMS.LandingEmailTemplate Where Id=@id",
                    new {id}).FirstOrDefault();
        }

        public List<LandingEmailTemplate> GetList(int blockId)
        {
            return
                SQLDataAccess.Query<LandingEmailTemplate>(
                    "Select * From CMS.LandingEmailTemplate Where BlockId=@BlockId", new {blockId}).ToList();
        }

        public int Add(LandingEmailTemplate tmpl)
        {
            tmpl.Id = SQLDataAccess.ExecuteScalar<int>(
                "Insert Into CMS.LandingEmailTemplate ([BlockId],[Subject],[Body],[SendingTime]) Values (@BlockId,@Subject,@Body,@SendingTime); Select Scope_identity();",
                CommandType.Text,
                new SqlParameter("@BlockId", tmpl.BlockId),
                new SqlParameter("@Subject", tmpl.Subject ?? ""),
                new SqlParameter("@Body", tmpl.Body ?? ""),
                new SqlParameter("@SendingTime", tmpl.SendingTime));

            return tmpl.Id;
        }

        public void Update(LandingEmailTemplate tmpl)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update CMS.LandingEmailTemplate Set Subject=@Subject, Body=@Body, SendingTime=@SendingTime Where Id=@Id",
                CommandType.Text, 
                new SqlParameter("@Id", tmpl.Id),
                new SqlParameter("@Subject", tmpl.Subject ?? ""),
                new SqlParameter("@Body", tmpl.Body ?? ""),
                new SqlParameter("@SendingTime", tmpl.SendingTime));
        }

        public void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From CMS.LandingEmailTemplate Where Id=@id", CommandType.Text, new SqlParameter("@id", id));
        }
    }
}
