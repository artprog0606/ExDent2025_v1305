using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using AdvantShop.Controllers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Filters.Headers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [Warehouse]
    [AllowAnyCrossOriginResourceSharing]
    public class BaseApiController : BaseController
    {
        protected ActionResult Error404()
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "NotFound");

            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(HttpContext, routeData));
            return new EmptyResult();
        }

        protected JsonResult JsonApi<T>(ICommandHandler<T> handler) where T : IApiResponse
        {
            try
            {
                return JsonCamelCase(handler.Execute());
            }
            catch (BlException e)
            {
                return JsonCamelCase(new ApiError(e.Message));
            }
        }
        
        protected JsonResult JsonApi<T>(Func<T> action) where T : IApiResponse
        {
            try
            {
                return JsonCamelCase(action());
            }
            catch (BlException e)
            {
                return JsonCamelCase(new ApiError(e.Message));
            }
        }
    }
}