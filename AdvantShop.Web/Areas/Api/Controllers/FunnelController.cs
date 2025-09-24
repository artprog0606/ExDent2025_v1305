using System.Web.Mvc;
using AdvantShop.Areas.Api.Handlers.Funnels;
using AdvantShop.Areas.Api.Model.Funnels;

namespace AdvantShop.Areas.Api.Controllers
{
    public class FunnelController : BaseApiController
    {
        // GET: Api/Funnel
        [HttpGet]
        public ActionResult GetFunnels(string id)
        {
            return ProcessJsonResult(new GetFunnelQuery(), id);
        }

        [HttpGet]
        public ActionResult GetMapFunnelsOnDomain(string id)
        {
            return ProcessJsonResult(new GetMapFunnelsOnDomainQuery(), id);
        }

        [HttpPost]
        public ActionResult AddMapFunnelOnDomain(AddMapFunnelOnDomainDto model)
        {
            return ProcessJsonResult(new AddMapFunnelOnDomainCommand(), model);
        }

        [HttpPost]
        public ActionResult RemoveMapFunnelOnDomain(RemoveMapFunnelOnDomainDto model)
        {
            return ProcessJsonResult(new RemoveMapFunnelOnDomainCommand(), model);
        }

        [HttpPost]
        public ActionResult SetMainDomainOnFunnel(SetMainDomainOnFunnelDto model)
        {
            return ProcessJsonResult(new SetMainDomainOnFunnelCommand(), model);
        }

        [HttpPost]
        public ActionResult SetSettingDomain(SetSettingDomainDto model)
        {
            return ProcessJsonResult(new SetSettingDomainCommand(), model);
        }
    }
}