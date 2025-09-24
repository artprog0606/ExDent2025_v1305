using AdvantShop.Catalog;

namespace AdvantShop.ExportImport
{
    public class ExportFeedYandexDirectProduct : ExportProductModel
    {
        public string OfferArtNo { get; set; }
        public int Amount { get; set; }
        public float? AmountByWarehouses { get; set; }
        
        public float Price { get; set; }
        public float? ShippingPrice { get; set; }
        public float SupplyPrice { get; set; }
        public float? PriceByRule { get; set; }
        public float? OldPriceByRule { get; set; }

        public float Discount { get; set; }
        public float DiscountAmount { get; set; }
        public int ParentCategory { get; set; }
        
        public string Photos { get; set; }
        
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        
        public bool Main { get; set; }

        public string YandexSalesNote { get; set; }
        public string YandexTypePrefix { get; set; }
        public string YandexModel { get; set; }
        public bool ManufacturerWarranty { get; set; }
        public string YandexName { get; set; }
        public string YandexDeliveryDays { get; set; }

        public bool Adult { get; set; }

        public float CurrencyValue { get; set; }
        public float Weight { get; set; }
        public bool Enabled { get; set; }

        public string BrandCountry { get; set; }
        public string BrandCountryManufacture { get; set; }        

        public float Bid { get; set; }
        //public float Fee { get; set; }

        public string YandexSizeUnit { get; set; }     
        
        public bool AllowPreorder { get; set; }

        public float MinAmount { get; set; }
        public float Multiplicity { get; set; }

        //public bool Cpa { get; set; }
        public float? Length { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }

        public string BarCode { get; set; }
        public bool IsDigital { get; set; }
        public bool IsMarkingRequired { get; set; }

        public bool YandexProductDiscounted { get; set; }
        public EYandexDiscountCondition YandexProductDiscountCondition { get; set; }
        public string YandexProductDiscountReason { get; set; }
        public EYandexProductQuality YandexProductQuality { get; set; }

        public string YandexMarketExpiry { get; set; }
        public string YandexMarketWarrantyDays { get; set; }
        public string YandexMarketCommentWarranty { get; set; }
        public string YandexMarketPeriodOfValidityDays { get; set; }
        public string YandexMarketServiceLifeDays { get; set; }
        public string YandexMarketTnVedCode { get; set; }
        public string YandexMarketStepQuantity { get; set; }
        public string YandexMarketMinQuantity { get; set; }
        
        public string Unit { get; set; }
    }
}