
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.BoxBerry.UseInsurance', 'Включить страховку 0.5% в стоимость доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.BoxBerry.UseInsurance', '
Include insurance 0.5% in the cost of delivery')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsAvito.ProductDescriptionType', 'Какое описание товара следует использовать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsAvito.ProductDescriptionType', 'What product description should be used')
GO--
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.Services.Mails.MailService.NeedActivateYourEmailError', 'Рассылка по базе невозможна, Вам необходимо подключить свой Email адрес к проекту и подтвердить его владение. <a href="{0}" target="_blank">Подключить свой Email адрес</a>')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.Services.Mails.MailService.NeedActivateYourEmailError', 'Mailing is impossible, you need to connect your Email address to the project and confirm its ownership. <a href="{0}" target="_blank">Connect your Email address</a>')


GO--

Update CMS.StaticBlock set [Key]='headerCenterBlock' where [Key] = 'headerСenterBlock' -- "С" на кириллице было
Update CMS.StaticBlock set [Key]='headerCenterBlockAlt' where [Key] = 'headerСenterBlockAlt'

GO--

ALTER TABLE [CMS].[LandingForm] ADD DontSendLeadId bit NULL
GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@onlyCount BIT
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	DECLARE @lproduct TABLE (productId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT [ProductID]
	FROM [Settings].[ExportFeedSelectedProducts]
	WHERE [ExportFeedId] = @exportFeedId;

	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @l
	SELECT t.CategoryId
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND HirecalEnabled = 1
		AND Enabled = 1

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		INSERT INTO @lcategory
		SELECT id
		FROM Settings.GetChildCategoryByParent(@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @lcategory
				)
			AND HirecalEnabled = 1
			AND Enabled = 1

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	IF @onlyCount = 1
	BEGIN
		SELECT COUNT(Distinct OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		--AND
		--(	Exists(select 1 from @lcategory where CategoryId = productCategories.CategoryID)
		--	or 
		--	Exists(select 1 from @lproduct where productId = productCategories.ProductID)
		--)
		--AND		
		--(
		--	SELECT TOP (1) [ProductCategories].[CategoryId]
		--	FROM [Catalog].[ProductCategories]
		--	INNER JOIN @lcategory lc ON lc.[CategoryId] = [ProductCategories].[CategoryId] and [ProductID] = p.[ProductID]
		--	Order By Main DESC, [ProductCategories].[CategoryId] 
		--) = productCategories.[CategoryId]
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	ELSE
	BEGIN	
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,[ProductCategories].[CategoryId] AS [ParentCategory]
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
			,[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,p.BarCode
			,p.Bid			
			,p.YandexSizeUnit
			,p.MinAmount
			,p.Multiplicity			
			,p.YandexName
			,p.YandexDeliveryDays		
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		
		--LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type ='Product' AND [Photo].[Main] = 1
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		--AND
		--(	Exists(select 1 from @lcategory where CategoryId = productCategories.CategoryID)
		--	or 
		--	Exists(select 1 from @lproduct where productId = productCategories.ProductID)
		--)
		AND		
		(
			SELECT TOP (1) [ProductCategories].[CategoryId]
			FROM [Catalog].[ProductCategories]
			INNER JOIN @lcategory lc ON lc.[CategoryId] = [ProductCategories].[CategoryId] and [ProductID] = p.[ProductID]
			Order By Main DESC, [ProductCategories].[CategoryId] 
		) = productCategories.[CategoryId]
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Order.Position','№')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Order.Position','№')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.MailSettings.MailServiceDoesNotSupportTransactions', 'Почтовый сервис mail.ru не поддерживает транзакционные письма')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.MailSettings.MailServiceDoesNotSupportTransactions', 'Mail service mail.ru does not support transactional letters')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.MailSettings.ConnectEmailAddress', 'Подключение email адреса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.MailSettings.ConnectEmailAddress', 'Connect email address')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Js.GridCustomComponent.AreYouSureCopy', 'Сделать копию?')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Js.GridCustomComponent.AreYouSureCopy', 'Copy?')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Js.GridCustomComponent.Copying', 'Копирование')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Js.GridCustomComponent.Copying', 'Copying')

GO--


alter table Catalog.Product add PaymentSubjectType int null, PaymentMethodType int null
GO--

