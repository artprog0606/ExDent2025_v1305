using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Areas.Api.Models.Orders
{
    public class OrdersMeFilterModel : BaseFilterModel
    {
        public bool? LoadItems { get; set; }
        public bool? LoadCustomer { get; set; }
        public bool? LoadSource { get; set; }
        public bool? LoadReview { get; set; }
        public bool? LoadBillingApiLink { get; set; }
        
        public int? StatusId { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsCompleted { get; set; }
        public float? SumFrom { get; set; }
        public float? SumTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}