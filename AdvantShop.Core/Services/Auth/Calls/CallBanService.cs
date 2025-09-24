using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.Auth.Calls
{
    public class CallBanService
    {
        public static void Ban(long? phone, string ip, DateTime untilDate)
        {
            if (phone == null && string.IsNullOrEmpty(ip))
                return;
            
            var previousBansCount = GetPreviousBansCount(phone, ip);
            if (previousBansCount > 2)
                untilDate = untilDate.AddHours(previousBansCount);
            
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Customers.CallBan (Phone, Ip, UntilDate) Values (@Phone, @Ip, @UntilDate)",
                CommandType.Text,
                new SqlParameter("@Phone", phone ?? (object) DBNull.Value),
                new SqlParameter("@Ip", ip ?? (object) DBNull.Value),
                new SqlParameter("@UntilDate", untilDate)
            );
            
            Debug.Log.Info($"call auth ban {phone} {ip} {untilDate}");
        }
        
        public static int GetPreviousBansCount(long? phone, string ip)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                @"Select Count(*) 
                From Customers.CallBan 
                Where Phone=@Phone 
                  and Ip=@Ip 
                  and UntilDate > @UntilDate",
                CommandType.Text,
                new SqlParameter("@Phone", phone ?? (object) DBNull.Value),
                new SqlParameter("@Ip", ip ?? (object) DBNull.Value),
                new SqlParameter("@UntilDate", DateTime.Now.AddDays(-3))
            );
        }
        
        public static bool IsBannedByPhoneOrIp(long phone, string ip)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                @"If exists (
                    Select 1 
                    From Customers.CallBan 
                    Where ((Phone=@Phone and Ip=@Ip) or (@Ip <> '' and Ip=@Ip and Phone is null)) 
                        and UntilDate > @UntilDate) 
                Select 1 Else Select 0",
                CommandType.Text,
                new SqlParameter("@Phone", phone),
                new SqlParameter("@Ip", ip ?? ""),
                new SqlParameter("@UntilDate", DateTime.Now)) == 1;
        }
    }
}