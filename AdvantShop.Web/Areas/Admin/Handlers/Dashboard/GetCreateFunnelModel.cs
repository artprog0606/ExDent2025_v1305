using AdvantShop.Core.Services.Landing.Templates;

namespace AdvantShop.Web.Admin.Handlers.Dashboard
{
    public class GetCreateFunnelModel
    {
        private readonly string _id;

        public GetCreateFunnelModel(string id)
        {
            _id = id;
        }

        public LpTemplate Execute()
        {
            var template = new LpTemplateService().GetTemplate(_id);
            return template;
        }
    }
}
