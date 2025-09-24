using AdvantShop.Areas.Api.Models.Inits;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Inits
{
    public class InitApiV2 : AbstractCommandHandler<InitApiResponseV2>
    {
        private readonly InitApiModel _model;

        public InitApiV2(InitApiModel model)
        {
            _model = model;
        }
        
        protected override InitApiResponseV2 Handle()
        {
            var response = new InitApi(_model).Execute();
            return new InitApiResponseV2(response);
        }
    }
}