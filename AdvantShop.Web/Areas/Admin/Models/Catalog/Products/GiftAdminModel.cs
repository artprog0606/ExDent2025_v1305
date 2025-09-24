using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Models.Catalog.Products
{
    public class GiftAdminModel
    {
        public int ProductId { get; }
        public int? OfferId { get; }
        public int GiftOfferId { get; }
        public int ProductCount { get; }
        
        public string ArtNo { get; }
        public string Name { get; }
        public int? ColorId { get; }
        public int GiftProductId { get; }
        public string Url { get; set; }
        public string ImageSrc { get; set; }
        public List<SelectItemModel<int?>> Offers { get; }
        public string Warning { get; }

        public GiftAdminModel(OfferGift gift)
        {
            ProductId = gift.ProductId;
            GiftOfferId = gift.GiftOfferId;
            OfferId = gift.OfferId;
            ProductCount = gift.ProductCount;
            ArtNo = gift.ArtNo;
            Name = gift.Name;
            ColorId = gift.ColorId;
            GiftProductId = gift.GiftProductId;

            Offers = new List<SelectItemModel<int?>>()
            {
                new SelectItemModel<int?>("Все", null, OfferId == null)
            };

            Offers.AddRange(
                OfferService.GetProductOffers(ProductId)
                    .Select(x => 
                        new SelectItemModel<int?>($"{x.ArtNo} {x.Color?.ColorName} {x.Size?.SizeName}", x.OfferId, OfferId == x.OfferId)));

            var p = ProductService.GetProduct(gift.GiftProductId);
            if (p == null || !p.Enabled || !p.CategoryEnabled)
            {
                Warning = "Товар не активен или без категории и будет недоступен для оформления заказа, в том числе в качестве подарка";
            }
        }
    }
}