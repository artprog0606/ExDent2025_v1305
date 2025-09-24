using System.Collections.Generic;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvV2Offer
    {
        public int OfferId { get; set; }
        public string ArtNo { get; set; }
        public string Price { get; set; }
        public string PurchasePrice { get; set; }
        public string Amount { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string OfferPhotos { get; set; }
        public string Weight { get; set; }
        public string Dimensions { get; set; }
        public string BarCode { get; set; }
        public Dictionary<string, string> PricesByPriceRule { get; set; }
        public Dictionary<int, string> AmountByWarehouses { get; set; }
    }

    public class ExportFeedCsvV2Category
    {
        public string Path { get; set; }
        public string Sort { get; set; }
    }

    public class ExportFeedCsvV2Product : ExportProductModel
    {
        public List<ExportFeedCsvV2Offer> Offers { get; set; }
        public List<ExportFeedCsvV2Category> Categories { get; set; }
        public string Sorting { get; set; }
        public string Enabled { get; set; }
        public string Currency { get; set; }
        public string Photos { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string Unit { get; set; }
        public string Discount { get; set; }
        public string DiscountAmount { get; set; }
        public string ShippingPrice { get; set; }
        public string Related { get; set; }
        public string Alternative { get; set; }
        public string Videos { get; set; }
        public string MarkerNew { get; set; }
        public string MarkerBestseller { get; set; }
        public string MarkerRecomended { get; set; }
        public string MarkerOnSale { get; set; }
        public string ManualRatio { get; set; }
        public string Producer { get; set; }
        public string OrderByRequest { get; set; }
        public string CustomOptions { get; set; }
        public string YandexSalesNotes { get; set; }
        public string YandexDeliveryDays { get; set; }
        public string YandexTypePrefix { get; set; }
        public string YandexName { get; set; }
        public string YandexModel { get; set; }
        public string YandexSizeUnit { get; set; }
        public string YandexDiscounted { get; set; }
        public string YandexDiscountCondition { get; set; }
        public string YandexDiscountReason { get; set; }
        public string YandexProductQuality { get; set; }
        public string YandexBid { get; set; }
        public string GoogleGtin { get; set; }
        public string GoogleMpn { get; set; }
        public string GoogleProductCategory { get; set; }
        public string GoogleAvailabilityDate { get; set; }
        public string Adult { get; set; }
        public string ManufacturerWarranty { get; set; }
        public string AvitoProductProperties { get; set; }
        public string Tags { get; set; }
        public string Gifts { get; set; }
        public string MinAmount { get; set; }
        public string MaxAmount { get; set; }
        public string Multiplicity { get; set; }
        public string Tax { get; set; }
        public string PaymentSubjectType { get; set; }
        public string PaymentMethodType { get; set; }
        public string ModifiedDate { get; set; }
        public string IsMarkingRequired { get; set; }
        public string DoNotApplyOtherDiscounts { get; set; }
        public string IsDigital { get; set; }
        public string DownloadLink { get; set; }
        public string YandexMarketExpiry { get; set; }
        public string YandexMarketWarrantyDays { get; set; }
        public string YandexMarketCommentWarranty { get; set; }
        public string YandexMarketPeriodOfValidityDays { get; set; }
        public string YandexMarketServiceLifeDays { get; set; }
        public string YandexMarketTnVedCode { get; set; }
        public string YandexMarketStepQuantity { get; set; }
        public string YandexMarketMinQuantity { get; set; }
        public string YandexMarketCategoryId { get; set; }
        public string Comment { get; set; }
        public string AccrueBonuses { get; set; }
    }
}