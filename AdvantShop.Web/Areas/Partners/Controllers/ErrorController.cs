using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Partners.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    [ExcludeFilter(typeof(LogUserActivityAttribute))]
    public partial class ErrorController : BasePartnerController
    {
        public ActionResult NotFound()
        {
            SetResponse(HttpStatusCode.NotFound);
            var ext = VirtualPathUtility.GetExtension(Request.RawUrl);
            if (ext != null)
            {
                var list = new List<string> { ".css", ".js", ".jpg", ".jpeg", ".png", ".map", ".ico", ".gif" };
                if (list.Contains(ext.ToLower()))
                    return new EmptyResult();
            }
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