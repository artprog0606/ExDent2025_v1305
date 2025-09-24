using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Core.Services.TemplatesDocx.Templates;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Mails;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Core.Services.Partners
{
    public class PartnerReportService
    {
        #region enums

        private enum EPartnersReportColumnType
        {
            [Localize("Партнер")]
            Name,
            [Localize("Email")]
            Email,
            [Localize("Дата регистрации")]
            DateCreated,
            [Localize("Номер телефона")]
            Phone,
            [Localize("Город")]
            City,
            [Localize("Код купона")]
            CouponCode,
            [Localize("Тип")]
            Type,
            [Localize("Договор")]
            Contract,
            [Localize("Баланс")]
            Balance,
            [Localize("Кол-во клиентов, привлеченных за период")]
            ClientsCount,
            [Localize("Сумма начислений за период")]
            RewardsSum,
            [Localize("Сумма платежей клиентов за период")]
            PaymentsSum,
        }

        private enum EPartnersRewardsReportColumnType
        {
            [Localize("Партнер")]
            Name,
            [Localize("Email")]
            Email,
            [Localize("Дата регистрации")]
            DateCreated,
            [Localize("Номер телефона")]
            Phone,
            [Localize("Город")]
            City,
            [Localize("Тип")]
            Type,
            [Localize("Договор")]
            Contract,
            [Localize("Способ выплаты")]
            PaymentType,
            [Localize("№ счета")]
            PaymentAccountNumber,
            [Localize("Период выплаты, от")]
            PeriodFrom,
            [Localize("Баланс")]
            Balance,
            [Localize("Кол-во клиентов")]
            ClientsCount,
            [Localize("Сумма вознаграждения")]
            RewardsSum,
            [Localize("Сумма платежей клиентов за период выплаты")]
            PaymentsSum,
            [Localize("Комментарий")]
            Comment,
        }

        #endregion

        public static string GeneratePartnersReport(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var fileName = string.Format("partners{0}{1}.csv",
                (dateFrom.HasValue ? "_from" + dateFrom.Value.ToString("yyyyddMM") : string.Empty),
                (dateTo.HasValue ? "_to" + dateTo.Value.ToString("yyyyddMM") : string.Empty));
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp, fileName);

            using (var writer = new CsvWriter(new StreamWriter(filePath, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                writer.WriteField("Отчет по партнерам " +
                    (dateFrom.HasValue || dateTo.HasValue 
                        ? string.Format("{0} {1}", dateFrom.HasValue ? "с " + dateFrom.Value.ToString("dd.MM.yyy") : string.Empty, dateTo.HasValue ? " до " + dateTo.Value.ToString("dd.MM.yyy") : string.Empty).Trim()
                        : "за все время"));
                writer.NextRecord();

                // header
                foreach (EPartnersReportColumnType columnType in Enum.GetValues(typeof(EPartnersReportColumnType)))
                    writer.WriteField(columnType.Localize());
                writer.NextRecord();

                // rows
                foreach (var partner in PartnerService.GetPartners().OrderBy(x => x.DateCreated))
                {
                    try
                    {
                        var coupon = partner.CouponId.HasValue ? CouponService.GetCoupon(partner.CouponId.Value) : null;

                        foreach (EPartnersReportColumnType columnType in Enum.GetValues(typeof(EPartnersReportColumnType)))
                        {
                            switch (columnType)
                            {
                                case EPartnersReportColumnType.Name:
                                    writer.WriteField(partner.Name);
                                    break;
                                case EPartnersReportColumnType.Email:
                                    writer.WriteField(partner.Email);
                                    break;
                                case EPartnersReportColumnType.DateCreated:
                                    writer.WriteField(partner.DateCreated.ToString("dd.MM.yy HH:mm"));
                                    break;
                                case EPartnersReportColumnType.Phone:
                                    writer.WriteField(partner.Phone);
                                    break;
                                case EPartnersReportColumnType.City:
                                    writer.WriteField(partner.City);
                                    break;
                                case EPartnersReportColumnType.CouponCode:
                                    writer.WriteField(coupon != null ? coupon.Code : string.Empty);
                                    break;
                                case EPartnersReportColumnType.Type:
                                    writer.WriteField(partner.Type.Localize());
                                    break;
                                case EPartnersReportColumnType.Contract:
                                    writer.WriteField(partner.ContractConcluded && partner.ContractNumber.IsNotEmpty()
                                        ? string.Format("№{0}", partner.ContractNumber) + (partner.ContractDate.HasValue ? " от " + partner.ContractDate.Value.ToString("dd.MM.yy") : string.Empty)
                                        : string.Empty);
                                    break;
                                case EPartnersReportColumnType.Balance:
                                    var balance = partner.Balance.RoundConvertToDefault();
                                    writer.WriteField(balance.ToString());
                                    break;
                                case EPartnersReportColumnType.ClientsCount:
                                    var clientsCount = PartnerService.GetBindedCustomersCount(partner.Id, dateFrom, dateTo);
                                    writer.WriteField(clientsCount.ToString());
                                    break;
                                case EPartnersReportColumnType.RewardsSum:
                                    var rewards = TransactionService.GetRewardAmount(partner.Id, dateFrom, dateTo).RoundConvertToDefault();
                                    writer.WriteField(rewards.ToString());
                                    break;
                                case EPartnersReportColumnType.PaymentsSum:
                                    var payments = TransactionService.GetPaidOrderItemsSum(partner.Id, dateFrom, dateTo).RoundConvertToDefault();
                                    writer.WriteField(payments.ToString());
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
            }
            return filePath;
        }

        public static PayoutReport GeneratePartnersPayoutReport()
        {
            var dateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);

            var fileName = string.Format("payoutreport_{0}", dateTo.ToString("yyyyddMM"));
            var existFiles = Directory.GetFiles(FoldersHelper.GetPathAbsolut(FolderType.PartnerPayoutReports), fileName + "*.csv").ToList();
            var triesCount = 0;
            var newName = fileName;
            while (existFiles.Any(x => x.Contains(newName)))
            {
                newName = string.Format("{0}_{1}", fileName, triesCount + 1);
                triesCount++;
                if (triesCount > 100)
                    return null;
            }
            fileName = newName + ".csv";
            var filePath = FoldersHelper.GetPathAbsolut(FolderType.PartnerPayoutReports, fileName);

            int partnersCount = 0;

            using (var writer = new CsvWriter(new StreamWriter(filePath, false, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
            {
                writer.WriteField("Отчет по выплатам партнерам за " + dateTo.ToString("MMMM yyyy"));
                writer.NextRecord();

                // header
                foreach (EPartnersRewardsReportColumnType columnType in Enum.GetValues(typeof(EPartnersRewardsReportColumnType)))
                    writer.WriteField(columnType.Localize());
                writer.NextRecord();

                // rows
                foreach (var partner in PartnerService.GetPartners().OrderBy(x => x.DateCreated))
                {
                    try
                    {
                        var lastRewardPayout = TransactionService.GetLastRewardPayout(partner.Id);
                        var dateFrom = lastRewardPayout != null && lastRewardPayout.RewardPeriodTo.HasValue
                            ? lastRewardPayout.RewardPeriodTo.Value.AddDays(1)
                            : partner.DateCreated;

                        var rewards = TransactionService.GetRewardAmount(partner.Id, dateFrom, dateTo).RoundConvertToDefault();
                        if (rewards < SettingsPartners.PayoutMinBalance)
                            continue;
                        var clientsCount = PartnerService.GetBindedCustomersCount(partner.Id);
                        if (clientsCount < SettingsPartners.PayoutMinCustomersCount)
                            continue;

                        var naturalPerson = partner.Type == EPartnerType.NaturalPerson ? PartnerService.GetNaturalPerson(partner.Id) : null;

                        foreach (EPartnersRewardsReportColumnType columnType in Enum.GetValues(typeof(EPartnersRewardsReportColumnType)))
                        {
                            switch (columnType)
                            {
                                case EPartnersRewardsReportColumnType.Name:
                                    writer.WriteField(partner.Name);
                                    break;
                                case EPartnersRewardsReportColumnType.Email:
                                    writer.WriteField(partner.Email);
                                    break;
                                case EPartnersRewardsReportColumnType.DateCreated:
                                    writer.WriteField(partner.DateCreated.ToString("dd.MM.yy HH:mm"));
                                    break;
                                case EPartnersRewardsReportColumnType.Phone:
                                    writer.WriteField(partner.Phone);
                                    break;
                                case EPartnersRewardsReportColumnType.City:
                                    writer.WriteField(partner.City);
                                    break;
                                case EPartnersRewardsReportColumnType.Type:
                                    writer.WriteField(partner.Type.Localize());
                                    break;
                                case EPartnersRewardsReportColumnType.Contract:
                                    writer.WriteField(partner.ContractConcluded && partner.ContractNumber.IsNotEmpty()
                                        ? string.Format("№{0}", partner.ContractNumber) + (partner.ContractDate.HasValue ?  " от " + partner.ContractDate.Value.ToString("dd.MM.yy") : string.Empty)
                                        : string.Empty);
                                    break;
                                case EPartnersRewardsReportColumnType.PaymentType:
                                    var paymentType = naturalPerson != null && naturalPerson.PaymentTypeId.HasValue
                                        ? PaymentTypeService.GetPaymentType(naturalPerson.PaymentTypeId.Value)
                                        : null;
                                    writer.WriteField(paymentType != null ? paymentType.Name : null);
                                    break;
                                case EPartnersRewardsReportColumnType.PaymentAccountNumber:
                                    writer.WriteField(naturalPerson != null ? naturalPerson.PaymentAccountNumber + "\t" : null); // ms excel long numbers
                                    break;
                                case EPartnersRewardsReportColumnType.PeriodFrom:
                                    writer.WriteField(dateFrom.ToString("dd.MM.yy"));
                                    break;
                                case EPartnersRewardsReportColumnType.Balance:
                                    var balance = partner.Balance.RoundConvertToDefault();
                                    writer.WriteField(balance.ToString());
                                    break;
                                case EPartnersRewardsReportColumnType.ClientsCount:
                                    writer.WriteField(clientsCount.ToString());
                                    break;
                                case EPartnersRewardsReportColumnType.RewardsSum:
                                    writer.WriteField(rewards.ToString());
                                    break;
                                case EPartnersRewardsReportColumnType.PaymentsSum:
                                    var payments = TransactionService.GetPaidOrderItemsSum(partner.Id, dateFrom, dateTo).RoundConvertToDefault();
                                    writer.WriteField(payments.ToString());
                                    break;
                                case EPartnersRewardsReportColumnType.Comment:
                                    writer.WriteField(partner.AdminComment);
                                    break;
                            }
                        }
                        writer.NextRecord();

                        partnersCount++;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }
            }

            var payoutReport = new PayoutReport
            {
                FileName = fileName,
                PeriodTo = dateTo
            };
            PayoutReportService.AddPayoutReport(payoutReport);

            if (partnersCount > 0)
            {
                var subject = $"Сформирован отчет по выплатам партнерам за {dateTo:MMMM yyyy}";
                var text = string.Format("<p>{0}.</p><p>Загрузить и просмотреть отчет вы можете, перейдя по ссылке <a href=\"{1}\">{1}</a></p>",
                    subject, UrlService.GetAdminUrl("partnersreports/payoutreport/" + payoutReport.Id));
                // todo: send report file
                MailService.SendMailNow(Guid.Empty, SettingsMail.EmailForPartners, subject, text, true, logging: false);
            }
            return payoutReport;
        }

        public static void SendMonthReports()
        {
            var now = DateTime.Now;
            var dateFrom = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
            var dateTo = new DateTime(now.Year, now.Month, 1).AddDays(-1);

            foreach (var partner in PartnerService.GetPartners())
            {
                if (!partner.SendMessages.HasFlag(EPartnerMessageType.MonthReport))
                    continue;

                try
                {
                    var customerSount = PartnerService.GetBindedCustomersCount(partner.Id, dateFrom, dateTo);
                    var customersTotalCount = PartnerService.GetBindedCustomersCount(partner.Id);
                    var rewardsSum = TransactionService.GetRewardAmount(partner.Id, dateFrom, dateTo);
                    var rewardsTotalSum = TransactionService.GetRewardAmount(partner.Id);
                    var paymentsSum = TransactionService.GetPaidOrderItemsSum(partner.Id, dateFrom, dateTo);
                    var paymentsTotalSum = TransactionService.GetPaidOrderItemsSum(partner.Id);

                    var mail = new PartnerMonthReportMailTemplate(partner,
                        dateFrom.ToString("MMMM yyyy"),
                        customerSount, customersTotalCount,
                        rewardsSum.FormatRoundPriceDefault(), rewardsTotalSum.FormatRoundPriceDefault(),
                        paymentsSum.FormatRoundPriceDefault(), paymentsTotalSum.FormatRoundPriceDefault());
                    MailService.SendMailNow(partner.Email, mail);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        #region ActReports

        public const string ActReportTplDocxLegalEntityDefault = "PartnerAct_LegalEntity.docx";
        public const string ActReportTplDocxNaturalPersonDefault = "PartnerAct_NaturalPerson.docx";

        public static void GenerateAndSendActReports()
        {
            foreach (var partnerId in PartnerService.GetPartnerIds())
            {
                try
                {
                    GenerateActReport(partnerId, true, force: false);
                }
                catch (BlException) { }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        public static ActReport GenerateActReport(int partnerId, bool sendMail, bool force = true)
        {
            var partner = PartnerService.GetPartner(partnerId);
            if (partner == null)
                throw new BlException("Партнер не найден");

            var dateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            var lastRewardPayout = TransactionService.GetLastRewardPayout(partner.Id);
            var dateFrom = lastRewardPayout != null && lastRewardPayout.RewardPeriodTo.HasValue
                ? lastRewardPayout.RewardPeriodTo.Value.AddDays(1)
                : partner.DateCreated;

            if (!force && (TransactionService.GetRewardAmount(partner.Id, dateFrom, dateTo).RoundConvertToDefault() < SettingsPartners.PayoutMinBalance ||
                PartnerService.GetBindedCustomersCount(partner.Id) < SettingsPartners.PayoutMinCustomersCount))
            {
                throw new BlException("Партнер не выполнил условия для выплаты вознаграждения");
            }

            var periodFormatted = dateTo.ToString("MMMM yyyy");

            if (dateFrom >= dateTo)
                throw new BlException(string.Format("{0} {1}", 
                    (lastRewardPayout != null ? "Партнеру уже выплачено вознаграждение за " : "Партнер зарегистрирован позже отчетного периода: "),
                    periodFormatted));

            if (ActReportService.ActReportExists(partnerId, dateFrom, dateTo))
                throw new BlException(string.Format("Акт-отчет за {0} уже сформирован", periodFormatted));

            string generateFileName = null;

            decimal rewardsSum = 0;

            switch (partner.Type)
            {
                case EPartnerType.LegalEntity:
                    var legalEntityActTpl = new PartnerLegalEntityActTemplate(partner);
                    if (partner.LegalEntity != null)
                    {
                        legalEntityActTpl.Director = partner.LegalEntity.Director;
                        legalEntityActTpl.Accountant = partner.LegalEntity.Accountant;
                    }
                    generateFileName = GenerateActReport(legalEntityActTpl, partner, dateFrom, dateTo, out rewardsSum);
                    break;
                case EPartnerType.NaturalPerson:
                    var naturalPersonActTpl = new PartnerNaturalPersonActTemplate(partner);
                    if (partner.NaturalPerson != null)
                    {
                        naturalPersonActTpl.ShortName = partner.NaturalPerson.LastName.IsNotEmpty()
                            ? new List<string> { partner.NaturalPerson.FirstName, partner.NaturalPerson.Patronymic }
                                .Where(x => x.IsNotEmpty()).Select(x => x[0] + ". ").AggregateString() + partner.NaturalPerson.LastName
                            : partner.Name;
                    }
                    generateFileName = GenerateActReport(naturalPersonActTpl, partner, dateFrom, dateTo, out rewardsSum);
                    break;
            }

            if (rewardsSum <= 0)
                throw new BlException("Нет начислений за " + periodFormatted);

            if (generateFileName.IsNullOrEmpty())
                throw new BlException("Не удалось сформировать акт-отчет");

            var actReport = new ActReport
            {
                PartnerId = partner.Id,
                PeriodFrom = dateFrom,
                PeriodTo = dateTo,
                FileName = generateFileName
            };
            ActReportService.AddActReport(actReport);

            if (sendMail)
                SendActReportMail(partner, periodFormatted, rewardsSum, UrlService.GetUrl("partners/reports/actreport/" + actReport.Id));

            return actReport;
        }

        private static string GenerateActReport<T>(T actTpl, Partner partner, DateTime periodFrom, DateTime periodTo, out decimal rewardsSum)
            where T: BasePartnerActTemplate
        {
            var tplFile = FoldersHelper.GetPathAbsolut(FolderType.TemplateDocx, GetActReportTplDocx(partner.Type));
            if (!File.Exists(tplFile))
                throw new BlException(string.Format("Шаблон акт-отчета партнера не установлен. <a href=\"settingspartners#?partnerstab=reports\">Установить</a>"));

            actTpl.PeriodFrom = periodFrom;
            actTpl.PeriodTo = periodTo;

            var transctionsGroups = TransactionService.GetTransactions(partner.Id, periodFrom, periodTo).Where(x => !x.IsRewardPayout)
                .GroupBy(x => x.CustomerId)
                .ToDictionary(x => x.Key.HasValue ? x.Key.Value : Guid.Empty, x => x.ToList());

            foreach (var customerId in transctionsGroups.Keys)
            {
                var customer = customerId != Guid.Empty ? CustomerService.GetCustomer(customerId) : null;

                var sum = transctionsGroups[customerId].Sum(x => x.RoundedBaseAmount).RoundConvertToDefault();
                if (sum == 0)
                    continue;

                var bindedCustomer = customerId != Guid.Empty ? PartnerService.GetBindedCustomer(customerId) : null;
                var item = new PartnerCustomerRewardTemplate
                {
                    Email = customerId != Guid.Empty 
                        ? customer != null ? customer.EMail.AnonimizeEmail() : "Нет данных"
                        : "Прочие начисления/списания",
                    Phone = customer != null ? customer.Phone.AnonimizePhone() : null,
                    DateBinded = bindedCustomer != null && bindedCustomer.PartnerId == partner.Id ? bindedCustomer.DateCreated : (DateTime?)null,
                    RewardSum = sum,
                    PaymentSum = customerId != Guid.Empty ? TransactionService.GetPaidOrderItemsSum(partner.Id, periodFrom, periodTo, customerId).RoundConvertToDefault() : (decimal?)null
                };

                actTpl.Items.Add(item);
            }

            //var bindedCustomers = PartnerService.GetBindedCustomers(partner.Id, periodFrom, periodTo);
            //foreach (var bindedCustomer in bindedCustomers.OrderBy(x => x.DateCreated))
            //{
            //    try
            //    {
            //        Customer customer;
            //        decimal rewardSum;

            //        if ((customer = CustomerService.GetCustomer(bindedCustomer.CustomerId)) == null ||
            //            (rewardSum = TransactionService.GetRewardAmount(partner.Id, periodFrom, periodTo, customer.Id).RoundConvertToDefault()) <= 0)
            //            continue;

            //        var item = new PartnerCustomerRewardTemplate
            //        {
            //            Email = customer.EMail.AnonimizeEmail(),
            //            Phone = customer.Phone.AnonimizePhone(),
            //            DateBinded = bindedCustomer.DateCreated,
            //            RewardSum = rewardSum,
            //            PaymentSum = TransactionService.GetPaidOrderItemsSum(partner.Id, periodFrom, periodTo, customer.Id).RoundConvertToDefault()
            //        };

            //        actTpl.Items.Add(item);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.Log.Error(ex);
            //    }
            //}

            rewardsSum = actTpl.Items.Sum(x => x.RewardSum);
            if (rewardsSum > 0)
            {
                actTpl.RewardSum = rewardsSum;

                var generateFileName = Guid.NewGuid() + Path.GetExtension(tplFile);
                var generateFile = FoldersHelper.GetPathAbsolut(FolderType.PartnerActReports, generateFileName);
                File.Copy(tplFile, generateFile);

                TemplatesDocxServices.TemplateFillContent(generateFile, actTpl);
                return generateFileName;
            }

            return null;
        }

        public static string GetActReportTplDocx(EPartnerType type, bool useDefault = true)
        {
            switch (type)
            {
                case EPartnerType.LegalEntity:
                    return SettingsPartners.ActReportTplDocxLegalEntity.Default(useDefault ? ActReportTplDocxLegalEntityDefault : null);
                case EPartnerType.NaturalPerson:
                    return SettingsPartners.ActReportTplDocxNaturalPerson.Default(useDefault ? ActReportTplDocxNaturalPersonDefault : null);
                default:
                    throw new NotImplementedException();
            }
        }

        public static void SetActReportTplDocx(EPartnerType type, string fileName)
        {
            switch (type)
            {
                case EPartnerType.LegalEntity:
                    SettingsPartners.ActReportTplDocxLegalEntity = fileName;
                    return;
                case EPartnerType.NaturalPerson:
                    SettingsPartners.ActReportTplDocxNaturalPerson = fileName;
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void SendActReportMail(Partner partner, string rewardPeriod, decimal rewardsSum, string actLink)
        {
            switch (partner.Type)
            {
                case EPartnerType.LegalEntity:
                    MailService.SendMailNow(Guid.Empty, partner.Email,
                        new PartnerLegalEntityActReportMailTemplate(partner, rewardPeriod,
                            rewardsSum.FormatRoundPriceDefault(), actLink), logging: false);
                    break;
                case EPartnerType.NaturalPerson:
                    MailService.SendMailNow(Guid.Empty, partner.Email,
                        new PartnerNaturalPersonActReportMailTemplate(partner, rewardPeriod,
                            rewardsSum.FormatRoundPriceDefault(), actLink), logging: false);
                    break;
            }
        }


        #endregion
    }
}
