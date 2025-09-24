using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;

namespace AdvantShop.Core.Services.Crm.Instagram
{
    // Lib sources: https://github.com/itm-dev/InstaSharp


    public class Instagram
    {
        private static InstagramApiService _instance;
        public static InstagramApiService Instance
        {
            get { return _instance ?? (_instance = new InstagramApiService()); }
        }
    }


    public class InstagramApiService
    {
        private static IInstaApi _api;

        public bool IsActive()
        {
            return !string.IsNullOrEmpty(SettingsInstagram.Login) &&
                   !string.IsNullOrEmpty(SettingsInstagram.Password);
        }

        public void DeActivate()
        {
            _api = null;
            SettingsInstagram.InstaApiData = null;

            SettingsInstagram.Login = "";
            SettingsInstagram.Password = "";
            SettingsInstagram.ErrorCount = 0;

            SettingsInstagram.LastTheadActivity = DateTime.MinValue;
            SettingsInstagram.LastMediaActivity = DateTime.MinValue;
        }

        #region Auth

        public IInstagramAuthResult TryAuth(string login, string password)
        {
            return System.Threading.Tasks.Task.Run(() => TryAuthAsync(login, password)).Result;
        }

        public async Task<IInstagramAuthResult> TryAuthAsync(string username, string password)
        {
            try
            {
                await GetApi(username, password);

                var user = await _api.GetCurrentUserAsync();
                if (!user.Succeeded)
                    return new InstagramAuthResult(false, user.Info.Message);

                AfterSuccessAuth(user.Value.Pk, user.Value.UserName, password);

                return new InstagramAuthResult(true);
            }
            catch (InstagramChallengeRequiredException ex)
            {
                Debug.Log.Error(ex.Message + " " + ex.ChallengeRequired.Json);
                
                return new InstagramAuthChallengeRequiredResult(false, ex.Message)
                {
                    ApiPath = ex.ChallengeRequired.Challenge.ApiPath
                };
            }
            catch (BlException ex)
            { 
                return new InstagramAuthResult(false, ex.Message);
            }
        }

        private void AfterSuccessAuth(long pk, string username, string password)
        {
            SettingsInstagram.UserPk = pk.ToString();
            SettingsInstagram.UserName = username;

            SettingsInstagram.Login = username;
            SettingsInstagram.Password = password;
            SettingsInstagram.ErrorCount = 0;

            System.Threading.Tasks.Task.Run(async () => await Instagram.Instance.GetLastMessages());
        }

        #endregion

        public async Task<IInstaApi> GetApi(string username = null, string password = null)
        {
            InstaApiData apiData;
            UserSessionData user;

            var tryAuth = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
            if (tryAuth)
            {
                apiData = new InstaApiData();
                user = new UserSessionData() {UserName = username, Password = password};
                _api = null;
            }
            else
            {
                apiData = SettingsInstagram.InstaApiData;

                user = apiData.User ?? new UserSessionData()
                {
                    UserName = SettingsInstagram.Login,
                    Password = SettingsInstagram.Password
                };
            }
            

            if (_api == null)
            {
                _api =
                    InstaApiBuilder.CreateBuilder()
                        .SetUser(user)
                        .SetDevice(apiData.DeviceInfo)
                        .SetApiRequestMessage(apiData.RequestMethod)
                        .SetCookie(apiData.CookieContainer, apiData.Cookies)
                        .SetRequestDelay(RequestDelay.FromSeconds(4, 5))
                        .Build();
            }

            var data = _api.GetData();
            var needLogin = data.User == null || data.User.LastLoginTime == null ||
                            (DateTime.Now - data.User.LastLoginTime.Value).TotalHours > 3;

            if (needLogin)
            {
                var login = await _api.LoginAsync();
                if (!login.Succeeded)
                {
                    if (data.User != null)
                        data.User.LastLoginTime = null;

                    if (!tryAuth)
                    {
                        SettingsInstagram.InstaApiData = data;
                    }
                    else
                    {
                        if (login.Value != null && login.Value.Status == InstaLoginStatus.ChallengeRequired)
                        {
                            var challengeRequired = login.Value.Error as ChallengeRequiredResponse;

                            throw new InstagramChallengeRequiredException(
                                "Instagram: не удалось авторизоваться. Необходимо подтвердить аккаунт по sms или email.",
                                challengeRequired);
                        }
                    }

                    throw new BlException("Instagram: не удалось авторизоваться. " + login.Info.Message);
                }

                SaveInstaApiData();
            }

            return _api;
        }

