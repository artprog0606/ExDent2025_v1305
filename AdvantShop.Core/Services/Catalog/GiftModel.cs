namespace AdvantShop.Catalog
{
    public class GiftModel : Offer
    {
        public int ProductCount { get; set; }
        
        /// <summary>
        /// ProductGifts OfferId
        /// </summary>
        public int? ProductOfferId { get; set; }

        private Offer _productOffer;
        public Offer ProductOffer => _productOffer ?? 
                                     (_productOffer = ProductOfferId != null ? OfferService.GetOffer(ProductOfferId.Value) : null);
    }

    public class ProductGift
    {
        /// <summary>
        /// Id товара, к которому будет применяться подарок
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// Id модификации товара, к которому будет применяться подарок.
        /// Если не указан, то подарок применяется ко всем модификациям товара.
        /// </summary>
        public int? OfferId { get; set; }
        
        /// <summary>
        /// Id модификации подарка
        /// </summary>
        public int GiftOfferId { get; set; }
        
        /// <summary>
        /// Кол-во товара, с которого начинает применяться подарок
        /// </summary>
        public int ProductCount { get; set; }

        public bool Equals(int productId, int giftOfferId, int offerId)
        {
            return ProductId == productId && 
                   GiftOfferId == giftOfferId && 
                   (OfferId == null || OfferId == offerId);
        }
    }

    public class OfferGift : ProductGift
    {
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public int? ColorId { get; set; }
        public int GiftProductId { get; set; }

        private ProductPhoto _photo;
        public ProductPhoto Photo => _photo ?? (_photo = PhotoService.GetMainProductPhoto(GiftProductId, ColorId));

        private Offer _offer;
        public Offer GiftOffer => _offer ?? (_offer = OfferService.GetOffer(GiftOfferId));
    }
}