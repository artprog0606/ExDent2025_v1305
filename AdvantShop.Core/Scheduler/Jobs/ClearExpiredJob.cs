//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq.Expressions;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Auth.Smses;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Core.Services.Statistic.QuartzJobs;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Security;
using AdvantShop.SEO;
using AdvantShop.Statistic;
using AdvantShop.Track.EventLog;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    //[DisallowConcurrentExecution]
    public class ClearExpiredJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            context.TryRun(() => ShoppingCartService.DeleteExpiredShoppingCartItems(DateTime.Today.AddMonths(-3)));

            context.TryRun(() => Services.Booking.Cart.ShoppingCartService.DeleteExpiredShoppingCartItems(DateTime.Today.AddMonths(-3)));
            context.TryRun(OrderConfirmationService.DeleteExpired);
            context.TryRun(InternalServices.DeleteExpiredAppRestartLogData);
            context.TryRun(Secure.DeleteExpiredAuthorizeLog);
            context.TryRun(() => ClientCodeService.DeleteExpired(DateTime.Now.AddDays(-7)));
            context.TryRun(AdminNotificationService.ClearExpiredAdminNotifications);
            context.TryRun(Error404Service.DeleteExpired);
            context.TryRun(RecentlyViewService.DeleteExpired);
            context.TryRun(CouponService.DeleteExpiredGeneratedCoupons);
            context.TryRun(ChangeHistoryService.DeleteExpiredHistory);
            context.TryRun(QuartzJobsLoggingService.ClearExpiredLogs);
            context.TryRun(() => StatisticService.DeleteExpiredSearchStatistic(DateTime.Today.AddMonths(-4)));
            //почистить старые бекапы фоток
            context.TryRun(FileHelpers.CleanBackupPhoto);
            
            //перемешивать списки товаров
            context.TryRun(ProductOnMain.ShuffleLists);

            //пересчитать популярность товаров
            context.TryRun(ProductService.RecalcSortPopularMass);
            
            context.TryRun(SmsLogger.ClearLog);
            context.TryRun(SmsBanService.Clear);
            context.TryRun(new SmsCodeConfirmationRepository().ClearSmsCodes);
            context.TryRun(AdminInformerService.ClearExpired);

            context.TryRun(TrackEventLogService.ClearExpiredEventLogs);

            context.TryRun(() => ShareShoppingCartService.DeleteExpiredShareShoppingCartItems(DateTime.Today.AddMonths(-1)));

            context.WriteLastRun();
        }
    }
}