        private void SaveInstaApiData()
        {
            var data = _api.GetData();

            if (data != null && data.User != null)
                data.User.LastLoginTime = DateTime.Now;

            SettingsInstagram.InstaApiData = data;
        }

        /// <summary>
        /// Получаем сообщения из комментариев и из директа
        /// </summary>
        public async Task<bool> GetLastMessages()
        {
            var isFirstTime = SettingsInstagram.LastTheadActivity == DateTime.MinValue &&
                              SettingsInstagram.LastMediaActivity == DateTime.MinValue;

            await GetApi();

            var results = await System.Threading.Tasks.Task.WhenAll(GetNewDirectMessages(), GetNewMediaMessages());

            var messages = new List<InstagramMessage>();
            messages.AddRange(results[0]);
            messages.AddRange(results[1]);
            
            InstagramService.AddMessages(messages);

            SaveUsers(messages, isFirstTime);

            if (SettingsInstagram.LastTheadActivity == DateTime.MinValue)
                SettingsInstagram.LastTheadActivity = DateTime.MinValue.AddSeconds(1);

            return true;
        }

        /// <summary>
        /// Получаем новые сообщения из директа
        /// </summary>
        private async Task<List<InstagramMessage>> GetNewDirectMessages()
        {
            var messages = new List<InstagramMessage>();

            if (!SettingsInstagram.CreateLeadFromDirectMessages)
                return messages;

            var pendingDirectInbox = await _api.GetPendingDirectInboxAsync();
            if (!pendingDirectInbox.Succeeded)
            {
                Debug.Log.Warn("Instagram GetPendingDirectInboxAsync " + pendingDirectInbox.Info.Message);
            }
            else
            {
                if (pendingDirectInbox.Value != null && 
                    pendingDirectInbox.Value.Inbox != null &&
                    pendingDirectInbox.Value.Inbox.Threads != null)
                {
                    var peningThreadIds = pendingDirectInbox.Value.Inbox.Threads.Select(x => x.ThreadId).ToList();
                    if (peningThreadIds.Count > 0)
                    {
                        var res = await _api.ApprovePendingDirectThreads(peningThreadIds);
                        if (!res.Succeeded)
                            Debug.Log.Warn("Instagram ApprovePendingDirectThreads " + res.Info.Message);
                    }
                }
            }

            var directInbox = await _api.GetDirectInboxAsync();
            if (!directInbox.Succeeded)
            {
                Debug.Log.Error("Instagram GetDirectInboxAsync " + directInbox.Info.Message);
                return messages;
            }

            var threads = directInbox.Value.Inbox.Threads;

            if (threads == null || threads.Count == 0)
                return messages;

            var lastDate = SettingsInstagram.LastTheadActivity;

            foreach (var thread in threads.Where(x => x.LastActivity > lastDate && !x.IsSpam))
            {
                var th = await _api.GetDirectInboxThreadAsync(thread.ThreadId);
                if (!th.Succeeded)
                {
                    Debug.Log.Error("Instagram: !th.Succeeded " + th.Info.Message);
                    continue;
                }

                foreach (var message in th.Value.Items.Where(x => x.TimeStamp > lastDate))
                {
                    var toUser = th.Value.Users != null ? th.Value.Users.FirstOrDefault() : null;

                    var text = message.Text;

                    if (message.ItemType == InstaDirectThreadItemType.Media && message.Media != null &&
                        message.Media.MediaType == InstaMediaType.Image && 
                        message.Media.Images != null && message.Media.Images.Count > 0)
                    {
                        text = message.Media.Images.Aggregate("",
                            (current, image) => current + string.Format("<img src=\"{0}\" /> ", image.URI));
                    }

                    if (string.IsNullOrWhiteSpace(text))
                        continue;

                    messages.Add(new InstagramMessage()
                    {
                        ThreadId = thread.ThreadId,
                        Title = "Direct " + thread.Title,

                        InstagramId = message.ItemId,
                        Text = text,
                        FromUserPk = message.UserId.ToString(),
                        ToUserPk = toUser != null ? toUser.Pk.ToString() : message.UserId.ToString(),
                        CreatedDate = message.TimeStamp.ToLocalTime(),
                        ItemType = message.ItemType == InstaDirectThreadItemType.Text
                                        ? InstagramMessageType.DirectText
                                        : InstagramMessageType.DirectMedia
                    });
                }
            }

            SettingsInstagram.LastTheadActivity = threads[0].LastActivity;

            return messages;
        }

