using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.ExportImport
{
    public class ExportFeedAvitoProduct : ExportProductModel
    {
        public Currency Currency { get; set; }
        public float Price { get; set; }
        public float? PriceByRule { get; set; }
        public float Discount { get; set; }
        public float DiscountAmount { get; set; }
        public string Colors { get; set; }
        public string Sizes { get; set; }
        public string Unit { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public string Videos { get; set; }
        public string Properties { get; set; }
        public string BarCode { get; set; }
        
        public List<Offer> Offers { get; set; }
        public List<ProductPhoto> PhotosList { get; set; }

        private List<int> _photosIds;

        public List<int> PhotosIds
        {
            get => _photosIds ?? (_photosIds = (PhotosList ?? new List<ProductPhoto>())
                                                    .Select(x => x.PhotoId)
                                                    .ToList());
            set => _photosIds = value;
        }

        public List<ExportFeedAvitoProductProperty> AvitoProperties { get; set; }

        
        public static ExportFeedAvitoProduct Create(ExportFeedAvitoProduct product, Offer offer, ExportFeedAvitoOptions options)
        {
            var newProduct = new ExportFeedAvitoProduct()
            {
                ProductId = product.ProductId,
                OfferId = offer.OfferId,
                ArtNo = offer.ArtNo,
                Name = product.Name +
                       (offer.Color != null ? " " + offer.Color.ColorName : "") +
                       (offer.Size != null ? (offer.ColorID != null ? ", " : " ") + offer.Size.SizeName : ""),
                UrlPath = product.UrlPath,
                BriefDescription = product.BriefDescription,
                Description = product.Description,
                Currency = product.Currency,
                Price = offer.BasePrice,
                Colors = product.Colors,
                Sizes = product.Sizes,
                Unit = product.Unit,
                Discount = product.Discount,
                DiscountAmount = product.DiscountAmount,
                Weight = product.Weight,
                Size = product.Size,
                Videos = product.Videos,
                Properties = product.Properties,
                BarCode = product.BarCode,
                PhotosList = product.PhotosList,
                AvitoProperties = product.AvitoProperties
            };

            if (offer.ColorID != null)
            {
                newProduct.PhotosIds = (newProduct.PhotosList ?? new List<ProductPhoto>())
                    .Where(x => x.ColorID == null || x.ColorID == offer.ColorID)
                    .Select(x => x.PhotoId)
                    .ToList();
            }
            
            if (options.PriceRuleId != null)
            {
                var priceRule = 
                    PriceRuleService.GetOfferPriceRules(offer.OfferId)
                        .FirstOrDefault(x => x.PriceRuleId == options.PriceRuleId);
                
                if (priceRule != null)
                    newProduct.PriceByRule = priceRule.PriceByRule;
            }

            return newProduct;
        }
    }
}