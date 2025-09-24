using AdvantShop.Core.Services.Catalog;
using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.Models.Catalog.ProductLists
{
    public class ProductListItemModel
    {
        public bool Enabled { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ProductArtNo { get; set; }

        public int SortOrder { get; set; }

        public int ListId { get; set; }

        public string PhotoName { get; set; }
        public string PhotoSrc
        {
            get { return FoldersHelper.GetImageProductPath(ProductImageType.Small, PhotoName, true); }
        }

        public float Price { get; set; }

        public string PriceString
        {
            get
            {
                return PriceFormatService.FormatPrice(Price, CurrencyValue, CurrencyCode, CurrencyIso3, CurrencyIsCodeBefore);
            }
        }

        public string CurrencyCode { get; set; }
        public string CurrencyIso3 { get; set; }
        public float CurrencyValue { get; set; }
        public bool CurrencyIsCodeBefore { get; set; }

        public float Amount { get; set; }
    }
}