        /// <summary>
        /// Получаем новые сообщения из комментариев
        /// </summary>
        private async Task<List<InstagramMessage>> GetNewMediaMessages()
        {
            var messages = new List<InstagramMessage>();

            if (!SettingsInstagram.CreateLeadFromComments)
                return messages;

            var medias = await _api.GetUserMediaAsync(SettingsInstagram.UserPk.TryParseLong(), PaginationParameters.MaxPagesToLoad(1));
            if (!medias.Succeeded)
            {
                Debug.Log.Error("Instagram GetUserMediaAsync " + medias.Info.Message);
                return messages;
            }

            if (medias.Value == null || medias.Value.Count == 0)
                return messages;

            var lastDate = SettingsInstagram.LastMediaActivity;

            foreach (var media in medias.Value)
            {
                var commentsCount = media.CommentsCount.TryParseInt();
                if (commentsCount <= 0)
                    continue;

                var commentsCountInDb = InstagramService.GetMessagesCount(media.Pk);
                if (commentsCount == commentsCountInDb)
                    continue;

                var comments = await _api.GetMediaCommentsAsync(media.Pk, PaginationParameters.MaxPagesToLoad(3)); // todo: pagescount = commentsCount/?
                if (!comments.Succeeded)
                {
                    Debug.Log.Error("Instagram GetMediaCommentsAsync " + comments.Info.Message);
                    continue;
                }

                foreach (var comment in comments.Value.Comments.Where(x => x.CreatedAt > lastDate))
                {
                    var toUser = await GetUserInText(comment.Text);
                    var postLink = "https://www.instagram.com/p/" + media.Code;

                    messages.Add(new InstagramMessage()
                    {
                        MediaPk = media.Pk,
                        Title = String.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", postLink, media.Caption != null ? media.Caption.Text : postLink),
                        InstagramId = comment.Pk.ToString(),
                        Text = comment.Text,
                        FromUserPk = comment.UserId.ToString(),
                        ToUserPk = toUser != null ? toUser.Pk : "",
                        CreatedDate = comment.CreatedAt.ToLocalTime(),
                        ItemType = InstagramMessageType.Media
                    });
                }
            }

            if (messages.Count > 0)
                SettingsInstagram.LastMediaActivity = messages.OrderByDescending(x => x.CreatedDate).Select(x => x.CreatedDate).FirstOrDefault();

            return messages;
        }

        private async void SaveUsers(List<InstagramMessage> messages, bool isFirstTime)
        {
            if (messages.Count == 0)
                return;

            var users = new List<InstagramUser>();

            var userPks = messages.Select(x => x.FromUserPk).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var existUsers = InstagramService.GetUsersByPks(userPks);
            var existUserPks = existUsers.Select(x => x.Pk).ToList();

            // add exist users
            users.AddRange(existUsers);

            // add new users
            foreach (var pk in userPks.Where(x => !existUserPks.Contains(x)))
            {
                try
                {
                    var userResult = await _api.GetUserInfoByIdAsync(pk.TryParseLong());
                    if (!userResult.Succeeded)
                        continue;

                    var u = userResult.Value;

                    var user = new InstagramUser()
                    {
                        Pk = u.Pk.ToString(),
                        UserName = u.Username,
                        FullName = u.FullName,
                        //PhoneNumber = u.PhoneNumber,
                        //Email = u.Email,
                        ProfilePicture = u.ProfilePicUrl
                    };
                    users.Add(user);
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex);
                }
            }

            Debug.Log.Warn("Instagram: before SaveUser: users.Count - " + users.Count +
                            " messages.Count - " + messages.Count + " isFirstTime - " + isFirstTime);

            foreach (var user in users)
                SaveUser(user, messages, isFirstTime);
        }

