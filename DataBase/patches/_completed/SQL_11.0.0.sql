ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
	,@sqlMode NVARCHAR(200) = 'GetProducts'
	,@exportAdult BIT
	,@dontExportProductsWithoutDimensionsAndWeight BIT = 0
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @lcategorytemp TABLE (CategoryId INT);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)


	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		if ((Select Opened from @l where CategoryId=@l1)=1)
		begin
			INSERT INTO @lcategorytemp
			SELECT @l1
		end
		else
		begin
	 		INSERT INTO @lcategorytemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @lcategory
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1;

	IF @sqlMode = 'GetCountOfProducts'
	BEGIN
		SELECT COUNT(Distinct OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
	END
	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
		SELECT Distinct tmp.CategoryId
		FROM @lcategorytemp AS tmp
		INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
		WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)
		
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,crossCategory.[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,p.[Name]
			,p.[UrlPath]
			,p.[Description]
			,p.[BriefDescription]
			,p.SalesNote
			,OfferId
			,p.ArtNo
			,[Offer].Main
			,[Offer].ColorID
			,ColorName
			,[Offer].SizeID
			,SizeName
			,BrandName
			,country1.CountryName as BrandCountry
			,country2.CountryName as BrandCountryManufacture
			,GoogleProductCategory
			,YandexMarketCategory
			,YandexTypePrefix
			,YandexModel
			,Gtin
			,Adult
			,CurrencyValue
			,[Settings].PhotoToString(Offer.ColorID, p.ProductId) AS Photos
			,ManufacturerWarranty
			,[Offer].[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,[Offer].BarCode
			,p.Bid			
			,p.YandexSizeUnit
			,p.MinAmount
			,p.Multiplicity			
			,p.YandexName
			,p.YandexDeliveryDays
			,[Offer].Length
			,[Offer].Width
			,[Offer].Height
			,p.YandexProductDiscounted
			,p.YandexProductDiscountCondition
			,p.YandexProductDiscountReason
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		--INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		--RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		cross apply(SELECT TOP (1) [ProductCategories].[CategoryId] from [Catalog].[ProductCategories]
					INNER JOIN  cte lc ON lc.CategoryId = productCategories.CategoryID
					where  [ProductCategories].[ProductID] = p.[ProductID]
					Order By [ProductCategories].Main DESC, [ProductCategories].[CategoryId] ) crossCategory	
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)		
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		Order By p.ProductId
	END
	IF @sqlMode = 'GetOfferIds'
	BEGIN
		SELECT Distinct OfferId
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
	END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.DontExportProductsWithoutDimensionsAndWeight', 'Не выгружать товары без габаритов и веса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.DontExportProductsWithoutDimensionsAndWeight', 'Dont export products without dimensions and weight')

GO--

Update Settings.MailFormatType Set Comment = Replace(Comment, ')', '; #PAYMENTMETHOD#)') where MailType = 'OnBillingLink'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.BrandDiscountRegulation', 'Регулирование скидок на товары по производителю')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.BrandDiscountRegulation', 'Brand discount regulation')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.PriceRegulation.ChangeDiscountByBrandMsg', 'Для товаров выбранного производителя была установлена скидка {0}{1}, кроме товаров с нулевой ценой')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.PriceRegulation.ChangeDiscountByBrandMsg', 'Discount {0}{1} was set for products by selected brand, except for products with zero price')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsReseller.UnloadOnlyMainCategory', 'Выгружать только главную категорию товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsReseller.UnloadOnlyMainCategory', 'Unload only the main product category')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.UnloadProperties', 'Выгружать свойства товара в описании')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.UnloadProperties', 'Unload product properties in the description')

GO--

ALTER TABLE Catalog.Product ADD
	IsMarkingRequired bit NULL
GO--


ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled tinyint,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @SalesNote nvarchar(50),
    @GoogleProductCategory nvarchar(500),
    @YandexMarketCategory nvarchar(500),
    @Gtin nvarchar(50),
    @Adult bit,    
    @CurrencyID int,
    @ActiveView360 bit,
    @ManufacturerWarranty bit,
    @ModifiedBy nvarchar(50),
    @YandexTypePrefix nvarchar(500),
    @YandexModel nvarchar(500),
    @Bid float,
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @YandexSizeUnit nvarchar(10),
    @DateModified datetime,
    @YandexName nvarchar(255),
    @YandexDeliveryDays nvarchar(5),
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
    @YandexProductDiscounted bit,
	@YandexProductDiscountCondition nvarchar(10),
	@YandexProductDiscountReason nvarchar(3000),
	@IsMarkingRequired bit
AS
BEGIN
    DECLARE @Id int,
			@ArtNoUpdateRequired bit

	IF @ArtNo=''
	BEGIN
		SET @ArtNo = CONVERT(nvarchar(100), NEWID())
		SET @ArtNoUpdateRequired = 1
	END

    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
        ,[BriefDescription]
        ,[Description]
        ,[Enabled]
        ,[DateAdded]
        ,[DateModified]
        ,[Recomended]
        ,[New]
        ,[BestSeller]
        ,[OnSale]
        ,[BrandID]
        ,[AllowPreOrder]
        ,[UrlPath]
        ,[Unit]
        ,[ShippingPrice]
        ,[MinAmount]
        ,[MaxAmount]
        ,[Multiplicity]
        ,[HasMultiOffer]
        ,[SalesNote]
        ,GoogleProductCategory
        ,YandexMarketCategory
        ,Gtin
        ,Adult
        ,CurrencyID
        ,ActiveView360
        ,ManufacturerWarranty
        ,ModifiedBy
        ,YandexTypePrefix
        ,YandexModel
        ,Bid
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,YandexSizeUnit
        ,YandexName
        ,YandexDeliveryDays
        ,CreatedBy
        ,Hidden
        ,ManualRatio
        ,YandexProductDiscounted
		,YandexProductDiscountCondition
		,YandexProductDiscountReason
		,IsMarkingRequired
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
        ,@BriefDescription
        ,@Description
        ,@Enabled
        ,@DateModified
        ,@DateModified
        ,@Recomended
        ,@New
        ,@BestSeller
        ,@OnSale
        ,@BrandID
        ,@AllowPreOrder
        ,@UrlPath
        ,@Unit
        ,@ShippingPrice
        ,@MinAmount
        ,@MaxAmount
        ,@Multiplicity
        ,@HasMultiOffer
        ,@SalesNote
        ,@GoogleProductCategory
        ,@YandexMarketCategory
        ,@Gtin
        ,@Adult
        ,@CurrencyID
        ,@ActiveView360
        ,@ManufacturerWarranty
        ,@ModifiedBy
        ,@YandexTypePrefix
        ,@YandexModel
        ,@Bid
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@YandexSizeUnit
        ,@YandexName
        ,@YandexDeliveryDays
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
        ,@YandexProductDiscounted
		,@YandexProductDiscountCondition
		,@YandexProductDiscountReason
		,@IsMarkingRequired
        );

    SET @ID = SCOPE_IDENTITY();
    IF @ArtNoUpdateRequired = 1
    BEGIN
		DECLARE @NewArtNo nvarchar(100) = CONVERT(nvarchar(100),@ID)

        IF EXISTS (SELECT * FROM [Catalog].[Product] WHERE [ArtNo] = @NewArtNo)
        BEGIN
            SET @NewArtNo = @NewArtNo + '_' + SUBSTRING(@ArtNo, 1, 5)
        END

        UPDATE [Catalog].[Product] SET [ArtNo] = @NewArtNo WHERE [ProductID] = @ID
    END
    SELECT @ID
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
    @ProductID int,
    @ArtNo nvarchar(100),
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled bit,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice money,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @SalesNote nvarchar(50),
    @GoogleProductCategory nvarchar(500),
    @YandexMarketCategory nvarchar(500),
    @Gtin nvarchar(50),
    @Adult bit,
    @CurrencyID int,
    @ActiveView360 bit,
    @ManufacturerWarranty bit,
    @ModifiedBy nvarchar(50),
    @YandexTypePrefix nvarchar(500),
    @YandexModel nvarchar(500),
    @Bid float,
    @AccrueBonuses bit,
    @TaxId int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @YandexSizeUnit nvarchar(10),
    @DateModified datetime,
    @YandexName nvarchar(255),
    @YandexDeliveryDays nvarchar(5),
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
    @YandexProductDiscounted bit,
	@YandexProductDiscountCondition nvarchar(10),
	@YandexProductDiscountReason nvarchar(3000),
	@IsMarkingRequired bit
AS
BEGIN
    UPDATE [Catalog].[Product]
    SET 
         [ArtNo] = @ArtNo
        ,[Name] = @Name
        ,[Ratio] = @Ratio
        ,[Discount] = @Discount
        ,[DiscountAmount] = @DiscountAmount
        ,[BriefDescription] = @BriefDescription
        ,[Description] = @Description
        ,[Enabled] = @Enabled
        ,[Recomended] = @Recomended
        ,[New] = @New
        ,[BestSeller] = @BestSeller
        ,[OnSale] = @OnSale
        ,[DateModified] = @DateModified
        ,[BrandID] = @BrandID
        ,[AllowPreOrder] = @AllowPreOrder
        ,[UrlPath] = @UrlPath
        ,[Unit] = @Unit
        ,[ShippingPrice] = @ShippingPrice
        ,[MinAmount] = @MinAmount
        ,[MaxAmount] = @MaxAmount
        ,[Multiplicity] = @Multiplicity
        ,[HasMultiOffer] = @HasMultiOffer
        ,[SalesNote] = @SalesNote
        ,[GoogleProductCategory]=@GoogleProductCategory
        ,[YandexMarketCategory]=@YandexMarketCategory
        ,[Gtin]=@Gtin
        ,[Adult] = @Adult
        ,[CurrencyID] = @CurrencyID
        ,[ActiveView360] = @ActiveView360
        ,[ManufacturerWarranty] = @ManufacturerWarranty
        ,[ModifiedBy] = @ModifiedBy
        ,[YandexTypePrefix] = @YandexTypePrefix
        ,[YandexModel] = @YandexModel
        ,[Bid] = @Bid
        ,[AccrueBonuses] = @AccrueBonuses
        ,[TaxId] = @TaxId
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[YandexSizeUnit] = @YandexSizeUnit
        ,[YandexName] = @YandexName
        ,[YandexDeliveryDays] = @YandexDeliveryDays
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
        ,[YandexProductDiscounted] = @YandexProductDiscounted
		,[YandexProductDiscountCondition] = @YandexProductDiscountCondition
		,[YandexProductDiscountReason] = @YandexProductDiscountReason
		,[IsMarkingRequired] = @IsMarkingRequired
    WHERE ProductID = @ProductID
END

GO--

CREATE TABLE [Order].[MarkingOrderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[Code] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_MarkingOrderItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [Order].[MarkingOrderItem]  WITH CHECK ADD  CONSTRAINT [FK_MarkingOrderItem_OrderItems] FOREIGN KEY([OrderItemId])
REFERENCES [Order].[OrderItems] ([OrderItemID])
ON DELETE CASCADE
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.IsMarkingRequired', 'Обязательная маркировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.IsMarkingRequired', 'Marking')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.Edit.MarkingRequired', 'Обязательная маркировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.Edit.MarkingRequired', 'Marking')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.Edit.MarkingRequiredHint', 'Если в заказ добавлен товар с параметром "Обязательная маркировка", то в карточке заказа у этого товара будет отображаться иконка "Честного Знака"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.Edit.MarkingRequiredHint', 'If a product with this parameter is added to the order, the "Honest Mark" icon will be displayed in the order card for this product')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.Marking', 'Обязательная маркировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.Marking', 'Marking')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Marking', 'Обязательная маркировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Marking', 'Marking')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Купон или подарочный сертификат' WHERE [ResourceKey] = 'Checkout.CheckoutCoupon.CouponTitle' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Coupon or gift Certificate' WHERE [ResourceKey] = 'Checkout.CheckoutCoupon.CouponTitle' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.StaticPages.PageText', 'Текст страницы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.StaticPages.PageText', 'Page text')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.StaticBlock.Content', 'Содержание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.StaticBlock.Content', 'Content')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Users.Validate.EmailOrPhoneExists', 'Данный телефон или почта уже заняты другим покупателем')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Users.Validate.EmailOrPhoneExists', 'This phone or email is already used by another customer')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Основное' WHERE [ResourceKey] = 'Admin.Home.Menu.Main' AND [LanguageId] = 1

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Подключить свой домен' WHERE [ResourceKey] = 'Admin.Dashboard.ChangeDomain' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'How to connect your domain' WHERE [ResourceKey] = 'Admin.Dashboard.ChangeDomain' AND [LanguageId] = 2

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 27)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (27,'Sdek','','RU','Чувашская Республика','','','','Чувашия','','',0,1,0,'','','')



SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF


GO--

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[Files]') AND type in (N'U'))
BEGIN
	DROP TABLE [Settings].[Files]
END

GO--

CREATE TABLE [Settings].[Files](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](max) NOT NULL,
		[Path] [nvarchar](max) NOT NULL,
		[ContentType] [nvarchar](100) NOT NULL,
		[Content] [varbinary](max) NOT NULL,
		[CreatedDate] [datetime] NOT NULL,
		[ModifiedDate] [datetime] NOT NULL,
	 CONSTRAINT [PK_Files_1] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Редактор robots.txt' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Settings.SeoSettings.RobotsTxtHeader'

Update [Settings].[Localization] Set [ResourceValue] = 'Будьте внимательны при редактировании данного файла. Инструкция по работе с <a href="https://www.advantshop.net/help/pages/robots-txt#1" target="_blank">файлом robots.txt</a>' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsSeo.RobotsTxt.BeCareful'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSeo.RobotsTxt.Warning', '<b>Обратите внимание!</b><br> В корне сайта находится файл robots.txt который конфликтует с данными указанными ниже.<br>Пожалуйста удалите файл из корня сайта, и воспользуйтесь механизмами сайта для работы с robots.txt.<br><a href="https://www.advantshop.net/help/pages/robots-txt#1" target="_blank">Подробнее</a>.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSeo.RobotsTxt.Warning', '<b>Pay attention!</b><br> There is a file in the root of the site robots.txt which conflicts with the data specified below.<br>Please delete the file from the root of the site, and use the site''s mechanisms to work with robots.txt.<br><a href="https://www.advantshop.net/help/pages/robots-txt#1" target="_blank">More</a>.')

GO--

If exists (Select 1 From [Settings].[Settings] Where [Name] = 'GoogleAnalyticsNumber' and [Value] <> '' and [Value] like '%UA-%')
	Update [Settings].[Settings] Set [Value] = 'UA-' + Replace([Value], 'UA-', '') Where [Name] = 'GoogleAnalyticsNumber'
	
GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.PaymentMethods.Bill.RequiredPaymentDetails')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Bill.RequiredPaymentDetails', 'Сделать обязательным заполнение ИНН и название компании')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Bill.RequiredPaymentDetails', 'Request details in the client side')
end

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.JobStartHour', 'Время запуска в часах')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.JobStartHour', 'Job start time in hours')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.JobStartMinute', 'Время запуска в минутах')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.JobStartMinute', 'Job start time in minutes')

GO--

UPDATE [Customers].[Region] SET [RegionName] = 'Туркестанская область' WHERE [RegionName] = 'Южно-Казахстанская область'

GO--

IF NOT EXISTS (SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'Checkout.Success.ProceedToPayment')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
		Values
			(1,'Checkout.Success.ProceedToPayment', 'Сейчас вы будете перенаправлены на страницу оплаты <span class="icon-spinner-before icon-animate-spin-before checkout-success-progress"></span><br>Если этого не произошло, нажмите на кнопку ниже'),
			(2,'Checkout.Success.ProceedToPayment', 'You will now be redirected to the payment page <span class="icon-spinner-before icon-animate-spin-before checkout-success-progress"></span><br>If this did not happen, click on the button below')
ELSE
BEGIN
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'Сейчас вы будете перенаправлены на страницу оплаты <span class="icon-spinner-before icon-animate-spin-before checkout-success-progress"></span><br>Если этого не произошло, нажмите на кнопку ниже'
	WHERE [ResourceKey] = 'Checkout.Success.ProceedToPayment' AND [LanguageId] = 1
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'You will now be redirected to the payment page <span class="icon-spinner-before icon-animate-spin-before checkout-success-progress"></span><br>If this did not happen, click on the button below'
	WHERE [ResourceKey] = 'Checkout.Success.ProceedToPayment' AND [LanguageId] = 2
END

GO--

UPDATE [Settings].[TemplateSettings] SET [Value] = 'beauty' WHERE [Template] = '_default' and [Name] = 'Theme' and [Value] = 'beauty2'

GO--

ALTER TABLE [Order].OrderItems ADD
	IsMarkingRequired bit NULL
GO--

ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(255),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
	 @BarCode nvarchar(50),
	 @SupplyPrice float,  
	 @Weight float,  
	 @IsCouponApplied bit,  
	 @Color nvarchar(50),  
	 @Size nvarchar(50),  
	 @DecrementedAmount float,  
	 @PhotoID int,  
	 @IgnoreOrderDiscount bit,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @TaxName nvarchar(50),
	 @TaxType int,
	 @TaxRate float(53),
	 @TaxShowInPrice bit,
	 @Width float,
	 @Height float,
	 @Length float,
	 @PaymentMethodType int,
	 @PaymentSubjectType int,
	 @IsGift bit,
	 @BookingServiceId int,
	 @TypeItem nvarchar(50),
	 @IsMarkingRequired bit
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
	   ,[BarCode]  
	   ,[SupplyPrice]  
	   ,[Weight]  
	   ,[IsCouponApplied]  
	   ,[Color]  
	   ,[Size]  
	   ,[DecrementedAmount]  
	   ,[PhotoID]  
	   ,[IgnoreOrderDiscount]  
	   ,[AccrueBonuses] 
	   ,TaxId
	   ,TaxName
	   ,TaxType
	   ,TaxRate
	   ,TaxShowInPrice
	   ,Width
	   ,Height
	   ,[Length]
	   ,PaymentMethodType
	   ,PaymentSubjectType
	   ,IsGift
	   ,BookingServiceId
	   ,TypeItem
	   ,IsMarkingRequired
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
	   ,@BarCode
	   ,@SupplyPrice  
	   ,@Weight  
	   ,@IsCouponApplied  
	   ,@Color  
	   ,@Size  
	   ,@DecrementedAmount  
	   ,@PhotoID  
	   ,@IgnoreOrderDiscount  
	   ,@AccrueBonuses
	   ,@TaxId
	   ,@TaxName
	   ,@TaxType
	   ,@TaxRate
	   ,@TaxShowInPrice   
	   ,@Width
	   ,@Height
	   ,@Length
	   ,@PaymentMethodType
	   ,@PaymentSubjectType
	   ,@IsGift
	   ,@BookingServiceId
	   ,@TypeItem
	   ,@IsMarkingRequired
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--

Update l

Set l.FirstName = c.FirstName,
    l.LastName = c.LastName,
	l.Patronymic = c.Patronymic,
	l.Email = c.Email,
	l.Phone = c.Phone,
	l.Organization = c.Organization

From [Order].[Lead] as l 
Left Join [Customers].Customer as c on l.CustomerId = c.CustomerID

Where l.CustomerId is not null and 
	  exists(Select 1 From Customers.Customer c Where c.CustomerID = l.CustomerId) and
	  (l.FirstName is null or l.FirstName = '') and 
	  (l.Phone is null or l.Phone = '') and c.Phone is not null and
	  (l.Email is null or l.Email = '') and 
	  (l.LastName is null or l.LastName = '') and 
	  (l.Patronymic is null or l.Patronymic = '') and 
	  (l.Organization is null or l.Organization = '') 
	  
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.Managers', 'Список менеджеров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.Managers', 'Get managers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.CustomerGroups', 'Список групп покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.CustomerGroups', 'Get customer groups')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Files.Index.Warning', '<b>Обратите внимание!</b><br> В корне сайта находятся файлы которые не соответствуют файлам перечисленным ниже.<br>Пожалуйста, удалите ранее загруженные файлы из корня сайта, и воспользуйтесь механизмом ниже для загрузки нужных файлов. <a href="https://www.advantshop.net/help/pages/podtverzhdenie-prava-vladeniya-saitom#3" target="_blank">Подробнее</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Files.Index.Warning', '<b>Please note!</b><br> In the root of the site there are files that do not correspond to the files listed below.<br>Please delete previously downloaded files from the root of the site, and use the mechanism below to download the necessary files. <a href="https://www.advantshop.net/help/pages/podtverzhdenie-prava-vladeniya-saitom#3" target="_blank">Learn more</a>')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.CertificateNotPaid', N'Сертификат не оплачен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.CertificateNotPaid', 'Certificate not paid')

GO--



ALTER PROCEDURE [Settings].[sp_GetCsvProducts] 
    @exportFeedId INT, 
    @onlyCount BIT, 
    @exportNoInCategory BIT, 
    @exportAllProducts BIT, 
    @exportNotAvailable BIT,
	@exportAdult BIT,
	@exportFromMainCategories BIT = 0
AS 
BEGIN
    DECLARE @res TABLE (productid INT PRIMARY KEY CLUSTERED);
    DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED);

    IF (@exportNoInCategory = 1)
    BEGIN
        INSERT INTO @lproductNoCat
            SELECT [productid] 
            FROM [Catalog].product 
            WHERE [productid] NOT IN (SELECT [productid] FROM [Catalog].[productcategories]);
    END

    DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED);
    DECLARE @lcategorytemp TABLE (CategoryId INT);
    DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit);
    
    INSERT INTO @l
        SELECT t.categoryid, t.Opened
        FROM [Settings].[exportfeedselectedcategories] AS t
            INNER JOIN catalog.category ON t.categoryid = category.categoryid
        WHERE [exportfeedid] = @exportFeedId 

    DECLARE @l1 INT
    SET @l1 = (SELECT Min(categoryid) FROM @l);
    WHILE @l1 IS NOT NULL
    BEGIN 
        if ((Select Opened from @l where CategoryId = @l1) = 1)
        begin
            INSERT INTO @lcategorytemp
            SELECT @l1
        end
        else
        begin
            INSERT INTO @lcategorytemp
            SELECT id
            FROM Settings.GetChildCategoryByParent(@l1)
        end

        SET @l1 = (SELECT Min(categoryid) FROM   @l WHERE  categoryid > @l1); 
    END; 

    INSERT INTO @lcategory
        SELECT Distinct tmp.CategoryId
        FROM @lcategorytemp AS tmp

    IF @onlyCount = 1 
    BEGIN 
        SELECT Count(productid) 
        FROM [Catalog].[product] 
        WHERE 
        (
            EXISTS (
                SELECT 1 FROM [Catalog].[productcategories]
                WHERE [productcategories].[productid] = [product].[productid] AND (@exportFromMainCategories = 0 OR [productcategories].[main] = 1)
                AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
            ) OR EXISTS (
                SELECT 1 
                FROM @lproductNoCat AS TEMP
                WHERE  TEMP.productid = [product].[productid]
            ) 
        ) AND (
            @exportAllProducts = 1 
            OR (
                SELECT Count(productid)
                FROM settings.exportfeedexcludedproducts
                WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
            ) = 0
        ) AND (
            Product.Enabled = 1 OR @exportNotAvailable = 1
        ) AND (
            @exportNotAvailable = 1
            OR EXISTS (
                SELECT 1
                FROM [Catalog].[Offer] o
                Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0
            )
        ) AND (
			@exportAdult = 1
			OR (
				Product.Adult = 0
			)
		)
    END
    ELSE
    BEGIN
        SELECT *
        FROM [Catalog].[product]
            LEFT JOIN [Catalog].[photo] ON [photo].[objid] = [product].[productid] AND type = 'Product' AND photo.[main] = 1
        WHERE
        (
            EXISTS (
                SELECT 1
                FROM [Catalog].[productcategories]
                WHERE [productcategories].[productid] = [product].[productid] AND (@exportFromMainCategories = 0 OR [productcategories].[main] = 1)
                    AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
            ) OR EXISTS (
                SELECT 1
                FROM @lproductNoCat AS TEMP
                WHERE TEMP.productid = [product].[productid]
            )
        ) AND (
            @exportAllProducts = 1
            OR (
                SELECT Count(productid)
                FROM settings.exportfeedexcludedproducts
                WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
            ) = 0
        ) AND (
            Product.Enabled = 1 OR @exportNotAvailable = 1
        ) AND (
            @exportNotAvailable = 1
            OR EXISTS (
                SELECT 1
                FROM [Catalog].[Offer] o
                Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0
            )
        ) AND (
			@exportAdult = 1
			OR (
				Product.Adult = 0
			)
		)
    END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsReseller.ExportFromMainCategories', N'Выгружать товары, у которых выбранная категория являются основной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsReseller.ExportFromMainCategories', 'Unload products for which the selected category is the main')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.PriceRule', 'Типы цен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.PriceRule', 'Price rules')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Street', N'улица')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Street', 'street')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.House', N'д.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.House', 'house')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Structure', N'стр.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Structure', 'structure')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Apartment', N'кв.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Apartment', 'ap')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Entrance', N'подъезд')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Entrance', 'entrance')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Floor', N'эт.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Floor', 'floor')

GO--

if not exists (Select 1 From [Settings].[MailFormat] 
			   Inner Join  [Settings].[MailFormatType] On [MailFormatType].[MailFormatTypeId] = [MailFormat].[MailFormatTypeId] 
			   WHERE MailType = 'OnPreOrder')
begin
	Insert Into [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId]) 
	Select 'Под заказ', [FormatText], [MailFormat].[SortOrder], 1, getdate(), getdate(), 'Заказ № #ORDER_ID# (под заказ)', 
			(SELECT top(1) MailFormatTypeID FROM [Settings].[MailFormatType] WHERE MailType = 'OnPreOrder')
	
	From [Settings].[MailFormat]
	Inner Join  [Settings].[MailFormatType] On [MailFormatType].[MailFormatTypeId] = [MailFormat].[MailFormatTypeId] 
	WHERE MailType = 'OnBuyInOneClick'
end

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Перейти' WHERE [ResourceKey] = 'Admin.Design.Index.Install' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Move' WHERE [ResourceKey] = 'Admin.Design.Index.Install' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Установить' WHERE [ResourceKey] = 'Admin.Design.Index.Install' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Install' WHERE [ResourceKey] = 'Admin.Design.Index.Install' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выводить таблицу с кол-вом и ценой в карточке товара' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ShowAmountsTableInProduct' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.PriceFormat.OldPrice', 'Старая цена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.PriceFormat.OldPrice', 'Old price')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.PriceFormat.NewPrice', 'Новая цена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.PriceFormat.NewPrice', 'New price')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ShippingMethods.Common.ShippingMethodName', 'Название метода доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ShippingMethods.Common.ShippingMethodName', 'Shipping method name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ShippingMethods.Common.ShippingMethodManualRate', 'Стоимость метода доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ShippingMethods.Common.ShippingMethodManualRate', 'Shipping method cost')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.LogoSvg', 'Логотип на главной странице в фомате SVG')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.LogoSvg', 'Logo on main page in SVG format')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.LogoSvgRecommendations', '"Логотип в формате SVG" заменяет собой "Логотип на главной странице" и  показывается только на витрине магазина. В письмах и остальных случаях будет использоваться логотип, указанный выше.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.LogoSvgRecommendations', 'The SVG logo is shown only in store. In Mails and other cases, the logo specified above will be used.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.FaviconSvg', 'Favicon в адресной строке браузера в фомате SVG')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.FaviconSvg', 'Favicon in SVG format')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Catalog.AmountTable.Amount', 'Количество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Catalog.AmountTable.Amount', 'Amount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Catalog.AmountTable.Price', 'Цена за ед.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Catalog.AmountTable.Price', 'Price')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Catalog.AmountTable.More', ' и более')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Catalog.AmountTable.More', ' and more')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.OrderConfirmation.DiscountByPaymentRule', 'Скидка при оплате данным способом {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.OrderConfirmation.DiscountByPaymentRule', 'Discount {0}')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.WhoAllowReviews', 'Разрешить оставлять отзывы к товарам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.WhoAllowReviews', 'Who can add reviews to products')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.WhoAllowReviews.All', 'Всем')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.WhoAllowReviews.All', 'All')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.WhoAllowReviews.BoughtUser', 'Пользователям, которые заказывали этот товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.WhoAllowReviews.BoughtUser', 'Users, who ordered this product')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.WhoAllowReviews.Registered', 'Зарегистрированным пользователям')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.WhoAllowReviews.Registered', 'Registered users')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.Product.WhoCanAddReviews', 'Опция определяет, кому разрешить добавление отзывов к товарам.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.Product.WhoCanAddReviews', 'The option determines who is allowed to add reviews to products.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.RightPanel.ShowOnMainPageNoteTitle', 'Выводить на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.RightPanel.ShowOnMainPageNoteTitle', 'Output on main page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.RightPanel.ShowOnMainPageNoteText', 'Настройка актуальна для шаблона Modern и других шаблонов, для которых в трансформере дизайна есть настройка главной страницы "Отображать категории на главной".')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.RightPanel.ShowOnMainPageNoteText', 'The setting is relevant for the Modern template and other templates for which the design transformer has a "Display categories on the main page" setting.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SmsTemplate.IncreaseWarning', '(При использовании переменных число смс может значительно увеличиться)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SmsTemplate.IncreaseWarning', '(When using variables, the number of SMS can increase significantly)')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.ActivatePartners.Immediately', 'Сразу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Partners.ActivatePartners.Immediately', 'Immediately')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.ActivatePartners.ManualCheck', 'После ручной проверки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Partners.ActivatePartners.ManualCheck', 'After manual check')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.ActivatePartners', 'Активировать партнера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Partners.ActivatePartners', 'Activate partner')

