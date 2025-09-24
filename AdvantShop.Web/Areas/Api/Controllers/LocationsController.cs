using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Locations;
using AdvantShop.Areas.Api.Models.Locations;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser]
    public class LocationsController : BaseApiController
    {
        // GET locations/maincities
        [HttpGet]
        public JsonResult MainCities() => JsonApi(new GetMainCities());
        
        // GET locations/city
        [HttpGet]
        public JsonResult GetCity(GetCityModel model) => JsonApi(new GetCityApi(model));
        
        // POST locations/countries
        [HttpPost]
        public JsonResult GetCountries(GetCountriesModel model) => JsonApi(new GetCountriesApi(model));
    }
}