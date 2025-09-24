using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.AchievementsEvents
{
    public class AchievementsEventsController : BaseAdminController
    {
        [HttpGet]
        public JsonResult SubscribeToCompanySocialNetworks()
        {
            Track.TrackService.TrackEvent(Track.ETrackEvent.SocialNetworks_Subscribe);
            return JsonOk();
        }
        
        [HttpGet]
        public JsonResult GoToCompanySupportCenter()
        {
            Track.TrackService.TrackEvent(Track.ETrackEvent.Support_GoToSupportCenter);
            return JsonOk();
        }
    }
}