GO--
                                                                             
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.Additional', 'Дополнительно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.Additional', 'Additional')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.CategoryFields.ShowOnMainPage', 'Выводить на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.CategoryFields.ShowOnMainPage', 'Show on main page')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.saveOkMarketExportSettings.ExportPreorderProducts', 'Выгружать товары, доступные только под заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.saveOkMarketExportSettings.ExportPreorderProducts', 'Unload goods available only on order')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.Password', 'Пароль')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.Password', 'Password')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.StandardPhone', 'Телефон в числовом виде')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.StandardPhone', 'Phone in numeric format')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.SubscribedForNews', 'Подписка на новости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.SubscribedForNews', 'News subscription')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.BonusCardNumber', 'Бонусная карта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.BonusCardNumber', 'Bonus card number')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.AdminComment', 'Комментарий администратора')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.AdminComment', 'Admin сomment')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.Rating', 'Рейтиинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.Rating', 'Rating')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.Role', 'Роль')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.Role', 'Role')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.CustomerGroup', 'Группа покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.CustomerGroup', 'CustomerGroup')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.Contacts', 'Контакты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.Contacts', 'Contacts')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.IsManager', 'Является менеджером?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.IsManager', 'Is manager')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.Manager', 'Менеджера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.Manager', 'Manager')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.Enabled', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.Enabled', 'Enabled')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.BirthDay', 'День рождения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.BirthDay', 'Birthday')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.Code', 'Код клиента')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.Code', 'Code')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.SortOrder', 'Сортировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.SortOrder', 'SortOrder')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.ClientStatus', 'Статус')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.ClientStatus', 'Client status')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerHistory.CustomerCreated', 'Создан покупатель {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerHistory.CustomerCreated', 'Customer created {0}')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerHistory.CustomerDeleted', 'Удален покупатель {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerHistory.CustomerDeleted', 'Customer deleted {0}')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Name', 'Название контакта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Name', 'Contat name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Country', 'Страна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Country', 'Country')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.District', 'Район')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.District', 'District')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Region', 'Регион')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Region', 'Region')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.Zip', 'Индекс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.Zip', 'Zip')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerHistory.CustomerContactDeleted', 'Удален контакт покупателя {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerHistory.CustomerContactDeleted', 'Customer contact deleted {0}')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.WhoAllowReviews.None', 'Никому')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.WhoAllowReviews.None', 'None')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отображать отзывы к товарам' WHERE [ResourceKey] = 'Admin.Settings.Catalog.AllowReviews' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Display product reviews' WHERE [ResourceKey] = 'Admin.Settings.Catalog.AllowReviews' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Review.AllowOnlyBoughtUser', 'Отзывы могут оставлять только авторизованные пользователи, которые покупали данный товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Review.AllowOnlyBoughtUser', 'Reviews can be left only by authorized users who bought this product')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Review.AllowOnlyRegistered', 'Отзывы могут оставлять только авторизованные пользователи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Review.AllowOnlyRegistered', 'Reviews can only be posted by authorized users')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.PriceRule', 'Тип цены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.PriceRule', 'Price type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.PriceRule.Help', 'Если выбран тип цены, то будут экспортироваться товары, у которых есть цены с данным типом. Будет выгружаться указанная цена без скидок.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.PriceRule.Help', 'If the price type is selected, the products that have prices with this type will be exported')

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
	,@sqlMode NVARCHAR(200) = 'GetProducts'
	,@exportAdult BIT
	,@dontExportProductsWithoutDimensionsAndWeight BIT = 0
	,@priceRuleId int = 0
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @lcategorytemp TABLE (CategoryId INT);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)


	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		if ((Select Opened from @l where CategoryId=@l1)=1)
		begin
			INSERT INTO @lcategorytemp
			SELECT @l1
		end
		else
		begin
	 		INSERT INTO @lcategorytemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @lcategory
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1;

	IF @sqlMode = 'GetCountOfProducts'
	BEGIN
		SELECT COUNT(Distinct offer.OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId	
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId													
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
	END

	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
		SELECT Distinct tmp.CategoryId
		FROM @lcategorytemp AS tmp
		INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
		WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)
		
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,crossCategory.[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,p.[Name]
			,p.[UrlPath]
			,p.[Description]
			,p.[BriefDescription]
			,p.SalesNote
			,[Offer].OfferId
			,p.ArtNo
			,[Offer].Main
			,[Offer].ColorID
			,ColorName
			,[Offer].SizeID
			,SizeName
			,BrandName
			,country1.CountryName as BrandCountry
			,country2.CountryName as BrandCountryManufacture
			,GoogleProductCategory
			,YandexMarketCategory
			,YandexTypePrefix
			,YandexModel
			,Gtin
			,Adult
			,CurrencyValue
			,[Settings].PhotoToString(Offer.ColorID, p.ProductId) AS Photos
			,ManufacturerWarranty
			,[Offer].[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,[Offer].BarCode
			,p.Bid			
			,p.YandexSizeUnit
			,p.MinAmount
			,p.Multiplicity			
			,p.YandexName
			,p.YandexDeliveryDays
			,[Offer].Length
			,[Offer].Width
			,[Offer].Height
			,p.YandexProductDiscounted
			,p.YandexProductDiscountCondition
			,p.YandexProductDiscountReason
			,opr.PriceByRule
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		cross apply(SELECT TOP (1) [ProductCategories].[CategoryId] from [Catalog].[ProductCategories]
					INNER JOIN  cte lc ON lc.CategoryId = productCategories.CategoryID
					where  [ProductCategories].[ProductID] = p.[ProductID]
					Order By [ProductCategories].Main DESC, [ProductCategories].[CategoryId] ) crossCategory	
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)		
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
		Order By p.ProductId
	END

	IF @sqlMode = 'GetOfferIds'
	BEGIN
		SELECT Distinct offer.OfferId
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId	
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
	END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Contact.City', 'Город')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Contact.City', 'City')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.Customer.PasswordChanged', 'Пароль изменился')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.Customer.PasswordChanged', 'Password changed')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SmsCount.Characters', 'Символов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SmsCount.Characters', 'Characters')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Разрешить оставлять отзывы и рейтинг к товарам' WHERE [ResourceKey] = 'Admin.Settings.Catalog.WhoAllowReviews' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Allow to leave reviews and ratings of products' WHERE [ResourceKey] = 'Admin.Settings.Catalog.WhoAllowReviews' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, кому разрешить добавление отзывов и рейтинга к товарам.' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.WhoCanAddReviews' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines who is allowed to add reviews and ratings to products.' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.WhoCanAddReviews' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Супервайзер проекта' WHERE [ResourceKey] = 'Admin.Js.Taskgroups.AddEdit.Observers' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Project supervisor' WHERE [ResourceKey] = 'Admin.Js.Taskgroups.AddEdit.Observers' AND [LanguageId] = 2

GO--


CREATE TABLE [Catalog].[ProductExportOptions](
	[ProductId] [int] NOT NULL,
	[Adult] [bit] NOT NULL,
	[Gtin] [nvarchar](50) NULL,
	[GoogleProductCategory] [nvarchar](500) NULL,
	[YandexSalesNote] [nvarchar](50) NULL,
	[YandexTypePrefix] [nvarchar](500) NULL,
	[YandexModel] [nvarchar](500) NULL,
	[YandexName] [nvarchar](255) NULL,
	[YandexDeliveryDays] [nvarchar](5) NULL,
	[YandexSizeUnit] [nvarchar](10) NULL,
	[YandexProductDiscounted] [bit] NULL,
	[YandexProductDiscountCondition] [nvarchar](10) NULL,
	[YandexProductDiscountReason] [nvarchar](3000) NULL,
	[YandexMarketCategory] [nvarchar](500) NULL,
	[ManufacturerWarranty] [bit] NULL,
	[Bid] [float] NULL
) ON [PRIMARY]

GO--

ALTER TABLE [Catalog].[ProductExportOptions] ADD  CONSTRAINT [DF_ProductExportOptions_Adult]  DEFAULT ((0)) FOR [Adult]

GO--

ALTER TABLE [Catalog].[ProductExportOptions]  WITH CHECK ADD  CONSTRAINT [FK_ProductExportOptions_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE

GO--

ALTER TABLE [Catalog].[ProductExportOptions] CHECK CONSTRAINT [FK_ProductExportOptions_Product]

GO--

INSERT INTO [Catalog].[ProductExportOptions]
           ([ProductId]
           ,[Adult]
           ,[Gtin]
           ,[GoogleProductCategory]
           ,[YandexSalesNote]
           ,[YandexTypePrefix]
           ,[YandexModel]
           ,[YandexName]
           ,[YandexDeliveryDays]
           ,[YandexSizeUnit]
           ,[YandexProductDiscounted]
           ,[YandexProductDiscountCondition]
           ,[YandexProductDiscountReason]
           ,[YandexMarketCategory]
           ,[ManufacturerWarranty]
           ,[Bid])
SELECT		p.[ProductId]
           ,p.[Adult]
           ,p.[Gtin]
           ,p.[GoogleProductCategory]
           ,p.[SalesNote]
           ,p.[YandexTypePrefix]
           ,p.[YandexModel]
           ,p.[YandexName]
           ,p.[YandexDeliveryDays]
           ,p.[YandexSizeUnit]
           ,p.[YandexProductDiscounted]
           ,p.[YandexProductDiscountCondition]
           ,p.[YandexProductDiscountReason]
           ,p.[YandexMarketCategory]
           ,p.[ManufacturerWarranty]
           ,p.[Bid]
FROM [Catalog].[Product] p

GO--

ALTER TABLE Catalog.Product
	DROP CONSTRAINT DF_Product_Adult

GO--

ALTER TABLE Catalog.Product
	DROP COLUMN SalesNote, GoogleProductCategory, Gtin, Adult, YandexMarketCategory, 
         ManufacturerWarranty, YandexTypePrefix, YandexModel, Bid, Fee, YandexSizeUnit, 
         YandexName, Cpa, YandexDeliveryDays, YandexProductDiscounted, YandexProductDiscountCondition, 
         YandexProductDiscountReason

GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled tinyint,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,     
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),   
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit
AS
BEGIN
    DECLARE @Id int,
			@ArtNoUpdateRequired bit

	IF @ArtNo=''
	BEGIN
		SET @ArtNo = CONVERT(nvarchar(100), NEWID())
		SET @ArtNoUpdateRequired = 1
	END

    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
        ,[BriefDescription]
        ,[Description]
        ,[Enabled]
        ,[DateAdded]
        ,[DateModified]
        ,[Recomended]
        ,[New]
        ,[BestSeller]
        ,[OnSale]
        ,[BrandID]
        ,[AllowPreOrder]
        ,[UrlPath]
        ,[Unit]
        ,[ShippingPrice]
        ,[MinAmount]
        ,[MaxAmount]
        ,[Multiplicity]
        ,[HasMultiOffer]
        ,CurrencyID
        ,ActiveView360
        ,ModifiedBy
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,CreatedBy
        ,Hidden
        ,ManualRatio
		,IsMarkingRequired
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
        ,@BriefDescription
        ,@Description
        ,@Enabled
        ,@DateModified
        ,@DateModified
        ,@Recomended
        ,@New
        ,@BestSeller
        ,@OnSale
        ,@BrandID
        ,@AllowPreOrder
        ,@UrlPath
        ,@Unit
        ,@ShippingPrice
        ,@MinAmount
        ,@MaxAmount
        ,@Multiplicity
        ,@HasMultiOffer
        ,@CurrencyID
        ,@ActiveView360
        ,@ModifiedBy
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
		,@IsMarkingRequired
        );

    SET @ID = SCOPE_IDENTITY();
    IF @ArtNoUpdateRequired = 1
    BEGIN
		DECLARE @NewArtNo nvarchar(100) = CONVERT(nvarchar(100),@ID)

        IF EXISTS (SELECT * FROM [Catalog].[Product] WHERE [ArtNo] = @NewArtNo)
        BEGIN
            SET @NewArtNo = @NewArtNo + '_' + SUBSTRING(@ArtNo, 1, 5)
        END

        UPDATE [Catalog].[Product] SET [ArtNo] = @NewArtNo WHERE [ProductID] = @ID
    END
    SELECT @ID
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
    @ProductID int,
    @ArtNo nvarchar(100),
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled bit,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice money,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),
    @AccrueBonuses bit,
    @TaxId int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit
AS
BEGIN
    UPDATE [Catalog].[Product]
    SET 
         [ArtNo] = @ArtNo
        ,[Name] = @Name
        ,[Ratio] = @Ratio
        ,[Discount] = @Discount
        ,[DiscountAmount] = @DiscountAmount
        ,[BriefDescription] = @BriefDescription
        ,[Description] = @Description
        ,[Enabled] = @Enabled
        ,[Recomended] = @Recomended
        ,[New] = @New
        ,[BestSeller] = @BestSeller
        ,[OnSale] = @OnSale
        ,[DateModified] = @DateModified
        ,[BrandID] = @BrandID
        ,[AllowPreOrder] = @AllowPreOrder
        ,[UrlPath] = @UrlPath
        ,[Unit] = @Unit
        ,[ShippingPrice] = @ShippingPrice
        ,[MinAmount] = @MinAmount
        ,[MaxAmount] = @MaxAmount
        ,[Multiplicity] = @Multiplicity
        ,[HasMultiOffer] = @HasMultiOffer
        ,[CurrencyID] = @CurrencyID
        ,[ActiveView360] = @ActiveView360
        ,[ModifiedBy] = @ModifiedBy
        ,[AccrueBonuses] = @AccrueBonuses
        ,[TaxId] = @TaxId
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
		,[IsMarkingRequired] = @IsMarkingRequired
    WHERE ProductID = @ProductID
END

GO--

ALTER PROCEDURE [Settings].[sp_GetCsvProducts] 
    @exportFeedId INT, 
    @onlyCount BIT, 
    @exportNoInCategory BIT, 
    @exportAllProducts BIT, 
    @exportNotAvailable BIT,
	@exportAdult BIT,
	@exportFromMainCategories BIT = 0
AS 
BEGIN
    DECLARE @res TABLE (productid INT PRIMARY KEY CLUSTERED);
    DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED);

    IF (@exportNoInCategory = 1)
    BEGIN
        INSERT INTO @lproductNoCat
            SELECT [productid] 
            FROM [Catalog].product 
            WHERE [productid] NOT IN (SELECT [productid] FROM [Catalog].[productcategories]);
    END

    DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED);
    DECLARE @lcategorytemp TABLE (CategoryId INT);
    DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit);
    
    INSERT INTO @l
        SELECT t.categoryid, t.Opened
        FROM [Settings].[exportfeedselectedcategories] AS t
            INNER JOIN catalog.category ON t.categoryid = category.categoryid
        WHERE [exportfeedid] = @exportFeedId 

    DECLARE @l1 INT
    SET @l1 = (SELECT Min(categoryid) FROM @l);
    WHILE @l1 IS NOT NULL
    BEGIN 
        if ((Select Opened from @l where CategoryId = @l1) = 1)
        begin
            INSERT INTO @lcategorytemp
            SELECT @l1
        end
        else
        begin
            INSERT INTO @lcategorytemp
            SELECT id
            FROM Settings.GetChildCategoryByParent(@l1)
        end

        SET @l1 = (SELECT Min(categoryid) FROM   @l WHERE  categoryid > @l1); 
    END; 

    INSERT INTO @lcategory
        SELECT Distinct tmp.CategoryId
        FROM @lcategorytemp AS tmp

    IF @onlyCount = 1 
    BEGIN 
        SELECT Count(Product.ProductId) 
        FROM [Catalog].[Product] 
		LEFT JOIN [Catalog].[ProductExportOptions] ON [ProductExportOptions].[ProductId] = [Product].[ProductID] 
        WHERE 
        (
            EXISTS (
                SELECT 1 FROM [Catalog].[productcategories]
                WHERE [productcategories].[productid] = [product].[productid] AND (@exportFromMainCategories = 0 OR [productcategories].[main] = 1)
                AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
            ) OR EXISTS (
                SELECT 1 
                FROM @lproductNoCat AS TEMP
                WHERE  TEMP.productid = [product].[productid]
            ) 
        ) AND (
            @exportAllProducts = 1 
            OR (
                SELECT Count(productid)
                FROM settings.exportfeedexcludedproducts
                WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
            ) = 0
        ) AND (
            Product.Enabled = 1 OR @exportNotAvailable = 1
        ) AND (
            @exportNotAvailable = 1
            OR EXISTS (
                SELECT 1
                FROM [Catalog].[Offer] o
                Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0
            )
        ) AND (
			@exportAdult = 1
			OR (
				ProductExportOptions.Adult = 0
			)
		)
    END
    ELSE
    BEGIN
        SELECT *
        FROM [Catalog].[product]
			LEFT JOIN [Catalog].[ProductExportOptions] ON [ProductExportOptions].[ProductId] = [Product].[ProductID] 
            LEFT JOIN [Catalog].[photo] ON [photo].[objid] = [product].[productid] AND type = 'Product' AND photo.[main] = 1
        WHERE
        (
            EXISTS (
                SELECT 1
                FROM [Catalog].[productcategories]
                WHERE [productcategories].[productid] = [product].[productid] AND (@exportFromMainCategories = 0 OR [productcategories].[main] = 1)
                    AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
            ) OR EXISTS (
                SELECT 1
                FROM @lproductNoCat AS TEMP
                WHERE TEMP.productid = [product].[productid]
            )
        ) AND (
            @exportAllProducts = 1
            OR (
                SELECT Count(productid)
                FROM settings.exportfeedexcludedproducts
                WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
            ) = 0
        ) AND (
            Product.Enabled = 1 OR @exportNotAvailable = 1
        ) AND (
            @exportNotAvailable = 1
            OR EXISTS (
                SELECT 1
                FROM [Catalog].[Offer] o
                Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0
            )
        ) AND (
			@exportAdult = 1
			OR (
				ProductExportOptions.Adult = 0
			)
		)
    END
END

GO--


ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
	Begin
		SELECT TOP(@rowsCount) Product.ProductID, Product.Name, Product.UrlPath, Product.AllowPreOrder, Ratio, ManualRatio, isnull(PhotoNameSize1, PhotoName) as PhotoName,
				[Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue, 
				Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, Colors, NotSamePrices as MultiPrices
		
		FROM [Customers].RecentlyViewsData
		
		Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
		Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
		Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
		Left Join [Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]
		Left Join [Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]

		WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1
		
		ORDER BY ViewDate Desc
	End
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.Comment', N'Комментарий'),
           (2,'Js.CheckOrder.Comment', 'Comment')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.OrderStatus', N'Статус посылки'),
           (2,'Js.CheckOrder.OrderStatus', 'Order status')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.ShowAll', N'Показать все'),
           (2,'Js.CheckOrder.ShowAll', 'Show all')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.HideHistory', N'Скрыть историю'),
           (2,'Js.CheckOrder.HideHistory', 'Hide history')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.Address', N'Адрес доставки'),
           (2,'Js.CheckOrder.Address', 'Address')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.ShowOnMap', N'Показать на карте'),
           (2,'Js.CheckOrder.ShowOnMap', 'Show on map')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.WorkHours', N'Время работы'),
           (2,'Js.CheckOrder.WorkHours', 'Work hours')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.Phone', N'Телефон'),
           (2,'Js.CheckOrder.Phone', 'Phone')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.NoDetails', N'Нет детальных данных о статусе заказа'),
           (2,'Js.CheckOrder.NoDetails', 'No details on order status')
           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.CheckOrder.LoadingData', N'Загрузка данных'),
           (2,'Js.CheckOrder.LoadingData', 'Loading data')

GO--

ALTER TABLE [Order].[Lead] ADD
	[ShippingTaxType] int NULL,
	[AvailablePaymentCashOnDelivery] bit NULL,
	[AvailablePaymentPickPoint] bit NULL,
	[ShippingPaymentMethodType] int NOT NULL CONSTRAINT DF_Lead_ShippingPaymentMethodType DEFAULT ((1)),
	[ShippingPaymentSubjectType] int NOT NULL CONSTRAINT DF_Lead_ShippingPaymentSubjectType DEFAULT ((10))

GO--

UPDATE Settings.Localization SET ResourceValue = 'Доступны переменные о заказе и покупателе: <br> #ORDER_ID#, #ORDER_SUM#, #SHIPPING_SUM#, #CUSTOMER_EMAIL#, #CUSTOMER_FIRSTNAME#, #CUSTOMER_LASTNAME#, #CUSTOMER_PHONE#, #CUSTOMER_ID#<br><br> Строка товара помещается между тегами "&lt;&lt;" и "&gt;&gt;" и доступны переменные: #PRODUCT_ARTNO#, #PRODUCT_NAME#,  #PRODUCT_PRICE#, #PRODUCT_PRICE_RAW#, #PRODUCT_AMOUNT#' WHERE ResourceKey = 'Admin.Settings.Checkout.CheckoutSuccessScriptVariables' AND LanguageId = 1
UPDATE Settings.Localization SET ResourceValue = 'Available variables for the order and the customer:<br /> #ORDER_ID#, #ORDER_SUM#, #SHIPPING_SUM#, #CUSTOMER_EMAIL#, #CUSTOMER_FIRSTNAME#, #CUSTOMER_LASTNAME#, #CUSTOMER_PHONE#, #CUSTOMER_ID#<br /><br /> Product line is placed between "<<" and ">>" tags. Available variables for products: #PRODUCT_ARTNO#, #PRODUCT_NAME#,  #PRODUCT_PRICE#, #PRODUCT_PRICE_RAW#, #PRODUCT_AMOUNT#' WHERE ResourceKey = 'Admin.Settings.Checkout.CheckoutSuccessScriptVariables' AND LanguageId = 2

GO--

declare @MoskowRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Москва')
declare @MoskowOblRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Московская область')

IF (@MoskowRegionId IS NOT NULL AND @MoskowOblRegionId IS NOT NULL)
BEGIN
	UPDATE [Customers].[City]
	   SET [RegionID] = @MoskowRegionId
	WHERE [CityName] = 'Щербинка' AND [RegionID] = @MoskowOblRegionId
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'PaymentStatus.Success.MetaTitle', N'Успешная оплата заказа'),
    (2,'PaymentStatus.Success.MetaTitle', 'Successful order payment')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'PaymentStatus.Fail.MetaTitle', N'Оплата заказа не удалась'),
    (2,'PaymentStatus.Fail.MetaTitle', 'Order payment faild')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'PaymentStatus.Cancel.MetaTitle', N'Оплата заказа отменена'),
    (2,'PaymentStatus.Cancel.MetaTitle', 'Order payment cancelled')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SeoSettings.BrandsDefaultH1', 'Заголовок H1')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SeoSettings.BrandsDefaultH1', 'H1 header')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSeo.SeoSettings.ResetMetaListBrands', 'Сбросить мета информацию для списка брендов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSeo.SeoSettings.ResetMetaListBrands', 'Reset meta information for brands list')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Catalog.Product.YandexMarketExpiry', 'Срок годности или службы'),
    (2,'Core.Catalog.Product.YandexMarketExpiry', 'Shelf life or service life')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Catalog.Product.YandexMarketWarrantyDays', 'Гарантийный срок'),
    (2,'Core.Catalog.Product.YandexMarketWarrantyDays', 'Warranty period')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Catalog.Product.YandexMarketCommentWarranty', 'Дополнительные условия гарантии'),
    (2,'Core.Catalog.Product.YandexMarketCommentWarranty', 'Additional warranty terms')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Catalog.Product.YandexMarketPeriodOfValidityDays', 'Срок годности'),
    (2,'Core.Catalog.Product.YandexMarketPeriodOfValidityDays', 'Shelf life')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Catalog.Product.YandexMarketServiceLifeDays', 'Срок службы'),
    (2,'Core.Catalog.Product.YandexMarketServiceLifeDays', 'Service life')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Catalog.Product.YandexMarketTnVedCode', 'Код ТН ВЭД'),
    (2,'Core.Catalog.Product.YandexMarketTnVedCode', 'TN VED code')

GO--

