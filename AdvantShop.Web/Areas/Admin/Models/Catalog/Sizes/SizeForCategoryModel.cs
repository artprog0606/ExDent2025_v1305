namespace AdvantShop.Web.Admin.Models.Catalog.Sizes
{
    public class SizeForCategoryModel
    {
        public int CategoryId { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public string SizeNameForCategory { get; set; }
    }
}
