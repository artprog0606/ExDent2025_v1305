//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    public enum PageWithPaymentButton
    {
        myaccount,
        orderconfirmation
    }

    public class PaymentService
    {
        private const string PaymentCacheKey = "PaymentMethods_";
        private const string PaymentCreditCacheKey = PaymentCacheKey + "Credit";

        public static List<PaymentMethod> GetAllPaymentMethods(bool onlyEnabled)
        {
            var cacheKey = PaymentCacheKey + (onlyEnabled ? "Active" : "All");

            return CacheManager.Get(cacheKey, () =>
                SQLDataAccess.ExecuteReadList(
                    onlyEnabled
                        ? "SELECT * FROM [Order].[PaymentMethod] left join Catalog.Photo on Photo.ObjId=PaymentMethod.PaymentMethodID and Type=@Type where Enabled=1 ORDER BY [SortOrder]"
                        : "SELECT * FROM [Order].[PaymentMethod] left join Catalog.Photo on Photo.ObjId=PaymentMethod.PaymentMethodID and Type=@Type ORDER BY [SortOrder]",
                    CommandType.Text,
                    reader => GetPaymentMethodFromReader(reader, true),
                    new SqlParameter("@Type", PhotoType.Payment.ToString()))
                .Where(method => method != null) // т.к. GetPaymentMethodFromReader может возвращать null, если не не найдет реализацию метода оплаты указанного типа
                .ToList()
            );
        }

        public static IEnumerable<int> GetAllPaymentMethodIDs()
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("SELECT [PaymentMethodID] FROM [Order].[PaymentMethod]", CommandType.Text, "PaymentMethodID");
        }

        public static PaymentMethod GetPaymentMethod(int paymentMethodId)
        {
            var payment =
                SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Order].[PaymentMethod] WHERE [PaymentMethodID] = @PaymentMethodID", CommandType.Text,
                    reader => GetPaymentMethodFromReader(reader),
                    new SqlParameter("@PaymentMethodID", paymentMethodId));

            return payment;
        }

        public static PaymentMethod GetPaymentMethodByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT top(1) * FROM [Order].[PaymentMethod] WHERE [Name] = @Name",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader), new SqlParameter("@Name", name));
        }

        public static PaymentMethodAdminModel GetPaymentMethodAdminModel(int methodId)
        {
            var method = GetPaymentMethod(methodId);
            if (method == null)
                return null;

            var type = GetPaymentMethodAdminModelType(method.PaymentKey);
            if (type is null)
                return null;

            var model = (PaymentMethodAdminModel)Activator.CreateInstance(type);

            model.PaymentMethodId = method.PaymentMethodId;
            model.PaymentKey = method.PaymentKey;
            model.Name = method.Name;
            model.Description = method.Description;
            model.Enabled = method.Enabled;
            model.SortOrder = method.SortOrder;
            model.ExtrachargeInNumbers = method.ExtrachargeInNumbers;
            model.ExtrachargeInPercents = method.ExtrachargeInPercents;
            model.CurrencyId = method.CurrencyId;
            model.CurrencyAllAvailable = method.CurrencyAllAvailable;
            model.CurrencyIso3Available = method.CurrencyIso3Available;

            var icon = method.IconFileName;
            model.Icon = icon != null ? FoldersHelper.GetPath(FolderType.PaymentLogo, icon.PhotoName, false) : "";
            model.Parameters = method.Parameters;

            model.ProcessType = method.ProcessType;
            model.NotificationType = method.NotificationType;
            model.ShowUrls = method.ShowUrls;
            model.SuccessUrl = method.SuccessUrl;
            model.CancelUrl = method.CancelUrl;
            model.FailUrl = method.FailUrl;
            model.NotificationUrl = method.NotificationUrl;

            model.ShowCurrency = !(method is IPaymentCurrencyHide);
            model.TaxId = method.TaxId;
            model.Taxes = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = LocalizationService.GetResource("Admin.PaymentMethods.Common.DefaultUseTax"),
                    Value = ""
                }
            };
            model.CustomerType = (CustomerType)method.CustomerType;
            model.CustomerTypes = Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>().Select(x => new SelectListItem
            {
                Text = x.Localize(),
                Value = x.ToString()
            }).OrderBy(x => x.Text).ToList();

            var taxes = TaxService.GetTaxes();
            if (taxes != null && taxes.Count > 0)
            {
                var temp = taxes.Select(x => new SelectListItem { Text = x.Name, Value = x.TaxId.ToString() }).ToList();
                model.Taxes.AddRange(temp);
            }

            return model;
        }

        private static Type GetPaymentMethodAdminModelType(string paymentType)
        {
            return ReflectionExt.GetTypeByAttributeValue<PaymentAdminModelAttribute>(typeof(PaymentMethodAdminModel), atr => atr.Value, paymentType);
        }
        
        public static List<ICreditPaymentMethod> GetCreditPaymentMethods()
        {
            if (CacheManager.Contains(PaymentCreditCacheKey))
                return CacheManager.Get<List<ICreditPaymentMethod>>(PaymentCreditCacheKey);
            var list = GetAllPaymentMethods(true).OfType<ICreditPaymentMethod>().Where(x => x.ActiveCreditPayment).ToList();
            CacheManager.Insert(PaymentCreditCacheKey, list);
            return list;
        }

        public static PaymentMethod GetPaymentMethodFromReader(SqlDataReader reader, bool loadPicture = false)
        {
            var method = PaymentMethod.Create(SQLDataHelper.GetString(reader, "PaymentType"));
            if (method == null)
                return null;

            method.PaymentMethodId = SQLDataHelper.GetInt(reader, "PaymentMethodID");
            method.Name = SQLDataHelper.GetString(reader, "Name");
            method.Enabled = SQLDataHelper.GetBoolean(reader, "Enabled");
            method.Description = SQLDataHelper.GetString(reader, "Description");
            method.SortOrder = SQLDataHelper.GetInt(reader, "SortOrder");
            method.ExtrachargeInNumbers = SQLDataHelper.GetFloat(reader, "ExtrachargeInNumbers");
            method.ExtrachargeInPercents = SQLDataHelper.GetFloat(reader, "ExtrachargeInPercents");
            method.IconFileName = loadPicture
                ? new Photo(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"),
                    PhotoType.Payment)
                { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") }
                : null;
            method.CurrencyId = SQLDataHelper.GetInt(reader, "CurrencyId");
            method.TaxId = SQLDataHelper.GetNullableInt(reader, "TaxId");

            method.Parameters = GetPaymentMethodParameters(method.PaymentMethodId);
            method.CustomerType = SQLDataHelper.GetInt(reader, "CustomerType");
            method.ModuleStringId = SQLDataHelper.GetString(reader, "ModuleStringId");
            
            return method;
        }

        public static Dictionary<string, string> GetPaymentMethodParameters(int paymentMethodId)
        {
            return
                SQLDataAccess.ExecuteReadDictionary<string, string>(
                    "SELECT Name, Value FROM [Order].[PaymentParam] WHERE [PaymentMethodID] = @PaymentMethodID",
                    CommandType.Text, "Name", "Value", new SqlParameter("@PaymentMethodID", paymentMethodId));
        }

        public static int AddPaymentMethod(PaymentMethod method)
        {
            var id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [Order].[PaymentMethod] ([PaymentType], [Name], [Enabled], [Description], [SortOrder], ExtrachargeInNumbers, ExtrachargeInPercents, CurrencyId, CustomerType, ModuleStringId) " +
                "VALUES (@PaymentType, @Name, @Enabled, @Description, @SortOrder, @ExtrachargeInNumbers, @ExtrachargeInPercents, @CurrencyId, @CustomerType, @ModuleStringId); SELECT scope_identity();",
                CommandType.Text,
                new SqlParameter("@PaymentType", method.PaymentKey),
                new SqlParameter("@Name", method.Name ?? string.Empty),
                new SqlParameter("@Enabled", method.Enabled),
                new SqlParameter("@Description", method.Description ?? string.Empty),
                new SqlParameter("@SortOrder", method.SortOrder),
                new SqlParameter("@ExtrachargeInNumbers", method.ExtrachargeInNumbers),
                new SqlParameter("@ExtrachargeInPercents", method.ExtrachargeInPercents),
                new SqlParameter("@CurrencyId", method.CurrencyId),
                new SqlParameter("@CustomerType", (int)method.CustomerType),
                new SqlParameter("@ModuleStringId", method.ModuleStringId ?? (object)DBNull.Value));

            AddPaymentMethodParameters(id, method.Parameters);
            CacheManager.RemoveByPattern(PaymentCacheKey);

            return id;
        }

        private static void AddPaymentMethodParameters(int paymentMethodId, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters.Where(parameter => parameter.Value.IsNotEmpty()))
            {
                SQLDataAccess.ExecuteNonQuery(
                    "INSERT INTO [Order].[PaymentParam] (PaymentMethodID, Name, Value) VALUES (@PaymentMethodID, @Name, @Value)",
                    CommandType.Text,
                    new SqlParameter("@PaymentMethodID", paymentMethodId),
                    new SqlParameter("@Name", parameter.Key),
                    new SqlParameter("@Value", parameter.Value));
            }
        }

        public static void UpdatePaymentMethod(PaymentMethod paymentMethod, bool updateParams = true)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[PaymentMethod] SET [Name] = @Name,[Enabled] = @Enabled,[SortOrder] = @SortOrder,[Description] = @Description,[PaymentType] = @PaymentType, " +
                "ExtrachargeInNumbers=@ExtrachargeInNumbers, ExtrachargeInPercents=@ExtrachargeInPercents, CurrencyId=@CurrencyId, TaxId=@TaxId, CustomerType=@CustomerType WHERE [PaymentMethodID] = @PaymentMethodID",
                CommandType.Text,
                new SqlParameter("@PaymentMethodID", paymentMethod.PaymentMethodId),
                new SqlParameter("@Name", paymentMethod.Name),
                new SqlParameter("@Enabled", paymentMethod.Enabled),
                new SqlParameter("@SortOrder", paymentMethod.SortOrder),
                new SqlParameter("@Description", paymentMethod.Description),
                new SqlParameter("@PaymentType", paymentMethod.PaymentKey),
                new SqlParameter("@ExtrachargeInNumbers", paymentMethod.ExtrachargeInNumbers),
                new SqlParameter("@ExtrachargeInPercents", paymentMethod.ExtrachargeInPercents),
                new SqlParameter("@CurrencyId", paymentMethod.CurrencyId),
                new SqlParameter("@TaxId", paymentMethod.TaxId == null ? (object)DBNull.Value : paymentMethod.TaxId.Value),
                new SqlParameter("@CustomerType", paymentMethod.CustomerType)
                );

            if (updateParams)
            {
                UpdatePaymentParams(paymentMethod.PaymentMethodId, paymentMethod.Parameters);
                CacheManager.RemoveByPattern(CacheNames.PaymentOptions);
            }

            CacheManager.RemoveByPattern(PaymentCacheKey);

        }

        public static void UpdatePaymentParams(int paymentMethodId, Dictionary<string, string> parameters)
        {
            foreach (var kvp in parameters)
            {
                SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdatePaymentParam]", CommandType.StoredProcedure,
                    new SqlParameter("@PaymentMethodID", paymentMethodId),
                    new SqlParameter("@Name", kvp.Key),
                    new SqlParameter("@Value", kvp.Value ?? ""));
            }
            CacheManager.RemoveByPattern(CacheNames.PaymentOptions);
        }

        public static void DeletePaymentMethod(int paymentMethodId)
        {
            PhotoService.DeletePhotos(paymentMethodId, PhotoType.Payment);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[PaymentMethod] WHERE [PaymentMethodID] = @PaymentMethodID", CommandType.Text, new SqlParameter("@PaymentMethodID", paymentMethodId));
            CacheManager.RemoveByPattern(PaymentCacheKey);
        }

        public static List<PaymentMethod> GetCertificatePaymentMethods()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT * FROM [Settings].[GiftCertificatePayments] INNER JOIN [Order].[PaymentMethod] ON [PaymentMethod].[PaymentMethodID] = [GiftCertificatePayments].[PaymentID] WHERE [Enabled] = 1 ORDER BY [SortOrder]",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader));
        }

        public static void SaveOrderpaymentInfo(int orderId, int paymentId, string name, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                    "INSERT INTO [Order].[OrderPaymentInfo] (OrderID, PaymentMethodID, Name, Value) VALUES (@OrderID, @PaymentMethodID, @Name, @Value)", // DELETE FROM [Order].[OrderPaymentInfo] WHERE OrderID=@OrderID and PaymentMethodID=@PaymentMethodID and Name=@Name;
                    CommandType.Text,
                    new SqlParameter("@OrderID", orderId),
                    new SqlParameter("@PaymentMethodID", paymentId),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Value", value));
        }

        public static PaymentAdditionalInfo GetOrderIdByPaymentIdAndCode(int paymentMethodId, string responseCode)
        {
            return 
                GetPaymentAdditionalInfoBy(paymentId: paymentMethodId, value: responseCode ?? string.Empty)
               .FirstOrDefault();
        }

        public static IEnumerable<PaymentAdditionalInfo> GetPaymentAdditionalInfoBy(
            int? orderId = null, int? paymentId = null, string name = null, string value = null)
        {
            if (orderId is null
                && paymentId is null
                && name.IsNullOrEmpty()
                && value.IsNullOrEmpty())
                throw new ArgumentException("Must pass one of the parameters.");

            var where = new List<string>();

            if (orderId != null) 
                where.Add("[OrderID] = @OrderID");

            if (paymentId != null) 
                where.Add("[PaymentMethodID] = @PaymentMethodID");

            if (name != null) 
                where.Add("[Name] = @Name");

            if (value != null) 
                where.Add("[Value] = @Value");

            return SQLDataAccess.ExecuteReadIEnumerable(
                "Select * From [Order].[OrderPaymentInfo] Where"
                + string.Join(" AND ", where),
                CommandType.Text,
                reader => new PaymentAdditionalInfo
                {
                    PaymentMethodId = SQLDataHelper.GetInt(reader, "PaymentMethodID"),
                    OrderId = SQLDataHelper.GetInt(reader, "OrderID"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    Value = SQLDataHelper.GetString(reader, "Value")
                },
                new SqlParameter("@OrderID", orderId ?? (object)DBNull.Value),
                new SqlParameter("@PaymentMethodID", paymentId ?? (object)DBNull.Value),
                new SqlParameter("@Name", name ?? (object)DBNull.Value),
                new SqlParameter("@Value", value ?? (object)DBNull.Value));
        }

        public static List<PaymentMethod> UseGeoMapping(List<PaymentMethod> listMethods, string country, string city)
        {
            var items = new List<PaymentMethod>();
            foreach (var elem in listMethods)
            {
                if (ShippingPaymentGeoMaping.IsExistGeoPayment(elem.PaymentMethodId))
                {
                    if (ShippingPaymentGeoMaping.CheckPaymentEnabledGeo(elem.PaymentMethodId, country, city))
                        items.Add(elem);
                }
                else
                {
                    items.Add(elem);
                }
            }
            return items;
        }

        public static void ClearCach()
        {
            CacheManager.RemoveByPattern(PaymentCacheKey);
        }
    }
}