update Catalog.Product set PaymentSubjectType = 1, PaymentMethodType = 1

GO--

alter table Catalog.Product alter column PaymentSubjectType int not null

alter table Catalog.Product alter column PaymentMethodType int not null


ALTER TABLE Catalog.Product  ADD CONSTRAINT DF_PaymentSubjectType DEFAULT 1 FOR PaymentSubjectType;

ALTER TABLE Catalog.Product  ADD CONSTRAINT DF_PaymentMethodType DEFAULT 1 FOR PaymentMethodType;

GO--


ALTER PROCEDURE [Catalog].[sp_AddProduct]      
	 @ArtNo nvarchar(100) = '',    
	 @Name nvarchar(255),       
	 @Ratio float,    
	 @Discount float,  
	 @DiscountAmount float,    
	 @Weight float,     
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
	 @Length float,  
	 @Width float,  
	 @Height float,  
	 @CurrencyID int,  
	 @ActiveView360 bit,  
	 @ManufacturerWarranty bit,  
	 @ModifiedBy nvarchar(50),  
	 @YandexTypePrefix nvarchar(500),  
	 @YandexModel nvarchar(500),  
	 @BarCode nvarchar(50),  
	 @Bid float,    
	 @AccrueBonuses bit,
     @Taxid int, 
	 @PaymentSubjectType int,
	 @PaymentMethodType int,
	 @YandexSizeUnit nvarchar(10),
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5),
	 @CreatedBy nvarchar(50)
	AS    
BEGIN    
Declare @Id int    
INSERT INTO [Catalog].[Product]    
             ([ArtNo]    
			 ,[Name]                       
			 ,[Ratio]    
			 ,[Discount]  
			 ,[DiscountAmount]    
			 ,[Weight]    
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
			 ,Length  
			 ,Width  
			 ,Height  
			 ,CurrencyID  
			 ,ActiveView360  
			 ,ManufacturerWarranty  
			 ,ModifiedBy  
			 ,YandexTypePrefix  
			 ,YandexModel  
			 ,BarCode  
			 ,Bid   
			 ,AccrueBonuses
             ,TaxId
			 ,PaymentSubjectType
			 ,PaymentMethodType
			 ,YandexSizeUnit	 
			 ,YandexName
			 ,YandexDeliveryDays
			 ,CreatedBy
          )    
     VALUES    
           (@ArtNo,    
			 @Name,         
			 @Ratio,    
			 @Discount,    
			 @DiscountAmount,  
			 @Weight,  
			 @BriefDescription,    
			 @Description,    
			 @Enabled,    
			 @DateModified,    
			 @DateModified,    
			 @Recomended,    
			 @New,    
			 @BestSeller,    
			 @OnSale,    
			 @BrandID,    
			 @AllowPreOrder,    
			 @UrlPath,    
			 @Unit,    
			 @ShippingPrice,    
			 @MinAmount,    
			 @MaxAmount,    
			 @Multiplicity,    
			 @HasMultiOffer,    
			 @SalesNote,    
			 @GoogleProductCategory,    
			 @YandexMarketCategory,  
			 @Gtin,    
			 @Adult,  
			 @Length,  
			 @Width,  
			 @Height,  
			 @CurrencyID,  
			 @ActiveView360,  
			 @ManufacturerWarranty,  
			 @ModifiedBy,  
			 @YandexTypePrefix,  
			 @YandexModel,  
			 @BarCode,  
			 @Bid,   
			 @AccrueBonuses,
             @TaxId,
			 @PaymentSubjectType,
			 @PaymentMethodType,
			 @YandexSizeUnit,
			 @YandexName,
			 @YandexDeliveryDays,
			 @CreatedBy
   );    
    
 SET @ID = SCOPE_IDENTITY();    
 if @ArtNo=''    
 begin    
  set @ArtNo = Convert(nvarchar(100), @ID)     
    
  WHILE (SELECT COUNT(*) FROM [Catalog].[Product] WHERE [ArtNo] = @ArtNo) > 0    
  begin    
    SET @ArtNo = @ArtNo + '_A'    
  end    
    
  UPDATE [Catalog].[Product] SET [ArtNo] = @ArtNo WHERE [ProductID] = @ID     
 end    
 Select @ID    
END

GO--


