using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    [ExcludeFilter(typeof(LogUserActivityAttribute))]
    public partial class ErrorController : BaseAdminController
    {
        public ActionResult NotFound()
        {
            SetResponse(HttpStatusCode.NotFound);

            var path = Request.RawUrl.ToLower();
            var index = path.IndexOf('?');
            var pathWithoutQuery = index > 0 ? path.Substring(0, index) : path;

            var ext = FileHelpers.GetExtension(pathWithoutQuery);
            if (!string.IsNullOrEmpty(ext))
            {
                var list = new List<string>
                {
                    ".css", ".js", ".jpg", ".jpeg", ".png", ".bmp", ".map", ".ico", ".gif", ".svg", ".webp", ".cur",
                    ".tif", ".tiff", ".woff", ".woff2"
                };
                if (list.Contains(ext))
                    return new EmptyResult();
            }

            SetMetaInformation(T("Error.NotFound.Title"));

            return View();
        }

        public ActionResult NotFoundPartial()
        {
            return PartialView("NotFound");
        }

        private void SetResponse(HttpStatusCode httpStatusCode)
        {
            try
            {
                Response.Clear();
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)httpStatusCode;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription((int)httpStatusCode);
            }
            catch
            {

            }
        }
    }
}