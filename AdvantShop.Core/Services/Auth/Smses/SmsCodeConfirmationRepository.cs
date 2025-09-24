using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Auth.Smses
{
    public class SmsCodeConfirmationRepository
    {
        public const string TestEmailPrefix = "@sms.temp";
        
        public void AddSmsCode(long phone, string code)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Customers.SmsCodeConfirmation (Phone, Code, IsUsed, DateAdded) Values (@Phone, @Code, @IsUsed, @DateAdded)",
                CommandType.Text,
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Code", code),
                new SqlParameter("@IsUsed", false),
                new SqlParameter("@DateAdded", DateTime.Now));
        }

        public SmsCodeConfirmation GetSmsCode(long phone, string code)
        {
            return SQLDataAccess.Query<SmsCodeConfirmation>(
                "Select top(1) * from Customers.SmsCodeConfirmation Where Phone=@phone and [Code]=@code and IsUsed=0 Order By DateAdded Desc",
                new {phone, code}).FirstOrDefault();
        }
        
        public void SetSmsCodeUsed(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Customers.SmsCodeConfirmation Set IsUsed=1 Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public void ClearSmsCodes()
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Customers.SmsCodeConfirmation Where DateAdded < @DateAdded", 
                CommandType.Text, 60*5,
                new SqlParameter("@DateAdded", DateTime.Now.AddDays(-14)));
        }
    }
}