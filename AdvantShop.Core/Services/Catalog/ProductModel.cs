using System.Collections.Generic;
using AdvantShop.Catalog;
using System.Web;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Catalog
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        public int OfferId { get; set; }
        
        public string ArtNo { get; set; }
        public string OfferArtNo { get; set; }

        public string UrlPath { get; set; }

        public string Name { get; set; }

        public string BriefDescription { get; set; }
        

        public float Multiplicity { get; set; }
        
        public float AmountOffer { get; set; }

        public float Amount { get; set; }

        public float AmountByMultiplicity => ProductExtensions.GetAmountByMultiplicity(Amount, Multiplicity);

        public float MinAmount { get; set; }

        public float MaxAmount { get; set; }

        public bool AllowPreorder { get; set; }

        public bool Recomend { get; set; }

        public bool Sales { get; set; }

        public bool Bestseller { get; set; }

        public bool New { get; set; }

        public bool Gifts { get; set; }

        public bool Enabled { get; set; }

        public string Colors { get; set; }

        public List<ProductColorModel> ColorsList =>
            !string.IsNullOrEmpty(Colors)
                ? JsonConvert.DeserializeObject<List<ProductColorModel>>(Colors)
                : null;

        public int ColorId { get; set; }
        public int SizeId { get; set; }

        public double Ratio { get; set; }
        public double? ManualRatio { get; set; }
        public int? RatioId { get; set; }

        public float BasePrice { get; set; }

        public float Discount { get; set; }
        public float DiscountAmount { get; set; }

        public int Comments { get; set; }

        public float CurrencyValue { get; set; }
        
        public bool MultiPrices { get; set; }

        public string BarCode { get; set; }
        
        public bool DoNotApplyOtherDiscounts { get; set; }
        
        public int? MainCategoryId { get; set; }
        

        private string PhotoName { get; set; }
        private string PhotoNameSize1 { get; set; }
        private string PhotoNameSize2 { get; set; }
        private string PhotoDescription { get; set; }
        private bool PhotoMain { get; set; }
        private string AdditionalPhoto { get; set; }
        
        private ProductPhoto _photo;
        public ProductPhoto Photo
        {
            get
            {
                if (_photo != null)
                    return _photo;

                var title = !string.IsNullOrEmpty(PhotoDescription)
                            ? HttpUtility.HtmlAttributeEncode(PhotoDescription)
                            : HttpUtility.HtmlAttributeEncode($"{Name} {ArtNo}");

                _photo = new ProductPhoto()
                {
                    PhotoName = AdditionalPhoto ?? PhotoName,
                    PhotoNameSize1 = PhotoNameSize1,
                    PhotoNameSize2 = PhotoNameSize2,
                    Title = title,
                    Alt = title,
                    PhotoId = PhotoId,
                    Main = PhotoMain
                };

                return _photo;
            }
            set => _photo = value;
        }
        
        public string StartPhotoJson =>
            $"[{{PathSmall:'{Photo.ImageSrcSmall()}',PathMiddle:'{Photo.ImageSrcMiddle()}',PathBig:'{Photo.ImageSrcBig()}'}}]";

        public string PhotoSmall => Photo.ImageSrcSmall();

        public string PhotoMiddle => Photo.ImageSrcMiddle();

        public string PhotoBig => Photo.ImageSrcBig();

        public int PhotoId { get; set; }

        public int CountPhoto { get; set; }
        
        public int? SelectedColorId { get; set; }
        public int? PreSelectedColorId { get; set; }
        public int? SelectedSizeId { get; set; }
        public string UnitDisplayName { get; set; }
        public string UnitName { get; set; }
        
        public bool? AllowAddToCartInCatalog { get; set; }
    }

    public sealed class ProductColorModel
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string PhotoName { get; set; }
        public bool Main { get; set; }
    }
}