ALTER PROCEDURE [Catalog].[sp_UpdateProductById]    
	 @ProductID int,        
	 @ArtNo nvarchar(100),    
	 @Name nvarchar(255),    
	 @Ratio float,      
	 @Discount float,  
	 @DiscountAmount float,    
	 @Weight float,    
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
	 @Length float,  
	 @Width float,  
	 @Height float,  
	 @CurrencyID int,  
	 @ActiveView360 bit,  
	 @ManufacturerWarranty bit,  
	 @ModifiedBy nvarchar(50),  
	 @YandexTypePrefix nvarchar(500),  
	 @YandexModel nvarchar(500),  
	 @BarCode nvarchar(50),  
	 @Bid float,    
	 @AccrueBonuses bit,
	 @TaxId int,
	 @PaymentSubjectType int,
     @PaymentMethodType int,
	 @YandexSizeUnit nvarchar(10),
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5),
	 @CreatedBy nvarchar(50)

AS    
BEGIN    
	UPDATE [Catalog].[Product]    
	 SET [ArtNo] = @ArtNo    
	 ,[Name] = @Name    
	 ,[Ratio] = @Ratio    
	 ,[Discount] = @Discount  
	 ,[DiscountAmount] = @DiscountAmount    
	 ,[Weight] = @Weight  
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
	 ,[Length] = @Length  
	 ,[Width] = @Width  
	 ,[Height] = @Height  
	 ,[CurrencyID] = @CurrencyID  
	 ,[ActiveView360] = @ActiveView360  
	 ,[ManufacturerWarranty] = @ManufacturerWarranty  
	 ,[ModifiedBy] = @ModifiedBy  
	 ,[YandexTypePrefix] = @YandexTypePrefix  
	 ,[YandexModel] = @YandexModel  
	 ,[BarCode] = @BarCode  
	 ,[Bid] = @Bid   
	 ,[AccrueBonuses] = @AccrueBonuses
	 ,[TaxId] = @TaxId
	 ,[PaymentSubjectType] = @PaymentSubjectType
     ,[PaymentMethodType] = @PaymentMethodType
	 ,[YandexSizeUnit] = @YandexSizeUnit
	 ,[YandexName] = @YandexName
	 ,[YandexDeliveryDays] = @YandexDeliveryDays
	 ,[CreatedBy] = @CreatedBy
	WHERE ProductID = @ProductID      
END
GO--


alter table [order].orderitems add PaymentSubjectType int null, PaymentMethodType int null
GO--

update [order].orderitems set PaymentSubjectType = 1, PaymentMethodType = 1

GO--

alter table [order].orderitems alter column PaymentSubjectType int not null

alter table [order].orderitems alter column PaymentMethodType int not null


ALTER TABLE [order].orderitems  ADD CONSTRAINT DF_PaymentSubjectType DEFAULT 1 FOR PaymentSubjectType;

ALTER TABLE [order].orderitems   ADD CONSTRAINT DF_PaymentMethodType DEFAULT 1 FOR PaymentMethodType;


GO--


ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(255),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
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
	@PaymentSubjectType int
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
 Where OrderItemID = @OrderItemID  
END  


GO--

ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(255),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
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
	@PaymentSubjectType int
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
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
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
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
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--



Update Settings.ExportFeedSettings Set AdvancedSettings = Replace (AdvancedSettings,'"CsvSeparator":";"','"CsvSeparator":"SemicolonSeparated"') Where AdvancedSettings Like '%"CsvSeparator":";"%'
Update Settings.ExportFeedSettings Set AdvancedSettings = Replace (AdvancedSettings,'"CsvSeparator":","','"CsvSeparator":"CommaSeparated"') Where AdvancedSettings Like '%"CsvSeparator":","%'
Update Settings.ExportFeedSettings Set AdvancedSettings = Replace (AdvancedSettings,'"CsvSeparator":"\t"','"CsvSeparator":"TabSeparated"') Where AdvancedSettings Like '%"CsvSeparator":"\t"%'
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.DefaultAvitoCategory', 'Категория товара по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.DefaultAvitoCategory', 'Default avito product category')

GO--

ALTER TABLE CMS.LandingForm ADD
	OfferPrice nvarchar(MAX) NULL
GO--

ALTER TABLE Catalog.Tag DROP CONSTRAINT IX_Tag
GO--


