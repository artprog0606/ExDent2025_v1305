using System;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Orders;

namespace AdvantShop.CMS
{
    public class AdminNotification
    {
        public AdminNotification()
        {
            DateCreated = DateTime.Now;
            InNewTab = SettingsNotifications.WebNotificationInNewTab;
        }

        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        public string Tag { get; set; }
        public AdminNotificationType Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string IconPath { get; set; }
        public bool InNewTab { get; set; }
        public object Data { get; set; }
    }

    public class AdminNotificationData
    {
        public string Url { get; set; }
    }

    /// <summary>
    /// Notification with avatar of person who sent the notification
    /// </summary>
    public class PersonAdminNotification : AdminNotification
    {
        public PersonAdminNotification(Customer sender)
        {
            Type = AdminNotificationType.Notify;
            IconPath = sender != null && sender.Avatar.IsNotEmpty()
                ? FoldersHelper.GetPath(FolderType.Avatar, sender.Avatar, false)
                : UrlService.GetAdminStaticUrl() + "images/no-avatar.jpg";
        }
    }

    #region AdminCommentNotifications

    public class AdminCommentNotification : PersonAdminNotification
    {
        public AdminCommentNotification(AdminComment comment, Customer author) : base(author)
        {
            Title = LocalizationService.GetResource("Core.Services.CMS.AdminCommentNotification.Title");
            var commentText = comment.Text.Replace("\n", " ");
            Body = string.Format("{0}", commentText.Length > 250 ? commentText.Substring(0, 250) + " ..." : commentText);
            if (comment.ObjUrl.IsNotEmpty())
            {
                if (comment.ObjUrl.Contains("#") && comment.ObjUrl.Contains("modal") && !comment.ObjUrl.Contains("modalShow"))
                    comment.ObjUrl += "&modalShow=true";

                Data = new AdminNotificationData() {Url = comment.ObjUrl};
            }
        }
    }

    public class AdminCommentAnswerNotification : AdminCommentNotification
    {
        public AdminCommentAnswerNotification(AdminComment comment, Customer author) : base(comment, author)
        {
            Title = LocalizationService.GetResource("Core.Services.CMS.AdminCommentAnswerNotification.Title");
        }
    }

    #endregion

    #region OrderNotifications

    public class OrderNotification : AdminNotification
    {
        public OrderNotification(Order order)
        {
            Type = AdminNotificationType.Notify;
            Data = new
            {
                Url = UrlService.GetAdminUrl("orders/edit/" + order.OrderID, true)
            };
        }
    }

    public class OrderAddedNotification : OrderNotification
    {
        public OrderAddedNotification(Order order) : base(order)
        {
            Title = LocalizationService.GetResource("Core.Services.CMS.OrderAddedNotification.Title");
            Body = LocalizationService.GetResourceFormat("Core.Services.CMS.OrderAddedNotification.Body", order.Number);
        }
    }

    public class OrderManagerAssignedNotification : OrderNotification
    {
        public OrderManagerAssignedNotification(Order order) : base(order)
        {
            Title = LocalizationService.GetResource("Core.Services.CMS.OrderManagerAssignedNotification.Title");
            Body = LocalizationService.GetResourceFormat("Core.Services.CMS.OrderManagerAssignedNotification.Body", order.Number);
        }
    }

    #region OrderCommentNotifications