ALTER TABLE Catalog.ProductExportOptions ADD
	YandexMarketExpiry nvarchar(25) NULL,
	YandexMarketWarrantyDays nvarchar(25) NULL,
	YandexMarketCommentWarranty nvarchar(500) NULL,
	YandexMarketPeriodOfValidityDays nvarchar(25) NULL,
	YandexMarketServiceLifeDays nvarchar(25) NULL,
	YandexMarketTnVedCode nvarchar(10) NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.ExportFeed.SettingsYandex.ExportExpiry', 'Выгружать тег expiry'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportExpiry', 'Export expiry tag'),
	(1,'Admin.ExportFeed.SettingsYandex.ExportExpiryHint', 'Выгружать срок годности или службы в тег expiry'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportExpiryHint', 'Export shelf life or service life in expiry tag')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.ExportFeed.SettingsYandex.ExportWarrantyDays', 'Выгружать тег warranty-days'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportWarrantyDays', 'Export warranty-days tag'),
	(1,'Admin.ExportFeed.SettingsYandex.ExportWarrantyDaysHint', 'Выгружать гарантийный срок в тег warranty-days'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportWarrantyDaysHint', 'Export warranty period in warranty-days tag')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.ExportFeed.SettingsYandex.ExportCommentWarranty', 'Выгружать тег comment-warranty'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportCommentWarranty', 'Export comment-warranty tag'),
	(1,'Admin.ExportFeed.SettingsYandex.ExportCommentWarrantyHint', 'Выгружать дополнительные условия гарантии в тег comment-warranty'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportCommentWarrantyHint', 'Export additional warranty terms in comment-warranty tag')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Admin.ExportFeed.SettingsYandex.ExportPeriodOfValidityDays', 'Выгружать тег period-of-validity-days'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportPeriodOfValidityDays', 'Export period-of-validity-days tag'),
    (1,'Admin.ExportFeed.SettingsYandex.ExportPeriodOfValidityDaysHint', 'Выгружать срок годности в тег period-of-validity-days'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportPeriodOfValidityDaysHint', 'Export shelf life in period-of-validity-days tag')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Admin.ExportFeed.SettingsYandex.ExportServiceLifeDays', 'Выгружать тег service-life-days'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportServiceLifeDays', 'Export service-life-days tag'),
    (1,'Admin.ExportFeed.SettingsYandex.ExportServiceLifeDaysHint', 'Выгружать срок службы в тег service-life-days'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportServiceLifeDaysHint', 'Export service life in service-life-days tag')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Admin.ExportFeed.SettingsYandex.ExportMarketTnVedCode', 'Выгружать тег tn-ved-code'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportMarketTnVedCode', 'Export tn-ved-code tag'),
    (1,'Admin.ExportFeed.SettingsYandex.ExportMarketTnVedCodeHint', 'Выгружать код ТН ВЭД в тег tn-ved-code'),
    (2,'Admin.ExportFeed.SettingsYandex.ExportMarketTnVedCodeHint', 'Export TN VED code in tn-ved-code tag')

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
	,@sqlMode NVARCHAR(200) = 'GetProducts'
	,@exportAdult BIT
	,@dontExportProductsWithoutDimensionsAndWeight BIT = 0
	,@priceRuleId int = 0
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @lcategorytemp TABLE (CategoryId INT);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)


	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		if ((Select Opened from @l where CategoryId=@l1)=1)
		begin
			INSERT INTO @lcategorytemp
			SELECT @l1
		end
		else
		begin
	 		INSERT INTO @lcategorytemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @lcategory
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1;

	IF @sqlMode = 'GetCountOfProducts'
	BEGIN
		SELECT COUNT(Distinct offer.OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Catalog].[ProductExportOptions] exop ON exop.ProductId = p.ProductId 
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId	
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId													
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				exop.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
	END

	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
		SELECT Distinct tmp.CategoryId
		FROM @lcategorytemp AS tmp
		INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
		WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)
		
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,crossCategory.[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,p.[Name]
			,p.[UrlPath]
			,p.[Description]
			,p.[BriefDescription]			
			,p.ArtNo
			,[Offer].OfferId			
			,[Offer].Main
			,[Offer].ColorID			
			,[Offer].SizeID			
			,[Offer].Length
			,[Offer].Width
			,[Offer].Height
			,ColorName
			,SizeName
			,BrandName
			,country1.CountryName as BrandCountry
			,country2.CountryName as BrandCountryManufacture			
			,CurrencyValue
			,[Settings].PhotoToString(Offer.ColorID, p.ProductId) AS Photos
			,[Offer].[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,[Offer].BarCode			
			,p.MinAmount
			,p.Multiplicity
			,exop.Adult
			,exop.Gtin
			,exop.GoogleProductCategory
			,exop.YandexSalesNote
			,exop.YandexTypePrefix
			,exop.YandexModel
			,exop.YandexName
			,exop.YandexDeliveryDays
			,exop.YandexSizeUnit
			,exop.YandexProductDiscounted
			,exop.YandexProductDiscountCondition
			,exop.YandexProductDiscountReason
			,exop.YandexMarketCategory
			,exop.ManufacturerWarranty
			,exop.Bid
			,exop.YandexMarketExpiry
			,exop.YandexMarketWarrantyDays
			,exop.YandexMarketCommentWarranty
			,exop.YandexMarketPeriodOfValidityDays
			,exop.YandexMarketServiceLifeDays
			,exop.YandexMarketTnVedCode
			,opr.PriceByRule
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		LEFT JOIN [Catalog].[ProductExportOptions] exop ON exop.ProductId = p.ProductId 
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		cross apply(SELECT TOP (1) [ProductCategories].[CategoryId] from [Catalog].[ProductCategories]
					INNER JOIN  cte lc ON lc.CategoryId = productCategories.CategoryID
					where  [ProductCategories].[ProductID] = p.[ProductID]
					Order By [ProductCategories].Main DESC, [ProductCategories].[CategoryId] ) crossCategory	
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)		
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				exop.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
		Order By p.ProductId
	END

	IF @sqlMode = 'GetOfferIds'
	BEGIN
		SELECT Distinct offer.OfferId
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Catalog].[ProductExportOptions] exop ON exop.ProductId = p.ProductId 
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId	
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				exop.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
	END
END

GO--

DELETE FROM [Settings].[Error404] WHERE UrlReferer LIKE '%localhost%'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Core.ExportImport.ProductFields.YandexMarketExpiry', 'Яндекс.Маркет: Срок годности или службы (expiry)'),
	(2,'Core.ExportImport.ProductFields.YandexMarketExpiry', 'Yandex.Market: Shelf life or service life (expiry)'),
	(1,'Core.ExportImport.EProductField.YandexMarketExpiry', 'Яндекс.Маркет: Срок годности или службы (expiry)'),
	(2,'Core.ExportImport.EProductField.YandexMarketExpiry', 'Yandex.Market: Shelf life or service life (expiry)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Core.ExportImport.ProductFields.YandexMarketWarrantyDays', 'Яндекс.Маркет: Гарантийный срок (warranty-days)'),
	(2,'Core.ExportImport.ProductFields.YandexMarketWarrantyDays', 'Yandex.Market: Warranty period (warranty-days)'),
	(1,'Core.ExportImport.EProductField.YandexMarketWarrantyDays', 'Яндекс.Маркет: Гарантийный срок (warranty-days)'),
	(2,'Core.ExportImport.EProductField.YandexMarketWarrantyDays', 'Yandex.Market: Warranty period (warranty-days)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Core.ExportImport.ProductFields.YandexMarketCommentWarranty', 'Яндекс.Маркет: Дополнительные условия гарантии (comment-warranty)'),
	(2,'Core.ExportImport.ProductFields.YandexMarketCommentWarranty', 'Yandex.Market: Additional warranty conditions (comment-warranty)'),
	(1,'Core.ExportImport.EProductField.YandexMarketCommentWarranty', 'Яндекс.Маркет: Дополнительные условия гарантии (comment-warranty)'),
	(2,'Core.ExportImport.EProductField.YandexMarketCommentWarranty', 'Yandex.Market: Additional warranty conditions (comment-warranty)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Core.ExportImport.ProductFields.YandexMarketPeriodOfValidityDays', 'Яндекс.Маркет: Срок годности (period-of-validity-days)'),
	(2,'Core.ExportImport.ProductFields.YandexMarketPeriodOfValidityDays', 'Yandex.Market: Shelf life (period-of-validity-days)'),
	(1,'Core.ExportImport.EProductField.YandexMarketPeriodOfValidityDays', 'Яндекс.Маркет: Срок годности (period-of-validity-days)'),
	(2,'Core.ExportImport.EProductField.YandexMarketPeriodOfValidityDays', 'Yandex.Market: Shelf life (period-of-validity-days)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Core.ExportImport.ProductFields.YandexMarketServiceLifeDays', 'Яндекс.Маркет: Срок службы (service-life-days)'),
	(2,'Core.ExportImport.ProductFields.YandexMarketServiceLifeDays', 'Yandex.Market: Service life (service-life-days)'),
	(1,'Core.ExportImport.EProductField.YandexMarketServiceLifeDays', 'Яндекс.Маркет: Срок службы (service-life-days)'),
	(2,'Core.ExportImport.EProductField.YandexMarketServiceLifeDays', 'Yandex.Market: Service life (service-life-days)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Core.ExportImport.ProductFields.YandexMarketTnVedCode', 'Яндекс.Маркет: Код ТН ВЭД (tn-ved-code)'),
	(2,'Core.ExportImport.ProductFields.YandexMarketTnVedCode', 'Yandex.Market: Tn ved code (tn-ved-code)'),
	(1,'Core.ExportImport.EProductField.YandexMarketTnVedCode', 'Яндекс.Маркет: Код ТН ВЭД (tn-ved-code)'),
	(2,'Core.ExportImport.EProductField.YandexMarketTnVedCode', 'Yandex.Market: Tn ved code (tn-ved-code)')

GO--

Update [Settings].[Localization]
Set [ResourceValue] = 'Яндекс.Маркет: Модель'
Where [ResourceKey] = 'Core.ExportImport.ProductFields.YandexModel' and [LanguageId] = 1


Update [Settings].[Localization]
Set [ResourceValue] = 'Яндекс.Маркет: Название товара'
Where [ResourceKey] = 'Core.ExportImport.ProductFields.YandexName' and [LanguageId] = 1


Update [Settings].[Localization]
Set [ResourceValue] = 'Яндекс.Маркет: Тип'
Where [ResourceKey] = 'Core.ExportImport.ProductFields.YandexTypePrefix' and [LanguageId] = 1

Update [Settings].[Localization]
Set [ResourceValue] = 'Яндекс.Маркет: Ставка для карточки модели'
Where [ResourceKey] = 'Core.ExportImport.ProductFields.Bid' and [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.EmailNotifications', 'Email для уведомлений от партнеров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Partners.EmailNotifications', 'Email for notifications from partners')

GO--

UPDATE [Settings].[ExportFeedSettings]
SET [AdvancedSettings] = '{"CsvEnconing":"UTF-8","CsvSeparator":"SemicolonSeparated","CsvSeparatorCustom":null,"CsvColumSeparator":";","CsvPropertySeparator":":","CsvExportNoInCategory":false,"CsvCategorySort":false,"FieldMapping":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,51,52,53,54,56,57,58,59,60,61,62,63,64,65,66,67],"ModuleFieldMapping":[],"FileName":null,"FileExtention":null,"PriceMargin":0.0,"AdditionalUrlTags":null,"Active":false,"IntervalType":0,"Interval":0,"JobStartTime":"0001-01-01T00:00:00","AdvancedSettings":null,"ExportAllProducts":true}'
WHERE ExportFeedId = '2' AND AdvancedSettings = '{"CsvEnconing":"UTF-8","CsvSeparator":"SemicolonSeparated","CsvSeparatorCustom":null,"CsvColumSeparator":";","CsvPropertySeparator":":","CsvExportNoInCategory":false,"CsvCategorySort":false,"FieldMapping":[1,2,3,4,5,6,7,8,9,10,11,12,13,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65],"ModuleFieldMapping":[],"FileName":null,"FileExtention":null,"PriceMargin":0.0,"AdditionalUrlTags":null,"Active":false,"IntervalType":0,"Interval":0,"JobStartTime":"0001-01-01T00:00:00","AdvancedSettings":null,"ExportAllProducts":true}'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Admin.ExportFeeed.SettingsYandex.ExportOnlyUseInDetailsProperties', 'Выгружать только свойства, у которых стоит "Использовать в карточке товара"'),
	(2,'Admin.ExportFeeed.SettingsYandex.ExportOnlyUseInDetailsProperties', 'Export only properties that should have "Use in product"'),
	(1,'Admin.ExportFeeed.SettingsYandex.ExportOnlyUseInDetailsPropertiesHint', 'Выгружать только свойства, у которых стоит "Использовать в карточке товара". Если настройка активна, то среди остальных свойств можно дополнительно выбрать какие будут выгружаться.'),
	(2,'Admin.ExportFeeed.SettingsYandex.ExportOnlyUseInDetailsPropertiesHint', 'Export only properties that should have "Use in product". If the setting is active, then among the other properties, you can additionally choose which ones will be exported.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Admin.Js.ModalAddEditAdditionalProperties.Title', 'Дополнительные свойства'),
	(2,'Admin.Js.ModalAddEditAdditionalProperties.Title', 'Additional properties')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Mails.MailFormat.DeliveryDate', 'Дата доставки: '),
    (2,'Core.Mails.MailFormat.DeliveryDate', 'Delivery date: ')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.Crm.EOrderFieldType.DeliveryDate', 'Есть дата и время доставки'),
    (2,'Core.Crm.EOrderFieldType.DeliveryDate', 'Has delivery date and time')
GO--

Update [Settings].[MailFormatType]
Set Comment = 'Письмо при оформнении нового заказа (#ORDER_ID#, #NUMBER#, #BILLING_LINK#, #EMAIL#, #CUSTOMERCONTACTS#, #SHIPPINGMETHOD#, #PAYMENTTYPE#, #ORDERTABLE#, #CURRENTCURRENCYCODE#, #TOTALPRICE#, #COMMENTS#, #MANAGER_NAME#,#ADDITIONALCUSTOMERFIELDS#, #INN#, #COMPANYNAME#, "#FIRSTNAME#", "#LASTNAME#", "#CITY#", "#ADDRESS#", "#DELIVERYDATE#", "#DELIVERYDATE_DATE#")'
Where [MailType] = 'OnNewOrder' and Comment like 'Письмо при оформнении нового заказа%' and Comment not like '%DELIVERYDATE%'

GO--

Update [Settings].[MailFormatType]
Set Comment = 'При смене статуса заказа (#ORDERID#, #NUMBER#, #ORDERSTATUS#, #STATUSCOMMENT#,  #ORDERTABLE#, #TRACKNUMBER#, #MANAGER_NAME#, #BILLING_LINK#, #BILLING_SHORTLINK#, #SHIPPINGMETHOD#, #PAYMENTTYPE#, #TOTALPRICE#, #FIRSTNAME#, #LASTNAME#, #CITY#, #ADDRESS#, #DELIVERYDATE#, #DELIVERYDATE_DATE#)'
Where [MailType] = 'OnChangeOrderStatus' and Comment like 'При смене статуса заказа%' and Comment not like '%DELIVERYDATE%'

GO--

Update [Settings].[MailFormatType]
Set Comment = 'Письмо при проведении/отмене оплаты (#ORDER_ID#, #NUMBER#, #PAY_STATUS#, #STORE_NAME#, #SUM#; #MANAGER_NAME#, #SHIPPINGMETHOD#, #PAYMENTTYPE#, #FIRSTNAME#, #LASTNAME#, #CITY#, #ADDRESS#, #DELIVERYDATE#, #DELIVERYDATE_DATE#)'
Where [MailType] = 'OnPayOrder' and Comment like 'Письмо при проведении/отмене оплаты%' and Comment not like '%DELIVERYDATE%'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Cards.CustomerGroup', 'Группа пользователей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Cards.CustomerGroup', 'Customer group')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.PaymentMethods.IntellectMoneyMainProtocol.ReceiptDataInn', 'Ваш номер ИНН'),
    (2,'Admin.PaymentMethods.IntellectMoneyMainProtocol.ReceiptDataInn', 'INN')
    
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Bonuses.ERule.PostingReview', 'Начисление баллов при размещении отзыва')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Bonuses.ERule.PostingReview', 'Points for posting a review')

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ObjectType' OR name = N'ObjectId') AND object_id = OBJECT_ID(N'[Bonus].[RuleLog]'))
    BEGIN
        ALTER TABLE Bonus.RuleLog 
			ADD ObjectType int NOT NULL DEFAULT 0,
				ObjectId NVARCHAR(50) NOT NULL DEFAULT ''

		ALTER TABLE Bonus.RuleLog
			DROP CONSTRAINT PK_RuleLog
		
		ALTER TABLE Bonus.RuleLog ADD CONSTRAINT
			PK_RuleLog PRIMARY KEY CLUSTERED 
			(
			CardId ASC,
			RuleType ASC,
			Created ASC,
			ObjectType ASC,
			ObjectId ASC
			) WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    END
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Rules.PostingReview.AccrueAdditionalBonuses', 'Начислить доп. бонусы на карту')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Rules.PostingReview.AccrueAdditionalBonuses', 'Accrue additional card bonuses')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsBonus.Index.NotificationMethod', 'Способ уведомления')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsBonus.Index.NotificationMethod', 'Notification method')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsBonus.Index.Notifications', 'Уведомления')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsBonus.Index.Notifications', 'Notifications')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Шаблоны уведомлений' WHERE [ResourceKey] = 'Admin.SmsTemplates.Index.Title' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Notification templates' WHERE [ResourceKey] = 'Admin.SmsTemplates.Index.Title' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'История уведомлений' WHERE [ResourceKey] = 'Admin.SmsTemplates.SmsLog.Title' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Notification log' WHERE [ResourceKey] = 'Admin.SmsTemplates.SmsLog.Title' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.RussianPostContract','Выберите договор Почты России')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Grastin.RussianPostContract','Select a contract Russian Post')

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'Contact') AND object_id = OBJECT_ID(N'Bonus.NotificationLog'))
    BEGIN
        ALTER TABLE Bonus.SmsLog ADD
			ContactType NVARCHAR(15) NULL
		EXEC sp_RENAME 'Bonus.SmsLog.Phone' , 'Contact', 'COLUMN'

		ALTER TABLE Bonus.SmsLog ALTER COLUMN Contact NVARCHAR(MAX) NOT NULL;
		EXEC sp_RENAME 'Bonus.SmsLog', 'NotificationLog';

    END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE NAME='NotificationTemplate')	
	BEGIN
		EXEC sp_RENAME 'Bonus.SmsTemplate.SmsTypeId' , 'NotificationTypeId', 'COLUMN'
		EXEC sp_RENAME 'Bonus.SmsTemplate.SmsBody' , 'NotificationBody', 'COLUMN'
		EXEC sp_RENAME 'Bonus.SmsTemplate', 'NotificationTemplate';

    END

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Контакт' WHERE [ResourceKey] = 'Admin.Js.Smstemplates.Number' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Contact' WHERE [ResourceKey] = 'Admin.Js.Smstemplates.Number' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отправить уведомление' WHERE [ResourceKey] = 'Admin.Js.Bonus.SendSms' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Send notification' WHERE [ResourceKey] = 'Admin.Js.Bonus.SendSms' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.CdekContract','Выберите договор СДЭК')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Grastin.CdekContract','Select a contract CDEK')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkYandexZenActive','Яндекс.Дзен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkYandexZenActive','Yandex.Zen')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkRutubeActive','Rutube')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkRutubeActive','Rutebe')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.YandexZen','Яндекс.Дзен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.YandexZen','Yandex.Zen')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Rutube','Rutube')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Rutube','Rutebe')
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Бонусная карта' 
WHERE [ResourceKey] = 'Admin.Orders.ClientBonuseCard.Bonuses' AND [LanguageId] = 1

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка актуальна для шаблона Modern и других шаблонов, для которых в трансформере дизайна есть настройка главной страницы "Отображать категории на главной".<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank" >Инструкция. Категории на главной.</a>' WHERE [ResourceKey] = 'Admin.Category.RightPanel.ShowOnMainPageNoteText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting is relevant for the Modern template and other templates for which the design transformer has the "Display categories on homepage" homepage setting.<br/><br/>Read more: <br/> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank" >Instructions. Categories on the home page.</a>' WHERE [ResourceKey] = 'Admin.Category.RightPanel.ShowOnMainPageNoteText' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за вывод категорий на главной странице. Отобразятся только те категории, для которых включена настройка "Выводить на главной".  <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Js.Builder.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for displaying categories on the main page. Only those categories for which the "Show on home" setting is enabled will be displayed. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </a>' WHERE [ResourceKey] = 'Js.Builder.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за количество категорий, которое будет отображаться на главной странице в каждом блоке. <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Js.Builder.CountMainPageCategoriesInSectionTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for the number of categories that will be displayed on the main page in each block. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </a>' WHERE [ResourceKey] = 'Js.Builder.CountMainPageCategoriesInSectionTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за количество категорий, которое будет отображаться в одной строке каждого блока на главной странице.<br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Js.Builder.CountMainPageCategoriesInLineTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for the number of categories that will be displayed in one line of each block on the main page. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Js.Builder.CountMainPageCategoriesInLineTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за вывод категорий на главной странице. Отобразятся только те категории, для которых включена настройка "Выводить на главной".  <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for displaying categories on the main page. Only those categories for which the "Show on home" setting is enabled will be displayed. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за количество категорий, которое будет отображаться на главной странице в каждом блоке. <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for the number of categories that will be displayed on the main page in each block. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за количество категорий, которое будет отображаться на главной странице в каждом блоке. <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for the number of categories that will be displayed on the main page in each block. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.DeleteFile','Удалить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.DeleteFile','Delete')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.NotExistFile','Пока нет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.NotExistFile','Not yet')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.File','Файл')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.File','File')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ShowNotAvailableLableInProduct','Отображать маркер "Нет в наличии"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ShowNotAvailableLableInProduct','Show "Not available" label')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.Product.OptionShowNotAvailableLable','Опция определяет, отображать или нет маркер "Нет в наличии" в карточке товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.Product.OptionShowNotAvailableLable','The option determines whether or not to display the marker "Not available" in the product card')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.BelowAdditionalDescription','Добавить дополнительное описание в конец')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.BelowAdditionalDescription','Add additional description to the end')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.AboveAdditionalDescription','Добавить дополнительное описание в начало')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.AboveAdditionalDescription','Add additional description to the beginning')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.TextAboveAdditionalDescription','Текст дополнительного описания в начало')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.TextAboveAdditionalDescription','Additional description text to the beginning')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.TextBelowAdditionalDescription','Текст дополнительного описания в конец')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.TextBelowAdditionalDescription','Additional description text to the end')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ShowAvailableLableInProduct','Отображать маркер "Есть в наличии"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ShowAvailableLableInProduct','Show "Available" label')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.Product.OptionShowAvailableLable','Опция определяет, отображать или нет маркер "Есть в наличии" в карточке товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.Product.OptionShowAvailableLable','The option determines whether or not to display the marker "Available" in the product card')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
    VALUES
        (1,'Admin.Catalog.ErrorUploadImageFormat', 'Ошибка при загрузке изображения. Возможно, изображение имеет неподдерживаемый формат внутри.'),
        (2,'Admin.Catalog.ErrorUploadImageFormat', 'Error uploading the image. Perhaps the image has an unsupported format inside.')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Комментарий (для внутреннего использования)' WHERE [ResourceKey] = 'Admin.ExportFeeed.Settings.Description' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Comment (for internal use)' WHERE [ResourceKey] = 'Admin.ExportFeeed.Settings.Description' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.Settings.ExportParameters','Параметры выгрузки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.Settings.ExportParameters','Export parameters')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.FormatSettings','Настройки формата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.FormatSettings','Format settings')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.NameHelp','Название выгрузки в списке всех выгрузок.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.NameHelp','The name of the download in the list of all downloads.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.DescriptionHelp','Поле для указания дополнительных сведений о выгрузке для внутреннего использования.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.DescriptionHelp','A field for specifying additional information about the upload for internal use.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.FileNameHelp','Имя выгружаемого файла и его формат (CSV или TXT).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.FileNameHelp','The name of the uploaded file and its format (CSV or TXT).')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.ActiveHelp','Возможность автоматической выгрузки каталога в формате CSV с заданной периодичностью.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.ActiveHelp','Ability to automatically upload the catalog in CSV format at a specified frequency.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.DoNotExportAdultHelp','Настройка дает возможность исключить из выгрузки товары, которые отмечены как товары для взрослых (adult).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.DoNotExportAdultHelp','The setting allows you to exclude from unloading products that are marked as products for adults (adult).')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvExportNoInCategoryHelp','Данная опция позволяет включить в выгрузку каталога в формате CSV товары, которые загружены в магазин, но пока не размещены в категориях.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvExportNoInCategoryHelp','This option allows you to include in the catalog upload in CSV format products that are uploaded to the store, but not yet placed in categories.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvCategorySortHelp','Активация данной опции позволит в файле CSV выгрузить сортировку товара в той категории, в которой он находится.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvCategorySortHelp','Activating this option will allow you to download the sorting of the product in the category in which it is located in the CSV file.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvEnconingHelp','Это кодировка, в которой выгружается каталог. Microsoft Excel воспринимает кодировки UTF-8 и Windows-1251.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvEnconingHelp','This is the encoding in which the directory is uploaded. Microsoft Excel accepts UTF-8 and Windows-1251 encodings.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvSeparatorHelp','Разделитель, который будет указан между столбцами или колонками в файле CSV.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvSeparatorHelp','The separator to be specified between columns or columns in the CSV file.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvColumSeparatorHelp','Символ, который будет указываться в файле CSV при перечислении свойств.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvColumSeparatorHelp','The character to be specified in the CSV file when listing properties.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvPropertySeparatorHelp','Символ, который будет указываться в файле CSV при разделении свойства и его значения.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvPropertySeparatorHelp','The character that will be specified in the CSV file when separating the property and its value.')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Имя выгрузки' WHERE [ResourceKey] = 'Admin.ExportFeeed.Settings.Name' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Export name' WHERE [ResourceKey] = 'Admin.ExportFeeed.Settings.Name' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Будет произведена выгрузка всех товаров каталога' WHERE [ResourceKey] = 'Admin.ExportFeeds.ChoiceOfProducts.ExportAllGoods' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'All items in the catalog will be downloaded' WHERE [ResourceKey] = 'Admin.ExportFeeds.ChoiceOfProducts.ExportAllGoods' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.AllOffersToMultiOfferColumnHelp','Активация данной опции позволит в файле CSV выгрузить все цены в колонку модификаций.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.AllOffersToMultiOfferColumnHelp','Activating this option will allow uploading all prices in the modifications column in the CSV file.')

UPDATE [Settings].[ExportFeed] SET [LastExport] = null WHERE [LastExport] = '2001-01-01 00:00:00.000' and [Id] = 2

GO--


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[PriceRule]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[PriceRule](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](max) NOT NULL,
		[SortOrder] [int] NOT NULL,
		[Amount] [float] NOT NULL,
		[CustomerGroupId] int NOT NULL,
		[PaymentMethodId] int NULL,
		[ApplyDiscounts] bit NOT NULL
	CONSTRAINT [PK_PriceRule] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[OfferPriceRule]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[OfferPriceRule](
		[OfferId] [int] NOT NULL,
		[PriceRuleId] [int] NOT NULL,
		[PriceByRule] [float] NULL
	) ON [PRIMARY]
END

GO--

If not Exists (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_OfferPriceRule_Offer')
Begin
	ALTER TABLE [Catalog].[OfferPriceRule]  WITH CHECK ADD  CONSTRAINT [FK_OfferPriceRule_Offer] FOREIGN KEY([OfferId])
	REFERENCES [Catalog].[Offer] ([OfferID])
	ON DELETE CASCADE
End

GO--

If not Exists (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_OfferPriceRule_PriceRule')
Begin
	ALTER TABLE [Catalog].[OfferPriceRule]  WITH CHECK ADD  CONSTRAINT [FK_OfferPriceRule_PriceRule] FOREIGN KEY([PriceRuleId])
	REFERENCES [Catalog].[PriceRule] ([Id])
	ON DELETE CASCADE
End

GO--

IF NOT EXISTS(SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Settings.Catalog.ShowAmountsTableInProduct')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
		VALUES
			(1,'Admin.Settings.Catalog.ShowAmountsTableInProduct', 'Выводить таблицу с кол-вом и ценой в продукте'),
			(2,'Admin.Settings.Catalog.ShowAmountsTableInProduct', 'Display table with amounts and prices in product page'),
			(1,'Admin.Settings.Catalog.ShowAmountsTableInProductHint', 'Если опция активна на странице товара будет выводиться таблица с кол-вом и ценой'),
			(2,'Admin.Settings.Catalog.ShowAmountsTableInProductHint', 'If the option is active, a table with the quantity and price will be displayed on the product page'),
			(1,'Admin.Settings.Catalog.ShowAmountsTableInCatalog', 'Выводить таблицу с кол-вом и ценой в каталоге'),
			(2,'Admin.Settings.Catalog.ShowAmountsTableInCatalog', 'Display table with amounts and prices in category page'),
			(1,'Admin.Settings.Catalog.ShowAmountsTableInCatalogHint', 'Если опция активна на странице товара будет выводиться таблица с кол-вом и ценой'),
			(2,'Admin.Settings.Catalog.ShowAmountsTableInCatalogHint', 'If the option is active, a table with the quantity and price will be displayed on the product page')
END

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'IsNotBeCompleted') AND object_id = OBJECT_ID(N'[Customers].[TaskGroup]'))
    BEGIN
        ALTER TABLE [Customers].[TaskGroup]
			ADD IsNotBeCompleted bit NOT NULL DEFAULT 0
    END

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.NotSelected','Не выбран')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.NotSelected','NotSelected')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Tasks.CompleteTask.CannotInSelectedProject','Нельзя завершить задачу в выбранный проект.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Tasks.CompleteTask.CannotInSelectedProject','You cannot complete a task in the selected project.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Market.Category','Категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Market.Category','Category')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.DisabledModules','Деактивированные модули')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.DisabledModules','Disabled modules')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.EnabledModules','Активные модули')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.EnabledModules','Enabled modules')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.DisabledModulesHelp','Обратите внимание, что списания за деактивированные модули продолжаются в полном объеме. <br>Чтобы полностью отключить модуль, его необходимо удалить.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.DisabledModulesHelp','Please note that charges for deactivated modules continue in full. <br>To completely disable a module, it must be removed.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvSeparatorCustomHelp','Свой разделитель, который будет указан между столбцами или колонками в файле CSV.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvSeparatorCustomHelp','Custom separator to be specified between columns or columns in the CSV file.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.Marker.CustomModule','Доработанный')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.Marker.CustomModule','Custom')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.Marker.LocalModule','Не из галереи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.Marker.LocalModule','Not from gallery')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.Marker.PersonalModule','Персональный')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.Marker.PersonalModule','Personal')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsReport.DetailByDays','Детализация: по дням')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsReport.DetailByDays','Detail by day')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsReport.DetailByWeeks','Детализация: по неделям')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsReport.DetailByWeeks','Detail by week')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsReport.DetailByMonths','Детализация: по месяцам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsReport.DetailByMonths','Detail by month')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Неделя' WHERE [ResourceKey] = 'Admin.Js.AnalyticsReport.ByWeeks' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Week' WHERE [ResourceKey] = 'Admin.Js.AnalyticsReport.ByWeeks' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Месяц' WHERE [ResourceKey] = 'Admin.Js.AnalyticsReport.ByMonths' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Month' WHERE [ResourceKey] = 'Admin.Js.AnalyticsReport.ByMonths' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsReport.ByYear','Год')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsReport.ByYear','Year')

GO--

ALTER TABLE Customers.Subscription
	DROP COLUMN UnsubscribeReason
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Найдено подписчиков: {0}' WHERE [ResourceKey] = 'Admin.Subscribe.Grid.FildTotal' AND [LanguageId] = 1

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Subscription.Index.Instruction', N'Инструкция. Подписчики на новости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Subscription.Index.Instruction','Instruction. News subscribers')
    
GO--


Update [Settings].[Localization] Set [ResourceValue] = 'Файл (.zip, .rar, .pdf, .txt, .doc, .docx, .xls, .xlsx, .rtf)' Where [LanguageId] = 1 and [ResourceKey] = 'App.Landing.Domain.Forms.ELpFormFieldType.FileArchive'
Update [Settings].[Localization] Set [ResourceValue] = 'File (.zip, .rar, .pdf, .txt, .doc, .docx, .xls, .xlsx, .rtf)' Where [LanguageId] = 2 and [ResourceKey] = 'App.Landing.Domain.Forms.ELpFormFieldType.FileArchive'

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.City','Город')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.City','City')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Region','Регион')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Region','Region')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Country','Страна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Country','Country')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Weight','Вес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Weight','Weight')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Dimensions','Габариты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Dimensions','Dimensions')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Result','Результат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Result','Result')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate','Тестовый расчет доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate','Test calculation of delivery')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductSelection.Calculate','Рассчитать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductSelection.Calculate','Calculate')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.TestCalculation','Пробный расчет доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.TestCalculation','Trial shipping calculation')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Amount','Количество товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Amount','Product amount')

GO--

ALTER TABLE Catalog.ProductExportOptions ADD
	YandexMarketStepQuantity int NULL,
	YandexMarketMinQuantity int NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
VALUES
		(1,'Core.ExportImport.EProductField.YandexMarketStepQuantity', 'Яндекс.Маркет: Квант продажи (step-quantity)'),
		(2,'Core.ExportImport.EProductField.YandexMarketStepQuantity', 'Yandex.Market: Step quantity (step-quantity)'),
		(1,'Core.ExportImport.EProductField.YandexMarketMinQuantity', 'Яндекс.Маркет: Минимальное количество товара (min-quantity)'),
		(2,'Core.ExportImport.EProductField.YandexMarketMinQuantity', 'Yandex.Market: Minimum quantity (min-quantity)'),
		(1,'Core.ExportImport.ProductFields.YandexMarketStepQuantity', 'Яндекс.Маркет: Квант продажи (step-quantity)'),
		(2,'Core.ExportImport.ProductFields.YandexMarketStepQuantity', 'Yandex.Market: Step quantity (step-quantity)'),
		(1,'Core.ExportImport.ProductFields.YandexMarketMinQuantity', 'Яндекс.Маркет: Минимальное количество товара (min-quantity)'),
		(2,'Core.ExportImport.ProductFields.YandexMarketMinQuantity', 'Yandex.Market: Minimum quantity (min-quantity)'),
		(1,'Core.Catalog.Product.YandexMarketStepQuantity', 'Квант продажи (step-quantity)'),
		(2,'Core.Catalog.Product.YandexMarketStepQuantity', 'Step quantity (step-quantity)'),
		(1,'Core.Catalog.Product.YandexMarketMinQuantity', 'Минимальное количество товара (min-quantity)'),
		(2,'Core.Catalog.Product.YandexMarketMinQuantity', 'Minimum quantity (min-quantity)'),
		(1,'Admin.ExportFeed.SettingsYandex.ExportMarketStepQuantity', 'Выгружать тег step-quantity'),
		(2,'Admin.ExportFeed.SettingsYandex.ExportMarketStepQuantity', 'Export step-quantity tag'),
		(1,'Admin.ExportFeed.SettingsYandex.ExportMarketStepQuantityHint', 'Выгружать квант продажи в тег step-quantity'),
		(2,'Admin.ExportFeed.SettingsYandex.ExportMarketStepQuantityHint', 'Export step quantity in step-quantity tag'),
		(1,'Admin.ExportFeed.SettingsYandex.ExportMarketMinQuantity', 'Выгружать тег min-quantity'),
		(2,'Admin.ExportFeed.SettingsYandex.ExportMarketMinQuantity', 'Export min-quantity tag'),
		(1,'Admin.ExportFeed.SettingsYandex.ExportMarketMinQuantityHint', 'Выгружать квант продажи в тег min-quantity'),
		(2,'Admin.ExportFeed.SettingsYandex.ExportMarketMinQuantityHint', 'Export step quantity in min-quantity tag')
           
GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
	,@sqlMode NVARCHAR(200) = 'GetProducts'
	,@exportAdult BIT
	,@dontExportProductsWithoutDimensionsAndWeight BIT = 0
	,@priceRuleId int = 0
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @lcategorytemp TABLE (CategoryId INT);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)


	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		if ((Select Opened from @l where CategoryId=@l1)=1)
		begin
			INSERT INTO @lcategorytemp
			SELECT @l1
		end
		else
		begin
	 		INSERT INTO @lcategorytemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @lcategory
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1;

	IF @sqlMode = 'GetCountOfProducts'
	BEGIN
		SELECT COUNT(Distinct offer.OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Catalog].[ProductExportOptions] exop ON exop.ProductId = p.ProductId 
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId	
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId													
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				exop.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
	END

	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
		SELECT Distinct tmp.CategoryId
		FROM @lcategorytemp AS tmp
		INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
		WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)
		
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,crossCategory.[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,p.[Name]
			,p.[UrlPath]
			,p.[Description]
			,p.[BriefDescription]			
			,p.ArtNo
			,[Offer].OfferId			
			,[Offer].Main
			,[Offer].ColorID			
			,[Offer].SizeID			
			,[Offer].Length
			,[Offer].Width
			,[Offer].Height
			,ColorName
			,SizeName
			,BrandName
			,country1.CountryName as BrandCountry
			,country2.CountryName as BrandCountryManufacture			
			,CurrencyValue
			,[Settings].PhotoToString(Offer.ColorID, p.ProductId) AS Photos
			,[Offer].[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,[Offer].BarCode			
			,p.MinAmount
			,p.Multiplicity
			,exop.Adult
			,exop.Gtin
			,exop.GoogleProductCategory
			,exop.YandexSalesNote
			,exop.YandexTypePrefix
			,exop.YandexModel
			,exop.YandexName
			,exop.YandexDeliveryDays
			,exop.YandexSizeUnit
			,exop.YandexProductDiscounted
			,exop.YandexProductDiscountCondition
			,exop.YandexProductDiscountReason
			,exop.YandexMarketCategory
			,exop.ManufacturerWarranty
			,exop.Bid
			,exop.YandexMarketExpiry
			,exop.YandexMarketWarrantyDays
			,exop.YandexMarketCommentWarranty
			,exop.YandexMarketPeriodOfValidityDays
			,exop.YandexMarketServiceLifeDays
			,exop.YandexMarketTnVedCode
			,exop.YandexMarketStepQuantity
			,exop.YandexMarketMinQuantity
			,opr.PriceByRule
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		LEFT JOIN [Catalog].[ProductExportOptions] exop ON exop.ProductId = p.ProductId 
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		cross apply(SELECT TOP (1) [ProductCategories].[CategoryId] from [Catalog].[ProductCategories]
					INNER JOIN  cte lc ON lc.CategoryId = productCategories.CategoryID
					where  [ProductCategories].[ProductID] = p.[ProductID]
					Order By [ProductCategories].Main DESC, [ProductCategories].[CategoryId] ) crossCategory	
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)		
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				exop.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
		Order By p.ProductId
	END

	IF @sqlMode = 'GetOfferIds'
	BEGIN
		SELECT Distinct offer.OfferId
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Catalog].[ProductExportOptions] exop ON exop.ProductId = p.ProductId 
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId	
		LEFT JOIN [Catalog].[OfferPriceRule] opr On offer.OfferID = opr.OfferId and opr.PriceRuleId = @priceRuleId
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		AND (
			@exportAdult = 1
			OR (
				exop.Adult = 0
			)
		)
		AND (@dontExportProductsWithoutDimensionsAndWeight = 0 
			OR (
				[Offer].[Width] IS NOT NULL AND [Offer].[Width] != 0
				AND [Offer].[Height] IS NOT NULL AND [Offer].[Height] != 0
				AND [Offer].[Length] IS NOT NULL AND [Offer].[Length] != 0
				AND [Offer].[Weight] IS NOT NULL AND [Offer].[Weight] != 0
			))
		AND (@priceRuleId = 0 OR PriceByRule is not null)
	END
END

GO--

IF NOT EXISTS (SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'MyAccount.SaveUserInfo.ErrorRequiredFields')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
		Values
			(1,'MyAccount.SaveUserInfo.ErrorRequiredFields', 'Заполните обязательные поля'),
			(2,'MyAccount.SaveUserInfo.ErrorRequiredFields', 'Fill in all required fields')
ELSE
BEGIN
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'Заполните обязательные поля'
	WHERE [ResourceKey] = 'MyAccount.SaveUserInfo.ErrorRequiredFields' AND [LanguageId] = 1
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'Fill in all required fields'
	WHERE [ResourceKey] = 'MyAccount.SaveUserInfo.ErrorRequiredFields' AND [LanguageId] = 2
