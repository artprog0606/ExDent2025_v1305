//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum ProductFields
    {
        [StringName("none")]
        [Localize("Core.ExportImport.ProductFields.NotSelected")]
        [CsvFieldsStatus(CsvFieldStatus.None)]
        None,

        [StringName("sku")]
        [Localize("Core.ExportImport.ProductFields.Sku")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Sku,

        [StringName("name")]
        [Localize("Core.ExportImport.ProductFields.Name")]
        [CsvFieldsStatus(CsvFieldStatus.StringRequired)]
        Name,

        [StringName("paramsynonym")]
        [Localize("Core.ExportImport.ProductFields.Synonym")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ParamSynonym,

        [StringName("category")]
        [Localize("Core.ExportImport.ProductFields.Categories")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Category,

        [StringName("sorting")]
        [Localize("Core.ExportImport.ProductFields.Sorting")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Sorting,

        [StringName("enabled")]
        [Localize("Core.ExportImport.ProductFields.Enabled")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Enabled,

        [StringName("currency")]
        [Localize("Core.ExportImport.ProductFields.Currency")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Currency,

        [StringName("price")]
        [Localize("Core.ExportImport.ProductFields.Price")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        Price,

        [StringName("purchaseprice")]
        [Localize("Core.ExportImport.ProductFields.PurchasePrice")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        PurchasePrice,

        [StringName("amount")]
        [Localize("Core.ExportImport.ProductFields.Amount")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        Amount,

        [StringName("sku:size:color:price:purchaseprice:amount")]
        [Localize("Core.ExportImport.ProductFields.MultiOffer")]
        [CsvFieldsStatus(CsvFieldStatus.NotEmptyString)]
        MultiOffer,

        [StringName("unit")]
        [Localize("Core.ExportImport.ProductFields.Unit")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Unit,

        [StringName("discount")]
        [Localize("Core.ExportImport.ProductFields.Discount")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        Discount,

        [StringName("discountamount")]
        [Localize("Core.ExportImport.ProductFields.DiscountAmount")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        DiscountAmount,

        [StringName("shippingprice")]
        [Localize("Core.ExportImport.ProductFields.ShippingPrice")]
        [CsvFieldsStatus(CsvFieldStatus.NullableFloat)]
        ShippingPrice,

        [StringName("yandexdeliverydays")]
        [Localize("Core.ExportImport.ProductFields.YandexDeliveryDays")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexDeliveryDays,

        [StringName("weight")]
        [Localize("Core.ExportImport.ProductFields.Weight")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        Weight,

        [StringName("size")]
        [Localize("Core.ExportImport.ProductFields.Size")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Size,

        [StringName("briefdescription")]
        [Localize("Core.ExportImport.ProductFields.BriefDescription")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        BriefDescription,

        [StringName("description")]
        [Localize("Core.ExportImport.ProductFields.Description")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Description,

        [StringName("title")]
        [Localize("Core.ExportImport.ProductFields.SeoTitle")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Title,

        [StringName("metakeywords")]
        [Localize("Core.ExportImport.ProductFields.MetaKeywords")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        MetaKeywords,

        [StringName("metadescription")]
        [Localize("Core.ExportImport.ProductFields.MetaDescription")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        MetaDescription,

        [StringName("h1")]
        [Localize("Core.ExportImport.ProductFields.H1")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        H1,

        [StringName("photos")]
        [Localize("Core.ExportImport.ProductFields.Photos")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Photos,

        [StringName("videos")]
        [Localize("Core.ExportImport.ProductFields.Videos")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Videos,

        [StringName("markers")]
        [Localize("Core.ExportImport.ProductFields.Markers")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Markers,

        [StringName("properties")]
        [Localize("Core.ExportImport.ProductFields.Properties")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Properties,

        [StringName("producer")]
        [Localize("Core.ExportImport.ProductFields.Producer")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Producer,

        [StringName("preorder")]
        [Localize("Core.ExportImport.ProductFields.PreOrder")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        OrderByRequest,
        
        [StringName("AccrueBonuses")]
        [Localize("Core.ExportImport.ProductFields.AccrueBonuses")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        AccrueBonuses,

        [StringName("salesnote")]
        [Localize("Core.ExportImport.ProductFields.SalesNotes")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        SalesNotes,


        [StringName("related sku")]
        [Localize("Core.ExportImport.ProductFields.RelatedSKU")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Related,

        [StringName("alternative sku")]
        [Localize("Core.ExportImport.ProductFields.AlternativeSKU")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Alternative,

        [StringName("custom options")]
        [Localize("Core.ExportImport.ProductFields.CustomOptions")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        CustomOption,

        [StringName("gtin")]
        [Localize("Core.ExportImport.ProductFields.Gtin")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Gtin,
        
        [StringName("mpn")]
        [Localize("Core.ExportImport.ProductFields.GoogleMpn")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        GoogleMpn,

        [StringName("googleproductcategory")]
        [Localize("Core.ExportImport.ProductFields.GoogleProductCategory")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        GoogleProductCategory,
        
        [StringName("googleavailabilitydate")]
        [Localize("Core.ExportImport.ProductFields.GoogleAvailabilityDate")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        GoogleAvailabilityDate,
        

        [StringName("yandextypeprefix")]
        [Localize("Core.ExportImport.ProductFields.YandexTypePrefix")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexTypePrefix,

        [StringName("yandexname")]
        [Localize("Core.ExportImport.ProductFields.YandexName")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexName,

        [StringName("yandexmodel")]
        [Localize("Core.ExportImport.ProductFields.YandexModel")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexModel,

        [StringName("yandexsizeunit")]
        [Localize("Core.ExportImport.ProductFields.YandexSizeUnit")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexSizeUnit,

        [StringName("yandexdiscounted")]
        [Localize("Core.ExportImport.ProductFields.YandexDiscounted")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexDiscounted,

        [StringName("yandexdiscountcondition")]
        [Localize("Core.ExportImport.ProductFields.YandexDiscountCondition")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexDiscountCondition,

        [StringName("yandexproductquality")]
        [Localize("Core.ExportImport.ProductFields.YandexProductQuality")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexProductQuality,

        [StringName("yandexdiscountreason")]
        [Localize("Core.ExportImport.ProductFields.YandexDiscountReason")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexDiscountReason,

        [StringName("adult")]
        [Localize("Core.ExportImport.ProductFields.Adult")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Adult,

        [StringName("manufacturer_warranty")]
        [Localize("Core.ExportImport.ProductFields.ManufacturerWarranty")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ManufacturerWarranty,

        [StringName("tags")]
        [Localize("Core.ExportImport.ProductFields.Tags")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Tags,

        [StringName("gifts")]
        [Localize("Core.ExportImport.ProductFields.Gifts")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Gifts,

        [StringName("minamount")]
        [Localize("Core.ExportImport.ProductFields.Minamount")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        MinAmount,

        [StringName("maxamount")]
        [Localize("Core.ExportImport.ProductFields.Maxamount")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        MaxAmount,

        [StringName("multiplicity")]
        [Localize("Core.ExportImport.ProductFields.Multiplicity")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        Multiplicity,

        [StringName("bid")]
        [Localize("Core.ExportImport.ProductFields.Bid")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        Bid,

        [StringName("barcode")]
        [Localize("Core.ExportImport.ProductFields.BarCode")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        BarCode,

        [StringName("tax")]
        [Localize("Core.ExportImport.ProductFields.Tax")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Tax,

        [StringName("paymentsubjecttype")]
        [Localize("Core.ExportImport.ProductFields.PaymentSubjectType")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        PaymentSubjectType,

        [StringName("paymentmethodtype")]
        [Localize("Core.ExportImport.ProductFields.PaymentMethodType")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        PaymentMethodType,


        [StringName("externalcategoryid")]
        [Localize("Core.ExportImport.ProductFields.ExternalCategoryId")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        ExternalCategoryId,

        [StringName("avitoproductproperties")]
        [Localize("Core.ExportImport.ProductFields.AvitoProductProperties")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        AvitoProductProperties,

        [StringName("manualratio")]
        [Localize("Core.exportImport.ProductFields.ManualRatio")]
        [CsvFieldsStatus(CsvFieldStatus.Float)]
        ManualRatio,

        [StringName("marking")]
        [Localize("Core.ExportImport.ProductFields.Marking")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        IsMarkingRequired,

        [StringName("donotapplyotherdiscounts")]
        [Localize("Core.ExportImport.ProductFields.DoNotApplyOtherDiscounts")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DoNotApplyOtherDiscounts,
        
        [StringName("isdigital")]
        [Localize("Core.ExportImport.ProductFields.IsDigital")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        IsDigital,
        
        [StringName("downloadlink")]
        [Localize("Core.ExportImport.ProductFields.DownloadLink")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        DownloadLink,

        [StringName("modifieddate")]
        [Localize("Core.ExportImport.ProductFields.ModifiedDate")]
        [CsvFieldsStatus(CsvFieldStatus.NullableDateTime)]
        ModifiedDate,

        [Localize("Core.ExportImport.ProductFields.Comment")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Comment,

        [Localize("Core.ExportImport.ProductFields.FullUrl")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        Url,

        [Localize("Core.ExportImport.ProductFields.SizeChart")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        SizeChart,

        [StringName("yandexmarket expiry")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketExpiry")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexMarketExpiry,
        
        [StringName("yandexmarket warranty-days")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketWarrantyDays")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexMarketWarrantyDays,
        
        [StringName("yandexmarket comment-warranty")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketCommentWarranty")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexMarketCommentWarranty,
        
        [StringName("yandexmarket period-of-validity-days")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketPeriodOfValidityDays")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexMarketPeriodOfValidityDays,
        
        [StringName("yandexmarket service-life-days")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketServiceLifeDays")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexMarketServiceLifeDays,
        
        [StringName("yandexmarket tn-ved-code")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketTnVedCode")]
        [CsvFieldsStatus(CsvFieldStatus.String)]
        YandexMarketTnVedCode,
        
        [StringName("yandexmarket step-quantity")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketStepQuantity")]
        [CsvFieldsStatus(CsvFieldStatus.NullableInt)]
        YandexMarketStepQuantity,

        [StringName("yandexmarket min-quantity")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketMinQuantity")]
        [CsvFieldsStatus(CsvFieldStatus.NullableInt)]
        YandexMarketMinQuantity, 
        
        [StringName("yandexmarket market_category_id")]
        [Localize("Core.ExportImport.ProductFields.YandexMarketCategoryId")]
        [CsvFieldsStatus(CsvFieldStatus.NullableLong)]
        YandexMarketCategoryId, 

        #region Sales Channels

        //[StringName("Store")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelStore")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Store,

        //[StringName("Funnel")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelFunnel")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Funnel,

        //[StringName("Vk")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelVk")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Vk,

        //[StringName("Instagram")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelInstagram")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Instagram,

        //[StringName("YandexMarket")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelYandex")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Yandex,

        //[StringName("Avito")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelAvito")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Avito,

        //[StringName("GoogleMerchantCenter")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelGoogle")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Google,

        //[StringName("Facebook")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelFacebook")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Facebook,

        //[StringName("Bonus")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelBonus")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Bonus,

        //[StringName("Referal")]
        //[Localize("Core.ExportImport.ProductFields.SaleschannelReferal")]
        //[CsvFieldsStatus(CsvFieldStatus.String)]
        //[Active(false)]
        //Referal

        #endregion
    }
}