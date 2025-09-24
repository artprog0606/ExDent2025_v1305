//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Diagnostics;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Scheduler.Jobs
{
    public class DiscountByTimeJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (!context.CanStart()) 
                return;

            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                var jobData = dataMap.Get(TaskManager.DataMap) as TaskSetting;
                var dataStr = jobData?.DataMap?.ToString();
                if (dataStr.IsNullOrEmpty())
                {
                    TaskManager.TaskManagerInstance().RemoveTask(jobData.GetUniqueName(), TaskManager.TaskGroup);
                    context.LogInformation($" DataMap: invalid data");
                    return;
                }
                var discountByTimeId = dataStr.Split(",")[0].TryParseInt(true);
                if (!discountByTimeId.HasValue)
                {
                    TaskManager.TaskManagerInstance().RemoveTask(jobData.GetUniqueName(), TaskManager.TaskGroup);
                    context.LogInformation($" DiscountByTime: invalid Id");
                    return;
                }
                var discountByTime = DiscountByTimeService.Get(discountByTimeId.Value);
                if (discountByTime == null)
                {
                    TaskManager.TaskManagerInstance().RemoveTask(jobData.GetUniqueName(), TaskManager.TaskGroup);
                    context.LogInformation($" DiscountByTime: {discountByTimeId.Value} not found");
                    return;
                }
                var currentDiscounts = DiscountByTimeService.GetCurrentDiscountsByTime();
                var currentActiveByTimeCategories = new HashSet<int>();
                foreach (var currentDiscount in currentDiscounts)
                {
                    if (currentDiscount.Id == discountByTime.Id)
                        continue;
                    currentDiscount.ActiveByTimeCategories.ForEach(x => currentActiveByTimeCategories.Add(x.CategoryId));
                }
                var dateTimeNow = DateTime.Now;
                if (discountByTime.Enabled)
                    // меняем активность у категорий, которые должны быть активны на время скидки, но не трогаем категории, которые также есть и в другой активной скидке 
                    DiscountByTimeService.ChangeCategoryEnabled(
                        DiscountByTimeService.GetDiscountCategories(discountByTime.Id, activeByTime: true)
                            .Where(x => !currentActiveByTimeCategories.Contains(x.CategoryId))
                            .Select(x => x.CategoryId)
                            .ToList(),
                        dateTimeNow.TimeOfDay >= discountByTime.TimeFrom && dateTimeNow.TimeOfDay < discountByTime.TimeTo && discountByTime.DaysOfWeek.Contains(dateTimeNow.DayOfWeek));
            }
            catch (Exception ex)
            {
                context.LogError(ex.Message);
                Debug.Log.Error(ex);
            }

            context.WriteLastRun();
        }
    }
}