namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Export
{
    public class VkExportProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string UrlPath { get; set; }
        public string Description { get; set; }
        public string BriefDescription { get; set; }
        public string ProductArtNo { get; set; }
        public float Discount { get; set; }
        public float DiscountAmount { get; set; }
        public bool AllowPreOrder { get; set; }
        public bool Enabled { get; set; }
        public float? MinAmount { get; set; }
        public string Unit { get; set; }

        public int OfferId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public string OfferArtNo { get; set; }
        public float Price { get; set; }
        public float Amount { get; set; }
        public bool Main { get; set; }
        public float Weight { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }

        public string ColorName { get; set; }
        public string SizeName { get; set; }

        public float CurrencyValue { get; set; }

        public long VkProductId { get; set; }
        public long VkMainPhotoId { get; set; }
        public string VkPhotoIds { get; set; }
        public string PhotosMapIds { get; set; }

        public string VariantIds { get; set; }
        public long? ItemGroupId { get; set; }
        
        public string CurrentVkProduct { get; set; }
    }
}
