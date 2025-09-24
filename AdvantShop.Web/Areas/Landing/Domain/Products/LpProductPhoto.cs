namespace AdvantShop.App.Landing.Domain.Products
{
    public class LpProductPhoto
    {
        public string PathXSmall { get; set; }
        public string PathSmall { get; set; }
        public string PathMiddle { get; set; }
        public string PathBig { get; set; }
        public int? ColorId { get; set; }
        public int PhotoId { get; set; }
        public string Description { get; set; }
        public int XSmallProductImageHeight { get; set; }
        public int XSmallProductImageWidth { get; set; }
        public int SmallProductImageHeight { get; set; }
        public int SmallProductImageWidth { get; set; }
        public int MiddleProductImageWidth { get; set; }
        public int MiddleProductImageHeight { get; set; }
    }
}
