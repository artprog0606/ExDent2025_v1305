using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Barcodes;
using AdvantShop.Areas.Api.Models.Barcodes;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser, AuthUserApi]
    public class BarcodesController : BaseApiController
    {
        // POST api/barcodes/search
        [HttpPost]
        public JsonResult Search(SearchBarcode model) => JsonApi(new GetProductByBarcodeApi(model));
    }
}