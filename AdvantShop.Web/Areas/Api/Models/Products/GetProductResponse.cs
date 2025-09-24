using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Handlers.Products;
using AdvantShop.Areas.Api.Models.Shared;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Handlers.ProductDetails;
using AdvantShop.Orders;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetProductResponse : IApiResponse
    {
        public int ProductId { get; }
        public string ArtNo { get; }
        public string Name { get; }
        public string UrlPath { get; }
        public string BriefDescription { get; }
        public string Description { get; }
        public bool Enabled { get; }
        public bool CategoryEnabled { get; }
        public bool Hidden { get; }
        public double? Ratio { get; }
        public bool IsAdult { get; }
        public bool Recomended { get; }
        public bool NewProduct { get; }
        public bool Bestseller { get; }
        public bool Sales { get; }
        public bool Favorite { get; }
        public bool Gifts { get; }
        public List<ProductGiftApi> GiftProducts { get; }
        public bool AllowPreorder { get; }
        public float MinAmount { get; }
        public float? MaxAmount { get; }
        public float Multiplicity { get; }
        public string Unit { get; }
        public int CommentsCount { get; set; }

        public ProductSizeColorPickerApi SizeColorPicker { get; set; }
        
        public ProductDiscountApi Discount { get; }

        public List<ProductOffer> Offers { get; }
        public int? OfferSelectedId { get; set; }
        
        public List<CustomOptionApi> CustomOptions { get; }

        public BrandShortApi Brand { get; }
        public CurrencyApi Currency { get; }

        public string PhotoSmall { get; }
        public string PhotoMiddle { get; }
        public string PhotoBig { get; }
        
        public List<ProductPhotoApi> Photos { get; }
        public bool ShowPhotos360 { get; }
        public List<ProductPhotoApi> Photos360 { get; }
        public List<ProductVideoApi> Videos { get; }
        public List<ProductMarker> Markers { get; set; }

        public SizeChartApi SizeChart { get; }

        public GetProductResponse(Product product)
        {
            ProductId = product.ProductId;
            ArtNo = product.ArtNo;
            Name = product.Name;
            UrlPath = product.UrlPath;
            BriefDescription = !string.IsNullOrWhiteSpace(product.BriefDescription) ? product.BriefDescription : null;
            Description = !string.IsNullOrWhiteSpace(product.Description) ? product.Description : null;
            Enabled = product.Enabled;
            CategoryEnabled = product.CategoryEnabled;
            Hidden = product.Hidden;
            IsAdult = product.ExportOptions.Adult;
            Ratio = product.ManualRatio != null && product.ManualRatio.Value > 0
                ? product.ManualRatio > 0 ? product.ManualRatio : default(double?)
                : product.Ratio > 0
                    ? product.Ratio
                    : default(double?);
            Recomended = product.Recomended;
            NewProduct = product.New;
            Bestseller = product.BestSeller;
            Sales = product.OnSale;
            Gifts = product.HasGifts();
            AllowPreorder = product.AllowPreOrder;

            MinAmount = product.GetMinAmount();

            MaxAmount = product.MaxAmount ?? int.MaxValue;
            Multiplicity = product.Multiplicity;
            Unit = product.Unit?.DisplayName;
            
            var offerIds = product.Offers.Select(x => x.OfferId).ToList();
            Favorite = ShoppingCartService.CurrentWishlist.Any(x => offerIds.Contains(x.OfferId));
            
            CommentsCount = ReviewService.GetReviewsCount(ProductId, EntityType.Product, SettingsCatalog.ModerateReviews, true);

            Discount = new ProductDiscountApi(product.Discount);

            Brand = product.Brand != null ? new BrandShortApi(product.Brand) : null;
            Currency = new CurrencyApi(product.Currency);

            var photo = product.ProductPhotos.FirstOrDefault() ?? new ProductPhoto();

            PhotoSmall = photo.ImageSrcSmall();
            PhotoMiddle = photo.ImageSrcMiddle();
            PhotoBig = photo.ImageSrcBig();
            Photos = product.ProductPhotos.Select(x => new ProductPhotoApi(x)).ToList();
            ShowPhotos360 = product.ActiveView360;
            Photos360 = ShowPhotos360 ? product.ProductPhotos360.Select(x => new ProductPhotoApi(x)).ToList() : null;
            Videos = product.ProductVideos.Select(x => new ProductVideoApi(x)).ToList();

            var customOptions = CustomOptionsService.GetCustomOptionsByProductIdCached(ProductId);
            CustomOptions = customOptions.Select(x => new CustomOptionApi(x)).ToList();

            
            Offers = product.Offers.Select(x => new ProductOffer(x, product, customOptions, null, MinAmount)).ToList();

            SizeColorPicker = new ProductSizeColorPickerApi(new GetSizeColorPicker(product, null, null).Execute());

            GiftProducts = Gifts ? new GetGiftsApi(ProductId, null).Execute() : null;


            var sizeChart =
                SizeChartService.Get(ProductId, ESizeChartEntityType.Product, true).FirstOrDefault() ??
                SizeChartService.GetFilteredSizeChart(
                    product.CategoryId,
                    ESizeChartEntityType.Category,
                    product.BrandId,
                    SQLDataAccess.Query<int>(
                        "Select PropertyValueId from [Catalog].[ProductPropertyValue] Where ProductId = @ProductId",
                        new { ProductId }).ToList(),
                    true).FirstOrDefault();
            if (sizeChart != null)
                SizeChart = new SizeChartApi(sizeChart);
        }
    }
}