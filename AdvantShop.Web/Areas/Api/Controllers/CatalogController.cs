using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Catalogs;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi, Warehouse]
    public class CatalogController : BaseApiController
    {
        // POST api/catalog
        [HttpPost]
        public JsonResult Index(CatalogFilter filter) => JsonApi(new GetCatalogApi(filter));
        
        // POST api/catalog/all
        [HttpPost]
        public JsonResult All(CatalogAllFilter filter) => JsonApi(new GetCatalogAllApi(filter));
        
        // POST api/catalog/filter
        [HttpPost]
        public JsonResult GetFilter(CatalogFilter filter) => JsonApi(new GetCatalogFilterApi(filter));

        // POST api/catalog/filtercount
        [HttpPost]
        public JsonResult GetFilterCount(CatalogFilter filter) => JsonApi(new GetCatalogFilterCountApi(filter));
    }
}