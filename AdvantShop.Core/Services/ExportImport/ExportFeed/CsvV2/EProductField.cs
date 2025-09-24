using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum EProductField
    {
        [Localize("Core.ExportImport.EProductField.None")]
        [CsvV2Field(CsvFieldStatus.None)]
        None,

        [Localize("Core.ExportImport.EProductField.Code")]
        [CsvV2Field(CsvFieldStatus.String)]
        Code,

        [Localize("Core.ExportImport.EProductField.Sku")]
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        Sku,

        [Localize("Core.ExportImport.EProductField.Name")]
        [CsvV2Field(CsvFieldStatus.StringRequired)]
        Name,

        #region Offer Fields

        [Localize("Core.ExportImport.EProductField.Price")]
        [CsvV2Field(CsvFieldStatus.Float, IsOfferField = true)]
        Price,

        [Localize("Core.ExportImport.EProductField.PurchasePrice")]
        [CsvV2Field(CsvFieldStatus.Float, IsOfferField = true)]
        PurchasePrice,

        [Localize("Core.ExportImport.EProductField.Amount")]
        [CsvV2Field(CsvFieldStatus.Float, IsOfferField = true)]
        Amount,

        [Localize("Core.ExportImport.EProductField.WarehouseStock")]
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        WarehouseStock,

        [Localize("Core.ExportImport.EProductField.Size")] // Settings.SizesHeader
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        Size,

        [Localize("Core.ExportImport.EProductField.Color")]  // Settings.ColorsHeader
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        Color,

        [Localize("Core.ExportImport.EProductField.OfferPhotos")]
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        OfferPhotos,

        [Localize("Core.ExportImport.EProductField.Weight")]
        [CsvV2Field(CsvFieldStatus.Float, IsOfferField = true)]
        Weight,

        [Localize("Core.ExportImport.EProductField.Dimensions")]
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        Dimensions,

        [Localize("Core.ExportImport.EProductField.BarCode")]
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        BarCode,

        [Localize("Core.ExportImport.EProductField.PriceRule")]
        [CsvV2Field(CsvFieldStatus.String, IsOfferField = true)]
        PriceRule,

        #endregion Offer Fields

        [Localize("Core.ExportImport.EProductField.ParamSynonym")]
        [CsvV2Field(CsvFieldStatus.String)]
        ParamSynonym,

        [Localize("Core.ExportImport.EProductField.Category")] // 1, 2, ...
        [CsvV2Field(CsvFieldStatus.String)]
        Category,

        [Localize("Core.ExportImport.EProductField.Sorting")] // 1, 2, ...
        [CsvV2Field(CsvFieldStatus.Int)]
        Sorting,

        [Localize("Core.ExportImport.EProductField.Enabled")]
        [CsvV2Field(CsvFieldStatus.String)]
        Enabled,

        [Localize("Core.ExportImport.EProductField.Currency")]
        [CsvV2Field(CsvFieldStatus.String)]
        Currency,

        [Localize("Core.ExportImport.EProductField.Photos")]
        [CsvV2Field(CsvFieldStatus.String)]
        Photos,

        [Localize("Core.ExportImport.EProductField.Property")]
        [CsvV2Field(CsvFieldStatus.String)]
        Property,

        [Localize("Core.ExportImport.EProductField.Unit")]
        [CsvV2Field(CsvFieldStatus.String)]
        Unit,

        [Localize("Core.ExportImport.EProductField.Discount")]
        [CsvV2Field(CsvFieldStatus.Float)]
        Discount,

        [Localize("Core.ExportImport.EProductField.DiscountAmount")]
        [CsvV2Field(CsvFieldStatus.Float)]
        DiscountAmount,

        [Localize("Core.ExportImport.EProductField.ShippingPrice")]
        [CsvV2Field(CsvFieldStatus.NullableFloat)]
        ShippingPrice,

        [Localize("Core.ExportImport.EProductField.BriefDescription")]
        [CsvV2Field(CsvFieldStatus.String)]
        BriefDescription,

        [Localize("Core.ExportImport.EProductField.Description")]
        [CsvV2Field(CsvFieldStatus.String)]
        Description,

        [Localize("Core.ExportImport.EProductField.SeoTitle")]
        [CsvV2Field(CsvFieldStatus.String)]
        SeoTitle,

        [Localize("Core.ExportImport.EProductField.SeoMetaKeywords")]
        [CsvV2Field(CsvFieldStatus.String)]
        SeoMetaKeywords,

        [Localize("Core.ExportImport.EProductField.SeoMetaDescription")]
        [CsvV2Field(CsvFieldStatus.String)]
        SeoMetaDescription,

        [Localize("Core.ExportImport.EProductField.SeoH1")]
        [CsvV2Field(CsvFieldStatus.String)]
        SeoH1,

        [Localize("Core.ExportImport.EProductField.Related")] // Settings.RelatedHeader
        [CsvV2Field(CsvFieldStatus.String, IsPostProcessField = true)]
        Related,

        [Localize("Core.ExportImport.EProductField.Alternative")] // Settings.AlternativeHeader
        [CsvV2Field(CsvFieldStatus.String, IsPostProcessField = true)]
        Alternative,

        [Localize("Core.ExportImport.EProductField.Videos")]
        [CsvV2Field(CsvFieldStatus.String)]
        Videos,

        [Localize("Core.ExportImport.EProductField.MarkerNew")]
        [CsvV2Field(CsvFieldStatus.String)]
        MarkerNew,

        [Localize("Core.ExportImport.EProductField.MarkerBestseller")]
        [CsvV2Field(CsvFieldStatus.String)]
        MarkerBestseller,

        [Localize("Core.ExportImport.EProductField.MarkerRecomended")]
        [CsvV2Field(CsvFieldStatus.String)]
        MarkerRecomended,

        [Localize("Core.ExportImport.EProductField.MarkerOnSale")]
        [CsvV2Field(CsvFieldStatus.String)]
        MarkerOnSale,

        [Localize("Core.ExportImport.EProductField.ManualRatio")]
        [CsvV2Field(CsvFieldStatus.Float)]
        ManualRatio,

        [Localize("Core.ExportImport.EProductField.Producer")]
        [CsvV2Field(CsvFieldStatus.String)]
        Producer,

        [Localize("Core.ExportImport.EProductField.OrderByRequest")]
        [CsvV2Field(CsvFieldStatus.String)]
        OrderByRequest,
        
        [Localize("Core.ExportImport.ProductFields.AccrueBonuses")]
        [CsvV2Field(CsvFieldStatus.String)]
        AccrueBonuses,

        [Localize("Core.ExportImport.EProductField.CustomOptions")]
        [CsvV2Field(CsvFieldStatus.String)]
        CustomOptions,

        [Localize("Core.ExportImport.EProductField.Tags")]
        [CsvV2Field(CsvFieldStatus.String)]
        Tags,

        [Localize("Core.ExportImport.EProductField.Gifts")]
        [CsvV2Field(CsvFieldStatus.String, IsPostProcessField = true)]
        Gifts,

        [Localize("Core.ExportImport.EProductField.MinAmount")]
        [CsvV2Field(CsvFieldStatus.Float)]
        MinAmount,

        [Localize("Core.ExportImport.EProductField.MaxAmount")]
        [CsvV2Field(CsvFieldStatus.Float)]
        MaxAmount,

        [Localize("Core.ExportImport.EProductField.Multiplicity")]
        [CsvV2Field(CsvFieldStatus.Float)]
        Multiplicity,

        [Localize("Core.ExportImport.EProductField.Tax")]
        [CsvV2Field(CsvFieldStatus.String)]
        Tax,

        [Localize("Core.ExportImport.EProductField.PaymentSubjectType")]
        [CsvV2Field(CsvFieldStatus.String)]
        PaymentSubjectType,

        [Localize("Core.ExportImport.EProductField.PaymentMethodType")]
        [CsvV2Field(CsvFieldStatus.String)]
        PaymentMethodType,

        [Localize("Core.ExportImport.EProductField.ExternalCategoryId")] // only import
        [CsvV2Field(CsvFieldStatus.String)]
        ExternalCategoryId,

        [Localize("Core.ExportImport.EProductField.Adult")]
        [CsvV2Field(CsvFieldStatus.String)]
        Adult,

        [Localize("Core.ExportImport.EProductField.ManufacturerWarranty")]
        [CsvV2Field(CsvFieldStatus.String)]
        ManufacturerWarranty,

        [Localize("Core.ExportImport.EProductField.Marking")]
        [CsvV2Field(CsvFieldStatus.String)]
        Marking,

        [Localize("Core.ExportImport.EProductField.DoNotApplyOtherDiscounts")]
        [CsvV2Field(CsvFieldStatus.String)]
        DoNotApplyOtherDiscounts,
        
        [Localize("Core.ExportImport.EProductField.IsDigital")]
        [CsvV2Field(CsvFieldStatus.String)]
        IsDigital,
        
        [Localize("Core.ExportImport.EProductFields.DownloadLink")]
        [CsvV2Field(CsvFieldStatus.String)]
        DownloadLink,

        [Localize("Core.ExportImport.EProductField.Comment")]
        [CsvV2Field(CsvFieldStatus.String)]
        Comment,

        [Localize("Core.ExportImport.EProductFields.FullUrl")]
        [CsvV2Field(CsvFieldStatus.String)]
        Url,

        [Localize("Core.ExportImport.EProductFields.SizeChart")]
        [CsvV2Field(CsvFieldStatus.String)]
        SizeChart,

        [Localize("Core.ExportImport.EProductField.YandexSalesNotes")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexSalesNotes,

        [Localize("Core.ExportImport.EProductField.YandexDeliveryDays")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexDeliveryDays,

        [Localize("Core.ExportImport.EProductField.YandexTypePrefix")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexTypePrefix,

        [Localize("Core.ExportImport.EProductField.YandexName")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexName,

        [Localize("Core.ExportImport.EProductField.YandexModel")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexModel,

        [Localize("Core.ExportImport.EProductField.YandexSizeUnit")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexSizeUnit,

        [Localize("Core.ExportImport.EProductField.YandexBid")]
        [CsvV2Field(CsvFieldStatus.Float)]
        YandexBid,

        [Localize("Core.ExportImport.EProductField.YandexDiscounted")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexDiscounted,

        [Localize("Core.ExportImport.EProductField.YandexDiscountCondition")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexDiscountCondition,

        [Localize("Core.ExportImport.EProductField.YandexProductQuality")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexProductQuality,

        [Localize("Core.ExportImport.EProductField.YandexDiscountReason")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexDiscountReason,

        [Localize("Core.ExportImport.EProductField.GoogleGtin")]
        [CsvV2Field(CsvFieldStatus.String)]
        GoogleGtin,

        [Localize("Core.ExportImport.EProductField.GoogleMpn")]
        [CsvV2Field(CsvFieldStatus.String)]
        GoogleMpn,
        
        [Localize("Core.ExportImport.EProductField.GoogleProductCategory")]
        [CsvV2Field(CsvFieldStatus.String)]
        GoogleProductCategory,
        
        [Localize("Core.ExportImport.EProductField.GoogleAvailabilityDate")]
        [CsvV2Field(CsvFieldStatus.String)]
        GoogleAvailabilityDate,

        [Localize("Core.ExportImport.EProductField.AvitoProductProperties")]
        [CsvV2Field(CsvFieldStatus.String)]
        AvitoProductProperties,

        [Localize("Core.ExportImport.ProductFields.ModifiedDate")]
        [CsvV2Field(CsvFieldStatus.NullableDateTime)]
        ModifiedDate,
        
        
        [Localize("Core.ExportImport.EProductField.YandexMarketExpiry")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexMarketExpiry,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketWarrantyDays")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexMarketWarrantyDays,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketCommentWarranty")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexMarketCommentWarranty,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketPeriodOfValidityDays")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexMarketPeriodOfValidityDays,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketServiceLifeDays")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexMarketServiceLifeDays,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketTnVedCode")]
        [CsvV2Field(CsvFieldStatus.String)]
        YandexMarketTnVedCode,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketStepQuantity")]
        [CsvV2Field(CsvFieldStatus.NullableInt)]
        YandexMarketStepQuantity,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketMinQuantity")]
        [CsvV2Field(CsvFieldStatus.NullableInt)]
        YandexMarketMinQuantity,
        
        [Localize("Core.ExportImport.EProductField.YandexMarketCategoryId")]
        [CsvV2Field(CsvFieldStatus.NullableLong)]
        YandexMarketCategoryId,
    }
}