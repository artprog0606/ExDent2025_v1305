using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Services.Loging.CallsAuth;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Auth.Calls
{
    public class CallLogData
    {
        public long Phone { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public string Ip { get; private set; }
        public Guid? CustomerId { get; private set; }
        public CallAuthStatus Status { get; private set; }
        
        public CallLogData(){}
        
        public CallLogData(long phone, string ip, Guid? customerId, CallAuthStatus status)
        {
            Phone = phone;
            Ip = ip;
            CustomerId = customerId;
            Status = status;
        }
        
        public CallLogData(long phone,  DateTime createdOn, string ip, Guid? customerId, CallAuthStatus status)
            : this(phone, ip, customerId, status)
        {
            CreatedOn = createdOn;
        }
    }

    public class CallLogger
    {
        public static void Log(CallLogData smsLogData)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Customers.CallLog (Phone, CreatedOn, Ip, CustomerId, Status) Values (@Phone, @CreatedOn, @Ip, @CustomerId, @Status)",
                CommandType.Text,
                new SqlParameter("@Phone", smsLogData.Phone),
                new SqlParameter("@CreatedOn", DateTime.Now),
                new SqlParameter("@Ip", smsLogData.Ip ?? ""),
                new SqlParameter("@CustomerId", smsLogData.CustomerId ?? (object) DBNull.Value),
                new SqlParameter("@Status", (int) smsLogData.Status)
            );
        }
        
        public static List<CallLogData> GetLastCallLogsByPhoneOrIp(double seconds, long phone, string ip)
        {
            return SQLDataAccess
                .Query<CallLogData>(
                    @"Select 
                        Phone
                        , CreatedOn
                        , Ip
                        , Status 
                    From Customers.CallLog 
                    Where (Phone = @phone or Ip = @ip) 
                      and CreatedOn >= @dateFrom Order by Id",
                    new {phone, ip, dateFrom = DateTime.Now.AddSeconds(-seconds) })
                .ToList();
        }
    }
}