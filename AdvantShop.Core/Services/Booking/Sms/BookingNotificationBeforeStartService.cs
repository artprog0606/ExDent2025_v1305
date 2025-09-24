using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Booking.Sms
{
    public class BookingNotificationBeforeStartService
    {
        public List<BookingNotification> GetList(DateTime now)
        {
            return
                SQLDataAccess.Query<BookingNotification>("SELECT [Booking].*, Affiliate.SmsTemplateBeforeStartBooiking FROM [Booking].[Booking]" +
                                             " INNER JOIN [Booking].[Affiliate] ON [Booking].[AffiliateId] = [Affiliate].[Id]" +
                                             " WHERE [Affiliate].[IsActiveSmsNotification] = 1 AND [ForHowManyMinutesToSendSms] IS NOT NULL" +
                                             " AND [Booking].[IsSendedSmsBeforeStart] = 0 AND [Booking].[BeginDate] >= @ToDay" +
                                             " AND @Now >= DATEADD(minute, -1*[Affiliate].[ForHowManyMinutesToSendSms], [Booking].[BeginDate])",
                    new {ToDay = now.Date, Now = now})
                    .ToList();
        }
    }

    public class BookingNotification : Booking
    {
        public string SmsTemplateBeforeStartBooiking { get; set; }
    }
}
