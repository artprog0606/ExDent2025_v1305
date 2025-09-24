using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Newtonsoft.Json;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkAuthError = VkNet.Model.VkAuthError;

namespace AdvantShop.Core.Services.Crm.Vk
{
    public class VkApiService
    {
        #region Auth

        public VkApi Auth(bool secondTry = false, int? requestsPerSecond = 2)
        {
            if (SettingsVk.UserTokenData == null
                || SettingsVk.UserTokenData.access_token.IsNullOrEmpty()
                || SettingsVk.UserId == 0)
            {
                Debug.Log.Info($"VkApiService.Auth UserTokenData {(SettingsVk.UserTokenData != null ? JsonConvert.SerializeObject(SettingsVk.UserTokenData) : "is null")}, user_id: {SettingsVk.UserId}");
                return null;
            }

            VkApi vk = null;

            try
            {
                vk = VkApiInstance.GetInstance(requestsPerSecond);
                
                if (VkUserTokenTimer.NeedRefresh())
                {
                    vk = RefreshToken()
                        ? vk.AuthorizeByUserToken(requestsPerSecond) 
                        : null;
                }
            }
            catch (UserAuthorizationFailException ex)
            {
                Debug.Log.Error(ex);

                if (!secondTry && RefreshToken())
                {
                    vk.AuthorizeByUserToken(requestsPerSecond);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                var errors = SettingsVk.TokenUserErrorCount;
                if (errors > 5)
                {
                    SettingsVk.TokenUserErrorCount = 0;
                    SettingsVk.UserTokenData = null;
                    SettingsVk.UserId = 0;
                }
                else
                {
                    SettingsVk.TokenUserErrorCount = errors + 1;
                }
            }

            return vk;
        }
        
        /// <summary>
        /// Get access token 
        /// https://id.vk.com/about/business/go/docs/ru/vkid/latest/vk-id/connection/api-integration/api-description#Poluchenie-cherez-kod-podtverzhdeniya
        /// </summary>
        public Result<ExchangeCodeResponse> GetAccessToken(ExchangeCode exchangeCode)
        {
            var data = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code_verifier", exchangeCode.code_verifier ?? SettingsVk.UserTokenCodeVerifier },
                { "redirect_uri", exchangeCode.redirect_uri },
                { "code", exchangeCode.code },
                { "client_id", exchangeCode.client_id },
                { "device_id", exchangeCode.device_id },
                { "state", exchangeCode.state }
            };

            var dataStr = string.Join("&", data.Select(x => $"{x.Key}={x.Value}"));
            
            try
            {
                var response = MakeRequest<ExchangeCodeResponse>("https://id.vk.com/oauth2/auth", dataStr);
                
                if (!response.IsSuccess || response.Value?.access_token == null)
                {
                    // если code_verifier не правильно собран, то ошибки не будет, но придет пустой access_token
                    if (response.Value?.access_token == null)
                        Debug.Log.Info($"GetAccessToken response {JsonConvert.SerializeObject(response.Value)} data: {dataStr}");
                    
                    return Result.Failure<ExchangeCodeResponse>(new Error("Ошибка при авторизации"));
                }

                return response;
            }
            catch (Exception)
            {
                return Result.Failure<ExchangeCodeResponse>(new Error("Ошибка при авторизации"));
            }
        }
        
        /// <summary>
        /// Refresh user token
        /// https://id.vk.com/about/business/go/docs/ru/vkid/latest/vk-id/connection/api-integration/api-description#Poluchenie-cherez-Refresh-token
        /// </summary>
        public bool RefreshToken()
        {
            try
            {
                var tokenData = SettingsVk.UserTokenData;
                
                if (tokenData == null)
                    return false;

                var data = new Dictionary<string, string>()
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", tokenData.refresh_token },
                    { "client_id", tokenData.client_id },
                    { "device_id", tokenData.device_id },
                    { "state", tokenData.state }
                };
                
                var dataStr = string.Join("&", data.Select(x => $"{x.Key}={x.Value}"));
                
                var refreshTokenResult = MakeRequest<RefreshTokenResponse>("https://id.vk.com/oauth2/auth", dataStr);

                if (!refreshTokenResult.IsSuccess || refreshTokenResult.Value?.access_token == null)
                {
                    if (refreshTokenResult.Value?.access_token == null)
                        Debug.Log.Info($"RefreshToken response {JsonConvert.SerializeObject(refreshTokenResult.Value)} data: {dataStr}");
                    
                    SettingsVk.UserTokenData = null;
                    
                    return false;
                }
                
                Debug.Log.Info("vk refresh token success");
                
                SettingsVk.UserTokenData = SettingsVk.UserTokenData.Refresh(refreshTokenResult.Value);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("vk RefreshToken", ex);
                
                SettingsVk.UserTokenData = null;
                
                return false;
            }
        }

