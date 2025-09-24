using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Search;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Handlers.Search;
using AdvantShop.Models.Catalog;

namespace AdvantShop.Areas.Api.Handlers.Catalogs
{
    public class GetSearchFilterApi : AbstractCommandHandler<GetCatalogFilterResponse>
    {
        private readonly SearchFilter _filter;
    
        public GetSearchFilterApi(SearchFilter filter)
        {
            _filter = filter;
        }

        protected override void Validate()
        {
            if (_filter == null || string.IsNullOrWhiteSpace(_filter.Query))
                throw new BlException("Укажите поисковый запрос");
        }

        protected override GetCatalogFilterResponse Handle()
        {
            var searchModel = (SearchCatalogModel) _filter;
            var result = new GetSearchFilterHandler(searchModel).Execute();

            return new GetCatalogFilterResponse(result);
        }
    }
}