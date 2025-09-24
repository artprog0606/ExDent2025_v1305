using System;
using System.Web.Hosting;
using System.Web.Mvc;
using AdvantShop.ExportImport;

namespace AdvantShop.Areas.Api.Controllers
{
    public class ResellersController : BaseApiController
    {
        public FileResult Catalog(Guid id)
        {
            var resellerSettings = ExportFeedSettingsProvider.GetSettingsByParam<ExportFeedResellerOptions>("ResellerCode", id.ToString());
            if (resellerSettings == null)
                return null;

            var fileName = HostingEnvironment.MapPath(string.Format("~/{0}", resellerSettings.FileFullName));
            if (!System.IO.File.Exists(fileName))
                return null;

            //var fileBytes = System.IO.File.ReadAllBytes(fileName);

            return File(fileName, System.Net.Mime.MediaTypeNames.Application.Octet, resellerSettings.FileFullName);
        }
    }
}