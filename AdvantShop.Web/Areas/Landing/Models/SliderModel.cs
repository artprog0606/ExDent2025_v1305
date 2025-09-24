using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class SliderModel
    {
        public bool Accessibility { get; set; }
        public bool AdaptiveHeight { get; set; }
        public string AppendArrows { get; set; }
        public string AppendDots { get; set; }
        public bool Arrows { get; set; }
        public string AsNavFor { get; set; }
        public bool Autoplay { get; set; }
        public int AutoplaySpeed { get; set; }
        public bool CenterMode { get; set; }
        public string CenterPadding { get; set; }
        public string CustomPaging { get; set; }
        public bool Dots { get; set; }
        public string DotsClass { get; set; }
        public bool Draggable { get; set; }
        public string Easing { get; set; }
        public float EdgeFriction { get; set; }
        public bool Fade { get; set; }
        public bool FocusOnSelect { get; set; }
        public bool FocusOnChange { get; set; }
        public bool Infinite { get; set; }
        public int InitialSlide { get; set; }
        public string LazyLoad { get; set; }
        public bool MobileFirst { get; set; }
        public string NextArrow { get; set; }
        public bool PauseOnDotsHover { get; set; }
        public bool PauseOnFocus { get; set; }
        public bool PauseOnHover { get; set; }
        public string PrevArrow { get; set; }
        public string RespondTo { get; set; }
        public List<object> Responsive { get; set; }
        public int Rows { get; set; }
        public bool Rtl { get; set; }
        public string Slide { get; set; }
        public int SlidesPerRow { get; set; }
        public int SlidesToScroll { get; set; }
        public int SlidesToShow { get; set; }
        public int Speed { get; set; }
        public bool Swipe { get; set; }
        public bool SwipeToSlide { get; set; }
        public bool TouchMove { get; set; }
        public int TouchThreshold { get; set; }
        public bool UseCSS { get; set; }
        public bool UseTransform { get; set; }
        public bool VariableWidth { get; set; }
        public bool Vertical { get; set; }
        public bool VerticalSwiping { get; set; }
        public bool WaitForAnimate { get; set; }
        public int ZIndex { get; set; }
    }
}
