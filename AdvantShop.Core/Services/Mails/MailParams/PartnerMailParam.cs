using System;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Customers;

namespace AdvantShop.Mails
{
    public abstract class BasePartnerMailTemplate : MailTemplate
    {
        protected readonly Partner Partner;
        protected readonly Coupon Coupon;
        private readonly string _partnersUrl;

        public BasePartnerMailTemplate(Partner partner)
        {
            Partner = partner;
            Coupon = partner.CouponId.HasValue ? CouponService.GetCoupon(partner.CouponId.Value) : null;
            _partnersUrl = SettingsMain.SiteUrl.TrimEnd('/') + "/partners";
        }

        protected override string FormatString(string formatedStr)
        {
            return formatedStr
                .Replace("#SITEURL#", SettingsMain.SiteUrl)
                .Replace("#SITENAME#", SettingsMain.ShopName)
                .Replace("#PARTNERS_URL#", _partnersUrl)
                .Replace("#NAME#", Partner.Name)
                .Replace("#EMAIL#", Partner.Email)
                .Replace("#PHONE#", Partner.Phone)
                .Replace("#CITY#", Partner.City)
                .Replace("#COUPONCODE#", Coupon != null ? Coupon.Code : null)
                .Replace("#BALANCE#", Partner.Balance.FormatRoundPriceDefault())
                .Replace("#REWARDPERCENT#", Partner.RewardPercent.ToString())
                .Replace("#TYPE#", Partner.Type.Localize());
        }
    }

    public class PartnerRegistrationMailTemplate : BasePartnerMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnPartnerRegistration; }
        }

        public PartnerRegistrationMailTemplate(Partner partner) : base(partner)
        {
        }

        protected override string FormatString(string formatedStr)
        {
            return base.FormatString(formatedStr)
                .Replace("#REGDATE#", Localization.Culture.ConvertDate(DateTime.Now))
                .Replace("#PASSWORD#", Partner.Password);
        }
    }

    public class PartnerCustomerBindedMailTemplate : BasePartnerMailTemplate
    {
        private readonly Customer _customer;

        public override MailType Type
        {
            get { return MailType.OnPartnerCustomerBinded; }
        }

        public PartnerCustomerBindedMailTemplate(Partner partner, Customer customer) : base(partner)
        {
            _customer = customer;
        }

        protected override string FormatString(string formatedStr)
        {
            return base.FormatString(formatedStr)
                .Replace("#CUSTOMER_EMAIL#", _customer.EMail.AnonimizeEmail())
                .Replace("#CUSTOMER_PHONE#", _customer.Phone.AnonimizePhone());
        }
    }

    public class PartnerMoneyAddedMailTemplate : BasePartnerMailTemplate
    {
        private readonly string _customerEmail;
        private readonly string _customerPhone;
        private readonly string _rewardSum;
        private readonly string _paymentSum;

        public override MailType Type
        {
            get { return MailType.OnPartnerMoneyAdded; }
        }

        public PartnerMoneyAddedMailTemplate(Partner partner, string customerEmail, string customerPhone, string rewardSum, string paymentSum) : base(partner)
        {
            _customerEmail = customerEmail;
            _customerPhone = customerPhone;
            _rewardSum = rewardSum;
            _paymentSum = paymentSum;
        }

        protected override string FormatString(string formatedStr)
        {
            return base.FormatString(formatedStr)
                .Replace("#CUSTOMER_EMAIL#", _customerEmail.AnonimizeEmail())
                .Replace("#CUSTOMER_PHONE#", _customerPhone.AnonimizePhone())
                .Replace("#REWARD_SUM#", _rewardSum)
                .Replace("#PAYMENT_SUM#", _paymentSum);
        }
    }

    public class PartnerMonthReportMailTemplate : BasePartnerMailTemplate
    {
        private readonly string _reportPeriod;
        private readonly int _customersCount;
        private readonly int _customersTotalCount;
        private readonly string _rewardsSum;
        private readonly string _rewardsTotalSum;
        private readonly string _paymentsSum;
        private readonly string _paymentsTotalSum;

        public override MailType Type
        {
            get { return MailType.OnPartnerMonthReport; }
        }

        public PartnerMonthReportMailTemplate(Partner partner, string reportPeriod, int customersCount, int customersTotalCount, 
            string rewardsSum, string rewardsTotalSum, string paymentsSum, string paymentsTotalSum) : base(partner)
        {
            _reportPeriod = reportPeriod;
            _customersCount = customersCount;
            _customersTotalCount = customersTotalCount;
            _rewardsSum = rewardsSum;
            _rewardsTotalSum = rewardsTotalSum;
            _paymentsSum = paymentsSum;
            _paymentsTotalSum = paymentsTotalSum;
        }

        protected override string FormatString(string formatedStr)
        {
            return base.FormatString(formatedStr)
                .Replace("#REPORTPERIOD#", _reportPeriod)
                .Replace("#CUSTOMERS_COUNT#", _customersCount.ToString())
                .Replace("#CUSTOMERS_TOTALCOUNT#", _customersTotalCount.ToString())
                .Replace("#REWARDS_SUM#", _rewardsSum)
                .Replace("#REWARDS_TOTALSUM#", _rewardsTotalSum)
                .Replace("#PAYMENTS_SUM#", _paymentsSum)
                .Replace("#PAYMENTS_TOTALSUM#", _paymentsTotalSum);
        }
    }

    public abstract class BasePartnerReportMailTemplate : BasePartnerMailTemplate
    {
        private readonly string _rewardPeriod;
        private readonly string _rewardsSum;
        private readonly string _actLink;

        public BasePartnerReportMailTemplate(Partner partner, string rewardPeriod, string rewardsSum, string actLink) : base(partner)
        {
            _rewardPeriod = rewardPeriod;
            _rewardsSum = rewardsSum;
            _actLink = actLink;
        }

        protected override string FormatString(string formatedStr)
        {
            return base.FormatString(formatedStr)
                .Replace("#REWARDPERIOD#", _rewardPeriod)
                .Replace("#REWARDS_SUM#", _rewardsSum)
                .Replace("#ACT_LINK#", _actLink);
        }
    }

    public class PartnerLegalEntityActReportMailTemplate : BasePartnerReportMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnPartnerLegalEntityActReport; }
        }

        public PartnerLegalEntityActReportMailTemplate(Partner partner, string rewardPeriod, string rewardsSum, string actLink)
            : base(partner, rewardPeriod, rewardsSum, actLink)
        {

        }
    }

    public class PartnerNaturalPersonActReportMailTemplate : BasePartnerReportMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnPartnerNaturalPersonActReport; }
        }

        public PartnerNaturalPersonActReportMailTemplate(Partner partner, string rewardPeriod, string rewardsSum, string actLink)
            : base(partner, rewardPeriod, rewardsSum, actLink)
        {

        }
    }
}