        private void SaveUser(InstagramUser user, List<InstagramMessage> messages, bool isFirstTime)
        {
            try
            {
                // если это мы, то лид не создаем
                if (user.Pk == SettingsInstagram.UserPk)
                {
                    var cu = InstagramService.GetUser(user.Pk);
                    if (cu == null)
                    {
                        var c = user.ToCustomer();
                        user.CustomerId = CustomerService.InsertNewCustomer(c);
                        InstagramService.AddUser(user);

                        SocialNetworkService.SaveAvatar(c, user.ProfilePicture);
                    }
                    return;
                }

                var u = InstagramService.GetUser(user.Pk);
                if (u != null)
                {
                    // Если все лиды закрыты, то создаем новый
                    var customerLeads = LeadService.GetLeadsByCustomer(u.CustomerId);
                    var hasNoClosedLeads = customerLeads.Any(x => !SettingsCrm.InstagramFinalDealStatusIds.Contains(x.DealStatusId));
                    if (!hasNoClosedLeads)
                    {
                        // Если все заказы закрыты, то создаем лид
                        var hasNoClosedOrders = false; //OrderService.GetCustomerOrderHistory(u.CustomerId).Any(x => !OrderStatusService.GetOrderStatus(x.StatusID).IsCompleted);
                        foreach (var statusId in OrderService.GetCustomerOrderHistory(u.CustomerId).Select(x => x.StatusID))
                        {
                            var status = OrderStatusService.GetOrderStatus(statusId);
                            if (status != null && !status.IsCompleted && !status.IsCanceled)
                            {
                                hasNoClosedOrders = true;
                                break;
                            }
                        }

                        if (!hasNoClosedOrders || customerLeads.Count == 0)
                        {
                            CreateLead(CustomerService.GetCustomer(u.CustomerId), u, messages);
                            return;
                        }
                    }

                    BizProcessExecuter.MessageReply(CustomerService.GetCustomer(u.CustomerId), EMessageReplyFieldType.Instagram);

                    var userMsg = messages.Where(x => x.FromUserPk == user.Pk).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (userMsg != null)
                    {
                        AdminInformerService.Add(new AdminInformer(AdminInformerType.Instagram, userMsg.Id, u.CustomerId));
                        BizProcessExecuter.LeadEvent(userMsg.Id, LeadEventType.Instagram);
                    }

                    return;
                }

                // add customer
                var customer = user.ToCustomer();
                CustomerService.InsertNewCustomer(customer);
                SocialNetworkService.SaveAvatar(customer, user.ProfilePicture);

                // add instagram user
                user.CustomerId = customer.Id;
                InstagramService.AddUser(user);

                if (!isFirstTime)
                    CreateLead(customer, user, messages);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private void CreateLead(Customer customer, InstagramUser user, List<InstagramMessage> messages)
        {
            var source = OrderSourceService.GetOrderSource(OrderType.Instagram);
            var customerMessages = messages.Where(x => x.FromUserPk == user.Pk || x.ToUserPk == user.Pk).ToList();
            var firsttMessage = customerMessages.Count > 0 ? customerMessages.OrderBy(x => x.CreatedDate).FirstOrDefault() : null;

            var lead = new Lead()
            {
                CustomerId = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                //Comment = String.Join("<br>\r\n ", customerMessages.OrderBy(x => x.CreatedDate).Select(x => x.Text + " [" + x.Title + "]")),
                OrderSourceId = source.Id,
                CreatedDate =
                    firsttMessage != null && (DateTime.Now - firsttMessage.CreatedDate).Days < 1
                        ? firsttMessage.CreatedDate.AddSeconds(-1)
                        : DateTime.Now,
                LeadItems = new List<LeadItem>(),
                SalesFunnelId = SettingsCrm.DefaultInstagramSalesFunnelId
            };
            LeadService.AddLead(lead, true);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_SocialNetwork, "Instagram");
        }


        private async Task<InstagramUser> GetUserInText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text[0] != '@')
                return null;

            var index = text.IndexOf(" ", StringComparison.Ordinal);
            if (index == -1)
                return null;

            var userName = text.Substring(1, index - 1);

            var user = InstagramService.GetUserByUserName(userName) ?? await GetUserByName(userName);

