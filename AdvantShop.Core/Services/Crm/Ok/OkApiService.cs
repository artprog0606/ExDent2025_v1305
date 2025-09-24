using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.Crm.Ok.OkMarket;
using AdvantShop.Core.Services.Crm.OK.Domain;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Crm.Ok
{
    public static class OkApiService
    {
        public static bool IsActive()
        {
            return !string.IsNullOrEmpty(SettingsOk.GroupSocialAccessToken);
        }

        public static void DeActivate()
        {
            WebHookUnsubscribe();
            SettingsOk.GroupSocialAccessToken = null;
            SettingsOk.GroupId = null;
            SettingsOk.GroupName = null;
        }

        /// <summary>
        /// Запрос информации о группе, которой принадлежит API ключ
        /// </summary>
        public static OkGroupResponse GetGroupInfo()
        {
            return RequestHelper.MakeRequest<OkGroupResponse>("https://api.ok.ru/graph/me/info/?access_token=" + SettingsOk.GroupSocialAccessToken,
                method: ERequestMethod.GET);
        }

        private static string BuildUrl(string url)
        {
            return StringHelper.ToPuny(SettingsMain.SiteUrl.Replace("http://", "https://")).TrimEnd('/') + url;
        }
        
        /// <summary>
        /// Запрос на добавление webhook'а
        /// </summary>
        public static bool WebHookSubscribe()
        {
            var result = RequestHelper.MakeRequest<OkBaseResponse>("https://api.ok.ru/graph/me/subscribe?access_token=" + SettingsOk.GroupSocialAccessToken,
                new { url = BuildUrl("/integration/OK/Webhook") }, method: ERequestMethod.POST);
            return result.ErrorMsg == null;
        }

        /// <summary>
        /// Отправить запрос на удаления webhook'а
        /// </summary>
        public static bool WebHookUnsubscribe()
        {
            var result = RequestHelper.MakeRequest<OkBaseResponse>("https://api.ok.ru/graph/me/unsubscribe?access_token=" + SettingsOk.GroupSocialAccessToken,
                new { url = BuildUrl("/integration/OK/Webhook") }, method: ERequestMethod.POST);
            return result.ErrorMsg == null;
        }

        /// <summary>
        /// Получение информации о чате
        /// </summary>
        /// <param name="chatId">ID чата в ОК</param>
        public static OkChatResponse GetChatInfo(string chatId)
        {
            try
            {
                var result = RequestHelper.MakeRequest<OkChatResponse>("https://api.ok.ru/graph/me/chat?access_token=" + SettingsOk.GroupSocialAccessToken + "&chat_id=" + chatId, method: ERequestMethod.GET);
                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new OkChatResponse() { ErrorMsg = ex.Message };
            }
        }

        /// <summary>
        /// Сохранение сообщения из webhook'а
        /// </summary>
        /// <param name="json">Json body</param>
        public static bool SaveMessage(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                var notification = JsonConvert.DeserializeObject<OkWebhookNotification>(json);
                if (notification == null || notification.Message == null)
                    return false;

                _saveMessage(notification);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
            return false;
        }

        private static void _saveMessage(OkWebhookNotification notification)
        {
            var message = new OkMessage(notification);

            if (notification.Message.Attachments != null)
            {
                var attachmentText = "";
                foreach (var attachment in notification.Message.Attachments)
                {
                    switch (attachment.Type)
                    {
                        case AttachmentType.SHARE:
                            if (!attachment.Payload.Url.IsNullOrEmpty())
                            {
                                attachmentText += "<br /><a href=\"" + attachment.Payload.Url + "\" target = \"_blank\" >ссылка</a>";
                            }
                            break;
                        case AttachmentType.IMAGE:
                            attachmentText += "<br /><a href=\"" + attachment.Payload.Url + "\" target = \"_blank\" >изображение</a>";
                            break;
                        case AttachmentType.FILE:
                            attachmentText += "<br /><a href=\"" + attachment.Payload.Url + "\" target = \"_blank\" >файл</a>";
                            break;
                        case AttachmentType.VIDEO:
                            attachmentText += "<br /><a href=\"" + attachment.Payload.Url + "\" target = \"_blank\" >видео</a>";
                            break;
                        case AttachmentType.AUDIO:
                            attachmentText += "<br /><a href=\"" + attachment.Payload.Url + "\" target = \"_blank\" >аудио файл</a>";
                            break;
                        default:
                            break;
                    }
                }
                message.Text += attachmentText.IsNullOrEmpty() ? "" : ((message.Text.IsNullOrEmpty() ? "" : "<br />") + "Прикреплено:" + attachmentText);
            }

            int relatedProductId = 0;
            string relatedAdvertText = "";
            var chat = GetChatInfo(message.ChatId);
            if (chat.ErrorMsg != null)
            {
                Debug.Log.Warn("OK Graph API - " + chat.ErrorMsg);
            }
            else if (chat.AdvertId != null)
            {
                var relatedAdvertId = chat.AdvertId.Split(':')[1];
                relatedAdvertText += message.Text.IsNullOrEmpty() ? "" : "<br />" + "Прикреплен <a href=\"https://ok.ru/group/" + SettingsOk.GroupId + "/product/" + relatedAdvertId + "\" target = \"_blank\" >товар в OK</a>";
                var product = OkMarketService.GetProductByOkId(relatedAdvertId.TryParseLong());
                if (product != null)
                {
                    relatedProductId = product.ProductId;
                    relatedAdvertText += " (<a href=\"" + UrlService.GetAdminUrl("product/edit/" + relatedProductId) + "\" target = \"_blank\" >в Вашем магазине</a>)";
                }
            }

            if (message.Text.IsNullOrEmpty() && relatedAdvertText.IsNullOrEmpty())
            {
                return;
            }

            var user = OkService.GetUser(notification.Sender.UserID.Split(':')[1]);
            if (user != null)
            {
                var customerLeads = LeadService.GetLeadsByCustomer(user.CustomerId);
                var hasNoClosedLeads = customerLeads.Any(x => x.DealStatus.Status != DealStatuses.SalesFunnelStatusType.FinalSuccess && x.DealStatus.Status != DealStatuses.SalesFunnelStatusType.Canceled);
                if (!hasNoClosedLeads)
                {
                    message.Text += relatedAdvertText;
                    OkService.AddMessage(message);

                    CreateLead(CustomerService.GetCustomer(user.CustomerId), message, relatedProductId);
                    return;
                }
                
                var hasNoOpenLeadWithSameOffer =
                                customerLeads.Any(
                                    x =>
                                        x.DealStatus.Status != DealStatuses.SalesFunnelStatusType.FinalSuccess && x.DealStatus.Status != DealStatuses.SalesFunnelStatusType.Canceled &&
                                        x.LeadItems != null &&
                                        x.LeadItems.Count == 1 &&
                                        x.LeadItems.Find(i => i.ProductId == relatedProductId) != null);
                if (!hasNoOpenLeadWithSameOffer && relatedProductId != 0)
                {
                    message.Text += relatedAdvertText;
                    OkService.AddMessage(message);

                    CreateLead(CustomerService.GetCustomer(user.CustomerId), message, relatedProductId);
                    return;
                }

                BizProcessExecuter.MessageReply(CustomerService.GetCustomer(user.CustomerId), EMessageReplyFieldType.Ok);
                if (message.Text.IsNullOrEmpty())
                {
                    return;
                }

                OkService.AddMessage(message);
                AdminInformerService.Add(new AdminInformer(AdminInformerType.Ok, message.Id, user.CustomerId)
                {
                    Type = AdminInformerType.Ok,
                    CustomerId = user.CustomerId,
                    Body = "Новое сообщение в ОК"
                });
                BizProcessExecuter.LeadEvent(message.Id, LeadEventType.Ok);
                return;
            }

            try
            {
                var OkUser = new OkUser(notification.Sender);

                var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    FirstName = OkUser.FirstName,
                    LastName = OkUser.LastName,
                    IsAgreeForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter 
                                                      && SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
                };
                CustomerService.InsertNewCustomer(customer);

                var u = new OkMarketApiService().GetUserInfo(message.FromUser);
                if(u.Photo != null && !u.Photo.Contains(".gif"))
                {
                    OkUser.Photo = u.Photo;
                    SocialNetworkService.SaveAvatar(customer, OkUser.Photo);
                }
                
                OkUser.CustomerId = customer.Id;
                
                OkService.AddUser(OkUser);

                message.Text += relatedAdvertText;
                OkService.AddMessage(message);

                CreateLead(customer, message, relatedProductId);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private static void CreateLead(Customer customer, OkMessage message, int productId = 0)
        {
            var source = OrderSourceService.GetOrderSource(OrderType.Ok);

            var lead = new Lead()
            {
                CustomerId = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                OrderSourceId = source.Id,
                CreatedDate = message.CreatedDate,
                LeadItems = new List<LeadItem>(),
                SalesFunnelId = SettingsCrm.DefaultOkSalesFunnelId,
            };
            if (productId != 0)
            {
                var offer = OfferService.GetProductOffers(productId).FirstOrDefault();
                if (offer != null)
                {
                    lead.LeadItems.Add(new LeadItem(offer, 1));
                    lead.Sum = lead.LeadItems.Sum(x => x.Amount * x.Price);
                }
            }
            LeadService.AddLead(lead, true);
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_SocialNetwork, "OK");
        }

        /// <summary>
        /// Отправить сообщение в ОК
        /// </summary>
        /// <param name="messageId">ID сообщения, на которое требуется ответить</param>
        /// <param name="messageBody">Текст сообщения</param>
        /// <param name="customerId">id (в ОК) получателя сообщения</param>
        public static long SendOKMessage(int messageId, string messageBody, Guid customerId = new Guid(), bool firstTime = true)
        {
            if (string.IsNullOrWhiteSpace(messageBody))
                throw new BlException("Сообщение не может быть пустым");
            try
            {
                var message = OkService.GetMessage(messageId);
                if ((message == null && !customerId.IsDefault()) || !firstTime)
                    message = OkService.GetMessageByCustomerId(customerId);
                if (message == null)
                    throw new Exception("No messages from this user");

                var sendMessageRequest = new OkSendMessageRequest(messageBody, message.ChatId);
                var result = RequestHelper.MakeRequest<OkSendMessageResponse>("https://api.ok.ru/graph/me/messages/chat:" + message.ChatId + "?access_token=" + SettingsOk.GroupSocialAccessToken,
                    sendMessageRequest, method: ERequestMethod.POST);
                if (result.ErrorMsg != null)
                    throw new Exception(result.ErrorMsg);
                OkUser user = null;
                if (!customerId.IsDefault())
                    user = OkService.GetUser(customerId);
                else
                {
                    var mess = OkService.GetMessage(messageId);
                    user = OkService.GetUser(mess.FromUser);
                    if (user == null)
                        user = OkService.GetUser(mess.UserId);
                }
                if (result.ErrorMsg == null)
                {
                    OkService.AddMessage(new OkMessage
                    {
                        MessageId = result.MessageId,
                        ChatId = message.ChatId,
                        UserId = user.Id,
                        FromUser = SettingsOk.GroupId,
                        Text = messageBody ?? "",
                        CreatedDate = DateTime.Now
                    });
                    UpdateChatStatus(message.ChatId, OkChatStatus.mark_seen);
                }
                return result.MessageId.TryParseLong();
            }
            catch (Exception ex)
            {
                if (firstTime)
                    SendOKMessage(messageId, messageBody, customerId, false);

                Debug.Log.Warn(ex);

                if (ex.Message == "No messages from this user")
                    throw new BlException("Не удалось отправить сообщение. (Группы не могут писать первыми, только отвечать на сообщения)");

                if (ex.Message.Contains("closed"))
                    throw new BlException("Не удалось отправить сообщение - пользователь вышел из чата.");

                if (ex.Message.Contains("NOT_FOUND")) 
                    throw new BlException("Не удалось отправить сообщение - чат не найден.");
            }

            return 0;
        }


        /// <summary>
        /// Отправка сообщения в ЛС, если пользлователь разрешил отправку ему сообщений от группы
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="message">текст сообщения</param>
        /// <returns>Id чата в который было отправлено сообщение</returns>
        public static OkSendDirectMessageResponse SendDirectMessage(string userId, string message)
        {
            try
            {
                var data = new OkSendMessageRequest(message, userId: userId);
                var result = RequestHelper.MakeRequest<OkSendDirectMessageResponse>("https://api.ok.ru/graph/me/messages?access_token=" + SettingsOk.GroupSocialAccessToken,
                    method: ERequestMethod.POST, data: data);
                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new OkSendDirectMessageResponse() { ErrorMsg = ex.Message };
            }
        }

        /// <summary>
        /// Обновление текущего статуса чата
        /// </summary>
        /// <param name="chatId">id чата</param>
        /// <param name="status">сатус</param>
        public static OkBaseResponse UpdateChatStatus(string chatId, OkChatStatus status)
        {
            try
            {
                var data = new OkUpdateStatusRequest(chatId, status);
                var result = RequestHelper.MakeRequest<OkBaseResponse>("https://api.ok.ru/graph/me/messages/chat:" + chatId + "?access_token=" + SettingsOk.GroupSocialAccessToken,
                    method: ERequestMethod.POST, data: data);
                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new OkBaseResponse() { ErrorMsg = ex.Message };
            }
        }

        public static bool AddOkUserByLink(Guid customerId, string link)
        {
            var apiServie = new OkMarketApiService();

            var customer = CustomerService.GetCustomer(customerId);
            var linkId = apiServie.GetIdFromUrl(link);
            
            if (linkId == null)
                throw new BlException("Не удалось найти пользователя");

            var user = apiServie.GetUserInfo(linkId);

            var u = OkService.GetUser(linkId);
            if(u != null)
            {
                var c = CustomerService.GetCustomer(u.CustomerId);
                if (c != null)
                    throw new BlException(
                        String.Format(
                            "Аккаунт {0} уже прикреплен к пользователю <a href=\"{1}\" target=\"_blank\">{2} {3}</a>",
                            link, UrlService.GetAdminUrl("customers/view/" + c.Id), c.FirstName, c.LastName));

                u.CustomerId = customer.Id;
                OkService.UpdateUser(u);
            }
            else
            {
                user.CustomerId = customer.Id;

                if (user.Photo != null && user.Photo.Contains(".gif"))
                {
                    user.Photo = null;
                }
                OkService.AddUser(user);
            }

            SocialNetworkService.SaveAvatar(customer, user.Photo);

            return true;
        }
    }
}