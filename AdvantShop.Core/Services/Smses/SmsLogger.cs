using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Services.Loging.Smses;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Smses
{
    public class SmsLogData
    {
        public long Phone { get; private set; }
        public string SmsText { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public string Ip { get; private set; }
        public Guid? CustomerId { get; private set; }
        public SmsStatus Status { get; private set; }
        
        public SmsLogData(){}

        public SmsLogData(long phone, string smsText, string ip, Guid? customerId, SmsStatus status)
        {
            Phone = phone;
            SmsText = smsText;
            Ip = ip;
            CustomerId = customerId;
            Status = status;
        }

        public SmsLogData(long phone, string smsText, DateTime createdOn, string ip, Guid? customerId, SmsStatus status)
                        : this(phone, smsText, ip, customerId, status)
        {
            CreatedOn = createdOn;
        }
    }
    
    public class SmsLogger
    {
        public static void Log(SmsLogData smsLogData)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Customers.SmsLog (Phone, SmsText, CreatedOn, Ip, CustomerId, Status) Values (@Phone, @SmsText, @CreatedOn, @Ip, @CustomerId, @Status)",
                CommandType.Text,
                new SqlParameter("@Phone", smsLogData.Phone),
                new SqlParameter("@SmsText", smsLogData.SmsText),
                new SqlParameter("@CreatedOn", DateTime.Now),
                new SqlParameter("@Ip", smsLogData.Ip ?? ""),
                new SqlParameter("@CustomerId", smsLogData.CustomerId ?? (object) DBNull.Value),
                new SqlParameter("@Status", (int) smsLogData.Status)
            );
        }
        
        public static List<SmsLogData> GetLastSmsLogsByPhoneOrIp(double seconds, long phone, string ip)
        {
            return SQLDataAccess
                .Query<SmsLogData>("Select Phone, CreatedOn, Ip, Status From Customers.SmsLog Where (Phone = @phone or Ip = @ip) and CreatedOn >= @dateFrom Order by Id",
                    new {phone, ip, dateFrom = DateTime.Now.AddSeconds(-seconds) })
                .ToList();
        }
        
        public static List<SmsLogData> GetLogsByIpAndSeconds(string ip, double seconds)
        {
            return SQLDataAccess
                .Query<SmsLogData>("Select Phone, CreatedOn, Ip From Customers.SmsLog Where Ip = @ip and CreatedOn >= @dateFrom",
                    new {ip, dateFrom = DateTime.Now.AddSeconds(-seconds) })
                .ToList();
        }
        
        public static List<SmsLogData> GetLogsByPhoneAndSeconds(long phone, double seconds)
        {
            return SQLDataAccess
                .Query<SmsLogData>("Select Phone, CreatedOn, Ip From Customers.SmsLog Where Phone = @phone and CreatedOn >= @dateFrom",
                    new {phone, dateFrom = DateTime.Now.AddSeconds(-seconds) })
                .ToList();
        }

        public static void ClearLog()
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Customers.SmsLog Where CreatedOn < @date", 
                CommandType.Text, 
                60*5,
                new SqlParameter("@date", DateTime.Now.AddMonths(-4)));
        }
    }
}