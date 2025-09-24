using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.Facebook;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Loging.Smses;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;
using AdvantShop.Web.Infrastructure.Handlers;
using Task = System.Threading.Tasks.Task;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetCrmEvents : AbstractCommandHandler<LeadEventsModel>
    {
        #region Ctor

        private readonly int _objId;
        private readonly string _objType;
        private readonly Guid? _customerId;

        public GetCrmEvents(int? objId, string objType, Guid? customerId)
        {
            _objId = objId ?? 0;
            _objType = objType;
            _customerId = customerId;
        }

        #endregion

        protected override LeadEventsModel Handle()
        {
            var model = new LeadEventsModel();
            var modelEvents = new List<LeadEventModel>();

            var sw = new Stopwatch();
            sw.Start();

            Customer customer = null;
            string customerEmail = null;
            long? standardPhone = null;
            List<LeadEvent> events = null;
            var commentsByObj = new List<AdminCommentModel>();

            if (_objType == "lead")
            {
                if (_objId != 0)
                {
                    var lead = LeadService.GetLead(_objId);

                    customer = lead.Customer;
                    if (customer == null)
                    {
                        customerEmail = lead.Email;
                        standardPhone = StringHelper.ConvertToStandardPhone(lead.Phone, true, true);
                    }
                    events = LeadEventService.GetEvents(lead.Id);
                    commentsByObj = AdminCommentService.GetAdminComments(lead.Id, AdminCommentType.Lead)
                        .Where(x => !x.Deleted)
                        .Select(x => (AdminCommentModel) x)
                        .ToList();

                    model.ShowComments = true;
                }
            }
            else if (_objType == "booking")
            {
                if (_objId != 0)
                {
                    var booking = BookingService.Get(_objId);

                    customer = booking.Customer;
                    if (customer == null)
                    {
                        customerEmail = booking.Email;
                        standardPhone = StringHelper.ConvertToStandardPhone(booking.Phone, true, true);
                    }
                    commentsByObj = AdminCommentService.GetAdminComments(booking.Id, AdminCommentType.Booking)
                        .Where(x => !x.Deleted)
                        .Select(x => (AdminCommentModel)x)
                        .ToList();

                    model.ShowComments = true;
                }
            }
            else if (_objType == "order")
            {
                commentsByObj = AdminCommentService.GetAdminComments(_objId, AdminCommentType.Order)
                        .Where(x => !x.Deleted)
                        .Select(x => (AdminCommentModel)x)
                        .ToList();
                model.ShowComments = true;
            }

            if (customer == null)
                customer = _customerId != null ? CustomerService.GetCustomer(_customerId.Value) : null;

            //if (customer == null)
            //    return null;

            Guid customerId = Guid.Empty;
            List<AdminCommentModel> commentsByUser = null;
            List<Call> calls = null;
            List<VkUserMessage> vkMessages = null;
            List<InstagramUserMessage> instagramMessages = null;
            List<FacebookUserMessage> facebookMessages = null;
            List<TelegramUserMessage> telegramMessages = null;
            List<OkUserMessage> okMessages = null;
            List<EmailLogItem> sendedEmails = null;
            List<EmailImap> imapEmails = null;
            List<SmsLogDto> smses = null;

            if (customer != null)
            {
                model.ShowComments = true;
                model.ShowEmails = true;
                model.ShowSms = true;
                model.ShowVkMessages = true;
                model.ShowFacebookMessages = true;
                model.ShowInstagramMessages = true;
                model.ShowTelegramMessages = true;
                model.ShowOkMessages = true;

                standardPhone = StringHelper.ConvertToStandardPhone(customer.Phone, true, true);
                customerEmail = customer.EMail;
                customerId = customer.Id;

                commentsByUser = AdminCommentService.GetAdminComments(customer.InnerId, AdminCommentType.Customer)
                    .Where(x => !x.Deleted)
                    .Select(x => (AdminCommentModel) x)
                    .ToList();

                var sendedEmailsTask = MeasuringTask.Run(() => CustomerService.GetEmails(customerId, customerEmail));

                var imapEmailsTask = MeasuringTask.Run(() =>
                    !string.IsNullOrWhiteSpace(customerEmail)
                        ? CustomerService.GetEmails(customerEmail)
                        : null);

                var smsesTask = MeasuringTask.Run(() =>
                    standardPhone != null && standardPhone != 0
                        ? CustomerService.GetSms(customerId, standardPhone.Value)
                        : null);

                var vkMessagesTask = MeasuringTask.Run(() => VkService.GetCustomerMessages(customerId));
                var instagramMessagesTask = MeasuringTask.Run(() => InstagramService.GetCustomerMessages(customerId));
                var facebookMessagesTask = MeasuringTask.Run(() => FacebookService.GetCustomerMessages(customerId));
                var telegramMessagesTask = MeasuringTask.Run(() => new TelegramService().GetCustomerMessages(customerId));
                var okMessagesTask = MeasuringTask.Run(() => OkService.GetCustomerMessages(customerId));

                Task.WaitAll(vkMessagesTask, instagramMessagesTask, facebookMessagesTask, okMessagesTask);

                vkMessages = vkMessagesTask.Result != null ? vkMessagesTask.Result.Result : null;
                instagramMessages = instagramMessagesTask.Result != null ? instagramMessagesTask.Result.Result : null;
                facebookMessages = facebookMessagesTask.Result != null ? facebookMessagesTask.Result.Result : null;
                telegramMessages = telegramMessagesTask.Result != null ? telegramMessagesTask.Result.Result : null;
                okMessages = okMessagesTask.Result != null ? okMessagesTask.Result.Result : null;

                model.ElapsedTask.Add("vkMessages", vkMessagesTask.Result != null ? vkMessagesTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);
                model.ElapsedTask.Add("instagramMessages", instagramMessagesTask.Result != null ? instagramMessagesTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);
                model.ElapsedTask.Add("facebookMessages", facebookMessagesTask.Result != null ? facebookMessagesTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);
                model.ElapsedTask.Add("telegramMessages", telegramMessagesTask.Result != null ? telegramMessagesTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);
                model.ElapsedTask.Add("okMessages", okMessagesTask.Result != null ? okMessagesTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);

                Task.WaitAll(new Task[] { sendedEmailsTask, imapEmailsTask, smsesTask}, 10000);

                sendedEmails = sendedEmailsTask.Result != null ? sendedEmailsTask.Result.Result : null;
                imapEmails = imapEmailsTask.Result != null ? imapEmailsTask.Result.Result : null;
                smses = smsesTask.Result != null ? smsesTask.Result.Result : null;

                model.ElapsedTask.Add("sendedEmails", sendedEmailsTask.Result != null ? sendedEmailsTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);
                model.ElapsedTask.Add("imapEmails", imapEmailsTask.Result != null ? imapEmailsTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);
                model.ElapsedTask.Add("smses", smsesTask.Result != null ? smsesTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);

            }
            else
            {
                commentsByUser = new List<AdminCommentModel>();

                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    model.ShowEmails = true;

                    var imapEmailsTask = MeasuringTask.Run(() =>
                            CustomerService.GetEmails(customerEmail));

                    imapEmailsTask.Wait(TimeSpan.FromMilliseconds(10000));
                    imapEmails = imapEmailsTask.Result != null ? imapEmailsTask.Result.Result : null;
                    model.ElapsedTask.Add("imapEmails", imapEmailsTask.Result != null ? imapEmailsTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);
                }
            }
            model.ShowCalls = customer != null || standardPhone.HasValue;

            calls = standardPhone != null && standardPhone != 0
                ? CallService.GetCallsByNum(standardPhone.Value)
                : null;

            var historyTask = MeasuringTask.Run(() =>
            {
                if (_objType == "lead")
                    return ChangeHistoryService.Get(_objId, ChangeHistoryObjType.Lead);
                if (_objType == "booking")
                    return ChangeHistoryService.Get(_objId, ChangeHistoryObjType.Booking);
                return null;
            });

            historyTask.Wait();

            var history = historyTask.Result != null ? historyTask.Result.Result : null;
            model.ElapsedTask.Add("history", historyTask.Result != null ? historyTask.Result.StopwatchTotalMilliseconds.ToString("F2") : null);


            if (events != null && events.Count > 0)
            {
                model.EventsCount = events.Count;
                model.CommentsCount += model.EventsCount; // events.Count(x => x.Type == LeadEventType.Comment);
                modelEvents.AddRange(events.Select(x => new LeadEventModel(x)));
            }

            var comments = commentsByUser.Concat(commentsByObj).ToList();
            if (comments.Count > 0)
            {
                foreach (var comment in comments.Where(x => x.ParentId.HasValue))
                {
                    var parent = comments.FirstOrDefault(x => x.Id == comment.ParentId.Value);
                    if (parent != null)
                        comment.ParentComment = parent;
                }
                model.CommentsCount += comments.Count;
                modelEvents.AddRange(comments.Select(x => new LeadEventModel(x)));
            }

            if (sendedEmails != null && sendedEmails.Count > 0)
            {
                model.SendedEmailsCount = sendedEmails.Count;
                modelEvents.AddRange(sendedEmails.Select(x => new LeadEventModel(x, customerId, customerEmail)));
            }

            if (imapEmails != null && imapEmails.Count > 0)
            {
                model.ReceivedEmailsCount = imapEmails.Count(x => x.FromEmail == customerEmail);
                model.SendedEmailsCount += imapEmails.Count(x => x.FromEmail != customerEmail);

                modelEvents.AddRange(imapEmails.Select(x => new LeadEventModel(x)));
            }

            if (calls != null && calls.Count > 0)
            {
                model.InCallsCount = calls.Count(x => x.Type == ECallType.In);
                model.OutCallsCount = calls.Count(x => x.Type == ECallType.Out);
                model.OtherCallsCount = calls.Count - model.InCallsCount - model.OutCallsCount;
                modelEvents.AddRange(calls.Select(x => new LeadEventModel(x)));
            }

            if (smses != null && smses.Count > 0)
            {
                model.SmsCount = smses.Count;
                modelEvents.AddRange(smses.Select(x => new LeadEventModel(x)));
            }

            if (vkMessages != null && vkMessages.Count > 0)
            {
                model.VkMessagesCount = vkMessages.Count;
                model.VkMessagesInCount = vkMessages.Count(x => x.Type == VkMessageType.Received);
                model.VkMessagesOutCount = vkMessages.Count(x => x.Type == VkMessageType.Sended);

                modelEvents.AddRange(vkMessages.Select(x => new LeadEventModel(x)));
            }

            if (instagramMessages != null && instagramMessages.Count > 0)
            {
                model.InstagramReceivedMessagesCount = instagramMessages.Count(x => x.CustomerId == customerId);
                model.InstagramSendedMessagesCount = instagramMessages.Count - model.InstagramReceivedMessagesCount;

                modelEvents.AddRange(instagramMessages.Select(x => new LeadEventModel(x)));
            }

            if (facebookMessages != null && facebookMessages.Count > 0)
            {
                model.FacebookReceivedMessagesCount = facebookMessages.Count(x => x.CustomerId == customerId);
                model.FacebookSendedMessagesCount = facebookMessages.Count - model.FacebookReceivedMessagesCount;

                modelEvents.AddRange(facebookMessages.Select(x => new LeadEventModel(x)));
            }

            if (telegramMessages != null && telegramMessages.Count > 0)
            {
                model.TelegramReceivedMessagesCount = telegramMessages.Count(x => x.CustomerId == customerId);
                model.TelegramSendedMessagesCount = telegramMessages.Count - model.TelegramReceivedMessagesCount;

                modelEvents.AddRange(telegramMessages.Select(x => new LeadEventModel(x)));
            }

            if (okMessages != null && okMessages.Count > 0)
            {
                model.OkReceivedMessagesCount = okMessages.Count(x => x.CustomerId == customerId);
                model.OkSendedMessagesCount = okMessages.Count - model.OkReceivedMessagesCount;
                modelEvents.AddRange(okMessages.Select(x => new LeadEventModel(x)));
            }


            model.ShowHistory = history != null;
            if (history != null && history.Count > 0)
            {
                model.HistoryCount += history.Count;
                modelEvents.AddRange(history.Select(x => new LeadEventModel(x)));
            }

            foreach (var leadEvent in modelEvents.OrderByDescending(x => x.CreatedDate).Take(1000))
            {
                var group = model.EventGroups.Find(x => x.CreatedDate.Year == leadEvent.CreatedDate.Year && 
                                                        x.CreatedDate.Month == leadEvent.CreatedDate.Month && 
                                                        x.CreatedDate.Day == leadEvent.CreatedDate.Day);
                if (group == null)
                {
                    var date = new DateTime(leadEvent.CreatedDate.Year, leadEvent.CreatedDate.Month, leadEvent.CreatedDate.Day);
                    var newGroup = new LeadEventGroupModel() {Title = GetTitleByDate(leadEvent.CreatedDate), CreatedDate = date};
                    newGroup.Events.Add(leadEvent);
                    model.EventGroups.Add(newGroup);
                }
                else
                {
                    group.Events.Add(leadEvent);
                }
            }

            model.EventTypes = new List<SelectItemModel>();

            foreach (LeadEventType eventType in Enum.GetValues(typeof(LeadEventType)))
            {
                if (eventType == LeadEventType.None)
                    continue;

                model.EventTypes.Add(new SelectItemModel(eventType.Localize(), eventType.ToString()));
            }

            sw.Stop();
            model.Elapsed = sw.Elapsed.TotalMilliseconds.ToString("F2");

            return model;
        }

        private string GetTitleByDate(DateTime date)
        {
            var now = DateTime.Now;

            if (date.Day == now.Day && date.Month == now.Month && date.Year == now.Year)
                return T("Admin.Today");

            if (date.Day == now.Day - 1 && date.Month == now.Month && date.Year == now.Year)
                return T("Admin.Yesteday");

            if (date.Year == now.Year)
                return date.ToString("dd MMMM");

            return date.ToString("D");
        }

        private class MeasuringTask<TResult>
        {
            public TResult Result { get; set; }
            public double StopwatchTotalMilliseconds { get; set; }
        }

        private class MeasuringTask
        {
            public static System.Threading.Tasks.Task<MeasuringTask<TResult>> Run<TResult>(Func<TResult> function)
            {
                return Task.Run(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var result = function();
                    sw.Stop();

                    return new MeasuringTask<TResult>()
                    {
                        Result = result,
                        StopwatchTotalMilliseconds = sw.Elapsed.TotalMilliseconds
                    };
                });
            }
        }
    }
}
