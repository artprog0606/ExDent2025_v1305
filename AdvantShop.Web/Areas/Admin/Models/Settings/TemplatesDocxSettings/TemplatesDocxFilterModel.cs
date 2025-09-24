using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.TemplatesDocxSettings
{
    public class TemplatesDocxFilterModel : BaseFilterModel<int>
    {
        public TemplateDocxType? Type { get; set; }
    }
}
