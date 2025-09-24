using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.Models.Crm.Leads
{
    public class LeadTempProduct
    {
        public int OfferId { get; set; }
        public string ArtNo { get; set; }
        public float Amount { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string PreparedPrice { get; set; }
        public string PhotoName { get; set; }
        public string PhotoSrc
        {
            get { return FoldersHelper.GetImageProductPath(ProductImageType.Small, PhotoName, true); }
        }
    }
}
