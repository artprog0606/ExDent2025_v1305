using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Search;
using AdvantShop.Core;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Handlers.Search;
using AdvantShop.Models.Catalog;

namespace AdvantShop.Areas.Api.Handlers.Search
{
    public class GetSearchFilterCountApi : AbstractCommandHandler<GetCatalogFilterCountResponse>
    {
        private readonly SearchFilter _filter;
    
        public GetSearchFilterCountApi(SearchFilter filter)
        {
            _filter = filter;
        }

        protected override void Validate()
        {
            if (_filter == null || string.IsNullOrWhiteSpace(_filter.Query))
                throw new BlException("Укажите поисковый запрос");
        }

        protected override GetCatalogFilterCountResponse Handle()
        {
            var searchModel = (SearchCatalogModel) _filter;
            var pager = new SearchPagingHandler(searchModel, false).GetForFilterProductCount();

            if (pager == null)
                throw new BlException("Ошибка при фильтрации");

            return new GetCatalogFilterCountResponse(pager.TotalItemsCount);
        }
    }
}