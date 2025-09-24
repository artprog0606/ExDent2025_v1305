using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm.Facebook.Models;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Core.Services.Crm.Facebook
{
    public enum FacebookMessageType
    {
        PostComment = 0,
        MessangerComment = 1,
        MessangerWebhook = 2
    }

    public class FacebookMessage
    {
        /// <summary>
        /// Id in db
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id from facebook
        /// </summary>
        public string MessageId { get; set; }

        public string Message { get; set; }

        public string FromId { get; set; }

        public string ToId { get; set; }

        public DateTime CreatedTime { get; set; }

        public string PostId { get; set; }

        public string ConversationId { get; set; }

        public FacebookMessageType Type { get; set; }

        public string Title
        {
            get
            {
                var title = "";

                if (FromId == SettingsFacebook.GroupId)
                    title += "Исходящее сообщение";
                else if (ToId == SettingsFacebook.GroupId)
                    title += "Входящее сообщение";

                if (PostId != null)
                    title += string.Format(" к <a href=\"https://www.facebook.com/{0}\" target=\"_blank\">посту</a>", MessageId);

                if (ConversationId != null)
                    title+= " inbox";

                return title;
            }
        }


        public FacebookMessage() { }

        public FacebookMessage(FbPostComment comment, FbPost post, List<FbPostComment> comments)
        {
            MessageId = comment.Id;
            FromId = comment.From != null ? comment.From.Id : "";
            Message = comment.Message;
            CreatedTime = comment.CreatedTime;

            PostId = post.Id;

            if (comment.Parent != null)
            {
                var parentComment = comments.Find(x => x.Id == comment.Parent.Id);
                if (parentComment != null)
                    ToId = parentComment.From.Id;
            }
            Type = FacebookMessageType.PostComment;
        }

        public FacebookMessage(FbConversationMessage message, FbConversation conversation)
        {
            MessageId = message.Id;
            FromId = message.From != null ? message.From.Id : "";
            Message = message.Message;
            ToId = message.To != null && message.To.Data != null && message.To.Data.Count > 0
                ? message.To.Data[0].Id
                : "";
            CreatedTime = message.CreatedTime;
            ConversationId = conversation.Id;
            Type = FacebookMessageType.MessangerComment;
        }

        public FacebookMessage(FbWebhookMessaging messaging)
        {
            MessageId = messaging.Message.Mid;
            FromId = messaging.Sender.Id;
            ToId = messaging.Recipient.Id;
            Message = messaging.Message.Text;
            Type = FacebookMessageType.MessangerWebhook;
            CreatedTime =
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    .AddMilliseconds(messaging.Timestamp)
                    .ToLocalTime();
        }
    }


    public class FacebookUserMessage : FacebookMessage
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserId { get; set; }

        public string PhotoPicByPsyId { get; set; }

        // https://developers.facebook.com/docs/graph-api/reference/user/picture/
        public string Picture
        {
            get
            {
                return
                    !string.IsNullOrEmpty(PhotoPicByPsyId)
                        ? PhotoPicByPsyId
                        : (!string.IsNullOrEmpty(UserId)
                            ? "https://graph.facebook.com/" + UserId + "/picture?type=large"
                            : UrlService.GetUrl("areas/admin/content/images/no-avatar.jpg"));
            }
        }

        public Guid CustomerId { get; set; }
    }

    public class FacebookSendedMessage
    {
        public string Id { get; set; }
    }
}
