using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Carousels;
using AdvantShop.Areas.Api.Models.Carousels;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKey]
    public class CarouselsController : BaseApiController
    {
        // GET api/carousels
        [HttpGet]
        public JsonResult Get(CarouselSlidesFilter filter) => JsonApi(new GetCarouselSlides(filter));
    }
}