using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Booking.Sms;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Mails;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Booking
{
    public class BookingService
    {
        #region Get Add Update Delete

        public static Booking Get(int id)
        {
            return SQLDataAccess.Query<Booking>("SELECT * FROM [Booking].[Booking] WHERE Id = @Id", new {Id = id})
                .FirstOrDefault();
        }

        public static int GetIdByOrder(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT Id FROM [Booking].[Booking] WHERE OrderId = @OrderId",
                CommandType.Text,
                new SqlParameter("@OrderId", orderId));
        }

        public static List<Booking> GetAllForCancellation(DateTime now)
        {
            return
                SQLDataAccess.Query<Booking>(
                    "SELECT [Booking].* FROM [Booking].[Booking]" +
                    " INNER JOIN [Booking].[Affiliate] ON [Booking].[AffiliateId] = [Affiliate].[Id]" +
                    " WHERE [Affiliate].[CancelUnpaidViaMinutes] IS NOT NULL" +
                    " AND [Booking].[Status] != @CancelStatus AND [Booking].[PaymentDate] IS NULL AND [Booking].[DateAdded] >= @Yesterday AND [Booking].[Sum] > 0 " +
                    " AND @Now >= DATEADD(minute, [Affiliate].[CancelUnpaidViaMinutes], [Booking].[DateAdded])",
                    new { Yesterday = now.Date.AddDays(-1), Now = now, CancelStatus = (int)BookingStatus.Cancel})
                    .ToList();
        }

        public static List<Booking> GetList(int affiliateId, bool onlyActive = true)
        {
            return
                SQLDataAccess.Query<Booking>(string.Format("SELECT * FROM [Booking].[Booking] WHERE [AffiliateId] = @AffiliateId{0}", onlyActive ? " AND [Status] != @CancelStatus" : ""),
                    new {AffiliateId = affiliateId, CancelStatus = (int)BookingStatus.Cancel})
                    .ToList();
        }

        public static List<Booking> GetListByDate(int affiliateId, DateTime date, bool onlyActive = true)
        {
            return
                SQLDataAccess.Query<Booking>(
                    string.Format("SELECT * FROM [Booking].[Booking] WHERE [AffiliateId] = @AffiliateId AND @StartDate < [EndDate] AND [BeginDate] < @EndDate{0}", onlyActive ? " AND [Status] != @CancelStatus" : ""),
                    new {AffiliateId = affiliateId, StartDate = date.Date, EndDate = date.Date.AddDays(1), CancelStatus = (int)BookingStatus.Cancel })
                    .ToList();
        }

        public static List<Booking> GetListByDateAndReservationResource(int affiliateId, DateTime date, int? reservationResourceId, bool onlyActive = true)
        {
            return GetListByDateFromToAndReservationResource(affiliateId, date.Date, date.Date.AddDays(1), reservationResourceId, onlyActive);
        }

        public static List<Booking> GetListByDateFromToAndReservationResource(int affiliateId, DateTime dateFrom, DateTime dateTo, int? reservationResourceId, bool onlyActive = true)
        {
            return
                SQLDataAccess.Query<Booking>(
                    string.Format("SELECT * FROM [Booking].[Booking] WHERE [AffiliateId] = @AffiliateId AND @StartDate < [EndDate] AND [BeginDate] < @EndDate{0}{1}",
                    reservationResourceId.HasValue ? " AND [ReservationResourceId] = @ReservationResourceId" : " AND [ReservationResourceId] IS NULL",
                    onlyActive ? " AND [Status] != @CancelStatus" : ""),
                    new
                    {
                        AffiliateId = affiliateId,
                        StartDate = dateFrom,
                        EndDate = dateTo,
                        ReservationResourceId = reservationResourceId ?? (object)DBNull.Value,
                        CancelStatus = (int)BookingStatus.Cancel
                    })
                    .ToList();
        }

        //todo: нужно протестировать возможно надо поменять на условие @MinDate < [EndDate] AND [BeginDate] < @MaxDate
        public static List<Booking> GetListByDateFromTo(int? affiliateId, DateTime dateFrom, DateTime dateTo, int? managerId = null, bool onlyActive = true)
        {
            var affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            var manager = managerId.HasValue ? ManagerService.GetManager(managerId.Value) : null;

            if (manager == null || (affiliate != null && AffiliateService.CheckAccess(affiliate, manager, false)))
                return
                    SQLDataAccess.Query<Booking>(
                    string.Format("SELECT * FROM [Booking].[Booking] WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate {0}{1}",
                    (affiliateId != null ? " AND [AffiliateId] = @AffiliateId " : ""),
                    onlyActive ? " AND [Status] != @CancelStatus" : ""),
                    new
                    {
                        AffiliateId = affiliateId ?? 0,
                        MinDate = dateFrom,
                        MaxDate = dateTo,
                        CancelStatus = (int)BookingStatus.Cancel
                    })
                    .ToList();

            return
                SQLDataAccess.Query<Booking>(
                    string.Format(
                        "SELECT Booking.* " +
                        "FROM [Booking].[Booking] " +
                        "INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                        "INNER JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id]" +
                        "WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate {0}{1}" +
                        " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) ",
                        (affiliateId != null ? " AND [AffiliateId] = @AffiliateId " : ""),
                        onlyActive ? " AND [Status] != @CancelStatus " : ""),
                    new
                    {
                        AffiliateId = affiliateId ?? 0,
                        MinDate = dateFrom,
                        MaxDate = dateTo,
                        CancelStatus = (int) BookingStatus.Cancel,
                        ManagerId = managerId.Value
                    })
                    .ToList();
        }

        public static List<int> GetListIds(int affiliateId, bool onlyActive = true)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    string.Format("SELECT [Id] FROM [Booking].[Booking] WHERE [AffiliateId] = @AffiliateId{0}", onlyActive ? " AND [Status] != @CancelStatus" : string.Empty),
                    CommandType.Text, "Id",
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel));
        }

        public static int Add(Booking booking, bool updateCustomer = true, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (updateCustomer)
                AddUpdateCustomer(booking, changedBy, trackChanges);

            booking.Id = SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[Booking] " +
                                                    " ([AffiliateId], [ReservationResourceId], [ManagerId], [CustomerId], [FirstName]," +
                                                    "    [LastName], [Email], [Phone], [Patronymic], [StandardPhone], [BeginDate]," +
                                                    "    [EndDate], [Status], [Sum], [OrderSourceId], [DateAdded], [IsSendedSmsBeforeStart]," +
                                                    "    [BookingDiscount], [BookingDiscountValue], [DiscountCost], [PaymentDate], [PaymentCost]," +
                                                    "    [PaymentMethodID], [ArchivedPaymentName], [OrderId], [AdminComment]) " +
                                                    " VALUES (@AffiliateId, @ReservationResourceId, @ManagerId, @CustomerId, @FirstName," +
                                                    "    @LastName, @Email, @Phone, @Patronymic, @StandardPhone, @BeginDate," +
                                                    "    @EndDate, @Status, @Sum, @OrderSourceId, @DateAdded, 0," +
                                                    "    @BookingDiscount, @BookingDiscountValue, @DiscountCost, @PaymentDate, @PaymentCost," +
                                                    "    @PaymentMethodID, @ArchivedPaymentName, @OrderId, @AdminComment); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@AffiliateId", booking.AffiliateId),
                new SqlParameter("@ReservationResourceId", booking.ReservationResourceId ?? (object) DBNull.Value),
                new SqlParameter("@ManagerId", booking.ManagerId ?? (object) DBNull.Value),
                new SqlParameter("@CustomerId", booking.CustomerId ?? (object) DBNull.Value),
                new SqlParameter("@FirstName", booking.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", booking.LastName ?? (object)DBNull.Value),
                new SqlParameter("@Email", booking.Email ?? (object)DBNull.Value),
                new SqlParameter("@Phone", booking.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Patronymic", booking.Patronymic ?? (object)DBNull.Value),
                new SqlParameter("@StandardPhone", booking.StandardPhone ?? (object)DBNull.Value),
                new SqlParameter("@BeginDate", booking.BeginDate),
                new SqlParameter("@EndDate", booking.EndDate),
                new SqlParameter("@Status", (int)booking.Status),
                new SqlParameter("@Sum", booking.Sum),
                new SqlParameter("@OrderSourceId", booking.OrderSourceId),
                new SqlParameter("@DateAdded", DateTime.Now),
                new SqlParameter("@BookingDiscount", booking.BookingDiscount),
                new SqlParameter("@BookingDiscountValue", booking.BookingDiscountValue),
                new SqlParameter("@DiscountCost", booking.DiscountCost),
                new SqlParameter("@PaymentDate", booking.PaymentDate ?? (object)DBNull.Value),
                new SqlParameter("@PaymentCost", booking.PaymentCost),
                new SqlParameter("@PaymentMethodID", booking.PaymentMethodId ?? (object)DBNull.Value),
                new SqlParameter("@ArchivedPaymentName", booking.PaymentMethodName ?? (object)DBNull.Value),
                new SqlParameter("@OrderId", booking.OrderId ?? (object)DBNull.Value),
                new SqlParameter("@AdminComment", booking.AdminComment ?? (object)DBNull.Value)
                );

            if (trackChanges)
                BookingHistoryService.NewBooking(booking, changedBy);

            if (booking.BookingCurrency != null)
            {
                booking.BookingCurrency.BookingId = booking.Id;
                BookingCurrencyService.Add(booking.BookingCurrency);
            }

            if (booking.PaymentDetails != null)
                BookingPaymentDetailsService.AddPaymentDetails(booking.Id, booking.PaymentDetails);

            if (booking.BookingItems == null)
                booking.BookingItems = new List<BookingItem>();

            foreach (var x in booking.BookingItems)
            {
                x.BookingId = booking.Id;

                BookingItemsService.Add(x, changedBy, trackChanges);
            }

            RefreshTotal(booking, changedBy, trackChanges);

            Crm.LeadService.CompleteLeadsOnNewBooking(booking);

            if (SettingsMail.EmailForBookings.IsNotEmpty() ||
                (booking.Customer != null && booking.Customer.EMail.IsNotEmpty()) ||
                (booking.Manager != null && booking.Manager.Email.IsNotEmpty()) ||
                (booking.ReservationResource != null && booking.ReservationResource.Manager != null))
            {
                var mail = new BookingCreatedMailTemplate(booking);

                if (booking.Customer != null && booking.Customer.EMail.IsNotEmpty())
                    MailService.SendMailNow(booking.Customer.Id, booking.Customer.EMail, mail);

                if (!booking.IsFromAdminArea && SettingsMail.EmailForBookings.IsNotEmpty())
                    MailService.SendMailNow(SettingsMail.EmailForBookings, mail);

                if (booking.Manager != null && booking.Manager.Email.IsNotEmpty() && 
                    booking.Manager.Enabled && booking.Manager.HasRoleAction(RoleAction.Booking))
                {
                    bool currentCustomerIsBookingManager =
                        CustomerContext.CurrentCustomer != null &&
                        CustomerContext.CurrentCustomer.IsManager &&
                        booking.Manager.CustomerId == CustomerContext.CurrentCustomer.Id;

                    if (!currentCustomerIsBookingManager)
                        MailService.SendMailNow(Guid.Empty, booking.Manager.Email, mail.Subject, mail.Body, true, logging: false);
                }

                if (booking.ReservationResource != null &&
                    booking.ReservationResource.ManagerId != booking.ManagerId &&
                    booking.ReservationResource.Manager != null && 
                    booking.ReservationResource.Manager.Email.IsNotEmpty() &&
                    booking.ReservationResource.Manager.Enabled &&
                    booking.ReservationResource.Manager.HasRoleAction(RoleAction.Booking))
                {
                    bool currentCustomerIsReservationResourceManager =
                        CustomerContext.CurrentCustomer != null &&
                        CustomerContext.CurrentCustomer.IsManager &&
                        booking.ReservationResource.Manager.CustomerId == CustomerContext.CurrentCustomer.Id;

                    if (!currentCustomerIsReservationResourceManager)
                        MailService.SendMailNow(Guid.Empty, booking.ReservationResource.Manager.Email, mail.Subject, mail.Body, true, logging: false);
                }
            }


            if (booking.StandardPhone.HasValue && booking.Affiliate.IsActiveSmsNotification)
            {
                var smsTemplate = SmsTemplateService.Get(booking.AffiliateId, booking.Status).FirstOrDefault(x => x.Enabled);
                if (smsTemplate != null)
                {
                    var smsBody = SmsTemplateService.BuildTemplate(smsTemplate.Text, booking);

                    SmsNotifier.SendSms(booking.StandardPhone.Value, smsBody, booking.CustomerId, isInternal: true);
                }
            }

            return booking.Id;
        }

        public static void Update(Booking booking, bool updateCustomer = true, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var beforeBooking = Get(booking.Id);

            if (trackChanges)
                BookingHistoryService.TrackBookingChanges(booking, beforeBooking, changedBy);

            if (updateCustomer)
                AddUpdateCustomer(booking, changedBy, trackChanges);

            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[Booking] SET [ReservationResourceId] = @ReservationResourceId, [ManagerId] = @ManagerId, " +
                "   [CustomerId] = @CustomerId, [BeginDate] = @BeginDate, [EndDate] = @EndDate, [Status] = @Status, [Sum] = @Sum, " +
                "   [FirstName] = @FirstName, [LastName] = @LastName, [Email] = @Email, [Phone] = @Phone, [Patronymic] = @Patronymic, " +
                "   [StandardPhone] = @StandardPhone, [OrderSourceId] = @OrderSourceId, [BookingDiscount] = @BookingDiscount, " +
                "   [BookingDiscountValue] = @BookingDiscountValue, [DiscountCost] = @DiscountCost, [PaymentDate] = @PaymentDate, " +
                "   [PaymentCost] = @PaymentCost, [PaymentMethodID] = @PaymentMethodID, [ArchivedPaymentName] = @ArchivedPaymentName, " +
                "   [OrderId] = @OrderId, [AdminComment] = @AdminComment" +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", booking.Id),
                new SqlParameter("@ReservationResourceId", booking.ReservationResourceId ?? (object)DBNull.Value),
                new SqlParameter("@ManagerId", booking.ManagerId ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", booking.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@FirstName", booking.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", booking.LastName ?? (object)DBNull.Value),
                new SqlParameter("@Email", booking.Email ?? (object)DBNull.Value),
                new SqlParameter("@Phone", booking.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Patronymic", booking.Patronymic ?? (object)DBNull.Value),
                new SqlParameter("@StandardPhone", booking.StandardPhone ?? (object)DBNull.Value),
                new SqlParameter("@BeginDate", booking.BeginDate),
                new SqlParameter("@EndDate", booking.EndDate),
                new SqlParameter("@Status", (int)booking.Status),
                new SqlParameter("@Sum", booking.Sum),
                new SqlParameter("@OrderSourceId", booking.OrderSourceId),
                new SqlParameter("@BookingDiscount", booking.BookingDiscount),
                new SqlParameter("@BookingDiscountValue", booking.BookingDiscountValue),
                new SqlParameter("@DiscountCost", booking.DiscountCost),
                new SqlParameter("@PaymentDate", booking.PaymentDate ?? (object)DBNull.Value),
                new SqlParameter("@PaymentCost", booking.PaymentCost),
                new SqlParameter("@PaymentMethodID", booking.PaymentMethodId ?? (object)DBNull.Value),
                new SqlParameter("@ArchivedPaymentName", booking.PaymentMethodName ?? (object)DBNull.Value),
                new SqlParameter("@OrderId", booking.OrderId ?? (object)DBNull.Value),
                new SqlParameter("@AdminComment", booking.AdminComment ?? (object)DBNull.Value)
                );

            if (booking.PaymentDetails != null)
                BookingPaymentDetailsService.UpdatePaymentDetails(booking.Id, booking.PaymentDetails, changedBy, trackChanges);

            if (booking.BookingItems != null)
            {
                var bookingItems = BookingItemsService.GetList(booking.Id);

                // delete
                bookingItems.Where(current => booking.BookingItems.All(newItem => newItem.Id != current.Id))
                    .ForEach(x => BookingItemsService.Delete(x.Id, changedBy, trackChanges));

                // add\update
                foreach (var newItem in booking.BookingItems)
                {
                    if (bookingItems.All(current => newItem.Id != current.Id))
                        BookingItemsService.Add(newItem, changedBy, trackChanges);
                    else
                        BookingItemsService.Update(newItem, changedBy, trackChanges);
                }
            }
            else
            {
                BookingItemsService.ClearByBooking(booking.Id, changedBy, trackChanges);
            }

            booking.BookingItems = BookingItemsService.GetList(booking.Id);

            RefreshTotal(booking, changedBy, trackChanges);


            if (beforeBooking.Status != booking.Status)
            {
                OnChangedStatus(booking);
            }
        }

        public static void ChangeStatus(int id, BookingStatus status, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var beforeBooking = Get(id);

            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[Booking] SET [Status] = @Status " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@Status", (int)status)
                );

            Booking booking = null;
            if (trackChanges)
            {
                booking = Get(id);
                BookingHistoryService.TrackBookingChanges(booking, beforeBooking, changedBy);
            }

            if (beforeBooking != null && beforeBooking.Status != status)
            {
                if (booking == null)
                    booking = Get(id);

                OnChangedStatus(booking);
            }
        }

        public static void OnChangedStatus(Booking booking)
        {
            if (booking.StandardPhone.HasValue && booking.Affiliate.IsActiveSmsNotification)
            {
                var smsTemplate = SmsTemplateService.Get(booking.AffiliateId, booking.Status).FirstOrDefault(x => x.Enabled);
                if (smsTemplate != null)
                {
                    var smsBody = SmsTemplateService.BuildTemplate(smsTemplate.Text, booking);
                    SmsNotifier.SendSms(booking.StandardPhone.Value, smsBody, booking.CustomerId, isInternal: true);
                }
            }

            if (booking.Status == BookingStatus.Cancel)
            {
                if (booking.OrderId.HasValue)
                {
                    AdvantShop.Orders.OrderStatusService.ChangeOrderStatus(booking.OrderId.Value, AdvantShop.Orders.OrderStatusService.CanceledOrderStatus, LocalizationService.GetResource("Core.Booking.BookingCanceled"));

                    var order = AdvantShop.Orders.OrderService.GetOrder(booking.OrderId.Value);

                    var mail = new OrderStatusMailTemplate(order);

                    MailService.SendMailNow(order.OrderCustomer.CustomerID, order.OrderCustomer.Email, mail);
                    MailService.SendMailNow(SettingsMail.EmailForOrders, mail);
                }
            }
        }

        public static void PayBooking(int id, bool pay, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var beforeBooking = Get(id);
            if (beforeBooking == null)
                throw new Exception("Can't pay empty booking");

            if (pay == beforeBooking.Payed)
                return;

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Booking].[Booking] SET [PaymentDate] = @PaymentDate WHERE [Id] = @Id", CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@PaymentDate", pay ? DateTime.Now : (object)DBNull.Value));

            if (trackChanges)
            {
                var booking = Get(id);
                BookingHistoryService.TrackBookingChanges(booking, beforeBooking, changedBy);
            }

            //var mail = new PayOrderTemplate(order, pay);

            //MailService.SendMailNow(SettingsMail.EmailForBookings, mail);
        }

        public static void OnOrderPaid(AdvantShop.Orders.Order order, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var bookingId = GetIdByOrder(order.OrderID);
            if (bookingId > 0)
            {
                changedBy = changedBy ?? new ChangedBy(CustomerContext.CurrentCustomer);
                changedBy.Name += " (FromOrder)";
                PayBooking(bookingId, true, changedBy, trackChanges);

                var booking = Get(bookingId);
                booking.PaymentMethodId = order.PaymentMethodId != 0 ? order.PaymentMethodId : default(int?);
                booking.ArchivedPaymentName = order.ArchivedPaymentName;
                booking.PaymentCost = order.PaymentCost;//под сомнением, что надо обновлять
                booking.PaymentDetails = order.PaymentDetails;

                Update(booking, false, changedBy, trackChanges);

                Crm.BusinessProcesses.BizProcessExecuter.BookingChanged(booking);//под сомнением
            }
        }

        public static void Delete(int id, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var booking = Get(id);
            if (booking == null)
                return;

            Delete(booking, changedBy, trackChanges);
        }

        public static void Delete(Booking booking, ChangedBy changedBy = null, bool trackChanges = true)
        {
            if (!CheckAccessToEditing(booking))
                return;

            BeforeDelete(booking.Id);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[Booking] WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", booking.Id));

            if (trackChanges)
                BookingHistoryService.DeleteBooking(booking, changedBy);
        }

        private static void BeforeDelete(int id)
        {
            //todo Удалить связанные ресурсы
            AttachmentService.DeleteAttachments<BookingAttachment>(id);
            AdminCommentService.DeleteAdminComments(id, AdminCommentType.Booking);
        }

        #endregion

        #region Help

        private static void AddUpdateCustomer(Booking booking, ChangedBy changedBy = null, bool trackChanges = true)
        {
            Customer findedCustomer = null;

            var customerAdded = booking.Customer != null && booking.Customer.RegistredUser;
            if (!customerAdded && booking.Customer != null)
            {
                booking.CustomerId = CustomerService.InsertNewCustomer(booking.Customer); // если пользователь с таким email существует, то Guid.Empty
                customerAdded = booking.CustomerId != Guid.Empty;
            }

            if (!customerAdded && booking.Customer != null)
            {
                if (booking.CustomerId != null && booking.CustomerId != Guid.Empty)
                    findedCustomer = CustomerService.GetCustomerFromDb(booking.CustomerId.Value);

                if (findedCustomer == null && !string.IsNullOrEmpty(booking.Email) && !string.IsNullOrEmpty(booking.Phone))
                    findedCustomer = CustomerService.GetCustomerByEmailAndPhone(booking.Email, booking.Phone, booking.StandardPhone);

                if (findedCustomer == null && !string.IsNullOrEmpty(booking.Email))
                    findedCustomer = CustomerService.GetCustomerByEmail(booking.Email);

                if (findedCustomer == null && !string.IsNullOrEmpty(booking.Phone))
                    findedCustomer = CustomerService.GetCustomerByPhone(booking.Phone, booking.StandardPhone);


                if (findedCustomer != null)
                {
                    booking.CustomerId = findedCustomer.Id;
                    booking.Customer.Id = findedCustomer.Id;
                    var updateCustomer = false;

                    if (!string.IsNullOrWhiteSpace(booking.Customer.FirstName) && booking.Customer.FirstName != findedCustomer.FirstName)
                    {
                        findedCustomer.FirstName = booking.Customer.FirstName;
                        updateCustomer = true;
                    }

                    if (!string.IsNullOrWhiteSpace(booking.Customer.LastName) && booking.Customer.LastName != findedCustomer.LastName)
                    {
                        findedCustomer.LastName = booking.Customer.LastName;
                        updateCustomer = true;
                    }

                    if (!string.IsNullOrWhiteSpace(booking.Customer.Patronymic) && booking.Customer.Patronymic != findedCustomer.Patronymic)
                    {
                        findedCustomer.Patronymic = booking.Customer.Patronymic;
                        updateCustomer = true;
                    }

                    if (!string.IsNullOrWhiteSpace(booking.Customer.Organization) && booking.Customer.Organization != findedCustomer.Organization)
                    {
                        findedCustomer.Organization = booking.Customer.Organization;
                        updateCustomer = true;
                    }

                    if (!string.IsNullOrWhiteSpace(booking.Customer.Phone) && booking.Customer.Phone != findedCustomer.Phone)
                    {
                        findedCustomer.Phone = booking.Customer.Phone;
                        findedCustomer.StandardPhone = booking.Customer.StandardPhone ?? AdvantShop.Helpers.StringHelper.ConvertToStandardPhone(booking.Customer.Phone);
                        updateCustomer = true;
                    }

                    if (!string.IsNullOrWhiteSpace(booking.Customer.EMail) && string.IsNullOrWhiteSpace(findedCustomer.EMail))
                    {
                        findedCustomer.EMail = booking.Customer.EMail;
                        updateCustomer = true;
                    }

                    if (booking.Customer.BirthDay != null && booking.Customer.BirthDay != findedCustomer.BirthDay)
                    {
                        findedCustomer.BirthDay = booking.Customer.BirthDay;
                        updateCustomer = true;
                    }

                    if (updateCustomer)
                    {
                        if (trackChanges)
                            BookingHistoryService.TrackBookingCustomerChanges(booking.Id, findedCustomer, booking.Customer, changedBy);

                        CustomerService.UpdateCustomer(findedCustomer);
                    }
                }
            }

            if (customerAdded)
            {
                if (booking.Customer.RegistredUser) // добавлен не сейчас (кодом выше)
                {
                    if (trackChanges)
                        BookingHistoryService.TrackBookingCustomerChanges(booking.Id, booking.Customer, CustomerService.GetCustomerFromDb(booking.Customer.Id), changedBy);

                    CustomerService.UpdateCustomer(booking.Customer);
                }

                if (booking.Customer.Contacts != null && booking.Customer.Contacts.Count > 0)
                {
                    foreach (var contact in booking.Customer.Contacts)
                    {
                        if (contact.ContactId == Guid.Empty ||
                            CustomerService.GetCustomerContact(contact.ContactId.ToString()) == null)
                        {
                            CustomerService.AddContact(contact, booking.Customer.Id);
                        }
                        else
                        {
                            CustomerService.UpdateContact(contact);
                        }
                    }
                }
            }

            if (findedCustomer != null)
            {
                booking.Customer = CustomerService.GetCustomerFromDb(findedCustomer.Id);
            }
        }

        public static int GetLastBookingCount(int? affiliateId = null, int? managerId = null, int? orderSourceId = null)
        {
            var affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            var startDate = DateTime.Today;

            if (managerId == null || (affiliate != null && (affiliate.AccessForAll || affiliate.ManagerIds.Contains(managerId.Value))))
                return SQLDataAccess.ExecuteScalar<int>(
                    string.Format(
                        "SELECT COUNT(Id) FROM [Booking].[Booking] WHERE [Status] = @NewStatus AND [BeginDate] >= @StartDate{0}{1}",
                        affiliateId.HasValue ? " AND [AffiliateId] = @AffiliateId" : string.Empty,
                        orderSourceId.HasValue ? " AND [OrderSourceId] = @OrderSourceId" : string.Empty),
                    CommandType.Text,
                    new SqlParameter("@NewStatus", (object) ((int) BookingStatus.New)),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@AffiliateId", affiliateId ?? (object)DBNull.Value),
                    new SqlParameter("@OrderSourceId", orderSourceId ?? (object)DBNull.Value));

            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Booking.Id) FROM [Booking].[Booking] " +
                "INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                "INNER JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id]" +
                "WHERE [Status] = @NewStatus AND [BeginDate] >= @StartDate " +
                (affiliateId.HasValue ? " AND [AffiliateId] = @AffiliateId" : string.Empty) +
                (orderSourceId.HasValue ? " AND [OrderSourceId] = @OrderSourceId" : string.Empty) +
                " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId)",
                CommandType.Text,
                new SqlParameter("@NewStatus", (object) ((int) BookingStatus.New)),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@ManagerId", managerId.Value),
                new SqlParameter("@AffiliateId", affiliateId ?? (object) DBNull.Value),
                new SqlParameter("@OrderSourceId", orderSourceId ?? (object)DBNull.Value));

        }

        public static int GetCustomerBookingsCount(Guid customerId, bool onlyActive = true)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                string.Format("SELECT COUNT([Id]) FROM [Booking].[Booking] WHERE [CustomerId] = @CustomerId{0}", onlyActive ? " AND [Status] != @CancelStatus" : ""),
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel));
        }

        public static Dictionary<BookingStatus, int> GetStatusesWithCount(int affiliateId)
        {
            var statusesWithCount = SQLDataAccess.ExecuteReadDictionary<int, int>(
                "SELECT [Status], count([Status]) as count FROM [Booking].[Booking] WHERE [AffiliateId] = @AffiliateId GROUP BY [Status]",
                CommandType.Text,
                "Status",
                "count",
                new SqlParameter("@AffiliateId", affiliateId));

            return Enum.GetValues(typeof (BookingStatus))
                .Cast<BookingStatus>()
                .ToDictionary(
                    status => status,
                    status => statusesWithCount.ContainsKey((int)status) ? statusesWithCount[(int)status] : 0);
        }

        public static Dictionary<BookingStatus, int> GetStatusesWithCountByManager(int affiliateId, int managerId)
        {
            var statusesWithCount = SQLDataAccess.ExecuteReadDictionary<int, int>(
                "SELECT [Booking].[Status], count([Booking].[Status]) as count FROM [Booking].[Booking] " +
                "INNER JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                "WHERE [AffiliateId] = @AffiliateId AND ReservationResource.ManagerId = @ManagerId " +
                "GROUP BY [Booking].[Status]",
                CommandType.Text,
                "Status",
                "count",
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ManagerId", managerId));

            return Enum.GetValues(typeof (BookingStatus))
                .Cast<BookingStatus>()
                .ToDictionary(
                    status => status,
                    status => statusesWithCount.ContainsKey((int)status) ? statusesWithCount[(int)status] : 0);
        }

        public static bool Exist(int affiliateId, int? reservationResourceId, DateTime startDate, DateTime endDate, int? ignoreBookingId = null)
        {
            var paramenters = new List<SqlParameter>
            {
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel)
            };

            if (ignoreBookingId.HasValue)
                paramenters.Add(new SqlParameter("@BookingId", ignoreBookingId.Value));

            if (reservationResourceId.HasValue)
                paramenters.Add(new SqlParameter("@ReservationResourceId", reservationResourceId.Value));

            return SQLDataAccess.ExecuteScalar<bool>(
                string.Format(
                    @"SELECT (CASE WHEN EXISTS(SELECT [Booking].[Id] FROM [Booking].[Booking] WHERE [AffiliateId] = @AffiliateId AND @StartDate < [EndDate] AND [BeginDate] < @EndDate AND [Status] != @CancelStatus{0}{1}) THEN 1 ELSE 0 END)",
                    ignoreBookingId.HasValue ? " AND [Booking].[Id] != @BookingId" : "",
                    reservationResourceId.HasValue ? " AND [Booking].[ReservationResourceId] = @ReservationResourceId" : " AND [Booking].[ReservationResourceId] IS NULL"),
                CommandType.Text,
                paramenters.ToArray());
        }

        public static Booking RefreshTotal(Booking booking, ChangedBy changedBy = null, bool trackChanges = true)
        {

            float totalPrice = 0;
            float totalItemsPrice = 0;
            float totalDiscount = 0;

            var oldRefreshTotalBooking = trackChanges ? new OnRefreshTotalBooking() {Sum = booking.Sum} : null;

            totalItemsPrice =
                booking.BookingItems.Sum(item => item.Price * item.Amount);

            totalDiscount += booking.BookingDiscount > 0 ? (booking.BookingDiscount * totalItemsPrice / 100) : 0;
            totalDiscount += booking.BookingDiscountValue;

            totalDiscount = totalDiscount.RoundPrice(booking.BookingCurrency.CurrencyValue, booking.BookingCurrency);

            totalPrice = (totalItemsPrice - totalDiscount + booking.PaymentCost).RoundPrice(booking.BookingCurrency.CurrencyValue, booking.BookingCurrency);

            if (totalPrice < 0) totalPrice = 0;

            booking.Sum = totalPrice;
            booking.DiscountCost = totalDiscount;

            if (trackChanges)
            {
                var newRefreshTotalOrder = new OnRefreshTotalBooking() {Sum = booking.Sum};

                BookingHistoryService.TrackChangingBookingTotal(booking.Id, newRefreshTotalOrder, oldRefreshTotalBooking, changedBy);
            }

            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[Booking] SET [Sum] = @Sum, [DiscountCost] = @DiscountCost " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", booking.Id),
                new SqlParameter("@Sum", booking.Sum),
                new SqlParameter("@DiscountCost", booking.DiscountCost)
                );

            return booking;
        }

        public static List<Tuple<TimeSpan, TimeSpan>> GetListTimes(int intervalMinutes)
        {
            if (intervalMinutes <= 0)
                return null;

            var listTimes = new List<Tuple<TimeSpan, TimeSpan>>();
            var time = new TimeSpan();
            var timeEnd = new TimeSpan(1, 0, 0, 0);

            while (time < timeEnd)
            {
                listTimes.Add(
                    new Tuple<TimeSpan, TimeSpan>(
                        time,
                        (time = time.Add(new TimeSpan(0, intervalMinutes, 0))))
                    );
            }
            return listTimes;
        }

        public static bool CheckAccess(Booking booking, Manager manager = null)
        {
            // по общей логике доступа к ресурсу и филиалу


            if (booking.ReservationResourceId.HasValue
                && !ReservationResourceService.ExistsRefAffiliate(
                    booking.AffiliateId, booking.ReservationResourceId.Value))
                return false;

            var currentCustomer = CustomerContext.CurrentCustomer;
            var curManager = manager ??
                             (currentCustomer.IsManager
                                 ? ManagerService.GetManager(CustomerContext.CurrentCustomer.Id)
                                 : null);

            if (booking.ManagerId.HasValue)
            {
                if (curManager != null && curManager.ManagerId == booking.ManagerId)
                    return true;
            }


            if (booking.Affiliate.AccessForAll)
                return true;

            if (manager == null)
            {
                if (currentCustomer.IsAdmin || currentCustomer.IsVirtual)
                    return true;

                if (currentCustomer.IsManager)
                {
                    manager = curManager;

                    if (CheckAccessManager(booking, manager))
                        return true;
                }
            }
            else if (CheckAccessManager(booking, manager))
                return true;

            return false;
        }

        private static bool CheckAccessManager(Booking booking, Manager manager)
        {
            // дополненная логика из ReservationResourceService
            if (manager != null && manager.Enabled)
            {
                if ((booking.ReservationResourceId.HasValue && booking.ReservationResource.ManagerId == manager.ManagerId && booking.Affiliate.AccessToViewBookingForResourceManagers)// допиленная часть
                    || manager.Customer.IsAdmin 
                    || booking.Affiliate.ManagerIds.Contains(manager.ManagerId))
                    return true;
            }
            return false;
        }

        public static bool CheckAccessToEditing(Booking booking, Manager manager = null)
        {
            return AffiliateService.CheckAccessToEditing(booking.Affiliate, manager);
        }

        public static string GenerateHtmlBookingItemsTable(IList<BookingItem> bookingItems, BookingCurrency bookingCurrency)
        {
            var htmlOrderTable = new StringBuilder();

            htmlOrderTable.Append("<table class='orders-table' style='border-collapse: collapse; width: 100%;'>");
            htmlOrderTable.Append("<tr class='orders-table-header'>");
            htmlOrderTable.AppendFormat("<th class='photo' style='border-bottom: 1px solid #e3e3e3; margin-right: 15px; padding: 20px 0; padding-left: 20px; text-align: left;'>{0}</th>", LocalizationService.GetResource("Core.Booking.Letter.Service"));
            htmlOrderTable.Append("<th class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left; width: 50%;'></th>");
            htmlOrderTable.AppendFormat("<th class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</th>", LocalizationService.GetResource("Core.Booking.Letter.Price"));
            htmlOrderTable.AppendFormat("<th class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;' >{0}</th>", LocalizationService.GetResource("Core.Booking.Letter.Count"));
            htmlOrderTable.AppendFormat("<th class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}<span class='curency' style='font-weight: normal;'> ({1})</span></th>", LocalizationService.GetResource("Core.Booking.Letter.Cost"), CurrencyService.CurrentCurrency.Symbol);
            htmlOrderTable.Append("</tr>");

            Currency currency = bookingCurrency;

            // Добавление услуг
            foreach (var item in bookingItems)
            {
                htmlOrderTable.Append("<tr>");

                var service = item.ServiceId.HasValue ? ServiceService.Get(item.ServiceId.Value) : null;

                if (service != null)
                {
                    if (service.Image.IsNotEmpty())
                    {
                        htmlOrderTable.AppendFormat("<td class='photo' style='border-bottom: 1px solid #e3e3e3; margin-right: 15px; padding: 20px 0; padding-left: 20px; text-align: left;'><img src='{0}' /></td>",
                                                        FoldersHelper.GetPath(FolderType.BookingService, service.Image, false));
                    }
                    else
                    {
                        htmlOrderTable.AppendFormat("<td>&nbsp;</td>");
                    }
                }

                htmlOrderTable.AppendFormat("<td class='name' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: left; width: 50%;'>" +
                                                    "<div class='description' style='display: inline-block;'>" +
                                                        "({1}) {0}" +
                                                    "</div>" +
                                            "</td>",
                                            item.Name, item.ArtNo);

                htmlOrderTable.AppendFormat("<td class='price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", PriceFormatService.FormatPrice(item.Price, currency));
                htmlOrderTable.AppendFormat("<td class='amount' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", item.Amount);
                htmlOrderTable.AppendFormat("<td class='total-price' style='border-bottom: 1px solid #e3e3e3; padding: 20px 0; text-align: center;'>{0}</td>", PriceFormatService.FormatPrice(item.Price * item.Amount, currency));
                htmlOrderTable.Append("</tr>");
            }

            // Стоимость
            htmlOrderTable.Append("<tr>");
            htmlOrderTable.AppendFormat("<td class='footer-name' colspan='4' style='border-bottom: none; font-weight: bold; padding: 10px 0; text-align: right;'>{0}:</td>", LocalizationService.GetResource("Core.Booking.Letter.BookingCost"));
            htmlOrderTable.AppendFormat("<td class='footer-value' style='border-bottom: none; font-weight: bold; padding: 10px 0; text-align: center;'>{0}</td>", PriceFormatService.FormatPrice(bookingItems.Sum(item => item.Price * item.Amount), currency));
            htmlOrderTable.Append("</tr>");

            htmlOrderTable.Append("</table>");

            return htmlOrderTable.ToString();
        }

        #endregion
    }
}
