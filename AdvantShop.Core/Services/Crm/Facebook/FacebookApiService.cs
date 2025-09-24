using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.Crm.Facebook.Models;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook
{
    public class FacebookApiService
    {
        private const string FbGraphUrl = "https://graph.facebook.com/v2.11/";

        #region Auth

        public bool GetUserAuthTokenByCode(string clientId, string clientSecret, string code, string redirectUrl)
        {
            var url =
                string.Format(
                    "oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                    clientId, redirectUrl, clientSecret, code);

            var result = GetData<FacebookAccessToken>(url);
            if (result == null)
                return false;

            SettingsFacebook.ClientId = clientId;
            SettingsFacebook.ClientSecret = clientSecret;
            SettingsFacebook.UserToken = result.AccessToken;

            return true;
        }

        public List<FacebookGroupItemAccessToken> GetUserGroupTokens()
        {
            // https://developers.facebook.com/docs/facebook-login/access-tokens
            //
            // Page access tokens используются для управлениями страницами. Необходимы права manage_pages. 
            // Возвращает список групп (у каждой группы свой access_token, который нам и нужен для дальнейшей работы с группой)
            
            var result = GetData<FacebookGroupAccessToken>("me/accounts?access_token=" + SettingsFacebook.UserToken);
            return result == null ? null : result.Data;
        }

        #endregion

        #region Active

        public void DeActivate()
        {
            SettingsFacebook.ClientId = null;
            SettingsFacebook.ClientSecret = null;
            SettingsFacebook.UserToken = null;

            SettingsFacebook.GroupId = null;
            SettingsFacebook.GroupName = null;
            SettingsFacebook.GroupToken = null;
            SettingsFacebook.GroupPerms = null;
        }

        public bool IsActive()
        {
            return SettingsFacebook.GroupToken != null;
        }

        #endregion

        public void GetLastMessages()
        {
            var messages = GetLastPostComments();
            //var messangerComments = GetMessangerComments();
            //messages.AddRange(messangerComments);

            if (messages.Count > 0)
            {
                var userIds = messages.Select(x => x.FromId).Where(x => !string.IsNullOrEmpty(x)).Distinct();
                var users = GetUsersInfo(userIds);

                FacebookService.AddMessages(messages);

                SaveUsers(users, messages, !SettingsFacebook.IsDataLoaded);
            }

            SettingsFacebook.IsDataLoaded = true;
        }

        #region Get post comments 

        /// <summary>
        /// Получаем последние посты и комментарии к ним
        /// </summary>
        private List<FacebookMessage> GetLastPostComments()
        {
            var messages = new List<FacebookMessage>();

            if (!SettingsFacebook.CreateLeadFromComments)
                return messages;

            var url = GetUrl(SettingsFacebook.GroupId + "/feed",
                new FbParam()
                {
                    {"limit", "10"},
                    { "fields", "id,message,updated_time,link,comments.fields(id,message,created_time,from,parent.fields(id)).summary(1).filter(stream)"}
                });

            var posts = GetData<FbPostCollection>(url);
            if (posts == null || posts.Data == null)
                return messages;
            
            foreach (var post in posts.Data)
            {
                if (post.Comments == null || post.Comments.Data == null || post.Comments.Data.Count == 0)
                    continue;

                var dbComments = FacebookService.GetPostMessages(post.Id);

                foreach (var comment in post.Comments.Data)
                {
                    if (dbComments.Find(x => x.MessageId == comment.Id) != null)
                        continue;

                    messages.Add(new FacebookMessage(comment, post, post.Comments.Data));
                }
            }

            return messages;
        }
        
        /*
        private List<FacebookMessage> GetPostComments(FbPost post)
        {
            var messages = new List<FacebookMessage>();

            // https://developers.facebook.com/docs/graph-api/reference/v2.11/object/comments

            var url = GetUrl(post.Id + "/comments",
                new FbParam()
                {
                    {"summary", "1"},
                    {"filter", "stream"},
                    {"fields", "id,message,created_time,from,parent.fields(id)"}
                });

            var comments = GetData<FbPostCommentCollection>(url);
            if (comments == null || comments.Data == null || comments.Summary == null || comments.Summary.TotalCount == 0)
                return messages;
            
            var dbComments = FacebookService.GetPostMessages(post.Id);
            foreach (var comment in comments.Data)
            {
                if (dbComments.Find(x => x.MessageId == comment.Id) != null)
                    continue;

                messages.Add(new FacebookMessage(comment, post, comments.Data));
            }

            return messages;
        }
        */

        #endregion

        #region Get messanger comments

        private List<FacebookMessage> GetMessangerComments()
        {
            var messages = new List<FacebookMessage>();

            // https://developers.facebook.com/docs/graph-api/reference/page/conversations/

            var url = GetUrl(SettingsFacebook.GroupId + "/conversations", new FbParam()
            {
                {"limit", "15" },
                {"fields", "id,link,updated_time,message_count,unread_count,can_reply,messages.fields(id,message,from,to,created_time).limit(1000)"} // ,shares{link}
            });

            var conversations = GetData<FbConversationCollection>(url);

            if (conversations == null || conversations.Data == null || conversations.Data.Count == 0)
                return messages;

            foreach (var conversation in conversations.Data)
            {
                if (conversation.Messages == null || conversation.Messages.Data == null || conversation.Messages.Data.Count == 0)
                    continue;

                var dbMessages = FacebookService.GetConversationMessages(conversation.Id);

                if (dbMessages.Count == conversation.Messages.Data.Count)
                    continue;

                foreach (var message in conversation.Messages.Data)
                {
                    if (dbMessages.Find(x => x.MessageId == message.Id) != null)
                        continue;

                    messages.Add(new FacebookMessage(message, conversation));
                }
            }

            return messages;
        }

        /*
        private List<FbConversationMessage> GetConversationComments(string conversationId)
        {
            // https://developers.facebook.com/docs/graph-api/reference/v2.11/conversation/messages

            var url = GetUrl(conversationId + "/messages", new FbParam() {{"fields", "id,message,from,to,created_time,shares{link}"}});

            var msgs = GetData<FbConversationMessageCollection>(url);

            return msgs != null && msgs.Data != null && msgs.Data.Count > 0 
                ? msgs.Data 
                : new List<FbConversationMessage>();
        }
        */

        #endregion

        #region Save web hook message

        public void SaveWebHookMessage(FbWebhookModel model)
        {
            if (model == null || model.Entry == null || model.Entry.Count == 0)
                return;

            if (!SettingsFacebook.CreateLeadFromMessages)
                return;

            var messages = new List<FacebookMessage>();
            var users = new List<FacebookUser>();

            foreach (var entry in model.Entry)
            {
                foreach (var messaging in entry.Messaging)
                {
                    if (messaging.Sender == null || messaging.Recipient == null || messaging.Message == null)
                        continue;

                    var msg = new FacebookMessage(messaging);


                    // Так как в месенджере userId отличается от id реального пользователя, то ищем сообщение в диалогах (conversations)
                    // И подменяем id отправителя и получателя

                    var fromUser = FacebookService.GetUserByPsyId(msg.FromId);
                    if (fromUser != null)
                    {
                        msg.FromId = fromUser.Id;
                        users.Add(fromUser);
                    }
                    else
                    {
                        FacebookUser fbUser = null;
                        var psyId = msg.FromId;

                        if (IsActive())
                        {
                            var convMessage = GetMessageByConverstation(msg.MessageId);
                            if (convMessage != null)
                            {
                                msg.FromId = convMessage.From.Id;

                                var url = GetUrl(msg.FromId, new FbParam() {{"fields", "id,first_name,last_name,email,gender"}});
                                var user = GetData<FbUser>(url);
                                if (user != null)
                                    fbUser = new FacebookUser(user) {PsyId = psyId};
                            }
                        }
                        
                        if (fbUser == null)
                        {
                            var user = GetMessengerUserByPsyId(psyId);
                            if (user != null)
                                fbUser = new FacebookUser(psyId, user);
                        }

                        if (fbUser != null)
                            users.Add(fbUser);
                    }

                    messages.Add(msg);
                }
            }

            if (messages.Count > 0)
            {
                FacebookService.AddMessages(messages);

                SaveUsers(users, messages, false);
            }
        }

        public FbMessengerUser GetMessengerUserByPsyId(string psyId)
        {
            // https://developers.facebook.com/docs/messenger-platform/identity/user-profile

            var url = GetUrl(psyId, new FbParam() {{"fields", "first_name,last_name,profile_pic,gender" } });
            return GetData<FbMessengerUser>(url);
        }

        public FbConversationMessage GetMessageByConverstation(string messageId)
        {
            var url = GetUrl(SettingsFacebook.GroupId + "/conversations", new FbParam() {{"limit", "5"}, {"fields", "id"}});
            var conversations = GetData<FbConversationCollection>(url);

            if (conversations == null || conversations.Data == null || conversations.Data.Count == 0)
                return null;
            
            foreach (var conversation in conversations.Data)
            {
                var convUrl = GetUrl(conversation.Id + "/messages", new FbParam() {{"limit", "10"}, {"fields", "id,message,from,to,created_time"}});
                var convMessages = GetData<FbConversationMessageCollection>(convUrl);

                if (convMessages == null || convMessages.Data == null || convMessages.Data.Count == 0)
                    continue;

                var msg = convMessages.Data.FirstOrDefault(x => x.Id == messageId || x.Id.Contains(messageId.Replace("mid.", "")));
                if (msg != null && msg.From != null)
                    return msg;
            }

            Debug.Log.Info("GetUserIdByConverstation can't find " + messageId);

            return null;
        }
        
        #endregion

        #region GetUsersInfo

        private List<FacebookUser> GetUsersInfo(IEnumerable<string> userIds)
        {
            var users = new List<FacebookUser>();
            var groupId = SettingsFacebook.GroupId;

            foreach (var id in userIds)
            {
                if (id == groupId)
                {
                    users.Add(new FacebookUser() { Id = id, FirstName = SettingsFacebook.GroupName });
                    continue;
                }
                
                var dbUser = FacebookService.GetUser(id);
                if (dbUser != null)
                {
                    users.Add(dbUser);
                    continue;
                }

                // https://developers.facebook.com/docs/graph-api/reference/user/

                var url = GetUrl(id, new FbParam() {{"fields", "id,first_name,last_name,email,gender"}});
                var user = GetData<FbUser>(url);
                if (user == null)
                {
                    var u = GetData<FbUserShort>(GetUrl(id));
                    if (u != null)
                        user = new FbUser() {Id = u.Id, FirstName = u.Name};
                }

                if (user != null)
                    users.Add(new FacebookUser(user));
            }

            return users;
        }

        #endregion

        #region Save users 

        private void SaveUsers(List<FacebookUser> users, List<FacebookMessage> messages, bool isFirstTime)
        {
            if (users == null || users.Count == 0)
                return;
            
            var finalDealStatusId = SettingsCrm.FacebookFinalDealStatusId;
            
            foreach (var user in users)
            {
                var u = FacebookService.GetUser(user.Id);
                if (u != null)
                {
                    if (isFirstTime)
                        continue;

                    // Если все лиды закрыты, то создаем новый
                    var customerLeads = LeadService.GetLeadsByCustomer(u.CustomerId);
                    var hasNoClosedLeads = customerLeads.Any(x => x.DealStatusId != finalDealStatusId);
                    if (!hasNoClosedLeads)
                    {
                        // Если все заказы закрыты, то создаем лид
                        //var hasNoClosedOrders = OrderService.GetCustomerOrderHistory(u.CustomerId).Any(x => !OrderStatusService.GetOrderStatus(x.StatusID).IsCompleted);

                        //if (!hasNoClosedOrders || customerLeads.Count == 0)
                        //{
                            CreateLead(CustomerService.GetCustomer(u.CustomerId), u, messages);
                            continue;
                        //}
                    }
                    
                    BizProcessExecuter.MessageReply(CustomerService.GetCustomer(u.CustomerId), EMessageReplyFieldType.Facebook);

                    var userMsg = messages.Where(x => x.FromId == u.Id).OrderByDescending(x => x.CreatedTime).FirstOrDefault();
                    if (userMsg != null)
                    {
                        AdminInformerService.Add(new AdminInformer(AdminInformerType.Facebook, userMsg.Id, u.CustomerId));
                        BizProcessExecuter.LeadEvent(userMsg.Id, LeadEventType.Facebook);
                    }

                    continue;
                }

                try
                {
                    // add customer
                    var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EMail = user.Email,
                        IsAgreeForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter 
                                                          && SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
                    };
                    CustomerService.InsertNewCustomer(customer);

                    SocialNetworkService.SaveAvatar(customer, 
                        !string.IsNullOrEmpty(user.PhotoPicByPsyId) 
                            ? user.PhotoPicByPsyId
                            : "https://graph.facebook.com/" + user.Id + "/picture?type=large");

                    // add vk user
                    user.CustomerId = customer.Id;
                    FacebookService.AddUser(user);

                    // add lead
                    if (!isFirstTime)
                        CreateLead(customer, user, messages);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        private void CreateLead(Customer customer, FacebookUser user, List<FacebookMessage> messages)
        {
            if (user.Id == SettingsFacebook.GroupId)
                return;

            var source = OrderSourceService.GetOrderSource(OrderType.Facebook);
            var customerMessages = messages.Where(x => x.FromId == user.Id || x.ToId == user.Id).ToList();
            var lastMessage = customerMessages.Count > 0 ? customerMessages.OrderByDescending(x => x.CreatedTime).FirstOrDefault() : null;

            var lead = new Lead()
            {
                CustomerId = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                //Comment = String.Join("<br>\r\n ", customerMessages.OrderBy(x => x.CreatedTime).Select(x => x.Message)),
                OrderSourceId = source.Id,
                CreatedDate = lastMessage != null ? lastMessage.CreatedTime.AddSeconds(-1) : DateTime.Now,
                LeadItems = new List<LeadItem>(),
                SalesFunnelId = SettingsCrm.DefaultFacebookSalesFunnelId
            };

            LeadService.AddLead(lead, true);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_SocialNetwork, "Facebook");
        }

        #endregion

        #region Send message

        public bool SendMessage(int id, string text)
        {
            var message = FacebookService.Get(id);
            if (message == null)
                return false;
            
            var messageId = "";

            switch (message.Type)
            {
                case FacebookMessageType.PostComment:
                    messageId = SendToWall(message, text);
                    break;
                    
                case FacebookMessageType.MessangerComment:
                    messageId = SendToMessanger(message, text);
                    break;
                case FacebookMessageType.MessangerWebhook:
                    var user = FacebookService.GetUser(message.FromId);
                    if (user != null)
                        messageId = SendToMessangerByWebHook(user.PsyId, text);
                    break;
            }

            if (string.IsNullOrEmpty(messageId))
                return false;

            FacebookService.AddMessage(new FacebookMessage()
            {
                MessageId = messageId,
                ConversationId = message.ConversationId,
                PostId = message.PostId,
                CreatedTime = DateTime.Now,
                Message = text,
                FromId = SettingsFacebook.GroupId,
                ToId = message.FromId,
                Type = message.Type,
            });

            return true;
        }

        private string SendToWall(FacebookMessage message, string text)
        {
            if (string.IsNullOrEmpty(message.PostId))
                return null;

            var url = GetUrl(message.MessageId + "/comments");
            var result = GetData<FacebookSendedMessage>(url, "POST", "message=" + text);
            return result != null ? result.Id : null;
        }

        private string SendToMessanger(FacebookMessage message, string text)
        {
            if (string.IsNullOrEmpty(message.ConversationId))
                return null;

            var url = GetUrl(message.ConversationId + "/messages");
            var result = GetData<FacebookSendedMessage>(url, "POST", "message=" + text);
            return result != null ? result.Id : null;
        }

        public string SendToMessangerByWebHook(string recipientId, string text)
        {
            // https://developers.facebook.com/docs/messenger-platform/send-messages

            var msg = new FbSendMessage()
            {
                messaging_type = "RESPONSE",
                recipient = new FbSendMessageRecipient() { id = recipientId },
                message = new FbSendMessageText() { text = text }
            };
            
            var response = GetData<FbSendResponse>(GetUrl("me/messages"), "POST", JsonConvert.SerializeObject(msg));

            return response != null ? response.message_id : null;
        }

        #endregion

        #region Add Facebook User By Link

        private string TryGetIdByPage(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
                
                var content = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                                content = reader.ReadToEnd();
                }
                
                if (!string.IsNullOrEmpty(content))
                {
                    var rx = Regex.Match(content, "fb://profile/(.*?)\"");

                    if (rx.Success && rx.Groups.Count > 1)
                    {
                        return rx.Groups[1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }


        public bool AddFacebookUserByLink(Guid customerId, string link)
        {
            link = link.Split('?')[0];

            // Facebook запретил получать id пользователя по username, поэтому парсим страницу в поисках id.
            // На случай если перестанет работать есть сервисы: findmyfbid.com, lookup-id.com
            var userId = TryGetIdByPage(link);
            if (userId == null)
                throw new BlException("Не удалось найти пользователя");
            
            var customer = CustomerService.GetCustomer(customerId);

            var user = GetUsersInfo(new List<string>() { userId }).FirstOrDefault();
            if (user == null)
                throw new BlException("Не удалось найти пользователя");

            var u = FacebookService.GetUser(user.Id);
            if (u != null)
            {
                var c = CustomerService.GetCustomer(u.CustomerId);
                if (c != null)
                    throw new BlException(
                        String.Format(
                            "Аккаунт {0} уже прикреплен к пользователю <a href=\"{1}\" target=\"_blank\">{2} {3}</a>",
                            link, UrlService.GetAdminUrl("customers/view/" + c.Id), c.FirstName, c.LastName));

                u.CustomerId = customer.Id;
                FacebookService.UpdateUser(u);
            }
            else
            {
                user.CustomerId = customer.Id;
                FacebookService.AddUser(user);
            }

            SocialNetworkService.SaveAvatar(customer, "https://graph.facebook.com/" + user.Id + "/picture?type=large");

            return true;
        }

        #endregion
        
        #region help

        private string GetUrl(string url, FbParam queryParams = null)
        {
            url += (url.Contains('?') ? "&" : "?");

            if (queryParams != null)
                foreach (var queryParam in queryParams)
                    url += queryParam.Key + "=" + queryParam.Value + "&";

            url += "access_token=" + SettingsFacebook.GroupToken;
            return url;
        }

        private T GetData<T>(string url, string method = "GET", string data = null) where T : class
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(FbGraphUrl + url);
                request.Method = method;
                request.ContentType = "application/json";

                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                                responseContent = reader.ReadToEnd();
                }
                
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                using (var eStream = eResponse.GetResponseStream())
                using (var reader = new StreamReader(eStream))
                {
                    var error = reader.ReadToEnd();
                    Debug.Log.Error(url + " " + error);

                    // https://developers.facebook.com/docs/graph-api/advanced/rate-limiting
                    if (error.Contains("Page request limit reached") ||
                        error.Contains("Access to this data is temporarily disabled for non-active accounts due to changes we are making to the Facebook Platform"))
                    {
                        SettingsFacebook.StopTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        #endregion
    }
}