ALTER TABLE Catalog.Tag ADD CONSTRAINT
	IX_Tag UNIQUE NONCLUSTERED 
	(
	UrlPath
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 80, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--

CREATE NONCLUSTERED INDEX [Date_Booking] ON [Booking].[Booking]
(
	[BeginDate] DESC,
	[EndDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO--
Alter Table Crm.SalesFunnel 
Add [Enable] bit NULL
GO--
Update Crm.SalesFunnel  Set [Enable] = 1
GO--
Alter Table Crm.SalesFunnel 
Alter Column [Enable] bit NOT NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SalesFunnels.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SalesFunnels.Name', 'Name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SalesFunnels.SortOrder', 'Сортировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SalesFunnels.SortOrder', 'Sort order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SalesFunnels.Activity', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SalesFunnels.Activity', 'Activity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SalesFunnels.TheyActive', 'Активные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SalesFunnels.TheyActive', 'They active')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SalesFunnels.Inactive', 'Неактивные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SalesFunnels.Inactive', 'Inactive')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCrm.EditSalesFunnel', 'Редактирование списка лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCrm.EditSalesFunnel', 'Edit leads list')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCrm.LeadsLists', 'Списки лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCrm.LeadsLists', 'Leads lists')


GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.IncreaseDeliveryTime', 'Прибавка к времени доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Grastin.IncreaseDeliveryTime', 'Increase in delivery time')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.IncreaseDeliveryTimeHelp', 'Указывается в днях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Grastin.IncreaseDeliveryTimeHelp', 'In days')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Crm.LeadsListsHeader', 'Списки лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Crm.LeadsListsHeader', 'Leads lists')

GO--

ALTER TABLE Settings.ExportFeedSelectedCategories ADD
	Opened bit NOT NULL CONSTRAINT DF_ExportFeedSelectedCategories_Opened DEFAULT 0
	
GO--
ALTER PROCEDURE [Settings].[sp_GetExportFeedCategories] @exportFeedId int
	,@onlyCount BIT
AS
BEGIN
	--template for result array
	DECLARE @result TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	-- templete for array of categories
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @lcategory
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1
		AND [ExportFeedId] = @exportFeedId

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @lcategory
			);

	WHILE @l1 IS NOT NULL
	BEGIN
	if ((Select Opened from @lcategory where CategoryId=@l1)=0)
	begin
		--add categories by step thats no in array 
		INSERT INTO @result
		SELECT id
		FROM Settings.GetChildCategoryByParent(@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @result
				)
			AND HirecalEnabled = 1
			AND Enabled = 1
		end	
		insert into @result
		select id from [Settings].[GetParentsCategoryByChild] (@l1) as dt
		where dt.id not in (SELECT CategoryId FROM @result)
		
		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @lcategory
				WHERE CategoryId > @l1
				);
	END;

	-- templete for array of categoiries by only selected product
	DECLARE @lproduct TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT DISTINCT CategoryID
	FROM [Catalog].[ProductCategories]
	INNER JOIN [Settings].[ExportFeedSelectedProducts] ON [ProductCategories].[ProductID] = [ExportFeedSelectedProducts].[ProductID]
		AND [ExportFeedId] = @exportFeedId
	WHERE [ExportFeedSelectedProducts].[ProductID] IN (
			SELECT Product.[ProductID]
			FROM CATALOG.Product
			INNER JOIN [Catalog].[Offer] ON [Offer].[ProductID] = [Product].[ProductID]
			WHERE Offer.Price > 0
				AND (
					Offer.Amount > 0
					OR Product.AllowPreorder = 1
					)
				AND CategoryEnabled = 1
				AND Enabled = 1
			)

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @lproduct
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		--add categories by step thats no in array 
		INSERT INTO @result
		SELECT id
		FROM Settings.[GetParentsCategoryByChild](@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @result
				)
			AND HirecalEnabled = 1
			AND Enabled = 1
			
		
		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @lproduct
				WHERE CategoryId > @l1
				);
	END;

	IF @onlyCount = 1
	BEGIN
		SELECT Count([CategoryID])
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
	ELSE
	BEGIN
		SELECT [CategoryID]
			,[ParentCategory]
			,[Name]
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
END


