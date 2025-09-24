//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;

namespace AdvantShop.Payment
{
    public enum ProcessType
    {
        None,
        FormPost,
        Javascript,
        PageRedirect,
        ServerRequest
    }
    [Flags]
    public enum NotificationType
    {
        None = 0x0,
        ReturnUrl = 0x1,
        Handler = 0x2
    }
    [Flags]
    public enum UrlStatus
    {
        None = 0x0,
        ReturnUrl = 0x1,
        CancelUrl = 0x2,
        FailUrl = 0x4,
        NotificationUrl = 0x8
    }

    public static class NotificationMessahges
    {
        public static string InvalidRequestData => LocalizationService.GetResource("Core.Payment.NotificationMessage.InvalidRequestData");

        public static string TestMode => LocalizationService.GetResource("Core.Payment.NotificationMessage.TestModeEnabled");

        public static string Fail => LocalizationService.GetResource("Core.Payment.NotificationMessage.OrderProcessingFailed");

        public static string LogError(Exception ex)
        {
            Debug.Log.Error(ex);
            return LocalizationService.GetResource("Core.Payment.NotificationMessage.ProcessingFailedWithException") + ex.Message;
        }

        public static string SuccessfullPayment(string orderNumber)
        {
            return string.Format(LocalizationService.GetResource("Core.Payment.NotificationMessage.OrderSuccessfullyPaid"), orderNumber);
        }
    }

    public enum PaperPaymentType
    {
        NonPaperMethod = 0,
        SberBank,
        Bill,
        Check,
        BillUa
    }
    
    public enum ExtrachargeType
    {
        [Localize("Core.Payment.ExtrachargeType.Fixed")]
        Fixed,

        [Localize("Core.Payment.ExtrachargeType.Percent")]
        Percent
    }

    /// <summary>
    /// Hide payment currency in admin area
    /// </summary>
    public interface IPaymentCurrencyHide
    {
    }

    public class PaymentShippingAdminModel
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public int MethodsCount { get; set; }
        public bool Active { get { return MethodsCount == 0; } }
        public bool Enabled { get; set; }
    }
}