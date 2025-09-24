using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Partners
{
    public class PartnerCustomerModel
    {
        public Guid CustomerId { get; set; }
        public DateTime DateCreated { get; set; }

        public string Email { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }
        public decimal PaidOrdersSum { get; set; }
        public int PaidOrdersCount { get; set; }

        public string CouponCode { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmTerm { get; set; }
        public string UtmContent { get; set; }
        public DateTime? VisitDate { get; set; }

        public string PaidOrdersSumFormatted
        {
            get { return PaidOrdersSum.FormatRoundPriceDefault(); }
        }

        public string DateCreatedFormatted
        {
            get { return Culture.ConvertDate(DateCreated); }
        }

        public string VisitDateFormatted
        {
            get { return VisitDate.HasValue ? Culture.ConvertDate(VisitDate.Value) : null; }
        }

        public bool HasDetails
        {
            get { return CouponCode.IsNotEmpty() || Url.IsNotEmpty(); }
        }
    }
}