GO--
ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@onlyCount BIT
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	DECLARE @lproduct TABLE (productId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT [ProductID]
	FROM [Settings].[ExportFeedSelectedProducts]
	WHERE [ExportFeedId] = @exportFeedId;

	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND HirecalEnabled = 1
		AND Enabled = 1

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
	if ((Select Opened from @l where CategoryId=@l1)=0)
	begin
		INSERT INTO @lcategory
		SELECT id
		FROM Settings.GetChildCategoryByParent(@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @lcategory
				)
			AND HirecalEnabled = 1
			AND Enabled = 1
	 end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	IF @onlyCount = 1
	BEGIN
		SELECT COUNT(Distinct OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		--AND
		--(	Exists(select 1 from @lcategory where CategoryId = productCategories.CategoryID)
		--	or 
		--	Exists(select 1 from @lproduct where productId = productCategories.ProductID)
		--)
		--AND		
		--(
		--	SELECT TOP (1) [ProductCategories].[CategoryId]
		--	FROM [Catalog].[ProductCategories]
		--	INNER JOIN @lcategory lc ON lc.[CategoryId] = [ProductCategories].[CategoryId] and [ProductID] = p.[ProductID]
		--	Order By Main DESC, [ProductCategories].[CategoryId] 
		--) = productCategories.[CategoryId]
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	ELSE
	BEGIN	
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,[ProductCategories].[CategoryId] AS [ParentCategory]
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
			,[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,p.BarCode
			,p.Bid			
			,p.YandexSizeUnit
			,p.MinAmount
			,p.Multiplicity			
			,p.YandexName
			,p.YandexDeliveryDays		
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		
		--LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type ='Product' AND [Photo].[Main] = 1
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		--AND
		--(	Exists(select 1 from @lcategory where CategoryId = productCategories.CategoryID)
		--	or 
		--	Exists(select 1 from @lproduct where productId = productCategories.ProductID)
		--)
		AND		
		(
			SELECT TOP (1) [ProductCategories].[CategoryId]
			FROM [Catalog].[ProductCategories]
			INNER JOIN @lcategory lc ON lc.[CategoryId] = [ProductCategories].[CategoryId] and [ProductID] = p.[ProductID]
			Order By Main DESC, [ProductCategories].[CategoryId] 
		) = productCategories.[CategoryId]
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END

GO--
ALTER PROCEDURE [Settings].[sp_GetCsvProducts] @exportFeedId       INT, 
                                               @onlyCount          BIT, 
                                               @exportNoInCategory BIT, 
                                               @exportAllProducts  BIT, 
                                               @exportNotAvailable BIT 
AS 
  BEGIN 
      DECLARE @res TABLE (productid INT PRIMARY KEY CLUSTERED); 
      DECLARE @lproduct TABLE (productid INT PRIMARY KEY CLUSTERED); 
      DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED); 

      INSERT INTO @lproduct 
      SELECT [productid] 
      FROM   [Settings].[exportfeedselectedproducts] 
      WHERE  [exportfeedid] = @exportFeedId; 

      IF ( @exportNoInCategory = 1 ) 
        BEGIN 
            INSERT INTO @lproductNoCat 
				SELECT [productid] 
				FROM   [Catalog].product 
				WHERE  [productid] NOT IN (SELECT [productid] FROM   [Catalog].[productcategories]); 
        END 

      DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED); 
      DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit); 

      INSERT INTO @l 
      SELECT t.categoryid, t.Opened
      FROM   [Settings].[exportfeedselectedcategories] AS t 
             INNER JOIN catalog.category ON t.categoryid = category.categoryid 
      WHERE  [exportfeedid] = @exportFeedId 

      DECLARE @l1 INT 

      SET @l1 = (SELECT Min(categoryid) FROM @l); 

      WHILE @l1 IS NOT NULL 
        BEGIN 
		if ((Select Opened from @l where CategoryId=@l1)=0)
		begin
            INSERT INTO @lcategory 
				SELECT id 
				FROM   settings.Getchildcategorybyparent(@l1) AS dt 
				INNER JOIN catalog.category ON categoryid = id 
				WHERE  dt.id NOT IN (SELECT categoryid FROM @lcategory) 
		end;

            SET @l1 = (SELECT Min(categoryid) FROM   @l WHERE  categoryid > @l1); 
        END; 

      IF @onlyCount = 1 
        BEGIN 
            SELECT Count(productid) 
            FROM   [Catalog].[product] 
            WHERE  ( EXISTS (SELECT 1 
                             FROM   [Catalog].[productcategories] 
                             WHERE  [productcategories].[productid] = [product].[productid] 
                                    AND ( [productcategories].[productid] IN (SELECT productid FROM @lproduct) OR [productcategories].categoryid IN (SELECT categoryid FROM @lcategory) ) 
							 ) 
                      OR EXISTS (SELECT 1 
                                 FROM   @lproductNoCat AS TEMP 
                                 WHERE  TEMP.productid = [product].[productid]) 
                   ) 
                   AND ( @exportAllProducts = 1 
                          OR (SELECT Count(productid) 
                              FROM  settings.exportfeedexcludedproducts 
                              WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId) = 0 )
				   AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
				   AND ( @exportNotAvailable = 1
					      OR EXISTS (SELECT 1 
									 FROM [Catalog].[Offer] o 
									 Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0))
        END 
      ELSE 
        BEGIN 
            SELECT * 
            FROM   [Catalog].[product] 
                   LEFT JOIN [Catalog].[photo] ON [photo].[objid] = [product].[productid] AND type = 'Product' AND photo.[main] = 1 
            WHERE  ( EXISTS (SELECT 1 
                             FROM   [Catalog].[productcategories] 
                             WHERE  [productcategories].[productid] = [product].[productid] 
                                    AND ( [productcategories].[productid] IN (SELECT productid FROM @lproduct) OR [productcategories].categoryid IN (SELECT categoryid FROM @lcategory) ) 
							) 
                      OR EXISTS (SELECT 1 
                                 FROM   @lproductNoCat AS TEMP 
                                 WHERE  TEMP.productid = [product].[productid]) 
                   ) 
                   AND ( @exportAllProducts = 1 
                          OR (SELECT Count(productid) 
                              FROM   settings.exportfeedexcludedproducts 
                              WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId) = 0 ) 
				   AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
				   AND ( @exportNotAvailable = 1
					      OR EXISTS (SELECT 1 
									 FROM [Catalog].[Offer] o 
									 Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0))
        END 
  END 
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Lead.Kg', 'кг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Lead.Kg', 'kg')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Lead.Mm', 'мм')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Lead.Mm', 'mm')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Lead.Weight', 'Вес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Lead.Weight', 'Weight')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Lead.Dimensions', 'Габариты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Lead.Dimensions', 'Dimensions')
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCrm.NewLeadsList', 'Новый список')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCrm.NewLeadsList', 'New list')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.DefaultLeadList', 'Список лидов по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.DefaultLeadList', 'Default lead list')
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCrm.AddLeadsList', 'Новый список')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCrm.AddLeadsList', 'Add list')


