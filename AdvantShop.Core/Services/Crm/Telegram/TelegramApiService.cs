using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdvantShop.Core.Services.Crm.Telegram
{
    public class TelegramApiService
    {
        #region Ctor

        private static TgClient _client;
        private readonly TelegramService _telegramService;

        public TelegramApiService()
        {
            _telegramService = new TelegramService();
        }

        #endregion Ctor

        public bool IsActive()
        {
            return !string.IsNullOrEmpty(SettingsTelegram.Token);
        }

        public void DeActivate()
        {
            SettingsTelegram.Token = "";
            SettingsTelegram.BotUser = null;

            _client = null;
        }

        private TgClient GetClient(bool setPath = true)
        {
            if (_client != null)
                return _client;

            _client = new TgClient(SettingsTelegram.Token);

            if (setPath)
                _client.SetWebhookAsync(StringHelper.ToPuny(SettingsMain.SiteUrl.Replace("http://", "https://")).TrimEnd('/') + "/integration/telegram/webhook").Wait();
            
            return _client;
        }

        private bool TryGetClient()
        {
            try
            {
                _ = GetClient(false);
            }
            catch (Exception)
            {
                DeActivate();
                return false;
            }
            return true;
        }

        public void Initialize()
        {
            if (!IsActive())
                return;

            try
            {
                _ = GetClient();
            }
            catch (ApiRequestException ex)
            {
                Debug.Log.Warn(ex);
                DeActivate();
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        public TelegramUser GetMe()
        {
            _ = GetClient();
            var me = _client?.GetMeAsync().Result;
            SettingsTelegram.BotUser = me != null ? new TelegramUser(me) : null;

            return SettingsTelegram.BotUser;
        }

        public bool SaveMessage(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                Debug.Log.Info("telegram msg: " + json);

                var update = JsonConvert.DeserializeObject<Update>(json);
                if (update == null)
                    return false;

                if (!TryGetClient())
                    return false;

                SaveUser(update);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
            return false;
        }

        public bool SendTelegramMessage(int id, string text, Guid? customerId)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            try
            {
                if (!TryGetClient())
                    return false;

                var message = customerId == null
                    ? _telegramService.GetMessage(id)
                    : _telegramService.GetMessageByCustomerId(customerId.Value);

                if (message == null)
                    return false;

                var sendedMessage = _client.SendTextMessageAsync(message.ChatId, text, ParseMode.Html).Result;
                if (sendedMessage != null)
                {
                    _telegramService.AddMessage(new TelegramMessage(sendedMessage) { ToId = message.FromId });
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }

        #region Save user and create lead

        private void SaveUser(Update update)
        {
            if (update == null || update.Message == null)
                return;

            var message = update.Message;
            var photoUrl = string.Empty;
            if (message.Type == MessageType.Photo && message.Photo.Any())
            {
                if (TryGetClient())
                {
                    try
                    {
                        var file = _client.GetFileAsync(message.Photo.FirstOrDefault()?.FileId).Result;
                        if (file != null)
                            photoUrl = $"https://api.telegram.org/file/bot{SettingsTelegram.Token}/{file.FilePath}";
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }

            _telegramService.AddMessage(new TelegramMessage(message, photoUrl));

            var u = _telegramService.GetUser(message.From.Id);
            if (u != null)
            {
                // Если все лиды закрыты, то создаем новый
                var customerLeads = LeadService.GetLeadsByCustomer(u.CustomerId);
                var hasNoClosedLeads = customerLeads.Any(x => x.DealStatusId != SettingsCrm.TelegramFinalDealStatusId);
                if (!hasNoClosedLeads)
                {
                    CreateLead(CustomerService.GetCustomer(u.CustomerId), message);
                    return;
                }

                BizProcessExecuter.MessageReply(CustomerService.GetCustomer(u.CustomerId), EMessageReplyFieldType.Telegram);

                AdminInformerService.Add(new AdminInformer(AdminInformerType.Telegram, message.MessageId, u.CustomerId) { CustomerId = u.CustomerId });
                BizProcessExecuter.LeadEvent(message.MessageId, LeadEventType.Telegram);
                return;
            }

            try
            {
                var user = new TelegramUser(message.From);

                if (TryGetClient())
                {
                    try
                    {
                        var photos = _client.GetUserProfilePhotosAsync(user.Id).Result;
                        if (photos != null && photos.Photos != null && photos.Photos.Length > 0 && photos.Photos[0].Length > 0)
                        {
                            var file = _client.GetFileAsync(photos.Photos[0][0].FileId).Result;
                            if (file != null)
                                user.PhotoUrl = $"https://api.telegram.org/file/bot{SettingsTelegram.Token}/{file.FilePath}";
                        }
                    }
                    catch
                    {
                        //ignore
                    }
                }

                // add customer
                var customer = new Customer(CustomerGroupService.DefaultCustomerGroup)
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsAgreeForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter 
                                                      && SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked
                };
                CustomerService.InsertNewCustomer(customer);

                SocialNetworkService.SaveAvatar(customer, user.PhotoUrl);

                // add vk user
                user.CustomerId = customer.Id;
                _telegramService.AddUser(user);

                // add lead
                CreateLead(customer, message);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        private void CreateLead(Customer customer, Message message)
        {
            var source = OrderSourceService.GetOrderSource(OrderType.Telegram);

            var lead = new Lead()
            {
                CustomerId = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                OrderSourceId = source.Id,
                CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(message.Date.AddSeconds(-1), TimeZoneInfo.Local),
                LeadItems = new List<LeadItem>(),
                SalesFunnelId = SettingsCrm.DefaultTelegramSalesFunnelId
            };
            LeadService.AddLead(lead, true);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_LeadCreated_SocialNetwork, "Telegram");
        }

        #endregion Save user and create lead
    }
}