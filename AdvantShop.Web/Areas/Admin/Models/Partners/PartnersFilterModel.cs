using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Partners
{
    public class PartnersFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public EPartnerType? Type { get; set; }
        public float? BalanceFrom { get; set; }
        public float? BalanceTo { get; set; }
        public string DateCreatedFrom { get; set; }
        public string DateCreatedTo { get; set; }
        public bool? Enabled { get; set; }
    }
}