-- start переименование воронок продаж в списки лидов 
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.AddLead.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список для лидов из Facebook' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.FacebookAuth.Funnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список для лидов из Instagram' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.InstagramAuth.Funnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.Leads.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Добавить список' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.SettingsCrm.AddAFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Новый список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.SettingsCrm.NewSalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Удалить список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.SettingsCrm.RemoveAFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.SettingsCrm.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список для лидов из Telegram' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.TelegramAuth.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список для лидов из ВКонтакте' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.VkAuth.Funnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Добавить список' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Leads.CrmNavMenu.AddFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Leads.Description.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Списки лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Module.MailChimp.Funnels'
Update [Settings].[Localization] Set [ResourceValue] = 'Списки лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Module.UniSender.Funnels'
Update [Settings].[Localization] Set [ResourceValue] = 'id списка лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsApi.Index.SalesFunnelID'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов по умолчанию' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.DefaultFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов по умолчанию' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsCrm.Index.DefaultFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Списки лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsCrm.Index.SalesFunnels'
Update [Settings].[Localization] Set [ResourceValue] = 'Список для лидов из новых входящих звонков' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsTelephony.CallsSalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов по умолчанию' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsTelephony.DefaultCallsSalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Выберите список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Admin.SettingsTelephony.Index.SmartCalls.SelectFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Наличие открытых лидов в списке' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Core.Crm.ELeadFieldType.OpenLeadSalesFunnels'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Core.Crm.ELeadFieldType.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Список лидов' 
	Where [LanguageId] = 1 and [ResourceKey] = 'Core.Crm.Lead.SalesFunnel'

