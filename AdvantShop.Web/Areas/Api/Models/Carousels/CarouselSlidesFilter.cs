namespace AdvantShop.Areas.Api.Models.Carousels
{
    public class CarouselSlidesFilter
    {
        public bool? DisplayInOneColumn { get; set; }
        public bool? DisplayInTwoColumns { get; set; }
        public bool? DisplayInMobile { get; set; }
        public bool? Enabled { set; get; }
    }
}