END

IF NOT EXISTS (SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'MyAccount.SaveUserInfo.Error')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
		Values
			(1,'MyAccount.SaveUserInfo.Error', 'Ошибка при сохранении'),
			(2,'MyAccount.SaveUserInfo.Error', 'Error on saving')
ELSE
BEGIN
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'Ошибка при сохранении'
	WHERE [ResourceKey] = 'MyAccount.SaveUserInfo.Error' AND [LanguageId] = 1
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'Error on saving'
	WHERE [ResourceKey] = 'MyAccount.SaveUserInfo.Error' AND [LanguageId] = 2
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Price','Цена товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Price','Product price')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Rule.Geo','Применять правила геолокации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Rule.Geo','Apply geolocation rules')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Rule.Margin','Применять правила наценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Rule.Margin','Apply markup rules')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.Rule.ExtrachargeCargo','Применять правила коррекции размеров посылки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.Rule.ExtrachargeCargo','Apply correction rules parcel size')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TestShippingCalculate.ProductSettings','Настройки товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TestShippingCalculate.ProductSettings','Product settings')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Артикул' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.Sku' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'SKU' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.Sku' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Артикул модификации:Размер:Цвет:Цена:ЗакупочнаяЦена:Наличие' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.MultiOffer' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'OfferSKU:Size:Color:Price:PurchasePrice:Amount' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.MultiOffer' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Modules.ModuleUpdating.ErrorTitle','Модуль установлен не из магазина модулей.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Modules.ModuleUpdating.ErrorTitle','The module is not installed from the module store.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Modules.ModuleUpdating.ErrorBody','Для получения обновлений переустановите его')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Modules.ModuleUpdating.ErrorBody','To get updates, reinstall it')

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[sp_RemoveOffer]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [Catalog].[sp_RemoveOffer] AS' 
END

GO--

-- =============================================
-- Author:		<RuslanZV>
-- Create date: <17.05.2022>
-- Description:	<Удаляет оффер товара>
-- =============================================
ALTER PROCEDURE [Catalog].[sp_RemoveOffer] 
	-- Add the parameters for the stored procedure here
	@OfferID as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ProductId as int;

	SELECT TOP (1) @ProductId = [ProductID] FROM [Catalog].[Offer] WHERE [OfferID] = @OfferID

	IF @ProductId IS NOT NULL
	BEGIN
		-- offer exists
		DELETE FROM [Catalog].[Offer] WHERE [OfferID] = @OfferID;
		declare @temp int;
		set @temp=(CASE WHEN EXISTS(select * from [Catalog].[Offer] where [ProductID]=@ProductId and [Main]=1) THEN 1 ELSE 0 END);
		if @temp=0
		BEGIN
			UPDATE TOP (1) [Catalog].[Offer] 
			SET [Main]=1 
			WHERE [ProductID]=@ProductId
		END 
	END
END

GO--

CREATE TABLE [Customers].[SmsCodeConfirmation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Phone] [bigint] NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
 CONSTRAINT [PK_SmsCodeConfirmation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Builder.MainPageProductsVisibilityHelp', 'Настройка позволяет отображать на главной странице 3 категории товаров: Хит продаж, Новинки, Скидки, а также дополнительные списки товаров, добавленные администратором.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank" >Товары на главной</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Builder.MainPageProductsVisibilityHelp', 'This setting allows you to display 3 categories of products on the homepage: Hot sales, New products, Discounts, as well as additional product lists added by the administrator.<br/><br/>Read more: <br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank" >Homepage Products</a>')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться на главной странице в каждом блоке.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Товары на главной</a>' WHERE [ResourceKey] = 'Js.Builder.CountMainPageProductInSectionHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed on the main page in each block.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Homepage Products</a>' WHERE [ResourceKey] = 'Js.Builder.CountMainPageProductInSectionHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться в одной строке каждого блока на главной странице.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Товары на главной</a>' WHERE [ResourceKey] = 'Js.Builder.CountMainPageProductInLineHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed in one line of each block on the main page.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Homepage Products</a>' WHERE [ResourceKey] = 'Js.Builder.CountMainPageProductInLineHelp' AND [LanguageId] = 2

GO--

    DELETE FROM Settings.Localization WHERE ResourceKey IN (
        'Admin.PaymentMethods.PayOnline.CurrencyRateRelation',
        'Admin.Js.ProductProperties.AddValue',
        'Js.Bonus.HaveBonusCard',
        'Admin.ShippingMethods.YandexDelivery.LengthHeightWidth',
        'Admin.Settings.Commonpage.News',
        'Admin.Settings.Catalog.Common',
        'Admin.Home.Menu.SiteMapGenerate',
        'Admin.Js.Design.ErrorSavingTemplate',
        'Admin.ShippingMethods.ProductWidth',
        'Admin.Handlers.Tasks.EditTaskHandler.TaskMessage',
        'Admin.ShippingMethods.Boxberry.ItemHeight',
        'Admin.Settings.Checkout.ErrorAtMinimumCertificatePrice',
        'Admin.Customers.BonusCard.Bonuses',
        'Admin.PaymentMethods.UniversalPayGate.Password1UsedInterface',
        'Core.Catalog.Product.ProductInCategories',
        'Admin.Design.Index.CSSeditor',
        'Admin.Customers.Index.Administrators',
        'Admin.Partners.Partner.Email',
        'Landings.LeadSourceName.Funnel',
        'Admin.Customers.Add.Title',
        'Admin.Menus.Index.MainMenuSubTitle',
        'Catalog.Sorting.SortByPrice',
        'Admin.Product.Landing.NotActive',
        'Admin.Js.ExportOptions.Value220',
        'Admin.Js.TelegramAuth.Instruction',
        'Admin.Settings.User.VideoTutorial',
        'Js.Registration.Email',
        'Checkout.CheckoutUser.IAmNewCustomer',
        'Admin.Catalog.Index.VideoTutorialGoods',
        'Admin.Js.AdditionBonus.ErrorWhileWritingOffBonuses',
        'Admin.Js.AddEditNewsCategpry.H1)',
        'Bonuses.ErrorRequired',
        'Core.ExportImport.ProductFields.Fee',
        'Admin.Orders.AddEdit.Calls',
        'Admin.SettingsBonus.SMS.UniSender.Password',
        'Admin.Settings.BankSettings.CompanyName',
        'Admin.SettingsCrm.Index.SalesFunnels',
        'Admin.Js.Tasks.CompleteTask.DoNotChange',
        'Admin.ShippingMethods.Boxberry.ProductLength',
        'Admin.PaymentMethods.Qiwi.CurrencyRareRelation',
        'Admin.SettingsSystem.Localization.Russian',
        'Admin.ShippingMethods.Length',
        'Admin.SettingsMail.EmailSettings.HelpSection',
        'Admin.UseGlobalVariables',
        'Admin.ShippingMethods.ShipByWeight.DefaultProductParametersValues',
        'Admin.ShippingMethods.CheckoutRu.GroupByType',
        'Admin.Home.Menu.PriceRegulation',
        'Checkout.Index.PickpointError',
        'Admin.PaymentMethods.Platron.CurrencyRateRelation',
        'Admin.Settings.ErrorWhileRegistering',
        'Admin.Account.Password',
        'Admin.Js.CategoriesBlock.SelectAllCategories',
        'Admin.Home.Menu.Users',
        'Common.Copyright.Copyright',
        'Admin.Settings.Telephony.Telphin.FtpPass',
        'Admin.ShippingMethods.Weight',
        'Admin.Analytics.Income',
        'Admin.ExportFeeed.Settings.ExportNotAmountProducts',
        'Admin.Js.OrderItemsSummary.CreateDraftOrderBoxberry)',
        'Admin.Modules.VideoTutorialModules',
        'Admin.ShippingMethods.EmsPost.DefaultOrderWeight',
        'Core.ExportImport.MultiOrder.CouponCost',
        'Checkout.PrintOrder.PaymentMethod',
        'Admin.Settings.News.DisplayMode',
        'Core.Services.Features.EFeature.MobileApp.Description',
        'Module.SmsNotifications.SubscribeForOrders',
        'Admin.Home.Menu.SendMessage',
        'Admin.ShippingMethods.Height',
        'Shared.ProductView.Photos5',
        'Admin.Import.ImportProducts.AddTheFile',
        'Admin.Customers.BonusCard.Points',
        'Js.Registration.LastName',
        'Admin.Js.BookingUsers.FailedToGetServicesList',
        'Admin.Home.Menu.TaskGroups',
        'Admin.ShippingMethods.YandexDelivery.Mm',
        'Admin.ShippingMethods.BoxBerry.UseInsurance',
        'Checkout.CheckoutUser.WantToRegister',
        'Admin.Settings.System.API',
        'Shared.ProductView.Photos0',
        'Js.ShoppingCart.AddShoppingCartSuccess',
        'Core.FileHelpers.FilesHelpText.TaskAttachment',
        'Module.SmsNotifications.Legend',
        'Admin.Js.LeadItemsSummary.DeliveryDate',
        'MyAccount.ChangePassword.PasswordSaved',
        'Admin.ShippingMethods.Sdek.ForExample120',
        'Admin.ShippingMethods.NovaPoshta.RateToBaseCurrency',
        'Admin.PaymentMethods.Qiwi.PaymentCurrency',
        'Admin.NotAvailableFeature',
        'Admin.Settings.Customers.FreshdeskIntegration',
        'Admin.PaymentMethods.Assist.Password',
        'Admin.SmsTemplates.Error.TemplateExist',
        'Admin.ShippingMethods.Boxberry.ForExample1or0.2',
        'Admin.Partners.View.Type',
        'Js.Inplace.ErrorRichSave',
        'Admin.Home.Menu.StylesEditor',
        'Admin.Settings.Crm.Header',
        'Admin.Settings.Methods.Error',
        'Admin.SettingsMail.EmailSettings.VideoTutorial',
        'Admin.Catalog.NotSelected',
        'Admin.Js.ExportFeeds.ChangesNotSaved',
        'AdvantShop.Trial.TrialService.LeftDay2',
        'Admin.Js.Kanban.NewLead',
        'Admin.Modules.Index.InstalledModules',
        'Admin.Import.ImportProducts.VideoTutorialGoods',
        'Admin.PaymentMethods.Kupivkredit.AmountOfFirstPayment',
        'Admin.SettingsCrm.Index.DefaultFunnel',
        'Admin.Js.ExportOptions.Default',
        'Common.Telephony.PhoneMask',
        'Core.ExportImport.MultiOrder.DiscountPrice',
        'Catalog.Sorting.SortByName',
        'Admin.MailFormat.Title',
        'Module.SmsNotifications.SaveChanges',
        'Admin.ShippingMethods.Edost.RateToCurrency',
        'Admin.Settings.MobileVersion.RedirectToSubdomain',
        'Admin.Js.ShowEmail.To',
        'Js.CheckOrder.Address',
        'Admin.SettingsCheckout.CheckoutFields.VideoTutorial',
        'Admin.Category.AdminCategoryModel.Error.UrlFormat',
        'Admin.Js.Design.ApplyInstalledTempale',
        'Admin.ShippingMethods.YandexDelivery.Length',
        'Admin.SettingsBonus.SMS.UniSender.Login',
        'Admin.Menus.Index.MobileMenuTitle',
        'Core.FileHelpers.FilesHelpText.LeadAttachment',
        'Admin.Product.Landing.LandingPage',
        'Admin.CountryRegionCity.Index.AddRegion',
        'Admin.SettingsBonus.Index.SmsProviderGateway',
        'Admin.Js.MainBonus.Close',
        'Catalog.Index.CatalogTitle',
        'Admin.Orders.OrderItems.UsersComment',
        'MyAccount.CommonInfo.ErrorSavingData',
        'Admin.Js.Tasks.Tasks.CopyTaskName',
        'Admin.Js.AddLanding.LandingType1)',
        'Admin.Js.AddExportFeed.Type',
        'App.Landing.Domain.Forms.ELpFormFieldType.CustomerGroup',
        'Admin.ShippingMethods.ProductWeigtAssume',
        'Admin.Voting.Index.AddVoting',
        'Bonuses.PhoneNumberExists',
        'Js.OrderConfermationCart.PhoneMask',
        'Core.Orders.Order.PaymentCancelEmailSubject',
        'Admin.Customers.Index.CustomerGroup',
        'Admin.PaymentMethods.Assist.CodeMustBe',
        'Admin.Js.SendSMS.TrackNumber',
        'Admin.CustomerSegments.AddEdit.SendSmsNotEnabled',
        'Admin.Js.Landing.Name',
        'Admin.PaymentMethods.OnPay.CurrencyInWhichPaymentMade',
        'Admin.MailFormat.Subject',
        'Admin.Js.Landing.TheyActive',
        'Js.Builder.StructureText',
        'Admin.Answers.Index.Back',
        'Admin.Handlers.Tasks.EditTaskHandler.TaskCompleted',
        'Admin.Handlers.Tasks.AddTaskHandler.TaskAdded',
        'Admin.PaymentMethods.UniversalPayGate.EnterMerchantLogin',
        'Admin.ShippingMethods.Boxberry.Length',
        'Core.Diagnostics.Debug.BugTrackerSession',
        'Admin.ShippingMethods.Grastin.ForExample1or02',
        'Admin.ShippingMethods.ProductLength',
        'Bonuses.WrongPhoneNumber',
        'Admin.Product.Landing.Edit',
        'Admin.ShippingMethods.Width',
        'Admin.Leads.Index.Processing',
        'Core.Diagnostics.Debug.BugTrackerBrowser',
        'Admin.Settings.Index.LogoImgAlt',
        'Admin.Js.Order.OrderCertificateRemoveError',
        'Admin.Js.ExportOrders.Export',
        'Js.Address.FullName',
        'Core.Modules.TrialTill',
        'Shared.ProductView.Photos2',
        'Bonuses.CardNotExist',
        'Admin.Design.TemplateSettings.Title',
        'Admin.ShippingMethods.ShipByRange.ProductWeight',
        'Admin.Js.SendSocialMessage.Empty',
        'Admin.PaymentMethods.PayOnline.CurrencyCodeISO3',
        'Admin.Orders.AddEdit.ExportCsv',
        'Admin.Js.SettingsTelephony.DeActivating',
        'Admin.Leads.CustomerSocial.Loading',
        'Admin.PaymentMethods.Assist.TestMode',
        'Admin.ShippingMethods.ForExample120',
        'Admin.PaymentMethods.IntellectMoneyMainProtocol.SiteNumberWhichCustomerMustPay',
        'Admin.PaymentMethods.Platron.AmountCurrency',
        'Admin.Settings.Social.ShareButtonsType',
        'Admin.PaymentMethods.PayPal.CurrencyExchangeRateRelation',
        'Admin.Subscribe.Export.Header',
        'Core.Payment.PaymentMethod.ButtonTextPayOrder',
        'AdminMobile.Leads.ErrorManagerIsNotSelected',
        'Admin.Js.ProductProperties.Back',
        'Admin.Module.MailChimp.ShopSubscribers',
        'Core.ExportImport.MultiOrder.ShippingCost',
        'Core.Orders.Order.PaymentEmailSubject',
        'Admin.Js.AddEditNewsCategpry.MetaHeader)',
        'Admin.Js.SendSocialMessage.VKontakte',
        'Admin.PaymentMethods.Assist.ProcessingOperations',
        'Admin.Settings.Telephony.Telphin.DialInUrl',
        'Admin.Settings.Customers.EnableLoging',
        'Admin.Orders.AddEdit.Emails',
        'Core.ExportImport.ProductFields.Cpa',
        'Admin.Product.LandingFunnels.LandingFunnelsList',
        'Admin.Orders.OrderItem.PaymentExtracharge',
        'Admin.Certificates.Index.SettingsCertificates',
        'Admin.ShippingMethods.Boxberry.ProductWeight',
        'Admin.Account.Login',
        'Admin.Orders.ClientInfo.DateOfRegistration',
        'Admin.PaymentMethods.PayAnyWay.CurrencyRate',
        'Admin.ShippingMethods.Sdek.DefaultProductParametersValue',
        'Admin.Leads.Grid.FildTotal',
        'Admin.Module.UniSender.ShopSubscribers',
        'Admin.Settings.BankSettings.BIK',
        'Admin.Currencies.ECurrencyRound.UpToWhole',
        'Admin.ExportFeeed.ChoiceOfProducts.ExportCatalog',
        'Admin.ShippingMethods.ValueIsSpecifiedInKG',
        'Admin.Js.UserInfoPopup.AllFieldsAreRequired',
        'Admin.PaymentMethods.OnPay.ExchangeCurrencyRate',
        'Admin.Js.SettingsCrm.NewLeadsList',
        'Admin.SettingsTasks.Tasks.VideoTutorial',
        'Core.Orders.Order.PaymentEmailBody',
        'Core.Customers.Customer.Contacts',
        'Admin.MailFormat.Main',
        'Js.Registration.Patronymic',
        'Admin.Settings.SettingsCrm.OrderStatusFromLead',
        'Core.SalesChannels.SalesChannel.Facebook.Description',
        'Admin.Js.SettingsCrm.SalesFunnel',
        'Admin.Settings.System.LogError.Session',
        'Admin.Currencies.ECurrencyRound.UpToFirstPlace',
        'Admin.PaymentMethods.Robokassa.Password1UsedInterface',
        'Admin.Js.AddLanding.Name',
        'Admin.CountryRegionCity.Index.CityHeader',
        'Admin.SettingsBonus.Sms.Sms4B.Login',
        'Admin.Home.Menu.DesignConstructor',
        'Admin.Js.ProductProperties.EnterValue',
        'Admin.Customers.BonusCard.BonusCardNumber',
        'Admin.Settings.EnterTheData',
        'Admin.PaymentMethods.OnPay.ExchangeRate',
        'Admin.Settings.Checkout.ErrorAtMinimumOrderPriceForDefaultGroup',
        'Admin.PaymentMethods.PayPal.CurrencyRate',
        'Admin.Js.UserInfoPopup.ConsentToProcessing',
        'Admin.Customers.ViewCustomerInfo.NotAssigned',
        'Admin.PaymentMethods.Assist.CurrencyRateInRelation',
        'Admin.Currencies.ECurrencyRound.UpToTens',
        'Admin.ShippingMethods.Sdek.WidthOfProductWillTake',
        'Admin.ShippingMethods.ShipByRange.ProductWeightWillAssume',
        'Admin.Settings.System.LogError.Request',
        'Admin.SettingsBonus.Sms.Sms4b.Password',
        'Admin.Js.OffersSelect.VendorCode',
        'Admin.Settings.IncorrectEmail',
        'Admin.PaymentMethods.Kupivkredit.PercentageOfTheFirstPayment',
        'Admin.Currencies.ECurrencyRound.UpToSecondPlace',
        'Admin.PaymentMethods.MailRu.Iso3CurrencyCode',
        'Admin.Settings.BankSettings.BankName',
        'Admin.Js.FacebookAuth.VerifyToken',
        'Admin.PaymentMethods.Common.MarkupOfPaymentMethod',
        'Admin.ShippingMethods.Common.SpecifyListOfCountriesAndCitiesForPayment',
        'Admin.SettingsBonus.SMS.UniSender.HaveAccount',
        'Core.ExportImport.CategoryFields.ExternalCategoryId',
        'Admin.ExportFeed.SelectionGoods',
        'Admin.Js.MailSettings.SuccessRequestUpdateStatus',
        'Admin.Js.BookingSheduler.Date',
        'Admin.Landings.CreateFunnel.Back',
        'Core.Diagnostics.Debug.BugTrackerCommonInfo',
        'Admin.ExportFeeds.Index.VideoTutorialYandex',
        'Admin.SettingsBonus.Sms.EPochta.Register',
        'Admin.SearchBlock.Orders',
        'Admin.ShippingMethods.Sdek.Height',
        'Admin.ShippingMethods.Boxberry.ItemWidth',
        'Admin.Menus.Index.AddMenuItem',
        'Admin.ShippingMethods.Grastin.EnterTheMarkupInBaseCurrency',
        'Admin.Settings.Mobile.MobileVersion.VideoTutorialMobile',
        'Admin.SettingsTelephony.Index.VideoTutorial',
        'Admin.PaymentMethods.Kupivkredit.PartnerIdentifier',
        'Admin.PaymentMethods.Kupivkredit.SecretKey',
        'Admin.News.AddEdit.AllNews',
        'Admin.ShippingMethods.Sdek.ValueIsSpecifiedInKg',
        'Js.Registration.FirstName',
        'Admin.Settings.BankSettings.RS',
        'Admin.Settings.NotifyEMails.NotificationsHint',
        'Admin.PaymentMethods.Assist.Login',
        'Admin.OrderStatuses.Index.AddStatus',
        'Admin.MainPageProducts.Index.Goods',
        'Admin.Js.TemplateSettingsModal.Title',
        'Admin.Js.ProductProperties.AddProperty',
        'MyAccount.ChangePassword.ForgotPassword',
        'Admin.CountryRegionCity.Index.AddCountry',
        'Admin.Settings.BankSettings.Guidance',
        'Admin.Js.Tasks.Tasks.StatusChanged',
        'Admin.Settings.Template.VotingVisibilityHelp',
        'Admin.Settings.Shippings.ShippingMethods.VideoTutorialShipping',
        'Admin.SettingsCrm.Index.CommunicationWidget',
        'Admin.Leads.Index.ClosedDeal',
        'Admin.Settings.Telephony.Telphin.HangupUrl',
        'Core.Configuration.TemplateSettings.ErrorSaveSettings',
        'Admin.ShippingMethods.Common.ExtrachargeType',
        'Admin.ExportFeed.SettingsYandex.Bid',
        'Admin.SettingsCrm.Index.Save',
        'Admin.Js.UserInfoPopup.YourName',
        'Admin.Js.Partner.UnbindCoupon',
        'Admin.Js.Colors.ErrorWhileCreating',
        'Admin.ShippingMethods.NovaPoshta.DefaultWeight',
        'Admin.Home.BirthdayDashboard.Description.Days',
        'Admin.ShippingMethods.CheckoutRu.CustomerIdentificationKey',
        'Admin.Home.Congratulations.InputData',
        'PreOrder.Index.Name',
        'Js.Registration.Phone',
        'Admin.Js.SendSocialMessage.MessageText',
        'Admin.Layout.SaasBlock.SaasPeriodTill',
        'Admin.ShippingMethods.Sdek.LengthOfProductWillTakeSpecifiedValue',
        'Admin.Design.Index.MoreAboutPrices',
        'Core.Orders.OrderHistory.AddedCertificate',
        'Admin.ShippingMethods.Common.ExtrachargeFromOrderHelp',
        'AdvantShop.Trial.TrialService.LeftDay1',
        'Admin.ShippingMethods.Grastin.Weight',
        'Admin.Settings.MobileVersion.CatalogProductsCount',
        'Admin.ShippingMethods.YandexDelivery.IfProductsHaveNoSize',
        'Module.BonusSystem.ClientIdText',
        'Admin.ShippingMethods.Grastin.MarkupInBaseCurrency',
        'Admin.Js.Triggers.Title',
        'Admin.Js.Tasks.Tasks.HasBeenCopied',
        'Admin.Home.Menu.SiteMapXmlGenerate',
        'Admin.ShippingMethods.Sdek.ForExample100',
        'Admin.ShippingMethods.Boxberry.Height',
        'Core.Customers.RoleActionCategory.Cms',
        'Admin.Js.UserInfoPopup.YourDataIsProtected',
        'Admin.MailFormat.Type',
        'Admin.Settings.MailFormats.Title',
        'Admin.PaymentMethods.Robokassa.MerchantLogin',
        'Admin.ShippingMethods.Sdek.Length',
        'Admin.Marketing.ExportProducts',
        'Admin.Js.ExportProducts.Export',
        'Core.ExportImport.ProductFields.YandexProductCategory',
        'Admin.Close',
        'Admin.Leads.Index.NotClosedDeal',
        'Admin.MailFormat.SortOrder',
        'Admin.Settings.BankSettings.INN',
        'Admin.ShippingMethods.Sdek.ProductHeight',
        'Admin.PaymentMethods.Assist.CurrencyRate',
        'Admin.Files.Index.FileWithDangerousContent',
        'Core.Customers.RoleActionCategory.Design',
        'Core.Orders.OrderHistory.DeletedCertificate',
        'Bonuses.CardNotFound',
        'AdminMobile.Leads.ErrorNotManager',
        'Checkout.CheckOrder.OrderNotFound',
        'Admin.PaymentMethods.PayPal.CurrencyCode',
        'Js.Bonus.WantBonusCard',
        'Admin.ShippingMethods.ShipByRange.ValueSpecifiedInKg',
        'Admin.ShippingMethods.ProductWeight',
        'Admin.ShippingMethods.ProductWidthTakeValue',
        'Admin.Menus.Index.BottomMenuTitle',
        'Admin.Settings.BankSettings.KS',
        'Admin.ShippingMethods.YandexDelivery.AverageWeight',
        'Admin.Js.NewsCategory.AreYouSureDelete',
        'Admin.Js.AddExportFeed.TypeColon',
        'Admin.SettingsBonus.Sms.StreamTelecom.Password',
        'Admin.Js.UserInfoPopup.Answer7questions',
        'Admin.Js.SettingsTelephony.AreYouSureDeActivate',
        'Admin.BookingEmployees.Index.Employees',
        'Bonuses.CheckData',
        'Admin.Home.Menu.OrdersByRequest',
        'Admin.Js.Tasks.ModalEditTaskCtrl.CopySuccess',
        'Admin.PaymentMethods.UniversalPayGate.Password2UsedInterface',
        'Core.FileHelpers.FilesHelpText.BookingAttachment',
        'Admin.ShippingMethods.ShipByWeight.ValueIsSpecified',
        'Admin.Js.OrderItemsSummary.DeleteDraftOrderBoxberry)',
        'Module.SmsNotifications.SaveData',
        'Module.SmsNotifications.NoSaveData',
        'Admin.Home.Menu.TemplateSettings',
        'Admin.SettingsSystem.Localization.English',
        'Admin.ShippingMethods.Grastin.ProductWeight',
        'Admin.PaymentMethods.PayAnyWay.CurrencyCodeISO3',
        'Admin.MailFormat.Delete',
        'Admin.MailFormat.NewFormatTitle',
        'Admin.Settings.BankSettings.Manager',
        'Admin.Js.VkAuth.ConnectedGroup',
        'Admin.Home.Menu.Administrators',
        'Admin.ExportFeeds.Index.NoUnloadsAvailable',
        'Admin.Design.Index.TemplateDeveloper',
        'MyAccount.CommonInfo.DataSuccessSaved',
        'Admin.PaymentMethods.Assist.EnterSellerLogin',
        'Admin.ShippingMethods.ShipByWeight.ForExample1or02kg',
        'Admin.Orders.ClientInfo.GroupOfUsers',
        'Admin.Settings.BankSettings.CompanyStamp',
        'Admin.ShippingMethods.Boxberry.ProductWeightWillAssumeValue',
        'Js.Registration.SubscribeToNews',
        'Admin.Js.ExportOptions.NeedToBeExcluded',
        'Admin.ShippingMethods.Sdek.ProductWeight',
        'AdminAdmin.Settings.Checkout.AddTax',
        'Admin.Settings.Customers.Common',
        'Admin.MailFormat.Enabled',
        'Admin.Home.Congratulations.FewSteps',
        'Admin.ShippingMethods.Boxberry.IncreaseDeliveryTimeHelp',
        'Admin.Js.SettingsCrm.AddAFunnel',
        'User.Login.SignInFunnnel',
        'Admin.Js.Tasks.EditTask.Lead',
        'Admin.Product.LandingFunnels.LandingFunnelsDescription',
        'Admin.Js.UserInfoPopup.CompleteRegistration',
        'Admin.Settings.MobileVersion.DisplayCategoryMenuIcons',
        'Core.Orders.Order.PaymentCancelEmailBody',
        'Admin.Settings.System.PageStructure',
        'Admin.Js.SettingsCrm.ErrorWhileSaving',
        'Admin.Js.Partner.CantDelete',
        'Admin.Js.Landing.Inactive',
        'Admin.ExportFeeed.Settings.ExportNotActiveProducts',
        'Admin.ShippingMethods.CheckoutRu.KeyInPersonalAccount',
        'Admin.ShippingMethods.Edost.EnterTheRate',
        'Admin.PaymentMethods.Assist.SignCreditCardAuthorization',
        'Admin.JsProductlists.ChangesSaved',
        'Admin.Settings.BankSettings.KPP',
        'Admin.Js.ExportOptions.Thus',
        'Admin.CountryRegionCity.Index.RegionHeader',
        'Admin.Account.Login.Title',
        'Admin.Js.Landing.Activity',
        'Admin.Home.Menu.Voting',
        'Admin.Product.Landing.AdditionalDescription',
        'Admin.Leads.Index.New',
        'Admin.ShippingMethods.Sdek.ProductHeightWillAssumeValue',
        'Admin.SettingsBonus.SMS.UniSender.SignUp',
        'AdminMobile.Leads.Lead.AdminComment',
        'Admin.PaymentMethods.Robokassa.EnterMerchantLogin',
        'Js.ShoppingCart.СonfirmButtonText',
        'Admin.Product.CustomOptions.EnterValidNumber',
        'Admin.Settings.MailFormats.AddMailFormat',
        'Admin.ShippingMethods.Boxberry.ForExample120',
        'Admin.Properties.Index.GroupName',
        'Admin.Settings.System.Files',
        'Admin.Home.Menu.Moderators',
        'Checkout.PrintOrder.ShippingMethod',
        'Js.Bonus.OnCard',
        'Admin.Customers.Index.Moderators',
        'Admin.Settings.BankSettings.Address',
        'Checkout.CheckoutUser.IAmRegisteredCustomer',
        'Admin.Js.ProductProperties.EnterName',
        'Admin.ExportFeed.Delete',
        'Admin.ExportCategories.Settings.CategoriesExport',
        'Admin.ShippingMethods.Boxberry.ValueIsSpecifiedInMm',
        'Admin.Js.UserInfoPopup.PleaseAnswerAllQuestions',
        'Admin.Home.Menu.Resellers',
        'Admin.PaymentMethods.Robokassa.Password2UsedInterface',
        'Admin.Js.VkAuth.If',
        'Admin.PaymentMethods.LiqPay.StoreCurrency',
        'Admin.Js.ExportProducts.EnterProductArtno',
        'Admin.Settings.News.Title',
        'Admin.ShippingMethods.Sdek.Rub',
        'Admin.ExportFeed.SettingsAvito.CityArea',
        'Admin.Settings.Checkout.ErrorAtNextOrderNumber',
        'Admin.Settings.BankSettings.Director',
        'Admin.Currencies.ECurrencyRound.UpToHundreds',
        'Admin.ShippingMethods.Sdek.ForEachTariffPlan',
        'Admin.Settings.Template.VotingVisibilityTitle',
        'Admin.SettinsBonus.Index.SenderName',
        'Admin.TasksLayout.Menu.TaskGroups',
        'Admin.Js.VkAuth.Step3',
        'Admin.MailFormat.AddEditTitle',
        'Admin.ShippingMethods.Sdek.Weight',
        'Admin.ShippingMethods.Sdek.OnlyOneOptionCanBeSelected',
        'AdminMobile.Leads.ErrorUserEmailExist',
        'Admin.Orders.OrderItems.NoComments',
        'Admin.Home.Menu.VotingHistory',
        'AdvantShop.Trial.TrialService.LeftDay5',
        'Admin.ShippingMethods.ShipByWeight.ProductWeight',
        'Admin.ShippingMethods.Sdek.ValueIsIndicatedInMm',
        'Admin.Settings.Customers.FreshdeskWidgetCode',
        'Admin.Menus.Index.BottomMenuSubTitle',
        'Admin.Js.Colors.Error',
        'Admin.SearchBlock.Customers',
        'Js.ShoppingCart.СancelButtonText',
        'Admin.Home.Congratulations.AddFirstGoods',
        'Admin.Js.ExportOptions.Value1220',
        'Module.SmsNotifications.SubscribeForNews',
        'Admin.ExportFeed.ExportParameters',
        'Admin.ExportFeed.SettingsAvito.City',
        'Admin.ShippingMethods.Boxberry.Width',
        'Admin.PaymentMethods.MailRu.CheckTheSignature',
        'Admin.Customers.RightBlock.CustomerSegments',
        'Core.Catalog.Product.Categories',
        'Admin.ShippingMethods.ShipByRange.ProductDefaultParametersValues',
        'Admin.CountryRegionCity.Index.AddCity',
        'Admin.PaymentMethods.InvoiceBox.CurrencyOfPayment',
        'Admin.Design.Index.VideoTutorialDesign',
        'Admin.ShippingMethods.ProductLengthTakeValue',
        'Core.Bonuses.ESmsType.ChangeGrade',
        'Admin.Js.Design.TemplateSettingsSavedSuccessfully',
        'Admin.Analytics.AverageCheck',
        'Admin.Js.ExportOptions.CanIncreaseCommission',
        'Admin.Js.UserInfoPopup.CompanyName',
        'Admin.Settings.System.LogError.Browser',
        'User.Registration.ErrorCaptcha',
        'Admin.SettingsBonus.Sms.EPochta.PrivateKey',
        'Admin.Modules.LeftMenu.VideoTutorialModules',
        'Voting.VotingBlock.Vote',
        'Admin.Js.UserInfoPopup.ToSetUpYourAccount',
        'Voting.VotingBlock.VotingHeader',
        'Admin.Settings.Tasks.FilesSize',
        'Admin.Settings.Catalog.PriceTypes',
        'Admin.SettingsCatalog.Search.VideoTutorial',
        'Core.Customers.SendMail.NotTrackNumber',
        'Admin.Settings.Common.Common.MoreInfo',
        'Admin.Settings.Checkout.UserAgreementTextField',
        'Catalog.Sorting.SortByPopular',
        'Admin.ShippingMethods.ProductHeight',
        'Admin.PaymentMethods.PayOnline.CurrencyRate',
        'Core.Services.Features.EFeature.MobileApp',
        'Core.Services.Features.EFeature.ProductCategoriesAutoMapping',
        'Voting.VotingBlock.AllVotes',
        'Admin.Shared.Footer.MobileAdmin',
        'Admin.PaymentMethods.Assist.CurrencyCodeISO3',
        'Admin.Settings.System.Applications',
        'Admin.ExportFeeed.SettingsReseller.RecomendedPriceMargin',
        'Admin.ShippingMethods.Boxberry.LengthOfItemWillTakeValue',
        'Admin.ShippingMethods.NovaPoshta.EnterTheRateToStoreCurrency',
        'Core.ExportImport.MultiOrder.PaymentCost',
        'Admin.PaymentMethods.PayOnline.CanUseThisCurrencies',
        'Admin.ShippingMethods.Sdek.ProductLength',
        'Admin.ShippingMethods.Boxberry.ValueIsKg',
        'Admin.Js.Order.DiscountCanBe',
        'Admin.Js.SendSocialMessage.MessageTemplate',
        'Core.Lead.DealStatusChangedFormat',
        'Admin.ShippingMethods.DDelivery.DefaultProductParameters',
        'Admin.Home.Congratulations.CreateFirstCategories',
        'Admin.ShippingMethods.Boxberry.DefaultProductValues',
        'Admin.Product.Landing.Insctructions',
        'Admin.Common.TopMenu.DomainsManagment',
        'Admin.PaymentMethods.Platron.Currency',
        'Landings.OrderSourceName.Funnel',
        'Admin.Settings.Catalog.SearchIndexHint',
        'Admin.Customers.View.GeneralInformation',
        'Admin.Js.UserInfoPopup.FullFunctionality',
        'Admin.PaymentMethods.Kupivkredit.ButtonBuyOnCredit',
        'Admin.ShippingMethods.Sdek.CostInBaseCurrency',
        'Admin.Settings.SettingsCrm.StageSuccessful',
        'Admin.ExportFeed.SettingsYandex.PositiveIntegerNumbers',
        'Admin.ShippingMethods.Boxberry.WidthOfItemWillTakeValue',
        'Admin.Settings.Payments.PaymentMethods.VideoTutorialPayments',
        'Admin.ShippingMethods.Grastin.ProductWeightWillAssume',
        'Admin.ShippingMethods.Boxberry.HeightOfItemWillTakeValue',
        'AdvantShop.TrialTrialService.LeftDay0',
        'Admin.PaymentMethods.LiqPay.CustomersPaymentCurrency',
        'Admin.SettingsCheckout.CheckoutCommon.VideoTutorial',
        'Admin.Js.ExportCustomers.CustomersAtGroup',
        'Admin.ShippingMethods.ForExample1or0.2',
        'Admin.Js.SendSocialMessage.EditingTemplates',
        'AdminAdmin.Settings.Checkout.TaxesTitle',
        'Admin.Settings.Telephony.Telphin.FtpUser',
        'Admin.ShippingMethods.ShipByWeight.WeightWillAssume',
        'Admin.MailFormat.Name',
        'Admin.PaymentMethods.Qiwi.CurrencyRate',
        'Admin.Design.Index.TemplateShop',
        'Admin.ShippingMethods.Boxberry.Weight',
        'Admin.Product.LandingFunnels.LandingFunnels',
        'Catalog.Sorting.SortByDatetime',
        'Admin.Leads.CustomerSocial.Add',
        'Admin.TasksLayout.Menu.Tasks',
        'Admin.ShippingMethods.YandexDelivery.Kg',
        'Admin.TasksLayout.Menu.Settings',
        'Admin.Settings.Checkout.CheckoutCustomerFieldsInstruction',
        'Admin.SettingsApi.Index.ShowExample',
        'Admin.Settings.Russian',
        'Module.SmsNotifications.Wrong_CodeCountryPhone',
        'Admin.Voting.Index.Title',
        'Bonuses.ErrorBirthDay',
        'Admin.Customers.BonusCard.CardBlocked',
        'Admin.SettingsMail.EmailSettings.EmailFromWhichSending',
        'Admin.Common.SaasBlock.Balance',
        'Admin.ShippingMethods.Grastin.Example.Tver',
        'Admin.Js.Triggers.NewTrigger',
        'Bonuses.WrongCode',
        'Admin.PaymentMethods.InvoiceBox.CurrencyCode',
        'Admin.Settings.System.LogError.GeneralData',
        'User.Registration.BonusesApplied',
        'Admin.Currencies.ECurrencyRound.UpToThousands',
        'Admin.PaymentMethods.Assist.AuthorizationSign',
        'Admin.ShippingMethods.Boxberry.IncreaseDeliveryTime',
        'Mobile.Product.ReturnText',
        'Admin.ShippingMethods.Sdek.AdditionalMarkupForDelivery',
        'Admin.Cards.Index.VideoTutorial',
        'Landing.Domain.LpFunnelType.Booking',
        'Feedback.Index.AgreementText',
        'Voting.VotingBlock.Results',
        'Admin.Settings.System.LogError.OtherErrors',
        'Admin.PaymentMethods.MailRu.CurrencyCodeOfPayments',
        'Admin.Customers.Index.VideoTutorialCustomers',
        'Admin.SettingsBonus.Sms.StreamTelecom.Login',
        'Admin.Designs.TemplateDeleted',
        'Admin.Settings.System.LogError.Info',
        'Admin.Js.LeadEvents.Events',
        'Admin.ShippingMethods.ProductHeightTakeValue',
        'Admin.ShippingMethods.Sdek.ProductWidth',
        'Admin.PaymentMethods.UniversalPayGate.MerchantLogin',
        'Core.Orders.OnRefreshTotalOrder.DiscountCost',
        'Admin.Js.ExportOptions.CommissionAmount',
        'Common.MenuGeneral.Brands',
        'Core.Payment.PaymentButtonText',
        'Admin.ShippingMethods.Grastin.IncreaseDeliveryTimeHelp',
        'Admin.ExportFeeed.ChoiceOfProducts.AllProducts',
        'Admin.CountryRegionCity.Index.CountryHeader',
        'Admin.SettingsSeo.SEOSettings.VideoTutorial',
        'Admin.Js.UserInfoPopup.14daysFree',
        'Admin.MailFormat.AllFormats',
        'Admin.Settings.Customers.FreshdeskDomain',
        'Admin.Js.ChangeOrderStatus.And',
        'Admin.Settings.Commonpage.Partners',
        'Admin.Settings.SystemSettings.AdditionalHeadMetaTagInstruction',
        'Admin.SettingsMail.Notifications.VideoTutorial',
        'Admin.Orders.AddEdit.Vkontakte',
        'Admin.ShippingMethods.ValueIndicatedInMM',
        'Admin.Menus.Index.MainMenuTitle',
        'Admin.SettingsBonus.Sms.EPochta.PublicKey',
        'Admin.Answers.Index.Title',
        'Admin.SettingsMail.MailFormats.VideoTutorial',
        'Module.SmsNotifications.CodeCountryAndPhone',
        'Admin.Subscribe.Export.UnsubscribeReason',
        'Admin.Settings.English',
        'User.Registration.AgreementText',
        'Admin.Handlers.Leads.GetLeadEvents.LeadCreated',
        'Admin.PaymentMethods.Platron.CurrencyRate',
        'Admin.Product.LeftMenu.LandingPage',
        'Admin.SettingsCrm.Index.VideoTutorial',
        'Admin.Js.CopyProduct.AvailableVariablesTitle',
        'Admin.Menus.Index.MobileMenuSubTitle',
        'Admin.SearchBlock.Products',
        'Admin.ShippingMethods.Edost.DefaultProductParameters',
        'Admin.Js.UserInfoPopup.WhyThisNecessary',
        'Admin.ShippingMethods.Sdek.ProductWeightWillAssumeValue',
        'Admin.Designs.ErrorRemovingTemplate',
        'Admin.Attachments.InvalidFileExtension',
        'Admin.ShippingMethods.YandexDelivery.Height',
        'Admin.ShippingMethods.Grastin.ValueIsSpecifiedInKg',
        'Admin.ExportFeed.SettingsAvito.MetroStation',
        'Mobile.Catalog.Index.CatalogFilter',
        'Admin.Tasks.NavMenu.AllProjects',
        'Admin.TopMenu.Help',
        'Admin.Settings.Telephony.Telphin.DialOutUrl',
        'Admin.MailFormat.Text',
        'Shared.ProductView.Photos1',
        'Admin.Js.SettingsSystem.Err400',
        'Admin.ShippingMethods.Sdek.DeliveryMarkUp',
        'Catalog.Sorting.SortByRating',
        'Module.SmsNotifications.Wrong_NumberPhone',
        'Admin.Answers.Index.AddAnswers',
        'Admin.Home.Congratulations.EnterData',
        'AdminMobile.Leads.TaskName',
        'Bonuses.ErrorPhone',
        'Admin.ShippingMethods.Grastin.ExampleStPetersburg',
        'Js.Cart.PriceWithDiscount',
        'Admin.Js.AddEditRegions.SortingOrder',
        'Admin.Design.Index.Developer',
        'Admin.Settings.Catalog.SearchExampleHintTitle',
        'Admin.Js.ExportOptions.CommissionAmountForOffer',
        'Admin.Settings.Index.BankDetails',
        'Admin.PaymentMethods.OnPay.PaymentCurrency',
        'Admin.Js.AdditionBonus.Close',
        'Admin.ShippingMethods.Sdek.Width',
        'Js.Registration.Registrate',
        'Admin.PaymentMethods.Qiwi.ISOCurrencyCode',
        'Admin.PaymentMethods.Assist.URLforWorkMode',
        'Admin.Product.Landing.LinkToLandingPage',
        'Catalog.Sorting.SortByNoSorting',
        'Admin.ShippingMethods.NovaPoshta.DefaultWeightIfNotSpecified',
        'Admin.Leads.CrmNavMenu.AllLeads',
        'Admin.Orders.OrderItem.ShippingPrice',
        'Admin.Js.Tasks.ModalEditTaskCtrl.ChangesSaved',
        'Admin.Js.Product.ErorWhileSaving',
        'Admin.ExportFeed.SettingsAvito.Region',
        'Admin.Home.Congratulations.EnterNameStore',
        'Admin.Settings.System.LogError.Errors500',
        'Admin.Settings.BankSettings.Accountant',
        'Admin.Js.ExportOptions.ExcludeProduct',
        'Admin.ShippingMethods.YandexDelivery.Width',
        'Admin.Settings.System.CustomersNotifications',
        'Admin.Dashboard.BtnDelete',
        'Admin.ShippingMethods.CheckoutRu.BunchOfStoreAndService',
        'Admin.Currencies.ECurrencyRound.None',
        'Admin.Modules.Market.ModulesShop',
        'Admin.ShippingMethods.ShipByRange.ForExample02Kg',
        'Admin.ShippingMethods.Grastin.IncreaseDeliveryTime',
        'Admin.Catalog.Index.CatalogTitle',
        'Core.Diagnostics.Debug.BugTrackerRequest',
        'Admin.ShippingMethods.Sdek.ForExample1or02'
    )

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.CreateDraftOrderSberlogistic','Создать черновик заказа в Сберлогистике')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.CreateDraftOrderSberlogistic','Create a draft order in Sberlogistics')

