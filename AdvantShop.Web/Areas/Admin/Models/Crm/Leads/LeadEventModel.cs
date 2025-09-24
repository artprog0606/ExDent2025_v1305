using System;
using System.Linq;
using System.Web;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Loging.Smses;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadEventModel : LeadEvent
    {
        public Guid CustomerId { get; set; }
        public new string Title { get; set; }
        public string SubMessage { get; set; }
        public string EventType { get { return Type.ToString().ToLower(); } }

        public string EmailId { get; set; }
        public string EmailFolder { get; set; }

        public string UserPhoto { get; set; }
        public object Data { get; set; }

        public string CreateDateFormat { get; set; }

        public bool Seen { get; set; }

        public int InformerId { get; set; }
        public string InformerLink { get; set; }

        public LeadEventModel() { }

        public LeadEventModel(LeadEvent leadEvent)
        {
            Title = leadEvent.Title;
            Message = Encode(leadEvent.Message);
            CreatedDate = leadEvent.CreatedDate;
            CreateDateFormat = leadEvent.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss zzz");
            Type = LeadEventType.Comment;
            TaskId = leadEvent.TaskId;
            CreatedBy = leadEvent.CreatedBy;
            CreatedById = leadEvent.CreatedById;

            if (CreatedById != null)
            {
                var c = CustomerService.GetCustomer(CreatedById.Value);
                if (c != null && (c.IsAdmin || c.IsModerator || c.IsManager))
                    CreatedByIsModerator = true;
            }
        }

        public LeadEventModel(AdminCommentModel comment)
        {
            Id = comment.Id;
            Message = Encode(comment.Text).Replace("\n", "<br/>");
            CreatedDate = comment.DateCreated;
            CreateDateFormat = comment.DateCreated.ToString("yyyy-MM-dd HH:mm:ss zzz");
            Type = LeadEventType.Comment;
            CreatedBy = comment.Name;
            CreatedById = comment.CustomerId;

            if (CreatedById != null)
            {
                var c = CustomerService.GetCustomer(CreatedById.Value);
                if (c != null && (c.IsAdmin || c.IsModerator || c.IsManager))
                    CreatedByIsModerator = true;
            }

            var customer = CustomerContext.CurrentCustomer;

            Data = new
            {
                Type = comment.Type,
                CanDelete = (comment.CustomerId != null && customer.Id == comment.CustomerId) || customer.IsAdmin,
                CanEdit = (comment.CustomerId != null && customer.Id == comment.CustomerId) || customer.IsAdmin,
            };
        }

        public LeadEventModel(EmailImap email)
        {
            EmailId = email.Id;
            EmailFolder = email.Folder;

            Title = email.From.ToLower().Contains(AdvantShop.Configuration.SettingsMail.Login.ToLower())
                ? "<span class=\"bold\">" + LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.OutcomingLetter") + "</span>"
                : "<span class=\"bold\">" + LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.IncomingLetter") + "</span>";
            Message = email.Subject;
            CreatedDate = email.Date;
            Type = LeadEventType.Email;
        }


        public LeadEventModel(EmailLogItem email, Guid customerId, string customerEmail)
        {
            Data = new LeadEventEmailDataModel()
            {
                Id = email.Id,
                CustomerId = customerId,
                CustomerEmail = customerEmail,
                CreateOn = email.CreateOn
            };
            Title = "<span class=\"bold\">" + LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.OutcomingLetter") + "</span>" +
                    (email.Status == EmailStatus.Error
                        ? " [Не отправлено из-за ошибки]"
                        : " " + "<span class=\"crm-new-status\">" + email.Status.Localize() + "</span>"
                          + (email.Updated != email.CreateOn
                              ? " " + Localization.Culture.ConvertDate(email.Updated.HasValue
                                  ? email.Updated.Value
                                  : email.CreateOn)
                              : "")
                    );
            if (SettingsMail.UseAdvantshopMail)
                Title += "<help-trigger class=\"ng-cloak m-l-xs\" use-template=\"true\"><div class=\"help-content\">"+ email.Status.DescriptionKey() + "</div></help-trigger>";

            Message = email.Subject;
            CreatedDate = email.CreateOn;
            Type = LeadEventType.Email;
        }

        public LeadEventModel(Call call)
        {
            Id = call.Id;
            Title = call.Type.Localize() +
                    LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.CallTitle");
            CreatedDate = call.CallDate;
            Type = LeadEventType.Call;
            var callComment = AdminCommentService.GetAdminComments(call.Id, AdminCommentType.Call).FirstOrDefault();
            if (callComment != null)
            {
                var customer = CustomerContext.CurrentCustomer;

                Data = new
                {
                    callComment.Id,
                    callComment.ParentId,
                    callComment.ObjId,
                    callComment.Type,
                    callComment.CustomerId,
                    callComment.Name,
                    callComment.Email,
                    callComment.Text,
                    CanEdit = (callComment.CustomerId != null && callComment.CustomerId == customer.Id) || customer.IsAdmin
                };
            }

            SubMessage =
                call.CallAnswerDate != null
                    ? string.Format("<call-record call-id=\"{0}\" operator-type=\"{1}\"></call-record>", call.Id,
                        (int)call.OperatorType)
                    : null;
        }

        public LeadEventModel(SmsLogDto smsLog)
        {
            Title = LeadEventType.Sms.Localize();
            Message = Encode(smsLog.Message);
            CreatedDate = smsLog.CreatedOnUtc.ToLocalTime();
            Type = LeadEventType.Sms;
        }

        public LeadEventModel(VkUserMessage message)
        {
            CustomerId = message.CustomerId;
            Id = message.Id;
            Title =
                string.Format(
                    "<a href=\"https://vk.com/{0}\" target=\"_blank\" class=\"lead-vk-title\">{1} {2}</a> [{3}]",
                    !string.IsNullOrEmpty(message.ScreenName) ? message.ScreenName : "id" + message.UserId,
                    message.LastName, message.FirstName,
                    message.Type == VkMessageType.Received
                        ? LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.VkReceivedTitle")
                        : LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.VkSendedTitle"));
            Message = message.Body;
            UserPhoto = message.Photo100;

            Data = new LeadEventVkDataModel()
            {
                PostId = message.PostId,
                UserId = message.UserId,
            };
            CreatedDate = message.Date;
            Type = LeadEventType.Vk;

            InformerLink = UrlService.GetAdminUrl("customers/view/" + CustomerId + "?leadEventsFilterType=" + Type);
        }

        public LeadEventModel(InstagramUserMessage message)
        {
            CustomerId = message.CustomerId;
            Id = message.Id;
            Title =
                string.Format(
                    "<a href=\"https://instagram.com/{0}/\" target=\"_blank\" class=\"lead-vk-title\">{1}</a> {2}",
                    message.UserName, message.FullName,
                    !string.IsNullOrEmpty(message.Title)
                        ? "[" + message.Title + "]"
                        : "");
            Message = message.ItemType == InstagramMessageType.DirectMedia ? message.Text : Encode(message.Text);
            UserPhoto = message.ProfilePicture;

            CreatedDate = message.CreatedDate;
            Type = LeadEventType.Instagram;

            InformerLink = UrlService.GetAdminUrl("customers/view/" + CustomerId + "?leadEventsFilterType=" + Type);
        }

        public LeadEventModel(FacebookUserMessage message)
        {
            CustomerId = message.CustomerId;
            Id = message.Id;
            Title =
                string.Format(
                    "<a href=\"https://facebook.com/{0}/\" target=\"_blank\" class=\"lead-vk-title\">{1} {2}</a> {3}",
                    message.FromId, message.FirstName, message.LastName, message.Title);
            Message = Encode(message.Message);
            UserPhoto = message.Picture;
            CreatedDate = message.CreatedTime;
            Type = LeadEventType.Facebook;

            InformerLink = UrlService.GetAdminUrl("customers/view/" + CustomerId + "?leadEventsFilterType=" + Type);
        }

        public LeadEventModel(TelegramUserMessage message)
        {
            CustomerId = message.CustomerId;
            Id = message.Id;
            Title =
                !string.IsNullOrEmpty(message.FirstName + message.LastName)
                    ? message.FirstName + " " + message.LastName
                    : message.UserName;
            Message = message.Text;
            UserPhoto = message.PhotoUrl;
            if (!string.IsNullOrEmpty(UserPhoto))
                UserPhoto = UrlService.GetUrl("pictures/avatar/" + UserPhoto);

            CreatedDate = message.Date;
            Type = LeadEventType.Telegram;

            InformerLink = UrlService.GetAdminUrl("customers/view/" + CustomerId + "?leadEventsFilterType=" + Type);
        }

        public LeadEventModel(OkUserMessage message)
        {
            CustomerId = message.CustomerId;
            Id = message.Id;

            if (message.FromUser == Configuration.SettingsOk.GroupId)
            {
                Title =
                string.Format(
                    "<a href=\"https://ok.ru/group/{0}\" target =\"_blank\" class=\"lead-vk-title\">{1} {2}</a> [{3}]",
                    message.FromUser,
                    message.LastName, message.FirstName,
                    LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.OkSendedMsg"));
            }
            else
            {
                Title =
                string.Format(
                    "<a href=\"https://ok.ru/profile/{0}\" target =\"_blank\" class=\"lead-vk-title\">{1} {2}</a> [{3}]",
                    message.FromUser,
                    message.LastName, message.FirstName,
                    LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.OkReceivedMsg"));
            }

            Message = message.Text;
            CreatedDate = message.CreatedDate;
            Type = LeadEventType.Ok;
            UserPhoto = message.Photo;
            InformerLink = UrlService.GetAdminUrl("customers/view/" + CustomerId + "?leadEventsFilterType=" + Type);
        }

        public LeadEventModel(ChangeHistory history)
        {
            Message =
                !string.IsNullOrEmpty(history.OldValue)
                    ? String.Format("{0}: <div class=\"crm-old-status\">{1}</div> → <div class=\"crm-new-status\">{2}</div>",
                                    history.ParameterName, Encode(history.OldValue), Encode(history.NewValue))
                    : !string.IsNullOrEmpty(history.NewValue)
                        ? String.Format("{0}: {1}", history.ParameterName, Encode(history.NewValue))
                        : history.ParameterName;

            CreatedDate = history.ModificationTime;
            CreateDateFormat = history.ModificationTime.ToString("yyyy-MM-dd HH:mm:ss zzz");
            Type = LeadEventType.History;
            CreatedBy = history.ChangedByName;
            CreatedById = history.ChangedById;

            if (CreatedById != null)
            {
                var c = CustomerService.GetCustomer(CreatedById.Value);
                if (c != null && (c.IsAdmin || c.IsModerator || c.IsManager))
                    CreatedByIsModerator = true;
            }
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value);
        }


        public static LeadEventModel GetLeadEventModel(AdminInformer informer)
        {
            switch (informer.Type)
            {
                case AdminInformerType.Vk:
                    {
                        var msg = VkService.GetCustomerMessage(informer.ObjId);
                        if (msg != null)
                            return new LeadEventModel(msg) {Seen = informer.Seen, InformerId = informer.Id};
                        break;
                    }

                case AdminInformerType.Facebook:
                    {
                        var msg = FacebookService.GetCustomerMessage(informer.ObjId);
                        if (msg != null)
                            return new LeadEventModel(msg) {Seen = informer.Seen, InformerId = informer.Id};
                        break;
                    }

                case AdminInformerType.Instagram:
                    {
                        var msg = InstagramService.GetCustomerMessage(informer.ObjId);
                        if (msg != null)
                            return new LeadEventModel(msg) {Seen = informer.Seen, InformerId = informer.Id};
                        break;
                    }

                case AdminInformerType.Telegram:
                {
                    var msg = new TelegramService().GetCustomerMessage(informer.ObjId);
                    if (msg != null)
                        return new LeadEventModel(msg) { Seen = informer.Seen, InformerId = informer.Id };
                    break;
                }

                case AdminInformerType.Comment:
                    {
                        var comment = AdminCommentService.GetAdminComment(informer.ObjId);
                        if (comment != null)
                            return new LeadEventModel()
                            {
                                Id = comment.Id,
                                Title = informer.Title,
                                Message = Encode(informer.Body),
                                CreatedDate = informer.CreatedDate,
                                Type = LeadEventType.Comment,
                                Seen = informer.Seen,
                                Data = informer.Link,
                                InformerId = informer.Id,
                                InformerLink = informer.Link,
                            };
                        break;
                    }

                case AdminInformerType.Email:
                    {
                        var model = new LeadEventModel()
                        {
                            Id = informer.Id,
                            Title = informer.Title,
                            Message = informer.Body,
                            CreatedDate = informer.CreatedDate,
                            Type = LeadEventType.Email,
                            Seen = informer.Seen,
                            InformerId = informer.Id,
                            InformerLink = informer.Link,
                        };
                        return model;
                    }

                case AdminInformerType.Review:
                    {
                        var model = new LeadEventModel()
                        {
                            Id = informer.Id,
                            Title = informer.Title,
                            Message = informer.Body,
                            CreatedDate = informer.CreatedDate,
                            Type = LeadEventType.Review,
                            Seen = informer.Seen,
                            InformerId = informer.Id,
                            InformerLink = UrlService.GetAdminUrl("reviews?modal=" + informer.ObjId),
                        };
                        return model;
                    }

                case AdminInformerType.Ok:
                    {
                        var msg = OkService.GetUserMessage(informer.ObjId);
                        if (msg != null)
                            return new LeadEventModel(msg) { Seen = informer.Seen, InformerId = informer.Id };
                        break;
                    }


                default:
                    {
                        var model = new LeadEventModel()
                        {
                            Id = informer.Id,
                            Title = informer.Title,
                            Message = Encode(informer.Body),
                            CreatedDate = informer.CreatedDate,
                            Type = LeadEventType.Other,
                            Seen = informer.Seen,
                            InformerId = informer.Id,
                            InformerLink = informer.Link,
                        };
                        return model;
                    }
            }

            AdminInformerService.Delete(informer.Id);

            return null;
        }
    }

    public class LeadEventEmailDataModel
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreateOn { get; set; }
        public bool IsError { get; set; }
    }

    public class LeadEventVkDataModel
    {
        public long UserId { get; set; }
        public long? PostId { get; set; }
    }
}
