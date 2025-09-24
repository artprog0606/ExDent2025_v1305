using AdvantShop.Areas.Api.Models.StaticPages;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.StaticPages
{
    public class GetStaticPageApi : AbstractCommandHandler<StaticPageResponse>
    {
        private readonly int _id;
        private readonly StaticPageApiService _service;
        private StaticPageApi _page;

        public GetStaticPageApi(int id)
        {
            _id = id;
            _service = new StaticPageApiService();
        }

        protected override void Load()
        {
            _page = _service.Get(_id);
        }

        protected override void Validate()
        {
            if (_page == null || !_page.Enabled)
                throw new BlException("Страница не найдена");
        }

        protected override StaticPageResponse Handle()
        {
            return new StaticPageResponse(_page);
        }
    }
}