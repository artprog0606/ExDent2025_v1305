using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadEventsModel
    {
        public LeadEventsModel()
        {
            EventGroups = new List<LeadEventGroupModel>();
            ElapsedTask = new Dictionary<string, string>();
        }

        public List<LeadEventGroupModel> EventGroups { get; set; }
        public List<SelectItemModel> EventTypes { get; set; }


        public int EventsCount { get; set; }
        public int ReceivedEmailsCount { get; set; }
        public int SendedEmailsCount { get; set; }
        public int EmailsCount { get { return ReceivedEmailsCount + SendedEmailsCount; } }
        public int SmsCount { get; set; }

        public int InstagramReceivedMessagesCount { get; set; }
        public int InstagramSendedMessagesCount { get; set; }
        public int InstagramMessagesCount { get { return InstagramReceivedMessagesCount + InstagramSendedMessagesCount; } }

        public int FacebookSendedMessagesCount { get; set; }
        public int FacebookReceivedMessagesCount { get; set; }
        public int FacebookMessagesCount { get { return FacebookSendedMessagesCount + FacebookReceivedMessagesCount; } }

        public int InCallsCount { get; set; }
        public int OutCallsCount { get; set; }
        public int OtherCallsCount { get; set; }
        public int CallsCount { get { return InCallsCount + OutCallsCount + OtherCallsCount; } }

        public int VkMessagesCount { get; set; }
        public int VkMessagesInCount { get; set; }
        public int VkMessagesOutCount { get; set; }
        public int CommentsCount { get; set; }
        public int HistoryCount { get; set; }

        public bool ShowComments { get; set; }
        public bool ShowCalls { get; set; }
        public bool ShowEmails { get; set; }
        public bool ShowSms { get; set; }
        public bool ShowVkMessages { get; set; }
        public bool ShowFacebookMessages { get; set; }
        public bool ShowInstagramMessages { get; set; }
        public bool ShowTelegramMessages { get; set; }
        public bool ShowHistory { get; set; }

        public string Elapsed { get; set; }
        public Dictionary<string, string> ElapsedTask { get; set; }
        public int TelegramReceivedMessagesCount { get; set; }
        public int TelegramSendedMessagesCount { get; set; }
        public int TelegramMessagesCount { get { return TelegramSendedMessagesCount + TelegramReceivedMessagesCount; } }

        public bool ShowOkMessages { get; set; }
        public int OkReceivedMessagesCount { get; set; }
        public int OkSendedMessagesCount { get; set; }
        public int OkMessagesCount { get { return OkReceivedMessagesCount + OkSendedMessagesCount; } }
    }

    public class LeadEventGroupModel
    {
        public LeadEventGroupModel()
        {
            Events = new List<LeadEventModel>();
        }

        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<LeadEventModel> Events { get; set; }
    }
}