            return user;
        }

        public async Task<InstagramUser> GetUserByName(string name)
        {
            var userResult = await _api.GetUserAsync(name);
            if (!userResult.Succeeded)
                return null;

            var u = userResult.Value;

            var user = new InstagramUser()
            {
                Pk = u.Pk.ToString(),
                UserName = u.UserName,
                FullName = u.FullName,
                //PhoneNumber = u.PhoneNumber,
                //Email = u.Email,
                ProfilePicture = u.ProfilePicture
            };

            return user;
        }

        public async Task<bool> SendMessage(int messageId, string customerId, string text)
        {
            if (customerId != null)
            {
                var user = InstagramService.GetUserByCustomerId(customerId.TryParseGuid());
                if (user != null)
                {
                    await GetApi();
                    var result = await _api.SendDirectMessage(user.Pk, null, text);
                    if (result.Succeeded && result.Value.Count > 0)
                    {
                        SaveMessage(result.Value[0], user.Pk);
                        return true;
                    }
                }
                return false;
            }

            var message = InstagramService.GetMessage(messageId);
            if (message == null)
                return false;

            await GetApi();

            if (message.ItemType == InstagramMessageType.Media)
            {
                var user = text[0] != '@' ? InstagramService.GetUser(message.FromUserPk) : null;
                if (user != null)
                    text = "@" + user.UserName + " " + text;

                var result = await _api.CommentMediaAsync(message.MediaPk, text);
                if (!result.Succeeded)
                {
                    Debug.Log.Error("Instagram can't send message: " + text + "; Error: " + result.Info.Message);
                    return false;
                }
                SaveMessage(result.Value, message, message.FromUserPk);
            }
            else
            {
                var result = await _api.SendDirectMessage(message.ToUserPk, message.ThreadId, text);
                if (!result.Succeeded)
                {
                    Debug.Log.Error("Instagram can't send direct message: " + text + "; Error: " + result.Info.Message);
                    return false;
                }
                SaveMessage(result.Value[0], null);
            }

            return true;
        }

        private void SaveMessage(InstaDirectInboxThread thread, string toUserPk)
        {
            var message = thread.Items[0];
            InstagramService.AddMessage(new InstagramMessage()
            {
                ThreadId = thread.ThreadId,
                Title = "Direct " + thread.Title,

                InstagramId = message.ItemId,
                Text = message.Text,
                FromUserPk = message.UserId.ToString(),
                ToUserPk = toUserPk ?? (thread.Users != null && thread.Users.Count > 0 ? thread.Users[0].Pk.ToString() : null) ?? message.UserId.ToString(),
                CreatedDate = message.TimeStamp.ToLocalTime(),
                ItemType = message.ItemType == InstaDirectThreadItemType.Text
                    ? InstagramMessageType.DirectText
                    : InstagramMessageType.DirectMedia
            });
        }

        private void SaveMessage(InstaComment comment, InstagramUserMessage parentMessage, string toUserPk)
        {
            InstagramService.AddMessage(new InstagramMessage()
            {
                MediaPk = parentMessage.MediaPk,
                Title = parentMessage.Title,

                InstagramId = comment.Pk.ToString(),
                Text = comment.Text,
                FromUserPk = comment.UserId.ToString(),
                ToUserPk = toUserPk,
                CreatedDate = comment.CreatedAt.ToLocalTime(),
                ItemType = InstagramMessageType.Media
            });
        }


        public async Task<bool> AddUserByLink(Guid customerId, string link)
        {
            var userIdName = link.Replace("https://www.instagram.com/", "").Trim('/').Split('?')[0];

            await GetApi();

            var user = await GetUserByName(userIdName);

            if (user == null)
                throw new BlException("Не удалось найти пользователя");

            var customer = CustomerService.GetCustomer(customerId);

            var u = InstagramService.GetUser(user.Pk);
            if (u != null)
            {
                var c = CustomerService.GetCustomer(u.CustomerId);
                if (c != null)
                    throw new BlException(
                        String.Format(
                            "Аккаунт {0} уже прикреплен к пользователю <a href=\"{1}\" target=\"_blank\">{2} {3}</a>",
                            link, UrlService.GetAdminUrl("customers/view/" + c.Id), c.FirstName, c.LastName));

                u.CustomerId = customer.Id;
                InstagramService.UpdateUser(u);
            }
            else
            {
                user.CustomerId = customer.Id;
                InstagramService.AddUser(user);
            }

            SocialNetworkService.SaveAvatar(customer, user.ProfilePicture);

            return true;
        }

        /// <summary>
        /// Запрос кода на подтверждение аккаунта из-за checkpoint_challenge_required
        /// </summary>
        /// <param name="apiPath">apiPath</param>
        /// <param name="choice">0 - sms, 1 - email</param>
        /// <returns></returns>
        public async Task<bool> RequireChallengeCode(string apiPath, int choice)
        {
            var result = await _api.RequireChallengeCode(apiPath, choice);
            return result;
        }

        /// <summary>
        /// Отсылаем полученный из sms/email код
        /// </summary>
        /// <param name="apiPath"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool> SendChallengeCode(string apiPath, string code)
        {
            var result = await _api.SendChallengeCode(apiPath, code);
            if (result.Succeeded)
            {
                var user = result.Value.LoggedInUser;
                
                var data = _api.GetData();

                SaveInstaApiData();

                AfterSuccessAuth(user.Pk, user.Username, data.User.Password);

                return true;
            }

            return false;
        }
    }
}
