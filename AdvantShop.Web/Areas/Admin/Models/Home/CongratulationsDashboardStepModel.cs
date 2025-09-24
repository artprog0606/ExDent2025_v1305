using AdvantShop.Track;

namespace AdvantShop.Web.Admin.Models.Home
{
    public class CongratulationsDashboardStepModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }

        public string VideoPreviewSrc { get; set; }
        public string VideoSrc { get; set; }
        public string VideoDescription { get; set; }

        public string ButtonText { get; set; }
        public string ButtonActionLink { get; set; }

        public ETrackEvent OpenVideoEvent { get; set; }
        public ETrackEvent VideoButtonEvent { get; set; }
        public ETrackEvent ButtonEvent { get; set; }
    }
}
