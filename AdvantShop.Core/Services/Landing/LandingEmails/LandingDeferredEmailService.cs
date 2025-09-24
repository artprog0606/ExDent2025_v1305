using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Landing.LandingEmails
{
    public class LandingDeferredEmailService
    {
        public List<LandingDeferredEmail> GetList(DateTime date)
        {
            return
                SQLDataAccess.Query<LandingDeferredEmail>(
                    "Select * From CMS.LandingDeferredEmail Where SendingDate <= @date", new { date }).ToList();
        }

        public int Add(LandingDeferredEmail email)
        {
            email.Id = SQLDataAccess.ExecuteScalar<int>(
                "Insert Into CMS.LandingDeferredEmail ([CustomerId],[Email],[Subject],[Body],[SendingDate]) Values (@CustomerId,@Email,@Subject,@Body,@SendingDate); Select Scope_identity();",
                CommandType.Text,
                new SqlParameter("@CustomerId", email.CustomerId),
                new SqlParameter("@Email", email.Email),
                new SqlParameter("@Subject", email.Subject ?? ""),
                new SqlParameter("@Body", email.Body ?? ""),
                new SqlParameter("@SendingDate", email.SendingDate));

            return email.Id;
        }

        public void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From CMS.LandingDeferredEmail Where Id=@id", CommandType.Text, new SqlParameter("@id", id));
        }

    }
}
