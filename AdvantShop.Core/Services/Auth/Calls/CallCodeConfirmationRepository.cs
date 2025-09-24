using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Auth.Calls
{
    public class CallCodeConfirmationRepository
    {
        public void AddCallCode(long phone, string code)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Customers.CallCodeConfirmation (Phone, Code, IsUsed, DateAdded) " +
                " Values (@Phone, @Code, @IsUsed, @DateAdded)",
                CommandType.Text,
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Code", code),
                new SqlParameter("@IsUsed", false),
                new SqlParameter("@DateAdded", DateTime.Now));
        }

        public CallCodeConfirmation GetCallCode(long phone, string code)
        {
            return SQLDataAccess.Query<CallCodeConfirmation>(
                @"Select top(1) * 
                from Customers.CallCodeConfirmation 
                Where Phone=@phone 
                    and [Code]=@code 
                    and IsUsed=0 
                Order By DateAdded Desc",
                new {phone, code}).FirstOrDefault();
        }
        
        public void SetSmsCodeUsed(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Customers.CallCodeConfirmation Set IsUsed=1 Where Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }
    }
}