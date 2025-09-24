using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Core.Services.Payment.QiwiKassa
{
    public class CreateBillInfo
    {
        public MoneyAmount Amount { get; set; }
        public string Comment { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public Customer Customer { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
    }
    public class BillResponse
    {
        public string BillId { get; set; }
        public string SiteId { get; set; }
        public ResponseStatus Status { get; set; }
        public MoneyAmount Amount { get; set; }
        public string Comment { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime CreationDateTime { get; set; }
        public Customer Customer { get; set; }
        public string PayUrl { get; set; }
    }

    public class NotificationBill : IBillNotification
    {
        public BillResponse Bill { get; set; }
        public string Version { get; set; }

        public string GetBillId() => Bill?.BillId;
    }

    public interface IBillNotification
    {
        string GetBillId();
    }

    public class NotificationPayment : IBillNotification
    {
        public Payment Payment { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    
        public string GetBillId() => Payment?.BillId;
    }

    public class Payment
    {
        public string PaymentId { get; set; }
        public string CreatedDateTime { get; set; }
        public MoneyAmount Amount { get; set; }
        public string BillId { get; set; }
    }

    public class MoneyAmount
    {
        private decimal _value;
        public decimal Value
        {
            get
            {
                _value = Math.Round(_value, 2, MidpointRounding.ToEven);
                int[] partsValue = Decimal.GetBits(_value);
                byte scale = (byte) ((partsValue[3] >> 16) & 0x7F);

                if (scale == 0)
                    _value *= 1.00M;
                else if (scale == 1)
                    _value *= 1.0M;

                return _value;
            }
            set { _value = value; }
        }

        public string Currency { get; set; }
    }

    public class Customer
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Account { get; set; }
    }

    public class ResponseStatus
    {
        public DateTime ChangedDateTime { get; set; }
        public EnumStatus Value { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnumStatus
    {
        [EnumMember(Value = "WAITING")]
        Waiting,

        [EnumMember(Value = "PAID")]
        Paid,

        [EnumMember(Value = "REJECTED")]
        Rejected,

        [EnumMember(Value = "EXPIRED")]
        Expired,
    }
}
