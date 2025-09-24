using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public abstract class BasePartnerTemplate : BaseTemplate
    {
        public BasePartnerTemplate() { }

        public BasePartnerTemplate(Partner partner)
        {
            var coupon = partner.CouponId.HasValue ? CouponService.GetCoupon(partner.CouponId.Value) : null;

            Name = partner.Name;
            Email = partner.Email;
            Phone = partner.Phone;
            City = partner.City;
            CouponCode = coupon != null ? coupon.Code : null;
            Balance = partner.Balance;
            RewardPercent = partner.RewardPercent;
            Type = partner.Type;
            RegistrationDate = partner.DateCreated;
            ContractNumber = partner.ContractNumber;
            ContractDate = partner.ContractDate;
        }

        public abstract string Name { get; set; }

        [TemplateDocxProperty("Email", LocalizeDescription = "Email")]
        public string Email { get; set; }

        [TemplateDocxProperty("Phone", LocalizeDescription = "Телефон")]
        public string Phone { get; set; }

        [TemplateDocxProperty("City", LocalizeDescription = "Город")]
        public string City { get; set; }

        [TemplateDocxProperty("CouponCode", LocalizeDescription = "Код купона")]
        public string CouponCode { get; set; }

        [TemplateDocxProperty("Balance", LocalizeDescription = "Баланс")]
        public decimal Balance { get; set; }

        [TemplateDocxProperty("RewardPercent", LocalizeDescription = "Процент вознаграждения")]
        public float RewardPercent { get; set; }

        [TemplateDocxProperty("Type", LocalizeDescription = "Тип (юридическое/физическое лицо)")]
        public EPartnerType Type { get; set; }

        [TemplateDocxProperty("RegistrationDate", LocalizeDescription = "Дата регистрации")]
        public DateTime RegistrationDate { get; set; }

        [TemplateDocxProperty("ContractNumber", LocalizeDescription = "Номер договора")]
        public string ContractNumber { get; set; }

        [TemplateDocxProperty("ContractDate", LocalizeDescription = "Дата заключения договора")]
        public DateTime? ContractDate { get; set; }

        protected string GetShortName(string name)
        {
            if (name.IsNullOrEmpty())
                return name;
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return name;
            return parts[1][0] + ". " + parts[2][0] + ". " + parts[0];
        }
    }

    public abstract class BasePartnerActTemplate : BasePartnerTemplate
    {
        public BasePartnerActTemplate() : base()
        {
            Items = new List<PartnerCustomerRewardTemplate>();
        }

        public BasePartnerActTemplate(Partner partner) : base(partner)
        {
            Items = new List<PartnerCustomerRewardTemplate>();
        }

        public abstract string TplFileName { get; }

        [TemplateDocxProperty("PeriodFrom", LocalizeDescription = "Период начислений, от")]
        public DateTime PeriodFrom { get; set; }

        [TemplateDocxProperty("PeriodTo", LocalizeDescription = "Период начислений, до")]
        public DateTime PeriodTo { get; set; }

        [TemplateDocxProperty("RewardSum", LocalizeDescription = "Сумма вознаграждения")]
        public decimal RewardSum { get; set; }

        [TemplateDocxProperty("Table Items", Type = TypeItem.Table, LocalizeDescription = "Таблица с начислениями по клиентам")]
        public List<PartnerCustomerRewardTemplate> Items { get; set; }
    }

    public class PartnerLegalEntityActTemplate : BasePartnerActTemplate
    {
        public PartnerLegalEntityActTemplate() : base() { }

        public PartnerLegalEntityActTemplate(Partner partner) : base(partner) { }

        public override string TplFileName { get { return PartnerReportService.GetActReportTplDocx(EPartnerType.LegalEntity); } }

        [TemplateDocxProperty("Name", LocalizeDescription = "Наименование")]
        public override string Name { get; set; }

        [TemplateDocxProperty("Director", LocalizeDescription = "Руководитель")]
        public string Director { get; set; }

        [TemplateDocxProperty("DirectorShortName", LocalizeDescription = "Фамилия, инициалы руководителя")]
        public string DirectorShortName { get { return GetShortName(Director); } }

        [TemplateDocxProperty("Accountant", LocalizeDescription = "Бухгалтер")]
        public string Accountant { get; set; }

        [TemplateDocxProperty("AccountantShortName", LocalizeDescription = "Фамилия, инициалы бухгалтера")]
        public string AccountantShortName { get { return GetShortName(Accountant); } }
    }

    public class PartnerNaturalPersonActTemplate : BasePartnerActTemplate
    {
        public PartnerNaturalPersonActTemplate() : base() { }

        public PartnerNaturalPersonActTemplate(Partner partner) : base(partner)
        {
            PayoutCommission = SettingsPartners.PayoutCommissionNaturalPerson;
        }

        public override string TplFileName { get { return PartnerReportService.GetActReportTplDocx(EPartnerType.NaturalPerson); } }

        [TemplateDocxProperty("Name", LocalizeDescription = "ФИО")]
        public override string Name { get; set; }

        [TemplateDocxProperty("ShortName", LocalizeDescription = "Фамилия, инициалы (из паспортных данных)")]
        public string ShortName { get; set; }

        [TemplateDocxProperty("PayoutCommission", LocalizeDescription = "Процент комиссии на вывод средств")]
        public decimal PayoutCommission { get; set; }

        [TemplateDocxProperty("FinalRewardSum", LocalizeDescription = "Сумма вознаграждения с учетом комиссии")]
        public decimal FinalRewardSum { get { return (RewardSum - RewardSum * PayoutCommission / 100).RoundConvertToDefault(); } }
    }

    public class PartnerCustomerRewardTemplate
    {
        [TemplateDocxProperty("Email", LocalizeDescription = "Email")]
        public string Email { get; set; }

        [TemplateDocxProperty("Phone", LocalizeDescription = "Телефон")]
        public string Phone { get; set; }

        [TemplateDocxProperty("DateBinded", LocalizeDescription = "Дата привязки")]
        public DateTime? DateBinded { get; set; }

        [TemplateDocxProperty("PaymentSum", LocalizeDescription = "Сумма платежей")]
        public decimal? PaymentSum { get; set; }

        [TemplateDocxProperty("RewardSum", LocalizeDescription = "Сумма вознаграждения")]
        public decimal RewardSum { get; set; }
    }
}