GO--

IF NOT EXISTS (SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Settings.Template.DefaultSortOrderProductInBrand')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
		Values
			(1,'Admin.Settings.Template.DefaultSortOrderProductInBrand','Сортировка товаров по умолчанию'),
			(2,'Admin.Settings.Template.DefaultSortOrderProductInBrand','Sorting products by default')
ELSE
BEGIN
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'Сортировка товаров по умолчанию'
	WHERE [ResourceKey] = 'Admin.Settings.Template.DefaultSortOrderProductInBrand' AND [LanguageId] = 1
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'Sorting products by default'
	WHERE [ResourceKey] = 'Admin.Settings.Template.DefaultSortOrderProductInBrand' AND [LanguageId] = 2
END

IF NOT EXISTS (SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Settings.Template.DefaultSortOrderProductInBrandHelp')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
		Values
			(1,'Admin.Settings.Template.DefaultSortOrderProductInBrandHelp','При открытии страницы бренда, товары будут отсортированы по умолчанию'),
			(2,'Admin.Settings.Template.DefaultSortOrderProductInBrandHelp','When opening brand pages, products will be sorted by default')
ELSE
BEGIN
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'При открытии страницы бренда, товары будут отсортированы по умолчанию'
	WHERE [ResourceKey] = 'Admin.Settings.Template.DefaultSortOrderProductInBrandHelp' AND [LanguageId] = 1
	UPDATE [Settings].[Localization]
	SET [ResourceValue] = 'When opening brand pages, products will be sorted by default'
	WHERE [ResourceKey] = 'Admin.Settings.Template.DefaultSortOrderProductInBrandHelp' AND [LanguageId] = 2
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Colors.Updating','Цветам будут добавлены код или иконка цвета, для которых они не были заданы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Colors.Updating','Colors will be added with a code or icon for a color for which they have not been set')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Colors.AreYouSureUpdate','Вы уверены, что хотите обновить иконки цветов?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Colors.AreYouSureUpdate','Are you sure you want to update the flower icons?')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Colors.StartUpdateProcess','Процес обновления запущен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Colors.StartUpdateProcess','Update process started')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Colors.FinishUpdateProcess','Процес обновления завершен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Colors.FinishUpdateProcess','Update process completed')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Colors.Index.UpdateIconColors','Актуализировать иконки для цветов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Colors.Index.UpdateIconColors','Update icons for colors')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Sberlogistic.SynchronizeOrderStatuses','Синхронизировать статусы заказов из сберлогистики')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Sberlogistic.SynchronizeOrderStatuses','Synchronize order statuses from sberlogistic')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Sberlogistic.Statuses','Статусы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Sberlogistic.Statuses','Statuses')

GO--
                                                                           
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Modules.UpdatingAllModules','Идет обновление модулей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Modules.UpdatingAllModules','Modules are being updated')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.LogoOnBlog','Логотип в ссылках на магазин')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.LogoOnBlog','The logo in the links to the store')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.LogoBlogImgSizeRecommendations','Рекомендуемый размер 200 x 200 px<br> Формат может быть только *.gif, *.png или *.jpg')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.LogoBlogImgSizeRecommendations','Recomended size 200 X 200 px<br> The format can only be *.gif, *.png or *.jpg')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.StaticPages.Index.Pages','Страницы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.StaticPages.Index.Pages','Pages')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.StaticPages.Index.Blocks','Блоки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.StaticPages.Index.Blocks','Blocks')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Загрузить изображение' WHERE [ResourceKey] = 'Js.Inplace.AddPicture' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Download picture' WHERE [ResourceKey] = 'Js.Inplace.AddPicture' AND [LanguageId] = 2

GO--

ALTER TABLE CMS.LandingBlock ADD
	ShowOnAllPagesTop bit NULL,
	ShowOnAllPagesBottom bit NULL
GO--

Update [CMS].[LandingBlock] 
Set [ShowOnAllPagesTop] = [ShowOnAllPages], [ShowOnAllPagesBottom] = 0
where [Type] in ('header', 'delimiter', 'image', 'cover', 'video')
GO--

Update [CMS].[LandingBlock] 
Set [ShowOnAllPagesTop] = 0, [ShowOnAllPagesBottom] = [ShowOnAllPages]
where [Type] not in ('header', 'delimiter', 'image', 'cover', 'video')
GO--

ALTER TABLE [CMS].[LandingBlock] ALTER COLUMN [ShowOnAllPagesTop] bit NOT NULL
GO--

ALTER TABLE [CMS].[LandingBlock] ALTER COLUMN [ShowOnAllPagesBottom] bit NOT NULL
GO--

ALTER TABLE [CMS].[LandingBlock] DROP COLUMN [ShowOnAllPages]
GO--



UPDATE [Settings].[Localization] SET [ResourceValue] = 'Novofon (Zadarma)' WHERE [ResourceKey] = 'Core.IPTelephony.EOperatorType.Zadarma' AND [LanguageId] = 1

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выбираете оператора ip телефонии и далее настраиваете в соответствии с инструкцией того или иного оператора<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-sipuni" target="_blank" >Телефония. Sipuni</a> <br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-mango" target="_blank">Телефония. Манго Телеком</a><br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/phone-zadarma" target="_blank" >Телефония. Novofon (Zadarma)</a><br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/phone-telphin" target="_blank" >Телефония. Телфин</a>' WHERE [ResourceKey] = 'Admin.Settings.Telephony.OperatorHint' AND [LanguageId] = 1

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'Comment') AND object_id = OBJECT_ID(N'[Catalog].[Product]'))
    BEGIN
        ALTER TABLE [Catalog].[Product]
			ADD Comment varchar(max) NULL
    END

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.Edit.Comment','Комментарий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.Edit.Comment','Comment')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.Edit.CommentHint','Комментарий к товару для внутреннего пользования.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.Edit.CommentHint','Comment on the product for internal use.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Comment','Комментарий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Comment','Comment')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.Comment','Комментарий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.Comment','Comment')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Comment','Комментарий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Comment','Comment')

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
    @ProductID int,
    @ArtNo nvarchar(100),
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled bit,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice money,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),
    @AccrueBonuses bit,
    @TaxId int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max)
AS
BEGIN
    UPDATE [Catalog].[Product]
    SET 
         [ArtNo] = @ArtNo
        ,[Name] = @Name
        ,[Ratio] = @Ratio
        ,[Discount] = @Discount
        ,[DiscountAmount] = @DiscountAmount
        ,[BriefDescription] = @BriefDescription
        ,[Description] = @Description
        ,[Enabled] = @Enabled
        ,[Recomended] = @Recomended
        ,[New] = @New
        ,[BestSeller] = @BestSeller
        ,[OnSale] = @OnSale
        ,[DateModified] = @DateModified
        ,[BrandID] = @BrandID
        ,[AllowPreOrder] = @AllowPreOrder
        ,[UrlPath] = @UrlPath
        ,[Unit] = @Unit
        ,[ShippingPrice] = @ShippingPrice
        ,[MinAmount] = @MinAmount
        ,[MaxAmount] = @MaxAmount
        ,[Multiplicity] = @Multiplicity
        ,[HasMultiOffer] = @HasMultiOffer
        ,[CurrencyID] = @CurrencyID
        ,[ActiveView360] = @ActiveView360
        ,[ModifiedBy] = @ModifiedBy
        ,[AccrueBonuses] = @AccrueBonuses
        ,[TaxId] = @TaxId
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
		,[IsMarkingRequired] = @IsMarkingRequired
		,[Comment] = @Comment
    WHERE ProductID = @ProductID
END

GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled tinyint,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,     
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),   
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max)
AS
BEGIN
    DECLARE @Id int,
			@ArtNoUpdateRequired bit

	IF @ArtNo=''
	BEGIN
		SET @ArtNo = CONVERT(nvarchar(100), NEWID())
		SET @ArtNoUpdateRequired = 1
	END

    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
        ,[BriefDescription]
        ,[Description]
        ,[Enabled]
        ,[DateAdded]
        ,[DateModified]
        ,[Recomended]
        ,[New]
        ,[BestSeller]
        ,[OnSale]
        ,[BrandID]
        ,[AllowPreOrder]
        ,[UrlPath]
        ,[Unit]
        ,[ShippingPrice]
        ,[MinAmount]
        ,[MaxAmount]
        ,[Multiplicity]
        ,[HasMultiOffer]
        ,CurrencyID
        ,ActiveView360
        ,ModifiedBy
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,CreatedBy
        ,Hidden
        ,ManualRatio
		,IsMarkingRequired
		,Comment
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
        ,@BriefDescription
        ,@Description
        ,@Enabled
        ,@DateModified
        ,@DateModified
        ,@Recomended
        ,@New
        ,@BestSeller
        ,@OnSale
        ,@BrandID
        ,@AllowPreOrder
        ,@UrlPath
        ,@Unit
        ,@ShippingPrice
        ,@MinAmount
        ,@MaxAmount
        ,@Multiplicity
        ,@HasMultiOffer
        ,@CurrencyID
        ,@ActiveView360
        ,@ModifiedBy
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
		,@IsMarkingRequired
        ,@Comment
		);

    SET @ID = SCOPE_IDENTITY();
    IF @ArtNoUpdateRequired = 1
    BEGIN
		DECLARE @NewArtNo nvarchar(100) = CONVERT(nvarchar(100),@ID)

        IF EXISTS (SELECT * FROM [Catalog].[Product] WHERE [ArtNo] = @NewArtNo)
        BEGIN
            SET @NewArtNo = @NewArtNo + '_' + SUBSTRING(@ArtNo, 1, 5)
        END

        UPDATE [Catalog].[Product] SET [ArtNo] = @NewArtNo WHERE [ProductID] = @ID
    END
    SELECT @ID
END

GO--

ALTER TABLE [Order].OrderItems ADD
	IsCustomPrice bit NULL,
	BasePrice float(53) NULL,
	DiscountPercent float(53) NULL,
	DiscountAmount float(53) NULL
GO--

ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(255),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
	 @BarCode nvarchar(50),
	 @SupplyPrice float,  
	 @Weight float,  
	 @IsCouponApplied bit,  
	 @Color nvarchar(50),  
	 @Size nvarchar(50),  
	 @DecrementedAmount float,  
	 @PhotoID int,  
	 @IgnoreOrderDiscount bit,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @TaxName nvarchar(50),
	 @TaxType int,
	 @TaxRate float(53),
	 @TaxShowInPrice bit,
	 @Width float,
	 @Height float,
	 @Length float,
	 @PaymentMethodType int,
	 @PaymentSubjectType int,
	 @IsGift bit,
	 @BookingServiceId int,
	 @TypeItem nvarchar(50),
	 @IsMarkingRequired bit,
	 @IsCustomPrice bit,
	 @BasePrice float,
	 @DiscountPercent float,
	 @DiscountAmount float
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
	   ,[BarCode]  
	   ,[SupplyPrice]  
	   ,[Weight]  
	   ,[IsCouponApplied]  
	   ,[Color]  
	   ,[Size]  
	   ,[DecrementedAmount]  
	   ,[PhotoID]  
	   ,[IgnoreOrderDiscount]  
	   ,[AccrueBonuses] 
	   ,TaxId
	   ,TaxName
	   ,TaxType
	   ,TaxRate
	   ,TaxShowInPrice
	   ,Width
	   ,Height
	   ,[Length]
	   ,PaymentMethodType
	   ,PaymentSubjectType
	   ,IsGift
	   ,BookingServiceId
	   ,TypeItem
	   ,IsMarkingRequired
	   ,IsCustomPrice
	   ,BasePrice
	   ,DiscountPercent
	   ,DiscountAmount
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
	   ,@BarCode
	   ,@SupplyPrice  
	   ,@Weight  
	   ,@IsCouponApplied  
	   ,@Color  
	   ,@Size  
	   ,@DecrementedAmount  
	   ,@PhotoID  
	   ,@IgnoreOrderDiscount  
	   ,@AccrueBonuses
	   ,@TaxId
	   ,@TaxName
	   ,@TaxType
	   ,@TaxRate
	   ,@TaxShowInPrice   
	   ,@Width
	   ,@Height
	   ,@Length
	   ,@PaymentMethodType
	   ,@PaymentSubjectType
	   ,@IsGift
	   ,@BookingServiceId
	   ,@TypeItem
	   ,@IsMarkingRequired
	   ,@IsCustomPrice
	   ,@BasePrice
	   ,@DiscountPercent
	   ,@DiscountAmount
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--

ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(255),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
	@BarCode nvarchar(50),
	@SupplyPrice float,  
	@Weight float,  
	@IsCouponApplied bit,  
	@Color nvarchar(50),  
	@Size nvarchar(50),  
	@DecrementedAmount float,  
	@PhotoID int,  
	@IgnoreOrderDiscount bit,  
	@AccrueBonuses bit,
	@TaxId int,
	@TaxName nvarchar(50),
	@TaxType int,
	@TaxRate float(53),
	@TaxShowInPrice bit,
	@Width float,
	@Height float,
	@Length float,
	@PaymentMethodType int,
	@PaymentSubjectType int,
	@IsGift bit,
	@IsCustomPrice bit,
	@BasePrice float,
	@DiscountPercent float,
	@DiscountAmount float
AS  
BEGIN  
 Update [Order].[OrderItems]  
 Set  
     [Name] = @Name  
	,[Price] = @Price  
	,[Amount] = @Amount  
	,[ArtNo] = @ArtNo  
	,[SupplyPrice] = @SupplyPrice  
	,[Weight] = @Weight  
	,[IsCouponApplied] = @IsCouponApplied  
	,[Color] = Color  
    ,[Size] = Size  
    ,[DecrementedAmount] = DecrementedAmount  
    ,[PhotoID] = @PhotoID  
    ,[IgnoreOrderDiscount] = @IgnoreOrderDiscount  
    ,[AccrueBonuses] = @AccrueBonuses
	,TaxId = @TaxId
	,TaxName = @TaxName
	,TaxType = @TaxType
	,TaxRate = @TaxRate
	,TaxShowInPrice = @TaxShowInPrice
	,Width = @Width
	,Height = @Height
	,[Length] = @Length
	,PaymentMethodType = @PaymentMethodType
	,PaymentSubjectType = @PaymentSubjectType
	,IsGift = @IsGift
	,[BarCode] = @BarCode
	,IsCustomPrice = @IsCustomPrice
	,BasePrice = @BasePrice
	,DiscountPercent = @DiscountPercent
	,DiscountAmount = @DiscountAmount
 Where OrderItemID = @OrderItemID  
END 

GO--

ALTER TABLE Catalog.Product ADD
	DoNotApplyOtherDiscounts bit NULL
GO--


ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled tinyint,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,     
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),   
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max),
	@DoNotApplyOtherDiscounts bit
AS
BEGIN
    DECLARE @Id int,
			@ArtNoUpdateRequired bit

	IF @ArtNo=''
	BEGIN
		SET @ArtNo = CONVERT(nvarchar(100), NEWID())
		SET @ArtNoUpdateRequired = 1
	END

    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
        ,[BriefDescription]
        ,[Description]
        ,[Enabled]
        ,[DateAdded]
        ,[DateModified]
        ,[Recomended]
        ,[New]
        ,[BestSeller]
        ,[OnSale]
        ,[BrandID]
        ,[AllowPreOrder]
        ,[UrlPath]
        ,[Unit]
        ,[ShippingPrice]
        ,[MinAmount]
        ,[MaxAmount]
        ,[Multiplicity]
        ,[HasMultiOffer]
        ,CurrencyID
        ,ActiveView360
        ,ModifiedBy
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,CreatedBy
        ,Hidden
        ,ManualRatio
		,IsMarkingRequired
		,Comment
		,DoNotApplyOtherDiscounts
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
        ,@BriefDescription
        ,@Description
        ,@Enabled
        ,@DateModified
        ,@DateModified
        ,@Recomended
        ,@New
        ,@BestSeller
        ,@OnSale
        ,@BrandID
        ,@AllowPreOrder
        ,@UrlPath
        ,@Unit
        ,@ShippingPrice
        ,@MinAmount
        ,@MaxAmount
        ,@Multiplicity
        ,@HasMultiOffer
        ,@CurrencyID
        ,@ActiveView360
        ,@ModifiedBy
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
		,@IsMarkingRequired
        ,@Comment
		,@DoNotApplyOtherDiscounts
		);

    SET @ID = SCOPE_IDENTITY();
    IF @ArtNoUpdateRequired = 1
    BEGIN
		DECLARE @NewArtNo nvarchar(100) = CONVERT(nvarchar(100),@ID)

        IF EXISTS (SELECT * FROM [Catalog].[Product] WHERE [ArtNo] = @NewArtNo)
        BEGIN
            SET @NewArtNo = @NewArtNo + '_' + SUBSTRING(@ArtNo, 1, 5)
        END

        UPDATE [Catalog].[Product] SET [ArtNo] = @NewArtNo WHERE [ProductID] = @ID
    END
    SELECT @ID
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
    @ProductID int,
    @ArtNo nvarchar(100),
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled bit,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice money,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),
    @AccrueBonuses bit,
    @TaxId int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max),
	@DoNotApplyOtherDiscounts bit