    public class OrderCommentNotification : AdminCommentNotification
    {
        public OrderCommentNotification(Order order, Customer author, AdminComment comment) : base(comment, author)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OrderCommentNotification.Title", order.Number);
            if (Data == null)
            {
                Data = new AdminNotificationData() {Url = UrlService.GetAdminUrl("orders/edit/" + order.OrderID, true) };
            }
        }
    }

    public class OrderCommentAnswerNotification : OrderCommentNotification
    {
        public OrderCommentAnswerNotification(Order order, Customer author, AdminComment comment) : base(order, author, comment)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OrderCommentAnswerNotification.Title", order.Number);
        }
    }

    #endregion
    #endregion

    #region CustomerNotifications

    public class CustomerNotification : AdminNotification
    {
        public CustomerNotification(Customer customer)
        {
            Type = AdminNotificationType.Notify;
            Data = new
            {
                Url = UrlService.GetAdminUrl("customers/view/" + customer.Id, true)
            };
        }
    }

    #region CustomerCommentNotifications

    public class CustomerCommentNotification : AdminCommentNotification
    {
        public CustomerCommentNotification(Customer customer, Customer author, AdminComment comment) : base(comment, author)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.CustomerCommentNotification.Title", customer.GetFullName());
            if (Data == null)
            {
                Data = new AdminNotificationData { Url = UrlService.GetAdminUrl("customers/view/" + customer.Id) };
            }
        }
    }

    public class CustomerCommentAnswerNotification : CustomerCommentNotification
    {
        public CustomerCommentAnswerNotification(Customer customer, Customer author, AdminComment comment) : base(customer, author, comment)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.CustomerCommentAnswerNotification.Title", customer.GetFullName());
        }
    }

    #endregion
    #endregion

    #region Task Notifications

    public class TaskNotification : PersonAdminNotification
    {
        public TaskNotification(Task task, Customer modifier) : base(modifier)
        {
            Data = new
            {
                Url = UrlService.GetAdminUrl("tasks/#?modalShow=true&modal=" + task.Id, true)
            };
        }
    }

    public class OnSetTaskNotification : TaskNotification
    {
        public OnSetTaskNotification(Task task, Customer modifier) : base(task, modifier)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnSetTaskNotification.Title", task.Id);
            Body = task.Name;
        }
    }

    public class OnTaskChangeNotification : TaskNotification
    {
        public OnTaskChangeNotification(Task task, Customer modifier, string field, string oldValue, string newValue) : base(task, modifier)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnTaskChangeNotification.Title", task.Id);
            var sb = new StringBuilder();
            sb.Append(task.Name);
            if (field.IsNotEmpty())
                sb.AppendFormat("\n{0}", field);
            if (oldValue.IsNotEmpty() || newValue.IsNotEmpty())
            {
                if (oldValue.IsNotEmpty() && newValue.IsNotEmpty())
                    sb.AppendFormat("\n{0} -> {1}", oldValue, newValue);
                else
                    sb.AppendFormat("\n{0}", oldValue.IsNullOrEmpty() ? newValue : oldValue);
            }
            Body = sb.ToString();
            Data = new
            {
                Url = UrlService.GetAdminUrl("tasks/#?modalShow=true&modal=" + task.Id, true)
            };
        }
    }

    public class OnTaskDeletedNotification : TaskNotification
    {
        public OnTaskDeletedNotification(Task task, Customer modifier) : base(task, modifier)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnTaskDeletedNotification.Title", task.Id);
            Body = task.Name;
            Data = null;
        }
    }

    public class OnTaskAcceptedNotification : TaskNotification
    {
        public OnTaskAcceptedNotification(Task task, Customer modifier) : base(task, modifier)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnTaskAcceptedNotification.Title", task.Id);
            Body = task.Name;
        }
    }

    #region TaskCommentNotifications

    public class OnTaskCommentNotification : AdminCommentNotification
    {
        public OnTaskCommentNotification(Task task, Customer author, AdminComment comment) : base(comment, author)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnTaskCommentNotification.Title", task.Id);
            if (Data == null)
            {
                Data = new AdminNotificationData { Url = UrlService.GetAdminUrl("tasks/#?modalShow=true&modal=" + task.Id, true) };
            }
        }
    }

    public class OnTaskCommentAnswerNotification : OnTaskCommentNotification
    {
        public OnTaskCommentAnswerNotification(Task task, Customer author, AdminComment comment) : base(task, author, comment)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnTaskCommentAnswerNotification.Title", task.Id);
        }
    }

    #endregion

    #endregion

    #region LeadNotifications

    public class LeadNotification : AdminNotification
    {
        public LeadNotification(Lead lead)
        {
            Type = AdminNotificationType.Notify;
            Data = new
            {
                Url = UrlService.GetAdminUrl("leads#?leadIdInfo=" + lead.Id, true)
            };
        }
    }

    public class LeadAddedNotification : LeadNotification
    {
        public LeadAddedNotification(Lead lead) : base(lead)
        {
            Title = LocalizationService.GetResource("Core.Services.CMS.LeadAddedNotification.Title");
            Body = LocalizationService.GetResourceFormat("Core.Services.CMS.LeadAddedNotification.Body", lead.Id);
        }
    }

    public class LeadChangedNotification : LeadNotification
    {
        public LeadChangedNotification(Lead lead) : base(lead)
        {
            Title = LocalizationService.GetResource("Core.Services.CMS.LeadChangedNotification.Title");
            Body = LocalizationService.GetResourceFormat("Core.Services.CMS.LeadChangedNotification.Body", lead.Id);
        }
    }


    #region LeadCommentNotifications

    public class OnLeadCommentNotification : AdminCommentNotification
    {
        public OnLeadCommentNotification(Lead lead, Customer author, AdminComment comment) : base(comment, author)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnLeadCommentNotification.Title", lead.Id);
            if (Data == null)
            {
                Data = new AdminNotificationData { Url = UrlService.GetAdminUrl("leads#?leadIdInfo=" + lead.Id, true) };
            }
        }
    }

    public class OnLeadCommentAnswerNotification : OnLeadCommentNotification
    {
        public OnLeadCommentAnswerNotification(Lead lead, Customer author, AdminComment comment) : base(lead, author, comment)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnLeadCommentAnswerNotification.Title", lead.Id);
        }
    }

    #endregion

    #endregion

    #region LeadEvent Notifications

    public class LeadEventNotification : AdminNotification
    {
        public LeadEventNotification(Guid customerId, LeadEventType type)
        {
            Type = AdminNotificationType.Notify;
            Data = new
            {
                Url = UrlService.GetAdminUrl("customers/view/" + customerId + "?shown=true#?leadEventsFilterType=" + type, true)
            };
        }
    }

    public class VkMessageNotification : LeadEventNotification
    {
        public VkMessageNotification(VkUserMessage message) : base(message.CustomerId, LeadEventType.Vk)
        {
            Title = string.Join(" ", message.LastName, message.FirstName);
            Body = "ВКонтакте: входящее сообщение";
            IconPath = message.Photo100;
        }
    }

    public class FacebookMessageNotification : LeadEventNotification
    {
        public FacebookMessageNotification(FacebookUserMessage message) : base(message.CustomerId, LeadEventType.Facebook)
        {
            Title = string.Join(" ", message.LastName, message.FirstName);
            Body = string.Format("Facebook: {0}", message.Title);
            IconPath = message.Picture;
        }
    }

    public class InstagramMessageNotification : LeadEventNotification
    {
        public InstagramMessageNotification(InstagramUserMessage message) : base(message.CustomerId, LeadEventType.Instagram)
        {
            Title = message.FullName;
            Body = message.Title;
            IconPath = message.ProfilePicture;
        }
    }

    public class OkMessageNotification : LeadEventNotification
    {
        public OkMessageNotification(OkUserMessage message) : base(message.CustomerId, LeadEventType.Ok)
        {
            Title = string.Join(" ", message.LastName, message.FirstName);
            Body = "Одноклассники: входящее сообщение";
            IconPath = message.Photo;
        }
    }
    
    public class TelegramMessageNotification : LeadEventNotification
    {
        public TelegramMessageNotification(TelegramUserMessage message) : base(message.CustomerId, LeadEventType.Telegram)
        {
            Title = string.Join(" ", message.LastName, message.FirstName);
            Body = "Telegram: входящее сообщение";
            IconPath = message.PhotoUrl;
        }
    }
    #endregion

    #region Booking Notifications

    public class BookingNotification : AdminNotification
    {
        public BookingNotification(Booking booking)
        {
            Type = AdminNotificationType.Notify;
            Data = new
            {
                Url = UrlService.GetAdminUrl("booking#?modal=" + booking.Id, true)
            };
        }
    }

    public class BookingAddedNotification : BookingNotification
    {
        public BookingAddedNotification(Booking booking) : base(booking)
        {
            Title = LocalizationService.GetResource("Core.Services.CMS.BookingAddedNotification.Title");
            Body = LocalizationService.GetResourceFormat("Core.Services.CMS.BookingAddedNotification.Body", booking.Id);
        }
    }

    #region BookingCommentNotifications

    public class OnBookingCommentNotification : AdminCommentNotification
    {
        public OnBookingCommentNotification(Booking booking, Customer author, AdminComment comment) : base(comment, author)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnBookingCommentNotification.Title", booking.Id);
            if (Data == null)
            {
                Data = new AdminNotificationData { Url = UrlService.GetAdminUrl("booking#?modal=" + booking.Id, true) };
            }
        }
    }

    public class OnBookingCommentAnswerNotification : OnBookingCommentNotification
    {
        public OnBookingCommentAnswerNotification(Booking booking, Customer author, AdminComment comment) : base(booking, author, comment)
        {
            Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnBookingCommentAnswerNotification.Title", booking.Id);
        }
    }

    #endregion

    #endregion
}