Update [Settings].[Localization] Set [ResourceValue] = 'Leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.AddLead.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list for Facebook leads' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.FacebookAuth.Funnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list for Instagram leads' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.InstagramAuth.Funnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.Leads.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Add leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.SettingsCrm.AddAFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'New leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.SettingsCrm.NewSalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Delete leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.SettingsCrm.RemoveAFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.SettingsCrm.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list for Telegram leads' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.TelegramAuth.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list for VK leads' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.VkAuth.Funnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Add list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Leads.CrmNavMenu.AddFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Leads.Description.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads lists' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Module.MailChimp.Funnels'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads lists' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Module.UniSender.Funnels'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list id' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.SettingsApi.Index.SalesFunnelID'
Update [Settings].[Localization] Set [ResourceValue] = 'Default leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.DefaultFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Default leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.SettingsCrm.Index.DefaultFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads lists' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.SettingsCrm.Index.SalesFunnels'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list for new incoming calls leads' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.SettingsTelephony.CallsSalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Default leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.SettingsTelephony.DefaultCallsSalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Select leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Admin.SettingsTelephony.Index.SmartCalls.SelectFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'The presence of open leads in the list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Core.Crm.ELeadFieldType.OpenLeadSalesFunnels'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Core.Crm.ELeadFieldType.SalesFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Leads list' 
	Where [LanguageId] = 2 and [ResourceKey] = 'Core.Crm.Lead.SalesFunnel'
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.Errors.CantDeleteDefaultSalesFunnel', 'Невозможно удалить список лидов по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.Errors.CantDeleteLastSalesFunnel', 'Должен существовать хотя бы один список лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.Errors.CantDeleteNotEmptySalesFunnel', 'Вы не можете удалить список, пока в нем есть лиды. Перенесите лиды в другой список.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Leads.ChangeDealStatusToSelected', 'Изменить список лидов и этап сделки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ChangeLeadSalesFunnel.Header', 'Изменить список лидов и этап сделки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ChangeLeadSalesFunnel.SalesFunnel', 'Список лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.Errors.CantDeleteDefaultSalesFunnel', 'Can''t delete default leads list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.Errors.CantDeleteLastSalesFunnel', 'Can''t delete least leads list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.Errors.CantDeleteNotEmptySalesFunnel', 'Can''t delete not empty leads list. Move leads to another list.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Leads.ChangeDealStatusToSelected', 'Change leads list and deal status')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ChangeLeadSalesFunnel.Header', 'Change leads list and deal status')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ChangeLeadSalesFunnel.SalesFunnel', 'Leads list')
GO--
-- end переименование воронок продаж в списки лидов

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Leads.SetManagerToSelected', 'Назначить менеджера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ChangeLeadSalesFunnel.DealStatus', 'Этап сделки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Leads.SetManagerToSelected', 'Assign manager')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ChangeLeadSalesFunnel.DealStatus', 'Deal status')
GO--
ALTER TABLE [Order].PaymentMethod ADD TaxId int NULL
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Common.Tax', 'Налог')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Common.Tax', 'Tax')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Common.TaxPaymentMethod', 'Налог передаваемый в платежную систему')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Common.TaxPaymentMethod', 'Tax transferred to the payment system')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Common.DefaultUseTax', 'Использовать налог купленного товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Common.DefaultUseTax', 'Use tax purchased goods')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Lead.LaningShippingName', 'Доставка')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Lead.LaningShippingName', 'Delivery')
GO--

if (exists (select * from Catalog.Product where CurrencyId is null))
begin
	declare @currencyId int = (select top 1 Currency.CurrencyId from Catalog.Currency where CurrencyValue = 1)
	if (@currencyId is null)
		set @currencyId = (select top 1 Currency.CurrencyId from Catalog.Currency where CurrencyIso3 = (select top 1 Value from Settings.Settings where Name = 'DefaultCurrencyISO3'))



	if (@currencyId is not null)
		update Catalog.Product set CurrencyId = @currencyId where CurrencyId is null
end
-- falls if Catalog.Product contains records with null CurrencyId - no default currency
alter table Catalog.Product alter column CurrencyId int not null
GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '7.0.1' WHERE [settingKey] = 'db_version'