using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Booking.Sms;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class BookingProcessingJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            context.TryRun(SmsNotification);
            context.TryRun(CancelUnpaid);
        }

        private static void SmsNotification()
        {
            try
            {
                var service = new BookingNotificationBeforeStartService();
                foreach (var booking in service.GetList(DateTime.Now))
                {
                    if (!booking.StandardPhone.HasValue || string.IsNullOrWhiteSpace(booking.SmsTemplateBeforeStartBooiking))
                        continue;

                    var smsBody = SmsTemplateService.BuildTemplate(booking.SmsTemplateBeforeStartBooiking, booking);
                    if (string.IsNullOrWhiteSpace(smsBody))
                        continue;

                    SmsNotifier.SendSms(booking.StandardPhone.Value, smsBody, booking.CustomerId, inBackground: false, isInternal: true);

                    SQLDataAccess.ExecuteNonQuery(
                        "UPDATE Booking.Booking SET IsSendedSmsBeforeStart = 1 WHERE [Id] = @BookingId",
                        CommandType.Text,
                        new SqlParameter("@BookingId", booking.Id));
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }

        private static void CancelUnpaid()
        {
            try
            {
                var changeBy = new ChangedBy("Job CancelUnpaid");
                foreach (var booking in BookingService.GetAllForCancellation(DateTime.Now))
                {
                    BookingService.ChangeStatus(booking.Id, BookingStatus.Cancel, changeBy, trackChanges: true);

                    BizProcessExecuter.BookingChanged(booking);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
        }
    }
}
