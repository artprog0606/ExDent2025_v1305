using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Attributes;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Crm
{
    [SalesChannel(ESalesChannelType.OzonSeller)]
    public partial class OzonSellerController : BaseAdminController
    {
        public ActionResult Index()
        {
            return null;
        }
    }
}
