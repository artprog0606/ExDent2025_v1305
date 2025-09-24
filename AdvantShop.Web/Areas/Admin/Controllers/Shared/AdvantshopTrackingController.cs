using System;
using System.Web.Mvc;
using AdvantShop.Track;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public class AdvantshopTrackingController : BaseController
    {
        public JsonResult TrackEvent(ETrackEvent? eventKey, string eventKeyPostfix)
        {
            if (eventKey.HasValue && Enum.IsDefined(typeof(ETrackEvent), eventKey))
            {
                TrackService.TrackEvent(eventKey.Value, eventKeyPostfix: eventKeyPostfix);
            }
            return JsonOk();
        }
    }
}
