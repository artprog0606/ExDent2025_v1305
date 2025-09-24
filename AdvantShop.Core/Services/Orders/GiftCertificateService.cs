//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Security;

namespace AdvantShop.Orders
{
    public class GiftCertificateService
    {

        #region add, get, update, delete

        private static GiftCertificate GetFromReader(SqlDataReader reader)
        {
            return new GiftCertificate
            {
                CertificateId = SQLDataHelper.GetInt(reader, "CertificateID"),
                CertificateCode = SQLDataHelper.GetString(reader, "CertificateCode"),
                FromName = SQLDataHelper.GetString(reader, "FromName"),
                ToName = SQLDataHelper.GetString(reader, "ToName"),
                ApplyOrderNumber = SQLDataHelper.GetString(reader, "ApplyOrderNumber"),
                OrderId = SQLDataHelper.GetNullableInt(reader, "OrderID"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                Used = SQLDataHelper.GetBoolean(reader, "Used"),
                Enable = SQLDataHelper.GetBoolean(reader, "Enable"),
                CertificateMessage = SQLDataHelper.GetString(reader, "Message"),
                ToEmail = SQLDataHelper.GetString(reader, "ToEmail"),
                CreationDate = SQLDataHelper.GetDateTime(reader, "CreationDate"),
            };
        }


        public static GiftCertificate GetCertificateById(int certificateId)
        {
            return SQLDataAccess.ExecuteReadOne<GiftCertificate>("[Order].[sp_GetCertificateById]",
                                                                                        CommandType.StoredProcedure, GetFromReader,
                                                                                        new SqlParameter("@CertificateID", certificateId));
        }
        public static GiftCertificate GetCertificateByCode(string certificateCode)
        {
            return SQLDataAccess.ExecuteReadOne<GiftCertificate>("[Order].[sp_GetCertificateByCode]",
                                                                                        CommandType.StoredProcedure, GetFromReader,
                                                                                        new SqlParameter("@CertificateCode", certificateCode));
        }

        public static List<GiftCertificate> GetOrderCertificates(int orderId)
        {
            return SQLDataAccess.ExecuteReadList<GiftCertificate>("[Order].[sp_GetOrderCertificates]",
                                                                                        CommandType.StoredProcedure, GetFromReader,
                                                                                        new SqlParameter("@OrderID", orderId));
        }


        public static int AddCertificate(GiftCertificate certificate)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Order].[sp_AddCertificate]", CommandType.StoredProcedure,
                                                      new SqlParameter("@CertificateCode", certificate.CertificateCode),
                                                      new SqlParameter("@ApplyOrderNumber", String.IsNullOrEmpty(certificate.ApplyOrderNumber) ? (object)DBNull.Value : certificate.ApplyOrderNumber),
                                                      new SqlParameter("@OrderID", certificate.OrderId == 0 ? (object)DBNull.Value : certificate.OrderId),
                                                      new SqlParameter("@FromName", certificate.FromName),
                                                      new SqlParameter("@ToName", certificate.ToName),
                                                      new SqlParameter("@Used", certificate.Used),
                                                      new SqlParameter("@Enable", certificate.Enable),
                                                      new SqlParameter("@Sum", certificate.Sum),
                                                      new SqlParameter("@Message", certificate.CertificateMessage),
                                                      new SqlParameter("@ToEmail", certificate.ToEmail)
                );
        }

        public static void UpdateCertificateById(GiftCertificate certificate)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateCertificateById]", CommandType.StoredProcedure,
                new SqlParameter("@CertificateId", certificate.CertificateId),
                new SqlParameter("@CertificateCode", certificate.CertificateCode),
                new SqlParameter("@OrderID", certificate.OrderId == 0 || !certificate.OrderId.HasValue ? (object)DBNull.Value : certificate.OrderId),
                new SqlParameter("@ApplyOrderNumber", String.IsNullOrEmpty(certificate.ApplyOrderNumber) ? (object)DBNull.Value : certificate.ApplyOrderNumber),
                new SqlParameter("@FromName", certificate.FromName),
                new SqlParameter("@ToName", certificate.ToName),
                new SqlParameter("@Used", certificate.Used),
                new SqlParameter("@Enable", certificate.Enable),
                new SqlParameter("@Sum", certificate.Sum),
                new SqlParameter("@Message", certificate.CertificateMessage),
                new SqlParameter("@ToEmail", certificate.ToEmail));
        }

        public static void DeleteCertificateById(int certificateId)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_DeleteCertificateById]", CommandType.StoredProcedure, new SqlParameter("@CertificateId", certificateId));
        }
        #endregion

        #region CustomerSertificate
        public static GiftCertificate GetCustomerCertificate()
        {
            return GetCustomerCertificate(CustomerContext.CustomerId);
        }

        public static GiftCertificate GetCustomerCertificate(Guid customerId)
        {
            var certificate = SQLDataAccess.ExecuteReadOne<GiftCertificate>
                ("Select TOP 1 * From [Order].[Certificate] Where CertificateID = (Select CertificateID From Customers.CustomerCertificate Where CustomerID = @CustomerID)",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@CustomerID", customerId));

            if (certificate != null)
            {
                if (certificate.Paid && !certificate.Used)
                    return certificate;

                DeleteCustomerCertificate(certificate.CertificateId);
                return null;
            }
            return null;
        }

        public static void DeleteCustomerCertificate(int certificateId)
        {
            DeleteCustomerCertificate(certificateId, CustomerContext.CustomerId);
        }

        public static void DeleteCustomerCertificate(int certificateId, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Customers.CustomerCertificate Where CertificateID = @CertificateID and CustomerID = @CustomerID",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@CertificateID", certificateId));
        }


        public static void AddCustomerCertificate(int certificateId)
        {
            AddCustomerCertificate(certificateId, CustomerContext.CustomerId);
        }

        public static void AddCustomerCertificate(int certificateId, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Customers.CustomerCertificate (CustomerID, CertificateID) VALUES (@CustomerID, @CertificateID)",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@CertificateID", certificateId));
        }

        #endregion

        public static string GenerateCertificateCode()
        {
            var code = String.Empty;
            while (String.IsNullOrEmpty(code) || IsExistCertificateCode(code) || CouponService.IsExistCouponCode(code))
            {
                code = @"C-" + Strings.GetRandomString(8);
            }
            return code;
        }

        public static bool IsExistCertificateCode(string code)
        {
            return SQLDataHelper.GetInt(
                SQLDataAccess.ExecuteScalar(
                    "Select COUNT(CertificateID) From [Order].[Certificate] Where CertificateCode = @CertificateCode",
                    CommandType.Text,
                    new SqlParameter("@CertificateCode", code))) > 0;
        }

        public static List<GiftCertificate> GetCertificates()
        {
            List<GiftCertificate> certificates = SQLDataAccess.ExecuteReadList<GiftCertificate>("Select * From [Order].[Certificate]", CommandType.Text, GetFromReader);
            return certificates;
        }

        public static float GetCertificatePriceById(int id)
        {
            return SQLDataHelper.GetFloat(SQLDataAccess.ExecuteScalar("Select Sum From [Order].[Certificate] Where CertificateId = @CertificateId",
                                                                        CommandType.Text,
                                                                        new SqlParameter("@CertificateId", id)));
        }

        public static void SendCertificateMails(GiftCertificate certificate)
        {
            var certificateSumFormatPrice = PriceFormatService.FormatPrice(
                certificate.Sum, 
                certificate.CertificateOrder?.OrderCurrency 
                ?? CurrencyService.CurrentCurrency);

            var certificateMailTemplate = new CertificateMailTemplate(
                certificate.CertificateCode,
                certificate.FromName,
                certificate.ToName,
                certificateSumFormatPrice,
                certificate.CertificateMessage);

            MailService.SendMailNow(
                certificate.CertificateOrder?.OrderCustomer?.CustomerID 
                ?? certificate.CertificateOrder?.LinkedCustomerId 
                ?? CustomerContext.CustomerId,
                certificate.ToEmail,
                certificateMailTemplate);
            MailService.SendMailNow(SettingsMail.EmailForOrders, certificateMailTemplate, replyTo: certificate.ToEmail);
        }

        public static void DeleteCertificatePaymentMethods()
        {
            SQLDataAccess.ExecuteNonQuery("Delete FROM [Settings].[GiftCertificatePayments]", CommandType.Text);
        }

        public static void SaveCertificatePaymentMethods(List<int> paymentMethodsIds)
        {
            DeleteCertificatePaymentMethods();

            foreach (var paymentMethodsId in paymentMethodsIds)
            {
                SQLDataAccess.ExecuteScalar(
                    "INSERT INTO [Settings].[GiftCertificatePayments] ([PaymentID]) VALUES (@PaymentID)",
                    CommandType.Text,
                    new SqlParameter("@PaymentID", paymentMethodsId));
            }
        }

        public static List<int> GetCertificatePaymentMethodsID()
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT * FROM [Settings].[GiftCertificatePayments]",
                CommandType.Text, "PaymentID");
        }
    }
}