using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetGiftsResponse : List<ProductGiftApi>, IApiResponse
    {
        public GetGiftsResponse(List<ProductGiftApi> gifts)
        {
            this.AddRange(gifts);
        }
    }

    public class ProductGiftApi
    {
        public int ProductId { get; }
        public string Name { get; }
        public int MainProductsCount { get; }
        public string PhotoSmall { get; }
        public string PhotoMiddle { get; }
        public string Color { get; }
        public string Size { get; }
        public string ColorHeader { get; }
        public string SizeHeader { get; }
        public int? CanBeAppliedToOfferId { get; }
        public string CanBeAppliedTo { get; }

        public ProductGiftApi(GiftModel gift)
        {
            ProductId = gift.Product.ProductId;
            Name = gift.Product.Name;
            MainProductsCount = gift.ProductCount;
            PhotoSmall = gift.Photo.ImageSrcSmall();
            PhotoMiddle = gift.Photo.ImageSrcMiddle();
            Color = gift.Color?.ColorName;
            Size = gift.Size?.SizeName;
            ColorHeader = SettingsCatalog.ColorsHeader;
            SizeHeader = SettingsCatalog.SizesHeader;

            if (gift.ProductOfferId != null)
            {
                CanBeAppliedToOfferId = gift.ProductOfferId;
                if (gift.ProductOffer != null)
                    CanBeAppliedTo = $"{gift.ProductOffer.Product.Name} {gift.ProductOffer.Color?.ColorName} {gift.ProductOffer.Size?.SizeName}".Trim();
            }
        }
    }
}