AS
BEGIN
    UPDATE [Catalog].[Product]
    SET 
         [ArtNo] = @ArtNo
        ,[Name] = @Name
        ,[Ratio] = @Ratio
        ,[Discount] = @Discount
        ,[DiscountAmount] = @DiscountAmount
        ,[BriefDescription] = @BriefDescription
        ,[Description] = @Description
        ,[Enabled] = @Enabled
        ,[Recomended] = @Recomended
        ,[New] = @New
        ,[BestSeller] = @BestSeller
        ,[OnSale] = @OnSale
        ,[DateModified] = @DateModified
        ,[BrandID] = @BrandID
        ,[AllowPreOrder] = @AllowPreOrder
        ,[UrlPath] = @UrlPath
        ,[Unit] = @Unit
        ,[ShippingPrice] = @ShippingPrice
        ,[MinAmount] = @MinAmount
        ,[MaxAmount] = @MaxAmount
        ,[Multiplicity] = @Multiplicity
        ,[HasMultiOffer] = @HasMultiOffer
        ,[CurrencyID] = @CurrencyID
        ,[ActiveView360] = @ActiveView360
        ,[ModifiedBy] = @ModifiedBy
        ,[AccrueBonuses] = @AccrueBonuses
        ,[TaxId] = @TaxId
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
		,[IsMarkingRequired] = @IsMarkingRequired
		,[Comment] = @Comment
		,[DoNotApplyOtherDiscounts] = @DoNotApplyOtherDiscounts
    WHERE ProductID = @ProductID
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.Catalog.Product.DoNotApplyOtherDiscounts', N'Не применять другие скидки, купоны и бонусы'),
           (2,'Core.Catalog.Product.DoNotApplyOtherDiscounts', 'Do not apply other discounts, coupons and bonuses'),
           (1,'Admin.Product.Edit.DoNotApplyOtherDiscounts', N'Не применять другие скидки, купоны и бонусы'),
           (2,'Admin.Product.Edit.DoNotApplyOtherDiscounts', 'Do not apply other discounts, coupons and bonuses'),
           (1,'Admin.Product.Edit.DoNotApplyOtherDiscountsHint', N'Будет учитываться только скидка товара. Другие скидки, купоны, бонусы, скидки модулей и т.д. применяться к данному товару не будут.'),
           (2,'Admin.Product.Edit.DoNotApplyOtherDiscountsHint', 'Only the product discount will be taken into account. Other discounts, coupons, bonuses, etc. will not be applied to this product.'),
           (1,'Core.ExportImport.ProductFields.DoNotApplyOtherDiscounts', N'Не применять другие скидки, купоны и бонусы'),
           (2,'Core.ExportImport.ProductFields.DoNotApplyOtherDiscounts', 'Do not apply other discounts, coupons and bonuses')

GO--

ALTER TABLE [Order].OrderItems ADD
	DoNotApplyOtherDiscounts bit NULL
GO--

ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(255),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
	 @BarCode nvarchar(50),
	 @SupplyPrice float,  
	 @Weight float,  
	 @IsCouponApplied bit,  
	 @Color nvarchar(50),  
	 @Size nvarchar(50),  
	 @DecrementedAmount float,  
	 @PhotoID int,  
	 @IgnoreOrderDiscount bit,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @TaxName nvarchar(50),
	 @TaxType int,
	 @TaxRate float(53),
	 @TaxShowInPrice bit,
	 @Width float,
	 @Height float,
	 @Length float,
	 @PaymentMethodType int,
	 @PaymentSubjectType int,
	 @IsGift bit,
	 @BookingServiceId int,
	 @TypeItem nvarchar(50),
	 @IsMarkingRequired bit,
	 @IsCustomPrice bit,
	 @BasePrice float,
	 @DiscountPercent float,
	 @DiscountAmount float,
     @DoNotApplyOtherDiscounts bit
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
	   ,[BarCode]  
	   ,[SupplyPrice]  
	   ,[Weight]  
	   ,[IsCouponApplied]  
	   ,[Color]  
	   ,[Size]  
	   ,[DecrementedAmount]  
	   ,[PhotoID]  
	   ,[IgnoreOrderDiscount]  
	   ,[AccrueBonuses] 
	   ,TaxId
	   ,TaxName
	   ,TaxType
	   ,TaxRate
	   ,TaxShowInPrice
	   ,Width
	   ,Height
	   ,[Length]
	   ,PaymentMethodType
	   ,PaymentSubjectType
	   ,IsGift
	   ,BookingServiceId
	   ,TypeItem
	   ,IsMarkingRequired
	   ,IsCustomPrice
	   ,BasePrice
	   ,DiscountPercent
	   ,DiscountAmount
       ,DoNotApplyOtherDiscounts
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
	   ,@BarCode
	   ,@SupplyPrice  
	   ,@Weight  
	   ,@IsCouponApplied  
	   ,@Color  
	   ,@Size  
	   ,@DecrementedAmount  
	   ,@PhotoID  
	   ,@IgnoreOrderDiscount  
	   ,@AccrueBonuses
	   ,@TaxId
	   ,@TaxName
	   ,@TaxType
	   ,@TaxRate
	   ,@TaxShowInPrice   
	   ,@Width
	   ,@Height
	   ,@Length
	   ,@PaymentMethodType
	   ,@PaymentSubjectType
	   ,@IsGift
	   ,@BookingServiceId
	   ,@TypeItem
	   ,@IsMarkingRequired
	   ,@IsCustomPrice
	   ,@BasePrice
	   ,@DiscountPercent
	   ,@DiscountAmount
       ,@DoNotApplyOtherDiscounts
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--


ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(255),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
	@BarCode nvarchar(50),
	@SupplyPrice float,  
	@Weight float,  
	@IsCouponApplied bit,  
	@Color nvarchar(50),  
	@Size nvarchar(50),  
	@DecrementedAmount float,  
	@PhotoID int,  
	@IgnoreOrderDiscount bit,  
	@AccrueBonuses bit,
	@TaxId int,
	@TaxName nvarchar(50),
	@TaxType int,
	@TaxRate float(53),
	@TaxShowInPrice bit,
	@Width float,
	@Height float,
	@Length float,
	@PaymentMethodType int,
	@PaymentSubjectType int,
	@IsGift bit,
	@IsCustomPrice bit,
	@BasePrice float,
	@DiscountPercent float,
	@DiscountAmount float,
    @DoNotApplyOtherDiscounts bit
AS  
BEGIN  
 Update [Order].[OrderItems]  
 Set  
     [Name] = @Name  
	,[Price] = @Price  
	,[Amount] = @Amount  
	,[ArtNo] = @ArtNo  
	,[SupplyPrice] = @SupplyPrice  
	,[Weight] = @Weight  
	,[IsCouponApplied] = @IsCouponApplied  
	,[Color] = Color  
    ,[Size] = Size  
    ,[DecrementedAmount] = DecrementedAmount  
    ,[PhotoID] = @PhotoID  
    ,[IgnoreOrderDiscount] = @IgnoreOrderDiscount  
    ,[AccrueBonuses] = @AccrueBonuses
	,TaxId = @TaxId
	,TaxName = @TaxName
	,TaxType = @TaxType
	,TaxRate = @TaxRate
	,TaxShowInPrice = @TaxShowInPrice
	,Width = @Width
	,Height = @Height
	,[Length] = @Length
	,PaymentMethodType = @PaymentMethodType
	,PaymentSubjectType = @PaymentSubjectType
	,IsGift = @IsGift
	,[BarCode] = @BarCode
	,IsCustomPrice = @IsCustomPrice
	,BasePrice = @BasePrice
	,DiscountPercent = @DiscountPercent
	,DiscountAmount = @DiscountAmount
    ,DoNotApplyOtherDiscounts = @DoNotApplyOtherDiscounts
 Where OrderItemID = @OrderItemID  
END 

GO--

UPDATE [Settings].[ExportFeedSettings]
SET [AdvancedSettings] = '{"CsvEnconing":"UTF-8","CsvSeparator":"SemicolonSeparated","CsvSeparatorCustom":null,"CsvColumSeparator":";","CsvPropertySeparator":":","CsvExportNoInCategory":false,"CsvCategorySort":false,"FieldMapping":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,51,52,53,54,55,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77],"ModuleFieldMapping":[],"FileName":null,"FileExtention":null,"PriceMargin":0.0,"AdditionalUrlTags":null,"Active":false,"IntervalType":0,"Interval":0,"JobStartTime":"0001-01-01T00:00:00","AdvancedSettings":null,"ExportAllProducts":true}'
WHERE ExportFeedId = '2' AND AdvancedSettings = '{"CsvEnconing":"UTF-8","CsvSeparator":"SemicolonSeparated","CsvSeparatorCustom":null,"CsvColumSeparator":";","CsvPropertySeparator":":","CsvExportNoInCategory":false,"CsvCategorySort":false,"FieldMapping":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,51,52,53,54,56,57,58,59,60,61,62,63,64,65,66,67],"ModuleFieldMapping":[],"FileName":null,"FileExtention":null,"PriceMargin":0.0,"AdditionalUrlTags":null,"Active":false,"IntervalType":0,"Interval":0,"JobStartTime":"0001-01-01T00:00:00","AdvancedSettings":null,"ExportAllProducts":true}'

GO--

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CustomerType') AND object_id = OBJECT_ID(N'[Customers].[CustomerField]'))
    BEGIN
        ALTER TABLE [Customers].[CustomerField]
			ADD CustomerType int NOT NULL DEFAULT 0
    END

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerFieldCustomerType.Physical','Физические лица')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerFieldCustomerType.Physical','Physical entity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerFieldCustomerType.Legal','Юридические лица')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerFieldCustomerType.Legal','Legal entity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerFieldCustomerType.All','Физические и юридические лица')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerFieldCustomerType.All','Physical and legal entities')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCustomerField.CustomerType','Тип покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCustomerField.CustomerType','Customer type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCustomers.CustomerType','Тип покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCustomers.CustomerType','Customer type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.TypesOfCustomers.Physical','Физические лица')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.TypesOfCustomers.Physical','Physical entity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.TypesOfCustomers.Legal','Юридические лица')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.TypesOfCustomers.Legal','Legal entity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.TypesOfCustomers.Title','Тип покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.TypesOfCustomers.Title','Customer type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.CustomerFields.CustomerType','Тип покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.CustomerFields.CustomerType','Customer type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Customers.RightBlock.CustomerType','Тип покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Customers.RightBlock.CustomerType','Customer type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MyAccount.CommonInfo.CustomerTypeEntity','Тип покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MyAccount.CommonInfo.CustomerTypeEntity','Customer type')

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CustomerType') AND object_id = OBJECT_ID(N'[Customers].[Customer]'))
    BEGIN
        ALTER TABLE [Customers].[Customer]
			ADD CustomerType int NOT NULL DEFAULT 0
    END
	
IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CustomerType') AND object_id = OBJECT_ID(N'[Order].[PaymentMethod]'))
    BEGIN
        ALTER TABLE [Order].[PaymentMethod]
			ADD CustomerType int NOT NULL DEFAULT 2
    END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CustomerType') AND object_id = OBJECT_ID(N'[Order].[PaymentMethod]'))
    BEGIN
        ALTER TABLE [Order].[PaymentMethod]
			ADD CustomerType int NOT NULL DEFAULT 2
    END

GO--

ALTER PROCEDURE [Customers].[sp_AddCustomer]
    @CustomerID uniqueidentifier,
    @CustomerGroupID int,
    @Password nvarchar(100),
    @FirstName nvarchar(70),
    @LastName nvarchar(70),
    @Phone nvarchar(max),
    @StandardPhone bigint,
    @RegistrationDateTime datetime,
    @Email nvarchar(100),
    @CustomerRole int,
    @Patronymic nvarchar(70),
    @BonusCardNumber bigint,
    @AdminComment nvarchar(MAX),
    @ManagerId int,
    @Rating int,
    @Enabled bit,
    @HeadCustomerId uniqueidentifier,
    @BirthDay datetime,
    @City nvarchar(70),
    @Organization nvarchar(250),
    @ClientStatus int,
    @RegisteredFrom nvarchar(500),
	@CustomerType int
AS
BEGIN
    IF @CustomerID IS NULL
        SET @CustomerID = NEWID()

    INSERT INTO [Customers].[Customer]
        ([CustomerID]
        ,[CustomerGroupID]
        ,[Password]
        ,[FirstName]
        ,[LastName]
        ,[Phone]
        ,[StandardPhone]
        ,[RegistrationDateTime]
        ,[Email]
        ,[CustomerRole]
        ,[Patronymic]
        ,[BonusCardNumber]
        ,[AdminComment]
        ,[ManagerId]
        ,[Rating]
        ,[Enabled]
        ,[HeadCustomerId]
        ,[BirthDay]
        ,[City]
        ,[Organization]
        ,[ClientStatus]
        ,[RegisteredFrom]
		,[CustomerType])
    VALUES
        (@CustomerID
        ,@CustomerGroupID
        ,@Password
        ,@FirstName
        ,@LastName
        ,@Phone
        ,@StandardPhone
        ,@RegistrationDateTime
        ,@Email
        ,@CustomerRole
        ,@Patronymic
        ,@BonusCardNumber
        ,@AdminComment
        ,@ManagerId
        ,@Rating
        ,@Enabled
        ,@HeadCustomerId
        ,@BirthDay
        ,@City
        ,@Organization
        ,@ClientStatus
        ,@RegisteredFrom
		,@CustomerType);

    SELECT CustomerID, InnerId From [Customers].[Customer] WHERE CustomerId = @CustomerID
END

GO--

ALTER PROCEDURE [Customers].[sp_UpdateCustomerInfo]     
 @customerid uniqueidentifier,     
 @firstname nvarchar (70),     
 @lastname nvarchar(70),     
 @patronymic nvarchar(70),     
 @phone nvarchar(max),     
 @standardphone bigint,     
 @email nvarchar(100) ,    
 @customergroupid INT = NULL,     
 @customerrole INT,     
 @bonuscardnumber bigint,     
 @admincomment nvarchar (max),     
 @managerid INT,     
 @rating INT,    
 @avatar nvarchar(100),    
 @Enabled bit,    
 @HeadCustomerId uniqueidentifier,    
 @BirthDay datetime,    
 @City nvarchar(70),  
 @SortOrder int,  
 @Organization nvarchar(250),
 @ClientStatus int,
 @CustomerType int
AS    
BEGIN    
 UPDATE [customers].[customer]    
 SET [firstname] = @firstname,    
  [lastname] = @lastname,    
  [patronymic] = @patronymic,    
  [phone] = @phone,    
  [standardphone] = @standardphone,    
  [email] = @email,    
  [customergroupid] = @customergroupid,    
  [customerrole] = @customerrole,    
  [bonuscardnumber] = @bonuscardnumber,    
  [admincomment] = @admincomment,    
  [managerid] = @managerid,    
  [rating] = @rating,    
  [avatar] = @avatar,    
  [Enabled] = @Enabled,    
  [HeadCustomerId] = @HeadCustomerId,    
  [BirthDay] = @BirthDay,    
  [City] = @City,  
  [SortOrder] = @SortOrder,  
  [Organization] = @Organization,
  [ClientStatus] = @ClientStatus,
  [CustomerType] = @CustomerType
 WHERE customerid = @customerid    
END 

GO--

IF NOT EXISTS (SELECT Value FROM [Settings].[Settings] WHERE Name = 'DefaultBrandH1')
	INSERT INTO [Settings].[Settings] (Name, Value) VALUES ('DefaultBrandH1', '#STORE_NAME# - Производители')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ExportImport.EProductField.DoNotApplyOtherDiscounts', N'Не применять другие скидки, купоны и бонусы'),
           (2,'Core.ExportImport.EProductField.DoNotApplyOtherDiscounts', 'Do not apply other discounts, coupons and bonuses')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerFieldType.Tel','Телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerFieldType.Tel','Phone')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerFieldType.Checkbox','Флажок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerFieldType.Checkbox','Checkbox')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.CustomerFieldType.Email','Почта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.CustomerFieldType.Email','Email')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Bill.CustomerCompanyName','Дополнительное поле покупателя, содержащее название организации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Bill.CustomerCompanyName','Additional field of the buyer containing the name of the organization')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Bill.CustomerINN','Дополнительное поле покупателя, содержащее ИНН')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Bill.CustomerINN','Additional buyer field containing INN')
	
IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CustomerType') AND object_id = OBJECT_ID(N'[Order].[OrderCustomer]'))
    BEGIN
        ALTER TABLE [Order].[OrderCustomer]
			ADD CustomerType int NULL
    END
		
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderCustomer.CustomerType','Тип покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderCustomer.CustomerType','Customer type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Payments.Bill.GetCustomerDataMethod.InPayment','Запрашивать ИНН и название организации в методе оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Payments.Bill.GetCustomerDataMethod.InPayment','Request the TIN and the name of the organization in the payment method')
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Payments.Bill.GetCustomerDataMethod.FromAdditionalFields','Запрашивать расширенные данные о компании из доп. полей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Payments.Bill.GetCustomerDataMethod.FromAdditionalFields','Request extended company data from additional fields')
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Bill.GetCustomerDataMethod','Алгоритм работы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Bill.GetCustomerDataMethod','Algorithm of operation')

CREATE TABLE [Order].[OrderCoupon](
	[OrderId] [int] NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Type] [int] NOT NULL,
	[Value] [float] NOT NULL,
	[CurrencyIso3] [nvarchar](3) NOT NULL,
	[MinimalOrderPrice] [money] NOT NULL,
	[IsMinimalOrderPriceFromAllCart] [bit] NOT NULL,
 CONSTRAINT [PK_OrderCoupon] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Order].[OrderCoupon]  WITH CHECK ADD  CONSTRAINT [FK_OrderCoupon_Order] FOREIGN KEY([OrderId])
REFERENCES [Order].[Order] ([OrderID])
ON DELETE CASCADE
GO--


if not exists (Select 1 From [Order].[OrderCoupon])
begin
    Insert Into [Order].[OrderCoupon] ([OrderId],[Code],[Type],[Value],[CurrencyIso3],[MinimalOrderPrice],[IsMinimalOrderPriceFromAllCart]) 
        Select OrderId, [CouponCode], [CouponType], [CouponValue], '', 0, 0
        FROM [Order].[Order]
        Where [CouponCode] is not null and [CouponCode] <> ''
end

GO--

ALTER TABLE [Order].[Order]
	DROP COLUMN CouponCode, CouponType, CouponValue
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ExportImport.LeadFields.District', N'Район региона'),
           (2,'Core.ExportImport.LeadFields.District', 'District'),
           (1,'Core.ExportImport.LeadFields.Comment', N'Комментарий покупателя'),
           (2,'Core.ExportImport.LeadFields.Comment', 'Comment'),
           (1,'Core.ExportImport.LeadFields.CreatedDate', N'Дата создания'),
           (2,'Core.ExportImport.LeadFields.CreatedDate', 'Created date')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ExportImport.LeadFields.LeadId', N'Id лида'),
           (2,'Core.ExportImport.LeadFields.LeadId', 'Lead id')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Settings.APIAuth', N'API с авторизацией покупателя'),
           (2,'Admin.Settings.APIAuth', 'API with customer authentication')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.MailSettings.PublicMailDomainsAreNotSupported', 'Публичные почтовые домены не поддерживаются. Необходимо завести почтовый ящик на своем домене. Подробнее в '),
           (2,'Admin.Js.MailSettings.PublicMailDomainsAreNotSupported', 'Public mail domains are not supported. You need to create a new mailbox on your domain. Learn more in ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.MailSettings.Instruction', 'инструкции'),
           (2,'Admin.Js.MailSettings.Instruction', 'instruction')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.Order.ChangeCoupon', N'Будут пересчитаны цены у товаров, к которым купон может быть применен. Вы уверены, что хотите применить купон?'),
           (2,'Admin.Js.Order.ChangeCoupon', 'The prices of the goods to which the coupon can be applied will be recalculated. Are you sure you want to apply the coupon?')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Product.Edit.DiscountLinks', N'Подробнее: <a href="https://www.advantshop.net/help/pages/discount" target="_blank">Скидка</a>,  <a href="https://www.advantshop.net/help/pages/discount-mechanism" target="_blank">Механизм работы скидок</a>'),
           (2,'Admin.Product.Edit.DiscountLinks', 'More: <a href="https://www.advantshop.net/help/pages/discount" target="_blank">Discount</a>,  <a href="https://www.advantshop.net/help/pages/discount-mechanism" target="_blank">How discounts work</a>')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.Order.PriceWhenOrdering', N'Цена на момент заказа'),
           (2,'Admin.Js.Order.PriceWhenOrdering', 'Price when ordering'),
           (1,'Admin.Js.Order.IsCustomPrice', N'Цена изменялась'),
           (2,'Admin.Js.Order.IsCustomPrice', 'Price changed')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.Leads.Description', 'Описание'),
           (2,'Admin.Js.Leads.Description', 'Description')

GO--

ALTER PROCEDURE [Order].[sp_DecrementProductsCountAccordingOrder]   
 @orderId int  
AS
BEGIN
UPDATE offer
SET offer.Amount = Round(offer.Amount - orderItem.Amount + orderItem.DecrementedAmount, 4)
    FROM Catalog.Offer offer  
        JOIN (SELECT orderItems.ArtNo, SUM(orderItems.Amount) as Amount, SUM(orderItems.DecrementedAmount) as DecrementedAmount
          FROM [Order].[OrderItems] orderItems
          WHERE orderItems.OrderID = @orderId AND orderItems.TypeItem = 'Product'
          GROUP BY orderItems.ArtNo) orderItem
        ON offer.Artno = orderItem.ArtNo;


UPDATE [Order].[OrderItems]
SET decrementedAmount = amount
WHERE [OrderID] = @orderId AND TypeItem = 'Product'

UPDATE [Order].[Order]
SET [Decremented] = 1
WHERE [OrderID] = @orderId

END

GO--

ALTER PROCEDURE [Catalog].[sp_AddOption]
		@CustomOptionsId int,
		@Title nvarchar(255),
		@PriceBC money,
		@PriceType int,
		@SortOrder int
AS
BEGIN
INSERT INTO [Catalog].[Options]
           ([CustomOptionsId]
           ,[Title]
           ,[PriceBC]
           ,[PriceType]
           ,[SortOrder])
     VALUES
           (@CustomOptionsId,
			@Title,
			@PriceBC,
			@PriceType,
			@SortOrder)
	SELECT SCOPE_IDENTITY();
