using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductItemShort
    {
        public int ProductId { get; }
        public int OfferId { get; set; }
        public string ArtNo { get; }
        public string Name { get; }
        public string UrlPath { get; }
        public string BriefDescription { get; }
        
        public int? ColorId { get; }
        public int? SizeId { get; }
        
        public List<ColorApi> Colors { get; }

        public ProductDiscountApi Discount { get; }
        public float? OldPrice { get; }
        public float Price { get; }
        public string PreparedOldPrice { get; }
        public string PreparedPrice { get; }
        public bool AddToCart { get; }

        public ProductPhotoApi Photo { get; }
        
        public string PhotoSmall { get; }

        public string PhotoMiddle { get; }
        
        public List<ProductPhotoApi> Photos { get; set; }

        public ProductItemShort(Product product) : this(product, null)
        {
        }

        public ProductItemShort(Product product, Offer offer)
        {
            ProductId = product.ProductId;
            ArtNo = product.ArtNo;
            Name = product.Name;
            UrlPath = product.UrlPath;
            BriefDescription = !string.IsNullOrWhiteSpace(product.BriefDescription) ? product.BriefDescription : null;
            Discount = new ProductDiscountApi(product.Discount);

            var photo = product.ProductPhotos.FirstOrDefault();
            if (photo != null)
            {
                Photo = new ProductPhotoApi(photo);
                PhotoSmall = photo.ImageSrcSmall();
                PhotoMiddle = photo.ImageSrcMiddle();
            }
            else
            {
                var noPhoto = new ProductPhoto();
                PhotoSmall = noPhoto.ImageSrcSmall();
                PhotoMiddle = noPhoto.ImageSrcMiddle();
            }

            Photos = product.ProductPhotos.Select(x => new ProductPhotoApi(x)).ToList();

            if (offer == null)
                offer = OfferService.GetMainOffer(product.Offers, product.AllowPreOrder);

            if (offer != null)
            {
                var warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
                if (warehouseIds != null)
                    offer.SetAmountByStocksAndWarehouses(warehouseIds);
                
                ArtNo = offer.ArtNo;
                OfferId = offer.OfferId;
                
                var productOffer = new ProductOffer(offer, product, null, null, product.GetMinAmount());

                ColorId = productOffer.ColorId;
                SizeId = productOffer.SizeId;
                OldPrice = productOffer.OldPrice;
                PreparedOldPrice = productOffer.PreparedOldPrice;
                Price = productOffer.Price;
                PreparedPrice = productOffer.PreparedPrice;
                AddToCart = productOffer.AddToCart;
            }
            
            var colors =
                SettingsCatalog.ComplexFilter
                    ? product.Offers.Where(x => x.ColorID != null && x.Color != null)
                        .Select(x => new ColorApi(x.Color))
                        .ToList()
                    : null;

            Colors = colors != null && colors.Count > 0 ? colors : null;
        }
    }
}