using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Areas.Partners.Models.Customers
{
    public class CustomerModel
    {
        public string Email { get; set; }
        public string EmailAnonimized { get { return Email.AnonimizeEmail(); } }
        public string Phone { get; set; }
        public string PhoneAnonimized { get { return Phone.AnonimizePhone(); } }
        public DateTime BindDate { get; set; }
        /// <summary>
        /// Сумма товаров в оплаченных покупателем заказах с учетом скидок
        /// </summary>
        public decimal PaymentSum { get; set; }
        public string PaymentSumFormatted { get { return PaymentSum.FormatRoundPriceDefault(); } }
        /// <summary>
        /// Сумма вознаграждения, начисленного с заказов покупателя
        /// </summary>
        public decimal RewardSum { get; set; }
        public string RewardSumFormatted { get { return RewardSum.FormatRoundPriceDefault(); } }

        public DateTime? VisitDate { get; set; }
        public string CouponCode { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmTerm { get; set; }
        public string UtmContent { get; set; }

        public bool HasDetails
        {
            get { return CouponCode.IsNotEmpty() || Url.IsNotEmpty(); }
        }
    }
}