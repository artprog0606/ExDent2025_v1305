namespace AdvantShop.Web.Admin.Models.Booking.Category
{
    public class CategoriesTree
    {
        public int? AffiliateId { get; set; }
        public int? ReservationResourceId { get; set; }
        public string Id { get; set; }

        public int? CategoryIdSelected { get; set; }

        public string ExcludeIds { get; set; }
        public string SelectedIds { get; set; }

        public bool ShowRoot { get; set; }
    }
}
