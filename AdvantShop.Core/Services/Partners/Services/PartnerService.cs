using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Partners
{
    public class PartnerService
    {
        public const string ReferralRequestParam = "r_rid";
        private const string ReferralCookieName = "referralData";
        private const int ReferralCookieExpires = 365;

        #region CRUD Partner

        public static List<Partner> GetPartners(bool onlyEnabled = true)
        {
            return SQLDataAccess.Query<Partner>("SELECT * FROM [Partners].[Partner]" + (onlyEnabled ? " WHERE Enabled = 1" : string.Empty)).ToList();
        }

        public static List<int> GetPartnerIds(bool onlyEnabled = true)
        {
            return SQLDataAccess.Query<int>("SELECT Id FROM [Partners].[Partner]" + (onlyEnabled ? " WHERE Enabled = 1" : string.Empty)).ToList();
        }

        public static Partner GetPartner(int id)
        {
            var partner = SQLDataAccess.Query<Partner>("SELECT * FROM [Partners].[Partner] WHERE Id = @Id", new { Id = id }).FirstOrDefault();
            GetPartnerEntity(partner);

            return partner;
        }

        public static Partner GetPartner(string email)
        {
            var partner = SQLDataAccess.Query<Partner>("SELECT * FROM [Partners].[Partner] WHERE Email = @Email", new { email }).FirstOrDefault();
            GetPartnerEntity(partner);

            return partner;
        }

        public static Partner GetPartner(string email, string password, bool isPasswordHash)
        {
            var partner = SQLDataAccess.Query<Partner>(
                "SELECT * FROM [Partners].[Partner] WHERE Email = @Email AND Password = @Password",
                new
                {
                    email,
                    password = isPasswordHash ? password : SecurityHelper.GetPasswordHash(password)
                }).FirstOrDefault();
            GetPartnerEntity(partner);

            return partner;
        }

        private static void GetPartnerEntity(Partner partner)
        {
            if (partner == null)
                return;

            switch (partner.Type)
            {
                case EPartnerType.NaturalPerson:
                    partner.NaturalPerson = GetNaturalPerson(partner.Id);
                    break;
                case EPartnerType.LegalEntity:
                    partner.LegalEntity = GetLegalEntity(partner.Id);
                    break;
            }
        }

        public static Partner GetPartnerByCoupon(int couponId)
        {
            return SQLDataAccess.Query<Partner>("SELECT * FROM [Partners].[Partner] WHERE CouponId = @CouponId", new { couponId }).FirstOrDefault();
        }

        public static Partner GetPartnerByCoupon(string couponCode)
        {
            return SQLDataAccess.Query<Partner>(
                "SELECT [Partner].* FROM [Partners].[Partner] INNER JOIN [Catalog].[Coupon] ON Coupon.CouponId = [Partner].CouponId WHERE [Code] = @CouponCode", 
                new { couponCode }).FirstOrDefault();
        }

        public static void AddPartner(Partner partner)
        {
            partner.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Partners].[Partner] " +
                "(Email,Password,Name,Phone,City,DateCreated,DateUpdated,SendMessages,AdminComment,Enabled,Balance," +
                "Type,CouponId,RewardPercent,ContractConcluded,ContractNumber,ContractDate,ContractScan) " +
                "VALUES (@Email,@Password,@Name,@Phone,@City,@DateCreated,@DateUpdated,@SendMessages,@AdminComment,@Enabled,@Balance," +
                "@Type,@CouponId,@RewardPercent,@ContractConcluded,@ContractNumber,@ContractDate,@ContractScan);" +
                "SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Email", partner.Email),
                new SqlParameter("@Password", SecurityHelper.GetPasswordHash(partner.Password)),
                new SqlParameter("@Name", partner.Name ?? string.Empty),
                new SqlParameter("@Phone", partner.Phone ?? string.Empty),
                new SqlParameter("@City", partner.City ?? string.Empty),
                new SqlParameter("@DateCreated", DateTime.Now),
                new SqlParameter("@DateUpdated", DateTime.Now),
                new SqlParameter("@SendMessages", partner.SendMessages),
                new SqlParameter("@AdminComment", partner.AdminComment ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", partner.Enabled),
                new SqlParameter("@Balance", partner.Balance),
                new SqlParameter("@Type", partner.Type),
                new SqlParameter("@CouponId", partner.CouponId ?? (object)DBNull.Value),
                new SqlParameter("@RewardPercent", partner.RewardPercent),
                new SqlParameter("@ContractConcluded", partner.ContractConcluded),
                new SqlParameter("@ContractNumber", partner.ContractNumber ?? (object)DBNull.Value),
                new SqlParameter("@ContractDate", partner.ContractDate ?? (object)DBNull.Value),
                new SqlParameter("@ContractScan", partner.ContractScan ?? (object)DBNull.Value)
            );
            switch (partner.Type)
            {
                case EPartnerType.NaturalPerson:
                    if (partner.NaturalPerson != null)
                    {
                        partner.NaturalPerson.PartnerId = partner.Id;
                        AddUpdateNaturalPerson(partner.NaturalPerson);
                    }
                    break;
                case EPartnerType.LegalEntity:
                    if (partner.LegalEntity != null)
                    {
                        partner.LegalEntity.PartnerId = partner.Id;
                        AddUpdateLegalEntity(partner.LegalEntity);
                    }
                    break;
            }
        }

        public static void UpdatePartner(Partner partner)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Partners].[Partner] " +
                "SET Email=@Email, Name=@Name, Phone=@Phone, City=@City, DateUpdated=@DateUpdated, SendMessages=@SendMessages, AdminComment=@AdminComment, " +
                "Enabled=@Enabled, Balance=@Balance, Type=@Type, CouponId=@CouponId, RewardPercent=@RewardPercent, " +
                "ContractConcluded=@ContractConcluded, ContractNumber=@ContractNumber, ContractDate=@ContractDate, ContractScan=@ContractScan " +
                "WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", partner.Id),
                new SqlParameter("@Email", partner.Email),
                new SqlParameter("@Name", partner.Name ?? string.Empty),
                new SqlParameter("@Phone", partner.Phone ?? string.Empty),
                new SqlParameter("@City", partner.City ?? string.Empty),
                new SqlParameter("@DateUpdated", DateTime.Now),
                new SqlParameter("@SendMessages", partner.SendMessages),
                new SqlParameter("@AdminComment", partner.AdminComment ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", partner.Enabled),
                new SqlParameter("@Balance", partner.Balance),
                new SqlParameter("@Type", partner.Type),
                new SqlParameter("@CouponId", partner.CouponId ?? (object)DBNull.Value),
                new SqlParameter("@RewardPercent", partner.RewardPercent),
                new SqlParameter("@ContractConcluded", partner.ContractConcluded),
                new SqlParameter("@ContractNumber", partner.ContractNumber ?? (object)DBNull.Value),
                new SqlParameter("@ContractDate", partner.ContractDate ?? (object)DBNull.Value),
                new SqlParameter("@ContractScan", partner.ContractScan ?? (object)DBNull.Value)
                );
            switch (partner.Type)
            {
                case EPartnerType.NaturalPerson:
                    if (partner.NaturalPerson != null)
                    {
                        partner.NaturalPerson.PartnerId = partner.Id;
                        DeleteLegalEntity(partner.Id);
                        AddUpdateNaturalPerson(partner.NaturalPerson);
                    }
                    break;
                case EPartnerType.LegalEntity:
                    if (partner.LegalEntity != null)
                    {
                        partner.LegalEntity.PartnerId = partner.Id;
                        DeleteNaturalPerson(partner.Id);
                        AddUpdateLegalEntity(partner.LegalEntity);
                    }
                    break;
            }
        }

        public static void DeletePartner(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Partners].[Partner] WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }

        public static NaturalPerson GetNaturalPerson(int partnerId)
        {
            return SQLDataAccess.Query<NaturalPerson>(
                "SELECT * FROM [Partners].[NaturalPerson] WHERE PartnerId = @PartnerId", 
                new { PartnerId = partnerId }).FirstOrDefault();
        }

        private static void DeleteNaturalPerson(int partnerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Partners].[NaturalPerson] WHERE PartnerId = @PartnerId", 
                CommandType.Text, new SqlParameter("@PartnerId", partnerId));
        }

        private static void AddUpdateNaturalPerson(NaturalPerson naturalPerson)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(PartnerId) FROM Partners.NaturalPerson WHERE PartnerId = @PartnerId) = 0 " +
                "BEGIN " +
                    "INSERT INTO Partners.NaturalPerson " +
                        "(PartnerId, FirstName, LastName, Patronymic, PassportSeria, PassportNumber, PassportWhoGive, PassportWhenGive, RegistrationAddress, Zip, DateCreated, DateUpdated, PaymentTypeId, PaymentAccountNumber) " +
                    "VALUES (@PartnerId, @FirstName, @LastName, @Patronymic, @PassportSeria, @PassportNumber, @PassportWhoGive, @PassportWhenGive, @RegistrationAddress, @Zip, @DateCreated, @DateUpdated, @PaymentTypeId, @PaymentAccountNumber) " +
                "END ELSE BEGIN " +
                    "UPDATE Partners.NaturalPerson " +
                    "SET FirstName=@FirstName, LastName=@LastName, Patronymic=@Patronymic, PassportSeria=@PassportSeria, PassportNumber=@PassportNumber, " +
                        "PassportWhoGive=@PassportWhoGive, PassportWhenGive=@PassportWhenGive, RegistrationAddress=@RegistrationAddress, Zip=@Zip, DateUpdated=@DateUpdated, PaymentTypeId=@PaymentTypeId, PaymentAccountNumber=@PaymentAccountNumber " +
                    "WHERE PartnerId = @PartnerId " +
                "END",
                CommandType.Text,
                new SqlParameter("@PartnerId", naturalPerson.PartnerId),
                new SqlParameter("@FirstName", naturalPerson.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", naturalPerson.LastName ?? (object)DBNull.Value),
                new SqlParameter("@Patronymic", naturalPerson.Patronymic ?? (object)DBNull.Value),
                new SqlParameter("@PassportSeria", naturalPerson.PassportSeria ?? (object)DBNull.Value),
                new SqlParameter("@PassportNumber", naturalPerson.PassportNumber ?? (object)DBNull.Value),
                new SqlParameter("@PassportWhoGive", naturalPerson.PassportWhoGive ?? (object)DBNull.Value),
                new SqlParameter("@PassportWhenGive", naturalPerson.PassportWhenGive ?? (object)DBNull.Value),
                new SqlParameter("@RegistrationAddress", naturalPerson.RegistrationAddress ?? (object)DBNull.Value),
                new SqlParameter("@Zip", naturalPerson.Zip ?? (object)DBNull.Value),
                new SqlParameter("@PaymentTypeId", naturalPerson.PaymentTypeId ?? (object)DBNull.Value),
                new SqlParameter("@PaymentAccountNumber", naturalPerson.PaymentAccountNumber ?? (object)DBNull.Value),
                new SqlParameter("@DateCreated", DateTime.Now),
                new SqlParameter("@DateUpdated", DateTime.Now)
            );
        }

        public static LegalEntity GetLegalEntity(int partnerId)
        {
            return SQLDataAccess.Query<LegalEntity>(
                "SELECT * FROM [Partners].[LegalEntity] WHERE PartnerId = @PartnerId",
                new { PartnerId = partnerId }).FirstOrDefault();
        }

        private static void DeleteLegalEntity(int partnerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Partners].[LegalEntity] WHERE PartnerId = @PartnerId",
                CommandType.Text, new SqlParameter("@PartnerId", partnerId));
        }

        private static void AddUpdateLegalEntity(LegalEntity legalEntity)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(PartnerId) FROM Partners.LegalEntity WHERE PartnerId = @PartnerId) = 0 " +
                "BEGIN " +
                    "INSERT INTO Partners.LegalEntity " +
                        "(PartnerId, CompanyName, INN, KPP, LegalAddress, PostAddress, Zip, ActualAddress, SettlementAccount, Bank, CorrespondentAccount, " +
                        "BIK, Phone, ContactPerson, Director, Accountant, DateCreated, DateUpdated) " +
                    "VALUES (@PartnerId, @CompanyName, @INN, @KPP, @LegalAddress, @PostAddress, @Zip, @ActualAddress, @SettlementAccount, @Bank, @CorrespondentAccount, " +
                        "@BIK, @Phone, @ContactPerson, @Director, @Accountant, @DateCreated, @DateUpdated) " +
                "END ELSE BEGIN " +
                    "UPDATE Partners.LegalEntity " +
                    "SET CompanyName=@CompanyName, INN=@INN, KPP=@KPP, LegalAddress=@LegalAddress, PostAddress=@PostAddress, Zip=@Zip, " +
                        "ActualAddress=@ActualAddress, SettlementAccount=@SettlementAccount, Bank=@Bank, CorrespondentAccount=@CorrespondentAccount, " +
                        "BIK=@BIK, Phone=@Phone, ContactPerson=@ContactPerson, Director=@Director, Accountant=@Accountant, DateUpdated=@DateUpdated " +
                    "WHERE PartnerId = @PartnerId " +
                "END",
                CommandType.Text,
                new SqlParameter("@PartnerId", legalEntity.PartnerId),
                new SqlParameter("@CompanyName", legalEntity.CompanyName ?? (object)DBNull.Value),
                new SqlParameter("@INN", legalEntity.INN ?? (object)DBNull.Value),
                new SqlParameter("@KPP", legalEntity.KPP ?? (object)DBNull.Value),
                new SqlParameter("@LegalAddress", legalEntity.LegalAddress ?? (object)DBNull.Value),
                new SqlParameter("@PostAddress", legalEntity.PostAddress ?? (object)DBNull.Value),
                new SqlParameter("@Zip", legalEntity.Zip ?? (object)DBNull.Value),
                new SqlParameter("@ActualAddress", legalEntity.ActualAddress ?? (object)DBNull.Value),
                new SqlParameter("@SettlementAccount", legalEntity.SettlementAccount ?? (object)DBNull.Value),
                new SqlParameter("@Bank", legalEntity.Bank ?? (object)DBNull.Value),
                new SqlParameter("@CorrespondentAccount", legalEntity.CorrespondentAccount ?? (object)DBNull.Value),
                new SqlParameter("@BIK", legalEntity.BIK ?? (object)DBNull.Value),
                new SqlParameter("@Phone", legalEntity.Phone ?? (object)DBNull.Value),
                new SqlParameter("@ContactPerson", legalEntity.ContactPerson ?? (object)DBNull.Value),
                new SqlParameter("@Director", legalEntity.Director ?? (object)DBNull.Value),
                new SqlParameter("@Accountant", legalEntity.Accountant ?? (object)DBNull.Value),
                new SqlParameter("@DateCreated", DateTime.Now),
                new SqlParameter("@DateUpdated", DateTime.Now)
            );
        }

        public static void ChangePassword(int partnerId, string strNewPassword, bool isPassHashed)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE Partners.Partner SET Password = @Password WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", partnerId),
                new SqlParameter("@Password", isPassHashed ? strNewPassword : SecurityHelper.GetPasswordHash(strNewPassword)));
        }

        public static bool ExistsEmail(string email)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Id) FROM [Partners].[Partner] WHERE [Email] = @Email;", CommandType.Text, new SqlParameter("@Email", email)) > 0;
        }

        #endregion

        #region BindedCustomer

        public static List<BindedCustomer> GetBindedCustomers(int partnerId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            return SQLDataAccess.Query<BindedCustomer>(
                "SELECT * FROM Partners.BindedCustomer WHERE PartnerId = @PartnerId " +
                (dateFrom.HasValue ? " AND DateCreated >= @DateFrom" : string.Empty) +
                (dateTo.HasValue ? " AND DateCreated < @DateTo" : string.Empty),
                new
                {
                    partnerId,
                    dateFrom = dateFrom.HasValue ? dateFrom.Value.Date : (DateTime?)null,
                    dateTo = dateTo.HasValue ? dateTo.Value.Date.AddDays(1) : (DateTime?)null
                }).ToList();
        }

        public static BindedCustomer GetBindedCustomer(Guid customerId)
        {
            return SQLDataAccess.Query<BindedCustomer>(
                "SELECT * FROM Partners.BindedCustomer WHERE CustomerId = @CustomerId",
                new { customerId }).FirstOrDefault();
        }

        public static Partner GetCustomersPartner(Guid customerId)
        {
            return SQLDataAccess.Query<Partner>(
                "SELECT p.* FROM Partners.[Partner] p INNER JOIN Partners.BindedCustomer bc ON bc.PartnerId = p.Id WHERE CustomerId = @CustomerId",
                new { customerId }).FirstOrDefault();
        }

        public static void AddBindedCustomer(BindedCustomer bc)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Partners.BindedCustomer " +
                "(PartnerId, CustomerId, DateCreated, DateUpdated, UrlReferrer, UtmSource, UtmMedium, UtmCampaign, UtmTerm, UtmContent, Url, CouponCode, Enabled, VisitDate) " +
                "VALUES (@PartnerId, @CustomerId, @DateCreated, @DateUpdated, @UrlReferrer, @UtmSource, @UtmMedium, @UtmCampaign, @UtmTerm, @UtmContent, @Url, @CouponCode, @Enabled, @VisitDate)", 
                CommandType.Text,
                new SqlParameter("@PartnerId", bc.PartnerId),
                new SqlParameter("@CustomerId", bc.CustomerId),
                new SqlParameter("@Enabled", bc.Enabled),
                new SqlParameter("@DateCreated", DateTime.Now),
                new SqlParameter("@DateUpdated", DateTime.Now),
                new SqlParameter("@VisitDate", bc.VisitDate ?? (object)DBNull.Value),
                new SqlParameter("@CouponCode", bc.CouponCode.IsNotEmpty() ? bc.CouponCode.Reduce(50) : (object)DBNull.Value),
                new SqlParameter("@Url", bc.Url.IsNotEmpty() ? bc.Url.Reduce(500) : (object)DBNull.Value),
                new SqlParameter("@UrlReferrer", bc.UrlReferrer.IsNotEmpty() ? bc.UrlReferrer.Reduce(500) : (object)DBNull.Value),
                new SqlParameter("@UtmSource", bc.UtmSource.IsNotEmpty() ? bc.UtmSource.Reduce(500) : (object)DBNull.Value),
                new SqlParameter("@UtmMedium", bc.UtmMedium.IsNotEmpty() ? bc.UtmMedium.Reduce(500) : (object)DBNull.Value),
                new SqlParameter("@UtmCampaign", bc.UtmCampaign.IsNotEmpty() ? bc.UtmCampaign.Reduce(500) : (object)DBNull.Value),
                new SqlParameter("@UtmTerm", bc.UtmTerm.IsNotEmpty() ? bc.UtmTerm.Reduce(500) : (object)DBNull.Value),
                new SqlParameter("@UtmContent", bc.UtmContent.IsNotEmpty() ? bc.UtmContent.Reduce(500) : (object)DBNull.Value)
                );
        }

        public static void DeleteBindedCustomer(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Partners.BindedCustomer WHERE CustomerId = @CustomerId", CommandType.Text,
                new SqlParameter("@CustomerId", customerId));
        }

        public static int GetBindedCustomersCount(int partnerId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var sqlParams = new List<SqlParameter> { new SqlParameter("@PartnerId", partnerId) };
            if (dateFrom.HasValue)
                sqlParams.Add(new SqlParameter("@DateFrom", dateFrom.Value.Date));
            if (dateTo.HasValue)
                sqlParams.Add(new SqlParameter("@DateTo", dateTo.Value.Date.AddDays(1)));
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Partners.BindedCustomer WHERE PartnerId = @PartnerId" +
                (dateFrom.HasValue ? " AND DateCreated >= @DateFrom " : string.Empty) +
                (dateTo.HasValue ? " AND DateCreated < @DateTo" : string.Empty),
                CommandType.Text, sqlParams.ToArray());
        }

        public static void BindNewCustomer(Customer customer)
        {
            if (HttpContext.Current == null)
                return;

            //if admin or form admin area
            if (CustomerContext.CurrentCustomer != null && (CustomerContext.CurrentCustomer.IsAdmin ||
                                                            CustomerContext.CurrentCustomer.IsModerator))
            {
                return;
            }

            try
            {
                Partner partner;
                var data = GetReferralCookieData();

                if (data == null
                    || data.CouponCode.IsNullOrEmpty()
                    || (partner = GetPartnerByCoupon(data.CouponCode)) == null
                    || !partner.Enabled
                    || GetCustomersPartner(customer.Id) != null)
                {
                    return;
                }

                var bindedCustomer = new BindedCustomer
                {
                    PartnerId = partner.Id,
                    CustomerId = customer.Id,
                    CouponCode = data.AppliedCoupon ? data.CouponCode : null,
                    Url = data.Url,
                    UrlReferrer = data.Referrer,
                    UtmSource = data.UtmSource,
                    UtmMedium = data.UtmMedium,
                    UtmCampaign = data.UtmCampaign,
                    UtmContent = data.UtmContent,
                    UtmTerm = data.UtmTerm,
                    VisitDate = data.VisitDate
                };
                AddBindedCustomer(bindedCustomer);

                // commented: cookies needed to apply coupon automatically
                //ClearReferralCookie();

                if (partner.SendMessages.HasFlag(EPartnerMessageType.CustomerBinded))
                {
                    var mail = new PartnerCustomerBindedMailTemplate(partner, customer);
                    
                    MailService.SendMailNow(partner.Email, mail);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        #endregion

        #region Balance

        public static bool ProcessMoney(int partnerId, decimal amount, string basis, 
            bool isRewardPayout = false, DateTime? rewardPeriodTo = null, int? orderId = null, Guid? customerId = null,
            TransactionDetails details = null)
        {
            var partner = GetPartner(partnerId);
            if (partner == null || !partner.Enabled || amount == 0)
                return false;

            var currency = SettingsCatalog.DefaultCurrency;

            using (var scope = new TransactionScope())
            {
                // partner's balance in base currency
                partner.Balance += amount.RoundConvertToBase();

                // transaction in default currency
                var transaction = new Transaction
                {
                    PartnerId = partnerId,
                    Amount = amount,
                    Basis = basis,
                    Balance = partner.Balance.RoundConvertToDefault(),
                    IsRewardPayout = isRewardPayout,
                    RewardPeriodTo = rewardPeriodTo,
                    DetailsJson = details != null ? JsonConvert.SerializeObject(details) : null,
                    OrderId = orderId,
                    CustomerId = customerId,
                    Currency = currency
                };

                UpdatePartner(partner);
                TransactionService.AddTransaction(transaction);

                scope.Complete();
            }

            return true;
        }

        public static void ProcessOrderReward(Order order)
        {
            var bindedCustomer = GetBindedCustomer(order.OrderCustomer.CustomerID);
            if (bindedCustomer == null || !bindedCustomer.Enabled ||
                bindedCustomer.Partner == null || !bindedCustomer.Partner.Enabled ||
                TransactionService.OrderHasTransaction(order.OrderID))
                return;

            var orderItems = order.GetOrderItemsWithDiscountsAndFee()
                                  .AcceptableDifference(0.1f)
                                  .GetItems();

            var details = new TransactionDetails();
            var categoriesRewardPercent = GetRewardPercentCategories();
            decimal rewardSum = 0,
                productsSum = 0;
            var currency = order.OrderCurrency;
            foreach (var orderItem in orderItems)
            {
                if (orderItem.Price <= 0 || !orderItem.ProductID.HasValue)
                    continue;

                var oiDetails = new TransactionOrderItemDetails
                {
                    Amount = orderItem.Amount,
                    ArtNo = orderItem.ArtNo,
                    Name = orderItem.Name +
                        (orderItem.Size.IsNotEmpty() ? ", " + orderItem.Size : string.Empty) +
                        (orderItem.Color.IsNotEmpty() ? ", " + orderItem.Color : string.Empty),
                    Price = ((decimal)orderItem.Price).RoundConvertToDefault(currency.CurrencyValue),
                };

                var rewardPercent = bindedCustomer.Partner.RewardPercent;
                // если указан процент вознаграждения по категориям
                if (categoriesRewardPercent.Any())
                {
                    var productCategoryId = ProductService.GetFirstCategoryIdByProductId(orderItem.ProductID.Value);
                    var parentCategories = CategoryService.GetParentCategories(productCategoryId);
                    var category = parentCategories.FirstOrDefault(c => categoriesRewardPercent.ContainsKey(c.CategoryId));
                    if (category != null)   // есть категория с указанным процентом вознаграждения
                    {
                        rewardPercent = categoriesRewardPercent[category.CategoryId];
                        oiDetails.CategoryRewardPercent = rewardPercent;
                        oiDetails.CategoryPath = parentCategories.Reverse().Select(c => c.Name).AggregateString(" / ");
                    }
                    else
                    {
                        oiDetails.RewardPercent = rewardPercent;
                    }
                }

                var oiSum = ((decimal)(orderItem.Amount * orderItem.Price)).RoundConvertToDefault(currency.CurrencyValue);
                var oiReward = (oiSum * (decimal)rewardPercent / 100).RoundConvertToDefault();
                oiDetails.Sum = oiSum;
                oiDetails.Reward = oiReward;

                details.OrderItemsDetails.Add(oiDetails);

                rewardSum += oiReward;
                productsSum += oiSum;
            }

            if (rewardSum > 0)
            {
                var basis = string.Format("Начисление вознаграждения за заказ №{0} клиента {1}", order.Number, order.OrderCustomer.Email);
                ProcessMoney(bindedCustomer.PartnerId, rewardSum, basis, orderId: order.OrderID, customerId: order.OrderCustomer.CustomerID, details: details);

                var partner = GetPartner(bindedCustomer.PartnerId);
                if (partner.SendMessages.HasFlag(EPartnerMessageType.RewardAdded))
                {
                    var mail = new PartnerMoneyAddedMailTemplate(partner, order.OrderCustomer.Email, order.OrderCustomer.Phone, rewardSum.FormatRoundPriceDefault(), productsSum.FormatRoundPriceDefault());
                    
                    MailService.SendMailNow(partner.Email, mail);
                }
            }
        }

        #endregion

        #region CategoryRewardPercent

        public static void AddUpdateCategoryRewardPercent(int categoryId, float rewardPercent)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(CategoryId) FROM Partners.CategoryRewardPercent WHERE CategoryId = @CategoryId) = 0 " +
                    "INSERT INTO Partners.CategoryRewardPercent (CategoryId, RewardPercent, DateAdded) VALUES (@CategoryId, @RewardPercent, @DateAdded) " +
                "ELSE " +
                    "UPDATE Partners.CategoryRewardPercent SET RewardPercent = @RewardPercent WHERE CategoryId = @CategoryId",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RewardPercent", rewardPercent),
                new SqlParameter("@DateAdded", DateTime.Now)
            );
        }

        public static void UpdateCategoryRewardPercent(int categoryId, float rewardPercent)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Partners.CategoryRewardPercent SET RewardPercent = @RewardPercent WHERE CategoryId = @CategoryId",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@RewardPercent", rewardPercent)
            );
        }

        public static void DeleteCategoryRewardPercent(int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Partners.CategoryRewardPercent WHERE CategoryId = @CategoryId",
                CommandType.Text, new SqlParameter("@CategoryId", categoryId));
        }

        public static Dictionary<int, float> GetRewardPercentCategories()
        {
            return SQLDataAccess.ExecuteReadDictionary<int, float>(
                "SELECT CategoryId, RewardPercent FROM Partners.CategoryRewardPercent", CommandType.Text, "CategoryId", "RewardPercent");
        }

        #endregion

        /// <summary>
        /// apply partner coupon automatically if customer visited site by referral link
        /// </summary>
        public static void ApplyPartnerCoupon()
        {
            if (HttpContext.Current == null || !SettingsPartners.AutoApplyPartnerCoupon)
                return;

            var referralData = GetReferralCookieData();
            if (referralData != null && referralData.CouponCode.IsNotEmpty() && !referralData.AppliedCoupon &&
                CommonHelper.GetCookieString("refCouponApplied").IsNullOrEmpty() &&
                CustomerContext.CurrentCustomer.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup && 
                GiftCertificateService.GetCustomerCertificate() == null)
            {
                var coupon = CouponService.GetCouponByCode(referralData.CouponCode);
                if (coupon != null && CouponService.CanApplyCustomerCoupon(coupon))
                {
                    var customerGroupId = CustomerContext.CurrentCustomer.CustomerGroupId;
                    if (coupon.CustomerGroupIds.Contains(customerGroupId))
                    {
                        CouponService.AddCustomerCoupon(coupon.CouponID, false);
                        CommonHelper.SetCookie("refCouponApplied", "true", TimeSpan.FromDays(ReferralCookieExpires), true);
                    }
                }
            }
        }

        public static void OnOrderAdded(Order order)
        {
            if (HttpContext.Current == null)
                return;

            CommonHelper.DeleteCookie("refCouponApplied");
        }

        #region Referral Link

        /// <summary>
        /// customer visited site by referral link
        /// </summary>
        /// <param name="request"></param>
        public static void SetReferralCookie(HttpRequestBase request)
        {
            if (request == null || request[ReferralRequestParam].IsNullOrEmpty())// || CustomerContext.CurrentCustomer.RegistredUser)
                return;

            var couponCode = request[ReferralRequestParam];
            var currentData = GetReferralCookieData();
            if (currentData != null && currentData.CouponCode == couponCode && currentData.AppliedCoupon)
                return;

            var ts = new PartnerTrafficSource
            {
                CouponCode = couponCode,
                Referrer = request.GetUrlReferrer() != null ? request.GetUrlReferrer().ToString() : null,
                Url = (UrlService.IsSecureConnection(request) ? "https://" : "http://") + request.Url.Authority + request.RawUrl,
                UtmSource = HttpUtility.HtmlEncode(request["utm_source"]),
                UtmMedium = HttpUtility.HtmlEncode(request["utm_medium"]),
                UtmCampaign = HttpUtility.HtmlEncode(request["utm_campaign"]),
                UtmContent = HttpUtility.HtmlEncode(request["utm_content"]),
                UtmTerm = HttpUtility.HtmlEncode(request["utm_term"])
            };

            SetReferralCookie(ts);
        }

        /// <summary>
        /// customer applied coupon
        /// </summary>
        /// <param name="couponCode"></param>
        public static void SetReferralCookie(int couponId)
        {
            if (HttpContext.Current == null)
                return;
            //if admin or form admin area
            if (CustomerContext.CurrentCustomer != null && (CustomerContext.CurrentCustomer.IsAdmin ||
                                                            CustomerContext.CurrentCustomer.IsModerator))
            {
                return;
            }

            Coupon coupon;
            if (//CustomerContext.CurrentCustomer.RegistredUser || 
                GetPartnerByCoupon(couponId) == null || (coupon = CouponService.GetCoupon(couponId)) == null)
                return;

            var currentData = GetReferralCookieData();
            if (currentData != null && currentData.CouponCode == coupon.Code && !currentData.AppliedCoupon)
                return;

            SetReferralCookie(new PartnerTrafficSource
            {
                CouponCode = coupon.Code,
                AppliedCoupon = true
            });
        }

        private static void SetReferralCookie(PartnerTrafficSource ts)
        {
            ts.Hash = GetTrafficSourceHash(ts);
            ts.VisitDate = DateTime.Now;

            var expires = TimeSpan.FromDays(ReferralCookieExpires);
            var valueHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ts)));
            CommonHelper.SetCookie(ReferralCookieName, valueHash, expires, true);
        }

        public static void ClearReferralCookie()
        {
            CommonHelper.DeleteCookie(ReferralCookieName);
            CommonHelper.DeleteCookie("refCouponApplied");
        }

        public static PartnerTrafficSource GetReferralCookieData()
        {
            if (HttpContext.Current == null)
                return null;
            //if admin or form admin area
            if (CustomerContext.CurrentCustomer != null && (CustomerContext.CurrentCustomer.IsAdmin ||
                                                            CustomerContext.CurrentCustomer.IsModerator))
            {
                ClearReferralCookie();
                return null;
            }

            var valueHash = CommonHelper.GetCookieString(ReferralCookieName);
            if (valueHash.IsNullOrEmpty())
                return null;

            try
            {
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(valueHash)));
                var result = json.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<PartnerTrafficSource>(json);
                if (result != null && result.Hash != GetTrafficSourceHash(result))
                    return null;
                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }

        private static string GetTrafficSourceHash(PartnerTrafficSource source)
        {
            return new List<string>
            {
                source.CouponCode,
                source.Url,
                source.Referrer,
                source.UtmSource,
                source.UtmMedium,
                source.UtmCampaign,
                source.UtmContent,
                source.UtmTerm,

            }.Select(x => x.DefaultOrEmpty()).AggregateString(":").Md5(false);
        }
        #endregion
    }
}
