using System;
using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Areas.Api.Models.Inits
{
    public class CarouselItem
    {
        public int Id { get; }
        public string ImageSrc { get; }
        public string Title { get; }
        public string Text { get; }
        public string ShortDescription { get; }
        public string FullDescription { get; }
        public bool ShowOnMain { get; }
        public string CouponCode { get; }
        public DateTime? ExpirationDate { get; }
        public CarouselItemProduct Product { get; }
        public CarouselItemCategory Category { get; }

        public CarouselItem(CarouselApi carousel)
        {
            Id = carousel.Id;
            ImageSrc = carousel.Picture.ImageSrc();
            Title = !string.IsNullOrWhiteSpace(carousel.Title) ? carousel.Title : null;
            ShortDescription = Text = !string.IsNullOrWhiteSpace(carousel.ShortDescription) ? carousel.ShortDescription : null;
            FullDescription = !string.IsNullOrWhiteSpace(carousel.FullDescription) ? carousel.FullDescription : null;
            CouponCode = !string.IsNullOrWhiteSpace(carousel.CouponCode) ? carousel.CouponCode : null;
            ExpirationDate = carousel.ExpirationDate;
            ShowOnMain = carousel.ShowOnMain;

            var product = carousel.ProductId != null ? ProductService.GetProduct(carousel.ProductId.Value) : null;
            if (product != null && product.Enabled)
                Product = new CarouselItemProduct(product);
            
            var category = carousel.CategoryId != null ? CategoryService.GetCategory(carousel.CategoryId.Value) : null;
            if (category != null && category.Enabled)
                Category = new CarouselItemCategory(category);
        }
    }
    
    public class CarouselItemProduct : ProductItemShort
    {
        public CarouselItemProduct(Product product) : base(product)
        {
        }
    }

    public class CarouselItemCategory
    {
        public int Id { get; }
        public string Name { get; }
        public string Url { get; }
        public string Description { get; }
        public string BriefDescription { get; }
        public string PictureUrl { get; }
        public string MiniPictureUrl { get; }
        
        public CarouselItemCategory(Category category)
        {
            Id = category.CategoryId;
            Name = category.Name;
            Url = category.UrlPath;
            Description = category.Description;
            BriefDescription = category.BriefDescription;

            PictureUrl = category.Picture != null && !string.IsNullOrEmpty(category.Picture.PhotoName)
                ? category.Picture.ImageSrcBig()
                : null;
            MiniPictureUrl = category.MiniPicture != null && !string.IsNullOrEmpty(category.MiniPicture.PhotoName)
                ? category.MiniPicture.ImageSrcSmall()
                : null;
        }
    }
}