END

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 33)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (33, 'Boxberry','Казахстан','KZ','Алматинская область','Алматы','','','','Алма-Ата','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 34)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (34, 'Boxberry','Казахстан','KZ','','Нур-Султан','','','Акмолинская область','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 35)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (35, 'Boxberry','Казахстан','KZ','Актюбинская область','Актюбинск','','','','Актобе','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 36)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (36, 'Boxberry','Беларусь','BY','Могилёвская область','Могилёв','','','Могилевская область','Могилев','',0,1,10,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 37)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (37, 'Boxberry','Беларусь','BY','Могилёвская область','','','','Могилевская область','','',0,1,0,'','','')

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

declare @KyrgyzstanId int = (SELECT TOP 1 [CountryID] FROM [Customers].[Country] WHERE [CountryISO3] = 'KGZ')

IF (@KyrgyzstanId IS NOT NULL)
BEGIN

	declare @ChuyId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Чуйская область' AND [CountryID] = @KyrgyzstanId)
	IF (@ChuyId IS NULL)
	BEGIN
		INSERT INTO [Customers].[Region] ([CountryID],[RegionName],[RegionCode],[RegionSort])
		VALUES (@KyrgyzstanId,'Чуйская область','',0)
		
		SET @ChuyId = (SELECT SCOPE_IDENTITY())
	END

	declare @KyrgyzstanRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Киргизия' AND [CountryID] = @KyrgyzstanId)
	declare @BishkekId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Бишкек' AND [RegionID] = @KyrgyzstanRegionId)

	IF (@BishkekId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @ChuyId
		WHERE [CityID] = @BishkekId
	END
	
	declare @KantId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Кант' AND [RegionID] = @KyrgyzstanRegionId)

	IF (@KantId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @ChuyId
		WHERE [CityID] = @KantId
	END
	
	declare @OshRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Ошская обл.' AND [CountryID] = @KyrgyzstanId)

	IF (@OshRegionId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[Region]
		   SET [RegionName] = 'Ошская область'
		WHERE [RegionID] = @OshRegionId
	END

	declare @OshId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Ош' AND [RegionID] = @KyrgyzstanRegionId)
	DELETE FROM [Customers].[City] WHERE [CityID]=@OshId
	
	SET @OshId = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Ош' AND [RegionID] = @OshRegionId)
	IF (@OshId IS NULL and @OshRegionId IS not NULL)
	BEGIN
		INSERT INTO [Customers].[City] ([RegionID],[CityName],[CitySort],[DisplayInPopup],[PhoneNumber],[MobilePhoneNumber],[Zip],[District])
		VALUES (@OshRegionId, 'Ош', 0, 0, '', '', '', '')
	END

	declare @IssykKulId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Иссык-Кульская обл.' AND [CountryID] = @KyrgyzstanId)

	IF (@IssykKulId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[Region]
		   SET [RegionName] = 'Иссык-Кульская область'
		WHERE [RegionID] = @IssykKulId
	END

	declare @KarakolId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Каракол' AND [RegionID] = @KyrgyzstanRegionId)

	IF (@KarakolId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @IssykKulId
		WHERE [CityID] = @KarakolId
	END

	declare @JalalAbadRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Джалал-Абадская область' AND [CountryID] = @KyrgyzstanId)
	IF (@JalalAbadRegionId IS NULL)
	BEGIN
		INSERT INTO [Customers].[Region] ([CountryID],[RegionName],[RegionCode],[RegionSort])
		VALUES (@KyrgyzstanId,'Джалал-Абадская область','',0)
		
		SET @JalalAbadRegionId = (SELECT SCOPE_IDENTITY())
	END

	declare @JalalAbadId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Джалал-Абад' AND [RegionID] = @OshRegionId)
	IF (@JalalAbadId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @JalalAbadRegionId
			  ,[DisplayInPopup] = 1
		WHERE [CityID] = @JalalAbadId
	END

	
	declare @NarynRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Нарынская обл.' AND [CountryID] = @KyrgyzstanId)

	IF (@NarynRegionId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[Region]
		   SET [RegionName] = 'Нарынская область'
		WHERE [RegionID] = @NarynRegionId
	END

	declare @NarynId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Нарын' AND [RegionID] = @KyrgyzstanRegionId)
	DELETE FROM [Customers].[City] WHERE [CityID]=@NarynId
	
	SET @NarynId = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Нарын' AND [RegionID] = @NarynRegionId)
	IF (@NarynId IS NULL and @NarynRegionId is not NULL)
	BEGIN
		INSERT INTO [Customers].[City] ([RegionID],[CityName],[CitySort],[DisplayInPopup],[PhoneNumber],[MobilePhoneNumber],[Zip],[District])
		VALUES (@NarynRegionId, 'Нарын', 0, 1, '', '', '', '')
	END

	declare @BelovodskoyeId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Беловодское' AND [RegionID] = @KyrgyzstanRegionId)

	IF (@BelovodskoyeId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @ChuyId
		WHERE [CityID] = @BelovodskoyeId
	END

	declare @KaraBaltaId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Кара-Балта' AND [RegionID] = @KyrgyzstanRegionId)

	IF (@KaraBaltaId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @ChuyId
		WHERE [CityID] = @KaraBaltaId
	END

	declare @SokulukId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Сокулук' AND [RegionID] = @KyrgyzstanRegionId)

	IF (@SokulukId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @ChuyId
		WHERE [CityID] = @SokulukId
	END

END

GO--

UPDATE [Order].[ShippingReplaceGeo]
   SET [Enabled] = 0
WHERE [Id] = 32

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.Customers.CustomerLegalEntityField.CompanyName', 'Название организации'),
           (2,'Core.Customers.CustomerLegalEntityField.CompanyName', 'Company name'),
           (1,'Core.Customers.CustomerLegalEntityField.LegalAddress', 'Юридический адрес'),
           (2,'Core.Customers.CustomerLegalEntityField.LegalAddress', 'Legal address'),
           (1,'Core.Customers.CustomerLegalEntityField.INN', 'ИНН'),
           (2,'Core.Customers.CustomerLegalEntityField.INN', 'INN'),
           (1,'Core.Customers.CustomerLegalEntityField.KPP', 'КПП'),
           (2,'Core.Customers.CustomerLegalEntityField.KPP', 'KPP'),
           (1,'Core.Customers.CustomerLegalEntityField.OGRN', 'ОГРН'),
           (2,'Core.Customers.CustomerLegalEntityField.OGRN', 'OGRN'),
           (1,'Core.Customers.CustomerLegalEntityField.OKPO', 'ОКПО'),
           (2,'Core.Customers.CustomerLegalEntityField.OKPO', 'OKPO'),
           (1,'Core.Customers.CustomerLegalEntityField.BIK', 'БИК'),
           (2,'Core.Customers.CustomerLegalEntityField.BIK', 'BIK'),
           (1,'Core.Customers.CustomerLegalEntityField.BankName', 'Название банка'),
           (2,'Core.Customers.CustomerLegalEntityField.BankName', 'Bank name'),
           (1,'Core.Customers.CustomerLegalEntityField.CorrespondentAccount', 'Корреспондентский счёт'),
           (2,'Core.Customers.CustomerLegalEntityField.CorrespondentAccount', 'Correspondent account'),
           (1,'Core.Customers.CustomerLegalEntityField.PaymentAccount', 'Расчётный счёт'),
           (2,'Core.Customers.CustomerLegalEntityField.PaymentAccount', 'Payment account'),
           (1,'Admin.Js.AddEditCustomerField.FieldAssignment', 'Поле содержит данные'),
           (2,'Admin.Js.AddEditCustomerField.FieldAssignment', 'Field contains data'),
           (1,'Core.Customers.CustomerLegalEntityField.None', 'Не выбрано'),
           (2,'Core.Customers.CustomerLegalEntityField.None', 'None'),
           (1,'Admin.Leads.Customer.CustomerType', 'Тип покупателя'),
           (2,'Admin.Leads.Customer.CustomerType', 'Customer type'),
           (1,'Admin.Js.AddLead.CustomerType', 'Тип покупателя'),
           (2,'Admin.Js.AddLead.CustomerType', 'Customer type'),
           (1,'Core.Customers.Customer.CustomerType', 'Тип покупателя'),
           (2,'Core.Customers.Customer.CustomerType', 'Customer type'),
           (1,'Admin.Js.AddUpdateBooking.CustomerType', 'Тип покупателя'),
           (2,'Admin.Js.AddUpdateBooking.CustomerType', 'Customer type'),
           (1,'Admin.CustomerSegments.Filters.CustomerType', 'Тип покупателя'),
           (2,'Admin.CustomerSegments.Filters.CustomerType', 'Customer type')

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'FieldAssignment') AND object_id = OBJECT_ID(N'[Customers].[CustomerField]'))
    BEGIN
        ALTER TABLE [Customers].[CustomerField]
			ADD FieldAssignment int NULL
    END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CustomerType') AND object_id = OBJECT_ID(N'[Order].[Lead]'))
    BEGIN
        ALTER TABLE [Order].[Lead]
			ADD CustomerType int NULL
    END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.ExportFeed.FileLink', 'Ссылка на файл: '),
           (2,'Admin.ExportFeed.FileLink', 'Link to the file: '),
		   (1,'Admin.ExportFeed.DateCreated', 'Дата создания: '),
           (2,'Admin.ExportFeed.DateCreated', 'Date created: ')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.ExportFeed.SettingsGoogle.NoteTitle', 'Категория товаров google по умолчанию'),
           (2,'Admin.ExportFeed.SettingsGoogle.NoteTitle', 'Default google product category'),
           (1,'Admin.ExportFeed.SettingsGoogle.NoteText', 'Заполнение данного атрибута помогает Google Merchant Center лучше классифицировать товар и поместить в нужную категорию из иерархии Google Merchant Center.<br/><br/>Подробнее: <br/> <a href="https://support.google.com/merchants/answer/6324436?hl=ru" target="_blank" >Категория товара в google.</a>'),
           (2,'Admin.ExportFeed.SettingsGoogle.NoteText', 'Filling in this attribute helps Google Merchant Center to better classify the product and place it in the right category from the Google Merchant Center hierarchy.<br/><br/>Read more: <br/> <a href="https://support.google.com/merchants/answer/6324436" target="_blank" >Product category on google.</a>')
GO--
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Выбираете действие, которое произойдет, если клиент воспользуется формой обратной связи на сайте.<br/><br/>  Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/otpravit-soobschenie" target="_blank">Форма обратной связи.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Choose the action that will happen if the client uses the feedback form on the site.<br/><br/> More details: <br/><a href ="https://www.advantshop.net/help/pages/otpravit-soobschenie" target="_blank">Feedback form.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Индекс поиска необходим, чтоб собрать информацию о товарах и обеспечить быстрый и точный поиск информации<br/><br/>Подробнее:<br/><a href ="https://www.advantshop.net/help/pages/search#5" target="_blank">Индекс поиска</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchIndexHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'A search index is needed to collect information about products and provide a quick and accurate search for information.<br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/search # 5 "target ="_blank">Search index</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchIndexHint' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.PaymentMethods.Bill.AccountDetails', 'Данные для счета'),
           (2,'Admin.PaymentMethods.Bill.AccountDetails', 'Account details')

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CMS].[CarouselApi]') AND type in (N'U'))
BEGIN
    CREATE TABLE [CMS].[CarouselApi](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Text] [nvarchar](max) NULL,
        [Enabled] [bit] NOT NULL,
        [SortOrder] [int] NOT NULL,
    CONSTRAINT [PK_CarouselApi] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

ALTER TABLE Catalog.PriceRule ADD
	Enabled bit NULL
GO--

Update [Catalog].[PriceRule] Set Enabled = 1
GO--

ALTER TABLE [Catalog].[PriceRule] ALTER COLUMN [Enabled] bit NOT NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.PriceRules.Name', N'Название'),
           (2,'Admin.Js.PriceRules.Name', 'Name'),
           (1,'Admin.Js.PriceRules.Rule', N'Правило'),
           (2,'Admin.Js.PriceRules.Rule', 'Rule'),
           (1,'Admin.Js.PriceRules.Enabled', N'Активность'),
           (2,'Admin.Js.PriceRules.Enabled', 'Enabled')

GO--

ALTER TABLE CMS.CarouselApi ADD
	Title nvarchar(MAX) NULL
GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 38)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (38, 'Sdek','','','Республика Северная Осетия - Алания','Владикавказ','','','','','',0,0,10,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 39)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (39, 'Sdek','','','Кемеровская область','Новокузнецк','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 40)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (40, 'Sdek','','','Кемеровская область','Кемерово','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 41)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (41, 'Sdek','','','Кемеровская область','Юрга','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 42)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (42, 'Sdek','','','Кемеровская область','Белово','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 43)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (43, 'Sdek','','','Кемеровская область','Берёзовский','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 44)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (44, 'Sdek','','','Кемеровская область','Тайга','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 45)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (45, 'Sdek','','','Кемеровская область','Полысаево','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 46)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (46, 'Sdek','','','Кемеровская область','Таштагол','','','','','',0,0,10,'','','')
    
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 47)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (47, 'Sdek','','','Кемеровская область','Салаир','','','','','',0,0,10,'','','')

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF
    
GO--

UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] >= 39 AND [Id] <= 47

GO--

UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 25
UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 38
UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 24
UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 2
   
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.Crm.ELeadFieldType.CustomerType', N'Тип покупателя'),
           (2,'Core.Crm.ELeadFieldType.CustomerType', 'Customer type'),
           (1,'Core.Crm.EOrderFieldType.CustomerType', N'Тип покупателя'),
           (2,'Core.Crm.EOrderFieldType.CustomerType', 'Customer type')
   
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Тип поля' WHERE [ResourceKey] = 'Admin.Js.AddEditCustomerField.Type' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Field type' WHERE [ResourceKey] = 'Admin.Js.AddEditCustomerField.Type' AND [LanguageId] = 2


UPDATE [Settings].[Localization] SET [ResourceValue] = N'Тип поля' WHERE [ResourceKey] = 'Admin.Js.SettingsCustomers.Type' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Field type' WHERE [ResourceKey] = 'Admin.Js.SettingsCustomers.Type' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.ExportField.CustomerType', N'Тип покупателя'),
           (2,'Admin.ExportField.CustomerType', 'Customer type')
   
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.OrderItemsSummary.CreateDraftOrderMeasoft', 'Создать заказ'),
           (2,'Admin.Js.OrderItemsSummary.CreateDraftOrderMeasoft', 'Create order'),
		   (1,'Admin.Js.OrderItemsSummary.DeleteDraftOrderMeasoft', 'Отменить заказ'),
           (2,'Admin.Js.OrderItemsSummary.DeleteDraftOrderMeasoft', 'Delete order'),
		   (1,'Admin.ShippingMethods.Measoft.Login', 'Логин'),
           (2,'Admin.ShippingMethods.Measoft.Login', 'Login'),
		   (1,'Admin.ShippingMethods.Measoft.Login.HelpText', 'Введите логин от личного кабинета Measoft'),
           (2,'Admin.ShippingMethods.Measoft.Login.HelpText', 'Enter the username from the Measoft personal account'),
		   (1,'Admin.ShippingMethods.Measoft.Password', 'Пароль'),
           (2,'Admin.ShippingMethods.Measoft.Password', 'Password'),
		   (1,'Admin.ShippingMethods.Measoft.Password.HelpText', 'Введите пароль от личного кабинета Measoft'),
           (2,'Admin.ShippingMethods.Measoft.Password.HelpText', 'Enter the password from the Measoft personal acount'),
		   (1,'Admin.ShippingMethods.Measoft.OrderStatuses', 'Статусы заказов'),
           (2,'Admin.ShippingMethods.Measoft.OrderStatuses', 'Order statuses'),
		   (1,'Admin.ShippingMethods.Measoft.SyncStatuses', 'Синхронизировать статусы заказов из Measoft'),
           (2,'Admin.ShippingMethods.Measoft.SyncStatuses', 'Synchronize order statuses from Measoft'),
		   (1,'Admin.ShippingMethods.Measoft.Statuses', 'Статусы'),
           (2,'Admin.ShippingMethods.Measoft.Statuses', 'Statuses')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.ShippingMethods.Measoft.ExtraCode', 'Экстра код'),
           (2,'Admin.ShippingMethods.Measoft.ExtraCode', 'Extra code'),
		   (1,'Admin.ShippingMethods.Measoft.ExtraCode.HelpText', 'Введите экстра код от личного кабинета Measoft'),
           (2,'Admin.ShippingMethods.Measoft.ExtraCode.HelpTextt', 'Enter the extra code from the Measoft personal acount')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Сделать обязательным заполнение ИНН и название организации' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.RequiredPaymentDetails' AND [LanguageId] = 1

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Запрашивать ИНН и название организации у покупателя' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.ShowPaymentDetails' AND [LanguageId] = 1

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Название организации' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.CompanyName' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Organization name' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.CompanyName' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddRecomProperty.Property', 'Свойство')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddRecomProperty.Property', 'Property')
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddRecomProperty.PropertyValue', 'Значение свойства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddRecomProperty.PropertyValue', 'Property value')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.SelectDelivery', 'Выбрать способ доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.SelectDelivery', 'Choose a shipping method')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ExportImport.ImportCsvV2.WrongPriceRuleHeader', 'Некорректный заголовок типа цены в колонке {0}'),
           (2,'Core.ExportImport.ImportCsvV2.WrongPriceRuleHeader', 'Wrong price rules header in column {0}')
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Сегмент' WHERE [ResourceKey] = 'Admin.CustomerSegments.AddEdit.Segment' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Segment' WHERE [ResourceKey] = 'Admin.CustomerSegments.AddEdit.Segment' AND [LanguageId] = 2

UPDATE [Customers].[Country] SET [CountryName] = N'Папский Престол (Государство-город Ватикан)' WHERE [CountryName] = 'Папский Престол (Государство &mdash; город Ватикан)'

GO--

UPDATE [Settings].[SettingsSearch] SET Link = REPLACE(Link, '#?', '?')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.SelectPayment', 'Выбрать способ оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.SelectPayment', 'Choose a payment method')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.SettingsCoupon.ApplyCoupon', 'Применить купон'),
           (2,'Admin.Js.SettingsCoupon.ApplyCoupon', 'Apply coupon')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.Orders.OrderType.MobileApp', 'Мобильное приложение'),
           (2,'Core.Orders.OrderType.MobileApp', 'Mobile application')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
	VALUES
		(1, 'Admin.ShippingMethods.LPost.SecretKey', 'Секрет партнёра'),
		(2, 'Admin.ShippingMethods.LPost.SecretKey', 'Partner secret'),
		(1, 'Admin.ShippingMethods.LPost.SecretKey.HelpText', 'Укажите секретный ключ, полученный при заключении договора с L-post'),
		(2, 'Admin.ShippingMethods.LPost.SecretKey.HelpText', 'Enter the secret key received at the conclusion of the contract with L-post'),
		(1, 'Admin.ShippingMethods.LPost.DeliveryTypes', 'Расчитывать методы доставки'),
		(2, 'Admin.ShippingMethods.LPost.DeliveryTypes', 'Calculate delivery types'),
		(1, 'Admin.ShippingMethods.LPost.DeliveryTypes.HelpText', 'Можно выбрать несколько вариантов зажав клавиши Shift или Ctrl'),
		(2, 'Admin.ShippingMethods.LPost.DeliveryTypes.HelpText', 'You can select several options by holding down the Shift or Ctrl keys')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Settings.Checkout.ShowUserAgreementForNewsletter', 'Показывать пользовательское соглашение для подписки на новости'),
           (2,'Admin.Settings.Checkout.ShowUserAgreementForNewsletter', 'Show the user agreement for subscribing to news'),
           (1,'Admin.Settings.SystemSettings.ShowUserAgreementForNewsletterNote', 'Запрашивать подтверждение согласия с условиями пользовательского соглашения для рассылки новостей. <br><br> Подробнее: <br> <a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">Как соблюсти требования закона 152-ФЗ на платформе AdvantShop</a>'),
           (2,'Admin.Settings.SystemSettings.ShowUserAgreementForNewsletterNote', 'Request confirmation of consent to the terms of the user agreement for the newsletter. <br><br><a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">More</a>'),
           (1,'Admin.Settings.Checkout.UserAgreementForNewsletter', 'Пользовательское соглашение для подписки на новости'),
           (2,'Admin.Settings.Checkout.UserAgreementForNewsletter', 'User agreement for subscription to news')
GO--

If not exists (Select 1 From [Settings].[Settings] Where [Name] = 'UserAgreementForNewsletter')
	Insert Into [Settings].[Settings] (Name, [Value]) Values('UserAgreementForNewsletter', 'Нажимая «Подписаться», я даю согласие на обработку своих персональных данных.')
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Js.ShippingTemplate.SelectInterval.TimeNotSelected', 'Время доставки не выбрано'),
           (2,'Js.ShippingTemplate.SelectInterval.TimeNotSelected', 'The delivery time is not selected'),
           (1,'Js.ShippingTemplate.SelectInterval.DateNotSelected', 'Дата доставки не выбрана'),
           (2,'Js.ShippingTemplate.SelectInterval.DateNotSelected', 'The delivery date is not selected'),
		   (1,'Js.ShippingTemplate.SelectInterval.SelectDate', 'Выберите дату доставки'),
           (2,'Js.ShippingTemplate.SelectInterval.SelectDate', 'Select the delivery date'),
		   (1,'Js.ShippingTemplate.SelectInterval.SelectTime', 'Выберите время доставки'),
           (2,'Js.ShippingTemplate.SelectInterval.SelectTime', 'Select the delivery time')
GO--


ALTER PROCEDURE [Settings].[sp_GetCsvProductPropertyNames] 
	@exportFeedId INT,
	@exportAllProducts BIT,
	@exportNotAvailable BIT,
	@exportNoInCategory BIT
AS
BEGIN
	DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED);

    IF (@exportNoInCategory = 1)
    BEGIN
        INSERT INTO @lproductNoCat
            SELECT [productid] 
            FROM [Catalog].product 
            WHERE [productid] NOT IN (SELECT [productid] FROM [Catalog].[productcategories]);
    END
	
	
	DECLARE @lcategorytemp TABLE (CategoryId INT)
	DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit)
    
	INSERT INTO @l
		SELECT t.categoryid, t.Opened
		FROM [Settings].[exportfeedselectedcategories] AS t 
			INNER JOIN catalog.category ON t.categoryid = category.categoryid
		WHERE [exportfeedid] = @exportFeedId 

	DECLARE @l1 INT = (SELECT Min(categoryid) FROM @l)
	WHILE @l1 IS NOT NULL
	BEGIN 
		if ((Select Opened from @l where CategoryId = @l1) = 1)
			INSERT INTO @lcategorytemp SELECT @l1
		else
			INSERT INTO @lcategorytemp SELECT id FROM Settings.GetChildCategoryByParent(@l1)

		SET @l1 = (SELECT Min(categoryid) FROM @l WHERE  categoryid > @l1)
	END

	DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED);
	INSERT INTO @lcategory SELECT Distinct CategoryId FROM @lcategorytemp

	SELECT DISTINCT prop.Name, prop.Sortorder
	FROM Catalog.Product p 
		INNER JOIN Catalog.ProductPropertyValue ON ProductPropertyValue.ProductId = p.ProductId
		INNER JOIN Catalog.PropertyValue propVal ON propVal.PropertyValueID = ProductPropertyValue.PropertyValueID
		INNER JOIN Catalog.Property prop ON prop.PropertyId = propVal.PropertyId
	WHERE 
		(
			EXISTS (
						SELECT 1 FROM [Catalog].[productcategories]
						WHERE [productcategories].[productid] = p.[productid] 
						AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
					) OR EXISTS (
						SELECT 1 
						FROM @lproductNoCat AS TEMP
						WHERE  TEMP.productid = p.[productid]
					) 
		)
		AND 
		(
			@exportAllProducts = 1 
			OR (
				SELECT Count(productid)
				FROM settings.exportfeedexcludedproducts
				WHERE exportfeedexcludedproducts.productid = p.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
			) = 0
		) AND (
			p.Enabled = 1 OR @exportNotAvailable = 1
		) AND (
			@exportNotAvailable = 1
			OR EXISTS (
				SELECT 1
				FROM [Catalog].[Offer] o
				Where o.[ProductId] = p.[productid] AND o.Price > 0 and o.Amount > 0
			)
		)
	ORDER BY prop.SortOrder, prop.Name
END

GO--

Delete From Vk.VkProduct 
Where not exists (Select 1 From Catalog.Product as p Where p.ProductId = VkProduct.ProductId)

GO--

ALTER TABLE Vk.VkProduct ADD CONSTRAINT
	FK_VkProduct_Product FOREIGN KEY
	(ProductId) REFERENCES Catalog.Product	(ProductId) 
     ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.Orders.Order.OrderPaid', 'оплачен'),
           (2,'Core.Orders.Order.OrderNotPaid', 'не оплачен')
GO--

Update [Settings].[Localization] 
Set ResourceValue = 'Шаблон письма ответа #FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#, #MANAGER_SIGN#, #LAST_ORDER_NUMBER#, в заказе доступны #ORDER_NUMBER#, #ORDER_SUM#, #ORDER_STATUS#, #ORDER_STATUS_COMMENT#, #ORDER_PAYMENT_NAME#, #ORDER_SHIPPING_NAME#, #ORDER_PICKPOINT_ADDRESS#, #ORDER_TRACK_NUMBER#, #ORDER_PAY_STATUS#, #ORDER_IS_PAID#, #ORDER_BILLING_SHORTLINK#, в лиде доступны #LEAD_TITLE#, #LEAD_SUM#, #LEAD_DEAL_STATUS#, #LEAD_SALES_FUNNEL#, #LEAD_SHIPPING_NAME#, #LEAD_PICKPOINT_ADDRESS#'
where ResourceKey = 'Admin.Js.AddEditMailAnswerTemplate.Template' and LanguageId = 1

Update [Settings].[Localization] 
Set ResourceValue = 'Response mail template #FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#, #MANAGER_SIGN#, #LAST_ORDER_NUMBER#, from order #ORDER_NUMBER#, #ORDER_SUM#, #ORDER_STATUS#, #ORDER_STATUS_COMMENT#, #ORDER_PAYMENT_NAME#, #ORDER_SHIPPING_NAME#, #ORDER_PICKPOINT_ADDRESS#, #ORDER_TRACK_NUMBER#, #ORDER_PAY_STATUS#, #ORDER_IS_PAID#, #ORDER_BILLING_SHORTLINK#, for lead #LEAD_TITLE#, #LEAD_SUM#, #LEAD_DEAL_STATUS#, #LEAD_SALES_FUNNEL#, #LEAD_SHIPPING_NAME#, #LEAD_PICKPOINT_ADDRESS# '
where ResourceKey = 'Admin.Js.AddEditMailAnswerTemplate.Template' and LanguageId = 2

GO--