        public VkApi AuthGroup()
        {
            if (string.IsNullOrEmpty(SettingsVk.TokenGroup) || SettingsVk.UserId == 0)
                return null;

            try
            {
                return VkApiGroupInstance.GetInstance(requestsPerSecond: 3);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                var errors = SettingsVk.TokenGroupErrorCount;
                if (errors > 5)
                {
                    SettingsVk.TokenGroupErrorCount = 0;
                    SettingsVk.TokenGroup = null;
                    SettingsVk.UserId = 0;
                }
                else
                {
                    SettingsVk.TokenGroupErrorCount = errors + 1;
                }
            }

            return null;
        }

        #endregion

        public List<VkGroup> GetUserGroups()
        {
            try
            {
                var vk = Auth();
                if (vk == null)
                {
                    Debug.Log.Info("GetUserGroups vk == null");
                    return null;
                }

                var groups = vk.Groups.Get(new GroupsGetParams() {Filter = GroupsFilters.Moderator});
                if (groups != null && groups.Count > 0)
                {
                    return
                        vk.Groups.GetById(groups.Select(x => x.Id.ToString()), null, GroupsFields.Description)
                            .Select(x => new VkGroup(x))
                            .ToList();
                }
                else
                {
                    Debug.Log.Info("GetUserGroups groups count = 0");
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Активность интеграции с Vk
        /// </summary>
        /// <returns></returns>
        public static bool IsVkActive()
        {
            return !string.IsNullOrEmpty(SettingsVk.TokenGroup) && SettingsVk.UserId != 0;
        }

        /// <summary>
        /// Получаем все сообщения из группы
        /// </summary>
        public void GetLastMessagesByApi()
        {
            var isFirstTime = !SettingsVk.IsMessagesLoaded;

            if (SettingsVk.GroupMessageErrorStatus != "")
                SettingsVk.GroupMessageErrorStatus = "";

            var vk = AuthGroup();
            if (vk == null)
                return;
            
            var messages = GetGroupMessages(vk);

             var userAuth = Auth();
             if (userAuth != null)
                 messages.AddRange(GetLastPostComments(userAuth));

             if (messages.Count > 0)
             {
                 VkService.AddMessages(messages);

                 var userIds = messages.Where(x => x.UserId != null).Select(x => x.UserId.Value).Distinct();
                 var users = GetUsersInfo(userIds, vk);

                 SaveUsers(users, messages, isFirstTime);
             }

             if (!SettingsVk.IsMessagesLoaded)
                SettingsVk.IsMessagesLoaded = true;
        }

        private List<VkMessage> GetGroupMessages(VkApi vk)
        {
            var messages = new List<VkMessage>();

            if (!SettingsVk.CreateLeadFromMessages)
                return messages;
            
            MessagesGetObject dialogs;

            try
            {
                dialogs = vk.Messages.GetDialogs(new MessagesDialogsGetParams() { Count = 50 });
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex.Message.Contains("Access denied"))
                {
                    SettingsVk.GroupMessageErrorStatus = "У вас не включены сообщения в группе ВКонтакте. Проверьте настройки группы во ВКонтакте.";
                    throw new VkMessengerGroupMessageError(ex.Message);
                }
                Debug.Log.Error(ex);
                return messages;
            }

            if (dialogs == null || dialogs.Messages.Count == 0)
                return messages;

            foreach (var dialogMessage in dialogs.Messages.Where(x => x.Deleted == null || !x.Deleted.Value))
            {
                if (dialogMessage.Id == null || dialogMessage.UserId == null)
                    continue;

                var dbMessage = VkService.GetMessage(dialogMessage.Id.Value);
                if (dbMessage != null)
                    continue;

                var dialogMessages = GetDialogMessages(vk, dialogMessage.UserId.Value);
                //костыльчик тк сообщения приходят с пустым UserId
                dialogMessages.ForEach(x => x.UserId = dialogMessage.UserId);
                if (dialogMessages.Count > 0)
                    messages.AddRange(dialogMessages.Select(x => new VkMessage(x, true)));
            }

            return messages;
        }

        private List<Message> GetDialogMessages(VkApi vk, long userId)
        {
            var messages = new List<Message>();
            var userMessages = VkService.GetUserMessageIds(userId);

            for (var i = 0; i < 3; i++)
            {
                var msgs = vk.Messages.GetHistory(new MessagesGetHistoryParams()
                {
                    Count = 200,
                    Offset = i * 200,
                    UserId = userId,
                });

                if (msgs.Messages != null && msgs.Messages.Any())
                {
                    messages.AddRange(msgs.Messages.Where(x => x.Id != null && !userMessages.Contains(x.Id.Value)));
                }

                if (msgs.TotalCount < (i + 1) * 200)
                    break;

                Thread.Sleep(100);
            }

            return messages;
        }

        #region Save users and create lead

        private void SaveUsers(List<VkUser> users, List<VkMessage> messages, bool isFirstTime)
        {
            if (users == null || users.Count == 0)
                return;

            var finalDealStatusId = SettingsCrm.VkFinalDealStatusId;

            foreach (var user in users)
            {
                var u = VkService.GetUser(user.Id);
                if (u != null)
                {
                    if (isFirstTime)
                        continue;

                    var createLead = messages.Where(x => x.UserId == u.Id).Any(x => x.CreateLead);
                    if (createLead)
                    {
                        // Если все лиды закрыты, то создаем новый
                        var customerLeads = LeadService.GetLeadsByCustomer(u.CustomerId);
                        var hasNoClosedLeads = customerLeads.Any(x => x.DealStatusId != finalDealStatusId);
                        if (!hasNoClosedLeads)
                        {
                            CreateLead(CustomerService.GetCustomer(u.CustomerId), u, messages);
                            continue;
                        }

                        // Если пришло сообщение к товару и открытых лидов с таким товаром нет, то создаем новый
                        var msg = messages.FirstOrDefault(x => x.Offers != null && x.Offers.Count > 0);
                        if (msg != null)
                        {
                            var hasNoOpenLeadWithSameOffer =
                                customerLeads.Any(
                                    x =>
                                        x.DealStatusId != finalDealStatusId &&
                                        x.LeadItems != null &&
                                        x.LeadItems.Count == 1 &&
                                        x.LeadItems.Find(i => msg.Offers.Any(o => o.ArtNo == i.ArtNo)) != null);

                            if (!hasNoOpenLeadWithSameOffer)
                            {
                                CreateLead(CustomerService.GetCustomer(u.CustomerId), u, messages);
                                continue;
                            }
                        }
                    }

                    BizProcessExecuter.MessageReply(CustomerService.GetCustomer(u.CustomerId), EMessageReplyFieldType.Vk);

                    var userMsg = messages.Where(x => x.UserId == u.Id && x.CreateLead).OrderByDescending(x => x.Date).FirstOrDefault();
                    if (userMsg != null)
                    {
                        var informer = new AdminInformer(AdminInformerType.Vk, userMsg.Id, u.CustomerId);

                        var customer = CustomerService.GetCustomer(u.CustomerId);
                        if (customer.Manager != null && customer.Manager.Customer != null)
                            informer.PrivateCustomerId = customer.Manager.CustomerId;

                        AdminInformerService.Add(informer);
                        if (userMsg.Type == VkMessageType.Received)
                            BizProcessExecuter.LeadEvent(userMsg.Id, LeadEventType.Vk);
                    }

                    continue;
                }

                try
                {
                    var phone = user.MobilePhone ?? user.HomePhone;

                    // add customer
                    var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Phone = phone,
                        StandardPhone = !string.IsNullOrEmpty(phone) ? StringHelper.ConvertToStandardPhone(phone) : null,
                        IsAgreeForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter 
                                                          && SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
                    };
                    CustomerService.InsertNewCustomer(customer);

                    SocialNetworkService.SaveAvatar(customer, user.Photo100);

                    // add vk user
                    user.CustomerId = customer.Id;
                    VkService.AddUser(user);

                    // add lead
                    var createLead = messages.Where(x => x.UserId == user.Id).Any(x => x.CreateLead);
                    if (!isFirstTime && createLead)
                        CreateLead(customer, user, messages);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        private void CreateLead(Customer customer, VkUser user, List<VkMessage> messages)
        {
            var source = OrderSourceService.GetOrderSource(OrderType.Vk);
            var customerMessages = messages.Where(x => x.UserId == user.Id).ToList();
            var lastMessage = customerMessages.Count > 0 ? customerMessages.OrderByDescending(x => x.Date).FirstOrDefault() : null;

            var lead = new Lead()
            {
                CustomerId = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                //Comment = String.Join("<br>\r\n ", customerMessages.OrderBy(x => x.Date).Select(x => x.Body)),
                OrderSourceId = source.Id,
                CreatedDate = lastMessage != null && lastMessage.Date != null ? lastMessage.Date.Value.AddSeconds(-1) : DateTime.Now,
                LeadItems = new List<LeadItem>(),
                SalesFunnelId = SettingsCrm.DefaultVkSalesFunnelId
            };

            foreach (var offer in customerMessages.Where(x => x.Offers != null && x.Offers.Count > 0).SelectMany(x => x.Offers))
            {
                lead.LeadItems.Add(new LeadItem(offer, 1));
            }

            LeadService.AddLead(lead, true);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_SocialNetwork, "Vk");
        }

        #endregion

        #region Last post comments

        /// <summary>
        /// Получаем новые комментарии из постов
        /// </summary>
        /// <param name="vk"></param>
        public List<VkMessage> GetLastPostComments(VkApi vk)
        {
            var messages = new List<VkMessage>();

            if (!SettingsVk.CreateLeadFromComments)
                return messages;

            // Берем 30 последних постов
            var vkMessages = vk.Wall.Get(new WallGetParams() { Count = 30, OwnerId = -SettingsVk.Group.Id });

            if (vkMessages == null || vkMessages.WallPosts.Count == 0)
                return messages;

            var postsCount = VkService.GetPostMessagesCount();

            foreach (var post in vkMessages.WallPosts)
            {
                if (post.Id == null || post.Comments.Count == 0)
                    continue;

                // Если кол-во комментариев не изменилось, то выходим
                var pc = postsCount.Find(x => x.PostId == post.Id);
                if (pc != null && pc.Count == post.Comments.Count)
                    continue;

                var comments = GetComments(vk, post);
                if (comments.Count == 0)
                    continue;

                var dbComments = VkService.GetPostMessages(post.Id.Value);

                foreach (var comment in comments)
                {
                    if (dbComments.Find(x => x.MessageId == comment.MessageId) != null)
                        continue;

                    messages.Add(comment);
                }
            }

            return messages;
        }

        public List<VkMessage> GetComments(VkApi vk, Post post)
        {
            var comments = new List<Comment>();
            var count = Math.Ceiling((decimal)post.Comments.Count / 100);

            for (var i = 0; i < count; i++)
            {
                try
                {
                    var c = vk.Wall.GetComments(new WallGetCommentsParams()
                    {
                        Count = 100,
                        Offset = i * 100,

                        OwnerId = -SettingsVk.Group.Id,
                        PostId = post.Id.Value,
                        Extended = true,
                    });
                    
                    if (c != null && c.Items != null && c.Items.Count > 0)
                        comments.AddRange(c.Items);

                    Thread.Sleep(200);
                }
                catch (VkNet.Exception.TooManyRequestsException)
                {
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex);
                }
            }

            return comments.Select(x => new VkMessage(post, x, true)).ToList();
        }

        #endregion

        #region Send message to user

        /// <summary>
        /// Посылает личное сообщение
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, которому отправляется сообщение</param>
        /// <param name="message">Сообщение</param>
        /// <returns>Идентификатор отправленного сообщения</returns>
        public long SendMessageByGroup(long userId, string message)
        {
            try
            {
                var vk = AuthGroup();
                if (vk == null)
                    throw new BlException("VkService.SendMessage авторизация не прошла");

                var groupId = -SettingsVk.Group.Id;

                var messageId = vk.Messages.Send(new MessagesSendParams()
                {
                    Message = message ?? "",
                    UserId = userId,
                    PeerId = groupId,
                    RandomId = DateTime.Now.Ticks
                });

                // add to db
                VkService.AddMessage(new VkMessage()
                {
                    MessageId = messageId,
                    UserId = userId,
                    FromId = groupId,
                    Body = message ?? "",
                    Type = VkMessageType.Sended,
                });

                return messageId;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                if (ex.Message == "Can't send messages for users without permission")
                    throw new BlException("Не удалось отправить сообщение. (Группы не могут писать первыми, только отвечать на сообщения)");
            }

            return 0;
        }

        /// <summary>
        /// Посылаем сообщение на стену
        /// </summary>
        /// <param name="id">Id in db</param>
        /// <param name="messageText">Сообщение</param>
        /// <returns>Идентификатор отправленного сообщения</returns>
        public long SendMessageToWall(int id, string messageText)
        {
            try
            {
                var message = VkService.GetMessage(id);
                if (message == null || message.PostId == null)
                    throw new BlException("Пост не найден");

                var vk = Auth();
                if (vk == null)
                    throw new BlException("Авторизация для отправки сообщений на стену не удалась. Пожалуйста проверьте настройки ВКонтакте в CRM.");

                var groupId = -SettingsVk.Group.Id;

                var messageId = vk.Wall.CreateComment(new WallCreateCommentParams()
                {
                    Message = messageText ?? "",
                    OwnerId = -SettingsVk.Group.Id,
                    FromGroup = SettingsVk.Group.Id,
                    PostId = message.PostId.Value,
                    ReplyToComment = message.MessageId
                });

                // add to db
                VkService.AddMessage(new VkMessage()
                {
                    MessageId = messageId,
                    UserId = message.UserId,
                    FromId = groupId,
                    Body = messageText ?? "",
                    PostId = message.PostId.Value,
                    Type = VkMessageType.Sended,
                });

                return messageId;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                if (ex.Message == "Access denied: no access to call this method")
                    throw new BlException("Доступ запрещен. Для того чтобы отвечать на сообщения от лица группы необходимо войти по логину и паролю в настройках интеграции в CRM");
            }

            return 0;
        }

        #endregion

        #region GetUsersInfo

        public List<VkUser> GetUsersInfoByIds(IEnumerable<long> userIds)
        {
            return GetUsersInfo(userIds, null);
        }

        public List<VkUser> GetUsersInfo(IEnumerable<long> userIds, VkApi vkApi)
        {
            try
            {
                var vk = vkApi ?? AuthGroup();

                return
                    vk.Users.Get(userIds,
                        ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.BirthDate |
                        ProfileFields.Photo100 | ProfileFields.Sex | ProfileFields.ScreenName | ProfileFields.IsHiddenFromFeed)
                        .Select(x => new VkUser(x)).ToList();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return null;
        }

        public List<VkUser> GetUsersInfoByIds(IEnumerable<string> sceenNames)
        {
            return GetUsersInfo(sceenNames, null);
        }

        public List<VkUser> GetUsersInfo(IEnumerable<string> sceenNames, VkApi vkApi)
        {
            try
            {
                var vk = vkApi ?? AuthGroup();

                return
                    vk.Users.Get(sceenNames,
                        ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.BirthDate |
                        ProfileFields.Photo100 | ProfileFields.Sex | ProfileFields.ScreenName | ProfileFields.IsHiddenFromFeed)
                        .Select(x => new VkUser(x)).ToList();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }

        #endregion

        public bool AddVkUserByLink(Guid customerId, string link)
        {
            var customer = CustomerService.GetCustomer(customerId);

            var linkId = link.Replace("https://vk.com/", "").Replace("vk.com/", "");


            var id = linkId.Replace("id", "").TryParseLong();
            var user = id != 0
                ? GetUsersInfoByIds(new List<long>() { id }).FirstOrDefault()
                : GetUsersInfoByIds(new List<string>() { linkId }).FirstOrDefault();

            if (user == null)
                throw new BlException("Не удалось найти пользователя");

            var u = VkService.GetUser(user.Id);
            if (u != null)
            {
                var c = CustomerService.GetCustomer(u.CustomerId);
                if (c != null)
                    throw new BlException(
                        String.Format(
                            "Аккаунт {0} уже прикреплен к пользователю <a href=\"{1}\" target=\"_blank\">{2} {3}</a>",
                            link, UrlService.GetAdminUrl("customers/view/" + c.Id), c.FirstName, c.LastName));

                u.CustomerId = customer.Id;
                VkService.UpdateUser(u);
            }
            else
            {
                user.CustomerId = customer.Id;
                VkService.AddUser(user);
            }

            SocialNetworkService.SaveAvatar(customer, user.Photo100);

            return true;
        }

        public void DeleteVkUser(Guid customerId)
        {
            var u = VkService.GetUser(customerId);
            if (u != null)
                VkService.DeleteUser(u.Id);
        }
        
        private Result<T> MakeRequest<T>(string url, string data)
        {
            var result = RequestHelper.MakeRequest<string>(url, data, method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

            if (result.Contains("error"))
            {
                var errorResponse = JsonConvert.DeserializeObject<VkAuthError>(result);
                if (errorResponse != null)
                {
                    Debug.Log.Error($"{url} {errorResponse.Error} {errorResponse.ErrorDescription} data: {data}");

                    return Result.Failure<T>(new Error(errorResponse.ErrorDescription));
                }
            }

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
