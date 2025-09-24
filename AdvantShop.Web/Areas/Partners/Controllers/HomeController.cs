using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.Partners.ViewModels.Home;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Shared.Common;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Areas.Partners.Controllers
{
    public class HomeController : BasePartnerController
    {
        public ActionResult Index()
        {
            SetMetaInformation("Личный кабинет партнера");
            SetNgController(NgControllers.NgControllersTypes.PartnerHomeCtrl);

            var partner = PartnerContext.CurrentPartner;

            var model = new HomeViewModel
            {
                Coupon = partner.CouponId.HasValue ? CouponService.GetCoupon(partner.CouponId.Value) : null,
                ReferralRequestParam = PartnerService.ReferralRequestParam,
                Balance = partner.Balance.FormatRoundPriceDefault(),
                RewardPercent = partner.RewardPercent.ToString(),
                RewardsSum = TransactionService.GetRewardPayoutSum(partner.Id).FormatRoundPriceDefault(),
                OrderItemsSum = TransactionService.GetPaidOrderItemsSum(partner.Id).FormatRoundPriceDefault(),
                CustomersCount = PartnerService.GetBindedCustomersCount(partner.Id),
                AllowPayoutRewards = partner.Balance != 0 && !string.IsNullOrEmpty(SettingsPartners.EmailNotification)
            };

            return View(model);
        }

        public JsonResult GetAccrualChartData(string from, string to)
        {
            var dateFrom = from.TryParseDateTime(isNullable: true);
            var dateTo = to.TryParseDateTime(isNullable: true);
            if (!dateFrom.HasValue || !dateTo.HasValue || dateTo.Value < dateFrom.Value)
                return JsonError();

            var transactions = TransactionService.GetTransactions(PartnerContext.CurrentPartner.Id, dateFrom.Value, dateTo.Value).Where(x => x.Amount > 0);
            var transactionsByDates = transactions.GroupBy(x => x.DateCreated.Date)
                .ToDictionary(x => x.Key, x=> x.Sum(tr => tr.RoundedBaseAmount));

            var result = new Dictionary<DateTime, decimal>();
            var date = dateFrom.Value;
            while (date <= dateTo)
            {
                result.Add(date, transactionsByDates.ContainsKey(date) ? transactionsByDates[date] : 0);
                date = date.AddDays(1).Date;
            }

            return JsonOk(new
            {
                sum = transactions.Sum(x => x.RoundedBaseAmount).FormatRoundPriceDefault(),
                chartData = new ChartDataJsonModel
                {
                    Data = new List<object>() { result.Values },
                    Labels = result.Keys.Select(x => x.ToString("d MMM")).ToList(),
                    Series = new List<string> { "Начислено" }
                }
            });
        }

        public JsonResult SendMailPayoutRewards()
        {
            var email = SettingsPartners.EmailNotification;
            var partner = PartnerContext.CurrentPartner;
            var text = $"Партнер {partner.Name} ({partner.Email}) запрашивает вывод партнерского вознаграждения.";
            
            var result = MailService.SendMailNow(Guid.Empty, email, "Партнерская программа", text,false, logging: false);
            
            return result ? JsonOk() : JsonError();
        }
    }
}