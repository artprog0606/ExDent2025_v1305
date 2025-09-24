using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AdvantShop.Areas.Partners.Handlers.Customers;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.Controllers;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Areas.Partners.Controllers
{
    public class CustomersController : BasePartnerController
    {
        #region enums

        private enum ECustomersReportColumnType
        {
            [Localize("Email")]
            Email,
            [Localize("Телефон")]
            Phone,
            [Localize("Сумма начислений")]
            RewardsSum,
            [Localize("Сумма платежей")]
            PaymentsSum,
            [Localize("Дата привязки")]
            DateCreated,
            [Localize("Источник")]
            Source,
            [Localize("referer")]
            Referer,
            [Localize("utm_source")]
            UtmSource,
            [Localize("utm_medium")]
            UtmMedium,
            [Localize("utm_campaign")]
            UtmCampaign,
            [Localize("utm_content")]
            UtmContent,
            [Localize("utm_term")]
            UtmTerm,
        }

        #endregion

        public ActionResult Index(int? page)
        {
            SetMetaInformation("Клиенты - Личный кабинет партнера");
            SetNgController(NgControllers.NgControllersTypes.PartnerCustomersCtrl);

            var model = new GetCustomersHandler(page).Get();
            if ((model.Pager.TotalPages < model.Pager.CurrentPage && model.Pager.CurrentPage > 1) || model.Pager.CurrentPage < 0)
                return Error404();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetReport(string from, string to)
        {
            var dateFrom = from.TryParseDateTime(isNullable: true);
            var dateTo = to.TryParseDateTime(isNullable: true);
            if (!dateFrom.HasValue || !dateTo.HasValue || dateTo.Value < dateFrom.Value)
            {
                ShowMessage(Core.Controls.NotifyType.Error, "Укажите даты");
                return RedirectToAction("Index");
            }

            var partner = PartnerContext.CurrentPartner;

            using (var outputStream = new MemoryStream())
            using (var writer = new CsvWriter(new StreamWriter(outputStream, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                writer.WriteField($"Отчет по клиентам с {dateFrom.Value:dd.MM.yyyy} до {dateTo.Value:dd.MM.yyyy}");
                writer.NextRecord();

                // header
                foreach (ECustomersReportColumnType columnType in Enum.GetValues(typeof(ECustomersReportColumnType)))
                    writer.WriteField(columnType.Localize());
                writer.NextRecord();

                // rows
                var bindedCustomers = PartnerService.GetBindedCustomers(partner.Id, dateFrom, dateTo);
                foreach (var bindedCustomer in bindedCustomers.OrderBy(x => x.DateCreated))
                {
                    try
                    {
                        var customer = CustomerService.GetCustomer(bindedCustomer.CustomerId);
                        if (customer == null)
                            continue;

                        foreach (ECustomersReportColumnType columnType in Enum.GetValues(typeof(ECustomersReportColumnType)))
                        {
                            switch (columnType)
                            {
                                case ECustomersReportColumnType.Email:
                                    writer.WriteField(customer.EMail.AnonimizeEmail());
                                    break;
                                case ECustomersReportColumnType.Phone:
                                    writer.WriteField(customer.Phone.AnonimizePhone());
                                    break;
                                case ECustomersReportColumnType.RewardsSum:
                                    var rewards = TransactionService.GetRewardAmount(partner.Id, dateFrom, dateTo, customer.Id).RoundConvertToDefault();
                                    writer.WriteField(rewards.ToString());
                                    break;
                                case ECustomersReportColumnType.PaymentsSum:
                                    var payments = TransactionService.GetPaidOrderItemsSum(partner.Id, dateFrom, dateTo, customer.Id).RoundConvertToDefault();
                                    writer.WriteField(payments.ToString());
                                    break;
                                case ECustomersReportColumnType.DateCreated:
                                    writer.WriteField(bindedCustomer.DateCreated.ToString("dd.MM.yyyy HH:mm"));
                                    break;
                                case ECustomersReportColumnType.Source:
                                    if (bindedCustomer.CouponCode.IsNotEmpty())
                                        writer.WriteField("Купон " + bindedCustomer.CouponCode);
                                    else if (bindedCustomer.Url.IsNotEmpty())
                                        writer.WriteField("Реферальная ссылка " + bindedCustomer.Url);
                                    else
                                        writer.WriteField(null);
                                    break;
                                case ECustomersReportColumnType.Referer:
                                    writer.WriteField(bindedCustomer.UrlReferrer);
                                    break;
                                case ECustomersReportColumnType.UtmSource:
                                    writer.WriteField(bindedCustomer.UtmSource);
                                    break;
                                case ECustomersReportColumnType.UtmMedium:
                                    writer.WriteField(bindedCustomer.UtmMedium);
                                    break;
                                case ECustomersReportColumnType.UtmCampaign:
                                    writer.WriteField(bindedCustomer.UtmCampaign);
                                    break;
                                case ECustomersReportColumnType.UtmContent:
                                    writer.WriteField(bindedCustomer.UtmContent);
                                    break;
                                case ECustomersReportColumnType.UtmTerm:
                                    writer.WriteField(bindedCustomer.UtmTerm);
                                    break;
                            }
                        }
                        writer.NextRecord();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }

                writer.Dispose();
                return File(outputStream.ToArray(), "application/octet-stream", string.Format("report_{0}_{1}.csv", dateFrom.Value.ToString("yyyyMMdd"), dateTo.Value.ToString("yyyyMMdd")));
            }
        }
    }
}