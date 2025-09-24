using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Catalogs;
using AdvantShop.Areas.Api.Handlers.Search;
using AdvantShop.Areas.Api.Models.Search;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class SearchController : BaseApiController
    {
        // POST api/search
        [HttpPost]
        public JsonResult Index(SearchFilter filter) => JsonApi(new GetSearchApi(filter));
        
        // POST api/search/filter
        [HttpPost]
        public JsonResult GetFilter(SearchFilter filter) => JsonApi(new GetSearchFilterApi(filter));
        
        // POST api/search/filtercount
        [HttpPost]
        public JsonResult GetFilterCount(SearchFilter filter) => JsonApi(new GetSearchFilterCountApi(filter));
        
        // POST api/search/autocomplete
        [HttpPost]
        public JsonResult Autocomplete(SearchAutocomplete model) => JsonApi(new GetSearchAutocompleteApi(model));
    }
}