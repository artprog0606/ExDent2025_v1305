using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Areas.Api.Models.StaticPages
{
    public class StaticPagesFilter : BaseFilterModel
    {
        public bool? LoadText { get; set; }
        public bool? ShowInProfile { get; set; }
        public string Title { get; set; }
    }
}