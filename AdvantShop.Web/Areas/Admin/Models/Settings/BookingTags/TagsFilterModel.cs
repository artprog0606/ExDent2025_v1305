using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.BookingTags
{
    public class TagsFilterModel : BaseFilterModel<int>
    {
        public string Name { get; set; }
    }
}
