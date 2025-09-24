using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Localization;

namespace AdvantShop.Core.Services.Crm
{
    public enum LeadEventType
    {
        [Localize("Core.Services.Crm.LeadEventType.Orher")]
        None = 0,

        [Localize("Core.Services.Crm.LeadEventType.Comment")]
        Comment = 1,

        [Localize("Core.Services.Crm.LeadEventType.Call")]
        Call = 2,

        [Localize("Core.Services.Crm.LeadEventType.SMS")]
        Sms = 3,

        [Localize("Core.Services.Crm.LeadEventType.Email")]
        Email = 4,

        [Localize("Core.Services.Crm.LeadEventType.Task")]
        Task = 5,

        [Localize("Core.Services.Crm.LeadEventType.Vk")]
        Vk = 6,

        [Localize("Core.Services.Crm.LeadEventType.Instagram")]
        Instagram = 7,

        [Localize("Core.Services.Crm.LeadEventType.Facebook")]
        Facebook = 8,

        [Localize("Core.Services.Crm.LeadEventType.History")]
        History = 9,

        [Localize("Core.Services.Crm.LeadEventType.Telegram")]
        Telegram = 10,

        [Localize("Core.Services.Crm.LeadEventType.Review")]
        Review = 11,

        [Localize("Core.Services.Crm.LeadEventType.Ok")]
        Ok = 12,

        [Localize("")]
        Other = 1000,
    }

    public class LeadEvent
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public LeadEventType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted { get { return Culture.ConvertDate(CreatedDate); } }
        public string CreatedDateShort { get { return CreatedDate.ToString("HH:mm"); } }

        public string FromCreatedDate { get { return StringHelper.FormatDateTimeInterval(CreatedDate); } }
        public string CreatedBy { get; set; }
        public Guid? CreatedById { get; set; }
        public bool CreatedByIsModerator { get; set; }

        public int? TaskId { get; set; }
        
        public LeadEvent()
        {
            CreatedDate = DateTime.Now;
        }

        public LeadEvent(Customer customer) : this()
        {
            if (customer != null)
            {
                CreatedBy = customer.GetShortName();
                CreatedById = customer.Id;
            }
        }
    }
}
