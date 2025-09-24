//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Trial;

namespace AdvantShop.Configuration
{
    public class SettingsMail
    {

        public const string SIX_STARS = "******";

        /// <summary>
        /// Get or sets the name of the SMTP server.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string SMTP
        {
            get => SettingProvider.Items["EmailSettingSMTP"];
            set => SettingProvider.Items["EmailSettingSMTP"] = value;
        }

        /// <summary>
        /// Get or sets the port that SMTP clients use to connect to an SMTP mail server. The default value is 25.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int Port
        {
            get => SettingProvider.Items["EmailSettingPort"].TryParseInt();
            set => SettingProvider.Items["EmailSettingPort"] = value.ToString();
        }

        /// <summary>
        /// Get or sets the user password to use to connect to SMTP mail server.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Password
        {
            get => SettingProvider.Items["EmailSettingPassword"];
            set => SettingProvider.Items["EmailSettingPassword"] = value;
        }

        public static string InternalDataL
        {
            get => SettingProvider.Items["InternalDataL"];
            set => SettingProvider.Items["InternalDataL"] = value;
        }

        public static string InternalDataP
        {
            get => SettingProvider.Items["InternalDataP"];
            set => SettingProvider.Items["InternalDataP"] = value;
        }




        /// <summary>
        /// Get or sets the login to use to connect to an SMTP mail server.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Login
        {
            get => SettingProvider.Items["EmailSettingLogin"];
            set => SettingProvider.Items["EmailSettingLogin"] = value;
        }

        /// <summary>
        /// Get or sets the default value that indicates who the email message is from.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string From
        {
            get => SettingProvider.Items["EmailSettingFrom"];
            set => SettingProvider.Items["EmailSettingFrom"] = value;
        }


        public static string SenderName
        {
            get => SettingProvider.Items["EmailSettingSenderName"];
            set => SettingProvider.Items["EmailSettingSenderName"] = value;
        }

        /// <summary>
        /// Get or sets the default value that indicates SLL option
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool SSL
        {
            get => SettingProvider.Items["EmailSettingSSL"].TryParseBool(true) ?? false;
            set => SettingProvider.Items["EmailSettingSSL"] = value.ToString();
        }

        public static string EmailForRegReport
        {
            get => SettingProvider.Items["Email_4_RegReport"];
            set => SettingProvider.Items["Email_4_RegReport"] = value;
        }
        public static string EmailForOrders
        {
            get => SettingProvider.Items["Email_4_orders"];
            set => SettingProvider.Items["Email_4_orders"] = value;
        }
        public static string EmailForProductDiscuss
        {
            get => SettingProvider.Items["Email_4_ProductDiscuss"];
            set => SettingProvider.Items["Email_4_ProductDiscuss"] = value;
        }

        public static string EmailForFeedback
        {
            get => SettingProvider.Items["Email_4_Feedback"];
            set => SettingProvider.Items["Email_4_Feedback"] = value;
        }

        public static string EmailForLeads
        {
            get => SettingProvider.Items["Email_NewLead"];
            set => SettingProvider.Items["Email_NewLead"] = value;
        }

        public static string EmailForBookings
        {
            get => SettingProvider.Items["Email_NewBooking"];
            set => SettingProvider.Items["Email_NewBooking"] = value;
        }

        public static string EmailForPartners
        {
            get => SettingProvider.Items["Email_NewPartner"];
            set => SettingProvider.Items["Email_NewPartner"] = value;
        }

        public static string EmailForMissedCall
        {
            get => SettingProvider.Items["Email_MissedCall"];
            set => SettingProvider.Items["Email_MissedCall"] = value;
        }

        public static int MailErrorCurrentCount
        {
            get => int.Parse(SettingProvider.Items["MailErrorCurrentCount"]);
            set => SettingProvider.Items["MailErrorCurrentCount"] = value.ToString(CultureInfo.InvariantCulture);
        }

        public static DateTime MailErrorLastSend
        {
            get => DateTime.Parse(SettingProvider.Items["MailErrorLastSend"], CultureInfo.InvariantCulture);
            set => SettingProvider.Items["MailErrorLastSend"] = value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ImapHost
        {
            get => SettingProvider.Items["EmailSettingImapHost"];
            set
            {
                SettingProvider.Items["EmailSettingImapHost"] = value;
                JobActivationManager.SettingUpdated();
            }
        }

        public static int ImapPort
        {
            get => SettingProvider.Items["EmailSettingImapPort"].TryParseInt();
            set => SettingProvider.Items["EmailSettingImapPort"] = value.ToString();
        }

        public static uint ImapLastUpdateLetterId
        {
            get => Convert.ToUInt32(SettingProvider.Items["ImapLastUpdateId"]);
            set => SettingProvider.Items["ImapLastUpdateId"] = value.ToString();
        }

        public static bool UseAdvantshopMail
        {
            get => SettingProvider.Items["EmailSettingUseAdvantshopMail"].TryParseBool();
            set => SettingProvider.Items["EmailSettingUseAdvantshopMail"] = value.ToString();
        }

        public static bool IsAdvantshopTrialFreeEmailEnabled =>
            TrialService.IsTrialEnabled
            && (UseAdvantshopMail
                ? !CapShopSettings.ConfirmDate.HasValue
                : IsSmtpNotConfigured);

        private static bool IsSmtpNotConfigured =>
            string.IsNullOrWhiteSpace(InternalDataL) && !string.IsNullOrWhiteSpace(InternalDataP)
            || ((string.IsNullOrWhiteSpace(Login) || Login.Equals(SIX_STARS))
                && (string.IsNullOrWhiteSpace(Password) || Password.Equals(SIX_STARS)));

        public static string AdvantshopTrialFreeEmailPostfix =>
            SettingProvider.Items["AdvantshopTrialFreeEmailPostfix"];

        public static int AdvantshopTrialFreeEmailSendCount
        {
            get => SettingProvider.Items["AdvantshopTrialFreeEmailSendCount"].TryParseInt();
            set => SettingProvider.Items["AdvantshopTrialFreeEmailSendCount"] = value.ToString();
        }

        public static int AdvantshopTrialFreeEmailSendLimit =>
            SettingProvider.Items["AdvantshopTrialFreeEmailLimit"].TryParseInt(defaultValue: 50);

        // ReSharper disable once InconsistentNaming
        public static string AdvantshopTrialFreeSMTP =>
            SettingProvider.Items["AdvantshopTrialFreeSMTP"];

        public static int AdvantshopTrialFreePort =>
            SettingProvider.Items["AdvantshopTrialFreePort"].TryParseInt();

        // ReSharper disable once InconsistentNaming
        public static bool AdvantshopTrialFreeSSL =>
            SettingProvider.Items["AdvantshopTrialFreeSSL"].TryParseBool();

        public static string AdvantshopTrialFreeLogin =>
            SettingProvider.Items["AdvantshopTrialFreeLogin"];

        public static string AdvantshopTrialFreePassword =>
            SettingProvider.Items["AdvantshopTrialFreePassword"];

        public static string AdvantshopTrialFreeEmailFrom =>
            SettingProvider.Items["AdvantshopTrialFreeEmailFrom"];

        public static string AdvantshopTrialFreeSenderName =>
            SettingProvider.Items["AdvantshopTrialFreeSenderName"];
    }
}