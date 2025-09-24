using AdvantShop.Catalog;

namespace AdvantShop.Core.Services.Catalog
{
    public class RelatedProductAdminModel
    {
        private string PhotoName { get; set; }

        public int RelatedProductId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ArtNo { get; set; }

        public ProductPhoto Photo
        {
            get { return new ProductPhoto() { PhotoName = PhotoName }; }
        }

    }
}