Update [Settings].[MailTemplate]
Set [Body] =
    Replace(
        Replace(
            Replace(
                Replace(
                    Replace(
                        Replace(Replace([Body], '#STATUS_COMMENT#', '#ORDER_STATUS_COMMENT#'), 
                                '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
                        '#SHIPPING_NAME#', '#ORDER_SHIPPING_NAME#'), 
                    '#PICKPOINT_ADDRESS#', '#ORDER_PICKPOINT_ADDRESS#'),
                '#PAYMENT_NAME#', '#ORDER_PAYMENT_NAME#'),
            '#PAY_STATUS#', '#ORDER_PAY_STATUS#'),
        '#BILLING_SHORTLINK#', '#ORDER_BILLING_SHORTLINK#'),

[Subject] = 
    Replace(
        Replace(
            Replace(
                Replace(
                    Replace(
                        Replace(Replace([Subject], '#STATUS_COMMENT#', '#ORDER_STATUS_COMMENT#'), 
                                '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
                        '#SHIPPING_NAME#', '#ORDER_SHIPPING_NAME#'), 
                    '#PICKPOINT_ADDRESS#', '#ORDER_PICKPOINT_ADDRESS#'),
                '#PAYMENT_NAME#', '#ORDER_PAYMENT_NAME#'),
            '#PAY_STATUS#', '#ORDER_PAY_STATUS#'),
        '#BILLING_SHORTLINK#', '#ORDER_BILLING_SHORTLINK#')
GO--

Update [Settings].[SmsTemplate]
Set [Name] =
    Replace(
        Replace(
            Replace(
                Replace(
                    Replace(
                        Replace(Replace([Name], '#STATUS_COMMENT#', '#ORDER_STATUS_COMMENT#'), 
                                '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
                        '#SHIPPING_NAME#', '#ORDER_SHIPPING_NAME#'), 
                    '#PICKPOINT_ADDRESS#', '#ORDER_PICKPOINT_ADDRESS#'),
                '#PAYMENT_NAME#', '#ORDER_PAYMENT_NAME#'),
            '#PAY_STATUS#', '#ORDER_PAY_STATUS#'),
        '#BILLING_SHORTLINK#', '#ORDER_BILLING_SHORTLINK#'),

[Text] = 
    Replace(
        Replace(
            Replace(
                Replace(
                    Replace(
                        Replace(Replace([Text], '#STATUS_COMMENT#', '#ORDER_STATUS_COMMENT#'), 
                                '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
                        '#SHIPPING_NAME#', '#ORDER_SHIPPING_NAME#'), 
                    '#PICKPOINT_ADDRESS#', '#ORDER_PICKPOINT_ADDRESS#'),
                '#PAYMENT_NAME#', '#ORDER_PAYMENT_NAME#'),
            '#PAY_STATUS#', '#ORDER_PAY_STATUS#'),
        '#BILLING_SHORTLINK#', '#ORDER_BILLING_SHORTLINK#')
GO--

Update [Settings].[SmsTemplateOnOrderChanging]
Set [SmsText] =
    Replace(
        Replace(
            Replace(
                Replace(
                    Replace(
                        Replace(Replace([SmsText], '#STATUS_COMMENT#', '#ORDER_STATUS_COMMENT#'), 
                                '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
                        '#SHIPPING_NAME#', '#ORDER_SHIPPING_NAME#'), 
                    '#PICKPOINT_ADDRESS#', '#ORDER_PICKPOINT_ADDRESS#'),
                '#PAYMENT_NAME#', '#ORDER_PAYMENT_NAME#'),
            '#PAY_STATUS#', '#ORDER_PAY_STATUS#'),
        '#BILLING_SHORTLINK#', '#ORDER_BILLING_SHORTLINK#')

GO--

Update [Settings].[Settings]
Set [Value] =
    Replace(
        Replace(
            Replace(
                Replace(
                    Replace(
                        Replace(Replace([Value], '#STATUS_COMMENT#', '#ORDER_STATUS_COMMENT#'), 
                                '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
                        '#SHIPPING_NAME#', '#ORDER_SHIPPING_NAME#'), 
                    '#PICKPOINT_ADDRESS#', '#ORDER_PICKPOINT_ADDRESS#'),
                '#PAYMENT_NAME#', '#ORDER_PAYMENT_NAME#'),
            '#PAY_STATUS#', '#ORDER_PAY_STATUS#'),
        '#BILLING_SHORTLINK#', '#ORDER_BILLING_SHORTLINK#')
Where [Name] = 'SmsTextOnNewOrder'

GO--

Update [Settings].[MailTemplate]
Set [Body] =
    Replace(
        Replace(
            Replace(
                Replace(Replace([Body], '#DEAL_STATUS#', '#LEAD_DEAL_STATUS#'), 
                        '#SALES_FUNNEL#', '#LEAD_SALES_FUNNEL#'),
                '#SHIPPING_NAME#', '#LEAD_SHIPPING_NAME#'), 
            '#PICKPOINT_ADDRESS#', '#LEAD_PICKPOINT_ADDRESS#'),
        '#TITLE#', '#LEAD_TITLE#'),

[Subject] = 
    Replace(
        Replace(
            Replace(
                Replace(Replace([Subject], '#DEAL_STATUS#', '#LEAD_DEAL_STATUS#'), 
                        '#SALES_FUNNEL#', '#LEAD_SALES_FUNNEL#'),
                '#SHIPPING_NAME#', '#LEAD_SHIPPING_NAME#'), 
            '#PICKPOINT_ADDRESS#', '#LEAD_PICKPOINT_ADDRESS#'),
        '#TITLE#', '#LEAD_TITLE#')
GO--

Update [Settings].[SmsTemplate]
Set [Name] =
    Replace(
        Replace(
            Replace(
                Replace(Replace([Name], '#DEAL_STATUS#', '#LEAD_DEAL_STATUS#'), 
                        '#SALES_FUNNEL#', '#LEAD_SALES_FUNNEL#'),
                '#SHIPPING_NAME#', '#LEAD_SHIPPING_NAME#'), 
            '#PICKPOINT_ADDRESS#', '#LEAD_PICKPOINT_ADDRESS#'),
        '#TITLE#', '#LEAD_TITLE#'),

[Text] = 
    Replace(
        Replace(
            Replace(
                Replace(Replace([Text], '#DEAL_STATUS#', '#LEAD_DEAL_STATUS#'), 
                        '#SALES_FUNNEL#', '#LEAD_SALES_FUNNEL#'),
                '#SHIPPING_NAME#', '#LEAD_SHIPPING_NAME#'), 
            '#PICKPOINT_ADDRESS#', '#LEAD_PICKPOINT_ADDRESS#'),
        '#TITLE#', '#LEAD_TITLE#')
GO--

Update [Settings].[SmsTemplateOnOrderChanging]
Set [SmsText] =
    Replace(
        Replace(
            Replace(
                Replace(Replace([SmsText], '#DEAL_STATUS#', '#LEAD_DEAL_STATUS#'), 
                        '#SALES_FUNNEL#', '#LEAD_SALES_FUNNEL#'),
                '#SHIPPING_NAME#', '#LEAD_SHIPPING_NAME#'), 
            '#PICKPOINT_ADDRESS#', '#LEAD_PICKPOINT_ADDRESS#'),
        '#TITLE#', '#LEAD_TITLE#')

GO--

Update [Settings].[Settings]
Set [Value] =
    Replace(
        Replace(
            Replace(
                Replace(Replace([Value], '#DEAL_STATUS#', '#LEAD_DEAL_STATUS#'), 
                        '#SALES_FUNNEL#', '#LEAD_SALES_FUNNEL#'),
                '#SHIPPING_NAME#', '#LEAD_SHIPPING_NAME#'), 
            '#PICKPOINT_ADDRESS#', '#LEAD_PICKPOINT_ADDRESS#'),
        '#TITLE#', '#LEAD_TITLE#')
Where [Name] = 'SmsTextOnNewLead'

GO--

Update [Settings].[Localization]
Set [ResourceValue] = 'Шаблон смс ответа. Переменные #STORE_NAME#, #FIRST_NAME#, #LAST_NAME#, #FULL_NAME#, в заказе доступны #ORDER_NUMBER#, #ORDER_SUM#, #ORDER_STATUS#, #ORDER_STATUS_COMMENT#, #ORDER_PAYMENT_NAME#, #ORDER_SHIPPING_NAME#, #ORDER_PICKPOINT_ADDRESS#, #ORDER_TRACK_NUMBER#, #ORDER_PAY_STATUS#, #STORE_NAME#, в лиде доступны #LEAD_TITLE#, #LEAD_SUM#, #LEAD_SALES_FUNNEL#, #LEAD_DEAL_STATUS#, #LEAD_SHIPPING_NAME#, #LEAD_PICKPOINT_ADDRESS#'
Where [ResourceKey] = 'Admin.Js.AddEditSmsAnswerTemplate.Template' and [LanguageId] = 1

GO--

Update [Settings].[Localization]
Set [ResourceValue] = 'SMS template variables #STORE_NAME#, #FIRST_NAME#, #LAST_NAME#, #FULL_NAME#, for order #ORDER_NUMBER#, #ORDER_SUM#, #ORDER_STATUS#, #ORDER_STATUS_COMMENT#, #ORDER_PAYMENT_NAME#, #ORDER_SHIPPING_NAME#, #ORDER_PICKPOINT_ADDRESS#, #ORDER_TRACK_NUMBER#, #ORDER_PAY_STATUS#, #STORE_NAME#, for lead #LEAD_TITLE#, #LEAD_SUM#, #LEAD_SALES_FUNNEL#, #LEAD_DEAL_STATUS#, #LEAD_SHIPPING_NAME#, #LEAD_PICKPOINT_ADDRESS#'
Where [ResourceKey] = 'Admin.Js.AddEditSmsAnswerTemplate.Template' and [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Шаблон' WHERE [ResourceKey] = 'Admin.Home.Menu.Design' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Template' WHERE [ResourceKey] = 'Admin.Home.Menu.Design' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Mobile.Modules.Market.ModuleDescription', 'Описание'),
           (2,'Admin.Mobile.Modules.Market.ModuleDescription', 'Description')
GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ProductHistory.OfferChanged', 'Модификация изменилась'),
           (2,'Core.ProductHistory.OfferChanged', 'Offer has been changed')
GO--

CREATE TABLE [Customers].[ProjectStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[Color] [nvarchar](10),
	[Status] [int] NOT NULL,
	[StatusType] [int] NOT NULL,
CONSTRAINT [PK_ProjectStatus] PRIMARY KEY CLUSTERED
(
	[Id] Asc
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
)ON [PRIMARY]

GO--

CREATE TABLE [Customers].[TaskGroup_ProjectStatus](
	[TaskGroupId] [int] NOT NULL,
	[ProjectStatusId] [int] NOT NULL,
CONSTRAINT [PK_TaskGroup_ProjectStatus] PRIMARY KEY CLUSTERED
(
	[TaskGroupId] Asc, 
	[ProjectStatusId] Asc
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
CONSTRAINT [FK_TaskGroupProjectStatus_TaskGroup] FOREIGN KEY ([TaskGroupId])
REFERENCES [Customers].[TaskGroup] (Id)
ON DELETE CASCADE,
CONSTRAINT [FK_TaskGroupProjectStatus_ProjectStatus] FOREIGN KEY ([ProjectStatusId])
REFERENCES [Customers].[ProjectStatus] (Id)
ON DELETE CASCADE
)ON [PRIMARY]

GO--

ALTER TABLE [Customers].[Task] ADD StatusId [INT]

GO--
CREATE VIEW TaskGroupView AS (
	SELECT [Id], ROW_NUMBER() OVER (ORDER BY [Id]) AS RowNumber
	FROM [Customers].[TaskGroup]
)
/*генерация статусов для проектов*/
GO--

DECLARE @numberGroups INT, @i INT = 0
SET @numberGroups = (SELECT COUNT([Id]) from [Customers].[TaskGroup])
WHILE @i < @numberGroups
BEGIN
	DECLARE @groupId INT = (SELECT [Id] FROM TaskGroupView WHERE RowNumber = @i + 1)
	
	if ((SELECT COUNT([ProjectStatusId]) FROM [Customers].[TaskGroup_ProjectStatus] WHERE [TaskGroupId] = @groupId) > 0)
	BEGIN
		SELECT @i = @i + 1
		CONTINUE
	END

	INSERT INTO [Customers].[ProjectStatus] ([Name], [SortOrder], [Color], [Status], [StatusType])
	VALUES ('Новая', 10, '8bc34a', 0, 0)
	DECLARE @newProjectStatusId INT = (SELECT SCOPE_IDENTITY())
	INSERT INTO [Customers].[TaskGroup_ProjectStatus]([TaskGroupId], [ProjectStatusId])
		VALUES(@groupId, @newProjectStatusId)

	INSERT INTO [Customers].[ProjectStatus] ([Name], [SortOrder], [Color], [Status], [StatusType])
		VALUES ('В работе', 20, 'ffc73e', 0, 1)
	DECLARE @inProgressProjectStatusId INT = (SELECT SCOPE_IDENTITY())
	INSERT INTO [Customers].[TaskGroup_ProjectStatus]([TaskGroupId], [ProjectStatusId])
		VALUES(@groupId, @inProgressProjectStatusId)

	INSERT INTO [Customers].[ProjectStatus] ([Name], [SortOrder], [Color], [Status], [StatusType])
		VALUES ('Сделана', 30, '1ec5b8', 0, 2)
	DECLARE @completedProjectStatusId INT = (SELECT SCOPE_IDENTITY())
	INSERT INTO [Customers].[TaskGroup_ProjectStatus]([TaskGroupId], [ProjectStatusId])
		VALUES(@groupId, @completedProjectStatusId)

	INSERT INTO [Customers].[ProjectStatus] ([Name], [SortOrder], [Color], [Status], [StatusType])
		VALUES ('Задача завершена', 40, NULL, 1, 3)
	DECLARE @taskCompletedProjectStatusId INT = (SELECT SCOPE_IDENTITY())
	INSERT INTO [Customers].[TaskGroup_ProjectStatus]([TaskGroupId], [ProjectStatusId])
		VALUES(@groupId, @taskCompletedProjectStatusId)

	INSERT INTO [Customers].[ProjectStatus] ([Name], [SortOrder], [Color], [Status], [StatusType])
		VALUES ('Задача отклонена', 50, 'b0bec5', 2, 4)
	DECLARE @taskRejectedProjectStatusId INT = (SELECT SCOPE_IDENTITY())
	INSERT INTO [Customers].[TaskGroup_ProjectStatus]([TaskGroupId], [ProjectStatusId])
		VALUES(@groupId, @taskRejectedProjectStatusId)

	UPDATE [Customers].[Task]
	SET [StatusId] = @newProjectStatusId
	WHERE Status = 0 AND TaskGroupId = @groupId

	UPDATE [Customers].[Task]
	SET [StatusId] = @inProgressProjectStatusId
	WHERE Status = 1 AND TaskGroupId = @groupId

	UPDATE [Customers].[Task]
	SET [StatusId] = @completedProjectStatusId
	WHERE Status = 2 AND TaskGroupId = @groupId AND Accepted = 0

	UPDATE [Customers].[Task]
	SET [StatusId] = @taskCompletedProjectStatusId
	WHERE Status = 2 AND TaskGroupId = @groupId AND Accepted = 1

	SELECT @i = @i + 1
END

/*Заменяем столец Status на столбец StatusId*/
GO--

ALTER TABLE [Customers].[Task] ALTER COLUMN [StatusId] [INT] NOT NULL

GO--

ALTER TABLE [Customers].[Task] 
ADD CONSTRAINT [FK_Task_ProjectStatus] FOREIGN KEY ([StatusId])
REFERENCES [Customers].[ProjectStatus] ([Id])

GO--

ALTER TABLE [Customers].[Task]
DROP COLUMN [Status]

GO--

DROP VIEW TaskGroupView

/*Меняем Status на StatusId в триггерах и процедурах*/
GO--

ALTER TRIGGER [Customers].[TaskAdded]
	ON [Customers].[Task]
	AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @StatusId int = (SELECT [StatusId] FROM Inserted)
	DECLARE @TaskId int = (SELECT Id FROM Inserted)
	IF (SELECT IsDeferred FROM Inserted) = 0
	BEGIN
		UPDATE Customers.Task 
		SET SortOrder = (SELECT ISNULL(MAX(SortOrder), 0) + 10 FROM Customers.Task WHERE [StatusId] = @StatusId)
		WHERE Id = @TaskId
	END
END

GO--

ALTER TRIGGER [Customers].[TaskUpdated] 
	ON [Customers].[Task]
	WITH EXECUTE AS CALLER 
	FOR UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	
	if (SELECT COUNT(*) FROM Inserted) > 1
		return;

	DECLARE @NewSort int
    DECLARE @TaskId int = (SELECT Id FROM Inserted)
	DECLARE @StatusId int = (SELECT [StatusId] FROM Inserted)
    DECLARE @IsDeferred bit = (SELECT IsDeferred FROM Inserted)
    DECLARE @IsDeferredOld bit = (SELECT IsDeferred FROM Deleted)
    DECLARE @Accepted bit = (SELECT Accepted FROM Inserted)
    DECLARE @Priority int = (SELECT [Priority] FROM Inserted)
    DECLARE @PriorityOld int = (SELECT [Priority] FROM Deleted)

	-- only if task not accepted and not deffered
	IF @Accepted = 0 AND @IsDeferred = 0
	BEGIN
		-- new task from deferred tasks
		IF @IsDeferred <> @IsDeferredOld
		BEGIN
			SELECT @NewSort = (SELECT ISNULL(MAX(SortOrder), 0) + 10 FROM Customers.Task 
			WHERE [StatusId] = @StatusId AND ((@Priority <> 2 AND [Priority] <> 2) OR (@Priority = 2 AND [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0)
		END
		-- changed priority
		ELSE IF (@Priority = 2 OR @PriorityOld = 2) AND @Priority <> @PriorityOld
		BEGIN
			SELECT @NewSort = 
				(SELECT case when @Priority <> 2
					then ISNULL(MIN(SortOrder), 0) - 10			-- priority changed to high - set task first
					else ISNULL(MAX(SortOrder), 0) + 10 end 	-- priority changed to not high - set task last
				FROM Customers.Task WHERE [StatusId] = @StatusId AND ((@Priority <> 2 AND [Priority] <> 2) OR (@Priority = 2 AND [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0)
		END
	END

	IF @NewSort is not null
		UPDATE Customers.Task SET SortOrder = @NewSort WHERE Id = @TaskId
END

GO--

ALTER PROCEDURE [CRM].[ChangeTaskSorting]
	@Id int,
	@prevId int,
	@nextId int
AS
BEGIN
	IF @prevId IS NULL AND @nextId IS NULL
		RETURN;

	DECLARE @NewSort int

	DECLARE @priority int = (SELECT [Priority] FROM Customers.Task WHERE Id = @Id)
	DECLARE @statusId int = (SELECT [StatusId] FROM Customers.Task WHERE Id = @Id)

	DECLARE @prevSort int = (SELECT SortOrder FROM Customers.Task WHERE Id = @prevId)
	DECLARE @nextSort int = (SELECT SortOrder FROM Customers.Task WHERE Id = @nextId)

	if @prevSort IS NULL OR @nextSort IS NULL
	BEGIN
		SELECT @NewSort = 
			(SELECT CASE WHEN @prevSort IS NULL 
				THEN ISNULL(MIN(SortOrder), 0) - 10 
				ELSE ISNULL(MAX(SortOrder), 0) + 10 END 
			FROM Customers.Task 
			WHERE Id <> @Id AND [StatusId] = @statusId AND ((@priority <> 2 AND [Priority] <> 2) OR (@priority = 2 AND [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0)
	END
	ELSE
	BEGIN
		if @nextSort - @prevSort > 1
		BEGIN
			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
		ELSE
		BEGIN
			UPDATE Customers.Task SET SortOrder = TaskSort.Sort * 10
			FROM ( 
				SELECT Id, ROW_NUMBER() OVER (ORDER BY SortOrder) AS Sort FROM Customers.Task 
				WHERE [StatusId] = @statusId AND ((@priority <> 2 AND [Priority] <> 2) OR (@priority = 2 OR [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0
			) TaskSort INNER JOIN Customers.Task ON TaskSort.Id = Task.Id

			SELECT @prevSort = (SELECT SortOrder FROM Customers.Task WHERE Id = @prevId)
			SELECT @nextSort = (SELECT SortOrder FROM Customers.Task WHERE Id = @nextId)

			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
	END

	if @NewSort IS NOT NULL
		UPDATE Customers.Task SET SortOrder = @NewSort WHERE Id = @Id
END

/*Локализация*/
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.Accepted', 'Принята')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.Accepted', 'Accepted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.AddEditStatus.Edit', 'Редактирование списка статусов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.AddEditStatus.Edit', 'Edit status list')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.AddEditStatus.Add', 'Новый список статусов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.AddEditStatus.Add', 'New status list')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.AddEditStatus.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.AddEditStatus.Name', 'Name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.AddEditStatus.Statuses', 'Статусы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.AddEditStatus.Statuses', 'Statuses')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.AddEditStatus.SortOrder', 'Порядок сортировки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.AddEditStatus.SortOrder', 'Sort order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.AddEditStatus.Enabled', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.AddEditStatus.Enabled', 'Enabled')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.ChangesSaved', 'Изменения сохранены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.ChangesSaved', 'Changes saved')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.AreYouSureDelete', 'Вы уверены, что хотите удалить?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.AreYouSureDelete', 'Are you sure you want to delete?')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.Delete.', 'Удаление')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.Delete', 'Delete')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.Color', 'Цвет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.Color', 'Color')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditProjectStatus.Title', 'Статус')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditProjectStatus.Title', 'Project status')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditProjectStatus.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditProjectStatus.Name', 'Name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.Error', 'Ошибка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.Error', 'Error')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TaskStatuses.ErrorWhileEditing', 'Ошибка при сохранении')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TaskStatuses.ErrorWhileEditing', 'Error while editing')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AcceptTask.Completion', 'Завершение задачи №')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AcceptTask.Completion', 'Completion of task №')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AcceptTask.Choose', 'Выберите результат завершения задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AcceptTask.Choose', 'Choose the outcome of the task completing')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.EditTask.TaskStatus', 'Статус задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.EditTask.TaskStatus', 'Task status')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Tasks.Index.ShowAcceptedTasks', 'Показывать завершённые задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Tasks.Index.ShowAcceptedTasks', 'Show accepted tasks')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Models.Tasks.TasksPreFilterType.Canceled', 'Отклонённые')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Models.Tasks.TasksPreFilterType.Canceled', 'Canceled')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
	VALUES 
		(1,'Core.Crm.TaskStatusType.Open','Новая'),
		(2,'Core.Crm.TaskStatusType.Open','New'),
		(1,'Core.Crm.TaskStatusType.InProgress','В работе'),
		(2,'Core.Crm.TaskStatusType.InProgress','InProgress'),
		(1,'Core.Crm.TaskStatusType.Completed','Завершена'),
		(2,'Core.Crm.TaskStatusType.Completed','Completed'),
		(1,'Core.Crm.TaskStatusType.Accepted','Принята'),
		(2,'Core.Crm.TaskStatusType.Accepted','Accepted'),
		(1,'Core.Crm.TaskStatusType.Canceled','Отклонена'),
		(2,'Core.Crm.TaskStatusType.Canceled','Canceled'),
		(1,'Admin.Js.EditProjectStatus.StatusType','Статус'),
		(2,'Admin.Js.EditProjectStatus.StatusType','Status')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
	VALUES 
		(1,'Admin.Js.TaskGroups.SystemStatuses','Системные статусы'),
		(2,'Admin.Js.TaskGroups.SystemStatuses','System statuses'),
		(1,'Admin.Js.TaskStatuses.AddEditStatus.StatusType','Тип статуса'),
		(2,'Admin.Js.TaskStatuses.AddEditStatus.StatusType','Type of status')
		
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
	VALUES 
		(1,'Admin.Js.Tasks.ChangeTaskStatus.ChangeTheStatusOfTask','Изменение статуса задачи'),
		(2,'Admin.Js.Tasks.ChangeTaskStatus.ChangeTheStatusOfTask','Change the status of tsak'),
		(1,'Admin.Js.Tasks.ChangeTaskSatus.Edit','Изменить'),
		(2,'Admin.Js.Tasks.ChangeTaskSatus.Edit','Edit'),
		(1,'Admin.Js.Tasks.ChangeTaskStatus.Cancel','Отмена'),
		(2,'Admin.Js.Tasks.ChangeTaskStatus.Cancel','Cancel')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Files.Index.FileAlreadyExistInRoot', 'В корне сайта уже существует файл "{0}". Чтобы добавить новый, пожалуйста, удалите ранее загруженный файл.'),
           (2,'Admin.Files.Index.FileAlreadyExistInRoot', 'The file "{0}" already exists in the root of the site. To add a new one, please delete previously uploaded files.')
GO--

Update [Settings].[Localization] Set [ResourceValue] = N'Ваша корзина пуста' Where [ResourceKey] = 'Js.Cart.EmptyCart' and [LanguageId] = 1
Update [Settings].[Localization] Set [ResourceValue] = 'Your cart is empty' Where [ResourceKey] = 'Js.Cart.EmptyCart' and [LanguageId] = 2

GO--


CREATE NONCLUSTERED INDEX [ClientCode_CreatedDate]
ON [Customers].[ClientCode] ([CreatedDate])
INCLUDE ([UserId],[Code])

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
	VALUES 
		(1,'Admin.Modules.ModulesLayout.Module','Модуль'),
		(2,'Admin.Modules.ModulesLayout.Module','Module')
		
GO--   

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[Units]') AND type in (N'U'))
BEGIN
CREATE TABLE [Catalog].[Units](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[MeasureType] [tinyint] NULL,
	[SortOrder] [int] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

WITH DT AS (
	SELECT DISTINCT [Unit] FROM [Catalog].[Product] WHERE [Unit] != '' AND [Unit] IS NOT NULL)
INSERT INTO [Catalog].[Units] ([Name],[DisplayName],[MeasureType],[SortOrder],[DateAdded],[DateModified])
SELECT [Unit], [Unit], NULL, 0, GETDATE(), GETDATE() FROM DT

GO--

UPDATE [Catalog].[Product]
SET [Unit] = (SELECT [Units].[Id] FROM [Catalog].[Units] WHERE [Units].[DisplayName] = [Product].[Unit])
WHERE [Unit] != '' AND [Unit] IS NOT NULL

GO--

UPDATE [Catalog].[Product]
SET [Unit] = NULL
WHERE [Unit] = ''

GO--

ALTER TABLE [Catalog].[Product]
ALTER COLUMN [Unit] int NULL;

GO--

ALTER TABLE Catalog.Product ADD CONSTRAINT
	FK_Product_Units FOREIGN KEY
	(
	Unit
	) REFERENCES Catalog.Units
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 

GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled tinyint,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit int,
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,     
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),   
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max),
	@DoNotApplyOtherDiscounts bit
AS
BEGIN
    DECLARE @Id int,
			@ArtNoUpdateRequired bit

	IF @ArtNo=''
	BEGIN
		SET @ArtNo = CONVERT(nvarchar(100), NEWID())
		SET @ArtNoUpdateRequired = 1
	END

    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
        ,[BriefDescription]
        ,[Description]
        ,[Enabled]
        ,[DateAdded]
        ,[DateModified]
        ,[Recomended]
        ,[New]
        ,[BestSeller]
        ,[OnSale]
        ,[BrandID]
        ,[AllowPreOrder]
        ,[UrlPath]
        ,[Unit]
        ,[ShippingPrice]
        ,[MinAmount]
        ,[MaxAmount]
        ,[Multiplicity]
        ,[HasMultiOffer]
        ,CurrencyID
        ,ActiveView360
        ,ModifiedBy
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,CreatedBy
        ,Hidden
        ,ManualRatio
		,IsMarkingRequired
		,Comment
		,DoNotApplyOtherDiscounts
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
        ,@BriefDescription
        ,@Description
        ,@Enabled
        ,@DateModified
        ,@DateModified
        ,@Recomended
        ,@New
        ,@BestSeller
        ,@OnSale
        ,@BrandID
        ,@AllowPreOrder
        ,@UrlPath
        ,@Unit
        ,@ShippingPrice
        ,@MinAmount
        ,@MaxAmount
        ,@Multiplicity
        ,@HasMultiOffer
        ,@CurrencyID
        ,@ActiveView360
        ,@ModifiedBy
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
		,@IsMarkingRequired
        ,@Comment
		,@DoNotApplyOtherDiscounts
		);

    SET @ID = SCOPE_IDENTITY();
    IF @ArtNoUpdateRequired = 1
    BEGIN
		DECLARE @NewArtNo nvarchar(100) = CONVERT(nvarchar(100),@ID)

        IF EXISTS (SELECT * FROM [Catalog].[Product] WHERE [ArtNo] = @NewArtNo)
        BEGIN
            SET @NewArtNo = @NewArtNo + '_' + SUBSTRING(@ArtNo, 1, 5)
        END

        UPDATE [Catalog].[Product] SET [ArtNo] = @NewArtNo WHERE [ProductID] = @ID
    END
    SELECT @ID
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
    @ProductID int,
    @ArtNo nvarchar(100),
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled bit,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit int,
    @ShippingPrice money,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),
    @AccrueBonuses bit,
    @TaxId int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max),
	@DoNotApplyOtherDiscounts bit
AS
BEGIN
    UPDATE [Catalog].[Product]
    SET 
         [ArtNo] = @ArtNo
        ,[Name] = @Name
        ,[Ratio] = @Ratio
        ,[Discount] = @Discount
        ,[DiscountAmount] = @DiscountAmount
        ,[BriefDescription] = @BriefDescription
        ,[Description] = @Description
        ,[Enabled] = @Enabled
        ,[Recomended] = @Recomended
        ,[New] = @New
        ,[BestSeller] = @BestSeller
        ,[OnSale] = @OnSale
        ,[DateModified] = @DateModified
        ,[BrandID] = @BrandID
        ,[AllowPreOrder] = @AllowPreOrder
        ,[UrlPath] = @UrlPath
        ,[Unit] = @Unit
        ,[ShippingPrice] = @ShippingPrice
        ,[MinAmount] = @MinAmount
        ,[MaxAmount] = @MaxAmount
        ,[Multiplicity] = @Multiplicity
        ,[HasMultiOffer] = @HasMultiOffer
        ,[CurrencyID] = @CurrencyID
        ,[ActiveView360] = @ActiveView360
        ,[ModifiedBy] = @ModifiedBy
        ,[AccrueBonuses] = @AccrueBonuses
        ,[TaxId] = @TaxId
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
		,[IsMarkingRequired] = @IsMarkingRequired
		,[Comment] = @Comment
		,[DoNotApplyOtherDiscounts] = @DoNotApplyOtherDiscounts
    WHERE ProductID = @ProductID
END

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 0
WHERE [DisplayName] in ('шт', 'шт.', 'штука', '1 шт.', '1шт', '1 шт', 'шт.,', ' шт.', '(шт)', 'ед.', 'рулон', 'рул', 'рул.', 'пара', 'пар', 'пар.', 'Комплект', 'компл', 'компл.', 'комп.', 'комлпект', 'набор', 'Наб', 'наб.', 'упак', 'упаковка', 'упак.', 'уп.', 'уп', ' упак.')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 10
WHERE [DisplayName] in ('г', 'гр', 'гр.', 'грамм')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 11
WHERE [DisplayName] in ('кг', 'кг.', 'Килограмм', 'киллограмм')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 12
WHERE [DisplayName] in ('Тонна', 'т', 'тн')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 20
WHERE [DisplayName] in ('Сантиметр', 'см')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 21
WHERE [DisplayName] in ('Дециметр', 'дм')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 22
WHERE [DisplayName] in ('Метр', 'м', 'м.', 'пог. м', 'пог.м', 'пог.м.', 'пог. м.', 'погонный метр', 'пог.метр', 'пог. метр', '1 метр')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 30
WHERE [DisplayName] in ('Квадратный сантиметр', 'кв. см')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 31
WHERE [DisplayName] in ('Квадратный дециметр', 'кв. дм')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 32
WHERE [DisplayName] in ('Квадратный метр', 'кв. м', 'кв.м', 'кв.м.', 'м.кв', 'м2', N'м²')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 40
WHERE [DisplayName] in ('Миллилитр', 'мл')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 41
WHERE [DisplayName] in ('Литр', 'л', 'л.')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 42
WHERE [DisplayName] in ('Кубический метр', 'куб. м', 'м3', N'м³')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 50
WHERE [DisplayName] in ('Киловатт час')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 51
WHERE [DisplayName] in ('Гигакалория', 'Гкал')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 70
WHERE [DisplayName] in ('Сутки', 'день')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 71
WHERE [DisplayName] in ('Час')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 72
WHERE [DisplayName] in ('Минута', 'мин')

GO--

UPDATE [Catalog].[Units]
SET [MeasureType] = 73
WHERE [DisplayName] in ('Секунда', 'с')

GO--

ALTER TABLE [Order].[OrderItems] ADD
	[Unit] [nvarchar](50) NULL,
	[MeasureType] [tinyint] NULL

GO--

UPDATE [Order].[OrderItems]
SET [Unit] = (SELECT [Units].DisplayName FROM [Catalog].[Product] INNER JOIN [Catalog].[Units] ON [Product].[Unit] = [Units].[Id] WHERE [Product].[ProductId] = [OrderItems].[ProductID])
WHERE [ProductID] IS NOT NULL

GO--

UPDATE [Order].[OrderItems]
SET [MeasureType] = (SELECT [Units].MeasureType FROM [Catalog].[Product] INNER JOIN [Catalog].[Units] ON [Product].[Unit] = [Units].[Id] WHERE [Product].[ProductId] = [OrderItems].[ProductID])
WHERE [ProductID] IS NOT NULL

GO--

ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(255),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
	 @BarCode nvarchar(50),
	 @SupplyPrice float,  
	 @Weight float,  
	 @IsCouponApplied bit,  
	 @Color nvarchar(50),  
	 @Size nvarchar(50),  
	 @DecrementedAmount float,  
	 @PhotoID int,  
	 @IgnoreOrderDiscount bit,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @TaxName nvarchar(50),
	 @TaxType int,
	 @TaxRate float(53),
	 @TaxShowInPrice bit,
	 @Width float,
	 @Height float,
	 @Length float,
	 @PaymentMethodType int,
	 @PaymentSubjectType int,
	 @IsGift bit,
	 @BookingServiceId int,
	 @TypeItem nvarchar(50),
	 @IsMarkingRequired bit,
	 @IsCustomPrice bit,
	 @BasePrice float,
	 @DiscountPercent float,
	 @DiscountAmount float,
     @DoNotApplyOtherDiscounts bit,
	 @Unit nvarchar(50),
	 @MeasureType tinyint
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
	   ,[BarCode]  
	   ,[SupplyPrice]  
	   ,[Weight]  
	   ,[IsCouponApplied]  
	   ,[Color]  
	   ,[Size]  
	   ,[DecrementedAmount]  
	   ,[PhotoID]  
	   ,[IgnoreOrderDiscount]  
	   ,[AccrueBonuses] 
	   ,TaxId
	   ,TaxName
	   ,TaxType
	   ,TaxRate
	   ,TaxShowInPrice
	   ,Width
	   ,Height
	   ,[Length]
	   ,PaymentMethodType
	   ,PaymentSubjectType
	   ,IsGift
	   ,BookingServiceId
	   ,TypeItem
	   ,IsMarkingRequired
	   ,IsCustomPrice
	   ,BasePrice
	   ,DiscountPercent
	   ,DiscountAmount
       ,DoNotApplyOtherDiscounts
	   ,[Unit]
	   ,[MeasureType]
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
	   ,@BarCode
	   ,@SupplyPrice  
	   ,@Weight  
	   ,@IsCouponApplied  
	   ,@Color  
	   ,@Size  
	   ,@DecrementedAmount  
	   ,@PhotoID  
	   ,@IgnoreOrderDiscount  
	   ,@AccrueBonuses
	   ,@TaxId
	   ,@TaxName
	   ,@TaxType
	   ,@TaxRate
	   ,@TaxShowInPrice   
	   ,@Width
	   ,@Height
	   ,@Length
	   ,@PaymentMethodType
	   ,@PaymentSubjectType
	   ,@IsGift
	   ,@BookingServiceId
	   ,@TypeItem
	   ,@IsMarkingRequired
	   ,@IsCustomPrice
	   ,@BasePrice
	   ,@DiscountPercent
	   ,@DiscountAmount
       ,@DoNotApplyOtherDiscounts
       ,@Unit
       ,@MeasureType
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--


ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(255),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
	@BarCode nvarchar(50),
	@SupplyPrice float,  
	@Weight float,  
	@IsCouponApplied bit,  
	@Color nvarchar(50),  
	@Size nvarchar(50),  
	@DecrementedAmount float,  
	@PhotoID int,  
	@IgnoreOrderDiscount bit,  
	@AccrueBonuses bit,
	@TaxId int,
	@TaxName nvarchar(50),
	@TaxType int,
	@TaxRate float(53),
	@TaxShowInPrice bit,
	@Width float,
	@Height float,
	@Length float,
	@PaymentMethodType int,
	@PaymentSubjectType int,
	@IsGift bit,
	@IsCustomPrice bit,
	@BasePrice float,
	@DiscountPercent float,
	@DiscountAmount float,
    @DoNotApplyOtherDiscounts bit,
	@Unit nvarchar(50),
	@MeasureType tinyint
AS  
BEGIN  
 Update [Order].[OrderItems]  
 Set  
     [Name] = @Name  
	,[Price] = @Price  
	,[Amount] = @Amount  
	,[ArtNo] = @ArtNo  
	,[SupplyPrice] = @SupplyPrice  
	,[Weight] = @Weight  
	,[IsCouponApplied] = @IsCouponApplied  
	,[Color] = Color  
    ,[Size] = Size  
    ,[DecrementedAmount] = DecrementedAmount  
    ,[PhotoID] = @PhotoID  
    ,[IgnoreOrderDiscount] = @IgnoreOrderDiscount  
    ,[AccrueBonuses] = @AccrueBonuses
	,TaxId = @TaxId
	,TaxName = @TaxName
	,TaxType = @TaxType
	,TaxRate = @TaxRate
	,TaxShowInPrice = @TaxShowInPrice
	,Width = @Width
	,Height = @Height
	,[Length] = @Length
	,PaymentMethodType = @PaymentMethodType
	,PaymentSubjectType = @PaymentSubjectType
	,IsGift = @IsGift
	,[BarCode] = @BarCode
	,IsCustomPrice = @IsCustomPrice
	,BasePrice = @BasePrice
	,DiscountPercent = @DiscountPercent
	,DiscountAmount = @DiscountAmount
    ,DoNotApplyOtherDiscounts = @DoNotApplyOtherDiscounts
    ,Unit = @Unit
    ,MeasureType = @MeasureType
 Where OrderItemID = @OrderItemID  
END 

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Units.Index.Title', 'Единицы измерения'),
           (2,'Admin.Units.Index.Title', 'Units'),
           (1,'Admin.Units.Index.AddUnit', 'Добавить единицу'),
           (2,'Admin.Units.Index.AddUnit', 'Add a unit'),
           (1,'Admin.Js.Units.Name', 'Название'),
           (2,'Admin.Js.Units.Name', 'Name'),
           (1,'Admin.Js.Units.SortOrder', 'Поряд.'),
           (2,'Admin.Js.Units.SortOrder', 'Sort order'),
           (1,'Admin.Js.Units.UsedForProducts', 'Используется у товаров'),
           (2,'Admin.Js.Units.UsedForProducts', 'Used for products'),
           (1,'Admin.Js.Units.DeleteSelected', 'Удалить выделенные'),
           (2,'Admin.Js.Units.DeleteSelected', 'Delete selected'),
           (1,'Admin.Js.Units.AreYouSureDelete', 'Вы уверены, что хотите удалить?'),
           (2,'Admin.Js.Units.AreYouSureDelete', 'Are you sure you want to delete?'),
           (1,'Admin.Js.Units.Deleting', 'Удаление'),
           (2,'Admin.Js.Units.Deleting', 'Deleting'),
           (1,'Admin.Js.Units.DeletingIsImpossible', 'Удаление невозможно'),
           (2,'Admin.Js.Units.DeletingIsImpossible', 'Deleting is impossible'),
           (1,'Admin.Js.Units.UnitsCanNotBeDeleted', 'Единицы измерения, которые используются у товаров, удалять нельзя'),
           (2,'Admin.Js.Units.UnitsCanNotBeDeleted', 'Units that are used for goods can not be deleted'),
           (1,'Admin.Js.Units.AddEdit.Unit', 'Единица измерения'),
           (2,'Admin.Js.Units.AddEdit.Unit', 'Unit'),
           (1,'Admin.Js.Units.AddEdit.Name', 'Название'),
           (2,'Admin.Js.Units.AddEdit.Name', 'Name'),
           (1,'Admin.Js.Units.AddEdit.NameInClient', 'Название в клиентской части'),
           (2,'Admin.Js.Units.AddEdit.NameInClient', 'Name in the client part'),
           (1,'Admin.Js.Units.AddEdit.SortingOrder', 'Порядок сортировки'),
           (2,'Admin.Js.Units.AddEdit.SortingOrder', 'Sorting order'),
           (1,'Admin.Js.Units.AddEdit.MeasureType', 'Мера'),
           (2,'Admin.Js.Units.AddEdit.MeasureType', 'Measure'),
           (1,'Admin.Js.Units.AddEdit.MeasureTypeHelpName', 'Мера количества предмета расчета'),
           (2,'Admin.Js.Units.AddEdit.MeasureTypeHelpName', 'Measure'),
           (1,'Admin.Js.Units.AddEdit.MeasureTypeHelp', 'Данный параметр будет передаваться для печати чеков'),
           (2,'Admin.Js.Units.AddEdit.MeasureTypeHelp', 'This parameter will be passed for printing receipts'),
           (1,'Admin.Js.Units.AddEdit.Save', 'Сохранить'),
           (2,'Admin.Js.Units.AddEdit.Save', 'Save'),
           (1,'Admin.Js.Units.AddEdit.Cancel', 'Отмена'),
           (2,'Admin.Js.Units.AddEdit.Cancel', 'Cancel'),
           (1,'Admin.Js.Units.AddEdit.NotSelected', 'Не выбрана'),
           (2,'Admin.Js.Units.AddEdit.NotSelected', 'Not selected'),
           (1,'Admin.Units.NotSelected', 'Не выбрана'),
           (2,'Admin.Units.NotSelected', 'Not selected'),
           (1,'Admin.Js.Units.AddEdit.ChangesSaved', 'Изменения сохранены'),
           (2,'Admin.Js.Units.AddEdit.ChangesSaved', 'Changes saved'),
           (1,'Admin.Js.Units.AddEdit.Error', 'Ошибка'),
           (2,'Admin.Js.Units.AddEdit.Error', 'Error'),
           (1,'Admin.Js.Units.AddEdit.ErrorAddingEditing', 'Ошибка при создании/редактировании'),
           (2,'Admin.Js.Units.AddEdit.ErrorAddingEditing', 'Error creating / editing'),
           (1,'Core.Product.Units.NotSelected', 'Не выбрана'),
           (2,'Core.Product.Units.NotSelected', 'Not selected'),
           (1,'Admin.Js.Units.WithOutMeasureType', 'Без меры'),
           (2,'Admin.Js.Units.WithOutMeasureType', 'Without measure'),
           (1,'Admin.Js.Units.Yes', 'Да'),
           (2,'Admin.Js.Units.Yes', 'Yes'),
           (1,'Admin.Js.Units.No', 'Нет'),
           (2,'Admin.Js.Units.No', 'No')

GO--



INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Order.Index.Title"', 'Заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Order.Index.Title"', 'Order')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Home.Menu.HomePage', 'Главная')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Home.Menu.HomePage', 'Main')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Sites', 'Сайты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Sites', 'Sites')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Orders.AddEdit.OrderDraftTitle', 'Черновик заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Orders.AddEdit.OrderDraftTitle', 'Order draft')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Orders.AddEdit.OrderTitle', 'Заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Orders.AddEdit.OrderTitle', 'Order')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.Mobile.GridCustomSelectionAction.SelectedRows', 'Выбрано:'),
    (2,'Admin.Js.Mobile.GridCustomSelectionAction.SelectAnAction', 'Action')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.Mobile.GridCustomSelectionAction.SelectAnAction', 'Действие'),
    (2,'Admin.Js.Mobile.GridCustomSelectionAction.SelectedRows', 'Selected:')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.AdminMobile.AddRemovePropertyToProducts.EnterPropertyName', 'Название'),
    (2,'Admin.Js.AdminMobile.AddRemovePropertyToProducts.EnterPropertyName', 'Name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.AdminMobile.AddRemovePropertyToProducts.EnterPropertyValue', 'Значение'),
    (2,'Admin.Js.AdminMobile.AddRemovePropertyToProducts.EnterPropertyValue', 'Properties:')
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Catalog.Index.ProductSearch', 'Поиск по товарам'),
    (2,'Admin.Catalog.Index.ProductSearch', 'Product search')
	
GO--
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.Mobile.Partials.CategoryBlock.Edit', 'Редактировать'),
    (2,'Admin.Js.Mobile.Partials.CategoryBlock.Edit', 'Edit')
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.Mobile.Partials.CategoryBlock.Delete', 'Удалить'),
    (2,'Admin.Js.Mobile.Partials.CategoryBlock.Delete', 'Delete')
	
GO--
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Mobile.Category.Index.Delete', 'Удалить категорию'),
    (2,'Admin.Mobile.Category.Index.Delete', 'Delete category')
	
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Mobile.Category.Index.Edit', 'Редактировать категорию'),
    (2,'Admin.Mobile.Category.Index.Edit', 'Edit category')	
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Mobile.Order.DeliveryAddress', 'Адрес доставки'),
    (2,'Admin.Mobile.Order.DeliveryAddress', 'Delivery address')	
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Mobile.Home.Menu.Help','Помощь')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Mobile.Home.Menu.Help','Help')

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '11.0.0' WHERE [settingKey] = 'db_version'
