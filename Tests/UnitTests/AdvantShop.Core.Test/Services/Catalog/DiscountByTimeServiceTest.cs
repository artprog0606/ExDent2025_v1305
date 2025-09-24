using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using NUnit.Framework;

namespace AdvantShop.Core.Test.Services.Catalog
{
    public class DiscountByTimeServiceTest
    {
        //[Test]
        // [TestCase(11232.76f, 1, 11232.76f, 1f, false, 0.01f, 1f)]
        // [TestCase(11232.76f, 6, 67396.56f, 1f, false, 0.01f, 1f)]
        public void GetCacheMinutes_ShouldBeEqual()
        {
            /*
            var discount = new DiscountByTime()
            {
                TimeFrom = TimeSpan.FromHours(20), TimeTo = TimeSpan.FromHours(10), DaysOfWeek =
                {
                    DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday,
                    DayOfWeek.Saturday, DayOfWeek.Sunday
                }
            };
            var now = new DateTime(2024, 09, 03, 09, 00, 00);
            
            // скидка c 20 до 10, сейчас 9 утра
            var shouldBeEqual = DiscountByTimeService.GetCacheMinutes(discount, now) == 1*60;
            
            now = new DateTime(2024, 09, 02, 23, 00, 00);
            
            // скидка c 20 до 10, сейчас 23 вечера
            shouldBeEqual = DiscountByTimeService.GetCacheMinutes(discount, now) == 11*60;
            
            discount.TimeFrom = TimeSpan.FromHours(10);
            discount.TimeTo = TimeSpan.FromHours(20);
            now = new DateTime(2024, 09, 03, 15, 00, 00);
            
            // скидка c 10 до 20, сейчас 15
            shouldBeEqual = DiscountByTimeService.GetCacheMinutes(discount, now) == 5*60;
            */
            
            // пробовал Pose, Smocks (не работают и не поддерживаются), telerik - только в платном варианте, Microsoft Fakes - только vs 
            
            // var shim = Shim.Replace(
            //     () => DiscountByTimeService.GetList(Pose.Is.A<bool>())).With((bool enabled) => new List<DiscountByTime>());
            //
            // PoseContext.Isolate(() =>
            // {
            //     //var discounts = DiscountByTimeService.GetByDatetime(new DateTime(2024, 09, 02, 22, 0, 0));
            //     var list = DiscountByTimeService.GetList();
            //
            //     Assert.AreEqual(new List<DiscountByTime>(), list);
            // }, shim);
            
            // Mock.Arrange(() => StaticClass.StaticMethod()).Returns("Mocked Result");
            //
            // // Вызов метода, который должен быть замещен
            // string result = StaticClass.StaticMethod();
            //
            // // Проверяем, что вернулось замещенное значение
            // Assert.AreEqual("Mocked Result", result);
            
            // Smock.Run(context =>
            // {
            //     context.Setup(() => DateTime.Now).Returns(new DateTime(2000, 1, 1));
            //
            //     // Outputs "2000"
            //     Console.WriteLine(DateTime.Now.Year);
            //     
            //     // context
            //     //     .Setup(() => DiscountByTimeService.GetList(It.IsAny<bool>()))
            //     //     .Returns(new List<DiscountByTime>());
            //     //
            //     // var list = DiscountByTimeService.GetList();
            //
            //     //Assert.AreEqual(new List<DiscountByTime>(), list);
            // });
        }

        //[Test]
        public void GetByDatetime_GetCacheMinutes()
        {
            // 1 test case: скидка c 20 до 10, сейчас 9 утра
            var discounts = new List<DiscountByTime>
            {
                new DiscountByTime()
                {
                    TimeFrom = TimeSpan.FromHours(20), TimeTo = TimeSpan.FromHours(10), DaysOfWeek =
                    {
                        DayOfWeek.Monday, 
                        DayOfWeek.Tuesday, 
                        DayOfWeek.Wednesday, 
                        DayOfWeek.Thursday, 
                        DayOfWeek.Friday,
                        DayOfWeek.Saturday, 
                        DayOfWeek.Sunday
                    }
                }
            };
            var now = new DateTime(2024, 09, 03, 09, 00, 00);

            var result = GetDiscounts(discounts, now);

            var shouldBeEqual1 = result.Count == 1;
            
            // 2 test case: скидка c 20 до 10, сейчас 9 утра, но утро другого дня не стоит у скидки
            discounts = new List<DiscountByTime>
            {
                new DiscountByTime()
                {
                    TimeFrom = TimeSpan.FromHours(20), TimeTo = TimeSpan.FromHours(10), DaysOfWeek =
                    {
                        DayOfWeek.Monday, 
                    }
                }
            };
            
            var result2 = GetDiscounts(discounts, now);
            
            var shouldBeEqual2 = result2.Count == 0;
            
            // 3 test case: скидка c 20 до 10, сейчас 23 вечера
            discounts = new List<DiscountByTime>
            {
                new DiscountByTime()
                {
                    TimeFrom = TimeSpan.FromHours(20), TimeTo = TimeSpan.FromHours(10), DaysOfWeek =
                    {
                        DayOfWeek.Monday, 
                    }
                }
            };
            now = new DateTime(2024, 09, 02, 23, 00, 00);
            
            var result3 = GetDiscounts(discounts, now);
            
            var shouldBeEqual3 = result3.Count == 1;
            
            // 4 test case: скидка c 10 до 20, сейчас 15
            discounts = new List<DiscountByTime>
            {
                new DiscountByTime()
                {
                    TimeFrom = TimeSpan.FromHours(10), TimeTo = TimeSpan.FromHours(20), DaysOfWeek =
                    {
                        DayOfWeek.Monday, 
                    }
                }
            };
            now = new DateTime(2024, 09, 02, 15, 00, 00);
            
            var result4 = GetDiscounts(discounts, now);
            
            var shouldBeEqual4 = result4.Count == 1;
            
            // 5 test case: скидка c 10 до 20, сейчас 09
            discounts = new List<DiscountByTime>
            {
                new DiscountByTime()
                {
                    TimeFrom = TimeSpan.FromHours(10), TimeTo = TimeSpan.FromHours(20), DaysOfWeek =
                    {
                        DayOfWeek.Monday, 
                    }
                }
            };
            now = new DateTime(2024, 09, 02, 09, 00, 00);
            
            var result5 = GetDiscounts(discounts, now);
            
            var shouldBeEqual5 = result5.Count == 0;
        }
        
        private List<DiscountByTime> GetDiscounts(List<DiscountByTime> discountByTimes, DateTime dateTime)
        {
            var timeOfDay = dateTime.TimeOfDay;
            
            return discountByTimes.Where(x => 
                    (x.TimeTo > x.TimeFrom 
                        ? x.TimeFrom <= timeOfDay && timeOfDay <= x.TimeTo  // 09 <= time <= 18 
                        : x.TimeFrom <= timeOfDay || timeOfDay <= x.TimeTo) // 22 <= time <= 09
                        
                    && x.DaysOfWeek.Contains(dateTime.DayOfWeek))
                .ToList();
        }
    }
}